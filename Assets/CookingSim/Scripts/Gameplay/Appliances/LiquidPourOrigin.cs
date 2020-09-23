using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Gameplay
{
    public class LiquidPourOrigin : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The container from where the liquid will pour")]
        private Container container;
        [SerializeField]
        [Tooltip("The container from where the liquid will pour")]
        private float liquidVolumePerParticle = 20;

        private List<IngredientAmount> pouringIngredients;
        private float currentLiquidVolume;
        private float particlesRemovedFromCollision;

        private int particlesRemainingToSpawn;
        private bool isPouringActive;

        private ParticleSystem _particleSystem;
        private ParticleSystem.EmissionModule emission;
        private ParticleSystem.MinMaxCurve emissionPerTime;

        private List<ParticleCollisionEvent> collisionEvents;
        private ParticleSystem.Particle[] particles;
        private HashSet<uint> aliveParticles;

        public void StartPour(List<IngredientAmount> newIngredientsToPour)
        {
            IngredientAmount.AddToIngredientsList(pouringIngredients, newIngredientsToPour);
            var addedLiquidVolume = newIngredientsToPour.Select(a => a.Amount).Sum();
            currentLiquidVolume += addedLiquidVolume;
            particlesRemainingToSpawn = Mathf.CeilToInt((currentLiquidVolume / liquidVolumePerParticle) - aliveParticles.Count);

            // TODO Arthur: Change particles color based on ingredients
            emission.rateOverTime = emissionPerTime;
            isPouringActive = true;

            if (!_particleSystem.isPlaying)
            {
                _particleSystem.Play();
            }
        }

        private void EndPour()
        {
            emission.rateOverTime = 0;
            isPouringActive = false;
        }

        private List<IngredientAmount> GetIngredientsForVolume(float liquidVolume)
        {
            var newList = new List<IngredientAmount>();

            foreach (var pouringIngredient in pouringIngredients)
            {
                newList.Add(new IngredientAmount
                {
                    Ingredient = pouringIngredient.Ingredient,
                    Amount = pouringIngredient.Amount * liquidVolume / currentLiquidVolume
                });
            }

            return newList;
        }

        private void RemoveLiquidVolume(float liquidVolume)
        {
            if (liquidVolume <= 0)
            {
                return;
            }

            foreach (var pouringIngredient in pouringIngredients.ToList())
            {
                pouringIngredient.Amount -= pouringIngredient.Amount * liquidVolume / currentLiquidVolume;

                if (pouringIngredient.Amount <= 0.001f)
                {
                    pouringIngredients.Remove(pouringIngredient);
                }
            }

            currentLiquidVolume -= liquidVolume;
        }

        private int AddNewlyInstantiatedParticles(int numParticlesAlive)
        {
            int newlyAdded = 0;
            for (var index = 0; index < numParticlesAlive; index++)
            {
                var particle = particles[index];
                if (!aliveParticles.Contains(particle.randomSeed))
                {
                    newlyAdded++;
                    aliveParticles.Add(particle.randomSeed);
                }
            }

            return newlyAdded;
        }

        private int RemoveNewlyDeletedParticles(int numParticlesAlive)
        {
            int newlyRemoved = 0;
            foreach (var key in aliveParticles.ToList())
            {
                var particleIndex = GetParticleIndex(key);
                if (particleIndex == -1 || particleIndex >= numParticlesAlive)
                {
                    newlyRemoved++;
                    aliveParticles.Remove(key);
                }
            }

            if (aliveParticles.Count == 0 && !isPouringActive)
            {
                _particleSystem.Stop();
            }

            return newlyRemoved;
        }

        private int GetParticleIndex(uint particleSeed)
        {
            for (var index = 0; index < particles.Length; index++)
            {
                var particle = particles[index];
                if (particle.randomSeed == particleSeed)
                {
                    return index;
                }
            }

            return -1;
        }

        // TODO Arthur: Remove
        private IngredientConfig config1;
        private IngredientConfig config2;

        private void Update()
        {
            // TODO Arthur: Remove
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartPour(new List<IngredientAmount>
                {
                    new IngredientAmount { Ingredient = config1, Amount = 200},
                    new IngredientAmount { Ingredient = config2, Amount = 50},
                });
            }

            if (!_particleSystem.isPlaying)
            {
                return;
            }

            // TODO Arthur: Position particle system on lowest container edge

            int numParticlesAlive = _particleSystem.GetParticles(particles);

            var newlyAdded = AddNewlyInstantiatedParticles(numParticlesAlive);

            // Remove particles that ended their lifetime
            var newlyRemoved = RemoveNewlyDeletedParticles(numParticlesAlive);
            var volumeRemoved = Mathf.Min((
                    newlyRemoved - particlesRemovedFromCollision) * liquidVolumePerParticle,
                currentLiquidVolume);
            RemoveLiquidVolume(volumeRemoved);
            particlesRemovedFromCollision = Mathf.Max(particlesRemovedFromCollision - newlyRemoved, 0);

            particlesRemainingToSpawn -= newlyAdded;
            if (particlesRemainingToSpawn <= 0)
            {
                EndPour();
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            var otherContainer = other.GetComponent<Container>();
            if (otherContainer == null)
            {
                return;
            }

            int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);

            var volumeRemoved = Mathf.Min(numCollisionEvents * liquidVolumePerParticle, currentLiquidVolume);
            var ingredients = GetIngredientsForVolume(volumeRemoved);
            RemoveLiquidVolume(volumeRemoved);
            particlesRemovedFromCollision += numCollisionEvents;

            otherContainer.AddIngredients(ingredients);
        }

        private void Awake()
        {
            pouringIngredients = new List<IngredientAmount>();

            _particleSystem = GetComponent<ParticleSystem>();
            emission = _particleSystem.emission;
            emissionPerTime = emission.rateOverTime;

            EndPour();
            _particleSystem.Stop();

            collisionEvents = new List<ParticleCollisionEvent>();
            particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
            aliveParticles = new HashSet<uint>();

            // TODO Arthur: Remove
            config1 = ScriptableObject.CreateInstance<IngredientConfig>();
            config1.name = "TestIngredient 1";
            config2 = ScriptableObject.CreateInstance<IngredientConfig>();
            config2.name = "TestIngredient 2";
        }
    }
}
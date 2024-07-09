using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Server;
using MEC;
using Exiled.Events.Handlers;
using PlayerRoles;
using UnityEngine;
using Utils.NonAllocLINQ;
using Random = UnityEngine.Random;

namespace ZombieMod2
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    
    public class EventHandler
    {
        // Coroutines
        private CoroutineHandle coroutineWave;
        
        private Config Config => ZombieMod2.Instance.Config;
        private List<Player> Players => ZombieMod2.Instance.Players;
        private Dictionary<Player, float> Balances => ZombieMod2.Instance.Balances;
        private int WavesPassed
        {
            get => ZombieMod2.Instance.WavesPassed;
            set => ZombieMod2.Instance.WavesPassed = value;
        }
        private bool isRoundEnded = false;

        /// <summary>
        /// Calculates the value of the progression depending on the coefficient k,
        /// the initial state x, the complement of b and the number of Passed Waves
        /// Using formula kx+b
        /// </summary>
        /// <param name="k">coefficient k</param>
        /// <param name="x">the initial state x</param>
        /// <param name="b">the complement of b</param>
        /// <returns>Progression state at Passed Waves point</returns>
        private float ProgressionByWave(float k, float x, float b)
        {
            if (Mathf.Approximately(k, 1))
            {
                return k * x + b;
            }
            return (float)(Math.Pow(k, WavesPassed) * x + b * (Math.Pow(k, WavesPassed) - 1) / (k - 1));
        }
        
        /// <summary>
        /// Coroutine responsible for actions between waves
        /// Issuing rewards, setting pauses etc
        /// </summary>
        private IEnumerator<float> CoroutineWave()
        {
            // Pause before the first wave
            yield return Timing.WaitForSeconds(Config.StartWavePause);

            // Selecting players who will become Zombies in the first wave
            List<Player> players = ZombieMod2.Instance.Players;
            int numberPlayersToConvert = (int)(players.Count * (1 - Config.StartRatioSpawn));
            if (Mathf.Approximately(Config.StartRatioSpawn, 1.0f))
            {
                numberPlayersToConvert = 1;
            } else if (Mathf.Approximately(Config.StartRatioSpawn, 0.0f))
            {
                numberPlayersToConvert = players.Count - 1;
            }
            players = players.OrderBy(x => Random.Range(0, players.Count)).ToList();
            
            // Spawning chosen players as Zombie
            foreach (var player in players.Take(numberPlayersToConvert))
            {
                Config.StartZombiePreset.SpawnPreset(player);
            }

            while (!isRoundEnded)
            {
                // Waiting for the wave to end
                yield return Timing.WaitForSeconds(Config.WaveDuration);
                // Rewards all players for survival
                WavesPassed++;
                foreach (var balance in Balances)
                {
                    if (balance.Key.IsAlive) 
                        Balances[balance.Key] += Config.MoneyPerWave;
                }

                foreach (var player in players)
                {
                    if (player.Role != RoleTypeId.Scp0492) continue;
                    player.MaxHealth = Config.ZombieHpImprovementFactor * player.MaxHealth +
                                       Config.ZombieHpImprovementCount;
                    player.Health = player.MaxHealth;
                }
                // Pause between the waves
                yield return Timing.WaitForSeconds(Config.WavePause);
                
                // Spawning spectators at Wave
                List<Player> spectators = Player.List.Where(p => p.Role == RoleTypeId.Spectator).ToList();
                int numberMtfToSpawn = (int)(spectators.Count * Config.MiddleRatioSpawn);
                
                if (Mathf.Approximately(Config.MiddleRatioSpawn, 1.0f))
                {
                    numberMtfToSpawn = spectators.Count();
                } else if (Mathf.Approximately(Config.StartRatioSpawn, 0.0f))
                {
                    numberMtfToSpawn = 0;
                }
                int numberZombiesToSpawn = spectators.Count - numberMtfToSpawn;
                
                /*
                 * TODO:
                 * Add spawns to MtfSpawns and ZombieSpawns
                 */
                
                spectators = spectators.OrderBy(x => Random.Range(0, spectators.Count)).ToList();
                foreach (var player in spectators.Take(numberMtfToSpawn))
                {
                    Config.StartMtfPreset.SpawnPreset(player);
                }
                foreach (var player in spectators.Skip(numberMtfToSpawn).Take(numberZombiesToSpawn))
                {
                    Config.StartZombiePreset.SpawnPreset(player);
                }
            }
        }

        private void OnRoundStarted()
        {
            isRoundEnded = false;
            WavesPassed = 0;
            foreach (var player in Players)
            {
                Config.StartMtfPreset.SpawnPreset(player);
            }
            coroutineWave = Timing.RunCoroutine(CoroutineWave());
        }
        
        public EventHandler()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
        }

        ~EventHandler()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
        }

    }
}
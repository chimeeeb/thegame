using System;
using System.Collections.Generic;
using System.Threading;
using Game;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using Server;
using Agent = Player.Agent;
using NUnit.Framework;

namespace TCPTests
{
    public class Environment : IDisposable
    {
        public TcpServer Server { get; }
        public GameMaster GameMaster { get; }
        public List<Agent> Players { get; }

        public Environment(GameSettings settings = null)
        {
            if(settings == null)
            {
                settings = GameSettings.GetDefault();
            }
            GameMaster = new GameMaster(settings);
            Server = new TcpServer(settings.ServerIp, settings.ServerPort);
            Players = new List<Agent>();
            using (ManualResetEvent gmConnectedEventRaised = new ManualResetEvent(false))
            {
                Server.Listen();
                if(!GameMaster.ConnectClientToServer())
                {
                    throw new Exception("Could not connect GM TCP client to server.");
                }

                GameMaster.GmConnected += () => gmConnectedEventRaised.Set();
                GameMaster.ConnectToServer();

                if (!gmConnectedEventRaised.WaitOne())
                {
                    throw new TimeoutException("Connect GM operation has timed out.");
                }
            }
        }

        public void AddNewPlayer(Team team = Team.Blue, StrategyType strategy = StrategyType.Normal, bool wantBeALeader = false, bool isUsingStrategy = true)
        {
            var agent = new Agent(new AgentSettings
            {
                ServerPort = GameMaster.Settings.ServerPort,
                ServerIp = GameMaster.Settings.ServerIp,
                Strategy = strategy,
                Team = team,
                WantBeALeader = wantBeALeader
            });
            agent.IsUsingStrategy = isUsingStrategy;

            Players.Add(agent);
            if(!agent.ConnectToServer())
            {
                throw new Exception("Could not connect the agent to the server.");
            }
        }

        public void ConnectPlayers()
        {
            using (CountdownEvent cde = new CountdownEvent(Players.Count))
            {
                Players.ForEach(player =>
                {
                    player.ConnectResponse += (bool connected) =>
                    {
                        if (connected)
                            cde.Signal();
                        else
                            throw new Exception("Player was rejected from the game.");
                    };
                    player.ConnectToGame();
                });

                if (!cde.Wait(Players.Count * 1000))
                {
                    throw new TimeoutException($"Connect players operation has timed out, {cde.CurrentCount} players not connected yet.");
                }
            }
        }

        public void Dispose()
        {
            /*using (CountdownEvent cde = new CountdownEvent(Server.ListClients().Count + 1))
            {
                Server.ClientDisconnectedEvent += () => cde.Signal();

                Players.ForEach(player =>
                {
                    if (player.State != AgentState.Disconnected) player.Disconnect();
                    else cde.Signal();
                });

                if (GameMaster.State != GameMasterState.Disconnected) GameMaster.Disconnect();
                else cde.Signal();
                
                Server.CloseServer();

                if (!cde.Wait(5000))
                {
                    throw new TimeoutException("Disconnecting clients timed out.");
                }
            }*/
            Server.CloseServer();
            GameMaster.Disconnect();
            Players.ForEach(player => player.Disconnect());
        }

        public static Environment CreateEnvironment(int playersPerTeam, GameSettings settings, bool agentsIdle, bool gameRunning)
        {
            settings.NumberOfPlayers = 2 * playersPerTeam;
            var env = new Environment(settings);

            // Fill game with agents
            for (int i = 0; i < playersPerTeam; i++)
            {
                env.AddNewPlayer(team: Team.Blue, isUsingStrategy: !agentsIdle);
                env.AddNewPlayer(team: Team.Red, isUsingStrategy: !agentsIdle);
            }

            env.ConnectPlayers();

            if (gameRunning)
            {
                using (CountdownEvent cde = new CountdownEvent(env.Players.Count))
                {
                    env.Players.ForEach(player => player.GameStarted += () => cde.Signal());

                    // GM sends GameInfoMessage to all Agents
                    env.GameMaster.StartGame();

                    if (!cde.Wait(5000))
                    {
                        throw new TimeoutException("Game start has timed out.");
                    }
                }
            }
            return env;
        }
        
        public void CheckAgentDisconnected(Agent player)
        {
            Assert.AreEqual(AgentState.Disconnected, player.State, "Player is still connected.");
            Assert.IsNull(player.Map, "Player map invalid.");
            Assert.IsNull(player.ReceivedMessage, "Player received message invalid.");
            Assert.IsNull(player.Tile, "Player tile invalid.");
            Assert.IsNull(player.Strategy, "Player strategy invalid.");
        }

        public void CheckAllAgentsDisconnected()
        {
            Players.ForEach((Agent player) => CheckAgentDisconnected(player));
        }

        public void CheckGmDisconnected(GameSettings settings)
        {
            Assert.AreEqual(0, GameMaster.BlueTeamIds.Count, "GM Blue team ids count invalid.");
            Assert.AreEqual(0, GameMaster.RedTeamIds.Count, "GM Red team ids count invalid.");
            Assert.AreEqual(0, GameMaster.BlueTeamPoints, "GM Blue team points number invalid.");
            Assert.AreEqual(0, GameMaster.RedTeamPoints, "GM Red team points number invalid.");
            Assert.AreEqual(0, GameMaster.Agents.Count, "GM Agents count invalid.");
            Assert.AreEqual(0, GameMaster.Exchanges.Count, "GM exchanges count invalid.");
            Assert.AreEqual(GameMasterState.Disconnected, GameMaster.State);
            Assert.AreEqual(settings.MapHeight, GameMaster.Map.Height, "GM map height invalid.");
            Assert.AreEqual(settings.MapWidth, GameMaster.Map.Width, "GM map width invalid.");
            Assert.AreEqual(settings.GoalAreaHeight, GameMaster.Map.GoalAreaHeight, "GM map goal area height invalid.");
            Assert.AreEqual(0, GameMaster.NumberOfPieces, "GM number of pieces invalid.");
            Assert.AreEqual(0, GameMaster.NumberOfPlayers, "GM number of players invalid.");
            Assert.AreEqual(0, GameMaster.NumberOfTeamBluePlayers, "GM number of blue players invalid.");
            Assert.AreEqual(0, GameMaster.NumberOfTeamRedPlayers, "GM number of red players invalid.");
            Assert.AreEqual(Team.None, GameMaster.WinningTeam, "GM winning team invalid.");
            Assert.AreEqual(settings, GameMaster.Settings, "GM settings invalid.");
        }

        public void CheckServerDisconnected()
        {
            Assert.AreEqual(0, Server.AgentIdsToIpPort.Count, "Server agents ids count invalid.");
            Assert.IsFalse(Server.Debug, "Server debug invalid.");
            Assert.AreEqual(ServerState.GmNotConnected, Server.State, "Server state invalid.");
            Assert.IsEmpty(Server.GmIpPort, "Server GM IpPort string invalid.");
            Assert.IsNull(Server.GameEndedMessage, "Game ended message invalid.");
        }
    }
}
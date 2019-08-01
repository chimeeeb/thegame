using System;
using System.IO;
using System.Threading;
using System.Linq;
using GameLibrary.Configuration;
using GameLibrary.Enum;
using GameLibrary.Messages;
using NUnit.Framework;

namespace TCPTests
{
    [TestFixture]
    public class ConnectionProblemsTests
    {
        static ConnectionProblemsTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void GM_Should_MarkAgentAndContinue_When_AcceptedAgentDisconnected(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(true, false)] bool areAgentsIdle,
            [Values(true, false)] bool isGameRunning)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var environment =
                Environment.CreateEnvironment(playersPerTeam, settings, areAgentsIdle, isGameRunning))
            {
                using (var playerDisconnectedEventRaised = new ManualResetEvent(false))
                {
                    environment.GameMaster.PlayerDisconnected += (int id) => playerDisconnectedEventRaised.Set();

                    int currentPlayers = 2 * playersPerTeam;

                    foreach (var player in environment.Players)
                    {
                        Assert.AreEqual(isGameRunning ? AgentState.IsPlaying : AgentState.Connected, player.State);

                        player.Disconnect();

                        Assert.IsTrue(playerDisconnectedEventRaised.WaitOne(), "Disconnecting player operation has timed out.");
                        playerDisconnectedEventRaised.Reset();

                        environment.CheckAgentDisconnected(player);

                        currentPlayers--;
                        int currentBlue = player.Team == Team.Blue ?
                            currentPlayers / 2 : (currentPlayers + 1) / 2;
                        int currentRed = player.Team == Team.Blue ?
                            (currentPlayers + 1) / 2 : currentPlayers / 2;

                        Assert.AreEqual(currentPlayers, environment.GameMaster.NumberOfPlayers, "NumberOfPlayers invalid after an agent disconnected.");
                        Assert.AreEqual(currentPlayers, environment.GameMaster.Agents.Where(pair => !pair.Value.Disconnected).Count(), "Active players count invalid after an agent disconnected.");
                        Assert.AreEqual(isGameRunning ? GameMasterState.GameRunning : GameMasterState.Connected, environment.GameMaster.State, "The game is not running after an agent disconnected.");

                        Assert.AreEqual(currentRed, environment.GameMaster.NumberOfTeamRedPlayers, "Number of red players invalid after an agent disconnected.");
                        Assert.AreEqual(currentRed, environment.GameMaster.RedTeamIds.Count, "Count of red team ids invalid after an agent disconnected.");
                        Assert.AreEqual(currentBlue, environment.GameMaster.NumberOfTeamBluePlayers, "Number of blue players invalid after an agent disconnected.");
                        Assert.AreEqual(currentBlue, environment.GameMaster.BlueTeamIds.Count, "Count of blue team ids invalid after an agent disconnected.");
                    }
                }
            }
        }

        [Test]
        public void AcceptedAgents_Should_DisconnectAndEndPlaying_When_GMDisconnected(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(true, false)] bool areAgentsIdle,
            [Values(true, false)] bool isGameRunning)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var environment =
                Environment.CreateEnvironment(playersPerTeam, settings, areAgentsIdle, isGameRunning))
            {
                using (CountdownEvent cde = new CountdownEvent(environment.Players.Count))
                {
                    foreach (var player in environment.Players)
                    {
                        player.ServerDisconnected += () => cde.Signal();
                    }

                    environment.GameMaster.Disconnect();
                    Assert.IsTrue(cde.Wait(5000), "Disconnecting GM operation has timed out.");

                    environment.CheckAllAgentsDisconnected();
                }
            }
        }

        [Test]
        public void AgentsAndGm_Should_EndPlayingAndDisconnect_When_ServerDisconnected(
            [Values(1, 2, 3, 4)] int playersPerTeam,
            [Values(true, false)] bool isGameRunning
            )
        {
            GameSettings settings = GameSettings.GetDefault();
            var environment =
                Environment.CreateEnvironment(playersPerTeam, settings, false, isGameRunning);
            {
                using (CountdownEvent cde = new CountdownEvent(environment.Players.Count + 1))
                {
                    environment.GameMaster.ServerDisconnected += () => cde.Signal();
                    foreach (var player in environment.Players)
                    {
                        player.ServerDisconnected += () => cde.Signal();
                    }

                    environment.Server.CloseServer();
                    
                    for(int i = 0; i < 5; i++)
                    {
                        // Dummy messages so as GM and players will detect disconnected server
                        environment.GameMaster.CheckConnection();
                        foreach (var player in environment.Players)
                        {
                            player.CheckConnection();
                        }
                    }

                        Assert.IsTrue(cde.Wait(1000), "Disconnecting Server operation has timed out.");

                    environment.CheckServerDisconnected();
                    environment.CheckAllAgentsDisconnected();
                    environment.CheckGmDisconnected(settings);
                }
            }
            // Do NOT call Dispose() !!!
        }
    }
}
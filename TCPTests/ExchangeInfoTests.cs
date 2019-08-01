using Player;
using System;
using System.IO;
using NUnit.Framework;
using System.Threading;
using GameLibrary.Enum;
using GameLibrary.Messages;
using GameLibrary.Configuration;

namespace TCPTests
{
    [TestFixture]
    public class ExchangeInfoTests
    {
        static ExchangeInfoTests()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        [Test]
        public void Agent_Should_ReceiveAskingMessage_When_AnotherAgentSendsExchangeRequest([Values(2, 3, 4, 5)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var env = Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                Agent AskingAgent = null, RespondingAgent = null;
                foreach (var player in env.Players)
                {
                    Console.WriteLine(player.State);
                    if (player.Team == Team.Red)
                    {
                        if (AskingAgent == null)
                            AskingAgent = player;
                        else
                        {
                            RespondingAgent = player;
                            break;
                        }
                    }
                }
                using (var PlayerReceivedMsg = new ManualResetEvent(false))
                {
                    RespondingAgent.ReceivedMessageDuringGame += () => PlayerReceivedMsg.Set();
                    AskingAgent.InfosExchange(RespondingAgent.Id);
                    Assert.IsTrue(PlayerReceivedMsg.WaitOne(1000), "Player didnt receive response in less than a second after asking agent sends request");
                    Assert.IsInstanceOf(typeof(ExchangeInfosAskingMessage), RespondingAgent.ReceivedMessage);
                }
            }
        }

        [Test]
        public void Agent_Should_ReceiveDataResultMessage_When_AnotherAgentRespond([Values(2, 3, 4, 5)] int playersPerTeam)
        {
            GameSettings settings = GameSettings.GetDefault();
            using (var env = Environment.CreateEnvironment(playersPerTeam, settings, true, true))
            {
                Agent AskingAgent = null, RespondingAgent = null;
                foreach (var player in env.Players)
                {
                    Console.WriteLine(player.State);
                    if (player.Team == Team.Red)
                    {
                        if (AskingAgent == null)
                            AskingAgent = player;
                        else
                        {
                            RespondingAgent = player;
                            break;
                        }
                    }
                }
                using (var pp = new ManualResetEvent(false))
                {
                    using (var PlayerReceivedMsg = new ManualResetEvent(false))
                    {
                        RespondingAgent.ReceivedMessageDuringGame += () => PlayerReceivedMsg.Set();
                        AskingAgent.ReceivedMessageDuringGame += () => pp.Set();
                        AskingAgent.InfosExchange(RespondingAgent.Id);
                        Assert.IsTrue(PlayerReceivedMsg.WaitOne(1000), "Waiting for exchange request timed out.");
                        Assert.IsInstanceOf(typeof(ExchangeInfosAskingMessage), RespondingAgent.ReceivedMessage, "Invalid responding agent received message.");
                        Assert.IsTrue(pp.WaitOne(2000), "Waiting for exchange info data timed out.");
                        Assert.IsInstanceOf(typeof(ExchangeInfosDataResultMessage), AskingAgent.ReceivedMessage, "Invalid asking agent received message.");
                    }
                }
            }
        }
    }
}

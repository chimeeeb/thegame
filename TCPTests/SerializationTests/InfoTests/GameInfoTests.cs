using NUnit.Framework;
using GameLibrary.Messages;
using GameLibrary.Configuration;

namespace TCPTests.SerializationTests.InfoTests
{
    class GameInfoTests
    {
        #region SerializationTests

        [Test]
        public void Should_ReturnCorrectString_When_Given_GameInfoMessage()
        {
            GameSettings settings = new GameSettings
            {
                NumberOfPlayers = 8,
                NumberOfGoalsPerTeam = 6,
                PieceGenerationInterval = 10,
                ProbabilityOfBadPiece = 0.1,
                WaitMove = 1,
                WaitPickPiece = 2,
                WaitTestPiece = 3,
                WaitPutPiece = 4,
                WaitDestroyPiece = 5,
                WaitDiscovery = 6,
                WaitInfoExchange = 7,
                NumberOfPieces = 10,
                MapWidth = 12,
                MapHeight = 12,
                GoalAreaHeight = 4,
            };
            var message = new GameInfoMessage(settings)
            {
                AgentId = 9,
                TeamLeaderId = 8,
                AgentIdsFromTeam = new int[] { 6, 7, 8 },
                GameTime = 10,
                InitialXPosition = 0,
                InitialYPosition = 0,
                BaseTimePenalty = 0,
                RequestId = 0,
            };
            string expected = "{\"msgId\":32,\"agentId\":9,\"agentIdsFromTeam\":[6,7,8]," +
                "\"teamLeaderId\":8,\"timestamp\":10,\"boardSizeX\":12,\"boardSizeY\":4," +
                "\"goalAreaHeight\":4,\"initialXPosition\":0,\"initialYPosition\":0,\"numberOfGoals\":6," +
                "\"numberOfPlayers\":8,\"pieceSpawnDelay\":10,\"maxNumberOfPiecesOnBoard\":10," +
                "\"probabilityOfBadPiece\":0.1,\"baseTimePenalty\":0,\"tpm_move\":1,\"tpm_discoverPieces\":6," +
                "\"tpm_pickPiece\":2,\"tpm_checkPiece\":3,\"tpm_destroyPiece\":5,\"tpm_putPiece\":4,\"tpm_infoExchange\":7,\"requestId\":0}";
            TestsBase.SerializeAndCompareCertainMessage(message, expected);
        }

        #endregion

        #region DeserializationTests

        [Test]
        public void Should_Return_GameInfoMessage_When_Given_String()
        {
            string messageString = "{\"msgId\":32,\"agentId\":9,\"agentIdsFromTeam\":[6,7,8]," +
                "\"teamLeaderId\":8,\"timestamp\":10,\"boardSizeX\":12,\"boardSizeY\":4," +
                "\"goalAreaHeight\":4,\"initialXPosition\":0,\"initialYPosition\":0,\"numberOfGoals\":6," +
                "\"numberOfPlayers\":8,\"pieceSpawnDelay\":10,\"maxNumberOfPiecesOnBoard\":10," +
                "\"probabilityOfBadPiece\":0.1,\"baseTimePenalty\":0,\"tpm_move\":1,\"tpm_discoverPieces\":6," +
                "\"tpm_pickPiece\":2,\"tpm_checkPiece\":3,\"tpm_destroyPiece\":5,\"tpm_putPiece\":4,\"tpm_infoExchange\":7,\"requestId\":0}";
            GameSettings settings = new GameSettings
            {
                NumberOfPlayers = 8,
                NumberOfGoalsPerTeam = 6,
                PieceGenerationInterval = 10,
                ProbabilityOfBadPiece = 0.1,
                WaitMove = 1,
                WaitPickPiece = 2,
                WaitTestPiece = 3,
                WaitPutPiece = 4,
                WaitDestroyPiece = 5,
                WaitDiscovery = 6,
                WaitInfoExchange = 7,
                NumberOfPieces = 10,
                MapWidth = 12,
                MapHeight = 12,
                GoalAreaHeight = 4,
            };
            GameInfoMessage expected = new GameInfoMessage(settings)
            {
                AgentId = 9,
                TeamLeaderId = 8,
                AgentIdsFromTeam = new int[] { 6, 7, 8 },
                GameTime = 10,
                InitialXPosition = 0,
                InitialYPosition = 0,
                BaseTimePenalty = 0,
                RequestId = 0,
            };
            TestsBase.DeserializeAndCompareCertainMessage(messageString, expected);
        }

        #endregion
    }
}

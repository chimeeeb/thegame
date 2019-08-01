//using System;
//using NUnit.Framework;
//using ProjectGame;

//using Tests.TestClasses;

//namespace Tests.GameMasterTests
//{
//    [TestFixture]
//    public class CreatingGameMasterTests
//    {
//        [Test]
//        public void Should_CreateGame_When_PassingCorrectConfiguration()
//        {
//            Assert.That(() => new GameMasterMock(7, 12, 3, 5), Throws.Nothing);
//        }

//        [Test]
//        public void ShouldNot_CreateGame_When_PassingInvalidConfiguration()
//        {
//            Assert.Throws<ArgumentException>(() => new GameMasterMock(3, 12, 3, 5));
//            Assert.Throws<ArgumentException>(() => new GameMasterMock(7, 2, 3, 5));
//            Assert.Throws<ArgumentException>(() => new GameMasterMock(7, 12, 0, 5));
//            Assert.Throws<ArgumentException>(() => new GameMasterMock(7, 12, 11, 5));
//            Assert.Throws<ArgumentException>(() => new GameMasterMock(7, 12, 3, 0));
//        }

//        [Test]
//        public void Should_Game_HaveGenerated_CorrectNumberOfPieces()
//        {
//            int maxNumberOfPieces = 5;
//            GameMasterMock gm = new GameMasterMock(7, 12, 3, maxNumberOfPieces);
//            gm.PlayGeneratePieceOnly();

//            int numberOfPieces = 0;
//            for (int y = 0; y < gm.Map.Height; y++)
//                for (int x = 0; x < gm.Map.Width; x++)
//                    if (gm.Map.GetTile(x, y).Piece != null)
//                        numberOfPieces++;

//            Assert.AreEqual(gm.NumberOfPieces, numberOfPieces);
//            Assert.AreEqual(Parameters.NumberOfPieces, numberOfPieces);
//            Assert.AreEqual(maxNumberOfPieces, numberOfPieces);
//        }
//    }
//}
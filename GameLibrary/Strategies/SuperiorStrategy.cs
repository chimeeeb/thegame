using System;
using GameLibrary.Enum;
using GameLibrary.Interface;

namespace GameLibrary.Strategies
{
    public class SuperiorStrategy : NormalStrategy
    {
        public int HomeX { get; set; }
        public int HomeY { get; set; } 
        public int BaseGoalX { get; set; }
        public int BaseGoalY { get; set; }
        public bool GoingHome { get; set; }

        public SuperiorStrategy(Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId) : base(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId) { }

        public override Actions UseStrategy(ITile position, int distanceToPiece)
        {
            if (Piece == PieceStatus.Unidentified)
                return Actions.TestPiece;
            else if (Piece == PieceStatus.Sham)
                return Actions.DestroyPiece;
            else if (Piece == PieceStatus.None && distanceToPiece == 0)
                return Actions.PickPiece;
            else if (Piece == PieceStatus.None && GoingHome)
            {
                if (position.X == HomeX && position.Y == HomeY)
                {
                    GoingHome = false;
                    return Actions.Discovery;
                }
                Random rand = new Random();
                if (rand.Next(2) == 1 && DonePutPieceActions > 3 && !WaitingForExchangeAnswer)
                {
                    return Actions.InfoExchange;
                }
                if (CouldntMove < 5)
                    return GoToDestination(position, HomeX, HomeY);
            }
            else if (Piece == PieceStatus.None && !GoingHome && CouldntMove < 5)
                return Actions.Discovery;
            else if (Piece == PieceStatus.Real && position.X == GoingToX && position.Y == GoingToY)
            {
                GoingToX = GoingToY = -1;
                GoingHome = true;
                return Actions.PutPiece;
            }
            else if (Piece == PieceStatus.Real)
                return GoToDestination(position, GoingToX, GoingToY);
            else if (DonePutPieceActions > 3 && !WaitingForExchangeAnswer)
                return Actions.InfoExchange;
            return RandomAction(position);
        }
    }
}

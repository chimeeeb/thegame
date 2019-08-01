using GameLibrary.Enum;
using GameLibrary.Interface;

namespace GameLibrary.Strategies
{
    public class DiscovererStrategy : NormalStrategy
    {
        public DiscovererStrategy(Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId) : base(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId) { }

        public override Actions UseStrategy(ITile position, int distanceToPiece)
        {
            if (Piece == PieceStatus.Unidentified)
                return Actions.TestPiece;
            else if (Piece == PieceStatus.Sham)
                return Actions.DestroyPiece;
            else if (Piece == PieceStatus.None && distanceToPiece == 0)
                return Actions.PickPiece;
            else if (Piece == PieceStatus.None && CouldntMove < 5 && ((Team == Team.Red && position.Y < MapGoalAreaHeight) || (Team == Team.Blue && position.Y >= (MapHeight - MapGoalAreaHeight))))
            {
                if (Team == Team.Red)
                {
                    return Actions.MoveUp;
                }
                else if (Team == Team.Blue)
                {
                    return Actions.MoveDown;
                }
            }
            else if (Piece == PieceStatus.None && CouldntMove < 3)
            {
                return Actions.Discovery;
            }
            else if (position.X == GoingToX && position.Y == GoingToY)
            {
                GoingToX = GoingToY = -1;
                return Actions.PutPiece;
            }
            else if (Piece == PieceStatus.Real)
            {
                return GoToDestination(position, GoingToX, GoingToY);
            }
            return RandomAction(position);
        }
    }
}

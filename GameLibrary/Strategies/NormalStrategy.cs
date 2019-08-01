using GameLibrary.Enum;
using GameLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLibrary.Strategies
{
    public class NormalStrategy : StrategyBase
    {
        public int GoingToX, GoingToY;
        public List<int> ExchangeInfoTargets;
        public int DonePutPieceActions;

        public NormalStrategy(Team team, int mapHeight, int mapWidth, int mapGoalAreaHeight, int[] agentsIdFromTeam, int leaderId) : base(team, mapHeight, mapWidth, mapGoalAreaHeight, agentsIdFromTeam, leaderId)
        {
            GoingToX = GoingToY = -1;
            Random rand = new Random();
            ExchangeInfoTargets = new List<int>(agentsIdFromTeam);
            ExchangeInfoTargets = ExchangeInfoTargets.OrderBy(a => rand.Next()).ToList();
            DonePutPieceActions = 0;
        }

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
        
        public Actions GoToDestination(ITile position, int destinationX, int destinationY)
        {
            Random rand = new Random();
            if (destinationY > position.Y && CouldntMove < 5)
            {
                if (destinationX > position.X)
                {
                    switch(rand.Next(2))
                    {
                        case 0:
                            return Actions.MoveUp;
                        case 1:
                            return Actions.MoveRight;
                    }
                }
                if (destinationX < position.X)
                {
                    switch (rand.Next(2))
                    {
                        case 0:
                            return Actions.MoveUp;
                        case 1:
                            return Actions.MoveLeft;
                    }
                }
                return Actions.MoveUp;
            }
            if (destinationY < position.Y && CouldntMove < 5)
            {
                if (destinationX > position.X)
                {
                    switch (rand.Next(2))
                    {
                        case 0:
                            return Actions.MoveDown;
                        case 1:
                            return Actions.MoveRight;
                    }
                }
                if (destinationX < position.X)
                {
                    switch (rand.Next(2))
                    {
                        case 0:
                            return Actions.MoveDown;
                        case 1:
                            return Actions.MoveLeft;
                    }
                }
                return Actions.MoveDown;
            }
            if (destinationX > position.X && CouldntMove < 5)
                return Actions.MoveRight;
            if (destinationX < position.X && CouldntMove < 5)
                return Actions.MoveLeft;
            return RandomAction(position);
        }

        public Actions RandomAction(ITile position)
        {
            Random rand = new Random();
            switch (rand.Next(4))
            {
                case 0:
                    return Actions.MoveUp;
                case 1:
                    return Actions.MoveLeft;
                case 2:
                    return Actions.MoveDown;
                case 3:
                    return Actions.MoveRight;
                default:
                    return Actions.Discovery;
            }
        }

        public override int ExchangeInfoTarget()
        {
            return ExchangeInfoTargets[0];
        }
    }
}

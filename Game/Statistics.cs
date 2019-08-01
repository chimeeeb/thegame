using System;
using System.Text;
using GameLibrary.Enum;

namespace Game
{
    public class Statistics
    {
        public Team WinningTeam { get; set; }
        public int BlueTeamPoints { get; set; }
        public int RedTeamPoints { get; set; }
        public TimeSpan GameTime { get; set; }

        /// <summary>
        /// Generates game results information for logging.
        /// </summary>
        /// <returns>Strings containing game results.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine();
            result.AppendLine(new string('#', 30));
            result.AppendLine($"Team {WinningTeam:G} won");
            result.AppendLine($"Blue {BlueTeamPoints}:{RedTeamPoints} Red");
            result.AppendLine($"Time: {GameTime}");
            result.AppendLine(new string('#', 30));
            return result.ToString();
        }
    }
}
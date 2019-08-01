using System.Collections.Generic;
using GameLibrary.Interface;

namespace Game.GUI
{
    /// <summary>
    /// Hold information about added maps.
    /// </summary>
    public class MapContainer
    {
        /// <summary>
        /// Name of the container
        /// </summary>
        public string Name =>
            Maps != null && Maps.Count > 0 ?
                Maps.Count > 1 ?
                    "All" :
                    Maps[0] is Map ?
                        "Game master" :
                        $"Agent" :
                "No map";
        /// <summary>
        /// List of maps assigned to the container
        /// </summary>
        public List<IMap> Maps { get; set; }

        /// <summary>
        /// Creates an empty map container
        /// </summary>
        public MapContainer() => Maps = new List<IMap>();
    }
}
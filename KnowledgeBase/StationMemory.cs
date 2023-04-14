using System.Collections.Generic;
using UnityEngine;
using System;

namespace GOAP
{
    public class StationMemory
    {
        public Dictionary<StationSO, List<Station>> stationLocations =
            new Dictionary<StationSO, List<Station>>();
        public Dictionary<StatType, List<Station>> stationsByStat =
            new Dictionary<StatType, List<Station>>();

        public StationMemory()
        {
            stationLocations = new Dictionary<StationSO, List<Station>>();
            stationsByStat = new Dictionary<StatType, List<Station>>();
        }

        public void AddStation(Station station)
        {
            StationSO stationData = station.stationData;
            if (!stationLocations.ContainsKey(stationData))
            {
                stationLocations[stationData] = new List<Station>();
            }
            stationLocations[stationData].Add(station);

            foreach (StatEffect statEffect in stationData.statEffects)
            {
                StatType statType = statEffect.statType;
                if (!stationsByStat.ContainsKey(statType))
                {
                    stationsByStat[statType] = new List<Station>();
                }
                stationsByStat[statType].Add(station);
            }
        }

        public List<Station> ReturnGoalStations(Stat goal)
        {
            StatType goalStatType = goal.statType;
            List<Station> stations = new List<Station>();

            switch (goalStatType)
            {
                case StatType.Use_Station:
                    if (stationLocations.ContainsKey(goal.stationData))
                    {
                        stations.AddRange(stationLocations[goal.stationData]);
                    }
                    break;
                default:

                    if (stationsByStat.ContainsKey(goalStatType))
                    {
                        stations.AddRange(stationsByStat[goalStatType]);
                    }

                    break;
            }

            return stations;
        }
    }
}

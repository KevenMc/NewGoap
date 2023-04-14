using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class BlueprintRepertoire
    {
        public List<Blueprint> knownBlueprints = new List<Blueprint>();
        public Dictionary<StatType, List<Blueprint>> blueprintsByStatType =
            new Dictionary<StatType, List<Blueprint>>();

        public Dictionary<ItemSO, List<Blueprint>> blueprintsByCraftedItem =
            new Dictionary<ItemSO, List<Blueprint>>();

        public BlueprintRepertoire() { }

        public BlueprintRepertoire(List<Blueprint> blueprints)
        {
            knownBlueprints.AddRange(blueprints);
            IndexBlueprints();
        }

        public BlueprintRepertoire(List<BlueprintCatalogue> catalogues)
        {
            foreach (BlueprintCatalogue catalogue in catalogues)
            {
                knownBlueprints.AddRange(catalogue.blueprints);
            }
            IndexBlueprints();
        }

        public BlueprintRepertoire(List<BlueprintCatalogue> catalogues, List<Blueprint> blueprints)
        {
            knownBlueprints.AddRange(blueprints);

            foreach (BlueprintCatalogue catalogue in catalogues)
            {
                knownBlueprints.AddRange(catalogue.blueprints);
            }
            IndexBlueprints();
        }

        private void IndexBlueprints()
        {
            foreach (Blueprint blueprint in knownBlueprints)
            {
                if (!blueprintsByCraftedItem.ContainsKey(blueprint.craftedItem))
                {
                    blueprintsByCraftedItem[blueprint.craftedItem] = new List<Blueprint>();
                }
                blueprintsByCraftedItem[blueprint.craftedItem].Add(blueprint);

                foreach (StatEffect statEffect in blueprint.craftedItem.statEffects)
                {
                    StatType statType = statEffect.statType;
                    if (!blueprintsByStatType.ContainsKey(statType))
                    {
                        blueprintsByStatType[statType] = new List<Blueprint>();
                    }
                    blueprintsByStatType[statType].Add(blueprint);
                }
            }
        }

        public void AddBlueprintCatalogues(List<BlueprintCatalogue> catalogues)
        {
            foreach (BlueprintCatalogue catalogue in catalogues)
            {
                knownBlueprints.AddRange(catalogue.blueprints);
            }
            IndexBlueprints();
        }

        public List<Blueprint> GetBlueprintsWithGoalStatType(Stat goal)
        {
            List<Blueprint> matchingBlueprints = new List<Blueprint>();

            switch (goal.statType)
            {
                case StatType.Have_Item_Equipped:
                    if (blueprintsByCraftedItem.ContainsKey(goal.itemData))
                    {
                        matchingBlueprints.AddRange(blueprintsByCraftedItem[goal.itemData]);
                    }
                    break;

                default:
                    if (blueprintsByStatType.ContainsKey(goal.statType))
                    {
                        matchingBlueprints.AddRange(blueprintsByStatType[goal.statType]);
                    }

                    break;
            }
            return matchingBlueprints;
        }
    }
}

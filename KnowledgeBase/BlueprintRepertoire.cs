using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class BlueprintRepertoire
    {
        public List<Blueprint> knownBlueprints = new List<Blueprint>();

        public BlueprintRepertoire() { }

        public BlueprintRepertoire(List<BlueprintCatalogue> catalogues)
        {
            foreach (BlueprintCatalogue catalogue in catalogues)
            {
                knownBlueprints.AddRange(catalogue.blueprints);
            }
        }
    }
}

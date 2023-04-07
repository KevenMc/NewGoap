using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "New Blueprint Catalogue", menuName = "GOAP/Blueprint/Catalogue")]
    public class BlueprintCatalogue : ScriptableObject
    {
        public List<Blueprint> blueprints = new List<Blueprint>();
    }
}

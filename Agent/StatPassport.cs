using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "New Stat Passport", menuName = "GOAP/Stat Passport")]
    public class StatPassport : ScriptableObject
    {
        public string passportName;

        [SerializeField]
        public List<Stat> stats = new List<Stat>();
    }
}

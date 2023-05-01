using System;
using UnityEngine;

namespace Calco.LD53.Components
{
    [Serializable]
    public enum Faction
    {
        Player,
        Enemy,
        RagdollLimbTest
    }
    
    public class FactionComponent : MonoBehaviour
    {
        public Faction Faction;
    }
}
using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class PawnData : ScriptableObject
    {
        public Role role;
        public Sex sex;
        public float age;
        public bool alive;
        
        public int headId;
        public Color headColor;
        public int bodyId;
        public Color bodyColor;
    }
}


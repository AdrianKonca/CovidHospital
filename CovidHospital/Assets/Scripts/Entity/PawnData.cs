using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public PawnData()
        {
            role = Role.Patient;
        }

        public PawnData(int role)
        {
            this.role = (Role) role;
        }

        private void OnEnable()
        {
            sex = (Sex) Random.Range(0, 1);
            
            //Todo : add Normal distr to age generation
            age = Random.Range(18, 100);
            alive = true;
        }
    }
}
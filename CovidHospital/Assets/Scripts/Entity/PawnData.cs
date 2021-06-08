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

        public int HairId;
        public int HeadId;
        public int BodyId;
        
        public void Initialize(Role role)
        {
            HeadId = SpriteManager.GetRandomBodyPartId(BodyPart.Head);
            HairId = SpriteManager.GetRandomBodyPartId(BodyPart.Hair);
            BodyId = SpriteManager.GetRandomBodyPartId(BodyPart.Body);
            sex = (Sex)Random.Range(0, 1);
            this.role = role;
            switch (role)
            {
                case Role.Doctor:
                    age = Random.Range(30, 70);
                    break;
                case Role.Nurse:
                    age = Random.Range(24, 70);
                    break;
                case Role.Patient:
                    age = Random.Range(35, 70);
                    break;
            }
            alive = true;
        }
    }
}
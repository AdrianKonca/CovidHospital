using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity
{
    [Serializable]
    public class PawnData : ScriptableObject
    {
        static private Dictionary<Sex, string[]> NAMES = new Dictionary<Sex, string[]>
        {
            {Sex.Male, new string[]{"James", "Robert", "John", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Jerry", "Tyler", "Aaron", "Jose", "Adam", "Henry", "Nathan", "Douglas", "Zachary", "Peter", "Kyle", "Walter", "Ethan", "Jeremy", "Harold", "Keith", "Christian", "Roger", "Noah", "Gerald", "Carl", "Terry", "Sean", "Austin", "Arthur", "Lawrence", "Jesse", "Dylan", "Bryan", "Joe", "Jordan", "Billy", "Bruce", "Albert", "Willie", "Gabriel", "Logan", "Alan", "Juan", "Wayne", "Roy", "Ralph", "Randy", "Eugene", "Vincent", "Russell", "Elijah", "Louis", "Bobby", "Philip", "Johnny"} },
            {Sex.Female, new string[]{ "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Nancy", "Lisa", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle", "Dorothy", "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Kathleen", "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Emma", "Samantha", "Katherine", "Christine", "Debra", "Rachel", "Catherine", "Carolyn", "Janet", "Ruth", "Maria", "Heather", "Diane", "Virginia", "Julie", "Joyce", "Victoria", "Olivia", "Kelly", "Christina", "Lauren", "Joan", "Evelyn", "Judith", "Megan", "Cheryl", "Andrea", "Hannah", "Martha", "Jacqueline", "Frances", "Gloria", "Ann", "Teresa", "Kathryn", "Sara", "Janice", "Jean", "Alice", "Madison", "Doris", "Abigail", "Julia", "Judy", "Grace", "Denise", "Amber", "Marilyn", "Beverly", "Danielle", "Theresa", "Sophia", "Marie", "Diana", "Brittany", "Natalie", "Isabella", "Charlotte", "Rose", "Alexis", "Kayla" } }
        };

        public string Name;
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
            sex = (Sex)Random.Range(0, 2);
            
            this.role = role;
            Name = NAMES[sex][Random.Range(0, NAMES[sex].Length)];

            switch (role)
            {
                case Role.Doctor:
                    age = Random.Range(30, 70);
                    HeadId = SpriteManager.GetRandomBodyPartId(BodyPart.Head);
                    HairId = SpriteManager.GetRandomBodyPartId(BodyPart.Hair);
                    BodyId = SpriteManager.GetRandomBodyPartId(BodyPart.Body);
                    break;
                case Role.Nurse:
                    HeadId = 0;
                    HairId = 0;
                    BodyId = 0;
                    age = Random.Range(24, 70);
                    break;
                case Role.Patient:
                    HeadId = SpriteManager.GetRandomBodyPartId(BodyPart.Head);
                    HairId = SpriteManager.GetRandomBodyPartId(BodyPart.Hair);
                    BodyId = SpriteManager.GetRandomBodyPartId(BodyPart.Body);
                    age = Random.Range(35, 70);
                    break;
            }
            alive = true;
        }
    }
}
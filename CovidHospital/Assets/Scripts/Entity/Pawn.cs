using UnityEngine;

namespace Entity
{
    public class Pawn : MonoBehaviour
    {
        public PawnData PawnData;

        internal void Initialize(Role role)
        {
            PawnData = ScriptableObject.CreateInstance<PawnData>();
            PawnData.Initialize(role);
            transform.name = PawnData.Name;
            CreateBodyParts();
        }
        
        private GameObject GenerateBodyPart(Sprite sprite, BodyPart bodyPart)
        {
            GameObject obj = new GameObject();
            obj.name = bodyPart.ToString();
            obj.transform.parent = transform;

            obj.AddComponent<SpriteRenderer>();
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            obj.AddComponent<SpriteSorting>();
            obj.GetComponent<SpriteSorting>();
            //spriteRenderer.sortingOrder = 100;
            //if (bodyPart == BodyPart.Hair)
            //    spriteRenderer.sortingOrder = 200;
            return obj;
        }

        public void CreateBodyParts()
        {
            GenerateBodyPart(SpriteManager.GetPawnSprite(PawnData.HairId, BodyPart.Hair, Direction.Front), BodyPart.Hair);
            GenerateBodyPart(SpriteManager.GetPawnSprite(PawnData.HeadId, BodyPart.Head, Direction.Front), BodyPart.Head);
            GenerateBodyPart(SpriteManager.GetPawnSprite(PawnData.BodyId, BodyPart.Body, Direction.Front), BodyPart.Body);

            Turn(Direction.Front);
        }

        private void Turn(Direction direction)
        {
            transform.Find(BodyPart.Hair.ToString()).GetComponent<SpriteRenderer>().sprite = 
                SpriteManager.GetPawnSprite(PawnData.HairId, BodyPart.Hair, direction);
            transform.Find(BodyPart.Head.ToString()).GetComponent<SpriteRenderer>().sprite = 
                SpriteManager.GetPawnSprite(PawnData.HeadId, BodyPart.Head, direction);
            transform.Find(BodyPart.Body.ToString()).GetComponent<SpriteRenderer>().sprite = 
                SpriteManager.GetPawnSprite(PawnData.BodyId, BodyPart.Body, direction);
        }

    }
}
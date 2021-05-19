using UnityEngine;

namespace Entity
{
    public class Pawn : MonoBehaviour
    {
        public PawnData PawnData;
        public Sprite Hair;
        public Sprite Head;
        public Sprite Body;

        public Pawn(PawnData data)
        {
            Hair = SpriteManager.GetPawnSprite(data.HairId, BodyPart.Hair, Direction.Front);
            Head = SpriteManager.GetPawnSprite(data.HeadId, BodyPart.Head, Direction.Front);
            Body = SpriteManager.GetPawnSprite(data.BodyId, BodyPart.Body, Direction.Front); 
        }

    }
}
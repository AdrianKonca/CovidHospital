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
            Hair = AppearanceGenerator.spritesGlobal[(data.HairId, BodyPart.Hair, Direction.Front)];
            Head = AppearanceGenerator.spritesGlobal[(data.HeadId, BodyPart.Head, Direction.Front)];
            Body = AppearanceGenerator.spritesGlobal[(data.BodyId, BodyPart.Body, Direction.Front)];

        }

    }
}
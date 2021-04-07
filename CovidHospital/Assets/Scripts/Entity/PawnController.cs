using UnityEngine;

namespace Entity
{
    public abstract class Pawn : MonoBehaviour
    {
        private PawnData _pawnData;
        private Sprite _head;
        private Sprite _body;
    }

    public abstract class PatientPawn : Pawn
    {
        //etc.
    }

    public abstract class PersonnelPawn : Pawn
    {
        //etc.
    }
}
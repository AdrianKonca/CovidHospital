using UnityEngine;

namespace Entity
{
    //nonserializable representation of data, contains sprites and other nonserializable stuff constructed based on PawnData
    public abstract class Pawn : MonoBehaviour
    {
        private PawnData _pawnData;
        private Sprite _head;
        private Sprite _body;
    }

    public class PatientPawn : Pawn
    {
        private PatientData _patientData;
    }

    public class PersonnelPawn : Pawn
    {
        private PersonnelData _personnelData;
    }
}
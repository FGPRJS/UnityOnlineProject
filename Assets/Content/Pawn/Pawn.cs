using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Content.Pawn
{
    public class Pawn : MonoBehaviour
    {
        public long id;
        public string pawnName;
        public Vector3 velocity;

        protected Vector3 GravityValue;

        [SerializeField]
        public CharacterController controller;
    }
}
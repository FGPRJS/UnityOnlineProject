using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Content.Pawn
{
    public class Pawn : MonoBehaviour
    {
        public long id;
        public Vector3 velocity;

        public string pawnName;

        protected Vector3 GravityValue;

        [SerializeField]
        public CharacterController controller;
    }
}
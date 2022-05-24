﻿using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace Content.Pawn
{
    public class Pawn : MonoBehaviour
    {
        public long id;

        public PawnData pawnData;

        public string pawnName;

        public CinemachineVirtualCamera mainCamera;

        protected Vector3 GravityValue;

        [SerializeField]
        public CharacterController controller;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class LevelPiece
    {
        private float posX;
        private float posZ;
        private GameObject piece;

        public LevelPiece(float posX, float posZ, GameObject piece)
        {
            this.posX = posX;
            this.posZ = posZ;
            this.piece = piece;
        }

        public GameObject Piece
        {
            get
            {
                return piece;
            }
        }

        public float PosX
        {
            get
            {
                return posX;
            }
        }

        public float PosZ
        {
            get
            {
                return posZ;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cschmid.BoxPlatformer.Multiplayer
{
    [System.Serializable]
    public struct PlayerPackage
    {
        public Vector2 pos;
        public float zRot;

        public override string ToString()
        {
            return $"{pos.x},{pos.y},{zRot}";
        }
    }
}

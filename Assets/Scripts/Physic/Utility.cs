﻿using UnityEngine;

namespace Physic
{
    public static  class Utility
    {
        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
        
        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.x, 0f, v.y);
        }
    }
}
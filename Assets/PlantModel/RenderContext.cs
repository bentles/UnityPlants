using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PlantModel
{
    public struct RenderContext
    {
        //public int Index { get; set; }
        public Vector3 Translation { get; set; }
        public float Distance { get; set; }
        public Quaternion Rotation { get; set; }
    }
}

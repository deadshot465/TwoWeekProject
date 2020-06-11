using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Transactions;
using Microsoft.Xna.Framework;

namespace TwoWeekProject.Content
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Vector2 pos)
        {
            Transform = Matrix.CreateTranslation(-pos.X,  0, 0) * Matrix.CreateTranslation(50, 0, 0);
        }


    }
}

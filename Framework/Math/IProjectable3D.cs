using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foster.Framework
{
    public interface IProjectable3D
    {
        void Project(Vector3 axis, out float min, out float max);
    }
}

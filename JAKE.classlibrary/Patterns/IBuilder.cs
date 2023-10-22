using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IBuilder<T>
    {
        IBuilder<T> New();
        IBuilder<T> SetId(int id);
        IBuilder<T> SetColor(string color);
        IBuilder<T> SetCurrentPosition(double x, double y);
        T Build();
    }
}

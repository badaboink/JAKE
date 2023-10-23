using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns
{
    public interface IBuilderVisual<T>
    {
        IBuilderVisual<T> New();
        IBuilderVisual<T> SetColor(string color);
        IBuilderVisual<T> SetName(string color);
        IBuilderVisual<T> SetSize(int size);
        IBuilderVisual<T> SetPosition(double x, double y);
        T Build();
    }
}

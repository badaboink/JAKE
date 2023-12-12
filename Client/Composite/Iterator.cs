using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.client.Composite
{
    public interface IIterator
    {
        IMenuItem GetNext();
        bool HasMore();
    }
}

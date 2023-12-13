using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.ChainOfResponsibility
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);

        void Handle(object request);
    }

    public abstract class Handler : IHandler
    {
        private IHandler nextHandler;

        public IHandler SetNext(IHandler handler)
        {
            this.nextHandler = handler;
            return handler;
        }

        public virtual void Handle(object request)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}

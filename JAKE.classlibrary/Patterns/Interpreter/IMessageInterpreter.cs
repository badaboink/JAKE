﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAKE.classlibrary.Patterns.Interpreter
{
    public interface IMessageInterpreter
    {
        string interpret(string message);
    }
}

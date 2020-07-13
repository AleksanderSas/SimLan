using System;

namespace SimLan.Evaluator
{
    class ReturnException : Exception
    {
        public readonly BaseComputable Value;

        public ReturnException(BaseComputable value)
        {
            Value = value;
        }
    }
}

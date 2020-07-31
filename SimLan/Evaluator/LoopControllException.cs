using System;

namespace SimLan.Evaluator
{
    class LoopControllException : Exception
    {
        public bool IsBreak { get; }
        public LoopControllException(bool isBreak)
        {
            IsBreak = isBreak;
        }
    }
}

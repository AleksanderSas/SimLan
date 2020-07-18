using System.Collections.Generic;

namespace SimLan.Evaluator
{
    class BaseComputable
    {
        public virtual BaseComputable Clone()
        {
            return this;
        }

        public virtual BaseComputable ExecuteOperation(string opr, BaseComputable rigth)
        {
            throw new System.Exception("Variable doesn't support operator " + opr);
        }

        public virtual BaseComputable CallFunction(IList<BaseComputable> args)
        {
            throw new System.Exception("Variable is not a function");
        }

        public virtual BaseComputable CallArray(int idx)
        {
            throw new System.Exception("Variable is not an array");
        }

        public virtual void Assign(int idx, BaseComputable value)
        {
            throw new System.Exception("Variable is not an array");
        }

        public virtual int GetValue()
        {
            throw new System.Exception("Variable has no rigth value");
        }

        public virtual BaseComputable Resolve()
        {
            throw new System.Exception("Variable is undefined");
        }
    }
}

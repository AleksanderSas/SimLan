using System.Collections.Generic;

namespace SimLan.Evaluator
{
    class BaseComputable
    {
        public virtual BaseComputable CallFunction(IList<BaseComputable> args)
        {
            throw new System.Exception("Variable is not a function");
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

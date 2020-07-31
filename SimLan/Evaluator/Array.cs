using System.Collections.Generic;

namespace SimLan.Evaluator
{
    internal class Array : BaseComputable
    {
        private BaseComputable[] _data;

        public Array(int size)
        {
            _data = new BaseComputable[size];
        }

        public override ref BaseComputable CallArray(int idx)
        {
            return ref _data[idx];
        }

        public override BaseComputable CallFunction(IList<BaseComputable> args)
        {
            return new SimpleValue(_data.Length);
        }
    }
}

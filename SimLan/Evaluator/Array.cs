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

        public override BaseComputable CallArray(int idx)
        {
            return _data[idx];
        }

        public override void Assign(int idx, BaseComputable value)
        {
            _data[idx] = value;
        }

        public override BaseComputable CallFunction(IList<BaseComputable> args)
        {
            return new SimpleValue(_data.Length);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace SimLan.Evaluator
{
    class Str : BaseComputable
    {
        private BaseComputable[] _value;

        public Str(string value)
        {
            _value = value.Select(x => (BaseComputable)new SimpleValue(x)).ToArray();
        }

        private Str(SimpleValue[] value)
        {
            _value = value;
        }

        public override BaseComputable Clone()
        {
            return new Str((SimpleValue[])_value.Clone());
        }

        public override BaseComputable ExecuteOperation(string opr, BaseComputable rigth)
        {
            if (rigth is Str s)
            {
                switch (opr)
                {
                    case "+":
                        var tmp = new SimpleValue[_value.Length + s._value.Length];
                        _value.CopyTo(tmp, 0);
                        s._value.CopyTo(tmp, _value.Length);
                        return new Str(tmp);
                    case "==":
                        return new SimpleValue(Enumerable.SequenceEqual(_value, s._value) ? 1 : 0);
                    case "<>":
                        return new SimpleValue(Enumerable.SequenceEqual(_value, s._value) ? 0 : 1);
                    default: throw new Exception($"operator {opr} is unknown");
                }
            }
            return base.ExecuteOperation(opr, rigth);
        }

        public override BaseComputable CallFunction(IList<BaseComputable> args)
        {
            return new SimpleValue(_value.Length);
        }

        public override ref BaseComputable CallArray(int idx)
        {
            return ref _value[idx];
        }
    }
}

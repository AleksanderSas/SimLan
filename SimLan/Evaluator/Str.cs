using System;
using System.Collections.Generic;
using System.Linq;

namespace SimLan.Evaluator
{
    class Str : BaseComputable
    {
        private char[] _value;

        public Str(string value)
        {
            _value = value.ToCharArray();
        }

        private Str(char[] value)
        {
            _value = value; ;
        }

        public override BaseComputable Clone()
        {
            return new Str((char[])_value.Clone());
        }

        public override BaseComputable ExecuteOperation(string opr, BaseComputable rigth)
        {
            if (rigth is Str s)
            {
                switch (opr)
                {
                    case "+":
                        var tmp = new char[_value.Length + s._value.Length];
                        _value.CopyTo(tmp, 0);
                        s._value.CopyTo(tmp, _value.Length);
                        _value = tmp;
                        break;
                    case "==":
                        return new SimpleValue(Enumerable.SequenceEqual(_value, s._value) ? 1 : 0);
                    case "<>":
                        return new SimpleValue(Enumerable.SequenceEqual(_value, s._value) ? 0 : 1);
                    default: throw new Exception($"operator {opr} is unknown");
                }
                return this;
            }
            return base.ExecuteOperation(opr, rigth);
        }

        public override BaseComputable CallFunction(IList<BaseComputable> args)
        {
            return new SimpleValue(_value.Length);
        }

        public override BaseComputable CallArray(int idx)
        {
            return new SimpleValue(_value[idx]);
        }

        public override void Assign(int idx, BaseComputable value)
        {
            _value[idx] = (char)value.GetValue();
        }
    }
}

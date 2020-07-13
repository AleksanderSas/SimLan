using System;

namespace SimLan.Evaluator
{
    class SimpleValue : BaseComputable
    {
        private int _value;

        public SimpleValue(int value)
        {
            _value = value;
        }

        public override int GetValue()
        {
            return _value;
        }

        public override BaseComputable Clone()
        {
            return new SimpleValue(_value);
        }

        public override BaseComputable ExecuteOperation(string opr, BaseComputable rigth)
        {
            if(rigth is SimpleValue r)
            {
                switch(opr)
                {
                    case "+":
                        _value += r._value;
                        break;
                    case "-":
                        _value -= r._value;
                        break;
                    case "*":
                        _value *= r._value;
                        break;
                    case "/":
                        _value /= r._value;
                        break;
                    case "==":
                        _value = _value == r._value? 1 : 0;
                        break;
                    case "<>":
                        _value = _value != r._value ? 1 : 0;
                        break;
                    case "<":
                        _value = _value < r._value ? 1 : 0;
                        break;
                    case ">":
                        _value = _value > r._value ? 1 : 0;
                        break;
                    case ">=":
                        _value = _value >= r._value ? 1 : 0;
                        break;
                    case "<=":
                        _value = _value <= r._value ? 1 : 0;
                        break;
                    case "&&":
                        _value &= r._value;
                        break;
                    case "||":
                        _value |= r._value;
                        break;
                    default: throw new Exception($"operator {opr} is unknown");
                }
                return this;
            }
            return base.ExecuteOperation(opr, rigth);
        }
    }
}

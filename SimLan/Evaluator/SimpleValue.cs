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

        public override bool Equals(object obj)
        {
            return obj is SimpleValue v && v._value == _value;
        }

        public override BaseComputable ExecuteOperation(string opr, BaseComputable rigth)
        {
            if (rigth is SimpleValue r)
            {
                switch (opr)
                {
                    case "+":
                        return new SimpleValue(_value + r._value);
                    case "-":
                        return new SimpleValue(_value - r._value);
                    case "*":
                        return new SimpleValue(_value * r._value);
                    case "/":
                        return new SimpleValue(_value / r._value);
                    case "==":
                        return new SimpleValue(_value == r._value ? 1 : 0);
                    case "<>":
                        return new SimpleValue(_value != r._value ? 1 : 0);
                    case "<":
                        return new SimpleValue(_value < r._value ? 1 : 0);
                    case ">":
                        return new SimpleValue(_value > r._value ? 1 : 0);
                    case ">=":
                        return new SimpleValue(_value >= r._value ? 1 : 0);
                    case "<=":
                        return new SimpleValue(_value <= r._value ? 1 : 0);
                    case "&&":
                        return new SimpleValue(_value & r._value);
                    case "||":
                        return new SimpleValue(_value | r._value);

                    default: throw new Exception($"operator {opr} is unknown");
                }
            }
            return base.ExecuteOperation(opr, rigth);
        }
    }
}

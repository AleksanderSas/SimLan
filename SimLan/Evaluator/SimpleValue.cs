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
    }
}

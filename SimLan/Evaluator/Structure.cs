using System.Collections.Generic;
using System.Linq;

namespace SimLan.Evaluator
{
    class Structure : BaseComputable
    {
        private Dictionary<string, int> _fieldMapping;
        private BaseComputable[] _fields = new BaseComputable[10];

        internal static IDictionary<string, List<string>> DefinedStructures = new Dictionary<string, List<string>>();

        public Structure(string type)
        {
            if(!DefinedStructures.TryGetValue(type, out var fields))
            {
                throw new System.Exception($"Struct {type} is not defined");
            }
            int idx = -1;
            _fieldMapping = fields.ToDictionary(x => x, x => ++idx);
        }

        public override ref BaseComputable Resolve(string field)
        {
            return ref _fields[_fieldMapping[field]];
        }

    }
}

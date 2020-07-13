using System;
using System.Collections.Generic;

namespace SimLan.Evaluator
{
    class Function : BaseComputable
    {
        private SimLanParser.BlockContext _body;
        private IList<string> _argNames;
        private IDictionary<string, BaseComputable> _baseVariables;
        public Function(IList<string> argNames, SimLanParser.BlockContext body, IDictionary<string, BaseComputable> baseVariables)
        {
            _body = body;
            _argNames = argNames;
            _baseVariables = baseVariables;
        }

        public override BaseComputable CallFunction(IList<BaseComputable> args)
        {
            var evaluationContext = new EvaluationContext(_baseVariables);
            if(args.Count != _argNames.Count)
            {
                throw new System.Exception($"Invalid args number passed to the function, expected {_argNames.Count} but was {args.Count}");
            }
            for(int i = 0; i < args.Count; i++)
            {
                evaluationContext.Variables.Add(_argNames[i], args[i]);
            }
            try
            {
                evaluationContext.ProgramEvaluator.VisitBlock(_body);
            }catch(ReturnException rex)
            {
                return rex.Value;
            }

            throw new Exception("function has no return path");
        }
    }
}

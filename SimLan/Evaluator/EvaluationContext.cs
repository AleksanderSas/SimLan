using Antlr4.Runtime;
using System.Collections.Generic;

namespace SimLan.Evaluator
{
    public class EvaluationContext
    {
        internal readonly ArthmeticEvaluator ArthmeticEvaluator;
        internal readonly ProgramEvaluator ProgramEvaluator;

        internal IDictionary<string, BaseComputable> Variables;

        internal EvaluationContext(IDictionary<string, BaseComputable> variables)
        {
            Variables = new Dictionary<string, BaseComputable>(variables);
            ArthmeticEvaluator = new ArthmeticEvaluator(this);
            ProgramEvaluator = new ProgramEvaluator(this);
        }

        public EvaluationContext()
        {
            Variables = new Dictionary<string, BaseComputable>();
            ArthmeticEvaluator = new ArthmeticEvaluator(this);
            ProgramEvaluator = new ProgramEvaluator(this);
        }

        public int RunProgram(string input)
        {
            var lexer = new SimLanLexer(new AntlrInputStream(input));
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            var parser = new SimLanParser(tokens);
            ProgramEvaluator.VisitProgram(parser.program());
            if(Variables.TryGetValue("main", out var function))
            {
                return function.CallFunction(new List<BaseComputable>()).GetValue();
            }
            throw new System.Exception("Missing main function");
        }

        public int RunArthmetic(string input)
        {
            var lexer = new SimLanLexer(new AntlrInputStream(input));
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            var parser = new SimLanParser(tokens);
            int result = ArthmeticEvaluator.VisitLogical_statement_1(parser.logical_statement_1()).GetValue();
            return result;
        }
    }
}

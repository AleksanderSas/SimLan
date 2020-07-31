using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace SimLan.Evaluator
{
    public class EvaluationContext
    {
        internal readonly ArthmeticEvaluator ArthmeticEvaluator;
        internal readonly ProgramEvaluator ProgramEvaluator;

        private static IDictionary<string, int> GlobalVariableMapping = new Dictionary<string, int>();
        private static BaseComputable[] GlobalVariables = new BaseComputable[20];

        private IDictionary<string, int> VariableMapping;
        private BaseComputable[] Variables;

        public static EvaluationContext CreateAndReset()
        {
            GlobalVariableMapping = new Dictionary<string, int>();
            GlobalVariables = new BaseComputable[20];
            return new EvaluationContext();
        }

        internal EvaluationContext(IDictionary<string, int> variablesMapping, BaseComputable[] variables)
        {
            VariableMapping = new Dictionary<string, int>(variablesMapping);
            Variables = (BaseComputable[])variables.Clone();
            ArthmeticEvaluator = new ArthmeticEvaluator(this);
            ProgramEvaluator = new ProgramEvaluator(this);
        }

        public EvaluationContext()
        {
            VariableMapping = new Dictionary<string, int>();
            Variables = new BaseComputable[20];
            ArthmeticEvaluator = new ArthmeticEvaluator(this);
            ProgramEvaluator = new ProgramEvaluator(this);
        }

        public int RunProgram(string input)
        {
            SimLanParser parser = CreateParser(input);
            ProgramEvaluator.VisitProgram(parser.program());
            var function = GetVaribale("main");
            return function.CallFunction(new List<BaseComputable>()).GetValue();
        }

        public int RunArthmetic(string input)
        {
            SimLanParser parser = CreateParser(input);
            int result = ArthmeticEvaluator.VisitLogical_statement_1(parser.logical_statement_1()).GetValue();
            return result;
        }

        internal ref BaseComputable GetVaribale(string name)
        {
            if (VariableMapping.TryGetValue(name, out int idx))
            {
                return ref Variables[idx];
            }
            if (GlobalVariableMapping.TryGetValue(name, out int idx2))
            {
                return ref GlobalVariables[idx2];
            }
            throw new System.Exception($"Varibale {name} not defined");
        }

        internal void DeclareGlobalVariable(string name, BaseComputable value = null)
        {
            int idx = GlobalVariableMapping.Count;
            if (!GlobalVariableMapping.TryAdd(name, idx))
                throw new System.Exception($"{name} is already defined");
            GlobalVariables[idx] = value;
        }

        internal void DeclareVariable(string name, BaseComputable value = null)
        {
            if (!VariableMapping.TryGetValue(name, out var idx))
            {
                idx = VariableMapping.Count;
                VariableMapping[name] = idx;
            }
            Variables[idx] = value;
        }

        private static SimLanParser CreateParser(string input)
        {
            var lexer = new SimLanLexer(new AntlrInputStream(input));
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            var parser = new SimLanParser(tokens);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ParsingErrorListener());
            return parser;
        }

        private class ParsingErrorListener : BaseErrorListener
        {
            public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
            {
                if (offendingSymbol.Text != "<EOF>")
                {
                    throw new System.Exception($"Syntax error at line {line}, at position {charPositionInLine}, offending symbol '{offendingSymbol.Text}'");
                }
            }
        }
    }
}

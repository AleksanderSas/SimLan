using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using static SimLan.SimLanParser;

namespace SimLan.Evaluator
{
    class ArthmeticEvaluator : SimLanBaseVisitor<int>
    {
        private EvaluationContext _evaluationContext;

        public ArthmeticEvaluator(EvaluationContext evaluationContext)
        {
            _evaluationContext = evaluationContext;
        }

        public override int VisitLogical_statement_1([NotNull] SimLanParser.Logical_statement_1Context context)
        {
            int value = context.logical_statement_2().Accept(this);
            var sub = context.logical_statement_1_2();
            return evaluareLogical1Sub(value, sub);
        }

        private int evaluareLogical1Sub(int value, Logical_statement_1_2Context sub)
        {
            if (sub.logical_statement_2() == null)
                return value;

            value |= sub.logical_statement_2().Accept(this);

            if (sub.logical_statement_1_2() != null)
            {
                return evaluareLogical1Sub(value, sub.logical_statement_1_2());
            }
            return value;
        }

        public override int VisitLogical_statement_2([NotNull] SimLanParser.Logical_statement_2Context context)
        {
            int value = context.logical_value().Accept(this);
            var sub = context.logical_statement_2_2();
            return evaluareLogical2Sub(value, sub);
        }

        private int evaluareLogical2Sub(int value, Logical_statement_2_2Context sub)
        {
            if (sub.logical_value() == null)
                return value;

            value &= sub.logical_value().Accept(this);

            if (sub.logical_statement_2_2() != null)
            {
                return evaluareLogical2Sub(value, sub.logical_statement_2_2());
            }
            return value;
        }

        public override int VisitLogical_value([NotNull] Logical_valueContext context)
        {
            if (context.logical_statement_1() != null)
            {
                return context.logical_statement_1().Accept(this);
            }

            var leftValue = context.v1.Accept(this);
            if(context.CMP() == null)
            {
                return leftValue;
            }
            var rigthValue = context.v2.Accept(this);
            switch(context.CMP().GetText())
            {
                case "==":
                    return leftValue == rigthValue? 1 : 0;
                case "<>":
                    return leftValue != rigthValue ? 1 : 0;
                case "<":
                    return leftValue < rigthValue ? 1 : 0;
                case ">":
                    return leftValue > rigthValue ? 1 : 0;
                case ">=":
                    return leftValue >= rigthValue ? 1 : 0;
                case "<=":
                    return leftValue <= rigthValue ? 1 : 0;

                default:
                    throw new Exception($"operator {context.CMP().GetText()} is unknown");

            }
        }

        public override int VisitArthmetic_statement_1([NotNull] SimLanParser.Arthmetic_statement_1Context context)
        {
            int value = context.arthmetic_statement_2().Accept(this);
            var sub = context.arthmetic_statement_1_2();
            return evaluareArthmetic1Sub(value, sub);
        }

        private int evaluareArthmetic1Sub(int value, Arthmetic_statement_1_2Context sub)
        {
            if (sub.arthmetic_statement_2() == null)
                return value;

            var operatorName = sub.OPERATOR_1().GetText();
            var rigthValue = sub.arthmetic_statement_2().Accept(this);
            switch (operatorName)
            {
                case "+":
                    value += rigthValue;
                    break;
                case "-":
                    value -= rigthValue;
                    break;
                default:
                throw new Exception($"operator {operatorName} is unknown");
            }
            if(sub.arthmetic_statement_1_2() != null)
            {
                return evaluareArthmetic1Sub(value, sub.arthmetic_statement_1_2());
            }
            return value;
        }

        public override int VisitArthmetic_statement_2([NotNull] SimLanParser.Arthmetic_statement_2Context context)
        {
            int value = context.arthmetic_value().Accept(this);
            var sub = context.arthmetic_statement_2_2();
            return evaluareArthmetic2Sub(value, sub);
        }

        private int evaluareArthmetic2Sub(int value, Arthmetic_statement_2_2Context sub)
        {
            if (sub.arthmetic_value() == null)
                return value;

            var operatorName = sub.OPERATOR_2().GetText();
            var rigthValue = sub.arthmetic_value().Accept(this);
            switch (operatorName)
            {
                case "*":
                    value *= rigthValue;
                    break;
                case "/":
                    value /= rigthValue;
                    break;
                default:
                    throw new Exception($"operator {operatorName} is unknown");
            }
            if (sub.arthmetic_statement_2_2() != null)
            {
                return evaluareArthmetic2Sub(value, sub.arthmetic_statement_2_2());
            }
            return value;
        }

        public override int VisitArthmetic_value([NotNull] Arthmetic_valueContext context)
        {
            if(context.simpleValue() != null)
            {
                return context.simpleValue().Accept(this);
            }
            return context.arthmetic_statement_1().Accept(this);
        }

        public override int VisitArthmetic_statement_1_2([NotNull] Arthmetic_statement_1_2Context context)
        {
            return base.VisitArthmetic_statement_1_2(context);
        }

        public override int VisitSimpleValue([NotNull] SimLanParser.SimpleValueContext context)
        {
            //simple constant
            if(context.NUM() != null)
            {
                return int.Parse(context.NUM().GetText());
            }

            string variableName = context.ID().GetText();
            if (!_evaluationContext.Variables.TryGetValue(variableName, out var runnable))
            {
                throw new Exception($"{variableName} is unknow");
            }

            //variable
            if(context.args() == null)
            {
                return runnable.GetValue();
            }

            //function
            return CallFunction(runnable, context.args()).GetValue();
        }

        private BaseComputable CallFunction(BaseComputable function, ArgsContext args)
        {
            List<BaseComputable> evaluatedArgs = new List<BaseComputable>();
            if (args.a1 != null)
            {
                evaluatedArgs.Add(new SimpleValue(args.a1.Accept(this)));
            }
            if (args._a2.Any())
            {
                evaluatedArgs.AddRange(args._a2.Select(x => new SimpleValue(x.Accept(this))));
            }
            return function.CallFunction(evaluatedArgs);
        }
    }
}

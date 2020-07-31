using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;
using static SimLan.SimLanParser;

namespace SimLan.Evaluator
{
    class ArthmeticEvaluator : SimLanBaseVisitor<BaseComputable>
    {
        private EvaluationContext _evaluationContext;

        public ArthmeticEvaluator(EvaluationContext evaluationContext)
        {
            _evaluationContext = evaluationContext;
        }

        public override BaseComputable VisitLogical_statement_1([NotNull] Logical_statement_1Context context)
        {
            BaseComputable value = context.logical_statement_2().Accept(this);
            var sub = context.logical_statement_1_2();
            return evaluareLogical1Sub(value, sub);
        }

        private BaseComputable evaluareLogical1Sub(BaseComputable value, Logical_statement_1_2Context sub)
        {
            if (sub.logical_statement_2() == null)
                return value;

            value = value.ExecuteOperation("||", sub.logical_statement_2().Accept(this));

            if (sub.logical_statement_1_2() != null)
            {
                return evaluareLogical1Sub(value, sub.logical_statement_1_2());
            }
            return value;
        }

        public override BaseComputable VisitLogical_statement_2([NotNull] Logical_statement_2Context context)
        {
            BaseComputable value = context.logical_value().Accept(this);
            var sub = context.logical_statement_2_2();
            return evaluareLogical2Sub(value, sub);
        }

        private BaseComputable evaluareLogical2Sub(BaseComputable value, Logical_statement_2_2Context sub)
        {
            if (sub.logical_value() == null)
                return value;

            value = value.ExecuteOperation("&&", sub.logical_value().Accept(this));

            if (sub.logical_statement_2_2() != null)
            {
                return evaluareLogical2Sub(value, sub.logical_statement_2_2());
            }
            return value;
        }

        public override BaseComputable VisitLogical_value([NotNull] Logical_valueContext context)
        {
            var leftValue = context.v1.Accept(this);
            if (context.CMP() == null)
            {
                return leftValue;
            }
            var rigthValue = context.v2.Accept(this);
            return leftValue.ExecuteOperation(context.CMP().GetText(), rigthValue);
        }

        public override BaseComputable VisitArthmetic_statement_1([NotNull] Arthmetic_statement_1Context context)
        {
            BaseComputable value = context.arthmetic_statement_2().Accept(this);
            var sub = context.arthmetic_statement_1_2();
            return evaluareArthmetic1Sub(value, sub);
        }

        private BaseComputable evaluareArthmetic1Sub(BaseComputable value, Arthmetic_statement_1_2Context sub)
        {
            if (sub.arthmetic_statement_2() == null)
                return value;

            var operatorName = sub.OPERATOR_1().GetText();
            var rigthValue = sub.arthmetic_statement_2().Accept(this);
            value = value.ExecuteOperation(operatorName, rigthValue);
            if (sub.arthmetic_statement_1_2() != null)
            {
                return evaluareArthmetic1Sub(value, sub.arthmetic_statement_1_2());
            }
            return value;
        }

        public override BaseComputable VisitArthmetic_statement_2([NotNull] Arthmetic_statement_2Context context)
        {
            BaseComputable value = context.arthmetic_value().Accept(this);
            var sub = context.arthmetic_statement_2_2();
            return evaluareArthmetic2Sub(value, sub);
        }

        private BaseComputable evaluareArthmetic2Sub(BaseComputable value, Arthmetic_statement_2_2Context sub)
        {
            if (sub.arthmetic_value() == null)
                return value;

            var operatorName = sub.OPERATOR_2().GetText();
            var rigthValue = sub.arthmetic_value().Accept(this);
            value = value.ExecuteOperation(operatorName, rigthValue);
            if (sub.arthmetic_statement_2_2() != null)
            {
                return evaluareArthmetic2Sub(value, sub.arthmetic_statement_2_2());
            }
            return value;
        }

        public override BaseComputable VisitArthmetic_value([NotNull] Arthmetic_valueContext context)
        {
            if (context.simpleValue() != null)
            {
                return context.simpleValue().Accept(this);
            }
            return context.logical_statement_1().Accept(this);
        }

        public override BaseComputable VisitArthmetic_statement_1_2([NotNull] Arthmetic_statement_1_2Context context)
        {
            return base.VisitArthmetic_statement_1_2(context);
        }

        public override BaseComputable VisitSimpleValue([NotNull] SimpleValueContext context)
        {
            //simple constant
            if (context.NUM() != null)
            {
                return new SimpleValue(int.Parse(context.NUM().GetText()));
            }

            //simple constant
            if (context.CHAR() != null)
            {
                return new SimpleValue(context.CHAR().GetText()[1]);
            }

            //string
            if (context.STR() != null)
            {
                var str = context.STR().GetText();
                return new Str(str.Substring(1, str.Length - 2));
            }

            if (context.NEW() != null)
            {
                if (context.a2 != null)
                {
                    return new Array(context.a2.logical_statement_1().Accept(this).GetValue());
                }else
                {
                    return new Structure(context.ID().GetText());
                }
            }

            string variableName = context.ID().GetText();
            var runnable = _evaluationContext.GetVaribale(variableName);
            
            foreach (var args in context.args())
            {
                runnable = CallFunction(runnable, args);
            }
            runnable = Resolve(context, ref runnable);

            return runnable;
        }

        public ref BaseComputable GetReference(SimpleValueContext context)
        {
            string variableName = context.ID().GetText();
            ref var runnable = ref _evaluationContext.GetVaribale(variableName);
            runnable = ref Resolve(context, ref runnable);
            return ref runnable;
        }


        private ref BaseComputable Resolve(SimpleValueContext context, ref BaseComputable runnable)
        {
            foreach (var subValue in context.referenceValueResolver())
            {
                runnable = ref Resolve(runnable, subValue);
            }

            return ref runnable;
        }

        private ref BaseComputable Resolve(BaseComputable bs, [NotNull] ReferenceValueResolverContext contex)
        {
            if(contex.ID() != null)
            {
                return ref bs.Resolve(contex.ID().GetText());
            }
            else
            {
                int idx = contex.array().logical_statement_1().Accept(this).GetValue();
                return ref bs.CallArray(idx);
            }
        }

        private BaseComputable CallFunction(BaseComputable function, ArgsContext args)
        {
            List<BaseComputable> evaluatedArgs = new List<BaseComputable>();
            if (args.a1 != null)
            {
                evaluatedArgs.Add(args.a1.Accept(this));
            }
            if (args._a2.Any())
            {
                evaluatedArgs.AddRange(args._a2.Select(x => x.Accept(this)));
            }
            return function.CallFunction(evaluatedArgs);
        }
    }
}

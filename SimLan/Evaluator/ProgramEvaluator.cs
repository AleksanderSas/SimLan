using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;

namespace SimLan.Evaluator
{
    class Do
    {
        public static Do Notking;
    }

    class ProgramEvaluator : SimLanBaseVisitor<Do>
    {
        private EvaluationContext _evaluationContext;

        protected override Do AggregateResult(Do aggregate, Do nextResult)
        {
            return Do.Notking;
        }

        public ProgramEvaluator(EvaluationContext evaluationContext)
        {
            _evaluationContext = evaluationContext;
        }

        public override Do VisitIf_statement([NotNull] SimLanParser.If_statementContext context)
        {
            if(_evaluationContext.ArthmeticEvaluator.VisitLogical_statement_1(context.logical_statement_1()) > 0)
            {
                context.b1.Accept(this);
            }
            else
            {
                if (context.b2 != null)
                {
                    context.b2.Accept(this);
                }
            }

            return Do.Notking;
        }

        public override Do VisitAssignment([NotNull] SimLanParser.AssignmentContext context)
        {
            var value = new SimpleValue(context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator));
            _evaluationContext.Variables[context.ID().GetText()] = value;

            return Do.Notking;
        }

        public override Do VisitReturn_statement([NotNull] SimLanParser.Return_statementContext context)
        {
            //FunctionResult = new SimpleValue(context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator));
            throw new ReturnException(new SimpleValue(context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator)));
        }

        public override Do VisitFor_statement([NotNull] SimLanParser.For_statementContext context)
        {
            context.a1.Accept(this);
            while(context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator) > 0)
            {
                context.block().Accept(this);
                context.a2.Accept(this);
            }
            return Do.Notking;
        }

        public override Do VisitFunction([NotNull] SimLanParser.FunctionContext context)
        {
            var funcName = context.ID().GetText();
            var function = new Function(GetFuncArgs(context.args_def()), context.block(), _evaluationContext.Variables);
            _evaluationContext.Variables.Add(funcName, function);

            return Do.Notking;
        }

        private IList<string> GetFuncArgs(SimLanParser.Args_defContext argsDef)
        {
            var args = new List<string>();
            if(argsDef.a1 != null)
            {
                args.Add(argsDef.a1.Text);
            }
            args.AddRange(argsDef._a2.Select(x => x.Text));
            return args;
        }
    }
}

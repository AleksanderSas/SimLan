using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Linq;

namespace SimLan.Evaluator
{
    class Do
    {
        public static Do Nothing;
    }

    class ProgramEvaluator : SimLanBaseVisitor<Do>
    {
        private EvaluationContext _evaluationContext;

        protected override Do AggregateResult(Do aggregate, Do nextResult)
        {
            return Do.Nothing;
        }

        public ProgramEvaluator(EvaluationContext evaluationContext)
        {
            _evaluationContext = evaluationContext;
        }

        public override Do VisitIf_statement([NotNull] SimLanParser.If_statementContext context)
        {
            if (_evaluationContext.ArthmeticEvaluator.VisitLogical_statement_1(context.logical_statement_1()).GetValue() > 0)
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

            return Do.Nothing;
        }

        public override Do VisitAssignment([NotNull] SimLanParser.AssignmentContext context)
        {
            var value = context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator);

            if (context.VAR() != null)
            {
                _evaluationContext.DeclareVariable(context.ID().GetText(), value);
            }
            else
            {
                ref var variable = ref _evaluationContext.ArthmeticEvaluator.GetReference(context.simpleValue());
                variable = value;
            }

            return Do.Nothing;
        }

        public override Do VisitLoopControll([NotNull] SimLanParser.LoopControllContext context)
        {
            throw new LoopControllException(context.BREAK() != null);
        }

        public override Do VisitReturn_statement([NotNull] SimLanParser.Return_statementContext context)
        {
            throw new ReturnException(context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator));
        }

        public override Do VisitFor_statement([NotNull] SimLanParser.For_statementContext context)
        {
            context.a1.Accept(this);
            while (context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator).GetValue() > 0)
            {
                try
                {
                    context.block().Accept(this);
                }catch(LoopControllException ex)
                {
                    if (ex.IsBreak) return Do.Nothing;
                }
                context.a2.Accept(this);
            }
            return Do.Nothing;
        }

        public override Do VisitWhile_statement([NotNull] SimLanParser.While_statementContext context)
        {
            while (context.logical_statement_1().Accept(_evaluationContext.ArthmeticEvaluator).GetValue() > 0)
            {
                try
                {
                    context.block().Accept(this);
                }
                catch (LoopControllException ex)
                {
                    if (ex.IsBreak) return Do.Nothing;
                }
            }
            return Do.Nothing;
        }

        public override Do VisitFunction([NotNull] SimLanParser.FunctionContext context)
        {
            var funcName = context.ID().GetText();
            var function = new Function(GetFuncArgs(context.args_def()), context.block(), _evaluationContext, funcName);
            _evaluationContext.DeclareGlobalVariable(funcName, function);

            return Do.Nothing;
        }

        public override Do VisitStructDefinition([NotNull] SimLanParser.StructDefinitionContext context)
        {
            Structure.DefinedStructures.Add(context.name.Text, context._id.Select(x => x.Text).ToList());
            return Do.Nothing;
        }

        private IList<string> GetFuncArgs(SimLanParser.Args_defContext argsDef)
        {
            var args = new List<string>();
            if (argsDef.a1 != null)
            {
                args.Add(argsDef.a1.Text);
            }
            args.AddRange(argsDef._a2.Select(x => x.Text));
            return args;
        }
    }
}

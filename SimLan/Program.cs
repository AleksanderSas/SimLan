using System;

namespace SimLan
{
    class Program
    {
        static void Main(string[] args)
        {
            var evaluator = new Evaluator.EvaluationContext();
            string input =
@"
main()
{
    return 5;
}
";
            Console.WriteLine(evaluator.RunProgram(input));
        }
    }
}

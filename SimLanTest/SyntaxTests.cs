using NUnit.Framework;
using SimLan.Evaluator;

namespace SimLan
{
    public class SyntaxTests
    {
        private EvaluationContext evaluator;

        [SetUp]
        public void Setup()
        {
            evaluator = new EvaluationContext();
        }

        [Test]
        public void ArthmeticExpressiona()
        {
            Assert.AreEqual(6, evaluator.RunArthmetic("1 + 2 + 3"));
            Assert.AreEqual(-4, evaluator.RunArthmetic("1 - 2 - 3"));
            Assert.AreEqual(6, evaluator.RunArthmetic("12 - 1 * 5 - 1 "));
            Assert.AreEqual(40, evaluator.RunArthmetic("12 * 5 - 2 * 10"));
            Assert.AreEqual(18, evaluator.RunArthmetic("3 * (5 - 2) * 2"));
        }

        [Test]
        public void LogicalExpressiona()
        {
            Assert.AreEqual(1, evaluator.RunArthmetic("1 && 1"));
            Assert.AreEqual(0, evaluator.RunArthmetic("0 && 1"));
            Assert.AreEqual(0, evaluator.RunArthmetic("1 && 0"));
            Assert.AreEqual(0, evaluator.RunArthmetic("0 && 0"));

            Assert.AreEqual(1, evaluator.RunArthmetic("1 || 1"));
            Assert.AreEqual(1, evaluator.RunArthmetic("0 || 1"));
            Assert.AreEqual(1, evaluator.RunArthmetic("1 || 0"));
            Assert.AreEqual(0, evaluator.RunArthmetic("0 || 0"));

            Assert.AreEqual(0, evaluator.RunArthmetic("1 && 0 || 1 && 0"));
            Assert.AreEqual(1, evaluator.RunArthmetic("1 && 0 || 1 && 1"));

            Assert.AreEqual(1, evaluator.RunArthmetic("0 &&  1 || 1 && 1"));
            Assert.AreEqual(0, evaluator.RunArthmetic("0 && (1 || 1) && 1"));
        }

        [Test]
        public void LogicalAndArthmeticCombinedExpressiona()
        {
            Assert.AreEqual(0, evaluator.RunArthmetic("1 == 1 && 1 <> 1"));
            Assert.AreEqual(1, evaluator.RunArthmetic("1 == 1 && 1 < 2"));
            Assert.AreEqual(1, evaluator.RunArthmetic("2 > 1 && 1 < 2"));

            Assert.AreEqual(0, evaluator.RunArthmetic("2 > 1 + 5"));
            Assert.AreEqual(1, evaluator.RunArthmetic("2 < 1 + 5"));
        }

        [Test]
        public void Return()
        {
            Assert.AreEqual(5, evaluator.RunProgram("main(){ return 5; }"));
        }

        [Test]
        public void IfTrue()
        {
            Assert.AreEqual(125, evaluator.RunProgram("main(){ if( 1 > 2 ) return 42; else return 125; }"));
        }

        [Test]
        public void IfFalse()
        {
            Assert.AreEqual(42, evaluator.RunProgram("main(){ if( 1 < 2 ) return 42; else return 125; }"));
        }

        [Test]
        public void Assign()
        {
            Assert.AreEqual(42, evaluator.RunProgram("main(){ x = 42; return x; }"));
        }

        [Test]
        public void LValueIsNotModified()
        {
            Assert.AreEqual(16, evaluator.RunProgram("main(){ x = 4; return x + x + x + x; }"));
        }

        [Test]
        public void For()
        {
            Assert.AreEqual(48, evaluator.RunProgram("main(){ x = 42; for(i = 0; i < 3; i = i + 1;) x = x + 2; return x; }"));
        }

        [Test]
        public void FunctionCall()
        {
            Assert.AreEqual(20, evaluator.RunProgram("foo(x){ return x * 2; } main(){ return foo(10); }"));
        }

        [Test]
        public void PassFunctionAsArgument()
        {
            Assert.AreEqual(22, evaluator.RunProgram("foo(func){ return func(11); } goo(x){ return x * 2; } main(){ return foo(goo); }"));
        }

        [Test]
        public void ReturnFunctionAndDoubleCall()
        {
            Assert.AreEqual(22, evaluator.RunProgram("foo(){ return goo; } goo(x){ return x * 2; } main(){ return foo()(11); }"));
        }

        [TestCase("main(){ if( 1 < 2 )return 10; return 12; }", 10)]
        [TestCase("main(){ if( 1 > 2 )return 10; return 12; }", 12)]
        public void ReturnTerminateExecution(string program, int expectedValue)
        {
            Assert.AreEqual(expectedValue, evaluator.RunProgram(program));
        }

        [Test]
        public void Fibonacci()
        {
            string program =
            @"
            fib_base(n, x_1, x_2) {
               if(n == 1) 
                  return x_1;
               else
                  return fib_base(n - 1, x_1 + x_2, x_1);
            }

            fib(n) { return fib_base(n, 1, 0); }
            main() { return fib(8); }
            ";

            Assert.AreEqual(21, evaluator.RunProgram(program));
        }

        [Test]
        public void Fibonacci_2()
        {
            string program =
            @"
            fib(n) {
               x_1 = 1;
               x_2 = 0;
               for(i = n; i > 1; i = i - 1;){
                  tmp = x_2;
                  x_2 = x_1;
                  x_1 = x_1 + tmp;
               }

               return x_1;
            }

            main() { return fib(8); }
            ";

            Assert.AreEqual(21, evaluator.RunProgram(program));
        }
    }
}
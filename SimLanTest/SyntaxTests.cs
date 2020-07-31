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
            evaluator = EvaluationContext.CreateAndReset();
        }

        [Test]
        public void ArthmeticExpressiona()
        {
            Assert.AreEqual(3, evaluator.RunArthmetic("1 + 2"));
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
        public void MultiIfFalseFalse()
        {
            Assert.AreEqual(125, evaluator.RunProgram("main(){ if( 0 ) return 42; else if( 0 ) return 10; else return 125; }"));
        }

        [Test]
        public void MultiIfFalseTrue()
        {
            Assert.AreEqual(10, evaluator.RunProgram("main(){ if( 0 ) return 42; else if( 1 ) return 10; else return 125; }"));
        }

        [Test]
        public void Assign()
        {
            Assert.AreEqual(42, evaluator.RunProgram("main(){ var x = 42; return x; }"));
        }

        [Test]
        public void LValueIsNotModified()
        {
            Assert.AreEqual(16, evaluator.RunProgram("main(){ var x = 4; return x + x + x + x; }"));
        }

        [Test]
        public void For()
        {
            Assert.AreEqual(48, evaluator.RunProgram("main(){ var x = 42; for(var i = 0; i < 3; i = i + 1;) x = x + 2; return x; }"));
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
        public void VariableAreAssignementByValue()
        {
            string program =
            @"
            main() {
               var x = 5;
               var y = 10;
               x = y;
               y = 42;
               return x;
            ";

            Assert.AreEqual(10, evaluator.RunProgram(program));
        }

        [Test]
        public void Array()
        {
            string program =
            @"
            main() {
               var array = new[5];
               array[0] = 1;
               array[1] = 2;
               array[2] = 3;
               return array[1];
            ";

            Assert.AreEqual(2, evaluator.RunProgram(program));
        }

        [Test]
        public void DoubleArray()
        {
            string program =
            @"
            main() {
               var array = new[5];
               array[0] = 1;
               array[1] = new [5];
               array[2] = 3;

               array[1][0] = 10;
               array[1][1] = 20;
               array[1][2] = 30;
               return array[1][1];
            ";

            Assert.AreEqual(20, evaluator.RunProgram(program));
        }

        [Test]
        public void ArraySize()
        {
            Assert.AreEqual(5, evaluator.RunProgram("main(){ var a = new[5]; return a(); }"));
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
               var x_1 = 1;
               var x_2 = 0;
               for(var i = n; i > 1; i = i - 1;){
                  var tmp = x_2;
                  x_2 = x_1;
                  x_1 = x_1 + tmp;
               }

               return x_1;
            }

            main() { return fib(8); }
            ";

            Assert.AreEqual(21, evaluator.RunProgram(program));
        }

        [Test]

        public void Comment()
        {
            var program =
            @"
                main(){ 
                    //return 5;
                    return 42;
                }
            ";
            Assert.AreEqual(42, evaluator.RunProgram(program));
        }

        [Test]
        public void StringSize()
        {
            Assert.AreEqual(20, evaluator.RunProgram("main(){ var str = 'Wielki test stringow'; return str(); }"));
        }

        [Test]
        public void AccessCharFromString()
        {
            Assert.AreEqual((int)'i', evaluator.RunProgram("main(){ var str = 'Wielki test stringow';  return str[5]; }"));
        }

        [Test]
        public void AccessCharFromString2()
        {
            Assert.AreEqual((int)'A', evaluator.RunProgram("main(){ var str = 'Wielki test stringow'; str[5] = `A`; return str[5]; }"));
        }

        [Test]
        public void StringComparisin()
        {
            Assert.AreEqual(1, evaluator.RunProgram("main(){ return 'qwer' == 'qwer'; }"));
        }

        [Test]
        public void StringComparisin2()
        {
            Assert.AreEqual(0, evaluator.RunProgram("main(){ return 'qwer' <> 'qwer'; }"));
        }

        [Test]
        public void Char()
        {
            Assert.AreEqual(97, evaluator.RunProgram("main(){ return `a`; }"));
        }

        [Test]
        public void LValueOfArrayElementIsNotModified()
        {
            string program =
            @"
            main() {
               var t = new[2];
               t[0] = 1;
               t[1] = 2;
               var x = t[0] > 5;
               return t[0];
            }
            ";

            Assert.AreEqual(1, evaluator.RunProgram(program));
        }

        [Test]
        public void Definition()
        {
            string program =
            @"
            def d1 {
                var c;
                var t;
                var s;
            }

            main() {
               var d = new d1;
               d.t = new [3];
               d.t[2] = 42;
               return d.t[2];
            }
            ";

            Assert.AreEqual(42, evaluator.RunProgram(program));
        }

        [Test]
        public void LongestCommonSubstr()
        {
            string program =
            @"
            
            max(x, y, z)
            {
                if(x > y)
                    if(x > z) return x; else return z;
                else 
                    if(y > z) return y; else return z;
            }

            LCS(s1, s2) {
               var t = new[s1()];
               for(var i = 0; i < s1(); i = i + 1;)
               {
                  t[i] = new [s2()];
               }

               t[0][0] = s1[0] == s2[0];
               for(var x = 1; x < s2(); x = x + 1;)
               {
                  t[0][x] = s1[0] == s2[x] || t[0][x-1];
               }

               for(var y = 1; y < s1(); y = y + 1;)
               {
                  t[y][0] = s1[y] == s2[0] || t[y-1][0];
               }

               for(var y = 1; y < s1(); y = y + 1;)
                  for(var x = 1; x < s2(); x = x + 1;)
                  {
                     t[y][x] = max(t[y-1][x], t[y][x-1], t[y-1][x-1] + (s1[y] == s2[x]));
                  }

               return t[s1() - 1][s2() - 1];
            }

            main() { return LCS('abacd', 'cbcaed');}
            ";

            Assert.AreEqual(3, evaluator.RunProgram(program));
        }
    }
}
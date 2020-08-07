using NUnit.Framework;
using SimLan.Evaluator;

namespace SimLanTest
{
    class Regex
    {
        private EvaluationContext evaluator;

        [SetUp]
        public void Setup()
        {
            evaluator = EvaluationContext.CreateAndReset();
        }

        string regex =
@"
def tokenBuffer
{
    var tokens;
    var size;
}

def token
{
    var c;

    //0: simple char 
    //1: *
    //2: +
    //3: ?
    var type; 
}

parse(pattern)
{
    var size = -1;
    var tokens = new[10];
    var currentToken = new token;

    for(var i = 0; i < pattern(); i = i + 1;)
    {
        
        if(pattern[i] == `*`)
        {
            currentToken.type = 1;
        }
        else if(pattern[i] == `+`)
        {
            currentToken.type = 2;
        }
        else if(pattern[i] == `?`)
        {
            currentToken.type = 3;
        }
        else
        {
            if(size >= 0)
            {
                tokens[size] = currentToken;
            }
            size = size + 1;  
            currentToken = new token;
            currentToken.c = pattern[i];
            currentToken.type = 0;
        }          
    }
    tokens[size] = currentToken;
    size = size + 1;  
    var result = new tokenBuffer;

    result.tokens = tokens;
    result.size = size;
    return result;
}

IsNodeNotFullfield(charRepeats, node)
{
    if ((node.type == 0 || node.type == 3) && charRepeats > 1)
        return 1;
    if ((node.type == 0 || node.type == 2) && charRepeats == 0)
        return 1;
    return 0;
}

simpleRegex(pattern, value)
{
    var t = parse(pattern);
    var acceptedNodes = 0;
    var charRepeats = 0;
    var charPosition = 0;
    while(charPosition < value())
    {
        if(value[charPosition] == t.tokens[acceptedNodes].c)
        {
            charRepeats = charRepeats + 1;
            if(IsNodeNotFullfield(charRepeats, t.tokens[acceptedNodes]))
                return 0;
            charPosition = charPosition + 1;
        }
        else while(value[charPosition] <> t.tokens[acceptedNodes].c)
        {

            if(IsNodeNotFullfield(charRepeats, t.tokens[acceptedNodes]) || acceptedNodes + 1 == t.size)
                return 0;
            acceptedNodes = acceptedNodes + 1;
            charRepeats = 0;
                        
        }
    }

    for (var foo = 0; acceptedNodes < t.size; acceptedNodes = acceptedNodes + 1;)
    {
        if (IsNodeNotFullfield(charRepeats, t.tokens[acceptedNodes]))
        {
            return 0;
        }
        charRepeats = 0;
    }
    return acceptedNodes == t.size;
}";



        [Test]
        public void test()
        {
            var program = regex + @"
            main() { return simpleRegex('qwe', 'qwe') && 
                            simpleRegex('q+w*e?', 'qqqwwww') && 
                            simpleRegex('q+w*e?', 'qqqe') &&
                            simpleRegex('q+w*e?', 'we') == 0 &&
                            simpleRegex('q+w*e?', 'qwee') == 0;}";
            Assert.AreEqual(1, evaluator.RunProgram(program));
        }
    }
}

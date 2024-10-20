
namespace Sprache.Tests;

public class Part07_ChainOperatorUnitest
{
    [Fact]
    public void TestChainOperator()
    {
        // Chain a left-associative operator.

        // Parser<T> ChainOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)
        {
            Parser<char> add = Parse.Char('+').Token();
            Parser<int> number = Parse.Number.Token().Select(int.Parse);

            Parser<int> expr = Parse.ChainOperator(add, number, (op, left, right) => left + right);

            Assert.Equal(3, expr.Parse("1 + 2"));
            Assert.Equal(9, expr.Parse("1 + 2 + 3 + 3"));
            Assert.Equal(1, expr.Parse("1"));
            // Unexpected end of input reached; expected numeric character
            Assert.Throws<ParseException>(() => expr.Parse(""));

            // [Sprache/src/Sprache/Parse.cs -- ChainOperator](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L656)
        }

        {
            Parser<char> add = Parse.Char('+').Token();
            Parser<char> subtract = Parse.Char('-').Token();
            Parser<string> number = Parse.Number.Token();

            Parser<string> expr = Parse.ChainOperator(add.Or(subtract), number, 
                (op, left, right) => $"({left} {op} {right})");

            Assert.Equal("(1 + 2)", expr.Parse("1 + 2"));
            Assert.Equal("(((1 + 2) - 3) + 3)", expr.Parse("1 + 2 - 3 + 3"));
            Assert.Equal("1", expr.Parse("1"));
        }
    }

    [Fact]
    public void TestXChainOperator()
    {
        // Chain a left-associative operator.

        // Parser<T> XChainOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)

        Parser<char> addOp = Parse.Char('+').Token();
        Parser<int> number = Parse.Number.Token().Select(int.Parse);
        Parser<int> addX = Parse.XChainOperator(addOp, number, (op, left, right) => left + right);

        // unexpected 'a'; expected numeric character
        Assert.Throws<ParseException>(() => addX.Parse("1 + 3 + aaa"));

        Assert.Equal(8, addX.Parse("1 + 3 + 4a + 5"));

        // [Sprache/src/Sprache/Parse.cs -- XChainOperator](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L676)
    }

    [Fact]
    public void TestChainRightOperator()
    {
        // Chain a right-associative operator.

        // Parser<T> ChainRightOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)

        Parser<char> exp = Parse.Char('^').Token();
        Parser<string> number = Parse.Number.Token();

        Parser<string> expr = Parse.ChainRightOperator(exp, number,  (op, left, right) => $"({left} {op} {right})");

        Assert.Equal("(1 ^ 2)", expr.Parse("1 ^ 2"));
        Assert.Equal("(1 ^ (2 ^ (3 ^ 3)))", expr.Parse("1 ^ 2 ^ 3 ^ 3"));

        // [Sprache/src/Sprache/Parse.cs -- ChainRightOperator](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L712)
    }

    [Fact]
    public void TestXChainRightOperator()
    {
        // Chain a right-associative operator.

        // Parser<T> XChainRightOperator<T, TOp>(Parser<TOp> op, Parser<T> operand, Func<TOp, T, T, T> apply)

        Parser<char> exp = Parse.Char('^').Token();
        Parser<string> number = Parse.Number.Token();

        Parser<string> exprX = Parse.ChainRightOperator(exp, number,  (op, left, right) => $"({left} {op} {right})");

        Assert.Throws<ParseException>(() => exprX.Parse("a ^ 2 ^ 3"));

        // [Sprache/src/Sprache/Parse.cs -- XChainRightOperator](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L732)
    }
}


namespace Sprache.Tests;

public class Part12_1_RefUnitest
{
    [Fact]
    public void TestRef()
    {
        // Refer to another parser indirectly. This allows circular compile-time dependency between parsers.

        // Parser<T> Ref<T>(Func<Parser<T>> reference)

        Assert.Equal(26, MyParserRef.AdditiveExpression.End().Parse("1+(2+3)*4+5"));

        // [Sprache/src/Sprache/Parse.cs -- Ref](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L387)
    }

}


class MyParserRef
{
    public static readonly Parser<float> Integer =
        Parse.Number.Token().Select(float.Parse);

    public static readonly Parser<float> PrimaryExpression =
        Integer.Or(Parse.Ref(() => AdditiveExpression).Contained(Parse.Char('('), Parse.Char(')')));

    public static readonly Parser<float> MultiplicativeExpression =
        Parse.ChainOperator(Parse.Char('*'), PrimaryExpression, (c, left, right) => left * right);

    public static readonly Parser<float> AdditiveExpression =
        Parse.ChainOperator(Parse.Char('+'), MultiplicativeExpression, (c, left, right) => left + right);
}

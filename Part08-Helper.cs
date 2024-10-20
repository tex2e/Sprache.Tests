
namespace Sprache.Tests;

public class Part08_HelperUnitest
{
    [Fact]
    public void TestToken()
    {
        // Parse the token, embedded in any amount of whitespace characters.

        // Parser<T> Token<T>(this Parser<T> parser)

        Parser<int> expression =
            from left in Parse.Number.Token()
            from plus in Parse.Char('+').Token()
            from right in Parse.Number.Token()
            select int.Parse(left) + int.Parse(right);

        Assert.Equal(4, expression.Parse("2 + 2"));
        Assert.Equal(4, expression.Parse(" 2 + 2"));
        Assert.Equal(4, expression.Parse("\n2\n  +   \n 2 \n "));

        // [Sprache/src/Sprache/Parse.cs -- Token](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L371)
    }

    [Fact]
    public void TestContained()
    {
        // Helper that identifies elements contained by some other tokens.

        // Parser<T> Contained<T, U, V>(this Parser<T> parser, Parser<U> open, Parser<V> close)

        Parser<string> parser = Parse.Letter.Many().Text().Contained(Parse.Char('('), Parse.Char(')'));

        Assert.Equal("foo", parser.Parse("(foo)"));
        // Empty elements are allowed
        Assert.Equal("", parser.Parse("()"));

        // Unexpected end of input reached; expected )
        Assert.Throws<ParseException>(() => parser.Parse("(foo"));

        // [Sprache/src/Sprache/Parse.Sequence.cs -- Contained](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Sequence.cs#L146)
    }

    [Fact]
    public void TestIdentifier()
    {
        // Parser for identifier starting with firstLetterParser and continuing with tailLetterParser

        // Parser<string> Identifier(Parser<char> firstLetterParser, Parser<char> tailLetterParser)
        Parser<string> identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        Assert.Equal("d1", identifier.Parse("d1"));

        // unexpected '1'; expected letter
        Assert.Throws<ParseException>(() => identifier.Parse("1d"));

        // [Sprache/src/Sprache/Parse.Primitives.cs -- Identifier](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Primitives.cs#L26)
    }

    [Fact]
    public void TestLineTerminator()
    {
        // Line ending or end of input

        // Parser<string> LineTerminator
        Parser<string> parser = Parse.LineTerminator;

        Assert.Equal("", parser.Parse(""));
        Assert.Equal("\n", parser.Parse("\n foo"));
        Assert.Equal("\r\n", parser.Parse("\r\n foo"));

        // [Sprache/src/Sprache/Parse.Primitives.cs -- LineTerminator](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Primitives.cs#L17C23-L17C52)
    }
}
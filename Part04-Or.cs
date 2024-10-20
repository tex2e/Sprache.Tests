
namespace Sprache.Tests;

public class Part04_OrUnittest
{
    [Fact]
    public void TestOr()
    {
        // Parse first, if it succeeds, return first, otherwise try second.

        // Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        Parser<string> keyword = Parse.String("return")
            .Or(Parse.String("function"))
            .Or(Parse.String("switch"))
            .Or(Parse.String("if"))
            .Text();

        Assert.Equal("return", keyword.Parse("return"));
        Assert.Equal("if", keyword.Parse("if"));

        // [Sprache/src/Sprache/Parse.cs -- Or](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L432)
    }

    [Fact]
    public void TestXOr()
    {
        // Parse first, if it succeeds, return first, otherwise try second.
        // Assumes that the first parsed character will determine the parser chosen (see Try).
        // XOrは最初のParserが1文字以上一致したときに、それ以降のParserで試行しません。

        // Parser<T> XOr<T>(this Parser<T> first, Parser<T> second)
        var parser = Parse.String("foo")
            .XOr(Parse.Identifier(Parse.Letter, Parse.LetterOrDigit));

        Assert.Equal("bar", parser.Parse("bar"));
        //  unexpected 'a'; expected o
        Assert.Throws<ParseException>(() => parser.Parse("far"));

        // [Sprache/src/Sprache/Parse.cs -- XOr](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L477)
    }
}

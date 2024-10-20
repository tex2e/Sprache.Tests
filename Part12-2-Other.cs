
namespace Sprache.Tests;

public class Part12_OthersUnitest
{
    [Fact]
    public void TestNamed()
    {
        // Names part of the grammar for help with error messages.

        // Parser<T> Named<T>(this Parser<T> parser, string name)

        Parser<string> quotedText =
            (from open in Parse.Char('"')
                from content in Parse.CharExcept('"').Many().Text()
                from close in Parse.Char('"')
                select content).Named("quoted text");

        // This throws:
        //   unexpected 'f'; expected quoted text
        // instead of:
        //   unexpected 'f'; expected "
        Assert.Throws<ParseException>(() => quotedText.Parse("foo"));

        // [Sprache/src/Sprache/Parse.cs -- Named](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L459)
    }

    [Fact]
    public void TestEnd()
    {
        // Parse end-of-input.

        // Parser<T> End<T>(this Parser<T> parser)

        Assert.Equal("12", Parse.Number.End().Parse("12"));

        // unexpected '_'; expected end of input
        Assert.Throws<ParseException>(() => Parse.Number.End().Parse("12_"));

        // [Sprache/src/Sprache/Parse.cs -- End](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L336)
    }

    [Fact]
    public void TestNot()
    {
        // Constructs a parser that will fail if the given parser succeeds, and will succeed if the given parser fails. 
        // In any case, it won't consume any input. It's like a negative look-ahead in regex.

        // Parser<object> Not<T>(this Parser<T> parser)

        Parser<string> Keyword(string text) =>
            Parse.IgnoreCase(text).Then(n => Parse.Not(Parse.LetterOrDigit.Or(Parse.Char('_')))).Return(text);

        Parser<string> returnKeyword = Keyword("return");

        Assert.Equal("return", returnKeyword.Parse("return"));
        Assert.Throws<ParseException>(() => returnKeyword.Parse("return_"));
        Assert.Throws<ParseException>(() => returnKeyword.Parse("returna"));

        // [Sprache/src/Sprache/Parse.cs -- Not](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L216)
    }

    [Fact]
    public void TestExcept()
    {
        // Attempt parsing only if the parameter except parser fails.

        // Parser<T> Except<T, U>(this Parser<T> parser, Parser<U> except)

        const char Quote = '\'';
        const char OpenCurly = '{';
        const char CloseCurly = '}';
        const char Comma = ',';
        const char EqualSign = '=';

        Parser<char> validChars =
            Parse.AnyChar.Except(
                Parse.Chars(Quote, OpenCurly, CloseCurly, EqualSign, Comma).Or(Parse.WhiteSpace));

        Assert.Equal('t', validChars.Parse("t"));
        Assert.Throws<ParseException>(() => validChars.Parse(" "));

        // [Sprache/src/Sprache/Parse.cs -- Except](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L578C23-L578C86)
    }

    [Fact]
    public void TestThen()
    {
        // Parse first, and if successful, then parse second.
        // Returns the result of the second parser.

        // Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)

        Parser<string> identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        Parser<string[]> memberAccess =
            from first in identifier.Once()
            from subs in Parse.Char('.').Then(_ => identifier).Many()
            select first.Concat(subs).ToArray();

        Assert.Equal(["foo", "bar", "baz"], memberAccess.Parse("foo.bar.baz"));

        // [Sprache/src/Sprache/Parse.cs -- Then](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L241)
    }

    [Fact]
    public void TestWhere()
    {
        // Succeed if the parsed value matches predicate.

        // Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)

        Parser<int> parser = Parse.Number.Select(int.Parse).Where(n => n >= 100 && n < 200);

        Assert.Equal(151, parser.Parse("151"));

        // Unexpected 201.;
        Assert.Throws<ParseException>(() => parser.Parse("201"));

        // [Sprache/src/Sprache/Parse.cs -- Where](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L614)


        // Can also be used as part of a linq expression:

        string[] keywords = ["return", "var", "function"];

        Parser<string> identifier =
            from id in Parse.Identifier(Parse.Letter, Parse.LetterOrDigit.Or(Parse.Char('_')))
            where !keywords.Contains(id)
            select id;

        // Unexpected return.;
        Assert.Throws<ParseException>(() => identifier.Parse("return"));
    }

    [Fact]
    public void TestConcat()
    {
        // Concatenate two streams of elements.

        // Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second)

        Parser<string> identifierRule =
            (from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('_')).Many()
            select new string(first.Concat(rest).ToArray())).Named("identifier");

        Assert.Equal("my_variable1", identifierRule.Parse("my_variable1"));

        // [Sprache/src/Sprache/Parse.cs -- Concat](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L537)
    }

    [Fact]
    public void TestPreview()
    {
        // Construct a parser that indicates that the given parser is optional and non-consuming. The returned parser will succeed on any input no matter whether the given parser succeeds or not. In any case, it won’t consume any input, like a positive look-ahead in regex.

        // Parser<IOption<T>> Preview<T>(this Parser<T> parser)

        var parser =
            from name in Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Text()
            from hash in Parse.Char('#').Preview()
            from hashtag in Parse.AnyChar.Until(Parse.LineTerminator).Text()
            select new { NAME = name, HASHTAG = hashtag, HAS_HASH = !hash.IsEmpty };

        var result = parser.Parse("foo#bar123");
        Assert.Equal("foo", result.NAME);
        Assert.Equal("#bar123", result.HASHTAG);
        Assert.True(result.HAS_HASH);

        var result2 = parser.Parse("foo_bar123");
        Assert.Equal("foo", result2.NAME);
        Assert.Equal("_bar123", result2.HASHTAG);
        Assert.False(result2.HAS_HASH);

        // [Sprache/src/Sprache/Parse.Optional.cs -- Preview](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Optional.cs#L65)

        // [Sprache/src/Sprache/Option.cs -- IOption\#IsEmpty](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Option.cs#L14)
    }

    [Fact]
    public void TestSpan()
    {
        // Constructs a parser that returns the ITextSpan of the parsed value, which includes information about the position of the parsed value in the original source.

        // Parser<ITextSpan<T>> Span<T>(this Parser<T> parser)

        Parser<string> sample =
            from a in Parse.Char('a').Many().Text().Token()
            from b in Parse.Char('b').Many().Text().Token().Span()
            where b.Start.Pos <= 10
            select a + b.Value;

        Assert.Equal("aaabbb", sample.Parse(" aaa bbb "));
        Assert.Throws<ParseException>(() => sample.Parse(" aaaaaaa      bbbbbb "));

        // [Sprache/src/Sprache/Parse.Commented.cs -- Span](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Commented.cs#L30)
    }

}
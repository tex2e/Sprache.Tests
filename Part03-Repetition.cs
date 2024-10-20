
namespace Sprache.Tests;

public class Part03_RepetitionUnittest
{
    [Fact]
    public void TestMany()
    {
        // Parse a stream of 0 or more elements.

        // Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        Parser<string> quotedString =
            from open in Parse.Char('"')
            from value in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"')
            select value;

        Assert.Equal("Hello, World!", quotedString.Parse("\"Hello, World!\""));

        // The empty string is allowed, as many can match zero times
        Assert.Equal("", quotedString.Parse("\"\""));

        // [Sprache/src/Sprache/Parse.cs -- Many](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L256)
    }

    [Fact]
    public void TestXMany()
    {
        // Parse a stream of elements, failing if any element is only partially parsed.
        // The X* methods typically give more helpful errors and are easier to debug than their unqualified counterparts.

        // Parser<IEnumerable<T>> XMany<T>(this Parser<T> parser)

        // Single record e.g. "(monday)"
        Parser<string> record =
            from lparem in Parse.Char('(')
            from name in Parse.Letter.Many().Text()
            from rparem in Parse.Char(')')
            select name;

        string input = "(monday)(tuesday0(wednesday)(thursday)";

        Assert.Equal(["monday"], record.Many().Parse(input));

        // unexpected '('; expected end of input
        Assert.Throws<ParseException>(() => record.XMany().End().Parse(input));

        // unexpected '0'; expected )
        Assert.Throws<ParseException>(() => record.XMany().Parse(input));

        // [Sprache/src/Sprache/Parse.cs -- XMany](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L296)
    }

    [Fact]
    public void TestAtLeastOnce()
    {
        // TryParse a stream of elements with at least one item.

        // Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
        Parser<IEnumerable<string>> parser = Parse.String("Foo").Text().AtLeastOnce();

        Assert.Equal(["Foo", "Foo"], parser.Parse("FooFooBar"));

        // unexpected 'B'; expected Foo
        Assert.Throws<ParseException>(() => parser.Parse("Bar"));

        // [Sprache/src/Sprache/Parse.cs -- AtLeastOnce](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L309)
    }

    [Fact]
    public void TestXAtLeastOnce()
    {
        // TryParse a stream of elements with at least one item.
        // Except the first item, all other items will be matched with the XMany operator.

        // Parser<IEnumerable<T>> XAtLeastOnce<T>(this Parser<T> parser)
        Parser<IEnumerable<string>> parser = Parse.String("Foo").Text().XAtLeastOnce();

        Assert.Equal(["Foo", "Foo"], parser.Parse("FooFooBar"));

        // unexpected 'B'; expected Foo
        Assert.Throws<ParseException>(() => parser.Parse("Bar"));

        // [Sprache/src/Sprache/Parse.cs -- XAtLeastOnce](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L323)
    }

    [Fact]
    public void TestUntil()
    {
        // Parse a sequence of items until a terminator is reached.

        // Parser<IEnumerable<T>> Until<T, U>(this Parser<T> parser, Parser<U> until)
        Parser<string> parser =
            from first in Parse.String("/*")
            from comment in Parse.AnyChar.Until(Parse.String("*/")).Text()
            select comment;

        Assert.Equal("this is a comment", parser.Parse("/*this is a comment*/"));

        parser.Parse(
            @"/*
            This comment
            can span
            over multiple lines*/");

        // [Sprache/src/Sprache/Parse.cs -- Until](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L602)
    }

    [Fact]
    public void TestRepeat()
    {
        // Parses a sequence of items until a specific number have been parsed.

        // Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        Parser<string> parserRepeat3Times = Parse.Char('a').Repeat(3).Text();
        Assert.Equal("aaa", parserRepeat3Times.Parse("aaa"));
        Assert.Throws<ParseException>(() => parserRepeat3Times.Parse("aab"));

        // [Sprache/src/Sprache/Parse.Sequence.cs -- Repeat(int)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Sequence.cs#L77)


        // Parses a sequence of items until a specific number have been parsed.

        // Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int? minimumCount, int? maximumCount)
        Parser<string> parser = Parse.Digit.Repeat(3, 6).Text();

        Assert.Equal("123", parser.Parse("123"));
        Assert.Equal("123456", parser.Parse("123456"));
        // The parser will not consume more than the maximum
        Assert.Equal("123456", parser.Parse("123456789"));
        // Unexpected 'end of input'; expected 'digit' between 3 and 6 times, but found 2
        Assert.Throws<ParseException>(() => parser.Parse("12"));

        // [Sprache/src/Sprache/Parse.Sequence.cs -- Repeat(int, int)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Sequence.cs#L91)
    }

    [Fact]
    public void TestOnce()
    {
        // Parse a stream of elements containing only one item.

        // Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
        Parser<string> identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        Parser<IEnumerable<string>> memberAccess =
            from first in identifier.Once()
            from subs in Parse.Char('.').Then(_ => identifier).Many()
            select first.Concat(subs);

        Assert.Equal(["foo", "bar", "baz"], memberAccess.Parse("foo.bar.baz"));

        // [Sprache/src/Sprache/Parse.cs -- Once](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L523)
    }
}

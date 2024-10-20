
namespace Sprache.Tests;

public class Part06_DelimitedByUnittest
{
    [Fact]
    public void TestDelimitedBy()
    {
        // Parses elements matched by parser, delimited by elements matched by delimiter

        // Parser<IEnumerable<T>> DelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)

        Parser<string> typeReference = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        Parser<IEnumerable<string>> typeParameters =
            from open in Parse.Char('<')
            from elements in typeReference.DelimitedBy(Parse.Char(',').Token())
            from close in Parse.Char('>')
            select elements;

        Assert.Equal(["string"], typeParameters.Parse("<string>"));
        Assert.Equal(["string", "int"], typeParameters.Parse("<string, int>"));

        // unexpected ','; expected >
        Assert.Throws<ParseException>(() => typeParameters.Parse("<string,>"));

        // unexpected '>'; expected letter
        Assert.Throws<ParseException>(() => typeParameters.Parse("<>"));

        // [Sprache/src/Sprache/Parse.Sequence.cs -- DelimitedBy](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Sequence.cs#L34)


        // Note that trailing delimiters are not matched, and that the element must be matched at least once, which is where Optional comes in handy:

        Parser<IEnumerable<string>> array =
            from open in Parse.Char('[')
            from elements in Parse.Number.DelimitedBy(Parse.Char(',').Token()).Optional()
            from trailing in Parse.Char(',').Token().Optional()
            from close in Parse.Char(']')
            select elements.GetOrElse([]);

        Assert.Equal(["1", "2", "3"], array.Parse("[1, 2, 3]"));
        Assert.Equal(["1", "2"], array.Parse("[1, 2, ]"));
        Assert.Equal([], array.Parse("[]"));
    }

    [Fact]
    public void TestXDelimitedBy()
    {
        // Fails on the first itemParser failure, if it reads at least one character.

        // Parser<IEnumerable<T>> XDelimitedBy<T, U>(this Parser<T> itemParser, Parser<U> delimiter)
        Parser<IEnumerable<string>> numbers = Parse.Number.DelimitedBy(Parse.Char(',').Token());
        Parser<IEnumerable<string>> numbersX = Parse.Number.XDelimitedBy(Parse.Char(',').Token());

        Assert.Equal(["1", "2"], numbers.Parse("1, 2, "));
        Assert.Throws<ParseException>(() => numbersX.Parse("1, 2, "));

        Assert.Equal(["1", "2"], numbers.Parse("1, 2a, 3"));
        Assert.Equal(["1", "2"], numbersX.Parse("1, 2a, 3"));

        Assert.Equal(["1", "2"], numbers.Parse("1, 2 "));
        Assert.Throws<ParseException>(() => numbersX.Parse("1, 2 "));

        // [Sprache/src/Sprache/Parse.Sequence.cs -- XDelimitedBy](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Sequence.cs#L56)
    }
}
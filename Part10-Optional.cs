
namespace Sprache.Tests;

public class Part10_OptionalUnitest
{
    [Fact]
    public void TestOptional()
    {
        // Construct a parser that indicates that the given parser is optional. 
        // The returned parser will succeed on any input no matter whether the given parser succeeds or not.

        // Parser<IOption<T>> Optional<T>(this Parser<T> parser)

        Parser<string> identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        Parser<string> label =
            from labelName in identifier.Token()
            from colon in Parse.Char(':').Token()
            select labelName;

        Parser<Tuple<string, string[]>> instruction =
            from instructionName in Parse.LetterOrDigit.Many().Text().Token()
            from operands in identifier.Token().XDelimitedBy(Parse.Char(','))
            select Tuple.Create(instructionName, operands.ToArray());

        // Example of returning anonymous type from a sprache parser
        var assemblyLine =
            from l in label.Optional()
            from i in instruction.Optional()
            select new {Label = l, Instruction = i};

        Assert.Equal("test", assemblyLine.Parse("test: mov ax, bx").Label.Get());
        Assert.False(assemblyLine.Parse("mov ax, bx").Label.IsDefined);

        // [Sprache/src/Sprache/Parse.Optional.cs -- Optional](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Optional.cs#L16)
    }

    [Fact]
    public void TestXOptional()
    {
        // Constructs the eXclusive version of the Optional{T} parser.

        // Parser<IOption<T>> XOptional<T>(this Parser<T> parser)

        Parser<string> identifier = Parse.Identifier(Parse.Letter, Parse.LetterOrDigit);

        Parser<string> label =
            from labelName in identifier.Token()
            from colon in Parse.Char(':').Token()
            select labelName;

        // unexpected 'l'; expected :
        Assert.Throws<ParseException>(() => label.XOptional().Parse("invalid label:"));

        // [Sprache/src/Sprache/Parse.Optional.cs -- XOptional](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Optional.cs#L38)
    }
}
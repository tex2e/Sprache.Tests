
namespace Sprache.Tests;

public class Part02_ParsingStringsUnittest
{
    [Fact]
    public void TestString()
    {
        // Parse a string of characters.

        // Parser<IEnumerable<char>> String(string s)
        Parser<IEnumerable<char>> keywordReturn = Parse.String("return");
        Assert.Equal(['r', 'e', 't', 'u', 'r', 'n'], keywordReturn.Parse("return"));

        // [Sprache/src/Sprache/Parse.cs -- String](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L196)
    }

    [Fact]
    public void TestText()
    {
        // Convert a stream of characters to a string.

        // Parser<string> Text(this Parser<IEnumerable<char>> characters)
        Parser<string> keywordReturn = Parse.String("return").Text();
        Assert.Equal("return", keywordReturn.Parse("return"));

        Parser<string> parser = Parse.CharExcept(',').Many().Text();
        Assert.Equal("foo", parser.Parse("foo,bar"));

        // [Sprache/src/Sprache/Parse.cs -- Text](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L420)
    }

    [Fact]
    public void TestIgnoreCase()
    {
        // Parse a single character in a case-insensitive fashion.

        // Parser<char> IgnoreCase(char c)
        Parser<char> parseChar = Parse.IgnoreCase('a');
        Assert.Equal('a', parseChar.Parse("a"));
        Assert.Equal('A', parseChar.Parse("A"));

        // [Sprache/src/Sprache/Parse.cs -- IgnoreCase(char)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L129)

        // Parse a string in a case-insensitive fashion.

        // Parser<IEnumerable<char>> IgnoreCase(string s)
        Parser<string> sprach = Parse.IgnoreCase("sprache").Text();
        Assert.Equal("SprachE", sprach.Parse("SprachE"));

        // [Sprache/src/Sprache/Parse.cs -- IgnoreCase(string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L139)
    }

    [Fact]
    public void TestNumber()
    {
        // Parse a number.

        // Parser<string> Number = Numeric.AtLeastOnce().Text()
        Assert.Equal("123", Parse.Number.Parse("123"));

        // This parser only returns "1", as '.' is not considered numeric
        Assert.Equal("1", Parse.Number.Parse("1.23"));

        // [Sprache/src/Sprache/Parse.cs -- Number](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L763)
    }

    [Fact]
    public void TestDecimal()
    {
        // Parse a decimal number using the current culture's separator character.
        // フランスでは小数点にカンマ「,」を使用する

        // Parser<string> Decimal
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
        Assert.Equal("123,45", Parse.Decimal.Parse("123,45"));

        // [Sprache/src/Sprache/Parse.cs -- Decimal](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L782)
    }

    [Fact]
    public void TestDecimalInvariant()
    {
        // Parse a decimal number with separator '.'.

        // Parser<string> Decimal
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
        Assert.Equal("123.45", Parse.DecimalInvariant.Parse("123.45"));

        // [Sprache/src/Sprache/Parse.cs -- DecimalInvariant](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L787)
    }

    [Fact]
    public void TestLineEnd()
    {
        // Parses line endings (\n or \r\n).

        // Parser<string> LineEnd
        Assert.Equal("\r\n", Parse.LineEnd.Parse("\r\n"));
        Assert.Equal("\n", Parse.LineEnd.Parse("\n"));

        // Unexpected end of input reached; expected 
        Assert.Throws<ParseException>(() => Parse.LineEnd.Parse("\r"));

        // [Sprache/src/Sprache/Parse.Primitives.cs -- LineEnd](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Primitives.cs#L8)
    }

}

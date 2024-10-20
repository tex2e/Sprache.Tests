
namespace Sprache.Tests;

public class Part01_ParsingCharactersUnittest
{
    [Fact]
    public void TestChar()
    {
        // Parse a single character of any in c

        // Parser<char> Char(char c)
        Parser<char> multiply = Parse.Char('*');
        Assert.Equal('*', multiply.Parse("*"));

        // [Sprache/src/Sprache/Parse.cs -- Char(char)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L66C40-L66C41)

        // Parser<char> Char(Predicate<char> predicate, string description)
        Parser<char> punctuation = Parse.Char(char.IsPunctuation, "punctuation");
        Assert.Equal(',', punctuation.Parse(","));

        // [Sprache/src/Sprache/Parse.cs -- Char(Predicate\<char\>, string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L27)
    }

    [Fact]
    public void TestChars()
    {
        // Parse a single character of any in c

        // Parser<char> Chars(params char[] c)
        Parser<char> op = Parse.Chars('+', '-', '*', '/');
        Assert.Equal('-', op.Parse("-"));
        Assert.Equal('*', op.Parse("*"));

        // [Sprache/src/Sprache/Parse.cs -- Chars(params char\[\])](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L77)

        // Parser<char> Chars(string c)
        Parser<char> parens = Parse.Chars("()");
        Assert.Equal(')', parens.Parse(")"));

        // [Sprache/src/Sprache/Parse.cs -- Chars(string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L87)
    }

    [Fact]
    public void TestCharExcept()
    {
        // Parses a single character except for those in c

        // Parser<char> CharExcept(char c)
        Parser<char> parser1 = Parse.CharExcept('"');
        Assert.Equal('a', parser1.Parse("a"));
        Assert.Throws<ParseException>(() => parser1.Parse("\""));

        // [Sprache/src/Sprache/Parse.cs -- CharExcept(char)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L98)

        // Parser<char> CharExcept(IEnumerable<char> c)
        Parser<char> parser2 = Parse.CharExcept(['1', '2', '3']);
        Assert.Equal('4', parser2.Parse("4"));
        Assert.Throws<ParseException>(() => parser2.Parse("2"));

        // [Sprache/src/Sprache/Parse.cs -- CharExcept(IEnumerable\<char\>)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L108)

        // Parser<char> CharExcept(string c)
        Parser<char> parser3 = Parse.CharExcept("123");
        Assert.Equal('4', parser3.Parse("4"));
        Assert.Throws<ParseException>(() => parser3.Parse("2"));

        // [Sprache/src/Sprache/Parse.cs -- CharExcept(string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L119)

        // Parser<char> CharExcept(Predicate<char> predicate, string description)
        Parser<char> parser4 = Parse.CharExcept(char.IsPunctuation, "punctuation");
        Assert.Equal('a', parser4.Parse("a"));
        Assert.Throws<ParseException>(() => parser4.Parse("."));

        // [Sprache/src/Sprache/Parse.cs -- CharExcept(Predicate\<char\>, string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L56)
    }

    [Fact]
    public void TestIgnoreCase()
    {
        // Parse a single character in a case-insensitive fashion.

        // Parser<char> IgnoreCase(char c)
        Parser<char> parser = Parse.IgnoreCase('a');
        Assert.Equal('A', parser.Parse("A"));

        // [Sprache/src/Sprache/Parse.cs -- IgnoreCase(char)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L129)

        // Parse a string in a case-insensitive fashion.

        // Parser<IEnumerable<char>> IgnoreCase(string s)
        Parser<IEnumerable<char>> parserString = Parse.IgnoreCase("test");
        Assert.Equal(['T','e','s','T'], parserString.Parse("TesT"));

        // [Sprache/src/Sprache/Parse.cs -- IgnoreCase(string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L139)
    }

    [Fact]
    public void TestWhiteSpace()
    {
        // Parse a whitespace.

        // Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "whitespace")
        Assert.Equal(' ', Parse.WhiteSpace.Parse(" "));
        Assert.Equal('\t', Parse.WhiteSpace.Parse("\t"));

        // [Sprache/src/Sprache/Parse.cs -- WhiteSpace](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L159)
    }

    [Fact]
    public void TestDigit()
    {
        // Parse a digit.

        // Parser<char> Digit = Char(char.IsDigit, "digit")
        Assert.Equal('7', Parse.Digit.Parse("7"));

        // [Sprache/src/Sprache/Parse.cs -- Digit](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L164)
    }

    [Fact]
    public void TestNumeric()
    {
        // Parse a numeric character.

        // Parser<char> Numeric = Char(char.IsNumber, "numeric character")
        Assert.Equal('1', Parse.Numeric.Parse("1"));
        Assert.Equal('¼', Parse.Numeric.Parse("¼"));

        // [Sprache/src/Sprache/Parse.cs -- Numeric](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L189)
    }

    [Fact]
    public void TestLetter()
    {
        // Parse a letter.

        // Parser<char> Letter = Char(char.IsLetter, "letter")
        Assert.Equal('a', Parse.Letter.Parse("a"));

        // [Sprache/src/Sprache/Parse.cs -- Letter](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L169)
    }

    [Fact]
    public void TestLetterOrDigit()
    {
        // Parse a letter or digit.

        // LetterOrDigit = Char(char.IsLetterOrDigit, "letter or digit")
        Assert.Equal('a', Parse.LetterOrDigit.Parse("a"));
        Assert.Equal('4', Parse.LetterOrDigit.Parse("4"));

        // [Sprache/src/Sprache/Parse.cs -- LetterOrDigit](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L174)
    }

    [Fact]
    public void TestLower()
    {
        // Parse a lowercase letter.

        // Parser<char> Lower = Char(char.IsLower, "lowercase letter")
        Assert.Equal('a', Parse.Lower.Parse("a"));
        // unexpected '4'; expected lowercase letter
        Assert.Throws<ParseException>(() => Parse.Lower.Parse("4"));

        // [Sprache/src/Sprache/Parse.cs -- Lower](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L179)
    }

    [Fact]
    public void TestUpper()
    {
        // Parse an uppercase letter.

        // Parser<char> Upper = Char(char.IsUpper, "uppercase letter")
        Assert.Equal('A', Parse.Upper.Parse("A"));

        // [Sprache/src/Sprache/Parse.cs -- Upper](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L184)
    }

    [Fact]
    public void TestAnyChar()
    {
        // Parse any character.

        // Parser<char> AnyChar = Char(c => true, "any character")
        Assert.Equal('a', Parse.AnyChar.Parse("abcd"));

        // [Sprache/src/Sprache/Parse.cs -- AnyChar](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L154)
    }

}
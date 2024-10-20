
using System.Text.RegularExpressions;

namespace Sprache.Tests;

public class Part05_TransformUnittest
{
    [Fact]
    public void TestSelect()
    {
        // Take the result of parsing, and project it onto a different domain.

        // Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        Parser<int> number = Parse.Number.Select(int.Parse);
        Assert.Equal(12, number.Parse("12"));

        Parser<int> numberLambda = Parse.Number.Select(x => int.Parse(x));
        Assert.Equal(123, numberLambda.Parse("123"));

        var identifier =
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
            select new string(first.Concat(rest).ToArray());

        var tag =
            from lt in Parse.Char('<')
            from t in identifier
            from gt in Parse.Char('>').Token()
            select t;

        Assert.Equal("test", tag.Parse("<test>"));

        // [Sprache/src/Sprache/Parse.cs -- Select](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L357)
    }

    [Fact]
    public void TestReturn()
    {
        // Succeed immediately and return value.

        // Parser<T> Return<T>(T value)
        Parser<OperatorType> parser = Parse.String("*").Then(_ => Parse.Return(OperatorType.Mul));
        Assert.Equal(OperatorType.Mul, parser.Parse("*"));

        // [Sprache/src/Sprache/Parse.cs -- Return\<T\>(T)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L551)

        // Succeed immediately and return value with simpler inline syntax.

        // Parser<U> Return<T, U>(this Parser<T> parser, U value)
        Parser<OperatorType> add = Parse.String("+").Return(OperatorType.Add);
        Assert.Equal(OperatorType.Add, add.Parse("+"));

        // [Sprache/src/Sprache/Parse.cs -- Return\<T, U\>(this Parser\<T\>, U)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.cs#L564)

    }

    [Fact]
    public void TestRegex()
    {
        // Construct a parser from the given regular expression.

        // Parser<string> Regex(string pattern, string description = null)
        // Parser<string> Regex(Regex regex, string description = null)
        Parser<string> digits = Parse.Regex(@"\d(\d*)");

        Assert.Equal("123", digits.Parse("123d"));
        Assert.Throws<ParseException>(() => digits.Parse("d123"));

        // [Sprache/src/Sprache/Parse.Regex.cs -- Regex(string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Regex.cs#L14)

        // [Sprache/src/Sprache/Parse.Regex.cs -- Regex(Regex)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Regex.cs#L27)

    }

    [Fact]
    public void TestRegexMatch()
    {
        // Construct a parser from the given regular expression, returning a parser of type Match.

        // Parser<Match> RegexMatch(string pattern, string description = null)
        // Parser<Match> RegexMatch(Regex regex, string description = null)
        Parser<Match> digits = Parse.RegexMatch(@"(\d+)-(\d+)");

        Assert.Equal("123", digits.Parse("123-4567").Groups[1].Value);
        Assert.Equal("4567", digits.Parse("123-4567").Groups[2].Value);

        // [Sprache/src/Sprache/Parse.Regex.cs -- RegexMatch(string)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Regex.cs#L41)

        // [Sprache/src/Sprache/Parse.Regex.cs -- RegexMatch(Regex)](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/Parse.Regex.cs#L55)
    }
}


public enum OperatorType
{
    Add,
    Sub,
    Mul,
    Div
}

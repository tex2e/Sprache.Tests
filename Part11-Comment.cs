
namespace Sprache.Tests;

public class Part11_CommentUnitest
{
    [Fact]
    public void TestComment()
    {
        // Constructs customizable comment parsers.

        {
            // Parse a comment.

            // Parser<string> AnyComment

            var comment = new CommentParser("<!--", "-->", "\r\n");

            Assert.Equal(" Commented text ", comment.AnyComment.Parse("<!-- Commented text -->"));

            // No single-line comment header was defined, so this throws an exception
            Assert.Throws<ParseException>(() => comment.SingleLineComment);

            // [Sprache/src/Sprache/CommentParser.cs -- CommentParser](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/CommentParser.cs#L6)

            // [Sprache/src/Sprache/CommentParser.cs -- CommentParser#AnyComment](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/CommentParser.cs#L109)
        }

        {
            // Parse a multi-line comment.
            // Parse a single-line comment.

            // Parser<string> MultiLineComment
            // Parser<string> SingleLineComment

            // default is new CommentParser("//", "/*", "*/", "\n")
            var comment = new CommentParser();

            Assert.Equal("single-line comment", comment.SingleLineComment.Parse("//single-line comment"));
            Assert.Equal("multi-line comment", comment.MultiLineComment.Parse("/*multi-line comment*/"));

            // [Sprache/src/Sprache/CommentParser.cs -- CommentParser#SingleLineComment](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/CommentParser.cs#L72)

            // [Sprache/src/Sprache/CommentParser.cs -- CommentParser#MultiLineComment](https://github.com/sprache/Sprache/blob/9d1721bb0dea638e35b9bbb2334fea6f99bf778e/src/Sprache/CommentParser.cs#L89)
        }
    }
}

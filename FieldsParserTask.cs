using NUnit.Framework;
using System.Collections.Generic;

namespace TableParser
{
    [TestFixture]
    public class FieldParserTaskTests
    {
        public static void Test(string input, string[] expectedResult)
        {
            var actualResult = FieldsParserTask.ParseLine(input);
            Assert.AreEqual(expectedResult.Length, actualResult.Count);
            for (int i = 0; i < expectedResult.Length; ++i)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i].Value);
            }
        }

        [TestCase("text", new[] { "text" })]
        [TestCase("hello world", new[] { "hello", "world" })]
        [TestCase("hello  world", new[] { "hello", "world" })]
        [TestCase("h", new[] { "h" })]
        [TestCase("\"text\"", new[] { "text" })]
        [TestCase("'text'", new[] { "text" })]
        [TestCase("'text", new[] { "text" })]
        [TestCase("'text ", new[] { "text " })]
        [TestCase("'a''b'", new[] { "a", "b" })]
        [TestCase("\'a \\\'b\\\''", new[] { "a 'b'" })]
        [TestCase("\"a \'b\'\"", new[] { "a 'b'" })]
        [TestCase("\"a \\\"b\\\"\"", new[] { "a \"b\"" })]
        [TestCase("'\\\"a\\\"'", new[] { "\"a\"" })]
        [TestCase("'\\\\'", new[] { "\\" })]
        [TestCase("\\", new[] { "\\" })]
        [TestCase("''", new[] { "" })]
        [TestCase(" a ", new[] { "a" })]
        [TestCase("", new string[0])]
        [TestCase("a'abc'c", new[] { "a", "abc", "c" })]

        public static void RunTests(string input, string[] expectedOutput) => Test(input, expectedOutput);
    }

    public class FieldsParserTask
    {
        public static List<Token> ParseLine(string line)
        {
            var fields = new List<Token>();            
            for (int i = 0; i < line.Length; i = fields[fields.Count - 1].GetIndexNextToToken())
            {
                if (line[i] == ' ')
                {
                    while (line[i] == ' ')
                    {                        
                        i++;
                        if (i == line.Length)
                            return fields;
                    }                    
                }
                if (line[i] == '\'' || line[i] == '"')
                    fields.Add(ReadQuotedField(line, i));
                else
                    fields.Add(ReadField(line, i));
            }            
            return fields;
        }

        private static Token ReadField(string line, int startIndex)
        {

            int actualLength = line.IndexOfAny(new char[] { ' ', '\'', '"' }, startIndex) != -1
                ? line.IndexOfAny(new char[] { ' ', '\'', '"' }, startIndex) - startIndex
                : line.Length - startIndex;
            if (actualLength == 0)
                return null;
            line = line.Substring(startIndex, actualLength);
            return new Token(line, startIndex, actualLength);
        }

        public static Token ReadQuotedField(string line, int startIndex)
        {
            return QuotedFieldTask.ReadQuotedField(line, startIndex);
        }
    }
}
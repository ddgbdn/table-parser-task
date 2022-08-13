using NUnit.Framework;
using System.Linq;
using System;

namespace TableParser
{
    [TestFixture]
    public class QuotedFieldTaskTests
    {        
        [TestCase("''", 0, "", 2)]
        [TestCase("'a'", 0, "a", 3)] 
        [TestCase("\"abc\"", 0, "abc", 5)]
        [TestCase("b \"a'\"", 2, "a'", 4)]
        [TestCase(@"'a\' b'", 0, "a' b", 7)]
        [TestCase("\"", 0, "", 1)]
        [TestCase("'", 0, "", 1)]
        [TestCase("'\\\\'", 0, "\\", 4)]
        public void Test(string line, int startIndex, string expectedValue, int expectedLength)
        {
            var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
            Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
        }

        // Добавьте свои тесты
    }

    class QuotedFieldTask
    {
        public static Token ReadQuotedField(string line, int startIndex)
        {
            int length = GetLength(line, startIndex);
            if (length != -1)
                return new Token(RemoveSlashesAndQuotes(line, startIndex, length), startIndex, length);
            else
                return new Token(RemoveSlashesAndQuotes(line, startIndex, length), startIndex, line.Length - startIndex);
        }

        private static int GetLength(string line, int startIndex)
        {
            int searchIndex = startIndex + 1;
            int nextQuote;
            char quote = line[startIndex];
            
            while (true)
            {
                nextQuote = line.IndexOf(quote, searchIndex);
                if (nextQuote == -1)
                    return -1;
                if (line[nextQuote - 1] == '\\')
                {
                    int backSlashCount = 2;
                    while (true)
                    {
                        if (line[nextQuote - backSlashCount] == '\\')
                            backSlashCount++;
                        else
                            break;

                    }
                    if (backSlashCount % 2 == 0)
                    {
                        searchIndex = nextQuote + 1;
                        continue;
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return nextQuote - startIndex + 1;
        }
        
        private static string RemoveSlashesAndQuotes(string line, int startIndex, int length)
        {
            if (length != -1)
                line = line.Substring(startIndex + 1, length - 2);
            else
                line = line.Substring(startIndex + 1);

            int findIndex = 0;
            int nextBackSlash;

            while (findIndex < line.Length)
            {
                nextBackSlash = line.IndexOf("\\", findIndex);
                if (nextBackSlash == -1)
                    break;
                line = line.Remove(nextBackSlash, 1);
                findIndex = nextBackSlash + 2;
            }

            return line;
        }
    }
}

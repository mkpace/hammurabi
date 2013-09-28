// Copyright (c) 2012-2013 Hammura.bi LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AkkadianCompiler
{
    /// <summary>
    /// Processes metadata about questions.
    /// </summary>
    public class Questions
    {
        // List of all questions with metadata defined in the .akk files.
        public static List<Qdata> questionData = new List<Qdata>();

        /// <summary>
        /// Class for keeping track of question metadata.
        /// </summary>
        public class Qdata
        {
            public string relationship;
            public string questionType;
            public string questionText;
            public string filePath;
            public string fullMethod;
            public string param1Type;
            public string param2Type;
            public string param3Type;

            public Qdata(string rel, string type, string text, string file, 
                         string method, string param1, string param2, string param3)
            {
                relationship = rel;
                questionType = type;
                questionText = text;
                filePath = file;
                fullMethod = method;
                param1Type = param1;
                param2Type = param2;
                param3Type = param3;
            }
        }

        /// <summary>
        /// Extracts the question metadata from the lines.
        /// </summary>
        public static void GatherMetadata(string line, string previousLine, string fileName, string docNameSpace)
        {
            // Use a regex to identify the function parts
            const string wrd = @"[a-zA-Z0-9_]+";
            Match match = Regex.Match(line, 
                // In the following regex, <p#> = param, <c#> = comma, <a#> = arg
                @"(Tbool|Tnum|Tdate|Tstr|Tset)(In)?(Sym)?\?? (?<fcn>"+wrd+@")\((?<p1>"+wrd+@" ?)?(?<a1>"+wrd+@")?(?<c1>, ?)?(?<p2>"+wrd+@" )?(?<a2>"+wrd+@")?(?<c2>, ?)?(?<p3>"+wrd+@" )?(?<a3>"+wrd+@")?\)");

            if (match.Success)
            {
                // Extract basic relationship details
                string type = match.Groups[1].Value.Trim();
                string rel = match.Groups[4].Value.Trim();

                // Determine the question text (default to the relationship text)
                string questionText = rel;
                if (previousLine.Contains("# >>"))
                {
                    questionText = previousLine.Trim().Replace("# >>","").Trim();
                }

                // Determine the nature of the parameters
                string param1 = match.Groups[5].Value.Trim();
                string param2 = match.Groups[8].Value.Trim();
                string param3 = match.Groups[11].Value.Trim();

                // Get the name of the appropriate namespace
                string relName = Util.NamespaceConvert(docNameSpace) + rel;

                // Only get method-related data if it's a main rule and all parameters are Things.
                // This is because only rule conclusions will be used as interview starting points.
                if (Util.IsMainRule(line))
                {
                    // Details captured for the purpose of interview goals
                    string method = docNameSpace + "." + rel;
                    string file = fileName;

                    // Add metadata to the question list
                    questionData.Add(new Qdata(relName, type, questionText, file, method, param1, param2, param3));
                }
                // Capture metadata from embedded TvarIn statement
                else if (match.Groups[2].Value == "In")
                {
                    // Add metadata to the question list
                    questionData.Add(new Qdata(relName, type, questionText, null, null, null, null, null));
                }
            }
        }

        /// <summary>
        /// Write the metadata to .../Generated source/Question metadata.cs.
        /// </summary>
        public static void GenerateMetadataFile(string filePath)
        {
            // Open the question template class
            string result = 
                "using System; \r\nusing System.Collections.Generic; \r\nusing Hammurabi; \r\nnamespace Interactive { \r\npublic class Templates {" +
                "\r\npublic static Question GetQ(string rel) { \r\nswitch(rel) { \r\n";

            // Keep track of what has already been added to the file
            string questionsSoFar = ",";

            // Write the metadata for each question
            foreach(Questions.Qdata q in Questions.questionData)
            {
                // Determine if question has already been written
                bool alreadyHave = questionsSoFar.Contains("," + q.relationship + ",");

                // Write it...
                if (!alreadyHave)
                    result += "case \"" + q.relationship + "\": return new Question(rel, \"" + q.questionType + "\", \"" + q.questionText + "\", \"\", @\"" +
                        q.filePath + "\",\"" + q.fullMethod + "\",\"" + q.param1Type + "\",\"" + q.param2Type + "\",\"" + q.param3Type + "\"" +
                        FuncText(q) + ");\r\n";
  
                // Add it to the list of questions that have already been written
                questionsSoFar += q.relationship + ",";
            }

            // Close out the question template class
            result += "\r\ndefault: return new Question(rel, \"bool\", rel, \"\"); }}}}";

            // Write the string to a file
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "Question metadata.cs");
            file.WriteLine(result);
            file.Close();
        }

        /// <summary>
        /// Creates the first-order function text to write to the metadata file.
        /// </summary>
        private static string FuncText(Questions.Qdata q)
        {
            // If no method is provided, don't create a '()=>Fcn' string
            if (q.fullMethod == null) return ", null";

            string result = ",()=>" + q.fullMethod + "(";
            if (q.param1Type != "") result += "Engine." + q.param1Type + "1";
            if (q.param2Type != "") result += ",Engine." + q.param2Type + "2";
            if (q.param3Type != "") result += ",Engine." + q.param3Type + "3";
            return result + ")";
        }
    }
}
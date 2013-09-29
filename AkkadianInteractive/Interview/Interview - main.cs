// Copyright (c) 2012 Hammura.bi LLC
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
using Hammurabi;
using Interactive;

namespace Akkadian.DosInteractive
{
    /// <summary>
    /// Generates a DOS-based, utterly primitive interview designed
    /// to illustrate how Hammurabi can simulate backwards chaining.
    /// </summary>
    public static partial class Interview 
    {
        /// <summary>
        /// Assesses the state of asserted facts and then decides what to do next.
        /// </summary>
        public static void ProcessRequest(string request) 
        {
            Facts.Fact goalblob = ParseRequest(request);
            InitializeSession();

            while (true)
            {
                // Get the response object from the interview engine
                Engine.Response response = Engine.Investigate(new List<Facts.Fact>(){goalblob});

                // Ask the current question, or display the results
                if (!response.InvestigationComplete)
                {
                    // Ask the next question
                    DisplayQuestion(response); 

                    // Get and validate the answer
                    GetAndParseAnswer(response);
                } 
                else
                {
                    DisplayResults(goalblob);
                    break;
                }
            }

//             Display all facts that have been asserted (for diagnostic purposes)
//             Console.WriteLine("\nFacts: \n" + Facts.AssertedFacts());
        }

        /// <summary>
        /// Parses the interview goal request.
        /// </summary>
        private static Facts.Fact ParseRequest(string request)
        {
            // Request format: <goal> <Thing1> <Thing2>? <Thing3>?
            // Example: IsUSCitizen Jane
            string[] req = request.Split(' ');
            string t1 = req.Length > 1 ? req[1] : "";
            string t2 = req.Length > 2 ? req[2] : "";
            string t3 = req.Length > 3 ? req[3] : "";

            // Set values for the initial Things
            // This is necessary for test case generation
            Engine.Thing1 = new Thing(t1);
            Engine.Thing2 = new Thing(t2);
            Engine.Thing3 = new Thing(t3);

            return new Facts.Fact(req[0], t1, t2, t3);
        }

        /// <summary>
        /// Sets/resets data structures for new interview session.
        /// </summary>
        public static void InitializeSession()
        {
            // Clear fact base and assert any assumed facts
            Facts.Clear();
            AssertPreliminaryFacts();    

            // Initialize the .akk unit test text string
            AkkTest.testStr = "";
            AkkTest.InitializeUnitTest();
        }

        /// <summary>
        /// Asserts facts (for testing or pre-seeding purposes).
        /// </summary>
        private static void AssertPreliminaryFacts()   
        {
            // Put any facts to be asserted here, like this:
//            Thing A = Facts.AddThing("A");
//            Facts.Assert(A,"Sandbox.Gender","Female");
        }
        
        /// <summary>
        /// Displays a question to the user.
        /// </summary>
        private static void DisplayQuestion(Engine.Response response)  
        {
            // Get data about the next question
            Facts.Fact theFact = response.NextFact;
            Question theQ = Templates.GetQ(theFact.Relationship);

            // TODO: Display appropriate question control (using theQ.questionType)

            // White space
            Console.WriteLine();

            // Display progress percentage
            Console.WriteLine("Percent complete: " + response.PercentComplete);

            // Display question text
            string qText = theFact.QuestionText(); // QuestionText(theFact, theQ);
            Console.WriteLine(qText);
            AkkTest.assertedRelationship = AkkTest.AddUnitTestAssertRel(theFact, theQ);

            // Display an explanation (if any) 
            if (theQ.explanation != "")
            {
                Console.WriteLine("Note: " + theQ.explanation);
            }
        }

        /// <summary>
        /// Gets the and validates the user's answer to a question.
        /// </summary>
        private static void GetAndParseAnswer(Engine.Response response)
        {
            // Get data pertaining to the current question
            string currentRel = response.NextFact.Relationship;
            Question currentQuestion = Templates.GetQ(currentRel);
            string currentQType = currentQuestion.questionType;

            // Read (and gently massage) the answer
            string answer = Console.ReadLine();
            answer = CleanBooleans(currentQType, answer);

            // Validate answer, then assert it
            if (AnswerIsValid(currentQuestion, answer))
            {
                AssertAnswer(response, answer);
            }
            else
            {
                GetAndParseAnswer(response);
            }
        }

        /// <summary>
        /// Displays the engine's results of the interview session.
        /// </summary>
        private static void DisplayResults(Facts.Fact goal)
        {
            Console.WriteLine("\n");

            // Indent and format results
            string tline = "\t" + goal.ValueAsString().Replace("\n","\n\t");

            // For eternal values, only show value
            if (goal.Value().IsEternal)
            {
                tline = tline.Replace("DawnOfTime   ","");
            }

            // Concatenate question and answer
            string result = "\t" + goal.QuestionText() + "\n\n" + tline + "\n";
            
            // Add result to test case
            Tvar testResult = goal.GetFunction().Invoke();
            AkkTest.CloseUnitTest(testResult, goal.Relationship);

            Console.WriteLine(result);
        }

        /// <summary>
        /// Converts free-text DOS answers into valid booleans, if applicable.
        /// </summary>
        private static string CleanBooleans(string currentQType, string input)
        {
            string i = input.Trim();

            if (currentQType == "Tbool" && !input.StartsWith("{"))
            {
                i = i.ToLower();
                if (i == "t" || i == "yes" || i == "y") return "true";
                if (i == "f" || i == "no" || i == "n") return "false";
            }

            return i;
        }
    }
}
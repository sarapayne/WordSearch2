﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch
{
    class Validation
    {
        private static List<Letter>[,] validationArray;
        private static List<Word> words;
        static int collumIndexes;
        static int collumIndex;
        static int rowIndexes;
        static int rowIndex;
        static int numberOfWords;

        /// <summary>
        /// takes the provided user input and validates it against the availible options. If the choice is not valid it will ask the user to try again. 
        /// </summary>
        /// <param name="input">This is what the user typed into the termrminal</param>
        /// <returns>any user input which is found to be valid. </returns>
        public static string InitialMenu(string input)
        {
            while (input != "1" && input != "2")
            {
                Console.WriteLine("Sorry but the value: " + input + " which you entered is not valid. Your entry must be 1 or 2, please try again");
                input = Console.ReadLine();
            }
            return input;
        }

        /// <summary>
        /// Takes the supplied file contents and uses that to check if game can be loaded, then outputs the objects needed to build the gameFile object. Inluding refusal reasons. 
        /// </summary>
        /// <param name="fileContents">Contents of the file .wrd file loaded from the disc</param>
        /// <param name="wordsOut">A list of all the word objects to be contained in this gameFile object</param>
        /// <param name="boardDimensions">a 2d vector object used to store the array sidze of the 2 dimensional array needed for this game file.</param>
        /// <param name="rejectReason">If the file can not be loaded, this is used to tell the user why it can not be loaded.</param>
        /// <returns></returns>
        public static bool GameFile(string[] fileContents, out List<Word> wordsOut, out Vector boardDimensions, out string rejectReason)
        {
            words = new List<Word>();
            string fileRejectReason = "";
            string[] lineSegments;
            bool fileOk = true; //set to true then if it changes to false at any time break the loop
            bool isIntRows;
            bool isIntCollums;
            bool isIntWords;
            bool matchingFileType;
            int rowIndexes = 0; ;
            int collumIndexes = 0;
            int numberOfWords = 0;
            int lineIndex = 0;
            while (fileOk && lineIndex < fileContents.Length)
            {
                if (lineIndex == 0)
                {
                    lineSegments = fileContents[lineIndex].Split(",");
                    if (lineSegments.Length != 3)
                    {
                        fileRejectReason = "1st Line or later does not contain 3 elements as a CSV";
                        fileOk = false; //drop strait out of the while loop
                    }
                    try
                    {
                        isIntRows = int.TryParse(lineSegments[0], out rowIndexes);
                        isIntCollums = int.TryParse(lineSegments[1], out collumIndexes);
                        isIntWords = int.TryParse(lineSegments[2], out numberOfWords);
                        matchingFileType = true;
                    }
                    catch
                    {
                        matchingFileType = false;
                        isIntRows = false;
                        isIntCollums = false;
                        isIntWords = false;
                    }
                    if(rowIndexes < 1)
                    {   //reversed row/collum names for user perspective. 
                        fileRejectReason = "Board has no collums";
                        fileOk = false;
                    }
                    if (collumIndexes < 1 )
                    {   //reversed row/collum names for user perspective.
                        fileRejectReason = "Board has no rows";
                        fileOk = false;
                    }
                    if (numberOfWords <1 )
                    {
                        fileRejectReason = "Board has no words";
                        fileOk = false;
                    }
                    if (!(isIntCollums && isIntRows && isIntWords))
                    {
                        fileRejectReason = "Board size integers are not integers in the file";
                        fileOk = false;
                    }
                    if (fileContents.Length != (numberOfWords + 1))
                    {
                        fileRejectReason = "Number of words reported does not match the number of lines in the file";
                        fileOk = false;
                    }
                    if (!matchingFileType)
                    {
                        fileRejectReason = "File is not a text file. This file is likely a renamed bin or library file.";
                        fileOk = false;
                    }
                    validationArray = new List<Letter>[rowIndexes+1, collumIndexes+1];
                }//close line inxex is 0
                else
                {
                    lineSegments = fileContents[lineIndex].Split(",");
                    if (lineSegments.Length == 4)
                    {
                        bool isIntRow = int.TryParse(lineSegments[1], out rowIndex);
                        bool isIntCol = int.TryParse(lineSegments[2], out collumIndex);
                        int numIndexesToCheck = (lineSegments[0].Length) - 1;//remove one as we are already at first index. 
                        rowIndex++;//add 1 to row and collum indexes to allow for formatting rows and collums
                        collumIndex++; //add 1 to row and collum indexes to allow for formatting rows and collums
                        if (!(isIntRow && isIntCol))
                        {
                            fileRejectReason = "Row or Collum provided is not an integer";
                            fileOk = false;
                            break;
                        }
                        else if (lineSegments[3] == "left")
                        {
                            if (rowIndex - numIndexesToCheck < 0)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds left";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "right")
                        {
                            if (rowIndex + numIndexesToCheck > rowIndexes)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds right";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "up")
                        {
                            if (collumIndex - numIndexesToCheck < 0)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds up";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "down")
                        {
                            if (collumIndex + numIndexesToCheck > collumIndexes)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds down";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "leftup")
                        {
                            if (rowIndex - numIndexesToCheck < 1 || collumIndex - numIndexesToCheck < 1)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds leftup";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "rightup")
                        {
                            if (rowIndex + numIndexesToCheck > rowIndexes || collumIndex - numIndexesToCheck < 1)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds rightup";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "leftdown")
                        {
                            if (rowIndex - numIndexesToCheck < 1 || collumIndex + numIndexesToCheck > collumIndexes)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds leftdown";
                                fileOk = false;
                                break;
                            }
                        }
                        else if (lineSegments[3] == "rightdown")
                        {
                            if (rowIndex + numIndexesToCheck > rowIndexes || collumIndex + numIndexesToCheck > collumIndexes)
                            {
                                fileRejectReason = lineSegments[0] + " Index out of bounds rightdown";
                                fileOk = false;
                                break;
                            }
                        }
                        else
                        { //if its none of the above there is a problem, file not ok
                            fileRejectReason = "malformed direction";
                            fileOk = false;
                            break;
                        }
                    }
                    else
                    {
                        fileRejectReason = "1 or more of the word lines does not have 4 elements as CSV";
                        fileOk = false;
                        break;
                    }
                    List<Letter> wordObjects = GenerateWordObjects(rowIndex, collumIndex, lineSegments[0], lineSegments[3]);
                    bool wordPlacementOk = TestLettersOnBoard(wordObjects, lineSegments[0], out string reject, out int rowReject, out int colReject); //adds object AND returns the bool based on results in the method
                    if (wordPlacementOk)
                    {
                        Word word = new Word(lineSegments[0], wordObjects, false);
                        words.Add(word);
                    }
                    else
                    {
                        fileRejectReason = "Crossing Words at (" + rowReject + "," + colReject + ") incompatible.";
                        fileOk = false;
                        break;
                    }
                    if (!fileOk)
                    {
                        break;
                    }
                }//close else not line 1
                lineIndex++;
            }//close while file ok and line index is less than limit
            wordsOut = words;
            boardDimensions = new Vector(validationArray.GetLength(0), validationArray.GetLength(1));
            rejectReason = fileRejectReason;
            return fileOk;
        }//close GameFile


        /// <summary>
        /// Generates a list of Letter objects which form the Word object which is later added to a list as well which will form a property of the GameFile object. 
        /// </summary>
        /// <param name="firstRowIndex">row position of the first letter of the word</param>
        /// <param name="firstCollumIndex">collum position of the first letter of the word</param>
        /// <param name="word">A string of the word the contained objects make</param>
        /// <param name="direction">a string showing the direction the word is displayed</param>
        /// <returns></returns>
        private static List<Letter> GenerateWordObjects(int firstRowIndex, int firstCollumIndex, string word, string direction)
        {
            List<Letter> wordObjects = new List<Letter>();
            for (int charIndex = 0; charIndex < word.Length; charIndex++)
            {
                Letter letter = new Letter();
                letter.Direction = direction;
                letter.Word = word;
                string displayChar = Convert.ToString(word[charIndex]);
                letter.Character = displayChar;
                if (charIndex == 0)
                {
                    letter.WordStart = true;
                }
                else
                {
                    letter.WordStart = false;
                }
                if (charIndex == word.Length - 1)
                {
                    letter.WordEnd = true;
                }
                else
                {
                    letter.WordEnd = false;
                }
                Vector position = new Vector();
                if (direction == "left")
                {
                    position.Row = firstRowIndex - charIndex;
                    position.Collum = firstCollumIndex;
                }
                else if (direction == "right")
                {
                    position.Row = firstRowIndex + charIndex;
                    position.Collum = firstCollumIndex;
                }
                else if (direction == "up")
                {
                    position.Row = firstRowIndex;
                    position.Collum = firstCollumIndex - charIndex;
                }
                else if (direction == "down")
                {
                    position.Row = firstRowIndex;
                    position.Collum = firstCollumIndex + charIndex;
                }
                else if (direction == "leftup")
                {
                    position.Row = firstRowIndex - charIndex;
                    position.Collum = firstCollumIndex - charIndex;
                }
                else if (direction == "rightup")
                {
                    position.Row = firstRowIndex + charIndex;
                    position.Collum = firstCollumIndex - charIndex;
                }
                else if (direction == "leftdown")
                {
                    position.Row = firstRowIndex - charIndex;
                    position.Collum = firstCollumIndex + charIndex;
                }
                else if (direction == "rightdown")
                {
                    position.Row = firstRowIndex + charIndex;
                    position.Collum = firstCollumIndex + charIndex;
                }
                letter.Positon = position;
                wordObjects.Add(letter);
            }//close loop through characters in the word
            return wordObjects;
        }

        /// <summary>
        /// Takes the supplied list of letter objects. Then loops through each one checking to see if an object already exists in the array in the same position. If there are any objects already in that position it will then test to see if they have the same letter. If they do not it will return false, otherwise true. 
        /// </summary>
        /// <param name="wordObjects">a list of the letter objects used to make up the word being checked</param>
        /// <param name="wordName">simple string to show the word being checked</param>
        /// <param name="reject">the reason the word has been rejected if it has</param>
        /// <param name="rowReject">int showing the row position of the rejected letter</param>
        /// <param name="colReject">int showing the collum position of the rejected letter</param>
        /// <returns></returns>
        private static bool TestLettersOnBoard(List<Letter> wordObjects, string wordName, out string reject, out int rowReject, out int colReject)
        {
            string rejectWord = "";
            int colRefused = 0;
            int rowRefused = 0;
            bool completedSuccessfully = true;
            int wordIndex = 0;
            while (completedSuccessfully && wordIndex < wordObjects.Count)
            {
                int rowIndex = wordObjects[wordIndex].Positon.Row;
                int colIndex = wordObjects[wordIndex].Positon.Collum;
                int rowIndexes = validationArray.GetLength(0);
                int colIndexes = validationArray.GetLength(1);
                if (validationArray[rowIndex, colIndex] == null)
                {
                    List<Letter> cellList = new List<Letter>();
                    cellList.Add(wordObjects[wordIndex]);
                    validationArray[wordObjects[wordIndex].Positon.Row, wordObjects[wordIndex].Positon.Collum] = cellList;
                }
                else
                {
                    List<Letter> cellList = validationArray[wordObjects[wordIndex].Positon.Row, wordObjects[wordIndex].Positon.Collum];
                    for (int cellListIndex = 0; cellListIndex < cellList.Count; cellListIndex++)
                    {
                        if (cellList[cellListIndex].Character != wordObjects[wordIndex].Character)
                        {
                            rejectWord = wordName;
                            rowRefused = rowIndex;
                            colIndex = colRefused;
                            completedSuccessfully = false;
                        }
                    }
                    cellList.Add(wordObjects[wordIndex]);
                }
                wordIndex++;
            }
            reject = rejectWord;
            rowReject = rowRefused;
            colReject = colRefused;
            return completedSuccessfully;
        }

        /// <summary>
        /// Take a string input form the user, ensures it is a valid integer and that it is within the desired range. If it is returns the integer value. 
        /// </summary>
        /// <param name="input">String user puts into the terminal</param>
        /// <param name="lowest">lowest allowed value</param>
        /// <param name="highest">highest allowed value</param>
        /// <returns>The integer value the user entered as a string.</returns>
        public static int CheckIntInRange(string input, int lowest, int highest)
        {
            bool isInt = int.TryParse(input, out int number);
            bool valid = isInt && (number >= lowest) && (number <= highest);
            while (!valid)
            {
                if (!isInt)
                {
                    Console.WriteLine("Sorry but your entry of: " + input + "was not a whole number, please enter a valid file number");
                }
                if (!(number >= lowest && number <= highest))
                {
                    Console.WriteLine("Sorry but your entry of: " + input + "was not a file number in existence. Please choose from numbers between " + lowest + " and " + highest + " inclusive");
                }
                Console.WriteLine("Please Press any key to return to the menu choices");
                Console.ReadKey();
                Program.wordSearch.InitialMenu();
            }
            return number;
        }

        /// <summary>
        /// Takes the user input, then makes sure it is valid by ensuring it is an in range integer. If it is not then the user is prompted to try again. 
        /// </summary>
        /// <param name="userInput">This is what the user typed into terminal</param>
        /// <param name="maxIndex">The highest choice availible.</param>
        /// <returns>any valid inputput supplied by the user. </returns>
        public int InGameMenu(string userInput, int maxIndex)
        {
            bool isInt = int.TryParse(userInput, out int number);
            while (!(isInt && number >=0 && number <= maxIndex))
            {
                Console.WriteLine("Sorry but your entry was invalid, please enter value again");
                userInput = Console.ReadLine();
                isInt = int.TryParse(userInput, out number);
            }
            return number;
        }

    }
}



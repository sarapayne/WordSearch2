﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch
{
    class WordSearch
    {
        private Board board;
        private Validation validation;
        private static List<Word> wordsInCurrentGame;
        private Storage storage;
        private int gameIndex;

        ConsoleColor textColour;
        private Vector lastWrongStartIndex;
        private Vector lastWrongEndIndex;
        
        public WordSearch()
        {
            lastWrongStartIndex = new Vector();
            lastWrongEndIndex = new Vector();
            validation = new Validation();
            wordsInCurrentGame = new List<Word>();
            storage = new Storage();
            InitialMenu();
            gameIndex = new int();
            DisplayWordChoices();
            InGameMenu();
        }

        public Storage Storage
        {
            get { return this.Storage; }
            set { this.Storage = value; }
        }

        private void InGameMenu()
        {
            //from user prespective rowIndexs are actually collum numbers and collumIndex are actually row numbers
            //from user perspective row and collum 0 is actually index1 in the array. So the maximum value is two less than the array length
            Console.WriteLine("Please Enter a start collum");
            string userInput = Console.ReadLine();
            int startRowIndex = validation.InGameMenu(userInput, board.BoardArray.GetLength(0)-2);
            startRowIndex++; // actual index will be one higher than the user enters
            Console.WriteLine("Please Enter a start row");
            userInput = Console.ReadLine();
            int startColIndex = validation.InGameMenu(userInput, board.BoardArray.GetLength(1) - 2);
            startColIndex++; //actual index will be one higher than the user enters. 
            Console.WriteLine("Please Enter the end collum");
            userInput = Console.ReadLine();
            int endRowIndex = validation.InGameMenu(userInput, board.BoardArray.GetLength(0) - 2);
            endRowIndex++; // actual index will be one higher than the user enters
            Console.WriteLine("Please Enter the end row");
            userInput = Console.ReadLine();
            int endColIndex = validation.InGameMenu(userInput, board.BoardArray.GetLength(1) - 2);
            endColIndex++; //actual index will be one higher than the user enters.
            Vector startCell = new Vector(startRowIndex, startColIndex);
            Vector endCell = new Vector(endRowIndex, endColIndex);
            bool wordIsFound = CheckIfWordFound(startRowIndex, startColIndex, endRowIndex, endColIndex, out string wordFound);
            board.UpdateBoardArrayCellRange(lastWrongStartIndex, lastWrongEndIndex, ConsoleColor.White);
            if (wordIsFound)
            {
                UpdateStausInsideGameWordsList(wordFound);
                CheckIfAllWordsFound();
                board.UpdateBoardArray(startRowIndex, startColIndex, wordFound, true, false);
                RefreshDisplay();
            }
            else
            {
                Console.WriteLine("Sorry but that did not match any words in the list, please press a key to continue and try again");
                Console.ReadKey();
                board.UpdateBoardArrayCellRange(startCell, endCell, ConsoleColor.Red);
                lastWrongStartIndex = startCell;
                lastWrongEndIndex = endCell;
                RefreshDisplay();
            }
        }

        private void RefreshDisplay()
        {
            Console.Clear();
            board.DisplayBoard();
            DisplayWordChoices();
            InGameMenu();
        }

        private void CheckIfAllWordsFound()
        {
            int wordsIndex = 0;
            bool winnerFound = true;
            while (winnerFound && wordsIndex < wordsInCurrentGame.Count)
            {
                if (!wordsInCurrentGame[wordsIndex].Found)
                {
                    winnerFound = false;
                }
                wordsIndex++;
            }
            if (winnerFound)
            {
                Console.WriteLine("Congratuations you have found all words in the current game. Press any key to return to the main menu.");
                Console.ReadKey();
                Program.Startup();
            }
        }

        private void UpdateStausInsideGameWordsList(string wordFound)
        {
            foreach (Word word in wordsInCurrentGame)
            {
                if (word.Name == wordFound)
                {
                    word.Found = true;
                    break;
                }
            }
        }

        private bool CheckIfWordFound(int startRowIndex, int startCollumIndex, int endRowIndex, int endColIndex, out string word)
        {
            bool wordFound = false;
            word = "";
            List<Letter> startCellContents = board.BoardArray[startRowIndex, endColIndex];
            List<Letter> endCellContents = board.BoardArray[endRowIndex, endColIndex];
            for (int startListIndex = 0; startListIndex < startCellContents.Count; startListIndex++)
            {
                for (int endListIndex = 0; endListIndex < endCellContents.Count; endListIndex++)
                {
                    string startWord = startCellContents[startListIndex].Word;
                    bool isStartWord = startCellContents[startListIndex].WordStart;
                    string endWord = endCellContents[endListIndex].Word;
                    bool isEndWord = endCellContents[endListIndex].WordEnd;
                    if (isStartWord && isEndWord)
                    {
                        if (startWord == endWord)
                        {
                            word = startWord;
                            wordFound =  true;
                        }
                    }
                }
            }
            return wordFound;
        }

        public static void AddWordToGameWordsList(Word word)
        {
            Word newWord = new Word();
            newWord = word;
            wordsInCurrentGame.Add(newWord);
        }

        private void DisplayWordChoices()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Words To Find");
            foreach (Word word in wordsInCurrentGame)
            {
                if (word.Found)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine(word.Name);
            }
            Console.ResetColor();
        }

        public void InitialMenu()
        {
            wordsInCurrentGame = new List<Word>();
            Console.Clear();
            Console.WriteLine("Select An Option \n" +
                "1. Use default wordsearch \n" +
                "2. Load wordsearch from file \n" +
                "\n" +
                "Enter a number from 1 to 2 inclusive");
            string menuChoice = Console.ReadLine();
            menuChoice = Validation.InitialMenu(menuChoice);
            GameFile gameFileToLoad = LoadGameChoice(menuChoice);
            wordsInCurrentGame = new List<Word>();
            board = new Board(gameFileToLoad);
        }

        private GameFile LoadGameChoice(string menuChoice)
        {
            GameFile gameToLoad = new GameFile();
            if (menuChoice == "1")
            {
                bool gameFile1Valid = storage.GameFiles[0].Validated;
                if (gameFile1Valid)
                {   //we know this is the same as file index0, so if its valid save some leg work. 
                    gameToLoad = storage.DefaultGameFile;
                } 
            }
            else if (menuChoice == "2")
            {
                gameIndex = FileChoiceMenu();
                gameToLoad = storage.GameFiles[gameIndex];
            }
            return gameToLoad;
        }

        private int FileChoiceMenu()
        {
            Console.Clear();
            Console.Write("Select a file to load, games shown in ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("blue ");
            Console.ResetColor();
            Console.Write("are saved but have errors and can not be loaded");
            Console.WriteLine();
            for (int filesIndex = 0; filesIndex < storage.GameFiles.Count; filesIndex++)
            {
                Console.Write(filesIndex + 1 + ". ");
                if (!(storage.GameFiles[filesIndex].Validated))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.Write(storage.GameFiles[filesIndex].Name);
                if (storage.GameFiles[filesIndex].RejectReason != "")
                {
                    Console.Write(": " + storage.GameFiles[filesIndex].RejectReason);
                }
                Console.WriteLine();
                Console.ResetColor();
            }
            Console.ResetColor();
            Console.WriteLine("\n" +
                "Enter a number from: 1 to " + storage.GameFiles.Count + " inclusive");
            int userChoice = (Validation.CheckIntInRange(Console.ReadLine(), 1, storage.GameFiles.Count));
            bool validFile = storage.GameFiles[userChoice - 1].Validated;
            if (!validFile)
            {
                Console.WriteLine("Sorry but the file choice you chose has an error, please press any key to return to the main menu then select a choice in white");
                Console.ReadKey();
                InitialMenu();
            }
            return userChoice;

        }
    }
}




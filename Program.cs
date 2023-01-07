using System;
using System.Collections.Generic;

namespace LR3
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var userOne = Creator.CreateClassicAccount("Kolia");
            var userTwo = Creator.CreateVipAccount("Olia");
            // var GameOne = Creator.PlayGameWithRating(UserOne, UserTwo, 10);
            // var GameTwo = Creator.PlayGameWithoutRating(UserOne, UserTwo);
            // GameOne.PlayGame();
            // GameTwo.PlayGame();
            // Console.Write(UserOne.GetHistory());
            // Console.Write(UserTwo.GetHistory());
            // Console.Write(UserOne.PlayerInformation());
            // Console.Write(UserTwo.PlayerInformation());
            var gameTicTacToe = new TicTacToe(userOne, userTwo, 20);
            gameTicTacToe.PlayGame();
            Console.Write(userOne.PlayerInformation());
            Console.Write(userTwo.PlayerInformation());
        }
        public class SetHistory
        {
            public int Rating { get; }
            public string Status { get; }
            public string OpponentName { get; }
            public int Index { get; }

            public SetHistory(int rating, string status, string opponentName, int index)
            {
                Rating = rating;
                Status = status;
                OpponentName = opponentName;
                Index = index;
            }
        }
        public class ClassicAccount
        {
            protected List<SetHistory> MyHistory = new List<SetHistory>();
            
            public string UserName { get; }
            public string UserAccount { get; }
            public int CurrentRating { get; set; }
            public ClassicAccount(string userName)
            {
                UserName = userName;
                UserAccount = GetType().Name;
                CurrentRating = 500;
            }
            public virtual void WinGame(string opponentName, BaseGame baseGame)
            {
                CurrentRating += baseGame.GameRating;
                SetHistory game = new SetHistory(baseGame.GameRating, "Winner", opponentName, 1);
                MyHistory.Add(game);
            }
            public virtual void LoseGame(string opponentName, BaseGame baseGame)
            {
                if (CurrentRating - baseGame.GameRating < 1)
                {
                    throw new InvalidOperationException($"The {UserName} lost");
                }
                CurrentRating -= baseGame.GameRating;
                SetHistory game = new SetHistory(baseGame.GameRating, "Loser", opponentName, 1);
                MyHistory.Add(game);
            }
            public virtual void DrawGame(string opponentName, BaseGame baseGame)
            {
                SetHistory game = new SetHistory(baseGame.GameRating, "Friendship won", opponentName, 1);
                MyHistory.Add(game);
            }
            public string GetHistory()
            {
                Console.WriteLine($"History games player - {UserName}:");
                var report = new System.Text.StringBuilder();
                int index = 0;
                foreach (var game in MyHistory)
                {
                    index += game.Index;
                    report.AppendLine(
                        $"GAME: {index}|{UserName} vs {game.OpponentName}|Rating: {game.Rating}|Status for {UserName}: {game.Status}");
                }
                return report.ToString();
            }
            public string PlayerInformation()
            {
                Console.WriteLine("Information about player:");
                var report = new System.Text.StringBuilder();
                report.AppendLine($"Player: {UserName}, Rating: {CurrentRating}, Account: {UserAccount}");
                return report.ToString();
            }
        }
        
        public class VipAccount: ClassicAccount
        {
            public VipAccount(string userName) : base(userName)
            {
            }
            public override void WinGame(string opponentName, BaseGame baseGame)
            {
                CurrentRating += baseGame.GameRating * 2;
                SetHistory game = new SetHistory(baseGame.GameRating * 2, "Winner(VIP)", opponentName, 1);
                MyHistory.Add(game);
            }
            public override void LoseGame(string opponentName, BaseGame baseGame)
            {
                if (CurrentRating - baseGame.GameRating < 1)
                {
                    throw new InvalidOperationException($"The {UserName} lost");
                }
                CurrentRating -= baseGame.GameRating / 2;
                SetHistory game = new SetHistory(baseGame.GameRating / 2, "Loser(VIP)", opponentName, 1);
                MyHistory.Add(game);
            }
        }
        public abstract class BaseGame
        {
            protected readonly ClassicAccount PlayerOne;
            protected readonly ClassicAccount PlayerTwo;
            public int GameRating { get; }
            protected BaseGame(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating)
            {
                if (gameRating <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(gameRating), "The rating must be greater than 0");
                }
                PlayerOne = playerOne;
                PlayerTwo = playerTwo;
                GameRating = gameRating;
            }
            protected BaseGame(ClassicAccount playerOne, ClassicAccount playerTwo)
            {
                PlayerOne = playerOne;
                PlayerTwo = playerTwo;
                GameRating = 0;
            }
            public abstract void PlayGame();
        }
        public class GameWithRating: BaseGame
        {
            public GameWithRating(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating) : base(playerOne, playerTwo, gameRating)
            {
            }
            public int Result { get; set; }
            public override void PlayGame()
            {
                Random rnd = new Random();
                Result = rnd.Next(1, 3);
                switch (Result)
                {
                    case 1:
                        PlayerOne.WinGame(PlayerTwo.UserName, this);
                        PlayerTwo.LoseGame(PlayerOne.UserName, this);
                        break;
                    case 2:
                        PlayerTwo.WinGame(PlayerOne.UserName, this);
                        PlayerOne.LoseGame(PlayerTwo.UserName, this);
                        break;
                }
            }
        } 
        public class GameWithoutRating: BaseGame
        {
            public GameWithoutRating(ClassicAccount playerOne, ClassicAccount playerTwo) : base(playerOne, playerTwo)
            { 
            }
            public int Result { get; set; }
            public override void PlayGame()
            {
                Random rnd = new Random();
                Result = rnd.Next(1, 3);
                switch (Result)
                {
                    case 1:
                        PlayerOne.WinGame(PlayerTwo.UserName, this);
                        PlayerTwo.LoseGame(PlayerOne.UserName, this);
                        break;
                    case 2:
                        PlayerTwo.WinGame(PlayerOne.UserName, this);
                        PlayerOne.LoseGame(PlayerTwo.UserName, this);
                        break;
                }
            }
        }

        public class TicTacToe : BaseGame
        {
            char[] Array = new char[9] { '-', '-', '-', '-','-','-','-','-','-'};
            private static string win;
            public TicTacToe(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating) : base(playerOne, playerTwo, gameRating)
            {
            }

            public override void PlayGame()
            { 
                Board1(); //поле гри
                Console.Write("Enter numbers from 1-9:\n");
                for (int i = 0; i <= 9 ; i++)
                {
                   Board(Array); //поле гри
                   if (i%2 == 0) {
                       Console.Write(PlayerTwo.UserName + " your turn\n" );
                       int move = Convert.ToInt32(Get_Move());
                       Array[move - 1] = '0';
                       if (Victory_check(Array) == "x")
                       {
                           PlayerOne.WinGame(PlayerTwo.UserName, this);
                           PlayerTwo.LoseGame(PlayerOne.UserName, this);
                           return;
                       }
                       if (Victory_check(Array) == "o")
                       {
                           PlayerOne.LoseGame(PlayerTwo.UserName, this);
                           PlayerTwo.WinGame(PlayerOne.UserName, this);
                           return;
                       }
                   }else 
                   {
                       Console.Write(PlayerOne.UserName + " your turn\n" );
                       int move = Convert.ToInt32(Get_Move());
                       Array[move - 1] = 'X';
                       if (Victory_check(Array) == "x")
                       {
                           PlayerOne.WinGame(PlayerTwo.UserName, this);
                           PlayerTwo.LoseGame(PlayerOne.UserName, this);
                           return;
                       }
                       if (Victory_check(Array) == "o")
                       {
                           PlayerOne.LoseGame(PlayerTwo.UserName, this);
                           PlayerTwo.WinGame(PlayerOne.UserName, this);
                           return;
                       }
                   }
                }
                Friendship_won();
                PlayerOne.DrawGame(PlayerTwo.UserName, this);
                PlayerTwo.DrawGame(PlayerOne.UserName, this);
            }

            private void Board1() //поле гри, нумерація клітинок
            {
                Console.WriteLine("Game field: (each cell has a number)");
                Console.WriteLine("\t-7-|-8-|-9-");
                Console.WriteLine("\t-4-|-5-|-6-");
                Console.WriteLine("\t-1-|-2-|-3-");
            }
            private void Board(char[] Array) //поле гри
            {
                Console.WriteLine("\t-"+Array[6]+"-|-"+Array[7]+"-|-"+Array[8]+"-" + "\t-7-|-8-|-9-");
                Console.WriteLine("\t-"+Array[3]+"-|-"+Array[4]+"-|-"+Array[5]+"-" + "\t-4-|-5-|-6-");
                Console.WriteLine("\t-"+Array[0]+"-|-"+Array[1]+"-|-"+Array[2]+"-" + "\t-1-|-2-|-3-");
            }
            private string Get_Move() //хід гравця
            {
                var move = Console.ReadLine();
                return move;
            }
            public string Victory_check(char[] Array)  //перевірка на виграш
            {
              if (Array[0] == 'X' && Array[1] == 'X' && Array[2] == 'X' ||
                  Array[3] == 'X' && Array[4] == 'X' && Array[5] == 'X' ||
                  Array[6] == 'X' && Array[7] == 'X' && Array[8] == 'X' ||
                  Array[0] == 'X' && Array[4] == 'X' && Array[8] == 'X' ||
                  Array[6] == 'X' && Array[4] == 'X' && Array[3] == 'X' ||
                  Array[0] == 'X' && Array[3] == 'X' && Array[6] == 'X' ||
                  Array[1] == 'X' && Array[4] == 'X' && Array[7] == 'X' ||
                  Array[2] == 'X' && Array[5] == 'X' && Array[8] == 'X' 
                 )
              {
                Board(Array);
                Game_over();
                return "x";
              }
              if (Array[0] == '0' && Array[1] == '0' && Array[2] == '0' ||
                  Array[3] == '0' && Array[4] == '0' && Array[5] == '0' ||
                  Array[6] == '0' && Array[7] == '0' && Array[8] == '0' ||
                  Array[0] == '0' && Array[4] == '0' && Array[8] == '0' ||
                  Array[6] == '0' && Array[4] == '0' && Array[3] == '0' ||
                  Array[0] == '0' && Array[3] == '0' && Array[6] == '0' ||
                  Array[1] == '0' && Array[4] == '0' && Array[7] == '0' ||
                  Array[2] == '0' && Array[5] == '0' && Array[8] == '0' 
                 )
              {
                Board(Array);
                Game_over();
                return "o";
              }
              return "nixto";
            }
            private void Game_over() //кінець гри
            {
                Console.WriteLine("game over | GG WP");
            }

            private void Friendship_won() //перевірка не нічию
            {
              if (win != "X - won" && win != "0 - won")
              {
                Console.WriteLine("Friendship won");
                Game_over();
              }
            }
            
        }
        public class Creator
        {
            public static BaseGame PlayGameWithRating(ClassicAccount playerOne, ClassicAccount playerTwo, int gameRating)
            {
                return new GameWithRating(playerOne, playerTwo, gameRating);
            }
            public static BaseGame PlayGameWithoutRating(ClassicAccount playerOne, ClassicAccount playerTwo)
            {
                return new GameWithoutRating(playerOne, playerTwo);
            }
            public static ClassicAccount CreateClassicAccount(string userName)
            {
                return new ClassicAccount(userName);
            }
            public static VipAccount CreateVipAccount(string userName)
            {
                return new VipAccount(userName);
            }
        }
    }
}

#nullable disable
using static System.Console;
using static Tools.Console;
using static System.Convert;
using static System.Math;
using System.Collections.Generic;
using System;

namespace BasketballGame
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Start run\n");

            try
            {
                char matchType = ToChar(Input("Введите тип матча (c - командный, d - дуэль): "));

                int memberQuantity;
                if (matchType == 'c') { memberQuantity = ToInt32(Input("Введите общее количесвто участников: ")); }
                else { memberQuantity = 2; }

                int matchQuantity = ToInt32(Input("Введите количество таймов: "));
                if (matchQuantity == 0) { WriteLine("Победила дружба!"); System.Environment.Exit(1); }

                string team_1 = Input("Введите название 1 команды: ");
                string team_2 = Input("Введите название 2 команды: ");

                bool debugMode;
                Write("Нужно ли показывать шансы? ");
                switch (ReadLine().ToLower())
                {
                    case "true" or "t" or "y" or "yes" or "да" or "д": { debugMode = true; break; }
                    case "false" or "f" or "n" or "no" or "нет" or "н" or _: { debugMode = false; break; }
                }
                Game game = new Game(
                teamTitles: new string[2] { team_1, team_2 },
                type: matchType,
                membersQuantity: memberQuantity,
                matchesQuantity: matchQuantity,
                debugMode: debugMode
                );
                WriteLine();
                game.Match();
            }
            catch (System.FormatException)
            {
                WriteLine("Вы что-то не так введи, попробуйсте в следующий раз.");
            }
        }
    }
    class Game
    {
        private char type; // d - duel, c - comand
        private bool debugMode;
        private int membersQuantity;
        private int SetMembersQuantity
        {
            set
            {
                this.membersQuantity = this.type switch
                {
                    'd' => 2,
                    'c' or _ => value
                };
            }
        }
        private int matchesQuantity;
        private int SetMatchesQuantity
        {
            set
            {
                this.matchesQuantity = this.type switch
                {
                    'd' => 1,
                    'c' or _ => value
                };
            }
        }
        private string[] teamTitles = new string[2];
        public Game(string[] teamTitles, char type = 'd', int membersQuantity = 12, int matchesQuantity = 4, bool debugMode = false)
        {
            this.teamTitles = teamTitles;
            this.type = type;
            this.SetMembersQuantity = membersQuantity;
            this.SetMatchesQuantity = matchesQuantity;
            this.debugMode = debugMode;
        }

        public void Match()
        {
            Team leftTeam = new((int)(this.membersQuantity / 2), this.teamTitles[0]);
            Team rightTeam = new((int)(this.membersQuantity / 2 + this.membersQuantity % 2), this.teamTitles[1]);

            leftTeam.GetTeamComposition(this.debugMode);
            WriteLine("");
            rightTeam.GetTeamComposition(this.debugMode);
            WriteLine("1... 2... 3... Начали!");
            for (int i = 0; i < this.matchesQuantity; i++) //таймы
            {
                WriteLine($"Тайм {i + 1} начался");
                for (int j = 0; j < 10; j++) // минуты
                {
                    WriteLine($"Пошла {j + 1} минута");
                    leftTeam.Confrontation();
                    rightTeam.Confrontation();
                }
                WriteLine($"Тайм {i + 1} закончился");
            }
            WriteLine($"Итоговый счёт:\nкоманда {leftTeam.teamTitle}, счёт - {leftTeam.teamScore}\nкоманда {rightTeam.teamTitle}, счёт - {rightTeam.teamScore}");
            string victoryTeam = (leftTeam.teamScore > rightTeam.teamScore) ? leftTeam.teamTitle : rightTeam.teamTitle;
            WriteLine($"Победу одержала команда {victoryTeam}");
        }
    }
    class Team
    {
        public int teamScore = 0;
        public string teamTitle;
        private int memberQuantity;
        private List<Player> teamComposition = new();

        public Team(int memberQuantity, string teamTitle)
        {
            this.teamTitle = teamTitle;
            this.memberQuantity = memberQuantity;
            for (int i = 0; i < this.memberQuantity; i++) AddPlayer();
        }

        private void AddPlayer()
        {
            this.teamComposition.Add(PlayerGenerator.Generate());
        }

        public void Confrontation()
        {
            int playerIndex = new Random().Next(0, memberQuantity - 1);
            teamComposition[playerIndex].ComandThrowAttempt(ref this.teamScore, teamTitle);
        }

        public void GetTeamComposition(bool debugMode = false)
        {
            WriteLine($"Команда {this.teamTitle}");
            WriteLine("Игроки команды:");
            if (!debugMode)
            {
                foreach (Player player in this.teamComposition) WriteLine(player.BasketCard);
            }
            else
            {
                foreach (Player player in this.teamComposition) WriteLine(player.FullBasketCard);
            }
        }

    }
    static class PlayerGenerator
    {
        private static string[] names = { "Jerry", "Gregory", "Terry", "Raymond", "Eileen", "James", "Dorothy", "Julia", "Laura", "Joshua", "Mary", "Judith", "Carmen", "Jose", "Michael", "Armando", "Henry", "Dorothy", "Kathy", "Joanne", "Michael", "Rita", "Deborah", "Agnes", "Kristin", "Tonya", "Carol", "Allan", "Richard", "Keith", "Joe", "Elizabeth", "Alexander", "Christopher", "Robert", "Troy", "Herbert", "Willie", "Rose", "Carrie", "Victor", "Derek", "Heidi", "John", "Carol", "Priscilla", "Audrey", "Joan", "Sherri" };
        private static Random random = new();

        public static Player Generate()
        {
            string name = names[random.Next(0, names.Length - 1)];
            int age = random.Next(3, 100);
            float height = random.Next(46, 250);
            float weight = random.Next(10, 150);
            float basketExperience = random.Next(0, 10);
            return new Player
            (
                name: name,
                age: age,
                height: height,
                weight: weight,
                basketExperience: basketExperience
            );
        }
    }
    class Player
    {
        private string name;
        private int age;
        private float height;
        private float weight;
        private float basketExperience; // стаж в годах

        private enum Influence //Влияние на BasketSkill в процентах
        {
            Age = 15,
            Height = 20,
            Wight = 20,
            Experience = 45
        }

        public string BasketCard
        {
            get { return $"Игрок: {this.name}\nВозраст: {this.age} лет\nРост: {this.height} см.\nВес: {this.weight} кг.\nСтаж игры (года): {this.basketExperience}\n"; }
        }
        public string FullBasketCard
        {
            get { return $"Игрок: {this.name}\nВозраст: {this.age} лет | +{GetChangeByProperty("age")}%\nРост: {this.height} см. | +{GetChangeByProperty("height")}%\nВес: {this.weight} кг. | +{GetChangeByProperty("weight")}%\nСтаж игры (года): {this.basketExperience} | +{GetChangeByProperty("be")}%\nСумарный шанс на попадание: {GetChangeByProperty("sum")}%\n"; }
        }
        private float Age
        {
            get
            {
                return this.age switch
                {
                    <= 35 => (float)this.age / 35 * (int)Influence.Age,
                    > 35 => 36 / (float)this.age * (int)Influence.Age
                };
            }
        }
        private float Height
        {
            get
            {
                return this.height / 125 * ((int)Influence.Height / 2);
            }
        }
        private float Weight
        {
            get
            {
                return this.weight switch
                {
                    <= 50 => this.weight / 50 * (int)Influence.Wight,
                    > 50 => 50 / this.weight * (int)Influence.Wight,
                    _ => (int)Influence.Wight
                };
            }
        }
        private float BasketExperience
        {
            get
            {
                float skillInfluence = (float)(Sqrt(this.basketExperience) / PI * (int)Influence.Experience);
                return (skillInfluence > (float)Influence.Experience) ? (float)Influence.Experience : skillInfluence;
            }
        }
        private float BasketSkill
        {
            get
            {
                return Age + Height + Weight + BasketExperience;
            }
        }

        public Player(string name, int age, float height, float weight, float basketExperience = 0)
        {
            this.name = name;
            this.age = age;
            this.height = height;
            this.weight = weight;
            this.basketExperience = basketExperience;
        }

        public double GetChangeByProperty(string property)
        {
            float chance = property switch
            {
                "age" => Age,
                "height" => Height,
                "weight" => Weight,
                "basket experience" or "be" => BasketExperience,
                "sum" => BasketSkill,
                _ => 0.0f
            };
            return Round((double)chance, 2);
        }
        public bool ThrowAttempt()
        {
            Random chance = new();
            return chance.Next(0, 100) < BasketSkill;
        }

        public void ComandThrowAttempt(ref int teamScore, string teamTitle)
        {
            bool throwResult = ThrowAttempt();
            teamScore += ToInt32(throwResult);
            string prefix = (!throwResult) ? " не " : " ";
            WriteLine($"{this.name} из команды \"{teamTitle}\"{prefix}попал в кольцо");
        }
    }
}

namespace Tools
{
    static class Console // Pythonish
    {
        public static string Input(string promt = "")
        {
            if (promt != "") { Write(promt); }
            string input = ReadLine();
            return input;
        }
    }
}

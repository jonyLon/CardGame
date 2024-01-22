using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
using static System.Reflection.Metadata.BlobBuilder;

namespace CardGame
{



    class Card : IComparable<Card>
    {
        public int suit{ get; set; }
        public int Type { get; set; }

        private string[] name = { "HEARTS", "DIAMONDS", "SPADES", "CLUBS" };
        private string[] nameof = { "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "JACK", "QUEEN", "KING", "ACE" };

        public Card() {
            this.suit = -1;
            Type = -1;
        }
        public Card(int suit, int type)
        {
            this.suit = suit;
            Type = type;
        }
        public override string ToString()
        {
            return $"{nameof[Type]} of {name[suit]}";
        }
        public int CompareTo(Card? other)
        {
            return this.Type.CompareTo(other.Type);
        }

    }

    class Player
    {
        public List<Card> cards = new List<Card>();
        public string name;
        public Card Last { get; set; }
        public Player(string name) {
            this.name = name;
        }
        public void GetCard(Card card)
        {
            this.cards.Add(card);
        }
        public void GetCards(params Card[] cards)
        {
            this.cards.AddRange(cards);
        }
        public void PrintCards()
        {
            foreach (var item in cards)
            {
                Console.WriteLine(item.ToString());
            }
            Console.WriteLine();
        }
        public Card ShowCard()
        {
            Card card = cards[0];
            Last = card;
            cards.RemoveAt(0);
            return card;
        }


    }

    class Game
    {
        public List<Player> players = new List<Player>();
        public List<Card> deckOfCards = new List<Card>();

        public Game(params Player[] players)
        {
            if (players.Length < 2)
            {
                throw new ArgumentException("Provide at least 2 players");
            }
            this.players.AddRange(players);
            FormDeck();
        }

        protected void FormDeck()
        {
            foreach (var item in genetateCard())
            {
                deckOfCards.Add(item);
            }
        }

        protected IEnumerable<Card> genetateCard()
        {
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>() {
                [0] = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                [1] = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                [2] = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                [3] = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }
            };
            List<Card> cards = new List<Card>();
            foreach (var item in dict)
            {
                foreach (var type in item.Value)
                {
                    Card c = new Card(item.Key, type);
                    cards.Add(c);
                }
            }

            foreach (var item in cards)
            {
                yield return item;
            }
        }

        public void ShuffleDeck()
        {

            var shuffled = deckOfCards.OrderBy(_ => Guid.NewGuid()).ToList();
            deckOfCards = shuffled;
        }

        public void PrintDeck() {
            foreach (var item in deckOfCards)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void GiveCards()
        {
            ShuffleDeck();
            ShuffleDeck();
            int amount = 36/players.Count;
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    players[j].GetCard(deckOfCards[deckOfCards.Count-1]);
                    deckOfCards.RemoveAt(deckOfCards.Count-1);
                }
            }


        }

        public void Play()
        {
            List<Card> turn = new List<Card>();
            foreach (var pl in players)
            {
                Card newcard = pl.ShowCard();
                Console.Write($"{newcard.ToString(), -20} {pl.cards.Count}\t\t\t");
                turn.Add(newcard);
            }
            turn.Sort();
            Card max = turn[turn.Count - 1];
            Console.Write($"{max, -20} - ");
            foreach (var pl in players)
            {
                if (pl.Last == max)
                {
                    Console.Write(pl.name);
                    pl.GetCards(turn.ToArray());                    
                }
            }
            turn.Clear();

            Console.WriteLine();
        }
        public void StartGame()
        {
            foreach (var pl in players)
            {
                Console.Write($"Player {pl.name, -10} {pl.cards.Count}\t\t\t");
            }
            Console.WriteLine();
            while (StopGame())
            {
                Play();
            }
            Console.WriteLine("================== Game over ==================");
            win();

        }
        public void win()
        {
            foreach (var item in players)
            {
                if (item.cards.Count == 36)
                {
                    Console.WriteLine(item.name + " is winer!!");
                }
                Console.WriteLine(item.name + " Cards count: " + item.cards.Count);

            }

        }
        public bool StopGame()
        {
            return players.FindAll(s => s.cards.Count > 0).Count >= 2;
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Player p1 = new Player("Vlad");
            Player p2 = new Player("Kolia");

            Game game = new Game(p1,p2);
            game.GiveCards();
            game.StartGame();
        }
    }
}
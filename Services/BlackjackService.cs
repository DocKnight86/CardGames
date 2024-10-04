namespace CardGames.Services
{
    public class BlackjackService
    {
        private List<Card> deck;
        private List<Card> playerHand;
        private List<Card> dealerHand;
        private bool dealerHandRevealed;

        public BlackjackService()
        {
            InitializeGame();
        }

        public void InitializeGame()
        {
            deck = GenerateDeck();
            playerHand = new List<Card>();
            dealerHand = new List<Card>();
            dealerHandRevealed = false;

            // Deal initial cards
            playerHand.Add(DrawCard());
            playerHand.Add(DrawCard());
            dealerHand.Add(DrawCard());  // Visible card
            dealerHand.Add(DrawCard());  // Hidden card
        }

        public void PlayerStands()
        {
            int playerValue = CalculateHandValue(playerHand);
            int dealerValue = CalculateHandValue(dealerHand);

            // Reveal dealer's second card
            RevealDealerHand();

            // Dealer will stand if their value is >= 17, otherwise they draw a card
            while (dealerValue < 17)
            {
                dealerHand.Add(DrawCard());
                dealerValue = CalculateHandValue(dealerHand);
            }

            if (dealerValue > 21)
            {
                // Dealer busts, player wins
                Console.WriteLine("Dealer busts! Player wins.");
            }
            else if (dealerValue >= playerValue)
            {
                // Dealer wins if their hand is greater than or equal to the player's hand
                Console.WriteLine("Dealer wins.");
            }
            else
            {
                // Player wins if their hand is higher than the dealer's hand
                Console.WriteLine("Player wins.");
            }
        }

        public void RevealDealerHand()
        {
            dealerHandRevealed = true;
        }

        public List<Card> GetVisibleDealerHand()
        {
            if (dealerHandRevealed)
            {
                return dealerHand;
            }

            // Only return the first card when the hand is not revealed
            return new List<Card> { dealerHand[0] };
        }

        private List<Card> GenerateDeck()
        {
            var deck = new List<Card>();
            var suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
            var values = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    deck.Add(new Card(suit, value));
                }
            }

            var random = new Random();
            return deck.OrderBy(x => random.Next()).ToList();
        }

        public Card DrawCard()
        {
            var card = deck.First();
            deck.Remove(card);

            // Debug
            Console.WriteLine(card);

            return card;
        }

        public List<Card> GetPlayerHand() => playerHand;

        public List<Card> GetDealerHand() => dealerHand;

        public int CalculateHandValue(List<Card> hand)
        {
            int totalValue = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                if (int.TryParse(card.Value, out int cardValue))
                {
                    totalValue += cardValue;
                }
                else if (card.Value == "A")
                {
                    aceCount++;
                    totalValue += 11; // Ace initially counts as 11
                }
                else
                {
                    totalValue += 10; // J, Q, K are worth 10
                }
            }

            // Adjust for Aces if totalValue exceeds 21
            while (totalValue > 21 && aceCount > 0)
            {
                totalValue -= 10; // Count Ace as 1 instead of 11
                aceCount--;
            }

            return totalValue;
        }

        public virtual bool IsGameOver() => CalculateHandValue(playerHand) >= 21 || CalculateHandValue(dealerHand) >= 21;
    }

    public class Card
    {
        public string Suit { get; }
        public string Value { get; }

        public Card(string suit, string value)
        {
            Suit = suit;
            Value = value;
        }

        public override string ToString() => $"{Value} of {Suit}";
    }
}

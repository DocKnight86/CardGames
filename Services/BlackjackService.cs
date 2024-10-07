namespace CardGames.Services
{
    public class BlackjackService
    {
        private List<Card> _deck;
        private List<Card> _playerHand;
        private List<Card> _dealerHand;
        private bool _dealerHandRevealed;

        public void InitializeGame()
        {
            _deck = GenerateDeck();
            _playerHand = new List<Card>();
            _dealerHand = new List<Card>();
            _dealerHandRevealed = false;

            // Deal initial cards
            _playerHand.Add(DrawCard());
            _playerHand.Add(DrawCard());
            _dealerHand.Add(DrawCard());  // Visible card
            _dealerHand.Add(DrawCard());  // Hidden card
        }

        public void PlayerStands()
        {
            int playerValue = CalculateHandValue(_playerHand);
            int dealerValue = CalculateHandValue(_dealerHand);

            // Reveal dealer's second card
            RevealDealerHand();

            // Dealer will stand if their value is >= 17, otherwise they draw a card
            while (dealerValue < 17)
            {
                _dealerHand.Add(DrawCard());
                dealerValue = CalculateHandValue(_dealerHand);
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
            _dealerHandRevealed = true;
        }

        public List<Card> GetVisibleDealerHand()
        {
            if (_dealerHandRevealed)
            {
                return _dealerHand;
            }

            // Only return the first card when the hand is not revealed
            return new List<Card> { _dealerHand[0] };
        }

        private List<Card> GenerateDeck()
        {
            List<Card> deck = new List<Card>();
            string[] suits = new[] { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] values = new[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (string suit in suits)
            {
                foreach (string value in values)
                {
                    deck.Add(new Card(suit, value));
                }
            }

            Random random = new Random();
            return deck.OrderBy(x => random.Next()).ToList();
        }

        public Card DrawCard()
        {
            Card card = _deck.First();
            _deck.Remove(card);

            return card;
        }

        public List<Card> GetPlayerHand() => _playerHand;

        public List<Card> GetDealerHand() => _dealerHand;

        public int CalculateHandValue(List<Card> hand)
        {
            int totalValue = 0;
            int aceCount = 0;

            foreach (Card card in hand)
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

        public virtual bool IsGameOver() => CalculateHandValue(_playerHand) >= 21 || CalculateHandValue(_dealerHand) >= 21;
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

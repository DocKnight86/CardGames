﻿using Microsoft.AspNetCore.SignalR;
using CardGames.Hubs;

namespace CardGames.Services
{
    public class BlackjackService
    {
        private List<PlayingCard> _deck;
        private List<PlayingCard> _playerHand;
        private List<PlayingCard> _dealerHand;
        private bool _dealerHandRevealed;
        private readonly IHubContext<GameHub> _hubContext; // Injecting SignalR Hub

        public BlackjackService(IHubContext<GameHub> hubContext) // Constructor receives SignalR Hub
        {
            _hubContext = hubContext;
        }

        public void InitializeGame()
        {
            _deck = GenerateDeck(6); // Generate a deck of 6 decks of cards
            _playerHand = new List<PlayingCard>();
            _dealerHand = new List<PlayingCard>();
            _dealerHandRevealed = false;

            // Ensure we always have a valid hand before accessing it
            if (_deck.Count >= 4) // Checking if there are enough cards in the deck to deal
            {
                _playerHand.Add(DrawCard());
                _playerHand.Add(DrawCard());

                _dealerHand.Add(DrawCard());  // Visible card
                _dealerHand.Add(DrawCard());  // Hidden card
            }
        }

        public List<PlayingCard> GetDealerHand()
        {
            return _dealerHand ?? new List<PlayingCard>(); // Ensure it returns an empty list if _dealerHand is null
        }

        public List<PlayingCard> GetPlayerHand() => _playerHand;

        public void RevealDealerHand()
        {
            _dealerHandRevealed = true;
        }

        public List<PlayingCard> GetVisibleDealerHand()
        {
            // Check if _dealerHand is null or empty to avoid null reference
            if (_dealerHand == null || !_dealerHand.Any())
            {
                return new List<PlayingCard>(); // Return an empty list to prevent the app from crashing
            }

            if (_dealerHandRevealed)
            {
                return _dealerHand;
            }

            // Only return the first card when the hand is not revealed
            return new List<PlayingCard> { _dealerHand[0] };
        }

        private List<PlayingCard> GenerateDeck(int numDecks)
        {
            List<PlayingCard> deck = [];

            string[] suits = ["Hearts", "Diamonds", "Clubs", "Spades"];
            string[] values = ["2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"];

            for (int i = 0; i < numDecks; i++)
            {
                foreach (string suit in suits)
                {
                    foreach (string value in values)
                    {
                        deck.Add(new PlayingCard(suit, value));
                    }
                }
            }

            Random random = new Random();

            // DEBUG
            foreach (PlayingCard playingCard in deck)
            {
                Console.WriteLine($"Playing Card:{playingCard.Suit} {playingCard.Value}");
            }

            return deck.OrderBy(x => random.Next()).ToList();
        }

        public PlayingCard DrawCard()
        {
            PlayingCard card = _deck.First();
            _deck.Remove(card);
            return card;
        }

        public int CalculateHandValue(List<PlayingCard> hand)
        {
            // Safeguard: return 0 if hand is null or empty
            if (hand == null || hand.Count == 0)
            {
                return 0;
            }

            int totalValue = 0;
            int aceCount = 0;

            foreach (PlayingCard card in hand)
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

        public virtual bool IsGameOver() => CalculateHandValue(_playerHand) > 21 || CalculateHandValue(_dealerHand) > 21;
    }

    public class PlayingCard
    {
        public string Suit { get; }
        public string Value { get; }

        public PlayingCard(string suit, string value)
        {
            Suit = suit;
            Value = value;
        }

        public override string ToString() => $"{Value} of {Suit}";
    }
}

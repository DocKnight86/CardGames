﻿using Microsoft.AspNetCore.SignalR;
using CardGames.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGames.Services
{
    public class BlackjackService
    {
        private List<Card> _deck;
        private List<Card> _playerHand;
        private List<Card> _dealerHand;
        private bool _dealerHandRevealed;
        private readonly IHubContext<GameHub> _hubContext; // Injecting SignalR Hub

        public BlackjackService(IHubContext<GameHub> hubContext) // Constructor receives SignalR Hub
        {
            _hubContext = hubContext;
        }

        public void InitializeGame()
        {
            _deck = GenerateDeck();
            _playerHand = new List<Card>();
            _dealerHand = new List<Card>();
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

        public List<Card> GetDealerHand()
        {
            return _dealerHand ?? new List<Card>(); // Ensure it returns an empty list if _dealerHand is null
        }

        public List<Card> GetPlayerHand() => _playerHand;

        public void RevealDealerHand()
        {
            _dealerHandRevealed = true;
        }

        public List<Card> GetVisibleDealerHand()
        {
            // Check if _dealerHand is null or empty to avoid null reference
            if (_dealerHand == null || !_dealerHand.Any())
            {
                return new List<Card>(); // Return an empty list to prevent the app from crashing
            }

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

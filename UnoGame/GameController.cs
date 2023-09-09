using System;
using System.Collections.Generic;
using System.Linq;

namespace UnoGame
{
    public class GameController
    {
        private List<PlayerData> _playerDataList;
        private List<ICard> _discardPileList;
        private bool _isReversed = false;
        private int _currentPlayerIndex = 0;
        private int _defaultStartingHand = 7;
        private int _drawTwo = 2;
        private int _drawFour = 4;


        public GameController()
        {
            _playerDataList = new List<PlayerData>();
            _discardPileList = new List<ICard>();
        }
        public PlayerData AddPlayer(IPlayer player)
        {
            PlayerData playerData = new PlayerData(player);
            _playerDataList.Add(playerData);
            return playerData;
        }
        public List<ICard> DiscardedPile => _discardPileList;

        public List<IPlayer> Players
        {
            get { return _playerDataList.Select(playerData => playerData.Player).ToList(); }
        }

        public PlayerData GetPlayerData(IPlayer player)
        {
            return _playerDataList.FirstOrDefault(pd => pd.Player == player);
        }

        public bool IsPlayerNameTaken(string playerName)
        {
            return _playerDataList.Any(playerData => playerData.Player.PlayerName == playerName);
        }


        private IPlayer GetNextPlayer()
        {
            if (_playerDataList.Count == 0)
            {
                return null;
            }
            int nextPlayerIndex;
            if (!_isReversed)
            {
                nextPlayerIndex = (_currentPlayerIndex + 1) % _playerDataList.Count;
            }
            else
            {
                nextPlayerIndex = (_currentPlayerIndex - 1 + _playerDataList.Count) % _playerDataList.Count;
            }

            return _playerDataList[nextPlayerIndex].Player;
        }

        public ICard DrawCard()
        {
            ICard generatedCard = null;
            do
            {
                generatedCard = GenerateCard();
            }
            while (!IsCardValidToGenerate(generatedCard));

            return generatedCard;
        }

        private int GetMaxCopiesAllowed(CardValue cardValue)
        {
            switch (cardValue)
            {
                case CardValue.Zero:
                    return 1;
                case CardValue.Wild:
                case CardValue.WildDrawFour:
                    return 4;
                default:
                    return 2;
            }
        }
        public bool IsCardValidToGenerate(ICard card)
        {
            int maxCopiesAllowed = GetMaxCopiesAllowed(card.CardValue);

            // Count how many cards with the same value and color exist in all player hands
            int sameValueAndColorCountInHands = _playerDataList.SelectMany(pd => pd.HandCard)
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            // Count how many cards with the same value and color exist in the discard pile
            int sameValueAndColorCountInDiscardPile = _discardPileList
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            return (sameValueAndColorCountInHands + sameValueAndColorCountInDiscardPile) < maxCopiesAllowed;
        }
        public ICard GenerateCard()
        {
            Random random = new Random();
            CardValue randomValue;
            CardColor randomColor;
            randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

            if (randomValue == CardValue.Wild || randomValue == CardValue.WildDrawFour)
            {

                return new Card { CardValue = randomValue, CardColor = CardColor.Blank, IsWild = true };
            }
            else
            {
                do
                {
                    randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
                }
                while (randomColor == CardColor.Blank);

                return new Card { CardColor = randomColor, CardValue = randomValue };
            }
        }
        public void DealStartingHands()
        {
            foreach (PlayerData playerData in _playerDataList)
            {
                for (int i = 0; i < _defaultStartingHand; i++)
                {
                    ICard drawnCard = DrawCard();
                    playerData.AddCardToHand(drawnCard);
                }
            }
        }

        public ICard DrawCardToPlayerHand(IPlayer player)
        {
            PlayerData playerData = GetPlayerData(player);

            if (playerData != null)
            {
                ICard drawnCard = DrawCard();
                playerData.AddCardToHand(drawnCard);
                return drawnCard;
            }
            return null; //
        }

        public bool DiscardCard(IPlayer player, ICard card, int choice)
        {
            PlayerData playerData = GetPlayerData(player);


            if (playerData == null || !IsCardValidToDiscard(card))
            {
                return false;
            }

            if (IsActionCard(card))
            {
                HandleActionCard(card, choice);
            }
            playerData.HandCard.Remove(card);

            _discardPileList.Add(card);

            return true;
        }
        public bool IsCardValidToDiscard(ICard card)
        {
            ICard topDiscardCard = _discardPileList.Last();

            if (card.IsWild)
            {
                return true;
            }

            return card.CardColor == topDiscardCard.CardColor || card.CardValue == topDiscardCard.CardValue;
        }
        public bool IsActionCard(ICard card)
        {
            CardValue[] _actionCardValues = { CardValue.Skip, CardValue.Reverse, CardValue.DrawTwo, CardValue.Wild, CardValue.WildDrawFour };

            return _actionCardValues.Contains(card.CardValue);
        }
        public bool IsWildCard(ICard card)
        {
            CardValue[] _wildCardValues = { CardValue.Wild, CardValue.WildDrawFour };
            return _wildCardValues.Contains(card.CardValue);
        }

        private CardValue HandleActionCard(ICard card, int choice)
        {
            switch (card.CardValue)
            {
                case CardValue.Skip:
                    SkipNextPlayer();
                    return CardValue.Skip;
                case CardValue.Reverse:
                    ReverseTurnDirection();
                    return CardValue.Reverse;
                case CardValue.DrawTwo:
                    DrawTwoCardsNextPlayer();
                    SkipNextPlayer();
                    return CardValue.DrawTwo;
                case CardValue.Wild:
                    ChangeWildCardColor(card, choice);
                    return CardValue.Wild;
                case CardValue.WildDrawFour:
                    ChangeWildCardColor(card, choice);
                    DrawFourCardsNextPlayer();
                    SkipNextPlayer();
                    return CardValue.WildDrawFour;
                default:
                    throw new ArgumentException("Invalid Action Card");
            }
        }

        public ICard InitialDiscardPile()
        {
            ICard drawnCard;

            do
            {
                drawnCard = DrawCard();
            } while (drawnCard.IsWild);

            _discardPileList.Add(drawnCard);

            return drawnCard;
        }
        public IPlayer GetPlayerTurn()
        {
            if (_playerDataList.Count == 0)
            {
                return null;
            }

            PlayerData currentPlayerData = _playerDataList[_currentPlayerIndex];
            return currentPlayerData.Player;
        }
        public IPlayer NextPlayerTurn()
        {
            if (_playerDataList.Count == 0)
            {
                return null;
            }

            if (!_isReversed)
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % _playerDataList.Count;
            }
            else
            {
                _currentPlayerIndex = (_currentPlayerIndex - 1 + _playerDataList.Count) % _playerDataList.Count;
            }

            PlayerData nextPlayerData = _playerDataList[_currentPlayerIndex];
            return nextPlayerData.Player;
        }
        public bool ReverseTurnDirection()
        {
            _isReversed = !_isReversed;
            return _isReversed;
        }
        public IPlayer SkipNextPlayer()
        {
            IPlayer nextPlayer = GetNextPlayer();

            if (IsValidPlayer(nextPlayer))
            {
                NextPlayerTurn(); // Skip the next player
            }

            return nextPlayer;
        }
        public void DrawTwoCardsNextPlayer()
        {
            IPlayer nextPlayer = GetNextPlayer();

            if (IsValidPlayer(nextPlayer))
            {
                for (int i = 0; i < _drawTwo; i++)
                {
                    ICard drawnCard = DrawCard();
                    GetPlayerData(nextPlayer).AddCardToHand(drawnCard);
                }
            }
        }
        public void DrawFourCardsNextPlayer()
        {
            IPlayer nextPlayer = GetNextPlayer();

            if (IsValidPlayer(nextPlayer))
            {
                for (int i = 0; i < _drawFour; i++)
                {
                    ICard drawnCard = DrawCard();
                    GetPlayerData(nextPlayer).AddCardToHand(drawnCard);
                }
            }
        }
        private bool IsValidPlayer(IPlayer player)
        {
            return _playerDataList.Any(playerData => playerData.Player == player);
        }

        public bool IsGameOver()
        {
            foreach (PlayerData playerData in _playerDataList)
            {
                if (playerData.HandCard.Count == 0)
                {
                    return true;//Game Over if hand card empty
                }
            }
            return false; //continue
        }
        public CardColor ChangeWildCardColor(ICard card, int choice)
        {
            if (card.IsWild)
            {
                switch (choice)
                {
                    case 1:
                        return CardColor.Red;
                    case 2:
                        return CardColor.Green;
                    case 3:
                        return CardColor.Blue;
                    case 4:
                        return CardColor.Yellow;

                }
            }
            return CardColor.Blank; // Change this to your default behavior.
        }


    }
}

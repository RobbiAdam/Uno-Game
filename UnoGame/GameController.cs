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
        private bool _hasDiscarded = false;
        private int _remainingCard = 108;

        // ====================
        // Constructor
        // ====================

        public GameController()
        {
            _playerDataList = new List<PlayerData>();
            _discardPileList = new List<ICard>();
        }

        // ====================
        // Player Management
        // ====================

        public PlayerData AddPlayer(IPlayer player)
        {
            PlayerData playerData = new PlayerData(player);
            _playerDataList.Add(playerData);
            return playerData;
        }
        public PlayerData GetPlayerData(IPlayer player)
        {
            return _playerDataList.FirstOrDefault(pd => pd.Player == player);
        }
        public bool IsPlayerNameTaken(string playerName)
        {
            return _playerDataList.Any(playerData => playerData.Player.PlayerName == playerName);
        }
        // ====================
        // Game Initialization
        // ====================
        public void DealStartingHands()
        {
            foreach (PlayerData _playerData in _playerDataList)
            {
                for (int i = 0; i < _defaultStartingHand; i++)
                {
                    ICard _drawnCard = DrawCard();
                    _playerData.AddCardToHand(_drawnCard);
                }
            }
        }
        public ICard InitialDiscardPile()
        {
            ICard _drawnCard;
            do
            {
                _drawnCard = DrawCard();
            } while (_drawnCard.IsWild);

            _discardPileList.Add(_drawnCard);

            return _drawnCard;
        }
        // ====================
        // Properties and Lists
        // ====================
        public List<ICard> DiscardedPile => _discardPileList;

        public List<IPlayer> Players
        {
            get { return _playerDataList.Select(playerData => playerData.Player).ToList(); }
        }

        public bool HasDiscarded
        {
            get { return _hasDiscarded; }
            set { _hasDiscarded = value; }
        }
        // ====================
        // Card Management
        // ====================
        private ICard DrawCard()
        {
            ICard _generatedCard = null;
            do
            {
                _generatedCard = GenerateCard();
            }
            while (!IsCardValidToGenerate(_generatedCard));

            _remainingCard--;
            return _generatedCard;
        }
        public bool CanDrawCard()
        {
            if (_remainingCard == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
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
        private bool IsCardValidToGenerate(ICard card)
        {
            int _maxCopiesAllowed = GetMaxCopiesAllowed(card.CardValue);

            // Count how many cards with the same value and color exist in all player hands
            int _sameValueAndColorCountInHands = _playerDataList.SelectMany(pd => pd.HandCard)
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            // Count how many cards with the same value and color exist in the discard pile
            int _sameValueAndColorCountInDiscardPile = _discardPileList
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            return (_sameValueAndColorCountInHands + _sameValueAndColorCountInDiscardPile) < _maxCopiesAllowed;
        }
        private ICard GenerateCard()
        {
            Random random = new Random();
            CardValue _randomValue;
            CardColor _randomColor;
            _randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

            if (_randomValue == CardValue.Wild || _randomValue == CardValue.WildDrawFour)
            {

                return new Card { CardValue = _randomValue, CardColor = CardColor.Blank, IsWild = true };
            }
            else
            {
                do
                {
                    _randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
                }
                while (_randomColor == CardColor.Blank);

                return new Card { CardColor = _randomColor, CardValue = _randomValue };
            }
        }

        public bool IsCardValidToDiscard(ICard card)
        {
            ICard _topDiscardCard = _discardPileList.Last();

            if (card.IsWild)
            {
                return true;
            }

            return card.CardColor == _topDiscardCard.CardColor || card.CardValue == _topDiscardCard.CardValue;
        }
        public bool HasMatchingCardInHand(IPlayer player)
        {
            PlayerData _playerData = GetPlayerData(player);

            if (_playerData == null)
            {
                return false;
            }
            ICard _topDiscardCard = DiscardedPile.Last();

            foreach (ICard card in _playerData.HandCard)
            {
                if (card.IsWild || card.CardColor == _topDiscardCard.CardColor || card.CardValue == _topDiscardCard.CardValue)
                {
                    return true;
                }
            }
            return false;
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
        // ====================
        // Player Turn Management:
        // ====================

        public IPlayer GetNextPlayer()
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
        private bool ReverseTurnDirection()
        {
            _isReversed = !_isReversed;
            return _isReversed;
        }
        private IPlayer SkipNextPlayer()
        {
            IPlayer _nextPlayer = GetNextPlayer();

            if (IsValidPlayer(_nextPlayer))
            {
                NextPlayerTurn(); // Skip the next player
            }

            return _nextPlayer;
        }
        private void DrawTwoCardsNextPlayer()
        {
            IPlayer _nextPlayer = GetNextPlayer();

            if (IsValidPlayer(_nextPlayer))
            {
                for (int i = 0; i < _drawTwo; i++)
                {
                    ICard drawnCard = DrawCard();
                    GetPlayerData(_nextPlayer).AddCardToHand(drawnCard);
                }
            }
        }
        private void DrawFourCardsNextPlayer()
        {
            IPlayer _nextPlayer = GetNextPlayer();

            if (IsValidPlayer(_nextPlayer))
            {
                for (int i = 0; i < _drawFour; i++)
                {
                    ICard _drawnCard = DrawCard();
                    GetPlayerData(_nextPlayer).AddCardToHand(_drawnCard);
                }
            }
        }
        private bool IsValidPlayer(IPlayer player)
        {
            return _playerDataList.Any(playerData => playerData.Player == player);
        }
        // ====================
        // Game State:
        // ====================
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
        // ====================
        // Discard and Draw Actions:
        // ====================
        public ICard DrawCardToPlayerHand(IPlayer player)
        {
            PlayerData _playerData = GetPlayerData(player);

            if (_playerData != null)
            {
                ICard drawnCard = DrawCard();
                _playerData.AddCardToHand(drawnCard);
                return drawnCard;
            }
            return null; //
        }
        public bool DiscardCard(IPlayer player, ICard card, int choice)
        {
            PlayerData _playerData = GetPlayerData(player);

            if (_playerData == null || !IsCardValidToDiscard(card))
            {
                return false;
            }
            if (IsActionCard(card))
            {
                HandleActionCard(card, choice);
            }
            _playerData.HandCard.Remove(card);

            _discardPileList.Add(card);

            return true;
        }
    }
}

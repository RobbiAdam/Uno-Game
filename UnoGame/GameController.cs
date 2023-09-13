using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace UnoGame
{

    public class GameController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private List<PlayerData> _playerDataList;
        private List<ICard> _discardPileList;
        //Game Status config
        private bool _isReversed = false;
        private int _currentPlayerIndex = 0;
        private int _defaultStartingHand = 7;//Enum Config
        private int _drawTwo = 2;//Enum Config
        private int _drawFour = 4;//Enum Config
        private bool _hasDiscarded = false;
        private int _remainingCard = 108; //Enum Config


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
            logger.Info($"Player '{player.PlayerName}' added to the game.");
            return playerData;
        }
        public PlayerData GetPlayerData(IPlayer player)
        {
            logger.Info($"Getting player data for: {player.PlayerName}");
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
            logger.Info("Dealing starting hands to players.");
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
            logger.Info("Setting up the initial discard pile.");
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
            logger.Info("Drawing a card.");
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
            logger.Info("Checking if a card can be drawn.");
            if (_remainingCard == 0)
            {
                logger.Info("Card cannot be drawn.");
                return false;
            }
            else
            {
                logger.Info("Card can be drawn.");
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
            logger.Info($"Checking if the generated card is valid: {card}");
            int _maxCopiesAllowed = GetMaxCopiesAllowed(card.CardValue);

            // Count how many cards with the same value and color exist in all player hands
            int _sameValueAndColorCountInHands = _playerDataList.SelectMany(pd => pd.HandCard)
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            // Count how many cards with the same value and color exist in the discard pile
            int _sameValueAndColorCountInDiscardPile = _discardPileList
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            bool _isValid = (_sameValueAndColorCountInHands + _sameValueAndColorCountInDiscardPile) < _maxCopiesAllowed;

            logger.Info($"Card validity check result for {card}: {_isValid}");

            return _isValid;
        }
        private ICard GenerateCard()
        {
            logger.Info("Generating a card.");

            Random random = new Random();
            CardValue _randomValue;
            CardColor _randomColor;
            _randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

            if (_randomValue == CardValue.Wild || _randomValue == CardValue.WildDrawFour)
            {
                logger.Info($"Generated card: Wild ({_randomValue})");
                return new Card { CardValue = _randomValue, CardColor = CardColor.Blank, IsWild = true };
            }
            else
            {
                do
                {
                    _randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
                }
                while (_randomColor == CardColor.Blank);

                logger.Info($"Generated card: {_randomColor} {_randomValue}");
                return new Card { CardColor = _randomColor, CardValue = _randomValue };
            }
        }

        public bool IsCardValidToDiscard(ICard card)
        {
            ICard _topDiscardCard = _discardPileList.Last();

            if (card.IsWild)
            {
                logger.Info($"Card validity check result for {card}: true (Wild card can be discarded)");
                return true;
            }

            bool _isValid = card.CardColor == _topDiscardCard.CardColor || card.CardValue == _topDiscardCard.CardValue;

            logger.Info($"Card validity check result for {card}: {_isValid}");

            return _isValid;
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
        private void HandleActionCard(ICard card, int choice)
        {
            logger.Info($"Handling action card: {card.CardValue}");
            switch (card.CardValue)
            {
                case CardValue.Skip:
                    SkipNextPlayer();
                    break;
                case CardValue.Reverse:
                    ReverseTurnDirection();
                    break;
                case CardValue.DrawTwo:
                    DrawTwoCardsNextPlayer();
                    SkipNextPlayer();
                    break;
                case CardValue.Wild:
                    ChangeWildCardColor(card, choice);
                    break;
                case CardValue.WildDrawFour:
                    ChangeWildCardColor(card, choice);
                    DrawFourCardsNextPlayer();
                    SkipNextPlayer();
                    break;
                default:
                    throw new ArgumentException("Invalid Action Card");
            }
        }
        public CardColor ChangeWildCardColor(ICard card, int choice)
        {
            logger.Info($"Changing wild card color for: {card.CardValue}");

            if (card.IsWild)
            {
                CardColor _newColor = CardColor.Blank;

                switch (choice)
                {
                    case 1:
                        _newColor = CardColor.Red;
                        break;
                    case 2:
                        _newColor = CardColor.Green;
                        break;
                    case 3:
                        _newColor = CardColor.Blue;
                        break;
                    case 4:
                        _newColor = CardColor.Yellow;
                        break;
                }

                logger.Info($"Changed wild card color to: {_newColor}");
                return _newColor;
            }

            logger.Info($"No color change performed for non-wild card: {card.CardValue}");
            return CardColor.Blank;
        }

        // ====================
        // Player Turn Management:
        // ====================

        public IPlayer GetNextPlayer()
        {
            logger.Info("Getting the next player.");

            if (_playerDataList.Count == 0)
            {
                logger.Warn("No players in the game.");
                return null;
            }

            int _nextPlayerIndex;
            if (!_isReversed)
            {
                _nextPlayerIndex = (_currentPlayerIndex + 1) % _playerDataList.Count;
            }
            else
            {
                _nextPlayerIndex = (_currentPlayerIndex - 1 + _playerDataList.Count) % _playerDataList.Count;
            }

            IPlayer _nextPlayer = _playerDataList[_nextPlayerIndex].Player;
            logger.Info($"Next player: {_nextPlayer.PlayerName}");

            return _nextPlayer;
        }

        public IPlayer GetPlayerTurn()
        {
            logger.Info("Getting the current player's turn.");

            if (_playerDataList.Count == 0)
            {
                logger.Warn("No players in the game.");
                return null;
            }

            PlayerData _currentPlayerData = _playerDataList[_currentPlayerIndex];
            IPlayer _currentPlayer = _currentPlayerData.Player;

            logger.Info($"Current player's turn: {_currentPlayer.PlayerName}");

            return _currentPlayer;
        }

        public IPlayer NextPlayerTurn()
        {
            logger.Info("Advancing to the next player's turn.");

            if (_playerDataList.Count == 0)
            {
                logger.Warn("No players in the game.");
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

            PlayerData _nextPlayerData = _playerDataList[_currentPlayerIndex];
            IPlayer _nextPlayer = _nextPlayerData.Player;

            logger.Info($"Next player's turn: {_nextPlayer.PlayerName}");

            return _nextPlayer;
        }

        private bool ReverseTurnDirection()
        {
            logger.Info("Reverse card is played");
            _isReversed = !_isReversed;
            return _isReversed;
        }
        private IPlayer SkipNextPlayer()
        {
            IPlayer _nextPlayer = GetNextPlayer();
            logger.Info($"{_nextPlayer} affected by Skip card");
            if (IsValidPlayer(_nextPlayer))
            {
                NextPlayerTurn(); // Skip the next player
            }

            return _nextPlayer;
        }
        private void DrawTwoCardsNextPlayer()
        {

            IPlayer _nextPlayer = GetNextPlayer();
            logger.Info($"{_nextPlayer} affected by Draw Two card");
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
            logger.Info($"{_nextPlayer} affected by Wild Draw Four card");
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
            logger.Info($"Checking if the player is valid: {player.PlayerName}");

            return _playerDataList.Any(playerData => playerData.Player == player);
        }

        // ====================
        // Game State:
        // ====================
        public bool IsGameOver()
        {
            logger.Info("Checking if the game is over.");

            foreach (PlayerData playerData in _playerDataList)
            {
                if (playerData.HandCard.Count == 0)
                {
                    logger.Info($"Game over: Player {playerData.Player.PlayerName} has an empty hand.");
                    return true;
                }
            }
            logger.Info("Game is not over.");
            return false;
        }

        // ====================
        // Discard and Draw Actions:
        // ====================
        public ICard DrawCardToPlayerHand(IPlayer player)
        {
            logger.Info($"Drawing a card to {player.PlayerName}'s hand.");

            PlayerData _playerData = GetPlayerData(player);

            if (_playerData != null)
            {
                ICard drawnCard = DrawCard();
                _playerData.AddCardToHand(drawnCard);

                logger.Info($"Drew card {drawnCard} to {player.PlayerName}'s hand.");
                return drawnCard;
            }

            logger.Warn($"Failed to draw card to {player.PlayerName}'s hand: Player not found.");
            return null;
        }

        public bool DiscardCard(IPlayer player, ICard card, int choice)
        {
            PlayerData _playerData = GetPlayerData(player);

            if (_playerData == null || !IsCardValidToDiscard(card))
            {
                logger.Warn($"Failed to discard card {card} by {player.PlayerName}: Invalid player or card.");
                return false;
            }

            if (IsActionCard(card))
            {
                HandleActionCard(card, choice);
            }

            _playerData.HandCard.Remove(card);
            _discardPileList.Add(card);

            logger.Info($"Discarded card {card} by {player.PlayerName}.");

            return true;
        }

    }
}

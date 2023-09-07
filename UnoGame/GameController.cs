using System;
using System.Collections.Generic;
using System.Linq;

namespace UnoGame
{
    public class GameController
    {
        private List<IPlayer> _playerList;
        private List<PlayerData> _playerDataList;
        internal List<ICard> _discardPileList;
        private bool _isReversed = false;
        private int _currentPlayerIndex = 0;
        private int _defaultStartingHand = 7;

        public GameController()
        {
            _playerList = new List<IPlayer>();
            _playerDataList = new List<PlayerData>();
            _discardPileList = new List<ICard>();
        }
        public PlayerData AddPlayer(IPlayer player)
        {
            PlayerData playerData = new PlayerData(player);
            _playerList.Add(player);
            _playerDataList.Add(playerData);
            return playerData;
        }

        public List<IPlayer> Players
        {
            get { return _playerList; }
        }
        public PlayerData GetPlayerData(IPlayer player)
        {
            int _playerIndex = _playerList.IndexOf(player);
            if (_playerIndex != -1 && _playerIndex < _playerDataList.Count)
            {
                return _playerDataList[_playerIndex];
            }
            return null;
        }
        private IPlayer GetNextPlayer()
        {
            if (_playerList.Count == 0)
            {
                return null;
            }
            int _nextPlayerIndex;
            if (!_isReversed)
            {
                _nextPlayerIndex = (_currentPlayerIndex + 1) % _playerList.Count;
            }
            else
            {
                _nextPlayerIndex = (_currentPlayerIndex - 1 + _playerList.Count) % _playerList.Count;
            }

            return _playerList[_nextPlayerIndex];
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

        public int GetMaxCopiesAllowed(CardValue cardValue)
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
            int sameValueAndColorCountInHands = _playerDataList.SelectMany(pd => pd._playerHandList)
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            // Count how many cards with the same value and color exist in the discard pile
            int sameValueAndColorCountInDiscardPile = _discardPileList
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            return (sameValueAndColorCountInHands + sameValueAndColorCountInDiscardPile) < maxCopiesAllowed;
        }


        public ICard GenerateCard()
        {
            Random random = new Random();
            CardValue randomValue = (CardValue)random.Next(Enum.GetValues(typeof(CardValue)).Length);

            if (randomValue == CardValue.Wild || randomValue == CardValue.WildDrawFour)
            {
                return new Card { CardValue = randomValue, IsWild = true };
            }
            else
            {
                CardColor randomColor = (CardColor)random.Next(Enum.GetValues(typeof(CardColor)).Length);
                return new Card { CardColor = randomColor, CardValue = randomValue };
            }
        }
        public void DealStartingHands()
        {
            foreach (IPlayer player in _playerList)
            {
                PlayerData playerData = GetPlayerData(player);
                if (playerData == null)
                {
                    continue;
                }
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

        public bool DiscardCard(IPlayer player, ICard card)
        {
            PlayerData playerData = GetPlayerData(player);

            if (playerData == null || !IsCardValidToDiscard(card))
            {
                return false;
            }

            playerData._playerHandList.Remove(card);
            _discardPileList.Add(card);
            return true;
        }

        public bool IsCardValidToDiscard(ICard card)
        {
            ICard topDiscardCard = _discardPileList.Last();

            if (topDiscardCard == null)
            {
                return true;
            }
            return card.CardColor == topDiscardCard.CardColor || card.CardValue == topDiscardCard.CardValue;
        }


        private bool IsActionCard(ICard card)
        {
            CardValue[] _actionCardValues = { CardValue.Skip, CardValue.Reverse, CardValue.DrawTwo, CardValue.Wild, CardValue.WildDrawFour };

            return _actionCardValues.Contains(card.CardValue);
        }

        private ActionCard HandleActionCard(ICard card)
        {
            switch (card.CardValue)
            {
                case CardValue.Skip:
                    SkipNextPlayer();
                    return ActionCard.Skip;
                case CardValue.Reverse:
                    ReverseTurnOrder();
                    return ActionCard.Reverse;
                case CardValue.DrawTwo:
                    DrawTwoCardsNextPlayer();
                    return ActionCard.DrawTwo;
                case CardValue.Wild:
                    DrawFourCardsNextPlayer();
                    return ActionCard.WildCard;
                case CardValue.WildDrawFour:
                    DrawFourCardsNextPlayer();
                    return ActionCard.WildCardFour;
                default:

                    throw new ArgumentException();
            }
        }

        public ICard SetDiscardPile()
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
            if (_playerList.Count == 0)
            {
                return null;
            }
            return _playerList[_currentPlayerIndex];
        }
        public IPlayer NextPlayerTurn()
        {
            if (_playerList.Count == 0)
            {
                return null;
            }

            if (!_isReversed)
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % _playerList.Count;
            }
            else
            {
                _currentPlayerIndex = (_currentPlayerIndex - 1 + _playerList.Count) % _playerList.Count;
            }

            return _playerList[_currentPlayerIndex];
        }

        public bool ReverseTurnDirection()
        {
            _isReversed = !_isReversed;
            return _isReversed;
        }

        public void SkipNextPlayer()
        {
            IPlayer nextPlayer = GetNextPlayer();

            if (IsValidPlayer(nextPlayer))
            {
                NextPlayerTurn(); // Skip the next player
            }
        }

        public void ReverseTurnOrder()
        {
            ReverseTurnDirection();
        }

        public void DrawTwoCardsNextPlayer()
        {
            IPlayer nextPlayer = GetNextPlayer();

            if (IsValidPlayer(nextPlayer))
            {
                for (int i = 0; i < 2; i++)
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
                for (int i = 0; i < 4; i++)
                {
                    ICard drawnCard = DrawCard();
                    GetPlayerData(nextPlayer).AddCardToHand(drawnCard);
                }
            }
        }
        public void ChooseWildCardColor(IPlayer player, CardColor chosenColor)
        {
            ICard topDiscardCard = _discardPileList.LastOrDefault();

            if (topDiscardCard != null && topDiscardCard.IsWild)
            {
                if (IsValidPlayer(player) && IsValidCardColor(chosenColor))
                {
                    topDiscardCard.CardColor = chosenColor;
                }
            }
        }
        private bool IsValidCardColor(CardColor color)
        {
            return color == CardColor.Red || color == CardColor.Green || color == CardColor.Blue || color == CardColor.Yellow;
        }

        private bool IsValidPlayer(IPlayer player)
        {

            return _playerList.Contains(player);
        }

        public bool CheckForWinner(IPlayer player)
        {
            PlayerData playerData = GetPlayerData(player);

            return playerData != null && playerData._playerHandList.Count == 0;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnoGame
{
    public class GameController
    {
        private List<IPlayer> PlayerList;
        private List<PlayerData> PlayerDataList;
        private Dictionary<CardValue, int> GeneratedCardDictionary;
        internal List<ICard> DiscardPileList;
        private bool _isReversed = false;
        private int _currentPlayerIndex = 0;

        public GameController()
        {
            PlayerList = new List<IPlayer>();
            PlayerDataList = new List<PlayerData>();
            GeneratedCardDictionary = new Dictionary<CardValue, int>();
            DiscardPileList = new List<ICard>();

        }

        public void AddPlayer(IPlayer player)
        {
            PlayerList.Add(player);
            PlayerDataList.Add(new PlayerData(player));
        }

        public List<IPlayer> Players
        {
            get { return PlayerList; }
        }
        public PlayerData GetPlayerData(IPlayer player)
        {
            int _playerIndex = PlayerList.IndexOf(player);
            if (_playerIndex != -1 && _playerIndex < PlayerDataList.Count)
            {
                return PlayerDataList[_playerIndex];
            }
            return null;
        }

        private IPlayer GetNextPlayer()
        {
            if (PlayerList.Count == 0)
            {
                return null;
            }
            int _nextPlayerIndex;
            if (!_isReversed)
            {
                _nextPlayerIndex = (_currentPlayerIndex + 1) % PlayerList.Count;
            }
            else
            {
                _nextPlayerIndex = (_currentPlayerIndex - 1 + PlayerList.Count) % PlayerList.Count;
            }

            return PlayerList[_nextPlayerIndex];
        }
        public ICard DrawCard()
        {
            ICard generatedCard = null;
            do
            {
                generatedCard = GenerateCard();

            } while (!IsCardValidToGenerate(generatedCard));

            if (!generatedCard.IsWild)
            {

                GeneratedCardDictionary[generatedCard.CardValue]++;
            }

            return generatedCard;
        }
        public bool IsCardValidToGenerate(ICard card)
        {
            int _maxCopiesAllowed = 0;

            if (card.CardValue == CardValue.Zero)
            {
                _maxCopiesAllowed = 1;
            }
            else if (!card.IsWild)
            {
                _maxCopiesAllowed = 2;
            }
            else
            {
                _maxCopiesAllowed = 4;
            }
            // Count how many cards with the same value and color exist in all player hands
            int _sameValueAndColorCount =
            PlayerDataList.SelectMany(pd => pd.PlayerHandList).Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            return _sameValueAndColorCount < _maxCopiesAllowed;
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
            foreach (IPlayer player in PlayerList)
            {
                PlayerData playerData = GetPlayerData(player);
                if (playerData == null)
                {
                    continue;
                }
                for (int i = 0; i < 7; i++)
                {
                    ICard drawnCard = DrawCard();
                    playerData.AddCardToHand(drawnCard);
                }
            }
        }
        public void DrawCardToPlayerHand(IPlayer player)
        {
            PlayerData playerData = GetPlayerData(player);

            if (playerData != null)
            {
                ICard drawnCard = DrawCard();
                playerData.AddCardToHand(drawnCard);
            }
        }
        public bool DiscardCard(IPlayer player, ICard card)
        {
            PlayerData playerData = GetPlayerData(player);

            if (playerData != null)
            {
                // if (IsActionCard(card))
                // {
                //     HandleActionCard(card);
                //     DiscardPileList.Add(card); // remove card to discard pile
                //     return true;
                // }
                // else 
                if (IsCardValidToDiscard(card))
                {
                    if (playerData.PlayerHandList.Remove(card))
                    {
                        DiscardPileList.Add(card); // remove card to discard pile
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsCardValidToDiscard(ICard card)
        {
            ICard topDiscardCard = DiscardPileList.Last();

            if (topDiscardCard == null)
            {
                return true;
            }
            return card.CardColor == topDiscardCard.CardColor || card.CardValue == topDiscardCard.CardValue;
        }


        private bool IsActionCard(ICard card)
        {
            return card.CardValue == CardValue.Skip || card.CardValue == CardValue.Reverse ||
                   card.CardValue == CardValue.DrawTwo || card.CardValue == CardValue.WildDrawFour;
        }

        private void HandleActionCard(ICard card)
        {
            if (card.CardValue == CardValue.Skip)
            {
                SkipNextPlayer();
            }
            else if (card.CardValue == CardValue.Reverse)
            {
                ReverseTurnOrder();
            }
            else if (card.CardValue == CardValue.DrawTwo)
            {
                DrawTwoCardsNextPlayer();
            }
            else if (card.CardValue == CardValue.WildDrawFour)
            {
                DrawFourCardsNextPlayer();
            }
        }
        public void SetDiscardPile()
        {
            ICard drawnCard;

            do
            {
                drawnCard = DrawCard();
            } while (drawnCard.IsWild);

            DiscardPileList.Add(drawnCard);
        }

        public IPlayer GetPlayerTurn()
        {
            if (PlayerList.Count == 0)
            {
                return null;
            }
            return PlayerList[_currentPlayerIndex];
        }
        public void NextPlayerTurn()
        {
            if (PlayerList.Count == 0)
            {
                return;
            }
            if (!_isReversed)
            {
                _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerList.Count;
            }
            else
            {
                _currentPlayerIndex = (_currentPlayerIndex - 1 + PlayerList.Count) % PlayerList.Count;
            }
        }
        public void ReverseTurnDirection()
        {
            _isReversed = !_isReversed;
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
            ICard topDiscardCard = DiscardPileList.LastOrDefault();

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

            return PlayerList.Contains(player);
        }

        public bool CheckForWinner(IPlayer player)
        {
            PlayerData playerData = GetPlayerData(player);

            return playerData != null && playerData.PlayerHandList.Count == 0;
        }

    }
}

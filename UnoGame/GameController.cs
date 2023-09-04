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

        public GameController()
        {
            PlayerList = new List<IPlayer>();
            PlayerDataList = new List<PlayerData>();
            GeneratedCardDictionary = new Dictionary<CardValue, int>();
            foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
            {
                GeneratedCardDictionary[value] = 0;
            }
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



        public ICard GenerateValidCard()
        {
            ICard generatedCard = null;

            do
            {

                generatedCard = DrawCard();

            } while (!IsCardValidToGenerate(generatedCard));


            if (!generatedCard.IsWild)
            {

                GeneratedCardDictionary[generatedCard.CardValue]++;
            }

            return generatedCard;
        }
        public bool IsCardValidToGenerate(ICard card)
        {
            int maxCopiesAllowed = 0;

            if (card.CardValue == CardValue.Zero)
            {
                maxCopiesAllowed = 1;
            }
            else if (!card.IsWild)
            {
                maxCopiesAllowed = 2;
            }
            else
            {
                maxCopiesAllowed = 4;
            }

            int sameValueAndColorCount = PlayerDataList.SelectMany(pd => pd.PlayerHandList)
                .Count(c => c.CardValue == card.CardValue && c.CardColor == card.CardColor);

            return sameValueAndColorCount < maxCopiesAllowed;
        }
        public ICard DrawCard()
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

                for (int i = 0; i < 27; i++)
                {
                    ICard drawnCard = GenerateValidCard();
                    playerData.AddCardToHand(drawnCard);
                }
            }
        }
    }
}

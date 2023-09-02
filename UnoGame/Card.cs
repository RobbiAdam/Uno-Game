namespace UnoGame
{
    public class Card : ICard
    {
        private CardColor _cardColor;
        private CardValue _cardValue;

        public CardColor CardColor
        {
            get => _cardColor;
            set => _cardColor = value;
        }

        public CardValue CardValue
        {
            get => _cardValue;
            set => _cardValue = value;
        }
    }
}

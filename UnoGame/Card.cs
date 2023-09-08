namespace UnoGame
{
    public class Card : ICard
    {
        private CardColor _cardColor;
        private CardValue _cardValue;
        private bool _isWild;

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

        public bool IsWild
        {
            get => _isWild;
            set => _isWild = value;
        }


    }
}


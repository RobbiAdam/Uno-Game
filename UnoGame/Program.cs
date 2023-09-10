using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();
    static bool _gameStatus = false;
    static void Main()
    {
        SetupGame();
        PlayGame();
        EndGame();
    }

    static void SetupGame()
    {
        _numberOfPlayer = InsertNumberOfPlayers();
        ShowPlayerList();
        gameController.DealStartingHands();
        gameController.InitialDiscardPile();
        DisplayPlayerHands();
        DisplayTopCardOnDiscardPile();
    }

    static void PlayGame()
    {
        while (!gameController.IsGameOver() && !_gameStatus)
        {
            TakeAction();
            Console.WriteLine("");
            gameController.NextPlayerTurn();
        }
    }
    static void EndGame()
    {
        Console.WriteLine("Game over!");
    }
    static int InsertNumberOfPlayers()
    {
        int _numberOfPlayers = 0;
        bool _validInput = false;

        while (!_validInput)
        {
            Console.WriteLine("Enter the numbers of players: ");
            if (int.TryParse(Console.ReadLine(), out _numberOfPlayers) && _numberOfPlayers > 1 && _numberOfPlayers <= 4)
            {
                _validInput = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 2 and 4.");
            }
        }
        return _numberOfPlayers;
    }

    static void ShowPlayerList()
    {
        for (int _playerId = 1; _playerId <= _numberOfPlayer; _playerId++)
        {
            string _playerName;
            bool _isNameTaken;

            do
            {
                Console.WriteLine($"Enter the name for Player {_playerId}");
                _playerName = Console.ReadLine();

                _isNameTaken = gameController.IsPlayerNameTaken(_playerName);

                if (_isNameTaken)
                {
                    Console.WriteLine("Player name is already taken. Please choose a different name.");
                }

            } while (_isNameTaken);

            gameController.AddPlayer(new Player(_playerId, _playerName));
        }

        foreach (IPlayer _player in gameController.Players)
        {
            Console.WriteLine("");
            Console.WriteLine($"Player ID: {_player.PlayerId}, Player Name: {_player.PlayerName}");
        }
    }

    static void DisplayPlayerHands()
    {
        Console.WriteLine("Players and their hands:");

        foreach (IPlayer _player in gameController.Players)
        {
            PlayerData _playerData = gameController.GetPlayerData(_player);

            if (_playerData == null)
            {
                Console.WriteLine("No current player data.");
                continue;
            }

            Console.WriteLine($"{_player.PlayerName} (ID: {_player.PlayerId}):");

            foreach (ICard card in _playerData.HandCard)
            {
                string _cardDescription = GetCardDescription(card);
                Console.WriteLine($"  {_cardDescription}");
            }
        }
    }
    static void DisplayTopCardOnDiscardPile()
    {
        if (gameController.DiscardedPile.Count > 0)
        {
            ICard _topDiscardPile = gameController.DiscardedPile.Last();
            Console.WriteLine($"Top Card on Discard Pile: {_topDiscardPile.CardColor} {_topDiscardPile.CardValue}");
            Console.WriteLine("");
        }
        else
        {
            Console.WriteLine("Discard pile is empty.");
        }
    }
    static void DisplayCurrentPlayerHand(string playerName, List<ICard> HandCard)
    {
        Console.WriteLine("");
        Console.WriteLine($"{playerName}'s Hand:");

        for (int i = 0; i < HandCard.Count; i++)
        {
            ICard _card = HandCard[i];
            string _cardDescription = GetCardDescription(_card);
            Console.WriteLine($"{i + 1}. {_cardDescription}");
        }
    }
    static string GetCardDescription(ICard card)
    {
        if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
        {
            return card.CardValue.ToString();
        }
        else
        {
            return $"{card.CardColor} {card.CardValue}";
        }
    }
    static void TakeAction()
    {
        IPlayer _currentPlayer = gameController.GetPlayerTurn();
        bool _hasDiscarded = gameController.HasDiscarded;
        bool _canDrawCard = gameController.CanDrawCard();
        bool _canPlayCard = gameController.HasMatchingCardInHand(_currentPlayer);

        if (_currentPlayer == null)
        {
            Console.WriteLine("No current player.");
            return;
        }

        PlayerData _currentPlayerData = gameController.GetPlayerData(_currentPlayer);

        if (_currentPlayerData == null)
        {
            Console.WriteLine("No current player data.");
            return;
        }
        _hasDiscarded = false;
        Console.WriteLine($"{_currentPlayer.PlayerName}'s turn.");
        DisplayCurrentPlayerHand(_currentPlayer.PlayerName, _currentPlayerData.HandCard);

        while (true)
        {
            DisplayTopCardOnDiscardPile();
            Console.WriteLine("1. Draw a card");
            Console.WriteLine("2. Discard a card");
            Console.WriteLine("3. End turn");
            Console.WriteLine("4. End the game");
            Console.WriteLine($"{_currentPlayer.PlayerName} Choose an action:");

            if (int.TryParse(Console.ReadLine(), out int _choice))
            {
                switch (_choice)
                {
                    case 1:
                        if (!_canDrawCard)
                        {
                            Console.WriteLine("No card to draw");
                        }
                        else if (!_canPlayCard && !_hasDiscarded)
                        {
                            gameController.DrawCardToPlayerHand(_currentPlayer);
                            Console.WriteLine($"{_currentPlayer.PlayerName} drew a card.");
                            DisplayCurrentPlayerHand(_currentPlayer.PlayerName, _currentPlayerData.HandCard);
                        }
                        else
                        {
                            Console.WriteLine("You cannot draw a card.");
                        }
                        break;

                    case 2:
                        if (!_hasDiscarded)
                        {
                            if (ChooseCardToDiscard(_currentPlayer))
                            {
                                _hasDiscarded = true;
                            }
                        }
                        else
                            Console.WriteLine($"{_currentPlayer.PlayerName} already discard a card");
                        break;
                    case 3:
                        if (!_hasDiscarded)
                        {
                            Console.WriteLine("You must discard a card before ending your turn.");
                        }
                        else
                        {
                            if (!_canPlayCard)
                            {
                                Console.WriteLine($"{_currentPlayer.PlayerName} couldn't play any cards and ends their turn.");
                                return;
                            }
                            else
                            {
                                Console.WriteLine($"{_currentPlayer.PlayerName}'s ending their turn");
                                return;
                            }
                        }
                        break;
                    case 4:
                        _gameStatus = true; // End Game
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please choose a valid action.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a valid action number.");
            }
        }
    }

    static bool ChooseCardToDiscard(IPlayer player)
    {
        PlayerData _playerData = gameController.GetPlayerData(player);

        if (_playerData == null)
        {
            Console.WriteLine("No current player data.");
            return false;
        }

        List<ICard> _handCards = _playerData.HandCard;

        Console.WriteLine("");
        Console.WriteLine($"{player.PlayerName}, choose a card to discard:");
        DisplayTopCardOnDiscardPile();


        for (int i = 0; i < _handCards.Count; i++)
        {
            ICard _card = _handCards[i];
            string _cardDescription = GetCardDescription(_card);
            Console.WriteLine($"{i + 1}. {_cardDescription}");
        }
        Console.WriteLine("Input number to discard a card");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= _handCards.Count)
        {
            ICard _selectedCard = _handCards[choice - 1];

            if (gameController.IsCardValidToDiscard(_selectedCard))
            {
                if (gameController.IsWildCard(_selectedCard))
                {
                    Console.WriteLine($"{player.PlayerName} discarded a wild card!");
                    DisplayWildCardMessage(_selectedCard);
                }
                if (gameController.IsActionCard(_selectedCard))
                {
                    Console.WriteLine($"{player.PlayerName} discarded an action card!");
                    DisplayActionCardMessage(_selectedCard, player.PlayerName);
                }
                gameController.DiscardCard(player, _selectedCard, choice);
                Console.WriteLine($"{player.PlayerName} discarded: {GetCardDescription(_selectedCard)}");
                return true;
            }
            else
            {
                Console.WriteLine("You can't discard that card.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid card number.");
        }

        return false;
    }
    static void DisplayActionCardMessage(ICard card, string playerName)
    {
        IPlayer _nextPlayer = gameController.GetNextPlayer();
        string _nextplayerName = _nextPlayer.PlayerName;
        switch (card.CardValue)
        {
            case CardValue.Skip:
                Console.WriteLine($"{_nextplayerName} Turn is skipped.");
                break;
            case CardValue.Reverse:
                Console.WriteLine($"Player {playerName} reverses the turn order.");
                break;
            case CardValue.DrawTwo:
                Console.WriteLine($"{_nextplayerName} draws two cards and their turn is skipped.");
                break;
            case CardValue.WildDrawFour:
                Console.WriteLine($"{_nextplayerName} draws four cards and their turn is skipped.");
                break;
            default:
                Console.WriteLine($"Action card played by {playerName}: {card.CardValue}");
                break;
        }
    }

    static int GetColorChoiceFromUser()
    {
        int _userInput;
        do
        {
            Console.WriteLine("Input number to pick a color (1 for Red, 2 for Green, 3 for Blue, 4 for Yellow): ");
            if (!int.TryParse(Console.ReadLine(), out _userInput) || _userInput < 1 || _userInput > 4)
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
            }
        } while (_userInput < 1 || _userInput > 4);

        return _userInput;
    }

    static void DisplayWildCardMessage(ICard card)
    {
        int _colorChoice = GetColorChoiceFromUser();
        CardColor _newColor = gameController.ChangeWildCardColor(card, _colorChoice);
        card.CardColor = _newColor;
        Console.WriteLine($"Wild card color is changed to {_newColor}");

    }
}

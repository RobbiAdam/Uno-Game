﻿using UnoGame;

class Program
{
    static int _numberOfPlayer;
    static GameController gameController = new();
    bool _isGameOver = false;

    static void Main()
    {
        _numberOfPlayer = InsertNumberOfPlayers();
        ShowPlayerList();
        gameController.DealStartingHands();
        gameController.SetDiscardPile();
        DisplayTopCardOnDiscardPile();
        DisplayPlayerHands();

    }

    static int InsertNumberOfPlayers()
    {
        int _numberOfPlayers = 0;
        bool validInput = false;

        while (!validInput)
        {
            Console.WriteLine("Enter the numbers of players: ");
            if (int.TryParse(Console.ReadLine(), out _numberOfPlayers) && _numberOfPlayers > 1 && _numberOfPlayers <= 4)
            {
                validInput = true;
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
        for (int playerId = 1; playerId <= _numberOfPlayer; playerId++)
        {
            Console.WriteLine($"Enter the name for Player {playerId}");
            string playerName = Console.ReadLine();

            gameController.AddPlayer(new Player(playerId, playerName));
        }
        foreach (IPlayer player in gameController.Players)
        {
            Console.WriteLine($"Player ID: {player.PlayerId}, Player Name: {player.PlayerName}");
        }
    }


    static void DisplayPlayerHands()
    {
        Console.WriteLine("Players and their hands:");
        foreach (IPlayer player in gameController.Players)
        {
            PlayerData playerData = gameController.GetPlayerData(player);
            if (playerData != null)
            {
                Console.WriteLine($"{player.PlayerName} (ID: {player.PlayerId}):");
                foreach (ICard card in playerData.PlayerHandList)
                {
                    if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
                    {
                        Console.WriteLine($"  {card.CardValue}");
                    }
                    else
                    {
                        Console.WriteLine($"  {card.CardColor} {card.CardValue}");
                    }
                }
            }
        }
    }

    static void DisplayTopCardOnDiscardPile()
    {
        if (gameController.DiscardPileList.Count > 0)
        {
            ICard topCard = gameController.DiscardPileList.Last();
            Console.WriteLine($"Top Card on Discard Pile: {topCard.CardColor} {topCard.CardValue}");
        }
        else
        {
            Console.WriteLine("Discard pile is empty.");
        }
    }

    static void DisplayCurrentPlayerHand(string playerName, List<ICard> playerHandList)
    {
        Console.WriteLine($"{playerName}'s Hand:");
        foreach (ICard card in playerHandList)
        {
            if (card.CardValue == CardValue.Wild || card.CardValue == CardValue.WildDrawFour)
            {
                Console.WriteLine($"  {card.CardValue}");
            }
            else
            {
                Console.WriteLine($"  {card.CardColor} {card.CardValue}");
            }
        }
    }





}
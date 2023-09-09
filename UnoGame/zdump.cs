// public CardColor ChangeWildCardColor(ICard card, int choice)
// {
//     if (card.IsWild)
//     {
//         switch (choice)
//         {
//             case 1:
//                 return CardColor.Red;
//             case 2:
//                 return CardColor.Green;
//             case 3:
//                 return CardColor.Blue;
//             case 4:
//                 return CardColor.Yellow;

//         }
//     }
//     return CardColor.Blank; // Change this to your default behavior.
// }

// public bool DiscardCard(IPlayer player, ICard card, int choice)
// {
//     PlayerData playerData = GetPlayerData(player);

//     if (playerData == null || !IsCardValidToDiscard(card))
//     {
//         return false;
//     }
//     if (IsWildCard(card))
//     {
//         card.CardColor = ChangeWildCardColor(card, choice);
//     }

//     if (IsActionCard(card))
//     {
//         HandleActionCard(card);
//     }

//     playerData.HandCard.Remove(card);
//     _discardPileList.Add(card);

//     return true;
// }


//     static int GetColorChoiceFromUser()
//     {
//         int userInput;
//         do
//         {
//             Console.WriteLine("Choose a color (1 for Red, 2 for Green, 3 for Blue, 4 for Yellow): ");
//         } while (!int.TryParse(Console.ReadLine(), out userInput) || userInput < 1 || userInput > 4);

//         switch (userInput)
//         {
//             case 1:
//                 return 1; // Red
//             case 2:
//                 return 2; // Green
//             case 3:
//                 return 3; // Blue
//             case 4:
//                 return 4; // Yellow
//             default:
//                 throw new ArgumentException("Invalid color choice");
//         }
//     }

//     static void DisplayWildCardMessage(ICard card)
//     {
//         int _colorChoice = GetColorChoiceFromUser();
//         CardColor newColor = gameController.ChangeWildCardColor(card, _colorChoice);
//         switch (card.CardValue)
//         {
//             case CardValue.Wild:
//                 card.CardColor = newColor;
//                 Console.WriteLine("Wild card color is changed");
//                 break;
//             case CardValue.WildDrawFour:
//                 card.CardColor = newColor;
//                 Console.WriteLine("Wild card color is changed");
//                 break;
//         }
//     }

//         if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= handCards.Count)
//         {
//             ICard selectedCard = handCards[choice - 1];

//             if (gameController.IsCardValidToDiscard(selectedCard))
//             {
//                 if (gameController.IsWildCard(selectedCard))
//                 {
//                     DisplayWildCardMessage(selectedCard);
//                 }
//                 if (gameController.IsActionCard(selectedCard))
//                 {
//                     Console.WriteLine($"{player.PlayerName} discarded an action card!");
//                     DisplayActionCardMessage(selectedCard, player.PlayerName);
//                 }
//                 gameController.DiscardCard(player, selectedCard, choice);
//                 Console.WriteLine($"{player.PlayerName} discarded: {GetCardDescription(selectedCard)}");
//                 return true;
//             }
//             else
//             {
//                 Console.WriteLine("You can't discard that card.");
//             }
//         }
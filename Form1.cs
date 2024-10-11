using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MemoryMatchGame
{
    public partial class Form1 : Form
    {
        List<Button> buttons = new List<Button>();
        List<string> cardValues = new List<string>();
        Button? firstCard = null;
        Button? secondCard = null;
        int turns = 0;
        int playerTurn = 1;
        int[] scores = new int[2];
        bool multiplayer = false;
        int matchedPairs = 0; // Counter for matched pairs

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            buttons = new List<Button>() { button1, button2, button3, button4, button5, button6, button7, button8, button9, button10,
                                           button11, button12, button13, button14, button15, button16, button17, button18, button19, button20 };

            // Initialize game logic variables
            cardValues = GenerateCardValues();
            AssignValuesToCards();
            turns = 0;
            scores = new int[] { 0, 0 }; // Player 1 and Player 2 score
            matchedPairs = 0; // Reset matched pairs counter
            UpdateScoreLabel();
        }

        private List<string> GenerateCardValues()
        {
            // Generate 10 pairs of values (e.g., "A", "B", ...)
            List<string> values = new List<string> { "📌", "📌", "⭐", "⭐", "❤️", "❤️", "✈️", "✈️", "🎶", "🎶",
                                                    "🍕", "🍕", "🎈", "🎈", "🏈", "🏈", "🍁", "🍁", "🥑", "🥑" };
            Random random = new Random();
            return values.OrderBy(x => random.Next()).ToList();
        }

        private void AssignValuesToCards()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Tag = new CardInfo { Value = cardValues[i], IsMatched = false }; // Assign the value and matched flag to the button's Tag
                buttons[i].Text = ""; // Set the button text to empty (face down)
                buttons[i].Enabled = true; // Enable the button for clicking
                buttons[i].Font = new Font("Segoe UI Emoji", 12); // Set the font to support emojis
                buttons[i].Click += new EventHandler(Card_Click);
            }
        }

        private void Card_Click(object? sender, EventArgs e)
        {
            if (sender is not Button clickedButton) return;
            if (clickedButton.Tag is not CardInfo cardInfo) return;

            // If the clicked card is already matched, do nothing
            if (cardInfo.IsMatched) return;

            // If no card is selected, set the first card
            if (firstCard == null)
            {
                firstCard = clickedButton;
                firstCard.Text = cardInfo.Value; // Reveal the card's value
                firstCard.Enabled = false; // Disable the first card to prevent double-clicking
            }
            // If a card is already selected, set the second card and check for match
            else if (secondCard == null && clickedButton != firstCard)
            {
                secondCard = clickedButton;
                secondCard.Text = cardInfo.Value; // Reveal the second card's value
                secondCard.Enabled = false; // Disable the second card to prevent double-clicking

                // Check for a match between the first and second card
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            if (firstCard == null || secondCard == null)
                return; // Ensure both cards are selected

            if (firstCard.Tag is not CardInfo firstCardInfo || secondCard.Tag is not CardInfo secondCardInfo)
                return;

            // If the selected cards match
            if (firstCardInfo.Value == secondCardInfo.Value)
            {
                // It's a match, increment score and matched pairs
                scores[playerTurn - 1]++;
                matchedPairs++;

                // Mark the cards as matched
                firstCardInfo.IsMatched = true;
                secondCardInfo.IsMatched = true;

                // Clear the selection of first and second card immediately after matching
                firstCard = null;
                secondCard = null;

                // Update the score label before checking if the game is over
                UpdateScoreLabel();

                // Check if the game is over (all pairs matched)
                if (matchedPairs == cardValues.Count / 2)
                {
                    EndGame();
                    return;
                }
            }
            else
            {
                // Start the timer to flip the cards back if not a match
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 500; // Delay to show the second card before flipping back
                timer.Tick += (s, e) =>
                {
                    if (firstCard != null && secondCard != null)
                    {
                        // Flip both cards face down if not a match
                        firstCard.Text = "";
                        secondCard.Text = "";

                        // Re-enable the cards for future selections (since they weren't a match)
                        firstCard.Enabled = true;
                        secondCard.Enabled = true;
                    }

                    // Reset the selected cards
                    firstCard = null;
                    secondCard = null;

                    // Stop the timer after it's done
                    timer.Stop();
                };
                timer.Start(); // Start the timer to flip cards back
            }

            // Increment the number of turns
            turns++;

            // Switch player turns if in multiplayer mode
            if (multiplayer)
            {
                playerTurn = (playerTurn == 1) ? 2 : 1;
            }

            // Update the score label
            UpdateScoreLabel();
        }

        private void UpdateScoreLabel()
        {
            if (multiplayer)
            {
                lblScore.Text = $"Player {playerTurn}'s turn. Player 1: {scores[0]} | Player 2: {scores[1]}";
            }
            else
            {
                lblScore.Text = $"Turns: {turns}";
            }

            // Center the label
            CenterLabel(lblScore);
        }

        private void CenterLabel(Label label)
        {
            // Calculate the new X position to center the label
            int newX = (this.ClientSize.Width - label.Width) / 2;
            label.Location = new Point(newX, label.Location.Y);
        }

        private void EndGame()
        {
            string message;
            if (multiplayer)
            {
                if (scores[0] == scores[1])
                {
                    message = "It's a tie!";
                }
                else if (scores[0] > scores[1])
                {
                    message = "Player 1 wins!";
                }
                else
                {
                    message = "Player 2 wins!";
                }
            }
            else
            {
                message = $"Game Over! It took {turns} turns.";
            }

            MessageBox.Show(message);
        }

        private void btnMode_Click(object sender, EventArgs e)
        {
            multiplayer = !multiplayer; // Switch between single and multiplayer
            playerTurn = 1; // Reset to Player 1's turn
            InitializeGame(); // Restart the game
            btnMode.Text = multiplayer ? "Multiplayer" : "Single Player";
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            InitializeGame(); // Restart the game
        }

        // Class to store card information
        private class CardInfo
        {
            public string Value { get; set; } = string.Empty;
            public bool IsMatched { get; set; }
        }
    }
}

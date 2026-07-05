using System;
using System.Text;

namespace PhonePad
{
    public class Keypad
    {
        public static string OldPhonePad(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var result = new StringBuilder();
            char currentKey = '\0';
            int pressCount = 0;

            // Map the dial pad. Index matches the digit (0-9).
            string[] keypadMap = {
                " ",    // 0
                "",     // 1 (Usually empty or symbols, omitted for this scope)
                "ABC",  // 2
                "DEF",  // 3
                "GHI",  // 4
                "JKL",  // 5
                "MNO",  // 6
                "PQRS", // 7
                "TUV",  // 8
                "WXYZ"  // 9
            };

            foreach (char c in input)
            {
                // Send button: Commit current sequence and exit
                if (c == '#')
                {
                    CommitCharacter(result, currentKey, pressCount, keypadMap);
                    break;
                }

                // Backspace: Discard current sequence or delete last committed char
                if (c == '*')
                {
                    if (pressCount > 0)
                    {
                        // Delete the character currently being cycled
                        pressCount = 0;
                        currentKey = '\0';
                    }
                    else if (result.Length > 0)
                    {
                        // Delete the last confirmed character
                        result.Length--;
                    }
                    continue;
                }

                // Pause: Commit current sequence, reset state, and wait for next
                if (c == ' ')
                {
                    CommitCharacter(result, currentKey, pressCount, keypadMap);
                    pressCount = 0;
                    currentKey = '\0';
                    continue;
                }

                // Standard Number Input
                if (char.IsDigit(c))
                {
                    if (c == currentKey)
                    {
                        // Consecutive press on the same key
                        pressCount++;
                    }
                    else
                    {
                        // Different key pressed: commit the previous one and start new
                        CommitCharacter(result, currentKey, pressCount, keypadMap);
                        currentKey = c;
                        pressCount = 1;
                    }
                }
            }

            return result.ToString();
        }

        // Helper method to resolve and append the character based on presses
        private static void CommitCharacter(StringBuilder result, char key, int count, string[] map)
        {
            if (count > 0 && char.IsDigit(key))
            {
                int digit = key - '0';
                string letters = map[digit];
                
                if (!string.IsNullOrEmpty(letters))
                {
                    // Use modulo arithmetic to wrap around (e.g., '2' pressed 4 times = 'A')
                    int index = (count - 1) % letters.Length;
                    result.Append(letters[index]);
                }
            }
        }
    }
}
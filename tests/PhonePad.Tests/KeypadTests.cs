using Xunit;
using PhonePad;

namespace PhonePad.Tests
{
    public class KeypadTests
    {
        [Theory]
        [InlineData("33#", "E")]
        [InlineData("227*#", "B")]
        [InlineData("4433555 555666#", "HELLO")]
        [InlineData("8 88777444666*664#", "TURING")]
        public void OldPhonePad_ValidInputs_ReturnsExpectedOutput(string input, string expected)
        {
            // Act
            string actual = Keypad.OldPhonePad(input);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OldPhonePad_EmptyInput_ReturnsEmptyString()
        {
            // Act
            string actual = Keypad.OldPhonePad("");

            // Assert
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void OldPhonePad_BackspaceDeletesCommittedCharacter()
        {
            // Act
            string actual = Keypad.OldPhonePad("22 2*#"); // B, then A, then backspace A

            // Assert
            Assert.Equal("B", actual);
        }
    }
}
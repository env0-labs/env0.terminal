using Env0.Terminal.Config;
using Env0.Terminal.Config.Pocos;
using Env0.Terminal.Filesystem;
using Env0.Terminal.Network;
using Env0.Terminal.Terminal;
using Env0.Terminal.Login;
using Env0.Terminal.API_DTOs;
using System.Collections.Generic;

namespace Env0.Terminal
{
    public class TerminalEngineAPI
    {
        // Core managers and handlers
        private TerminalStateManager _stateManager;
        private SessionState _session;
        private FilesystemManager _filesystemManager;
        private NetworkManager _networkManager;
        private SSHHandler _sshHandler;
        private CommandParser _commandParser;
        private CommandHandler _commandHandler;
        private LoginHandler _loginHandler;

        // Startup flag
        private bool _initialized = false;

        // Phase state
        private bool _bootShown = false;

        // Login state tracking
        private enum LoginStep { None, Username, Password }
        private LoginStep _loginStep = LoginStep.None;
        private string _capturedUsername;
        private string _capturedPassword;

        // Terminal phase tracking
        private TerminalPhase _phase = TerminalPhase.Booting;

        public void Initialize()
        {
            JsonLoader.LoadAll();

            var bootConfig = JsonLoader.BootConfig;
            var userConfig = JsonLoader.UserConfig;
            var devices = JsonLoader.Devices;
            var fsPoco = JsonLoader.Filesystems.ContainsKey("Filesystem_1.json")
                ? JsonLoader.Filesystems["Filesystem_1.json"]
                : null;

            if (devices == null || devices.Count == 0 || fsPoco == null)
                throw new System.Exception("Critical config missing: Devices or Filesystem_1.json");

            var fsRootNode = new FileEntry { Children = fsPoco.Root };
            _filesystemManager = FilesystemManager.FromPocoRoot(fsRootNode);

            var localDevice = devices[0];
            _networkManager = new NetworkManager(devices, localDevice);
            _sshHandler = new SSHHandler(_networkManager);

            _session = new SessionState
            {
                Username = userConfig.Username,
                Password = userConfig.Password,
                Hostname = localDevice.Hostname,
                Domain = "",
                CurrentWorkingDirectory = "/",
                FilesystemManager = _filesystemManager,
                NetworkManager = _networkManager,
                DeviceInfo = localDevice
            };

            _stateManager = new TerminalStateManager();
            _commandParser = new CommandParser();
            _commandHandler = new CommandHandler(_session.DebugMode);
            _loginHandler = new LoginHandler();

            _initialized = true;
            _phase = TerminalPhase.Booting;
            _bootShown = false;
            _loginStep = LoginStep.None;
            _capturedUsername = null;
            _capturedPassword = null;
        }

        public TerminalRenderState Execute(string input)
        {
            if (!_initialized) throw new System.Exception("TerminalEngineAPI not initialized. Call Initialize() first.");

            switch (_phase)
            {
                case TerminalPhase.Booting:
                    // Show boot lines ONCE, then advance immediately to Login phase on any next call
                    if (!_bootShown)
                    {
                        _bootShown = true;
                        return new TerminalRenderState
                        {
                            Phase = TerminalPhase.Booting,
                            BootSequenceLines = JsonLoader.BootConfig?.BootText ?? new List<string>(),
                            Output = string.Join("\n", JsonLoader.BootConfig?.BootText ?? new List<string>()) + "\n",
                            IsLoginPrompt = false,
                            IsPasswordPrompt = false,
                            Prompt = null
                        };
                    }
                    // On *any* input after boot, move to Login phase
                    _phase = TerminalPhase.Login;
                    _loginStep = LoginStep.Username;
                    _capturedUsername = null;
                    _capturedPassword = null;
                    return new TerminalRenderState
                    {
                        Phase = TerminalPhase.Login,
                        IsLoginPrompt = true,
                        IsPasswordPrompt = false,
                        Prompt = "Username: "
                    };

                case TerminalPhase.Login:
                    // Username prompt
                    if (_loginStep == LoginStep.Username)
                    {
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            // FLAVOR: Empty username not allowed!
                            return new TerminalRenderState
                            {
                                Phase = TerminalPhase.Login,
                                IsLoginPrompt = true,
                                IsPasswordPrompt = false,
                                Prompt = "Username: ",
                                Output = "Trying to log in as a ghost? You need a username.\n"
                            };
                        }
                        else
                        {
                            // Only set username if it is NOT whitespace
                            _capturedUsername = input;
                            _loginStep = LoginStep.Password;
                            return new TerminalRenderState
                            {
                                Phase = TerminalPhase.Login,
                                IsLoginPrompt = false,
                                IsPasswordPrompt = true,
                                Prompt = "Password: "
                            };
                        }
                    }

                    // Password prompt
                    else if (_loginStep == LoginStep.Password && string.IsNullOrEmpty(_capturedPassword))
                    {
                        // Accept even blank input!
                        _capturedPassword = input ?? "";
                        _loginHandler.SetUsername(_session, _capturedUsername);
                        _loginHandler.SetPassword(_session, _capturedPassword);
                        _phase = TerminalPhase.Terminal;

                        string warning = string.IsNullOrEmpty(_capturedPassword)
                            ? "no password? well you like to live dangerously... I'll allow it\n"
                            : "";

                        return BuildRenderState($"{warning}Login complete!\n");
                    }

                    // Fallback to username prompt (should not be hit anymore)
                    else
                    {
                        _loginStep = LoginStep.Username;
                        _capturedUsername = null;
                        _capturedPassword = null;
                        return new TerminalRenderState
                        {
                            Phase = TerminalPhase.Login,
                            IsLoginPrompt = true,
                            IsPasswordPrompt = false,
                            Prompt = "Username: "
                        };
                    }

                case TerminalPhase.Terminal:
                default:
                    var parsed = _commandParser.Parse(input);
                    if (parsed == null)
                        return BuildRenderState("", isError: false);

                    var result = _commandHandler.Execute(input, _session);
                    return BuildRenderState(result.Output, result.IsError);
            }
        }

        public void Reset()
        {
            Initialize();
        }

        public void SetDebugMode(bool enabled)
        {
            if (_session != null)
                _session.DebugMode = enabled;

            _commandHandler = new CommandHandler(enabled);
        }

        private TerminalRenderState BuildRenderState(string output, bool isError = false)
        {
            return new TerminalRenderState
            {
                Phase = _phase,
                BootSequenceLines = _phase == TerminalPhase.Booting
                    ? JsonLoader.BootConfig?.BootText ?? new List<string>()
                    : null,

                IsLoginPrompt = _phase == TerminalPhase.Login && _loginStep == LoginStep.Username,
                IsPasswordPrompt = _phase == TerminalPhase.Login && _loginStep == LoginStep.Password && string.IsNullOrEmpty(_capturedPassword),
                Prompt = _phase == TerminalPhase.Terminal
                    ? $"{_session.Username}@{_session.Hostname}:{_session.CurrentWorkingDirectory}$ "
                    : (_phase == TerminalPhase.Login && _loginStep == LoginStep.Username ? "Username: "
                      : (_phase == TerminalPhase.Login && _loginStep == LoginStep.Password ? "Password: " : "")),

                Output = output,
                CurrentDirectory = _session.CurrentWorkingDirectory,
                DirectoryListing = _filesystemManager.ListCurrentDirectory(),
                SessionStackDepth = _session.SshStack.Count,
                SessionStackView = BuildSessionStackView(),
                ClearScreen = false,
                IsError = isError,
                ErrorMessage = isError ? output : null,
                ShowMOTD = false,
                MOTD = _session.DeviceInfo?.Motd,
                DebugMode = _session.DebugMode,
                DebugInfo = null
            };
        }

        private List<SessionContext> BuildSessionStackView()
        {
            var stack = new List<SessionContext>();
            foreach (var ssh in _session.SshStack)
            {
                stack.Add(new SessionContext
                {
                    Username = ssh.Username,
                    Hostname = ssh.Hostname,
                    Prompt = $"{ssh.Username}@{ssh.Hostname}:{ssh.CurrentWorkingDirectory}$ "
                });
            }
            stack.Add(new SessionContext
            {
                Username = _session.Username,
                Hostname = _session.Hostname,
                Prompt = $"{_session.Username}@{_session.Hostname}:{_session.CurrentWorkingDirectory}$ "
            });
            return stack;
        }
    }
}

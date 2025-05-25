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
        private TerminalStateManager _stateManager;
        private SessionState _session;
        private FilesystemManager _filesystemManager;
        private NetworkManager _networkManager;
        private SSHHandler _sshHandler;
        private CommandParser _commandParser;
        private CommandHandler _commandHandler;
        private LoginHandler _loginHandler;

        private bool _initialized = false;
        private bool _bootShown = false;

        // SSH login flow tracking
        private enum LoginMode { None, Local, Ssh }
        private LoginMode _loginMode = LoginMode.None;
        private string _pendingSshTarget = null;
        private DeviceInfo _pendingSshDevice = null;
        private string _pendingSshUser = null;

        private enum LoginStep { None, Username, Password }
        private LoginStep _loginStep = LoginStep.None;
        private string _capturedUsername = null;
        private string _capturedPassword = null;

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

            _loginMode = LoginMode.None;
            _loginStep = LoginStep.None;
            _pendingSshTarget = null;
            _pendingSshDevice = null;
            _pendingSshUser = null;
            _capturedUsername = null;
            _capturedPassword = null;
        }

        public TerminalRenderState Execute(string input)
        {
            if (!_initialized) throw new System.Exception("TerminalEngineAPI not initialized. Call Initialize() first.");

            switch (_phase)
            {
                case TerminalPhase.Booting:
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
                    // Move to local login on any input after boot
                    _phase = TerminalPhase.Login;
                    _loginMode = LoginMode.Local;
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
                    // LOCAL LOGIN
                    if (_loginMode == LoginMode.Local)
                    {
                        if (_loginStep == LoginStep.Username)
                        {
                            if (string.IsNullOrWhiteSpace(input))
                            {
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
                        else if (_loginStep == LoginStep.Password)
                        {
                            _capturedPassword = input ?? "";
                            _loginHandler.SetUsername(_session, _capturedUsername);
                            _loginHandler.SetPassword(_session, _capturedPassword);
                            _phase = TerminalPhase.Terminal;
                            _loginMode = LoginMode.None;
                            string warning = string.IsNullOrEmpty(_capturedPassword)
                                ? "no password? well you like to live dangerously... I'll allow it\n"
                                : "";
                            return BuildRenderState($"{warning}Login complete!\nType read tutorial.txt for instructions\n");
                        }
                        // Reset to username prompt on any weird state
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

                    // SSH LOGIN
                    if (_loginMode == LoginMode.Ssh)
                    {
                        // Username first (if not provided)
                        if (_loginStep == LoginStep.Username)
                        {
                            if (string.IsNullOrWhiteSpace(input))
                            {
                                return new TerminalRenderState
                                {
                                    Phase = TerminalPhase.Login,
                                    IsLoginPrompt = true,
                                    IsPasswordPrompt = false,
                                    Prompt = "Username: ",
                                    Output = "Username required for SSH login.\n"
                                };
                            }
                            _pendingSshUser = input.Trim();
                            _loginStep = LoginStep.Password;
                            return new TerminalRenderState
                            {
                                Phase = TerminalPhase.Login,
                                IsLoginPrompt = false,
                                IsPasswordPrompt = true,
                                Prompt = "Password: "
                            };
                        }
                        // Password next
                        if (_loginStep == LoginStep.Password)
                        {
                            var user = _pendingSshUser;
                            var pass = input ?? "";

                            // Validate credentials
                            var expectedUser = _pendingSshDevice.Username;
                            var expectedPass = _pendingSshDevice.Password;

                            if (!string.Equals(user, expectedUser))
                            {
                                // Retry username
                                _loginStep = LoginStep.Username;
                                _pendingSshUser = null;
                                return new TerminalRenderState
                                {
                                    Phase = TerminalPhase.Login,
                                    IsLoginPrompt = true,
                                    IsPasswordPrompt = false,
                                    Prompt = "Username: ",
                                    Output = "Login failed\n"
                                };
                            }
                            if (pass != expectedPass)
                            {
                                // Retry password (do NOT say why failed)
                                return new TerminalRenderState
                                {
                                    Phase = TerminalPhase.Login,
                                    IsLoginPrompt = false,
                                    IsPasswordPrompt = true,
                                    Prompt = "Password: ",
                                    Output = "Login failed\n"
                                };
                            }

                            // Success: push SSH context, switch session, drop to terminal, show MOTD/banner
                            _session.SshStack.Push(new SshSessionContext(
                                _session.Username,
                                _session.Hostname,
                                _session.CurrentWorkingDirectory,
                                _session.FilesystemManager,
                                _session.NetworkManager
                            ));
                            _session.Username = user;
                            _session.Hostname = _pendingSshDevice.Hostname;
                            _session.CurrentWorkingDirectory = "/";
                            _session.DeviceInfo = _pendingSshDevice;
                            // Load correct FS for this device
                            _session.FilesystemManager = _sshHandler.LoadFilesystemForDevice(_pendingSshDevice);
                            _session.NetworkManager.CurrentDevice = _pendingSshDevice;

                            var motd = !string.IsNullOrWhiteSpace(_pendingSshDevice.Motd)
                                ? _pendingSshDevice.Motd
                                : $"Connected to {_pendingSshDevice.Hostname} ({_pendingSshDevice.Ip})";

                            _phase = TerminalPhase.Terminal;
                            _loginMode = LoginMode.None;
                            _loginStep = LoginStep.None;
                            _pendingSshTarget = null;
                            _pendingSshDevice = null;
                            _pendingSshUser = null;
                            return BuildRenderState($"{motd}\n");
                        }
                    }

                    // Should never hit this, but reset state if it does
                    _loginMode = LoginMode.None;
                    _loginStep = LoginStep.None;
                    _pendingSshTarget = null;
                    _pendingSshDevice = null;
                    _pendingSshUser = null;
                    return new TerminalRenderState
                    {
                        Phase = TerminalPhase.Login,
                        IsLoginPrompt = true,
                        IsPasswordPrompt = false,
                        Prompt = "Username: "
                    };

                case TerminalPhase.Terminal:
                default:
                    var parsed = _commandParser.Parse(input);
                    if (parsed == null)
                        return BuildRenderState("", isError: false);

                    // Special-case SSH: API takes over phase/SSH handling
                    if (parsed.CommandName == "ssh")
                    {
                        if (parsed.Arguments.Length == 0 || string.IsNullOrWhiteSpace(parsed.Arguments[0]))
                            return BuildRenderState("bash: ssh: Missing host\n\n", true);

                        // Parse user@host
                        string user = null, host = null;
                        var target = parsed.Arguments[0].Trim();
                        if (target.Contains("@"))
                        {
                            var split = target.Split('@');
                            user = split[0];
                            host = split[1];
                        }
                        else
                        {
                            host = target;
                        }

                        // Lookup device
                        var device = _networkManager.FindDevice(host);
                        if (device == null || device.Ports == null || !device.Ports.Contains("22"))
                            return BuildRenderState($"ssh: connect to host {host} port 22: Connection refused\n", true);

                        _pendingSshTarget = host;
                        _pendingSshDevice = device;
                        _pendingSshUser = user;

                        _phase = TerminalPhase.Login;
                        _loginMode = LoginMode.Ssh;

                        if (string.IsNullOrEmpty(user))
                        {
                            _loginStep = LoginStep.Username;
                            return new TerminalRenderState
                            {
                                Phase = TerminalPhase.Login,
                                IsLoginPrompt = true,
                                IsPasswordPrompt = false,
                                Prompt = "Username: "
                            };
                        }
                        else
                        {
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
                    else if (parsed.CommandName == "exit")
                    {
                        // Handle SSH stack pop if possible
                        if (_session.SshStack.Count > 0)
                        {
                            var prev = _session.SshStack.Pop();
                            _session.Username = prev.Username;
                            _session.Hostname = prev.Hostname;
                            _session.CurrentWorkingDirectory = prev.CurrentWorkingDirectory;
                            _session.FilesystemManager = prev.FilesystemManager;
                            _session.NetworkManager = prev.NetworkManager;
                            _session.DeviceInfo = _session.NetworkManager.CurrentDevice;

                            var banner = $"Connection to {_session.Hostname} closed.\n";
                            // Do not drop out of Terminal phase unless truly back to base session
                            return BuildRenderState(banner, false);
                        }
                        else
                        {
                            // Already at local terminal
                            return BuildRenderState("You are already at the local terminal.\n", false);
                        }
                    }

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
                IsPasswordPrompt = _phase == TerminalPhase.Login && _loginStep == LoginStep.Password,
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

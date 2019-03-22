﻿using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class MinerProfileViewModel : ViewModelBase, IMinerProfile {
        public static readonly MinerProfileViewModel Current = new MinerProfileViewModel();

        private MinerProfileViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            NTMinerRoot.RefreshArgsAssembly = () => {
                this.ArgsAssembly = NTMinerRoot.Current.BuildAssembleArgs();
            };
            NTMinerRoot.Current.OnReRendContext += () => {
                OnPropertyChanged(nameof(CoinVm));
            };
            VirtualRoot.On<WalletAddedEvent>(
                "添加了钱包后刷新VM",
                LogEnum.Console,
                action: message => {
                    if (message.Source.CoinId == CoinVm?.Id) {
                        OnPropertyChanged(nameof(CoinVm));
                    }
                });
            VirtualRoot.On<WalletRemovedEvent>(
                "添加了钱包后刷新VM",
                LogEnum.Console,
                action: message => {
                    if (message.Source.CoinId == CoinVm?.Id) {
                        OnPropertyChanged(nameof(CoinVm));
                    }
                });
            VirtualRoot.Accept<RefreshAutoBootStartCommand>(
                "刷新开机自动启动和启动后自动开始挖矿的展示",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(nameof(IsAutoBoot));
                    OnPropertyChanged(nameof(IsAutoStart));
                });
            VirtualRoot.On<MinerProfilePropertyChangedEvent>(
                "MinerProfile设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });
            VirtualRoot.On<MineWorkPropertyChangedEvent>(
                "MineWork设置变更后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    OnPropertyChanged(message.PropertyName);
                });

            VirtualRoot.On<GpuProfileSetRefreshedEvent>(
                "Gpu超频集合刷新后刷新附着在当前币种上的超频数据",
                LogEnum.Console,
                action: message => {
                    CoinVm.OnPropertyChanged(nameof(CoinVm.GpuAllOverClockDataVm));
                    CoinVm.OnPropertyChanged(nameof(CoinVm.GpuOverClockVms));
                    CoinVm.OnPropertyChanged(nameof(CoinVm.IsOverClockEnabled));
                    CoinVm.OnPropertyChanged(nameof(CoinVm.IsOverClockGpuAll));
                });

            NTMinerRoot.Current.OnReRendMinerProfile += () => {
                MinerProfileIndexViewModel.Current.OnPropertyChanged(nameof(MinerProfileIndexViewModel.CoinVms));
                this.CoinVm?.OnPropertyChanged(nameof(CoinVm.CoinKernel));
                OnPropertyChanged(nameof(CoinVm));
                OnPropertyChanged(nameof(MinerName));
                OnPropertyChanged(nameof(IsFreeClient));
                OnPropertyChanged(nameof(MineWork));
                OnPropertyChanged(nameof(IsWorker));
                OnPropertyChanged(nameof(IsAutoBoot));
                OnPropertyChanged(nameof(IsNoShareRestartKernel));
                OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                OnPropertyChanged(nameof(IsAutoStart));
                OnPropertyChanged(nameof(IsAutoRestartKernel));
            };
            NTMinerRoot.RefreshArgsAssembly.Invoke();
        }

        public IMineWork MineWork {
            get {
                return NTMinerRoot.Current.MinerProfile.MineWork;
            }
        }

        public bool IsFreeClient {
            get {
                return MineWork == null || VirtualRoot.IsControlCenter;
            }
        }

        public Guid Id {
            get { return NTMinerRoot.Current.MinerProfile.GetId(); }
        }

        public Guid GetId() {
            return this.Id;
        }

        public string MinerName {
            get => NTMinerRoot.Current.MinerProfile.MinerName;
            set {
                if (NTMinerRoot.Current.MinerProfile.MinerName != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(MinerName), value);
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                    OnPropertyChanged(nameof(MinerName));
                }
            }
        }

        public bool IsShowInTaskbar {
            get => NTMinerRegistry.GetIsShowInTaskbar();
            set {
                if (NTMinerRegistry.GetIsShowInTaskbar() != value) {
                    NTMinerRegistry.SetIsShowInTaskbar(value);
                    OnPropertyChanged(nameof(IsShowInTaskbar));
                }
            }
        }

        public bool IsShowNotifyIcon {
            get => NTMinerRegistry.GetIsShowNotifyIcon();
            set {
                if (NTMinerRegistry.GetIsShowNotifyIcon() != value) {
                    NTMinerRegistry.SetIsShowNotifyIcon(value);
                    OnPropertyChanged(nameof(IsShowNotifyIcon));
                    AppHelper.NotifyIcon.RefreshIcon();
                }
            }
        }

        public string HotKey {
            get { return NTMinerRoot.GetHotKey(); }
            set {
                if (NTMinerRoot.GetHotKey() != value) {
                    if (NTMinerRoot.SetHotKey(value)) {
                        OnPropertyChanged(nameof(HotKey));
                    }
                }
            }
        }

        private string _argsAssembly;
        public string ArgsAssembly {
            get {
                return _argsAssembly;
            }
            set {
                _argsAssembly = value;
                OnPropertyChanged(nameof(ArgsAssembly));
                NTMinerRoot.UserKernelCommandLine = value;
            }
        }

        public bool IsAutoBoot {
            get => NTMinerRegistry.GetIsAutoBoot();
            set {
                NTMinerRegistry.SetIsAutoBoot(value);
                OnPropertyChanged(nameof(IsAutoBoot));
            }
        }

        public bool IsAutoStart {
            get => NTMinerRegistry.GetIsAutoStart();
            set {
                NTMinerRegistry.SetIsAutoStart(value);
                OnPropertyChanged(nameof(IsAutoStart));
            }
        }

        public bool IsNoShareRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsNoShareRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsNoShareRestartKernel != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(IsNoShareRestartKernel), value);
                    OnPropertyChanged(nameof(IsNoShareRestartKernel));
                }
            }
        }

        public int NoShareRestartKernelMinutes {
            get => NTMinerRoot.Current.MinerProfile.NoShareRestartKernelMinutes;
            set {
                if (NTMinerRoot.Current.MinerProfile.NoShareRestartKernelMinutes != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(NoShareRestartKernelMinutes), value);
                    OnPropertyChanged(nameof(NoShareRestartKernelMinutes));
                }
            }
        }

        public bool IsPeriodicRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsPeriodicRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsPeriodicRestartKernel != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartKernel), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartKernel));
                }
            }
        }

        public int PeriodicRestartKernelHours {
            get => NTMinerRoot.Current.MinerProfile.PeriodicRestartKernelHours;
            set {
                if (NTMinerRoot.Current.MinerProfile.PeriodicRestartKernelHours != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartKernelHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartKernelHours));
                }
            }
        }

        public bool IsPeriodicRestartComputer {
            get => NTMinerRoot.Current.MinerProfile.IsPeriodicRestartComputer;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsPeriodicRestartComputer != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(IsPeriodicRestartComputer), value);
                    OnPropertyChanged(nameof(IsPeriodicRestartComputer));
                }
            }
        }

        public int PeriodicRestartComputerHours {
            get => NTMinerRoot.Current.MinerProfile.PeriodicRestartComputerHours;
            set {
                if (NTMinerRoot.Current.MinerProfile.PeriodicRestartComputerHours != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(PeriodicRestartComputerHours), value);
                    OnPropertyChanged(nameof(PeriodicRestartComputerHours));
                }
            }
        }

        public bool IsAutoRestartKernel {
            get => NTMinerRoot.Current.MinerProfile.IsAutoRestartKernel;
            set {
                if (NTMinerRoot.Current.MinerProfile.IsAutoRestartKernel != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(IsAutoRestartKernel), value);
                    OnPropertyChanged(nameof(IsAutoRestartKernel));
                }
            }
        }

        public bool IsShowCommandLine {
            get { return NTMinerRoot.GetIsShowCommandLine(); }
            set {
                if (NTMinerRoot.GetIsShowCommandLine() != value) {
                    NTMinerRoot.SetIsShowCommandLine(value);
                    OnPropertyChanged(nameof(IsShowCommandLine));
                }
            }
        }

        public Guid CoinId {
            get => NTMinerRoot.Current.MinerProfile.CoinId;
            set {
                if (NTMinerRoot.Current.MinerProfile.CoinId != value) {
                    NTMinerRoot.Current.MinerProfile.SetMinerProfileProperty(nameof(CoinId), value);
                    OnPropertyChanged(nameof(CoinId));
                }
            }
        }

        public CoinViewModel CoinVm {
            get {
                if (!CoinViewModels.Current.TryGetCoinVm(this.CoinId, out CoinViewModel coinVm)) {
                    coinVm = CoinViewModels.Current.AllCoins.FirstOrDefault();
                    if (coinVm != null) {
                        CoinId = coinVm.Id;
                    }
                }
                return coinVm;
            }
            set {
                if (value == null) {
                    value = CoinViewModels.Current.MainCoins.OrderBy(a => a.SortNumber).FirstOrDefault();
                }
                if (value != null) {
                    this.CoinId = value.Id;
                    OnPropertyChanged(nameof(CoinVm));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public bool IsWorker {
            get {
                return MineWork != null && !VirtualRoot.IsControlCenter;
            }
        }
    }
}

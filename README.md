# 🔷 HostMonitor

<p align="center">
  <img src="Assets/hostmonitor-icon.svg" alt="HostMonitor Logo" width="150" height="150">
</p>

<p align="center">
  <strong>🔹 企業級主機監控系統</strong><br>
  <em>即時監控 • 視覺化圖表 • 智慧告警 • 系統托盤整合</em>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10">
  <img src="https://img.shields.io/badge/WPF-MVVM-0078D4?style=for-the-badge&logo=windows" alt="WPF MVVM">
  <img src="https://img.shields.io/badge/Material%20Design-5.x-2196F3?style=for-the-badge" alt="Material Design">
  <img src="https://img.shields.io/badge/License-MIT-00ACC1?style=for-the-badge" alt="License">
</p>

---

## 🔷 摘要

**HostMonitor** 是一款專為 IT 運維人員設計的 Windows 桌面應用程式，提供企業內部主機的即時健康狀態監控功能。系統採用現代化的 **MVVM 架構模式**，結合 **Material Design** 精美 UI，支援 **ICMP Ping** 和 **TCP Port** 雙重監控方式。

透過直覺化的操作介面，使用者可以輕鬆管理多台主機，即時查看回應時間圖表、命令執行日誌，並在主機離線時收到系統托盤通知。所有監控數據均以視覺化方式呈現，讓 IT 人員能夠快速掌握整體系統健康狀況。

### 🔹 核心亮點

| 特色 | 說明 |
|:---:|------|
| ⚡ | **即時監控** — 可配置 1-3600 秒監控間隔，毫秒級回應偵測 |
| 📊 | **視覺化圖表** — 即時折線圖顯示回應時間趨勢 |
| 🔔 | **智慧告警** — 離線自動通知，支援系統托盤氣泡提示 |
| 🎯 | **精準控制** — 個別主機可獨立啟用/停用監控 |
| 💾 | **資料持久化** — 主機配置自動保存至本地 JSON 檔案 |

---

## 🔷 功能特色

### 🔹 監控功能
- ✅ **ICMP Ping 監控** — 使用 ICMP 協議測試主機可達性和回應時間
- ✅ **TCP Port 監控** — 測試指定 TCP 端口的連線狀態（如 80, 443, 3389, 1433）
- ✅ **多方法監控** — 每台主機可同時配置多種監控方法和多個端口
- ✅ **可配置間隔** — 全域監控間隔設定，1-3600 秒自由調整
- ✅ **獨立控制** — 每台主機可個別啟用或停用監控
- ✅ **手動檢查** — 支援單一主機即時手動觸發檢查

### 🔹 視覺化功能
- 📊 **回應時間圖表** — 自訂折線圖控制項，顯示最近 30 次回應時間
- 🖥️ **命令日誌面板** — Mini Console 風格，顯示最近 200 條監控命令
- 🎨 **狀態顏色指示** — 🟢 在線 / 🔴 離線 / 🟠 警告 / 🔵 檢查中 / ⚪ 未知
- ⏳ **進度動畫** — 檢查中狀態顯示動態進度條
- 🔄 **自動滾動** — 命令日誌自動滾動至最新訊息

### 🔹 通知功能
- 💬 **Snackbar 通知** — 應用內即時通知，支援成功/警告/錯誤三種樣式
- 🔔 **系統托盤整合** — 最小化至托盤，右鍵選單快速控制
- 💡 **氣泡提示** — 主機離線時顯示 Windows 系統通知
- 🔁 **週期提醒** — 持續離線主機每 30 秒重複通知

### 🔹 管理功能
- 🏷️ **主機分類** — 支援 PC、DB、AP、FILE、WEB、API 六種類型標籤
- 📝 **CRUD 操作** — 完整的新增、編輯、刪除主機功能
- 🔍 **搜尋過濾** — 快速搜尋主機名稱
- ⚙️ **設定面板** — 可視化調整監控間隔

---

## 🔷 快速開始

### 🔹 系統需求

| 項目 | 需求 |
|------|------|
| **作業系統** | Windows 10 / 11 (x64) |
| **執行環境** | .NET 10.0 Runtime |
| **建議解析度** | 1920 × 1080 或更高 |
| **記憶體** | 建議 4GB 以上 |

### 🔹 安裝步驟

```bash
# 1. 複製專案
git clone https://github.com/anthropic/SGS.OAD.MonitR.git

# 2. 進入專案目錄
cd SGS.OAD.MonitR/HostMonitor

# 3. 還原 NuGet 套件
dotnet restore

# 4. 建置專案
dotnet build

# 5. 執行應用程式
dotnet run
```

### 🔹 首次使用指南

1. 🚀 啟動應用程式
2. ➕ 點擊左上角 **+** 按鈕新增主機
3. 📝 填寫主機資訊（名稱、Hostname/IP、類型）
4. ☑️ 選擇監控方式（Ping 和/或 TCP Port）
5. ▶️ 點擊右上角 **播放** 按鈕開始監控

---

## 🔷 專案架構

### 🔹 技術棧

| 類別 | 技術 | 版本 |
|------|------|------|
| **框架** | .NET | 10.0 |
| **UI 框架** | WPF (Windows Presentation Foundation) | - |
| **架構模式** | MVVM (Model-View-ViewModel) | - |
| **MVVM 工具** | CommunityToolkit.Mvvm | 8.x |
| **UI 設計** | MaterialDesignThemes | 5.x |
| **DI 容器** | Microsoft.Extensions.DependencyInjection | 10.x |
| **序列化** | System.Text.Json | Built-in |

### 🔹 專案結構

```
HostMonitor/
│
├── 📁 Models/                      # 資料模型層
│   ├── 📁 Enums/                   # 列舉型別
│   │   ├── HostStatus.cs           # 主機狀態 (Unknown/Online/Offline/Warning/Checking)
│   │   ├── HostType.cs             # 主機類型 (PC/DB/AP/FILE/WEB/API)
│   │   └── MonitorType.cs          # 監控類型 (IcmpPing/TcpPort)
│   ├── Host.cs                     # 主機模型 (ObservableObject)
│   ├── MonitorMethod.cs            # 監控方法配置
│   └── MonitorResult.cs            # 監控結果封裝
│
├── 📁 Services/                    # 服務層
│   ├── 📁 Interfaces/              # 服務介面
│   │   ├── IHostDataService.cs     # 主機資料服務介面
│   │   └── IMonitorService.cs      # 監控服務介面 (策略模式)
│   ├── 📁 Monitoring/              # 監控服務實作
│   │   ├── MonitorOrchestrator.cs  # 監控編排器 (協調者模式)
│   │   ├── PingMonitorService.cs   # ICMP Ping 監控
│   │   ├── TcpPortMonitorService.cs# TCP Port 監控
│   │   └── MonitorCommandEventArgs.cs
│   ├── HostDataService.cs          # 主機資料持久化 (JSON)
│   ├── NotificationService.cs      # 通知服務 (Snackbar + Toast)
│   ├── SettingsService.cs          # 設定服務
│   ├── NotificationKind.cs         # 通知類型列舉
│   └── NotificationEventArgs.cs    # 通知事件參數
│
├── 📁 ViewModels/                  # 視圖模型層
│   ├── MainViewModel.cs            # 主視窗 VM (監控狀態協調)
│   ├── HostListViewModel.cs        # 主機列表 VM (CRUD + 日誌)
│   ├── AddEditHostViewModel.cs     # 新增/編輯 VM (表單驗證)
│   └── SettingsViewModel.cs        # 設定 VM
│
├── 📁 Views/                       # 視圖層
│   ├── HostListView.xaml           # 主機列表視圖 (三欄佈局)
│   ├── AddEditHostDialog.xaml      # 新增/編輯對話框
│   ├── SettingsDialog.xaml         # 設定對話框
│   └── ConfirmDeleteDialog.xaml    # 刪除確認對話框
│
├── 📁 Messages/                    # MVVM 訊息 (WeakReferenceMessenger)
│   ├── HostChangedMessage.cs       # 主機變更通知
│   ├── OpenAddEditDialogMessage.cs # 開啟編輯對話框
│   ├── OpenSettingsDialogMessage.cs# 開啟設定對話框
│   ├── ConfirmDeleteHostMessage.cs # 刪除確認請求
│   └── CloseDialogMessage.cs       # 關閉對話框
│
├── 📁 Converters/                  # 值轉換器
│   ├── BoolToVisibilityConverter.cs
│   ├── InverseBoolConverter.cs
│   └── StatusToColorConverter.cs   # 狀態 → 顏色
│
├── 📁 Behaviors/                   # 附加行為
│   └── AutoScrollBehavior.cs       # 自動滾動行為
│
├── 📁 Controls/                    # 自訂控制項
│   └── ResponseTimeChart.cs        # 回應時間圖表 (Canvas + Polyline)
│
├── 📁 Assets/                      # 資源檔案
│   ├── hostmonitor-icon.svg        # 應用程式圖示
│   ├── hostmonitor-icon-small.svg
│   └── hostmonitor-icon-tray.svg
│
├── App.xaml                        # 應用程式定義
├── App.xaml.cs                     # 啟動邏輯 + DI 配置
├── MainWindow.xaml                 # 主視窗
└── MainWindow.xaml.cs              # 訊息路由中心
```

### 🔹 設計模式

| 模式 | 應用場景 | 實作位置 |
|------|---------|---------|
| **MVVM** | 整體架構 | ViewModels ↔ Views |
| **策略模式** | 監控服務 | `IMonitorService` → Ping/TCP |
| **觀察者模式** | 狀態更新 | `ObservableObject`, `ObservableCollection` |
| **命令模式** | UI 操作 | `RelayCommand` |
| **訊息模式** | 元件通訊 | `WeakReferenceMessenger` |
| **編排器模式** | 監控協調 | `MonitorOrchestrator` |
| **儲存庫模式** | 資料存取 | `IHostDataService` |
| **依賴注入** | 服務管理 | `ServiceCollection` |

---

## 🔷 系統架構圖

### 🔹 C4 Model — Level 1: System Context

> 系統上下文圖：展示 HostMonitor 與外部實體的互動關係

```mermaid
C4Context
    title System Context Diagram - HostMonitor

    Person(user, "IT 運維人員", "監控企業內部主機狀態<br/>接收離線告警通知")

    System(hostmonitor, "HostMonitor", "Windows 桌面監控應用程式<br/>即時監控主機健康狀態")

    System_Ext(targets, "監控目標主機", "企業內部伺服器群<br/>PC / DB / AP / FILE / WEB / API")

    System_Ext(filesystem, "本地檔案系統", "儲存主機配置資料<br/>%LocalAppData%\\HostMonitor")

    System_Ext(windows, "Windows 系統", "系統托盤整合<br/>氣泡通知")

    Rel(user, hostmonitor, "操作監控", "GUI")
    Rel(hostmonitor, targets, "健康檢查", "ICMP / TCP")
    Rel(hostmonitor, filesystem, "持久化", "JSON")
    Rel(hostmonitor, windows, "通知", "NotifyIcon")
```

### 🔹 C4 Model — Level 2: Container Diagram

> 容器圖：展示應用程式內部的主要分層架構

```mermaid
C4Container
    title Container Diagram - HostMonitor

    Person(user, "IT 運維人員")

    Container_Boundary(app, "HostMonitor Application") {
        Container(views, "Views Layer", "XAML + Code-behind", "使用者介面<br/>Material Design 樣式")

        Container(viewmodels, "ViewModel Layer", "C# + CommunityToolkit.Mvvm", "UI 邏輯、狀態管理<br/>命令處理、資料綁定")

        Container(services, "Service Layer", "C#", "業務邏輯<br/>監控執行、通知、設定")

        Container(models, "Model Layer", "C# POCO + ObservableObject", "資料模型<br/>Host, MonitorMethod, MonitorResult")
    }

    System_Ext(targets, "Target Hosts", "ICMP / TCP")
    System_Ext(fs, "File System", "JSON")

    Rel(user, views, "操作")
    Rel(views, viewmodels, "Data Binding<br/>Commands")
    Rel(viewmodels, services, "呼叫服務")
    Rel(services, models, "操作資料")
    Rel(services, targets, "網路監控")
    Rel(services, fs, "讀寫設定")
```

### 🔹 C4 Model — Level 3: Component Diagram

> 元件圖：展示各層的詳細元件及其依賴關係

```mermaid
flowchart TB
    subgraph Views["🖼️ Views Layer"]
        MW[MainWindow<br/>訊息路由中心]
        HLV[HostListView<br/>主機列表]
        AED[AddEditHostDialog<br/>新增/編輯]
        SD[SettingsDialog<br/>設定]
        CDD[ConfirmDeleteDialog<br/>刪除確認]
    end

    subgraph ViewModels["📋 ViewModel Layer"]
        MVM[MainViewModel<br/>監控狀態協調]
        HLVM[HostListViewModel<br/>列表管理 + 日誌]
        AEVM[AddEditHostViewModel<br/>表單驗證]
        SVM[SettingsViewModel<br/>設定管理]
    end

    subgraph Services["⚙️ Service Layer"]
        MO[MonitorOrchestrator<br/>監控編排器]
        PMS[PingMonitorService<br/>ICMP Ping]
        TPMS[TcpPortMonitorService<br/>TCP Port]
        HDS[HostDataService<br/>資料持久化]
        NS[NotificationService<br/>通知服務]
        SS[SettingsService<br/>設定服務]
    end

    subgraph Models["📦 Model Layer"]
        H[Host]
        MM[MonitorMethod]
        MR[MonitorResult]
        HS[HostStatus]
        HT[HostType]
        MT[MonitorType]
    end

    subgraph External["🌐 External"]
        TH[(Target Hosts)]
        FS[(File System<br/>hosts.json)]
    end

    MW --> MVM
    HLV --> HLVM
    AED --> AEVM
    SD --> SVM

    MVM --> HLVM
    MVM --> MO
    MVM --> NS
    MVM --> SVM

    HLVM --> HDS
    HLVM --> AEVM
    HLVM --> MO
    HLVM --> NS

    AEVM --> HDS
    AEVM --> NS

    SVM --> SS
    SVM --> NS

    MO --> PMS
    MO --> TPMS
    MO --> SS

    PMS --> MR
    TPMS --> MR

    HDS --> H
    HDS --> FS

    H --> MM
    H --> HS
    H --> HT
    MM --> MT

    PMS -.->|ICMP| TH
    TPMS -.->|TCP| TH

    style Views fill:#E3F2FD
    style ViewModels fill:#E8F5E9
    style Services fill:#FFF3E0
    style Models fill:#F3E5F5
    style External fill:#ECEFF1
```

---

## 🔷 流程圖

### 🔹 應用程式啟動流程

```mermaid
flowchart TD
    A[🚀 App.OnStartup] --> B[建立 ServiceCollection]
    B --> C[註冊 Singleton Services]
    C --> C1[IHostDataService]
    C --> C2[MonitorOrchestrator]
    C --> C3[NotificationService]
    C --> C4[SettingsService]

    C1 & C2 & C3 & C4 --> D[註冊 Transient Services]
    D --> D1[PingMonitorService]
    D --> D2[TcpPortMonitorService]

    D1 & D2 --> E[註冊 ViewModels]
    E --> E1[MainViewModel]
    E --> E2[HostListViewModel]
    E --> E3[AddEditHostViewModel]
    E --> E4[SettingsViewModel]

    E1 & E2 & E3 & E4 --> F[BuildServiceProvider]
    F --> G[GetRequiredService MainWindow]
    G --> H[MainWindow 建構]
    H --> I[注入 MainViewModel]
    I --> J[MainViewModel 初始化]
    J --> K[載入 Hosts from JSON]
    K --> L[初始化系統托盤 NotifyIcon]
    L --> M[註冊 MVVM Messages]
    M --> N[MainWindow.Show]
    N --> O[✅ 應用程式就緒]

    style A fill:#2196F3,color:#fff
    style O fill:#4CAF50,color:#fff
```

### 🔹 監控執行流程

```mermaid
flowchart TD
    A[▶️ 點擊開始監控] --> B{IsMonitoring?}
    B -->|Yes| Z[⏹️ 結束]
    B -->|No| C[IsMonitoring = true]

    C --> D[建立 CancellationTokenSource]
    D --> E[訂閱 MonitorResultReceived 事件]
    E --> F[遍歷所有 IsMonitoringEnabled = true 的主機]

    F --> G[Host.CurrentStatus = Checking]
    G --> H[清空 ResponseTimeHistory]
    H --> I[新增初始點 0]
    I --> J[呼叫 StartMonitoringAsync]

    J --> K[遍歷主機的 MonitorMethods]
    K --> L[啟動 RunMonitorLoopAsync]

    L --> M[ExecuteCheckAsync 初始檢查]
    M --> N[發送 MonitorCommandIssued 事件]
    N --> O[IMonitorService.CheckAsync]

    O --> P{監控類型?}
    P -->|IcmpPing| Q[Ping.SendPingAsync]
    P -->|TcpPort| R[TcpClient.ConnectAsync]

    Q & R --> S[建立 MonitorResult]
    S --> T[發送 MonitorResultReceived 事件]

    T --> U[MainViewModel.ApplyResult]
    U --> V[聚合所有方法結果]
    V --> W{計算狀態}

    W -->|全部成功| X1[🟢 Online]
    W -->|全部失敗| X2[🔴 Offline]
    W -->|部分失敗| X3[🟠 Warning]

    X1 & X2 & X3 --> Y[更新 Host 屬性]
    Y --> Y1[AverageResponseTimeMs]
    Y --> Y2[LastCheckTime]
    Y --> Y3[LastErrorMessage]

    Y1 & Y2 & Y3 --> AA{狀態變化?}
    AA -->|Offline| AB[🔔 ShowError 通知]
    AA -->|Online from Offline| AC[✅ ShowSuccess 通知]
    AA -->|No Change| AD[繼續]

    AB & AC & AD --> AE{已取消?}
    AE -->|Yes| Z
    AE -->|No| AF[等待 IntervalSeconds]
    AF --> M

    style A fill:#2196F3,color:#fff
    style Z fill:#9E9E9E,color:#fff
    style X1 fill:#4CAF50,color:#fff
    style X2 fill:#F44336,color:#fff
    style X3 fill:#FF9800,color:#fff
```

### 🔹 主機狀態判斷邏輯

```mermaid
flowchart TD
    A[📥 收到 MonitorResult] --> B[取得主機的所有 MonitorMethods]
    B --> C[過濾 IsEnabled = true 的方法]
    C --> D[建立 expectedKeys 列表]

    D --> E{expectedKeys.Count == 0?}
    E -->|Yes| F[⚪ Unknown<br/>無啟用的監控方法]

    E -->|No| G{所有 expectedKeys<br/>都有結果?}
    G -->|No| H[🔵 Checking<br/>等待其他方法結果]

    G -->|Yes| I[計算 successCount 和 failureCount]
    I --> J{successCount == total?}

    J -->|Yes| K[🟢 Online<br/>所有檢查通過]
    J -->|No| L{failureCount == total?}

    L -->|Yes| M[🔴 Offline<br/>所有檢查失敗]
    L -->|No| N[🟠 Warning<br/>部分檢查失敗]

    K & M & N --> O[計算 AverageResponseTimeMs]
    O --> P[收集 ErrorMessages]
    P --> Q[更新 Host 屬性]

    F & H & Q --> R[🔄 觸發 UI 更新]

    style F fill:#9E9E9E,color:#fff
    style H fill:#2196F3,color:#fff
    style K fill:#4CAF50,color:#fff
    style M fill:#F44336,color:#fff
    style N fill:#FF9800,color:#fff
```

### 🔹 新增主機流程

```mermaid
flowchart TD
    A[➕ 點擊新增主機] --> B[HostListViewModel.AddHostCommand]
    B --> C[AddEditHostViewModel.ResetForm]
    C --> D[發送 OpenAddEditDialogMessage]

    D --> E[MainWindow 接收訊息]
    E --> F[DialogHost.Show AddEditHostDialog]

    F --> G[👤 使用者填寫表單]
    G --> H[點擊儲存按鈕]
    H --> I[SaveCommand.Execute]

    I --> J{CanSave 驗證}
    J -->|❌ 失敗| K[顯示錯誤提示]
    K --> G

    J -->|✅ 通過| L[BuildMonitorMethods]
    L --> M[建立 Host 物件]
    M --> N[HostDataService.AddHost]

    N --> O[_hosts.Add host]
    O --> P[SaveHosts → hosts.json]

    P --> Q[發送 HostChangedMessage]
    Q --> R[發送 CloseDialogMessage]
    R --> S[DialogHost.Close]

    Q --> T[HostListViewModel 接收]
    T --> U[Hosts.Add host]
    U --> V[SelectedHost = host]
    V --> W[ShowSuccess 通知]

    Q --> X[MainViewModel 接收]
    X --> Y{IsMonitoring &&<br/>host.IsMonitoringEnabled?}
    Y -->|Yes| Z[自動開始監控]
    Y -->|No| AA[完成]

    W & Z --> AA

    style A fill:#2196F3,color:#fff
    style AA fill:#4CAF50,color:#fff
```

---

## 🔷 序列圖

### 🔹 監控執行序列

```mermaid
sequenceDiagram
    autonumber
    box rgb(227, 242, 253) User Interface
        actor User
        participant MW as MainWindow
    end

    box rgb(232, 245, 233) ViewModel Layer
        participant MVM as MainViewModel
        participant HLVM as HostListViewModel
    end

    box rgb(255, 243, 224) Service Layer
        participant MO as MonitorOrchestrator
        participant PMS as PingMonitorService
        participant NS as NotificationService
    end

    box rgb(236, 239, 241) External
        participant Host as Target Host
    end

    User->>MW: 點擊 ▶️ 開始監控
    MW->>MVM: StartMonitoringCommand

    MVM->>MVM: IsMonitoring = true
    MVM->>MVM: 建立 CancellationTokenSource
    MVM->>MO: Subscribe MonitorResultReceived

    loop 每個啟用的主機
        MVM->>MVM: host.CurrentStatus = Checking
        MVM->>MO: StartMonitoringAsync(host)

        loop 每個監控方法
            MO->>MO: RunMonitorLoopAsync()

            loop 監控循環
                MO->>HLVM: Event: MonitorCommandIssued
                HLVM->>HLVM: AppendCommand 到 CommandLog

                MO->>PMS: CheckAsync(host, method)
                PMS->>Host: ICMP Ping
                Host-->>PMS: Reply (25ms)
                PMS-->>MO: MonitorResult

                MO->>MVM: Event: MonitorResultReceived
                MO->>HLVM: Event: MonitorResultReceived

                MVM->>MVM: ApplyResult 聚合結果
                MVM->>MVM: 更新 host.CurrentStatus

                alt 狀態變為 Offline
                    MVM->>NS: ShowError("主機離線")
                    NS-->>User: Snackbar 通知
                    NS-->>User: 系統托盤氣泡
                end

                HLVM->>HLVM: AppendResponse 到 CommandLog
                HLVM->>HLVM: host.AddResponseTime 更新圖表

                MO->>MO: Delay(IntervalSeconds)
            end
        end
    end

    User->>MW: 點擊 ⏹️ 停止監控
    MW->>MVM: StopMonitoringCommand
    MVM->>MO: CancellationToken.Cancel
    MVM->>MVM: IsMonitoring = false
```

### 🔹 新增主機序列

```mermaid
sequenceDiagram
    autonumber

    actor User
    participant HLV as HostListView
    participant HLVM as HostListViewModel
    participant AEVM as AddEditHostViewModel
    participant MW as MainWindow
    participant HDS as HostDataService
    participant FS as FileSystem
    participant MVM as MainViewModel

    User->>HLV: 點擊 ➕ 新增
    HLV->>HLVM: AddHostCommand

    HLVM->>AEVM: ResetForm()
    AEVM->>AEVM: 清空所有欄位

    HLVM->>MW: Send OpenAddEditDialogMessage
    MW->>MW: ShowAddEditDialogAsync()
    MW-->>User: 顯示 AddEditHostDialog

    User->>AEVM: 填寫表單資料
    Note over User,AEVM: Name, Hostname, IP,<br/>HostType, MonitorMethods

    User->>AEVM: 點擊儲存
    AEVM->>AEVM: CanSave() 驗證

    alt 驗證失敗
        AEVM-->>User: 顯示錯誤提示
    else 驗證成功
        AEVM->>AEVM: BuildMonitorMethods()
        AEVM->>AEVM: 建立 Host 物件

        AEVM->>HDS: AddHost(host)
        HDS->>HDS: _hosts.Add(host)
        HDS->>FS: SaveHosts() → hosts.json
        FS-->>HDS: 寫入成功

        AEVM->>MW: Send HostChangedMessage(host, isEdit=false)
        AEVM->>MW: Send CloseDialogMessage
        MW->>MW: DialogHost.Close()

        par 並行處理訊息
            MW->>HLVM: HostChangedMessage
            HLVM->>HLVM: Hosts.Add(host)
            HLVM->>HLVM: SelectedHost = host
            HLVM-->>User: ShowSuccess("已新增主機")
        and
            MW->>MVM: HostChangedMessage
            alt IsMonitoring && host.IsMonitoringEnabled
                MVM->>MVM: StartMonitoringHostAsync(host)
            end
        end
    end
```

### 🔹 設定變更序列

```mermaid
sequenceDiagram
    autonumber

    actor User
    participant MW as MainWindow
    participant MVM as MainViewModel
    participant SVM as SettingsViewModel
    participant SS as SettingsService
    participant MO as MonitorOrchestrator

    User->>MW: 點擊 ⚙️ 設定
    MW->>MVM: OpenSettingsCommand

    MVM->>SVM: Load()
    SVM->>SS: get MonitorIntervalSeconds
    SS-->>SVM: 5 (當前值)

    MVM->>MW: Send OpenSettingsDialogMessage
    MW->>MW: ShowSettingsDialogAsync()
    MW-->>User: 顯示 SettingsDialog

    User->>SVM: 調整滑桿至 10 秒
    Note over User,SVM: Slider 雙向綁定<br/>MonitorIntervalSeconds = 10

    User->>SVM: 點擊儲存
    SVM->>SS: TrySetInterval(10)
    SS->>SS: 驗證範圍 1-3600
    SS->>SS: MonitorIntervalSeconds = 10
    SS-->>SVM: true (成功)

    SVM-->>User: ShowSuccess("設定已儲存")
    SVM->>MW: Send CloseDialogMessage
    MW->>MW: DialogHost.Close()

    Note over MO: 下次監控循環
    MO->>SS: get MonitorIntervalSeconds
    SS-->>MO: 10 (新值)
    MO->>MO: Task.Delay(10 秒)
    Note over MO: 間隔變更立即生效
```

---

## 🔷 資料模型

### 🔹 核心模型

```csharp
// Host.cs - 主機模型
public class Host : ObservableObject
{
    public Guid Id { get; set; }                          // 唯一識別碼
    public string Name { get; set; }                      // 顯示名稱
    public string HostnameOrIp { get; set; }              // 監控目標位址
    public string Hostname { get; set; }                  // 主機名稱
    public string? IpAddress { get; set; }                // IP 位址 (選填)
    public HostType Type { get; set; }                    // 主機類型
    public List<MonitorMethod> MonitorMethods { get; set; }// 監控方法列表
    public HostStatus CurrentStatus { get; set; }         // 當前狀態
    public DateTime? LastCheckTime { get; set; }          // 最後檢查時間
    public double? AverageResponseTimeMs { get; set; }    // 平均回應時間
    public string? LastErrorMessage { get; set; }         // 最後錯誤訊息
    public bool IsMonitoringEnabled { get; set; }         // 是否啟用監控
    public ObservableCollection<string> CommandLog { get; }      // 命令日誌 (max 200)
    public ObservableCollection<double> ResponseTimeHistory { get; } // 回應時間歷史 (max 30)
}

// MonitorMethod.cs - 監控方法配置
public class MonitorMethod
{
    public MonitorType Type { get; set; }     // 監控類型
    public bool IsEnabled { get; set; }       // 是否啟用
    public int? Port { get; set; }            // TCP 端口 (TcpPort 類型使用)
    public int TimeoutMs { get; set; } = 5000;// 超時時間 (毫秒)
    public int IntervalSeconds { get; set; } = 5; // 檢查間隔 (秒)
}

// MonitorResult.cs - 監控結果
public class MonitorResult
{
    public Guid HostId { get; set; }          // 主機 ID
    public MonitorType MonitorType { get; set; }// 監控類型
    public bool IsSuccess { get; set; }       // 是否成功
    public long ResponseTimeMs { get; set; }  // 回應時間 (毫秒)
    public DateTime CheckTime { get; set; }   // 檢查時間
    public string? ErrorMessage { get; set; } // 錯誤訊息
    public int? Port { get; set; }            // TCP 端口
}
```

### 🔹 列舉型別

```csharp
// HostStatus.cs
public enum HostStatus
{
    Unknown,   // ⚪ 未知
    Online,    // 🟢 在線
    Offline,   // 🔴 離線
    Warning,   // 🟠 警告 (部分檢查失敗)
    Checking   // 🔵 檢查中
}

// HostType.cs
public enum HostType
{
    PC,    // 個人電腦
    DB,    // 資料庫伺服器
    AP,    // 應用程式伺服器
    FILE,  // 檔案伺服器
    WEB,   // Web 伺服器
    API    // API 伺服器
}

// MonitorType.cs
public enum MonitorType
{
    IcmpPing,  // ICMP Ping
    TcpPort    // TCP 端口
}
```

---

## 🔷 設定與儲存

### 🔹 資料儲存位置

```
%LocalAppData%\HostMonitor\hosts.json
```

### 🔹 hosts.json 格式範例

```json
[
  {
    "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "Name": "Web Server",
    "HostnameOrIp": "webserver.local",
    "Hostname": "webserver.local",
    "IpAddress": "192.168.1.100",
    "Type": "WEB",
    "MonitorMethods": [
      {
        "Type": "IcmpPing",
        "IsEnabled": true,
        "Port": null,
        "TimeoutMs": 5000,
        "IntervalSeconds": 5
      },
      {
        "Type": "TcpPort",
        "IsEnabled": true,
        "Port": 443,
        "TimeoutMs": 5000,
        "IntervalSeconds": 5
      }
    ]
  },
  {
    "Id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "Name": "Database Server",
    "HostnameOrIp": "192.168.1.50",
    "Hostname": "dbserver",
    "IpAddress": "192.168.1.50",
    "Type": "DB",
    "MonitorMethods": [
      {
        "Type": "IcmpPing",
        "IsEnabled": true,
        "Port": null,
        "TimeoutMs": 5000,
        "IntervalSeconds": 5
      },
      {
        "Type": "TcpPort",
        "IsEnabled": true,
        "Port": 1433,
        "TimeoutMs": 5000,
        "IntervalSeconds": 5
      }
    ]
  }
]
```

---

## 🔷 特殊技術實作

### 🔹 自動滾動行為 (AutoScrollBehavior)

```csharp
// 使用附加屬性實現可重用的自動滾動
public static class AutoScrollBehavior
{
    public static readonly DependencyProperty EnableProperty =
        DependencyProperty.RegisterAttached(...);

    // 監聽 CollectionChanged 事件，自動滾動到底部
    private static void OnCollectionChanged(...)
    {
        var scrollViewer = FindScrollViewer(itemsControl);
        scrollViewer?.ScrollToEnd();
    }
}
```

**XAML 使用方式：**
```xml
<ListBox behaviors:AutoScrollBehavior.Enable="True"
         ItemsSource="{Binding CommandLog}" />
```

### 🔹 自訂圖表控制項 (ResponseTimeChart)

```csharp
// 使用 WPF 原生繪圖，不依賴第三方庫
public class ResponseTimeChart : UserControl
{
    private readonly Canvas _canvas;
    private readonly Polyline _line;  // 折線
    private readonly Polygon _fill;   // 填充區域

    // 監聽資料變化重繪圖表
    private void UpdateChart()
    {
        // 1. 計算資料範圍
        // 2. 正規化座標
        // 3. 更新 Polyline 和 Polygon 點集
    }
}
```

### 🔹 非同步確認對話框

```csharp
// 使用 TaskCompletionSource 實現同步等待
var completion = new TaskCompletionSource<bool>();
Messenger.Send(new ConfirmDeleteHostMessage(host, completion));
var confirmed = await completion.Task;  // 等待使用者確認
```

---

## 🔷 授權條款

本專案採用 **MIT 授權條款**。

```
MIT License

Copyright (c) 2024 SGS OAD Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software...
```

---

## 🔷 貢獻指南

我們歡迎所有形式的貢獻！

### 🔹 如何貢獻

1. **Fork** 本專案
2. 建立功能分支 (`git checkout -b feature/amazing-feature`)
3. 提交變更 (`git commit -m 'Add amazing feature'`)
4. 推送分支 (`git push origin feature/amazing-feature`)
5. 建立 **Pull Request**

### 🔹 程式碼規範

- 遵循 C# 命名慣例
- 使用 `///` XML 文件註解
- 維持 MVVM 架構分離
- 新增功能請附帶單元測試

---

## 🔷 聯絡方式

| 管道 | 連結 |
|------|------|
| **專案維護** | SGS OAD Team |
| **問題回報** | [GitHub Issues](https://github.com/anthropic/SGS.OAD.MonitR/issues) |
| **功能建議** | [GitHub Discussions](https://github.com/anthropic/SGS.OAD.MonitR/discussions) |

---

<p align="center">
  <img src="Assets/hostmonitor-icon-small.svg" alt="HostMonitor" width="48" height="48">
  <br><br>
  <strong>HostMonitor</strong><br>
  <em>Made with 💙 by SGS OAD Team</em>
</p>

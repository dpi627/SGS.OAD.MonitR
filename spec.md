# 主機監控系統 - 軟體開發規格書

## 一、專案概述

### 1.1 專案名稱
HostMonitor - 多協定主機監控系統

### 1.2 技術堆疊
- **.NET 10** (WPF)
- **CommunityToolkit.Mvvm** 8.x
- **MaterialDesignThemes** 5.x
- **MaterialDesignColors** 3.x

### 1.3 開發原則
- 嚴格遵循 MVVM 模式
- 依賴注入 (Dependency Injection)
- 關注點分離 (Separation of Concerns)
- 單一職責原則 (Single Responsibility Principle)

---

## 二、系統架構

### 2.1 專案結構

```
HostMonitor/
├── Models/                          # 資料模型
│   ├── Host.cs
│   ├── MonitorMethod.cs
│   ├── MonitorResult.cs
│   └── Enums/
│       ├── HostType.cs
│       ├── MonitorType.cs
│       └── HostStatus.cs
│
├── Services/                        # 服務層
│   ├── Interfaces/
│   │   ├── IMonitorService.cs
│   │   ├── IHostDataService.cs
│   │   └── INotificationService.cs
│   ├── Monitoring/
│   │   ├── PingMonitorService.cs
│   │   ├── TcpPortMonitorService.cs
│   │   └── MonitorOrchestrator.cs
│   └── HostDataService.cs
│
├── ViewModels/                      # 視圖模型
│   ├── MainViewModel.cs
│   ├── HostListViewModel.cs
│   ├── AddEditHostViewModel.cs
│   └── MonitorStatusViewModel.cs
│
├── Views/                           # 視圖
│   ├── MainWindow.xaml
│   ├── HostListView.xaml
│   └── AddEditHostDialog.xaml
│
├── Converters/                      # 值轉換器
│   ├── StatusToColorConverter.cs
│   └── BoolToVisibilityConverter.cs
│
└── App.xaml.cs                      # 應用程式進入點
```

---

## 三、資料模型設計

### 3.1 核心模型

#### Host.cs
```csharp
public class Host
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string HostnameOrIp { get; set; }
    public HostType Type { get; set; }
    public List<MonitorMethod> MonitorMethods { get; set; }
    public HostStatus CurrentStatus { get; set; }
    public DateTime? LastCheckTime { get; set; }
    public string LastErrorMessage { get; set; }
}
```

#### MonitorMethod.cs
```csharp
public class MonitorMethod
{
    public MonitorType Type { get; set; }
    public bool IsEnabled { get; set; }
    public int? Port { get; set; }  // 僅用於 TCP 監測
    public int TimeoutMs { get; set; } = 5000;
    public int IntervalSeconds { get; set; } = 60;
}
```

#### MonitorResult.cs
```csharp
public class MonitorResult
{
    public Guid HostId { get; set; }
    public MonitorType MonitorType { get; set; }
    public bool IsSuccess { get; set; }
    public long ResponseTimeMs { get; set; }
    public DateTime CheckTime { get; set; }
    public string ErrorMessage { get; set; }
}
```

### 3.2 列舉定義

#### HostType.cs
```csharp
public enum HostType
{
    WindowsPC,
    DatabaseServer,
    ApplicationServer,
    FileServer
}
```

#### MonitorType.cs
```csharp
public enum MonitorType
{
    IcmpPing,
    TcpPort
}
```

#### HostStatus.cs
```csharp
public enum HostStatus
{
    Unknown,
    Online,
    Offline,
    Warning,
    Checking
}
```

---

## 四、服務層設計

### 4.1 監控服務介面

#### IMonitorService.cs
```csharp
public interface IMonitorService
{
    MonitorType SupportedType { get; }
    Task<MonitorResult> CheckAsync(Host host, MonitorMethod method, CancellationToken cancellationToken = default);
}
```

### 4.2 監控服務實作

#### PingMonitorService.cs
**職責**: ICMP Ping 監測
**功能**:
- 發送 ICMP Echo Request
- 測量回應時間
- 處理超時與異常

**關鍵實作**:
```csharp
public class PingMonitorService : IMonitorService
{
    public MonitorType SupportedType => MonitorType.IcmpPing;
    
    public async Task<MonitorResult> CheckAsync(
        Host host, 
        MonitorMethod method, 
        CancellationToken cancellationToken = default)
    {
        // 使用 System.Net.NetworkInformation.Ping
        // 實作 ICMP 檢測邏輯
    }
}
```

#### TcpPortMonitorService.cs
**職責**: TCP 端口連接測試
**功能**:
- 嘗試建立 TCP 連接
- 支援自訂端口
- 測量連接時間

**關鍵實作**:
```csharp
public class TcpPortMonitorService : IMonitorService
{
    public MonitorType SupportedType => MonitorType.TcpPort;
    
    public async Task<MonitorResult> CheckAsync(
        Host host, 
        MonitorMethod method, 
        CancellationToken cancellationToken = default)
    {
        // 使用 TcpClient
        // 實作端口檢測邏輯
    }
}
```

### 4.3 監控協調器

#### MonitorOrchestrator.cs
**職責**: 協調多種監控服務
**功能**:
- 管理所有監控服務實例
- 根據 MonitorMethod 選擇對應服務
- 執行並彙總監控結果
- 管理監控排程

**關鍵方法**:
```csharp
public class MonitorOrchestrator
{
    private readonly IEnumerable<IMonitorService> _monitorServices;
    private readonly Dictionary<Guid, PeriodicTimer> _timers;
    
    public async Task StartMonitoringAsync(Host host);
    public void StopMonitoring(Guid hostId);
    public async Task<List<MonitorResult>> CheckHostAsync(Host host);
    
    public event EventHandler<MonitorResult> MonitorResultReceived;
}
```

### 4.4 資料服務

#### IHostDataService.cs
```csharp
public interface IHostDataService
{
    ObservableCollection<Host> GetAllHosts();
    Host GetHostById(Guid id);
    void AddHost(Host host);
    void UpdateHost(Host host);
    void DeleteHost(Guid id);
}
```

#### HostDataService.cs
**職責**: 主機資料管理（記憶體儲存）
**功能**:
- 使用 `ObservableCollection<Host>` 儲存資料
- 提供 CRUD 操作
- 開發期間僅記憶體儲存

---

## 五、ViewModel 設計

### 5.1 MainViewModel.cs

**職責**: 主視窗協調器
**屬性**:
```csharp
[ObservableProperty]
private HostListViewModel hostListViewModel;

[ObservableProperty]
private bool isMonitoring;
```

**命令**:
```csharp
[RelayCommand]
private async Task StartMonitoringAsync();

[RelayCommand]
private void StopMonitoring();

[RelayCommand]
private async Task ShowAddHostDialogAsync();
```

### 5.2 HostListViewModel.cs

**職責**: 主機列表管理
**屬性**:
```csharp
public ObservableCollection<Host> Hosts { get; }

[ObservableProperty]
private Host selectedHost;

[ObservableProperty]
private string searchText;
```

**命令**:
```csharp
[RelayCommand]
private async Task AddHostAsync();

[RelayCommand]
private async Task EditHostAsync(Host host);

[RelayCommand]
private async Task DeleteHostAsync(Host host);

[RelayCommand]
private async Task ManualCheckAsync(Host host);
```

**過濾功能**:
```csharp
private ICollectionView _hostsView;
// 根據 SearchText 和 HostType 過濾
```

### 5.3 AddEditHostViewModel.cs

**職責**: 新增/編輯主機對話框
**屬性**:
```csharp
[ObservableProperty]
private string name;

[ObservableProperty]
private string hostnameOrIp;

[ObservableProperty]
private HostType selectedHostType;

[ObservableProperty]
private bool enablePingMonitor = true;

[ObservableProperty]
private bool enableTcpMonitor;

[ObservableProperty]
private ObservableCollection<int> tcpPorts = new() { 80 };

[ObservableProperty]
private int newPort = 443;
```

**命令**:
```csharp
[RelayCommand]
private void AddPort();

[RelayCommand]
private void RemovePort(int port);

[RelayCommand(CanExecute = nameof(CanSave))]
private void Save();

[RelayCommand]
private void Cancel();
```

**驗證**:
```csharp
private bool CanSave()
{
    return !string.IsNullOrWhiteSpace(Name) 
        && !string.IsNullOrWhiteSpace(HostnameOrIp)
        && (EnablePingMonitor || EnableTcpMonitor);
}
```

### 5.4 MonitorStatusViewModel.cs

**職責**: 單一主機監控狀態顯示
**屬性**:
```csharp
[ObservableProperty]
private Host host;

[ObservableProperty]
private ObservableCollection<MonitorResult> recentResults;

[ObservableProperty]
private string statusText;

[ObservableProperty]
private Brush statusColor;
```

---

## 六、UI 設計規格

### 6.1 MainWindow.xaml

**佈局**:
```
┌─────────────────────────────────────────┐
│  HostMonitor         [開始監控] [停止]   │
├─────────────────────────────────────────┤
│ [+ 新增主機]  [搜尋: ________] [篩選▼]  │
├─────────────────────────────────────────┤
│  主機列表                                │
│  ┌───────────────────────────────────┐ │
│  │ ● Server01 (192.168.1.10)        │ │
│  │   Windows PC | 正常 | 12ms       │ │
│  │   [編輯] [刪除] [立即檢查]        │ │
│  ├───────────────────────────────────┤ │
│  │ ● DB-Server (10.0.0.5)           │ │
│  │   Database Server | 離線          │ │
│  │   [編輯] [刪除] [立即檢查]        │ │
│  └───────────────────────────────────┘ │
└─────────────────────────────────────────┘
```

**Material Design 元件**:
- `md:Card` - 主機項目卡片
- `md:Chip` - 主機類型標籤
- `md:ColorZone` - 標題列
- `md:DialogHost` - 對話框容器
- `md:ProgressBar` - 檢查進度
- `md:Snackbar` - 通知訊息

### 6.2 AddEditHostDialog.xaml

**欄位**:
- 主機名稱 (必填)
- 主機位址/IP (必填，驗證格式)
- 主機類型 (下拉選單)
- **監控方式** (CheckBox 群組):
  - ☑ ICMP Ping
  - ☐ TCP 端口監測
- **TCP 端口設定** (啟用時顯示):
  - 端口列表 (Chips 顯示，可刪除)
  - 新增端口輸入框 + 按鈕
  - 預設值: 80

**驗證規則**:
- 主機位址: IP 格式或有效主機名稱
- 至少選擇一種監控方式
- TCP 端口: 1-65535

### 6.3 狀態視覺化

**狀態顏色**:
- **Online**: 綠色 (`#4CAF50`)
- **Offline**: 紅色 (`#F44336`)
- **Warning**: 橘色 (`#FF9800`)
- **Checking**: 藍色 (`#2196F3`)
- **Unknown**: 灰色 (`#9E9E9E`)

**動畫**:
- 檢查中: 脈動效果
- 狀態變更: 淡入淡出轉場

---

## 七、功能規格

### 7.1 主機管理

#### F1: 新增主機
- 開啟對話框輸入主機資訊
- 選擇主機類型
- 選擇監控方式（可複選）
- TCP 監控時設定端口（預設 80）
- 儲存到記憶體

#### F2: 編輯主機
- 載入現有主機資訊
- 修改所有欄位
- 更新監控設定

#### F3: 刪除主機
- 顯示確認對話框
- 停止該主機的監控
- 從記憶體移除

#### F4: 主機列表顯示
- 卡片式佈局
- 顯示主機名稱、IP、類型
- 即時狀態顯示（Online/Offline）
- 最後檢查時間
- 回應時間（ms）

### 7.2 監控功能

#### F5: 自動監控
- 點擊「開始監控」啟動所有主機
- 根據每個 MonitorMethod 的 IntervalSeconds 定期檢查
- 即時更新 UI 狀態

#### F6: 手動檢查
- 點擊主機的「立即檢查」按鈕
- 顯示檢查進度
- 更新結果

#### F7: 多重監控方式
- 同一主機可同時使用 Ping 和多個 TCP 端口
- 所有方式都成功才顯示 Online
- 任一方式失敗顯示 Offline/Warning

#### F8: 端口管理
- 預設端口: 80
- 可新增多個端口（80, 443, 3389, 8080 等）
- 每個端口獨立監測
- 可動態新增/移除端口

### 7.3 通知功能

#### F9: 狀態變更通知
- 使用 Material Design Snackbar
- Online → Offline: 紅色通知
- Offline → Online: 綠色通知
- 可配置音效提示

---

## 八、技術實作細節

### 8.1 依賴注入設定

**App.xaml.cs**:
```csharp
public partial class App : Application
{
    private ServiceProvider _serviceProvider;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // Services
        services.AddSingleton<IHostDataService, HostDataService>();
        services.AddSingleton<MonitorOrchestrator>();
        services.AddTransient<IMonitorService, PingMonitorService>();
        services.AddTransient<IMonitorService, TcpPortMonitorService>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<HostListViewModel>();
        services.AddTransient<AddEditHostViewModel>();
        
        // Views
        services.AddTransient<MainWindow>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
```

### 8.2 監控執行流程

```
1. User 點擊「開始監控」
   ↓
2. MainViewModel.StartMonitoringAsync()
   ↓
3. MonitorOrchestrator.StartMonitoringAsync(host)
   ↓
4. 為每個 Host 建立 PeriodicTimer
   ↓
5. 根據 IntervalSeconds 定期執行
   ↓
6. CheckHostAsync(host)
   ↓
7. 遍歷 host.MonitorMethods
   ↓
8. 根據 MonitorType 選擇對應 IMonitorService
   ↓
9. 執行 CheckAsync()
   ↓
10. 回傳 MonitorResult
   ↓
11. 觸發 MonitorResultReceived 事件
   ↓
12. ViewModel 更新 UI (ObservableProperty)
```

### 8.3 MVVM 最佳實踐

#### 通訊機制
- ViewModel → View: `ObservableProperty`, `ObservableCollection`
- View → ViewModel: `RelayCommand`
- ViewModel ↔ ViewModel: `WeakReferenceMessenger` (CommunityToolkit.Mvvm)

#### 範例: 主機新增通知
```csharp
// AddEditHostViewModel.cs
WeakReferenceMessenger.Default.Send(new HostAddedMessage(newHost));

// HostListViewModel.cs
WeakReferenceMessenger.Default.Register<HostAddedMessage>(this, (r, m) =>
{
    Hosts.Add(m.Host);
});
```

### 8.4 非同步操作

所有 I/O 操作使用 `async/await`:
- 監控檢查
- 對話框顯示
- 資料載入

使用 `CancellationTokenSource` 支援取消:
```csharp
private CancellationTokenSource _cts;

[RelayCommand]
private async Task StartMonitoringAsync()
{
    _cts = new CancellationTokenSource();
    await _orchestrator.StartMonitoringAsync(_cts.Token);
}

[RelayCommand]
private void StopMonitoring()
{
    _cts?.Cancel();
}
```

---

## 九、資料流程圖

### 9.1 新增主機流程
```
User → MainWindow → [+ 新增主機] 
  → DialogHost.Show(AddEditHostDialog)
  → AddEditHostViewModel (輸入資料)
  → [儲存] RelayCommand
  → IHostDataService.AddHost()
  → WeakReferenceMessenger.Send(HostAddedMessage)
  → HostListViewModel 接收
  → Hosts.Add(newHost)
  → UI 更新
```

### 9.2 監控執行流程
```
User → [開始監控]
  → MainViewModel.StartMonitoringAsync()
  → foreach Host in Hosts
    → MonitorOrchestrator.StartMonitoringAsync(host)
    → 建立 PeriodicTimer(IntervalSeconds)
    → Loop:
      → CheckHostAsync(host)
      → foreach MonitorMethod
        → 選擇 IMonitorService
        → CheckAsync() → MonitorResult
      → 彙總結果
      → MonitorResultReceived 事件
      → HostListViewModel 更新 Host.CurrentStatus
      → UI 更新（顏色、文字、時間）
```

---

## 十、開發階段規劃

### Phase 1: 基礎架構 (Week 1)
- [ ] 建立專案結構
- [ ] 定義所有 Models 和 Enums
- [ ] 實作 IHostDataService (記憶體版本)
- [ ] 設定 DI Container

### Phase 2: 監控服務 (Week 2)
- [ ] 實作 PingMonitorService
- [ ] 實作 TcpPortMonitorService
- [ ] 實作 MonitorOrchestrator
- [ ] 單元測試

### Phase 3: UI 與 ViewModel (Week 3)
- [ ] MainWindow 與 MainViewModel
- [ ] HostListView 與 ViewModel
- [ ] AddEditHostDialog 與 ViewModel
- [ ] 套用 Material Design Theme

### Phase 4: 整合與優化 (Week 4)
- [ ] 整合所有模組
- [ ] 實作通知機制
- [ ] 狀態視覺化
- [ ] 效能優化
- [ ] 使用者測試

---

## 十一、測試策略

### 11.1 單元測試
- **Services**: 使用 Moq 模擬網路回應
- **ViewModels**: 測試命令執行與屬性變更
- **Converters**: 測試轉換邏輯

### 11.2 整合測試
- 監控服務與協調器整合
- ViewModel 與 Service 互動

### 11.3 手動測試案例
- [ ] 新增主機（各種類型）
- [ ] 編輯主機資訊
- [ ] 刪除主機
- [ ] 啟動/停止監控
- [ ] 手動立即檢查
- [ ] 多端口設定
- [ ] 狀態變更通知
- [ ] 主機離線/上線切換

---

## 十二、未來擴充方向

### 可擴充的監控服務
- HTTP/HTTPS 健康檢查
- Windows 事件日誌查詢
- WMI 服務狀態檢查
- 資料庫連線測試

### 持久化儲存
- JSON 檔案儲存
- SQLite 本地資料庫
- 設定檔匯入/匯出

### 進階功能
- 歷史紀錄與圖表
- 告警規則設定
- Email/Teams 通知
- 排程停用（維護視窗）

---

## 十三、開發檢查清單

### MVVM Best Practices
- [ ] View 不含任何商業邏輯
- [ ] ViewModel 不引用 View
- [ ] 使用 `ObservableProperty` 而非手動實作 INotifyPropertyChanged
- [ ] 使用 `RelayCommand` 而非手動實作 ICommand
- [ ] 所有集合使用 `ObservableCollection`
- [ ] 使用 `WeakReferenceMessenger` 進行跨 ViewModel 通訊

### 程式碼品質
- [ ] 每個類別單一職責
- [ ] 介面隔離（小而專精的介面）
- [ ] 依賴注入所有服務
- [ ] 非同步方法命名以 `Async` 結尾
- [ ] 適當的異常處理
- [ ] XML 文件註解

### Material Design
- [ ] 使用 Material Design 調色板
- [ ] 正確使用 Card, Chip, Button 等元件
- [ ] 一致的間距與字體
- [ ] 支援深色/淺色主題切換

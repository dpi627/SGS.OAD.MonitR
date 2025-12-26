```markdown
# HostMonitor Development Tasks

## Project Setup

### TASK-001: Create Project Structure
**Goal**: Initialize .NET 10 WPF project with proper folder structure

**Steps**:
1. Create new WPF .NET 10 project named "HostMonitor"
2. Create folder structure:
   - Models/
   - Models/Enums/
   - Services/
   - Services/Interfaces/
   - Services/Monitoring/
   - ViewModels/
   - Views/
   - Converters/
3. Install NuGet packages:
   - CommunityToolkit.Mvvm (8.x)
   - MaterialDesignThemes (5.x)
   - MaterialDesignColors (3.x)
   - Microsoft.Extensions.DependencyInjection (10.x)
4. Verify project compiles successfully

**Deliverables**:
- Empty folder structure
- Package references in .csproj
- Successful compilation

---

## Phase 1: Core Models and Enums

### TASK-002: Implement Enums
**Goal**: Create all enumeration types

**Files to Create**:
- `Models/Enums/HostType.cs`
  - WindowsPC, DatabaseServer, ApplicationServer, FileServer
- `Models/Enums/MonitorType.cs`
  - IcmpPing, TcpPort
- `Models/Enums/HostStatus.cs`
  - Unknown, Online, Offline, Warning, Checking

**Requirements**:
- Use proper enum syntax
- Add XML documentation comments

**Verify**: Project compiles without errors

---

### TASK-003: Implement Core Models
**Goal**: Create data models for host monitoring

**Files to Create**:

1. `Models/MonitorMethod.cs`
```csharp
Properties:
- MonitorType Type
- bool IsEnabled
- int? Port (nullable, for TCP only)
- int TimeoutMs (default: 5000)
- int IntervalSeconds (default: 60)
```

2. `Models/Host.cs`
```csharp
Properties:
- Guid Id
- string Name
- string HostnameOrIp
- HostType Type
- List<MonitorMethod> MonitorMethods (initialize as new List)
- HostStatus CurrentStatus (default: Unknown)
- DateTime? LastCheckTime
- string LastErrorMessage
```

3. `Models/MonitorResult.cs`
```csharp
Properties:
- Guid HostId
- MonitorType MonitorType
- bool IsSuccess
- long ResponseTimeMs
- DateTime CheckTime
- string ErrorMessage
```

**Requirements**:
- All properties with public getters and setters
- Initialize collections in constructors where needed
- Add XML documentation

**Verify**: Project compiles without errors

---

## Phase 2: Service Interfaces

### TASK-004: Create Service Interfaces
**Goal**: Define contracts for all services

**Files to Create**:

1. `Services/Interfaces/IMonitorService.cs`
```csharp
Interface with:
- MonitorType SupportedType { get; }
- Task<MonitorResult> CheckAsync(Host host, MonitorMethod method, CancellationToken cancellationToken = default)
```

2. `Services/Interfaces/IHostDataService.cs`
```csharp
Interface with:
- ObservableCollection<Host> GetAllHosts()
- Host GetHostById(Guid id)
- void AddHost(Host host)
- void UpdateHost(Host host)
- void DeleteHost(Guid id)
```

**Requirements**:
- Use System.Collections.ObjectModel for ObservableCollection
- Proper async Task signatures
- XML documentation

**Verify**: Project compiles without errors

---

## Phase 3: Data Service Implementation

### TASK-005: Implement HostDataService
**Goal**: In-memory data storage for hosts

**File to Create**: `Services/HostDataService.cs`

**Implementation Requirements**:
- Implement IHostDataService interface
- Use private `ObservableCollection<Host> _hosts` field
- Initialize in constructor
- GetAllHosts(): return _hosts
- GetHostById(Guid id): use LINQ to find
- AddHost(Host host): 
  - Generate new Guid for Id if empty
  - Add to collection
- UpdateHost(Host host): 
  - Find existing by Id
  - Update all properties
- DeleteHost(Guid id): 
  - Find and remove from collection

**Verify**: Project compiles without errors

---

## Phase 4: Monitor Service Implementations

### TASK-006: Implement PingMonitorService
**Goal**: ICMP ping monitoring

**File to Create**: `Services/Monitoring/PingMonitorService.cs`

**Implementation Requirements**:
- Implement IMonitorService interface
- SupportedType returns MonitorType.IcmpPing
- CheckAsync implementation:
  - Use System.Net.NetworkInformation.Ping
  - Create Ping instance in using statement
  - Send ping with timeout from method.TimeoutMs
  - Handle PingReply:
    - Success: reply.Status == IPStatus.Success
    - ResponseTimeMs: reply.RoundtripTime
  - Handle exceptions (catch all):
    - IsSuccess = false
    - ErrorMessage = exception.Message
  - Return MonitorResult with all fields populated

**Required usings**:
- System.Net.NetworkInformation
- System.Diagnostics

**Verify**: Project compiles without errors

---

### TASK-007: Implement TcpPortMonitorService
**Goal**: TCP port connectivity monitoring

**File to Create**: `Services/Monitoring/TcpPortMonitorService.cs`

**Implementation Requirements**:
- Implement IMonitorService interface
- SupportedType returns MonitorType.TcpPort
- CheckAsync implementation:
  - Validate method.Port has value, throw if null
  - Use System.Net.Sockets.TcpClient
  - Use Stopwatch to measure connection time
  - Try ConnectAsync with timeout using CancellationTokenSource
  - Success: connection established
  - ResponseTimeMs: stopwatch elapsed milliseconds
  - Handle SocketException and TaskCanceledException:
    - IsSuccess = false
    - ErrorMessage = exception.Message
  - Always dispose TcpClient
  - Return MonitorResult with all fields populated

**Required usings**:
- System.Net.Sockets
- System.Diagnostics

**Verify**: Project compiles without errors

---

### TASK-008: Implement MonitorOrchestrator
**Goal**: Coordinate multiple monitoring services

**File to Create**: `Services/Monitoring/MonitorOrchestrator.cs`

**Implementation Requirements**:
- Constructor injection: `IEnumerable<IMonitorService> monitorServices`
- Private fields:
  - `Dictionary<MonitorType, IMonitorService> _serviceMap` (initialize in constructor from enumerable)
  - `Dictionary<Guid, CancellationTokenSource> _monitoringTasks`
  - `Dictionary<Guid, List<Task>> _activeTasks`
- Public event: `EventHandler<MonitorResult> MonitorResultReceived`

**Methods**:

1. `public async Task StartMonitoringAsync(Host host, CancellationToken cancellationToken = default)`
   - Stop existing monitoring for this host if any
   - Create new CancellationTokenSource
   - Store in _monitoringTasks
   - For each enabled MonitorMethod in host.MonitorMethods:
     - Create Task that loops with PeriodicTimer (method.IntervalSeconds)
     - Execute CheckAsync
     - Invoke MonitorResultReceived event
   - Store tasks in _activeTasks

2. `public void StopMonitoring(Guid hostId)`
   - Cancel CancellationTokenSource from _monitoringTasks
   - Remove from dictionaries

3. `public async Task<List<MonitorResult>> CheckHostAsync(Host host, CancellationToken cancellationToken = default)`
   - Loop through enabled MonitorMethods
   - Get appropriate IMonitorService from _serviceMap
   - Call CheckAsync
   - Collect all results
   - Return list

4. `private IMonitorService GetMonitorService(MonitorType type)`
   - Return from _serviceMap, throw if not found

**Verify**: Project compiles without errors

---

## Phase 5: Base ViewModels

### TASK-009: Setup DI Container in App.xaml.cs
**Goal**: Configure dependency injection

**File to Modify**: `App.xaml.cs`

**Implementation Requirements**:
- Add private field `ServiceProvider _serviceProvider`
- Override OnStartup:
  - Create ServiceCollection
  - Register services as Singleton:
    - IHostDataService -> HostDataService
    - MonitorOrchestrator
  - Register IMonitorService implementations as Transient:
    - PingMonitorService
    - TcpPortMonitorService
  - Register ViewModels as Transient (will add in next tasks):
    - MainViewModel
    - HostListViewModel
    - AddEditHostViewModel
  - Register MainWindow as Transient
  - Build ServiceProvider
  - Resolve and show MainWindow

**Required usings**:
- Microsoft.Extensions.DependencyInjection

**Verify**: Project compiles without errors (MainWindow already exists by default)

---

### TASK-010: Implement MainViewModel
**Goal**: Main window coordinator

**File to Create**: `ViewModels/MainViewModel.cs`

**Implementation Requirements**:
- Inherit from ObservableObject
- Constructor injection:
  - HostListViewModel hostListViewModel
  - MonitorOrchestrator orchestrator
- Observable properties:
  - HostListViewModel HostListViewModel
  - bool IsMonitoring (default: false)
- Private fields:
  - MonitorOrchestrator _orchestrator
  - CancellationTokenSource _monitoringCts

**Commands**:

1. `[RelayCommand]` StartMonitoringAsync():
   - Set IsMonitoring = true
   - Create new CancellationTokenSource
   - Loop through all hosts in HostListViewModel.Hosts
   - Call _orchestrator.StartMonitoringAsync for each
   - Subscribe to _orchestrator.MonitorResultReceived event
   - Handle exceptions

2. `[RelayCommand]` StopMonitoring():
   - Cancel _monitoringCts
   - Loop through all hosts and call _orchestrator.StopMonitoring
   - Set IsMonitoring = false

3. MonitorResultReceived event handler:
   - Find host by HostId
   - Update host.CurrentStatus based on result.IsSuccess
   - Update host.LastCheckTime
   - Update host.LastErrorMessage

**Required usings**:
- CommunityToolkit.Mvvm.ComponentModel
- CommunityToolkit.Mvvm.Input

**Verify**: Project compiles without errors

---

### TASK-011: Implement HostListViewModel
**Goal**: Manage host list and operations

**File to Create**: `ViewModels/HostListViewModel.cs`

**Implementation Requirements**:
- Inherit from ObservableObject
- Constructor injection: IHostDataService hostDataService
- Public property: `ObservableCollection<Host> Hosts` (get from hostDataService)
- Observable properties:
  - Host SelectedHost
  - string SearchText

**Commands**:

1. `[RelayCommand]` AddHost():
   - Set up for opening AddEditHostDialog (implementation will come later)
   - For now, just prepare the structure

2. `[RelayCommand]` EditHost(Host host):
   - Validate host is not null
   - Prepare for editing (implementation later)

3. `[RelayCommand]` DeleteHost(Host host):
   - Validate host is not null
   - Call _hostDataService.DeleteHost(host.Id)

4. `[RelayCommand]` ManualCheck(Host host):
   - Placeholder for manual check (will integrate with orchestrator later)

**Verify**: Project compiles without errors

---

### TASK-012: Implement AddEditHostViewModel
**Goal**: Add/Edit host dialogViewModel

**File to Create**: `ViewModels/AddEditHostViewModel.cs`

**Implementation Requirements**:
- Inherit from ObservableObject
- Constructor injection: IHostDataService hostDataService
- Observable properties:
  - string Name
  - string HostnameOrIp
  - HostType SelectedHostType (default: HostType.WindowsPC)
  - bool EnablePingMonitor (default: true)
  - bool EnableTcpMonitor (default: false)
  - ObservableCollection<int> TcpPorts (initialize with { 80 })
  - int NewPort (default: 443)
  - bool IsEditMode (default: false)
- Private field: Guid? _editingHostId

**Methods**:

1. `public void LoadHost(Host host)` - for edit mode:
   - Set IsEditMode = true
   - Store _editingHostId
   - Load all properties from host
   - Load MonitorMethods into EnablePingMonitor, EnableTcpMonitor, TcpPorts

**Commands**:

1. `[RelayCommand]` AddPort():
   - Validate NewPort (1-65535)
   - Check not already in TcpPorts
   - Add to TcpPorts

2. `[RelayCommand]` RemovePort(int port):
   - Remove from TcpPorts

3. `[RelayCommand(CanExecute = nameof(CanSave))]` Save():
   - Create or update Host object
   - Create MonitorMethod list based on checkboxes
   - If IsEditMode: call UpdateHost
   - Else: call AddHost
   - Send success message via WeakReferenceMessenger

4. `[RelayCommand]` Cancel():
   - Reset all properties
   - Send cancel message

**CanSave() method**:
- Return true if Name and HostnameOrIp not empty
- And at least one monitor enabled

**Verify**: Project compiles without errors

---

## Phase 6: Basic UI Implementation

### TASK-013: Setup Material Design in App.xaml
**Goal**: Configure Material Design themes

**File to Modify**: `App.xaml`

**Implementation Requirements**:
- Add Material Design resource dictionaries in Application.Resources:
  - materialDesign:BundledTheme (BaseTheme="Light", PrimaryColor="Blue", SecondaryColor="Amber")
  - MaterialDesignThemes.Wpf.themes.Generic
  - materialDesignColors.Recommended

**Required xmlns**:
```xml
xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
```

**Verify**: Project compiles without XAML errors

---

### TASK-014: Implement MainWindow.xaml
**Goal**: Main application window

**File to Modify**: `Views/MainWindow.xaml` (or `MainWindow.xaml` if at root)

**Layout Requirements**:
- Set DataContext to MainViewModel (via DI)
- Window properties:
  - Title="HostMonitor"
  - Width="1200" Height="800"
  - Background="{DynamicResource MaterialDesignPaper}"
  - TextElement.Foreground="{DynamicResource MaterialDesignBody}"

**Structure**:
```xml
<materialDesign:DialogHost>
  <DockPanel>
    <!-- Top bar with ColorZone -->
    <materialDesign:ColorZone DockPanel.Dock="Top" Mode="PrimaryMid" Padding="16">
      <DockPanel>
        <TextBlock Text="HostMonitor" FontSize="22" VerticalAlignment="Center"/>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
          <Button Content="開始監控" 
                  Command="{Binding StartMonitoringCommand}"
                  IsEnabled="{Binding IsMonitoring, Converter={StaticResource InverseBoolConverter}}"
                  Margin="8,0"/>
          <Button Content="停止監控" 
                  Command="{Binding StopMonitoringCommand}"
                  IsEnabled="{Binding IsMonitoring}"
                  Margin="8,0"/>
        </StackPanel>
      </DockPanel>
    </materialDesign:ColorZone>
    
    <!-- Content area -->
    <ContentControl Content="{Binding HostListViewModel}" DockPanel.Dock="Top"/>
  </DockPanel>
</materialDesign:DialogHost>
```

**Note**: InverseBoolConverter will be created in next task

**Verify**: Project compiles, window structure visible (may need converters)

---

### TASK-015: Implement Value Converters
**Goal**: Create necessary value converters

**Files to Create**:

1. `Converters/InverseBoolConverter.cs`
   - Implement IValueConverter
   - Convert: return !(bool)value
   - ConvertBack: return !(bool)value

2. `Converters/StatusToColorConverter.cs`
   - Implement IValueConverter
   - Convert HostStatus to Brush:
     - Online: #4CAF50 (Green)
     - Offline: #F44336 (Red)
     - Warning: #FF9800 (Orange)
     - Checking: #2196F3 (Blue)
     - Unknown: #9E9E9E (Grey)
   - Use SolidColorBrush with ColorConverter.ConvertFromString

3. `Converters/BoolToVisibilityConverter.cs`
   - Implement IValueConverter
   - Convert bool to Visibility
   - true -> Visible, false -> Collapsed

**Add converters to App.xaml Resources**:
```xml
<Application.Resources>
  <local:InverseBoolConverter x:Key="InverseBoolConverter"/>
  <local:StatusToColorConverter x:Key="StatusToColorConverter"/>
  <local:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
</Application.Resources>
```

**Verify**: Project compiles without errors

---

### TASK-016: Implement HostListView.xaml
**Goal**: Display host list

**File to Create**: `Views/HostListView.xaml`

**Implementation Requirements**:
- UserControl with DataContext expecting HostListViewModel
- Layout structure:

```xml
<UserControl>
  <DockPanel Margin="16">
    <!-- Top toolbar -->
    <StackPanel DockPanel.Dock="Top" Orientation="Horizontal" Margin="0,0,0,16">
      <Button Content="新增主機" 
              Command="{Binding AddHostCommand}"
              Style="{StaticResource MaterialDesignRaisedButton}"/>
      <TextBox materialDesign:HintAssist.Hint="搜尋主機..."
               Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"
               Width="200" Margin="16,0"/>
    </StackPanel>
    
    <!-- Host list -->
    <ScrollViewer VerticalScrollBarVisibility="Auto">
      <ItemsControl ItemsSource="{Binding Hosts}">
        <ItemsControl.ItemTemplate>
          <DataTemplate>
            <materialDesign:Card Margin="0,0,0,8" Padding="16">
              <Grid>
                <Grid.ColumnDefinitions>
                  <ColumnDefinition Width="*"/>
                  <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                
                <!-- Host info -->
                <StackPanel Grid.Column="0">
                  <TextBlock Text="{Binding Name}" FontSize="18" FontWeight="Bold"/>
                  <TextBlock Text="{Binding HostnameOrIp}" FontSize="14" Opacity="0.7"/>
                  <StackPanel Orientation="Horizontal" Margin="0,8,0,0">
                    <materialDesign:Chip Content="{Binding Type}" Margin="0,0,8,0"/>
                    <TextBlock Text="{Binding CurrentStatus}" 
                               Foreground="{Binding CurrentStatus, Converter={StaticResource StatusToColorConverter}}"
                               FontWeight="Bold"/>
                  </StackPanel>
                </StackPanel>
                
                <!-- Action buttons -->
                <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                  <Button Content="編輯" 
                          Command="{Binding DataContext.EditHostCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          CommandParameter="{Binding}"
                          Margin="4"/>
                  <Button Content="刪除"
                          Command="{Binding DataContext.DeleteHostCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          CommandParameter="{Binding}"
                          Margin="4"/>
                  <Button Content="立即檢查"
                          Command="{Binding DataContext.ManualCheckCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          CommandParameter="{Binding}"
                          Margin="4"/>
                </StackPanel>
              </Grid>
            </materialDesign:Card>
          </DataTemplate>
        </ItemsControl.ItemTemplate>
      </ItemsControl>
    </ScrollViewer>
  </DockPanel>
</UserControl>
```

**Verify**: Project compiles without XAML errors

---

### TASK-017: Implement AddEditHostDialog.xaml
**Goal**: Add/Edit host dialog

**File to Create**: `Views/AddEditHostDialog.xaml`

**Implementation Requirements**:
- UserControl with DataContext expecting AddEditHostViewModel
- Dialog structure:

```xml
<UserControl Width="500">
  <StackPanel Margin="16">
    <TextBlock Text="新增/編輯主機" Style="{StaticResource MaterialDesignHeadline5TextBlock}" Margin="0,0,0,16"/>
    
    <!-- Host Name -->
    <TextBox materialDesign:HintAssist.Hint="主機名稱*"
             Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"
             Margin="0,0,0,8"/>
    
    <!-- Hostname or IP -->
    <TextBox materialDesign:HintAssist.Hint="主機位址/IP*"
             Text="{Binding HostnameOrIp, UpdateSourceTrigger=PropertyChanged}"
             Margin="0,0,0,8"/>
    
    <!-- Host Type -->
    <ComboBox materialDesign:HintAssist.Hint="主機類型"
              ItemsSource="{Binding Source={StaticResource HostTypeValues}}"
              SelectedItem="{Binding SelectedHostType}"
              Margin="0,0,0,16"/>
    
    <!-- Monitor Methods -->
    <TextBlock Text="監控方式" FontWeight="Bold" Margin="0,0,0,8"/>
    <CheckBox Content="ICMP Ping" IsChecked="{Binding EnablePingMonitor}" Margin="0,0,0,4"/>
    <CheckBox Content="TCP 端口監測" IsChecked="{Binding EnableTcpMonitor}" Margin="0,0,0,8"/>
    
    <!-- TCP Ports (visible when EnableTcpMonitor is true) -->
    <StackPanel Visibility="{Binding EnableTcpMonitor, Converter={StaticResource BoolToVisibilityConverter}}">
      <TextBlock Text="監測端口" FontWeight="Bold" Margin="0,0,0,8"/>
      <ItemsControl ItemsSource="{Binding TcpPorts}">
        <ItemsControl.ItemsPanel>
          <ItemsPanelTemplate>
            <WrapPanel/>
          </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
          <DataTemplate>
            <materialDesign:Chip Content="{Binding}" 
                                 IsDeletable="True"
                                 DeleteCommand="{Binding DataContext.RemovePortCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                 DeleteCommandParameter="{Binding}"
                                 Margin="4"/>
          </DataTemplate>
        </ItemsControl.ItemTemplate>
      </ItemsControl>
      
      <StackPanel Orientation="Horizontal" Margin="0,8,0,0">
        <TextBox materialDesign:HintAssist.Hint="新端口"
                 Text="{Binding NewPort}"
                 Width="100"
                 Margin="0,0,8,0"/>
        <Button Content="新增" Command="{Binding AddPortCommand}"/>
      </StackPanel>
    </StackPanel>
    
    <!-- Action Buttons -->
    <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,24,0,0">
      <Button Content="取消" 
              Command="{Binding CancelCommand}"
              Style="{StaticResource MaterialDesignFlatButton}"
              Margin="0,0,8,0"/>
      <Button Content="儲存" 
              Command="{Binding SaveCommand}"
              Style="{StaticResource MaterialDesignRaisedButton}"/>
    </StackPanel>
  </StackPanel>
</UserControl>
```

**Add ObjectDataProvider for enum in Resources**:
```xml
<ObjectDataProvider x:Key="HostTypeValues" MethodName="GetValues" ObjectType="{x:Type sys:Enum}">
  <ObjectDataProvider.MethodParameters>
    <x:Type TypeName="local:HostType"/>
  </ObjectDataProvider.MethodParameters>
</ObjectDataProvider>
```

**Verify**: Project compiles without XAML errors

---

## Phase 7: Integration and Dialog Wiring

### TASK-018: Wire AddEditHostDialog to MainWindow
**Goal**: Connect dialog to host list operations

**Files to Modify**:
1. `ViewModels/HostListViewModel.cs`
2. `ViewModels/AddEditHostViewModel.cs`

**Changes in HostListViewModel**:
- Add field: `private readonly AddEditHostViewModel _addEditHostViewModel`
- Add to constructor injection
- Update AddHostCommand:
  - Reset _addEditHostViewModel
  - Send message to open dialog via WeakReferenceMessenger
- Update EditHostCommand:
  - Call _addEditHostViewModel.LoadHost(host)
  - Send message to open dialog

**Changes in AddEditHostViewModel**:
- Update SaveCommand:
  - After save, send HostChangedMessage via WeakReferenceMessenger
  - Clear properties
- Update CancelCommand:
  - Just clear properties

**Create Message classes**:
- Create `Messages/OpenAddEditDialogMessage.cs`
- Create `Messages/HostChangedMessage.cs`

**Changes in MainWindow.xaml.cs code-behind**:
- Register for OpenAddEditDialogMessage
- Show dialog using MaterialDesignThemes.Wpf.DialogHost.Show()
- Pass AddEditHostDialog as content

**Changes in HostListViewModel**:
- Register for HostChangedMessage
- Refresh host list or update specific host

**Verify**: Can open dialog, add host, see it in list

---

### TASK-019: Implement Manual Check Functionality
**Goal**: Allow manual host checking

**File to Modify**: `ViewModels/HostListViewModel.cs`

**Changes**:
- Add constructor injection: MonitorOrchestrator orchestrator
- Update ManualCheckCommand implementation:
  - Set host.CurrentStatus = HostStatus.Checking
  - Call orchestrator.CheckHostAsync(host)
  - Update host with results
  - Handle exceptions

**Verify**: Manual check button works and updates host status

---

### TASK-020: Complete Monitoring Integration
**Goal**: Full monitoring loop with UI updates

**File to Modify**: `ViewModels/MainViewModel.cs`

**Enhancements**:
- In MonitorResultReceived handler:
  - Use Application.Current.Dispatcher.Invoke for UI thread updates
  - Update host properties:
    - CurrentStatus (Online if all results success, Offline if any failed)
    - LastCheckTime = DateTime.Now
    - LastErrorMessage (concatenate all error messages)
  
**Add computed status logic**:
- If host has multiple MonitorMethods:
  - Aggregate results per host
  - Online: all methods succeed
  - Warning: some methods succeed
  - Offline: all methods fail

**Verify**: 
- Start monitoring updates UI in real-time
- Status colors change correctly
- Stop monitoring works

---

### TASK-021: Enhance UI Polish
**Goal**: Improve user experience

**Changes**:

1. Add loading indicators in `HostListView.xaml`:
   - Show ProgressBar when host.CurrentStatus == Checking
   - Add to Card template

2. Add last check time display:
   - TextBlock showing host.LastCheckTime formatted
   - Show "未檢查" if null

3. Add response time display:
   - Store average ResponseTimeMs in Host model (add property)
   - Display in UI

4. Add empty state message:
   - When Hosts.Count == 0, show friendly message with Add Host button

5. Add confirmation dialog for delete:
   - Use MaterialDesignThemes DialogHost for confirmation

**Verify**: UI looks polished and provides good feedback

---

### TASK-022: Add Notification System
**Goal**: Show notifications for status changes

**File to Create**: `Services/NotificationService.cs`

**Implementation**:
- Add Snackbar to MainWindow.xaml in DialogHost
- Create NotificationService with methods:
  - ShowSuccess(string message)
  - ShowError(string message)
  - ShowWarning(string message)
- Inject into MainViewModel
- Show notifications on:
  - Host goes offline
  - Host comes back online
  - Monitoring started/stopped
  - Host added/updated/deleted

**Verify**: Notifications appear at bottom of window

---

### TASK-023: Add System Tray Icon (Optional Enhancement)
**Goal**: Allow app to run in background

**Implementation**:
- Add System.Drawing reference for NotifyIcon
- Create NotifyIcon in App.xaml.cs
- Add context menu: Show/Hide, Start/Stop Monitoring, Exit
- Minimize to tray instead of closing
- Show balloon notifications for status changes

**Verify**: App can minimize to tray and restore

---

### TASK-024: Final Polish and Bug Fixes
**Goal**: Ensure smooth operation

**Checklist**:
- Verify all commands have proper CanExecute logic
- Ensure proper disposal of CancellationTokenSources
- Handle edge cases (empty host list, invalid IP format, etc.)
- Add input validation to AddEditHostDialog
- Ensure thread-safe UI updates (use Dispatcher)
- Test rapid start/stop monitoring
- Test with multiple hosts
- Verify memory doesn't leak (dispose timers properly)

**Verify**: App runs smoothly without crashes or UI freezes

---

## Completion Criteria

Each task must:
1. ✅ Compile without errors
2. ✅ Not break existing functionality
3. ✅ Follow MVVM pattern strictly
4. ✅ Use Material Design components correctly
5. ✅ Handle exceptions gracefully

After all tasks complete, the application should:
- ✅ Allow adding/editing/deleting hosts
- ✅ Support ICMP Ping monitoring
- ✅ Support TCP port monitoring (single or multiple ports)
- ✅ Show real-time status updates
- ✅ Provide manual check option
- ✅ Display host information clearly
- ✅ Use Material Design theming
- ✅ Store data in memory during session
```
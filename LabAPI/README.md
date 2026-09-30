# KeycardInventoryBypass (LabAPI)

这是 LabAPI 版本。玩家与门交互时，如果手持物品没有开门权限，插件会检查背包中的钥匙卡；找到具备该门全部权限的卡后允许开门。锁住的门、已取消的交互和无钥匙卡权限要求的门不会被改动。Gate A/B 使用 `ExitGates` 权限，`ScpOverride` 不作为钥匙卡要求。

## 构建

需要 .NET SDK、LabAPI 1.1.7 的 NuGet 包，以及与服务器版本一致的 SCP:SL 服务端 `SCPSL_Data/Managed` 目录。

```powershell
dotnet build .\LabAPI\KeycardInventoryBypass.LabAPI.csproj -c Release -p:ScpslReferences="C:\path\to\SCPSL_Data\Managed"
```

GitHub Release 会同时提供 Exiled 与 LabAPI 两个 DLL。本地构建产物位于 `LabAPI/bin/Release/net48/KeycardInventoryBypass.LabAPI.dll`。服务器自带 LabAPI；部署时只复制 `KeycardInventoryBypass-LabAPI.dll` 到 `%AppData%\SCP Secret Laboratory\LabAPI\plugins\<端口号>\`（或 `global`），不要复制 NuGet 依赖或构建目录里的 PDB。

背包中的地表通行证仅在实际成功开门后按游戏原生规则消耗，不能用于关闭大门。优先使用可重复使用的钥匙卡，避免不必要地消耗通行证；手持一次性通行证时由游戏原生逻辑处理。

## 配置

LabAPI 在 `%AppData%\SCP Secret Laboratory\LabAPI\configs\<端口号>\KeycardInventoryBypass\config.yml` 管理配置：

```yaml
is_enabled: true
debug: false
```

`is_enabled` 控制是否注册门交互事件，`debug` 控制调试日志。修改配置后重新加载插件或重启服务器。

不要同时安装本仓库的 Exiled 版和 LabAPI 版，以免两个插件同时处理同一次开门交互。

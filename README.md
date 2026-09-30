# KeycardInventoryBypass

让玩家无需切换手持物品，即可使用背包里的钥匙卡权限开门。项目为 SCP: Secret Laboratory 提供 **Exiled** 和 **LabAPI** 两种实现。

[下载 1.1.1](https://github.com/jikekei/KeycardInventoryBypass/releases/tag/1.1.1) · [查看更新记录](CHANGELOG.md) · [MIT License](LICENSE)

## 选择版本

| 服务器框架 | 适用版本 | 下载 | 安装位置 |
| --- | --- | --- | --- |
| Exiled | Exiled 9.6.0 或更高版本 | [下载 Exiled DLL](https://github.com/jikekei/KeycardInventoryBypass/releases/download/1.1.1/KeycardInventoryBypass-Exiled.dll) | `Exiled/Plugins` |
| LabAPI | LabAPI 1.1.7 | [下载 LabAPI DLL](https://github.com/jikekei/KeycardInventoryBypass/releases/download/1.1.1/KeycardInventoryBypass-LabAPI.dll) | `%AppData%\SCP Secret Laboratory\LabAPI\plugins\<端口号>\` 或 `global` |

按服务器正在使用的框架下载并安装对应 DLL。**不要同时安装两个版本。** 安装后重启服务器，或使用对应框架的插件重载功能。

## 功能说明

当玩家手持的物品没有开门权限时，插件会检查其背包中的钥匙卡；若其中一张卡满足该门所需权限，就允许开门。插件保留游戏原有的门禁权限检查。

LabAPI 版还会保留锁定门和已取消交互的原有行为；Gate A/B 使用 `ExitGates` 权限，`ScpOverride` 不作为钥匙卡要求。更多 LabAPI 行为与构建说明见 [LabAPI 使用指南](LabAPI/README.md)。

背包中的地表通行证仅在实际成功开门后按游戏原生规则消耗，不能用于关闭大门。优先使用可重复使用的钥匙卡，避免不必要地消耗通行证；手持一次性通行证时由游戏原生逻辑处理。

## 配置

Exiled 配置由插件写入 `Exiled/Configs/{端口号}-config.yml`：

```yaml
keycard_inventory_bypass:
  is_enabled: true
  debug: false
```

- `is_enabled`：启用或停用插件。
- `debug`：输出调试日志；排查问题时可开启。

LabAPI 版使用独立配置文件，位置和配置方式见 [LabAPI 使用指南](LabAPI/README.md#配置)。

## 许可证

本项目采用 [MIT License](LICENSE)。

# KeycardInventoryBypass

## 项目简介

KeycardInventoryBypass 是一个用于 SCPSL SCP: Secret Laboratory 的 Exiled 插件，允许玩家在无需手持钥匙卡的情况下，直接使用背包中的钥匙卡完成开门操作。该插件在不破坏原有权限机制的前提下，显著提升了玩家的操作流畅度与游戏体验。

## 核心特性

- 🚪 背包钥匙卡生效：无需切换到手持状态即可开门
- ⚡ 无感知增强：不影响原版权限判断与游戏逻辑
- 🔧 即装即用：无需额外配置，启用后立即生效

## 环境要求

- Exiled 9.6.0 或更高版本

## 安装方法

1. 下载最新版本的 KeycardInventoryBypass.dll
2. 将 DLL 文件放入服务器目录：`Exiled/Plugins`
3. 重启服务器或使用 Exiled 提供的插件热重载命令加载插件

## 配置说明

插件配置文件位于：`Exiled/Configs/{端口号}-config.yml`

默认配置如下：

```yaml
keycard_inventory_bypass:
  is_enabled: true
  debug: false
```

### 配置项说明

- **is_enabled**：是否启用插件（true / false）
- **debug**：是否输出调试日志，用于排查问题（建议仅在测试环境开启）

## 工作原理

插件通过监听玩家的开门交互事件（InteractingDoor）实现功能增强，流程如下：

1. 玩家尝试与门进行交互
2. 插件检测该门是否需要钥匙卡权限
3. 遍历玩家背包中的所有物品
4. 判断是否存在满足权限要求的钥匙卡
5. 若匹配成功，则允许开门，无需手持钥匙卡

整个过程对玩家完全透明，不会改变原有门禁权限逻辑。

## 贡献方式

欢迎任何形式的贡献 🎉

- 提交 Issue：反馈 Bug 或提出功能建议
- 提交 Pull Request：改进代码或新增功能

## 许可证

本项目采用 MIT License，详情请参阅 [LICENSE](LICENSE) 文件

## 致谢


感谢所有使用、测试并支持本插件的玩家与服务器管理员 ❤️

## LabAPI 版本

仓库的 [LabAPI 目录](LabAPI/README.md) 提供独立的 LabAPI 1.1.7 实现及构建、安装说明。现有根目录项目仍为 Exiled 版本。两种版本只需安装其中一种。

`1.1.0` GitHub Release 会同时提供 Exiled 与 LabAPI DLL，请按服务器安装的框架选择对应文件。

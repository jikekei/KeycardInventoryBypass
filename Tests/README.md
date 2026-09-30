# 通行证回归测试

运行：`dotnet run --project Tests/KeycardInventoryBypass.Regression.csproj -c Release`

测试链接两套生产事件处理器和共享权限策略，以最小游戏 / 框架契约模拟交互及完成事件，验证消耗时机、关门限制、锁门 / 取消、重复回调、离线 / 卸载清理及普通卡优先。无需加载 Unity 或 NuGet 测试框架。此测试不替代真实游戏服联调。

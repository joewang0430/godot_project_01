namespace GodotLab.Features.Interaction;

using Godot;

/// <summary>
/// SOTA 交互协议：所有可以被玩家按键交互的物体（门、雕像、掉落物、NPC）都必须实现此接口。
/// 消除硬编码的 is 类型判断，完美符合开闭原则 (OCP)。
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 当玩家按下交互键时触发
    /// </summary>
    /// <param name="interactor">发起交互的实体（通常是 Player，用 Node2D 方便获取位置等信息）</param>
    void Interact(Node2D interactor);

    /// <summary>
    /// 可选：如果靠近时需要在屏幕上显示提示（比如 "按 [E] 切换剑士"）
    /// </summary>
    string GetInteractPrompt();
}
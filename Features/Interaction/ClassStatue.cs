using Godot;

namespace GodotLab.Features.Interaction;

public partial class ClassStatue : Area2D, IInteractable
{
    [Export]
    public PlayerStatsConfig ClassConfig { get; set; } = null!;

    [Export]
    public string ClassName { get; set; } = "Unknown Class";

    public void Interact(Node2D interactor)
    {
        if (ClassConfig == null)
        {
            GD.PrintErr($"Statue {Name} is missing ClassConfig data payload in memory. Execution denied!");
            return;
        }

        if (interactor is Player player)
        {
            // 第一步：指针剥夺与覆盖 (Pointer Overwrite)
            // 将玩家原本指向旧配置的 8 字节指针，强行指向当前雕像口袋里的新配置地址
            player.Stats.Config = ClassConfig;

            // 第二步：内存重初始化 (Runtime State Re-init)
            // 基于新装配的芯片，重新计算血量等运行期数值
            player.Stats.ResetRuntimeState();

            GD.Print($"Player switched to class: {ClassName}, Current Max Health: {player.MaxHealth}");
        }
    }

    public string GetInteractPrompt()
    {
        return $"Press interact button to switch to: {ClassName}";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players;

namespace HinamoriMod.HinamoriModCode.Debug;

// Debug-only fallback: inspect game-version-dependent HP members at runtime.
internal static class DebugEnemyHelper
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly string[] CurrentNames = { "CurrentHp", "CurrentHP", "Hp", "HP", "Health", "CurrentHealth" };
    private static readonly string[] MaxNames = { "MaxHp", "MaxHP", "MaximumHp", "MaxHealth", "MaximumHealth" };

    public static List<object> Enemies(Player player) =>
        player.Creature.CombatState?.GetOpponentsOf(player.Creature)
            .Where(c => c.IsAlive).Cast<object>().ToList() ?? new List<object>();

    private static MemberInfo? Find(object target, string[] names)
    {
        var type = target.GetType();
        foreach (var name in names)
        {
            var property = type.GetProperty(name, Flags);
            if (property != null && property.GetIndexParameters().Length == 0 && property.CanRead) return property;
            var field = type.GetField(name, Flags);
            if (field != null) return field;
        }
        return null;
    }

    private static object? Read(object target, MemberInfo? member) => member switch
    {
        PropertyInfo p => p.GetValue(target),
        FieldInfo f => f.GetValue(target),
        _ => null
    };

    public static int? CurrentHp(object enemy)
    {
        var member = Find(enemy, CurrentNames);
        var value = Read(enemy, member);
        return value == null ? null : Convert.ToInt32(value);
    }

    public static int? MaxHp(object enemy)
    {
        var member = Find(enemy, MaxNames);
        var value = Read(enemy, member);
        return value == null ? null : Convert.ToInt32(value);
    }

    private static void Write(object target, MemberInfo? member, int hp)
    {
        switch (member)
        {
            case PropertyInfo p when p.SetMethod != null:
                p.SetValue(target, Convert.ChangeType(hp, p.PropertyType));
                return;
            case FieldInfo f when !f.IsInitOnly:
                f.SetValue(target, Convert.ChangeType(hp, f.FieldType));
                return;
            default:
                throw new NotSupportedException("HPを書き換えられるメンバーが見つかりません。Creature.cs のHP更新APIを確認してください。");
        }
    }

    public static void SetHp(object enemy, int desiredHp, bool updateMax)
    {
        if (desiredHp < 1) throw new ArgumentOutOfRangeException(nameof(desiredHp));
        var current = Find(enemy, CurrentNames);
        var maximum = Find(enemy, MaxNames);
        if (current == null) throw new NotSupportedException("現在HPのメンバーが見つかりません。");
        int max = MaxHp(enemy) ?? desiredHp;
        if (updateMax)
        {
            if (maximum == null) throw new NotSupportedException("最大HPのメンバーが見つかりません。");
            Write(enemy, maximum, desiredHp);
            max = desiredHp;
        }
        Write(enemy, current, Math.Min(desiredHp, max));
    }
}

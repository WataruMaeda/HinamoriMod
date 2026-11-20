namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>
/// 完全詠唱の秘薬 (Perfect Incantation Elixir)
/// 詠唱を10獲得する。次の破道・縛道のダメージ・ブロックを2倍にする。
/// </summary>
/// <remarks>
/// PotionModel / CustomPotionModel への接続は未実装。
/// ポーション使用時のフック、レアリティ登録、ターゲット選択、画像は要確認。
/// </remarks>
public static class PerfectIncantationElixir
{
    public const string JapaneseName = "完全詠唱の秘薬";
    public const string EnglishName = "Perfect Incantation Elixir";
    public const string RarityName = "Rare";
    public const string EffectKind = "ChantAndDouble";
    public const int PrimaryAmount = 10;
    public const decimal DamageAndBlockMultiplier = 2m;
}

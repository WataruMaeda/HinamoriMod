namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>
/// 霊圧の霊薬 (Spiritual Elixir)
/// 詠唱を5獲得する。
/// </summary>
/// <remarks>
/// PotionModel / CustomPotionModel への接続は未実装。
/// ポーション使用時のフック、レアリティ登録、ターゲット選択、画像は要確認。
/// </remarks>
public static class SpiritualElixir
{
    public const string JapaneseName = "霊圧の霊薬";
    public const string EnglishName = "Spiritual Elixir";
    public const string RarityName = "Common";
    public const string EffectKind = "Chant";
    public const int PrimaryAmount = 5;
}

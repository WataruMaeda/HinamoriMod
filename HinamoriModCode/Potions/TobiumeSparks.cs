namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>
/// 飛梅の火粉 (Tobiume Sparks)
/// 火種を15獲得する。
/// </summary>
/// <remarks>
/// PotionModel / CustomPotionModel への接続は未実装。
/// ポーション使用時のフック、レアリティ登録、ターゲット選択、画像は要確認。
/// </remarks>
public static class TobiumeSparks
{
    public const string JapaneseName = "飛梅の火粉";
    public const string EnglishName = "Tobiume Sparks";
    public const string RarityName = "Common";
    public const string EffectKind = "Ember";
    public const int PrimaryAmount = 15;
}

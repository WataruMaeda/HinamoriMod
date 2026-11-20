namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>
/// 飛梅の霊髄 (Tobiume Essence)
/// 火種を30獲得し、敵全体に火傷5を付与する。
/// </summary>
/// <remarks>
/// PotionModel / CustomPotionModel への接続は未実装。
/// ポーション使用時のフック、レアリティ登録、ターゲット選択、画像は要確認。
/// </remarks>
public static class TobiumeEssence
{
    public const string JapaneseName = "飛梅の霊髄";
    public const string EnglishName = "Tobiume Essence";
    public const string RarityName = "Rare";
    public const string EffectKind = "EmberAndBurn";
    public const int PrimaryAmount = 30;
    public const int BurnAmount = 5;
}

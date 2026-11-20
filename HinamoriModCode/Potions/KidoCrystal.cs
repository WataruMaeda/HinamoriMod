namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>
/// 鬼道の結晶 (Kido Crystal)
/// このターン、破道・縛道の詠唱コストを0にする。
/// </summary>
/// <remarks>
/// PotionModel / CustomPotionModel への接続は未実装。
/// ポーション使用時のフック、レアリティ登録、ターゲット選択、画像は要確認。
/// </remarks>
public static class KidoCrystal
{
    public const string JapaneseName = "鬼道の結晶";
    public const string EnglishName = "Kido Crystal";
    public const string RarityName = "Uncommon";
    public const string EffectKind = "FreeChant";
    public const int PrimaryAmount = 0;
}

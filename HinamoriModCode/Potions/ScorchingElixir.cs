namespace HinamoriMod.HinamoriModCode.Potions;

/// <summary>
/// 灼熱の霊薬 (Scorching Elixir)
/// 敵1体に火傷10を付与する。
/// </summary>
/// <remarks>
/// PotionModel / CustomPotionModel への接続は未実装。
/// ポーション使用時のフック、レアリティ登録、ターゲット選択、画像は要確認。
/// </remarks>
public static class ScorchingElixir
{
    public const string JapaneseName = "灼熱の霊薬";
    public const string EnglishName = "Scorching Elixir";
    public const string RarityName = "Uncommon";
    public const string EffectKind = "Burn";
    public const int PrimaryAmount = 10;
}

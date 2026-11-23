using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using HinamoriMod.HinamoriModCode.Cards;
using HinamoriMod.HinamoriModCode.Extensions;
using HinamoriMod.HinamoriModCode.Relics;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace HinamoriMod.HinamoriModCode.Character;

public class HinamoriMod : PlaceholderCharacterModel
{
    public const string CharacterId = "HinamoriMod";
    
    /// <summary>CharactorColor</summary>
    public static readonly Color Color = new("ff69b4");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;

    /// <summary>StartHP</summary>
    public override int StartingHp => 70;
    
    /// <summary>初期カード</summary>
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<SlashMomo>(),
        ModelDb.Card<SlashMomo>(),
        ModelDb.Card<SlashMomo>(),
        ModelDb.Card<SlashMomo>(),
        ModelDb.Card<ShunpoMomo>(),
        ModelDb.Card<ShunpoMomo>(),
        ModelDb.Card<ShunpoMomo>(),
        ModelDb.Card<ShunpoMomo>(),
        ModelDb.Card<MeditationMomo>(),
        ModelDb.Card<RedFrameCannonMomo>()
    ];

    /// <summary>初期レリック</summary>
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<KidoFundamentals>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<HinamoriModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<HinamoriModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<HinamoriModPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    /// <summary>戦闘画面左上のキャラクターアイコン</summary>
    public override string CustomIconTexturePath => "res://HinamoriMod/images/icons/hinamori_icon.png";
    /// <summary>キャラクター選択画面のキャラクターアイコン</summary>
    public override string CustomCharacterSelectIconPath => "res://HinamoriMod/images/character/hinamori_icon.png";
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    /// <summary>キャラクター選択画面の画像</summary>
    public override string CustomCharacterSelectBg => "res://HinamoriMod/scenes/hinamori_select_bg.tscn";

    /// <summary>戦闘画面の雛森桃：8フレームの待機アニメーションを表示</summary>
    public override string CustomVisualPath => "res://HinamoriMod/scenes/hinamori_idle.tscn";
}
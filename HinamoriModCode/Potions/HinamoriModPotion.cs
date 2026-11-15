using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using HinamoriMod.HinamoriModCode.Character;
using HinamoriMod.HinamoriModCode.Extensions;

namespace HinamoriMod.HinamoriModCode.Potions;

[Pool(typeof(HinamoriModPotionPool))]
public abstract class HinamoriModPotion : CustomPotionModel
{
	public override string? CustomPackedImagePath =>
		$"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
	public override string? CustomPackedOutlinePath =>
		$"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}
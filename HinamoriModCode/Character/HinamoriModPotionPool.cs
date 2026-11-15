using BaseLib.Abstracts;
using HinamoriMod.HinamoriModCode.Extensions;
using Godot;

namespace HinamoriMod.HinamoriModCode.Character;

public class HinamoriModPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => HinamoriMod.Color;
    

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
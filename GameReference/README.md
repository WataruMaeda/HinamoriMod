# Slay the Spire 2 ゲーム API リファレンス

雛森 MOD の開発で参照するため、インストール済みのゲーム DLL を Mono.Cecil で調べた資料です。対象ビルドと SHA-256 は [GameVersion.md](GameVersion.md) を参照してください。

「DLL で確認した」節は型メタデータと実装 IL に基づきます。「MOD 向け使用例」は参照方法を示したスニペットで、ゲーム内での動作は未確認です。一部のシグネチャは参照型の null 許容注釈を省略しています。記載範囲は MOD で使う主要メンバーと主要経路です。

## API

- [AbstractModel](API/AbstractModel.md)
- [CardCmd](API/CardCmd.md)
- [CardFactory](API/CardFactory.md)
- [CardModel](API/CardModel.md)
- [CardPileCmd](API/CardPileCmd.md)
- [CardPlay](API/CardPlay.md)
- [CardSelectCmd](API/CardSelectCmd.md)
- [PlayerCmd](API/PlayerCmd.md)
- [PowerCmd](API/PowerCmd.md)
- [RelicModel](API/RelicModel.md)

## Mechanics

- [CardGeneration](Mechanics/CardGeneration.md)
- [CardPlayFlow](Mechanics/CardPlayFlow.md)
- [CostCalculation](Mechanics/CostCalculation.md)
- [DamageCalculation](Mechanics/DamageCalculation.md)
- [PowerApplication](Mechanics/PowerApplication.md)
- [TurnLifecycle](Mechanics/TurnLifecycle.md)

## Powers

- [MayhemPower](Powers/MayhemPower.md)
- [VoidFormPower](Powers/VoidFormPower.md)

## 未確認の範囲

他の Steam ビルドの差分、すべての API と例外経路、マルチプレイでの実動作、他 MOD との競合。個別の未確認事項は各処理ページに記載しています。

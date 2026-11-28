# 解析対象バージョン

| 項目 | 値 |
| --- | --- |
| Steam App ID | `2868840` |
| Steam ビルド番号 | `26660232`（ローカルの `appmanifest_2868840.acf` の `buildid`） |
| アセンブリ | `sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null` |
| SHA-256 | `62ECBE91093E1FA862DE4329AAC865209DCB297804CAF0F0520D92D8C0B4A9C3` |
| モジュール MVID | `9d417b39-1d40-4fc7-b993-28acdc29e151` |
| 解析日時（JST） | `2026-11-23T02:29:10+09:00` |
| DLL パス | `D:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll` |
| MOD ブランチ | `feature/ongoing-fixes` |

Mono.Cecil で型・メソッド・引数・IL を読み取り、SHA-256 でバイナリを識別した。ゲーム更新後は DLL のハッシュを比較してから処理順や IL オフセットを再検証する。

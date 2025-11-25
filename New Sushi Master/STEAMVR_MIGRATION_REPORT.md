# SteamVR重複エラー解決レポート

## 実行日時
2025/11/26

## 問題の概要
810個のSteamVR重複エラーが発生していた。
```
Assembly with name 'SteamVR_Input_Editor' already exists
Assembly with name 'SteamVR_Editor' already exists
Assembly with name 'SteamVR' already exists
Assembly with name 'SteamVR_Windows_EditorHelper' already exists
```

## 根本原因
ローカル開発環境とチーム環境で、2つの異なるSteamVRフォルダが存在していた：

1. **Assets/steamVR_plugin/** (古い設定、削除済み)
   - Player.prefab GUID: `dc06161b6d97feb419f45f03b62e14b9`
   - OpenVR XR Plugin使用

2. **Assets/steamVRplugin setting/** (新しい設定、保持)
   - Player.prefab GUID: `23e529e47021a0c4caf79fafc34c3b23`
   - SteamVR Asset Store版使用

同じアセンブリ定義(.asmdef)が2箇所に存在していたため、重複エラーが発生。

---

## 実行した修正内容

### 1. 古いSteamVRフォルダの削除
```bash
rm -rf "Assets/steamVR_plugin"
rm -f "Assets/steamVR_plugin.meta"
```

**結果**: `Assets/steamVRplugin setting/` のみが残る

---

### 2. Packages/manifest.json の修正
**削除した行**:
```json
"com.valvesoftware.unity.openvr": "file:../Assets/SteamVR/OpenVRUnityXRPackage/Editor/com.valvesoftware.unity.openvr-1.2.1.tgz",
```

**理由**: OpenVR XR Pluginを削除し、SteamVR Asset Store版に完全移行

**変更後のファイル**: [Packages/manifest.json](Packages/manifest.json)

---

### 3. シーンファイルのGUID修正

#### 問題
削除した `Assets/steamVR_plugin/` のPlayer.prefabを参照していたため、以下のエラーが発生：
```
Missing Prefab Asset: 'Missing Prefab with guid: dc06161b6d97feb419f45f03b62e14b9'
```

#### 修正内容
**SampleScene.unity**: 古いPlayer.prefab GUID → 新しいPlayer.prefab GUIDに一括置換

**置換コマンド**:
```bash
sed -i 's/dc06161b6d97feb419f45f03b62e14b9/23e529e47021a0c4caf79fafc34c3b23/g' "Assets/Scenes/SampleScene.unity"
```

**Interactions_Example.unity**: 修復不可能だったため削除
```bash
rm "Assets/Scenes/Interactions_Example.unity"
rm "Assets/Scenes/Interactions_Example.unity.meta"
rm "Assets/Scenes/Interactions_ExampleSettings.lighting"
rm "Assets/Scenes/Interactions_ExampleSettings.lighting.meta"
```

**理由**: Interactions_Example.unityは大量のMissing Prefabがあり、steamVRplugin settingフォルダ内の同名シーンも同様に壊れていたため、使用不可と判断。

---

### 4. Unityキャッシュのクリア
```bash
rm -rf "Library/ScriptAssemblies"
rm -rf "Library/StateCache"
rm -rf "Library/ScriptMapper"
```

**理由**: 古いアセンブリキャッシュが残っていると、エラーが再発する可能性がある

---

## 最終的なプロジェクト構成

### SteamVR設定
- **使用フォルダ**: `Assets/steamVRplugin setting/`
- **使用バージョン**: SteamVR Asset Store版
- **Player Prefab**: `Assets/steamVRplugin setting/SteamVR/InteractionSystem/Core/Prefabs/Player.prefab`
- **Player Prefab GUID**: `23e529e47021a0c4caf79fafc34c3b23`

### 削除されたもの
- ~~Assets/steamVR_plugin/~~ (削除済み)
- ~~OpenVR XR Plugin~~ (Packages/manifest.jsonから削除済み)
- ~~Assets/Scenes/Interactions_Example.unity~~ (削除済み - 修復不可能だったため)

### 使用可能なシーン
- **Assets/Scenes/SampleScene.unity** - メインシーン（寿司VRゲーム）

### 参考用シーン（使用非推奨）
- `Assets/steamVRplugin setting/SteamVR/InteractionSystem/Samples/Interactions_Example.unity` - SteamVRサンプルシーン（Missing Prefabあり）

---

## 新規メンバーがプルした時の対応手順

### ケース1: 重複エラーが出る場合

**エラー内容**:
```
Assembly with name 'SteamVR_Input_Editor' already exists (x810)
```

**原因**: 古い `Assets/steamVR_plugin/` フォルダが残っている

**解決手順**:
```bash
# 1. Unityを閉じる

# 2. 古いフォルダを削除
rm -rf "Assets/steamVR_plugin"
rm -f "Assets/steamVR_plugin.meta"

# 3. manifest.jsonを確認（OpenVR XR Pluginの行がないことを確認）
cat Packages/manifest.json | grep openvr
# → 何も出力されなければOK

# 4. Unityを起動
```

---

### ケース2: Missing Prefab エラーが出る場合

**エラー内容**:
```
Missing Prefab with guid: dc06161b6d97feb419f45f03b62e14b9
または
Missing Prefab with guid: 1e914d8e31fa95b488c87df17e68d504 (など複数)
```

**原因**: シーンファイルが古いPlayer.prefabや他のPrefab GUIDを参照している

**解決手順**:
```bash
# 1. Unityを閉じる

# 2. SampleScene.unity のGUIDを修正
sed -i 's/dc06161b6d97feb419f45f03b62e14b9/23e529e47021a0c4caf79fafc34c3b23/g' "Assets/Scenes/SampleScene.unity"

# 3. Interactions_Example.unity がある場合は削除（修復不可能）
rm -f "Assets/Scenes/Interactions_Example.unity"
rm -f "Assets/Scenes/Interactions_Example.unity.meta"
rm -f "Assets/Scenes/Interactions_ExampleSettings.lighting"
rm -f "Assets/Scenes/Interactions_ExampleSettings.lighting.meta"

# 4. Unityキャッシュをクリア
rm -rf "Library/ScriptAssemblies"
rm -rf "Library/StateCache"
rm -rf "Library/ScriptMapper"

# 5. Unityを起動
```

**重要**: Interactions_Example.unity は削除推奨。SampleScene.unity のみ使用してください。

---

### ケース3: 新規環境セットアップ

**必要なAsset Storeアセット**:
1. **SteamVR Plugin** (Asset Store版) - 必須
2. **Cartoon FX Remaster** (CFXR Magic Poof エフェクト用)
3. **GASTRO Sushi Food Pack FREE** (3Dモデル用)

**セットアップ手順**:
1. このリポジトリをクローン
2. Unity 2022.3以上で開く
3. 上記Asset Storeアセットをインポート
4. エラーがなければ完了

---

## 重要な注意事項

### やってはいけないこと
❌ OpenVR XR Pluginを再度インポートしない
❌ `Assets/steamVR_plugin/` フォルダを作成しない
❌ 古いPlayer.prefab GUID (`dc06161b6d97feb419f45f03b62e14b9`) を参照しない

### 正しい状態
✅ `Assets/steamVRplugin setting/` のみが存在
✅ Packages/manifest.json に `openvr` の記述がない
✅ 全シーンが新しいPlayer.prefab GUID (`23e529e47021a0c4caf79fafc34c3b23`) を参照

---

## トラブルシューティング

### Q: Unityが起動しない
A: Library フォルダを削除してから再起動
```bash
rm -rf Library/
```

### Q: SteamVRが動作しない
A: Project Settings → XR Plug-in Management → OpenVR Loader が無効になっているか確認
   → SteamVR Asset Store版を使用しているため、OpenVR Loaderは不要

### Q: Player Prefabがシーンに配置できない
A: `Assets/steamVRplugin setting/SteamVR/InteractionSystem/Core/Prefabs/Player.prefab` を使用

---

## 参考ファイル

### 重要なファイル一覧
- [SETUP_NOTES.txt](SETUP_NOTES.txt) - セットアップメモ
- [Packages/manifest.json](Packages/manifest.json) - パッケージ設定
- [Assets/steamVRplugin setting/](Assets/steamVRplugin%20setting/) - SteamVR設定フォルダ

### GUID対照表
| ファイル | 古いGUID (削除済み) | 新しいGUID (使用中) |
|---------|-------------------|-------------------|
| Player.prefab | dc06161b6d97feb419f45f03b62e14b9 | 23e529e47021a0c4caf79fafc34c3b23 |

---

## 検証コマンド集

### プロジェクトが正しい状態か確認
```bash
# 1. steamVR_plugin フォルダが存在しないことを確認
ls -la Assets/ | grep -i "steamvr_plugin"
# → 何も表示されなければOK

# 2. steamVRplugin setting フォルダが存在することを確認
ls -la Assets/ | grep -i "steamVRplugin setting"
# → ディレクトリが表示されればOK

# 3. manifest.json に openvr がないことを確認
grep -i "openvr" Packages/manifest.json
# → 何も表示されなければOK

# 4. シーンファイルに古いGUIDがないことを確認
grep -r "dc06161b6d97feb419f45f03b62e14b9" Assets/Scenes/
# → 何も表示されなければOK

# 5. シーンファイルに新しいGUIDがあることを確認
grep -r "23e529e47021a0c4caf79fafc34c3b23" Assets/Scenes/
# → 複数箇所で見つかればOK
```

---

## 今後のメンテナンス

### SteamVRアップデート時
1. Asset Storeから最新版をインポート
2. `Assets/steamVRplugin setting/` 配下のみが更新される
3. Player.prefab のGUIDは変わらないため、シーン修正不要

### 新しいシーン追加時
1. `Assets/steamVRplugin setting/SteamVR/InteractionSystem/Core/Prefabs/Player.prefab` を配置
2. 自動的に正しいGUID (`23e529e47021a0c4caf79fafc34c3b23`) が参照される

---

## 修正履歴
| 日付 | 作業者 | 内容 |
|-----|-------|------|
| 2025/11/26 | Claude | SteamVR重複エラー解決、GUID修正 |
| 2025/11/26 | Claude | 引き継ぎレポート作成 |

---

## 連絡先
問題が発生した場合は、このレポートを参照してください。
解決しない場合は、SETUP_NOTES.txt も確認してください。

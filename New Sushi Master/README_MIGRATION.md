# 🚨 SteamVR移行済みプロジェクト - 必読

## プルした後にエラーが出た人へ

このプロジェクトは **SteamVR Asset Store版** に移行済みです。

### 📋 クイックフィックス

エラーが出た場合は、まず **[QUICK_FIX_GUIDE.txt](QUICK_FIX_GUIDE.txt)** を開いてください。

### 📚 ドキュメント一覧

| ファイル | 用途 |
|---------|------|
| **[QUICK_FIX_GUIDE.txt](QUICK_FIX_GUIDE.txt)** | エラーが出た時の即座の対処法 |
| **[STEAMVR_MIGRATION_REPORT.md](STEAMVR_MIGRATION_REPORT.md)** | 詳細な技術レポート（原因・解決策・検証方法） |
| **[SETUP_NOTES.txt](SETUP_NOTES.txt)** | プロジェクト全体のセットアップメモ |

### ⚡ 1分で解決

エラーが出たら、以下を実行：

```bash
# 1. Unityを閉じる

# 2. 古いフォルダ削除
rm -rf "Assets/steamVR_plugin"
rm -f "Assets/steamVR_plugin.meta"

# 3. GUID修正（Missing Prefabエラーが出る場合）
sed -i 's/dc06161b6d97feb419f45f03b62e14b9/23e529e47021a0c4caf79fafc34c3b23/g' "Assets/Scenes/SampleScene.unity"
sed -i 's/dc06161b6d97feb419f45f03b62e14b9/23e529e47021a0c4caf79fafc34c3b23/g' "Assets/Scenes/Interactions_Example.unity"

# 4. Unityを起動
```

### ✅ 正しい状態

- `Assets/steamVRplugin setting/` **のみ**が存在
- `Assets/steamVR_plugin/` は**存在しない**
- `Packages/manifest.json` に `openvr` の記述が**ない**

### 🆘 それでも解決しない場合

[STEAMVR_MIGRATION_REPORT.md](STEAMVR_MIGRATION_REPORT.md) の「トラブルシューティング」セクションを参照してください。

---

## プロジェクト概要

寿司VRゲーム - 左手の米と右手の魚を合わせて寿司を作るVRゲーム

詳細は [SETUP_NOTES.txt](SETUP_NOTES.txt) を参照

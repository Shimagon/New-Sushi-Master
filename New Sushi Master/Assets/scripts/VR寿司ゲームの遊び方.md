# VR寿司ゲームの遊び方とセットアップ

## ゲームの流れ

1. **左手で米を掴む**
2. **右手で魚ネタ（マグロ、たまご、サーモン）を掴む**
3. **両手を合わせて米とネタを合体させる**
4. **自動的に寿司が生成されて右手に持ち替わる**
5. **お客さんの要望を確認する**
6. **寿司をお客さんに向かって投げる**
7. **正しい寿司なら高得点！間違った寿司なら低得点**

---

## OpenVRのエラーについて

起動時に以下のエラーが出る場合がありますが、動作には影響ありません：
```
ReflectionTypeLoadException: Exception of type 'System.Reflection.ReflectionTypeLoadException' was thrown.
```

これはUnityのバージョンとOpenVRパッケージの互換性の問題ですが、ゲームは正常にプレイできます。

---

## 修正したスクリプト

### 1. GrabbableRice.cs（修正済み）
左手で持つ米オブジェクト用のスクリプトです。

**主な機能:**
- VRコントローラーで掴める
- 魚ネタと衝突すると寿司を生成
- 魚の種類に応じて異なる寿司を生成（マグロ、たまご、サーモン）
- 生成した寿司を自動的に右手にアタッチ

**Inspector設定項目:**
- `Maguro Sushi Prefab`: マグロ寿司のPrefab
- `Tamago Sushi Prefab`: たまご寿司のPrefab
- `Salmon Sushi Prefab`: サーモン寿司のPrefab
- `Sushi Spawn Offset`: 寿司の生成位置オフセット
- `Sushi Rotation`: 寿司の回転（デフォルト: -90, 0, 0）
- `Sushi Make Sound`: 寿司生成時の効果音

### 2. GrabbableFish.cs（修正済み）
右手で掴む魚ネタ用のスクリプトです。

**主な機能:**
- VRコントローラーで掴める
- FishTypeコンポーネントがあれば自動的に種類を取得
- 掴んだときに色が変わる（視覚的フィードバック）

**Inspector設定項目:**
- `Fish Type`: 魚の種類（"Maguro", "Tamago", "Salmon"）
- `Held Color`: 掴んだときの色
- `Normal Color`: 通常時の色

### 3. SushiThrowable.cs（修正済み）
完成した寿司用のスクリプトです。

**主な機能:**
- VRコントローラーで掴んで投げられる
- SushiTypeコンポーネントから寿司の種類とスコアを自動取得
- お客さんに当たるとスコアが加算される
- 一定時間経過すると自動的に消える

### 4. FishType.cs（新規作成）
魚ネタの種類を識別するコンポーネントです。

### 5. SushiType.cs（新規作成）
完成した寿司の種類を識別するコンポーネントです。

---

## Unityエディタでのセットアップ

### ステップ1: Interactions_Exampleシーンを開く

1. Project > Assets > Scenes > Interactions_Example.unity を開く
2. Hierarchy > Player を確認

### ステップ2: 米のプレハブを作成

#### 米オブジェクトの設定
1. GameObject > 3D Object > Cube で米オブジェクトを作成
2. 名前を "Rice" に変更
3. Transform設定:
   - Scale: (0.1, 0.05, 0.15) くらい（寿司のシャリサイズ）
4. マテリアル設定:
   - 白色のマテリアルを作成して適用
5. 必要なコンポーネントを追加:
   - `Rigidbody`: 物理演算用
     - Mass: 0.1
     - Use Gravity: チェック
   - `Box Collider`: 衝突判定用
   - `Interactable` (SteamVR): VRで掴めるようにする
   - `VelocityEstimator` (SteamVR): 投げる速度を計算
   - `GrabbableRice` スクリプト

#### GrabbableRiceの設定
1. `Maguro Sushi Prefab`: maguro.prefab をドラッグ
2. `Tamago Sushi Prefab`: TamagoSushi.prefab をドラッグ（後で作成）
3. `Salmon Sushi Prefab`: SalmonSushi.prefab をドラッグ（後で作成）
4. `Sushi Rotation`: (-90, 0, 0)

#### Prefab化
- Assetsフォルダに "Rice.prefab" として保存

---

### ステップ3: 魚ネタのプレハブを作成

#### 3-1. マグロネタ
1. GameObject > 3D Object > Cube
2. 名前: "MaguroNeta"
3. Scale: (0.08, 0.02, 0.12)
4. マテリアル: 赤色
5. コンポーネント:
   - `Rigidbody` (Mass: 0.05)
   - `Box Collider`
   - `Interactable` (SteamVR)
   - `VelocityEstimator` (SteamVR)
   - `GrabbableFish`: Fish Type = "Maguro"
   - `FishType`: Fish Type Name = "Maguro", Display Name = "マグロ"
6. Tag: "Fish"
7. Prefab化: "MaguroNeta.prefab"

#### 3-2. たまごネタ
1. 同様にCubeを作成
2. 名前: "TamagoNeta"
3. マテリアル: 黄色 (R:1, G:0.9, B:0)
4. `GrabbableFish`: Fish Type = "Tamago"
5. `FishType`: Fish Type Name = "Tamago", Display Name = "たまご"
6. Tag: "Fish"
7. Prefab化: "TamagoNeta.prefab"

#### 3-3. サーモンネタ
1. 同様にCubeを作成
2. 名前: "SalmonNeta"
3. マテリアル: オレンジ色 (R:1, G:0.5, B:0.3)
4. `GrabbableFish`: Fish Type = "Salmon"
5. `FishType`: Fish Type Name = "Salmon", Display Name = "サーモン"
6. Tag: "Fish"
7. Prefab化: "SalmonNeta.prefab"

---

### ステップ4: 完成寿司のプレハブを作成

#### 4-1. マグロ寿司（既存のmaguro.prefabを使用）
1. maguro.prefabを開く
2. コンポーネントを追加:
   - `Rigidbody`
   - `Box Collider`
   - `Interactable` (SteamVR)
   - `VelocityEstimator` (SteamVR)
   - `SushiType`: Sushi Type Name = "Maguro", Score Value = 100
   - `SushiThrowable`
3. 保存

#### 4-2. たまご寿司（cube008を使用）
1. Hierarchyでcube008を探す（なければ新規Cube作成）
2. 名前: "TamagoSushi"
3. 上記と同じコンポーネントを追加
4. `SushiType`: Sushi Type Name = "Tamago", Score Value = 100
5. Prefab化: "TamagoSushi.prefab"

#### 4-3. サーモン寿司（cube006を使用）
1. Hierarchyでcube006を探す（なければ新規Cube作成）
2. 名前: "SalmonSushi"
3. 上記と同じコンポーネントを追加
4. `SushiType`: Sushi Type Name = "Salmon", Score Value = 100
5. Prefab化: "SalmonSushi.prefab"

---

### ステップ5: シーンに配置

#### 米と魚ネタの配置
1. Interactions_Exampleシーンを開く
2. テーブルや台の上に以下を配置:
   - Rice.prefab × 複数個
   - MaguroNeta.prefab × 複数個
   - TamagoNeta.prefab × 複数個
   - SalmonNeta.prefab × 複数個

#### お客さんの配置
1. GameObject > Create Empty
2. 名前: "Customer1"
3. `Customer` スクリプトをアタッチ
4. `Requested Sushi Type`: "Maguro"
5. 位置: プレイヤーの前方

同様に複数のお客さんを配置して、それぞれ異なる寿司を要求させる。

---

## プレイ方法

1. **VRヘッドセットとコントローラーを装着**
2. **Unityでプレイボタンを押す**
3. **左手のトリガーで米を掴む**
4. **右手のトリガーで魚ネタを掴む**
5. **両手を合わせる** → 寿司が生成されて右手に持ち替わる
6. **お客さんの吹き出しを確認**（「マグロが欲しい！」など）
7. **右手を振って寿司を投げる**
8. **お客さんに当たるとスコアゲット！**

---

## トラブルシューティング

### 寿司が生成されない
- 米と魚ネタの両方を手で持っているか確認
- GrabbableRiceに3つのPrefabが設定されているか確認
- 魚ネタにFishTypeコンポーネントが付いているか確認
- 魚ネタのTag が "Fish" になっているか確認

### 寿司が右手に移動しない
- 寿司のPrefabにInteractableコンポーネントがあるか確認
- SushiThrowableコンポーネントが付いているか確認
- Playerオブジェクトに左手と右手が正しく設定されているか確認

### VRコントローラーで掴めない
- Interactableコンポーネントが付いているか確認
- Rigidbodyが付いているか確認
- SteamVR Input Actionsが正しく設定されているか確認

### OpenVRのエラーが気になる
- 無視して大丈夫です
- または、OpenVRパッケージを最新バージョンに更新

---

## 次のステップ

- エフェクトの追加（パーティクルなど）
- 効果音の追加
- お客さんのアニメーション
- スコアUIの改善
- タイムアタックモード
- コンボシステム

お疲れ様でした！楽しんでください！

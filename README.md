# NuclearEnergyGame
<a name="japanese"></a>

## 概要
本プロジェクトは、カナダの**原子力イノベーション研究所（Nuclear Innovation Institute）**から受注した実際のクライアントワークです。高校生がクリーンエネルギー管理について学ぶことを目的とした教育用ストラテジーゲームを、実クライアント・実納期・実際の受講生を対象として開発しました。
プレイヤーはエネルギー生産とコストのバランスを取りながら成長する都市に電力を供給するリソース管理型ストラテジーゲームです。プログラマーとデザイナー合わせて6名のチームにおいてリードプログラマーを担当し、昼夜サイクル・グリッドベースの建設配置システム・資金・リソース管理システムの3つのコアシステムを設計・実装しました。納期は8週間。

[![Watch the video](https://i.sstatic.net/Vp2cE.png)](https://www.youtube.com/watch?v=rPK-dSAsbIk)

## 担当箇所
_本プロジェクトはチーム制作です。以下に記載するシステムは私が単独で担当した部分です。UI・ゲームバランス・アートパイプライン・プレイヤー進行などは他のチームメンバーが担当しました。_

**昼夜サイクルシステム**
- 正規化された時間値（0.0 = 深夜 → 0.5 = 正午 → 1.0 = 深夜）で全ての時間依存システムを制御するリアルタイム昼夜サイクルを実装
- 太陽と月の位置を、Quaternion.Eulerを使って事前計算した弧に沿ってディレクショナルライトを回転させることで表現し、毎フレーム滑らかに補間
- アンビエントライトの色と強度はInspectorでデザイナーが設定できるGradientで遷移 — 夜明けや夕暮れの色調はコード変更なく自動的に切り替わる
- -空の色の遷移は、サイクルコントローラーから時間パラメータを受け取るShaderLabカスタムシェーダーで処理 — リポジトリの言語内訳にあるShaderLabコンポーネントの担当箇所
- シミュレートされた都市からのエネルギー需要が時間帯に連動してスケール — 朝夕にピーク、夜間に低下し、プレイヤーに供給管理の戦略的プレッシャーを与える
- 昼夜の進行速度はランタイムで設定可能 — 授業中のデモンストレーションで教師がサイクルを速めたり遅らせたりできる

**グリッド建設システム**
- ワールド空間上に2D配列でセルの占有状態を管理するタイルベースの配置グリッドを実装
- 建物はScriptableObjectのデータアセットとして定義 — フットプリント（1×1、2×2など）・コスト・エネルギー出力・維持費をデータとして持ち、デザイナーがコードを変更せずに新しい建物種別を追加できる完全データドリブン設計
- 配置バリデーションはグリッドの境界・セルの占有状態・購入可否をリアルタイムで確認 — 無効な配置はゴーストプレビューの色変化（緑 = 有効、赤 = 無効）でプレイヤーに即時フィードバック
- 配置確定時にグリッド状態を更新し、スナップされたワールド座標に建物をインスタンス化し、購入コストを資金システムへ通知
- 建物の売却は設定可能な払い戻し率で対応 — グリッドセルを解放し残高を加算する処理を同じグリッド状態マネージャーでクリーンに管理

**資金管理システム**
- プレイヤーの現在残高・収入・支出を単一の信頼できる情報源として管理 — 他のすべてのシステム（建物購入・エネルギー売却・維持費）はクリーンなAPIを通じてこのマネージャーへ通知し、残高を直接操作しない設計
- パッシブ収入はゲーム内1日単位で計算（グリッドへのエネルギー売上 − 稼働中の建物維持費） — 連続更新ではなく日末に一括更新することで、プレイヤーが因果関係を直感的に理解できる
- イベント駆動の残高更新 — 残高変化時にイベントが発火し、UIがそれをリッスンする設計。資金システム内にポーリングやUI参照は一切なし
- 残高不足時はプレイヤーフィードバックイベント（UIフラッシュ・効果音）を発火 — 資金システムはUI実装について何も知る必要がない
- 残高履歴を1日単位で記録し、ゲーム内1週間ごとの収支サマリー画面を実現

## アーキテクチャ・設計方針
| パターン | 使用箇所 |
| ----------- | ----------- |
| ScriptableObject（データコンテナ） | 建物定義 — コスト・フットプリント・出力をコード変更なしで設定可能 |
| Observer / イベントシステム | 残高変化・昼夜遷移・建設イベントをすべてC#イベント経由で通知 |
| Singleton | MoneyManager・TimeManagerをグローバルにアクセス可能なサービスクラスとして実装 |
| 単一責任の原則 | グリッドロジック・資金ロジック・時間ロジックを完全に独立させ、システム間の直接参照を排除 |
| ShaderLabカスタムシェーダー | サイクルコントローラーから渡された時間パラメータで空の色遷移を制御 |

最も重要な設計判断は、3つのシステムを完全に疎結合に保つことでした。昼夜サイクルは資金を知りません。グリッドはサイクルを知りません。システム間の通信はイベントのみを介します。これにより、UIやゲームバランスなど依存する機能を担当する他のチームメンバーが、マージコンフリクトやシステムレベルの変更を引き起こすことなく並行して作業することができました。

| 技術スタック | |
| ----------- | ----------- |
| エンジン | Unity (LTS) |
| 言語 | C# |
| チーム規模 | 6名（プログラマー＋デザイナー） |
| クライアント | U原子力イノベーション研究所（カナダ）|
| 開発期間 | 8週間 |
| バージョン管理 | Git |

## 学んだこと
- 固定納期内でのシステムのスコープと納品 — コア機能を優先し、ポリッシュは後回しにする判断
- チーム開発におけるイベント駆動アーキテクチャの価値 — 直接参照ではなくイベントで通信することで複数人が衝突なく並行作業できる
- ScriptableObjectを用いたデータドリブン設計 — プログラマーの関与なしにデザイナーがコンテンツを独立してイテレーションできる仕組み
- クライアントとのコミュニケーション — 非技術者のクライアント（原子力イノベーション研究所）に進捗を報告し、教育上の要件に基づいてゲームを調整する実務経験
- ShaderLabとC# MonoBehaviourの連携 — ゲームプレイデータからビジュアルトランジションを駆動する方法

## チーム構成・背景
| 役割 | 担当内容 |
| ----------- | ----------- |
| スリエア ローガン（担当者） | リードプログラマー — 昼夜サイクル・グリッドシステム・資金管理システム |
| その他チームメンバー | UI・ゲームバランス・アートパイプライン・プレイヤー進行・レベルデザイン |

<a name="english"></a>
## Overview
This project was a contracted real-world deliverable developed for the Nuclear Innovation Institute of Canada — not a personal exercise, but a commissioned game with a real client, a real deadline, and a real audience of high school students learning about clean energy.
The game is a resource management strategy game where players must balance energy production and cost to power a growing city. As lead programmer on a team of six (programmers and designers), I was responsible for designing and implementing three of the game's core systems: the day/night cycle, the grid-based building placement system, and the money and resource management system — all delivered within an 8-week development schedule.

## My Contributions
_This was a team project. The systems documented below were solely my responsibility. Other systems (UI, game balance, art pipeline, player progression) were implemented by other team members._

**Day/Night Cycle System**
- Implemented a real-time day/night cycle driven by a normalised time value (0.0 = midnight → 0.5 = noon → 1.0 = midnight) that controls all time-dependent systems
- The sun and moon positions are driven by rotating a directional light along a precalculated arc using Quaternion.Euler, smoothly interpolated each frame
- Ambient light colour and intensity transition through a designer-configurable Gradient in the Inspector — dawn/dusk tones shift automatically without code changes
- Sky colour transitions handled via ShaderLab shader with a time parameter passed from the cycle controller — responsible for the ShaderLab component visible in the repository language breakdown
- Energy demand from the simulated city scales with time of day — demand peaks at morning and evening and drops at night, creating strategic pressure for the player to manage supply accordingly
- Day/night speed is configurable at runtime to allow educators to speed up or slow down cycles during classroom demonstrations

**Grid Building System**
- Implemented a tile-based placement grid over the world space using a 2D array to track cell occupancy
- Buildings are represented as ScriptableObject data assets defining their footprint (1×1, 2×2, etc.), cost, energy output, and maintenance fee — fully data-driven so designers can add new building types without touching code
- Placement validation checks grid bounds, tile occupancy, and affordability before confirming placement — invalid placements are visually indicated to the player in real time via a ghost preview that changes colour (green = valid, red = invalid)
- On placement confirmation, the grid state updates, the building is instantiated at the snapped world position, and the money system is notified of the purchase cost
- Buildings can be sold back at a configurable refund rate, freeing the grid cells and crediting the player — handled cleanly through the same grid state manager

**Money Management System**
- Manages the player's current balance, income, and expenses as a single source of truth — all other systems (building purchase, energy sales, maintenance) interact with this manager through a clean API rather than modifying the balance directly
- Passive income is calculated per game-day based on energy sold to the grid minus active building maintenance costs — the balance updates at day-end rather than continuously to give players a clear cause-and-effect understanding
- Event-driven balance updates: when the balance changes, an event fires that the UI listens to — no polling, no direct UI references in the money system
- Insufficient funds trigger a player feedback event (UI flash, audio cue) without the money system needing to know anything about the UI implementation
- Balance history is tracked per day to allow a simple financial summary screen at the end of each in-game week

## Architecture & Design Decisions
| Pattern | Where Used |
| ----------- | ----------- |
| ScriptableObject (Data Container) | Building definitions — cost, footprint, output all configurable without code changes |
| Observer / Event System | Balance changes, day transitions, and build events all communicated via C# events |
| Singleton | MoneyManager and TimeManager as globally accessible service classes |
| Single Responsibility | Grid logic, money logic, and time logic are fully independent — no cross-system direct references |
| ShaderLab Custom Shader | Sky colour transitions driven by a time parameter from the cycle controller |

The most important architectural decision was keeping the three systems fully decoupled from each other. The day/night cycle does not know about money. The grid does not know about the cycle. They communicate only through events. This made it possible for different team members to work on dependent features (like UI and game balance) without causing merge conflicts or requiring system-level changes.

| Technical Stack | |
| ----------- | ----------- |
| Engine | Unity (LTS) |
| Language | C# |
| Team Size | 6 (programmers + designers)） |
| Client | Nuclear Innovation Institute, Canada |
| Timeline | 8 weeks |
| Version Control | Git |

## What I Learned
- How to scope and deliver systems on a fixed deadline within a team — prioritising core functionality first, polish second
- The value of event-driven architecture in a team environment: when systems communicate through events rather than direct references, multiple people can work in parallel without conflicts
- How to design data-driven systems with ScriptableObjects so that designers can iterate on content independently without needing programmer involvement
- Practical experience with client communication — presenting progress to a non-technical client (the Nuclear Innovation Institute) and adjusting the game based on their educational requirements
- How ShaderLab integrates with C# MonoBehaviours to drive visual transitions from gameplay data

## Team & Context
| Role | Responsibility |
| ----------- | ----------- |
| Logan Soulliere (me) | Lead Programmer — Day/Night Cycle, Grid System, Money System |
| Team Members | UI, Game Balance, Art Pipeline, Player Progression, Level Design |

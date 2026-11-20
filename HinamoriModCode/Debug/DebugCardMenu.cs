
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace HinamoriMod.HinamoriModCode.Debug;

public partial class DebugCardMenu : Control
{
    private readonly Player _player;
    private IReadOnlyList<Type> _cards = CardDebugHelper.GetCards();
    private OptionButton _categorySelector = null!;
    private OptionButton _destinationSelector = null!;
    private readonly Dictionary<int, CardModel> _handCards = new();
    private static readonly Dictionary<Type, string> NameCache = new();

    private VBoxContainer _results = null!;
    private LineEdit _search = null!;
    private CheckBox _upgraded = null!;
    private Label _status = null!;
    private Label _resourceStatus = null!;
    private OptionButton _cardSelector = null!;
    private SpinBox _replay = null!;
    private CheckBox _retain = null!;
    private CheckBox _ethereal = null!;
    private CheckBox _exhaust = null!;

    private readonly Dictionary<int, object> _enemies = new();
    private OptionButton _enemySelector = null!;
    private SpinBox _enemyHp = null!;
    private CheckBox _changeMaxHp = null!;
    private Label _enemyStatus = null!;

    // ゲーム標準のカードUIを使用したホバープレビュー
    private NCard? _previewCard;
    private Type? _hoveredCardType;

    private bool _busy;
    private bool _updatingEffects;

    public event Action? Closed;

    public DebugCardMenu(Player player) => _player = player;

    private static CardModel? CreatePreviewCard(Type type)
    {
        try
        {
            var factory = typeof(ModelDb)
                .GetMethods(
                    BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "Card" &&
                    m.IsGenericMethodDefinition &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().Length == 0);

            if (factory == null)
                return null;

            return factory.MakeGenericMethod(type)
                .Invoke(null, null) as CardModel;
        }
        catch (Exception ex)
        {
            GD.PushWarning(
                $"[Hinamori Debug] カード取得失敗 " +
                $"({type.Name}): {ex.GetBaseException().Message}");

            return null;
        }
    }

    private static string DisplayName(Type type)
    {
        if (NameCache.TryGetValue(type, out var cached))
            return cached;

        var card = CreatePreviewCard(type);

        if (!string.IsNullOrWhiteSpace(card?.Title))
            return NameCache[type] = card.Title;

        return type.Name;
    }

    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Stop;
        ZIndex = 100;

        var shade = new ColorRect
        {
            Color = new Color(0, 0, 0, 0.75f)
        };

        shade.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        shade.MouseFilter = MouseFilterEnum.Stop;
        AddChild(shade);

        var panel = new PanelContainer();
        panel.SetAnchorsPreset(LayoutPreset.Center);
        panel.Position = new Vector2(-340, -320);
        panel.CustomMinimumSize = new Vector2(680, 640);
        AddChild(panel);

        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 10);
        panel.AddChild(root);

        var header = new HBoxContainer();
        root.AddChild(header);

        header.AddChild(new Label
        {
            Text = "雛森桃 MOD - デバッグ",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        });

        var close = new Button { Text = "閉じる" };
        close.Pressed += Close;
        header.AddChild(close);

        var tabs = new TabContainer
        {
            SizeFlagsVertical = SizeFlags.ExpandFill
        };

        root.AddChild(tabs);

        BuildCardsTab(tabs);
        BuildEffectsTab(tabs);
        BuildResourcesTab(tabs);
        BuildEnemiesTab(tabs);

        _status = new Label { Text = "準備完了" };
        root.AddChild(_status);

        RefreshCards();
        RefreshHandCards();
        UpdateResourceStatus();
    }

    private void BuildCardsTab(TabContainer tabs)
    {
        var tab = new VBoxContainer { Name = "カード一覧" };
        tabs.AddChild(tab);

        _search = new LineEdit
        {
            PlaceholderText = "日本語名・クラス名で検索..."
        };

        _search.TextChanged += _ =>
        {
            HideCardPreview();
            RefreshCards();
        };

        tab.AddChild(_search);

        var options = new HBoxContainer();
        tab.AddChild(options);
        options.AddChild(new Label { Text = "種類" });
        _categorySelector = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _categorySelector.AddItem("雛森専用カード", (int)CardDebugHelper.DebugCardCategory.Hinamori);
        _categorySelector.AddItem("状態異常カード", (int)CardDebugHelper.DebugCardCategory.Status);
        _categorySelector.AddItem("呪いカード", (int)CardDebugHelper.DebugCardCategory.Curse);
        _categorySelector.Select(0);
        _categorySelector.ItemSelected += _ =>
        {
            var category = (CardDebugHelper.DebugCardCategory)_categorySelector.GetSelectedId();
            try
            {
                _cards = CardDebugHelper.GetCards(category);
                RefreshCards();
            }
            catch (Exception ex)
            {
                GD.PushError($"[Hinamori Debug] カード一覧取得失敗: {ex}");
                _status.Text = $"カード一覧取得失敗: {ex.GetBaseException().Message}";
            }
        };
        options.AddChild(_categorySelector);
        options.AddChild(new Label { Text = "追加先" });
        _destinationSelector = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _destinationSelector.AddItem("手札", (int)PileType.Hand);
        _destinationSelector.AddItem("廃棄", (int)PileType.Exhaust);
        _destinationSelector.AddItem("捨て札", (int)PileType.Discard);
        _destinationSelector.AddItem("山札", (int)PileType.Draw);
        _destinationSelector.Select(0);
        options.AddChild(_destinationSelector);

        _upgraded = new CheckBox
        {
            Text = "強化版を生成"
        };

        _upgraded.Toggled += _ =>
        {
            if (_hoveredCardType != null)
                ShowCardPreview(_hoveredCardType);
        };

        tab.AddChild(_upgraded);

        var scroll = new ScrollContainer
        {
            CustomMinimumSize = new Vector2(620, 380),
            SizeFlagsVertical = SizeFlags.ExpandFill
        };

        tab.AddChild(scroll);

        _results = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        scroll.AddChild(_results);
    }

    // ========================================
    // カードプレビュー
    // ========================================

    private void ShowCardPreview(Type cardType)
    {
        HideCardPreview();

        try
        {
            var card = CreatePreviewCard(cardType);
            if (card == null)
                return;

            // プレビュー専用のモデルを強化する（実際の手札には影響しない）
            if (_upgraded.ButtonPressed && !card.IsUpgraded)
            {
                card.UpgradeInternal();
                card.FinalizeUpgradeInternal();
            }

            // プールから取得すると返却APIが必要になるため、
            // デバッグ専用ノードはゲーム本体のシーンから直接生成する。
            var scene = GD.Load<PackedScene>("res://scenes/cards/card.tscn");
            if (scene == null)
                throw new InvalidOperationException("カードUIシーンを読み込めませんでした。");

            var preview = scene.Instantiate<NCard>();
            preview.MouseFilter = MouseFilterEnum.Ignore;
            preview.ZIndex = 200;
            preview.Scale = Vector2.One * 0.85f;

            _previewCard = preview;
            _hoveredCardType = cardType;
            AddChild(preview);
            preview.Model = card;
            preview.UpdateVisuals(PileType.None, CardPreviewMode.Normal);

            var viewportSize = GetViewportRect().Size;
            var cardSize = NCard.defaultSize * preview.Scale;
            float x = viewportSize.X / 2f + 355f;
            float y = viewportSize.Y / 2f - 210f;
            x = Mathf.Clamp(x, 0f, Mathf.Max(0f, viewportSize.X - cardSize.X));
            y = Mathf.Clamp(y, 0f, Mathf.Max(0f, viewportSize.Y - cardSize.Y));
            preview.Position = new Vector2(x, y);
        }
        catch (Exception ex)
        {
            GD.PushError($"[Hinamori Debug] カードプレビュー失敗: {ex}");
            HideCardPreview();
        }
    }

    private void HideCardPreview()
    {
        _hoveredCardType = null;
        if (_previewCard == null)
            return;

        if (GodotObject.IsInstanceValid(_previewCard))
        {
            RemoveChild(_previewCard);
            _previewCard.QueueFree();
        }
        _previewCard = null;
    }

    // ========================================
    // カード効果
    // ========================================

    private void BuildEffectsTab(TabContainer tabs)
    {
        var tab = new VBoxContainer
        {
            Name = "カード効果"
        };

        tab.AddChild(new Label
        {
            Text = "手札のカードを選択して効果を変更します"
        });

        tabs.AddChild(tab);

        var selectRow = new HBoxContainer();
        tab.AddChild(selectRow);

        _cardSelector = new OptionButton
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        _cardSelector.ItemSelected += _ =>
            UpdateEffectControls();

        selectRow.AddChild(_cardSelector);

        var refresh = new Button
        {
            Text = "手札を更新"
        };

        refresh.Pressed += RefreshHandCards;
        selectRow.AddChild(refresh);

        tab.AddChild(new Label
        {
            Text = "追加リプレイ回数（BaseReplayCount）"
        });

        _replay = new SpinBox
        {
            MinValue = 0,
            MaxValue = 20,
            Step = 1,
            Value = 0
        };

        _replay.ValueChanged += value =>
        {
            if (_updatingEffects)
                return;

            var card = SelectedCard();

            if (card == null)
                return;

            card.BaseReplayCount = (int)value;

            _status.Text =
                $"{card.Title}: リプレイ {value:0} 回";
        };

        tab.AddChild(_replay);

        _retain = AddKeywordToggle(
            tab, "保留 (Retain)", CardKeyword.Retain);

        _ethereal = AddKeywordToggle(
            tab, "エセリアル (Ethereal)",
            CardKeyword.Ethereal);

        _exhaust = AddKeywordToggle(
            tab, "廃棄 (Exhaust)", CardKeyword.Exhaust);

        tab.AddChild(new Label
        {
            Text = "※効果は選択中のカード1枚に適用されます。"
        });
    }

    private CheckBox AddKeywordToggle(
        VBoxContainer tab,
        string title,
        CardKeyword keyword)
    {
        var check = new CheckBox { Text = title };

        check.Toggled += enabled =>
        {
            if (_updatingEffects)
                return;

            var card = SelectedCard();

            if (card == null)
                return;

            if (enabled)
                card.AddKeyword(keyword);
            else
                card.RemoveKeyword(keyword);

            _status.Text =
                $"{card.Title}: {title} " +
                (enabled ? "追加" : "解除");
        };

        tab.AddChild(check);
        return check;
    }

    private CardModel? SelectedCard()
    {
        int selected = _cardSelector.Selected;

        if (selected < 0)
            return null;

        int id = _cardSelector.GetItemId(selected);

        return _handCards.GetValueOrDefault(id);
    }

    private void RefreshHandCards()
    {
        _updatingEffects = true;

        try
        {
            _handCards.Clear();
            _cardSelector.Clear();

            int id = 0;

            foreach (var card in
                _player.PlayerCombatState.AllCards.Where(
                    c => c.Pile?.Type == PileType.Hand))
            {
                _handCards.Add(id, card);

                _cardSelector.AddItem(
                    $"{card.Title} " +
                    $"({card.GetType().Name}) #{id + 1}",
                    id);

                id++;
            }

            if (_cardSelector.ItemCount > 0)
                _cardSelector.Select(0);
        }
        finally
        {
            _updatingEffects = false;
        }

        UpdateEffectControls();
    }

    private void UpdateEffectControls()
    {
        _updatingEffects = true;

        try
        {
            var card = SelectedCard();
            bool enabled = card != null;

            _replay.Editable = enabled;
            _retain.Disabled = !enabled;
            _ethereal.Disabled = !enabled;
            _exhaust.Disabled = !enabled;

            _replay.Value = card?.BaseReplayCount ?? 0;

            _retain.ButtonPressed =
                card?.Keywords.Contains(
                    CardKeyword.Retain) ?? false;

            _ethereal.ButtonPressed =
                card?.Keywords.Contains(
                    CardKeyword.Ethereal) ?? false;

            _exhaust.ButtonPressed =
                card?.Keywords.Contains(
                    CardKeyword.Exhaust) ?? false;
        }
        finally
        {
            _updatingEffects = false;
        }
    }

    // ========================================
    // リソース
    // ========================================

    private void BuildResourcesTab(TabContainer tabs)
    {
        var tab = new VBoxContainer
        {
            Name = "リソース"
        };

        tabs.AddChild(tab);

        tab.AddChild(new Label
        {
            Text = "リソース調整（戦闘中）"
        });

        AddResourceRow(
            tab,
            "エナジー",
            DebugResourceHelper.AdjustEnergy);

        AddResourceRow(
            tab,
            "詠唱",
            DebugResourceHelper.AdjustChant);

        AddResourceRow(
            tab,
            "火種",
            DebugResourceHelper.AdjustEmber);

        _resourceStatus = new Label();
        tab.AddChild(_resourceStatus);

        var refresh = new Button
        {
            Text = "現在値を更新"
        };

        refresh.Pressed += UpdateResourceStatus;
        tab.AddChild(refresh);
    }

    // ========================================
    // 敵HP
    // ========================================

    private void BuildEnemiesTab(TabContainer tabs)
    {
        var tab = new VBoxContainer
        {
            Name = "敵HP"
        };

        tab.AddChild(new Label
        {
            Text = "戦闘中の敵を選択してHPを変更します"
        });

        var row = new HBoxContainer();
        tab.AddChild(row);

        _enemySelector = new OptionButton
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        _enemySelector.ItemSelected += _ =>
            UpdateEnemyStatus();

        row.AddChild(_enemySelector);

        var refresh = new Button
        {
            Text = "敵一覧を更新"
        };

        refresh.Pressed += RefreshEnemies;
        row.AddChild(refresh);

        tab.AddChild(new Label
        {
            Text = "設定するHP（1～999999）"
        });

        _enemyHp = new SpinBox
        {
            MinValue = 1,
            MaxValue = 999999,
            Step = 1,
            Value = 9999
        };

        tab.AddChild(_enemyHp);

        _changeMaxHp = new CheckBox
        {
            Text = "最大HPも同じ値に変更する",
            ButtonPressed = true
        };

        tab.AddChild(_changeMaxHp);

        var apply = new Button
        {
            Text = "選択した敵のHPを変更"
        };

        apply.Pressed += () =>
        {
            if (_busy)
                return;

            var id = _enemySelector.Selected;

            if (id < 0 ||
                !_enemies.TryGetValue(
                    _enemySelector.GetItemId(id),
                    out var enemy))
            {
                return;
            }

            _busy = true;

            try
            {
                DebugEnemyHelper.SetHp(
                    enemy,
                    (int)_enemyHp.Value,
                    _changeMaxHp.ButtonPressed);

                UpdateEnemyStatus();

                _status.Text = "敵のHPを変更しました";
            }
            catch (Exception ex)
            {
                GD.PushError(
                    $"[Hinamori Debug] HP変更失敗: {ex}");

                _status.Text =
                    $"HP変更失敗: " +
                    ex.GetBaseException().Message;
            }
            finally
            {
                _busy = false;
            }
        };

        tab.AddChild(apply);

        _enemyStatus = new Label();
        tab.AddChild(_enemyStatus);

        tab.AddChild(new Label
        {
            Text = "※HP変更はデバッグ用の暫定実装です。" +
                   "ゲーム版によってはHP更新APIへの対応が必要です。"
        });

        RefreshEnemies();
    }

    private void RefreshEnemies()
    {
        _enemies.Clear();
        _enemySelector.Clear();

        int i = 0;

        foreach (var enemy in DebugEnemyHelper.Enemies(_player))
        {
            _enemies[i] = enemy;

            _enemySelector.AddItem(
                $"敵 {i + 1}: {enemy.GetType().Name}",
                i);

            i++;
        }

        if (i > 0)
            _enemySelector.Select(0);

        UpdateEnemyStatus();
    }

    private void UpdateEnemyStatus()
    {
        int index = _enemySelector.Selected;

        if (index < 0 ||
            !_enemies.TryGetValue(
                _enemySelector.GetItemId(index),
                out var enemy))
        {
            _enemyStatus.Text = "対象の敵がいません";
            return;
        }

        _enemyStatus.Text =
            $"現在HP: " +
            $"{DebugEnemyHelper.CurrentHp(enemy)?.ToString() ?? "不明"}" +
            " / " +
            $"最大HP: " +
            $"{DebugEnemyHelper.MaxHp(enemy)?.ToString() ?? "不明"}";
    }

    // ========================================
    // カード一覧・カード生成
    // ========================================

    private void RefreshCards()
    {
        HideCardPreview();

        foreach (var child in _results.GetChildren())
        {
            _results.RemoveChild(child);
            child.QueueFree();
        }

        var keyword = _search.Text.Trim();

        foreach (var type in _cards.Where(t =>
            t.Name.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase) ||
            DisplayName(t).Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase)))
        {
            var selectedType = type;

            var button = new Button
            {
                Text = DisplayName(type),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            // カーソルを合わせるとプレビュー表示
            button.MouseEntered += () =>
            {
                ShowCardPreview(selectedType);
            };

            // カーソルを外すとプレビュー非表示
            button.MouseExited += () =>
            {
                if (_hoveredCardType == selectedType)
                    HideCardPreview();
            };

            // クリックするとカード生成
            button.Pressed += async () =>
            {
                if (_busy)
                    return;

                _busy = true;

                try
                {
                    await CardDebugHelper.AddCard(
                        _player,
                        selectedType,
                        _upgraded.ButtonPressed,
                        (PileType)_destinationSelector.GetSelectedId());

                    _status.Text =
                        $"追加しました: {DisplayName(selectedType)} → {_destinationSelector.GetItemText(_destinationSelector.Selected)}";

                    RefreshHandCards();
                    UpdateResourceStatus();
                }
                catch (Exception ex)
                {
                    GD.PushError(
                        $"[Hinamori Debug] {ex}");

                    _status.Text =
                        $"追加失敗: " +
                        ex.GetBaseException().Message;
                }
                finally
                {
                    _busy = false;
                }
            };

            _results.AddChild(button);
        }
    }

    private void AddResourceRow(
        VBoxContainer root,
        string name,
        Func<Player, int, System.Threading.Tasks.Task> adjust)
    {
        var row = new HBoxContainer();
        root.AddChild(row);

        row.AddChild(new Label
        {
            Text = name,
            CustomMinimumSize = new Vector2(75, 0)
        });

        foreach (int delta in new[] { -10, -1, 1, 10 })
        {
            int change = delta;

            var button = new Button
            {
                Text = change > 0
                    ? $"+{change}"
                    : change.ToString(),

                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };

            button.Pressed += async () =>
            {
                if (_busy)
                    return;

                _busy = true;

                try
                {
                    await adjust(_player, change);

                    UpdateResourceStatus();

                    _status.Text =
                        $"{name}を{change:+#;-#;0}調整しました";
                }
                catch (Exception ex)
                {
                    GD.PushError(
                        $"[Hinamori Debug] {ex}");

                    _status.Text =
                        $"{name}調整失敗: " +
                        ex.GetBaseException().Message;
                }
                finally
                {
                    _busy = false;
                }
            };

            row.AddChild(button);
        }
    }

    private void UpdateResourceStatus()
    {
        _resourceStatus.Text =
            $"現在値  エナジー: " +
            $"{DebugResourceHelper.Energy(_player)}" +
            $"  /  詠唱: " +
            $"{DebugResourceHelper.Chant(_player)}" +
            $"  /  火種: " +
            $"{DebugResourceHelper.Ember(_player)}";
    }

    private void Close()
    {
        HideCardPreview();
        Closed?.Invoke();
        QueueFree();
    }
}

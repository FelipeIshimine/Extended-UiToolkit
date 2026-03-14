using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ResourceDisplay : VisualElement
{
    // ── USS class names ───────────────────────────────────────────────
    public static readonly string UssClassName          = "resource-display";
    public static readonly string IconUssClassName      = UssClassName + "__icon";
    public static readonly string LabelUssClassName     = UssClassName + "__label";
    public static readonly string SufficientUssClass    = UssClassName + "--sufficient";
    public static readonly string InsufficientUssClass  = UssClassName + "--insufficient";

    // ── Child elements ────────────────────────────────────────────────
    private readonly VisualElement _icon;
    private readonly Label         _label;

    // ── Backing fields ────────────────────────────────────────────────
    private int                  _currentValue;
    private int                  _maxValue;
    private Sprite               _sprite;
    private ResourceDisplayMode  _displayMode;

    // ── UXML attributes ───────────────────────────────────────────────
    [UxmlAttribute]
    public int CurrentValue
    {
        get => _currentValue;
        set { _currentValue = value; Refresh(); }
    }

    [UxmlAttribute]
    public int MaxValue
    {
        get => _maxValue;
        set { _maxValue = value; Refresh(); }
    }

    [UxmlAttribute]
    public ResourceDisplayMode DisplayMode
    {
        get => _displayMode;
        set { _displayMode = value; Refresh(); }
    }

    // Sprite can't be set via UXML directly; set it from code.
    public Sprite Icon
    {
        get => _sprite;
        set { _sprite = value; ApplySprite(); }
    }

    // ── Constructor ───────────────────────────────────────────────────
    public ResourceDisplay()
    {
        AddToClassList(UssClassName);

        // Icon
        _icon = new VisualElement();
        _icon.AddToClassList(IconUssClassName);
        Add(_icon);

        // Label
        _label = new Label();
        _label.AddToClassList(LabelUssClassName);
        Add(_label);

        Refresh();
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Convenience setter for all values at once.
    /// </summary>
    public void SetValues(int current, int max, Sprite sprite = null,
        ResourceDisplayMode mode = ResourceDisplayMode.Default)
    {
        _currentValue = current;
        _maxValue     = max;
        _displayMode  = mode;

        if (sprite != null)
        {
            _sprite = sprite;
            ApplySprite();
        }

        Refresh();
    }

    // ── Private helpers ───────────────────────────────────────────────

    private void Refresh()
    {
        _label.text = BuildLabelText();
        UpdateSufficiencyClass();
    }

    private string BuildLabelText()
    {
        return _displayMode switch
        {
            ResourceDisplayMode.Default    => $"{_currentValue}/{_maxValue}",
            ResourceDisplayMode.Countdown  => $"{_maxValue - _currentValue}",
            ResourceDisplayMode.Percentage => FormatPercentage(),
            ResourceDisplayMode.CurrentOnly => $"{_currentValue}",
            _                              => string.Empty,
        };
    }

    private string FormatPercentage()
    {
        if (_maxValue == 0) return "0%";
        float pct = (float)_currentValue / _maxValue * 100f;
        return $"{pct:0.#}%";
    }

    /// <summary>
    /// Adds a USS modifier class depending on whether current >= max.
    /// Useful for tinting the label red/green from USS.
    /// </summary>
    private void UpdateSufficiencyClass()
    {
        bool sufficient = _currentValue >= _maxValue;
        EnableInClassList(SufficientUssClass,   sufficient);
        EnableInClassList(InsufficientUssClass, !sufficient);
    }

    private void ApplySprite()
    {
        if (_sprite == null)
        {
            _icon.style.backgroundImage = StyleKeyword.None;
            return;
        }

        _icon.style.backgroundImage = new StyleBackground(_sprite);
    }
}

public enum ResourceDisplayMode
{
    /// <summary> Shows: {current}/{max} </summary>
    Default,

    /// <summary> Shows: {max - current} </summary>
    Countdown,

    /// <summary> Shows: {current/max} as percentage e.g. 75% </summary>
    Percentage,

    /// <summary> Shows: {current} </summary>
    CurrentOnly,
}
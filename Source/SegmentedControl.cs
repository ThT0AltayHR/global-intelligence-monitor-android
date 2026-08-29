namespace GlobalIntelligence.CustomControls;

public class SegmentedControl : StackLayout
{
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList<string>), typeof(SegmentedControl),
        propertyChanged: OnItemsSourceChanged
    );

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex), typeof(int), typeof(SegmentedControl), 0
    );

    public IList<string> ItemsSource
    {
        get => (IList<string>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public SegmentedControl()
    {
        Orientation = StackOrientation.Horizontal;
        Spacing = 0;
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SegmentedControl)bindable;
        control.CreateSegments();
    }

    private void CreateSegments()
    {
        Children.Clear();
        if (ItemsSource == null) return;

        for (int i = 0; i < ItemsSource.Count; i++)
        {
            var index = i;
            var button = new Button
            {
                Text = ItemsSource[i],
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = i == SelectedIndex ? Colors.Cyan : Colors.Gray,
                TextColor = Colors.White,
                CornerRadius = 0,
                BorderWidth = 1,
                BorderColor = Colors.Cyan
            };

            button.Clicked += (s, e) => SelectedIndex = index;
            Children.Add(button);
        }
    }
}

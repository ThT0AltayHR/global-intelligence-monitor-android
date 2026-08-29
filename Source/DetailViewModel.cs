using GlobalIntelligence.MVVM;

namespace GlobalIntelligence.ViewModels;

public class DetailViewModel : BaseViewModel
{
    private string _itemId = "";
    private string _itemTitle = "";
    private string _itemDescription = "";
    private string _itemImage = "";
    private string _itemDetails = "";
    private bool _isFavorite = false;
    private DateTime _itemDate = DateTime.Now;

    public string ItemId { get => _itemId; set => SetProperty(value, nameof(ItemId)); }
    public string ItemTitle { get => _itemTitle; set => SetProperty(value, nameof(ItemTitle)); }
    public string ItemDescription { get => _itemDescription; set => SetProperty(value, nameof(ItemDescription)); }
    public string ItemImage { get => _itemImage; set => SetProperty(value, nameof(ItemImage)); }
    public string ItemDetails { get => _itemDetails; set => SetProperty(value, nameof(ItemDetails)); }
    public bool IsFavorite { get => _isFavorite; set => SetProperty(value, nameof(IsFavorite)); }
    public DateTime ItemDate { get => _itemDate; set => SetProperty(value, nameof(ItemDate)); }

    public DetailViewModel()
    {
        Title = "Details";
    }

    public RelayCommand ToggleFavoriteCommand => new(() =>
    {
        IsFavorite = !IsFavorite;
    });

    public RelayCommand ShareCommand => new(async () =>
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = $"{ItemTitle}\n\n{ItemDescription}",
            Title = "Share"
        });
    });
}

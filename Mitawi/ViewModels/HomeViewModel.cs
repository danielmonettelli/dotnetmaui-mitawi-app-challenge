namespace Mitawi.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IWeatherDataService _weatherDataService;

    [ObservableProperty]
    private bool isAnimation;

    [ObservableProperty]
    private List<Hourly> hourlies;

    [ObservableProperty]
    private Hourly myHourly;

    [ObservableProperty]
    private List<Daily> days;

    [ObservableProperty]
    private Placemark myPlacemark;

    public HomeViewModel(IWeatherDataService weatherDataService)
    {
        _weatherDataService = weatherDataService;

        WeatherDataFlow();
    }

    [RelayCommand]
    private void FullUpdate()
    {
        WeatherDataFlow();
    }

    [RelayCommand]
    private void SelectedHourly(Hourly hourly)
    {
        if (hourly is not null)
        {
            IsBusy = true;

            Task.Delay(1000).ContinueWith((t) =>
            {
                IsBusy = false;
            });

            MyHourly = hourly;
        }
    }

    [RelayCommand]
    private async Task DailyForecast7Days()
    {
        Dictionary<string, object> daysParameter = new()
        {
            [nameof(Days)] = Days
        };

        await Shell.Current.GoToAsync(
            state: nameof(HomeDetailPage),
            animate: true,
            parameters: daysParameter);
    }

    private async Task WeatherDataFlow()
    {
        IsBusy = true;

        var placemarkTask = _weatherDataService.GetPlacemarkAsync();
        var daysTask = _weatherDataService.GetDaysAsync();
        var hourliesTask = _weatherDataService.GetHourliesAsync();

        await Task.WhenAll(placemarkTask, daysTask, hourliesTask);

        MyPlacemark = await placemarkTask;
        Days = await daysTask;
        Hourlies = await hourliesTask;

        // Get current time schedule
        MyHourly = Hourlies.ElementAt(0);

        IsBusy = false;
    }

}

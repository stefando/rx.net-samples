using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using Wpf.Utils;

namespace Wpf.Autocomplete
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly Subject<string> _inputObservable = new Subject<string>();

        public MainWindowViewModel()
        {
            _inputObservable
                .Do(x => InputLog += $"{x}\n") // .Do() is for side-effects only
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(x =>
                {
                    if (string.IsNullOrEmpty(x))
                    {
                        Results = "Type search term...";
                        return Observable.Empty<SearchResult>();
                    }

                    Results = $"Searching '{x}' (api call)...";
                    return CallBackendApiSimple(x).Catch<SearchResult, Exception>(e =>
                        {
                            Results = $"api failure: {e.Message}";
                            return Observable.Empty<SearchResult>();
                        });
                })
                .Switch() // flatten, but unsubscribe previous inner observable first then subscribe current one (see Merge())
                .Subscribe(
                    x => Results = $"Results:\v{string.Join("\n", x.Results)}",
                    e => Results = $"observable died: {e}");
        }

        private IObservable<SearchResult> CallBackendApiSimple(string searchTerm)
        {
            return Observable.FromAsync(async () => await _httpClient.GetStringAsync("http://localhost:1337?term=" + searchTerm))
                .Select(resp =>
                {
                    var results = JsonSerializer.Deserialize<string[]>(resp);
                    return new SearchResult(results);
                });
        }

        private IObservable<SearchResult> CallBackendApiSupportCancellation(string searchTerm)
        {
            return Observable.Create<string>(async (observer, token) =>
                {
                    token.Register(() => InputLog += $"Cancelling '{searchTerm}'\n");
                    var response = await  _httpClient.GetAsync("http://localhost:1337?term=" + searchTerm, token);
                    token.ThrowIfCancellationRequested();
                    if (!response.IsSuccessStatusCode)
                    {
                        observer.OnError(new HttpRequestException($"HTTP exception: {(int)response.StatusCode} {response.ReasonPhrase}"));
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        token.ThrowIfCancellationRequested();
                        observer.OnNext(json);
                        observer.OnCompleted();
                    }
                    return () => InputLog += $"Disposing/unsubscribing '{searchTerm}'\n";
                })
                .Select(resp =>
                {
                    var results = JsonSerializer.Deserialize<string[]>(resp);
                    return new SearchResult(results);
                });
        }

        private IObservable<SearchResult> CallBackendApiSupportCancellation2(string searchTerm)
        {
            return Observable.FromAsync(async token =>
                {
                    token.Register(() => InputLog += $"Cancelling '{searchTerm}'\n");
                    var response = await  _httpClient.GetAsync("http://localhost:1337?term=" + searchTerm, token);
                    token.ThrowIfCancellationRequested();
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                })
                .Select(resp =>
                {
                    var results = JsonSerializer.Deserialize<string[]>(resp);
                    return new SearchResult(results);
                });
        }

        private IObservable<SearchResult> CallBackendApiBlocking(string searchTerm)
        {
            return Observable.Create<string>(observer =>
                {
                    var json = _httpClient.GetStringAsync("http://localhost:1337?term=" + searchTerm).Result;
                    observer.OnNext(json);
                    observer.OnCompleted();
                    return Disposable.Empty;
                })
                //.SubscribeOn(Scheduler.ThreadPool)
                .Select(resp =>
                {
                    var results = JsonSerializer.Deserialize<string[]>(resp);
                    return new SearchResult(results);
                });
        }

        public string Input
        {
            set => _inputObservable.OnNext(value);
        }

        private string _inputLog;
        public string InputLog
        {
            private set
            {
                _inputLog = value;
                OnPropertyChanged();
            }
            get => _inputLog;
        }

        private string _results;
        public string Results
        {
            private set
            {
                _results = value;
                OnPropertyChanged();
            }
            get => _results;
        }
    }
}
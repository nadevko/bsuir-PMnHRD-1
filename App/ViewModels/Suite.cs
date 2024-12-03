using System.Reactive;
using PMnHRD1.App.Models;
using ReactiveUI;
using Splat;

namespace PMnHRD1.App.ViewModels;

public class Suite : ReactiveObject, IRoutableViewModel
{
    public IScreen HostScreen => Locator.Current.GetService<Services.INavigate>()!.Screen;
    public string UrlPathSegment { get; } = "test";
    public ReactiveCommand<Unit, IRoutableViewModel> GoBack { get; }
    public ReactiveCommand<ITest, IRoutableViewModel> GoTest { get; }

    public Suite(Models.Suite suite)
    {
        Data = suite;
        GoBack = ReactiveCommand.CreateFromObservable(
            () => HostScreen!.Router.NavigateBack.Execute(Unit.Default)
        )!;
        GoTest = ReactiveCommand.CreateFromObservable<ITest, IRoutableViewModel>(test =>
            HostScreen.Router.Navigate.Execute(new Question(test))
        );
    }

    public Models.Suite Data { get; }
}
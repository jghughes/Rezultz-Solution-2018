using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.ViewModels01.April2022.Dialogs
{
    public interface IModalDialogService
    {
        bool? DialogResult { get; set; }

        /// <summary>
        ///     Shows the child window asynchronously (meant to!) and waits for clicking of OK button
        ///     that sets DialogResult property to true.. Returns DialogResult.
        ///     Problematic in Silverlight. Execution doesn't pausing to wait for a DialogResult.
        ///     In Silverlight - unlike WPF and Win10 - the ChildWindow.Show method is maddeningly nonblocking.
        ///     Meant to return DialogResult. In Silverlight always returns null.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TUserControl"></typeparam>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns>Meant to return DialogResult. In Silverlight always returns null.</returns>
        Task<bool> ShowDialogAsync<TUserControl, TViewModel>(TUserControl view, TViewModel viewModel)
            where TUserControl : IDialogUserControl where TViewModel : IDialogUserControlViewModel;

        /// <summary>
        ///     Shows the child window asynchronously (meant to!) and waits for clicking of OK button or Cancel button
        ///     that sets DialogResult property to true or false. Returns DialogResult.
        ///     Problematic in Silverlight. Execution doesn't pausing to wait for a DialogResult.
        ///     In Silverlight - unlike WPF and Win10 - the ChildWindow.Show method is maddeningly nonblocking.
        ///     Meant to return DialogResult. In Silverlight always returns null.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TUserControl"></typeparam>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        Task<bool?> ShowDialogAndGetConfirmationAsync<TUserControl, TViewModel>(TUserControl view, TViewModel viewModel)
            where TUserControl : IDialogUserControl where TViewModel : IDialogUserControlViewModel;
    }
}
using System;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    /// <summary>
    ///     Interface that is minimally satisfied by Silverlight MessageBox and ChildWindow classes for starters.
    /// </summary>
    public interface IDialogUserControl
    {
        #region handlers

        event EventHandler Closed; // experiment

        #endregion

        #region methods       

        /// <summary>
        ///     Shows the alert message asynchronously (meant to!) and waits for clicking of OK button or Cancel button
        ///     that sets DialogResult property to true or false. Returns DialogResult.
        ///     Problematic in Silverlight. Execution isn't pausing here to wait for a DialogResult.
        ///     In Silverlight - unlike WPF and Win10 - the ChildWindow.Show method is maddeningly nonblocking.
        /// </summary>
        /// <returns>
        ///     Meant to return DialogResult from base.Closed event. In Silverlight always returns null.
        /// </returns>
        void Show();

        #endregion

        #region props

        object DataContext { get; set; }

        /// <summary>
        ///     Gets the dialogue result. Initialised to null.
        ///     Set to True if user exits childwindow by clicking OK.
        /// </summary>
        /// <value>
        ///     The dialogue result.
        /// </value>
        bool? DialogResult { get; set; }

        #endregion
    }
}
using MetroRadiance.UI.Controls;

using Prism.Services.Dialogs;

namespace Robock.Views.Windows
{
    /// <summary>
    ///     MetroDialogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MetroDialogWindow : MetroWindow, IDialogWindow
    {
        public MetroDialogWindow()
        {
            InitializeComponent();
        }

        public IDialogResult Result { get; set; }

        object IDialogWindow.Content
        {
            get => MainContent.Content;
            set => MainContent.Content = value;
        }
    }
}
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab22
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int richTextBoxid = 0;
        public MainWindow()
        {
            InitializeComponent();
            fontComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            sizeComboBox.ItemsSource = new List<Double> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text Format (*.txt)|*.txt|all files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream filestream = new FileStream(dlg.FileName, FileMode.Open);

                string[] splitedPath = filestream.Name.Split('\\');

                TabItem tabItem = new TabItem();
                tabItem.Header = splitedPath[splitedPath.Length - 1];
                tabItem.Name = "Document" + richTextBoxid.ToString();

                RichTextBox richTextBox = new RichTextBox();
                richTextBox.Name = "DocRichTextBox" + richTextBoxid;
                richTextBox.AcceptsReturn = true;
                richTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                range.Load(filestream, DataFormats.Text);

                tabItem.Content = richTextBox;

                fileTabControl.Items.Add(tabItem);

                tabItem.IsSelected = true;
            }
        }

        public void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Format (*.txt)|*.txt|all files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                TabItem currentTab = fileTabControl.SelectedItem as TabItem;

                if (currentTab != null)
                {
                    RichTextBox currentRichTextBox = currentTab.Content as RichTextBox;

                    FileStream filestream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    TextRange range = new TextRange(currentRichTextBox.Document.ContentStart, currentRichTextBox.Document.ContentEnd);
                    range.Save(filestream, DataFormats.Text);

                }
            }

        }
        private void fontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontComboBox.SelectedItem != null)
            {
                TabItem currentTab = fileTabControl.SelectedItem as TabItem;

                if (currentTab != null)
                {
                    RichTextBox currentRichTextBox = currentTab.Content as RichTextBox;

                    currentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontComboBox.SelectedItem);
                }
            }
        }

        private void sizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TabItem currentTab = fileTabControl.SelectedItem as TabItem;

            if (currentTab != null)
            {
                RichTextBox currentRichTextBox = currentTab.Content as RichTextBox;

                double size = 12;

                if (sizeComboBox.Text != null)
                {
                    if (!double.TryParse(sizeComboBox.Text, out size))
                    {
                        size = 12;
                    }
                }

                currentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, size);
            }

        }

        // Кнопка для вставлення зображення. Копистувач обирає зображення, програма отримує поточний файл над яким працює користувач,
        // отримує RichTextBox. Програма відкриває зображення у BitMap та передає його у об'єкт класу Image. Отримуємо поточну позицію курсору.
        // Вставляємо InlineUIContainer з зображенням на місце курсору, встановюємо курсор на кінець картинки.
        private void insertPictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Зображення (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                TabItem currentTab = fileTabControl.SelectedItem as TabItem;

                if (currentTab != null)
                {
                    RichTextBox currentRichTextBox = currentTab.Content as RichTextBox;

                    BitmapImage btmImage = new BitmapImage(new Uri(openFileDialog.FileName));
                    Image image = new Image();
                    image.Source = btmImage;
                    image.Width = 200;

                    TextPointer caretPointer = currentRichTextBox.CaretPosition;

                    if (caretPointer != null)
                    {
                        InlineUIContainer inlineUIContainer = new InlineUIContainer(image, caretPointer);

                        currentRichTextBox.CaretPosition = inlineUIContainer.ElementEnd;

                        currentRichTextBox.Focus();
                    }
                }
            }
        }
    }
}
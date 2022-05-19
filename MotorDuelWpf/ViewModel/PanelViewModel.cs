using Model;
using ViewModel;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Uri = System.Uri;

namespace ViewModel
{
    class PanelViewModel : ViewModelBase
    {
        private Brush _fill = new SolidColorBrush((Color) ColorConverter.ConvertFromString("White"));
        private int _rotation = 0;

        public Brush Fill
        {
            get => _fill;
            set { _fill = value; OnPropertyChanged(); }
        }

        public int Rotation
        {
            get => _rotation;
            set { _rotation = value; OnPropertyChanged(); }
        }

        public Point Location { get; private set; }



        public PanelViewModel(int fieldSize, int index)
        {
            var y = index / fieldSize;
            var x = index % fieldSize;
            Location = new Point(x, y);
        }

        
        public static Brush ImageOf(Player player)
        {
            if (player == null) throw new System.ArgumentNullException(nameof(player));

            var imageName = $"pack://application:,,,/Assets/Motorcycle{player.Color}Up.png";
            var uri = new Uri(imageName);
            var image = new BitmapImage(uri);
            return new ImageBrush(image)
            {
                Stretch = Stretch.Uniform
            };
        }

        public static Brush TrailBrushOf(Player player)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(player.TrailColor));
        }
    }
}

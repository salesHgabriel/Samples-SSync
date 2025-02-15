using System.Globalization;

namespace PocClientSync.Converters;

public class FilePathToImageSourceConverter  : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string filePath && System.IO.File.Exists(filePath))
        {
            // Tenta carregar a imagem
            return ImageSource.FromFile(filePath);
        }
            
        // Retorna uma imagem padrão caso o arquivo não seja encontrado
        return ImageSource.FromFile("placeholder.png"); // Imagem padrão
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

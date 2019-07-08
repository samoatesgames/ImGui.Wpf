using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImGui.Wpf.Controls
{
    internal class ImImage : IImGuiControl
    {
        private readonly Image m_image;
        public FrameworkElement WindowsControl => m_image;

        private string m_imagePath;
        private ImageSource m_imageSource;

        public ImImage()
        {
            m_image = new Image();
        }

        public void Update(object[] data)
        {
            if (data[0] is string imagePath)
            {
                if (m_imagePath == imagePath)
                {
                    return;
                }

                if (File.Exists(imagePath))
                {
                    m_image.Source = new BitmapImage(new Uri(imagePath));
                    m_imagePath = imagePath;
                }
            }
            else if (data[0] is ImageSource imageSource)
            {
                if (m_imageSource == imageSource)
                {
                    return;
                }

                m_image.Source = imageSource;
                m_imageSource = imageSource;
            }

        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_image.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            return default(TResult);
        }
    }
}

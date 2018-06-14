using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPhotoFilter
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        BitmapImage openedBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }


        // Open 按鈕
        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            // 產生開啟檔案視窗 OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image files(*.jpg) | *.jpg";

            // 顯示視窗
            Nullable<bool> result = dlg.ShowDialog();

            // 當按下開啟之後的反應 
            if (result == true)
            {
                // 取得檔案路徑 
                string filePath = dlg.FileName;
                openedBitmap = new BitmapImage(new Uri(filePath));
                MainImage.Source = openedBitmap;
            }
        }


        // 濾鏡 - 重設
        private void ResetFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainImage.Source = openedBitmap;
        }


        // 濾鏡 - 調亮
        private void BrightFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 建立可讀寫的 bitmap
            WriteableBitmap wb = new WriteableBitmap(openedBitmap);

            // 複製每一個像素到 PixelData
            uint[] PixelData = new uint[wb.PixelWidth * wb.PixelHeight];
            wb.CopyPixels(PixelData, 4 * wb.PixelWidth, 0);

            // 操作每一個 PixelData 裡的像素
            for (int y = 0; y < wb.PixelHeight; y++)
            {
                for (int x = 0; x < wb.PixelWidth; x++)
                {
                    // 將像素由 uint32 轉為三色 bytes
                    uint pixel = PixelData[y * wb.PixelWidth + x];
                    byte[] dd = BitConverter.GetBytes(pixel);
                    int B = (int)dd[0];
                    int G = (int)dd[1];
                    int R = (int)dd[2];

                    // 操作
                    R += 50;
                    G += 50;
                    B += 50;

                    // 限制在 0 ~ 255
                    if (B < 0) B = 0;
                    if (B > 255) B = 255;
                    if (G < 0) G = 0;
                    if (G > 255) G = 255;
                    if (R < 0) R = 0;
                    if (R > 255) R = 255;

                    // 轉換回 uint32
                    dd[0] = (byte)B;
                    dd[1] = (byte)G;
                    dd[2] = (byte)R;
                    PixelData[y * wb.PixelWidth + x] = BitConverter.ToUInt32(dd, 0);
                }
            }

            // 將操作後的資料轉回 Bitmap 並放入圖片中
            wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), PixelData, 4 * wb.PixelWidth, 0);
            MainImage.Source = wb;
        }


        private void KernelFilter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            float[,] kernel = new float[3, 3]{
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 0 }
            };

            // 建立可讀寫的 bitmap
            WriteableBitmap wb = new WriteableBitmap(openedBitmap);

            // 複製每一個像素到 PixelData
            uint[] PixelData = new uint[wb.PixelWidth * wb.PixelHeight];
            wb.CopyPixels(PixelData, 4 * wb.PixelWidth, 0);

            // 操作每一個 PixelData 裡的像素
            for (int y = 1; y < wb.PixelHeight-1; y++)
            {
                for (int x = 1; x < wb.PixelWidth-1; x++)
                {
                    // 將像素由 uint32 轉為三色 bytes
                    uint pixel = PixelData[y * wb.PixelWidth + x];
                    byte[] dd = BitConverter.GetBytes(pixel);
                    int B = (int)dd[0];
                    int G = (int)dd[1];
                    int R = (int)dd[2];

                    // 操作 (kernel 運算)
                    float sumR = 0;
                    float sumG = 0;
                    float sumB = 0;
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int pos = (y + ky) * wb.PixelWidth + (x + kx);
                            uint _pixel = PixelData[pos];
                            byte[] _dd = BitConverter.GetBytes(_pixel);
                            int _B = (int)_dd[0];
                            int _G = (int)_dd[1];
                            int _R = (int)_dd[2];
                            sumR += kernel[ky + 1, kx + 1] * _R;
                            sumG += kernel[ky + 1, kx + 1] * _G;
                            sumB += kernel[ky + 1, kx + 1] * _B;
                        }
                    }
                    
                    R = (int)sumR;
                    G = (int)sumG;
                    B = (int)sumB;

                    // 限制在 0 ~ 255
                    if (B < 0) B = 0;
                    if (B > 255) B = 255;
                    if (G < 0) G = 0;
                    if (G > 255) G = 255;
                    if (R < 0) R = 0;
                    if (R > 255) R = 255;

                    // 轉換回 uint32
                    dd[0] = (byte)B;
                    dd[1] = (byte)G;
                    dd[2] = (byte)R;
                    PixelData[y * wb.PixelWidth + x] = BitConverter.ToUInt32(dd, 0);
                }
            }

            // 將操作後的資料轉回 Bitmap 並放入圖片中
            wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), PixelData, 4 * wb.PixelWidth, 0);
            MainImage.Source = wb;
        }
    }
}

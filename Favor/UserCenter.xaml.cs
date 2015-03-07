using Favor.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Favor
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserCenter : Page
    {
        private static readonly IEnumerable<string> SupportedImageFileTypes = new List<string> { ".jpeg", ".jpg", ".png", ".bmp" };
        public UserCenter()
        {
            this.InitializeComponent();

            // Attach event which will return the picked files
            var app = Application.Current as App;
            if (app != null)
            {
                app.FilesPicked += OnFilesPicked;
            }
        }

        private async void OnFilesPicked(IReadOnlyList<StorageFile> files)
        {
            Image.Source = null;
            if (files.Count > 0)
            {
                var imageFile = files.FirstOrDefault(f => SupportedImageFileTypes.Contains(f.FileType.ToLower()));
                FavorUser.instance.userImageStorageFile = imageFile;
                if (imageFile != null)
                {
                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(await imageFile.OpenReadAsync());
                    Image.Source = bitmapImage;
                }
            }
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ChoosePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker imagePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp" }
            };
            //FavorUser.instance.userImage =await imagePicker.PickSingleFileAsync();
            imagePicker.PickSingleFileAndContinue();
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (FavorUser.instance.userImageStorageFile != null)
            {
                await FavorUser.instance.UploadUserImage();
                Frame.Navigate(typeof(MissionsWall));
            }
            else
            {
                var dialog = new MessageDialog("Please Choose Photo Please");
                await dialog.ShowAsync();
            }
        }
    }
}

using Favor.DataModel;
using Favor.Common;
using Favor.Controller;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.UI;
using Favor.View;

// “空白应用程序”模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace Favor
{



    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {
        // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x804

      
        private TransitionCollection transitions;

        public static MobileServiceClient MobileService = new MobileServiceClient("https://favor3.azure-mobile.cn/", "JhkGhUGIjXOEqfTsQgIeTVFJvpKtlu78");
        public event Action<IReadOnlyList<StorageFile>> FilesPicked;

        public static Windows.UI.ViewManagement.StatusBar statusBar;
        /// <summary>
        /// 初始化单一实例应用程序对象。    这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以打开特定的文件或显示搜索结果等操作时，
        /// 将使用其他入口点。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                // TODO: 将此值更改为适合您的应用程序的缓存大小
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // 删除用于启动的旋转门导航。
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // 当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 新页面
                //if (AccountLocalStorage.instance.isvaild())
                //{
                //    FavorUser.instance.account = new Account();
                //    AccountLocalStorage.instance.LoadAccount(FavorUser.instance.account);
                //    if (!rootFrame.Navigate(typeof(MissionsWall), e.Arguments))
                //    {
                //        throw new Exception("Failed to create initial page");
                //    }
                //}
                if (!rootFrame.Navigate(typeof(Starting), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }


            statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = Color.FromArgb(255, 255, 143, 61);
            statusBar.BackgroundOpacity = 1;

            statusBar.ProgressIndicator.Text = "Loading";
            // await statusBar.ProgressIndicator.ShowAsync();


            // 确保当前窗口处于活动状态

            Window.Current.Activate();
            // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x804

            await Notifications.instance.RefreshChannel();


        }

        /// <summary>
        /// 启动应用程序后还原内容转换。
        /// </summary>
        /// <param name="sender">附加了处理程序的对象。</param>
        /// <param name="e">有关导航事件的详细信息。</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。    在不知道应用程序
        /// 将被终止还是恢复的情况下保存应用程序状态，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            var fopArgs = args as FileOpenPickerContinuationEventArgs;
            if (fopArgs != null)
            {
                // Pass the picked files to the subscribed event handlers
                // In a real world app you could also use a Messenger, Listener or any other subscriber-based model
                if (fopArgs.Files.Any() && FilesPicked != null)
                {
                    FilesPicked(fopArgs.Files);
                }
            }
            base.OnActivated(args);

            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                App.MobileService.LoginComplete(args as WebAuthenticationBrokerContinuationEventArgs);
            }

        }
    }
}
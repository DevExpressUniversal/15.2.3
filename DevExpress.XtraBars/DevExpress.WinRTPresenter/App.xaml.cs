#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.WinRTPresenter.BackgroundTasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
namespace DevExpress.WinRTPresenter
{
	sealed partial class App : Application {
		public App() {
			this.InitializeComponent();
			this.Suspending += OnSuspending;
		}
		protected override void OnLaunched(LaunchActivatedEventArgs args) {
			OpenRegFile(args);
			Frame rootFrame = Window.Current.Content as Frame;
			if(rootFrame == null) {
				rootFrame = new Frame();
				if(args.PreviousExecutionState == ApplicationExecutionState.Terminated) {
				}
				Window.Current.Content = rootFrame;
			}
			SettingsPane.GetForCurrentView().CommandsRequested -= App_CommandsRequested;
			SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
			if(rootFrame.Content == null) {
				if(!rootFrame.Navigate(typeof(MainPage), args.Arguments)) {
					throw new Exception("Failed to create initial page");
				}
			}
			MainPage mainPage = (MainPage)rootFrame.Content;
			Window.Current.Activate();
		}
		const string appStr = "App";
		const string regFolderName = "reg";
		const string extensionLiveTile = ".livetile";
		async void OpenRegFile(LaunchActivatedEventArgs args) {
			if(args.TileId == appStr) return;
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFolder regFolder = await localFolder.GetFolderAsync(regFolderName);
			StorageFile file = await regFolder.GetFileAsync(args.TileId + extensionLiveTile);
			if(file == null) return;
			Windows.System.LauncherOptions lo = new Windows.System.LauncherOptions() { DisplayApplicationPicker = false };
			bool launchResult = await Windows.System.Launcher.LaunchFileAsync(file, lo);
			if(!launchResult) {
				do launchResult = await Windows.System.Launcher.LaunchFileAsync(file, lo);
				while(!launchResult);
			}
		}
		protected override void OnActivated(IActivatedEventArgs args) {
			if(args.Kind == ActivationKind.Protocol) {
				ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;
				Frame rootFrame = Window.Current.Content as Frame;
				if(rootFrame == null) {
					rootFrame = new Frame();
					if(args.PreviousExecutionState == ApplicationExecutionState.Terminated) {
					}
					Window.Current.Content = rootFrame;
				}
				SettingsPane.GetForCurrentView().CommandsRequested -= App_CommandsRequested;
				SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
				if(rootFrame.Content == null) {
					if(!rootFrame.Navigate(typeof(MainPage))) {
						throw new Exception("Failed to create initial page");
					}
				}
				MainPage mainPage = (MainPage)rootFrame.Content;
				if(protocolArgs.Uri.OriginalString == @"dxlivetilemanager:\demo") mainPage.ShowDemo();
				Window.Current.Activate();
			}
		}
		void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args) {
			args.Request.ApplicationCommands.Add(new AboutFlyoutView().ShowFlayoutCommand);
			args.Request.ApplicationCommands.Add(new SettingsCommand("", "Privacy Policy", new Windows.UI.Popups.UICommandInvokedHandler(OnSettingsCommandExecute)));
		}
		private void OnSettingsCommandExecute(Windows.UI.Popups.IUICommand command) {
			LaunchUrlAsync("http://go.devexpress.com/Win8XAML_Privacy_Policy.aspx");
		}
		private async void LaunchUrlAsync(string url) {
			await Launcher.LaunchUriAsync(new Uri(url));
		}
		private void OnSuspending(object sender, SuspendingEventArgs e) {
			var deferral = e.SuspendingOperation.GetDeferral();
			deferral.Complete();
		}
	}
}

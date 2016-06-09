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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DevExpress.WinRTPresenter.BackgroundTasks;
using Windows.UI;
namespace DevExpress.WinRTPresenter {
	public sealed partial class MainPage : DevExpress.WinRTPresenter.Common.LayoutAwarePage {
		static RegObservableCollection<RegisterWinClass> regList = new RegObservableCollection<RegisterWinClass>();
		static RegObservableCollection<RegisterWinClass> tileIdList = new RegObservableCollection<RegisterWinClass>();
		static DispatcherTimer timer = null;
		static FileMessaging fileMessenger;
		static StorageFolder localFolder;
		static ListView listView;
		static Button pinButton;
		static Image guideImage;
		static bool pinProcess = false;
		public const string regListFileName = "reglist";
		public const string extension = ".livetile";
		public const string regFolderName = "reg";
		const string appDefaultName = "Default name";
		const string defaultImageName = "default.png";
		const string strDesc = "There are apps running on this machine that support the display of Live Tiles within the Start Screen. Click the Pin button to add a Live Tile to the Start Screen, or click the Unpin button to remove a Live Tile from the Start Screen.";
		const string emptyDesc = "The Live Tile Manager handles the display of Live Tiles for applications. Currently, there are no applications running on this machine that support Live Tiles.";
		public ObservableCollection<RegisterWinClass> RegList {
			get { return regList; }
		}
		public MainPage() {
			this.InitializeComponent();
			RequestLockScreenAccess();
			BackgroundTaskRegister();
			InitUI();
			InitFileMessaging();
			InitSyncTimer();
			regList.CollectionChanged += regList_CollectionChanged;
		}
		void regList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(regList.Count > 0) ChangeDescriptionLabel(false);
			else ChangeDescriptionLabel(true);
		}
		void ChangeDescriptionLabel(bool isEmpty) {
			descriptionText.Text = isEmpty == true ? emptyDesc : strDesc;
		}
		void InitSyncTimer() { 
			timer = new DispatcherTimer();
			timer.Interval = new TimeSpan(0,0,0,5,0);
			timer.Tick += timer_Tick;
			timer.Start();
		}
		async void timer_Tick(object sender, object e) {
			if(!listView.IsEnabled) return;
			await SyncUnpinnedTiles();
		}
		public async void ShowDemo() {
			MessageDialog md = new MessageDialog("Activated from demo module ","Helper");
			await md.ShowAsync();
		}
		void InitUI() {
			listView = FindName("listViewItems") as ListView;
			pinButton = FindName("buttonPin") as Button;
			guideImage = FindName("imgGuide") as Image;
		}
		async static void InitFileMessaging() {
			localFolder = ApplicationData.Current.LocalFolder;
			fileMessenger = new FileMessaging() { TileExpireMin = 30 };
			await ReadRegFiles();
		}
		async static Task<List<RegisterWinClass>> GetMismatchTiles() {
			List<RegisterWinClass> pinnedList = new List<RegisterWinClass>();
			IReadOnlyList<SecondaryTile> tilesList = await SecondaryTile.FindAllAsync();
			pinnedList = regList.Where(f => f.TileId != null).ToList();
			foreach(SecondaryTile tile in tilesList) {
				pinnedList.Remove(pinnedList.Find(f => f.TileId == tile.TileId));
			}
			if(pinProcess)
				return new List<RegisterWinClass>();
			return pinnedList;
		}
		async static Task<bool> IsNewRegAdded() {
			StorageFolder regFolder = await localFolder.GetFolderAsync(regFolderName);
			IReadOnlyList<StorageFile> regFilesList = await regFolder.GetFilesAsync();
			List<string> regFileNames = regFilesList.Select(f => f.Name).ToList();
			List<string> regListNames = regList.Select(f => f.Id + extension).ToList();
			regFileNames.Remove(regListFileName);
			if(!regFileNames.SequenceEqual(regListNames))
				return true;
			return false;
		}
		async static Task DeleteInactiveRegs() {
			StorageFolder regFolder = await localFolder.GetFolderAsync(regFolderName);
			IReadOnlyList<StorageFile> regFilesList = await regFolder.GetFilesAsync();
			foreach(StorageFile regFile in regFilesList) {
				RegisterWinClass regListObject = new RegisterWinClass();
				RegisterWinClass regObject = new RegisterWinClass();
				if(regFile.Name == regListFileName) continue;
				try {
					regObject = await fileMessenger.ReadObjectAsync(regFile);
				}
				catch(FileNotFoundException) { continue; }
				try {
					regListObject = regList.First(f => f.Id == regObject.Id);
				}
				catch(InvalidOperationException) { regListObject.TileId = "-1"; }
				if(regObject.ProcessId == 0 && regListObject.TileId == null) {
					try { await regFile.DeleteAsync(); }
					catch { }
				}
			}
		}
		static bool syncStarted = false;
		async static Task SyncUnpinnedTiles() {
			if(!listView.IsEnabled || syncStarted) return;
			syncStarted = true;
			await DeleteInactiveRegs();
			List<RegisterWinClass> pinnedList = await GetMismatchTiles();
			if(pinnedList.Count == 0 && !await IsNewRegAdded()) {
				syncStarted = false;
				return;
			}
			foreach(RegisterWinClass regObject in pinnedList) {
				await UnpinTile(regObject);
			}
			await UpdateView();
			syncStarted = false;
		}
		static void CheckListViewIsEmpty() {
			guideImage.Visibility = listView.Items.Count == 0 ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
		}
		async static Task ReadRegFiles() {
			StorageFolder regFolder = await localFolder.CreateFolderAsync(regFolderName, CreationCollisionOption.OpenIfExists);
			IReadOnlyList<StorageFile> regFilesList = await regFolder.GetFilesAsync();
			if(regList.Count > 0) 
				await regList.SaveAsync();
			tileIdList = await ReadRegListAsync();
			regList.Clear();
			if(regFilesList.Count == 0)
				return;
			foreach(StorageFile regFile in regFilesList) {
				if(regFile.Name == regListFileName) continue;
				RegisterWinClass regObject = new RegisterWinClass();
				try {
					regObject = await fileMessenger.ReadObjectAsync(regFile);
				}
				catch { }
				if(regObject.Id != null && regObject.ExePath != null) {
					try {
						regObject.TileId = tileIdList.First(f => f.Id == regObject.Id).TileId;
					}
					catch(InvalidOperationException) { regObject.TileId = null; }
					regList.Add(regObject);
				}
			}
			await regList.SaveAsync();
		}
		static async Task<RegObservableCollection<RegisterWinClass>> ReadRegListAsync() {
			RegObservableCollection<RegisterWinClass> content = new RegObservableCollection<RegisterWinClass>();
			StorageFolder regFolder = await localFolder.CreateFolderAsync(regFolderName, CreationCollisionOption.OpenIfExists);
			StorageFile regListFile = await regFolder.CreateFileAsync(regListFileName, CreationCollisionOption.OpenIfExists);
			IInputStream sessionInputStream = await regListFile.OpenReadAsync();
			XmlSerializer sessionSerializer = new XmlSerializer(typeof(RegObservableCollection<RegisterWinClass>));
			Stream inStream = Task.Run(() => sessionInputStream.AsStreamForRead()).Result;
			try {
				content = (RegObservableCollection<RegisterWinClass>)sessionSerializer.Deserialize(inStream);
				inStream.Dispose();
			}
			catch {
				content = new RegObservableCollection<RegisterWinClass>();
				inStream.Dispose();
			}
			return content;
		}
		static Style pinStyle = Application.Current.Resources["PinAppBarButtonStyle"] as Style;
		static Style unpinStyle = Application.Current.Resources["UnPinAppBarButtonStyle"] as Style;
		public static async Task UpdateView() {
			await ReadRegFiles();
		}
		async static Task<bool> CreateTile(RegisterWinClass regObject) {
			Uri defaultLogo = null;
			if(await FileMessaging.IsDefaultImageExists(regObject.Id))
				defaultLogo = new Uri("ms-appdata:///local/img/" + regObject.Id + @"/" + defaultImageName);
			else
				defaultLogo = new Uri("ms-appx:///Assets/empty.png");
			Uri smallLogo = new Uri("ms-appx:///Assets/30x30.png");
			string appName = regObject.AppName;
			if(String.IsNullOrEmpty(appName)) 
				appName = appDefaultName;
			string tileActivationArguments = regObject.TileId;
			SecondaryTile secondaryTile = new SecondaryTile(regObject.TileId,
															appName,
															appName,
															tileActivationArguments,
															TileOptions.ShowNameOnWideLogo,
															defaultLogo, defaultLogo);
			secondaryTile.SmallLogo = smallLogo;
			return await secondaryTile.RequestCreateAsync(GetScreenCenter());
		}
		static Point GetScreenCenter() {
			var bounds = Window.Current.Bounds;
			double height = bounds.Height;
			double width = bounds.Width;
			return new Point(width / 2, height / 2);
		}
		async static Task<bool> DeleteTile(string tileId) {
			IReadOnlyList<SecondaryTile> tilesList = await SecondaryTile.FindAllAsync();
			try {
				SecondaryTile secondaryTile = tilesList.First(f => f.TileId == tileId);
				return await secondaryTile.RequestDeleteAsync(GetScreenCenter());
			}
			catch { return false; }
		}
		async static Task<bool> PinTile(RegisterWinClass regObject) {
			string tileIDBackup = regObject.TileId as string;
			if(String.IsNullOrEmpty(regObject.Id))
				return false;
			pinProcess = true;
			regObject.TileId = regObject.Id;
			if(await CreateTile(regObject)) {
				for(int i = 0; i < regList.Count; i++)
					if(regList[i].Id == regObject.Id)
						regList[i] = regObject;
				listView.IsEnabled = true;
				await regList.SaveAsync();
				TileUpdateManager.CreateTileUpdaterForSecondaryTile(regObject.TileId).Clear();
				BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(regObject.TileId).Clear();
				return true;
			}
			else {
				regObject.TileId = tileIDBackup;
				listView.IsEnabled = true;
				await regList.SaveAsync();
				return false;
			}
		}
		async static Task<bool> UnpinTileCore(RegisterWinClass regObject) {
			string tileIdBackup = regObject.TileId as string;
			if(String.IsNullOrEmpty(regObject.Id))
				return false;
			regObject.TileId = null;
			if(await DeleteTile(tileIdBackup) && await fileMessenger.RegisterTile(regObject)) {
				for(int i = 0; i < regList.Count; i++)
					if(regList[i].Id == regObject.Id) {
						regList[i] = regObject;
					}
				listView.IsEnabled = true;
				await regList.SaveAsync();
				FileMessaging.CleanRegistration(tileIdBackup);
				return true;
			}
			else {
				regObject.TileId = tileIdBackup;
				listView.IsEnabled = true;
				await regList.SaveAsync();
				return false;
			}
		}
		async static Task UnpinTile(RegisterWinClass regObject) {
			if(String.IsNullOrEmpty(regObject.Id))
				return;
			string tileIdBackup = regObject.TileId;
			regObject.TileId = null;
			if(await fileMessenger.RegisterTile(regObject)) {
				FileMessaging.CleanRegistration(tileIdBackup);
				return;
			}
			else {
				regObject.TileId = tileIdBackup;
				return;
			}
		}
		public async void RequestLockScreenAccess() {
			MessageDialog messageDialog = new MessageDialog("");
			var status = BackgroundExecutionManager.GetAccessStatus();
			if(status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.Denied)
				status = await BackgroundExecutionManager.RequestAccessAsync();
		}
		public void BackgroundTaskRegister() {
			string taskNameTimer = "LiveTilesUpdateTimeTrigger";
			string taskNameTimeZoneChanging = "LiveTilesUpdateTimeZoneTrigger";
			string taskNameUserPresent = "LiveTilesUpdateUserPresentTrigger";
			bool alreadyRegisteredTimer = false;
			bool alreadyRegisteredTimeZone = false;
			bool alreadyRegisteredUserPresent = false;
			foreach(var task in BackgroundTaskRegistration.AllTasks) {
				if(task.Value.Name == taskNameTimer) alreadyRegisteredTimer = true;
				if(task.Value.Name == taskNameTimeZoneChanging) alreadyRegisteredTimeZone = true;
				if(task.Value.Name == taskNameUserPresent) alreadyRegisteredUserPresent = true;
			}
			if(!alreadyRegisteredTimer) {
				BackgroundTaskBuilder builder = new BackgroundTaskBuilder() { Name = taskNameTimer, TaskEntryPoint = "DevExpress.WinRTPresenter.BackgroundTasks.BackgroundClass" };
				IBackgroundTrigger trigger = new TimeTrigger(15, false);
				builder.SetTrigger(trigger);
				IBackgroundTaskRegistration task = builder.Register();
			}
			if(!alreadyRegisteredTimeZone) {
				BackgroundTaskBuilder builder = new BackgroundTaskBuilder() { Name = taskNameTimeZoneChanging, TaskEntryPoint = "DevExpress.WinRTPresenter.BackgroundTasks.BackgroundClass" };
				IBackgroundTrigger trigger = new SystemTrigger(SystemTriggerType.TimeZoneChange, false);
				builder.SetTrigger(trigger);
				IBackgroundTaskRegistration task = builder.Register();
			}
			if(!alreadyRegisteredUserPresent) {
				BackgroundTaskBuilder builder = new BackgroundTaskBuilder() { Name = taskNameUserPresent, TaskEntryPoint = "DevExpress.WinRTPresenter.BackgroundTasks.BackgroundClass" };
				IBackgroundTrigger trigger = new SystemTrigger(SystemTriggerType.UserPresent, false);
				builder.SetTrigger(trigger);
				IBackgroundTaskRegistration task = builder.Register();
			}
		}
		private async void btnPin_Click(object sender, RoutedEventArgs e) {
			List<RegisterWinClass> mismatchTilesList = await GetMismatchTiles();
			if(mismatchTilesList.Count != 0) {
				await SyncUnpinnedTiles();
				return;
			}
			listView.IsEnabled = false;
			Button button = sender as Button;
			RegisterWinClass regObject = regList.FirstOrDefault(f => f.Id == (string)button.Tag);
			if(regObject == null) return;
			if(button.Style == pinStyle) await PinTile(regObject);
			else await UnpinTileCore(regObject);
			pinProcess = false;
			await SyncUnpinnedTiles();
			await BackgroundClass.UpdateTilesAsync(fileMessenger, null);
		}
		private async void Image_Loaded(object sender, RoutedEventArgs e) {
			Image image = sender as Image;
			RegisterWinClass regObject = regList.FirstOrDefault(f => f.Id == (string)image.Tag);
			if(regObject == null) return;
			if(await FileMessaging.IsDefaultImageExists(regObject.Id)) {
				StorageFolder imgFolder = await localFolder.CreateFolderAsync("img", CreationCollisionOption.OpenIfExists);
				StorageFolder currentImgFolder = await imgFolder.CreateFolderAsync(regObject.Id, CreationCollisionOption.OpenIfExists);
				StorageFile imgFile = await currentImgFolder.GetFileAsync(defaultImageName);
				IRandomAccessStream stream = await imgFile.OpenAsync(FileAccessMode.Read);
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.SetSource(stream);
				image.Source = bitmapImage;
			}
		}
	}
	public class TileIdToStyleConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string culture) {
			if(value == null)
				return Application.Current.Resources["PinAppBarButtonStyle"] as Style;
			else
				return Application.Current.Resources["UnPinAppBarButtonStyle"] as Style;
		}
		public object ConvertBack(object value, Type targettype, object parameter, string culture) {
			throw new NotSupportedException("Implementation not supported");
		}
	}
	public class RegObservableCollection<T> : ObservableCollection<T> {
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
		}
		async public Task SaveAsync() {
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			StorageFolder regFolder = await localFolder.CreateFolderAsync(MainPage.regFolderName, CreationCollisionOption.OpenIfExists);
			StorageFile regListFile = await regFolder.CreateFileAsync(MainPage.regListFileName, CreationCollisionOption.ReplaceExisting);
			IRandomAccessStream writeStream = await regListFile.OpenAsync(FileAccessMode.ReadWrite);
			Stream outStream = Task.Run(() => writeStream.AsStreamForWrite()).Result;
			var sessionSerializer = new XmlSerializer(typeof(RegObservableCollection<RegisterWinClass>), new Type[] { typeof(RegisterWinClass) });
			sessionSerializer.Serialize(outStream, this);
			outStream.Dispose();
		}
	}
}

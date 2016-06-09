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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using DevExpress.WinRTPresenter.BackgroundTasks;
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing.Imaging;
using System.ComponentModel.Design;
using DevExpress.Utils.Win.Hook;
using System.Security.Principal;
using System.Text;
namespace DevExpress.XtraBars.WinRTLiveTiles {
	[Designer("DevExpress.XtraBars.WinRTLiveTiles.DesignTime.WinRTLiveTileManagerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign + AssemblyInfo.FullAssemblyVersionExtension, typeof(IDesigner)),
	Description("Allows you to create and manage live tiles for a WinForms app within the Windows 8 Start screen."),
	DXToolboxItem(false)
	]
	public class WinRTLiveTileManager : Component, ISupportInitialize, IWinRTLiveTileManagerDesignerMethods {
		string id;
		string shortcutFullPath;
		string applicationName = String.Empty;
		Image defaultTileImage = null;
		string launcherDestination = @"DevExpress.WinRTPresenter.Launcher\DevExpress.WinRTPresenter.Launcher.exe";
		const string defaultPackageName = @"Temp.LiveTileManager";
		const string shortcutsPath = @"Microsoft\Windows\Application Shortcuts";
		const string extension = ".livetile";
		const string extensionLnk = ".lnk";
		const string screenshotName = "screen.png";
		const string managerProtocolName = @"dxlivetilemanager:\";
		const string strForceupdate = "forceupdate";
		static string localFolderPath;
		internal static string launcherFullPath;
		static string packageName;
		static LiveTileLauncherHookController hookController;
		bool isWindows8;
		static internal string LocalFolder {
			get { return @"Packages\" + packageName + @"\LocalState"; }
		}
		internal static string lastUpdateTileFileName;
		static bool IsDesignMode = false;
		int changing = 0;
		public static event Action<string> OnNavigated;
		public string Id {
			get { return id; }
			set {
				if(changing == 0) return;
				id = value;
			}
		}
		[DefaultValue(null)]
		public string ApplicationName {
			get { return applicationName; }
			set {
				using(new DesignerNotificationHelper(this, changing)) {
					if(value == null) applicationName = String.Empty;
					applicationName = value;
				}
			}
		}
		[DefaultValue(null)]
		public Image DefaultTileImage {
			get { return defaultTileImage; }
			set {
				using(new DesignerNotificationHelper(this, changing)) {
					defaultTileImage = value;
				}
			}
		}
		public virtual void BeginInit() {
			changing++;
		}
		public virtual void EndInit() {
			changing--;
			if(changing < 0)
				changing = 0;
			Constructor();
		}
		public override ISite Site {
			get { return base.Site; }
			set {
				base.Site = value;
				if(value != null) {
					IDesignerHost host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if(host != null) {
						IComponent component = host.RootComponent;
						if(component is ContainerControl) {
							ContainerControl = (ContainerControl)component;
						}
					}
				}
			}
		}
		ContainerControl parentControl;
		public ContainerControl ContainerControl {
			get { return parentControl; }
			set {
				if(parentControl != value) {
					parentControl = value;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(IsDesignMode || !isWindows8) return;
			LastUpdate();
			WinformClosed();
		}
		public WinRTLiveTileManager() {
			CheckDesignMode();
			CheckWindowsVersion();
		}
		public WinRTLiveTileManager(IContainer container)
			: this() {
			container.Add(this);
		}
		void Constructor() {
			GetEnvironment();
			if(isWindows8) {
				PrepareLauncher();
				RegisterExtension();
			}
			if(IsDesignMode) {
				if(!isWindows8) ShowWindows8OnlyDialog();
				else if(!CheckWinRTAppInstalled()) WinRTAppInstallHelperWindow.Show(true);
				return;
			}
			if(isWindows8) RegisterComponent();
			InitUpdateTimer();
		}
		const string strWin8Only = "The WinRTLiveTileManager component is intended to control the contents of live tiles in Windows 8. The current OS does not support live tiles.";
		const string strWinRTNotFound = "The WinRTLiveTileManager component requires the WinRT application to be installed. You can install it via Windows Store."; 
		ArgumentException WinRTAppNotFoundException {
			get { return new ArgumentException(strWinRTNotFound); }
		}
		ArgumentException InvalidNumericForBadge {
			get { return new ArgumentException("Cannot update a badge. A badge can display any number from 1 to 99. "); }
		}
		const string appName = "DevExpress.DevExpressLiveTileManager";
		const string strDefault = "Default";
		const string getAppxCommand = "-Noninteractive Get-AppxPackage -Name " + appName;
		const string getUserNameCommand = "-Noninteractive (Get-WmiObject -class win32_process | where{$_.ProcessName -eq 'explorer.exe'})[0].getowner() | Select -property user ";
		string GetCurrentUserName() {
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			if(RunConsoleCommand("powershell", getUserNameCommand, stringBuilder) == 0) {
				string output = stringBuilder.ToString();
				output = output.Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
				return output.Length > 0 ? output : strDefault;
			}
			else return strDefault; 
		}
		public static bool CheckWinRTAppInstalled() {
			StringBuilder stringBuilder = new StringBuilder();
			if(RunConsoleCommand("powershell", getAppxCommand, stringBuilder) == 0) {
				string output = stringBuilder.ToString();
				output.Trim();
				return output.Length > 0;
			}
			else return false; 
		}
		static int RunConsoleCommand(string command, string arguments, StringBuilder stringBuilder) {
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				CreateNoWindow = true,
				FileName = command,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				Arguments = arguments
			};
			Process process = new Process() { 
				StartInfo = startInfo,
				EnableRaisingEvents = true
			};
			process.OutputDataReceived += new DataReceivedEventHandler(delegate(object sender, DataReceivedEventArgs e) { stringBuilder.Append(e.Data); });
			try {
				process.Start();
			}
			catch { return -1; }
			process.BeginOutputReadLine();
			process.WaitForExit();
			process.CancelOutputRead();
			return process.ExitCode;
		}
		void ShowWindows8OnlyDialog() {
			Win8ComponentHelper.ShowOSNotSupportedMessage(typeof(WinRTLiveTileManager));
		}
		static System.Timers.Timer updateTimer;
		const double updateInterval = 1001D;
		void InitUpdateTimer() {
			updateTimer = new System.Timers.Timer(updateInterval)
			{
				AutoReset = false,
				Enabled = false,
			};
			updateTimer.Elapsed += updateTimer_Elapsed;
		}
		void updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			updateTimer.Stop();
			ForceUpdate(true);
		}
		void LastUpdate() {
			if(updateTimer.Enabled) ForceUpdate(true);
			updateTimer.Stop();
		}
		static void GetHookController() {
			if(hookController == null)
				hookController = new LiveTileLauncherHookController();
		}
		public static void InitializeNavigation() {
			GetHookController();
		}
		internal delegate void NavigationDelegate(string data);
		internal static void InvokeNavigationEvent(string data) {
			if(OnNavigated != null)
				OnNavigated(data);
		}
		void CheckWindowsVersion() {
			isWindows8 = Win8ComponentHelper.IsOSSupported;
		}
		void CheckDesignMode() {
			IsDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
		}
		void RegisterComponent() {
			RegisterWinClass regObject = new RegisterWinClass();
			string appName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
			regObject.ExePath = System.Windows.Forms.Application.ExecutablePath;
			regObject.Id = this.Id;
			if(String.IsNullOrEmpty(this.ApplicationName))
				regObject.AppName = appName;
			else regObject.AppName = this.ApplicationName;
			regObject.ProcessId = Process.GetCurrentProcess().Id;
			FileMessaging.RegisterWinForm(regObject, localFolderPath);
			FileMessaging.SaveDefaultImage(this.DefaultTileImage, this.Id);
		}
		void WinformClosed() {
			RegisterWinClass regObject = GetClosedRegObject();
			FileMessaging.RegisterWinForm(regObject, localFolderPath);
		}
		RegisterWinClass GetClosedRegObject() {
			RegisterWinClass regObject = new RegisterWinClass();
			regObject.ExePath = System.Windows.Forms.Application.ExecutablePath;
			regObject.Id = this.Id;
			if(String.IsNullOrEmpty(this.ApplicationName))
				regObject.AppName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
			else regObject.AppName = this.ApplicationName;
			regObject.ProcessId = 0;
			return regObject;
		}
		void GetEnvironment() {
			string appDataFolderPath;
			if(CheckRunFromService())
				appDataFolderPath = GetAppDataPath();
			else
				appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			GetPackageName(appDataFolderPath);
			localFolderPath = Path.Combine(appDataFolderPath, LocalFolder);
			launcherFullPath = Path.Combine(appDataFolderPath, launcherDestination);
			string shortcutsFolderPath = Path.Combine(Path.Combine(appDataFolderPath, shortcutsPath), packageName);
			shortcutFullPath = Path.Combine(shortcutsFolderPath, this.Id + extensionLnk);
		}
		bool CheckRunFromService() {
			return WindowsIdentity.GetCurrent().Name.ToString() == @"NT AUTHORITY\SYSTEM";
		}
		static string cachedAppDataPath = null;
		void GetPackageName(string appDataPath) {
			string packagesPath = Path.Combine(appDataPath, "Packages");
			if(!Directory.Exists(packagesPath)) {
				packageName = defaultPackageName;
				return;
			}
			string[] folderList = Directory.GetDirectories(packagesPath, appName + "_*");
			packageName = folderList.Length == 0 ? defaultPackageName : Path.GetFileName(folderList[0]);
		}
		string GetAppDataPath() {
			if(cachedAppDataPath != null) return cachedAppDataPath;
			var userName = GetCurrentUserName();
			var profilesFolder = RegistryHelper.GetProfilesFolder();
			cachedAppDataPath = Path.Combine(profilesFolder, userName + @"\AppData\Local\");
			return cachedAppDataPath;
		}
		void RegisterExtension() {
			if(!RegistryHelper.IsAssociated(extension))
				RegistryHelper.Associate(extension, launcherFullPath, "LiveTile.Document");
		}
		const string launcherResourcePath = "DevExpress.XtraBars.WinRTPresenter.Launcher.DevExpress.WinRTPresenter.Launcher.exe";
		void PrepareLauncher() {
			if(File.Exists(launcherFullPath)) return;
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(launcherResourcePath);
			try { SaveFileFromStream(launcherFullPath, stream); }
			catch { }
		}
		void SaveFileFromStream(string filePath, Stream stream) {
			if(stream.Length == 0) return;
			if(!Directory.Exists(Path.GetDirectoryName(filePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			using(FileStream fileStream = File.Create(filePath, (int)stream.Length)) {
				byte[] streamBytes = new byte[stream.Length];
				stream.Read(streamBytes, 0, (int)streamBytes.Length);
				fileStream.Write(streamBytes, 0, streamBytes.Length);
			}
		}
		TileInfo PrepareTile(TileInfo inputTile) {
			if(inputTile == null) return null;
			if(inputTile.imagesList.Count > 0)
				inputTile.inputArray.AddRange(FileMessaging.SaveImages(inputTile.imagesList));
			return TileTemplateHelper.FillTemplate(inputTile.templateName, inputTile.inputArray);
		}
		UpdateTileResult UpdateTileCore(WideTile wideTile, SquareTile squareTile, bool enableQueue) {
			var checkResult = CheckUpdateRequirements();
			if(checkResult > 0) return checkResult;
			squareTile = (SquareTile)PrepareTile(squareTile);
			wideTile = (WideTile)PrepareTile(wideTile);
			WideTile resultTile = new WideTile();
			if(wideTile == null)
				throw new ArgumentException("wideTile value cannot be null.");
			if(squareTile != null)
				MergeTileTemplates(wideTile, squareTile, out resultTile);
			else
				resultTile = wideTile;
			lastUpdateTileFileName = FileMessaging.Send(this.Id, localFolderPath, resultTile, enableQueue, false);
			StartUpdateTimer();
			return UpdateTileResult.OK;
		}
		public UpdateTileResult UpdateTile(WideTile wideTile, SquareTile squareTile) {
			return UpdateTileCore(wideTile, squareTile, false);
		}
		public UpdateTileResult UpdateTile(WideTile wideTile, SquareTile squareTile, bool enableQueue) {
			return UpdateTileCore(wideTile, squareTile, enableQueue);
		}
		public UpdateTileResult ClearTile() {
			if(!isWindows8) return UpdateTileResult.OSVersionError;
			WideTile resultTile = WideTile.CreateTileWideText04("null");
			resultTile = (WideTile)PrepareTile(resultTile);
			lastUpdateTileFileName = FileMessaging.Send(this.Id, localFolderPath, resultTile, false, true);
			StartUpdateTimer();
			return UpdateTileResult.OK;
		}
		[Browsable(false)]
		public Size ActualWideTileSize { get { return GetActualTileSize(true); } }
		[Browsable(false)]
		public Size ActualSqareTileSize { get { return GetActualTileSize(false); } }
		[Browsable(false)]
		public bool IsStartScreenAvailable { get { return isWindows8; } }
		[SecuritySafeCritical]
		Size GetActualTileSize(bool wide) {
			int screenW, screenH;
			using(Graphics g = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = g.GetHdc();
				screenW = Import.GetDeviceCaps(hdc, 4);
				screenH = Import.GetDeviceCaps(hdc, 6);
				g.ReleaseHdc();
			}
			Size resolution = Screen.PrimaryScreen.Bounds.Size;
			float pixDensity = (float)screenW / resolution.Width;
			if(pixDensity < 0.245f)
				return wide ? new Size(310, 150) : new Size(150, 150);
			else
				return wide ? new Size(248, 120) : new Size(120, 120);
		}
		public void ShowLiveTileManager() {
			bool enableDemoHelper = false;
			if(!isWindows8) return;
			if(!CheckWinRTAppInstalled()) 
				throw WinRTAppNotFoundException;
			if(!FileMessaging.IsRegExist(GetClosedRegObject(), localFolderPath))
				RegisterComponent();
			if(!enableDemoHelper) Process.Start("Explorer.exe", managerProtocolName);
			else Process.Start(managerProtocolName + "demo");
		}
		public void RemoveTile() {
			if(!isWindows8) return;
			try { File.Delete(shortcutFullPath); }
			catch { }
		}
		[Browsable(false)]
		public bool HasPinnedTile {
			get {
				if(!isWindows8) return false;
				GetEnvironment();
				return File.Exists (shortcutFullPath);
			}
		}
		void ShowTileNotPinnedDialog() {
			ShowLiveTileManager();
		}
		void StartUpdateTimer() {
			updateTimer.Stop();
			updateTimer.Start();
		}
		void ForceUpdate(bool enabledForceupdate) {
			if(!isWindows8) return;
			if(!enabledForceupdate) return;
			RegisterExtension();
			string p = launcherFullPath;
			if(!File.Exists(p)) PrepareLauncher();
			ProcessStartInfo procInfo = new ProcessStartInfo(p)
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				Arguments = strForceupdate,
			};
			Process process = new Process() { StartInfo = procInfo };
			process.Start();
		}
		public UpdateTileResult UpdateBadge(int numeric) {
			return UpdateBadgeCore(null, numeric, true);
		}
		public UpdateTileResult UpdateBadge(BadgeGlyphTypes badgeType) {
			return UpdateBadgeCore(badgeType, 0, false);
		}
		UpdateTileResult UpdateBadgeCore(BadgeGlyphTypes? badgeType, int numeric, bool isNumeric) {
			var checkResult = CheckUpdateRequirements();
			if(checkResult > 0) return checkResult;
			if(isNumeric) {
				if(numeric < 0) return UpdateTileResult.InvalidBadgeError;
				FileMessaging.SendBadge(this.Id, localFolderPath, numeric.ToString());
			}
			else
				FileMessaging.SendBadge(this.Id, localFolderPath, badgeType.ToString());
			StartUpdateTimer();
			return UpdateTileResult.OK;
		}
		UpdateTileResult CheckUpdateRequirements() {
			if(!isWindows8) return UpdateTileResult.OSVersionError;
			GetEnvironment();
			if(!FileMessaging.IsRegExist(GetClosedRegObject(), localFolderPath)) RegisterComponent();
			if(!this.HasPinnedTile) return UpdateTileResult.TileNotPinnedError;
			return UpdateTileResult.OK;
		}
		void MergeTileTemplates(WideTile wideTile, SquareTile squareTile, out WideTile resultTile) {
			XmlNode node = wideTile.ImportNode(squareTile.GetElementsByTagName("binding").Item(0), true);
			wideTile.GetElementsByTagName("visual").Item(0).AppendChild(node);
			resultTile = wideTile;
			wideTile = null;
			squareTile = null;
		}
		#region IWinRTLiveTileManagerDesignerMethods Members
		private static readonly object changingEvent = new object();
		private static readonly object changedEvent = new object();
		event EventHandler IWinRTLiveTileManagerDesignerMethods.Changed {
			add { base.Events.AddHandler(changedEvent, value); }
			remove { base.Events.RemoveHandler(changedEvent, value); }
		}
		event CancelEventHandler IWinRTLiveTileManagerDesignerMethods.Changing {
			add { base.Events.AddHandler(changingEvent, value); }
			remove { base.Events.RemoveHandler(changingEvent, value); }
		}
		void IWinRTLiveTileManagerDesignerMethods.RaiseEvent(bool isChanging) {
			if(isChanging) {
				CancelEventHandler handler = (CancelEventHandler)this.Events[changingEvent];
				if(handler != null) handler(this, new CancelEventArgs());
			}
			else {
				EventHandler handler = (EventHandler)this.Events[changedEvent];
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}
		#endregion
	}
	public enum WideTileTypes {
		TileWideText01,
		TileWideText02,
		TileWideText03,
		TileWideText04,
		TileWideText05,
		TileWideText06,
		TileWideText07,
		TileWideText08,
		TileWideText09,
		TileWideText10,
		TileWideText11,
		TileWideImage,
		TileWideImageCollection,
		TileWideImageAndText01,
		TileWideImageAndText02,
		TileWideBlockAndText01,
		TileWideBlockAndText02,
		TileWideSmallImageAndText01,
		TileWideSmallImageAndText02,
		TileWideSmallImageAndText03,
		TileWideSmallImageAndText04,
		TileWideSmallImageAndText05,
		TileWidePeekImageCollection01,
		TileWidePeekImageCollection02,
		TileWidePeekImageCollection03,
		TileWidePeekImageCollection04,
		TileWidePeekImageCollection05,
		TileWidePeekImageCollection06,
		TileWidePeekImageAndText01,
		TileWidePeekImageAndText02,
		TileWidePeekImage01,
		TileWidePeekImage02,
		TileWidePeekImage03,
		TileWidePeekImage04,
		TileWidePeekImage05,
		TileWidePeekImage06
	};
	public enum SquareTileTypes {
		TileSquareBlock,
		TileSquareText01,
		TileSquareText02,
		TileSquareText03,
		TileSquareText04,
		TileSquareImage,
		TileSquarePeekImageAndText01,
		TileSquarePeekImageAndText02,
		TileSquarePeekImageAndText03,
		TileSquarePeekImageAndText04
	};
	public enum BadgeGlyphTypes {
		none,
		activity,
		alert,
		available,
		away,
		busy,
		newMessage,
		paused,
		playing,
		unavailable,
		error,
		attention
	};
	public enum UpdateTileResult { 
		OK,
		OSVersionError,
		TileNotPinnedError,
		InvalidBadgeError
	};
	class Import {
		public enum WM : uint {
			WM_COPYDATA = 0x004A,
			WM_WINDOWPOSCHANGED = 0x0047,
			WM_WINDOWPOSCHANGING = 0x0046
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct COPYDATASTRUCT {
			public IntPtr dwData;
			public int cbData;
			public IntPtr lpData;
		}
		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
	}
	class LiveTileLauncherHookController : IHookController, IDisposable {
		public LiveTileLauncherHookController() {
			HookManager.DefaultManager.AddController(this);
		}
		public bool InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		[SecuritySafeCritical]
		public bool InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(Msg == (int)(uint)Import.WM.WM_COPYDATA) {
				Import.COPYDATASTRUCT copyData = (Import.COPYDATASTRUCT)Marshal.PtrToStructure(LParam, typeof(Import.COPYDATASTRUCT));
				byte[] b = new byte[1];
				int length = copyData.cbData / Marshal.SizeOf(b[0]);
				b = new byte[length];
				Marshal.Copy(copyData.lpData, b, 0, b.Length);
				DevExpress.XtraBars.WinRTLiveTiles.WinRTLiveTileManager.NavigationDelegate navigationDelegate = new
					 WinRTLiveTileManager.NavigationDelegate(WinRTLiveTileManager.InvokeNavigationEvent);
				navigationDelegate.BeginInvoke(GetString(b), null, null);
			}
			return false;
		}
		static string GetString(byte[] bytes) {
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}
		public IntPtr OwnerHandle {
			get { return IntPtr.Zero; }
		}
		public void Dispose() {
			HookManager.DefaultManager.RemoveController(this);
		}
	}
	class DesignerNotificationHelper : IDisposable {
		IWinRTLiveTileManagerDesignerMethods ownerCore;
		int isChangingCore;
		public DesignerNotificationHelper(IWinRTLiveTileManagerDesignerMethods owner, int isChanging) {
			ownerCore = owner;
			isChangingCore = isChanging;
			if(isChangingCore == 0) ownerCore.RaiseEvent(true);
		}
		public void Dispose() {
			if(isChangingCore == 0) ownerCore.RaiseEvent(false);
		}
	}
	public class WideTile : TileInfo {
		internal WideTile() { }
		public static WideTile CreateTileWideText01(string header, string text2, string text3, string text4, string text5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, header, text2, text3, text4, text5);
		}
		public static WideTile CreateTileWideText02(string header, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, header, text2, text3, text4, text5, text6, text7, text8, text9);
		}
		public static WideTile CreateTileWideText03(string header) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, header);
		}
		public static WideTile CreateTileWideText04(string text1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1);
		}
		public static WideTile CreateTileWideText05(string text1, string text2, string text3, string text4, string text5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3, text4, text5);
		}
		public static WideTile CreateTileWideText06(string text1, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9, string text10) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3, text4, text5, text6, text7, text8, text9, text10);
		}
		public static WideTile CreateTileWideText07(string header, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, header, text2, text3, text4, text5, text6, text7, text8, text9);
		}
		public static WideTile CreateTileWideText08(string text1, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9, string text10) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3, text4, text5, text6, text7, text8, text9, text10);
		}
		public static WideTile CreateTileWideText09(string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, header, text2);
		}
		public static WideTile CreateTileWideText10(string header, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, header, text2, text3, text4, text5, text6, text7, text8, text9);
		}
		public static WideTile CreateTileWideText11(string text1, string text2, string text3, string text4, string text5, string text6, string text7, string text8, string text9, string text10) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3, text4, text5, text6, text7, text8, text9, text10);
		}
		public static WideTile CreateTileWideImage(Image image1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1);
		}
		public static WideTile CreateTileWideImageCollection(Image image1, Image image2, Image image3, Image image4, Image image5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5);
		}
		public static WideTile CreateTileWideImageAndText01(Image image1, string text1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, text1);
		}
		public static WideTile CreateTileWideImageAndText02(Image image1, string text1, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, text1, text2);
		}
		public static WideTile CreateTileWideBlockAndText01(string text1, string text2, string text3, string text4, string text5, string text6) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3, text4, text5, text6);
		}
		public static WideTile CreateTileWideBlockAndText02(string text1, string text2, string text3) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3);
		}
		public static WideTile CreateTileWideSmallImageAndText01(Image image1, string header) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header);
		}
		public static WideTile CreateTileWideSmallImageAndText02(Image image1, string header, string text2, string text3, string text4, string text5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2, text3, text4, text5);
		}
		public static WideTile CreateTileWideSmallImageAndText03(Image image1, string text1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, text1);
		}
		public static WideTile CreateTileWideSmallImageAndText04(Image image1, string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2);
		}
		public static WideTile CreateTileWideSmallImageAndText05(Image image1, string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2);
		}
		public static WideTile CreateTileWidePeekImageCollection01(Image image1, Image image2, Image image3, Image image4, Image image5, string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5, header, text2);
		}
		public static WideTile CreateTileWidePeekImageCollection02(Image image1, Image image2, Image image3, Image image4, Image image5, string header, string text2, string text3, string text4, string text5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5, header, text2, text3, text4, text5);
		}
		public static WideTile CreateTileWidePeekImageCollection03(Image image1, Image image2, Image image3, Image image4, Image image5, string header) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5, header);
		}
		public static WideTile CreateTileWidePeekImageCollection04(Image image1, Image image2, Image image3, Image image4, Image image5, string text1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5, text1);
		}
		public static WideTile CreateTileWidePeekImageCollection05(Image image1, Image image2, Image image3, Image image4, Image image5, Image image6, string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5, image6, header, text2);
		}
		public static WideTile CreateTileWidePeekImageCollection06(Image image1, Image image2, Image image3, Image image4, Image image5, Image image6, string header) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, image3, image4, image5, image6, header);
		}
		public static WideTile CreateTileWidePeekImageAndText01(Image image1, string text1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, text1);
		}
		public static WideTile CreateTileWidePeekImageAndText02(Image image1, string text1, string text2, string text3, string text4, string text5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, text1, text2, text3, text4, text5);
		}
		public static WideTile CreateTileWidePeekImage01(Image image1, string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2);
		}
		public static WideTile CreateTileWidePeekImage02(Image image1, string header, string text2, string text3, string text4, string text5) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2, text3, text4, text5);
		}
		public static WideTile CreateTileWidePeekImage03(Image image1, string header) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, header);
		}
		public static WideTile CreateTileWidePeekImage04(Image image1, string text1) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, text1);
		}
		public static WideTile CreateTileWidePeekImage05(Image image1, Image image2, string header, string text2) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, header, text2);
		}
		public static WideTile CreateTileWidePeekImage06(Image image1, Image image2, string header) {
			return CreateTileCore<WideTile>(MethodBase.GetCurrentMethod().Name, image1, image2, header);
		}
	}
	public class SquareTile : TileInfo {
		internal SquareTile() { }
		public static SquareTile CreateTileSquareBlock(string text1, string text2) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, text1, text2);
		}
		public static SquareTile CreateTileSquareText01(string header, string text2, string text3, string text4) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, header, text2, text3, text4);
		}
		public static SquareTile CreateTileSquareText02(string text1, string text2) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, text1, text2);
		}
		public static SquareTile CreateTileSquareText03(string text1, string text2, string text3, string text4) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, text1, text2, text3, text4);
		}
		public static SquareTile CreateTileSquareText04(string text1) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, text1);
		}
		public static SquareTile CreateTileSquareImage(Image image1) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, image1);
		}
		public static SquareTile CreateTileSquarePeekImageAndText01(Image image1, string header, string text2, string text3, string text4) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2, text3, text4);
		}
		public static SquareTile CreateTileSquarePeekImageAndText02(Image image1, string header, string text2) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, image1, header, text2);
		}
		public static SquareTile CreateTileSquarePeekImageAndText03(Image image1, string text1, string text2, string text3, string text4) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, image1, text1, text2, text3, text4);
		}
		public static SquareTile CreateTileSquarePeekImageAndText04(Image image1, string text1) {
			return CreateTileCore<SquareTile>(MethodBase.GetCurrentMethod().Name, image1, text1);
		}
	}
	public class TileInfo : XmlDocument {
		internal TileInfo() { }
		internal List<Image> imagesList = new List<Image>();
		internal List<string> inputArray = new List<string>();
		internal string templateName;
		internal const string createPrefix = "Create";
		protected static T CreateTileCore<T>(string templateName, params object[] args) where T: TileInfo {
			T tile = Activator.CreateInstance(typeof(T), true) as T;
			tile.imagesList = GetListOf<Image>(args);
			tile.inputArray = GetListOf<String>(args);
			tile.templateName = templateName.Replace(createPrefix, "");
			return tile;
		}
		protected static List<T> GetListOf<T>(object[] args) where T : class {
			var selected = args.Where(t => t is T).ToList();
			List<T> result = new List<T>();
			if(selected.Count > 0) {
				foreach(object obj in selected) {
					result.Add(obj as T);
				}
			}
			return result;
		}
	}
	class TileTemplateHelper {
		const string templateFolder = "DevExpress.XtraBars.WinRTPresenter.TileTemplates.";
		public static XmlDocument GetTemplateByName(string templateName) {
			string path = templateFolder + templateName;
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			XmlDocument tileTemplate = new XmlDocument();
			tileTemplate.Load(stream);
			return tileTemplate;
		}
		const string widePrefix = "Wide";
		public static TileInfo GetTileTemplate(string templateName) {
			string path = templateFolder + templateName;
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			if(templateName.Contains(widePrefix)) {
				WideTile result = new WideTile();
				result.Load(stream);
				return result;
			}
			else {
				SquareTile result = new SquareTile();
				result.Load(stream);
				return result;
			}
		}
		public static TileInfo FillTemplate(string templateName, List<string> inputArray) {
			TileInfo tileTemplate;
			if(templateName.Contains(widePrefix))
				tileTemplate = (WideTile)GetTileTemplate(templateName);
			else
				tileTemplate = (SquareTile)GetTileTemplate(templateName);
			XmlNodeList xmlTextNodes = tileTemplate.GetElementsByTagName("text");
			if(xmlTextNodes.Count > 0) {
				for(int i = 0; i <= xmlTextNodes.Count - 1; i++) {
					xmlTextNodes[i].InnerText = inputArray[i];
				}
			}
			XmlNodeList xmlImgNodes = tileTemplate.GetElementsByTagName("image");
			if(xmlImgNodes.Count > 0) {
				for(int i = 0; i <= xmlImgNodes.Count - 1; i++) {
					((XmlElement)xmlImgNodes[i]).SetAttribute("src", "ms-appdata:///local/" + inputArray[(i + xmlTextNodes.Count)]);
				}
			}
			XmlNodeList xmlNL = tileTemplate.GetElementsByTagName("binding");
			XmlAttribute branding = tileTemplate.CreateAttribute("branding");
			if(templateName.StartsWith(brandingNoneName1) || templateName.StartsWith(brandingNoneName2) || templateName.StartsWith(brandingNoneName3) || templateName.StartsWith(brandingNoneName4))
				branding.Value = "none";
			else
				branding.Value = "name";
			foreach(XmlNode xmlNode in xmlNL) {
				xmlNode.Attributes.Append(branding);
			}
			return tileTemplate;
		}
		const string brandingNoneName1 = "TileWideImageAndText01";
		const string brandingNoneName2 = "TileWideImageAndText02";
		const string brandingNoneName3 = "TileWidePeekImageAndText01";
		const string brandingNoneName4 = "TileWidePeekImageAndText02";
	}
	class RegistryHelper {
		public static void Associate(string extension, string launcherExePath, string classDataID) {
			const string subkey = "Software\\Classes";
			RegistryKey classesKey = Registry.CurrentUser.OpenSubKey(subkey, true);
			if(classDataID != null && classDataID.Length > 0) {
				classesKey.CreateSubKey(extension).SetValue("", classDataID);
				using(RegistryKey key = classesKey.CreateSubKey(classDataID)) {
					if(launcherExePath != null && launcherExePath.Length > 0)
						key.CreateSubKey(@"Shell\Open\Command").SetValue("", "\"" + launcherExePath + "\"" + "\"%1\"");
				}
			}
		}
		public static bool IsAssociated(string extension) {
			return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
		}
		public static string GetProfilesFolder() {
			return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList", "ProfilesDirectory", "");
		}
	}
	class FileMessaging {
		const string fileNameMask = "{0}_{1}";
		const string regFolderName = "reg";
		const string imgFolderName = "img";
		const string badgesFolderName = "badges";
		const string iconsFolderName = "icons";
		const string iconName = "icon.png";
		const string extension = ".livetile";
		const string defaultImageName = "default.png";
		const int maxTileQueueLenght = 5;
		public static List<string> SaveImages(List<Image> imageList) {
			string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string localFolderPath = Path.Combine(appDataFolderPath, WinRTLiveTileManager.LocalFolder);
			string imgFolderPath = Path.Combine(localFolderPath, imgFolderName);
			List<string> result = new List<string>();
			if(!Directory.Exists(imgFolderPath)) {
				Directory.CreateDirectory(imgFolderPath);
			}
			foreach(Image image in imageList) {
				if(image == null) continue;
				string imgFileName = Guid.NewGuid().ToString();
				string imgFullPath = Path.Combine(imgFolderPath, imgFileName);
				image.Save(imgFullPath);
				result.Add(Path.Combine(imgFolderName, imgFileName));
			}
			return result;
		}
		public static void SaveDefaultImage(Image defaultImageInput, string componentId) {
			if(String.IsNullOrEmpty(componentId)) return;
			string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string localFolderPath = Path.Combine(appDataFolderPath, WinRTLiveTileManager.LocalFolder);
			string imgFolderPath = Path.Combine(localFolderPath, imgFolderName);
			string componentImageFolder = Path.Combine(imgFolderPath, componentId);
			string defaultImageOutPath = Path.Combine(componentImageFolder, defaultImageName);
			if(!Directory.Exists(componentImageFolder)) {
				Directory.CreateDirectory(componentImageFolder);
			}
			File.Delete(defaultImageOutPath);
			if(defaultImageInput == null) return;
			if(defaultImageInput.PhysicalDimension.Height > 1025 || defaultImageInput.PhysicalDimension.Width > 1025) {
				return;
			}
			else {
				defaultImageInput.Save(defaultImageOutPath);
			}
		}
		public static bool IsRegExist(RegisterWinClass regObject, string localFolderPath) {
			string regFilePath = GetRegFileFullPath(regObject, localFolderPath);
			if(File.Exists(regFilePath)) return true;
			return false;
		}
		public static void RegisterWinForm(RegisterWinClass regObject, string localFolderPath) {
			string regFilePath = GetRegFileFullPath(regObject, localFolderPath);
			regObject.ExePath = System.Windows.Forms.Application.ExecutablePath;
			WriteObjectToFile(regFilePath, regObject);
		}
		static string GetRegFileFullPath(RegisterWinClass regObject, string localFolderPath) {
			if(String.IsNullOrEmpty(regObject.Id)) return string.Empty;
			string regFolderPath = Path.Combine(localFolderPath, regFolderName);
			if(!Directory.Exists(regFolderPath)) {
				Directory.CreateDirectory(regFolderPath);
			}
			return Path.Combine(regFolderPath, regObject.Id + extension);
		}
		static List<string> GetFileNames(string[] filesPaths) {
			List<string> result = new List<string>();
			if(filesPaths.Length <= 0) return result;
			foreach(string filePath in filesPaths) {
				result.Add(Path.GetFileName(filePath));
			}
			return result;
		}
		static void DeleteFile(string fileName, string localFolderPath) {
			string filePath = Path.Combine(localFolderPath, fileName);
			if(File.Exists(filePath))
				File.Delete(filePath);
		}
		static List<string> GetImagesNamesFromMessage(string messageFileName, string localFolderPath) {
			string messageFilePath = Path.Combine(localFolderPath, messageFileName);
			if(!File.Exists(messageFilePath)) return null;
			try {
				MessageClass message = new MessageClass();
				string tileText = File.ReadAllText(messageFilePath);
				XmlDocument xml = new XmlDocument();
				xml.LoadXml(tileText);
				return GetImgNames(xml);
			}
			catch { return null; }
		}
		const string strImage = "image";
		const string strSrc = "src";
		const string strAppdataImg = @"ms-appdata:///local/img\";
		static List<string> GetImgNames(XmlDocument tileTemplate) {
			XmlNodeList xmlImgNodes = tileTemplate.GetElementsByTagName(strImage);
			List<string> result = new List<string>();
			if(xmlImgNodes.Count <= 0) return result;
			foreach(XmlNode xmlImgNode in xmlImgNodes) {
				result.Add(((XmlElement)xmlImgNode).GetAttribute(strSrc).Replace(strAppdataImg, ""));
			}
			return result;
		}
		static string CheckNameIsFree(string fileName, string localFolderPath) {
			string filePath = Path.Combine(localFolderPath, fileName);
			string filePathNext = Path.Combine(localFolderPath, fileName + "0");
			if(!File.Exists(filePath) && !File.Exists(filePathNext))
				return fileName;
			else {
				string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
				string fileExt = Path.GetExtension(filePath);
				return CheckNameIsFree(String.Format("{0}0{1}", fileNameWithoutExt, fileExt), localFolderPath);
			}
		}
		static string CreateFileName(string fileName, string localFolderPath) {
			int currentYear = DateTime.Now.Year;
			DateTime yearBegin = new DateTime(currentYear, 1, 1);
			DateTime currentDate = DateTime.Now;
			long elapsedTicks = currentDate.Ticks - yearBegin.Ticks;
			string newFileName = String.Format(fileNameMask, fileName, elapsedTicks.ToString());
			return CheckNameIsFree(newFileName, localFolderPath);
		}
		public static void SendBadge(string fileName, string localFolderPath, string badgeString) {
			string badgesFolderPath = Path.Combine(localFolderPath, badgesFolderName);
			string file = Path.Combine(badgesFolderPath, fileName);
			if(!Directory.Exists(badgesFolderPath)) {
				Directory.CreateDirectory(badgesFolderPath);
			}
			if(File.Exists(file)) File.Delete(file);
			StreamWriter outStream;
			outStream = File.CreateText(file);
			outStream.WriteLine(badgeString);
			outStream.Close();
		}
		const string xmlTileNode = "tile";
		const string strQueue = "queue";
		const string strEnabled = "enabled";
		const string strCleartile = "cleartile";
		static XmlDocument GenerateXml(XmlDocument tile, bool enableQueue, bool clearTile) {
			XmlNode tileNode = tile.SelectSingleNode(xmlTileNode);
			XmlNode queueNode = tile.CreateNode(XmlNodeType.Element, strQueue, null);
			XmlAttribute xa = tile.CreateAttribute(strEnabled);
			xa.Value = enableQueue.ToString();
			queueNode.Attributes.Append(xa);
			tileNode.AppendChild(queueNode);
			if(clearTile) {
				XmlNode clearTileNode = tile.CreateNode(XmlNodeType.Element, strCleartile, null);
				tileNode.AppendChild(clearTileNode);
			}
			return tile;
		}
		static void SaveXmlFile(string outputFile, XmlDocument tile) {
			XmlTextWriter xmlWriter = new XmlTextWriter(outputFile, null);
			xmlWriter.Formatting = Formatting.Indented;
			tile.Save(xmlWriter);
			xmlWriter.Close();
		}
		public static string Send(string fileName, string localFolderPath, XmlDocument inputTile, bool enableQueue, bool clearTile) {
			string newFileName = CreateFileName(fileName, localFolderPath);
			string imgFolderPath = Path.Combine(localFolderPath, imgFolderName);
			string outputFile = Path.Combine(localFolderPath, newFileName);
			XmlDocument tile = GenerateXml(inputTile, enableQueue, clearTile);
			SaveXmlFile(outputFile, tile);
			List<string> allFileNames = GetFileNames(Directory.GetFiles(localFolderPath, "*_*"));
			if(allFileNames.Count != 0) {
				try {
					List<string> filesNamesList = allFileNames.Where(f => f.StartsWith(fileName)).ToList();
					filesNamesList.Sort();
					if(filesNamesList.Count > maxTileQueueLenght) {
						for(int i = 0; i < filesNamesList.Count - maxTileQueueLenght; i++) {
							List<string> obsoleteImagesNames = GetImagesNamesFromMessage(filesNamesList[i], localFolderPath);
							if(obsoleteImagesNames != null)
								foreach(string obsoleteImageName in obsoleteImagesNames)
									DeleteFile(obsoleteImageName, imgFolderPath);
							DeleteFile(filesNamesList[i], localFolderPath);
						}
					}
				}
				catch { }
			}
			return newFileName;
		}
		static bool WriteObjectToFile(string filePath, RegisterWinClass regObject) {
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(RegisterWinClass));
				using(Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
					serializer.Serialize(stream, regObject);
				};
				regObject = null;
				return true;
			}
			catch { return false; }
		}
	}
	public interface IWinRTLiveTileManagerDesignerMethods {
		event EventHandler Changed;
		event CancelEventHandler Changing;
		void RaiseEvent(bool isChanging);
	}
}

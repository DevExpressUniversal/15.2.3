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
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.Web.Internal {
	public enum BrowserPlatformType { Unknown, Windows, MacOS, MacOSMobile, AndroidMobile, WindowsPhone }
	public enum BrowserType { Unknown, IE, Netscape, Mozilla, Firefox, Opera, Safari, Chrome, Edge }
	public class BrowserInfoState {
		public readonly string UserAgent;
		public readonly double BrowserVersion;
		public readonly int BrowserMajorVersion;
		public readonly BrowserType BrowserType;
		public readonly BrowserPlatformType BrowserPlatformType;
		public static readonly BrowserInfoState Default;
		public BrowserInfoState(string userAgent, BrowserType browserType, double browserVersion, BrowserPlatformType browserPlatformType) {
			UserAgent = userAgent;
			BrowserType = browserType;
			BrowserVersion = Math.Truncate(10 * browserVersion) / 10;
			BrowserMajorVersion = (int)Math.Truncate(BrowserVersion);
			BrowserPlatformType = browserPlatformType;
		}
		static BrowserInfoState() {
			Default = new BrowserInfoState(null, BrowserInfo.DefaultBrowserType,
				BrowserInfo.DefaultBrowserVersions[BrowserInfo.DefaultBrowserType], BrowserInfo.DefaultBrowserPlatformType);
		}
		public bool IsActual(string userAgent) {
			return string.Compare(UserAgent, userAgent, StringComparison.InvariantCulture) == 0;
		}
	}
	public interface IBrowserPlatformProvider {
		bool IsWindows { get; }
		bool IsMacOS { get; }
		bool IsMacOSMobile { get; }
		bool IsAndroidMobile { get; }
		bool IsWindowsPhone { get; }
		bool IsMSTouch { get; }
	}
	public class BrowserInfo : IBrowserPlatformProvider {
		internal const string									
			IEVersionAccordingToTridentContextKey = "__DXPageIEVersionAccordingToTrident",			
			IEEdgeContextKey = "__DXIEEdge",
			IEVersionContextKey = "__DXIEVersion",
			IECompatibilityModeContextKey = "__DXIECompatibilityMode";
		internal const BrowserType DefaultBrowserType = BrowserType.IE;
		internal const BrowserPlatformType DefaultBrowserPlatformType = BrowserPlatformType.Windows;
		internal static readonly Dictionary<BrowserType, double> DefaultBrowserVersions = new Dictionary<BrowserType, double>();
		private const string OptionalSlashOrWhiteSpace = @"(?:/|\s*)?";
		private const string VersionPattern = @"(?<MAJOR>\d+)(?:\.(?<MINOR>(?:\d+?[1-9])|\d)0*?)?";
		private const string OptionalVersionPattern = @"(?:" + VersionPattern + @")?";
		private const string TridentVersionPattern = @"trident/(?<MAJORTRIDENT>\d+)";
		private const string TridentVersionGroupName = "MAJORTRIDENT";
		private static readonly Dictionary<BrowserType, Regex> UserAgentIdentRegExpressions = new Dictionary<BrowserType, Regex>();
		private static readonly Dictionary<string, BrowserPlatformType> BrowserPlatformTypeIdentStrings = new Dictionary<string, BrowserPlatformType>();
		private string userAgent;
		private BrowserInfoState state = BrowserInfoState.Default;
		private readonly BrowserPlatform browserPlatform;
		private readonly BrowserFamily browserFamily;
		private readonly object syncRoot = new object();
		public BrowserInfo()
			: this(null) {
		}
		public BrowserInfo(string userAgent) {
			this.userAgent = userAgent;
			this.browserPlatform = new BrowserPlatform(this);
			this.browserFamily = new BrowserFamily(this);
			LoadIECompatibiltyVersionFromConfig();
		}
		static BrowserInfo() {
			UserAgentIdentRegExpressions.Add(BrowserType.Safari, CreateRegex(@"applewebkit(?:.*?(?:version/" + VersionPattern + @"[\.\w\d]*?(?:\s+mobile/\S*)?\s+safari))?"));
			UserAgentIdentRegExpressions.Add(BrowserType.Chrome, CreateRegex(@"(?:chrome|crios)(?!frame)" + OptionalSlashOrWhiteSpace + OptionalVersionPattern));
			UserAgentIdentRegExpressions.Add(BrowserType.Mozilla, CreateRegex(@"mozilla(?:.*rv:" + OptionalVersionPattern + @".*Gecko)?"));
			UserAgentIdentRegExpressions.Add(BrowserType.Netscape, CreateRegex(@"(?:netscape|navigator)\d*/?\s*" + OptionalVersionPattern));
			UserAgentIdentRegExpressions.Add(BrowserType.Firefox, CreateRegex(@"firefox" + OptionalSlashOrWhiteSpace + OptionalVersionPattern));
			UserAgentIdentRegExpressions.Add(BrowserType.Opera, CreateRegex(@"opera" + OptionalSlashOrWhiteSpace + OptionalVersionPattern));
			UserAgentIdentRegExpressions.Add(BrowserType.IE, CreateRegex(@"msie\s*" + OptionalVersionPattern));
			UserAgentIdentRegExpressions.Add(BrowserType.Edge, CreateRegex(@"edge" + OptionalSlashOrWhiteSpace + OptionalVersionPattern));
			DefaultBrowserVersions.Add(BrowserType.Safari, 2);
			DefaultBrowserVersions.Add(BrowserType.Chrome, 0.1);
			DefaultBrowserVersions.Add(BrowserType.Mozilla, 1.9);
			DefaultBrowserVersions.Add(BrowserType.Netscape, 8);
			DefaultBrowserVersions.Add(BrowserType.Firefox, 2);
			DefaultBrowserVersions.Add(BrowserType.Opera, 9);
			DefaultBrowserVersions.Add(BrowserType.IE, 8);
			DefaultBrowserVersions.Add(BrowserType.Edge, 12);
			BrowserPlatformTypeIdentStrings.Add("Windows", BrowserPlatformType.Windows);
			BrowserPlatformTypeIdentStrings.Add("Macintosh", BrowserPlatformType.MacOS);
			BrowserPlatformTypeIdentStrings.Add("Mac OS", BrowserPlatformType.MacOS);
			BrowserPlatformTypeIdentStrings.Add("Mac_PowerPC", BrowserPlatformType.MacOS);
			BrowserPlatformTypeIdentStrings.Add("cpu os", BrowserPlatformType.MacOSMobile);
			BrowserPlatformTypeIdentStrings.Add("cpu iphone os", BrowserPlatformType.MacOSMobile);
			BrowserPlatformTypeIdentStrings.Add("android", BrowserPlatformType.AndroidMobile);
			BrowserPlatformTypeIdentStrings.Add("!windows phone", BrowserPlatformType.WindowsPhone);
			BrowserPlatformTypeIdentStrings.Add("!wpdesktop", BrowserPlatformType.WindowsPhone);
			BrowserPlatformTypeIdentStrings.Add("!zunewp", BrowserPlatformType.WindowsPhone);			
		}
		void LoadIECompatibiltyVersionFromConfig() {
			var ieVersionString = ConfigurationSettings.IECompatibilityVersion;
			ConfigHasCompatibilityVersion = !string.IsNullOrEmpty(ieVersionString);
			int version = -1;
			ConfigIsEdge = !int.TryParse(ieVersionString, out version);
			ConfigIEVersion = version;
		}
		protected bool ConfigHasCompatibilityVersion { get; set; }
		protected bool ConfigIsEdge { get; set; }
		protected int ConfigIEVersion { get; set; }
		bool IsIECompatModeEdge {
			get {
				if(HttpContext.Current.Items[IEEdgeContextKey] == null)
					HttpContext.Current.Items[IEEdgeContextKey] = ConfigIsEdge;
				return (bool)HttpContext.Current.Items[IEEdgeContextKey];
			}
			set { HttpContext.Current.Items[IEEdgeContextKey] = value; }
		}
		int IEVersion {
			get {
				if(HttpContext.Current.Items[IEVersionContextKey] == null)
					HttpContext.Current.Items[IEVersionContextKey] = IsIECompatModeEdge ? IEVersionAccordingToTrident : ValidateIEVersion(ConfigIEVersion);
				return (int)HttpContext.Current.Items[IEVersionContextKey];
			}
			set { HttpContext.Current.Items[IEVersionContextKey] = ValidateIEVersion(value); }
		}
		public bool IsIECompatibilityMode {
			get {
				if(!IsIE || HttpContext.Current == null)
					return false;
#if DebugTest
				if(HttpContext.Current.CurrentHandler is DevExpress.Web.Tests.IWebServerTestPage)
					return false;
#endif
				if(IEVersionAccordingToTrident < 0) 
					return false;
				if(HttpContext.Current.Items[IECompatibilityModeContextKey] == null)
					HttpContext.Current.Items[IECompatibilityModeContextKey] = ConfigHasCompatibilityVersion;
				return (bool)HttpContext.Current.Items[IECompatibilityModeContextKey];
			}
			set { HttpContext.Current.Items[IECompatibilityModeContextKey] = value; }
		}
		public string GetIECompatibilityHeader() {
			var ieVersionString = IEVersion.ToString(); 
			return IsIECompatModeEdge ? SettingsConfigurationSection.IECompatibilityVersionEdge : ieVersionString;
		}
		public string UserAgent {
			get {
#if DebugTest
				if(!string.IsNullOrEmpty(userAgent))
					return userAgent;
				else
#endif
				if(HttpContext.Current != null && HttpContext.Current.Request != null)
					return HttpContext.Current.Request.UserAgent;
				else
					return null;
			}
		}
		public bool IsIE { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.IE; } }
		public bool IsFirefox { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Firefox; } }
		public bool IsOpera { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Opera; } }
		public bool IsMozilla { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Mozilla; } }
		public bool IsNetscape { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Netscape; } }
		public bool IsChrome { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Chrome; } }
		public bool IsSafari { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Safari; } }
		public bool IsEdge { get { EnsureStateIsActual(); return state.BrowserType == BrowserType.Edge; } }
		public double Version { 
			get { 
				EnsureStateIsActual();
				if(IsIECompatibilityMode)
					return IEVersion;
				return state.BrowserVersion; 
			} 
		}
		public int MajorVersion { 
			get { 
				EnsureStateIsActual();
				if(IsIECompatibilityMode)
					return IEVersion;
				return state.BrowserMajorVersion; 
			} 
		}
		bool IsIETouch { get { return IsIE && IsUserAgentContains("touch") || state.BrowserPlatformType == BrowserPlatformType.WindowsPhone; } }
		bool IsWindowsPhone { get { return state.BrowserPlatformType == BrowserPlatformType.WindowsPhone || (IsEdge && IsUserAgentContains("arm")); } }
		bool IBrowserPlatformProvider.IsWindows { get { EnsureStateIsActual(); return state.BrowserPlatformType == BrowserPlatformType.Windows && !IsWindowsPhone; } }
		bool IBrowserPlatformProvider.IsMacOS { get { EnsureStateIsActual(); return state.BrowserPlatformType == BrowserPlatformType.MacOS; } }
		bool IBrowserPlatformProvider.IsMacOSMobile { get { EnsureStateIsActual(); return state.BrowserPlatformType == BrowserPlatformType.MacOSMobile; } }
		bool IBrowserPlatformProvider.IsAndroidMobile { get { EnsureStateIsActual(); return state.BrowserPlatformType == BrowserPlatformType.AndroidMobile; } }
		bool IBrowserPlatformProvider.IsMSTouch { get { return IsIETouch || IsEdge; } }
		bool IBrowserPlatformProvider.IsWindowsPhone { get { EnsureStateIsActual(); return IsWindowsPhone; } }
		public BrowserPlatform Platform { get { return browserPlatform; } }
		public BrowserFamily Family { get { return browserFamily; } }
		protected bool IsUserAgentContains(string substring) {
			return !string.IsNullOrEmpty(UserAgent) && UserAgent.ToLower().Contains(substring);
		}
		protected internal int IEVersionAccordingToTrident {
			get {
				if(HttpContext.Current != null && !string.IsNullOrEmpty(UserAgent)) {
					int? version = (int?)HttpContext.Current.Items[IEVersionAccordingToTridentContextKey];
					if(!version.HasValue)
						HttpContext.Current.Items[IEVersionAccordingToTridentContextKey] = version = GetIEVersionAccordingToTriden(UserAgent, state.BrowserPlatformType);
					return version.Value;
				}
				return -1;
			}
		}				
		[Obsolete("This method is now obsolete. Use the ASPxWebControl.SetIECompatibilityMode method instead.")]
		public void RequestIE7CompatibilityMode() {
			ASPxWebControl.SetIECompatibilityMode(7);
		}
#if DebugTest
		public void SetCurrentBrowser(BrowserType browserType) {
			SetCurrentBrowser(browserType, -1);
		}
		public void SetCurrentBrowser(BrowserType browserType, double version) {
			lock(this.syncRoot) {
				if(browserType == BrowserType.Unknown) {
					this.userAgent = null;
					this.state = BrowserInfoState.Default;
				} else {
					string versionStr = version > -1 ? version.ToString(CultureInfo.InvariantCulture) : "X.X";
					if(browserType == BrowserType.IE)
						this.userAgent = string.Format("MSIE {0}", versionStr);
					else if(browserType == BrowserType.Netscape)
						this.userAgent = string.Format("Netscape/{0}", versionStr);
					else if(browserType == BrowserType.Mozilla)
						this.userAgent = string.Format("mozilla rv:{0} Gecko", versionStr);
					else if(browserType == BrowserType.Firefox)
						this.userAgent = string.Format("Firefox/{0}", versionStr);
					else if(browserType == BrowserType.Opera)
						this.userAgent = string.Format("Opera/{0}", versionStr);
					else if(browserType == BrowserType.Safari)
						this.userAgent = string.Format("AppleWebKit Version/{0} Safari", versionStr);
					else if(browserType == BrowserType.Chrome)
						this.userAgent = string.Format("Chrome/{0}", versionStr);
				}
			}
		}
		public void SetCurrentBrowser(string userAgent) {
			lock(this.syncRoot) {
				this.userAgent = userAgent;
				state = BrowserInfoState.Default;
			}
		}
		public void ResetCurrentBrowser() {
			SetCurrentBrowser(BrowserType.Unknown, -1);
		}
#endif
		private void EnsureStateIsActual() {
			lock(syncRoot) {
				string userAgent = UserAgent;
				if(!this.state.IsActual(userAgent))
					this.state = ParseUserAgent(userAgent);
			}
		}
		private static BrowserInfoState ParseUserAgent(string userAgent) {
			if(string.IsNullOrEmpty(userAgent))
				return BrowserInfoState.Default;
			try {
				BrowserType browserType = BrowserType.Unknown;
				double browserVersion = -1;
				BrowserPlatformType browserPlatform = BrowserPlatformType.Unknown;
				BrowserType[] browserTypeCandidatesOrderedList = new BrowserType[] {
					BrowserType.Mozilla,
					BrowserType.IE,
					BrowserType.Firefox, BrowserType.Netscape,
					BrowserType.Safari, BrowserType.Chrome,
					BrowserType.Opera,
					BrowserType.Edge
				};
				foreach(BrowserType browserTypeCandidate in browserTypeCandidatesOrderedList) {
					Regex regex = UserAgentIdentRegExpressions[browserTypeCandidate];
					Match match = regex.Match(userAgent);
					if(match.Success) {
						browserType = browserTypeCandidate;
						browserVersion = -1;
						string major = match.Groups["MAJOR"].Value;
						string minor = match.Groups["MINOR"].Value;
						string versionStr = "";
						if(!string.IsNullOrEmpty(major)) {
							versionStr += major;
							if(!string.IsNullOrEmpty(minor))
								versionStr += "." + minor;
						}
						if(browserTypeCandidate == BrowserType.Opera && versionStr == "9.8") { 
							string realOperaVersionString = userAgent.Substring(userAgent.LastIndexOf("Version/") + 8);
							double realOperaVersion = 0;
							if(double.TryParse(realOperaVersionString, out realOperaVersion))
								versionStr = realOperaVersionString;
						}
						if(!string.IsNullOrEmpty(versionStr))
							double.TryParse(versionStr, NumberStyles.Float, CultureInfo.InvariantCulture, out browserVersion);
						if(browserType == BrowserType.Mozilla && browserVersion >= 11)
							browserType = BrowserType.IE;
					}
				}
				if(browserType == BrowserType.Unknown)
					browserType = DefaultBrowserType;
				if(browserVersion == -1)
					browserVersion = DefaultBrowserVersions[browserType];
				browserPlatform = BrowserPlatformType.Unknown;
				int minOccurenceIndex = int.MaxValue;
				foreach(KeyValuePair<string, BrowserPlatformType> pair in BrowserPlatformTypeIdentStrings) {
					bool importantIdent = pair.Key.StartsWith("!");
					string identStr = importantIdent ? pair.Key.Substring(1) : pair.Key;
					int occurenceIndex = userAgent.IndexOf(identStr, StringComparison.InvariantCultureIgnoreCase);
					if(occurenceIndex >= 0 && (occurenceIndex < minOccurenceIndex || importantIdent)) {
						minOccurenceIndex = importantIdent ? 0 : occurenceIndex;
						browserPlatform = pair.Value;
					}
				}
				if(browserPlatform == BrowserPlatformType.Unknown)
					browserPlatform = DefaultBrowserPlatformType;
				if(browserPlatform == BrowserPlatformType.WindowsPhone && browserVersion < 9)
					browserVersion = GetIEVersionAccordingToTriden(userAgent, browserPlatform);
				return new BrowserInfoState(userAgent, browserType, browserVersion, browserPlatform);
			} catch {
				return BrowserInfoState.Default;
			}
		}
		private static Regex CreateRegex(string pattern) {
			return new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}
		internal static int GetIEVersionAccordingToTriden(string userAgent, BrowserPlatformType platform) {
			Match match = CreateRegex(TridentVersionPattern).Match(userAgent);
			if(match.Success) {
				int tridentVersion = Int32.Parse(match.Groups[TridentVersionGroupName].Value);
				tridentVersion += 4;
				return platform == BrowserPlatformType.WindowsPhone ? Math.Max(9, tridentVersion) : tridentVersion;
			}
			return -1;
		}
		internal void SetIECompatibilityMode(int ieVersion) {
			IsIECompatModeEdge = false;
			IEVersion = ieVersion;			
			IsIECompatibilityMode = true;
		}
		internal void SetIECompatibilityModeEdge() {
			IsIECompatModeEdge = true;
			IEVersion = -1;			
			IsIECompatibilityMode = true;
		}		
		private int ValidateIEVersion(int version) {
			if(version < 7 || version > IEVersionAccordingToTrident)
				version = IEVersionAccordingToTrident;
			if(version == IEVersionAccordingToTrident)
				IsIECompatModeEdge = true;
			return version;
		}
	}
	public class BrowserPlatform {
		private BrowserInfo browserInfo;
		public BrowserPlatform(BrowserInfo browserInfo) {
			this.browserInfo = browserInfo;
		}
		IBrowserPlatformProvider BrowserPlatformProvider {
			get { return browserInfo as IBrowserPlatformProvider; }
		}
		public bool IsWindows {
			get { return BrowserPlatformProvider.IsWindows; }
		}
		public bool IsMacOS {
			get { return BrowserPlatformProvider.IsMacOS; }
		}
		public bool IsMacOSMobile {
			get { return BrowserPlatformProvider.IsMacOSMobile; }
		}
		public bool IsAndroidMobile {
			get { return BrowserPlatformProvider.IsAndroidMobile; }
		}
		public bool IsMSTouchUI {
			get { return BrowserPlatformProvider.IsMSTouch; }
		}
		public bool IsWebKitTouchUI {
			get { return IsMacOSMobile || IsAndroidMobile; }
		}
		public bool IsTouchUI {
			get { return IsMacOSMobile || IsAndroidMobile || IsMSTouchUI; }
		}
		public bool IsMobileUI {
			get { return IsMacOSMobile || IsAndroidMobile || IsWindowsPhone; }
		}
		public bool IsWindowsPhone {
			get { return BrowserPlatformProvider.IsWindowsPhone; }
		}
	}
	public class BrowserFamily {
		private BrowserInfo browserInfo;
		public BrowserFamily(BrowserInfo browserInfo) {
			this.browserInfo = browserInfo;
		}
		public bool IsWebKit {
			get { return Browser.IsChrome || Browser.IsSafari; }
		}
		public bool IsNetscape {
			get { return Browser.IsNetscape || Browser.IsMozilla || Browser.IsFirefox; }
		}
		private BrowserInfo Browser {
			get { return browserInfo; }
		}
	}
}

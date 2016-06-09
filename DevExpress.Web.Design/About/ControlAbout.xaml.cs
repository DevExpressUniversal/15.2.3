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

#if SLDESIGN
extern alias Platform;
using AssemblyInfo = Platform::AssemblyInfo;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.IO;
using System.Reflection;
#if SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf
{
	public partial class ControlAbout : UserControl {
		public ControlAbout(AboutInfo info) {
			DataContext = info;
			this.InitializeComponent();
			Initialize();
		}
		public AboutInfo AboutInfo { get { return DataContext as AboutInfo; } }
		protected void Initialize() {
			LicensedImage.Visibility = System.Windows.Visibility.Collapsed;
			ExpiredImage.Visibility = System.Windows.Visibility.Collapsed;
			TrialImage.Visibility = System.Windows.Visibility.Collapsed;
			TrialDays.Visibility = System.Windows.Visibility.Collapsed; 
			switch(AboutInfo.LicenseState) {
				case LicenseState.Licensed:
					LicensedImage.Visibility = System.Windows.Visibility.Visible;
					TrialSection.Visibility = System.Windows.Visibility.Collapsed;
					LicensedSection.Visibility = System.Windows.Visibility.Visible;
					break;
				case LicenseState.Trial:
					questions.Visibility = System.Windows.Visibility.Collapsed;
					TrialImage.Visibility = System.Windows.Visibility.Visible;
					trialOrLicensed.Text = "an eval version";
					TrialSection.Visibility = System.Windows.Visibility.Visible;
					LicensedSection.Visibility = System.Windows.Visibility.Collapsed;
					TrialDays.Visibility = GetDays() > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
					TrialDaysCount.Text = GetDays().ToString();
					break;
				case LicenseState.TrialExpired:
					questions.Visibility = System.Windows.Visibility.Collapsed;
					TrialSection.Visibility = System.Windows.Visibility.Visible;
					LicensedSection.Visibility = System.Windows.Visibility.Collapsed;
					ExpiredImage.Visibility = System.Windows.Visibility.Visible;
					trialOrLicensed.Text = "an expired version";
					break;
			}
			CopyrightText.Text = String.Format("Copyright © 2000-{0} Developer Express Inc.", DateTime.Now.Year.ToString());
			CloseButton.Click += new RoutedEventHandler(CloseButtonClick);
			HelpButton.Click += new RoutedEventHandler(HelpButtonClick);
			BuyButton.Click += new RoutedEventHandler(BuyButtonClick);
			RegisterButton.Click += new RoutedEventHandler(RegisterButtonClick);
			DiscountsButton.Click += new RoutedEventHandler(DiscountsButtonClick);
#if !SL
			PreviewMouseMove += buttonsPanel_PreviewMouseMove;
#endif
			SupportCenterButton.Click += new RoutedEventHandler(SupportCenterButton_Click);
			ChatButton.Click += new RoutedEventHandler(ChatButton_Click);
		}
		DependencyObject FindZIndexPropertyElement(Button button) {
#if SL
			return null;
#else
			return button.Template.FindName("rootButtonElement", button) as DependencyObject;
#endif
		}
		void buttonsPanel_PreviewMouseMove(object sender, MouseEventArgs e) {
			if(FindZIndexPropertyElement(HelpButton) == null) return;
			if((int)FindZIndexPropertyElement(HelpButton).GetValue(Canvas.ZIndexProperty) > 0) SubscribeButtonLine.Visibility = System.Windows.Visibility.Visible;
			else SubscribeButtonLine.Visibility = System.Windows.Visibility.Collapsed;
			if((int)FindZIndexPropertyElement(BuyButton).GetValue(Canvas.ZIndexProperty) > 0) RegisterButtonLine.Visibility = System.Windows.Visibility.Visible;
			else RegisterButtonLine.Visibility = System.Windows.Visibility.Collapsed;
			if((int)FindZIndexPropertyElement(DiscountsButton).GetValue(Canvas.ZIndexProperty) > 0) DiscountsButtonLine.Visibility = System.Windows.Visibility.Visible;
			else DiscountsButtonLine.Visibility = System.Windows.Visibility.Collapsed;
		}
		int GetDays() {
#if !SL || SLDESIGN
#endif
			return 0;
		}
		void ChatButton_Click(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkChat);
		}
		void SupportCenterButton_Click(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkGetSupport);
		}
		void DiscountsButtonClick(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkCompetitiveDiscounts);
		}
		void BuyButtonClick(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkBuyNow);
		}
		void RegisterButtonClick(object sender, RoutedEventArgs e) {
#if !SL || SLDESIGN
			string setupFilePath = CalcSetupFilePath(AssemblyInfo.InstallationRegistryKey, "SetupFilePath");
			bool showSite = false;
			if(setupFilePath == string.Empty)
				showSite = true;
			else {
				try {
					ProcessStart(setupFilePath, "/Register");
				} catch {
					showSite = true;
				}
			}
			if(showSite)
				Navigate(AssemblyInfo.DXLinkRegisterKB);
#else
			Navigate(AssemblyInfo.DXLinkRegisterKB);
#endif
			CloseButtonClick(null, null);
		}
		void HelpButtonClick(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkGetSupport);
		}
#if !SL || SLDESIGN
		public static void ProcessStart(string name) {
			ProcessStart(name, string.Empty);
		}
		public static void ProcessStart(string name, string arguments) {
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = name;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		string CalcSetupFilePath(string keyPath, string keyName) {
			string ret = GetSetupFilePath(keyPath, keyName);
			if(ret == string.Empty) ret = GetSetupFilePath(keyPath.Replace("SOFTWARE", "SOFTWARE\\Wow6432Node"), keyName);
			return ret;
		}
		string GetSetupFilePath(string keyPath, string keyName) {
			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath);
			if(key != null) {
				string keyValue = string.Format("{0}", key.GetValue(keyName));
				if(System.IO.File.Exists(keyValue)) return keyValue;
			}
			return string.Empty;
		}
#endif
		protected void Navigate(string url) {
#if !SL || SLDESIGN
			try {
				System.Diagnostics.Process.Start(url);
			} catch { }
#else
			DocumentPresenter.OpenTabLink(url, OpenLinkType.Smart);
#endif
			CloseButtonClick(null, null);
		}
		void CloseButtonClick(object sender, RoutedEventArgs e) {
#if !SL || SLDESIGN
			Window parentWindow = Parent as Window;
			if(parentWindow != null)
				parentWindow.Close();
#else
			if(AboutInfo.LicenseState != LicenseState.TrialExpired) ((System.Windows.Controls.Primitives.Popup)((FrameworkElement)Parent).Parent).IsOpen = false;
#endif
		}
	}
	public class AboutInfo : AboutInfoBase {
		public string RegistrationCode { get; set; }
		public string ProductPlatform { get; set; }
	}
	public enum LicenseState { Licensed, Trial, TrialExpired }
	[Serializable]
	public class AboutInfoBase {
		public LicenseState LicenseState { get; set; }
		public string ProductName { get; set; }
		public string ProductKind { get; set; }
		public string Version { get; set; }
	}
	public class EmbeddedResourceAboutImageConverter : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string resourceName = Convert.ToString(value);
#if ASP
			resourceName = resourceName.Replace(
				"DevExpress.Xpf.Core.Core.About.Images",
				"DevExpress.Web.Design.About.Images"); 
#endif
#if ASPTHEMEBUILDER
			resourceName = resourceName.Replace(
				"DevExpress.Xpf.Core.Core.About.Images",
				"ASPxThemeBuilder.About.Images"); 
#endif
#if SLDESIGN
			resourceName = resourceName.Replace(
				"DevExpress.Xpf.Core.Core.About.Images",
				"DevExpress.Xpf.Core.Design.About.Images");
#endif
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
			return CreateImageFromStream(stream);
		}
		public static BitmapImage CreateImageFromStream(Stream stream) {
			BitmapImage bitmapImage = new BitmapImage();
#if !SL || SLDESIGN
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = stream;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
#else
			bitmapImage.SetSource(stream);
#endif
			return bitmapImage;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}

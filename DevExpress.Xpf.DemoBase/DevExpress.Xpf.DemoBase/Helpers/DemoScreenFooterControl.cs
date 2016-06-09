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

using DevExpress.DemoData;
using DevExpress.DemoData.Core;
using DevExpress.DemoData.Helpers;
using DevExpress.DemoData.Model;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.DemoBase.Helpers {
	public class LinkInfo {
		public string Text { get; set; }
		public ICommand OpenCommand { get; set; }
		public Uri Preview { get; set; }
		public Uri PreviewHover { get; set; }
		public Uri CompactPreview { get; set; }
	}
	public class DemoScreenFooterControl : Control {
		public string Version { get; private set; }
		public bool IsRegistered { get; private set; }
		public string Copyright { get; private set; }
		public IEnumerable<LinkInfo> TextLinks { get; private set; }
		public ReadOnlyCollection<LinkInfo> ImageLinks { get; private set; }
		public bool IsCompactMode {
			get { return (bool)GetValue(IsCompactModeProperty); }
			set { SetValue(IsCompactModeProperty, value); }
		}
		public static readonly DependencyProperty IsCompactModeProperty =
			DependencyProperty.Register("IsCompactMode", typeof(bool), typeof(DemoScreenFooterControl), new PropertyMetadata(false));
		public string PlatformName {
			get { return (string)GetValue(PlatformNameProperty); }
			set { SetValue(PlatformNameProperty, value); }
		}
		public static readonly DependencyProperty PlatformNameProperty =
			DependencyProperty.Register("PlatformName", typeof(string), typeof(DemoScreenFooterControl), new PropertyMetadata(string.Empty));
		static DemoScreenFooterControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DemoScreenFooterControl), new FrameworkPropertyMetadata(typeof(DemoScreenFooterControl)));
		}
		string GetVersion() {
			return AssemblyInfo.Version.Remove(AssemblyInfo.Version.LastIndexOf('.'));
		}
		void CreateTextLinks() {
			if(!IsRegistered)
				return;
			TextLinks = new[] {
				CreateTextLinkItem("What's New", DemoDataSettings.WhatsNewLink),
				CreateTextLinkItem("Breaking Changes", DemoDataSettings.BreakingChangesLink),
				CreateTextLinkItem("Known Issues", DemoDataSettings.KnownIssuesLink)
			};
		}
		LinkInfo CreateTextLinkItem(string text, string link) {
			return new LinkInfo {
				Text = text,
				OpenCommand = new DelegateCommand(() => DocumentPresenter.OpenLink(link))
			};
		}
		static Uri CreateUriWithVersion(string path) {
			return new Uri(string.Format(path, AssemblyInfo.VersionShort), UriKind.Absolute);
		}
		void CreateLinks() {
			var list = new List<LinkInfo>();
			list.Add(new LinkInfo {
				Text = "Get Started",
				OpenCommand = new DelegateCommand(() => GetStarted(PlatformName)),
				Preview = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/StartedNormal.png"),
				PreviewHover = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/StartedHover.png"),
				CompactPreview = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/CompactGetStarted.png")
			});
			list.Add(new LinkInfo {
				Text = "Get Support",
				OpenCommand = new DelegateCommand(GetSupport),
				Preview = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/SupportNormal.png"),
				PreviewHover = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/SupportHover.png"),
				CompactPreview = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/CompactGetSupport.png")
			});
			if(!IsRegistered) {
				list.Add(new LinkInfo {
					Text = "Buy Now",
					OpenCommand = new DelegateCommand(BuyNow),
					Preview = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/BuyNormal.png"),
					PreviewHover = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/BuyHover.png"),
					CompactPreview = CreateUriWithVersion("pack://application:,,,/DevExpress.Xpf.DemoBase.v{0};component/Images/CompactBuynow.png")
				});
			}
			ImageLinks = list.AsReadOnly();
		}
		public static void BuyNow() {
			DocumentPresenter.OpenLink(DemoDataSettings.SubscriptionsBuyLink);
		}
		public static void GetSupport() {
			DocumentPresenter.OpenLink(DemoDataSettings.GetSupportLink);
		}
		public static string GetStartedLink(string platformName) {
			var platform = Repository.Platforms.FirstOrDefault(p => p.Name.ToLower() == platformName.ToLower());
			return platform == null ? DemoDataSettings.GetStartedLink : platform.GetStartedLink;
		}
		public static void GetStarted(string platformName) {
			DocumentPresenter.OpenLink(GetStartedLink(platformName));
		}
		public DemoScreenFooterControl() {
			Version = "VERSION " + AssemblyInfo.Version;
			Copyright = AssemblyInfo.AssemblyCopyright.Replace("(c)", "©");
			IsRegistered = Linker.IsRegistered;
			CreateLinks();
			CreateTextLinks();
		}
	}
}

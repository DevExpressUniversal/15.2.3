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
#if SL
using DevExpress.Xpf.Core;
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
namespace DevExpress.Xpf
{
	public partial class ToolAbout : UserControl {
		public ToolAbout(AboutToolInfo info) {
			DataContext = info;
			this.InitializeComponent();
			Initialize();
		}
		protected void Initialize() {
			LicensedImage.Visibility = System.Windows.Visibility.Collapsed;
			ExpiredImage.Visibility = System.Windows.Visibility.Collapsed;
			TrialImage.Visibility = System.Windows.Visibility.Collapsed; switch(AboutInfo.LicenseState) {
				case LicenseState.Licensed:
					LicensedImage.Visibility = System.Windows.Visibility.Visible;
					break;
				case LicenseState.Trial:
					TrialImage.Visibility = System.Windows.Visibility.Visible;
					break;
				case LicenseState.TrialExpired:
					ExpiredImage.Visibility = System.Windows.Visibility.Visible;
					break;
			}
			CopyrightText.Text = String.Format("Copyright © 2000-{0} Developer Express Inc.", DateTime.Now.Year.ToString());
			CloseButton.Click += new RoutedEventHandler(CloseButton_Click);
			SupportCenterButton.Click += new RoutedEventHandler(SupportCenterButton_Click);
			ChatButton.Click += new RoutedEventHandler(ChatButton_Click);
			ProductEULAButton.Click += new RoutedEventHandler(ProductEULAButton_Click);
		}
		public AboutToolInfo AboutInfo { get { return DataContext as AboutToolInfo; } }
		void ProductEULAButton_Click(object sender, RoutedEventArgs e) {
			Navigate(AboutInfo.ProductEULAUri);
		}
		void ChatButton_Click(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkChat);
		}
		void SupportCenterButton_Click(object sender, RoutedEventArgs e) {
			Navigate(AssemblyInfo.DXLinkGetSupport);
		}
		protected void Navigate(string url) {
#if !SL || SLDESIGN
			System.Diagnostics.Process.Start(url);
#else
			DocumentPresenter.OpenTabLink(url, OpenLinkType.Smart);
#endif
		}
		void CloseButton_Click(object sender, RoutedEventArgs e) {
#if !SL || SLDESIGN
			((Window)Parent).Close();
#else
			if(AboutInfo.LicenseState != LicenseState.TrialExpired) ((System.Windows.Controls.Primitives.Popup)((FrameworkElement)Parent).Parent).IsOpen = false;
#endif
		}
	}
	public class AboutToolInfo : AboutInfoBase {
		public string ProductDescriptionLine1 { get; set; }
		public string ProductDescriptionLine2 { get; set; }
		public string ProductDescriptionLine3 { get; set; }
		public string ProductEULA { get; set; }
		public string ProductEULAUri { get; set; }
	}
}

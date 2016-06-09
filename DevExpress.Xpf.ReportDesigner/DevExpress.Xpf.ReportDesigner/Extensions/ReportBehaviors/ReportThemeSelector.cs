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

using DevExpress.Mvvm;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	class ThemeInfo {
		public OfficeTheme Theme { get; set; }
		public OfficeThemesConverter.Shades Shades { get; set; }
	}
	class ReportThemeSelector : Control {
		public ThemeInfo SelectedTheme {
			get { return (ThemeInfo)GetValue(SelectedThemeProperty); }
			set { SetValue(SelectedThemeProperty, value); }
		}
		public static readonly DependencyProperty SelectedThemeProperty =
			DependencyProperty.Register("SelectedTheme", typeof(ThemeInfo), typeof(ReportThemeSelector), new PropertyMetadata(null,
				(d, e) => ((ReportThemeSelector)d).SelectedThemeChanged()));
		void SelectedThemeChanged() {
		}
		static ReportThemeSelector() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportThemeSelector), new FrameworkPropertyMetadata(typeof(ReportThemeSelector)));
		}
	}
	public class ReportWizardPageTemplateSelector : DataTemplateSelector {
		public DataTemplate ThemePageTemplate { get; set; }
		public DataTemplate PageSetupPageTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if(item is ThemesViewModel)
				return ThemePageTemplate;
			return PageSetupPageTemplate;
		}
	}
}

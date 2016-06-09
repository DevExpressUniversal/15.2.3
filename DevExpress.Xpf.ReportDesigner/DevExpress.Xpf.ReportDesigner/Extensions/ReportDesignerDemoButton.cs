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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Printing.PreviewControl.Bars;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Images;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	public abstract class ReportDesignerRibbonDemoButtonHelper {
		static bool loaded = false;
		public static bool Load() {
			loaded = true;
			return loaded;
		}
		public static readonly DependencyProperty CenterVerticallyInProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty OnSizeChangedHandlerProperty;
		static ReportDesignerRibbonDemoButtonHelper() {
			AssemblyInitializer.Init();
			DependencyPropertyRegistrator<ReportDesignerRibbonDemoButtonHelper>.New()
				.RegisterAttached((FrameworkElement d) => GetOnSizeChangedHandler(d), out OnSizeChangedHandlerProperty, null)
				.RegisterAttached((FrameworkElement d) => GetCenterVerticallyIn(d), out CenterVerticallyInProperty, null, OnCenterVerticallyInChanged)
			;
		}
		public static Panel GetCenterVerticallyIn(FrameworkElement button) { return (Panel)button.GetValue(CenterVerticallyInProperty); }
		public static void SetCenterVerticallyIn(FrameworkElement button, Panel panel) { button.SetValue(CenterVerticallyInProperty, panel); }
		static SizeChangedEventHandler GetOnSizeChangedHandler(FrameworkElement button) {
			var handler = (SizeChangedEventHandler)button.GetValue(OnSizeChangedHandlerProperty);
			if(handler == null) {
				handler = (s, e) => UpdateButtonPosition(button);
				button.SetValue(OnSizeChangedHandlerProperty, handler);
			}
			return handler;
		}
		static void OnCenterVerticallyInChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var button = (FrameworkElement)d;
			var onSizeChanged = GetOnSizeChangedHandler(button);
			var oldPanel = (Panel)e.OldValue;
			var newPanel = (Panel)e.NewValue;
			if(oldPanel != null) {
				button.SizeChanged -= onSizeChanged;
				oldPanel.SizeChanged -= onSizeChanged;
			}
			if(newPanel != null) {
				newPanel.SizeChanged += onSizeChanged;
				button.SizeChanged += onSizeChanged;
				UpdateButtonPosition(button);
			}
		}
		static void UpdateButtonPosition(FrameworkElement button) {
			double top;
			var ribbonPanel = GetCenterVerticallyIn(button);
			if(ribbonPanel == null) {
				top = 0.0;
			} else {
				FrameworkElement groupElement;
				string themeName = ThemeManager.GetTreeWalker(button).With(x => x.ThemeName);
				if(themeName == null || SpecialThemes.Where(x => themeName.StartsWith(x, StringComparison.Ordinal)).Any())
					groupElement = ribbonPanel;
				else
					groupElement = LayoutTreeHelper.GetVisualParents(ribbonPanel).OfType<RibbonPageGroupControl>().First();
				var parent = (FrameworkElement)VisualTreeHelper.GetParent(button);
				var parentMargin = parent.TransformToVisual(groupElement).Transform(new Point()).Y;
				top = Math.Floor((groupElement.ActualHeight - button.ActualHeight) / 2.0) - parentMargin;
			}
			button.Margin = new Thickness(button.Margin.Left, top, button.Margin.Right, 0.0);
		}
		static readonly string[] SpecialThemes = new string[] {
			Theme.LightGrayName,
			AssemblyHelper.GetCommonPart(new string[] { Theme.MetropolisLightName, Theme.MetropolisDarkName }, new string[] { }),
			AssemblyHelper.GetCommonPart(new string[] { Theme.Office2007BlackName, Theme.Office2007SilverName }, new string[] { }),
			Theme.TouchlineDarkName
		};
	}
}

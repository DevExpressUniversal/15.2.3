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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using System;
using System.Windows;
namespace DevExpress.Xpf.Bars {
	public enum BarSubItemThemeSelectorShowMode {
		List,
		GroupedInSubMenus,
		GroupedList
	}
	[DevExpress.Mvvm.UI.Interactivity.TargetTypeAttribute(typeof(BarSubItem))]
	public class BarSubItemThemeSelectorBehavior : BarItemThemeSelectorBehavior<BarSubItem> {
		public static readonly DependencyProperty ShowModeProperty;
		static BarSubItemThemeSelectorBehavior() {
			ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(BarSubItemThemeSelectorShowMode), typeof(BarSubItemThemeSelectorBehavior), new PropertyMetadata(BarSubItemThemeSelectorShowMode.List, OnShowModePropertyChanged));
		}
		private static void OnShowModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarSubItemThemeSelectorBehavior)d).OnShowModeChanged((BarSubItemThemeSelectorShowMode)e.OldValue);
		}
		public BarSubItemThemeSelectorShowMode ShowMode {
			get { return (BarSubItemThemeSelectorShowMode)GetValue(ShowModeProperty); }
			set { SetValue(ShowModeProperty, value); }
		}
		public BarSubItemThemeSelectorBehavior() { }
		protected override object CreateStyleKey() {
			switch(ShowMode) {
				case BarSubItemThemeSelectorShowMode.List:
					return new BarSubItemThemeSelectorThemeKeyExtension() { ResourceKey = BarSubItemThemeSelectorThemeKeys.Style, IsThemeIndependent = true };
				case BarSubItemThemeSelectorShowMode.GroupedInSubMenus:
				   return new BarSubItemThemeSelectorThemeKeyExtension() { ResourceKey = BarSubItemThemeSelectorThemeKeys.StyleGroupedInSubMenus, IsThemeIndependent = true };
				case BarSubItemThemeSelectorShowMode.GroupedList:
				   return new BarSubItemThemeSelectorThemeKeyExtension() { ResourceKey = BarSubItemThemeSelectorThemeKeys.StyleGroupedList, IsThemeIndependent = true };
				default:
					throw new NotSupportedException(string.Format("{0} view type is not supported", ShowMode));
			}
		}
		protected virtual void OnShowModeChanged(BarSubItemThemeSelectorShowMode oldValue) {
			StyleKey = CreateStyleKey();
			UpdateAssociatedObjectResourceReference();
		}
	}
}

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars.Customization {
	public partial class OptionsControl : UserControl {
		BarManagerCustomizationHelper helper;
		public OptionsControl(BarManagerCustomizationHelper helper) {
			InitializeComponent();
			this.helper = helper;
			Loaded += new RoutedEventHandler(OnLoaded);
			ceLargeIconsInToolbars.Loaded += new RoutedEventHandler(OnCELargeIconsInToolbarsLoaded);
			ceLargeIconsInToolbars.EditValueChanged += new EditValueChangedEventHandler(OnCELargeIconsInToolbarsEditValueChanged);
			ceLargeIconsInMenu.Loaded += new RoutedEventHandler(OnCELargeIconsInMenuLoaded);
			ceLargeIconsInMenu.EditValueChanged += new EditValueChangedEventHandler(OnCELargeIconsInMenuEditValueChanged);
			ceShowScreenTipsOnToolbars.Loaded += new RoutedEventHandler(OnCEShowScreenTipsOnToolbarsLoaded);
			ceShowScreenTipsOnToolbars.EditValueChanged += new EditValueChangedEventHandler(OnCEShowScreenTipsOnToolbarsEditValueChanged);
			ceShowShortcutKeysOnScreenTips.Loaded += new RoutedEventHandler(OnCEShowShortcutKeysOnScreenTipsLoaded);
			ceShowShortcutKeysOnScreenTips.EditValueChanged += new EditValueChangedEventHandler(OnCEShowShortcutKeysOnScreenTipsEditValueChanged);
		}		
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			OnCELargeIconsInToolbarsLoaded(ceLargeIconsInToolbars, null);
			OnCELargeIconsInMenuLoaded(ceLargeIconsInMenu, null);
			OnCEShowScreenTipsOnToolbarsLoaded(ceShowScreenTipsOnToolbars, null);
			OnCEShowShortcutKeysOnScreenTipsLoaded(ceShowShortcutKeysOnScreenTips, null);
		}
		protected virtual void OnCELargeIconsInToolbarsLoaded(object sender, RoutedEventArgs e) {
			if (helper == null)
				return;
			if (helper.ToolbarGlyphSize == GlyphSize.Default)
				((CheckEdit)sender).IsChecked = null;
			else
				((CheckEdit)sender).IsChecked = helper.ToolbarGlyphSize == GlyphSize.Large;
		}
		protected virtual void OnCELargeIconsInToolbarsEditValueChanged(object sender, EditValueChangedEventArgs e) {
			if (helper == null)
				return;
			if (((CheckEdit)sender).IsChecked == null)
				helper.ToolbarGlyphSize = GlyphSize.Default;
			else
				helper.ToolbarGlyphSize = (bool)((CheckEdit)sender).IsChecked ? GlyphSize.Large : GlyphSize.Small;
		}
		protected virtual void OnCELargeIconsInMenuLoaded(object sender, RoutedEventArgs e) {
			if (helper == null)
				return;
			if (helper.MenuGlyphSize == GlyphSize.Default)
				((CheckEdit)sender).IsChecked = null;
			else
				((CheckEdit)sender).IsChecked = helper.MenuGlyphSize == GlyphSize.Large;
		}
		protected virtual void OnCELargeIconsInMenuEditValueChanged(object sender, EditValueChangedEventArgs e) {
			if (helper == null)
				return;
			if (((CheckEdit)sender).IsChecked == null)
				helper.MenuGlyphSize = GlyphSize.Default;
			else
				helper.MenuGlyphSize = (bool)((CheckEdit)sender).IsChecked ? GlyphSize.Large : GlyphSize.Small;
		}
		protected virtual void OnCEShowScreenTipsOnToolbarsLoaded(object sender, RoutedEventArgs e) {
			if (helper == null)
				return;
			((CheckEdit)sender).IsChecked = helper.ShowScreenTips;
		}
		protected virtual void OnCEShowScreenTipsOnToolbarsEditValueChanged(object sender, EditValueChangedEventArgs e) {
			if (helper == null)
				return;
			helper.ShowScreenTips = (bool)((CheckEdit)sender).IsChecked;
			OnCEShowShortcutKeysOnScreenTipsLoaded(ceShowShortcutKeysOnScreenTips, null);
		}
		protected virtual void OnCEShowShortcutKeysOnScreenTipsLoaded(object sender, RoutedEventArgs e) {
			if (helper == null)
				return;
			((CheckEdit)sender).IsEnabled = (bool)ceShowScreenTipsOnToolbars.IsChecked;
			((CheckEdit)sender).IsChecked = helper.ShowShortcutInScreenTips;
		}
		protected virtual void OnCEShowShortcutKeysOnScreenTipsEditValueChanged(object sender, EditValueChangedEventArgs e) {
			if (helper == null)
				return;
			helper.ShowShortcutInScreenTips = (bool)((CheckEdit)sender).IsChecked;
		}
	}
}

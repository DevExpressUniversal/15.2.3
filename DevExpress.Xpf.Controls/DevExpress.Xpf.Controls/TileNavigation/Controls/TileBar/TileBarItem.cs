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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation {
	[ContentProperty("Content")]
	public class TileBarItem : TileSelectorItem, IContentSelectorItem, IFlyoutEventListener {
		#region static
		public static readonly DependencyProperty FlyoutContentProperty;
		public static readonly DependencyProperty FlyoutContentTemplateProperty;
		public static readonly DependencyProperty FlyoutContentTemplateSelectorProperty;
		static TileBarItem() {
			Type ownerType = typeof(TileBarItem);
			var dProp = new DependencyPropertyRegistrator<TileBarItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("FlyoutContent", ref FlyoutContentProperty, (object)null, OnFlyoutPropertyChanged);
			dProp.Register("FlyoutContentTemplate", ref FlyoutContentTemplateProperty, (DataTemplate)null, OnFlyoutPropertyChanged);
			dProp.Register("FlyoutContentTemplateSelector", ref FlyoutContentTemplateSelectorProperty, (DataTemplateSelector)null, OnFlyoutPropertyChanged);
		}
		private static void OnFlyoutPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			TileBarItem tileBarItem = o as TileBarItem;
			if(tileBarItem != null)
				tileBarItem.OnFlyoutPropertyChanged();
		}
		#endregion
		protected virtual void OnFlyoutPropertyChanged() {
			CoerceValue(IsFlyoutButtonVisibleProperty);
		}
		protected override object OnCoerceIsFlyoutButtonVisible(bool value) {
			return ShowFlyoutButton && (FlyoutContent != null || FlyoutContentTemplate != null || FlyoutContentTemplateSelector != null);
		}
		protected override UIElement GetFlyoutContainer() {
			var container = new TileNavPaneContentControl();
			container.SetBinding(TileNavPaneContentControl.ContentProperty, new Binding("FlyoutContent") { Source = this });
			container.SetBinding(TileNavPaneContentControl.ContentTemplateProperty, new Binding("FlyoutContentTemplate") { Source = this });
			container.SetBinding(TileNavPaneContentControl.ContentTemplateSelectorProperty, new Binding("FlyoutContentTemplateSelector") { Source = this });
			return container;
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			SelectIfPossible();
		}
		public object FlyoutContent {
			get { return (object)GetValue(FlyoutContentProperty); }
			set { SetValue(FlyoutContentProperty, value); }
		}
		public DataTemplate FlyoutContentTemplate {
			get { return (DataTemplate)GetValue(FlyoutContentTemplateProperty); }
			set { SetValue(FlyoutContentTemplateProperty, value); }
		}
		public DataTemplateSelector FlyoutContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(FlyoutContentTemplateSelectorProperty); }
			set { SetValue(FlyoutContentTemplateSelectorProperty, value); }
		}
	}
}

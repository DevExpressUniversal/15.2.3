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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI {
	public class AppBarSeparator : Control, IAppBarElement {
		#region static
		public static readonly DependencyProperty IsCompactProperty;
		static AppBarSeparator() {
			var dProp = new DependencyPropertyRegistrator<AppBarSeparator>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("IsCompact", ref IsCompactProperty, false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				(d, e) => ((AppBarSeparator)d).OnIsCompactChanged((bool)e.NewValue));
			HorizontalAlignmentProperty.AddOwner(typeof(AppBarSeparator), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, OnHorizontalAlignmentChanged));
		}
		private static void OnHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AppBarSeparator separator = d as AppBarSeparator;
			if(separator != null) separator.OnHorizontalAlignmentChanged((HorizontalAlignment)e.OldValue, (HorizontalAlignment)e.NewValue);
		}
		#endregion
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public AppBarSeparator() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(AppBarSeparator);
#endif
		}
		protected virtual void OnIsCompactChanged(bool newValue) {
			Dispatcher.BeginInvoke(new Action(() =>
			{
				VisualStateManager.GoToState(this, newValue ? "Compact" : "FullSize", false);
			}));
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			VisualStateManager.GoToState(this, IsCompact ? "Compact" : "FullSize", false);
		}
		protected virtual void OnHorizontalAlignmentChanged(HorizontalAlignment oldValue, HorizontalAlignment newValue) {
			AppBar bar = DevExpress.Xpf.Core.Native.LayoutHelper.FindParentObject<AppBar>(this);
			if(bar != null) bar.Invalidate();
		}
	}
}

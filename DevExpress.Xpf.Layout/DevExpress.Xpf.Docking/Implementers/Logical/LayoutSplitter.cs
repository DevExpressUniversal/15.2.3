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

using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public class LayoutSplitter : FixedItem {
		#region static
		internal static int LayoutSplitterHeight = 8;
		internal static int LayoutSplitterWidth = 8;
		public static readonly DependencyProperty OrientationProperty;
		internal static readonly DependencyPropertyKey OrientationPropertyKey;
		public static new readonly DependencyProperty IsEnabledProperty;
		static LayoutSplitter() {
			var dProp = new DependencyPropertyRegistrator<LayoutSplitter>();
			dProp.OverrideMetadata(ItemWidthProperty, new GridLength());
			dProp.OverrideMetadata(ItemHeightProperty, new GridLength());
			dProp.RegisterReadonly("Orientation", ref OrientationPropertyKey, ref OrientationProperty, Orientation.Horizontal, null,
			(dObj, value) => ((LayoutSplitter)dObj).CoerceOrientation((Orientation)value));
			dProp.Register("IsEnabled", ref IsEnabledProperty, true, null,
				(dObj, v) => ((LayoutSplitter)dObj).CoerceIsEnabled((bool)v));
		}
		#endregion
		public LayoutSplitter() {
		}
		protected override Layout.Core.LayoutItemType GetLayoutItemTypeCore() {
			return Layout.Core.LayoutItemType.LayoutSplitter;
		}
		protected override Size CalcMaxSizeValue(Size value) {
			if(Parent != null && Parent.Orientation == System.Windows.Controls.Orientation.Horizontal) {
				return new Size(LayoutSplitterWidth, value.Height);
			}
			else {
				return new Size(value.Width, LayoutSplitterHeight);
			}
		}
		protected override void OnParentChanged() {
			base.OnParentChanged();
			CoerceValue(ActualMaxSizeProperty);
			CoerceValue(OrientationProperty);
		}
		protected virtual bool CoerceIsEnabled(bool value) {
			return (Manager != null && Manager.IsCustomization) ? false : value;
		}
		protected virtual object CoerceOrientation(Orientation value) {
			return Parent != null ? Parent.Orientation : value;
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			internal set { SetValue(OrientationPropertyKey, value); }
		}
		public new bool IsEnabled {
			get { return ((bool)base.GetValue(IsEnabledProperty)); }
			set { base.SetValue(IsEnabledProperty, value); }
		}
	}
}

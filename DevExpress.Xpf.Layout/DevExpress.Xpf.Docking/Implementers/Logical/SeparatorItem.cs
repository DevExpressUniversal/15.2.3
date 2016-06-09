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
using DevExpress.Xpf.Layout.Core;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public class SeparatorItem : FixedItem {
		#region static
		internal static int SeparatorHeight = 8;
		internal static int SeparatorWidth = 8;
		public static readonly DependencyProperty OrientationProperty;
		internal static readonly DependencyPropertyKey OrientationPropertyKey;
		static SeparatorItem() {
			var dProp = new DependencyPropertyRegistrator<SeparatorItem>();
			dProp.OverrideMetadata(ItemWidthProperty, new GridLength());
			dProp.OverrideMetadata(ItemHeightProperty, new GridLength());
			dProp.RegisterReadonly("Orientation", ref OrientationPropertyKey, ref OrientationProperty, Orientation.Horizontal, null,
				(dObj, value) => ((SeparatorItem)dObj).CoerceOrientation((Orientation)value));
		}
		#endregion
		public SeparatorItem() {
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.Separator;
		}
		protected virtual object CoerceOrientation(Orientation value) {
			return Parent != null ? Parent.Orientation : value;
		}
		protected override void OnParentChanged() {
			base.OnParentChanged();
			CoerceValue(OrientationProperty);
		}
		protected override Size CalcMaxSizeValue(Size value) {
			if(Parent != null && Parent.Orientation == System.Windows.Controls.Orientation.Horizontal) {
				return new Size(SeparatorWidth, value.Height);
			}
			else {
				return new Size(value.Width, SeparatorHeight);
			}
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			internal set { SetValue(OrientationPropertyKey, value); }
		}
	}
}

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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class AxisLabelItem  : ChartNonVisualElement {
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty AngleProperty = DependencyPropertyManager.Register("Angle",
			typeof(double), typeof(AxisLabelItem), new PropertyMetadata(0.0));
		internal double Angle {
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}
		readonly WeakReference label;
		readonly AxisTextItem textItem;
		readonly CustomAxisLabel customAxisLabel;
		readonly int gridIndex;
		bool visible;
		bool isOutOfRange;
		bool staggered;
		Axis2DLabelItemLayout layout;
		Size size = Size.Empty;
		public AxisLabel Label { get { return label.Target as AxisLabel; } }
		public object Content { get { return textItem.Content; } }
		internal double Value { get { return textItem.Value; } }
		internal CustomAxisLabel CustomAxisLabel { get { return customAxisLabel; } }
		internal bool IsCustomLabel { get { return textItem.IsCustomLabel; } }
		internal bool Staggered {
			get { return staggered; }
			set { staggered = value; }
		}
		internal bool Visible {
			get { return visible; } 
			set { visible = value; }
		}
		internal bool IsOutOfRange {
			get { return isOutOfRange; }
			set { isOutOfRange = value; }
		}
		internal Axis2DLabelItemLayout Layout {
			get { return layout; }
			set { layout = value; }
		}
		internal Size Size { 
			get { return size; } 
			set { size = value; } 
		}
		internal int GridIndex {
			get { return gridIndex; }
		}
		internal AxisLabelItem(AxisLabel label, AxisTextItem textItem, bool staggered) {
			this.label = new WeakReference(label);
			this.textItem = textItem;
			this.gridIndex = textItem.GridIndex;
			visible = textItem.Visible;
			IsOutOfRange = !visible;
			customAxisLabel = textItem.CustomAxisLabel as CustomAxisLabel;
		}
		internal void AssignLayout(IAxisLabelLayout itemLayout) {
			Angle = itemLayout.Angle;
			visible = itemLayout.Visible && !isOutOfRange;
			staggered = itemLayout.Offset != new GRealPoint2D();
			layout = visible ? (Axis2DLabelItemLayout)itemLayout : null;
		}
	}
}

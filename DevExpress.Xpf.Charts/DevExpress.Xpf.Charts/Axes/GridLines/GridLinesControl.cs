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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[
	TemplatePart(Name = "PART_MajorItems", Type = typeof(ItemsControl)),
	TemplatePart(Name = "PART_MinorItems", Type = typeof(ItemsControl)),
	NonCategorized
	]
	public class GridLinesControl : ChartElementBase, IAxis2DElement {
		public static readonly DependencyProperty MajorItemsProperty = DependencyPropertyManager.Register("MajorItems", typeof(ObservableCollection<GridLinesItem>), typeof(GridLinesControl));
		public static readonly DependencyProperty MinorItemsProperty = DependencyPropertyManager.Register("MinorItems", typeof(ObservableCollection<GridLinesItem>), typeof(GridLinesControl));
		readonly AxisBase axis;
		[NonTestableProperty]
		public ObservableCollection<GridLinesItem> MajorItems {
			get { return (ObservableCollection<GridLinesItem>)GetValue(MajorItemsProperty); }
			set { SetValue(MajorItemsProperty, value); }
		}
		[NonTestableProperty]
		public ObservableCollection<GridLinesItem> MinorItems {
			get { return (ObservableCollection<GridLinesItem>)GetValue(MinorItemsProperty); }
			set { SetValue(MinorItemsProperty, value); }
		}
		internal AxisBase Axis { get { return axis; } }
		#region IAxis2DElement implementation
		AxisBase IAxis2DElement.Axis { get { return axis; } }
		bool IAxis2DElement.Visible { get { return true; } }
		#endregion
		public GridLinesControl(AxisBase axis) {
			DefaultStyleKey = typeof(GridLinesControl);
			this.axis = axis;
		}
		ObservableCollection<GridLinesItem> CreateItems(int count) {
			ObservableCollection<GridLinesItem> items = new ObservableCollection<GridLinesItem>();
			for (int i = 0; i < count; i++)
				items.Add(new GridLinesItem(axis));
			return items;
		}
		void UpdateItems(ObservableCollection<GridLinesItem> items, Rect viewport, IAxisMapping mapping, List<double> values, double itemsLength, LineStyle lineStyle) {
			int thickness = lineStyle.Thickness;
			DoubleCollection dashArray;
			double dashOffset;
			DashStyle dashStyle = lineStyle.DashStyle;
			if (dashStyle == null) {
				dashArray = null;
				dashOffset = 0;
			}
			else {
				dashArray = dashStyle.Dashes;
				dashOffset = dashStyle.Offset;
			}
			for (int i = 0; i < items.Count; i++) {
				GridLinesItem item = items[i];
				item.Geometry = axis.CreateGridLineGeometry(viewport, mapping, values[i], thickness);
				item.DashArray = CommonUtils.CloneDoubleCollection(dashArray);
				item.DashOffset = dashOffset;
			}
		}
		internal void UpdateItems(AxisGridDataEx gridData, Rect viewport) {
			IAxisMapping axisMapping = axis.CreateMapping(viewport);
			double itemsLength = axis.IsVertical ? viewport.Width : viewport.Height;
			List<double> majorValues = axis.GridLinesVisible && gridData != null ? gridData.Items.VisibleValues : new List<double>();
			int majorValuesCount = majorValues.Count;
			ObservableCollection<GridLinesItem> majorItems = MajorItems;
			if (majorItems == null || majorItems.Count != majorValuesCount) {
				majorItems = CreateItems(majorValuesCount);
				MajorItems = majorItems;
			}
			UpdateItems(majorItems, viewport, axisMapping, majorValues, itemsLength, axis.ActualGridLinesLineStyle);
			List<double> minorValues = axis.GridLinesMinorVisible && gridData != null ? gridData.MinorValues : new List<double>();
			int minorValuesCount = minorValues.Count;
			ObservableCollection<GridLinesItem> minorItems = MinorItems;
			if (minorItems == null || minorItems.Count != minorValuesCount) {
				minorItems = CreateItems(minorValuesCount);
				MinorItems = minorItems;
			}
			UpdateItems(minorItems, viewport, axisMapping, minorValues, itemsLength, axis.ActualGridLinesMinorLineStyle);
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size constraint = new Size(MathUtils.ConvertInfinityToDefault(availableSize.Width, 0), MathUtils.ConvertInfinityToDefault(availableSize.Height, 0));
			base.MeasureOverride(constraint);
			return constraint;
		}
	} 
}

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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class CircularDiagram2DItemsControl : Diagram2DItemsControl {
		public static readonly DependencyProperty DiagramProperty = DependencyPropertyManager.Register("Diagram",
			typeof(CircularDiagram2D), typeof(CircularDiagram2DItemsControl));
		public static readonly DependencyProperty AxisItemsProperty = DependencyPropertyManager.Register("AxisItems",
		   typeof(ObservableCollection<object>), typeof(CircularDiagram2DItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty DiagramContentProperty = DependencyPropertyManager.Register("DiagramContent",
			typeof(UIElement), typeof(CircularDiagram2DItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SeriesLabelItemsProperty = DependencyPropertyManager.Register("SeriesLabelItems",
			typeof(ObservableCollection<SeriesLabelItem>), typeof(CircularDiagram2DItemsControl), new PropertyMetadata(PropertyChanged));
		public CircularDiagram2D Diagram {
			get { return (CircularDiagram2D)GetValue(DiagramProperty); }
			set { SetValue(DiagramProperty, value); }
		}
		public UIElement DiagramContent {
			get { return (UIElement)GetValue(DiagramContentProperty); }
			set { SetValue(DiagramContentProperty, value); }
		}
		public ObservableCollection<object> AxisItems {
			get { return (ObservableCollection<object>)GetValue(AxisItemsProperty); }
			set { SetValue(AxisItemsProperty, value); }
		}		
		[
		Category(Categories.Common),
		NonTestableProperty
		]
		public ObservableCollection<SeriesLabelItem> SeriesLabelItems {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(SeriesLabelItemsProperty); }
			set { SetValue(SeriesLabelItemsProperty, value); }
		}
		protected override bool ShouldCreateItemPresentation(object item) {
			return (item is Axis2DItem) || (item is AxisLabelItem) || base.ShouldCreateItemPresentation(item);
		}
		protected override ILayoutElement GetItemPresentation(object item) {
			Axis2DItem axisItem = item as Axis2DItem;
			if (axisItem != null && !(axisItem.Axis is CircularAxisX2D))
				return new Axis2DPresentation(axisItem);
			AxisLabelItem axisLabelItem = item as AxisLabelItem;
			if (axisLabelItem != null)
				return new AxisLabelPresentation(axisLabelItem);
			SelectionGeometryItem selectionGeometryItem = item as SelectionGeometryItem;
			if (selectionGeometryItem != null)
				return new SelectionPresentation(selectionGeometryItem);
			return base.GetItemPresentation(item);
		}
		protected override ObservableCollection<object> CreateItems() {
			ObservableCollection<object> items = new ObservableCollection<object>();
			if (DiagramContent != null)
				items.Add(DiagramContent);
			if (AxisItems != null)
				 foreach (object item in AxisItems)
					items.Add(item);
			if (SeriesLabelItems != null) {
				foreach (SeriesLabelItem labelItem in SeriesLabelItems)
					if (labelItem.ConnectorItem != null)
						items.Add(labelItem.ConnectorItem);
				foreach (SeriesLabelItem labelItem in SeriesLabelItems)
					items.Add(labelItem);
			}
			return items;
		}
		#region ShouldSerialize
		public bool ShouldSerializeAxisItems(XamlDesignerSerializationManager manager) {
			return false;
		}
		#endregion
	}
}

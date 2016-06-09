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
using System.Windows;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class PaneItemsControl : Diagram2DItemsControl {
		public static readonly DependencyProperty PaneProperty = DependencyPropertyManager.Register("Pane", typeof(Pane), typeof(PaneItemsControl));
		public static readonly DependencyProperty PaneContentProperty = DependencyPropertyManager.Register("PaneContent", typeof(UIElement), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty MirrorControlProperty = DependencyPropertyManager.Register("MirrorControl", typeof(UIElement), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty Pseudo3DMirrorControlProperty = DependencyPropertyManager.Register("Pseudo3DMirrorControl",
			typeof(UIElement), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty CrosshairContainerProperty = DependencyPropertyManager.Register("CrosshairContainer", 
			typeof(UIElement), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty Pseudo3DBarSeriesContainerProperty = DependencyPropertyManager.Register("Pseudo3DBarSeriesContainer",
			typeof(UIElement), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SelectionControlProperty = DependencyPropertyManager.Register("SelectionControl", typeof(UIElement), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty PaneItemsProperty = DependencyPropertyManager.Register("PaneItems", 
		   typeof(ObservableCollection<object>), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty SeriesLabelItemsProperty = DependencyPropertyManager.Register("SeriesLabelItems",
			typeof(ObservableCollection<SeriesLabelItem>), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty AxisXScrollBarItemProperty = DependencyPropertyManager.Register("AxisXScrollBarItem",
			typeof(ScrollBarItem), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty AxisYScrollBarItemProperty = DependencyPropertyManager.Register("AxisYScrollBarItem",
			typeof(ScrollBarItem), typeof(PaneItemsControl), new PropertyMetadata(PropertyChanged));
		public Pane Pane {
			get { return (Pane)GetValue(PaneProperty); }
			set { SetValue(PaneProperty, value); }
		}
		public UIElement PaneContent {
			get { return (UIElement)GetValue(PaneContentProperty); }
			set { SetValue(PaneContentProperty, value); }
		}
		public UIElement MirrorControl {
			get { return (UIElement)GetValue(MirrorControlProperty); }
			set { SetValue(MirrorControlProperty, value); }
		}
		public UIElement Pseudo3DMirrorControl {
			get { return (UIElement)GetValue(Pseudo3DMirrorControlProperty); }
			set { SetValue(Pseudo3DMirrorControlProperty, value); }
		}
		public UIElement CrosshairContainer {
			get { return (UIElement)GetValue(CrosshairContainerProperty); }
			set { SetValue(CrosshairContainerProperty, value); }
		}
		public UIElement Pseudo3DBarSeriesContainer {
			get { return (UIElement)GetValue(Pseudo3DBarSeriesContainerProperty); }
			set { SetValue(Pseudo3DBarSeriesContainerProperty, value); }
		}
		public UIElement SelectionControl {
			get { return (UIElement)GetValue(SelectionControlProperty); }
			set { SetValue(SelectionControlProperty, value); }
		}
		[NonTestableProperty]
		public ObservableCollection<object> PaneItems {
			get { return (ObservableCollection<object>)GetValue(PaneItemsProperty); }
			set { SetValue(PaneItemsProperty, value); }
		}
		[NonTestableProperty]
		public ObservableCollection<SeriesLabelItem> SeriesLabelItems {
			get { return (ObservableCollection<SeriesLabelItem>)GetValue(SeriesLabelItemsProperty); }
			set { SetValue(SeriesLabelItemsProperty, value); }
		}
		public ScrollBarItem AxisXScrollBarItem {
			get { return (ScrollBarItem)GetValue(AxisXScrollBarItemProperty); }
			set { SetValue(AxisXScrollBarItemProperty, value); }
		}
		public ScrollBarItem AxisYScrollBarItem {
			get { return (ScrollBarItem)GetValue(AxisYScrollBarItemProperty); }
			set { SetValue(AxisYScrollBarItemProperty, value); }
		}
		protected override bool ShouldCreateItemPresentation(object item) {
			return (item is Axis2DItem) || (item is AxisLabelItem) || (item is AxisTitleItem) || (item is ScrollBarItem) || base.ShouldCreateItemPresentation(item);
		}
		protected override ILayoutElement GetItemPresentation(object item) {
			Axis2DItem axisItem = item as Axis2DItem;
			if (axisItem != null)
				return new Axis2DPresentation(axisItem);
			AxisLabelItem axisLabelItem = item as AxisLabelItem;
			if (axisLabelItem != null)
				return new AxisLabelPresentation(axisLabelItem);
			AxisTitleItem axisTitleItem = item as AxisTitleItem;
			if (axisTitleItem != null)
				return new AxisTitlePresentation(axisTitleItem);
			ScrollBarItem scrollBarItem = item as ScrollBarItem;
			if (scrollBarItem != null)
				return new ScrollBarPresentation(scrollBarItem);
			SelectionGeometryItem selectionGeometryItem = item as SelectionGeometryItem;
			if (selectionGeometryItem != null)
				return new SelectionPresentation(selectionGeometryItem);
			return base.GetItemPresentation(item);
		}
		protected override ObservableCollection<object> CreateItems() {
			ObservableCollection<object> items = new ObservableCollection<object>();
			UIElement paneContent = PaneContent;
			if (paneContent != null)
				items.Add(paneContent);
			UIElement mirrorControl = MirrorControl;
			if (mirrorControl != null)
				items.Add(mirrorControl);
			UIElement pseudo3DMirrorControl = Pseudo3DMirrorControl;
			if (pseudo3DMirrorControl != null)
				items.Add(pseudo3DMirrorControl);
			ObservableCollection<object> paneItems = PaneItems;
			if (paneItems != null)
				foreach (object paneItem in paneItems)
					items.Add(paneItem);
			UIElement pseudo3DBarSeriesContainer = Pseudo3DBarSeriesContainer;
			if (pseudo3DBarSeriesContainer != null)
				items.Add(pseudo3DBarSeriesContainer);
			UIElement selectionControl = SelectionControl;
			if (selectionControl != null)
				items.Add(selectionControl);
			UIElement crosshairControl = CrosshairContainer;
			if (crosshairControl != null)
				items.Add(crosshairControl);
			ObservableCollection<SeriesLabelItem> seriesLabelItems = SeriesLabelItems;
			if (seriesLabelItems != null) {
				foreach (SeriesLabelItem labelItem in seriesLabelItems) {
					SeriesLabelConnectorItem connectorItem = labelItem.ConnectorItem;
					if (connectorItem != null)
						items.Add(connectorItem);
				}
				foreach (SeriesLabelItem labelItem in seriesLabelItems)
					items.Add(labelItem);
			}
			if (AxisXScrollBarItem != null)
				items.Add(AxisXScrollBarItem);
			if (AxisYScrollBarItem != null)
				items.Add(AxisYScrollBarItem);
			return items;
		}
	}
}

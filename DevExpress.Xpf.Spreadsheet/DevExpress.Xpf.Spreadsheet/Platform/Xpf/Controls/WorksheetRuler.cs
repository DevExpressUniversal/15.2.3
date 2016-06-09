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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class HeaderItem : Control {
		public static readonly DependencyProperty TextProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		static HeaderItem() {
			Type ownerType = typeof(HeaderItem);
			TextProperty = DependencyProperty.Register("Text", typeof(string), ownerType);
			IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), ownerType);
		}
		public HeaderItem() { }
		public HeaderItem(string text) {
			Text = text;
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
	public class WorksheetHeadersControl : Control {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemsProperty;
		static WorksheetHeadersControl() {
			OrientationProperty =
				DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WorksheetHeadersControl));
			ItemTemplateProperty =
				DependencyProperty.Register("ItemTemplate", typeof(ControlTemplate), typeof(WorksheetHeadersControl));
			ItemsProperty =
				DependencyProperty.Register("Items", typeof(ObservableCollection<HeaderItem>), typeof(WorksheetHeadersControl),
				new FrameworkPropertyMetadata((d, e) => ((WorksheetHeadersControl)d).OnItemsChanged()));
		}
		private void OnItemsChanged() {
			Invalidate();
		}
		internal void Invalidate() {
			if (panel != null) panel.InvalidateArrange();
			InvalidateVisual();
		}
		public WorksheetHeadersControl() {
			this.DefaultStyleKey = typeof(WorksheetHeadersControl);
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public ControlTemplate ItemTemplate {
			get { return (ControlTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public ObservableCollection<HeaderItem> Items {
			get { return (ObservableCollection<HeaderItem>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		HeadersPanel panel;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			panel = LayoutHelper.FindElementByType(this, typeof(HeadersPanel)) as HeadersPanel;
			panel.ItemTemplate = ItemTemplate;
			Invalidate();
		}
	}
	public class HeadersPanel : Panel {
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty LayoutInfoProperty;
		static HeadersPanel() {
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(HeadersPanel));
			LayoutInfoProperty = DependencyProperty.Register("LayoutInfo", typeof(DocumentLayout), typeof(HeadersPanel),
				new FrameworkPropertyMetadata(null, (d, e) => ((HeadersPanel)d).OnLayoutInfoChanged()));
		}
		public HeadersPanel() {
			this.Loaded += HeadersPanelLoaded;
		}
		void HeadersPanelLoaded(object sender, RoutedEventArgs e) {
			InvalidateArrange();
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public DocumentLayout LayoutInfo {
			get { return (DocumentLayout)GetValue(LayoutInfoProperty); }
			set { SetValue(LayoutInfoProperty, value); }
		}
		public ControlTemplate ItemTemplate { get; set; }
		private void OnLayoutInfoChanged() { }
		protected override Size ArrangeOverride(Size finalSize) {
			if (LayoutInfo == null) return finalSize;
			HeaderPage page = LayoutInfo.HeaderPage;
			if (page != null) page.Update();
			Size offset = LayoutInfo.GroupItemsPage.GroupItemsOffset.ToSize();
			if (Orientation == System.Windows.Controls.Orientation.Horizontal)
				ArrangeHorizontalHeaders(page, offset);
			else
				ArrangeVerticalHeaders(page, offset);
			HideChildren();
			return finalSize;
		}
		void ArrangeVerticalHeaders(HeaderPage page, Size offset) {
			if (page != null)
				ArrangeHeaders(page.RowBoxes, offset);
		}
		void ArrangeHorizontalHeaders(HeaderPage page, Size offset) {
			if (page != null)
				ArrangeHeaders(page.ColumnBoxes, offset);
		}
		void ArrangeHeaders(List<HeaderTextBox> boxes, Size offset) {
			for (int i = 0; i < boxes.Count; i++) {
				HeaderItem item = GetChild();
				item.Text = boxes[i].Text;
				item.IsSelected = boxes[i].SelectType == HeaderBoxSelectType.None ? false : true;
				Rect rect = boxes[i].Bounds.ToRect();
				item.Measure(rect.Size);
				Rect actualRect = Rect.Offset(rect, -offset.Width, -offset.Height);
				item.Arrange(actualRect);
			}
		}
		private void HideChildren() {
			for (int i = childIndex; i < Children.Count; i++)
				Children[i].Arrange(new Rect(0, 0, 0, 0));
			childIndex = 0;
		}
		int childIndex = 0;
		private HeaderItem GetChild() {
			if (childIndex >= Children.Count) {
				HeaderItem item = new HeaderItem() { Focusable = false };
				item.Template = ItemTemplate;
				Children.Add(item);
			}
			return Children[childIndex++] as HeaderItem;
		}
	}
}

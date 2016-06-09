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

using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Docking.Design {
	public class SelectionBorder : Border {
		public ModelItem Root { get; set; }
		public ModelItem PrimarySelection {
			get { return modelItem; }
			set {
				if(modelItem == value) return;
				ModelItem oldModelItem = modelItem;
				modelItem = value;
				DesignerView = modelItem == null ? null : DesignerView.FromContext(modelItem.Context);
				OnPrimarySelectionChanged(oldModelItem);
			}
		}
		protected DesignerView DesignerView {
			get { return designerView; }
			set {
				if(designerView == value) return;
				UnsubscribeDesignerViewEvents();
				designerView = value;
				SubscribeDesignerViewEvents();
			}
		}
		protected FrameworkElement SelectedElement {
			get { return selectedElement; }
			set {
				if(selectedElement == value) return;
				FrameworkElement oldElement = selectedElement;
				selectedElement = value;
				OnSelectionElementChanged(oldElement);
			}
		}
		public SelectionBorder() {
			IsHitTestVisible = false;
		}
		protected FrameworkElement GetSelectedElement() {
			BaseLayoutItem item = PrimarySelection.As<BaseLayoutItem>();
			return item;
		}
		protected virtual void OnPrimarySelectionChanged(ModelItem oldSelection) {
			SelectedElement = PrimarySelection != null ? GetSelectedElement() : null;
		}
		protected virtual void OnSelectionElementChanged(FrameworkElement oldSelectionElement) {
			if(oldSelectionElement != null) UnsubscribeEvents(oldSelectionElement);
			if(SelectedElement != null) {
				SubscribeEvents(SelectedElement);
				UpdateBounds();
				Child = DesignTimeHelper.GetSelectionElement(GetText(PrimarySelection.As<BaseLayoutItem>()));
			}
		}
		string GetText(BaseLayoutItem item) {
			bool isWizardEnabled = DesignTimeHelper.GetIsWizardEnabled();
			if(isWizardEnabled) return null;
			LayoutPanel panel = item as LayoutPanel;
			if(panel != null && panel.Content == null) {
				return DockingLocalizer.GetString(DockingStringId.DTEmptyPanelText);
			}
			LayoutGroup group = item as LayoutGroup;
			if(group != null && group.ItemType == LayoutItemType.Group && group.Items.Count == 0) {
				return DockingLocalizer.GetString(DockingStringId.DTEmptyGroupText);
			}
			return null;
		}
		protected virtual void OnSelectionElementSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateBounds();
		}
		protected virtual void OnSelectedElementLayoutUpdated(object sender, System.EventArgs e) {
			UpdateBounds();
		}
		protected virtual void OnDesignerViewZoomLevelChanged(object sender, System.EventArgs e) {
			UpdateBounds();
		}
		internal void UpdateBounds() {
			if(SelectedElement == null || Root.View == null || !LayoutHelper.IsChildElement(Root.View.PlatformObject as DependencyObject, SelectedElement)) {
				Visibility = Visibility.Collapsed;
				return;
			}
			UpdateVisibility();
			UpdatePostion();
			UpdateSize();
		}
		void SubscribeEvents(FrameworkElement control) {
			control.LayoutUpdated += OnSelectedElementLayoutUpdated;
			control.SizeChanged += OnSelectionElementSizeChanged;
		}
		void UnsubscribeEvents(FrameworkElement control) {
			control.LayoutUpdated -= OnSelectedElementLayoutUpdated;
			control.SizeChanged -= OnSelectionElementSizeChanged;
		}
		void SubscribeDesignerViewEvents() {
			if(DesignerView == null) return;
			DesignerView.ZoomLevelChanged += OnDesignerViewZoomLevelChanged;
		}
		void UnsubscribeDesignerViewEvents() {
			if(DesignerView == null) return;
			DesignerView.ZoomLevelChanged -= OnDesignerViewZoomLevelChanged;
		}
		void UpdatePostion() {
			Point position = TranslatePoint(new Point());
			Canvas.SetLeft(this, position.X);
			Canvas.SetTop(this, position.Y);
		}
		void UpdateSize() {
			Point leftTop = TranslatePoint(new Point());
			Point rightBottom = TranslatePoint(new Point(SelectedElement.ActualWidth, SelectedElement.ActualHeight));
			Width = Math.Max(0d, rightBottom.X - leftTop.X);
			Height = Math.Max(0d, rightBottom.Y - leftTop.Y);
		}
		Point TranslatePoint(Point p) {
			ViewItem viewItem = ViewItemHelper.GetViewItem(Root.View, SelectedElement);
			ModelItem parent = ViewItemHelper.GetParentModelItem<DockLayoutManager>(Root.Context, viewItem);
			if(parent == null) return new Point();
			if(viewItem.IsDescendantOf(parent.View)) {
				Point point = viewItem.TransformToView(parent.View).Transform(p);
				Size size = CorrectSize(new Size(Math.Max(0, point.X), Math.Max(0, point.Y)));
				point.X = size.Width;
				point.Y = size.Height;
				point.X *= DesignerView.ZoomLevel;
				point.Y *= DesignerView.ZoomLevel;
				return point;
			}
			return new Point();
		}
		protected Size CorrectSize(Size newSize) {
			return newSize;
		}
		void UpdateVisibility() {
			Visibility = Visibility.Visible;
		}
		DesignerView designerView;
		ModelItem modelItem;
		FrameworkElement selectedElement;
	}
	class DesignTreeView : LayoutTreeView {
		public TreeView PartTreeView { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartTreeView = PartItemsControl as TreeView;
		}
		public void EnsureItemsSource(BaseLayoutItem item = null) {
			if(PartTreeView != null) {
				IEnumerable items = item != null ?
					new ObservableCollection<BaseLayoutItem>(new BaseLayoutItem[] { item.GetRoot() }) :
					GetCustomizationItems();
				PartTreeView.ItemsSource = items;
			}
		}
	}
}

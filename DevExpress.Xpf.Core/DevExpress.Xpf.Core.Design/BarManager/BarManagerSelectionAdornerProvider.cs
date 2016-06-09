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

extern alias Platform;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Policies;
using System.Collections.Generic;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Bars;
using Microsoft.Windows.Design;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Design.SmartTags;
using System.Linq;
using System.Windows.Media;
#if SL
using FrameworkElement = Platform::System.Windows.FrameworkElement;
using Orientation = Platform::System.Windows.Controls.Orientation;
using VisualTreeHelper = Platform::System.Windows.Media.VisualTreeHelper;
using PlatformVisibility = Platform::System.Windows.Visibility;
#else
using PlatformVisibility = System.Windows.Visibility;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class SelectionAdornerProviderBase : BarManagerAdornerProviderBase {
		public SelectionBorder SelectionBorder {
			get { return selectionBorder; }
			protected set {
				if (selectionBorder == value) return;
				SelectionBorder oldSelectionBorder = selectionBorder;
				selectionBorder = value;
				OnSelectionBorderChanged(oldSelectionBorder);
			}
		}
		public Canvas Canvas { get; private set; }
		public SelectionAdornerProviderBase() {
			Canvas = new Canvas();
			Adorners.Add(Canvas);
		}
		public abstract SelectionBorder CreateSelectionBorder();
		protected override void Activate(ModelItem item) {
			SelectionBorder = CreateSelectionBorder();
			base.Activate(item);
		}
		protected override void Deactivate() {
			base.Deactivate();
			SelectionBorder = null;
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			SelectionOperations.Subscribe(Context, new SubscribeContextCallback<Selection>(OnSelectionChanged));
		}
		protected override void UnsubscribeEvents() {
			SelectionOperations.Unsubscribe(Context, new SubscribeContextCallback<Selection>(OnSelectionChanged));
			base.UnsubscribeEvents();
		}
		protected virtual void OnSelectionBorderChanged(SelectionBorder oldSelectionBorder) {
			if(oldSelectionBorder != null) {
				Canvas.Children.Remove(oldSelectionBorder);
				oldSelectionBorder.PrimarySelection = null;
			}
			if(SelectionBorder != null)
				Canvas.Children.Add(SelectionBorder);
		}
		protected virtual void OnSelectionChanged(Selection newSelection) {
			ModelItem primarySelection = newSelection.PrimarySelection;
			if(primarySelection == null || AdornedElement == null || BarManagerDesignTimeHelper.FindParentByType(AdornedElement.ItemType, primarySelection) != AdornedElement)
				SelectionBorder = CreateSelectionBorder();
			else UpdateSelection(newSelection);
		}
		protected virtual void UpdateSelection(Selection newSelection) {
			ModelItem item = newSelection.PrimarySelection;
			SelectionBorder.PrimarySelection = item;
		}
		SelectionBorder selectionBorder;
	}
	[UsesItemPolicy(typeof(SmartTagAdornerSelectionPolicy))]
	public class BarManagerSelectionAdornerProvider : SelectionAdornerProviderBase {
		public override SelectionBorder CreateSelectionBorder() {
			return new BarManagerSelectionBorder();
		}
		protected override void Deactivate() {
			base.Deactivate();
			ClearCanvas();
		}
		protected override void OnSelectionChanged(Selection newSelection) {
			ClearCanvas();
			base.OnSelectionChanged(newSelection);
		}
		protected override void UpdateSelection(Selection newSelection) {
			base.UpdateSelection(newSelection);
			if(newSelection.PrimarySelection == null) return;
			ModelItem item = newSelection.PrimarySelection;
			if(item.IsItemOfType(typeof(BarItem)))
				AddSelectionBordersForBarItem(item);
			else if(item.IsItemOfType(typeof(BarItemLinkBase))) 
				AddSelectionBordersForBarItem(BarManagerDesignTimeHelper.GetBarItemFromLink(item));
		}
		void AddSelectionBordersForBarItem(ModelItem barItem) {
			var links = BarManagerDesignTimeHelper.GetBarItemLinks(barItem).ToList();
			foreach(ModelItem link in links) {
				AddSelectionBordersForBarItemLink(link);
			}
			AddSelectionBordersForCommonBarItem(barItem);
		}
		void AddSelectionBordersForCommonBarItem(ModelItem barItem) {
			foreach(BarItemLinkControl linkControl in BarManagerDesignTimeHelper.GetCommonBarItemLinkControls(barItem)) {
				Canvas.Children.Add(new BarManagerSelectionBorder(barItem.Root) { PrimarySelection = barItem, SelectedElement = linkControl });
			}
		}
		void AddSelectionBordersForBarItemLink(ModelItem barItemLink) {
			foreach(BarItemLinkControl linkControl in BarManagerDesignTimeHelper.GetBarItemLinkControls(barItemLink)) {
				Canvas.Children.Add(new BarManagerSelectionBorder(barItemLink.Root) { PrimarySelection = barItemLink, SelectedElement = linkControl });
			}
		}
		void ClearCanvas() {
			while(Canvas.Children.Count > 0) {
				SelectionBorder border = Canvas.Children[Canvas.Children.Count - 1] as SelectionBorder;
				if(border == null) continue;
				border.PrimarySelection = null;
				Canvas.Children.Remove(border);
			}
			SelectionBorder = CreateSelectionBorder();
		}
	}
	public class BarsViewProvider : IViewProvider {
		public FrameworkElement ProvideView(ModelItem item) {
			if(item.IsItemOfType(typeof(Bar))) {
				var bar = item.GetCurrentValue() as Bar;
				return bar == null ? null : BarManagerHelper.FindBarControl(bar);
			}
			if(item.IsItemOfType(typeof(BarItemLinkBase))) {
				FrameworkElement res = null;
				var link = item.GetCurrentValue() as BarItemLinkBase;
				if(link!= null && link.CommonBarItemCollectionLink) {
					var barItem = BarManagerDesignTimeHelper.GetBarItemFromLink(item);
					res = BarManagerDesignTimeHelper.GetCommonBarItemLinkControls(barItem).FirstOrDefault(fe => fe.IsVisible) as FrameworkElement;
				}
				return res ?? GetLinkControl(item);
			}
			if(item.IsItemOfType(typeof(BarItem))) {
				FrameworkElement res = BarManagerDesignTimeHelper.GetCommonBarItemLinkControls(item).FirstOrDefault(fe => fe.IsVisible) as FrameworkElement;
				if(res != null)
					return res;
				var links = BarManagerDesignTimeHelper.GetBarItemLinks(item);
				return links.Select(link => GetLinkControl(link)).FirstOrDefault(elem => elem != null);
			}
			return null;
		}
		FrameworkElement GetLinkControl(ModelItem item) {
			var controls = BarManagerDesignTimeHelper.GetBarItemLinkControls(item);
			foreach (FrameworkElement elem in controls) {
				if (!IsHidden(elem) && !elem.RenderSize.Equals(new Size()))
					return elem;
			}
			return null;
		}
		bool IsHidden(FrameworkElement element) {
			if(element == null) return true;
#if !SL
			if(!element.IsVisible || !element.IsInVisualTree()) return true;
#endif
			var parent = element;
			while(parent != null) {
				if(parent.Opacity == 0d || parent.Visibility != PlatformVisibility.Visible) return true;
				parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
			}
			return false;
		}
	}
	public class BarManagerSelectionBorder : SelectionBorder {
		public BarManagerSelectionBorder() { }
		public BarManagerSelectionBorder(ModelItem root) : base(root) { }
		protected override FrameworkElement GetSelectedElement() {
			if(PrimarySelection == null) return null;
			if(PrimarySelection.IsItemOfType(typeof(Bar))) {
				return BarManagerHelper.FindBarControl((Bar)PrimarySelection.GetCurrentValue());
			}
			return null;
		}
		protected override Size GetSize() {
			Size originalSize = base.GetSize();
			ModelItem bar = BarManagerDesignTimeHelper.FindParentByType<Bar>(PrimarySelection);
			if(bar != null && GetOrientation(bar) == Orientation.Vertical) {
				if(SelectedElement is BarControl || (SelectedElement is BarItemLinkControlBase && GetRotateWhenVerticalValue(bar)))
					return new Size(originalSize.Height, originalSize.Width);
			}
			return originalSize;
		}
		protected override Point GetPosition() {
			Point position = base.GetPosition();
			ModelItem bar = BarManagerDesignTimeHelper.FindParentByType<Bar>(PrimarySelection);
			if(bar != null && GetOrientation(bar) == Orientation.Vertical)
				if(SelectedElement is BarControl || (SelectedElement is BarItemLinkControlBase && GetRotateWhenVerticalValue(bar)))
					position.X -= GetSize().Width;
			return position;
		}
		protected override void UpdateVisibility() {
			base.UpdateVisibility();
			if(Visibility == Visibility.Collapsed) return;
			if(SelectedElement is BarItemLinkControl && SelectedElement.Parent != null) {
				FrameworkElement control = LayoutHelper.FindLayoutOrVisualParentObject<IRibbonControl>(SelectedElement) as FrameworkElement;
				if(control == null) return;
				var pt = SelectedElement.TranslatePoint(new Platform::System.Windows.Point(), control);
				Visibility = pt.X < 0 || pt.X > control.ActualWidth || pt.Y < 0 || pt.Y > control.ActualHeight ? Visibility.Collapsed : Visibility.Visible;
			}
		}
		protected override ModelItem GetParent() {
			ModelItem parent = AttributeHelper.GetAttributes<DesignTimeParentAttribute>(PrimarySelection.ItemType).
				Select(attribute => BarManagerDesignTimeHelper.FindParentByType(attribute.ParentType, PrimarySelection.Context.Items.GetValue<Selection>().PrimarySelection)).
				FirstOrDefault(item => item != null);
			return parent;
		}
		Orientation GetOrientation(ModelItem bar) {
			BarControl barControl = BarManagerHelper.FindBarControl((Bar)bar.GetCurrentValue());
			return barControl != null ? barControl.ContainerOrientation : Orientation.Horizontal;
		}
		protected bool GetRotateWhenVerticalValue(ModelItem bar) {
			return (bool)bar.Properties["RotateWhenVertical"].ComputedValue;
		}
	}
}

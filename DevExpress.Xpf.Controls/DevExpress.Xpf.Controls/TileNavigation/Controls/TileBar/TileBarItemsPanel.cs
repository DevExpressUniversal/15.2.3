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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation.Internal {
	public class TileBarItemsPanel : HeaderedStackLayoutPanel, IScrollablePanel, IItemsPanel, ILockable {
		#region static
		static TileBarItemsPanel() {
			Type ownerType = typeof(TileBarItemsPanel);
			AllowItemClippingProperty.OverrideMetadata(ownerType, new PropertyMetadata(false));
			ShowGroupHeadersProperty.OverrideMetadata(ownerType, new PropertyMetadata(false, OnShowGroupHeadersChanged));
			IsItemsHostProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false, null, CoerceIsItemsHost));
		}
		private static void OnShowGroupHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TileBarItemsPanel)d).OnShowGroupHeadersChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		static object CoerceIsItemsHost(DependencyObject d, object baseValue) {
			return ((TileBarItemsPanel)d).CoerceIsItemsHost(baseValue);
		}
		#endregion
		public TileBarItemsPanel() {
		}
		protected override void OnShowGroupHeadersChanged(bool oldValue, bool newValue) {
			base.OnShowGroupHeadersChanged(oldValue, newValue);
			bool isLockUpdate = ((ILockable)this).IsLockUpdate;
			if(IsItemsHost && newValue != showGroupHeadersCore && !isLockUpdate && !DesignerProperties.GetIsInDesignMode(this)) throw new InvalidOperationException("Cannot change ShowGroupHeaders after a UIElementCollection has been created");
			showGroupHeadersCore = newValue;
		}
		protected override System.Windows.Media.Geometry GetGeometry() {
			var result = RectHelper.New(this.GetSize());
			Thickness clipPadding = IsHorizontal ? new Thickness(ContentPadding.Left, 0, ContentPadding.Right, 0) : new Thickness(0, ContentPadding.Top, 0, ContentPadding.Bottom);
			RectHelper.Deflate(ref result, clipPadding);
			return new RectangleGeometry { Rect = result };
		}
		protected override void OnPaddingChanged(Thickness oldValue, Thickness newValue) {
			base.OnPaddingChanged(oldValue, newValue);
			UpdateClip();
		}
		protected override void AfterArrange() {
			base.AfterArrange();
			if(ScrollableControl != null) ScrollableControl.InvalidateScroll();
		}
		private double CalculateForwardScrollOffset() {
			bool isHorz = IsHorizontal;
			double viewPort = isHorz ? ContentBounds.Size.Width : ContentBounds.Size.Height;
			double scrollSize = isHorz ? ScrollAreaSize.Width : ScrollAreaSize.Height;
			double offset = isHorz ? HorizontalOffset : VerticalOffset;
			double extent = scrollSize - offset;
			Point hitPoint = isHorz ? new Point(offset + viewPort + ChildrenBounds.X, Size.Height / 2) : new Point(Size.Width / 2, offset + viewPort + ChildrenBounds.Y);
			FrameworkElement child = this.ChildAt(hitPoint, true, true, true) as FrameworkElement;
			bool isGrouping = ItemsControl.Return(x => x.IsGrouping, () => false);
			if(child != null && !isGrouping) {
				return isHorz ? this.GetChildBounds(child).X - ChildrenBounds.X : this.GetChildBounds(child).Y - ChildrenBounds.Y;
			}
			return offset + viewPort;
		}
		private double CalculateBackwardScrollOffset() {
			bool isHorz = IsHorizontal;
			double viewPort = isHorz ? ContentBounds.Size.Width : ContentBounds.Size.Height;
			double scrollSize = isHorz ? ScrollAreaSize.Width : ScrollAreaSize.Height;
			double offset = isHorz ? HorizontalOffset : VerticalOffset;
			double extent = scrollSize - offset;
			Point hitPoint = isHorz ? new Point(0, Size.Height / 2) : new Point(Size.Width / 2, 0);
			FrameworkElement child = this.ChildAt(hitPoint, true, true, true) as FrameworkElement;
			bool isGrouping = ItemsControl.Return(x => x.IsGrouping, () => false);
			if(child != null && !isGrouping) {
				Rect childBounds = this.GetChildBounds(child);
				return isHorz ? offset + childBounds.X - ContentBounds.X - viewPort + childBounds.Width :
					offset + childBounds.Y - ContentBounds.Y - viewPort + childBounds.Height;
			}
			return offset - viewPort;
		}
		protected override PanelControllerBase CreateController() {
			return new TileBarItemsPanelController(this);
		}
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			if(ItemsControl == null) ItemsControl = LayoutHelper.FindParentObject<TileBar>(this);
			showGroupHeadersCore = TileBar.Return(x => x.ShowGroupHeaders, () => showGroupHeadersCore);
			if(ItemsControl != null && !IsItemsHost && showGroupHeadersCore) {
				logicalParent = ItemsControl;
			}
			return base.CreateUIElementCollection(logicalParent);
		}
		protected override StackPanelGroupHeaders CreateGroupHeaders() {
			return new TileBarGroupHeaders(this);
		}
		object CoerceIsItemsHost(object baseValue) {
			showGroupHeadersCore = LayoutHelper.FindParentObject<TileBar>(this).Return(x => x.ShowGroupHeaders, () => showGroupHeadersCore);
			return showGroupHeadersCore ? false : baseValue;
		}
		#region IItemsPanel Members
		bool showGroupHeadersCore;
		TileBar TileBar { get { return ItemsControl as TileBar; } }
		ItemsControl ItemsControl;
		ItemsControl IItemsPanel.ItemsControl {
			get { return ItemsControl; }
			set { ItemsControl = value; }
		}
		#endregion
		#region IScrollablePanel Members
		ScrollableControl ScrollableControl;
		ScrollableControl IScrollablePanel.ScrollOwner {
			get { return ScrollableControl; }
			set { ScrollableControl = value; }
		}
		bool IScrollablePanel.CanScrollPrev {
			get { return (IsHorizontal ? HorizontalOffset : VerticalOffset) > 0; }
		}
		bool IScrollablePanel.CanScrollNext {
			get {
				bool isHorz = IsHorizontal;
				double viewPort = isHorz ? ContentBounds.Size.Width : ContentBounds.Size.Height;
				double scrollSize = isHorz ? ScrollAreaSize.Width : ScrollAreaSize.Height;
				double offset = isHorz ? HorizontalOffset : VerticalOffset;
				return scrollSize - offset > viewPort;
			}
		}
		void IScrollablePanel.ScrollNext() {
			Controller.Scroll(Orientation, CalculateForwardScrollOffset(), true);
		}
		void IScrollablePanel.ScrollPrev() {
			Controller.Scroll(Orientation, CalculateBackwardScrollOffset(), true);
		}
		bool IScrollablePanel.BringChildIntoView(FrameworkElement child) {
			return BringChildIntoView(child, true);
		}
		Orientation IScrollablePanel.Orientation {
			get { return Orientation; }
			set { Orientation = value; }
		}
		#endregion
		class TileBarItemsPanelController : StackLayoutPanelController {
			public TileBarItemsPanelController(IStackLayoutPanel control)
				: base(control) {
				ScrollBars = Core.ScrollBars.None;
			}
			public override bool IsScrollable() {
				return true;
			}
			protected override void CheckScrollBars() {
			}
		}
		#region ILockable Members
		int lockUpdate;
		void ILockable.BeginUpdate() {
			lockUpdate++;
		}
		void ILockable.EndUpdate() {
			lockUpdate--;
		}
		bool ILockable.IsLockUpdate {
			get { return lockUpdate > 0; }
		}
		#endregion
	}
	public class TileBarItemArrowControl : TileArrowControl {
		public TileBarItemArrowControl() {
			DefaultStyleKey = typeof(TileBarItemArrowControl);
		}
	}
	public class TileBarGroupHeader : StackPanelGroupHeader {
		public TileBarGroupHeader() {
			DefaultStyleKey = typeof(TileBarGroupHeader);
		}
	}
	public class TileBarGroupHeaders : StackPanelGroupHeaders {
		public TileBarGroupHeaders(Panel container)
			: base(container) {
		}
		protected override StackPanelGroupHeader CreateItemCore() {
			var result = new TileBarGroupHeader();
			if(ItemStyle != null)
				result.SetValueIfNotDefault(FrameworkElement.StyleProperty, ItemStyle);
			return result;
		}
	}
}

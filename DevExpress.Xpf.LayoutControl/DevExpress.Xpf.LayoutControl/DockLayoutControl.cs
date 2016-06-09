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
using System.ComponentModel;
using System.Windows;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl {
	public enum Dock { None, Left, Top, Right, Bottom, Client };
	public interface IDockLayoutModel : ILayoutModelBase {
	}
	public interface IDockLayoutControl : ILayoutControlBase, IDockLayoutModel {
	}
#if !SILVERLIGHT
#endif
	[StyleTypedProperty(Property = "ItemSizerStyle", StyleTargetType = typeof(ElementSizer))]
	[DXToolboxBrowsable]
	public class DockLayoutControl : LayoutControlBase, IDockLayoutControl {
		#region Dependency Properties
		public static readonly DependencyProperty AllowItemSizingProperty =
			DependencyProperty.Register("AllowItemSizing", typeof(bool), typeof(DockLayoutControl),
				new PropertyMetadata(true, (o, e) => ((DockLayoutControl)o).OnAllowItemSizingChanged()));
		public static readonly DependencyProperty AllowHorizontalSizingProperty =
			DependencyProperty.RegisterAttached("AllowHorizontalSizing", typeof(bool), typeof(DockLayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty AllowVerticalSizingProperty =
			DependencyProperty.RegisterAttached("AllowVerticalSizing", typeof(bool), typeof(DockLayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty DockProperty =
			DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockLayoutControl),
				new PropertyMetadata(Dock.Left, OnAttachedPropertyChanged));
		public static readonly DependencyProperty ItemSizerStyleProperty =
			DependencyProperty.Register("ItemSizerStyle", typeof(Style), typeof(DockLayoutControl),
				new PropertyMetadata((o, e) => ((DockLayoutControl)o).ItemSizers.ItemStyle = (Style)e.NewValue));
		public static readonly DependencyProperty UseDesiredWidthAsMaxWidthProperty =
			DependencyProperty.RegisterAttached("UseDesiredWidthAsMaxWidth", typeof(bool), typeof(DockLayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty UseDesiredHeightAsMaxHeightProperty =
			DependencyProperty.RegisterAttached("UseDesiredHeightAsMaxHeight", typeof(bool), typeof(DockLayoutControl),
				new PropertyMetadata(OnAttachedPropertyChanged));
		public static bool GetAllowHorizontalSizing(UIElement element) {
			return (bool)element.GetValue(AllowHorizontalSizingProperty);
		}
		public static void SetAllowHorizontalSizing(UIElement element, bool value) {
			element.SetValue(AllowHorizontalSizingProperty, value);
		}
		public static bool GetAllowVerticalSizing(UIElement element) {
			return (bool)element.GetValue(AllowVerticalSizingProperty);
		}
		public static void SetAllowVerticalSizing(UIElement element, bool value) {
			element.SetValue(AllowVerticalSizingProperty, value);
		}
		public static Dock GetDock(UIElement element) {
			return (Dock)element.GetValue(DockProperty);
		}
		public static void SetDock(UIElement element, Dock value) {
			element.SetValue(DockProperty, value);
		}
		public static bool GetUseDesiredWidthAsMaxWidth(UIElement element) {
			return (bool)element.GetValue(UseDesiredWidthAsMaxWidthProperty);
		}
		public static void SetUseDesiredWidthAsMaxWidth(UIElement element, bool value) {
			element.SetValue(UseDesiredWidthAsMaxWidthProperty, value);
		}
		public static bool GetUseDesiredHeightAsMaxHeight(UIElement element) {
			return (bool)element.GetValue(UseDesiredHeightAsMaxHeightProperty);
		}
		public static void SetUseDesiredHeightAsMaxHeight(UIElement element, bool value) {
			element.SetValue(UseDesiredHeightAsMaxHeightProperty, value);
		}
		#endregion Dependency Properties
		static DockLayoutControl() {
			PaddingProperty.OverrideMetadata(typeof(DockLayoutControl), new PropertyMetadata(new Thickness(0)));
		}
		public DockLayoutControl() {
			ItemSizers = CreateItemSizers();
			ItemSizers.SizingAreaWidth = ItemSpace;
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("DockLayoutControlAllowItemSizing")]
#endif
		public bool AllowItemSizing {
			get { return (bool)GetValue(AllowItemSizingProperty); }
			set { SetValue(AllowItemSizingProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("DockLayoutControlItemSizerStyle")]
#endif
		public Style ItemSizerStyle {
			get { return (Style)GetValue(ItemSizerStyleProperty); }
			set { SetValue(ItemSizerStyleProperty, value); }
		}
		#region Children
		protected override bool IsTempChild(UIElement child) {
			return base.IsTempChild(child) || ItemSizers.IsItem(child);
		}
		#endregion Children
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			var result = base.OnMeasure(availableSize);
			InitChildrenMaxSizeAsDesiredSize();
			return result;
		}
		protected override Size OnArrange(Rect bounds) {
			ItemSizers.MarkItemsAsUnused();
			var result = base.OnArrange(bounds);
			ItemSizers.DeleteUnusedItems();
			return result;
		}
		protected override LayoutProviderBase CreateLayoutProvider() {
			return new DockLayoutProvider(this);
		}
		protected override LayoutParametersBase CreateLayoutProviderParameters() {
			return new DockLayoutParameters(ItemSpace, AllowItemSizing ? ItemSizers : null);
		}
		#endregion Layout
		#region XML Storage
		protected override void ReadCustomizablePropertiesFromXML(FrameworkElement element, XmlReader xml) {
			base.ReadCustomizablePropertiesFromXML(element, xml);
			element.ReadPropertyFromXML(xml, WidthProperty, "Width", typeof(double));
			element.ReadPropertyFromXML(xml, HeightProperty, "Height", typeof(double));
		}
		protected override void WriteCustomizablePropertiesToXML(FrameworkElement element, XmlWriter xml) {
			base.WriteCustomizablePropertiesToXML(element, xml);
			element.WritePropertyToXML(xml, WidthProperty, "Width");
			element.WritePropertyToXML(xml, HeightProperty, "Height");
		}
		#endregion XML Storage
		protected override PanelControllerBase CreateController() {
			return new DockLayoutController(this);
		}
		protected virtual ElementSizers CreateItemSizers() {
			return new ElementSizers(this);
		}
#if SILVERLIGHT
		protected override Thickness GetDefaultPadding() {
			return new Thickness(0);
		}
#endif
		protected virtual void InitChildrenMaxSizeAsDesiredSize() {
			foreach(var child in GetLogicalChildren(true)) {
				if(GetUseDesiredWidthAsMaxWidth(child) && child.DesiredSize.Width != 0 && double.IsInfinity(child.MaxWidth))
					child.MaxWidth = child.DesiredSize.Width;
				if(GetUseDesiredHeightAsMaxHeight(child) && child.DesiredSize.Height != 0 && double.IsInfinity(child.MaxHeight))
					child.MaxHeight = child.DesiredSize.Height;
			}
		}
		protected virtual void OnAllowItemSizingChanged() {
			InvalidateArrange();
		}
		protected virtual void OnAllowHorizontalSizingChanged(FrameworkElement child) {
			InvalidateArrange();
		}
		protected virtual void OnAllowVerticalSizingChanged(FrameworkElement child) {
			InvalidateArrange();
		}
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if(property == AllowHorizontalSizingProperty)
				OnAllowHorizontalSizingChanged(child);
			if(property == AllowVerticalSizingProperty)
				OnAllowVerticalSizingChanged(child);
			if(property == DockProperty)
				OnDockChanged(child, (Dock)oldValue, (Dock)newValue);
			if(property == UseDesiredWidthAsMaxWidthProperty)
				OnUseDesiredWidthAsMaxWidthChanged(child);
			if(property == UseDesiredHeightAsMaxHeightProperty)
				OnUseDesiredHeightAsMaxHeightChanged(child);
		}
		protected virtual void OnDockChanged(FrameworkElement child, Dock oldValue, Dock newValue) {
			Changed();
		}
		protected override void OnItemSpaceChanged(double oldValue, double newValue) {
			base.OnItemSpaceChanged(oldValue, newValue);
			if(ItemSizers != null)
				ItemSizers.SizingAreaWidth = ItemSpace;
		}
		protected virtual void OnUseDesiredWidthAsMaxWidthChanged(FrameworkElement child) {
			if(GetUseDesiredWidthAsMaxWidth(child) && child.DesiredSize.Width != 0) {
				var prevChildWidth = child.Width;
				if(!double.IsNaN(prevChildWidth)) {
					child.Width = double.NaN;
					UpdateLayout();
				}
				child.MaxWidth = child.DesiredSize.Width;
				child.Width = prevChildWidth;
			}
			if(!GetUseDesiredWidthAsMaxWidth(child))
				child.MaxWidth = double.PositiveInfinity;
		}
		protected virtual void OnUseDesiredHeightAsMaxHeightChanged(FrameworkElement child) {
			if(GetUseDesiredHeightAsMaxHeight(child) && child.DesiredSize.Height != 0) {
				var prevChildHeight = child.Height;
				if(!double.IsNaN(prevChildHeight)) {
					child.Height = double.NaN;
					UpdateLayout();
				}
				child.MaxHeight = child.DesiredSize.Height;
				child.Height = prevChildHeight;
			}
			if(!GetUseDesiredHeightAsMaxHeight(child))
				child.MaxHeight = double.PositiveInfinity;
		}
		protected ElementSizers ItemSizers { get; private set; }
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.DockLayoutControlAutomationPeer(this);
		}
		#endregion
	}
	public class DockLayoutParameters : LayoutParametersBase {
		public DockLayoutParameters(double itemSpace, ElementSizers itemSizers)
			: base(itemSpace) {
			ItemSizers = itemSizers;
		}
		public ElementSizers ItemSizers { get; private set; }
	}
	public class DockLayoutProvider : LayoutProviderBase {
		public DockLayoutProvider(IDockLayoutModel model)
			: base(model) {
		}
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			var result = new Size(0, 0);
			var offsets = new Offsets(Parameters.ItemSpace);
			foreach (FrameworkElement item in items) {
				Dock itemDock = DockLayoutControl.GetDock(item);
				if (itemDock == Dock.None)
					item.Measure(maxSize);
				else
					if (itemDock != Dock.Client) {
						item.Measure(offsets.GetClientSize(maxSize));
						Size itemSize = item.GetDesiredSize();
						SizeHelper.UpdateMaxSize(ref result, offsets.GetFullSize(itemSize));
						offsets.Update(itemDock, itemSize);
					}
			}
			Size clientSize = offsets.GetClientSize(maxSize);
			Size clientDockSize = SizeHelper.Zero;
			foreach (FrameworkElement item in items)
				if (DockLayoutControl.GetDock(item) == Dock.Client) {
					item.Measure(clientSize);
					SizeHelper.UpdateMaxSize(ref clientDockSize, item.GetDesiredSize());
				}
			if (!clientDockSize.IsZero())
				SizeHelper.UpdateMaxSize(ref result, offsets.GetFullSize(clientDockSize));
			return result;
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			var offsets = new Offsets(Parameters.ItemSpace);
			var clientDockedItems = new FrameworkElements();
			foreach (FrameworkElement item in items) {
				Dock itemDock = DockLayoutControl.GetDock(item);
				if (itemDock == Dock.None)
					item.Arrange(new Rect(new Point(item.GetLeft(), item.GetTop()), item.DesiredSize));
				else
					if (itemDock == Dock.Client)
						clientDockedItems.Add(item);
					else {
						Rect itemBounds = RectHelper.New(item.DesiredSize);
						AlignBounds(ref itemBounds, offsets.GetClientBounds(bounds), itemDock);
						item.Arrange(itemBounds);
						offsets.Update(itemDock, itemBounds.Size());
					}
			}
			Rect clientDockBounds = offsets.GetClientBounds(bounds);
			foreach (FrameworkElement item in clientDockedItems)
				item.Arrange(clientDockBounds);
			if (Parameters.ItemSizers != null)
				AddItemSizers(items);
			return base.OnArrange(items, bounds, viewPortBounds);
		}
		protected virtual void AlignBounds(ref Rect bounds, Rect area, Dock dock) {
			if(dock == Dock.Left || dock == Dock.Right) {
				if(dock == Dock.Left)
					bounds.X = area.Left;
				else
					bounds.X = area.Right - bounds.Width;
				bounds.Y = area.Y;
				bounds.Height = area.Height;
			}
			else {
				bounds.X = area.X;
				bounds.Width = area.Width;
				if(dock == Dock.Top)
					bounds.Y = area.Top;
				else
					bounds.Y = area.Bottom - bounds.Height;
			}
		}
		protected virtual void AddItemSizers(FrameworkElements items) {
			Side? itemSizingSide;
			foreach(FrameworkElement item in items) {
				itemSizingSide = GetItemSizingSide(item);
				if(itemSizingSide != null && IsItemSizeable(item, itemSizingSide.Value))
					Parameters.ItemSizers.Add(item, itemSizingSide.Value);
			}
		}
		protected Side? GetItemSizingSide(FrameworkElement item) {
			var itemDock = DockLayoutControl.GetDock(item);
			switch(itemDock) {
				case Dock.Left:
					return Side.Right;
				case Dock.Top:
					return Side.Bottom;
				case Dock.Right:
					return Side.Left;
				case Dock.Bottom:
					return Side.Top;
				default:
					return null;
			}
		}
		protected bool IsItemSizeable(FrameworkElement item, Side side) {
			if(side == Side.Left || side == Side.Right)
				return DockLayoutControl.GetAllowHorizontalSizing(item);
			else
				return DockLayoutControl.GetAllowVerticalSizing(item);
		}
		protected new IDockLayoutModel Model { get { return (IDockLayoutModel)base.Model; } }
		protected new DockLayoutParameters Parameters { get { return (DockLayoutParameters)base.Parameters; } }
		protected class Offsets : Dictionary<Dock, double> {
			public Offsets(double itemSpace) {
				ItemSpace = itemSpace;
				Add(Dock.Left, 0);
				Add(Dock.Top, 0);
				Add(Dock.Right, 0);
				Add(Dock.Bottom, 0);
			}
			public Rect GetClientBounds(Rect bounds) {
				var clientSize = GetClientSize(bounds.Size());
				return new Rect(bounds.X + this[Dock.Left], bounds.Y + this[Dock.Top], clientSize.Width, clientSize.Height);
			}
			public Size GetClientSize(Size size) {
				return new Size(
					Math.Max(0, size.Width - (this[Dock.Left] + this[Dock.Right])),
					Math.Max(0, size.Height - (this[Dock.Top] + this[Dock.Bottom])));
			}
			public Size GetFullSize(Size size) {
				return new Size(this[Dock.Left] + size.Width + this[Dock.Right], this[Dock.Top] + size.Height + this[Dock.Bottom]);
			}
			public void Update(Dock itemDock, Size itemSize) {
				double itemWidth;
				if(itemDock == Dock.Left || itemDock == Dock.Right)
					itemWidth = itemSize.Width;
				else
					itemWidth = itemSize.Height;
				this[itemDock] += itemWidth + ItemSpace;
			}
			protected double ItemSpace { get; private set; }
		}
	}
	public class DockLayoutController : LayoutControllerBase {
		public DockLayoutController(IDockLayoutControl control)
			: base(control) {
		}
		#region Scrolling
		public override bool IsScrollable() {
			return false;
		}
		#endregion Scrolling
	}
}

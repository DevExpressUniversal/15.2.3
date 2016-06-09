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

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System;
using System.Windows.Data;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils.Themes;
using System.Collections.Generic;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Bars.Helpers;
namespace DevExpress.Xpf.Ribbon {
	public class GalleryDropDownPopupMenu : PopupMenu {
		#region static
		public static readonly DependencyProperty InitialVisibleRowCountProperty;
		public static readonly DependencyProperty InitialVisibleColCountProperty;
		public static readonly DependencyProperty GalleryProperty;
		static GalleryDropDownPopupMenu() {
			GalleryProperty = DependencyPropertyManager.Register("Gallery", typeof(Gallery), typeof(GalleryDropDownPopupMenu),
				new FrameworkPropertyMetadata(null, OnGalleryPropertyChanged));
			InitialVisibleRowCountProperty = DependencyPropertyManager.Register("InitialVisibleRowCount", typeof(int), typeof(GalleryDropDownPopupMenu),
				new FrameworkPropertyMetadata(0, OnInitialVisibleRowCountPropertyChanged));
			InitialVisibleColCountProperty = DependencyPropertyManager.Register("InitialVisibleColCount", typeof(int), typeof(GalleryDropDownPopupMenu),
				new FrameworkPropertyMetadata(0, OnInitialVisibleColCountPropertyChanged));			
		}
		static void OnGalleryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryDropDownPopupMenu)obj).OnGalleryChanged(e.OldValue as Gallery);
		}
		static void OnInitialVisibleRowCountPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryDropDownPopupMenu)obj).UpdateBarControlProperties();
		}
		static void OnInitialVisibleColCountPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryDropDownPopupMenu)obj).UpdateBarControlProperties();
		}
		#endregion
		public GalleryDropDownPopupMenu() : this(null, null) { }
		public GalleryDropDownPopupMenu(BarItemLinkCollection itemLinks, RibbonGalleryBarItemLinkControl ownerLinkControl) : base(itemLinks) {
			IsBranchHeader = true;
			OwnerLinkControl = ownerLinkControl;
			AllowMouseCapturing = true;
		}
		BarItemLinkCollection itemLinksCore = null;
		protected override void InitContentControl(object context) {
			UpdateItemLinks(context);
			base.InitContentControl(context);
		}
		protected override void OnClosedCore() {
			base.OnClosedCore();
			var lc = OwnerLinkControl as RibbonGalleryBarItemLinkControl;
			if(lc != null && lc.DropDownButton != null)
				lc.DropDownButton.ClearValue(RibbonCheckedBorderControl.IsCheckedProperty);
		}
		protected override void OnItemLinksProperyChanged(BarItemLinkCollection oldValue, BarItemLinkCollection newValue) {
			base.OnItemLinksProperyChanged(oldValue, newValue);
			if(itemLinksCore == null && oldValue != null)
				itemLinksCore = oldValue;
		}
		protected override void OnPreviewMouseOutside(MouseButtonEventArgs e) {
			if(e.OriginalSource == Mouse.Captured)
				return;
			base.OnPreviewMouseOutside(e);
		}
		protected override LinkContainerType GetLinkContainerType() {
			return LinkContainerType.DropDownGallery;
		}
		protected virtual void OnGalleryChanged(Gallery oldValue) {
			if (oldValue != null) {
				if (oldValue.Parent == this)
					RemoveLogicalChild(oldValue);
				oldValue.ItemClick -= OnGalleryItemClick;
			}
			if (Gallery != null) {
				if (Gallery.Parent == null)
					AddLogicalChild(Gallery);
				Gallery.ItemClick += new GalleryItemEventHandler(OnGalleryItemClick);
				InitialVisibleColCount = Gallery.MinColCount != 0 ? Gallery.MinColCount : Gallery.ColCount;
				InitialVisibleRowCount = Gallery.RowCount;
			}
			if (!HasVisibleItems())
				ClosePopup();
			if(DropDownControl != null) DropDownControl.Gallery = Gallery;
		}
		protected virtual void UpdateBarControlProperties() {
			if(DropDownControl != null) {
				DropDownControl.InitialVisibleColCount = InitialVisibleColCount;
				DropDownControl.InitialVisibleRowCount = InitialVisibleRowCount;
			}
		}
		void UpdateItemLinks(object context) {
			if(context is BarItemLinkCollection)
				this.SetValue(ItemLinksPropertyKey, (BarItemLinkCollection)context);
			else if(itemLinksCore != null && ItemLinks != itemLinksCore) {
				this.SetValue(ItemLinksPropertyKey, itemLinksCore);
			}
		}
		void OnGalleryItemClick(object sender, EventArgs e) {
			if(Gallery.AutoHideGallery) IsOpen = false;
		}	   
		public int InitialVisibleColCount {
			get { return (int)GetValue(InitialVisibleColCountProperty); }
			set { SetValue(InitialVisibleColCountProperty, value); }
		}
		public int InitialVisibleRowCount {
			get { return (int)GetValue(InitialVisibleRowCountProperty); }
			set { SetValue(InitialVisibleRowCountProperty, value); }
		}
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
		protected override Core.PopupBorderControl CreateBorderControl() {
			return new GalleryDropDownPopupBorderControl();
		}
		protected internal GalleryDropDownControl DropDownControl { get { return PopupContent as GalleryDropDownControl; } }
		protected override object CreatePopupContent() {
			GalleryDropDownControl content = new GalleryDropDownControl(this, GetLinkContainerType());
			content.Gallery = Gallery;
			return content;
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				List<object> newLogicalChildren = new List<object>();
				System.Collections.IEnumerator oldLogicalChildren = base.LogicalChildren;
				while (oldLogicalChildren.MoveNext())
					newLogicalChildren.Add(oldLogicalChildren.Current);
				if (Gallery != null && Gallery.Parent == this)
					newLogicalChildren.Add(Gallery);
				return newLogicalChildren.GetEnumerator();
			}
		}
		delegate void ResetDesiredRowCountDelegate();
		protected internal void CloseWithTimer() {
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(400);
			timer.Tick += OnCloseTimerTick;
			timer.Start();
		}
		void OnCloseTimerTick(object sender, EventArgs e){
			DispatcherTimer timer = sender as DispatcherTimer;
			if(timer != null)
				timer.Tick -= OnCloseTimerTick;
			ClosePopup();
		}
		protected override bool HasVisibleItems() {
			return base.HasVisibleItems() || Gallery != null;
		}
		protected override void UpdatePlacement(UIElement control) {
			Placement = PlacementMode.Bottom;
			if(SystemParameters.MenuDropAlignment && OwnerLinkControl is RibbonGalleryBarItemLinkControl) {
				Placement = ExpandMode == BarPopupExpandMode.Classic ? PlacementMode.Right : PlacementMode.Bottom;
				HorizontalOffset = ExpandMode == BarPopupExpandMode.Classic ? -control.RenderSize.Width : control.RenderSize.Width;
			}
		}
	}
	public class GalleryDropDownControl : PopupMenuBarControl {
		#region static
		public static readonly DependencyProperty GalleryProperty;
		public static readonly DependencyProperty IsMenuVisibleProperty;
		public static readonly DependencyProperty InitialVisibleColCountProperty;
		public static readonly DependencyProperty InitialVisibleRowCountProperty;
		static GalleryDropDownControl() {			
			GalleryProperty = DependencyPropertyManager.Register("Gallery", typeof(Gallery), typeof(GalleryDropDownControl), new FrameworkPropertyMetadata(null, OnGalleryPropertyChanged));
			IsMenuVisibleProperty = DependencyPropertyManager.Register("IsMenuVisible", typeof(bool), typeof(GalleryDropDownControl), new FrameworkPropertyMetadata(true));
			InitialVisibleRowCountProperty = DependencyPropertyManager.Register("InitialVisibleRowCount", typeof(int), typeof(GalleryDropDownControl), new FrameworkPropertyMetadata(0));
			InitialVisibleColCountProperty = DependencyPropertyManager.Register("InitialVisibleColCount", typeof(int), typeof(GalleryDropDownControl), new FrameworkPropertyMetadata(0));
		}
		static void OnGalleryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryDropDownControl)obj).OnGalleryChanged(e.OldValue as Gallery);
		}
		#endregion
		#region propdefs
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
		public bool IsMenuVisible {
			get { return (bool)GetValue(IsMenuVisibleProperty); }
			set { SetValue(IsMenuVisibleProperty, value); }
		}
		public int InitialVisibleRowCount {
			get { return (int)GetValue(InitialVisibleRowCountProperty); }
			set { SetValue(InitialVisibleRowCountProperty, value); }
		}
		public int InitialVisibleColCount {
			get { return (int)GetValue(InitialVisibleColCountProperty); }
			set { SetValue(InitialVisibleColCountProperty, value); }
		}
		#endregion
		public GalleryDropDownControl() : this(null, LinkContainerType.DropDownGallery) { }
		public GalleryDropDownControl(GalleryDropDownPopupMenu galleryDropDown, LinkContainerType containerType) : base() {
			Popup = galleryDropDown;
			ContainerType = containerType;
			DefaultStyleKey = typeof(GalleryDropDownControl);
			ItemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(OnItemCollectionChanged);
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			UnsubscribeTemplateEvents();
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
		}
		protected virtual void OnItemCollectionChanged(object sender, ItemsChangedEventArgs e) {
			IsMenuVisible = Items.Count != 0;
		}
		protected virtual void OnGalleryChanged(Gallery oldValue) {
			if(oldValue != null) {
				oldValue.SizeModeChanged -= OnGallerySizeModeChanged;
			}
			if(Gallery != null) Gallery.SizeModeChanged += new EventHandler(OnGallerySizeModeChanged);
			UpdateResizeMode();
		}
		protected virtual void UpdateResizeMode() {
			if(Gallery == null) {
				VisualStateManager.GoToState(this, "NoneResizeMode", false);
				return;
			}
			switch(Gallery.SizeMode) {
				case GallerySizeMode.Both:
					VisualStateManager.GoToState(this, "BothResizeMode", false);
					break;
				case GallerySizeMode.None:
					VisualStateManager.GoToState(this, "NoneResizeMode", false);
					break;
				case GallerySizeMode.Vertical:
					VisualStateManager.GoToState(this, "VerticalResizeMode", false);
					break;										
			}
		}
		void OnGallerySizeModeChanged(object sender, EventArgs e) {
			UpdateResizeMode();
		}
		public delegate void SizeGripDragEventHandler(double deltaWidth, double deltaHeight);
		public SizeGripDragEventHandler SizeGripDrag;
		protected Thumb ThumbForBothResize { get; private set; }
		protected Thumb ThumbForVertResize { get; private set; }
		protected internal GalleryControlInDropDown GalleryControl { get; private set; }
		ContentControl SizeGripContainer { get; set; } 
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			ThumbForBothResize = GetTemplateChild("PART_ThumbForBothResize") as Thumb;
			ThumbForVertResize = GetTemplateChild("PART_ThumbForVertResize") as Thumb;
			SizeGripContainer =  GetTemplateChild("PART_SizeGripContainer") as ContentControl;
			GalleryControl = GetTemplateChild("PART_GalleryControl") as GalleryControlInDropDown;
			SubscribeTemplateEvents();
			UpdateResizeMode();
		}
		Size OriginScrollHostSize { get; set; }
		Point DragOffset { get; set; }
		Size MinScrollHostSize { get; set; }
		Size MaxScrollHostSize { get; set; }
		protected virtual void SubscribeTemplateEvents() {
			if(ThumbForBothResize != null) {
				ThumbForBothResize.DragDelta += new DragDeltaEventHandler(OnThumbForBothResizeDragDelta);
				ThumbForBothResize.DragStarted += new DragStartedEventHandler(OnThumbForBothResizeDragStarted);				
				ThumbForBothResize.MouseEnter += new MouseEventHandler(OnThumbForBothResizeMouseEnter);
			}
			if(ThumbForVertResize != null) {
				ThumbForVertResize.DragDelta += new DragDeltaEventHandler(OnThumbForVertResizeDragDelta);
				ThumbForVertResize.DragStarted += new DragStartedEventHandler(OnThumbForVertResizeDragStarted);
			}
		}
		protected virtual void UnsubscribeTemplateEvents() {
			if(ThumbForBothResize != null) {
				ThumbForBothResize.DragDelta -= OnThumbForBothResizeDragDelta;
				ThumbForBothResize.DragStarted -= OnThumbForBothResizeDragStarted;
				ThumbForBothResize.MouseEnter -= OnThumbForBothResizeMouseEnter;
			}
			if(ThumbForVertResize != null) {
				ThumbForVertResize.DragDelta -= OnThumbForVertResizeDragDelta;
				ThumbForVertResize.DragStarted -= OnThumbForVertResizeDragStarted;
			}
		}
		protected virtual void OnThumbForBothResizeMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			UpdateDragCursor();
		}
		protected virtual void UpdateDragCursor() {
			if(ThumbForBothResize == null)
				return;
			ThumbForBothResize.Cursor = FlowDirection == FlowDirection.LeftToRight ? Cursors.SizeNWSE : Cursors.SizeNESW;
		}
		protected virtual Size GetMinScrollHostSize() {
			Size actualPopupSize = GetPopupActualSize();
			Size scrollHostSize = GalleryControl.GetScrollHostRenderSize();
			Size minScrollHostSize = GalleryControl.GetMinScrollHostSize();
			Size minPopupSize = GetPopupMinSize();
			if(actualPopupSize.Width - minPopupSize.Width < scrollHostSize.Width - minScrollHostSize.Width) 
				minScrollHostSize.Width = scrollHostSize.Width - actualPopupSize.Width + minPopupSize.Width;
			if(actualPopupSize.Height - minPopupSize.Height < scrollHostSize.Height - minScrollHostSize.Height)
				minScrollHostSize.Height = scrollHostSize.Height - actualPopupSize.Height + minPopupSize.Height;
			return minScrollHostSize;
		}
		protected virtual Size GetMaxScrollHostSize() {
			return new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
		protected virtual void OnDragStarted() {
			OriginScrollHostSize = GalleryControl.GetScrollHostRenderSize();
			MinScrollHostSize = GetMinScrollHostSize();
			MaxScrollHostSize = GetMaxScrollHostSize();
			DragOffset = new Point();
		}
		void OnThumbForVertResizeDragStarted(object sender, DragStartedEventArgs e) {
			OnDragStarted();
		}
		void OnThumbForBothResizeDragStarted(object sender, DragStartedEventArgs e) {
			OnDragStarted();
		}
		void OnThumbForVertResizeDragDelta(object sender, DragDeltaEventArgs e) {			
			ChangeSize(0, e.VerticalChange);
		}
		void OnThumbForBothResizeDragDelta(object sender, DragDeltaEventArgs e) {
			ChangeSize(e.HorizontalChange, e.VerticalChange);
		}
		Size GetPopupMinSize() {
			if(Popup == null) return new Size();
			return new Size(Popup.MinWidth, Popup.MinHeight);
		}
		Size GetPopupActualSize() {
			Size retVal = new Size(Width, Height);
			if(Popup == null) return retVal;
			if(double.IsNaN(retVal.Width)) {
				if(Popup.Child != null)
					retVal.Width = Popup.Child.RenderSize.Width;
				else
					retVal.Width = 0;
			}
			if(double.IsNaN(retVal.Height)) {
				if(Popup.Child != null)
					retVal.Height = Popup.Child.RenderSize.Height;
				else
					retVal.Height = 0;
			}
			return retVal;
		}
		void CorrectByScreenBounds(ref double horizontalChange, ref double verticalChange) {
			if(Popup == null) 
				return;
			Point targetOffset = ScreenHelper.GetScreenPoint(Popup.PlacementTarget as FrameworkElement);
			Rect rect = ScreenHelper.GetScreenRect(Popup.Child as FrameworkElement);
			Point offset = ScreenHelper.GetScreenPoint(Popup.Child as FrameworkElement);
			FrameworkElement child = Popup.Child as FrameworkElement;
			if(FlowDirection == System.Windows.FlowDirection.LeftToRight) {
				if(offset.X + child.ActualWidth + horizontalChange > rect.X + rect.Width) {
					horizontalChange = rect.X + rect.Width - offset.X - child.ActualWidth;
				}
				if(targetOffset.Y > offset.Y) {
					verticalChange = 0;
				}
				else if(offset.Y + child.ActualHeight + verticalChange > rect.Y + rect.Height) {
					verticalChange = rect.Y + rect.Height - offset.Y - child.ActualHeight;
				}
			}
			else {
				if(offset.X - child.ActualWidth - horizontalChange < rect.X) {
					horizontalChange = offset.X - child.ActualWidth - rect.X;
				}
				if(targetOffset.Y > offset.Y) {
					verticalChange = 0;
				}
			}		   
		}
		void ChangeSize(double horizontalChange, double verticalChange) {
			if(GalleryControl.Gallery == null) return;
			CorrectByScreenBounds(ref horizontalChange, ref verticalChange);
			Size currentScrollHostSize = GalleryControl.GetScrollHostRenderSize();
			DragOffset = new Point(currentScrollHostSize.Width - OriginScrollHostSize.Width + horizontalChange,
				currentScrollHostSize.Height - OriginScrollHostSize.Height + verticalChange);
			Size actualScrollHostSize = new Size(Math.Max(MinScrollHostSize.Width, OriginScrollHostSize.Width + DragOffset.X),
				Math.Max(MinScrollHostSize.Height, OriginScrollHostSize.Height + DragOffset.Y));
			if(!double.IsPositiveInfinity(MaxScrollHostSize.Width))
				actualScrollHostSize.Width = Math.Min(actualScrollHostSize.Width, MaxScrollHostSize.Width);
			if(!double.IsPositiveInfinity(MaxScrollHostSize.Height))
				actualScrollHostSize.Height = Math.Min(actualScrollHostSize.Height, MaxScrollHostSize.Height);
			GalleryControl.SetScrollHostSize(actualScrollHostSize);						
		}	   
	}
	[DXToolboxBrowsable(false)]
	public class GalleryControlInDropDown : GalleryControl {		
		protected override void OnLayoutUpdated() {
			base.OnLayoutUpdated();
		}
		internal void SetScrollHostSize(Size size) {
			ScrollHost.Width = size.Width;
			ScrollHost.Height = size.Height;
		}
		internal Size GetScrollHostDesiredSize() {
			return ScrollHost.DesiredSize;
		}
		internal Size GetScrollHostRenderSize() {
			return ScrollHost.RenderSize;
		}
		internal Size GetMinScrollHostSize() {
			return new Size(ScrollHost.GetColsWidth(Gallery.MinColCount), ScrollHost.GetRowsHeight(Gallery.RowCount));
		}
		internal Size GetMaxScrollHostSize() {
			return new Size(double.PositiveInfinity, double.PositiveInfinity);
		}
		internal GalleryItemGroupsControl GetGroupsControl() {
			return GroupsControl;
		}
		internal GalleryDropDownControl DropDownControl { get; set; }
	}
	public class GalleryDropDownPopupBorderControl : BarPopupBorderControl {
		public GalleryDropDownPopupBorderControl() {
			DefaultStyleKey = typeof(GalleryDropDownPopupBorderControl);
		}
	}
}

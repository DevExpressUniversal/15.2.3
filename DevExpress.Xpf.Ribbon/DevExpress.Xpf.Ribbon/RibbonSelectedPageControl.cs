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

using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using System.Windows;
using System;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon.Themes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using System.Linq;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonSelectedPageControl : ItemsControl, IComplexLayout, IToolTipPlacementTarget {
		#region static
		public static readonly DependencyProperty BorderTemplateProperty;
		public static readonly DependencyProperty SelectedPageProperty;
		public static readonly DependencyProperty ActualBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualBorderTemplatePropertyKey;
		public static readonly DependencyProperty IsPopupProperty;
		protected internal static readonly DependencyPropertyKey IsPopupPropertyKey;
		public static readonly DependencyProperty BorderTemplateInPopupProperty;
		public static readonly DependencyProperty HighlightedBorderTemplateProperty;
		public static readonly DependencyProperty HighlightedBorderTemplateInPopupProperty;
		public static readonly DependencyProperty RibbonProperty;
		public static readonly DependencyProperty ToolTipVerticalOffsetProperty =
			DependencyProperty.Register("ToolTipVerticalOffset", typeof(double), typeof(RibbonSelectedPageControl), new PropertyMetadata(0d));
		public static readonly DependencyProperty ToolTipVerticalOffsetInPopupProperty =
			DependencyProperty.Register("ToolTipVerticalOffsetInPopup", typeof(double), typeof(RibbonSelectedPageControl), new PropertyMetadata(0d));
		static RibbonSelectedPageControl() {
			BorderTemplateProperty = DependencyPropertyManager.Register("BorderTemplate", typeof(ControlTemplate), typeof(RibbonSelectedPageControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnBorderTemplatePropertyChanged)));
			BorderTemplateInPopupProperty = DependencyPropertyManager.Register("BorderTemplateInPopup", typeof(ControlTemplate), typeof(RibbonSelectedPageControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnBorderTemplateInPopupPropertyChanged)));
			SelectedPageProperty = DependencyPropertyManager.Register("SelectedPage", typeof(RibbonPage), typeof(RibbonSelectedPageControl),
				new PropertyMetadata(null, OnSelectedPagePropertyChanged));
			ActualBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderTemplate", typeof(ControlTemplate), typeof(RibbonSelectedPageControl),
				new UIPropertyMetadata(null));
			ActualBorderTemplateProperty = ActualBorderTemplatePropertyKey.DependencyProperty;
			IsPopupPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsPopup", typeof(bool), typeof(RibbonSelectedPageControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsPopupPropertyChanged)));
			IsPopupProperty = IsPopupPropertyKey.DependencyProperty;
			HighlightedBorderTemplateProperty = DependencyPropertyManager.Register("HighlightedBorderTemplate", typeof(ControlTemplate), typeof(RibbonSelectedPageControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedBorderTemplatePropertyChanged)));
			HighlightedBorderTemplateInPopupProperty = DependencyPropertyManager.Register("HighlightedBorderTemplateInPopup", typeof(ControlTemplate), typeof(RibbonSelectedPageControl),
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnHighlightedBorderTemplateInPopupPropertyChanged)));
			RibbonProperty = DependencyPropertyManager.Register("Ribbon", typeof(RibbonControl), typeof(RibbonSelectedPageControl), new PropertyMetadata(OnRibbonPropertyChanged));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(RibbonSelectedPageControl), typeof(LowerRibbonAutomationPeer), owner => new LowerRibbonAutomationPeer((RibbonSelectedPageControl)owner));
		}
		protected static void OnRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).OnRibbonChanged(((RibbonControl)e.OldValue));
		}
		protected static void OnSelectedPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).OnSelectedPageChanged((RibbonPage)e.OldValue);
		}
		protected static void OnIsPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).UpdateActualBorder();
		}
		protected static void OnBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).UpdateActualBorder();
		}
		protected static void OnBorderTemplateInPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).UpdateActualBorder();
		}
		protected static void OnHighlightedBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).UpdateActualBorder();
		}
		protected static void OnHighlightedBorderTemplateInPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonSelectedPageControl)d).UpdateActualBorder();
		}
		#endregion
		#region prop defs
		public double ToolTipVerticalOffsetInPopup {
			get { return (double)GetValue(ToolTipVerticalOffsetInPopupProperty); }
			set { SetValue(ToolTipVerticalOffsetInPopupProperty, value); }
		}
		public double ToolTipVerticalOffset {
			get { return (double)GetValue(ToolTipVerticalOffsetProperty); }
			set { SetValue(ToolTipVerticalOffsetProperty, value); }
		}
		public ControlTemplate BorderTemplate {
			get { return (ControlTemplate)GetValue(BorderTemplateProperty); }
			set { SetValue(BorderTemplateProperty, value); }
		}
		public RibbonPage SelectedPage {
			get { return (RibbonPage)GetValue(SelectedPageProperty); }
			set { SetValue(SelectedPageProperty, value); }
		}
		public ControlTemplate BorderTemplateInPopup {
			get { return (ControlTemplate)GetValue(BorderTemplateInPopupProperty); }
			set { SetValue(BorderTemplateInPopupProperty, value); }
		}
		public ControlTemplate ActualBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualBorderTemplateProperty); }
			protected set { this.SetValue(ActualBorderTemplatePropertyKey, value); }
		}
		public bool IsPopup {
			get { return (bool)GetValue(IsPopupProperty); }
			protected internal set { this.SetValue(IsPopupPropertyKey, value); }
		}
		public ControlTemplate HighlightedBorderTemplate {
			get { return (ControlTemplate)GetValue(HighlightedBorderTemplateProperty); }
			set { SetValue(HighlightedBorderTemplateProperty, value); }
		}
		public ControlTemplate HighlightedBorderTemplateInPopup {
			get { return (ControlTemplate)GetValue(HighlightedBorderTemplateInPopupProperty); }
			set { SetValue(HighlightedBorderTemplateInPopupProperty, value); }
		}
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		#endregion
		public RibbonSelectedPageControl() {
			IsFirstLayoutUpdated = true;
			DefaultStyleKey = typeof(RibbonSelectedPageControl);
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			ComplexLayoutStateCore = Bars.ComplexLayoutState.Updating;
		}
		public void ShowRibbonPageGroupInScrollViewer(RibbonPageGroup pageGroup) {
			if (ScrollViewer == null)
				return;
			ScrollViewer.ScrollToHorizontalOffset(GetGroupHorizontalOffsetInScrollViewer(GetIndexOfPageGroupControl(pageGroup)));
		}
		public void ScrollRight() {
			ScrollTo(GetFirstVisibleGroupInScrollViewer() + 1);
		}
		public void ScrollLeft() {
			ScrollTo(GetFirstVisibleGroupInScrollViewer() - 1);
		}
		protected virtual void ScrollTo(int idx) {
			double offset = GetGroupHorizontalOffsetInScrollViewer(idx);
			double actualWidth = GetActualScrollWidth();
			double diff = offset - ScrollViewer.HorizontalOffset;
			double absDiff = Math.Abs(diff);
			int coef = (int)(diff / absDiff);
			if (absDiff > actualWidth) {
				offset = ScrollViewer.HorizontalOffset + coef * Math.Min(absDiff, actualWidth);
			}
			ScrollViewer.ScrollToHorizontalOffset(offset);
		}
		double GetActualScrollWidth() {
			double actualWidth = ActualWidth;
			if (LeftRepeatButton != null)
				actualWidth -= LeftRepeatButton.ActualWidth;
			if (RightRepeatButton != null)
				actualWidth -= RightRepeatButton.ActualWidth;
			return actualWidth;
		}
		ComplexLayoutState ComplexLayoutStateCore;
		internal void SetComplexLayoutState(ComplexLayoutState state) {
			DXImage.SetLockUpdates(this, state == Bars.ComplexLayoutState.Updating);
			if(state == ComplexLayoutStateCore) return;			
			ComplexLayoutStateCore = state;
			if(ComplexLayoutStateChanged != null) ComplexLayoutStateChanged(this, new ComplexLayoutStateChangedEventArgs(ComplexLayoutStateCore));
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			LayoutUpdated -= OnLayoutUpdated;
			SetComplexLayoutState(Bars.ComplexLayoutState.Updating);
			UnsubscribeTemplateEvents();
		}
		protected internal RepeatButton LeftRepeatButton { get; private set; }
		protected internal RepeatButton RightRepeatButton { get; private set; }
		RibbonPageGroupsControl CurrentPageControl { get { return GetPageGroupsControlByPage(SelectedPage); } }
		int CurrentPageGroupIndex { get; set; }
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			ClearBindings();
			if(oldValue != null)
				oldValue.SelectedPageChanged -= OnRibbonSelectedPageChanged;
			UpdateItemsSource();
			if(Ribbon == null)
				return;
			if(OriginPageGroupControl != null)
				OriginPageGroupControl.Ribbon = Ribbon;
			SelectedPage = Ribbon.SelectedPage;
			Ribbon.SelectedPageChanged += OnRibbonSelectedPageChanged;
		}
		void OnRibbonSelectedPageChanged(object sender, RibbonPropertyChangedEventArgs e) {
			OnRibbonSelectedPageChanged();
		}
		protected internal virtual void OnRibbonSelectedPageChanged() {
			SelectedPage = Ribbon == null ? null : Ribbon.SelectedPage;
		}
		protected virtual void ClearBindings() {
			ClearValue(SelectedPageProperty);
		}
		bool IsSelectedPageFullyLoaded { get; set; }
		protected virtual void OnSelectedPageChanged(RibbonPage oldValue) {
			UpdateActualBorder();			
			foreach (var pagesControl in EnumeratePagesControls()) {
				pagesControl.SelectedPage = SelectedPage;
			}
			RibbonPageGroupsControl ctrl = GetPageGroupsControlByPage(oldValue);
			if (ctrl != null) {
				UIElement panel = VisualTreeHelper.GetParent(ctrl) as UIElement;
				if (panel != null)
					panel.InvalidateMeasure();
			}
			ctrl = GetPageGroupsControlByPage(SelectedPage);
			if (ctrl != null) {
				UIElement panel = VisualTreeHelper.GetParent(ctrl) as UIElement;
				if (panel != null)
					panel.InvalidateMeasure();
			}
			Reset();
			IsSelectedPageFullyLoaded = false;
			SetComplexLayoutState(Bars.ComplexLayoutState.Updating);
		}
		IEnumerable<RibbonPagesControl> EnumeratePagesControls() {
			return Enumerable.Range(0, Items.Count).Select(ItemContainerGenerator.ContainerFromIndex).OfType<RibbonPagesControl>();
		}
		protected virtual void UpdateActualBorder() {
			if((SelectedPage != null && SelectedPage.PageCategory != null && SelectedPage.PageCategory.IsDefault) || (SelectedPage == null) || (SelectedPage.PageCategory == null) ) {
				ActualBorderTemplate = IsPopup ? BorderTemplateInPopup : BorderTemplate;
			}
			else {
				ActualBorderTemplate = IsPopup ? HighlightedBorderTemplateInPopup : HighlightedBorderTemplate;
			}
		}
		RibbonPageGroupsControl GetPageGroupsControlByPage(RibbonPage page) {
			if(page == null)
				return null;
			foreach(RibbonPageGroupsControl groupsControl in page.PageGroupsControls) {
				if(groupsControl.PagesControl == null) continue;
				if(groupsControl.PagesControl.SelectedPageControl == this)
					return groupsControl;
			}
			return null;
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			ScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
			LeftRepeatButton = GetTemplateChild("PART_LeftRepeatButton") as RepeatButton;
			RightRepeatButton = GetTemplateChild("PART_RightRepeatButton") as RepeatButton;
			OriginPageGroupControl = GetTemplateChild("PART_OriginPageGroupControl") as RibbonPageGroupControl;
			if(OriginPageGroupControl != null) {
				OriginPageGroupControl.IsOrigin = true;
				OriginPageGroupControl.PageGroup = new RibbonPageGroup() { Caption = "Wg", ShowCaptionButton = true };
			}
			SubscribeTemplateEvents();
		}
		private void SubscribeTemplateEvents() {
			if(LeftRepeatButton != null)
				LeftRepeatButton.Click += new RoutedEventHandler(OnLeftRepeatButtonClick);
			if(RightRepeatButton != null)
				RightRepeatButton.Click += new RoutedEventHandler(OnRightRepeatButtonClick);
			if(OriginPageGroupControl != null)
				OriginPageGroupControl.Ribbon = Ribbon;
		}
		private void UnsubscribeTemplateEvents() {
			if(LeftRepeatButton != null)
				LeftRepeatButton.Click -= OnLeftRepeatButtonClick;
			if(RightRepeatButton != null)
				RightRepeatButton.Click -= OnRightRepeatButtonClick;
			if(OriginPageGroupControl != null)
				OriginPageGroupControl.Ribbon = null;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is RibbonPagesControl;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new RibbonPagesControl();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RibbonPagesControl ribbonPagesControl = (RibbonPagesControl)element;
			ribbonPagesControl.PageCategory = (RibbonPageCategoryBase)item;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			RibbonPagesControl ribbonPagesControl = (RibbonPagesControl)element;
			ribbonPagesControl.PageCategory = null;
		}
		protected override Size MeasureOverride(Size constraint) {
			return base.MeasureOverride(constraint);
		}
		void OnRightRepeatButtonClick(object sender, RoutedEventArgs e) {
			ScrollRight();
		}
		void OnLeftRepeatButtonClick(object sender, RoutedEventArgs e) {
			ScrollLeft();
		}
		protected internal virtual void UpdateItemsSource() {
			if(Ribbon != null)
				ItemsSource = Ribbon.ActualCategories;
			else
				ItemsSource = null;
		}
		internal ScrollViewer ScrollViewer { get; set; }
		protected RibbonPageGroupControl OriginPageGroupControl { get; private set; }
		private SpacingMode spacingMode;
		public SpacingMode SpacingMode {
			get { return spacingMode; }
			set {
				if (value == spacingMode)
					return;
				SpacingMode oldValue = spacingMode;
				spacingMode = value;
				OnSpacingModeChanged(oldValue);
			}
		}
		protected virtual void OnSpacingModeChanged(SpacingMode oldValue) { }
		int GetFirstVisibleGroupInScrollViewer() {
			double horizontalOffset = 0;
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if (group == null) continue;
				if(group.Visibility != Visibility.Visible)
					continue;
				if(horizontalOffset >= ScrollViewer.HorizontalOffset)
					return i;
				horizontalOffset += group.ActualWidth;
			}
			return 0;
		}
		internal double GetGroupHorizontalOffsetInScrollViewer(int visibleGroupIndex) {
			double horizontalOffset = 0;
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if(group == null || group.Visibility != Visibility.Visible)
					continue;
				if(i >= visibleGroupIndex)
					return horizontalOffset;
				horizontalOffset += group.ActualWidth;
			}
			return horizontalOffset;
		}
		bool isReduced = false;
		internal bool IsFirstLayoutUpdated { get; set; }
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			if(ScrollViewer == null)
				return;
			if(PrevActualWidth < ActualWidth && ScrollViewer.ExtentWidth <= ScrollViewer.ActualWidth || IsFirstLayoutUpdated) {
				IsFirstLayoutUpdated = false;
				Reset();
				SetComplexLayoutState(Bars.ComplexLayoutState.Updating);
			}
			PrevActualWidth = ActualWidth;
			if(ScrollViewer.ExtentWidth > ScrollViewer.ActualWidth) {
				SetComplexLayoutState(Bars.ComplexLayoutState.Updating);
				if(!Reduce())
					SetComplexLayoutState(Bars.ComplexLayoutState.Updated);
			}
			else {
				if(IsItemsControlFullyLoaded(CurrentPageControl))
					SetComplexLayoutState(Bars.ComplexLayoutState.Updated);
			}
			UpdateRepeatButtonsVisibility();
		}
		private void UpdateRepeatButtonsVisibility() {
			Visibility leftRepeatButtonVisibility = Visibility.Collapsed;
			Visibility rightRepeatButtonVisibility = Visibility.Collapsed;
			if(isReduced) {
				if(ScrollViewer.HorizontalOffset != 0) {
					leftRepeatButtonVisibility = Visibility.Visible;
				}
				if(ScrollViewer.ExtentWidth - ScrollViewer.HorizontalOffset > ScrollViewer.ActualWidth + RightRepeatButton.ActualWidth) {
					rightRepeatButtonVisibility = Visibility.Visible;
				}
			}
			LeftRepeatButton.Visibility = leftRepeatButtonVisibility;
			RightRepeatButton.Visibility = rightRepeatButtonVisibility;
		}
		internal int GetPageGroupCount() {
			if(CurrentPageControl == null) return 0;
			return CurrentPageControl.Items.Count;
		}
		internal RibbonPageGroupControl GetPageGroupControlFromIndex(int index) {			
			if(SelectedPage == null) return null;
			RibbonPageGroup group = SelectedPage.ActualGroups[index];
			foreach(RibbonPageGroupControl pageGroupControl in group.PageGroupControls) {				
				if(IsOwnPageGroupControl(pageGroupControl))
					return pageGroupControl;
			}
			return null;
		}
		bool IsOwnPageGroupControl(RibbonPageGroupControl groupControl) {
			if(groupControl == null) return false;
			if(groupControl.PageGroupsControl == null) return false;
			if(groupControl.PageGroupsControl.PagesControl == null) return false;
			return groupControl.PageGroupsControl.PagesControl.SelectedPageControl == this;
		}
		internal int GetIndexOfPageGroupControl(RibbonPageGroup control) {
			if(CurrentPageControl == null) return -1;
			return CurrentPageControl.Items.IndexOf(control);
		}
		protected void SetIsCollapsed(bool value) {
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if(group == null)
					continue;
				group.IsCollapsed = value;
			}
		}
		protected virtual Size CalcTotalSize() {
			Size sz = new Size(0, 0);
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if(group == null || group.Visibility == Visibility.Collapsed)
					continue;
				group.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				sz.Width += group.DesiredSize.Width;
				sz.Height = Math.Max(sz.Height, group.DesiredSize.Height);
			}
			return sz;
		}
		protected int LastReducedGoupIndex { get; set; }
		protected double PrevActualWidth { get; set; }
		protected internal virtual void ResetAllGroups() {
			if(Ribbon != null && (Ribbon.RibbonStyle == RibbonStyle.TabletOffice || Ribbon.RibbonStyle == RibbonStyle.OfficeSlim))
				return;
			SetIsCollapsed(false);
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if(group == null)
					continue;
				group.Reset();
			}
		}
		protected void ReduceGroup(int index) {
			var group = GetPageGroupControlFromIndex(index) as RibbonPageGroupControl;
			if (group == null)
				return;
			group.Reduce();
		}
		protected RibbonPageGroupControl GetLargestReducablePageControl(bool allowCollapse) {
			RibbonPageGroupControl retValue = null;
			double maxWidth = 0;
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if (group == null) continue;
				if(!group.CanReduce(allowCollapse)) continue;
				if(group.ActualWidth > maxWidth) {
					maxWidth = group.ActualWidth;
					retValue = group;
				}
			}
			return retValue;
		}
		protected bool ReduceGalleries() {			
			List<RibbonGalleryBarItemLinkControl> galleryList = new List<RibbonGalleryBarItemLinkControl>();
			RibbonGalleryBarItemLinkControl gallery = null;
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if(group == null)
					continue;
				gallery = RibbonPageGroupControl.FindLargestReducableGallery(group.GetGalleryList());
				if(gallery != null) galleryList.Add(gallery);
			}
			gallery = RibbonPageGroupControl.FindLargestReducableGallery(galleryList);
			if(gallery == null) return false;
			gallery.Reduce();
			return true;
		}
		protected bool ReduceButtonGroups() {
			for(int i = 0; i < GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = GetPageGroupControlFromIndex(i);
				if(group!=null && group.ReduceButtonGroups()) return true;
			}
			return false;
		}
		protected virtual bool Reduce() {
			bool allowCollapse = Ribbon != null && Ribbon.RibbonStyle != RibbonStyle.TabletOffice && Ribbon.RibbonStyle != RibbonStyle.OfficeSlim;
			if(ReduceGalleries() || ReduceButtonGroups())
				return true;
			RibbonPageGroupControl control = GetLargestReducablePageControl(false) ?? GetLargestReducablePageControl(allowCollapse);
			if(control != null) {
				control.Reduce();
				return true;
			}
			isReduced = true;
			return false;
		}
		protected int GetNextPageGroupIndex(int index) {
			for(int i = index - 1; i >= 0; i--) {
				var group = GetPageGroupControlFromIndex(i);
				if(group!=null && !group.IsCollapsed)
					return i;
			}
			return GetLastPageGroupIndex();
		}
		protected int GetLastPageGroupIndex() {
			for(int i = GetPageGroupCount() - 1; i >= 0; i--) {
				var group = GetPageGroupControlFromIndex(i);
				if(group!=null && !group.IsCollapsed)
					return i;
			}
			return -1;
		}
		protected internal virtual void Reset() {
			if(Ribbon == null)
				return;
			ResetAllGroups();
			LastReducedGoupIndex = -1;
			CurrentPageGroupIndex = -1;
			if(ScrollViewer != null) ScrollViewer.ScrollToLeftEnd();
			isReduced = false;
		}
		static protected internal bool IsItemsControlFullyLoaded(ItemsControl itemsControl) {
			if(itemsControl == null || (itemsControl.Visibility == Visibility.Visible &&
				(!itemsControl.IsLoaded || itemsControl.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)))
				return false;
			for(int i = 0; i < itemsControl.Items.Count; i++) {
				DependencyObject item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
				if(item is ItemsControl) {
					if(!IsItemsControlFullyLoaded(item as ItemsControl))
						return false;
				}
			}
			return true;
		}
		#region IComplexLayout Members
		public event ComplexLayoutStateChangedEventHandler ComplexLayoutStateChanged;
		public ComplexLayoutState ComplexLayoutState {
			get { return ComplexLayoutStateCore; }
		}
		#endregion
		protected internal virtual void RecreateEditors() {
			for(int i = 0; i < Items.Count; i++) {
				RibbonPagesControl control = (RibbonPagesControl)ItemContainerGenerator.ContainerFromIndex(i);
				if(control != null)
					control.RecreateEditors();
			}
		}
		#region IToolTipPlacementTarget
		DependencyObject IToolTipPlacementTarget.ExternalPlacementTarget {
			get { return this; }
		}
		double IToolTipPlacementTarget.HorizontalOffset {
			get { return 0d; }
		}
		BarItemLinkControlToolTipHorizontalPlacement IToolTipPlacementTarget.HorizontalPlacement {
			get { return BarItemLinkControlToolTipHorizontalPlacement.RightAtTargetLeft; }
		}
		BarItemLinkControlToolTipPlacementTargetType IToolTipPlacementTarget.HorizontalPlacementTargetType {
			get { return BarItemLinkControlToolTipPlacementTargetType.Internal; }
		}
		double IToolTipPlacementTarget.VerticalOffset {
			get { return IsPopup ? ToolTipVerticalOffsetInPopup : ToolTipVerticalOffset; }
		}
		BarItemLinkControlToolTipVerticalPlacement IToolTipPlacementTarget.VerticalPlacement {
			get { return BarItemLinkControlToolTipVerticalPlacement.BottomAtTargetBottom; }
		}
		BarItemLinkControlToolTipPlacementTargetType IToolTipPlacementTarget.VerticalPlacementTargetType {
			get { return BarItemLinkControlToolTipPlacementTargetType.External; }
		}
		#endregion
	}
}

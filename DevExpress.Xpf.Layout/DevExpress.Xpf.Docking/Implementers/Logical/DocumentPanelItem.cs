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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Bars;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.Windows.Input;
namespace DevExpress.Xpf.Docking {
	public class DocumentPanel : LayoutPanel {
		#region static
		public static readonly DependencyProperty IsMinimizedProperty;
		internal static readonly DependencyPropertyKey IsMinimizedPropertyKey;
		public static readonly DependencyProperty MDILocationProperty;
		public static readonly DependencyProperty MDISizeProperty;
		public static readonly DependencyProperty MDIStateProperty;
		public static readonly DependencyProperty MDIMergeStyleProperty;
		public static readonly DependencyProperty RestoreBoundsProperty;
		public static readonly DependencyProperty EnableMouseHoverWhenInactiveProperty;
		internal static Size DefaultMDISize = new Size(double.NaN, double.NaN);
		static DocumentPanel() {
			var dProp = new DependencyPropertyRegistrator<DocumentPanel>();
			dProp.OverrideMetadata(AllowHideProperty, false, null, CoerceAllowHide);
			dProp.RegisterReadonly("IsMinimized", ref IsMinimizedPropertyKey, ref IsMinimizedProperty, false,
				(dObj, e) => ((DocumentPanel)dObj).OnIsMinimizedChanged((bool)e.NewValue));
			dProp.RegisterAttached("MDILocation", ref MDILocationProperty, new Point(0,0), OnMDILocationChanged, CoerceMDILocation);
			dProp.RegisterAttached("MDISize", ref MDISizeProperty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DefaultMDISize, OnMDISizeChanged, CoerceMDISize);
			dProp.RegisterAttached("MDIState", ref MDIStateProperty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, MDIState.Normal, OnMDIStateChanged);
			dProp.RegisterAttached("RestoreBounds", ref RestoreBoundsProperty, new Rect());
			dProp.RegisterAttached("MDIMergeStyle", ref MDIMergeStyleProperty, MDIMergeStyle.Default, OnMDIMergeStyleChanged);
			dProp.Register("EnableMouseHoverWhenInactive", ref EnableMouseHoverWhenInactiveProperty, false);
		}
		static object CoerceMDILocation(DependencyObject dObj, object value) {
			Point location = (Point)value;
			return new Point(Math.Max(0.0, location.X), Math.Max(0.0, location.Y));
		}
		private static object CoerceMDISize(DependencyObject dObj, object baseValue) {
			return dObj is DocumentPanel ? ((DocumentPanel)dObj).CoerceMDISize((Size)baseValue) : baseValue;
		}
		static void OnMDILocationChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			MDIStateHelper.InvalidateMDIPanel(dObj);
		}
		static void OnMDISizeChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			MDIStateHelper.InvalidateMDIPanel(dObj);
		}
		static void OnMDIStateChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			if(dObj is DocumentPanel)
				((DocumentPanel)dObj).OnMDIStateChanged((MDIState)e.OldValue, (MDIState)e.NewValue);
		}
		static void OnMDIMergeStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is IMDIMergeStyleListener)
				((IMDIMergeStyleListener)d).OnMDIMergeStyleChanged((MDIMergeStyle)e.OldValue, (MDIMergeStyle)e.NewValue);
		}
		[XtraSerializableProperty]
		public static Rect GetRestoreBounds(DependencyObject dObj) {
			return (Rect)dObj.GetValue(RestoreBoundsProperty);
		}
		public static void SetRestoreBounds(DependencyObject dObj, Rect value) {
			dObj.SetValue(RestoreBoundsProperty, value);
		}
		[XtraSerializableProperty]
		public static Point GetMDILocation(DependencyObject dObj) {
			return (Point)dObj.GetValue(MDILocationProperty);
		}
		public static void SetMDILocation(DependencyObject dObj, Point value) {
			dObj.SetValue(MDILocationProperty, value);
		}
		[XtraSerializableProperty]
		public static Size GetMDISize(DependencyObject dObj) {
			return (Size)dObj.GetValue(MDISizeProperty);
		}
		public static void SetMDISize(DependencyObject dObj, Size value) {
			dObj.SetValue(MDISizeProperty, value);
		}
		[XtraSerializableProperty]
		public static MDIState GetMDIState(DependencyObject dObj) {
			return (MDIState)dObj.GetValue(MDIStateProperty);
		}
		public static void SetMDIState(DependencyObject dObj, MDIState value) {
			dObj.SetValue(MDIStateProperty, value);
		}
		[XtraSerializableProperty]
		public static MDIMergeStyle GetMDIMergeStyle(DependencyObject target) {
			return (MDIMergeStyle)target.GetValue(MDIMergeStyleProperty);
		}
		public static void SetMDIMergeStyle(DependencyObject target, MDIMergeStyle value) {
			target.SetValue(MDIMergeStyleProperty, value);
		}
		private static object CoerceAllowHide(DependencyObject d, object baseValue) {
			return ((DocumentPanel)d).CoerceAllowHide(baseValue);
		}
		#endregion static
		public DocumentPanel() {
			ActivateCommand = DelegateCommandFactory.Create<object>(o => IsActive = true, false);
			Bars.MergingProperties.SetElementMergingBehavior(this, Bars.ElementMergingBehavior.InternalWithInternal);
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.Document;
		}
		protected internal override void SetAutoHidden(bool autoHidden) { }
		protected override void ActivateItemCore() {
			if(!Manager.IsDisposing && IsMDIChild)
				Manager.MDIController.Activate(this, false);
			base.ActivateItemCore();
		}
		protected override void OnIsTabPageChanged() {
			UpdateButtons();
		}
		protected virtual void OnIsMinimizedChanged(bool minimized) {
			UpdateButtons();
			CoerceValue(MinWidthProperty);
			CoerceValue(MinHeightProperty);
			CoerceValue(ActualMinSizeProperty);
			RaiseVisualChanged();
		}
		protected override void OnShowRestoreButtonChanged(bool show) {
			base.OnShowRestoreButtonChanged(show);
			if(Parent != null)
				Parent.CoerceValue(IsRestoreButtonVisibleProperty);
		}
		protected internal override void UpdateButtons() {
			CoerceValue(IsCloseButtonVisibleProperty);
			CoerceValue(IsMinimizeButtonVisibleProperty);
			CoerceValue(IsMaximizeButtonVisibleProperty);
			CoerceValue(IsRestoreButtonVisibleProperty);
			CoerceValue(IsPinButtonVisibleProperty);
			DocumentGroup parentGroup = Parent as DocumentGroup;
			if(parentGroup != null)
				parentGroup.UpdateButtons();
		}
		protected override bool CoerceIsTabPage(bool value) {
			DocumentGroup dGroup = Parent as DocumentGroup;
			return (dGroup != null) && dGroup.IsTabbed;
		}
		protected override bool CoerceIsCloseButtonVisible(bool value) {
			value = base.CoerceIsCloseButtonVisible(value);
			DocumentGroup dGroup = Parent as DocumentGroup;
			if(dGroup != null && IsTabPage) {
				ClosePageButtonShowMode mode = dGroup.ClosePageButtonShowMode;
				switch(mode) {
					case ClosePageButtonShowMode.Default:
					case ClosePageButtonShowMode.NoWhere:
					case ClosePageButtonShowMode.InTabControlHeader:
						value = false;
						break;
					case ClosePageButtonShowMode.InAllTabPageHeaders:
					case ClosePageButtonShowMode.InAllTabPagesAndTabControlHeader:
						value &= true;
						break;
					case ClosePageButtonShowMode.InActiveTabPageHeader:
					case ClosePageButtonShowMode.InActiveTabPageAndTabControlHeader:
						value &= (dGroup.SelectedItem == this);
						break;
				}
			}
			return value;
		}
		protected override void OnParentChanged() {
			base.OnParentChanged();
			CoerceValue(IsCloseButtonVisibleProperty);
			if(Parent != null) MdiStateLockHelper.Unlock();
		}
		protected internal override void OnParentLoaded() {
			base.OnParentLoaded();
			if(MergingClient != null) MergingClient.QueueMerge();
		}
		protected internal override void OnParentUnloaded() {
			if(MergingClient != null) MergingClient.QueueUnmerge();
			base.OnParentUnloaded();
		}
		protected override void OnIsActiveChangedCore() {
			base.OnIsActiveChangedCore();
			if(!IsActive || Parent == null) return;
			DocumentGroup dGroup = Parent as DocumentGroup;
			if(dGroup == null || dGroup.IsTabbed) return;
			BaseLayoutItem[] items = dGroup.GetItems();
			if(items.Length > 0) {
				LayoutItemsHelper.UpdateZIndexes(items, this);
				if(Manager != null) {
					IUIElement pane = dGroup;
					if(pane != null) {
						pane.Children.MakeLast(GetUIElement<IUIElement>());
					}
					IView view = Manager.GetView(pane.GetRootUIScope());
					if(view != null)
						view.Invalidate();
				}
			}
			InvokeHelper.BeginInvoke(this, new Action(() => focusLocker.Unlock()), InvokeHelper.Priority.Input);
			focusLocker.Lock();
		}
		protected override void OnActualMaxSizeChanged() {
			base.OnActualMaxSizeChanged();
			CoerceValue(DocumentPanel.MDISizeProperty);
		}
		protected override void OnActualMinSizeChanged() {
			base.OnActualMinSizeChanged();
			CoerceValue(DocumentPanel.MDISizeProperty);
		}
		protected virtual object CoerceMDISize(Size size) {
			return MathHelper.ValidateSize(ActualMinSize, ActualMaxSize, size);
		}
		protected override Size CoerceActualMinSize() {
			return IsMinimized ? new Size(0, 0) : base.CoerceActualMinSize();
		}
		bool isMDIChildCore;
		protected internal bool IsMDIChild {
			get {
				DocumentGroup parent = Parent as DocumentGroup;
				return parent != null ? !parent.IsTabbed : isMDIChildCore;
			}
			set {
				isMDIChildCore = value;
			}
		}
		protected override void OnActualCaptionChanged(string value) {
			base.OnActualCaptionChanged(value);
			if(IsMDIChild && IsMaximized && IsSelectedItem) MDIControllerHelper.MergeMDITitles(this);
		}
		protected override double OnCoerceMinWidth(double value) {
			return IsMinimized ? 0d : base.OnCoerceMinWidth(value);
		}
		protected override double OnCoerceMinHeight(double value) {
			return IsMinimized ? 0d : base.OnCoerceMinHeight(value);
		}
		void ApplyMDIState(MDIState state) {
			DockLayoutManager container = this.FindDockLayoutManager();
			if(container == null || container.IsDisposing) {
				MDIStateHelper.SetMDIState(this, state);
			}
			else {
				var helperState = MDIStateHelper.GetMDIState(this);
				switch(state) {
					case MDIState.Normal:
						if(helperState != MDIState.Normal)
							container.MDIController.Restore(this);
						break;
					case MDIState.Minimized:
						if(helperState != Docking.MDIState.Minimized)
							container.MDIController.Minimize(this);
						break;
					case MDIState.Maximized:
						if(helperState != Docking.MDIState.Maximized)
							container.MDIController.Maximize(this);
						break;
				}
			}
		}
		void EnsureMDIState() {
			ParentLockHelper.DoWhenUnlocked(() =>
			{
				ApplyMDIState(MDIState);
			});
		}
		protected virtual void OnMDIStateChanged(MDIState oldValue, MDIState newValue) {
			if(Parent == null) MdiStateLockHelper.Lock();
			MdiStateLockHelper.DoWhenUnlocked(EnsureMDIState);
		}
		internal override void OnControlGotFocus() {
			if(!focusLocker.IsLocked)
				base.OnControlGotFocus();
		}
		protected virtual object CoerceAllowHide(object baseValue) {
			return false;
		}
		protected override void UnsubscribeAutoHideDisplayModeChanged(DockLayoutManager manager) { }
		protected override void SubscribeAutoHideDisplayModeChanged(DockLayoutManager manager) { }
		#region ControlBoxButtons
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DocumentPanelShowMinimizeButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowMinimizeButton {
			get { return (bool)GetValue(ShowMinimizeButtonProperty); }
			set { SetValue(ShowMinimizeButtonProperty, value); }
		}
		protected internal override bool IsMaximizable { get { return !IsTabPage; } }
		bool IsMinimizable { get { return IsMDIChild; } }
		protected override bool CoerceIsMinimizeButtonVisible(bool visible) {
			return IsMinimizable && AllowMinimize && ShowMinimizeButton && !IsMinimized;
		}
		protected override bool CoerceIsRestoreButtonVisible(bool visible) {
			return base.CoerceIsRestoreButtonVisible(visible) || IsMinimized;
		}
		#endregion ControlBoxButtons
		DevExpress.Xpf.Core.Locker focusLocker = new DevExpress.Xpf.Core.Locker();
		LockHelper mdiStateLockHelper;
		LockHelper MdiStateLockHelper {
			get {
				if(mdiStateLockHelper == null) mdiStateLockHelper = new LockHelper();
				return mdiStateLockHelper;
			}
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DocumentPanelIsMinimized")]
#endif
		public bool IsMinimized {
			get { return (bool)GetValue(IsMinimizedProperty); }
		}
		public MDIMergeStyle MDIMergeStyle {
			get { return (MDIMergeStyle)GetValue(MDIMergeStyleProperty); }
			set { SetValue(MDIMergeStyleProperty, value); }
		}
		[XtraSerializableProperty]
		public Size MDISize {
			get { return (Size)GetValue(MDISizeProperty); }
			set { SetValue(MDISizeProperty, value); }
		}
		[XtraSerializableProperty]
		public Point MDILocation {
			get { return (Point)GetValue(MDILocationProperty); }
			set { SetValue(MDILocationProperty, value); }
		}
		[XtraSerializableProperty]
		public MDIState MDIState {
			get { return (MDIState)GetValue(MDIStateProperty); }
			set { SetValue(MDIStateProperty, value); }
		}
		[XtraSerializableProperty(1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Rect SerializableMDIBounds {
			get { return MDIState == Docking.MDIState.Normal ? new Rect(MDILocation, MDISize) : GetRestoreBounds(this); }
			set {
				if(MDIState == Docking.MDIState.Normal) {
					MDILocation = value.Location();
					MDISize = value.Size();
				}
				else {
					SetRestoreBounds(this, value);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool EnableMouseHoverWhenInactive {
			get { return (bool)GetValue(EnableMouseHoverWhenInactiveProperty); }
			set { SetValue(EnableMouseHoverWhenInactiveProperty, value); }
		}
		public ICommand ActivateCommand { get; private set; }
		protected internal Size MDIDocumentSize { get; set; }
		protected internal Point? MinimizeLocation { get; set; }
		protected internal bool IsMinimizedBeforeMaximize { get; set; }
		internal IMergingClient MergingClient { get; set; }
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			if(IsMDIChild)
				return new UIAutomation.MDIDocumentAutomationPeer(this);
			return base.OnCreateAutomationPeer();
		}
		#endregion
	}
	class MDIStateHelper {
		public static readonly DependencyProperty MDIStateProperty;
		static MDIStateHelper() {
			var dProp = new DependencyPropertyRegistrator<MDIStateHelper>();
			dProp.RegisterAttached("MDIState", ref MDIStateProperty, MDIState.Normal, OnMDIStateChanged);
		}
		public static MDIState GetMDIState(DependencyObject target) {
			return (MDIState)target.GetValue(MDIStateProperty);
		}
		public static void SetMDIState(DependencyObject target, MDIState value) {
			target.SetValue(MDIStateProperty, value);
		}
		static void OnMDIStateChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs e) {
			InvalidateMDIPanel(dObj);
			InvalidateMDIContainer(dObj);
			DocumentPanel document = dObj as DocumentPanel;
			if(document != null) {
				MDIState mdiState = (MDIState)e.NewValue;
				bool minimized = mdiState == MDIState.Minimized;
				bool maximized = mdiState == MDIState.Maximized;
				document.SetValue(DocumentPanel.IsMinimizedPropertyKey, minimized);
				document.SetValue(DocumentPanel.IsMaximizedPropertyKey, maximized);
				if(minimized || maximized) {
					if((MDIState)e.OldValue == MDIState.Normal) {
						document.SetValue(DocumentPanel.RestoreBoundsProperty, GetMDIBounds(document));
						document.SetCurrentValue(DocumentPanel.MDISizeProperty, DocumentPanel.DefaultMDISize);
					}
				}
				else {
					Rect restoreBounds = DocumentPanel.GetRestoreBounds(document);
					document.SetValue(DocumentPanel.MDILocationProperty, new Point(restoreBounds.X, restoreBounds.Y));
					if(MathHelper.IsEmpty(restoreBounds.GetSize())) {
						Size mdiSize = DocumentPanel.GetMDISize(dObj);
						restoreBounds.Width = mdiSize.Width;
						restoreBounds.Height = mdiSize.Height;
					}
					document.SetValue(DocumentPanel.MDISizeProperty, new Size(restoreBounds.Width, restoreBounds.Height));
					document.ClearValue(DocumentPanel.RestoreBoundsProperty);
				}
				DocumentGroup dGroup = document.Parent as DocumentGroup;
				if(dGroup != null) dGroup.IsMaximized = maximized;
				if(document.MDIState != mdiState)
					document.MDIState = mdiState;
			}
		}
		static Rect GetMDIBounds(DependencyObject dObj) {
			return new Rect(DocumentPanel.GetMDILocation(dObj), DocumentPanel.GetMDISize(dObj));
		}
		public static void InvalidateMDIPanel(DependencyObject dObj) {
			var panel = VisualTreeHelper.GetParent(dObj) as Panel;
			if(panel != null) panel.InvalidateMeasure();
		}
		public static void InvalidateMDIContainer(DependencyObject dObj) {
			var mdiDocument = dObj;
			if(mdiDocument != null) {
				var container = LayoutHelper.FindParentObject<DocumentMDIContainer>(mdiDocument);
				if(container != null)
					container.CoerceValue(DocumentMDIContainer.HasMaximizedDocumentsProperty);
			}
		}
	}
}

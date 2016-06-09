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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Navigation;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Navigation {
	public interface INavigationFrame : DevExpress.Utils.Base.ISupportBatchUpdate {
		INavigationPageBase SelectedPage { get; set; }
		UserLookAndFeel LookAndFeel { get; }
		NavigationPageBaseCollection Pages { get; }
		bool IsRightToLeftLayout();
		bool IsRightToLeft();
		bool AttachedToNavigator { get; }
		void UpdateMergedBarsAndRibbon();
	}
	[DXToolboxItem(true), Description("A navigation control for single document interfaces. Stores a collection of pages and navigates through them using animation effects.")]
	[ToolboxBitmap(typeof(NavigationFrame), "NavigationFrame")]
	[ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	[Designer("DevExpress.XtraBars.Design.NavigationFrameDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)]
	[SmartTagSupport(typeof(NavigationFrameBounds), SmartTagSupportAttribute.SmartTagCreationMode.Auto), SmartTagFilter(typeof(NavigationFrameFilter)),
	SmartTagAction(typeof(NavigationFrameActions), "Pages", "Pages", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(NavigationFrameActions), "AddNavigationPage", "Add Page"),
	SmartTagAction(typeof(NavigationFrameActions), "BringToFront", "Bring to front", 0, SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(NavigationFrameActions), "SendToBack", "Send to back", 0, SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(NavigationFrameActions), "DockInParentContainer", "Dock in parent container", 1, SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(NavigationFrameActions), "UndockFromParentContainer", "Undock from parent container", 1, SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(NavigationFrameActions), "RemoveNavigationPage", "Remove Page")]
	public class NavigationFrame : ControlBase, ISupportLookAndFeel, INavigationFrame,
		DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory, INavigationBarClient {
		UserLookAndFeel lookAndFeelCore;
		NavigationPageBaseCollection pagesCore;
		INavigationPageBase selectedPageCore;
		TransitionManager transitionManagerCore;
		Transitions transitionTypeCore;
		public NavigationFrame() {
			this.lookAndFeelCore = new ControlUserLookAndFeel(this);
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			LookAndFeel.StyleChanged += OnStyleChanged;
			this.pagesCore = CreateNavigationPageCollection();
			this.transitionManagerCore = new TransitionManager();
			this.transitionTypeCore = Transitions.SlideFade;
			this.defaultTransitionAnimationParametersCore = CreateDefaultTransitionAnimationParameters();
			this.transitionAnimationPropertiesCore = CreateTransitionAnimationProperties(DefaultTransitionAnimationParameters);
			Pages.CollectionChanged += OnNavigationPagesCollectionChanged;
			AllowTransitionAnimation = DefaultBoolean.Default;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(!IsDisposing) {
					isDisposingCore = true;
					OnDisposing();
				}
			}
			base.Dispose(disposing);
		}
		bool isDisposingCore;
		protected internal bool IsDisposing {
			get { return isDisposingCore; }
		}
		void OnDisposing() {
			navigationStack.Clear();
			lookAndFeelCore.StyleChanged -= OnStyleChanged;
			Pages.CollectionChanged -= OnNavigationPagesCollectionChanged;
			Docking2010.Ref.Dispose(ref transitionManagerCore);
			Docking2010.Ref.Dispose(ref pagesCore);
		}
		[Category("Behavior")]
		public int SelectedPageIndex {
			get {
				if(SelectedPageCore != null)
					return Pages.IndexOf(SelectedPageCore as NavigationPageBase);
				return -1;
			}
			set { SetSelectedPageIndex(value); }
		}
		protected virtual void SetSelectedPageIndex(int newIndex) {
			newIndex = Math.Min(newIndex, Pages.Count - 1);
			if(SelectedPageIndex == newIndex) {
				if(newIndex == -1) {
					SelectedPageCore = null;
					return;
				}
			}
			SelectedPageCore = Pages.Count == 0 ? null : Pages[newIndex];
			OnSelectedPageIndexChenged();
		}
		protected virtual void OnSelectedPageIndexChenged() {
			RaiseSelectedPageIndexChanged();
		}
		RibbonAndBarsMergeStyle ribbonAndBarsMergeStyleCore = RibbonAndBarsMergeStyle.Default;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFrameRibbonAndBarsMergeStyle")]
#endif
		[DefaultValue(RibbonAndBarsMergeStyle.Default), Category("Behavior")]
		public RibbonAndBarsMergeStyle RibbonAndBarsMergeStyle {
			get { return ribbonAndBarsMergeStyleCore; }
			set { ribbonAndBarsMergeStyleCore = value; }
		}
		protected internal bool CanMergeOnSelectedPageChanged() {
			return !IsDisposing && (RibbonAndBarsMergeStyle == RibbonAndBarsMergeStyle.Always);
		}
		[Category("Behavior")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFrameSelectedPage")]
#endif
		public INavigationPage SelectedPage {
			get { return SelectedPageCore as INavigationPage; }
			set { SelectedPageCore = value; }
		}
		protected internal INavigationPageBase SelectedPageCore {
			get { return selectedPageCore; }
			set {
				if(SelectedPageCore == value) return;
				if(!RaiseSelectedPageChanging(SelectedPageCore, value)) return;
				using(TransitionCreator transition = new TransitionCreator(this, value)) {
					OnSelectedPageChanging(SelectedPageCore, value);
					var oldPage = SelectedPageCore;
					if(SelectedPageCore != null)
						SelectedPageCore.Visible = false;
					selectedPageCore = value;
					if(SelectedPageCore != null && IsHandleCreated)
						SelectedPageCore.Visible = true;
					OnSelectedPageChanged(oldPage, SelectedPageCore);
				}
			}
		}
		INavigationPageBase INavigationFrame.SelectedPage {
			get { return SelectedPageCore; }
			set { SelectedPageCore = value; }
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(SelectedPageCore != null && !SelectedPageCore.Visible) {
				SelectedPageCore.Visible = true;
				UpdateMergedBarsAndRibbon();
			}
		}
		protected TransitionManager TransitionManager {
			get { return transitionManagerCore; }
		}
		[Category("Behavior"), DefaultValue(DefaultBoolean.Default)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFrameAllowTransitionAnimation")]
#endif
		public DefaultBoolean AllowTransitionAnimation { get; set; }
		protected internal virtual bool CanUseTransition() {
			return AllowTransitionAnimation != DefaultBoolean.False;
		}
		[Category("Behavior"), DefaultValue(Transitions.SlideFade)]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFrameTransitionType")]
#endif
		public Transitions TransitionType {
			get { return transitionTypeCore; }
			set { transitionTypeCore = value; }
		}
		TransitionAnimationProperties transitionAnimationPropertiesCore;
		[Category("Behavior"), Description("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TransitionAnimationProperties TransitionAnimationProperties { get { return transitionAnimationPropertiesCore; } }
		IAnimationParameters defaultTransitionAnimationParametersCore;
		protected IAnimationParameters DefaultTransitionAnimationParameters {
			get { return defaultTransitionAnimationParametersCore; }
		}
		bool ShouldSerializeTransitionAnimationProperties() {
			return TransitionAnimationProperties != null && TransitionAnimationProperties.ShouldSerialize();
		}
		void ResetTransitionAnimationProperties() {
			TransitionAnimationProperties.Reset();
		}
		protected virtual TransitionAnimationProperties CreateTransitionAnimationProperties(IAnimationParameters defaultProperties) {
			return new TransitionAnimationProperties(defaultProperties);
		}
		protected virtual IAnimationParameters CreateDefaultTransitionAnimationParameters() {
			return new AnimationParameters(10000, 1000);
		}
		#region Transition
		protected class TransitionCreator : IDisposable {
			NavigationFrame navigationFrame;
			public bool ForwardDirection { get; set; }
			public TransitionCreator(NavigationFrame frame, INavigationPageBase newPage) {
				navigationFrame = frame;
				if(navigationFrame.SelectedPageCore == null) return;
				if(newPage != null) {
					ForwardDirection = navigationFrame.Pages.IndexOf(navigationFrame.SelectedPageCore as NavigationPageBase) < navigationFrame.Pages.IndexOf(newPage as NavigationPageBase);
				}
				if(!navigationFrame.IsHandleCreated || (navigationFrame.Site != null && navigationFrame.Site.DesignMode)) return;
				BaseTransition transition = CreateTransition(frame.TransitionType);
				transition.Parameters.Combine(frame.TransitionAnimationProperties.RetrieveTransitionAnimationParameters());
				navigationFrame.TransitionManager.Transitions.Add(new Transition()
				{
					Control = navigationFrame,
					ShowWaitingIndicator = DefaultBoolean.False,
					TransitionType = transition
				});
				if(navigationFrame.CanUseTransition())
					navigationFrame.TransitionManager.StartTransition(navigationFrame);
			}
			public void Dispose() {
				if(!navigationFrame.IsHandleCreated || (navigationFrame.Site != null && navigationFrame.Site.DesignMode)) return;
				if(navigationFrame.CanUseTransition() && navigationFrame.TransitionManager.IsTransition)
					navigationFrame.BeginInvoke(new MethodInvoker(() => navigationFrame.TransitionManager.EndTransition()));
				navigationFrame.TransitionManager.Transitions.Clear();
			}
			BaseTransition CreateTransition(Transitions transition) {
				switch(transition) {
					case Transitions.Dissolve: return new DissolveTransition();
					case Transitions.Fade: return new FadeTransition();
					case Transitions.Shape:
						var shapeTransition = new ShapeTransition();
						shapeTransition.Parameters.EffectOptions = ShapeEffectOptions.CircleIn;
						return new ShapeTransition();
					case Transitions.Clock: return new ClockTransition();
					case Transitions.SlideFade:
						var slideFadeTransition = new SlideFadeTransition();
						slideFadeTransition.Parameters.EffectOptions = ForwardDirection ? PushEffectOptions.FromRight : PushEffectOptions.FromLeft;
						return slideFadeTransition;
					case Transitions.Cover:
						return new CoverTransition();
					case Transitions.Comb: return new CombTransition();
					default:
						var pushTransition = new PushTransition();
						pushTransition.Parameters.EffectOptions = ForwardDirection ? PushEffectOptions.FromRight : PushEffectOptions.FromLeft;
						return pushTransition;
				}
			}
		}
		#endregion
		#region Events
		static readonly object selectedPageChangedCore = new object();
		static readonly object selectedPageChangingCore = new object();
		static readonly object selectedPageIndexChangedCore = new object();
		static readonly object pageAddedCore = new object();
		static readonly object pageRemovedCore = new object();
		static readonly object pageClosingCore = new object();
		[Category("Layout")]
		public event SelectedPageChangedEventHandler SelectedPageChanged {
			add { Events.AddHandler(selectedPageChangedCore, value); }
			remove { Events.RemoveHandler(selectedPageChangedCore, value); }
		}
		[Category("Layout")]
		public event SelectedPageChangingEventHandler SelectedPageChanging {
			add { Events.AddHandler(selectedPageChangingCore, value); }
			remove { Events.RemoveHandler(selectedPageChangingCore, value); }
		}
		[Category("Layout")]
		public event EventHandler SelectedPageIndexChanged {
			add { Events.AddHandler(selectedPageIndexChangedCore, value); }
			remove { Events.RemoveHandler(selectedPageIndexChangedCore, value); }
		}
		[Category("Layout")]
		public event NavigationPageEventHandler PageAdded {
			add { Events.AddHandler(pageAddedCore, value); }
			remove { Events.RemoveHandler(pageAddedCore, value); }
		}
		[Category("Layout")]
		public event NavigationPageEventHandler PageRemoved {
			add { Events.AddHandler(pageRemovedCore, value); }
			remove { Events.RemoveHandler(pageRemovedCore, value); }
		}
		[Category("Layout")]
		protected internal event SelectedPageChangingEventHandler PageClosing {
			add { Events.AddHandler(pageClosingCore, value); }
			remove { Events.RemoveHandler(pageClosingCore, value); }
		}
		protected virtual void RaiseSelectedPageChanged(INavigationPageBase oldPage, INavigationPageBase newPage) {
			SelectedPageChangedEventHandler handler = (SelectedPageChangedEventHandler)Events[selectedPageChangedCore];
			if(handler != null) handler(this, new SelectedPageChangedEventArgs(oldPage, newPage));
		}
		protected virtual bool RaiseSelectedPageChanging(INavigationPageBase oldPage, INavigationPageBase newPage) {
			SelectedPageChangingEventHandler handler = (SelectedPageChangingEventHandler)Events[selectedPageChangingCore];
			SelectedPageChangingEventArgs ea = new SelectedPageChangingEventArgs(oldPage, newPage);
			if(handler != null) handler(this, ea);
			return !ea.Cancel;
		}
		protected virtual void RaiseSelectedPageIndexChanged() {
			EventHandler handler = (EventHandler)Events[selectedPageIndexChangedCore];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaisePageAdded(INavigationPageBase page) {
			NavigationPageEventHandler handler = (NavigationPageEventHandler)Events[pageAddedCore];
			if(handler != null) handler(this, new NavigationPageEventArgs(page));
		}
		protected virtual void RaisePageRemoved(INavigationPageBase page) {
			NavigationPageEventHandler handler = (NavigationPageEventHandler)Events[pageRemovedCore];
			if(handler != null) handler(this, new NavigationPageEventArgs(page));
		}
		#endregion Events
		public void SelectNextPage() {
			SelectNextPageCore(true);
		}
		public void SelectPrevPage() {
			SelectNextPageCore(false);
		}
		protected internal bool IsRightToLeftLayout() {
			return DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeft(this) || Dock == DockStyle.Right;
		}
		bool INavigationFrame.IsRightToLeftLayout() {
			return IsRightToLeftLayout();
		}
		bool INavigationFrame.IsRightToLeft() {
			return DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeft(this);
		}
		protected virtual void SelectNextPageCore(bool forward) {
			int count = Pages.Count;
			int index = Pages.IndexOf((this as INavigationFrame).SelectedPage as NavigationPageBase);
			if(forward)
				index = (++index % count);
			else
				index = (--index + count) % count;
			(this as INavigationFrame).SelectedPage = Pages[index];
		}
		List<INavigationPageBase> navigationStack = new List<INavigationPageBase>();
		void UpdateNavigationStack(INavigationPageBase page) {
			navigationStack.Remove(page);
			navigationStack.Add(page);
		}
		protected virtual void OnSelectedPageChanging(INavigationPageBase oldPage, INavigationPageBase newPage) {
			NavigationPageBase page = oldPage as NavigationPageBase;
			if(page != null) page.ReleaseDeferredLoadControl(this);
		}
		protected virtual void OnSelectedPageChanged(INavigationPageBase oldPage, INavigationPageBase newPage) {
			UpdateNavigationStack(newPage);
			UpdateMergedBarsAndRibbon();
			RaiseSelectedPageChanged(oldPage, newPage);
			RaiseNavigationBarClientSelectedItemChanged();
			if(newPage != null)
				SelectedPageIndex = Pages.IndexOf(newPage as NavigationPageBase);
			else
				SelectedPageIndex = -1;
		}
		protected virtual NavigationPageBaseCollection CreateNavigationPageCollection() {
			return new NavigationPageBaseCollection(this);
		}
		[ListBindable(false)]
		[Editor("DevExpress.XtraBars.Design.NavigationPageBaseCollectionEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Pages")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFramePages")]
#endif
		public NavigationPageBaseCollection Pages { get { return pagesCore; } }
		void OnNavigationPagesCollectionChanged(DevExpress.XtraBars.Docking2010.Base.CollectionChangedEventArgs<NavigationPageBase> ea) {
			if(ea.Clear) {
				OnPagesCollectionClear();
				return;
			}
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				AddToContainer(ea.Element);
				OnPageAdded(ea.Element);
				RaisePageAdded(ea.Element);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				RemoveFormContainer(ea.Element);
				OnPageRemoved(ea.Element);
				RaisePageRemoved(ea.Element);
			}
			RaiseNavigationBarClientItemsSourceChanged();
		}
		protected void AddToContainer(NavigationPageBase navigationPage) {
			if(Container != null)
				Container.Add(navigationPage);
		}
		protected void RemoveFormContainer(NavigationPageBase navigationPage) {
			if(Container != null)
				Container.Remove(navigationPage);
		}
		protected virtual void OnPagesCollectionClear() { }
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			foreach(var item in Pages) {
				item.Bounds = ClientRectangle;
			}
		}
		protected virtual void OnPageAdded(NavigationPageBase navigationPage) {
			navigationPage.Parent = this;
			navigationPage.Bounds = ClientRectangle;
			if(navigationPage != SelectedPageCore)
				navigationPage.Visible = false;
			if(SelectedPageCore == null)
				SelectedPageCore = navigationPage;
		}
		protected virtual void OnPageRemoved(NavigationPageBase navigationPage) {
			navigationStack.Remove(navigationPage);
			if(SelectedPageCore == navigationPage)
				SelectedPageCore = navigationStack.LastOrDefault();
		}
		void OnStyleChanged(object sender, EventArgs e) {
			LayoutChanged();
			UpdateStyle();
		}
		protected virtual void UpdateStyle() { }
		public virtual void LayoutChanged() { }
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		[Category("Appearance")]
#if !SL
	[DevExpressXtraBarsLocalizedDescription("NavigationFrameLookAndFeel")]
#endif
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeelCore; }
		}
		#endregion
		#region MVVM
		DevExpress.Utils.MVVM.Services.IDocumentAdapter DevExpress.Utils.MVVM.Services.IDocumentAdapterFactory.Create() {
			return new XtraBars.MVVM.Services.NavigationPageAdapter(this);
		}
		#endregion MVVM
		protected bool IsDesignMode {
			get { return Site != null && Site.DesignMode; }
		}
		int lockUpdate;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		bool DevExpress.Utils.Base.ISupportBatchUpdate.IsUpdateLocked {
			get { return lockUpdate > 0; }
		}
		void DevExpress.Utils.Base.ISupportBatchUpdate.BeginUpdate() {
			lockUpdate++;
		}
		void DevExpress.Utils.Base.ISupportBatchUpdate.CancelUpdate() {
			lockUpdate--;
		}
		void DevExpress.Utils.Base.ISupportBatchUpdate.EndUpdate() {
			if(--lockUpdate == 0)
				OnUnlockUpdate();
		}
		void INavigationFrame.UpdateMergedBarsAndRibbon() {
			UpdateMergedBarsAndRibbon();
		}
		bool INavigationFrame.AttachedToNavigator {
			get { return attachCounter != 0; }
		}
		protected virtual void OnUnlockUpdate() { }
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(DesignMode)
				DrawDesignTimeBorder(e.Graphics);
		}
		static Pen borderPen = new Pen(Color.Red) { DashPattern = new float[] { 5.0f, 5.0f } };
		protected void DrawDesignTimeBorder(Graphics graphics) {
			Rectangle rect = ClientRectangle;
			rect.Inflate(-1, -1);
			graphics.DrawRectangle(borderPen, rect);
		}
		#region RibbonMerging
		void MergeBarsAndRibbons(BarManager parentManager, BarManager childManager) {
			MergeBarsAndRibbonsCore(parentManager, childManager, CanMergeBarManager, CanMergeRibbon);
		}
		void UnmergeBarsAndRibbons(BarManager parentManager) {
			UnmergeBarsAndRibbonsCore(parentManager, CanMergeBarManager, CanMergeRibbon);
		}
		void RefreshRibbon(BarManager parentManager) {
			if(parentManager is Ribbon.RibbonBarManager) {
				var parentRibbon = ((Ribbon.RibbonBarManager)parentManager).Ribbon;
				if(parentRibbon != null && parentRibbon.GetMdiMergeStyle() == Ribbon.RibbonMdiMergeStyle.Always)
					parentRibbon.Refresh();
			}
		}
		void MergeBarsAndRibbonsCore(BarManager parentManager, BarManager childManager, Predicate<BarManager> canMergeBarManager, Predicate<Ribbon.RibbonControl> canMergeRibbon) {
			if(childManager != null && parentManager != null) {
				if(!childManager.Helper.LoadHelper.Loaded)
					childManager.Helper.LoadHelper.Load();
				if(parentManager is Ribbon.RibbonBarManager && childManager is Ribbon.RibbonBarManager) {
					var parentRibbon = ((Ribbon.RibbonBarManager)parentManager).Ribbon;
					var childRibbon = ((Ribbon.RibbonBarManager)childManager).Ribbon;
					if(parentRibbon != null && childRibbon != null) {
						if(canMergeRibbon(parentRibbon)) {
							if(!childRibbon.IsHandleCreated)
								childRibbon.CreateControl();
							var childForm = childManager.GetForm();
							parentRibbon.MergeRibbon(childRibbon);
						}
					}
				}
				else {
					if(canMergeBarManager(parentManager))
						parentManager.Helper.MdiHelper.MergeManager(childManager);
				}
				MergedChildManager = new WeakReference(childManager);
			}
			else MergedChildManager = null;
		}
		void UnmergeBarsAndRibbonsCore(BarManager parentManager, Predicate<BarManager> canMergeBarManager, Predicate<Ribbon.RibbonControl> canMergeRibbon) {
			if(parentManager != null) {
				if(parentManager is Ribbon.RibbonBarManager) {
					var parentRibbon = ((Ribbon.RibbonBarManager)parentManager).Ribbon;
					if(parentRibbon != null) {
						if(canMergeRibbon(parentRibbon))
							parentRibbon.UnMergeRibbon();
					}
				}
				else {
					if(canMergeBarManager(parentManager))
						parentManager.Helper.MdiHelper.UnMergeManager();
				}
			}
			MergedChildManager = null;
		}
		bool CanMergeBarManager(BarManager parentManager) {
			return parentManager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.Always || parentManager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.WhenChildActivated;
		}
		bool CanMergeRibbon(Ribbon.RibbonControl parentRibbon) {
			return parentRibbon.GetMdiMergeStyle() == Ribbon.RibbonMdiMergeStyle.Always;
		}
		bool AllowMerging() {
			return CanMergeOnSelectedPageChanged() && IsHandleCreated && !(Site != null && Site.DesignMode);
		}
		Ribbon.IRibbonMergingContext GetRibbonMergingContext() {
			if(!AllowMerging() || Parent == null) return null;
			return GetRibbonMergingContext(GetParentBarManager());
		}
		Ribbon.IRibbonMergingContext GetRibbonMergingContext(BarManager parentManager) {
			Ribbon.IRibbonMergingContext mergingContext = null;
			Ribbon.RibbonBarManager ribbonManager = parentManager as Ribbon.RibbonBarManager;
			if(ribbonManager != null)
				mergingContext = ribbonManager.Ribbon.CreateRibbonMergingContext();
			return mergingContext;
		}
		protected virtual void UpdateMergedBarsAndRibbon() {
			if(!AllowMerging()) return;
			using(GetRibbonMergingContext()) {
				OnDocumentDeactivated();
				BarManager parentManager = GetParentBarManager();
				MergedParentManager = new WeakReference(parentManager);
				if(parentManager != null && SelectedPageCore != null) {
					BarManager childManager = GetChildBarManager();
					if(childManager != null) {
						MergeBarsAndRibbons(parentManager, childManager);
						MergedChildManager = new WeakReference(childManager);
					}
				}
			}
		}
		void OnDocumentDeactivated() {
			if(!AllowMerging()) return;
			BarManager parentManager = GetParentBarManager();
			MergedParentManager = new WeakReference(parentManager);
			UnmergeBarsAndRibbons(parentManager);
		}
		BarManager GetParentBarManager() {
			if(!IsHandleCreated || Parent == null) return null;
			return BarManager.FindManager(Parent, false);
		}
		BarManager GetChildBarManager() {
			if(!IsHandleCreated || SelectedPageCore == null) return null;
			BarManager result = BarManager.FindManager(SelectedPageCore as Control, false);
			if(result == null) {
				foreach(Control item in (SelectedPageCore as Control).Controls) {
					result = BarManager.FindManager(item, false);
					if(result != null)
						break;
				}
			}
			return result;
		}
		WeakReference MergedParentManager;
		WeakReference MergedChildManager;
		BarManager GetMergedParentManager() {
			return (MergedParentManager != null) ? MergedParentManager.Target as BarManager : null;
		}
		BarManager GetMergedChildManager() {
			return (MergedChildManager != null) ? MergedChildManager.Target as BarManager : null;
		}
		#endregion
		#region INavigationBarClient Members
		IEnumerable<INavigationItem> INavigationBarClient.ItemsSource {
			get { return Pages; }
		}
		INavigationItem INavigationBarClient.SelectedItem {
			get { return SelectedPageCore as INavigationItem; }
			set { SelectedPageCore = value as INavigationPage; }
		}
		static readonly object itemsSourceChanged = new object();
		event EventHandler INavigationBarClient.ItemsSourceChanged {
			add { AddHandler(itemsSourceChanged, value); }
			remove { RemoveHandler(itemsSourceChanged, value); }
		}
		static readonly object selectedItemChanged = new object();
		event EventHandler INavigationBarClient.SelectedItemChanged {
			add { AddHandler(selectedItemChanged, value); }
			remove { RemoveHandler(selectedItemChanged, value); }
		}
		void AddHandler(object eventKey, EventHandler value) {
			Events.AddHandler(eventKey, value);
			if(value != null) Attach(value.Target as INavigationBar);
		}
		void RemoveHandler(object eventKey, EventHandler value) {
			Events.RemoveHandler(eventKey, value);
			if(value != null) Dettach(value.Target as INavigationBar);
		}
		protected internal void RaiseNavigationBarClientItemsSourceChanged() {
			EventHandler handler = (EventHandler)Events[itemsSourceChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected internal void RaiseNavigationBarClientSelectedItemChanged() {
			EventHandler handler = (EventHandler)Events[selectedItemChanged];
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		int attachCounter = 0;
		void Attach(INavigationBar navigationBar) {
			attachCounter++;
		}
		void Dettach(INavigationBar navigationBar) {
			attachCounter--;
		}
		#endregion
		#region Deferred Load
		public NavigationPageBase AddPage(Control content) {
			NavigationPageBase page = ResolveDeferredControlLoadPage(content)
				?? CreatePage(content);
			if(!page.IsControlLoaded && content != null)
				AddContentToPage(content, page);
			if(!Pages.Contains(page))
				Pages.Add(page);
			return page;
		}
		public NavigationPageBase AddPage(string caption, string controlName) {
			NavigationPageBase page = CreateNewPage();
			page.MarkAsDeferredControlLoad();
			page.Caption = caption;
			page.ControlName = controlName;
			return page;
		}
		protected NavigationPageBase CreatePage(Control control) {
			NavigationPageBase page = CreateNewPage();
			if(control != null)
				AddContentToPage(control, page);
			return page;
		}
		public virtual NavigationPageBase CreateNewPage() {
			return new NavigationPage();
		}
		void AddContentToPage(Control content, NavigationPageBase page) {
			page.MarkAsDeferredControlLoad();
			content.Dock = DockStyle.Fill;
			page.Controls.Add(content);
		}
		protected NavigationPage ResolveDeferredControlLoadPage(Control control) {
			NavigationPage result = null;
			if(result == null)
				result = Pages.FindFirst((page) => page.CanLoadControlByName(control.Name)) as NavigationPage;
			if(result == null)
				result = Pages.FindFirst((page) => page.CanLoadControlByType(control.GetType())) as NavigationPage;
			if(result == null)
				result = Pages.FindFirst((page) => page.CanLoadControl(control)) as NavigationPage;
			return result;
		}
		static readonly object queryControl = new object();
		[Category("Deferred Control Load Events")]
		public event QueryControlEventHandler QueryControl {
			add { Events.AddHandler(queryControl, value); }
			remove { Events.RemoveHandler(queryControl, value); }
		}
		static readonly object controlReleasing = new object();
		[Category("Deferred Control Load Events")]
		public event ControlReleasingEventHandler ControlReleasing {
			add { Events.AddHandler(controlReleasing, value); }
			remove { Events.RemoveHandler(controlReleasing, value); }
		}
		protected internal bool HasQueryControlSubscription {
			get { return !IsDisposing && Events[queryControl] != null; }
		}
		protected internal NavigationPageBase GetPage(Control control) {
			return Pages.FirstOrDefault(p => NavigationPageBase.GetControl(p) == control);
		}
		protected internal Control RaiseQueryControl(NavigationPageBase document) {
			QueryControlEventHandler handler = (QueryControlEventHandler)Events[queryControl];
			QueryControlEventArgs ea = new QueryControlEventArgs(document);
			if(handler != null)
				handler(this, ea);
			return ea.Control;
		}
		protected internal bool RaiseControlReleasing(NavigationPageBase document, out bool disposeControl) {
			return RaiseControlReleasing(document, true, true, out disposeControl);
		}
		protected internal bool RaiseControlReleasing(NavigationPageBase document, bool keepControl, bool disposeControl, out bool disposeControlResult) {
			ControlReleasingEventHandler handler = (ControlReleasingEventHandler)Events[controlReleasing];
			ControlReleasingEventArgs e = new ControlReleasingEventArgs(document, keepControl, disposeControl);
			if(handler != null)
				handler(this, e);
			disposeControlResult = e.DisposeControl;
			return !e.KeepControl;
		}
		#endregion Deferred Load
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TransitionAnimationProperties {
		IAnimationParameters defaultPropertiesCore;
		public TransitionAnimationProperties(IAnimationParameters defaultProperties) {
			defaultPropertiesCore = defaultProperties;
			AssignProperties(defaultPropertiesCore);
		}
		public int FrameCount { get; set; }
		bool ShouldSerializeFrameCount() {
			return FrameCount != defaultPropertiesCore.FrameCount;
		}
		void ResetFrameCount() {
			FrameCount = defaultPropertiesCore.FrameCount.GetValueOrDefault();
		}
		public int FrameInterval { get; set; }
		bool ShouldSerializeFrameInterval() {
			return FrameInterval != defaultPropertiesCore.FrameInterval.GetValueOrDefault();
		}
		void ResetFrameInterval() {
			FrameInterval = defaultPropertiesCore.FrameInterval.GetValueOrDefault();
		}
		public bool ShouldSerialize() {
			return ShouldSerializeFrameCount() || ShouldSerializeFrameInterval();
		}
		internal IAnimationParameters RetrieveTransitionAnimationParameters() {
			return new AnimationParameters(FrameInterval, FrameCount);
		}
		void AssignProperties(IAnimationParameters properties) {
			FrameCount = properties.FrameCount.GetValueOrDefault();
			FrameInterval = properties.FrameInterval.GetValueOrDefault();
		}
		public void Reset() {
			AssignProperties(defaultPropertiesCore);
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this, true);
		}
	}
}
namespace DevExpress.XtraBars.MVVM.Services {
	using System.Windows.Forms;
	using DevExpress.Utils.MVVM.Services;
	using DevExpress.XtraBars.Navigation;
	class NavigationPageAdapter : IDocumentAdapter, IDocumentContentProvider {
		NavigationPageBase navigationPage;
		NavigationFrame navigationFrame;
		public NavigationPageAdapter(NavigationFrame frame) {
			this.navigationFrame = frame;
			navigationFrame.PageRemoved += navigationFrame_PageRemoved;
			navigationFrame.PageClosing += navigationFrame_PageClosing;
		}
		public void Dispose() {
			var control = NavigationPage.GetControl(navigationPage);
			if(control != null)
				control.TextChanged -= control_TextChanged;
			navigationFrame.PageClosing -= navigationFrame_PageClosing;
			navigationFrame.PageRemoved -= navigationFrame_PageRemoved;
			navigationPage = null;
		}
		void navigationFrame_PageClosing(object sender, SelectedPageChangingEventArgs e) {
			if(e.OldPage == navigationPage)
				RaiseClosing(e);
		}
		void navigationFrame_PageRemoved(object sender, NavigationPageEventArgs e) {
			if(e.Page == navigationPage) {
				RaiseClosed(e);
				Dispose();
			}
		}
		void control_TextChanged(object sender, EventArgs e) {
			navigationPage.Text = ((Control)sender).Text;
		}
		void RaiseClosed(NavigationPageEventArgs e) {
			if(Closed != null) Closed(navigationFrame, e);
		}
		void RaiseClosing(SelectedPageChangingEventArgs e) {
			var ea = new CancelEventArgs(e.Cancel);
			if(Closing != null) Closing(navigationFrame, ea);
			e.Cancel = ea.Cancel;
		}
		public void Show(Control control) {
			var page = navigationFrame.GetPage(control);
			if(page == null) {
				navigationPage = navigationFrame.AddPage(control);
				navigationPage.Text = control.Text;
				control.TextChanged += control_TextChanged;
			}
			(navigationFrame as INavigationFrame).SelectedPage = navigationPage;
		}
		public void Close(Control control, bool force = true) {
			if(force)
				navigationFrame.PageClosing -= navigationFrame_PageClosing;
			if(control != null)
				control.TextChanged -= control_TextChanged;
			navigationPage.Dispose();
		}
		public event EventHandler Closed;
		public event CancelEventHandler Closing;
		public bool CanAddContent {
			get { return navigationFrame.HasQueryControlSubscription; }
		}
		public void AddContent(string caption, string viewName) {
			navigationPage = navigationFrame.AddPage(caption, viewName);
		}
		public object Resolve(string name, params object[] parameters) {
			if(navigationPage == null) return null;
			return navigationPage.EnsureIsBoundToControl(navigationFrame) ? 
				NavigationPage.GetControl(navigationPage) : null;
		}
	}
}

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
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#if DXWhidbey
using System.Windows.Forms.Design.Behavior;
#endif
using System.Drawing;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils;
using System.Collections;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace DevExpress.XtraBars.Design {
	public class NavigationFrameDesigner : BaseParentControlDesignerSimple {
		Control adornerWindowCore;
		NavigationFrameDesignerPanel panelCore;
		Timer timer;
		EnvDTE.Window ParentWindow;
		EnvDTE.DTE DTE;
		EnvDTE.WindowEvents WindowEvents;
		internal const string SelectedPageProperty = "SelectedPage";
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			PostFilterPropertiesCore(properties);
		}
		protected virtual void PostFilterPropertiesCore(IDictionary properties) {
			PropertyDescriptor pd = properties[SelectedPageProperty] as PropertyDescriptor;
			if(pd != null) properties[SelectedPageProperty] = TypeDescriptor.CreateProperty(typeof(NavigationFrameDesigner), pd, new Attribute[0]);
		}
		public virtual NavigationFrame NavigationFrame {
			get { return Control as NavigationFrame; }
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
		INavigationPage selectedPage = null;
		INavigationPage SelectedPage {
			get {
				if(selectedPage != null && !NavigationFrame.Pages.Contains(selectedPage as NavigationPageBase)) selectedPage = null;
				if(selectedPage == null) selectedPage = NavigationFrame.SelectedPage;
				return selectedPage;
			}
			set {
				NavigationFrame.SelectedPage = value as INavigationPage;
				selectedPage = value;
			}
		}
		public Control AdornerWindow {
			get { return adornerWindowCore; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			AddPage(NavigationFrame);
			INavigationPageBase page = AddPage(NavigationFrame);
			try {(NavigationFrame as INavigationFrame).SelectedPage = page; }
			finally { }
			NavigationFrame.Size = new Size(300, 150);
		}
		NavigationFrameDesignerPanel Panel { 
			get { return panelCore; } 
		}
		public override void Initialize(IComponent component) {
			DesignTimeHelper.UpdateDesignTimeLookAndFeel(component);
			base.Initialize(component);
			FieldInfo info = BehaviorService.GetType().GetField("adornerWindow", BindingFlags.Instance | BindingFlags.NonPublic);
			adornerWindowCore = info.GetValue(BehaviorService) as Control;
			CreateComponents();
			Subscribe(component);
			DTE = component.Site.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
			if(DTE != null)
				WindowEvents = DTE.Events.WindowEvents;
			SubscribeDTE(WindowEvents);
		}
		protected override void Dispose(bool disposing) {
			Unsubscribe();
			UnsubscribeDTE(WindowEvents);
			DestroyComponents();
			ParentWindow = null;
			DTE = null;
			WindowEvents = null;
			adornerWindowCore = null;
			selectionService = null;
			base.Dispose(disposing);
		}
		protected virtual void CreateComponents() {
			CreatePanel();
			CreateTimer();
		}
		protected virtual void DestroyComponents() {
			DestroyTimer();
			DestroyPanel();
		}
		void CreatePanel() {
			panelCore = new NavigationFrameDesignerPanel(NavigationFrame, this);
		}
		void DestroyPanel() {
			if(panelCore != null) {
				Panel.HideBeakForm(true);
				Panel.Dispose();
				panelCore = null;
			}
		}
		void CreateTimer() {
			timer = new Timer() { Interval = 50 };
		}
		void DestroyTimer() {
			if(timer != null) {
				timer.Stop();
				timer.Tick -= OnTick;
				timer.Dispose();
				timer = null;
			}
		}
		protected virtual void Subscribe(IComponent component) {
			if(BehaviorService != null) {
				BehaviorService.Synchronize += OnSynchronize;
				BehaviorService.EndDrag += OnEndDrag;
			}
			if(SelectionService != null)
				SelectionService.SelectionChanged += OnSelectionChanged;
			if(AdornerWindow != null) {
				AdornerWindow.MouseDown += OnAdornerWindowMouseDown;
				AdornerWindow.MouseUp += OnAdornerWindowMouseUp;
			}
			if(NavigationFrame != null) {
				NavigationFrame.SelectedPageChanged += OnSelectedPageChanged;
				NavigationFrame.DockChanged += OnSynchronize;
				NavigationFrame.Pages.CollectionChanged += OnPagesCollectionChanged;
			}
		}
		protected virtual void Unsubscribe() {
			if(BehaviorService != null) {
				BehaviorService.Synchronize -= OnSynchronize;
				BehaviorService.EndDrag -= OnEndDrag;
			}
			if(AdornerWindow != null) {
				AdornerWindow.MouseDown -= OnAdornerWindowMouseDown;
				AdornerWindow.MouseUp -= OnAdornerWindowMouseUp;
			}
			if(NavigationFrame != null) {
				NavigationFrame.DockChanged -= OnSynchronize;
				NavigationFrame.SelectedPageChanged -= OnSelectedPageChanged;
				if(NavigationFrame.Pages != null)
					NavigationFrame.Pages.CollectionChanged -= OnPagesCollectionChanged;
			}
			if(SelectionService != null)
				SelectionService.SelectionChanged -= OnSelectionChanged;
		}
		GCHandle rcwEventsHandle;
		void SubscribeDTE(EnvDTE.WindowEvents events) {
			if(rcwEventsHandle != null) {
				rcwEventsHandle = GCHandle.Alloc(events, GCHandleType.Normal);
				if(events != null) {
					events.WindowActivated += OnWindowActivated;
					events.WindowMoved += OnWindowMoved;
				}
			}
		}
		void UnsubscribeDTE(EnvDTE.WindowEvents events) {
			if(rcwEventsHandle != null && rcwEventsHandle.IsAllocated) {
				if(events != null) {
					events.WindowActivated -= OnWindowActivated;
					events.WindowMoved -= OnWindowMoved;
				}
				rcwEventsHandle.Free();
			}
		}
		void OnEndDrag(object sender, BehaviorDragDropEventArgs e) {
			if(IsSelected)
				SetPanelVisibility(true);
		}
		void OnWindowMoved(EnvDTE.Window Window, int Top, int Left, int Width, int Height) {
			if(Window != DTE.ActiveWindow) return;
			Panel.UpdateLocation();
		}
		void OnWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus) {
			if(ParentWindow == LostFocus)
				SetPanelVisibility(false);
			if(ParentWindow == GotFocus && IsSelected) {
				timer.Tick += OnTick;
				timer.Start();
			}
		}
		void OnTick(object sender, EventArgs e) {
			if(timer != null) {
				timer.Stop();
				timer.Tick -= OnTick;
			}
			SetPanelVisibility(true);
		}
		void OnSelectedPageChanged(object sender, SelectedPageChangedEventArgs e) {
			if(Panel.IsPageRemoving) return;
			Panel.UpdateCounterContent();
			Panel.UpdatePanelBounds();
		}
		void OnPagesCollectionChanged(Docking2010.Base.CollectionChangedEventArgs<NavigationPageBase> ea) {
			Panel.UpdateCounterContent();
			Panel.UpdatePanelBounds();
		}
		void OnAdornerWindowMouseUp(object sender, MouseEventArgs e) {
			if(IsSelected)
				SetPanelVisibility(true);
			Panel.UpdatePanelBounds();
		}
		protected virtual NavigationPageBase AddPage(NavigationFrame navigationFrame) {
			NavigationPage page = new NavigationPage();
			navigationFrame.Pages.Add(page);
			navigationFrame.Container.Add(page);
			page.Text = page.Name;
			return page;
		}
		protected virtual void OnAdornerWindowMouseDown(object sender, MouseEventArgs e) {
			SetPanelVisibility(false);
		}
		void OnSynchronize(object sender, EventArgs e) {
			Panel.UpdateLocation();
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			if(!IsSelected)
				SetPanelVisibility(false);
			else
				SetPanelVisibility(true);
		}
		protected internal void SetPanelVisibility(bool visible) {
			if(visible)
				ShowBeakForm();
			else
				Panel.HideBeakForm(true);
		}
		void ShowBeakForm() {
			Panel.UpdatePanelBounds();
			Panel.ShowBeakForm(Panel.GetLocation(), true, NavigationFrame);
			if(ParentWindow == null)
				ParentWindow = DTE.ActiveWindow;
		}
		Control GetDesignerFrame(Control control) {
			while(!(control is Form || control is UserControl)) {
				control = control.Parent;
				if(control == null)
					return null;
			}
			return control.Parent;
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Design.Images.{0}.png", name), typeof(NavigationFrameDesigner).Assembly);
		}
		public override bool CanParent(Control control) {
			return (control is NavigationPage);
		}
		ISelectionService selectionService;
		protected ISelectionService SelectionService {
			get {
				if(selectionService == null)
					selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
				return selectionService;
			}
		}
		protected internal bool IsSelected {
			get {
				if(SelectionService == null) return false;
				if(SelectionService.GetComponentSelected(NavigationFrame)) return true;
				return false;
			}
		}
	}
	public class TabPaneDesigner : NavigationPaneDesigner {
		protected override void PostFilterPropertiesCore(IDictionary properties) {
			PropertyDescriptor pd = properties[SelectedPageProperty] as PropertyDescriptor;
			if(pd != null) properties[SelectedPageProperty] = TypeDescriptor.CreateProperty(typeof(TabPaneDesigner), pd, new Attribute[0]); ;
		}
		TabNavigationPage selectedPage = null;
		TabNavigationPage SelectedPage {
			get {
				if(selectedPage != null && !NavigationFrame.Pages.Contains(selectedPage as NavigationPageBase)) selectedPage = null;
				if(selectedPage == null) selectedPage = (NavigationFrame as INavigationFrame).SelectedPage as TabNavigationPage;
				return selectedPage;
			}
			set {
				(NavigationFrame as INavigationFrame).SelectedPage = value as INavigationPageBase;
				selectedPage = value;
			}
		}
		protected override NavigationPageBase AddPage(NavigationFrame navigationFrame) {
			TabNavigationPage page = new TabNavigationPage();
			navigationFrame.Pages.Add(page);
			navigationFrame.Container.Add(page);
			page.Text = page.Name;
			return page;
		}
	}
	public class NavigationPaneDesigner : NavigationFrameDesigner {
		Adorner pageNavigationAdorner;
		NavigationPaneGlyph paneGlyph;
		NavigationPageGlyph pageGlyph;
		public NavigationPane NavigationPane {
			get { return Component as NavigationPane; }
		}
		protected override void CreateComponents() {
			base.CreateComponents();
			pageNavigationAdorner = new Adorner();
			paneGlyph = new NavigationPaneGlyph(NavigationPane, AdornerWindow);
			pageGlyph = new NavigationPageGlyph(NavigationPane, AdornerWindow);
			BehaviorService.Adorners.Add(pageNavigationAdorner);
			pageNavigationAdorner.Glyphs.Add(paneGlyph);
			pageNavigationAdorner.Glyphs.Add(pageGlyph);
		}
		protected override void DestroyComponents() {
			pageNavigationAdorner.Glyphs.Remove(paneGlyph);
			pageNavigationAdorner.Glyphs.Remove(pageGlyph);
			BehaviorService.Adorners.Remove(pageNavigationAdorner);
			base.DestroyComponents();
		}
		protected override void Subscribe(IComponent component) {
			base.Subscribe(component);
			if(NavigationPane != null)
				NavigationPane.StateChanged += OnNavigationPaneStateChanged;
		}
		protected override void Unsubscribe() {
			if(NavigationPane != null)
				NavigationPane.StateChanged -= OnNavigationPaneStateChanged;
			base.Unsubscribe();
		}
		protected override void OnAdornerWindowMouseDown(object sender, MouseEventArgs e) {
			if((paneGlyph != null) && !paneGlyph.Bounds.Contains(e.Location))
				base.OnAdornerWindowMouseDown(sender, e);
		}
		void OnNavigationPaneStateChanged(object sender, StateChangedEventArgs e) {
			BehaviorService.SyncSelection();
		}
	}
	public class NavigationPaneGlyph : Glyph {
		NavigationPane navigationPane;
		WeakReference adornerWindowWeakReference;
		public NavigationPaneGlyph(NavigationPane pane, Control adornerWindow)
			: base(null) {
			adornerWindowWeakReference = new WeakReference(adornerWindow);
			navigationPane = pane;
		}
		Behavior behaviorCore;
		public override Rectangle Bounds {
			get {
				if(navigationPane != null) {
					Point clientLocation = new Point(navigationPane.Location.X + navigationPane.ButtonsPanel.Bounds.X,
						navigationPane.Location.Y + navigationPane.ButtonsPanel.Bounds.Y);
					Point newLocation = Point.Empty;
					Control target = adornerWindowWeakReference.Target as Control;
					if(target != null)
						newLocation = target.PointToClient(navigationPane.Parent.PointToScreen(clientLocation));
					Rectangle newBounds = new Rectangle(newLocation, navigationPane.ButtonsPanel.Bounds.Size);
					return newBounds;
				}
				return base.Bounds;
			}
		}
		public Rectangle ExcludedRectangle {
			get {
				int excludedRectangleWidth = 15;
				int excludedRectangleHeight = 8;
				int excludedRectangleOffset = 7;
				Size size = new Size(excludedRectangleWidth, excludedRectangleHeight);
				Point location = new Point(Bounds.X + excludedRectangleOffset, Bounds.Y);
				return new Rectangle(location, size);
			}
		}
		public override Behavior Behavior {
			get {
				Control target = adornerWindowWeakReference.Target as Control;
				if(behaviorCore == null && target != null)
					behaviorCore = new NavigationPaneGlyphBehavior(navigationPane, target);
				return behaviorCore;
			}
		}
		public override Cursor GetHitTest(Point p) {
			if(ProjectHelper.IsDebuggerLaunched(navigationPane.Site)) return null;
			if(navigationPane != null && Bounds.Contains(p) && !ExcludedRectangle.Contains(p))
				return Cursors.Default;
			return null;
		}
		public override void Paint(PaintEventArgs pe) { }
	}
	public class NavigationPageGlyph : Glyph {
		NavigationPane navigationPane;
		WeakReference adornerWindowWeakReference;
		public NavigationPageGlyph(NavigationPane pane, Control adornerWindow)
			: base(null) {
			adornerWindowWeakReference = new WeakReference(adornerWindow);
			navigationPane = pane as NavigationPane;
		}
		Behavior behaviorCore;
		public override Rectangle Bounds {
			get {
				INavigationPage selectedPage = navigationPane != null ? navigationPane.SelectedPage as INavigationPage : null;
				if(selectedPage != null && selectedPage.ButtonsPanel != null) {
					Point clientLocation = new Point(navigationPane.Location.X + selectedPage.ButtonsPanel.Bounds.X,
						navigationPane.Location.Y + selectedPage.ButtonsPanel.Bounds.Y);
					Point newLocation = Point.Empty;
					Control target = adornerWindowWeakReference.Target as Control;
					if(target != null)
						 newLocation = target.PointToClient(navigationPane.Parent.PointToScreen(clientLocation));
					Rectangle newBounds = new Rectangle(newLocation, selectedPage.ButtonsPanel.Bounds.Size);
					return newBounds;
				}
				return base.Bounds;
			}
		}
		public override Behavior Behavior {
			get {
				Control target = adornerWindowWeakReference.Target as Control;
				if(behaviorCore == null && target != null)
					behaviorCore = new NavigationPageGlyphBehavior(navigationPane, target);
				return behaviorCore;
			}
		}
		public override Cursor GetHitTest(Point p) {
			if(ProjectHelper.IsDebuggerLaunched(navigationPane.Site)) return null;
			if(navigationPane != null && Bounds.Contains(p))
				return Cursors.Default;
			return null;
		}
		public override void Paint(PaintEventArgs pe) { }
	}
	public class NavigationPaneGlyphBehavior : BaseBehavior {
		WeakReference adornerWindowWeakReference;
		public NavigationPaneGlyphBehavior(Component component, object adornerWindow)
			: base(component) {
			adornerWindowWeakReference = new WeakReference(adornerWindow);
		}
		public NavigationPane NavigationPane { 
			get { return Component as NavigationPane; } 
		}
		ISelectionService selectionServiceCore = null;
		protected ISelectionService SelectionService {
			get {
				if(selectionServiceCore == null)
					selectionServiceCore = GetService(typeof(ISelectionService)) as ISelectionService;
				return selectionServiceCore;
			}
		}
		Point GetCorrectLocation(Point point) {
			Point clientLocation = new Point();
			Control target = adornerWindowWeakReference.Target as Control;
			if(target != null)
				clientLocation = target.PointToClient(NavigationPane.Parent.PointToScreen((NavigationPane.Parent).ClientRectangle.Location));
			Point newLocation = new Point(NavigationPane.Location.X + clientLocation.X,
										  NavigationPane.Location.Y + clientLocation.Y);
			point = new Point(point.X - newLocation.X, point.Y - newLocation.Y);
			return point;
		}
		public override bool OnMouseLeave(Glyph g) {
			if(NavigationPane.ButtonsPanel != null)
				NavigationPane.ButtonsPanel.Handler.OnMouseLeave();
			return base.OnMouseLeave(g);
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) {
			mouseLoc = GetCorrectLocation(mouseLoc);
			if(NavigationPane.ButtonsPanel != null)
				NavigationPane.ButtonsPanel.Handler.OnMouseMove(new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
			savedPosition = mouseLoc;
			return base.OnMouseMove(g, button, mouseLoc);
		}
		Point savedPosition;
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			mouseLoc = GetCorrectLocation(mouseLoc);
			if(NavigationPane.ButtonsPanel != null)
				NavigationPane.ButtonsPanel.Handler.OnMouseDown(new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
			return base.OnMouseDown(g, button, mouseLoc);
		}
		public override bool OnMouseUp(Glyph g, MouseButtons button) {
			SelectionService.SetSelectedComponents(new Component[] { NavigationPane }, SelectionTypes.Auto);
			if(NavigationPane.ButtonsPanel != null)
				NavigationPane.ButtonsPanel.Handler.OnMouseUp(new MouseEventArgs(button, 1, savedPosition.X, savedPosition.Y, 0));
			return base.OnMouseUp(g, button);
		}
	}
	public class NavigationPageGlyphBehavior : BaseBehavior {
		WeakReference adornerWindowWeakReference;
		public NavigationPageGlyphBehavior(Component component, object adornerWindow)
			: base(component) {
			adornerWindowWeakReference = new WeakReference(adornerWindow);
		}
		public NavigationPane NavigationPane { 
			get { return Component as NavigationPane; } 
		}
		ISelectionService selectionServiceCore = null;
		protected ISelectionService SelectionService {
			get {
				if(selectionServiceCore == null) {
					selectionServiceCore = GetService(typeof(ISelectionService)) as ISelectionService;
				}
				return selectionServiceCore;
			}
		}
		Point GetCorrectLocation(Point point) {
			Point clientLocation = new Point();
			Control target = adornerWindowWeakReference.Target as Control;
			if(target != null)
				clientLocation = target.PointToClient(NavigationPane.Parent.PointToScreen((NavigationPane.Parent).ClientRectangle.Location));
			Point newLocation = new Point(NavigationPane.Location.X + clientLocation.X,
										  NavigationPane.Location.Y + clientLocation.Y);
			point = new Point(point.X - newLocation.X, point.Y - newLocation.Y);
			return point;
		}
		public override bool OnMouseEnter(Glyph g) {
			return base.OnMouseEnter(g);
		}
		public override bool OnMouseLeave(Glyph g) {
			INavigationPage selectedPage = NavigationPane != null ? NavigationPane.SelectedPage as INavigationPage : null;
			if(selectedPage != null)
				selectedPage.ButtonsPanel.Handler.OnMouseLeave();
			return base.OnMouseLeave(g);
		}
		public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) {
			INavigationPage selectedPage = NavigationPane != null ? NavigationPane.SelectedPage as INavigationPage : null;
			mouseLoc = GetCorrectLocation(mouseLoc);
			if(selectedPage != null)
				selectedPage.ButtonsPanel.Handler.OnMouseMove(new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
			return base.OnMouseMove(g, button, mouseLoc);
		}
		public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc) {
			INavigationPage selectedPage = NavigationPane != null ? NavigationPane.SelectedPage as INavigationPage : null;
			mouseLoc = GetCorrectLocation(mouseLoc);
			if(selectedPage != null) {
				selectedPage.ButtonsPanel.Handler.OnMouseDown(new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
				selectedPage.ButtonsPanel.Handler.OnMouseUp(new MouseEventArgs(button, 1, mouseLoc.X, mouseLoc.Y, 0));
			}
			return base.OnMouseDown(g, button, mouseLoc);
		}
	}
	public enum NavigationPageWorkingMode { InNavigationFrame, InNavigationFrameWithOfficeNavigationBar }
	public class NavigationPageUnexpectedPropertiesProvider {
		static Dictionary<NavigationPageWorkingMode, string[]> unexpectedProperties;
		static NavigationPageUnexpectedPropertiesProvider() {
			unexpectedProperties = new Dictionary<NavigationPageWorkingMode, string[]>();
			unexpectedProperties.Add(NavigationPageWorkingMode.InNavigationFrame, new string[] { 
				"PageVisible", "CustomHeaderButtons", "PageText", "Appearance", "Caption", "Properties", "Image"});
			unexpectedProperties.Add(NavigationPageWorkingMode.InNavigationFrameWithOfficeNavigationBar, new string[] { 
				"PageVisible", "CustomHeaderButtons", "PageText", "Appearance", "Properties"});
		}
		public static string[] GetProperties(NavigationPageWorkingMode workingMode) {
			return unexpectedProperties[workingMode];
		}
		public static void PrefilterProperties(IDictionary properties, NavigationPageWorkingMode workingMode) {
			string[] names = GetProperties(workingMode);
			ArrayList propertiesToRemove = new ArrayList();
			foreach(DictionaryEntry entry in properties) {
				bool found = false;
				foreach(string name in names) {
					if((string)entry.Key == name) {
						found = true;
						break;
					}
				}
				if(found) propertiesToRemove.Add(entry.Key);
			}
			foreach(object key in propertiesToRemove) properties.Remove(key);
		}
	}
	public class NavigationPageDesigner : BaseScrollableControlDesigner {
		public override SelectionRules SelectionRules {
			get { return SelectionRules.Locked; }
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(Control.Parent == null || NavigationPane != null) return;
			var navigationFrame = Control.Parent as INavigationFrame;
			if(navigationFrame == null) return;
			NavigationPageWorkingMode workingMode = navigationFrame.AttachedToNavigator ?
				NavigationPageWorkingMode.InNavigationFrameWithOfficeNavigationBar : NavigationPageWorkingMode.InNavigationFrame;
			NavigationPageUnexpectedPropertiesProvider.PrefilterProperties(properties, workingMode);
		}
		public override void Initialize(IComponent component) {
			bool prevVisible = ((Control)component).Visible;
			base.Initialize(component);
			Control.Visible = prevVisible;
			if(SelectionService != null) SelectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				if(SelectionService != null) SelectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
			base.Dispose(disposing);
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
		bool prevSelected = false;
		protected void OnSelectionChanged(object sender, EventArgs e) {
			if(Control == null) return;
			if(this.prevSelected != IsSelected) {
				this.prevSelected = IsSelected;
				Control.Invalidate();
				if(IsSelected && NavigationPane != null)
					(NavigationPane as INavigationFrame).SelectedPage = Page;
			}
		}
		public virtual NavigationPageBase Page {
			get { return Control as NavigationPageBase; }
		}
		public virtual NavigationPane NavigationPane {
			get {
				if(Page != null) return Page.Owner as NavigationPane;
				return null;
			}
		}
		protected ISelectionService SelectionService {
			get { return GetService(typeof(ISelectionService)) as ISelectionService; }
		}
		protected bool IsSelected {
			get {
				if(SelectionService == null) return false;
				if(SelectionService.GetComponentSelected(Control)) return true;
				return false;
			}
		}
		protected override void OnPaintAdornments(PaintEventArgs pe) {
			Rectangle outside = Control.ClientRectangle, inside;
			inside = outside;
			inside.Inflate(-3, -3);
			if(IsSelected)
				ControlPaint.DrawSelectionFrame(pe.Graphics, true, outside, inside, SystemColors.Control);
			base.OnPaintAdornments(pe);
		}
	}
	public class RemovePageButton : WindowsUIButton { }
	public class PageMenuButton : WindowsUIButton { }
	public class NavigationFrameDesignerPanel : FlyoutPanel, DevExpress.Utils.Base.ISupportBatchUpdate {
		NavigationButtonPanel buttonPanelCore;
		PageMenuButton pageCounterButtonCore;
		RemovePageButton removeButtonCore;
		NavigationFrame navigationFrameCore;
		NavigationFrameDesigner designerCore;
		NavigationFrame NavigationFrame { get { return navigationFrameCore; } }
		public NavigationFrameDesignerPanel(NavigationFrame navigationFrame, NavigationFrameDesigner designer) {
			designerCore = designer;
			navigationFrameCore = navigationFrame;
			InitButtonPanelControl();
			OptionsBeakPanel.BeakLocation = BeakPanelBeakLocation.Top;
			OptionsBeakPanel.CloseOnOuterClick = false;
			BackColor = LookAndFeelHelper.GetSystemColorEx(LookAndFeel.ActiveLookAndFeel, SystemColors.Window);
			LookAndFeel.StyleChanged += OnLookAndFeelStyleChanged;
		}
		public PageMenuButton PageCounterButton { get { return pageCounterButtonCore; } }
		protected override void Dispose(bool disposing) {
			buttonPanelCore.ButtonClick -= OnButtonClick;
			LookAndFeel.StyleChanged -= OnLookAndFeelStyleChanged;
			base.Dispose(disposing);
		}
		public RemovePageButton RemoveButton { get { return removeButtonCore; } }
		public NavigationButtonPanel NavigationButtonPanel { get { return buttonPanelCore; } }
		internal void InitButtonPanelControl() {
			buttonPanelCore = new NavigationButtonPanel();
			buttonPanelCore.AllowGlyphSkinning = true;
			buttonPanelCore.UseButtonBackgroundImages = false;
			var separator = new WindowsUISeparator() { Image = LoadImageFromResources("Separator") };
			buttonPanelCore.Buttons.AddRange(new IBaseButton[] {
						new WindowsUIButton() { Caption = "Prev", 
							UseCaption = false, 
							Image = LoadImageFromResources("Prev_21x21")},
						pageCounterButtonCore = new PageMenuButton() {UseCaption = true},
						new WindowsUIButton() { Caption = "Next", 
							UseCaption = false, 
							Image =  LoadImageFromResources("Next_21x21"),
						},
						separator,
						new WindowsUIButton() { Caption = "Add", 
							UseCaption = false, 
							Image =  LoadImageFromResources("Add_21x21")},
						removeButtonCore = new RemovePageButton() { Caption = "Remove", 
							UseCaption = false, 
							Image =  LoadImageFromResources("Remove_21x21")},
					});
			buttonPanelCore.Buttons.Owner.BeginUpdate();
			removeButtonCore.Appearance.ForeColor = CommonSkins.GetSkin(NavigationFrame.LookAndFeel).Colors.GetColor("Critical");
			pageCounterButtonCore.Appearance.Font = new Font("Segoe UI", 10f);
			UpdateCounterContent();
			buttonPanelCore.Buttons.Owner.CancelUpdate();
			buttonPanelCore.ButtonClick += OnButtonClick;
			buttonPanelCore.Dock = DockStyle.Fill;
			buttonPanelCore.ButtonInterval = 2;
			this.Controls.Add(buttonPanelCore);
		}
		protected internal void UpdatePanelBounds() {
			Size newSize = GetPanelSize();
			if(Size == newSize)
				return;
			Size = newSize;
			UpdateLocation();
		}
		protected internal void UpdateLocation() {
			BeginUpdate();
			OptionsBeakPanel.ScreenFormLocation = GetLocation();
			EndUpdate();
		}
		public Size GetPanelSize() {
			if(NavigationFrame != null) {
				Size buttonsSize = Size.Empty;
				using(Graphics g = CreateGraphics()) {
					buttonsSize = NavigationButtonPanel.Buttons.Owner.ViewInfo.CalcMinSize(g);
				}
				int horzPadding = 16;
				int vertPadding = 8;
				Size panelSize = new Size(buttonsSize.Width + horzPadding,
					buttonsSize.Height + vertPadding);
				return panelSize;
			}
			return new Size();
		}
		public Point GetLocation() {
			Point clientLocation = new Point(NavigationFrame.Bounds.X + NavigationFrame.Width / 2, NavigationFrame.Bounds.Y + NavigationFrame.Height + 5);
			if(NavigationFrame != null && NavigationFrame.Parent != null) {
				Point result = NavigationFrame.Parent.PointToScreen(clientLocation);
				return NavigationFrame.Parent.PointToScreen(clientLocation);
			}
			return Point.Empty;
		}
		void OnButtonClick(object sender, BaseButtonEventArgs e) {
			if(!(e.Button is WindowsUIButton) || ProjectHelper.IsDebuggerLaunched(NavigationFrame.Site)) return;
			NavigationPageBase selectedPage = (NavigationFrame as INavigationFrame).SelectedPage as NavigationPageBase;
			int currentPageIndex = NavigationFrame.Pages.IndexOf(selectedPage);
			switch(e.Button.Properties.Caption) {
				case "Prev":
					if(NavigationFrame.Pages.Count != 0)
						NavigationFrame.SelectPrevPage();
					break;
				case "Next":
					if(NavigationFrame.Pages.Count != 0)
						NavigationFrame.SelectNextPage();
					break;
				case "Add":
					if(NavigationFrame != null) {
						NavigationPageBase page = AddPage(NavigationFrame);
						try { (NavigationFrame as INavigationFrame).SelectedPage = page; }
						finally { }
						UpdateCounterContent();
					}
					break;
				case "Remove":
					if(selectedPage == null) return;
					if(currentPageIndex + 1 != NavigationFrame.Pages.Count) {
						pageRemoving = true;
						NavigationFrame.SelectNextPage();
					}
					else if(NavigationFrame.Pages.Count != 1)
						NavigationFrame.SelectPrevPage();
					pageRemoving = false;
					NavigationFrame.Container.Remove(selectedPage);
					selectedPage.Dispose();
					break;
				default:
					EditorContextHelper.EditValue(designerCore, NavigationFrame, "Pages");
					break;
			}
			UpdatePanelBounds();
		}
		bool pageRemoving;
		protected internal bool IsPageRemoving { get { return pageRemoving; } }
		protected internal void UpdateCounterContent() {
			string counterContent = String.Format("{0} of {1}", NavigationFrame.Pages.IndexOf((NavigationFrame as INavigationFrame).SelectedPage as NavigationPageBase) + 1, NavigationFrame.Pages.Count);
			PageCounterButton.Caption = counterContent;
		}
		protected virtual NavigationPageBase AddPage(NavigationFrame navigationFrame) {
			NavigationPageBase page = navigationFrame.CreateNewPage();
			page.Text = page.Name;
			navigationFrame.Pages.Add(page);
			navigationFrame.Container.Add(page);
			return page;
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.XtraBars.Design.Images.{0}.png", name), typeof(NavigationFrameDesigner).Assembly);
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			BackColor = LookAndFeelHelper.GetSystemColorEx(LookAndFeel.ActiveLookAndFeel, SystemColors.Control);
			RemoveButton.Appearance.ForeColor = CommonSkins.GetSkin(NavigationFrame.LookAndFeel).Colors.GetColor("Critical"); ;
		}
		protected override bool DoubleBuffered {
			get { return true; }
		}
		protected override void OnSizeChanged(System.EventArgs e) {
			if(IsUpdateLocked) return;
			base.OnSizeChanged(e);
		}
		protected override BeakPanelOptions CreateBeakPanelOptions() { return new NavigationFrameDesignerBeakPanelOptions(this); }
		protected override FlyoutPanelToolForm CreateBeakFormCore(System.Windows.Forms.Control owner, FlyoutPanel content, Point loc, Point offset) {
			if(loc.IsEmpty) loc = OptionsBeakPanel.ScreenFormLocation;
			NavigationFrameDesignerBeakFlyoutPanelOptions beakOptions = new NavigationFrameDesignerBeakFlyoutPanelOptions(loc);
			beakOptions.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.Manual;
			return new NavigationFlyoutPanelBeakForm(owner, content, beakOptions);
		}
		public new NavigationFrameDesignerBeakPanelOptions OptionsBeakPanel { get { return base.OptionsBeakPanel as NavigationFrameDesignerBeakPanelOptions; } }
		#region IBatchUpdateable Members
		int isUpdateLocked = 0;
		public void CancelUpdate() {
			isUpdateLocked = 0;
			EndUpdate();
		}
		public void EndUpdate() {
			if(--isUpdateLocked == 0 && FlyoutPanelState.IsActive) {
				((NavigationFrameDesignerBeakFlyoutPanelOptions)FlyoutPanelState.Form.Options).ScreenFormLocation = OptionsBeakPanel.ScreenFormLocation;
				OnSizeChanged(System.EventArgs.Empty);
				FlyoutPanelState.Form.UpdateLocation();
			}
		}
		public bool IsUpdateLocked { get { return isUpdateLocked > 0; } }
		public void BeginUpdate() { isUpdateLocked++; }
		#endregion
	}
	public class NavigationFrameDesignerBeakFlyoutPanelOptions : BeakFlyoutPanelOptions {
		public NavigationFrameDesignerBeakFlyoutPanelOptions(Point location) : base(location) { }
		public new Point Offset {
			get { return base.Offset; }
			set { base.Offset = value; }
		}
		public new Point ScreenFormLocation {
			get { return base.ScreenFormLocation; }
			set { base.ScreenFormLocation = value; }
		}
	}
	public class NavigationFrameDesignerBeakPanelOptions : BeakPanelOptions {
		Point screenFormLocation;
		public NavigationFrameDesignerBeakPanelOptions(FlyoutPanel owner) : base(owner) { }
		public override Color BackColor {
			get { return Owner.Appearance.BackColor; }
			set { }
		}
		public override Color BorderColor {
			get { return Owner.Appearance.ForeColor; }
			set { }
		}
		Point prev = Point.Empty;
		public Point ScreenFormLocation {
			get { return screenFormLocation; }
			set {
				if(ScreenFormLocation == value)
					return;
				prev = ScreenFormLocation;
				screenFormLocation = value;
				OnChanged("ScreenFormLocation", prev, ScreenFormLocation);
			}
		}
	}
	public class NavigationFlyoutPanelBeakForm : FlyoutPanelBeakForm {
		public NavigationFlyoutPanelBeakForm(Control owner, FlyoutPanel flyoutPanel, FlyoutPanelOptions options)
			: base(owner, flyoutPanel, options) {
		}
		protected override bool IsParentFormRequired {
			get { return false; }
		}
		protected override bool CanDisplayFlyout(Form form) {
			return true;
		}
	}
	public class NavigationButtonPanel : WindowsUIButtonPanel, IButtonsPanelOwner {
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(IsSkinPaintStyle)
				return new NavigationButtonPanelSkinPainter(LookAndFeel);
			return new WindowsUIButtonsPanelPainter();
		}
	}
	public class NavigationButtonPanelSkinPainter : WindowsUIButtonsPanelSkinPainter {
		public NavigationButtonPanelSkinPainter(UserLookAndFeel lookAndFeel) : base(lookAndFeel) { }
		public override BaseButtonPainter GetButtonPainter() {
			return new NavigationButtonSkinPainter(Provider);
		}
	}
	public class NavigationButtonSkinPainter : WindowsUIButtonSkinPainter {
		ISkinProvider skinProvider;
		public NavigationButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
			skinProvider = provider;
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			Rectangle rect = info.Bounds;
			Skin skin = CommonSkins.GetSkin(skinProvider);
			if(!(info.Button is PageMenuButton)) {
				SkinElementInfo elementInfo = new SkinElementInfo(skin[CommonSkins.SkinLayoutItemBackground], rect);
				if(info.Hot)
					elementInfo.ImageIndex = 1;
				if(info.Pressed & info.Hot || info.Pressed)
					elementInfo.ImageIndex = 2;
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
			}
		}
		protected override Color GetInvertedColor() {
			return GetForegroundColor(0);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle result = base.CalcBoundsByClientRectangle(e, client);
			result.Inflate(5, 5);
			return result;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle result = base.GetObjectClientRectangle(e);
			result.Inflate(-5, -5);
			return result;
		}
	}
	public class NavigationPageBaseCollectionEditor : DXCollectionEditorBase {
		public NavigationPageBaseCollectionEditor(Type type)
			: base(type) {
		}
		protected override Type[] CreateNewItemTypes() {
			if(Context.Instance is TabPane) {
				return new Type[] { typeof(TabNavigationPage) };
			}
			else {
				return new Type[] { typeof(NavigationPage) };
			}
		}
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
	}
}

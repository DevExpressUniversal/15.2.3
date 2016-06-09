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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Platform {
	[TemplatePart(Name = "PART_Dock", Type = typeof(DockHintButton))]
	[TemplatePart(Name = "PART_Hide", Type = typeof(DockHintButton))]
	public class SideDockHintElement : DockHintElement {
		#region static
		public static readonly DependencyProperty LeftTemplateProperty;
		public static readonly DependencyProperty RightTemplateProperty;
		public static readonly DependencyProperty TopTemplateProperty;
		public static readonly DependencyProperty BottomTemplateProperty;
		static SideDockHintElement() {
			var dProp = new DependencyPropertyRegistrator<SideDockHintElement>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("LeftTemplate", ref LeftTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((SideDockHintElement)dObj).OnHintTemplateChanged());
			dProp.Register("RightTemplate", ref RightTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((SideDockHintElement)dObj).OnHintTemplateChanged());
			dProp.Register("TopTemplate", ref TopTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((SideDockHintElement)dObj).OnHintTemplateChanged());
			dProp.Register("BottomTemplate", ref BottomTemplateProperty, (ControlTemplate)null,
				(dObj, e) => ((SideDockHintElement)dObj).OnHintTemplateChanged());
		}
		#endregion
		public ControlTemplate LeftTemplate {
			get { return (ControlTemplate)GetValue(LeftTemplateProperty); }
			set { SetValue(LeftTemplateProperty, value); }
		}
		public ControlTemplate RightTemplate {
			get { return (ControlTemplate)GetValue(RightTemplateProperty); }
			set { SetValue(RightTemplateProperty, value); }
		}
		public ControlTemplate TopTemplate {
			get { return (ControlTemplate)GetValue(TopTemplateProperty); }
			set { SetValue(TopTemplateProperty, value); }
		}
		public ControlTemplate BottomTemplate {
			get { return (ControlTemplate)GetValue(BottomTemplateProperty); }
			set { SetValue(BottomTemplateProperty, value); }
		}
		protected virtual void OnHintTemplateChanged() {
			if(LeftTemplate != null && RightTemplate != null && TopTemplate != null && BottomTemplate != null)
				SelectTemplate(Type);
		}
		public SideDockHintElement(DockVisualizerElement type)
			: base(type) {
			IsHorizontal = Type == DockVisualizerElement.Left || Type == DockVisualizerElement.Right;
		}
		public override void UpdateState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			UpdateAvailableState(configuration);
			UpdateEnabledState(configuration);
		}
		public override void UpdateAvailableState(bool dock, bool hide, bool fill) {
			if(DockButton != null)
				DockButton.IsAvailable = dock;
			if(HideButton != null) {
				HideButton.IsAvailable = hide;
				HideButton.Visibility = hide ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		public void UpdateAvailableState(DockHintsConfiguration configuration) {
			bool dock = configuration.GetSideIsEnabled(Type);
			bool hide = configuration.GetAutoHideIsEnabled(Type);
			if(DockButton != null)
				DockButton.IsAvailable = dock;
			if(HideButton != null) {
				HideButton.IsAvailable = hide;
				HideButton.Visibility = hide ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		public override void UpdateAvailableState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			UpdateAvailableState(configuration);
		}
		private void UpdateEnabledState(DockHintsConfiguration configuration) {
			bool dock = configuration.GetSideIsVisible(Type);
			bool hide = configuration.GetAutoHideIsVisible(Type);
			if(DockButton != null)
				DockButton.IsEnabled = dock;
			if(HideButton != null)
				HideButton.IsEnabled = hide;
		}
		public override void UpdateEnabledState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			UpdateEnabledState(configuration);
		}
		protected DockHintButton DockButton { get; private set; }
		protected DockHintButton HideButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DockButton = GetTemplateChild("PART_Dock") as DockHintButton;
			HideButton = GetTemplateChild("PART_Hide") as DockHintButton;
		}
		public override void UpdateHotTrack(DockHintButton hotButton) {
			UpdateHotTrack(DockButton, hotButton);
			UpdateHotTrack(HideButton, hotButton);
		}
		public bool IsHorizontal { get; private set; }
		protected override bool CalcVisibileState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			return configuration.GetGuideIsVisible(Type) && configuration.ShowSideDockHints;
		}
		protected override Rect CalcBounds(DockingHintAdorner adorner) {
			return adorner.SurfaceRect;
		}
		protected override void OnInitialized() {
			base.OnInitialized();
			SelectTemplate(Type);
		}
		protected virtual void SelectTemplate(DockVisualizerElement type) {
			if(IsDisposing) return;
			switch(type) {
				case DockVisualizerElement.Left: Template = LeftTemplate; break;
				case DockVisualizerElement.Right: Template = RightTemplate; break;
				case DockVisualizerElement.Top: Template = TopTemplate; break;
				case DockVisualizerElement.Bottom: Template = BottomTemplate; break;
				default: Template = null; break;
			}
		}
	}
	[TemplatePart(Name = "PART_Left", Type = typeof(DockHintButton))]
	[TemplatePart(Name = "PART_Right", Type = typeof(DockHintButton))]
	[TemplatePart(Name = "PART_Top", Type = typeof(DockHintButton))]
	[TemplatePart(Name = "PART_Bottom", Type = typeof(DockHintButton))]
	[TemplatePart(Name = "PART_Fill", Type = typeof(DockHintButton))]
	public class CenterDockHintElement : DockHintElement {
		public static readonly DependencyProperty DockHintsVisibilityProperty;
		public static readonly DependencyProperty TabHintsVisibilityProperty;
		static CenterDockHintElement() {
			var dProp = new DependencyPropertyRegistrator<CenterDockHintElement>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("DockHintsVisibility", ref DockHintsVisibilityProperty, Visibility.Visible,
				(dObj, e)=>((CenterDockHintElement)dObj).OnDockHintsVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue));
			dProp.Register("TabHintsVisibility", ref TabHintsVisibilityProperty, Visibility.Visible, 
				(dObj, e)=>((CenterDockHintElement)dObj).OnTabHintsVisibilityChanged((Visibility)e.OldValue, (Visibility)e.NewValue));
		}
		public CenterDockHintElement()
			: base(DockVisualizerElement.Center) {
		}
		public Visibility DockHintsVisibility {
			get { return (Visibility)GetValue(DockHintsVisibilityProperty); }
			set { SetValue(DockHintsVisibilityProperty, value); }
		}
		public Visibility TabHintsVisibility {
			get { return (Visibility)GetValue(TabHintsVisibilityProperty); }
			set { SetValue(TabHintsVisibilityProperty, value); }
		}
		protected DockHintButton LeftButton { get; private set; }
		protected DockHintButton RightButton { get; private set; }
		protected DockHintButton TopButton { get; private set; }
		protected DockHintButton BottomButton { get; private set; }
		protected DockHintButton FillButton { get; private set; }
		protected DockHintButton TabBottomButton { get; private set; }
		protected DockHintButton TabLeftButton { get; private set; }
		protected DockHintButton TabRightButton { get; private set; }
		protected DockHintButton TabTopButton { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LeftButton = GetTemplateChild("PART_Left") as DockHintButton;
			RightButton = GetTemplateChild("PART_Right") as DockHintButton;
			TopButton = GetTemplateChild("PART_Top") as DockHintButton;
			BottomButton = GetTemplateChild("PART_Bottom") as DockHintButton;
			FillButton = GetTemplateChild("PART_Fill") as DockHintButton;
			TabLeftButton = GetTemplateChild("PART_TabLeft") as DockHintButton;
			TabRightButton = GetTemplateChild("PART_TabRight") as DockHintButton;
			TabTopButton = GetTemplateChild("PART_TabTop") as DockHintButton;
			TabBottomButton = GetTemplateChild("PART_TabBottom") as DockHintButton;
			BindingHelper.SetBinding(BottomButton, VisibilityProperty, this, "DockHintsVisibility");
			BindingHelper.SetBinding(LeftButton, VisibilityProperty, this, "DockHintsVisibility");
			BindingHelper.SetBinding(RightButton, VisibilityProperty, this, "DockHintsVisibility");
			BindingHelper.SetBinding(TopButton, VisibilityProperty, this, "DockHintsVisibility");
			BindingHelper.SetBinding(TabBottomButton, VisibilityProperty, this, "TabHintsVisibility");
			BindingHelper.SetBinding(TabLeftButton, VisibilityProperty, this, "TabHintsVisibility");
			BindingHelper.SetBinding(TabRightButton, VisibilityProperty, this, "TabHintsVisibility");
			BindingHelper.SetBinding(TabTopButton, VisibilityProperty, this, "TabHintsVisibility");
		}
		public void UpdateState(DockHintsConfiguration configuration) {
			UpdateAvailableState(configuration);
			UpdateEnabledState(configuration);
			UpdateOpactity(configuration);
		}
		public override void UpdateState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			UpdateState(configuration);
		}
		public override void UpdateHotTrack(DockHintButton hotButton) {
			UpdateHotTrack(LeftButton, hotButton);
			UpdateHotTrack(RightButton, hotButton);
			UpdateHotTrack(TopButton, hotButton);
			UpdateHotTrack(BottomButton, hotButton);
			UpdateHotTrack(FillButton, hotButton);
			UpdateHotTrack(TabLeftButton, hotButton);
			UpdateHotTrack(TabRightButton, hotButton);
			UpdateHotTrack(TabTopButton, hotButton);
			UpdateHotTrack(TabBottomButton, hotButton);
		}
		public void UpdateAvailableState(DockHintsConfiguration configuration) {
			if(FillButton != null)
				FillButton.IsAvailable = configuration.GetIsEnabled(DockHint.Center);
			if(LeftButton != null)
				LeftButton.IsAvailable = configuration.GetIsEnabled(DockHint.CenterLeft);
			if(RightButton != null)
				RightButton.IsAvailable = configuration.GetIsEnabled(DockHint.CenterRight);
			if(TopButton != null)
				TopButton.IsAvailable = configuration.GetIsEnabled(DockHint.CenterTop);
			if(BottomButton != null)
				BottomButton.IsAvailable = configuration.GetIsEnabled(DockHint.CenterBottom);
			if(TabLeftButton != null)
				TabLeftButton.IsAvailable = configuration.GetIsEnabled(DockHint.TabLeft);
			if(TabRightButton != null)
				TabRightButton.IsAvailable = configuration.GetIsEnabled(DockHint.TabRight);
			if(TabTopButton != null)
				TabTopButton.IsAvailable = configuration.GetIsEnabled(DockHint.TabTop);
			if(TabBottomButton != null)
				TabBottomButton.IsAvailable = configuration.GetIsEnabled(DockHint.TabBottom);
		}
		public override void UpdateAvailableState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			UpdateAvailableState(configuration);
		}
		public void UpdateEnabledState(DockHintsConfiguration configuration) {
			DockHintsVisibility = VisibilityHelper.Convert(configuration.DisplayMode != DockGuidDisplayMode.TabOnly && configuration.DisplayMode!= DockGuidDisplayMode.FillOnly);
			TabHintsVisibility = VisibilityHelper.Convert(configuration.DisplayMode != DockGuidDisplayMode.DockOnly);
			if(FillButton != null)
				FillButton.IsEnabled = configuration.GetIsVisible(DockHint.Center);
			if(LeftButton != null)
				LeftButton.IsEnabled = configuration.GetIsVisible(DockHint.CenterLeft);
			if(RightButton != null)
				RightButton.IsEnabled = configuration.GetIsVisible(DockHint.CenterRight);
			if(TopButton != null)
				TopButton.IsEnabled = configuration.GetIsVisible(DockHint.CenterTop);
			if(BottomButton != null)
				BottomButton.IsEnabled = configuration.GetIsVisible(DockHint.CenterBottom);
			if(TabLeftButton != null)
				TabLeftButton.IsEnabled = configuration.GetIsVisible(DockHint.TabLeft);
			if(TabRightButton != null)
				TabRightButton.IsEnabled = configuration.GetIsVisible(DockHint.TabRight);
			if(TabTopButton != null)
				TabTopButton.IsEnabled = configuration.GetIsVisible(DockHint.TabTop);
			if(TabBottomButton != null)
				TabBottomButton.IsEnabled = configuration.GetIsVisible(DockHint.TabBottom);
		}
		private void UpdateOpactity(DockHintsConfiguration configuration) {
			bool hideTab = configuration.DisplayMode == DockGuidDisplayMode.DockAndFill || configuration.DisplayMode == DockGuidDisplayMode.FillOnly;
			if(TabLeftButton != null) TabLeftButton.Opacity = hideTab ? 0 : 1;
			if(TabRightButton != null) TabRightButton.Opacity = hideTab ? 0 : 1;
			if(TabTopButton != null) TabTopButton.Opacity = hideTab ? 0 : 1;
			if(TabBottomButton != null) TabBottomButton.Opacity = hideTab ? 0 : 1;
		}
		public override void UpdateEnabledState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			UpdateEnabledState(configuration);
		}
		protected override bool CalcVisibileState(DockingHintAdorner adorner) {
			DockHintsConfiguration configuration = adorner.DockHintsConfiguration;
			return configuration.GetGuideIsVisible(Type) && configuration.ShowSelfDockHint;
		}
		protected override Rect CalcBounds(DockingHintAdorner adorner) {
			return adorner.TargetRect;
		}
		protected virtual void OnTabHintsVisibilityChanged(Visibility oldValue, Visibility newValue) {
		}
		protected virtual void OnDockHintsVisibilityChanged(Visibility oldValue, Visibility newValue) {
		}
	}
	public class RectangleDockHint : DockHintElement {
		static RectangleDockHint() {
			var dProp = new DependencyPropertyRegistrator<RectangleDockHint>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public RectangleDockHint()
			: base(DockVisualizerElement.DockZone) {
			IsHitTestVisible = false;
		}
		protected DockHintButton HotButton { get; private set; }
		public override void UpdateHotTrack(DockHintButton button) {
			HotButton = button;
		}
		public override void UpdateEnabledState(DockingHintAdorner adorner) {
			IsEnabled = (HotButton != null) && HotButton.IsAvailable;
		}
		protected override bool CalcVisibileState(DockingHintAdorner adorner) {
			return !adorner.HintRect.IsEmpty;
		}
		protected override Rect CalcBounds(DockingHintAdorner adorner) {
			return adorner.HintRect;
		}
	}
	public class SelectionHint : DockHintElement {
		#region static
		static readonly DependencyPropertyKey ShowLeftMarkerPropertyKey;
		public static readonly DependencyProperty ShowLeftMarkerProperty;
		static readonly DependencyPropertyKey ShowRightMarkerPropertyKey;
		public static readonly DependencyProperty ShowRightMarkerProperty;
		static readonly DependencyPropertyKey ShowTopMarkerPropertyKey;
		public static readonly DependencyProperty ShowTopMarkerProperty;
		static readonly DependencyPropertyKey ShowBottomMarkerPropertyKey;
		public static readonly DependencyProperty ShowBottomMarkerProperty;
		static SelectionHint() {
			var dProp = new DependencyPropertyRegistrator<SelectionHint>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.RegisterReadonly("ShowLeftMarker", ref ShowLeftMarkerPropertyKey, ref ShowLeftMarkerProperty, false);
			dProp.RegisterReadonly("ShowRightMarker", ref ShowRightMarkerPropertyKey, ref ShowRightMarkerProperty, false);
			dProp.RegisterReadonly("ShowTopMarker", ref ShowTopMarkerPropertyKey, ref ShowTopMarkerProperty, false);
			dProp.RegisterReadonly("ShowBottomMarker", ref ShowBottomMarkerPropertyKey, ref ShowBottomMarkerProperty, false);
		}
		#endregion static
		public SelectionHint()
			: base(DockVisualizerElement.Selection) {
			Focusable = false;
			IsTabStop = false;
			IsHitTestVisible = false;
		}
		public bool ShowLeftMarker {
			get { return (bool)GetValue(ShowLeftMarkerProperty); }
			internal set { SetValue(ShowLeftMarkerPropertyKey, value); }
		}
		public bool ShowRightMarker {
			get { return (bool)GetValue(ShowRightMarkerProperty); }
			internal set { SetValue(ShowRightMarkerPropertyKey, value); }
		}
		public bool ShowTopMarker {
			get { return (bool)GetValue(ShowTopMarkerProperty); }
			internal set { SetValue(ShowTopMarkerPropertyKey, value); }
		}
		public bool ShowBottomMarker {
			get { return (bool)GetValue(ShowBottomMarkerProperty); }
			internal set { SetValue(ShowBottomMarkerPropertyKey, value); }
		}
		BaseLayoutItem itemCore;
		public BaseLayoutItem Item {
			get { return itemCore; }
			set {
				if(Item == value) return;
				itemCore = value;
				OnItemChanged();
			}
		}
		protected UIElement ItemElement { get; set; }
		protected void UpdateMarkers(bool isVerticalOrientation, bool isFirst, bool isLast) {
			if(isVerticalOrientation) {
				ShowTopMarker = !isFirst;
				ShowBottomMarker = !isLast;
				ShowLeftMarker = false;
				ShowRightMarker = false;
			}
			else {
				ShowLeftMarker = !isFirst;
				ShowRightMarker = !isLast;
				ShowTopMarker = false;
				ShowBottomMarker = false;
			}
		}
		protected void UpdateMarkers() {
			if(Item != null) {
				LayoutGroup parent = Item.Parent;
				if(parent != null) {
					int itemIndex = parent.Items.IndexOf(Item);
					bool isFirst = itemIndex == 0;
					bool isLast = itemIndex == parent.Items.Count - 1;
					UpdateMarkers(parent.Orientation == Orientation.Vertical, isFirst, isLast);
				}
			}
		}
		internal void OnItemElementMouseEnter(object sender, MouseEventArgs e) {
			UpdateMarkers();
		}
		internal void OnItemElementMouseLeave(object sender, MouseEventArgs e) {
			ShowLeftMarker = false;
			ShowRightMarker = false;
			ShowTopMarker = false;
			ShowBottomMarker = false;
		}
		protected virtual void OnItemChanged() {
			if(Item != null && Item.Manager != null)
				ItemElement = (UIElement)(Item.GetUIElement<BaseLayoutItem>() ?? Item.GetUIElement<IUIElement>());
			else ItemElement = null;
			UpdateMarkers();
		}
		protected internal Rect GetItemBounds() {
			if(Item != null && Item.Manager != null && ItemElement != null) {
				ILayoutElement element = Item.Manager.GetViewElement((IUIElement)ItemElement);
				if(element != null) {
					ForceUpdateElementBounds(element);
					return ElementHelper.GetRect(element);
				}
			}
			return Rect.Empty;
		}
		void ForceUpdateElementBounds(ILayoutElement element) {
			element.Invalidate();
			element.EnsureBounds();
		}
		protected override bool CalcVisibileState(DockingHintAdorner adorner) {
			return adorner.ShowSelectionHints && (Item != null) && Item.Visibility != System.Windows.Visibility.Collapsed;
		}
		protected override Rect CalcBounds(DockingHintAdorner adorner) {
			Rect bounds = GetItemBounds();
			if(!bounds.IsEmpty)
				RectHelper.Inflate(ref bounds, 1, 1);
			return bounds;
		}
	}
	[TemplatePart(Name = "PART_Text", Type = typeof(TextBox))]
	public class RenameHint : DockHintElement {
		#region static
		static RenameHint() {
			var dProp = new DependencyPropertyRegistrator<RenameHint>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public RenameHint()
			: base(DockVisualizerElement.RenameHint) {
		}
		protected TextBox PartText { get; private set; }
		DockLayoutManager Manager { get { return Item != null ? Item.Manager : null; } }
		public bool IsRenamingStarted { get { return lockRenaming > 0; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeEvents();
			PartText = GetTemplateChild("PART_Text") as TextBox;
			SubscribeEvents();
		}
		private void SubscribeEvents() {
			if(PartText == null) return;
			PartText.IsVisibleChanged += PartText_IsVisibleChanged;
			PartText.Loaded += PartText_Loaded;
		}
		private void UnsubscribeEvents() {
			if(PartText == null) return;
			PartText.IsVisibleChanged -= PartText_IsVisibleChanged;
			PartText.Loaded -= PartText_Loaded;
		}
		void PartText_Loaded(object sender, RoutedEventArgs e) {
			if(Item != null)
				PartText.Text = Item.Caption as string;
			PartText.Focus();
			PartText.SelectAll();
		}
		void PartText_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue) {
				PartText.Focus();
				PartText.SelectAll();
			}
		}
		void MeasureText() {
			string caption = Item != null ? Item.Caption as string : null;
			PartText.Text = !string.IsNullOrEmpty(caption) ? caption : "TestText";
			PartText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			TextMinSize = PartText.DesiredSize;
		}
		IDockLayoutElement LayoutElement;
		internal void StartRenaming(IDockLayoutElement element) {
			if(lockRenaming > 0)
				CancelRenaming();
			lockRenaming++;
			LayoutElement = element;
			Item = element.Item;
			if(Manager != null)
				Manager.CoerceValue(DockLayoutManager.IsRenamingProperty);
		}
		internal void EndRenaming() {
			if(lockRenaming > 0) {
				lockRenaming--;
				Item.Caption = PartText.Text;
				if(Manager != null) {
					Manager.RaiseEvent(new LayoutItemEndRenamingEventArgs(Item, OldCaption));
					Manager.CoerceValue(DockLayoutManager.IsRenamingProperty);
				}
				Item = null;
			}
		}
		internal void CancelRenaming() {
			if(lockRenaming > 0) {
				lockRenaming--;
				if(Manager != null) {
					Manager.RaiseEvent(new LayoutItemEndRenamingEventArgs(Item, OldCaption, true));
					Manager.CoerceValue(DockLayoutManager.IsRenamingProperty);
				}
				Item = null;
			}
		}
		BaseLayoutItem itemCore;
		string OldCaption;
		int lockRenaming = 0;
		internal BaseLayoutItem Item {
			get { return itemCore; }
			set {
				if(Item == value) return;
				itemCore = value;
				OnItemChanged();
				OldCaption = itemCore != null ? itemCore.Caption as string : null;
			}
		}
		protected virtual void OnItemChanged() {
			if(Item != null && Item.Manager != null) {
				if(PartText != null && Item != null) {
					MeasureText();
					PartText.Text = Item.Caption as string;
				}
			}
		}
		internal Size TextMinSize { get; private set; }
		protected internal Rect GetItemBounds() {
			var uiElement = LayoutElement != null ? LayoutElement.Element : Item;
			if(Item != null && Item.Manager != null && uiElement != null) {
				IDockLayoutElement element = LayoutElement;
				CaptionControl caption = LayoutHelper.FindElement((FrameworkElement)uiElement, (e) => e is CaptionControl) as CaptionControl;
				UIElement renameElement;
				FrameworkElement text = null;
				if(element != null) {
					if(caption != null) {
						text = caption.PartText as FrameworkElement;
						renameElement = text ?? caption;
					}
					else
						renameElement = uiElement;
					Size renderSize = renameElement.RenderSize;
					if(renderSize.Width <= 0 || renderSize.Height <= 0 || caption == null || string.IsNullOrEmpty(Item.Caption as string)) {
						renderSize = TextMinSize;
					}
					Point origin = renameElement.TranslatePoint(CoordinateHelper.ZeroPoint, element.View);
					Rect captionRect = new Rect(origin, renderSize);
					if(text != null) {
						RectHelper.Inflate(ref captionRect, 4, 2);
					}
					else {
						RectHelper.Inflate(ref captionRect, 2, 2);
					}
					return captionRect;
				}
			}
			return Rect.Empty;
		}
		protected override bool CalcVisibileState(DockingHintAdorner adorner) {
			return (Item != null) && Item.AllowRename;
		}
		protected override Rect CalcBounds(DockingHintAdorner adorner) {
			Rect bounds = GetItemBounds();
			if(!bounds.IsEmpty) {
				RectHelper.Inflate(ref bounds, 1, 1);
			}
			return bounds;
		}
	}
}

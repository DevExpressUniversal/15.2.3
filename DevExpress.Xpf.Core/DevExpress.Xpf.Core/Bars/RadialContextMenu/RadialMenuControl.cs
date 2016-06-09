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

using DevExpress.Utils;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using DevExpress.Mvvm.Native;
using System.Windows.Media.Animation;
using DevExpress.Mvvm.UI;
namespace DevExpress.Xpf.Bars {
	public class RadialMenuControl : PopupMenuBarControl, IToolTipPlacementTarget {
		const string ElementCenterButton = "PART_CenterButton";		
		const string ElementArrowButton = "PART_ArrowButton";
		#region static
		public static readonly DependencyProperty ToolTipVerticalOffsetProperty = DependencyProperty.Register("ToolTipVerticalOffset", typeof(double), typeof(RadialMenuControl), new PropertyMetadata(0d));
		private static readonly DependencyPropertyKey Content1PropertyKey = DependencyPropertyManager.RegisterReadOnly("Content1", typeof(object), typeof(RadialMenuControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
		public static readonly DependencyProperty Content1Property = Content1PropertyKey.DependencyProperty;
		private static readonly DependencyPropertyKey Content2PropertyKey = DependencyPropertyManager.RegisterReadOnly("Content2", typeof(object), typeof(RadialMenuControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
		public static readonly DependencyProperty Content2Property = Content2PropertyKey.DependencyProperty;
		private static readonly DependencyPropertyKey ActiveContentIndexPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActiveContentIndex", typeof(int), typeof(RadialMenuControl), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.None));
		public static readonly DependencyProperty ActiveContentIndexProperty = ActiveContentIndexPropertyKey.DependencyProperty;
		private static readonly DependencyPropertyKey Glyph1PropertyKey = DependencyPropertyManager.RegisterReadOnly("Glyph1", typeof(ImageSource), typeof(RadialMenuControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
		public static readonly DependencyProperty Glyph1Property = Glyph1PropertyKey.DependencyProperty;
		private static readonly DependencyPropertyKey Glyph2PropertyKey = DependencyPropertyManager.RegisterReadOnly("Glyph2", typeof(ImageSource), typeof(RadialMenuControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
		public static readonly DependencyProperty Glyph2Property = Glyph2PropertyKey.DependencyProperty;
		public static readonly DependencyProperty DefaultGlyphTemplateProperty = DependencyProperty.Register("DefaultGlyphTemplate", typeof(DataTemplate), typeof(RadialMenuControl), new PropertyMetadata(null, (d,e)=>((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty DefaultBackButtonGlyphTemplateProperty = DependencyProperty.Register("DefaultBackButtonGlyphTemplate", typeof(DataTemplate), typeof(RadialMenuControl), new PropertyMetadata(null, (d,e)=>((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty DefaultGlyphProperty = DependencyProperty.Register("DefaultGlyph", typeof(ImageSource), typeof(RadialMenuControl), new PropertyMetadata(null, (d,e)=>((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty DefaultBackButtonGlyphProperty = DependencyProperty.Register("DefaultBackButtonGlyph", typeof(ImageSource), typeof(RadialMenuControl), new PropertyMetadata(null, (d,e)=>((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(RadialMenuControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));
		public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(ImageSource), typeof(RadialMenuControl), new PropertyMetadata(null, (d, e) => ((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty BackButtonGlyphProperty = DependencyProperty.Register("BackButtonGlyph", typeof(ImageSource), typeof(RadialMenuControl), new PropertyMetadata(null, (d, e) => ((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty GlyphTemplateProperty = DependencyProperty.Register("GlyphTemplate", typeof(DataTemplate), typeof(RadialMenuControl), new PropertyMetadata(null, (d, e) => ((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty BackButtonGlyphTemplateProperty = DependencyProperty.Register("BackButtonGlyphTemplate", typeof(DataTemplate), typeof(RadialMenuControl), new PropertyMetadata(null, (d, e) => ((RadialMenuControl)d).UpdateActiveGlyph()));
		public static readonly DependencyProperty SectorGapProperty = DependencyPropertyManager.Register("SectorGap", typeof(double), typeof(RadialMenuControl), new FrameworkPropertyMetadata(0d));
		private static readonly DependencyPropertyKey IsRegularPopupOpenedPropertyKey = DependencyProperty.RegisterReadOnly("IsRegularPopupOpened", typeof(bool), typeof(RadialMenuControl), new PropertyMetadata(false));
		public static readonly DependencyProperty IsRegularPopupOpenedProperty = IsRegularPopupOpenedPropertyKey.DependencyProperty;
		protected static readonly DependencyPropertyKey Glyph1TemplatePropertyKey = DependencyProperty.RegisterReadOnly("Glyph1Template", typeof(DataTemplate), typeof(RadialMenuControl), new PropertyMetadata(null));
		public static readonly DependencyProperty Glyph1TemplateProperty = Glyph1TemplatePropertyKey.DependencyProperty;
		protected static readonly DependencyPropertyKey Glyph2TemplatePropertyKey = DependencyProperty.RegisterReadOnly("Glyph2Template", typeof(DataTemplate), typeof(RadialMenuControl), new PropertyMetadata(null));
		public static readonly DependencyProperty Glyph2TemplateProperty = Glyph2TemplatePropertyKey.DependencyProperty;	
		static RadialMenuControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialMenuControl), new FrameworkPropertyMetadata(typeof(RadialMenuControl)));
			FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(RadialMenuControl), new FrameworkPropertyMetadata(false, OnPropChanged));
		}
		private static void OnPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		#endregion
		#region dep props
		public DataTemplate Glyph1Template {
			get { return (DataTemplate)GetValue(Glyph1TemplateProperty); }
			protected set { SetValue(Glyph1TemplatePropertyKey, value); }
		}
		public DataTemplate Glyph2Template {
			get { return (DataTemplate)GetValue(Glyph2TemplateProperty); }
			protected set { SetValue(Glyph2TemplatePropertyKey, value); }
		}
		public ImageSource DefaultGlyph {
			get { return (ImageSource)GetValue(DefaultGlyphProperty); }
			set { SetValue(DefaultGlyphProperty, value); }
		}
		public ImageSource DefaultBackButtonGlyph {
			get { return (ImageSource)GetValue(DefaultBackButtonGlyphProperty); }
			set { SetValue(DefaultBackButtonGlyphProperty, value); }
		}
		public DataTemplate DefaultGlyphTemplate {
			get { return (DataTemplate)GetValue(DefaultGlyphTemplateProperty); }
			set { SetValue(DefaultGlyphTemplateProperty, value); }
		}
		public DataTemplate DefaultBackButtonGlyphTemplate {
			get { return (DataTemplate)GetValue(DefaultBackButtonGlyphTemplateProperty); }
			set { SetValue(DefaultBackButtonGlyphTemplateProperty, value); }
		}
		public DataTemplate GlyphTemplate {
			get { return (DataTemplate)GetValue(GlyphTemplateProperty); }
			set { SetValue(GlyphTemplateProperty, value); }
		}
		public DataTemplate BackButtonGlyphTemplate {
			get { return (DataTemplate)GetValue(BackButtonGlyphTemplateProperty); }
			set { SetValue(BackButtonGlyphTemplateProperty, value); }
		}
		public double ToolTipVerticalOffset {
			get { return (double)GetValue(ToolTipVerticalOffsetProperty); }
			set { SetValue(ToolTipVerticalOffsetProperty, value); }
		}
		public object Content1 {
			get { return (object)GetValue(Content1Property); }
			protected set { SetValue(Content1PropertyKey, value); }
		}
		public object Content2 {
			get { return (object)GetValue(Content2Property); }
			protected set { SetValue(Content2PropertyKey, value); }
		}
		public int ActiveContentIndex {
			get { return (int)GetValue(ActiveContentIndexProperty); }
			protected set { SetValue(ActiveContentIndexPropertyKey, value); }
		}
		public ImageSource Glyph1 {
			get { return (ImageSource)GetValue(Glyph1Property); }
			protected set { SetValue(Glyph1PropertyKey, value); }
		}
		public ImageSource Glyph2 {
			get { return (ImageSource)GetValue(Glyph2Property); }
			protected set { SetValue(Glyph2PropertyKey, value); }
		}
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public ImageSource BackButtonGlyph {
			get { return (ImageSource)GetValue(BackButtonGlyphProperty); }
			set { SetValue(BackButtonGlyphProperty, value); }
		}
		public double SectorGap {
			get { return (double)GetValue(SectorGapProperty); }
			set { SetValue(SectorGapProperty, value); }
		}
		public bool IsRegularPopupOpened {
			get { return (bool)GetValue(IsRegularPopupOpenedProperty); }
			protected set { SetValue(IsRegularPopupOpenedPropertyKey, value); }
		}
		#endregion
		#region template parts
		protected Button CenterButton { get; set; }		
		#endregion
		protected class RadialMenuLevelInfo {
			public object LevelContent { get; private set; }
			public IInputElement SavedFocus { get; private set; }			
			public RadialMenuLevelInfo() { }
			public RadialMenuLevelInfo(object content, IInputElement savedFocus) {
				LevelContent = content;
				SavedFocus = savedFocus;
			}
		}
		IInputElement SavedRegularPopupFocus { get; set; }
		IPopupControl RegularPopup { get; set; }
		Stack<RadialMenuLevelInfo> levelsCore = null;
		protected Stack<RadialMenuLevelInfo> Levels { get { return levelsCore ?? (levelsCore = new Stack<RadialMenuLevelInfo>()); } }
		protected RadialContextMenu RadialContextMenu { get; private set; }
		public RadialMenuControl() : this(null) {
		}
		internal RadialMenuControl(RadialContextMenu popup)
			: base(popup) {
			RadialContextMenu = popup;			
			SetBindings();
			AddHandler(Button.ClickEvent, new RoutedEventHandler(OnButtonClickEventHandler));
			IsVisibleChanged += OnIsVisibleChanged;
		}
		protected override void OnUnloaded(object sender, RoutedEventArgs e) {
			base.OnUnloaded(sender, e);
			FocusRoot();
		}
		void SetBindings() {
			if(RadialContextMenu != null) {
				SetBinding(GlyphProperty, new Binding("Glyph") { Source = RadialContextMenu });
				SetBinding(BackButtonGlyphProperty, new Binding("BackButtonGlyph") { Source = RadialContextMenu });
				SetBinding(GlyphTemplateProperty, new Binding("GlyphTemplate") { Source = RadialContextMenu });
				SetBinding(BackButtonGlyphTemplateProperty, new Binding("BackButtonGlyphTemplate") { Source = RadialContextMenu });
			}
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(!IsVisible)
				Reset();
			else {
				OnShow();				
			}
		}		
		protected virtual void OnShow() {
			RadialMenuLevelControl levelContent = CreateRootLevelContent();
			Levels.Push(new RadialMenuLevelInfo(levelContent, null));
			SetActiveContent(levelContent);
			UpdateActiveGlyph();
			SetIsExpandedOnShow();
		}
		protected virtual void SetSmartLayoutBindings(RadialMenuLevelControl levelControl, object source) {
			levelControl.SetBinding(RadialMenuLevelControl.FirstSectorIndexProperty, new Binding("FirstSectorIndex") { Source = source });
		}
		protected virtual RadialMenuLevelControl CreateRootLevelContent() {
			RadialMenuLevelControl levelControl = new RadialMenuLevelControl(LinksHolder);
			SetSmartLayoutBindings(levelControl, RadialContextMenu);			
			return levelControl;
		}
		private void SetIsExpandedOnShow() {
			if(RadialContextMenu == null) {
				IsExpanded = true;
				return;
			}
			if(RadialContextMenu.AutoExpand.HasValue)
				IsExpanded = RadialContextMenu.AutoExpand.Value;
			else {
				IsExpanded = (RadialContextMenu.OpenReason == ContextMenuOpenReason.RightMouseButtonClick) ||
					(RadialContextMenu.OpenReason == ContextMenuOpenReason.Keyboard);
			}
		}
		protected virtual void ProcessCenterButtonClick() {
			if(Levels.Count > 1) {
				OnBackButtonClick();
			} else {
				OnRootButtonClick();
			}
		}
		private void OnButtonClickEventHandler(object sender, RoutedEventArgs e) {
			Button button = e.OriginalSource as Button;
			if(button == null) return;
			if(button.Name == ElementCenterButton) {
				ProcessCenterButtonClick();
				e.Handled = true;
				return;
			}
			if(button.Name == ElementArrowButton) {
				BarItemLinkControlBase linkControlBase = button.VisualParents().OfType<BarItemLinkControlBase>().FirstOrDefault();
				(linkControlBase as BarItemLinkControl).With(lc => lc.Link).With(l => l.Item).Do(i => OnArrowButtonClick(i));
				e.Handled = true;
				return;
			}
			return;			
		}
		protected virtual void OnRootButtonClick() {
			SwitchMenuVisibility();
		}
		#region template control events
		public override void OnApplyTemplate() {
			UnsubscribeEvents();
			base.OnApplyTemplate();
			CenterButton = (Button)GetTemplateChild(ElementCenterButton);
			SubscribeEvents();
		}
		protected new virtual void UnsubscribeEvents() {			
			CenterButton.Do(x=>x.MouseEnter -= CenterButton_MouseEnter);			
		}
		protected new virtual void SubscribeEvents() {
			CenterButton.Do(x => x.MouseEnter += CenterButton_MouseEnter);			
		}
		void CenterButton_MouseEnter(object sender, MouseEventArgs e) {
			FocusElement(CenterButton);
		}
		#endregion
		protected override void OnMouseLeave(MouseEventArgs e) {
			FocusRoot();
		}
		protected virtual void SwitchMenuVisibility() {
			IsExpanded = !IsExpanded;
		}
		object GetLevelGlyph() {
			if(Levels.Count <= 1) {				
				if(GlyphTemplate != null) return GlyphTemplate;
				if(Glyph != null) return Glyph;
				if(DefaultGlyphTemplate != null) return DefaultGlyphTemplate;
				return DefaultGlyph;
			} else {
				if(BackButtonGlyphTemplate != null) return BackButtonGlyphTemplate;
				if(BackButtonGlyph != null) return BackButtonGlyph;
				if(DefaultBackButtonGlyphTemplate != null) return DefaultBackButtonGlyphTemplate;
				return DefaultBackButtonGlyph;
			}
		}
		protected internal virtual void ShowSubMenu(BarItem item, bool openFromKeyboard = false) {
			if(item is ILinksHolder) {
				RadialMenuLevelControl levelControl = new RadialMenuLevelControl(((ILinksHolder)item));
				SetSmartLayoutBindings(levelControl, item);
				AddAndShowLevel(levelControl);
				if(openFromKeyboard) {
					FocusElement(CenterButton);
				}
				return;
			}
			BarSplitButtonItem splitButtonItem = item as BarSplitButtonItem;
			if(splitButtonItem != null) {
				if(splitButtonItem.PopupControl != null && splitButtonItem.PopupControl is ILinksHolder) {
					RadialMenuLevelControl levelControl = new RadialMenuLevelControl(splitButtonItem.PopupControl as ILinksHolder);
					SetSmartLayoutBindings(levelControl, item);
					AddAndShowLevel(levelControl);
					if(openFromKeyboard) {
						FocusElement(CenterButton);
					}
				} else if(splitButtonItem.PopupControl != null) {
					SavedRegularPopupFocus = GetFocusedElement();
					splitButtonItem.PopupControl.Closed += OnRegularPopupClosed;
					(splitButtonItem.PopupControl.Popup.PopupContent as FrameworkElement).Do(x => x.Loaded += OnRegularPopupContentLoaded);
					IsRegularPopupOpened = true;
					splitButtonItem.PopupControl.Popup.IsBranchHeader = true;
					this.Popup.StaysOpenOnOuterClick = true;
					splitButtonItem.PopupControl.ShowPopup(this);
					RegularPopup = splitButtonItem.PopupControl;
				}				
				return;
			}
		}
		protected virtual void Return(bool byKeyboard) {
			IInputElement savedFocus = Levels.Peek().SavedFocus;
			Levels.Pop();
			SetBackContent(Levels.Peek().LevelContent);
			SetBackGlyph(GetLevelGlyph());
			SwapContents();
			if(byKeyboard)
				FocusElement(savedFocus);
		}
		protected virtual void OnBackButtonClick() {
			Return(false);			
		}
		protected virtual void OnArrowButtonClick(BarItem item) {
			ShowSubMenu(item, false);
		}
		protected virtual void OnRegularPopupContentLoaded(object sender, RoutedEventArgs e) {
			(sender as IInputElement).Do(x => FocusElement(x));
		}
		protected virtual void OnRegularPopupClosed(object sender, System.EventArgs e) {
			((IPopupControl)sender).Closed -= OnRegularPopupClosed;
			(((IPopupControl)sender).Popup.PopupContent as FrameworkElement).Do(x => x.Loaded -= OnRegularPopupContentLoaded);
			IsRegularPopupOpened = false;
			this.Popup.StaysOpenOnOuterClick = false;
			FocusElement(SavedRegularPopupFocus);
		}
		void SetContent(int index, object value) {
			if(index == 1) {
				Content1 = value;
			} else {
				Content2 = value;
			}
		}
		void SetGlyph(int index, object value) {
			if(index == 1) {
				if(value is DataTemplate) {
					Glyph1 = null;
					Glyph1Template = value as DataTemplate;
				} else {
					Glyph1 = value as ImageSource;
					Glyph1Template = null;
				}
			} else {
				if(value is DataTemplate) {
					Glyph2 = null;
					Glyph2Template = value as DataTemplate;
				} else {
					Glyph2 = value as ImageSource;
					Glyph2Template = null;
				}				
			}
		}
		protected object GetActiveContent() {
			return ActiveContentIndex == 1 ? Content1 : Content2;
		}
		protected void SetActiveContent(object value) {
			SetContent(ActiveContentIndex, value);
		}
		protected void SetActiveGlyph(object value) {
			SetGlyph(ActiveContentIndex, value);
		}
		protected int GetNextActiveContentIndex() { return 1 + ActiveContentIndex % 2; }
		protected void SetBackContent(object value) {
			SetContent(GetNextActiveContentIndex(), value);
		}
		protected void SetBackGlyph(object value) {
			SetGlyph(GetNextActiveContentIndex(), value);
		}
		protected void SwapContents() {
			ActiveContentIndex = GetNextActiveContentIndex();
		}
		protected virtual void UpdateActiveGlyph() {
			SetActiveGlyph(GetLevelGlyph());
		}
		protected override void OnLinksHolderChanged(DependencyPropertyChangedEventArgs e) {
			base.OnLinksHolderChanged(e);
		}
		protected void AddAndShowLevel(object levelContent) {
			Levels.Push(new RadialMenuLevelInfo(levelContent, GetFocusedElement()));
			if(Levels.Count > 1) {
				Width = ActualWidth;
				Height = ActualHeight;
			}
			SetBackContent(levelContent);
			SetBackGlyph(GetLevelGlyph());
			SwapContents();
		}
		protected virtual void ShowRootLevel() {
			Levels.Clear();
			AddAndShowLevel(CreateRootLevelContent());
		}
		protected internal override void OnFrameworkElementLostMouseCapture(MouseEventArgs e) {
			if((e.OriginalSource is UIElement) && ((UIElement)e.OriginalSource).VisualParents().FirstOrDefault(t => t == this) != null) {
				return;
			}
		}
		public virtual void Expand() {
			IsExpanded = true;
		}
		public virtual void Collapse() {
			if(Levels.Count == 1) {
				IsExpanded = false;
				return;
			}
			Levels.Clear();
			AddAndShowLevel(new RadialMenuLevelControl(LinksHolder));
			IsExpanded = false;
		}
		protected virtual void Reset() {
			Levels.Clear();
			Content1 = null;
			Content2 = null;
			ActiveContentIndex = 1;
			Glyph1 = null;
			Glyph2 = null;
			IsExpanded = false;
			IsRegularPopupOpened = false;
		}
		#region keyboard navigation        
		protected T GetItem<T>(object linkControl) where T:BarItem {
			return (linkControl as BarItemLinkControl).With(c=>c.Link).With(l=>l.Item) as T;
		}
		protected BarItem GetLevelOwnerItem(BarItemLinkControl linkControl) {
			if(linkControl == null)
				return null;
			if(linkControl is BarSubItemLinkControl || linkControl is BarSplitButtonItemLinkControl)
				return linkControl.With(lc => lc.Link).With(l => l.Item);
			return null;
		}
		List<IInputElement> GetActiveNavigationElements() {
			List<IInputElement> elements = new List<IInputElement>();
			elements.Add(CenterButton);
			if(!IsExpanded) return elements;
			if(GetActiveContent() is RadialMenuLevelControl) {
				elements.AddRange((GetActiveContent() as RadialMenuLevelControl).VisualChildren().OfType<BarItemLinkControl>().Where(lc=>lc.Focusable && lc.Visibility == System.Windows.Visibility.Visible));
			}
			return elements;
		}
		protected internal virtual bool ProcessNativeKeyDown(Key key, ModifierKeys modifiers) {
			if(modifiers.HasFlag(ModifierKeys.Shift) && key == Key.Tab) {
				return ProcessUp();				
			}
			switch(key) {
				case Key.Escape:
					return ProcessEscape();
				case Key.Back:
				case Key.Left:
					return ProcessLeft();
				case Key.Right:
					return ProcessRight();
				case Key.Up:
					return ProcessUp();
				case Key.Tab:
				case Key.Down:
					return ProcessDown();
				case Key.Space:
				case Key.Enter:
					return ProcessSpace();
				default:
					return false;
			}
		}
		protected virtual bool ProcessUp() {
			List<IInputElement> elements = GetActiveNavigationElements();
			int index = elements.IndexOf(GetFocusedElement());
			if(index == -1) {
				if(elements.Count > 0) 
					FocusElement(elements[0]);
				return true;
			}
			if(elements.Count > 0)
				FocusElement(elements[(index - 1 + elements.Count) % elements.Count]);
			return true;
		}		
		protected virtual bool ProcessSpace() {
			if(GetFocusedElement() == CenterButton) {
				if(Levels.Count > 1) {
					Return(true);
				} else {
					OnRootButtonClick();
				}				
				return true;
			}
			if(GetFocusedElement() is BarSplitCheckItemLinkControl)
				DoKeyboardActionForSplitCheckItem();
			else if(GetFocusedElement() is BarSubItemLinkControl)
				DoKeyabordActionForSubItem();
			else if(GetFocusedElement() is BarSplitButtonItemLinkControl)
				DoKeyboardActionForSplitButtonItem();
			else if(GetFocusedElement() is BarCheckItemLinkControl)
				DoKeyboardActionForCheckItem(GetFocusedElement() as BarCheckItemLinkControl);
			else if(GetFocusedElement() is BarButtonItemLinkControl)
				DoKeyboardActionForButtonItem(GetFocusedElement() as BarButtonItemLinkControl);
			else
				DoKeyboardActionForCommonItem();		   
			return true;
		}
		#region keyboard actions
		protected virtual void DoKeyboardActionForSplitCheckItem() {
			BarSplitCheckItemLinkControl splitCheckItemLinkControl = GetFocusedElement() as BarSplitCheckItemLinkControl;
			if(splitCheckItemLinkControl.ActualActAsDropDown) {
				ShowSubMenu(GetItem<BarSplitCheckItem>(splitCheckItemLinkControl), true);
			} else {
				GetItem<BarSplitButtonItem>(splitCheckItemLinkControl).PerformClick();
				PopupMenuManager.CloseAllPopups(splitCheckItemLinkControl, null);
			}
		}
		protected virtual void DoKeyabordActionForSubItem() {
			ShowSubMenu(GetItem<BarSubItem>(GetFocusedElement()), true);
		}
		protected virtual void DoKeyboardActionForSplitButtonItem() {
			BarSplitButtonItemLinkControl splitButtonItemLinkControl = GetFocusedElement() as BarSplitButtonItemLinkControl;
			if(splitButtonItemLinkControl.ActualActAsDropDown) {
				ShowSubMenu(GetItem<BarSplitButtonItem>(splitButtonItemLinkControl), true);
			} else {
				GetItem<BarSplitButtonItem>(splitButtonItemLinkControl).PerformClick();
				PopupMenuManager.CloseAllPopups(splitButtonItemLinkControl, null);
			}
		}
		protected virtual void DoKeyboardActionForCheckItem(BarCheckItemLinkControl checkItemLinkControl) {
			GetItem<BarCheckItem>(checkItemLinkControl).Do(i => i.PerformClick());
			PopupMenuManager.CloseAllPopups(checkItemLinkControl, null);
		}
		protected virtual void DoKeyboardActionForButtonItem(BarButtonItemLinkControl buttonItemLinkControl) {
			GetItem<BarButtonItem>(buttonItemLinkControl).Do(i => i.PerformClick());
			PopupMenuManager.CloseAllPopups(buttonItemLinkControl, null);
		}
		protected virtual void DoKeyboardActionForCommonItem() {
			GetItem<BarItem>(GetFocusedElement()).Do(i => i.PerformClick());
		}
		#endregion
		protected virtual bool ProcessDown() {
			List<IInputElement> elements = GetActiveNavigationElements();
			int index = elements.IndexOf(GetFocusedElement());
			if(index == -1) {
				if(elements.Count > 0)
					FocusElement(elements[0]);
				return true;
			}
			if(elements.Count > 0)
				FocusElement(elements[(index + 1) % elements.Count]);
			return true;
		}
		protected virtual bool ProcessEscape() {
			RadialContextMenu.Do(x=>x.ClosePopup());
			return true;
		}
		protected virtual bool ProcessLeft() {
			if(!IsExpanded)
				return true;
			if(Levels.Count == 1) {
				IsExpanded = false;
				if(GetFocusedElement() != this)
					FocusElement(CenterButton);
			} else {
				Return(true);
			}
			return true;
		}
		protected virtual bool ProcessRight() {
			if(!IsExpanded) {
				IsExpanded = true;
				return true;
			}
			BarItem item = GetLevelOwnerItem(GetFocusedElement() as BarItemLinkControl);
			if(item != null)				
				ShowSubMenu(item, true);
			return true;
		}
		protected override NavigationManager CreateNavigationManager() {
			return new RadialMenuKeyboardNavigationManager(this);
		}		
		protected IInputElement GetFocusedElement() { return Keyboard.FocusedElement; }
		protected void FocusElement(IInputElement element) {			
			element.Focus();
		}
		protected void FocusRoot() {			
			if(!IsRegularPopupOpened && Popup!=null && Popup.IsOpen)
				this.Focus();
		}
		protected virtual void OnNavigationItemLeave() {
			if(Mouse.DirectlyOver is FrameworkElement && ((FrameworkElement)Mouse.DirectlyOver).VisualParents().OfType<FrameworkElement>().Count((x) => { return x is BarItemLinkInfo || x.Name == ElementCenterButton; }) == 0) {
				FocusRoot();
			}
		}
		protected override void OnRemovedFromSelectionCore(bool destroying) {
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
		}
		protected override bool GetIsSelectable() { return true; }		
		protected override IList<INavigationElement> GetNavigationElements() {
			return new List<INavigationElement>();
		}		
		#endregion
		#region IToolTipPlacementTarget
		BarItemLinkControlToolTipHorizontalPlacement IToolTipPlacementTarget.HorizontalPlacement {
			get { return BarItemLinkControlToolTipHorizontalPlacement.CenterAtTargetCenter; }
		}
		BarItemLinkControlToolTipVerticalPlacement IToolTipPlacementTarget.VerticalPlacement {
			get { return BarItemLinkControlToolTipVerticalPlacement.TopAtTargetTop; }
		}
		BarItemLinkControlToolTipPlacementTargetType IToolTipPlacementTarget.HorizontalPlacementTargetType {
			get { return BarItemLinkControlToolTipPlacementTargetType.External; }
		}
		BarItemLinkControlToolTipPlacementTargetType IToolTipPlacementTarget.VerticalPlacementTargetType {
			get { return BarItemLinkControlToolTipPlacementTargetType.External; }
		}
		double IToolTipPlacementTarget.HorizontalOffset {
			get { return 0; }
		}
		double IToolTipPlacementTarget.VerticalOffset {
			get { return ToolTipVerticalOffset; }
		}
		DependencyObject IToolTipPlacementTarget.ExternalPlacementTarget {
			get { return this; }
		}
		#endregion
		protected internal virtual void CloseRegularPopup() {
			RegularPopup.Do(x=>x.ClosePopup());
		}
	}
	public class RadialMenuKeyboardNavigationManager : NavigationManager {
		protected RadialMenuControl RadialMenuControl { get; set; }
		public RadialMenuKeyboardNavigationManager(RadialMenuControl radialMenuControl)
			: base(radialMenuControl) {
			RadialMenuControl = radialMenuControl;
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(RadialMenuControl.ProcessNativeKeyDown(e.Key, Keyboard.Modifiers)) {
				e.Handled = true;
				return;
			}			
			base.ProcessKeyDown(e);			
		}
	}
	public class RadialMenuCenterButton : Button {
		public RadialMenuCenterButton() {
			PopupMenuManager.SetIgnorePopupItemClickBehaviour(this, true);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
		}
	}
}

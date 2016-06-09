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
using DevExpress.Xpf.Bars;
using System.Collections.Generic;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using DevExpress.Mvvm.Native;
using System.Windows.Documents;
using System.Windows.Input;
using System;
using System.Windows.Media;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	public enum RibbonNavigationLevelType { Headers = 0, Toolbar, Page, PageCaptions, PageGroup, PageGroupCaption, ToolbarInPopup, Menu, Popup, PageInPopup, PageCaptionsInPopup, Gallery, GalleryMenu, BackstageView }
	public enum RibbonKeyTipLevelType { Headers = 0, Page, Popup }
	public class RibbonNavigationLevelElements : List<DependencyObject> { };
	public class RibbonKeyTipPresenters : List<RibbonKeyTipPresenter> { };
	public class RibbonNavigationLevel {
		static RibbonNavigationLevel empty;
		public static RibbonNavigationLevel Empty { get { return empty ?? (empty = new RibbonNavigationLevel()); } }
		public object Owner { get; set; }
		public RibbonNavigationLevelElements Elements { get; set; }
		public RibbonNavigationLevelType NavigationLevelType { get; set; }
		public RibbonKeyTipLevelType KeyTipLevelType { get; set; }
		public bool VerticalLoop { get; set; }
		public bool HorizontalLoop { get; set; }
		public DependencyObject SelectedElement { get; set; }
		public RibbonKeyTipList KeyTips { get; set; }
		public RibbonKeyTipPresenters Presenters { get; set; }
		public bool IsKeyTipsVisible { get; set; }
	}
	public class RibbonNavigationManager : DependencyObject {
		#region static
		internal readonly Locker closePopupLocker = new Locker();
		public static readonly DependencyProperty SelectedElementProperty;		
		public static readonly DependencyProperty IsNavigationModeProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty IsKeyTipsVisibleProperty;
		static RibbonNavigationManager() {
			SelectedElementProperty = DependencyPropertyManager.Register("SelectedElement", typeof(DependencyObject), typeof(RibbonNavigationManager),
				new FrameworkPropertyMetadata(null, OnSelectedElementPropertyChanged));
			IsNavigationModeProperty = DependencyPropertyManager.Register("IsNavigationMode", typeof(bool), typeof(RibbonNavigationManager),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsNavigationModePropertyChanged)));
			IsSelectedProperty = DependencyPropertyManager.RegisterAttached("IsSelected", typeof(bool), typeof(RibbonNavigationManager), new FrameworkPropertyMetadata(false));
			IsKeyTipsVisibleProperty = DependencyPropertyManager.Register("IsKeyTipsVisible", typeof(bool), typeof(RibbonNavigationManager),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsKeyTipsVisiblePropertyChanged)));
		}
		static protected void OnIsNavigationModePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonNavigationManager)o).OnIsNavigationModeChanged((bool)e.OldValue);
		}
		static protected void OnSelectedElementPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonNavigationManager)o).OnSelectedElementChanged(e.OldValue as DependencyObject);
		}
		public static bool GetIsSelected(DependencyObject obj) {
			return (bool)obj.GetValue(IsSelectedProperty);
		}
		public static void SetIsSelected(DependencyObject obj, bool value) {
			obj.SetValue(IsSelectedProperty, value);
		}
		static protected void OnIsKeyTipsVisiblePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((RibbonNavigationManager)o).OnIsKeyTipsVisibleChanged((bool)e.OldValue);
		}	   
		#endregion
		#region dep props
		public bool IsNavigationMode {
			get { return (bool)GetValue(IsNavigationModeProperty); }
			set { SetValue(IsNavigationModeProperty, value); }
		}
		public DependencyObject SelectedElement {
			get { return (DependencyObject)GetValue(SelectedElementProperty); }
			set { SetValue(SelectedElementProperty, value); }
		}
		public bool IsKeyTipsVisible {
			get { return (bool)GetValue(IsKeyTipsVisibleProperty); }
			set { SetValue(IsKeyTipsVisibleProperty, value); }
		}
		#endregion
		RibbonControl ribbon;
		readonly Locker focusLocker = new Locker();
		public RibbonControl Ribbon {
			get { return ribbon; }
			protected set {
				if (value == ribbon) return;
				RibbonControl oldValue = ribbon;
				ribbon = value;
				OnRibbonChanged(oldValue);
			}
		}
		public RibbonNavigationManager(RibbonControl ribbon) {
			Levels = new List<RibbonNavigationLevel>();
			Ribbon = ribbon;
			FilterString = "";
		}
		protected string FilterString { get; private set; }
		protected List<RibbonNavigationLevel> Levels { get; set; }		
		public int LevelCount { get { return Levels.Count; } }
		protected RibbonNavigationLevel CurrentLevel {
			get {
				if(Levels.Count > 0)
					return Levels[Levels.Count - 1];
				return RibbonNavigationLevel.Empty;
			}
		}
		FrameworkElement TopElement { get; set; }
		DependencyObject FocusedElement { get; set; }
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			UnsubscribeRibbonEvents(oldValue);
			SubscribeRibbonEvents(Ribbon);
		}
		protected virtual void SubscribeRibbonEvents(RibbonControl target) {
			if (target == null) return;
			UnsubscribeRibbonEvents(target);
			target.PreviewGotKeyboardFocus += OnRibbonPreviewGotKeyboardFocus;
		}
		protected virtual void UnsubscribeRibbonEvents(RibbonControl target) {
			if (target == null) return;
			target.PreviewGotKeyboardFocus -= OnRibbonPreviewGotKeyboardFocus;
		}		
		protected virtual void OnRibbonPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			if (!Ribbon.AllowRibbonNavigationFromEditorOnTabPress || e.NewFocus as FrameworkElement==null || IsNavigationMode)
				return;
			var linkControl = LayoutHelper.FindLayoutOrVisualParentObject<BarItemLinkControl>((FrameworkElement)e.NewFocus, true);
			if (linkControl == null)
				return;			
			var page = LayoutHelper.FindLayoutOrVisualParentObject<RibbonSelectedPageControl>(linkControl, true);
			var group = LayoutHelper.FindLayoutOrVisualParentObject<RibbonPageGroupControl>(linkControl, true);			
			var groupElements = group==null ? null : GetRibbonPageGroupElements(group);
			var pageElements = page==null ? null : GetPageElements(page);
			var targetElements = pageElements ?? groupElements;
			if (targetElements != null) {
				focusLocker.DoLockedActionIfNotLocked(() => {
					IsNavigationMode = true;
					HideKeyTips();
					CurrentLevel.Elements = targetElements;
					SelectedElement = linkControl;
				});
			}
		}
		protected virtual void OnIsNavigationModeChanged(bool oldValue) {
			if(IsNavigationMode) {
				Ribbon.SetCurrentValue(RibbonControl.IsHiddenRibbonCollapsedProperty, false);
				FocusObserver.SaveFocus(true);
				Ribbon.Focusable = true;
				focusLocker.DoIfNotLocked(() => {					
					Ribbon.Focus();
				});
				RibbonNavigationLevel level = new RibbonNavigationLevel();
				Levels.Add(level);
				if(Ribbon.ApplicationButton != null && Ribbon.ApplicationButton.IsChecked == true) {
					Levels.Insert(0, CreateHeaderKeyTipLevel(new RibbonNavigationLevel()));
					KeyTipGenerator.Generate(Levels[0].KeyTips);
					CreatePresenters(Levels[0]);
					level.HorizontalLoop = false;
					level.VerticalLoop = true;
					level.KeyTipLevelType = RibbonKeyTipLevelType.Popup;
					if(!Ribbon.ApplicationMenuIsPopupMenu()) {
						if(Ribbon.ApplicationMenu is BackstageViewControl) {
							level.Owner = Ribbon.ApplicationMenu; 
							level.Elements = GetBackstageElements(); 
							level.NavigationLevelType = RibbonNavigationLevelType.BackstageView; 
							level.KeyTips = GetBackstageKeyTips();
						} else {
							IsNavigationMode = false;
							return;
						}
					}
					if(Ribbon.ApplicationMenuPopup != null && Ribbon.ApplicationMenuPopup.IsOpen) {
						level.Owner = Ribbon.ApplicationMenuPopup;
						level.NavigationLevelType = RibbonNavigationLevelType.Popup;
						level.Elements = GetLinksControlElements((PopupMenuBarControl)(level.Owner as BarPopupBase).PopupContent);
						level.HorizontalLoop = false;
						level.VerticalLoop = true;
						level.KeyTips = GetLinksControlKeyTips((PopupMenuBarControl)(level.Owner as BarPopupBase).PopupContent);
					}
					ShowKeyTips();
					IsKeyTipsVisible = true;
				} else if(Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Visible) {
					CreateHeaderKeyTipLevel(level);
					if(Ribbon.SelectedPage != null) {
						RibbonCaptionControl ctrl = GetCaptionControlByPage(Ribbon.SelectedPage);
						SelectedElement = ctrl;
					}
					IsKeyTipsVisible = true;
				} else {
					TryNavigateToPage(Dock.Bottom);
					if(Ribbon.SelectedPageControl.ComplexLayoutState == ComplexLayoutState.Updated) {
						OnSelectedPageControlComplexLayoutStateChanged(Ribbon, new ComplexLayoutStateChangedEventArgs(ComplexLayoutState.Updated));
						IsKeyTipsVisible = true;
					}
				}
				Ribbon.SelectedPageControl.ComplexLayoutStateChanged += new ComplexLayoutStateChangedEventHandler(OnSelectedPageControlComplexLayoutStateChanged);
				TopElement = LayoutHelper.FindRoot(Ribbon) as FrameworkElement;
				if(TopElement != null) {
					TopElement.PreviewMouseDown += new MouseButtonEventHandler(OnTopElementPreviewMouseDown);
					TopElement.AddHandler(AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnTopElementAccessKeyPressed), true);
				}
				Ribbon.SizeChanged += new SizeChangedEventHandler(OnRibbonSizeChanged);
			} else {
				Ribbon.ClearValue(RibbonControl.IsHiddenRibbonCollapsedProperty);
				Ribbon.SelectedPageControl.ComplexLayoutStateChanged -= OnSelectedPageControlComplexLayoutStateChanged;
				IsKeyTipsVisible = false;
				while(LevelCount != 0) PopLevel();
				SelectedElement = null;
				if(TopElement != null) {
					TopElement.PreviewMouseDown -= OnTopElementPreviewMouseDown;
					TopElement.RemoveHandler(AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnTopElementAccessKeyPressed));
				}					
				TopElement = null;
				Ribbon.SizeChanged -= OnRibbonSizeChanged;
				FilterString = string.Empty;
				RestoreFocus();
			}
		}
		void OnTopElementAccessKeyPressed(object sender, AccessKeyPressedEventArgs e) {
			if (!string.IsNullOrEmpty(e.Key))
				IsNavigationMode = false;
		}
		protected internal void RestoreFocus() {			
			FocusedElement = null;
			FocusObserver.RestoreFocus(false);
		}
		protected virtual void SetFocus(DependencyObject element) {			
			Keyboard.Focus(element as IInputElement);
		}
		protected RibbonNavigationLevel CreateHeaderKeyTipLevel(RibbonNavigationLevel level) {
			level.Elements = GetRibbonHeaderRow();
			level.NavigationLevelType = RibbonNavigationLevelType.Headers;
			level.HorizontalLoop = true;
			level.KeyTipLevelType = RibbonKeyTipLevelType.Headers;
			level.Owner = null;
			level.KeyTips = GetHeadersKeyTips();			
			return level;
		}
		protected virtual void OnRibbonSizeChanged(object sender, SizeChangedEventArgs e) {
			IsNavigationMode = false;
		}
		protected virtual void OnTopElementPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if(CurrentLevel != null && CurrentLevel.Owner is DependencyObject && e.OriginalSource is DependencyObject) {
				DependencyObject dObj = (DependencyObject)e.OriginalSource;
				DependencyObject owner = (DependencyObject)CurrentLevel.Owner;
				if(LayoutHelper.IsChildElementEx(owner, dObj, true))
					return;
			}
			IsNavigationMode = false;
		}
		protected virtual void OnSelectedPageControlComplexLayoutStateChanged(object sender, ComplexLayoutStateChangedEventArgs e) {			
			if(CurrentLevel.KeyTips == null && e.State == ComplexLayoutState.Updated && LevelCount == 1) {
				CurrentLevel.KeyTips = GetPageKeyTips(Ribbon.SelectedPageControl);
				ShowKeyTips();
			}
		}
		protected virtual void OnIsKeyTipsVisibleChanged(bool oldValue) {
			if(IsKeyTipsVisible) {
				ShowKeyTips();
			}
			else {
				HideKeyTips();
				FilterString = "";
			}
		}
		protected virtual void OnSelectedElementChanged(DependencyObject oldValue) {
			SetElementSelection(oldValue, false);
			CurrentLevel.SelectedElement = SelectedElement;
			SetElementSelection(SelectedElement, true);
			BringIntoView(SelectedElement);
		}
		void BringIntoView(DependencyObject element) {
			if(element is BarItemLinkControlBase) {
				BarItemLinkControlBase linkControl = element as BarItemLinkControlBase;
				if(!linkControl.IsInVisualTree())
					return;
				SubMenuScrollViewer scrollHost = LayoutHelper.FindParentObject<SubMenuScrollViewer>(element);
				if(scrollHost != null) {
					Rect bounds = linkControl.TransformToVisual(scrollHost).TransformBounds(new Rect(0, 0, linkControl.ActualWidth, linkControl.ActualHeight));
					if(bounds.Top < 0) {
						scrollHost.VerticalOffset -= bounds.Top;
					}
					if(bounds.Bottom > scrollHost.ViewportHeight)
						scrollHost.VerticalOffset -= (bounds.Bottom - scrollHost.ActualHeight);
				}
			}
		}
		protected virtual void SetElementSelection(DependencyObject element, bool isSelected) {
			if(element == null) return;
			SetIsSelected(element, isSelected);
			if(element is RibbonPageCaptionControl) {
				if(isSelected == true)
					Ribbon.SelectedPage = ((RibbonPageCaptionControl)element).Page;
			}
			else if(element is RibbonCheckedBorderControl) {
				((RibbonCheckedBorderControl)element).AppFocusValue = isSelected;
			}
			else if(element is ItemBorderControl) {
				((ItemBorderControl)element).State = isSelected ? BorderState.Hover : BorderState.Normal;
			}
			else if(element is RibbonGalleryBarItemLinkControl) {
				((RibbonGalleryBarItemLinkControl)element).DropDownButton.AppFocusValue = isSelected;
			}
			else if(element is GalleryItemControl) {
				((GalleryItemControl)element).IsSelected = isSelected;
			}
			else if(element is BarEditItemLinkControl) {
				if(isSelected) {
					FocusEditor((BarEditItemLinkControl)element);
					FocusedElement = ((BarEditItemLinkControl)element).Edit as DependencyObject;
				} else {
					FocusedElement = null;
					SetFocus(Ribbon);
				}
			}
			if(element is BarItemLinkControl) {
				((BarItemLinkControl)element).IsSelected = isSelected;
			}
			if(element is BackstageItem) {				
				if(element is BackstageTabItem && isSelected) {
					(element as BackstageTabItem).OnClick();
				}
				((BackstageItem)element).ActualIsFocused = isSelected;
			}
		}
		protected void SetKeyTipsVisibilityOnNavigate() {
			if(LevelCount == 1 || (CurrentLevel.NavigationLevelType != RibbonNavigationLevelType.Menu && CurrentLevel.NavigationLevelType != RibbonNavigationLevelType.GalleryMenu
				&& CurrentLevel.NavigationLevelType != RibbonNavigationLevelType.Popup && CurrentLevel.NavigationLevelType != RibbonNavigationLevelType.Gallery))
				IsKeyTipsVisible = false;
		}
		public virtual bool TryShowPopupOnFocusedElement(Key key) {
			if(FocusedElement is PopupBaseEdit) {
				if(!((PopupBaseEdit)FocusedElement).IsPopupOpen && key == Key.Down) {
					((PopupBaseEdit)FocusedElement).ShowPopup();
				}
			}
			return false;
		}
		public virtual bool IsFocusedElementKey(Key key) {
			if(key == Key.Tab || key == Key.Escape)
				return false;
			if(Keyboard.FocusedElement != null && Keyboard.FocusedElement!=Ribbon)
				return true;
			return key != Key.Left && key != Key.Right && key != Key.Down && key != Key.Up && key != Key.Space && key != Key.Enter;
		}
		public virtual bool ProcessChar(string symbol) {
			if(symbol.Length == 0)
				return false;
			char keyValue = symbol.ToUpper()[0];
			bool res = CurrentLevel.IsKeyTipsVisible;
			AddCharToFilterString(keyValue);
			return res;
		}
		public virtual bool Navigate(Key key, int platformKeyCode) {
		if(TryShowPopupOnFocusedElement(key) || IsFocusedElementKey(key))
				return false;
			switch(key) {
				case Key.Left:
					NavigateLeft();
					return true;
				case Key.Up:
					NavigateUp();
					return SelectedElement != null;
				case Key.Right:
					NavigateRight();
					return true;
				case Key.Down:
					NavigateDown();
					return SelectedElement != null;
				case Key.Tab:
					if(KeyboardHelper.IsShiftPressed)
						NavigatePrior();
					else
						NavigateNext();
					return true;
				case Key.Space:
				case Key.Enter:
					ExecuteAction(SelectedElement);
					return true;
				case Key.Back:
					RemoveCharFromFilterString();
					return true;
				case Key.Escape:
					if(CurrentLevel.Owner is BackstageViewControl) {
						(CurrentLevel.Owner as BackstageViewControl).Close();
					}
					if(LevelCount == 1) {
						if(FocusedElement != null) {
							IsNavigationMode = false;
							return true;
						}
						if(CurrentLevel.KeyTipLevelType == RibbonKeyTipLevelType.Headers) {
							IsNavigationMode = false;
							return true;
						}
						if(IsKeyTipsVisible && CurrentLevel.KeyTipLevelType == RibbonKeyTipLevelType.Page) {
							HideKeyTips();
							CurrentLevel.KeyTipLevelType = RibbonKeyTipLevelType.Headers;
							CurrentLevel.KeyTips = GetHeadersKeyTips();
							ShowKeyTips();
						} else {
							IsNavigationMode = false;
						}
					}
					else
						PopLevel();
					return true;
			}
			return false;
		}
		protected void NavigateUp() {
			SetKeyTipsVisibilityOnNavigate();
			DependencyObject nearestElement = GetNearestElementFromList(SelectedElement, CurrentLevel.Elements, Dock.Top);
			if(nearestElement != null) {
				SelectedElement = nearestElement;
				return;
			}
			switch(CurrentLevel.NavigationLevelType) {
				case RibbonNavigationLevelType.Toolbar:
					if(Ribbon.ToolbarShowMode == RibbonQuickAccessToolbarShowMode.ShowBelow) {
						if(!TryNavigateToPageCaptions(Dock.Top))
							if(!TryNavigateToPage(Dock.Top))
								TryNavigateToHeaders(Dock.Top);
					}
					break;
				case RibbonNavigationLevelType.PageCaptions:
					if(!TryNavigateToPage(Dock.Top))
						if(!TryNavigateToHeaders(Dock.Top))
							TryNavigateToToolbarRow(Dock.Top);
					break;
				case RibbonNavigationLevelType.Page:
					if(!TryNavigateToHeaders(Dock.Top))
						TryNavigateToToolbarRow(Dock.Top);
					break;
				case RibbonNavigationLevelType.Headers:
					TryNavigateToToolbarRow(Dock.Top);
					break;
				case RibbonNavigationLevelType.PageGroupCaption:
					TryNavigateToPageGroup();
					break;
				case RibbonNavigationLevelType.PageCaptionsInPopup:
					TryNavigateToPageInPopup();
					break;
				case RibbonNavigationLevelType.Menu:
				case RibbonNavigationLevelType.Popup:
				case RibbonNavigationLevelType.BackstageView:
					if(CurrentLevel.VerticalLoop && CurrentLevel.Elements != null && CurrentLevel.Elements.Count != 0)
						SelectedElement = CurrentLevel.Elements[CurrentLevel.Elements.Count - 1];
					break;
				case RibbonNavigationLevelType.GalleryMenu:
					TryNavigateToGallery();
					break;
			}
		}
		protected void NavigateDown() {
			SetKeyTipsVisibilityOnNavigate();
			DependencyObject nearestElement = GetNearestElementFromList(SelectedElement, CurrentLevel.Elements, Dock.Bottom);
			if(nearestElement != null) {
				SelectedElement = nearestElement;
				return;
			}
			switch(CurrentLevel.NavigationLevelType) {
				case RibbonNavigationLevelType.Toolbar:
					if(Ribbon.ToolbarShowMode == RibbonQuickAccessToolbarShowMode.ShowAbove) {
						TryNavigateToHeaders(Dock.Bottom);
					}
					break;
				case RibbonNavigationLevelType.Headers:
					TryNavigateDownFromPageHeaders();
					break;
				case RibbonNavigationLevelType.Page:
					if(!TryNavigateToPageCaptions(Dock.Bottom))
						TryNavigateToToolbarRow(Dock.Bottom);
					break;
				case RibbonNavigationLevelType.PageCaptions:
					TryNavigateToToolbarRow(Dock.Bottom);
					break;
				case RibbonNavigationLevelType.PageGroup:
					TryNavigateToPageGroupCaption();
					break;
				case RibbonNavigationLevelType.PageInPopup:
					TryNavigateToPageCaptionsInPopup();
					break;
				case RibbonNavigationLevelType.Menu:
				case RibbonNavigationLevelType.Popup:
				case RibbonNavigationLevelType.GalleryMenu:
				case RibbonNavigationLevelType.BackstageView:
					if(CurrentLevel.VerticalLoop && CurrentLevel.Elements != null && CurrentLevel.Elements.Count != 0)
						SelectedElement = CurrentLevel.Elements[0];
					break;
				case RibbonNavigationLevelType.Gallery:
					TryNavigateToGalleryMenu();
					break;
			}
		}
		protected bool TryNavigateDownFromPageHeaders() {
			if (SelectedElement is RibbonPageCaptionControl) {
				return TryNavigateToPage(Dock.Bottom) || TryNavigateToPageCaptions(Dock.Bottom) || TryNavigateToToolbarRow(Dock.Bottom);					
			}
			return false;
		}
		protected void NavigateLeft() {
			if(CurrentLevel.Elements == null || CurrentLevel.Elements.Count == 0) return;
			SetKeyTipsVisibilityOnNavigate();
			if((CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Menu || CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Popup || CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.GalleryMenu) && LevelCount > 2) {
				PopLevel();
				return;
			}
			DependencyObject nearestElement = GetNearestElementFromList(SelectedElement, CurrentLevel.Elements, Dock.Left);
			if(nearestElement == null && CurrentLevel.HorizontalLoop)
				nearestElement = CurrentLevel.Elements[CurrentLevel.Elements.Count - 1];
			if(nearestElement != null) SelectedElement = nearestElement;
		}
		protected void NavigateRight() {
			if(CurrentLevel.Elements == null || CurrentLevel.Elements.Count == 0) return;
			SetKeyTipsVisibilityOnNavigate();
			if((CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Menu || CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Popup || CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.GalleryMenu) && ElementIsPopupContainer(SelectedElement)) {
				ExecuteAction(SelectedElement);
				return;
			}
			DependencyObject nearestElement = GetNearestElementFromList(SelectedElement, CurrentLevel.Elements, Dock.Right);
			if(nearestElement == null && CurrentLevel.HorizontalLoop) nearestElement = CurrentLevel.Elements[0];
			if(nearestElement != null) SelectedElement = nearestElement;
		}
		protected void NavigateNext() {
			if(CurrentLevel.Elements == null || CurrentLevel.Elements.Count == 0) return;			
			SetKeyTipsVisibilityOnNavigate();
			if (TryNavigateDownFromPageHeaders())
				return;
			int index = CurrentLevel.Elements.IndexOf(SelectedElement) + 1;
			if(index == 0) return;
			if(index >= CurrentLevel.Elements.Count) index = 0;
			SelectedElement = CurrentLevel.Elements[index];
		}
		protected void NavigatePrior() {
			if(CurrentLevel.Elements == null || CurrentLevel.Elements.Count == 0) return;
			SetKeyTipsVisibilityOnNavigate();
			int selectedIndex = CurrentLevel.Elements.IndexOf(SelectedElement);
			if(selectedIndex == -1) return;
			int index = selectedIndex - 1;
			if(index < 0) index = CurrentLevel.Elements.Count - 1;
			SelectedElement = CurrentLevel.Elements[index];
		}		
		public virtual void PopLevel() {
			HideKeyTips();
			RibbonNavigationLevel level = CurrentLevel;
			BarPopupBase popup = level.Owner as BarPopupBase;
			if(popup != null) {				
				if(popup.PopupContent is IComplexLayout) {
					((IComplexLayout)popup.PopupContent).ComplexLayoutStateChanged -= OnComplexLayoutStateChanged;
				}
				else {
					popup.Loaded -= OnPopupLoaded;
				}
				popup.Closed -= OnPopupClosed;
				if (popup is GalleryDropDownPopupMenu)
					((GalleryDropDownPopupMenu)popup).CloseWithTimer();
				else {
					if (!closePopupLocker.IsLocked)
						popup.ClosePopup();
				}
			}
			else if(level.Owner is IComplexLayout) {
				((IComplexLayout)level.Owner).ComplexLayoutStateChanged -= OnComplexLayoutStateChanged;
			}
			SelectedElement = null;
			Levels.Remove(CurrentLevel);
			if(LevelCount != 0) {
				FilterString = "";
				SelectedElement = CurrentLevel.SelectedElement;
				CurrentLevel.IsKeyTipsVisible = IsKeyTipsVisible;
				UpdatePresentersVisibility();
			}
			else
				SelectedElement = null;
		}
		protected virtual void PushLevel(RibbonNavigationLevel level) {
			FilterString = "";
			HideKeyTips();
			Levels.Add(level);
			BarPopupBase popup = level.Owner as BarPopupBase;
			if(popup != null) {
				if(popup.PopupContent is IComplexLayout) {
					((IComplexLayout)popup.PopupContent).ComplexLayoutStateChanged += new ComplexLayoutStateChangedEventHandler(OnComplexLayoutStateChanged);
				} else {
					popup.Loaded += new RoutedEventHandler(OnPopupLoaded);
					NavigateFirstElementInPopup();
				}
				popup.Closed += new System.EventHandler(OnPopupClosed);
			}
			else if(level.Owner is IComplexLayout){
				((IComplexLayout)level.Owner).ComplexLayoutStateChanged += new ComplexLayoutStateChangedEventHandler(OnComplexLayoutStateChanged);			
			}
		}
		protected RibbonNavigationLevel FindLevelByPopup(BarPopupBase popup) {
			foreach(RibbonNavigationLevel level in Levels) {
				if(level.Owner == popup) return level;
			}
			return null;
		}
		protected virtual void OnPopupClosed(object sender, System.EventArgs e) {
			RibbonNavigationLevel level = FindLevelByPopup(sender as BarPopupBase);
			if(level == null) return;
			while(CurrentLevel != level) {
				PopLevel();
			}
			PopLevel();
		}
		protected virtual void OnPopupLoaded(object sender, RoutedEventArgs e) {
			NavigateFirstElementInPopup();
		}
		protected virtual void OnComplexLayoutStateChanged(object sender, ComplexLayoutStateChangedEventArgs e) {
			if(e.State == ComplexLayoutState.Updated) {
				if(CurrentLevel.Owner is BarPopupBase)
					NavigateFirstElementInPopup();
				if(CurrentLevel.Owner is BackstageViewControl)
					NavigateFirstElementInBackstageView();
			}
		}
		protected virtual void NavigateFirstElementInPopup() {
			if(CurrentLevel.KeyTips != null) {
				HideKeyTips();
			}
			if(CurrentLevel.Owner is RibbonSelectedPagePopup) {
				RibbonSelectedPageContentPresenter presenter = (CurrentLevel.Owner as BarPopupBase).PopupContent as RibbonSelectedPageContentPresenter;
				if(presenter == null) return;
				RibbonSelectedPageControl page = presenter.Content as RibbonSelectedPageControl;
				if(page == null) return;
				CurrentLevel.Elements = GetPageElements(page);
				CurrentLevel.KeyTips = GetPageKeyTips(page);
				ShowKeyTips();
			}
			else if(CurrentLevel.Owner is RibbonQuickAccessToolbarPopup) {
				CurrentLevel.Elements = GetToolbarPopupElements();
				CurrentLevel.KeyTips = GetToolbarKeyTips(Ribbon.Toolbar.Control.Popup.PopupContent as RibbonQuickAccessToolbarControl);
				ShowKeyTips();
			}
			else if(CurrentLevel.Owner is RibbonPageGroupPopup) {
				CurrentLevel.Elements = GetRibbonPageGroupElements((RibbonPageGroupControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
				CurrentLevel.KeyTips = GetPageGroupKeyTips((RibbonPageGroupControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
				ShowKeyTips();
			}
			else if(CurrentLevel.Owner is QuickAccessToolbarCustomizationMenu) {
				CurrentLevel.Elements = GetLinksControlElements((PopupMenuBarControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
			}
			else if(CurrentLevel.Owner is GalleryDropDownPopupMenu) {
				CurrentLevel.Elements = GetGalleryElements((GalleryDropDownControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
				CurrentLevel.KeyTips = GetLinksControlKeyTips((GalleryDropDownControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
				ShowKeyTips();
			} else if(CurrentLevel.Owner is PopupMenuBase) {
				CurrentLevel.Elements = GetLinksControlElements((PopupMenuBarControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
				CurrentLevel.HorizontalLoop = false;
				CurrentLevel.VerticalLoop = true;
				CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.Menu;
				CurrentLevel.KeyTips = GetLinksControlKeyTips((PopupMenuBarControl)(CurrentLevel.Owner as BarPopupBase).PopupContent);
				ShowKeyTips();
			} else {
				CurrentLevel.Elements = new RibbonNavigationLevelElements();
				CurrentLevel.KeyTips = new RibbonKeyTipList();
			}
			SelectedElement = (CurrentLevel.Elements != null && CurrentLevel.Elements.Count != 0) ? CurrentLevel.Elements[0] : null;
		}
		protected virtual void NavigateFirstElementInBackstageView() {			
			CurrentLevel.Elements =  GetBackstageElements();
			CurrentLevel.HorizontalLoop = false;
			CurrentLevel.VerticalLoop = true;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.BackstageView;
			CurrentLevel.KeyTips = GetBackstageKeyTips();
			ShowKeyTips();
			SelectedElement = (CurrentLevel.Elements != null && CurrentLevel.Elements.Count != 0) ? CurrentLevel.Elements[0] : null;
		}
		protected bool SelectedElementIsEditor(DependencyObject element) {
			return element is BarEditItemLinkControl;
		}
		protected bool ElementIsCaptionButton(DependencyObject element) {
			RibbonPageGroupControl group = LayoutHelper.FindParentObject<RibbonPageGroupControl>(element);
			return group != null && group.CaptionButton.Equals(element);
		}
		protected bool ElementIsGallery(DependencyObject element) {
			return element is RibbonGalleryBarItemLinkControl;
		}
		protected bool ElementIsApplicationButton(DependencyObject element) {
			return element is RibbonApplicationButtonControl;
		}
		protected bool ElementIsCollapsedPageGroup(DependencyObject element) {
			RibbonPageGroupControl pageGroup = LayoutHelper.FindParentObject<RibbonPageGroupControl>(element);
			if(pageGroup == null) return false;
			return pageGroup.CollapsedStateBorder.Equals(element);
		}
		protected bool ElementIsToolbarDropDownButton(DependencyObject element) {
			if(element == Ribbon.Toolbar.Control.DropDownButton) {
				return Ribbon.Toolbar.Control.IsDropDownButtonVisible;
			}
			RibbonQuickAccessToolbarControl toolbarInPopup = (RibbonQuickAccessToolbarControl)Ribbon.Toolbar.Control.Popup.PopupContent;
			if(toolbarInPopup != null && element == toolbarInPopup.DropDownButton) {
				return toolbarInPopup.IsDropDownButtonVisible;
			}
			return false;
		}
		protected bool ElementIsToolbarCustomizationButton(DependencyObject element) {			
			if(element == Ribbon.Toolbar.Control.CustomizationButton) {
				return Ribbon.Toolbar.Control.IsCustomizationButtonVisible;
			}
			RibbonQuickAccessToolbarControl toolbarInPopup = (RibbonQuickAccessToolbarControl)Ribbon.Toolbar.Control.Popup.PopupContent;
			if(toolbarInPopup != null &&  element == toolbarInPopup.CustomizationButton) {
				return toolbarInPopup.IsCustomizationButtonVisible;
			}
			return false;
		}
		protected bool ElementIsHeaderOfMinimizedRibbon(DependencyObject element) {
			return element is RibbonPageCaptionControl && CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Headers && Ribbon.IsMinimized;
		}
		protected bool ElementIsBarSplitButton(DependencyObject element) {
			return element is BarSplitButtonItemLinkControl;
		}
		protected bool ElementIsBarSplitCheckItem(DependencyObject element) {
			return element is BarSplitCheckItemLinkControl;
		}
		protected bool ElementIsBarSubItem(DependencyObject element) {
			return element is BarSubItemLinkControl;
		}
		protected bool ElementIsPopupContainer(DependencyObject element) {
			return ElementIsBarSplitButton(element) || ElementIsBarSplitCheckItem(element) || ElementIsBarSubItem(element);
		}
		#region actions
		public virtual void ExecuteAction(DependencyObject element) {
			if(SelectedElement is BarItemLinkControl) {
			}
			if(ElementIsApplicationButton(element)) {
				ExecuteActionForApplicationButton();
			}
			else if(ElementIsHeaderOfMinimizedRibbon(element)) {
				ExecuteActionForHeaderOfMinimizedRibbon(element);
			}
			else if(ElementIsCollapsedPageGroup(element)) {
				ExecuteActionForCollapsedPageGroup(element);
			}
			else if(ElementIsToolbarCustomizationButton(element)) {
				ExecuteActionForToolbarCustomizationButton();
			}
			else if(ElementIsToolbarDropDownButton(element)) {
				ExecuteActionForToolbarDropDownButton();
			}
			else if(ElementIsBarSubItem(element)) {
				ExecuteActionForBarSubItem(element);
			}
			else if(ElementIsBarSplitButton(element)) {
				ExecuteActionForBarSplitButton(element);
			}
			else if(ElementIsBarSplitCheckItem(element)) {
				ExecuteActionForBarSplitCheckItem(element);
			}
			else if(ElementIsGallery(element)) {
				ExecuteActionForGallery(element);
			}
			else if(ElementIsCaptionButton(element)) {
				ExecuteActionForCaptionButton(element);
			}
			else if(SelectedElementIsEditor(element)) {
				ExecuteActionForEditor(element);
			}
			else if(element is BarItemLinkControl) {
				((BarItemLinkControl)element).Link.Item.PerformClick();
				IsNavigationMode = false;
			}
			else if(element is BackstageButtonItem) {
				IsNavigationMode = false;
				((BackstageButtonItem)element).OnClick();				
			}
			else if(element is BackstageTabItem) {
				IsNavigationMode = false;
				((BackstageTabItem)element).OnClick();				
			}
		}
		protected void ExecuteActionForToolbarDropDownButton() {
			Ribbon.Toolbar.Control.IsPopupOpened = true;
			RibbonNavigationLevel level = new RibbonNavigationLevel();
			level.HorizontalLoop = true;
			level.NavigationLevelType = RibbonNavigationLevelType.ToolbarInPopup;
			level.KeyTipLevelType = RibbonKeyTipLevelType.Popup;
			level.Owner = Ribbon.Toolbar.Control.Popup;
			level.HorizontalLoop = true;
			PushLevel(level);
		}
		protected void ExecuteActionForToolbarCustomizationButton() {
			Ribbon.Toolbar.Control.ShowCustomizationMenu();
			RibbonNavigationLevel level = new RibbonNavigationLevel();
			level.HorizontalLoop = true;
			level.NavigationLevelType = RibbonNavigationLevelType.Menu;
			level.KeyTipLevelType = RibbonKeyTipLevelType.Popup;
			level.Owner = Ribbon.Toolbar.Control.CustomizationMenu;
			level.HorizontalLoop = true;
			PushLevel(level);
		}
		protected void ExecuteActionForEditor(DependencyObject element) {
			SelectedElement = element;
			IsKeyTipsVisible = false; 
			FocusEditor(element as BarEditItemLinkControl);
		}
		protected void FocusEditor(BarEditItemLinkControl editItem) {
			if(editItem == null) return;
			BaseEdit editor = editItem.Edit ?? editItem.TemplatedEdit;
			UIElement elem = (UIElement)editor ?? editItem;
			if(elem != null) {
				Keyboard.Focus(elem);
			}
		}
		protected void ExecuteActionForCaptionButton(DependencyObject element) {
			RibbonPageGroupControl group = LayoutHelper.FindParentObject<RibbonPageGroupControl>(element);
			if(group == null) return;
			group.OnCaptionButtonClick();
			IsNavigationMode = false;
		}
		protected void ExecuteActionForGallery(DependencyObject element) {
			RibbonGalleryBarItemLinkControl lc = element as RibbonGalleryBarItemLinkControl;
			lc.ShowDropDownGallery();
			if(lc.PopupGallery.IsOpen) {
				RibbonNavigationLevel level = new RibbonNavigationLevel();
				level.KeyTipLevelType = RibbonKeyTipLevelType.Popup;				
				level.HorizontalLoop = true;
				level.VerticalLoop = true;
				level.NavigationLevelType = RibbonNavigationLevelType.Gallery;
				level.KeyTipLevelType = RibbonKeyTipLevelType.Popup;
				level.Owner = lc.PopupGallery;
				PushLevel(level);
			}
		}
		protected void ExecuteActionForApplicationButton() {
			Ribbon.ShowApplicationMenu();
			if(!Ribbon.ApplicationMenuIsPopupMenu()) {
				if(Ribbon.ApplicationMenu is BackstageViewControl) {
					RibbonNavigationLevel level = new RibbonNavigationLevel();
					level.VerticalLoop = true;
					level.NavigationLevelType = RibbonNavigationLevelType.BackstageView;
					level.Owner = Ribbon.ApplicationMenu;
					PushLevel(level);
				}
				else {
					IsNavigationMode = false;
					return;
				}
			}
			if(Ribbon.ApplicationMenuPopup != null && Ribbon.ApplicationMenuPopup.IsOpen) {
				RibbonNavigationLevel level = new RibbonNavigationLevel();
				level.HorizontalLoop = true;
				level.NavigationLevelType = RibbonNavigationLevelType.Popup;
				level.Owner = Ribbon.ApplicationMenuPopup;
				PushLevel(level);
			}
		}
		protected void ExecuteActionForHeaderOfMinimizedRibbon(DependencyObject element) {
			SelectedElement = element;			
			RibbonNavigationLevel level = new RibbonNavigationLevel();
			level.HorizontalLoop = true;
			level.NavigationLevelType = RibbonNavigationLevelType.PageInPopup;
			level.KeyTipLevelType = RibbonKeyTipLevelType.Popup;
			level.Owner = Ribbon.SelectedPagePopup;
			PushLevel(level);
			Ribbon.OpenPopup();
		}
		protected void ExecuteActionForCollapsedPageGroup(DependencyObject element) {
			RibbonPageGroupControl group = LayoutHelper.FindParentObject<RibbonPageGroupControl>(element);
			group.OpenPopup(true);
			RibbonNavigationLevel level = new RibbonNavigationLevel();
			level.HorizontalLoop = true;
			level.NavigationLevelType = RibbonNavigationLevelType.PageGroup;
			level.Owner = group.PopupGroup;
			level.HorizontalLoop = true;
			PushLevel(level);
		}
		protected void ExecuteActionForBarSplitButton(DependencyObject element) {
			BarSplitButtonItemLinkControl lc = element as BarSplitButtonItemLinkControl;
			lc.ShowPopup();
			if(lc.IsPopupOpen) {
				RibbonNavigationLevel level = new RibbonNavigationLevel();
				level.NavigationLevelType = RibbonNavigationLevelType.Popup;
				level.Owner = BarManagerHelper.GetItemLinkControlPopup(lc) as BarPopupBase;
				PushLevel(level);
			}
		}
		protected void ExecuteActionForBarSubItem(DependencyObject element) {
			BarSubItemLinkControl lc = element as BarSubItemLinkControl;
			lc.ShowPopup();
			if(lc.IsPopupOpen) {
				RibbonNavigationLevel level = new RibbonNavigationLevel();
				level.NavigationLevelType = RibbonNavigationLevelType.Popup;
				level.Owner = BarManagerHelper.GetItemLinkControlPopup(lc) as BarPopupBase;
				CurrentLevel.SelectedElement = element;
				PushLevel(level);
			}
		}
		protected void ExecuteActionForBarSplitCheckItem(DependencyObject element) {
			BarSplitCheckItemLinkControl lc = element as BarSplitCheckItemLinkControl;
			lc.ShowPopup();
			if(lc.IsPopupOpen) {
				RibbonNavigationLevel level = new RibbonNavigationLevel();
				level.NavigationLevelType = RibbonNavigationLevelType.Popup;
				level.Owner = BarManagerHelper.GetItemLinkControlPopup(lc) as BarPopupBase;
				PushLevel(level);
			}
		}
		#endregion
		protected virtual bool TryNavigateToToolbarRow(Dock direction) {		   
			if(direction == Dock.Bottom && Ribbon.ToolbarShowMode != RibbonQuickAccessToolbarShowMode.ShowBelow) return false;
			if(direction == Dock.Top && Ribbon.ToolbarShowMode != RibbonQuickAccessToolbarShowMode.ShowAbove) return false;
			if(Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Collapsed)
				return false;
			RibbonNavigationLevelElements toolbarRow = GetRibbonToolbarElements();
			if(toolbarRow.Count == 0) return false;
			DependencyObject obj = GetNearestElementFromList(SelectedElement, toolbarRow, direction);
			if(obj == null) obj = toolbarRow[toolbarRow.Count - 1];
			CurrentLevel.Elements = toolbarRow;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.Toolbar;
			SelectedElement = obj;
			return true;
		}
		protected virtual bool TryNavigateToPage(Dock direction) {
			if(Ribbon.IsMinimized)
				return false;
			RibbonNavigationLevelElements elements = GetPageElements(Ribbon.SelectedPageControl);
			if(elements.Count == 0)
				return false;
			DependencyObject obj = direction == Dock.Top ? GetNearestElementFromList(SelectedElement, elements, direction) : elements[0];
			CurrentLevel.Elements = elements;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.Page;
			SelectedElement = obj;
			return true;
		}
		protected virtual bool TryNavigateToPageInPopup() {
			RibbonSelectedPageControl page = Ribbon.SelectedPagePopup.PopupContent as RibbonSelectedPageControl;
			RibbonNavigationLevelElements elements = GetPageElements(page);
			if(elements.Count == 0) return false;
			DependencyObject obj =  obj = GetNearestElementFromList(SelectedElement, elements, Dock.Top);
			if(obj == null) obj = elements[0];
			CurrentLevel.Elements = elements;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.PageInPopup;
			SelectedElement = obj;
			return true;
		}
		protected virtual bool TryNavigateToPageGroup() {
			RibbonPageGroupControl group = (RibbonPageGroupControl)(CurrentLevel.Owner as BarPopupBase).PopupContent;
			RibbonNavigationLevelElements list = GetRibbonPageGroupElements(group);
			if(list.Count == 0) return false;			
			CurrentLevel.Elements = list;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.PageGroup;
			SelectedElement = list[0];
			return true;
		}
		protected virtual bool TryNavigateToPageGroupCaption() {
			RibbonPageGroupControl group = (RibbonPageGroupControl)(CurrentLevel.Owner as BarPopupBase).PopupContent;
			if(!group.PageGroup.ShowCaptionButton) return false;
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			list.Add(group.CaptionButton);
			CurrentLevel.Elements = list;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.PageGroupCaption;
			SelectedElement = group.CaptionButton;
			return true;
		}
		protected virtual bool TryNavigateToPageCaptions(Dock direction) {
			RibbonSelectedPageControl page = Ribbon.SelectedPageControl;
			if(Ribbon.IsMinimized) return false;
			RibbonNavigationLevelElements elements = GetPageCaptionsElements(page);
			if(elements.Count == 0) return false;
			DependencyObject obj = null;
			if(direction == Dock.Bottom) {
				RibbonPageGroupControl groupControl = LayoutHelper.FindParentObject<RibbonPageGroupControl>(SelectedElement);
				if(groupControl != null) {
					if(groupControl.IsCollapsed) return false;
					if(groupControl.PageGroup.ShowCaptionButton) {
						obj = groupControl.CaptionButton;
					}
				}
			}
			if(obj == null) obj = elements[0];
			CurrentLevel.Elements = elements;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.PageCaptions;
			SelectedElement = obj;
			return true;
		}
		protected virtual bool TryNavigateToPageCaptionsInPopup() {
			RibbonSelectedPageControl page = Ribbon.SelectedPagePopup.PopupContent as RibbonSelectedPageControl;
			RibbonNavigationLevelElements elements = GetPageCaptionsElements(page);
			if(elements.Count == 0) return false;
			RibbonPageGroupControl groupControl = LayoutHelper.FindParentObject<RibbonPageGroupControl>(SelectedElement);
			DependencyObject obj = null;
			if(groupControl != null && groupControl.PageGroup.ShowCaptionButton) {
					obj = groupControl.CaptionButton;
			}			
			if(obj == null) obj = elements[0];
			CurrentLevel.Elements = elements;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.PageCaptionsInPopup;
			SelectedElement = obj;
			return true;
		}
		protected virtual bool TryNavigateToGalleryMenu() {
			GalleryDropDownControl gallery = (GalleryDropDownControl)(CurrentLevel.Owner as BarPopupBase).PopupContent;
			RibbonNavigationLevelElements elements = GetLinksControlElements(gallery);
			if(elements.Count == 0) return false;
			CurrentLevel.Elements = elements;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.GalleryMenu;
			CurrentLevel.VerticalLoop = false;
			CurrentLevel.HorizontalLoop = false;
			SelectedElement = elements[0];
			return true;
		}
		protected virtual bool TryNavigateToGallery() {
			GalleryDropDownControl gallery = (GalleryDropDownControl)(CurrentLevel.Owner as BarPopupBase).PopupContent;
			RibbonNavigationLevelElements elements = GetGalleryElements(gallery);
			if(elements.Count == 0) return false;
			CurrentLevel.Elements = elements;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.Gallery;
			CurrentLevel.VerticalLoop = false;
			CurrentLevel.HorizontalLoop = false;
			SelectedElement = elements[elements.Count - 1];
			return true;
		}
		protected virtual bool TryNavigateToHeaders(Dock direction) {
			if(Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Collapsed)
				return false;
			RibbonNavigationLevelElements row = GetRibbonHeaderRow();
			if(row.Count == 0) return false;
			DependencyObject nearestElement = null;
			if(direction == Dock.Top) {
				nearestElement = GetCaptionControlByPage(Ribbon.SelectedPage);
			}
			else {
				nearestElement = GetNearestElementFromList(SelectedElement, row, direction);
			}
			if(nearestElement == null) nearestElement = row[0];
			CurrentLevel.Elements = row;
			CurrentLevel.NavigationLevelType = RibbonNavigationLevelType.Headers;
			CurrentLevel.HorizontalLoop = true;
			SelectedElement = nearestElement;
			return true;
		}
		protected RibbonCaptionControl GetCaptionControlByPage(RibbonPage page) {
			RibbonPageCategoryBase pageCategory = page.MergedParentCategory == null ? page.PageCategory : page.MergedParentCategory;
			RibbonPageCategoryControl cat = Ribbon.GetPageCategoryControl(pageCategory);
			RibbonPageHeaderControl header = (RibbonPageHeaderControl)cat.ItemContainerGenerator.ContainerFromItem(page);
			return header.With(h => h.CaptionControl);
		}
		protected RibbonNavigationLevelElements GetRibbonHeaderRow() {
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			list.Add(Ribbon.ApplicationButton);
			for(int i = 0; i < Ribbon.ActualCategories.Count; i++) {
				RibbonPageCategoryControl cat = Ribbon.GetPageCategoryControl(i);
				if(!cat.PageCategory.IsVisible) continue;
				for(int j = 0; j < cat.Items.Count; j++) {
					RibbonPageHeaderControl header = (RibbonPageHeaderControl)cat.ItemContainerGenerator.ContainerFromIndex(j);
					if(!header.Page.IsVisible)
						continue;
					list.Add(header.CaptionControl);
				}
			}
			list.AddRange(GetLinksControlElements(Ribbon.PageHeaderLinksControl));
			return list;
		}
		protected BarItemLinkControlBase GetLinkControl(LinksControl linksControl, int index) {
			BarItemLinkInfo info = linksControl.ItemContainerGenerator.ContainerFromIndex(index) as BarItemLinkInfo;
			if(info == null)
				return null;
			return info.LinkControl;				
		}
		protected RibbonNavigationLevelElements GetLinksControlElements(LinksControl linksControl) {
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			for(int i = 0; i < linksControl.Items.Count; i++) {
				BarItemLinkControlBase lc = GetLinkControl(linksControl, i);
				if(lc == null) continue;
				if(lc.Visibility == Visibility.Collapsed || lc.Opacity == 0 || !(lc is BarItemLinkControl) || !lc.IsEnabled
					|| (lc is BarStaticItemLinkControl) || (lc is BarItemLinkSeparatorControl))
						continue;
				if(lc is BarButtonGroupLinkControl) {
					list.AddRange(GetLinksControlElements(((BarButtonGroupLinkControl)lc).ItemsControl));
				}
				else list.Add(lc);
			}
			return list;
		}
		protected RibbonNavigationLevelElements GetBackstageElements() {
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			BackstageViewControl backstageView = CurrentLevel.Owner as BackstageViewControl;
			foreach(object item in backstageView.Items) {
				BackstageItemBase backstageItem = backstageView.ItemContainerGenerator.ContainerFromItem(item) as BackstageItemBase;
				if(backstageItem == null || backstageItem is BackstageSeparatorItem)
					continue;
				list.Add(backstageItem);
			}
			return list;
		}
		protected RibbonNavigationLevelElements GetGalleryElements(GalleryDropDownControl control) {
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			GalleryItemGroupsControl groups = control.GalleryControl.GetGroupsControl();
			for(int i = 0; i < groups.Items.Count; i++) {
				GalleryItemGroupControl group = groups.ItemContainerGenerator.ContainerFromIndex(i) as GalleryItemGroupControl;
				for(int j = 0; j < group.Items.Count; j++) {
					GalleryItemControl item = group.ItemContainerGenerator.ContainerFromIndex(j) as GalleryItemControl;
					list.Add(item);
				}
			}
			return list;
		}
		protected RibbonNavigationLevelElements GetRibbonToolbarElements() {
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			list.AddRange(GetLinksControlElements(Ribbon.Toolbar.Control));
			if(Ribbon.Toolbar.Control.IsCustomizationButtonVisible) {
				list.Add(Ribbon.Toolbar.Control.CustomizationButton);
			}
			if(Ribbon.Toolbar.Control.IsDropDownButtonVisible) {
				list.Add(Ribbon.Toolbar.Control.DropDownButton);
			}
			return list;
		}
		protected RibbonNavigationLevelElements GetToolbarPopupElements() {
			RibbonQuickAccessToolbarPopup popup = CurrentLevel.Owner as RibbonQuickAccessToolbarPopup;
			RibbonQuickAccessToolbarControl control = (RibbonQuickAccessToolbarControl)popup.PopupContent;
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			list.AddRange(GetLinksControlElements(control));
			if(control.IsCustomizationButtonVisible) {
				list.Add(control.CustomizationButton);
			}
			if(control.IsDropDownButtonVisible) {
				list.Add(control.DropDownButton);
			}
			return list;
		}
		protected RibbonNavigationLevelElements GetPageElements(RibbonSelectedPageControl page) {
			RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
			for(int i = 0; i < page.GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = page.GetPageGroupControlFromIndex(i);
				if (group == null) continue;
				list.AddRange(GetRibbonPageGroupElements(group));
			}
			return list;
		}
		protected RibbonNavigationLevelElements GetRibbonPageGroupElements(RibbonPageGroupControl groupControl) {
			if(!groupControl.IsCollapsed)
				return GetLinksControlElements(groupControl);
			else {
				RibbonNavigationLevelElements list = new RibbonNavigationLevelElements();
				list.Add(groupControl.CollapsedStateBorder);
				return list;
			}		   
		}
		double CalcDistance(Size elementSize, Rect listElementRect) {
			double dx = elementSize.Width / 2 - listElementRect.X - listElementRect.Width / 2;
			double dy = elementSize.Height / 2 - listElementRect.Y - listElementRect.Height / 2;
			return dx * dx + dy * dy;
		}
		protected RibbonNavigationLevelElements GetPageCaptionsElements(RibbonSelectedPageControl page) {
			RibbonNavigationLevelElements elements = new RibbonNavigationLevelElements();
			if(page == null) return elements;
			for(int i = 0; i < page.GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = page.GetPageGroupControlFromIndex(i);
				if(group == null || group.PageGroup == null || group.CaptionButton==null) continue;
				if(group.PageGroup.ShowCaptionButton && !group.IsCollapsed)
					elements.Add(group.CaptionButton);
			}
			return elements;
		}
		protected DependencyObject GetNearestElementFromList(DependencyObject element, RibbonNavigationLevelElements elements, Dock searchDirection) {
			if(! (element is UIElement)) return null;
			DependencyObject nearestElement = null;
			double minDistance = double.MaxValue;
			double distance = 0;
			Size elementSize = ((UIElement)element).DesiredSize;
			foreach(DependencyObject listElement in elements) {
				if(listElement == element || !(listElement is UIElement)) continue;
				Rect listElementRect = ((UIElement)listElement).TransformToVisual((UIElement)element).TransformBounds(new Rect(0, 0, ((UIElement)listElement).RenderSize.Width, ((UIElement)listElement).RenderSize.Height));
				switch(searchDirection) {
					case Dock.Bottom:
						if(listElementRect.X + listElementRect.Width < 0 || listElementRect.X > elementSize.Width || listElementRect.Y <= 0) continue;
						break;
					case Dock.Left:
						if(listElementRect.Y + listElementRect.Height <= 0 || listElementRect.Y >= elementSize.Height || listElementRect.X >= 0) continue;
						break;
					case Dock.Right:
						if(listElementRect.Y + listElementRect.Height <= 0 || listElementRect.Y >= elementSize.Height || listElementRect.X <= 0) continue;
						break;
					case Dock.Top:
						if(listElementRect.X + listElementRect.Width < 0 || listElementRect.X > elementSize.Width || listElementRect.Y >= 0) continue;						
						break;
				}
				distance = CalcDistance(elementSize, listElementRect);					   
				if(distance < minDistance) {
					minDistance = distance;
					nearestElement = listElement;
				}
			}
			return nearestElement;
		}
		#region KeyTips
		protected void ShowKeyTips() {
			CurrentLevel.IsKeyTipsVisible = true;
			KeyTipGenerator.Generate(CurrentLevel.KeyTips);
			CreatePresenters(CurrentLevel);
			UpdatePresentersVisibility();
		}
		protected void HideKeyTips() {
			CurrentLevel.IsKeyTipsVisible = false;
			UpdatePresentersVisibility();
		}
		protected void CreatePresenters(RibbonNavigationLevel level) {
			level.Presenters = new RibbonKeyTipPresenters();
			foreach(RibbonKeyTip keyTip in level.KeyTips) {
				if(keyTip.IsVisible) {
					RibbonKeyTipPresenter presenter = new RibbonKeyTipPresenter(keyTip);
					level.Presenters.Add(presenter);
				}
			}
		}
		protected void RemoveCharFromFilterString() {
			if(FilterString.Length == 0) return;
			FilterString = FilterString.Substring(0, FilterString.Length - 1);
			UpdatePresentersVisibility();
		}
		protected void AddCharToFilterString(char filterChar) {
			FilterString += filterChar;
			RibbonKeyTip keyTip = GetKeyTipForExecute();
			if(keyTip != null) {
				if(keyTip.Owner is RibbonPageCaptionControl && !Ribbon.IsMinimized) {
					HideKeyTips();
					CurrentLevel.KeyTips = null;
					FilterString = "";
					RibbonPage page = Ribbon.SelectedPage;
					SelectedElement = keyTip.Owner;
					CurrentLevel.KeyTipLevelType = RibbonKeyTipLevelType.Page;
					if(page == Ribbon.SelectedPage && IsKeyTipsVisible) {
						CurrentLevel.KeyTips = GetPageKeyTips(Ribbon.SelectedPageControl);
						ShowKeyTips();
					}
					return;
				}
				if((ElementIsBarSplitButton(keyTip.Owner) || ElementIsBarSplitCheckItem(keyTip.Owner))) {
					BarSplitButtonItemLinkControl linkControl = keyTip.Owner as BarSplitButtonItemLinkControl;
					if(keyTip.TargetElement is ArrowControl) {
						ExecuteAction(linkControl);
						return;
					}
					if(LevelCount > 1 && (CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Gallery || CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.GalleryMenu
						|| CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Menu || CurrentLevel.NavigationLevelType == RibbonNavigationLevelType.Popup)) {
							if(!linkControl.SplitButtonItem.ActAsDropDown) {
								linkControl.Link.Item.PerformClick();
								IsNavigationMode = false;
								return;
							}
					}
				}
				ExecuteAction(keyTip.Owner);
			}
			else {
				UpdatePresentersVisibility();
			}
		}
		protected void UpdatePresentersVisibility() {
			UpdateActualKeyTipsVisibility();
			UpdatePresenters();
		}
		protected RibbonKeyTip GetKeyTipForExecute() {
			if(LevelCount == 0)
				return null;
			foreach(RibbonKeyTip keyTip in CurrentLevel.KeyTips)
				if(keyTip.ActualKeyTip == FilterString && keyTip.IsActualVisible && keyTip.IsEnabled)
					return keyTip;
			return null;
		}
		protected void UpdateActualKeyTipsVisibility() {
			if(CurrentLevel.KeyTips == null)
				return;
			foreach(RibbonKeyTip keyTip in CurrentLevel.KeyTips) {
				var owner = keyTip.Owner as FrameworkElement;
				keyTip.IsActualVisible = owner != null && owner.IsVisible && owner.ActualWidth > 0 && keyTip.ActualKeyTip != null && FilterString != null && (keyTip.IsVisible != false) && !FilterString.StartsWith(keyTip.ActualKeyTip) && keyTip.ActualKeyTip.StartsWith(FilterString) && IsKeyTipsVisible && CurrentLevel.IsKeyTipsVisible;
			}
		}
		protected void UpdatePresenters() {
			var action = new Action(() => {
				if(CurrentLevel.Presenters == null)
					return;
				foreach (RibbonKeyTipPresenter presenter in CurrentLevel.Presenters) {
					if (presenter.KeyTip.IsActualVisible && (CurrentLevel.Owner as Popup).If(x => !x.IsOpen).ReturnSuccess())
						return;
					presenter.UpdateView();
				}
			});
			if (Ribbon.IsMinimized && !Ribbon.SelectedPagePopup.wasOpened && IsNavigationMode)
				Dispatcher.BeginInvoke(action);
			else
				action();
		}
		protected RibbonKeyTipList GetPageGroupKeyTips(RibbonPageGroupControl groupControl) {
			RibbonKeyTipList list = GetLinksControlKeyTips(groupControl);
			list.Add(CreateKeyTipForCaptionButton(groupControl));
			if(groupControl.IsCollapsed) {
				foreach(RibbonKeyTip keyTip in list)
					keyTip.IsVisible = false;
			}
			list.Add(CreateKeyTipForCollapsedPageGroup(groupControl));
			return list;
		}
		protected RibbonKeyTip CreateKeyTipForCaptionButton(RibbonPageGroupControl groupControl) {
			RibbonKeyTip keyTip = new RibbonKeyTip();
			keyTip.KeyTip = groupControl.PageGroup.KeyTip;
			keyTip.Owner = groupControl.CaptionButton;
			keyTip.TargetElement = groupControl.CaptionButton;
			keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
			keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.CaptionGroupRow;
			keyTip.IsVisible = groupControl.PageGroup.ShowCaptionButton;
			keyTip.IsEnabled = true;
			return keyTip;
		}
		protected RibbonKeyTip CreateKeyTipForCollapsedPageGroup(RibbonPageGroupControl groupControl) {
			RibbonKeyTip keyTip = new RibbonKeyTip();
			keyTip.KeyTip = groupControl.PageGroup.KeyTipGroupExpanding;
			keyTip.Owner = groupControl.CollapsedStateBorder;
			keyTip.TargetElement = groupControl;
			keyTip.IsVisible = groupControl.IsCollapsed;
			keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
			keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipBottomAtTargetBottom;
			keyTip.IsEnabled = true;
			return keyTip;
		}
		protected RibbonKeyTipList GetPageKeyTips(RibbonSelectedPageControl page) {
			RibbonKeyTipList list = new RibbonKeyTipList();
			for(int i = 0; i < page.GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = page.GetPageGroupControlFromIndex(i);
				if (group == null || !group.IsVisible)
					continue;
				list.AddRange(GetPageGroupKeyTips(group));
			}
			return list;
		}
		protected RibbonKeyTipList GetHeadersKeyTips() {
			RibbonKeyTipList list = new RibbonKeyTipList();
			list.Add(CreateKeyTipForApplicationButton());
			list.AddRange(GetCaptionsKeyTips());
			list.AddRange(GetToolbarKeyTips(Ribbon.Toolbar.Control));
			return list;
		}
#if DEBUGTEST
		internal static RibbonKeyTipList GetLinksControlKeyTipsCore(LinksControl linksControl) {
			return new RibbonNavigationManager(null).GetLinksControlKeyTips(linksControl);
		}
#endif
		protected RibbonKeyTipList GetLinksControlKeyTips(LinksControl linksControl) {
			RibbonKeyTipList list = new RibbonKeyTipList();
			for(int i = 0; i < linksControl.Items.Count; i++) {
				BarItemLinkControlBase lc = GetLinkControl(linksControl, i);
				if(!(lc is BarItemLinkControl)  || (lc is BarStaticItemLinkControl) || (lc is BarItemLinkSeparatorControl) || !lc.IsVisible() || lc.LinkInfo.Item == null)
					continue;
				if(lc is BarButtonGroupLinkControl) {
					RibbonKeyTipList buttonGroupControlKeyTips = GetLinksControlKeyTips(((BarButtonGroupLinkControl)lc).ItemsControl);
					buttonGroupControlKeyTips.ForEach((keyTip) => { keyTip.IsEnabled &= (((BarButtonGroupLinkControl)lc).LinkBase.IsEnabled & ((BarButtonGroupLinkControl)lc).Link.Item.IsEnabled); });
					list.AddRange(buttonGroupControlKeyTips);
				} else {
					list.AddRange(CreateKeyTipsForLinkControl(lc as BarItemLinkControl));
				}
			}
			return list;
		}
		protected RibbonKeyTipList CreateKeyTipsForLinkControl(BarItemLinkControl lc) {
			RibbonKeyTipList list = new RibbonKeyTipList();
			RibbonKeyTip keyTip = new RibbonKeyTip();
			var itemKeyTip = lc.With(x => x.LinkInfo).With(x => x.Item).With(x => x.KeyTip).IfNot(string.IsNullOrEmpty);
			var linkKeyTip = lc.With(x => x.Link).With(x => x.KeyTip).IfNot(string.IsNullOrEmpty);
			keyTip.KeyTip = linkKeyTip ?? itemKeyTip ?? string.Empty;
			keyTip.Caption = lc.Link.Item.Content as string;
			keyTip.IsVisible = lc.Link.Item.IsVisible;
			keyTip.IsEnabled = lc.Link.Item.IsEnabled & lc.Link.IsEnabled;
			keyTip.TargetElement = lc;
			keyTip.Owner = lc;
			keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipCenterAtTargetBottom;
			keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
			if(lc.ContainerType == LinkContainerType.ApplicationMenu || lc.ContainerType == LinkContainerType.DropDownGallery ||
				lc.ContainerType == LinkContainerType.Menu  || (lc.ContainerType == LinkContainerType.RibbonPageGroup && lc.CurrentRibbonStyle != RibbonItemStyles.Large)
				|| lc.ContainerType == LinkContainerType.BarButtonGroup) {
				keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetRight;
				FrameworkElement targetElement = BarManagerHelper.GetItemLinkControlGlyph(lc);
				if (targetElement == null || (targetElement.ActualHeight == 0 && targetElement.ActualWidth == 0))
					targetElement = (FrameworkElement)BarManagerHelper.GetItemLinkControlGlyphBorder(lc);
				if (targetElement == null) {
					targetElement = (FrameworkElement)BarManagerHelper.GetItemLinkControlGlyph(lc);
					keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
					keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipCenterAtTargetCenter;
				}
				keyTip.TargetElement = targetElement ?? lc;
			}
			if(lc.ContainerType == LinkContainerType.RibbonPageGroup || lc.ContainerType == LinkContainerType.BarButtonGroup) {
				keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.AutoRow;
			}
			if(lc.ContainerType == LinkContainerType.ApplicationMenu || lc.ContainerType == LinkContainerType.Menu || lc.ContainerType == LinkContainerType.DropDownGallery) {
				if (lc is BarSplitButtonItemLinkControl &&  !((BarSplitButtonItemLinkControl)lc).ActualActAsDropDown) {
					list.Add(CreateKeyTipForArrow(lc));
				}
				if (lc is BarSplitCheckItemLinkControl &&  !((BarSplitCheckItemLinkControl)lc).ActualActAsDropDown) {
					list.Add(CreateKeyTipForArrow(lc));
				}
			}
			if(lc is BarEditItemLinkControl) {
				keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
				keyTip.TargetElement = lc;
			}
			if(lc is RibbonGalleryBarItemLinkControl) {
				keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.BottomRow;
				keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetRight;
				keyTip.TargetElement = ((RibbonGalleryBarItemLinkControl)lc).DropDownButton;
				keyTip.IsVisible = lc.Link.Item.IsVisible;
			}
			list.Add(keyTip);
			return list;
		}
		protected RibbonKeyTip CreateKeyTipForArrow(BarItemLinkControl lc) {
			RibbonKeyTip keyTip = new RibbonKeyTip();
			keyTip.TargetElement = BarManagerHelper.GetItemLinkControlArrow(lc);
			keyTip.Owner = lc;
			keyTip.KeyTip = lc.Link.KeyTipDropDown;
			keyTip.IsEnabled = lc.Link.Item.IsEnabled;
			keyTip.IsVisible = lc.Link.Item.IsVisible;
			keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipLeftAtTargetCenter;
			keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipTopAtTargetCenter;
			return keyTip;
		}
		protected RibbonKeyTipList GetToolbarKeyTips(RibbonQuickAccessToolbarControl toolbar) {
			RibbonKeyTipList list = new RibbonKeyTipList();
			list.AddRange(GetLinksControlKeyTips(toolbar));
			for(int i = 0; i < list.Count; i++) {
				list[i].IsVisible = (i >= toolbar.FirstVisibleItemIndex) && (i < toolbar.FirstVisibleItemIndex + toolbar.VisibleItemsCount);
			}
			RibbonKeyTip keyTip = new RibbonKeyTip();
			keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipRightAtTargetCenter;
			keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipTopAtTargetCenter;
			keyTip.IsEnabled = true;
			keyTip.IsVisible = toolbar.IsDropDownButtonVisible;
			keyTip.TargetElement = toolbar.DropDownButton;
			keyTip.Owner = toolbar.DropDownButton;
			list.Add(keyTip);
			return list;
		}
		protected RibbonKeyTip CreateKeyTipForApplicationButton() {
			RibbonKeyTip keyTip = new RibbonKeyTip();
			keyTip.TargetElement = Ribbon.ApplicationButton;
			keyTip.Owner = Ribbon.ApplicationButton;
			keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
			keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipCenterAtTargetCenter;
			keyTip.IsEnabled = true;
			keyTip.IsVisible = Ribbon.ShowApplicationButton && Ribbon.RibbonHeaderVisibility == RibbonHeaderVisibility.Visible;
			keyTip.KeyTip = Ribbon.KeyTipApplicationButton;			
			return keyTip;
		}
		protected RibbonKeyTipList GetCaptionsKeyTips() {
			RibbonKeyTipList list = new RibbonKeyTipList();
			for(int i = 0; i < Ribbon.ActualCategories.Count; i++) {
				var cateogory = Ribbon.ActualCategories[i];
				RibbonPageCategoryControl cat = Ribbon.GetPageCategoryControl(i);
				if(cat == null || !cat.IsVisible)
					continue;
				for(int j = 0; j < cateogory.ActualPages.Count; j++) {
					RibbonPage page = cateogory.ActualPages[j];
					RibbonPageHeaderControl pageHeader = (RibbonPageHeaderControl)cat.ItemContainerGenerator.ContainerFromItem(page);
					if (!pageHeader.IsVisible)
						continue;
					RibbonKeyTip keyTip = new RibbonKeyTip();
					keyTip.IsEnabled = true;
					keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter;
					keyTip.IsVisible = page.IsVisible && cateogory.IsVisible;
					keyTip.KeyTip = page.KeyTip;
					keyTip.Caption = page.Caption as string;
					keyTip.VerticalPlacement = (Ribbon.IsMinimized && Ribbon.SelectedPagePopup.IsOpen) ? RibbonKeyTipVerticalPlacement.KeyTipCenterAtTargetCenter : RibbonKeyTipVerticalPlacement.KeyTipTopAtTargetBottom;
					if(pageHeader != null) {
						keyTip.TargetElement = pageHeader.CaptionControl.ContentViewport;
						keyTip.Owner = pageHeader.CaptionControl;
					}					
					list.Add(keyTip);
				}
			}
			return list;
		}
		protected RibbonKeyTipList GetBackstageKeyTips() {
			RibbonKeyTipList list = new RibbonKeyTipList();
			BackstageViewControl backstageView = Ribbon.ApplicationMenu as BackstageViewControl;
			if(backstageView == null) return list;
			foreach(object obj in backstageView.Items) {
				var item = backstageView.ItemContainerGenerator.ContainerFromItem(obj) as FrameworkElement;
				if(!(item is BackstageItemBase)) {
					item = LayoutHelper.FindElement(item, elem => elem is BackstageItemBase);
				}
				BackstageItem owner = item as BackstageItem;
				if(owner == null)
					continue;
				RibbonKeyTip keyTip = new RibbonKeyTip();
				keyTip.Owner = owner;
				keyTip.IsEnabled = true;
				keyTip.IsVisible = owner.IsVisible;
				if(owner.Content is string)
					keyTip.Caption = owner.Content as string;
				keyTip.KeyTip = (item as BackstageItem).KeyTip;
				if(keyTip.Owner is BackstageButtonItem) {
					keyTip.TargetElement = (keyTip.Owner as BackstageButtonItem).IconContainer;
					keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipLeftAtTargetCenter;
					keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipBottomAtTargetCenter;
				} else if(keyTip.Owner is BackstageTabItem) {
					keyTip.TargetElement = (keyTip.Owner as BackstageTabItem).ContentContainer;
					keyTip.HorizontalPlacement = RibbonKeyTipHorizontalPlacement.KeyTipRightAtTargetRight;
					keyTip.VerticalPlacement = RibbonKeyTipVerticalPlacement.KeyTipBottomAtTargetTop;
				} else {
					keyTip.TargetElement = owner;
					keyTip.IsVisible = false;
				}
				list.Add(keyTip);
			}
			return list;
		}
		#endregion
	}
	public class KeyTipControl : Control {
		#region static
		public static readonly DependencyProperty TextProperty;
		static KeyTipControl() {
			TextProperty = DependencyPropertyManager.Register("Text", typeof(string), typeof(KeyTipControl),
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		#endregion
		#region dep props
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		#endregion
		public KeyTipControl() {
			DefaultStyleKey = typeof(KeyTipControl);			
		}
	}
	public class KeyTipToolTip : ToolTip {
		public KeyTipToolTip() {
			DefaultStyleKey = typeof(KeyTipToolTip);
		}		
	}
	public class RibbonKeyTipPresenter : Adorner {
		public RibbonKeyTipPresenter(RibbonKeyTip keyTip)
			: base(keyTip.TargetElement as UIElement) {
			KeyTipControl = new KeyTipControl();
			AddVisualChild(KeyTipControl);
			KeyTipControl.SizeChanged += new SizeChangedEventHandler(OnKeyTipControlSizeChanged);
			KeyTip = keyTip;
		}
		void OnKeyTipControlSizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidateMeasure();
		}
		KeyTipControl KeyTipControl { get; set; }
		public RibbonKeyTip KeyTip {
			get { return keyTip; }
			protected set {
				if(keyTip == value) return;
				var oldValue = keyTip;
				keyTip = value;
				OnKeyTipChanged(oldValue);
			}
		}
		void OnKeyTipChanged(RibbonKeyTip oldValue) {
			if(KeyTip == null)
				ClearValue(IsEnabledProperty);
			else
				KeyTipControl.SetBinding(IsEnabledProperty, new Binding() { Source = KeyTip, Path = new PropertyPath("IsEnabled") });
		}
		public virtual void UpdateView() {
			KeyTipControl.Visibility = KeyTip.IsActualVisible ? Visibility.Visible : Visibility.Collapsed;
			KeyTipControl.Text = KeyTip.ActualKeyTip;
			if(KeyTip.IsActualVisible)
				Show();
			else
				Hide();
		}
		void UpdateKeyTipControlOpacity() {
			double opacity = 1;
			if(KeyTip.Owner is BarItemLinkControlBase) {
				BarItemLinkControl ctrl = KeyTip.Owner as BarItemLinkControl;
				SubMenuScrollViewer scrollHost = LayoutHelper.FindParentObject<SubMenuScrollViewer>(ctrl);
				if(scrollHost != null) {
					Rect bounds = ctrl.TransformToVisual(scrollHost).TransformBounds(new Rect(0, 0, ctrl.ActualWidth, ctrl.ActualHeight));
					if((bounds.Top + bounds.Bottom) / 2 >= scrollHost.ViewportHeight || bounds.Bottom <= 0)
						opacity = 0;
				}
			}
			KeyTipControl.Opacity = opacity;
		}
		protected override Size MeasureOverride(Size constraint) {
			KeyTipControl.Measure(constraint);
			return KeyTipControl.DesiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect location = new Rect(KeyTip.HorizontalOffset, KeyTip.VerticalOffset, KeyTipControl.DesiredSize.Width, KeyTipControl.DesiredSize.Height);
			UpdateKeyTipControlOpacity();
			RibbonPageGroupControl group = LayoutHelper.FindParentObject<RibbonPageGroupControl>(KeyTip.TargetElement);
			Thickness Margin = new Thickness();
			if(AdornedElement is FrameworkElement) Margin = ((FrameworkElement)AdornedElement).Margin;
			RibbonKeyTipVerticalPlacement actualVerticalPlacement = KeyTip.VerticalPlacement;
			if(actualVerticalPlacement == RibbonKeyTipVerticalPlacement.AutoRow) {
				if(KeyTip.Owner is BarItemLinkControl && group != null) {
					BarItemLinkControl lc = KeyTip.Owner as BarItemLinkControl;
					if(lc.CurrentRibbonStyle == RibbonItemStyles.Large)
						actualVerticalPlacement = RibbonKeyTipVerticalPlacement.BottomRow;
					else {
						switch(RibbonPageGroupItemsPanel.GetRow(lc)) {
							case 0:
								actualVerticalPlacement = RibbonKeyTipVerticalPlacement.TopRow;
								break;
							case 1:
								actualVerticalPlacement = RibbonKeyTipVerticalPlacement.CenterRow;
								break;
							case 2:
								actualVerticalPlacement = RibbonKeyTipVerticalPlacement.BottomRow;
								break;
						}
					}
				}
			}
			switch(actualVerticalPlacement) {
				case RibbonKeyTipVerticalPlacement.KeyTipCenterAtTargetCenter:
					location.Y += (AdornedElement.DesiredSize.Height - Margin.Top - Margin.Bottom - KeyTipControl.DesiredSize.Height) / 2;
					break;
				case RibbonKeyTipVerticalPlacement.KeyTipCenterAtTargetBottom:
					location.Y += AdornedElement.DesiredSize.Height - Margin.Top - Margin.Bottom - KeyTipControl.DesiredSize.Height / 2;
					break;
				case RibbonKeyTipVerticalPlacement.KeyTipTopAtTargetBottom:
					location.Y += AdornedElement.DesiredSize.Height - Margin.Top - Margin.Bottom;
					break;
				case RibbonKeyTipVerticalPlacement.KeyTipBottomAtTargetTop:
					location.Y -= KeyTipControl.DesiredSize.Height;
					break;
				case RibbonKeyTipVerticalPlacement.KeyTipBottomAtTargetCenter:
					location.Y += (AdornedElement.DesiredSize.Height - Margin.Top - Margin.Bottom)/2 - KeyTipControl.DesiredSize.Height;
					break;
				case RibbonKeyTipVerticalPlacement.TopRow:
				case RibbonKeyTipVerticalPlacement.BottomRow:
				case RibbonKeyTipVerticalPlacement.CenterRow:
					if(group != null) {
						location.Y += group.GetKeyTipRowOffset(actualVerticalPlacement, AdornedElement) - KeyTipControl.DesiredSize.Height / 2;
					}
					break;
				case RibbonKeyTipVerticalPlacement.CaptionGroupRow:
					if(group != null) {
						location.Y += group.GetKeyTipRowOffset(actualVerticalPlacement, AdornedElement);
					}
					break;
			}
			switch(KeyTip.HorizontalPlacement) {
				case RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetCenter:
					location.X += (AdornedElement.DesiredSize.Width - Margin.Left - Margin.Right - KeyTipControl.DesiredSize.Width) / 2;
					break;
				case RibbonKeyTipHorizontalPlacement.KeyTipLeftAtTargetCenter:
					location.X += (AdornedElement.DesiredSize.Width - Margin.Left - Margin.Right) / 2;
					break;
				case RibbonKeyTipHorizontalPlacement.KeyTipCenterAtTargetRight:
					location.X += AdornedElement.DesiredSize.Width - Margin.Left - Margin.Right - KeyTipControl.DesiredSize.Width / 2;
					break;
				case RibbonKeyTipHorizontalPlacement.KeyTipRightAtTargetRight:
					location.X -= KeyTipControl.DesiredSize.Width;
					break;
			}
			KeyTipControl.Arrange(location);
			return finalSize;
		}
		protected override System.Windows.Media.Visual GetVisualChild(int index) {
			return KeyTipControl;
		}
		protected override int VisualChildrenCount {
			get {
				return 1;
			}
		}		
		AdornerLayer AdornerLayer { get; set; }
		protected void Show() {
			if(AdornerLayer != null)
				AdornerLayer.Remove(this);
			var owner = KeyTip.Owner as FrameworkElement;
			if(FrameworkElementHelper.GetIsLoaded(owner)) {
				UpdateAdornerLayer(owner);
			} else {
				owner.Loaded += OnOwnerLoaded;
			}
		}
		void OnOwnerLoaded(object sender, RoutedEventArgs e) {
			var fe = sender as FrameworkElement;
			fe.Loaded -= OnOwnerLoaded;
			UpdateAdornerLayer(fe);
		}
		void UpdateAdornerLayer(FrameworkElement fe) {
			AdornerLayer = FindAdornerLayer(fe);
			if(AdornerLayer != null)
				AdornerLayer.Add(this);
		}
		protected void Hide() {
			if(AdornerLayer != null)
				AdornerLayer.Remove(this);
			AdornerLayer = null;
		}
		AdornerLayer FindAdornerLayer(UIElement targetElement) {
			RibbonControl ribbonControl = LayoutHelper.FindParentObject<RibbonControl>(KeyTip.Owner);
			if(ribbonControl != null) {
				return AdornerLayer.GetAdornerLayer(ribbonControl.Manager ?? (UIElement)ribbonControl);
			}
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(KeyTip.Owner as UIElement);
			if(layer == null) return null;
			UIElement parent = LayoutHelper.GetParent(layer) as UIElement;
			while(AdornerLayer.GetAdornerLayer(parent) != null) {
				layer = AdornerLayer.GetAdornerLayer(parent);
				parent = LayoutHelper.GetParent(layer) as UIElement;
			}
			return layer;
		}
		RibbonKeyTip keyTip;
	}
}

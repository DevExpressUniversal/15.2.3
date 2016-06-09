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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Globalization;
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
namespace DevExpress.Xpf.Bars {
	public enum ContextMenuOpenReason { Custom, RightMouseButtonClick, LeftMouseButtonClick, Tap, Keyboard, MouseEnter };
	public static class ContextMenuManager {
		public static bool GetHasDXContextMenu(DependencyObject obj) {
			return (bool)obj.GetValue(HasDXContextMenuProperty);
		}
		public static void SetHasDXContextMenu(DependencyObject obj, bool value) {
			obj.SetValue(HasDXContextMenuProperty, value);
		}
		public static readonly DependencyProperty HasDXContextMenuProperty =
			DependencyPropertyManager.RegisterAttached("HasDXContextMenu", typeof(bool), typeof(ContextMenuManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnHasDXContextMenuPropertyChanged));
		static void OnHasDXContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			UpdateContexMenu(d, (bool)e.NewValue);
		}
		static void UpdateContexMenu(DependencyObject d, bool hasDXContextMenu) {
			if(d == null || d.GetValue(ContextMenuService.ContextMenuProperty) != null)
				return;
			if(IsDXContextMenuSet(d)) {
				d.SetValue(ContextMenu.ContextMenuProperty, null);
			} else if(d.IsPropertySet(ContextMenuService.ContextMenuProperty))
				d.ClearValue(ContextMenuService.ContextMenuProperty);
		}
		static bool IsDXContextMenuSet(DependencyObject editor) {
			if(!IsEditor(editor))
				return false;
			BaseEdit dxEdit = GetTemplatedParentEditor(editor);
			if(dxEdit == null)
				return GetContextMenu(editor) != null;
			 var holder = GetContextMenuHolder(editor);
			 return GetHasDXContextMenu(editor) && dxEdit.EditMode != EditMode.InplaceInactive && (holder == dxEdit || holder == editor);
		}
		public static int hashCode;
		public static BarPopupBase GetContextMenu(object contextElement) {
			IPopupControl popupControl = null;
			if(contextElement is UIElement)
				popupControl = BarManager.GetDXContextMenu((UIElement)contextElement);
			if(popupControl == null)
				return null;
			return popupControl.Popup;
		}
		public static void SetBarManager(DependencyObject contextElement, BarManager manager) {
			BarPopupBase popup = GetContextMenu(contextElement);
			if(popup == null) {
				return;
			}
		}		
		public static void RegistryContextElement(object contextElement, IPopupControl contextMenu) {
			if (Equals(null, contextElement) || Equals(null, contextMenu))
				return;
			contextMenu.ContextElement = new WeakReference(contextElement);
			SetHasDXContextMenu((DependencyObject)contextElement, true);
		}
		public static void UnregistryContextElement(object contextElement) {
			BarPopupBase contextMenu = GetContextMenu(contextElement);
			if(contextMenu != null) {				
				((IPopupControl)contextMenu).ContextElement = null;
				((DependencyObject)contextElement).ClearValue(BarManager.DXContextMenuProperty);
			}
			SetHasDXContextMenu((DependencyObject)contextElement, false);
		}
		public static bool ShowElementContextMenu(object contextElement, ContextMenuOpenReason openReason = ContextMenuOpenReason.Custom) {
			BarPopupBase popup = GetContextMenu(contextElement);
			if(popup == null || popup.Child == null || BarManager.GetDXContextMenu(popup) == popup)
				return false;
			UIElement element = (UIElement)contextElement;
			if(!element.IsEnabled && !ContextMenuService.GetShowOnDisabled(element))
				return false;
			if(BarManagerCustomizationHelper.IsInCustomizationMode(element))
				return false;
			PrepareContextMenu(element, popup, openReason);			
			if(popup.IsOpen)
				popup.SetCurrentValue(BarPopupBase.IsOpenProperty, false);
			popup.SetCurrentValue(BarPopupBase.IsOpenProperty, true);
			return popup.IsOpen;
		}
		static void SetContextMenuPosition(BarPopupBase contextMenu, Point position) {
			contextMenu.HorizontalOffset = position.X;
			contextMenu.VerticalOffset = position.Y;
		}
		static void PrepareContextMenu(UIElement contextElement, BarPopupBase contextMenu, ContextMenuOpenReason openReason) {
			if(!(contextMenu is RadialContextMenu)) {
				if(openReason == ContextMenuOpenReason.Keyboard && BarManager.GetDXContextMenuPlacement(contextElement) == PlacementMode.Bottom) {
					contextMenu.Placement = PlacementMode.Bottom;
				} else {
					contextMenu.Placement = PlacementMode.MousePoint;
				}
			} else {
				((RadialContextMenu)contextMenu).OpenReason = openReason;
			}
			contextMenu.PlacementTarget = contextElement;
			contextMenu.IsBranchHeader = true;
			contextMenu.ItemClickBehaviour = PopupItemClickBehaviour.CloseCurrentBranch;
		}		
		internal static void OnLeftClickContextMenuOpening(object sender, MouseButtonEventArgs e) {
			if(IsHashCodeNotRepeated(sender)) {
				object parentWithMenu = FindLeftMenu(e.Source);
				if(parentWithMenu != null)
					e.Handled = OpenContextMenu(parentWithMenu, ContextMenuOpenReason.LeftMouseButtonClick);
			}
		}
		internal static object FindLeftMenu(object sender) {
			FrameworkElement current = sender as FrameworkElement;
			while(current != null) {
				if(IsMenuEnabled(current, ButtonSwitcher.LeftButton))
					return current;
				current = current.GetParent() as FrameworkElement;
			}
			return null;
		}
		static WeakReference _openedMenu;
		internal static IPopupControl OpenedMenu {
			get {
				if(_openedMenu == null || !_openedMenu.IsAlive)
					return null;
				return _openedMenu.Target as IPopupControl;
			}
			set {
				if(OpenedMenu != value) {
					_openedMenu = new WeakReference(value);
				}
			}
		}
		internal static void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
			if(CheckIsDXContextMenuOpened()) {
				e.Handled = true;
				return;
			}
			DependencyObject dObj = sender as DependencyObject;
			if (dObj == null || HasStandardContextMenu(dObj) || HasStandardContextMenu(e.Source) || !IsMenuEnabled(sender, ButtonSwitcher.RightButton) || !GetHasDXContextMenu(dObj))
				return;
			e.Handled = OpenContextMenu(dObj, e.CursorLeft == -1 ? ContextMenuOpenReason.Keyboard : ContextMenuOpenReason.RightMouseButtonClick);
		}
		static bool HasStandardContextMenu(object source) {
			return source is DependencyObject && ContextMenuService.GetContextMenu((DependencyObject)source) != null;
		}
		static bool CheckIsDXContextMenuOpened() {
			if(OpenedMenu == null || OpenedMenu.ContextElement == null || !OpenedMenu.ContextElement.IsAlive)
				return false;
			UIElement uiElem = OpenedMenu.ContextElement.Target as UIElement;
			if(uiElem == null)
				return false;
			IPopupControl contextMenu = BarManager.GetDXContextMenu(uiElem);
			return contextMenu != null && contextMenu.IsPopupOpen;
		}
		static UIElement GetContextMenuHolder(DependencyObject from) {
			UIElement result = from as UIElement;
			while(result != null && BarManager.GetDXContextMenu(result) == null) {
				result = VisualTreeHelper.GetParent(result) as UIElement;
			}
			return result;
		}
		static bool IsEditor(object elem) {
			return elem is TextBoxBase || elem is PasswordBox;
		}
		static BaseEdit GetTemplatedParentEditor(DependencyObject from) {
			FrameworkElement nextParent = from as FrameworkElement;
			while(nextParent != null) {
				if(nextParent is BaseEdit)
					return (BaseEdit)nextParent;
				nextParent = nextParent.TemplatedParent as FrameworkElement;
			}
			return null;
		}
		static bool AllowSwitcher(UIElement elem, ButtonSwitcher button) {
			ButtonSwitcher allow = BarManager.GetMenuShowMouseButton(elem);
			if(allow == ButtonSwitcher.LeftRightButton)
				return true;
			return allow == button;
		}
		static bool IsMenuEnabled(object sender, ButtonSwitcher button) {
				UIElement elem = sender as UIElement;
				return (elem != null && BarManager.GetDXContextMenu(elem) != null && AllowSwitcher(elem, button));
		}
		static bool IsHashCodeNotRepeated(object sender) {
			int code = sender.GetHashCode();
			if(hashCode != code) {
				hashCode = code;
				return true;
			}
			return false;
		}
		static bool OpenContextMenu(object sender, ContextMenuOpenReason openReason) {
			BarPopupBase popup = GetContextMenu(sender);
			if(popup != null) popup.AllowMouseCapturing = true;
			return ShowElementContextMenu(sender, openReason);
		}
	}
}
namespace DevExpress.Xpf.Bars.Native {
	public static class TextEditorHelper {
		static readonly Type textEditorType;
		static readonly Func<object, object> getTextEditor;
		static readonly Action<object, bool> setIsContextMenuOpen;
		static readonly Func<object, bool> getIsContextMenuOpen;
		static TextEditorHelper() {
			textEditorType = typeof(TextBoxBase).Assembly.GetType("System.Windows.Documents.TextEditor");
			getTextEditor = ReflectionHelper.CreateInstanceMethodHandler<Func<object, object>>(null, "_GetTextEditor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, textEditorType);
			setIsContextMenuOpen = ReflectionHelper.CreateInstanceMethodHandler<Action<object, bool>>(null, "set_IsContextMenuOpen", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, textEditorType);
			getIsContextMenuOpen = ReflectionHelper.CreateInstanceMethodHandler<Func<object, bool>>(null, "get_IsContextMenuOpen", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, textEditorType);
		}
		public static void SetIsContextMenuOpen(object obj, bool value) {
			var textEditor = getTextEditor(obj);
			if (textEditor == null)
				return;
			setIsContextMenuOpen(textEditor, true);
		}
		public static bool GetIsContextMenuOpen(object obj) {
			var textEditor = getTextEditor(obj);
			if (textEditor == null)
				return false;
			return getIsContextMenuOpen(textEditor);
		}
	}
}

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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Bars.Native {
	class CommandSourceService : ICommandSourceService {
		ItemCommandSourceStrategy strategy;
		public CommandSourceService() { }
		public CommandSourceService(ItemCommandSourceStrategy strategy) {
			this.strategy = strategy;
		}
		void ICommandSourceService.KeyGestureChanged(BarItem element, KeyGesture oldValue, KeyGesture newValue) {
			if (strategy == null) return;
			strategy.KeyGestureChanged(element, oldValue, newValue);
		}
		void ICommandSourceService.CommandChanged(BarItem element, ICommand oldValue, ICommand newValue) {
			if (strategy == null) return;
			strategy.CommandChanged(element, oldValue, newValue);
		}
	}
	public class ItemCommandSourceStrategy : IBarNameScopeDecorator {
		BarNameScope scope;
		ElementRegistrator itemRegistrator;
		bool CanAttachKeyGestures { get { return ScopeTarget.ReturnSuccess(); } }
		bool CanAttachToRoot { get { return TopElement.ReturnSuccess(); } }
		UIElement ScopeTarget { get; set; }
		UIElement TopElement { get; set; }
		InputBindingCollection InputBindings { get { return ScopeTarget.With(x => x.InputBindings); } }
		bool IsVisible { get { return ScopeTarget.Return(x => x.IsVisible, () => false); } }
		KeyGestureWorkingMode KeyGestureWorkingMode { get { return scope.With(x=>x.Target).With(BarManager.GetBarManager).Return(x => x.KeyGestureWorkingMode, () => KeyGestureWorkingMode.UnhandledKeyGesture); } }
		bool LockKeyGestureEventAfterHandling { get { return scope.With(x => x.Target).With(BarManager.GetBarManager).Return(x => x.LockKeyGestureEventAfterHandling, () => false); } }
		public ItemCommandSourceStrategy() {
		}
		public virtual void KeyGestureChanged(BarItem element, KeyGesture oldValue, KeyGesture newValue) {
			UpdateBarItemComand(element);
		}
		public virtual void CommandChanged(BarItem element, ICommand oldValue, ICommand newValue) {
			element.UnhookCommand(oldValue);
			element.HookCommand(newValue);
		}
		protected virtual void OnItemRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e) {
			var item = (BarItem)e.Element;
			switch (e.ChangeType) {
				case ElementRegistratorChangeType.ElementAdded:
					item.HookCommand(item.Command);
					RegisterBarItemCommand(item);
					break;
				case ElementRegistratorChangeType.ElementRemoved:
					item.UnhookCommand(item.Command);
					UnregisterBarItemCommand(item);
					break;
			}
		}
		KeyGestureWorkingMode GetKeyGestureWorkingMode(DependencyObject element) {
			if (element == null)
				return KeyGestureWorkingMode.UnhandledKeyGesture;
			if (BarManager.GetKeyGestureWorkingMode(element).HasValue)
				return BarManager.GetKeyGestureWorkingMode(element).Value;
			return GetKeyGestureWorkingMode(LogicalTreeHelper.GetParent(element) ?? (element as Visual).With(VisualTreeHelper.GetParent));
		}
		protected virtual void OnScopeTargetPreviewKeyDown(object sender, KeyEventArgs e) {
			CheckExecuteGesture(e, KeyGestureWorkingMode.AllKeyGesture);
		}
		protected virtual void OnTopElementPreviewKeyDown(object sender, KeyEventArgs e) {
			CheckExecuteGesture(e, KeyGestureWorkingMode.AllKeyGestureFromRoot);
		}
		protected virtual void OnScopeTargetKeyDown(object sender, KeyEventArgs e) {
			CheckExecuteGesture(e, KeyGestureWorkingMode.UnhandledKeyGesture);
		}
		protected virtual void RegisterBarItemCommand(BarItem item) {
			if (!CanAttachKeyGestures || item.KeyGesture == null) return;
			foreach (KeyBinding keyBinding in InputBindings) {
				if (!(keyBinding.Command is ShortCutCommand)) continue;
				ShortCutCommand command = keyBinding.Command as ShortCutCommand;
				if (command != null && (command.Item == item || (command.BarItemName == item.Name && !string.IsNullOrEmpty(item.Name)))) {
					command.Item = item;
					command.BarItemName = item.Name;
					return;
				}
			}
			InputBindings.Add(new KeyBinding(new ShortCutCommand(item) { BarItemName = item.Name }, item.KeyGesture));
		}
		protected virtual void UnregisterBarItemCommand(BarItem item) {
			if (!CanAttachKeyGestures)
				return;
			KeyBinding kb = GetBarItemCommand(item);
			if (kb != null) {
				InputBindings.Remove(kb);
				ShortCutCommand scCommand = kb.Command as ShortCutCommand;
				if (scCommand != null) {
					scCommand.Item = null;
				}
				kb.Command = null;
			}
		}
		protected bool CheckExecuteGesture(KeyEventArgs e, KeyGestureWorkingMode targetMode) {
			if (!CanAttachKeyGestures)
				return false;
			if (!IsVisible || PopupMenuManager.IsAnyPopupOpened) return false;
			KeyBinding kb = GetKeyBinding(e);
			if (kb == null) return false;
			var scCommand = kb.Command as ShortCutCommand;
			if (scCommand == null || GetKeyGestureWorkingMode(scCommand.Item) != targetMode)
				return false;
			if (kb.Command.CanExecute(null)) {
				kb.Command.Execute(null);
				e.Handled = LockKeyGestureEventAfterHandling;
				return true;
			}
			return false;
		}
		protected void UpdateBarItemComand(BarItem item) {
			if (!CanAttachKeyGestures)
				return;
			KeyBinding kb = GetBarItemCommand(item);
			if (item.KeyGesture == null)
				UnregisterBarItemCommand(item);
			else if (kb != null)
				kb.Gesture = item.KeyGesture;
			else
				RegisterBarItemCommand(item);
		}
		protected KeyBinding GetKeyBinding(KeyEventArgs e) {
			if (!CanAttachKeyGestures)
				return null;
			foreach (InputBinding b in InputBindings) {
				KeyBinding kb = b as KeyBinding;
				if (kb == null || kb.Gesture == null) continue;
				KeyGesture kbg = kb.Gesture as KeyGesture;
#if DEBUGTEST
				var kbModifiers = ModifierKeysHelper.ForcedModifiers.HasValue ? ModifierKeysHelper.ForcedModifiers.Value : ModifierKeysHelper.GetKeyboardModifiers(e);
#else
				var kbModifiers = ModifierKeysHelper.GetKeyboardModifiers(e);				
#endif
				if (kbg.Key == (e.Key == Key.System ? e.SystemKey : e.Key) && kbg.Modifiers == kbModifiers) return kb;
			}
			return null;
		}
		protected void InitializeBarItemCommands() {
			if (!CanAttachKeyGestures)
				return;
			foreach (KeyBinding keyBinding in InputBindings) {
				ShortCutCommand command = keyBinding.Command as ShortCutCommand;
				if (command == null || command.Item != null) continue;
				foreach (BarItem item in itemRegistrator.Values.OfType<BarItem>()) {
					if (item != null && item.Name == command.BarItemName)
						command.Item = item;
				}
			}
		}
		protected KeyBinding GetBarItemCommand(BarItem item) {
			if (!CanAttachKeyGestures)
				return null;
			foreach (InputBinding binding in InputBindings) {
				KeyBinding kb = binding as KeyBinding;
				if (kb == null) continue;
				ShortCutCommand command = kb.Command as ShortCutCommand;
				if (command == null) continue;
				if (command.Item == item)
					return kb;
			}
			return null;
		}
		void IBarNameScopeDecorator.Attach(BarNameScope scope) {
			this.scope = scope;
			this.itemRegistrator = scope[typeof(BarItem)];
			ScopeTarget = GetContainingUIElement(scope.Target);
			var rootScope = scope;
			while (rootScope.Parent != null)
				rootScope = rootScope.Parent;
			TopElement = GetContainingUIElement(rootScope.Target);
			itemRegistrator.Changed += OnItemRegistratorChanged;
			if (CanAttachToRoot) {
				TopElement.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnTopElementPreviewKeyDown), true);
			}
			if (CanAttachKeyGestures) {
				ScopeTarget.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnScopeTargetPreviewKeyDown), true);
				ScopeTarget.KeyDown += OnScopeTargetKeyDown;
			}
		}
		UIElement GetContainingUIElement(DependencyObject obj) {
			return (obj as UIElement) ?? (LayoutHelper.FindLayoutOrVisualParentObject(obj, x => x is UIElement, true) as UIElement);
		}
		void IBarNameScopeDecorator.Detach() {
			if (CanAttachToRoot) {
				TopElement.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnTopElementPreviewKeyDown));
			}
			if (CanAttachKeyGestures) {
				ScopeTarget.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnScopeTargetPreviewKeyDown));
				ScopeTarget.KeyDown -= OnScopeTargetKeyDown;
			}			
			itemRegistrator.Changed -= OnItemRegistratorChanged;
			ScopeTarget = null;
			scope = null;
		}
	}
}

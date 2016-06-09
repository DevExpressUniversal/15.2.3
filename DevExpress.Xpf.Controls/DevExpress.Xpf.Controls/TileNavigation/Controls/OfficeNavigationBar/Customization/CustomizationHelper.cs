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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.Navigation.NavigationBar.Customization {
	public class NavigationBarCustomizationHelper {
		ICommand navigationOptionsCommand;
		ICommand NavigationOptionsCommand {
			get {
				if(navigationOptionsCommand == null) navigationOptionsCommand = DelegateCommandFactory.Create(() => ShowCustomizationForm(Owner));
				return navigationOptionsCommand;
			}
		}
		BarItem navigationOptionsBarItem;
		internal BarItem NavigationOptionsBarItem {
			get {
				if(navigationOptionsBarItem == null) navigationOptionsBarItem = new BarButtonItem() { Command = NavigationOptionsCommand, Content = NavigationLocalizer.GetString(NavigationStringId.CustomizationMenu_NavigationOptions) };
				return navigationOptionsBarItem;
			}
		}
		protected internal FloatingContainer CustomizationForm { get; set; }
		OfficeNavigationBar Owner;
		CustomizationState _State;
		public CustomizationControl CustomizationControl {
			get { return _customizationControl ?? DefaultCustomizationControl; }
			set { _customizationControl = value; }
		}
		CustomizationControl _customizationControl = null;
		CustomizationControl _defaultCustomizationControl = null;
		protected CustomizationControl DefaultCustomizationControl {
			get {
				if(_defaultCustomizationControl == null)
					_defaultCustomizationControl = CreateDefaultCustomizationControl();
				return _defaultCustomizationControl;
			}
		}
		BarManager CreateBarManager() {
			return new NavigationBarManager(Owner);
		}
		private NavigationBarMenu _NavigationBarCustomizationMenu;
		public NavigationBarMenu NavigationBarCustomizationMenu {
			get {
				if(_NavigationBarCustomizationMenu == null) {
					_NavigationBarCustomizationMenu = new NavigationBarMenu(Owner) { };
				}
				return _NavigationBarCustomizationMenu;
			}
		}
		BarManager barManagerCore;
		public BarManager BarManager {
			get {
				if(barManagerCore == null) {
					barManagerCore = CreateBarManager();
				}
				return barManagerCore;
			}
		}
		public bool IsCustomizationFormVisible {
			get {
#if !SILVERLIGHT
				return CustomizationForm != null && CustomizationForm.IsOpen;
#else
				return CustomizationForm != null && CustomizationForm.IsVisible; 
#endif
			}
		}
		List<CustomizationAction> customizationActions = new List<CustomizationAction>();
		List<CustomizationAction> undoActions = new List<CustomizationAction>();
		public NavigationBarCustomizationHelper(OfficeNavigationBar owner) {
			Owner = owner;
		}
		protected virtual CustomizationControl CreateDefaultCustomizationControl() {
			return new CustomizationControl(Owner);
		}
		public void ShowCustomizationForm(ILogicalOwner owner) {
			CustomizationForm = FloatingContainerFactory.Create(FloatingMode.Window);
			CustomizationForm.BeginUpdate();
			CustomizationForm.UseActiveStateOnly = true;
			CustomizationForm.Caption = NavigationLocalizer.GetString(NavigationStringId.CustomizationForm_Caption);
			owner.AddChild(CustomizationForm);
			object obj = DevExpress.Xpf.Core.Native.LayoutHelper.FindRoot(Owner);
			if(obj is Window)
				CustomizationForm.Owner = (FrameworkElement)((Window)obj).Content;
			else if(obj is FrameworkElement)
				CustomizationForm.Owner = (FrameworkElement)obj;
			if(CustomizationForm.Owner == null) CustomizationForm.Owner = Owner;
			CustomizationForm.Content = CustomizationControl;
			CustomizationForm.CloseOnEscape = true;
			CustomizationForm.ShowModal = true;
			CustomizationForm.ContainerStartupLocation = WindowStartupLocation.CenterOwner;
			CustomizationForm.FloatSize = Owner == null ? new Size(325, 275) : Owner.ValuesProvider.CustomizationFormFloatSize;
			CustomizationForm.MinHeight = Owner == null ? 325 : Owner.ValuesProvider.CustomizationFormMinHeight;
			CustomizationForm.MinWidth = Owner == null ? 275 : Owner.ValuesProvider.CustomizationFormMinWidth;
			CustomizationForm.Hidden += OnCustomizationFormHidden;
			CustomizationForm.EndUpdate();
			CustomizationForm.IsOpen = true;
		}
		void OnCustomizationFormHidden(object sender, RoutedEventArgs e) {
			Owner.RemoveChild(CustomizationForm);
			CustomizationForm.Hidden -= OnCustomizationFormHidden;
			CustomizationForm.Content = null;
			CustomizationControl.ClearValue(Control.ForegroundProperty); 
		}
		public void EnqueueCustomizationAction(CustomizationAction action) {
			if(action is ResetCustomizationAction) customizationActions.Clear();
			customizationActions.Add(action);
		}
		public void CancelCustomization() {
			customizationActions.Clear();
		}
		internal CustomizationState GetCustomizationState() {
			return _State;
		}
		int lockCustomizationState = 0;
		bool IsCustomizationStateLocked { get { return lockCustomizationState > 0; } }
		public void ApplyCustomization() {
			var targetCollection = Owner.ItemsSource ?? Owner.Items;
			IList targetList = targetCollection as IList;
			if(targetList == null) targetList = new EnumerableObservableWrapper<object>(targetCollection);
			if(_State == null) _State = new CustomizationState() {OriginalMaxItemsCount = Owner.MaxItemCount };
			if(!_State.IsLockUpdate) {
				_State.BeginUpdate();
				_State.OriginalItems = new ObservableCollection<object>(targetList.Cast<object>());
			}
			var actions = customizationActions.ToList();
			customizationActions.Clear();
			lockCustomizationState++;
			ISupportInitialize supportInitialize = Owner.NavigationClient as ISupportInitialize;
			try {
				if(supportInitialize != null) supportInitialize.BeginInit();
				ILockable lockable = targetList as ILockable;
				if(lockable != null) lockable.BeginUpdate();
				foreach(var action in actions) {
					ResetCustomizationAction resetAction = action as ResetCustomizationAction;
					if(resetAction != null) {
						ResetCustomization();
						continue;
					}
					MoveCustomizationAction moveAction = action as MoveCustomizationAction;
					if(moveAction != null) {
						if(!IsValidIndex(targetList, moveAction.FromIndex) || !IsValidIndex(targetList, moveAction.ToIndex)) continue;
						var item = targetList[moveAction.FromIndex];
						targetList.RemoveAt(moveAction.FromIndex);
						targetList.Insert(moveAction.ToIndex, item);
					}
					PropertyCustomizationAction propertyAction = action as PropertyCustomizationAction;
					if(propertyAction != null) {
						Owner.SetValue(propertyAction.Property, propertyAction.Value);
					}
					undoActions.Insert(0, action);
				}
				if(lockable != null) lockable.EndUpdate();
				if(supportInitialize != null) supportInitialize.EndInit();
			}
			finally {
				lockCustomizationState--;
			}
		}
		void ResetCustomization() {
			var actions = undoActions.ToList();
			undoActions.Clear();
			customizationActions.Clear();
			var targetCollection = Owner.ItemsSource ?? Owner.Items;
			IList targetList = targetCollection as IList;
			if(targetList == null) targetList = new EnumerableObservableWrapper<object>(targetCollection);
			ILockable lockable = targetList as ILockable;
			if(lockable != null) lockable.BeginUpdate();
			foreach(var action in actions) {
				MoveCustomizationAction moveAction = action as MoveCustomizationAction;
				if(moveAction != null) {
					if(!IsValidIndex(targetList, moveAction.FromIndex) || !IsValidIndex(targetList, moveAction.ToIndex)) continue;
					var item = targetList[moveAction.ToIndex];
					targetList.RemoveAt(moveAction.ToIndex);
					targetList.Insert(moveAction.FromIndex, item);
				}
			}
			Owner.MaxItemCount = _State.OriginalMaxItemsCount;
			if(lockable != null) lockable.EndUpdate();
		}
		bool IsValidIndex(IList collection, int index) {
			return index < collection.Count && index >= 0;
		}
		internal void InvalidateState() {
			if(IsCustomizationStateLocked) return;
			if(_State != null)
				_State.EndUpdate();
			undoActions.Clear();
			customizationActions.Clear();
		}
		public void CloseCustomizationForm() {
			CustomizationForm.Close();
		}
		class NavigationBarManager : BarManager {
			public NavigationBarManager(FrameworkElement container) {
				AllowCustomization = false;
				CreateStandardLayout = false;
			}
		}
		internal class CustomizationState : ILockable {
			public CustomizationState() {
			}
			public int OriginalMaxItemsCount { get; set; }
			public ObservableCollection<object> OriginalItems { get; set; }
			#region ILockable Members
			int lockCount = 0;
			public void BeginUpdate() {
				lockCount++;
			}
			public void EndUpdate() {
				lockCount--;
			}
			public bool IsLockUpdate {
				get { return lockCount > 0; }
			}
			#endregion
		}
	}
}

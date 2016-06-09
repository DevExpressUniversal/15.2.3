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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Docking {
	public class Selection : BaseReadOnlyList<BaseLayoutItem> {
		protected internal void ProcessSelectionChanged(BaseLayoutItem item, bool selected) {
			if(selected) {
				if(!List.Contains(item)) List.Add(item);
			}
			else List.Remove(item);
		}
		public BaseLayoutItem[] ToArray() {
			BaseLayoutItem[] items = new BaseLayoutItem[List.Count];
			List.CopyTo(items, 0);
			return items;
		}
	}
	public class LayoutController : ILayoutController, IActiveItemOwner {
		bool isDisposingCore = false;
		DockLayoutManager containerCore;
		Selection selectionCore;
		ActiveItemHelper ActivationHelper;
		BaseLayoutItem activeItemCore;
		public LayoutController(DockLayoutManager container) {
			containerCore = container;
			ActivationHelper = new ActiveItemHelper(this);
			selectionCore = CreateSelection();
			hiddenItemsCore = CreateHiddenItemsCollection();
			Container.LayoutItemSelectionChanged += OnItemSelectionChanged;
		}
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				this.isDisposingCore = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual Selection CreateSelection() {
			return new Selection();
		}
		protected bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected void OnDisposing() {
			Ref.Dispose(ref selectionCore);
			Ref.Dispose(ref hiddenItemsCore);
			Ref.Dispose(ref ActivationHelper);
			activeItemCore = null;
			Container.LayoutItemSelectionChanged -= OnItemSelectionChanged;
			containerCore = null;
		}
		void OnItemSelectionChanged(object sender, LayoutItemSelectionChangedEventArgs e) {
			Selection.ProcessSelectionChanged(e.Item, e.Selected);
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControllerContainer")]
#endif
		public DockLayoutManager Container {
			get { return containerCore; }
		}
		int lockActivateCounter = 0;
		public void Activate(BaseLayoutItem item) {
			Activate(item, true);
		}
		public void Activate(BaseLayoutItem item, bool focus) {
			if(lockActivateCounter > 0 || item == null || !item.AllowActivate || item.IsClosed) return;
			if(item is LayoutGroup && ((LayoutGroup)item).IsUngroupped) return;
			lockActivateCounter++;
			try {
				if(RaiseItemCancelEvent(item, DockLayoutManager.LayoutItemActivatingEvent)) {
					item.InvokeCancelActivation(ActiveItem);
					return;
				}
				ActivationHelper.ActivateItem(item, focus);
			}
			finally { lockActivateCounter--; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControllerActiveItem")]
#endif
		public BaseLayoutItem ActiveItem {
			get { return activeItemCore; }
			set {
				if(ActiveItem == value) return;
				SetActiveItemCore(value);
			}
		}
		void SetActiveItemCore(BaseLayoutItem value) {
			SetActive(false);
			BaseLayoutItem oldItem = activeItemCore;
			this.activeItemCore = value;
			SetActive(true);
			Container.isLayoutItemActivation++;
			Container.ActiveLayoutItem = ActiveItem;
			Container.isLayoutItemActivation--;
			RaiseActiveItemChanged(value, oldItem);
		}
		void SetActive(bool value) {
			if(ActiveItem != null) {
				ActiveItem.SetActive(value);
				if(value && lockActivateCounter == 0) 
					ActivationHelper.SelectInGroup(ActiveItem);
			}
		}
		void RaiseActiveItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			Container.RaiseEvent(
					new LayoutItemActivatedEventArgs(item, oldItem) { Source = Container }
				);
		}
		bool RaiseItemCancelEvent(BaseLayoutItem item, RoutedEvent routedEvent) {
			return Container.RaiseItemCancelEvent(item, routedEvent);
		}
		public T CreateCommand<T>(BaseLayoutItem[] items) where T : LayoutControllerCommand, new() {
			return new T() { Controller = this, Items = items };
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControllerIsCustomization")]
#endif
		public bool IsCustomization {
			get { return Container.CustomizationController.IsCustomization; }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControllerSelection")]
#endif
		public Selection Selection { get { return selectionCore; } }
		LayoutGroup TryUngroupGroup(LayoutGroup group, ref BaseLayoutItem nestedItem, ref int index) {
			if(group == null) return null;
			if(!group.DestroyOnClosingChildren) return group;
			LayoutGroup parentGroup = group.Parent;
			switch(group.Items.Count) {
				case 0:
					nestedItem = null;
					if(parentGroup != null) {
						index = parentGroup.Items.IndexOf(group);
						if(parentGroup.Items.Remove(group)) {
							group.IsUngroupped = true;
							return TryUngroupGroup(parentGroup, ref nestedItem, ref index);
						}
					}
					break;
				case 1:
					nestedItem = group.Items[0];
					TryUngroupGroup(nestedItem as LayoutGroup, ref nestedItem, ref index);
					if(parentGroup != null)
						index = parentGroup.Items.IndexOf(group);
					return DockControllerHelper.Unbox(Container, group);
			}
			return group;
		}
		public bool Hide(BaseLayoutItem item) {
			if(item == null) return false;
			if(item.IsHidden && item.IsSelected) item.SetSelected(Container, false);
			if(item.IsHidden || !item.AllowHide || item.Parent == null) return false;
			if(item.IsSelected) item.SetSelected(Container, false);
			bool result = false;
			LayoutGroup root = item.GetRoot();
			using(new UpdateBatch(Container)) {
				if(root != null) {
					LayoutGroup sourceGroup = item.Parent;
					BaseLayoutItem activeItem = LayoutItemsHelper.GetNextItem(item);
					if(sourceGroup.Remove(item)) {
						BaseLayoutItem nestedItem = null;
						int index = -1;
						LayoutGroup activeGroup = TryUngroupGroup(sourceGroup, ref nestedItem, ref index);
						HiddenItems.Add(item, root);
						if(ActiveItem != null && ActiveItem.IsHidden)
							Activate(activeItem ?? activeGroup);
						Container.RaiseEvent(new LayoutItemHiddenEventArgs(item));
						Container.Update();
						result = true;
					}
				}
				return result;
			}
		}
		public bool Restore(BaseLayoutItem item) {
			if(item == null || !item.IsHidden || !item.AllowRestore) return false;
			using(new UpdateBatch(Container)) {
				LayoutGroup restoreRoot = item.Parent ?? (Container.LayoutRoot.IsLayoutRoot ? Container.LayoutRoot : null);
				if(restoreRoot != null && HiddenItems.Remove(item)) {
					restoreRoot.Add(item);
					Container.RaiseEvent(new LayoutItemRestoredEventArgs(item));
					Container.Update();
					return true;
				}
				return false;
			}
		}
		void MoveCore(BaseLayoutItem item, LayoutGroup target) {
			MoveCore(item, target, -1);
		}
		void MoveCore(BaseLayoutItem item, LayoutGroup target, int index) {
			if(item.IsHidden)
				HiddenItems.Remove(item);
			if(index != -1)
				target.Insert(index, item);
			else
				target.Add(item);
		}
		bool CanMove(BaseLayoutItem item, BaseLayoutItem target, MoveType type) {
			if(item == null || target == null || item == target || type == MoveType.None) return false;
			if(!item.AllowMove || (item.IsHidden && !item.AllowRestore)) return false;
			if(type == MoveType.InsideGroup && (!(target is LayoutGroup) || (item.Parent == target && !item.IsHidden))) return false;
			if(type != MoveType.InsideGroup && target.Parent == null) return false;
			if(item is LayoutGroup && (LayoutItemsHelper.IsParent(target, item) || ((LayoutGroup)item).IsLayoutRoot)) return false;
			return true;
		}
		public bool Move(BaseLayoutItem item, BaseLayoutItem target, MoveType type, int insertIndex) {
			if(!CanMove(item, target, type)) return false;
			item.SetSelected(Container, false);
			LayoutGroup sourceGroup = item.Parent;
			bool isHidden = item.IsHidden;
			using(new NotificationBatch(Container)) {
				using(new UpdateBatch(Container)) {
					NotificationBatch.Action(Container, item.GetRoot());
					if(sourceGroup != null) {
						sourceGroup.Remove(item);
						BaseLayoutItem nestedItem = null;
						int index = -1;
						LayoutGroup ungroupResult = TryUngroupGroup(sourceGroup, ref nestedItem, ref index);
						if((target is LayoutGroup) && ((LayoutGroup)target).IsUngroupped && ungroupResult != sourceGroup) {
							if(nestedItem != null)
								target = nestedItem;
							else {
								if(index != -1)
									MoveCore(item, ungroupResult, index);
								else MoveCore(item, ungroupResult);
								OnMoveComplete(item, target, type, isHidden);
								return true;
							}
						}
					}
					if(type == MoveType.InsideGroup) {
						MoveCore(item, (LayoutGroup)target, insertIndex);
					}
					else {
						LayoutGroup targetGroup = target.Parent;
						if(targetGroup == null)
							return false;
						Orientation orientation = type.ToOrientation();
						int targetIndex = targetGroup.Items.IndexOf(target);
						if(!targetGroup.IgnoreOrientation && targetGroup.Orientation != orientation) {
							targetGroup.Remove(target);
							LayoutGroup newGroup = DockControllerHelper.BoxIntoGroup(target, orientation, Container);
							if(!target.ItemHeight.IsAuto && !item.ItemHeight.IsAuto)
								target.ItemHeight = item.ItemHeight;
							MoveCore(item, newGroup, (type.ToInsertType() == InsertType.Before ? 0 : 1));
							item = newGroup;
						}
						else targetIndex += (type.ToInsertType() == InsertType.Before ? 0 : 1);
						MoveCore(item, targetGroup, targetIndex);
					}
					OnMoveComplete(item, target, type, isHidden);
				}
			}
			return true;
		}
		public bool Move(BaseLayoutItem item, BaseLayoutItem target, MoveType type) {
			return Move(item, target, type, -1);
		}
		void OnMoveComplete(BaseLayoutItem item, BaseLayoutItem target, MoveType type, bool isHidden) {
			NotificationBatch.Action(Container, item.GetRoot());
			Container.Update();
			RaiseItemMovedEvent(item, target, type, isHidden);
		}
		void RaiseItemMovedEvent(BaseLayoutItem item, BaseLayoutItem target, MoveType type, bool isHidden) {
			if(isHidden)
				Container.RaiseEvent(new LayoutItemRestoredEventArgs(item));
			else
				Container.RaiseEvent(new LayoutItemMovedEventArgs(item, target, type));
		}
		public bool Group(BaseLayoutItem[] items) {
			if(!LayoutItemsHelper.AreInSameGroup(items)) return false;
			using(new UpdateBatch(Container)) {
				using(new NotificationBatch(Container)) {
					LayoutGroup parent = items[0].Parent;
					int minIndex = parent.Items.IndexOf(items[0]);
					foreach(BaseLayoutItem item in items) {
						minIndex = Math.Min(minIndex, parent.Items.IndexOf(item));
					}
					using(new LogicalTreeLocker(Container, items)) {
						foreach(BaseLayoutItem item in items) {
							item.SetSelected(Container, false);
							parent.Remove(item);
						}
						LayoutGroup group = DockControllerHelper.BoxIntoGroup(items, parent.Orientation);
						group.Caption = DockingLocalizer.GetString(DockingStringId.NewGroupCaption);
						group.ShowCaption = true;
						parent.Insert(minIndex, group);
						Activate(group);
					}
					Container.Update();
					NotificationBatch.Action(Container, parent.GetRoot());
				}
				return true;
			}
		}
		public bool ChangeGroupOrientation(LayoutGroup group, Orientation orientation) {
			if(group == null) return false;
			group.Orientation = orientation;
			Container.Update();
			return true;
		}
		public bool Rename(BaseLayoutItem item) {
			return Container.RenameHelper.Rename(item);
		}
		public bool CancelRenaming() {
			return Container.RenameHelper.CancelRenamingAndResetClickedState();
		}
		public bool EndRenaming() {
			return Container.RenameHelper.EndRenaming();
		}
		public bool SetGroupBorderStyle(LayoutGroup group, GroupBorderStyle style) {
			if(group == null) return false;
			group.GroupBorderStyle = style;
			Container.Update();
			return true;
		}
		public void HideSelectedItems() {
			BaseLayoutItem[] items = Selection.ToArray();
			foreach(BaseLayoutItem item in items) {
				Hide(item);
			}
		}
		HiddenItemsCollection hiddenItemsCore;
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControllerHiddenItems")]
#endif
		public HiddenItemsCollection HiddenItems {
			get { return hiddenItemsCore; }
		}
		protected virtual HiddenItemsCollection CreateHiddenItemsCollection() {
			return new HiddenItemsCollection(Container, null);
		}
		ReadOnlyCollection<BaseLayoutItem> fixedItemsCore;
#if !SL
	[DevExpressXpfDockingLocalizedDescription("LayoutControllerFixedItems")]
#endif
		public IEnumerable<BaseLayoutItem> FixedItems {
			get {
				if(fixedItemsCore == null && !IsDisposing) {
					fixedItemsCore = CreateFixedItemsCollection();
				}
				return fixedItemsCore;
			}
		}
		protected virtual ReadOnlyCollection<BaseLayoutItem> CreateFixedItemsCollection() {
			return new ReadOnlyCollection<BaseLayoutItem>(new BaseLayoutItem[]{
				FixedItemFactory.CreateFixedItem(LayoutItemType.EmptySpaceItem),
				FixedItemFactory.CreateFixedItem(LayoutItemType.Label),
				FixedItemFactory.CreateFixedItem(LayoutItemType.Separator),
				FixedItemFactory.CreateFixedItem(LayoutItemType.LayoutSplitter),
			});
		}
		public bool Ungroup(LayoutGroup group) {
			using(new UpdateBatch(Container)) {
				if(group == null || group.Parent == null) return false;
				group.SetSelected(Container, false);
				LayoutGroup parent = group.Parent;
				int index = parent.Items.IndexOf(group);
				BaseLayoutItem[] items = new BaseLayoutItem[group.Items.Count];
				group.Items.CopyTo(items, 0);
				foreach(BaseLayoutItem item in items) {
					group.Remove(item);
					parent.Insert(index++, item);
				}
				parent.Remove(group);
				Container.Update();
				return true;
			}
		}
	}
	#region Commands
	public abstract class LayoutControllerCommand : ICommand {
		internal LayoutControllerCommand() { }
		protected abstract void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items);
		protected abstract bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items);
		protected internal ILayoutController Controller { get; set; }
		protected internal BaseLayoutItem[] Items { get; set; }
		#region ICommand
		event EventHandler CanExecuteChangedCore;
		event EventHandler ICommand.CanExecuteChanged {
			add { CanExecuteChangedCore += value; }
			remove { CanExecuteChangedCore -= value; }
		}
		protected void RaiseCanExecuteChanged() {
			if(CanExecuteChangedCore != null)
				CanExecuteChangedCore(this, EventArgs.Empty);
		}
		bool ICommand.CanExecute(object parameter) {
			if(Items == null) return false;
			return CanExecuteCore(Controller, Items);
		}
		void ICommand.Execute(object parameter) {
			ExecuteCore(Controller, Items);
		}
		#endregion ICommand
		#region static
		static LayoutControllerCommand() {
			ShowCaption = new LayoutControllerCommandLink("ShowCaption", new ShowCaptionCommand());
			ShowControl = new LayoutControllerCommandLink("ShowControl", new ShowControlCommand());
			ShowCaptionImageBeforeText = new LayoutControllerCommandLink("ShowCaptionImageBeforeText", new CaptionImageBeforeTextCommand());
			ShowCaptionImageAfterText = new LayoutControllerCommandLink("ShowCaptionImageAfterText", new CaptionImageAfterTextCommand());
			ShowCaptionOnLeft = new LayoutControllerCommandLink("ShowCaptionOnLeft", new CaptionLocationLeftCommand());
			ShowCaptionOnRight = new LayoutControllerCommandLink("ShowCaptionOnRight", new CaptionLocationRightCommand());
			ShowCaptionAtTop = new LayoutControllerCommandLink("ShowCaptionAtTop", new CaptionLocationTopCommand());
			ShowCaptionAtBottom = new LayoutControllerCommandLink("ShowCaptionAtBottom", new CaptionLocationBottomCommand());
		}
		public static RoutedCommand ShowCaption { get; private set; }
		public static RoutedCommand ShowControl { get; private set; }
		public static RoutedCommand ShowCaptionImageBeforeText { get; private set; }
		public static RoutedCommand ShowCaptionImageAfterText { get; private set; }
		public static RoutedCommand ShowCaptionOnLeft { get; private set; }
		public static RoutedCommand ShowCaptionOnRight { get; private set; }
		public static RoutedCommand ShowCaptionAtTop { get; private set; }
		public static RoutedCommand ShowCaptionAtBottom { get; private set; }
		internal static void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
			ILayoutController controller = LayoutControllerHelper.GetLayoutController(sender);
			BaseLayoutItem[] items = e.Parameter as BaseLayoutItem[];
			if(e.Parameter is BaseLayoutItem)
				items = new BaseLayoutItem[] { (BaseLayoutItem)e.Parameter };
			LayoutControllerCommand command = ((LayoutControllerCommandLink)e.Command).Command;
			e.CanExecute = (controller != null && items != null) && command.CanExecuteCore(controller, items);
		}
		internal static void Executed(object sender, ExecutedRoutedEventArgs e) {
			ILayoutController controller = LayoutControllerHelper.GetLayoutController(sender);
			BaseLayoutItem[] items = e.Parameter as BaseLayoutItem[];
			if(e.Parameter is BaseLayoutItem)
				items = new BaseLayoutItem[] { (BaseLayoutItem)e.Parameter };
			if(controller != null && items != null) {
				LayoutControllerCommand command = ((LayoutControllerCommandLink)e.Command).Command;
				command.ExecuteCore(controller, items);
			}
		}
		#endregion static
		class LayoutControllerCommandLink : RoutedCommand {
			public LayoutControllerCommandLink(string name, LayoutControllerCommand command) :
				base(name, typeof(LayoutControllerCommand)) {
				Command = command;
			}
			public LayoutControllerCommand Command { get; private set; }
		}
	}
	public class ShowCaptionCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.ShowCaption = !item.ShowCaption;
			controller.Container.Update();
		}
	}
	public class ShowControlCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				if(item is LayoutControlItem)
					((LayoutControlItem)item).ShowControl = !((LayoutControlItem)item).ShowControl;
			}
			controller.Container.Update();
		}
	}
	public class CaptionImageBeforeTextCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.CaptionImageLocation = ImageLocation.BeforeText;
		}
	}
	public class CaptionImageAfterTextCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.CaptionImageLocation = ImageLocation.AfterText;
		}
	}
	public class CaptionLocationLeftCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.CaptionLocation = CaptionLocation.Left;
			controller.Container.Update();
		}
	}
	public class CaptionLocationRightCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.CaptionLocation = CaptionLocation.Right;
			controller.Container.Update();
		}
	}
	public class CaptionLocationTopCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.CaptionLocation = CaptionLocation.Top;
			controller.Container.Update();
		}
	}
	public class CaptionLocationBottomCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				item.CaptionLocation = CaptionLocation.Bottom;
			controller.Container.Update();
		}
	}
	public class HideItemCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization && items != null && items.Length > 0;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.Hide(item);
		}
	}
	public class RestoreItemCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.Restore(item);
		}
	}
	public class GroupCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			controller.Group(items);
		}
	}
	public class GroupOrientationHorizontalCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.ChangeGroupOrientation(item as LayoutGroup, Orientation.Horizontal);
		}
	}
	public class GroupOrientationVerticalCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.ChangeGroupOrientation(item as LayoutGroup, Orientation.Vertical);
		}
	}
	public class RenameCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] item) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] item) {
			controller.Rename(item[0]);
		}
	}
	public class ShowCaptionImageCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				item.ShowCaptionImage = !item.ShowCaptionImage;
			}
		}
	}
	public class SetStyleNoBorderCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.SetGroupBorderStyle(item as LayoutGroup, GroupBorderStyle.NoBorder);
		}
	}
	public class SetStyleGroupCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.SetGroupBorderStyle(item as LayoutGroup, GroupBorderStyle.Group);
		}
	}
	public class SetStyleGroupBoxCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.SetGroupBorderStyle(item as LayoutGroup, GroupBorderStyle.GroupBox);
		}
	}
	public class SetStyleTabbedCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.SetGroupBorderStyle(item as LayoutGroup, GroupBorderStyle.Tabbed);
		}
	}
	public class UngroupCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			if(items.Length != 1) return;
			controller.Ungroup(items[0] as LayoutGroup);
		}
	}
	public class ControlHorizontalAlignmentCommand : LayoutControllerCommand {
		public HorizontalAlignment HorizontalAlignment { get; set; }
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				if(item is LayoutControlItem) ((LayoutControlItem)item).ControlHorizontalAlignment = HorizontalAlignment;
			}
		}
	}
	public class ControlVerticalAlignmentCommand : LayoutControllerCommand {
		public VerticalAlignment VerticalAlignment { get; set; }
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				if(item is LayoutControlItem) ((LayoutControlItem)item).ControlVerticalAlignment = VerticalAlignment;
			}
		}
	}
	public class CaptionHorizontalAlignmentCommand : LayoutControllerCommand {
		public HorizontalAlignment HorizontalAlignment { get; set; }
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				if(item is LayoutControlItem) ((LayoutControlItem)item).CaptionHorizontalAlignment = HorizontalAlignment;
			}
		}
	}
	public class CaptionVerticalAlignmentCommand : LayoutControllerCommand {
		public VerticalAlignment VerticalAlignment { get; set; }
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				if(item is LayoutControlItem) ((LayoutControlItem)item).CaptionVerticalAlignment = VerticalAlignment;
			}
		}
	}
	public class VisibilityVisibleCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				item.Visibility = Visibility.Visible;
			}
		}
	}
	public class VisibilityHiddenCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				item.Visibility = Visibility.Hidden;
			}
		}
	}
	public class VisibilityCollapsedCommand : LayoutControllerCommand {
		protected override bool CanExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			return controller.IsCustomization;
		}
		protected override void ExecuteCore(ILayoutController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				item.Visibility = Visibility.Collapsed;
			}
		}
	}
	#endregion Commands
	public static class LayoutControllerHelper {
		public static T CreateCommand<T>(DockLayoutManager container, BaseLayoutItem[] items) where T : LayoutControllerCommand, new() {
			ILayoutController controller = GetLayoutController(container);
			return (controller != null) ? controller.CreateCommand<T>(items) : null;
		}
		static ILayoutController GetLayoutController(DockLayoutManager container) {
			return (container != null) ? container.LayoutController : null;
		}
		public static ILayoutController GetLayoutController(object obj) {
			DockLayoutManager container = obj as DockLayoutManager;
			if(container == null) {
				DependencyObject dObj = obj as DependencyObject;
				container = (dObj != null) ? DockLayoutManager.GetDockLayoutManager(dObj) : null;
			}
			return GetLayoutController(container);
		}
		public static object GetSameValue(DependencyProperty property, BaseLayoutItem[] items, Comparison<object> match) {
			if(items == null || items.Length == 0) return null;
			object value = items[0].GetValue(property);
			for(int i = 1; i < items.Length; i++) {
				if(match(items[i].GetValue(property), value) != 0) return null;
			}
			return value;
		}
		public static int CompareImageLocation(object imageLocation1, object imageLocation2) {
			bool isAfter1 = (ImageLocation)imageLocation1 == ImageLocation.AfterText;
			bool isAfter2 = (ImageLocation)imageLocation2 == ImageLocation.AfterText;
			return isAfter1 == isAfter2 ? 0 : 1;
		}
		public static int CompareCaptionLocation(object captionLocation1, object captionLocation2) {
			bool isLeft1 = (CaptionLocation)captionLocation1 == CaptionLocation.Default || (CaptionLocation)captionLocation1 == CaptionLocation.Left;
			bool isLeft2 = (CaptionLocation)captionLocation2 == CaptionLocation.Default || (CaptionLocation)captionLocation2 == CaptionLocation.Left;
			return isLeft1 && isLeft2 ? 0 : (object.Equals(captionLocation1, captionLocation2) ? 0 : 1);
		}
		public static int BoolComparer(object value1, object value2) {
			return (bool)value1 == (bool)value2 ? 0 : 1;
		}
		public static int CompareOrientation(object orientation1, object orientation2) {
			bool isHorizontal1 = (Orientation)orientation1 == Orientation.Horizontal;
			bool isHorizontal2 = (Orientation)orientation2 == Orientation.Horizontal;
			return isHorizontal1 == isHorizontal2 ? 0 : 1;
		}
		public static int CompareGroupBorderStyle(object style1, object style2) {
			return (GroupBorderStyle)style1 == (GroupBorderStyle)style2 ? 0 : 1;
		}
		public static int CompareObjects(object ha1, object ha2) {
			return (ha1 == ha2) ? 0 : 1;
		}
		public static bool HasOnlyGroups(BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items) {
				if(!(item is LayoutGroup)) return false;
			}
			return true;
		}
	}
}

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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Docking.Customization;
using DevExpress.Xpf.Layout.Core;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking {
	public interface IActiveItemOwner {
		DockLayoutManager Container { get; }
		BaseLayoutItem ActiveItem { get; set; }
		void Activate(BaseLayoutItem item);
		void Activate(BaseLayoutItem item, bool setFocus);
	}
	public interface IDockController : IActiveItemOwner, IDisposable {
		FloatGroup Float(BaseLayoutItem item);
		bool Dock(BaseLayoutItem item);
		bool Close(BaseLayoutItem item);
		bool CloseAllButThis(BaseLayoutItem item);
		bool Hide(BaseLayoutItem item);
		bool Restore(BaseLayoutItem item);
		bool Rename(BaseLayoutItem item);
		bool Dock(BaseLayoutItem item, BaseLayoutItem target, DockType type);
		bool Hide(BaseLayoutItem item, SWC.Dock dock);
		bool Hide(BaseLayoutItem item, AutoHideGroup target);
		bool Insert(LayoutGroup group, BaseLayoutItem item, int index);
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		void AddItem(BaseLayoutItem item, BaseLayoutItem target, DockType type);
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		void RemoveItem(BaseLayoutItem item);
		LayoutPanel AddPanel(Point floatLocation, Size floatSize);
		LayoutPanel AddPanel(DockType type);
		DocumentGroup AddDocumentGroup(DockType type);
		DocumentPanel AddDocumentPanel(DocumentGroup group);
		DocumentPanel AddDocumentPanel(Point floatLocation, Size floatSize);
		DocumentPanel AddDocumentPanel(DocumentGroup group, Uri uri);
		DocumentPanel AddDocumentPanel(Point floatLocation, Size floatSize, Uri uri);
		void RemovePanel(LayoutPanel panel);
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		bool CreateNewDocumentGroup(DocumentPanel item, System.Windows.Controls.Orientation orientation);
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		bool MoveToDocumentGroup(DocumentPanel item, bool next);
		bool CreateNewDocumentGroup(LayoutPanel item, System.Windows.Controls.Orientation orientation);
		bool MoveToDocumentGroup(LayoutPanel item, bool next);
		T CreateCommand<T>(BaseLayoutItem item) where T : DockControllerCommand, new();
	}
	public interface ICustomizationController : IControlHost, IDisposable {
		DockLayoutManager Container { get; }
		BarManager BarManager { get; }
		ClosedItemsBar ClosedItemsBar { get; }
		ClosedPanelsBarVisibility ClosedPanelsBarVisibility { get; set; }
		void ShowItemSelectorMenu(UIElement source, BaseLayoutItem[] items);
		void ShowContextMenu(BaseLayoutItem item);
		void ShowHiddenItemMenu(BaseLayoutItem item);
		void ShowControlItemContextMenu(BaseLayoutItem item);
		void CloseMenu();
		void UpdateClosedItemsBar();
		void ShowClosedItemsBar();
		void HideClosedItemsBar();
		void ShowCustomizationForm();
		void HideCustomizationForm();
		void BeginCustomization();
		void EndCustomization();
		bool IsClosedPanelsVisible { get; }
		bool ClosedPanelsVisibility { get; set; }
		bool IsCustomizationFormVisible { get; }
		T CreateCommand<T>() where T : CustomizationControllerCommand, new();
		UIElement MenuSource { get; set; }
		BarManagerMenuController ItemContextMenuController { get; }
		BarManagerMenuController ItemsSelectorMenuController { get; }
		BarManagerMenuController LayoutControlItemContextMenuController { get; }
		BarManagerMenuController LayoutControlItemCustomizationMenuController { get; }
		BarManagerMenuController HiddenItemsMenuController { get; }
		bool IsCustomization { get; set; }
		ObservableCollection<BaseLayoutItem> CustomizationItems { get; }
		LayoutGroup CustomizationRoot { get; set; }
		bool IsDragCursorVisible { get; }
		void ShowDragCursor(Point point, BaseLayoutItem item);
		void HideDragCursor();
		void SetDragCursorPosition(Point point);
		DragInfo DragInfo { get; }
		void UpdateDragInfo(DragInfo info);
		event DragInfoChangedEventHandler DragInfoChanged;
		FloatingContainer CustomizationForm { get; }
		void DesignTimeRaiseEvent(object sender, RoutedEventArgs e);
		bool IsDocumentSelectorVisible { get; }
		void ShowDocumentSelectorForm();
		void HideDocumentSelectorForm();
	}
	public interface ISerializationController : IDisposable {
		DockLayoutManager Container { get; }
		SerializableItemCollection Items { get; set; }
		bool IsDeserializing { get; }
		void SaveLayout(object path);
		void RestoreLayout(object path);
		void OnClearCollection(XtraItemRoutedEventArgs e);
		object OnCreateCollectionItem(XtraCreateCollectionItemEventArgs e);
		object OnFindCollectionItem(XtraFindCollectionItemEventArgs e);
		T CreateCommand<T>(object path) where T : SerializationControllerCommand, new();
	}
	public interface ILayoutController : IActiveItemOwner, IDisposable {
		bool IsCustomization { get; }
		Selection Selection { get; }
		bool Move(BaseLayoutItem item, BaseLayoutItem target, MoveType type);
		bool Move(BaseLayoutItem item, BaseLayoutItem target, MoveType type, int insertIndex);
		bool Hide(BaseLayoutItem item);
		bool Restore(BaseLayoutItem item);
		bool Group(BaseLayoutItem[] items);
		bool Ungroup(LayoutGroup group);
		bool Rename(BaseLayoutItem item);
		bool CancelRenaming();
		bool EndRenaming();
		HiddenItemsCollection HiddenItems { get; }
		IEnumerable<BaseLayoutItem> FixedItems { get; }
		bool ChangeGroupOrientation(LayoutGroup group, System.Windows.Controls.Orientation orientation);
		void HideSelectedItems();
		bool SetGroupBorderStyle(LayoutGroup group, GroupBorderStyle style);
		T CreateCommand<T>(BaseLayoutItem[] items) where T : LayoutControllerCommand, new();
	}
	public interface IMDIController : IActiveItemOwner, IDisposable {
		bool Maximize(BaseLayoutItem document);
		bool Maximize(DocumentPanel document);
		bool Minimize(DocumentPanel document);
		bool Restore(BaseLayoutItem document);
		bool Restore(DocumentPanel document);
		bool TileVertical(BaseLayoutItem item);
		bool TileHorizontal(BaseLayoutItem item);
		bool Cascade(BaseLayoutItem item);
		bool ArrangeIcons(BaseLayoutItem item);
		bool ChangeMDIStyle(BaseLayoutItem item);
		MDIMenuBar MDIMenuBar { get; }
		T CreateCommand<T>(BaseLayoutItem[] items) where T : MDIControllerCommand, new();
	}
	public interface IControlHost {
		FrameworkElement[] GetChildren();
	}
	interface IDockLayoutManagerListener {
		void Subscribe(DockLayoutManager manager);
		void Unsubscribe(DockLayoutManager manager);
	}
	interface ILockOwner {
		void Lock();
		void Unlock();
	}
	interface IMDIMergeStyleListener {
		void OnMDIMergeStyleChanged(MDIMergeStyle oldValue, MDIMergeStyle newValue);
	}
	interface IResizingPreviewHelper {
		void InitResizing(Point point, ILayoutElement element);
		void Resize(Point point);
		void EndResizing();
	}
	interface IDockController2010 : IDockController {
		bool DockAsDocument(BaseLayoutItem item, BaseLayoutItem target, DockType type);
	}
	public interface ISupportBatchUpdate {
		bool IsUpdatedLocked { get; }
		void BeginUpdate();
		void EndUpdate();
	}
	interface IGeneratorHost {
		DependencyObject GenerateContainerForItem(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector);
		DependencyObject LinkContainerToItem(DependencyObject container, object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector);
		void ClearContainer(DependencyObject container, object item);
	}
	interface IClosable {
		void OnClosed();
		bool CanClose();
	}
	interface IMergingClient {
		void Merge();
		void QueueMerge();
		void QueueUnmerge();
	}
}

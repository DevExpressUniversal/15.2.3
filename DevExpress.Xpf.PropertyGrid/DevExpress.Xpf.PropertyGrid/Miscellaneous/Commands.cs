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

using DevExpress.Xpf.Bars;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace DevExpress.Xpf.PropertyGrid {
	public static class PropertyGridCommands {
		static RoutedCommand selectView = new RoutedCommand("SelectView", typeof(PropertyGridCommands));
		static RoutedCommand reset = new RoutedCommand("Reset", typeof(PropertyGridCommands));
		static RoutedCommand refresh = new RoutedCommand("Refresh", typeof(PropertyGridCommands));
		static RoutedCommand addItem = new RoutedCommand("AddItem", typeof(PropertyGridCommands));
		static RoutedCommand removeItem = new RoutedCommand("RemoveItem", typeof(PropertyGridCommands));
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static RoutedCommand SelectView {
			get { return selectView; }
		}
		public static RoutedCommand Reset {
			get { return reset; }
		}
		public static RoutedCommand Refresh {
			get { return refresh; }
		}
		public static RoutedCommand AddItem {
			get { return addItem; }
		}
		public static RoutedCommand RemoveItem {
			get { return removeItem; }
		}
	}
	public static class BarItemNames {
		public const string Reset = "PropertyGridMenuItem_Reset",
							Refresh = "PropertyGridMenuItem_Refresh";
	}
	internal class BarItems {		
		PropertyGridView owner;
		public BarItems(PropertyGridView owner) {
			this.owner = owner;
		}
		public BarButtonItem CreateRefreshItem() {
			return new BarButtonItem() {
				Name = BarItemNames.Refresh,
				Glyph = DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromEmbeddedResource(typeof(BarItemNames).Assembly, "/Images/Refresh_16x16.png"),
				GlyphSize = GlyphSize.Small,
				Command = PropertyGridCommands.Refresh,
				Content = PropertyGridControlLocalizer.GetString(PropertyGridControlStringID.RefreshItemContent),
				CommandTarget = owner
			};
		}
		public BarButtonItem CreateResetItem() {
			return new BarButtonItem() {
				Name = BarItemNames.Reset,
				Glyph = DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromEmbeddedResource(typeof(BarItemNames).Assembly, "/Images/Reset_16x16.png"),
				GlyphSize = GlyphSize.Small,
				Command = PropertyGridCommands.Reset,
				Content = PropertyGridControlLocalizer.GetString(PropertyGridControlStringID.ResetItemContent),
				CommandTarget = owner
			};
		}		
	}
	internal class SelectSearchControlCommand : ICommand {
		PropertyGridControl owner;
		public SelectSearchControlCommand(PropertyGridControl owner) {
			this.owner = owner;
		}
		public bool CanExecute(object parameter) {
			return owner.ShowSearchBox && owner.SearchControl != null && !owner.SearchControl.IsKeyboardFocusWithin;
		}
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		public void Execute(object parameter) {
			NavigationManager.FocusElementOrFirstAvailableChild(owner.SearchControl, NavigationDirection.Next);
		}
	}
	internal class BarInstanceInitializerSplitButtonItem : BarSplitButtonItem {
		static BarInstanceInitializerSplitButtonItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(BarInstanceInitializerSplitButtonItem), typeof(BarInstanceInitializerSplitButtonItemLink), i => new BarInstanceInitializerSplitButtonItemLink());
		}
		IEnumerable<TypeInfo> typeInfos;
		TypeInfo currentTypeInfo;
		bool dataInitialized = false;
		protected string NullCaption { get; private set; }
		public bool IsCollectionInitializer { get; private set; }
		protected Dictionary<TypeInfo, BarItem> Items { get; private set; }
		protected RowDataGenerator RowDataGenerator { get; private set; }
		protected DataViewBase DataView { get { return RowDataGenerator.DataView; } }
		protected RowHandle Handle { get; private set; }
		protected PopupMenu Menu { get; set; }
		protected internal TypeInfo CurrentTypeInfo {
			get { return currentTypeInfo; }
			set {
				if (value == currentTypeInfo)
					return;
				TypeInfo oldValue = currentTypeInfo;
				currentTypeInfo = value;
				OnCurrentTypeInfoChanged(oldValue);
			}
		}
		protected IEnumerable<TypeInfo> TypeInfos {
			get { return typeInfos; }
			set {
				if (value == typeInfos)
					return;
				IEnumerable<TypeInfo> oldValue = typeInfos;
				typeInfos = value;
				OnTypeInfosChanged(oldValue);
			}
		}
		public bool HasItems {
			get { return Items.Count != 0; }
		}
		public bool HasCommonValue {
			get { return Items.Count == 1; }
		}
		public BarInstanceInitializerSplitButtonItem(RowDataGenerator generator, RowHandle handle) {
			this.Handle = handle;
			this.RowDataGenerator = generator;
			Items = new Dictionary<TypeInfo, BarItem>();
		}
		protected internal virtual void InitializeData() {
			if (dataInitialized)
				return;
			dataInitialized = true;
			NullCaption = PropertyGridControlLocalizer.GetString(PropertyGridControlStringID.NewItemInitializerButtonContent);
			IsCollectionInitializer = DataView.IsCollectionHandle(Handle);
			var typeInfo = IsCollectionInitializer ? DataView.GetCollectionNewItemValues(Handle) : DataView.GetStandardValues(Handle);
			TypeInfos = typeInfo.With(x => x.OfType<TypeInfo>());
			IsVisible = HasItems;
		}
		protected virtual void OnTypeInfosChanged(IEnumerable<TypeInfo> oldValue) {
			UpdateItems();
			if (TypeInfos == null || !TypeInfos.Any()) {
				CurrentTypeInfo = null;
			}
			else {
				if (IsCollectionInitializer)
					CurrentTypeInfo = DataView.GetSelectedCollectionNewItem(Handle) as TypeInfo;
				CurrentTypeInfo = CurrentTypeInfo ?? TypeInfos.ElementAt(0);
			}
			UpdatePopupMenu();
		}
		protected virtual void OnCurrentTypeInfoChanged(TypeInfo oldValue) {
			if (CurrentTypeInfo == null) {
				Content = NullCaption;
				Command = null;
			}
			else {
				var currentBarItem = Items[CurrentTypeInfo];
				Content = currentBarItem.Content;
				Command = currentBarItem.Command;
			}
		}
		protected virtual void UpdateItems() {
			if (TypeInfos == null) {
				Items.Clear();
			}
			else {
				foreach (var info in TypeInfos) {
					Items[info] = new BarInstanceInitializerItem(RowDataGenerator, Handle, info, this);
				}
			}
		}
		protected virtual void UpdatePopupMenu() {
			if (Menu == null) {
				Menu = new PopupMenu();
				this.PopupControl = Menu;
			}
			Menu.ItemLinks.Clear();
			foreach (var item in Items.Values) {
				Menu.ItemLinks.Add(item);
			}
		}
	}
	internal class BarInstanceInitializerSplitButtonItemLink : BarSplitButtonItemLink {
		static BarInstanceInitializerSplitButtonItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(BarInstanceInitializerSplitButtonItemLink), typeof(BarInstanceInitializerSplitButtonItemLinkControl), li => new BarInstanceInitializerSplitButtonItemLinkControl());
		}
	}
	internal class BarInstanceInitializerSplitButtonItemLinkControl : BarSplitButtonItemLinkControl {
		protected BarInstanceInitializerSplitButtonItemLink InitializerLink {
			get { return Link as BarInstanceInitializerSplitButtonItemLink; }
		}
		protected BarInstanceInitializerSplitButtonItem InitializerItem {
			get { return Item as BarInstanceInitializerSplitButtonItem; }
		}
		protected override void OnContainerTypeChanged(LinkContainerType oldValue) {
			base.OnContainerTypeChanged(oldValue);
			if (InitializerItem != null)
				InitializerItem.InitializeData();
		}
		protected override void UpdateLayoutPanelArrowAlignment() {
			base.UpdateLayoutPanelArrowAlignment();
			UpdateLayoutPanelArrowAndSecondBorderByItems();
		}
		protected override void UpdateLayoutPanelShowSecondBorder() {
			base.UpdateLayoutPanelShowSecondBorder();
			UpdateLayoutPanelArrowAndSecondBorderByItems();
		}
		protected virtual void UpdateLayoutPanelArrowAndSecondBorderByItems() {
			if (InitializerItem == null || LayoutPanel == null || !InitializerItem.HasItems)
				return;
			LayoutPanel.ShowArrow &= !InitializerItem.HasCommonValue;
			LayoutPanel.ShowSecondBorder &= !InitializerItem.HasCommonValue;
		}
	}
	internal class BarInstanceInitializerItem : BarButtonItem {
		static BarInstanceInitializerItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(BarInstanceInitializerItem), typeof(BarInstanceInitializerItemLink), i => new BarInstanceInitializerItemLink());
		}
		protected DataViewBase DataViewBase { get { return RowDataGenerator.With(x => x.DataView); } }
		protected RowDataGenerator RowDataGenerator { get; set; }
		protected RowHandle Handle { get; set; }
		protected TypeInfo Info { get; set; }
		protected BarInstanceInitializerSplitButtonItem Owner { get; set; }
		public BarInstanceInitializerItem(RowDataGenerator generator, RowHandle handle, TypeInfo info, BarInstanceInitializerSplitButtonItem owner) {
			Handle = handle;
			RowDataGenerator = generator;
			Info = info;
			Owner = owner;
			InitializeData();
		}
		protected virtual void InitializeData() {
			Content = Info.Name ?? Info.Type.Name;
			Command = DelegateCommandFactory.Create(OnCommandExecuted, () => true, false);
		}
		protected virtual void OnCommandExecuted() {
			if (Owner.IsCollectionInitializer) {
				RowDataGenerator.View.CollectionHelper.AddCollectionItem(RowDataGenerator.RowDataFromHandle(Handle), Info);
			}
			else {
				DataViewBase.SetValue(Handle, Info);
			}
			Owner.CurrentTypeInfo = Info;
		}
	}
	internal class BarInstanceInitializerItemLink : BarButtonItemLink {
	}
}

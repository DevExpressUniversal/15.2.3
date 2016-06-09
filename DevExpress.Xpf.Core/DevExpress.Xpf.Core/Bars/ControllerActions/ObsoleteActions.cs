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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[ContentProperty("Item")]
	public class InsertBarItemAction : BarManagerControllerActionBase {
		#region static
		public static readonly DependencyProperty ItemProperty;
		public static readonly DependencyProperty ItemIndexProperty;
		static InsertBarItemAction() {
			ItemProperty = DependencyPropertyManager.Register("Item", typeof(BarItem), typeof(InsertBarItemAction), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncElementProperty));
			ItemIndexProperty = DependencyPropertyManager.RegisterAttached("ItemIndex", typeof(int), typeof(InsertBarItemAction), new FrameworkPropertyMetadata(-1, CollectionActionHelper.SyncIndexProperty));
			CollectionAction.KindProperty.OverrideMetadata(typeof(InsertBarItemAction), new FrameworkPropertyMetadata(CollectionActionKind.Insert));
		}
		public static int GetItemIndex(DependencyObject d) { return (int)d.GetValue(ItemIndexProperty); }
		public static void SetItemIndex(DependencyObject d, int value) { d.SetValue(ItemIndexProperty, value); }
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("InsertBarItemActionItem")]
#endif
		public BarItem Item {
			get { return (BarItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("InsertBarItemActionItemIndex")]
#endif
		public virtual int ItemIndex {
			get { return GetItemIndex(this); }
			set { SetItemIndex(this, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			var wrapper = new CollectionActionWrapper(this, context);
			if (wrapper.Container is BarManager || wrapper.Container == null) {
				wrapper.Container = wrapper.Container ?? Manager;
				wrapper.Collection = CollectionActionHelper.Instance.GetCollectionForElement<BarManager, BarItem>(Manager, GetObjectCore(), this);
			}
			CollectionActionHelper.Execute(wrapper);
		}
		public override object GetObjectCore() {
			return Item;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			InsertBarItemAction act = action as InsertBarItemAction;
			bool res = base.IsEqual(act);
			if (!res) return false;
			if (act.Item == null || Item == null) return false;
			if (!string.IsNullOrEmpty(act.Item.Name) && act.Item.Name == Item.Name) return true;
			if (act.Item.Content != null && act.Item.Content == Item.Content) return true;
			return true;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class AddBarItemAction : InsertBarItemAction {
#if !SL
	[DevExpressXpfCoreLocalizedDescription("AddBarItemActionItemIndex")]
#endif
		public override int ItemIndex {
			get { return Manager.Items.Count; }
			set { }
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			AddBarItemAction act = action as AddBarItemAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return ItemIndex == act.ItemIndex;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BarItemActionBase : BarManagerControllerActionBase {
		#region static
		public static readonly DependencyProperty ItemNameProperty;
		public static readonly DependencyProperty ItemIndexProperty;
		static BarItemActionBase() {
			ItemNameProperty = DependencyPropertyManager.Register("ItemName", typeof(string), typeof(BarItemActionBase), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncElementNameProperty));
			ItemIndexProperty = DependencyPropertyManager.Register("ItemIndex", typeof(int), typeof(BarItemActionBase), new FrameworkPropertyMetadata(-1, CollectionActionHelper.SyncIndexProperty));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemActionBaseItemName")]
#endif
		public string ItemName {
			get { return (string)GetValue(ItemNameProperty); }
			set { SetValue(ItemNameProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemActionBaseItemIndex")]
#endif
		public virtual int ItemIndex {
			get { return (int)GetValue(ItemIndexProperty); }
			set { SetValue(ItemIndexProperty, value); }
		}
		protected BarItem GetItem() {
			var context = CollectionAction.GetContext(this);
			if (ItemName != null) {
				return (BarItem)CollectionActionHelper.Instance.FindElementByName(context, ItemName, Container, ScopeSearchSettings.Local, CollectionActionHelper.CanRemoveElement);
			} else if (ItemIndex != -1 && Manager != null)
				return Manager.Items[ItemIndex];
			return context as BarItem;
		}		
		public override object GetObjectCore() {
			return GetItem();
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			BarItemActionBase act = action as BarItemActionBase;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return ItemName == act.ItemName && ItemIndex == act.ItemIndex;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoveBarItemAction : BarItemActionBase {
		static RemoveBarItemAction() {
			CollectionAction.KindProperty.OverrideMetadata(typeof(RemoveBarItemAction), new FrameworkPropertyMetadata(CollectionActionKind.Remove));
		}
		protected override void ExecuteCore(DependencyObject context) {
			var wrapper = new CollectionActionWrapper(this, context);
			if (wrapper.Container is BarManager || wrapper.Container == null) {
				wrapper.Container = wrapper.Container ?? Manager;
				if (Manager != null)
					wrapper.Collection = CollectionActionHelper.Instance.GetCollectionForElement<BarManager, BarItem>(Manager, GetObjectCore(), this);
			}
			CollectionActionHelper.Execute(wrapper);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class UpdateBarItemAction : BarItemActionBase {
		#region static
		public static readonly DependencyProperty PropertyProperty;
		public static readonly DependencyProperty ValueProperty;
		static UpdateBarItemAction() {
			PropertyProperty = DependencyPropertyManager.Register("Property", typeof(DependencyProperty), typeof(UpdateBarItemAction), new FrameworkPropertyMetadata(null));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), typeof(UpdateBarItemAction), new FrameworkPropertyMetadata(null));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarItemActionProperty")]
#endif
		public DependencyProperty Property {
			get { return (DependencyProperty)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarItemActionValue")]
#endif
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			BarItem item = GetItem();
			if (item == null)
				return;
			item.SetValue(Property, Value);
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			UpdateBarItemAction act = action as UpdateBarItemAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return Property == act.Property && Value == act.Value;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class SetBarItemAction : InsertBarItemAction {
		static SetBarItemAction() {
			CollectionAction.KindProperty.OverrideMetadata(typeof(SetBarItemAction), new FrameworkPropertyMetadata(CollectionActionKind.Replace));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[ContentProperty("Bar")]
	public class InsertBarAction : BarManagerControllerActionBase {
		#region static
		public static readonly DependencyProperty BarProperty;
		public static readonly DependencyProperty BarIndexProperty;
		static InsertBarAction() {
			BarProperty = DependencyPropertyManager.Register("Bar", typeof(Bar), typeof(InsertBarAction), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncElementProperty));
			BarIndexProperty = DependencyPropertyManager.RegisterAttached("BarIndex", typeof(int), typeof(InsertBarAction), new FrameworkPropertyMetadata(-1, CollectionActionHelper.SyncIndexProperty));
			CollectionAction.KindProperty.OverrideMetadata(typeof(InsertBarAction), new FrameworkPropertyMetadata(CollectionActionKind.Insert));
		}
		public static int GetBarIndex(DependencyObject d) { return (int)d.GetValue(BarIndexProperty); }
		public static void SetBarIndex(DependencyObject d, int value) { d.SetValue(BarIndexProperty, value); }
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("InsertBarActionBar")]
#endif
		public Bar Bar {
			get { return (Bar)GetValue(BarProperty); }
			set { SetValue(BarProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("InsertBarActionBarIndex")]
#endif
		public virtual int BarIndex {
			get { return GetBarIndex(this); }
			set { SetBarIndex(this, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			CollectionActionHelper.Execute(new CollectionActionWrapper(this, context) { Container = Manager, Collection = Manager.Bars });
		}
		public override object GetObjectCore() {
			return Bar;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			return false;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class AddBarAction : InsertBarAction {
#if !SL
	[DevExpressXpfCoreLocalizedDescription("AddBarActionBarIndex")]
#endif
		public override int BarIndex {
			get { return Container.GetBarManager().Return(x => x.Bars.Count, () => 0); }
			set { }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class InsertMainMenuIfNotExistAction : InsertBarAction {
		protected override void ExecuteCore(DependencyObject context) {
			if (Manager.MainMenu != null)
				return;
			base.ExecuteCore(context);
			Bar.IsMainMenu = true;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class InsertStatusBarIfNotExistAction : InsertBarAction {
		protected override void ExecuteCore(DependencyObject context) {
			if (Manager.StatusBar != null)
				return;
			base.ExecuteCore(context);
			Bar.IsStatusBar = true;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BarActionBase : BarManagerControllerActionBase {
		#region static
		public static readonly DependencyProperty BarNameProperty;
		public static readonly DependencyProperty BarIndexProperty;
		static BarActionBase() {
			BarNameProperty = DependencyPropertyManager.Register("BarName", typeof(string), typeof(BarActionBase), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncElementNameProperty));
			BarIndexProperty = DependencyPropertyManager.Register("BarIndex", typeof(int), typeof(BarActionBase), new FrameworkPropertyMetadata(-1, CollectionActionHelper.SyncIndexProperty));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarActionBaseBarName")]
#endif
		public string BarName {
			get { return (string)GetValue(BarNameProperty); }
			set { SetValue(BarNameProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarActionBaseBarIndex")]
#endif
		public virtual int BarIndex {
			get { return (int)GetValue(BarIndexProperty); }
			set { SetValue(BarIndexProperty, value); }
		}
		protected Bar GetBar() {
			if (BarName != null) {
				if (Manager.Bars[BarName] != null)
					return Manager.Bars[BarName];
			}
			else if (BarIndex != -1)
				return Manager.Bars[BarIndex];
			return CollectionAction.GetContext(this) as Bar;
		}
		public override object GetObjectCore() {
			return null;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			BarActionBase act = action as BarActionBase;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return BarName == act.BarName && BarIndex == act.BarIndex;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoveBarAction : BarActionBase {
		static RemoveBarAction() {
			CollectionAction.KindProperty.OverrideMetadata(typeof(RemoveBarAction), new FrameworkPropertyMetadata(CollectionActionKind.Remove));
		}
		protected override void ExecuteCore(DependencyObject context) {
			CollectionActionHelper.Execute(new CollectionActionWrapper(this, context) { Container = Manager, Collection = Manager.Bars });
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class SetBarAction : InsertBarAction {
		static SetBarAction() {
			CollectionAction.KindProperty.OverrideMetadata(typeof(SetBarAction), new FrameworkPropertyMetadata(CollectionActionKind.Replace));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class UpdateBarAction : BarActionBase {
		#region static
		public static readonly DependencyProperty PropertyProperty;
		public static readonly DependencyProperty ValueProperty;
		static UpdateBarAction() {
			PropertyProperty = DependencyPropertyManager.Register("Property", typeof(DependencyProperty), typeof(UpdateBarAction), new FrameworkPropertyMetadata(null));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), typeof(UpdateBarAction), new FrameworkPropertyMetadata(null));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarActionProperty")]
#endif
		public DependencyProperty Property {
			get { return (DependencyProperty)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarActionValue")]
#endif
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			Bar bar = GetBar();
			if (bar == null)
				return;
			bar.SetValue(Property, Value);
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			UpdateBarAction act = action as UpdateBarAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return Property == act.Property && Value == act.Value;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class BarItemLinkActionBase : BarManagerControllerActionBase {
		#region static
		public static readonly DependencyProperty ItemLinkIndexProperty;
		public static readonly DependencyProperty TargetProperty;
		public static readonly DependencyProperty TargetTypeProperty;
		static BarItemLinkActionBase() {
			ItemLinkIndexProperty = DependencyPropertyManager.RegisterAttached("ItemLinkIndex", typeof(int), typeof(BarItemLinkActionBase), new FrameworkPropertyMetadata(-1, CollectionActionHelper.SyncIndexProperty));
			TargetProperty = DependencyPropertyManager.RegisterAttached("Target", typeof(string), typeof(BarItemLinkActionBase), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncContainerNameProperty));
			TargetTypeProperty = DependencyPropertyManager.RegisterAttached("TargetType", typeof(ItemLinksHolderType), typeof(BarItemLinkActionBase), new FrameworkPropertyMetadata(ItemLinksHolderType.Other, OnTargetTypeChanged));
		}
		static void OnTargetTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) {
			ItemLinksHolderType tType = (ItemLinksHolderType)args.NewValue;
			if (tType == ItemLinksHolderType.MainMenu) {
				BindingOperations.SetBinding(d, CollectionAction.ContainerProperty, new Binding() { Path = new PropertyPath("(0).(1)", BarManager.BarManagerProperty, BarManager.MainMenuProperty), RelativeSource = RelativeSource.Self });
				return;
			}
			if (tType == ItemLinksHolderType.StatusBar) {
				BindingOperations.SetBinding(d, CollectionAction.ContainerProperty, new Binding() { Path = new PropertyPath("(0).(1)", BarManager.BarManagerProperty, BarManager.StatusBarProperty), RelativeSource = RelativeSource.Self });
				return;
			}
			d.ClearValue(CollectionAction.ContainerProperty);
		}
		public static int GetItemLinkIndex(DependencyObject d) { return (int)d.GetValue(ItemLinkIndexProperty); }
		public static void SetItemLinkIndex(DependencyObject d, int value) { d.SetValue(ItemLinkIndexProperty, value); }
		public static string GetTarget(DependencyObject d) { return (string)d.GetValue(TargetProperty); }
		public static void SetTarget(DependencyObject d, string value) { d.SetValue(TargetProperty, value); }
		public static ItemLinksHolderType GetTargetType(DependencyObject d) { return (ItemLinksHolderType)d.GetValue(TargetTypeProperty); }
		public static void SetTargetType(DependencyObject d, ItemLinksHolderType value) { d.SetValue(TargetTypeProperty, value); }
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActionBaseItemLinkIndex")]
#endif
		public virtual int ItemLinkIndex {
			get { return GetItemLinkIndex(this); }
			set { SetItemLinkIndex(this, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActionBaseTarget")]
#endif
		public string Target {
			get { return GetTarget(this); }
			set { SetTarget(this, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarItemLinkActionBaseTargetType")]
#endif
		public ItemLinksHolderType TargetType {
			get { return GetTargetType(this); }
			set { SetTargetType(this, value); }
		}
		public static ILinksHolder GetLinksHolder(DependencyObject context, string name, IActionContainer container, ItemLinksHolderType targetType) {
			if (context is ILinksHolder)
				return context as ILinksHolder;
			BarManager manager = container.GetBarManager();
			ILinksHolder popupHolder = container.AssociatedObject as ILinksHolder;
			if (popupHolder != null)
				return popupHolder;
			if (manager != null) {
				if (targetType == ItemLinksHolderType.MainMenu)
					return manager.MainMenu;
				if (targetType == ItemLinksHolderType.StatusBar)
					return manager.StatusBar;
			}
			return
				BarNameScope.GetService<IElementRegistratorService>(container.AssociatedObject).GetElements<ILinksHolder>(name).FirstOrDefault()
			?? BarNameScope.GetService<IElementRegistratorService>(container).GetElements<ILinksHolder>(name).FirstOrDefault();
		}
		protected virtual ILinksHolder GetTarget(string name) {
			return GetLinksHolder(CollectionAction.GetContext(this), name, Container, TargetType);
		}
		public override object GetObjectCore() {
			return null;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			BarItemLinkActionBase act = action as BarItemLinkActionBase;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return ItemLinkIndex == act.ItemLinkIndex && Target == act.Target && TargetType == act.TargetType;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	[ContentProperty("ItemLink")]
	public class InsertBarItemLinkAction : BarItemLinkActionBase {
		#region static
		public static readonly DependencyProperty ItemLinkProperty;
		static InsertBarItemLinkAction() {
			ItemLinkProperty = DependencyPropertyManager.Register("ItemLink", typeof(BarItemLinkBase), typeof(InsertBarItemLinkAction), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncElementProperty));
			CollectionAction.KindProperty.OverrideMetadata(typeof(InsertBarItemLinkAction), new FrameworkPropertyMetadata(CollectionActionKind.Insert));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("InsertBarItemLinkActionItemLink")]
#endif
		public BarItemLinkBase ItemLink {
			get { return (BarItemLinkBase)GetValue(ItemLinkProperty); }
			set { SetValue(ItemLinkProperty, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			var wrapper = new CollectionActionWrapper(this, context);
			if (wrapper.Container == null) {
				wrapper.Container = GetTarget(Target);
				wrapper.Collection = CollectionActionHelper.Instance.GetCollectionForElement(wrapper.Container, wrapper.Element, this);
			}
			CollectionActionHelper.Execute(wrapper);
		}
		public override object GetObjectCore() {
			return ItemLink;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			InsertBarItemLinkAction act = action as InsertBarItemLinkAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			if (ItemLink == null || act.ItemLink == null) return false;
			if (ItemLink.GetType() != act.ItemLink.GetType()) return false;
			if (ItemLink is BarItemLinkSeparator) return true;
			BarItemLink link1 = ItemLink as BarItemLink;
			BarItemLink link2 = act.ItemLink as BarItemLink;
			if (link1 == null || link2 == null) return false;
			if (!string.IsNullOrEmpty(link1.BarItemName) && link1.BarItemName != link2.BarItemName) return false;
			return true;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class AddBarItemLinkAction : InsertBarItemLinkAction {
#if !SL
	[DevExpressXpfCoreLocalizedDescription("AddBarItemLinkActionItemLinkIndex")]
#endif
		public override int ItemLinkIndex {
			get {
				ILinksHolder holder = GetTarget(Target);
				if (holder == null)
					return -1;
				return holder.Links.Count;
			}
			set { }
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			InsertBarItemLinkAction act = action as InsertBarItemLinkAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return ItemLinkIndex == act.ItemLinkIndex;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class UpdateBarItemLinkActionBase : BarItemLinkActionBase {
		#region static
		public static readonly DependencyProperty ItemLinkNameProperty;
		static UpdateBarItemLinkActionBase() {
			ItemLinkNameProperty = DependencyPropertyManager.Register("ItemLinkName", typeof(string), typeof(UpdateBarItemLinkActionBase), new FrameworkPropertyMetadata(null, CollectionActionHelper.SyncElementNameProperty));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarItemLinkActionBaseItemLinkName")]
#endif
		public string ItemLinkName {
			get { return (string)GetValue(ItemLinkNameProperty); }
			set { SetValue(ItemLinkNameProperty, value); }
		}
		protected BarItemLinkBase GetLink() {
			ILinksHolder holder = GetTarget(Target);
			if (holder == null)
				return null;
			if (ItemLinkName != null)
				return holder.Links[ItemLinkName];
			if (ItemLinkIndex != -1 && holder.Links.Count > ItemLinkIndex)
				return holder.Links[ItemLinkIndex];
			return null;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			UpdateBarItemLinkActionBase act = action as UpdateBarItemLinkActionBase;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return ItemLinkName == act.ItemLinkName;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoveBarItemLinkAction : UpdateBarItemLinkActionBase {
		static RemoveBarItemLinkAction() {
			CollectionAction.KindProperty.OverrideMetadata(typeof(RemoveBarItemLinkAction), new FrameworkPropertyMetadata(CollectionActionKind.Remove));
		}
		protected override void ExecuteCore(DependencyObject context) {
			CollectionActionHelper.Execute(new CollectionActionWrapper(this, context));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class SetBarItemLinkAction : InsertBarItemLinkAction {
		static SetBarItemLinkAction() {
			CollectionAction.KindProperty.OverrideMetadata(typeof(SetBarItemLinkAction), new FrameworkPropertyMetadata(CollectionActionKind.Replace));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class UpdateBarItemLinkAction : UpdateBarItemLinkActionBase {
		#region static
		public static readonly DependencyProperty PropertyProperty;
		public static readonly DependencyProperty ValueProperty;
		static UpdateBarItemLinkAction() {
			PropertyProperty = DependencyPropertyManager.Register("Property", typeof(DependencyProperty), typeof(UpdateBarItemLinkAction), new FrameworkPropertyMetadata(null));
			ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), typeof(UpdateBarItemLinkAction), new FrameworkPropertyMetadata(null));
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarItemLinkActionProperty")]
#endif
		public DependencyProperty Property {
			get { return (DependencyProperty)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("UpdateBarItemLinkActionValue")]
#endif
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected override void ExecuteCore(DependencyObject context) {
			base.ExecuteCore(context);
			BarItemLinkBase link = GetLink();
			if (link != null)
				link.SetValue(Property, Value);
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			UpdateBarItemLinkAction act = action as UpdateBarItemLinkAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return Property == act.Property && Value == act.Value;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoveBarItemAndLinkAction : RemoveBarItemAction {
		protected override void ExecuteCore(DependencyObject context) {
			RemoveLinks();
			base.ExecuteCore(context);
		}
		protected virtual void RemoveLinks() {
			BarItem item = GetItem();
			if (item != null) {
				List<BarItemLinkBase> links = new List<BarItemLinkBase>(item.Links);
				foreach (BarItemLinkBase link in links) {
					if (link.Links != null && !link.CommonBarItemCollectionLink)
						link.Links.Remove(link);
				}
			}
else {
				ILinksHolder holder = BarItemLinkActionBase.GetLinksHolder(CollectionAction.GetContext(this), string.Empty, Container, ItemLinksHolderType.Other);
				if (holder != null && !string.IsNullOrEmpty(ItemName)) {
					BarItemLinkBase link = BarManagerHelper.GetLinkByItemName(ItemName, holder.Links);
					if (link != null && link.Links != null) {
						link.Links.Remove(link);
					}
				}
			}
		}
	}
}

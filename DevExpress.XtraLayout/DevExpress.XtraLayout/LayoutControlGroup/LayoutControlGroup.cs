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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Utils;
using Padding = DevExpress.XtraLayout.Utils.Padding;
namespace DevExpress.XtraLayout {
	[DesignTimeVisible(false)]
#if DEBUGTEST
	[System.Diagnostics.DebuggerVisualizer(typeof(Diagnostics.SimpleVisualizer), typeof(Diagnostics.BaseLayoutItemVisualizerObjectSource))]
#endif
	public class LayoutControlGroup : LayoutGroup {
		public LayoutControlGroup()
			: base(null) {
			TextLocation = Locations.Top;
		}
		protected override Padding DefaultSpaces {
			get {
				if(Owner != null) {
					if(Owner.RootGroup == this)
						return GroupBordersVisible ? Owner.DefaultValues.RootGroupSpacing : Owner.DefaultValues.RootGroupWithoutBordersSpacing;
					else return GroupBordersVisible ? Owner.DefaultValues.GroupSpacing : Owner.DefaultValues.GroupWithoutBordersSpacing;
				}
				return new Padding(2);
			}
		}
		public new LayoutControlItem AddItem() {
			return base.AddItem() as LayoutControlItem;
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ViewInfo.LayoutControlGroupViewInfo ViewInfo {
			get {
				return base.ViewInfo as ViewInfo.LayoutControlGroupViewInfo;
			}
		}
		public new LayoutControlItem AddItem(String text) {
			return base.AddItem(text) as LayoutControlItem;
		}
		public LayoutControlItem AddItem(String text, Control control) {
			return AddItem(text, control, null, DefaultInsertType);
		}
		public new LayoutControlItem AddItem(BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddItem(baseItem, insertType) as LayoutControlItem;
		}
		public LayoutControlItem AddItem(String text, Control control, BaseLayoutItem baseItem, InsertType insertType) {
			LayoutControlItem result = base.AddItem(text, baseItem, insertType) as LayoutControlItem;
			result.Control = control;
			return result;
		}
		public new LayoutControlItem AddItem(BaseLayoutItem newItem) {
			return base.AddItem(newItem) as LayoutControlItem;
		}
		public new LayoutControlItem AddItem(BaseLayoutItem newItem, BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddItem(newItem, baseItem, insertType) as LayoutControlItem;
		}
		protected LayoutControlItem AddItem(String text, Control control, BaseLayoutItem newItem, BaseLayoutItem baseItem, InsertType insertType) {
			LayoutControlItem result = base.AddItem(text, newItem, baseItem, insertType) as LayoutControlItem;
			result.Control = control;
			return result;
		}
		public new LayoutControlGroup AddGroup() {
			return base.AddGroup() as LayoutControlGroup;
		}
		public new LayoutControlGroup AddGroup(String text) {
			return base.AddGroup(text) as LayoutControlGroup;
		}
		public new LayoutControlGroup AddGroup(BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddGroup(baseItem, insertType) as LayoutControlGroup;
		}
		public new LayoutControlGroup AddGroup(String text, BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddGroup(text, baseItem, insertType) as LayoutControlGroup;
		}
		public new LayoutControlGroup AddGroup(LayoutGroup newGroup) {
			return base.AddGroup(newGroup) as LayoutControlGroup;
		}
		public new LayoutControlGroup AddGroup(LayoutGroup newGroup, BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddGroup(newGroup, baseItem, insertType) as LayoutControlGroup;
		}
		protected LayoutControlGroup AddGroup(String text, LayoutControlGroup newGroup, BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddGroup(text, newGroup, baseItem, insertType) as LayoutControlGroup;
		}
		public new TabbedControlGroup AddTabbedGroup() {
			return base.AddTabbedGroup() as TabbedControlGroup;
		}
		public new TabbedControlGroup AddTabbedGroup(TabbedGroup newTabbedGroup) {
			return base.AddTabbedGroup(newTabbedGroup) as TabbedControlGroup;
		}
		public new TabbedControlGroup AddTabbedGroup(BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddTabbedGroup(baseItem, insertType) as TabbedControlGroup;
		}
		public new TabbedControlGroup AddTabbedGroup(TabbedGroup newTabbedGroup, BaseLayoutItem baseItem, InsertType insertType) {
			return base.AddTabbedGroup(newTabbedGroup, baseItem, insertType) as TabbedControlGroup;
		}
		public override void HideToCustomization() {
			if(!AllowHide || IsHidden) return;
			using(new SafeBaseLayoutItemChanger(this)) {
				base.HideToCustomization();
				if(ParentTabbedGroup != null) {
					if(Owner != null) Owner.AllowManageDesignSurfaceComponents = false;
					ParentTabbedGroup.RemoveTabPage(this);
					if(Owner != null) Owner.AllowManageDesignSurfaceComponents = true;
				}
			}
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new ViewInfo.LayoutControlGroupViewInfo(this);
		}
		public LayoutControlGroup(LayoutGroup lg)
			: base(lg) {
			IsOwnerInvalidating = true;
			TextLocation = Locations.Top;
			IsOwnerInvalidating = false;
		}
		protected internal override bool InsertItem(Customization.LayoutItemDragController controller) {
			if(base.InsertItem(controller)) {
				LayoutControlItem controlItem = controller.DragItem as LayoutControlItem;
				if(controlItem != null) {
					if(controlItem.Control != null) {
						controlItem.Control.Visible = true;
					}
				}
				return true;
			}
			return false;
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlGroupOwner")]
#endif
		public override ILayoutControl Owner {
			get { return base.Owner; }
			set {
				object oldOwner = Owner;
				base.Owner = value;
				if(oldOwner == null && Owner != null && updateCount != 0) {
					UpdatedCount += updateCount;
					updateCount = 0;
				}
				ArrayList tempItems = new ArrayList(Items);
				foreach(BaseLayoutItem item in tempItems) {
					item.Owner = value;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(disposingFlagCore) return;
				try {
					disposingFlagCore = true;
					if(Owner != null && Owner.FocusHelper != null && Owner.FocusHelper.SelectedComponent == this) Owner.FocusHelper.SelectedComponent = null;
					using(new SafeBaseLayoutItemChanger(this)) {
						ArrayList itemsToDispose = new ArrayList(Items);
						BeginUpdate();
						foreach(BaseLayoutItem item in itemsToDispose) {
							item.Dispose();
						}
						EndUpdate();
						if(ParentTabbedGroup != null) {
							ParentTabbedGroup.RemoveTabPage(this);
							tabbedGroupParentCore = null;
						}
						if(this.viewInfoCore != null) this.viewInfoCore.Destroy();
						this.viewInfoCore = null;
						Items.Changed -= OnItems_Changed;
					}
					if(Owner != null && Owner.RootGroup == this) {
						ILayoutControl lOwner = Owner;
						lOwner.BeginUpdate();
						lOwner.RootGroup = null;
						lOwner.EndUpdate();
					}
				}
				finally { Owner = null; }
			}
			base.Dispose(disposing);
		}
		protected internal override TabbedGroup CreateTabbedGroup() {
			if(Owner != null) {
				return Owner.CreateTabbedGroup(this);
			}
			return new TabbedControlGroup(this);
		}
		protected internal override LayoutGroup GroupItems(BaseItemCollection items) {
			LayoutGroup lg = base.GroupItems(items);
			return lg;
		}
		protected internal override LayoutItem CreateLayoutItem() {
			if(Owner != null) {
				return (LayoutItem)Owner.CreateLayoutItem(this);
			}
			return new LayoutControlItem(this);
		}
		protected internal override LayoutGroup CreateLayoutGroup() {
			if(Owner != null) {
				return Owner.CreateLayoutGroup(this);
			}
			LayoutControlGroup lcg = new LayoutControlGroup(this);
			lcg.Owner = Owner;
			return lcg;
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlGroupItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		new public LayoutGroupItemCollection Items {
			get {
				return base.Items;
			}
		}
		public void AddRange(BaseLayoutItem[] items) {
			Items.AddRange(items);
		}
		protected override Type GetDefaultWrapperType() {
			if(Parent != null && Parent.LayoutMode == Utils.LayoutMode.Table) {
				if(LayoutMode == Utils.LayoutMode.Table) return typeof(TableLayoutControlGroupWrapperInTableGroup);
				else return typeof(LayoutControlGroupWrapperInTableGroup);
			}
		   if(LayoutMode == Utils.LayoutMode.Table) return typeof(TableLayoutControlGroupWrapper);
		   if(LayoutMode == Utils.LayoutMode.Flow) return typeof(FlowLayoutControlGroupWrapper);
		   else return typeof(LayoutControlGroupWrapper); 
		}
	}
	public class LayoutControlGroupWrapper : BaseLayoutItemWrapper {
		protected LayoutControlGroup Group { get { return WrappedObject as LayoutControlGroup; } }
		[Category("Appearance")]
		public virtual AppearanceObject AppearanceGroup { get { return Group.AppearanceGroup; } }
		[DefaultValue(true)]
		public virtual bool AllowCustomizeChildren { get { return Group.AllowCustomizeChildren; } set { Group.AllowCustomizeChildren = value; OnSetValue(Item, value); } }
		[DefaultValue(true)]
		public virtual bool Enabled { get { return Group.Enabled; } set { Group.Enabled = value; OnSetValue(Item, value); } }
		[DefaultValue(true), Category("Group")]
		public virtual bool Expanded { get { return Group.Expanded; } set { Group.Expanded = value; OnSetValue(Item, value); } }
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean EnableIndentsWithoutBorders { get { return Group.EnableIndentsWithoutBorders; } set { Group.EnableIndentsWithoutBorders = value; OnSetValue(Item, value); } }
		[DefaultValue(GroupElementLocation.Default),Category("Group")]
		public virtual GroupElementLocation HeaderButtonsLocation { get { return Group.HeaderButtonsLocation; } set { Group.HeaderButtonsLocation = value; OnSetValue(Item, value); } }
		[DefaultValue(ExpandButtonMode.Normal), Category("Group")]
		public virtual ExpandButtonMode ExpandButtonMode { get { return Group.ExpandButtonMode; } set { Group.ExpandButtonMode = value; OnSetValue(Item, value); } }
		[DefaultValue(false), Category("Group")]
		public virtual bool AllowBorderColorBlending { get { return Group.AllowBorderColorBlending; } set { Group.AllowBorderColorBlending = value; OnSetValue(Item, value); } }
		[DefaultValue(false), Category("Group")]
		public virtual bool ExpandButtonVisible { get { return Group.ExpandButtonVisible; } set { Group.ExpandButtonVisible = value; OnSetValue(Item, value); } }
		[DefaultValue(LayoutMode.Regular), Category("Group")]
		public virtual LayoutMode LayoutMode { get { return Group.LayoutMode; } set {Group.LayoutMode = value; OnSetValue(Item,value); } }
		[DefaultValue(true), Category("Group")]
		public virtual bool GroupBordersVisible { get { return Group.GroupBordersVisible; } set { Group.GroupBordersVisible = value; OnSetValue(Item, value); } }
		public OptionsItemTextGroup OptionsItemText { get { return Group.OptionsItemText; } }
		[DefaultValue(-1), Category("Caption Image")]
		public virtual int CaptionImageIndex { get { return Group.CaptionImageIndex; } set { Group.CaptionImageIndex = value; OnSetValue(Item, value); } }
		[DefaultValue(GroupElementLocation.Default), Category("Caption Image")]
		public virtual GroupElementLocation CaptionImageLocation { get { return Group.CaptionImageLocation; } set { Group.CaptionImageLocation = value; OnSetValue(Item, value); } }
		[DefaultValue(true), Category("Caption Image")]
		public virtual bool CaptionImageVisible { get { return Group.CaptionImageVisible; } set { Group.CaptionImageVisible = value; OnSetValue(Item, value); } }
		[Browsable(false)]
		public override int TextToControlDistance { get { return base.TextToControlDistance; } set { base.TextToControlDistance = value; OnSetValue(Item, value); } }
		public OptionsPrintGroup OptionsPrint { get { return Group.OptionsPrint; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new LayoutControlGroupWrapper();
		}
	}
	public class FlowLayoutControlGroupWrapper : LayoutControlGroupWrapper {
		[Category("OptionsFlowLayoutGroup")]
		public virtual Size CellSize { get { return Group.CellSize; } set { Group.CellSize = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new FlowLayoutControlGroupWrapper();
		}
	}
	public class TableLayoutControlGroupWrapper :LayoutControlGroupWrapper {
		[Category("OptionsTableLayoutGroup")]
		public virtual RowDefinitions RowDefinitions { get { return Group.OptionsTableLayoutGroup.RowDefinitions; } }
		[Category("OptionsTableLayoutGroup")]
		public virtual ColumnDefinitions ColumnDefinitions { get { return Group.OptionsTableLayoutGroup.ColumnDefinitions; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TableLayoutControlGroupWrapper();
		}
	}
	public class LayoutControlGroupWrapperInTableGroup :LayoutControlGroupWrapper {
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int RowSpan { get { return Group.OptionsTableLayoutItem.RowSpan; } set { Group.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Group, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Group.OptionsTableLayoutItem.ColumnSpan; } set { Group.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Group, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Group.OptionsTableLayoutItem.RowIndex; } set { Group.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Group, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Group.OptionsTableLayoutItem.ColumnIndex; } set { Group.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Group, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new LayoutControlGroupWrapperInTableGroup();
		}
	}
	public class TableLayoutControlGroupWrapperInTableGroup :TableLayoutControlGroupWrapper {
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int RowSpan { get { return Group.OptionsTableLayoutItem.RowSpan; } set { Group.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Group, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Group.OptionsTableLayoutItem.ColumnSpan; } set { Group.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Group, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Group.OptionsTableLayoutItem.RowIndex; } set { Group.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Group, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Group.OptionsTableLayoutItem.ColumnIndex; } set { Group.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Group, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TableLayoutControlGroupWrapperInTableGroup();
		}
	}
}

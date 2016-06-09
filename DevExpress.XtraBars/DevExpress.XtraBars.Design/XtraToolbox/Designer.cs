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

using DevExpress.Utils.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox.Design {
	public class ToolboxControlDesigner : BaseParentControlDesigner {
		DesignerActionListCollection actionLists;
		public ToolboxControlDesigner() {
			this.actionLists = null;
		}
		public override DesignerActionListCollection ActionLists {
			get {
				if(this.actionLists == null) {
					this.actionLists = new DesignerActionListCollection();
					this.actionLists.Add(new ToolboxActionList(Owner));
				}
				return this.actionLists;
			}
		}
		public ToolboxControl Owner { get { return Component as ToolboxControl; } }
		protected virtual bool GetHitTestCore(Point client) {
			ToolboxHitInfo hInfo = Owner.DesignManager.ViewInfo.CalcHitInfo(client);
			return AllowHitTest.Contains(hInfo.HitTest);
		}
		static List<ToolboxHitTest> AllowHitTest = new List<ToolboxHitTest>() {
			ToolboxHitTest.Group, ToolboxHitTest.Item, ToolboxHitTest.ExpandButton, ToolboxHitTest.ScrollButtonUp, ToolboxHitTest.ScrollButtonDown, ToolboxHitTest.ScrollBar
		};
		protected override bool GetHitTest(Point point) {
			bool res = base.GetHitTest(point);
			if(!AllowDesigner || DebuggingState)
				return res;
			if(Owner == null || res)
				return res;
			Point client = Owner.PointToClient(point);
			return GetHitTestCore(client);
		}
		protected override bool CanUseComponentSmartTags {
			get { return true; }
		}
	}
	public class ToolboxActionList : DesignerActionList {
		public ToolboxActionList(ToolboxControl owner)
			: base(owner) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection col = new DesignerActionItemCollection();
			col.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", true));
			col.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style"));
			return col;
		}
		public DockStyle Dock {
			get { return Owner.Dock; }
			set { EditorContextHelper.SetPropertyValue(Component.Site, Owner, "Dock", value); }
		}
		public void AddGroup() {
			if(DesignerHost == null || Owner == null) return;
			ToolboxGroup group = new ToolboxGroup();
			DesignerHost.Container.Add(group);
			group.Caption = group.Site.Name;
			Owner.Groups.Add(group);
		}
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		public ToolboxControl Owner { get { return Component as ToolboxControl; } }
	}
	public class ToolboxElementDesigner : BaseComponentDesigner {
		protected override bool AllowInheritanceWrapper { get { return true; } }
		protected override bool AllowEditInherited { get { return false; } }
		protected ToolboxElementBase Element { get { return (ToolboxElementBase)Component; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XtraToolboxElementActionList(this));
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		void OnDragDrop(object sender, EventArgs e) {
			BaseDesignerActionListGlyphHelper.RefreshSmartPanelBounds(Component);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Element != null) {
				}
			}
			base.Dispose(disposing);
		}
	}
	public class AccordionDesignerActionMethodItem : DesignerActionMethodItem {
		public AccordionDesignerActionMethodItem(DesignerActionList list, string memberName, string displayName, string category)
			: base(list, memberName, displayName, category) {
			this.list = list;
		}
		DesignerActionList list;
		public override void Invoke() {
			base.Invoke();
			BaseDesignerActionListGlyphHelper.RefreshSmartPanelBounds(list.Component);
		}
	}
	public class XtraToolboxElementActionList : DesignerActionList {
		ToolboxElementDesigner designer;
		public XtraToolboxElementActionList(ToolboxElementDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionHeaderItem("Appearance", "Appearance"));
			res.Add(new DesignerActionPropertyItem("Caption", "Caption", "Appearance"));
			res.Add(new DesignerActionHeaderItem("Actions"));
			res.Add(new DesignerActionMethodItem(this, "AddItem", "Add Item", "Actions"));
			if(Element is ToolboxGroup) {
				res.Add(new DesignerActionMethodItem(this, "AddGroup", "Add Group", "Actions"));
				res.Add(new AccordionDesignerActionMethodItem(this, "SelectGroup", "Select Group", "Actions"));
			}
			res.Add(new AccordionDesignerActionMethodItem(this, "MoveTowardBeginning", "Move Toward Beginning", "Actions"));
			res.Add(new AccordionDesignerActionMethodItem(this, "MoveTowardEnd", "Move Toward End", "Actions"));
			return res;
		}
		protected void SetPropertyValue(string property, object value) {
			EditorContextHelper.SetPropertyValue(Designer, Component, property, value);
		}
		public ToolboxElementDesigner Designer { get { return designer; } }
		IDesignerHost DesignerHost { get { return GetService(typeof(IDesignerHost)) as IDesignerHost; } }
		public ToolboxElementBase Element { get { return (ToolboxElementBase)Component; } }
		public ToolboxGroup Group { get { return Component as ToolboxGroup; } }
		public ToolboxItem Item { get { return Component as ToolboxItem; } }
		public ToolboxControl Toolbox { get { return (ToolboxControl)Element.Owner; } }
		[DefaultValue("")]
		public string Caption {
			get { return Element.Caption; }
			set { SetPropertyValue("Caption", value); }
		}
		public void AddItem() {
			if(DesignerHost == null) return;
			ToolboxItem item = new ToolboxItem();
			DesignerHost.Container.Add(item);
			item.Caption = item.Site.Name;
			AddItemCore(item);
		}
		protected void AddItemCore(ToolboxItem item) {
			if(Group != null) {
				Group.Items.Add(item);
				return;
			}
			if(Item == null || Item.OwnerGroup == null) return;
			int insertIndex = Item.OwnerGroup.Items.IndexOf(Item) + 1;
			Item.OwnerGroup.Items.Insert(insertIndex, item);
		}
		public void AddGroup() {
			if(DesignerHost == null || Toolbox == null) return;
			ToolboxGroup group = new ToolboxGroup();
			DesignerHost.Container.Add(group);
			group.Caption = group.Site.Name;
			int insertIndex = Toolbox.Groups.IndexOf(Group) + 1;
			Toolbox.Groups.Insert(insertIndex, group);
		}
		public void MoveTowardBeginning() {
			int index = -1;
			if(Group != null) {
				ToolboxControl toolbox = Toolbox;
				index = toolbox.Groups.IndexOf(Group);
				if(index > 0) {
					toolbox.Groups.Remove(Group);
					toolbox.Groups.Insert(index - 1, Group);
				}
			}
			else {
				ToolboxGroup ownerGroup = Item.OwnerGroup;
				index = ownerGroup.Items.IndexOf(Item);
				if(index > 0) {
					ownerGroup.Items.Remove(Item);
					ownerGroup.Items.Insert(index - 1, Item);
				}
			}
			Element.Owner.LayoutChanged();
		}
		public void MoveTowardEnd() {
			int index = -1;
			if(Group != null) {
				ToolboxControl toolbox = Toolbox;
				index = toolbox.Groups.IndexOf(Group);
				if(index < toolbox.Groups.Count - 1 && index != -1) {
					toolbox.Groups.Remove(Group);
					toolbox.Groups.Insert(index + 1, Group);
				}
			}
			else {
				ToolboxGroup ownerGroup = Item.OwnerGroup;
				index = ownerGroup.Items.IndexOf(Item);
				if(index < ownerGroup.Items.Count - 1 && index != -1) {
					ownerGroup.Items.Remove(Item);
					ownerGroup.Items.Insert(index - 1, Item);
				}
			}
			Element.Owner.LayoutChanged();
		}
		public void SelectGroup() {
			Toolbox.SelectedGroup = Group;
		}
	}
}

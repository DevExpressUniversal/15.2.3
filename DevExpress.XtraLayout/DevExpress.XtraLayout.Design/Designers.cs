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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Customization;
using System.Windows.Forms.Design;
using DevExpress.XtraLayout.Customization.Controls;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.Design {
	public class RowDefinitionsEditor :CollectionEditor {
		public RowDefinitionsEditor(Type type) : base(type) { }
		void form_Shown(object sender, EventArgs e) {
			if(collection != null) collection.Owner.BeginUpdate();
		}
		protected override object SetItems(object editValue, object[] value) {
			if(editValue != null && editValue is IList) {
				RowDefinitions list = (RowDefinitions)editValue;
				list.Clear();
				list.BeginUpdate();
				for(int index = 0; index < value.Length; ++index) {
					RowDefinition rowDefinition = value[index] as RowDefinition;
					list.Add(rowDefinition);
				}
				list.EndUpdate();
			}
			return editValue;
		}
		protected override string GetDisplayText(object value) {
			return "Row Definition";
		}
		CollectionForm form;
		RowDefinitions collection;
		protected override object CreateInstance(Type itemType) {
			RowDefinition row = new RowDefinition();
			row.SizeType = SizeType.AutoSize;
			return row;
		}
		protected override CollectionEditor.CollectionForm CreateCollectionForm() {
			form = base.CreateCollectionForm();
			form.Shown += form_Shown;
			form.FormClosed += form_FormClosed;
			return form;
		}
		void form_FormClosed(object sender, FormClosedEventArgs e) {
			if(collection != null) {
				List<DefinitionBase> list = collection.ConvertToTypedList();
				collection.Clear();
				foreach(RowDefinition column in list) {
					collection.Add(new RowDefinition(collection.Owner, column.Height, column.SizeType));
				}
				collection.Owner.EndUpdate();
				collection.Owner.EndUpdate();
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			collection = value as RowDefinitions;
			return base.EditValue(context, provider, value);
		}
	}
	public class ColumnDefinitionsEditor :CollectionEditor {
		public ColumnDefinitionsEditor(Type type) : base(type) { }
		void form_Shown(object sender, EventArgs e) {
			if(collection != null) collection.Owner.BeginUpdate();
		}
		protected override object SetItems(object editValue, object[] value) {
			if(editValue != null && editValue is IList) {
				ColumnDefinitions list = (ColumnDefinitions)editValue;
				list.Clear();
				list.BeginUpdate();
				for(int index = 0; index < value.Length; ++index) {
					ColumnDefinition rowDefinition = value[index] as ColumnDefinition;
					list.Add(rowDefinition);
				}
				list.EndUpdate();
			}
			return editValue;
		}
		protected override object CreateInstance(Type itemType) {
			ColumnDefinition column = new ColumnDefinition();
			column.SizeType = SizeType.AutoSize;
			return column;
		}
		protected override string GetDisplayText(object value) {
			return "Column Definition";
		}
		CollectionForm form;
		ColumnDefinitions collection;
		protected override CollectionEditor.CollectionForm CreateCollectionForm() {
			form = base.CreateCollectionForm();
			form.Shown += form_Shown;
			form.FormClosed += form_FormClosed;
			return form;
		}
		void form_FormClosed(object sender, FormClosedEventArgs e) {
			if(collection != null) {
				List<DefinitionBase> list = collection.ConvertToTypedList();
				collection.Clear();
				foreach(ColumnDefinition column in list) {
					collection.Add(new ColumnDefinition(collection.Owner, column.Width, column.SizeType));
				}
				collection.Owner.EndUpdate();
			}
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			collection = value as ColumnDefinitions;
			return base.EditValue(context, provider, value);
		}
	}
	public class TabbedGroupCollectionEditor :CollectionEditor {
		public TabbedGroupCollectionEditor(Type type) : base(type) { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			MessageBox.Show("Drag group groups into tab headers area to change this collection");
			return null;
		}
	}
	public class GroupCollectionEditor :CollectionEditor {
		public GroupCollectionEditor(Type type) : base(type) { }
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			MessageBox.Show("Use drag group to change this collection");
			return null;
		}
	}
	public class UserCustomizationFormDesigner :DocumentDesigner {
		public class UserCustomizationFormActionList :DesignerActionList {
			private UserCustomizationFormDesigner designer;
			public UserCustomizationFormActionList(UserCustomizationFormDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection items = new DesignerActionItemCollection();
				items.Add(new DesignerActionMethodItem(this, "InvokeAddHiddenItems", "Add Hidden Item List", "Controls", "", true));
				items.Add(new DesignerActionMethodItem(this, "InvokeAddLayoutTreeView", "AddLayoutTreeView", "Controls", "", true));
				items.Add(new DesignerActionMethodItem(this, "InvokeAddButtonsPanel", "Add Button Panel", "Controls", "", true));
				items.Add(new DesignerActionMethodItem(this, "InvokeAddPropertyGrid", "AddPropertyGrid", "Controls", "", true));
				return items;
			}
			public void InvokeAddHiddenItems() { AddTool(typeof(HiddenItemsList)); }
			public void InvokeAddLayoutTreeView() { AddTool(typeof(LayoutTreeView)); }
			public void InvokeAddButtonsPanel() { AddTool(typeof(ButtonsPanel)); }
			public void InvokeAddPropertyGrid() { AddTool(typeof(CustomizationPropertyGrid)); }
			protected void AddTool(Type type) {
				Control control = DevExpress.XtraDataLayout.LayoutCreator.GetIDesignerHost(designer.Component.Site.Container).CreateComponent(type) as Control;
				control.Parent = designer.Component as Control;
				designer.FakeRegisterForm();
			}
		}
		LayoutControl fakeLayoutControl;
		public UserCustomizationFormDesigner() { }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(DesignerHost != null) {
				DesignerHost.LoadComplete += new EventHandler(OnLoadComplete);
			}
		}
		IDesignerHost designerHost;
		public IDesignerHost DesignerHost {
			get {
				if(designerHost == null) designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
		private DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
				if(this.actionLists == null) {
					this.actionLists = new DesignerActionListCollection();
					this.actionLists.Add(new UserCustomizationFormActionList(this));
				}
				return this.actionLists;
			}
		}
		protected virtual void OnLoadComplete(object sender, EventArgs e) {
			FakeRegisterForm();
		}
		public void FakeRegisterForm() {
			if(fakeLayoutControl == null) fakeLayoutControl = new LayoutControl();
			if(Form != null) (Form as ICustomizationContainer).Register(fakeLayoutControl);
		}
		protected void FakeUnRegisterForm() {
			if(fakeLayoutControl == null) fakeLayoutControl = new LayoutControl();
			if(Form != null) (Form as ICustomizationContainer).UnRegister();
		}
		protected UserCustomizationForm Form { get { return Component as UserCustomizationForm; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(DesignerHost != null) {
					DesignerHost.LoadComplete -= new EventHandler(OnLoadComplete);
				}
				FakeUnRegisterForm();
				fakeLayoutControl.Dispose();
				fakeLayoutControl = null;
			}
			base.Dispose(disposing);
		}
	}
}

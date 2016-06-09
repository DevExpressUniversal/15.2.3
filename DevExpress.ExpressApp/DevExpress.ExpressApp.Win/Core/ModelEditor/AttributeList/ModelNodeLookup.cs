#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Nodes.Operations;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class ModelNodesLookup : ComboBoxEdit {
		public ModelNodesLookup()
			: base() {
		}
		static ModelNodesLookup() { RepositoryItemModelNodesLookup.RegisterModelNodeLookup(); }
		public override string EditorTypeName { get { return "ModelNodesLookup"; } }
		protected override string GetItemDescription(object item) {
			return base.GetItemDescription(item);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemModelNodesLookup Properties {
			get { return base.Properties as RepositoryItemModelNodesLookup; }
		}
	}
	public class RepositoryItemModelNodesLookup : RepositoryItemComboBox {
		static RepositoryItemModelNodesLookup() { RegisterModelNodeLookup(); }
		public RepositoryItemModelNodesLookup()
			: base() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		public override string EditorTypeName { get { return "ModelNodesLookup"; } }
		public static void RegisterModelNodeLookup() {
			Image img = null;
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("ModelNodesLookup", typeof(ModelNodesLookup), typeof(RepositoryItemModelNodesLookup), typeof(DevExpress.XtraEditors.ViewInfo.ComboBoxViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, img));
		}
		protected override DevExpress.XtraEditors.Controls.ComboBoxItemCollection CreateItemCollection() {
			return new ModelNodeLookupItemCollection(this);
		}
		public override string GetDisplayText(object editValue) {
			if (editValue is ModelNode) {
				return FastModelEditorHelper.GetModelNodeDisplayValue_Static(((ModelNode)editValue));
			}
			return base.GetDisplayText(editValue);
		}
		public override string GetDisplayText(DevExpress.Utils.FormatInfo format, object editValue) {
			if (editValue is ModelNode) {
				return FastModelEditorHelper.GetModelNodeDisplayValue_Static(((ModelNode)editValue));
			}
			return base.GetDisplayText(format, editValue);
		}
		public class ModelNodeLookupItemCollection : ComboBoxItemCollection {
			public ModelNodeLookupItemCollection(RepositoryItemComboBox properties)
				: base(properties) {
			}
			public override string GetItemDescription(object item) {
				if (item is ModelNode) {
					return FastModelEditorHelper.GetModelNodeDisplayValue_Static(((ModelNode)item));
				}
				return base.GetItemDescription(item);
			}
		}
	}
	public class FieldPickerPopupForm : PopupContainerForm {
		public FieldPickerPopupForm(PopupContainerEdit owner, ModelBrowser modelBrowser)
			: base(owner) {
			this.modelBrowser = modelBrowser;
		}
		public override object ResultValue {
			get {
				if(string.IsNullOrEmpty(modelBrowser.SelectedMember) || !ValueChanged) {
					return OldEditValue;
				}
				return modelBrowser.SelectedMember;
			}
		}
		public bool ValueChanged {
			get;
			set;
		}
		public void SelectObject(string editValue) {
			if (!string.IsNullOrEmpty(editValue)) {
				IMemberInfo memberInfo = ((ITypeInfo)modelBrowser.FieldsTreeView.DataSource).FindMember(editValue);
				if(memberInfo != null) {
					IList<IMemberInfo> path = memberInfo.GetPath();
					ObjectTreeListNode node = (ObjectTreeListNode)modelBrowser.FieldsTreeView.GetNodeByVisibleIndex(0);
					foreach(IMemberInfo info in path) {
						foreach(ObjectTreeListNode childNode in node.Nodes) {
							if(childNode.Object == info) {
								node = childNode;
								node.Expanded = true;
								break;
							}
						}
					}
					modelBrowser.FieldsTreeView.FocusedNode = node;
				}
			}
		}
		ModelBrowser modelBrowser;
		protected class FieldPickerTreeListOperation : TreeListOperation {
			IMemberInfo info;
			public FieldPickerTreeListOperation(IMemberInfo info) : base() {
				this.info = info;
			}
			public override void Execute(DevExpress.XtraTreeList.Nodes.TreeListNode node) {
				if(((ObjectTreeListNode)node).Object == info) {
				}
			}
		}
	}
	[ToolboxItem(false)]
	public class FieldPicker : PopupContainerEdit {
		PopupContainerControl popupControl;
		static FieldPicker() { RepositoryFieldPicker.RegisterModelNodeLookup(); }
		private void FieldsTreeView_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) {
			((FieldPickerPopupForm)PopupForm).ValueChanged = true;
			ClosePopup();
		}
		ModelBrowser modelBrowser;
		protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() {
			popupControl = new PopupContainerControl();
			modelBrowser = new ModelBrowser(Properties.ClassType);
			popupControl.Controls.Add(modelBrowser.FieldsTreeView);
			base.Properties.PopupControl = popupControl;
			DevExpress.XtraEditors.Popup.PopupBaseForm form = new FieldPickerPopupForm(this, modelBrowser);
			modelBrowser.FieldsTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(FieldsTreeView_MouseDoubleClick);
			return form;
		}
		protected override void DoShowPopup() {
			if(popupControl != null) {
				Properties.PopupControl = popupControl;
			}
			base.DoShowPopup();
			((FieldPickerPopupForm)PopupForm).SelectObject((string)EditValue);
		}
		protected override void DestroyPopupFormCore(bool dispose) {
			base.DestroyPopupFormCore(dispose);
		}
		public override string EditorTypeName {
			get {
				return "FieldPicker";
			}
		}
		public new RepositoryFieldPicker Properties {
			get { return (RepositoryFieldPicker)base.Properties; }
		}
	}
	public class RepositoryFieldPicker : RepositoryItemPopupContainerEdit {
		static RepositoryFieldPicker() { RegisterModelNodeLookup(); }
		public RepositoryFieldPicker()
			: base() {
			TextEditStyle = TextEditStyles.Standard; 
		}
		public static void RegisterModelNodeLookup() {
			Image img = null;
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("FieldPicker", typeof(FieldPicker), typeof(RepositoryFieldPicker), typeof(DevExpress.XtraEditors.ViewInfo.LookUpEditViewInfo), new DevExpress.XtraEditors.Drawing.ButtonEditPainter(), true, img));
		}
		public override string EditorTypeName {
			get {
				return "FieldPicker";
			}
		}
		public Type ClassType {
			get;
			set;
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			ClassType = ((RepositoryFieldPicker)item).ClassType;
		}
	}
}

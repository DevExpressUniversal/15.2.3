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
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.ExpressApp.Core;
using DevExpress.Data.ExpressionEditor;
using System.ComponentModel.Design;
namespace DevExpress.ExpressApp.Win.Editors {
	[PropertyEditor(typeof(string), false)]
	public class PopupExpressionPropertyEditor : DXPropertyEditor {
		protected override object CreateControlCore() {
			PopupExpressionEdit edit = new PopupExpressionEdit();
			return edit;
		}
		protected override RepositoryItem CreateRepositoryItem() {
			return new RepositoryItemPopupExpressionEdit();
		}
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			RepositoryItemPopupExpressionEdit expressionEdit = item as RepositoryItemPopupExpressionEdit;
			expressionEdit.EditingObject = CurrentObject;
			expressionEdit.DataColumnInfo = new XafDataColumnInfo(ObjectTypeInfo, MemberInfo);
			expressionEdit.DataColumnInfo.Update(CurrentObject);
		}
		protected override void ReadValueCore() {
			base.ReadValueCore();
			PopupExpressionEdit editor = Control as PopupExpressionEdit;
			if(editor != null) {
				editor.Properties.EditingObject = CurrentObject;
				editor.Properties.DataColumnInfo.Update(CurrentObject);
			}
		}
		public PopupExpressionPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			CanUpdateControlEnabled = true;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class PopupExpressionEdit : ButtonEdit, IGridInplaceEdit {
		public PopupExpressionEdit() : base() { }
		protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.OnClickButton(buttonInfo);
			Properties.DataColumnInfo.Update(GridEditingObject);
			Properties.DataColumnInfo.UnboundExpression = (string)EditValue;
			using(UnboundColumnExpressionEditorForm expressionEditorForm = new XafExpressionEditorForm(Properties.DataColumnInfo, null)) {
				if(expressionEditorForm.ShowDialog() == DialogResult.OK) {
					EditValue = expressionEditorForm.Expression;
				}
			}
		}
		static PopupExpressionEdit() { RepositoryItemPopupExpressionEdit.Register(); }
		public override string EditorTypeName {
			get {
				return RepositoryItemPopupExpressionEdit.EditorName;
			}
		}
		public new RepositoryItemPopupExpressionEdit Properties {
			get { return (RepositoryItemPopupExpressionEdit)base.Properties; }
		}
		#region IGridInplaceEdit Members
		public object GridEditingObject {
			get { return Properties.EditingObject; }
			set { Properties.EditingObject = value; }
		}
		#endregion
	}
	public class RepositoryItemPopupExpressionEdit : RepositoryItemButtonEdit {
		static RepositoryItemPopupExpressionEdit() {
			RepositoryItemPopupExpressionEdit.Register();
		}
		internal XafDataColumnInfo DataColumnInfo { get; set; }
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(PopupExpressionEdit),
					typeof(RepositoryItemPopupExpressionEdit), typeof(ButtonEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
			}
		}
		protected internal static string EditorName {
			get { return typeof(PopupExpressionEdit).Name; }
		}
		public RepositoryItemPopupExpressionEdit()
			: base() {
			TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemPopupExpressionEdit sourceItem = (RepositoryItemPopupExpressionEdit)item;
			DataColumnInfo = sourceItem.DataColumnInfo;
		}
		public override string EditorTypeName {
			get { return EditorName; }
		}
		public object EditingObject { get; set; }
	}
	internal class XafDataColumnInfo : IDataColumnInfo {
		private IMemberInfo memberInfo;
		private ITypeInfo ownerTypeInfo;
		public XafDataColumnInfo(ITypeInfo ownerTypeInfo, IMemberInfo memberInfo) {
			this.ownerTypeInfo = ownerTypeInfo;
			this.memberInfo = memberInfo;
		}
		public string Caption {
			get { return CaptionHelper.GetMemberCaption(ownerTypeInfo, memberInfo.Name); }
		}
		public List<IDataColumnInfo> Columns {
			get {
				List<IDataColumnInfo> result = new List<IDataColumnInfo>();
				foreach(IMemberInfo member in ownerTypeInfo.Members) {
					if(member.IsVisible && member.IsPublic) {
						result.Add(new XafDataColumnInfo(member.Owner, member));
					}
				}
				return result;
			}
		}
		public DataControllerBase Controller {
			get { return null; }
		}
		public string FieldName {
			get { return memberInfo.BindingName; }
		}
		public Type FieldType {
			get { return memberInfo.MemberType; }
		}
		public string Name {
			get { return memberInfo.Name; }
		}
		public string UnboundExpression { get; set; }
		public void Update(object currentObject) {
			ElementTypePropertyAttribute typeMemberAttribute = memberInfo.FindAttribute<ElementTypePropertyAttribute>(true);
			ITypeInfo targetTypeInfo = ownerTypeInfo;
			if(currentObject != null) {
				targetTypeInfo = XafTypesInfo.Instance.FindTypeInfo(currentObject.GetType());
			}
			if(targetTypeInfo != null && typeMemberAttribute != null) {
				IMemberInfo objectTypeMember = targetTypeInfo.FindMember(typeMemberAttribute.Name);
				if(objectTypeMember != null) {
					ownerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(objectTypeMember.GetValue(currentObject) as Type);
				}
			}
		}
	}
	internal class XafExpressionEditorLogic : ExpressionEditorLogicEx {
		string additionalFunctionsTitle = "Aggregate operations";
		protected override object[] GetListOfInputTypesObjects() {
			List<object> result = new List<object>(base.GetListOfInputTypesObjects());
			result.Add(additionalFunctionsTitle);
			return result.ToArray();
		}
		protected override ItemClickHelper GetItemClickHelper(string selectedItemText, IExpressionEditor editor) {
			if(selectedItemText == additionalFunctionsTitle) {
				return new AggregatesClickHelper(editor);
			}
			return base.GetItemClickHelper(selectedItemText, editor);
		}
		public XafExpressionEditorLogic(IExpressionEditor editor, IDataColumnInfo columnInfo)
			: base(editor, columnInfo) {
			additionalFunctionsTitle = CaptionHelper.GetLocalizedText("Texts", "Aggregates");
		}
	}
	internal class AggregatesClickHelper : ItemClickHelper {
		protected override void FillItemsTable() {
			this.AddItemTable("Avg()", CaptionHelper.GetLocalizedText("Texts", "AggregatesAvgDescription"), -1);
			this.AddItemTable("Count", CaptionHelper.GetLocalizedText("Texts", "AggregatesCountDescription"), -1);
			this.AddItemTable("Max()", CaptionHelper.GetLocalizedText("Texts", "AggregatesMaxDescription"), -1);
			this.AddItemTable("Min()", CaptionHelper.GetLocalizedText("Texts", "AggregatesMinDescription"), -1);
			this.AddItemTable("Sum()", CaptionHelper.GetLocalizedText("Texts", "AggregatesSumDescription"), -1);
		}
		public AggregatesClickHelper(IExpressionEditor editor)
			: base(editor) { }
		public override int GetCursorOffset(string item) {
			return 1;
		}
	}
	public class XafExpressionEditorForm : UnboundColumnExpressionEditorForm {
		protected override DevExpress.Data.ExpressionEditor.ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new XafExpressionEditorLogic(this, (IDataColumnInfo)ContextInstance);
		}
		public XafExpressionEditorForm(object contextInstance,
			IDesignerHost designerHost)
			: base(contextInstance, designerHost) { }
	}
}

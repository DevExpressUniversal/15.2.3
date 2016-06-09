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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraPivotGrid.Data {
	public abstract class FieldFilterColumnBase : FilterColumn {
		readonly PivotGridFieldBase field;
		readonly IDXMenuManager menuManager;
		RepositoryItemPopupBase repositoryItem;
		protected PivotGridFieldBase Field { get { return field; } }
		protected IDXMenuManager MenuManager { get { return menuManager; } }
		protected RepositoryItemPopupBase RepositoryItem {
			get {
				return ConfigureRepositoryItem();
			}
		}
		public override FilterColumnClauseClass ClauseClass {
			get {
				if(ColumnType == typeof(DateTime))
					return FilterColumnClauseClass.DateTime;
				return ColumnType == typeof(string) ? FilterColumnClauseClass.String : FilterColumnClauseClass.Generic;
			}
		}
		public override string ColumnCaption { get { return Field.ToString(); } }
		public override Type ColumnType { get { return Field.ActualDataType; } }
		public override Image Image { get { return null; } }
		public override RepositoryItem ColumnEditor { get { return RepositoryItem; } }
		protected FieldFilterColumnBase(PivotGridFieldBase field, IDXMenuManager menuManager) {
			this.field = field;
			this.menuManager = menuManager;
		}
		public override void Dispose() {
			if(repositoryItem != null)
				repositoryItem.Dispose();
			base.Dispose();
		}
		RepositoryItemPopupBase ConfigureRepositoryItem() {
			if(repositoryItem == null)
				repositoryItem = CreateRepositoryItem(MenuManager);
			return repositoryItem;
		}
		protected RepositoryItemPopupBase CreateRepositoryItem(IDXMenuManager menuManager) {
			if(field.ActualDataType == typeof(DateTime)) {
				PivotFieldValueRepositoryItemDateEdit repositoryItemDateEdit = new PivotFieldValueRepositoryItemDateEdit();
				repositoryItemDateEdit.Field = field;
				repositoryItemDateEdit.MenuManager = menuManager;
				repositoryItemDateEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
				return repositoryItemDateEdit;
			} else {
				PivotFieldValueRepositoryItemComboBox repositoryItemComboBox = new PivotFieldValueRepositoryItemComboBox();
				repositoryItemComboBox.Field = field;
				repositoryItemComboBox.MenuManager = menuManager;
				repositoryItemComboBox.Items.AddRange(CreateItems());
				repositoryItemComboBox.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
				return repositoryItemComboBox;
			}
		}
		FilterItem[] CreateItems() {
			object[] values = field.GetUniqueValues();
			FilterItem[] result = new FilterItem[values.Length];
			for(int i = 0; i < values.Length; i++)
				result[i] = new FilterItem(values[i], field.GetDisplayText(values[i]));
			return result;
		}
	}
	public class FieldFilterColumn : FieldFilterColumnBase {
		public FieldFilterColumn(PivotGridFieldBase field, IDXMenuManager menuManager) : base(field, menuManager) {
		}
		public override string FieldName { get { return Field.PrefilterColumnName; } }
	}
	public class FieldFilterColumnCollection : FilterColumnCollection {
		PivotGridFieldCollectionBase fields;
		public FieldFilterColumnCollection(PivotGridFieldCollectionBase fields, IList<string> prefilterColumnNames, IDXMenuManager menuManager) {
			this.fields = fields;
			IList visibleFields = fields.GetPrefilterFields(prefilterColumnNames);
			foreach(PivotGridFieldBase field in visibleFields) {
				this.Add(new FieldFilterColumn(field, menuManager));
			}
		}
		public override string GetDisplayPropertyName(DevExpress.Data.Filtering.OperandProperty property, string fullPath) {
			FilterColumn column = this[fullPath] ?? this[property];
			if(column != null || fields.IsPrefilterHiddenField(property.PropertyName)) {
				return base.GetDisplayPropertyName(property, fullPath);
			} else {
				return PivotGridFieldBase.InvalidPropertyDisplayText;
			}
		}
	}
	internal class PivotFieldValueRepositoryItemComboBox : RepositoryItemComboBox {
		const string EditorName = "PivotFieldValueComboBoxEdit";
		static PivotFieldValueRepositoryItemComboBox() { RegisterEditor(); }
		static void RegisterEditor() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(PivotFieldValueComboBoxEdit), typeof(PivotFieldValueRepositoryItemComboBox), typeof(ComboBoxViewInfo), new ButtonEditPainter(), true, EditImageIndexes.ComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
		}
		PivotGridFieldBase field;
		IDXMenuManager menuManager;
		internal IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		internal PivotGridFieldBase Field { get { return field; } set { field = value; } }
		public PivotFieldValueRepositoryItemComboBox()
			: base() {
		}
		public override string EditorTypeName { get { return PivotFieldValueComboBoxEdit.EditorName; } }
		public override BaseEditPainter CreatePainter() {
			return new ButtonEditPainter();
		}
		public override BaseEdit CreateEditor() {
			BaseEdit editor = new PivotFieldValueComboBoxEdit(field);
			editor.MenuManager = MenuManager;
			return editor;
		}
		internal void SetOwnerEditInternal(BaseEdit edit) {
			SetOwnerEdit(edit);
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new ComboBoxViewInfo(this);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			FilterItem item = editValue as FilterItem;
			if(item != null)
				return item.DisplayText;
			if(field != null)
				return field.GetDisplayText(editValue);
			return base.GetDisplayText(format, editValue);
		}
	}
	internal class PivotFieldValueComboBoxEdit : ComboBoxEdit {
		readonly PivotGridFieldBase field;
		public PivotFieldValueComboBoxEdit(PivotGridFieldBase field)
			: base() {
			this.field = field;
			((PivotFieldValueRepositoryItemComboBox)fProperties).Field = field;
		}
		internal const string EditorName = "PivotFieldValueComboBoxEdit";
		public new PivotFieldValueRepositoryItemComboBox Properties {
			get { return (PivotFieldValueRepositoryItemComboBox)base.Properties; }
		}
		public override string EditorTypeName { get { return EditorName; } }
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				FilterItem filterItem = value as FilterItem;
				base.EditValue = filterItem != null ? filterItem.Value : value;
			}
		}
		protected override void CreateRepositoryItem() {
			var repositoryItem = new PivotFieldValueRepositoryItemComboBox();
			fProperties = repositoryItem;
			repositoryItem.SetOwnerEditInternal(this);
		}
	}
	internal class PivotFieldValueRepositoryItemDateEdit : RepositoryItemDateEdit {
		const string EditorName = "PivotFieldValueDateEdit";
		static PivotFieldValueRepositoryItemDateEdit() { RegisterEditor(); }
		static void RegisterEditor() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(PivotFieldValueDateEdit), typeof(PivotFieldValueRepositoryItemDateEdit), typeof(DateEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.DateEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
		}
		PivotGridFieldBase field;
		IDXMenuManager menuManager;
		internal IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		internal PivotGridFieldBase Field { get { return field; } set { field = value; } }
		public override string EditorTypeName { get { return PivotFieldValueDateEdit.EditorName; } }
		public override BaseEditPainter CreatePainter() {
			return new ButtonEditPainter();
		}
		public override BaseEdit CreateEditor() {
			BaseEdit editor = new PivotFieldValueDateEdit(field);
			editor.MenuManager = MenuManager;
			return editor;
		}
		internal void SetOwnerEditInternal(BaseEdit edit) {
			SetOwnerEdit(edit);
		}
		public override BaseEditViewInfo CreateViewInfo() {
			return new DateEditViewInfo(this);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			return field != null ? field.GetDisplayText(editValue) : base.GetDisplayText(format, editValue);
		}
	}
	internal class PivotFieldValueDateEdit : DateEdit {
		readonly PivotGridFieldBase field;
		public PivotFieldValueDateEdit() { }
		public PivotFieldValueDateEdit(PivotGridFieldBase field)
			: base() {
			this.field = field;
			((PivotFieldValueRepositoryItemDateEdit)fProperties).Field = field;
		}
		internal const string EditorName = "PivotFieldValueDateEdit";
		public new PivotFieldValueRepositoryItemDateEdit Properties {
			get { return (PivotFieldValueRepositoryItemDateEdit)base.Properties; }
		}
		public override string EditorTypeName { get { return EditorName; } }
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				FilterItem filterItem = value as FilterItem;
				base.EditValue = filterItem != null ? filterItem.Value : value;
			}
		}
		protected override void CreateRepositoryItem() {
			var repositoryItem = new PivotFieldValueRepositoryItemDateEdit();
			fProperties = repositoryItem;
			repositoryItem.SetOwnerEditInternal(this);
		}
	}
}

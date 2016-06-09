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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public abstract class DataColumnCreatorBase : ColumnCreatorBase<IModelColumn, GridViewDataColumn> {
		private string displayFormat;
		protected DataColumnCreatorBase(IModelColumn columnInfo)
			: base(columnInfo) {
		}
		protected override GridViewDataColumn CreateColumn() {
			UpdateFormatting();
			GridViewDataColumn result = base.CreateColumn();
			SetupDataColumn((GridViewDataColumn)result);
			return result;
		}
		protected internal void SetupDataColumn(GridViewDataColumn column) {
			column.ReadOnly = true;
			if(column.PropertiesEdit != null) { 
				column.PropertiesEdit.DisplayFormatString = DisplayFormat;
			}
			string toolTip = Model.ToolTip;
			if(!string.IsNullOrEmpty(toolTip)) {
				column.ToolTip = toolTip;
			}
			IModelMember modelMember = Model.ModelMember;
			if(modelMember != null && modelMember.Type == typeof(Type)) { 
				column.Settings.AllowSort = DevExpress.Utils.DefaultBoolean.True;
				column.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
				column.Settings.AllowGroup = DevExpress.Utils.DefaultBoolean.True;
				column.Settings.FilterMode = ColumnFilterMode.DisplayText;
			}
			SetFieldName(column);
		}
		protected virtual void SetFieldName(GridViewDataColumn column) {
			column.FieldName = Model.FieldName;
		}
		protected void UpdateFormatting() {
			if(CanFormatPropertyValue) {
				if(displayFormat == null) {
					displayFormat = Model.DisplayFormat;
				}
			}
		}
		protected virtual bool IsProtectedContentColumn {
			get {
				return false;
			}
		}
		public string DisplayFormat {
			get { return displayFormat; }
		}
		public virtual bool CanFormatPropertyValue {
			get { return false; }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IColumnCreator {
		GridViewColumn CreateGridViewColumn();
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ColumnCreatorBase<ModelType, ColumnBaseType> : IColumnCreator
		where ModelType : IModelNode
		where ColumnBaseType : GridViewColumn {
		private ModelType model;
		protected ColumnCreatorBase(ModelType columnInfo) {
			Guard.ArgumentNotNull(columnInfo, "columnInfo");
			model = columnInfo;
		}
		public ColumnBaseType CreateGridViewColumn() {
			return CreateColumn();
		}
		protected virtual ColumnBaseType CreateColumn() {
			return CreateGridViewColumnCore();
		}
		public ModelType Model {
			get { return model; }
		}
		protected abstract ColumnBaseType CreateGridViewColumnCore();
		#region IColumnCreator Members
		GridViewColumn IColumnCreator.CreateGridViewColumn() {
			return CreateGridViewColumn();
		}
		#endregion
	}
}

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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public interface IGridViewDataColumnInfo : IDataColumnInfo {
		bool AllowSummaryChange { get; set; }
		bool IsProtectedContentColumn { get; }
		CollectionSourceDataAccessMode DataAccessMode { get; }
	}
	[Obsolete("This interface is not used anymore. Use 'IColumnInfo' or IDataColumnInfo (for GridViewDataColumn only) instead.", true)]
	public interface IXafColumnInfo : IXafColumn {
		void ApplyModel(IModelColumn columnInfo, WebColumnBase column);
		void SynchronizeModel(WebColumnBase column);
		ColumnWrapper CreateColumnWrapper(WebColumnBase column);
	}
	public interface IColumnInfo {
		IModelLayoutElement Model { get; }
		void ApplyModel(WebColumnBase column);
		void SynchronizeModel(WebColumnBase column);
		WebColumnBaseColumnWrapper CreateColumnWrapper(WebColumnBase column);
	}
	public interface IDataColumnInfo : IColumnInfo {
		new IModelColumn Model { get; }
		IMemberInfo MemberInfo { get; }
	}
	public class GridViewDataColumnInfo : IGridViewDataColumnInfo {
		private IMemberInfo memberInfo;
		private IModelColumn model;
		private bool allowSummaryChange;
		private bool isInitMemberInfo = false;
		private bool isProtectedContentColumn;
		private CollectionSourceDataAccessMode dataAccessMode;
		private ASPxGridView gridView;
		public GridViewDataColumnInfo(IModelColumn modelColumn, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode)
			: this(modelColumn, gridView, dataAccessMode, false) {
		}
		internal GridViewDataColumnInfo(IModelColumn modelColumn, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode, bool isProtectedColumn) {
			this.model = modelColumn;
			this.isProtectedContentColumn = isProtectedColumn;
			this.dataAccessMode = dataAccessMode;
			this.gridView = gridView;
		}
		public CollectionSourceDataAccessMode DataAccessMode {
			get { return dataAccessMode; }
		}
		public bool IsProtectedContentColumn {
			get { return isProtectedContentColumn; }
		}
		public virtual WebColumnBaseColumnWrapper CreateColumnWrapper(WebColumnBase column) {
			return new ASPxGridViewColumnWrapper(gridView, (GridViewDataColumn)column, this);
		}
		public void SetupColumn(GridViewDataColumn column) {
			if(Model.ModelMember != null) {
				if(column.UnboundType == Data.UnboundColumnType.Bound) {
					column.FieldName = new ObjectEditorHelperBase(XafTypesInfo.Instance.FindTypeInfo(Model.ModelMember.Type), Model).GetFullDisplayMemberName(Model.PropertyName);
				}
				memberInfo = ((IModelListView)Model.ParentView).ModelClass.TypeInfo.FindMember(column.FieldName);
				if(memberInfo != null) {
					column.CellStyle.HorizontalAlign = WebAlignmentProvider.GetAlignment(memberInfo.MemberType);
				}
			}
			else {
				if(column.UnboundType == Data.UnboundColumnType.Bound) {
					column.FieldName = Model.PropertyName;
				}
			}
		}
		public IMemberInfo MemberInfo {
			get {
				if(memberInfo == null && !isInitMemberInfo) {
					memberInfo = CalcMemberInfo();
					isInitMemberInfo = true;
				}
				return memberInfo;
			}
		}
		public IModelColumn Model {
			get { return model; }
		}
		public bool AllowSummaryChange {
			get { return allowSummaryChange; }
			set { allowSummaryChange = value; }
		}
		protected ASPxGridView GridView {
			get { return gridView; }
		}
		protected virtual void GridViewDataColumnApplyModel(GridViewDataColumn column) {
			CreateModelSynchronizer(column).ApplyModel();
		}
		protected virtual void GridViewDataColumnSynchronizeModel(GridViewDataColumn column) {
			CreateModelSynchronizer(column).SynchronizeModel();
		}
		protected virtual ModelSynchronizer CreateModelSynchronizer(WebColumnBase column) {
			return new ASPxGridViewColumnModelSynchronizer(CreateColumnWrapper(column), model, DataAccessMode, IsProtectedContentColumn);
		}
		private IMemberInfo CalcMemberInfo() {
			IModelListView listViewInfo = (IModelListView)model.ParentView;
			return listViewInfo.ModelClass.TypeInfo.FindMember(model.FieldName);
		}
		IModelLayoutElement IColumnInfo.Model {
			get {
				return model;
			}
		}
		void IColumnInfo.ApplyModel(WebColumnBase column) {
			memberInfo = null;
			isInitMemberInfo = false;
			if(column is GridViewDataColumn) {
				GridViewDataColumnApplyModel((GridViewDataColumn)column);
			}
			if(Model is IModelColumnWeb) {
				int currentAdaptivePriority = (Model as IModelColumnWeb).AdaptivePriority;
				(column as GridViewColumn).AdaptivePriority = currentAdaptivePriority;
			}
		}
		void IColumnInfo.SynchronizeModel(WebColumnBase column) {
			if(column is GridViewDataColumn) {
				GridViewDataColumnSynchronizeModel((GridViewDataColumn)column);
			}
		}
	}
}

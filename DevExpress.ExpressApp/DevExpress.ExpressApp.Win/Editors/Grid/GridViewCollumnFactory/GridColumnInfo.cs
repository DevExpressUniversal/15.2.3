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
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IGridColumnModelSynchronizer : IModelSynchronizer {
		new IModelColumn Model { get; }
		IMemberInfo MemberInfo { get; }
		IModelMember ModelMember { get; }
		ITypeInfo ObjectTypeInfo { get; }
		string PropertyName { get; }
		bool AllowSummaryChange { get; set; }
		CollectionSourceDataAccessMode DataAccessMode { get; }
		bool IsProtectedContentColumn { get; }
		bool IsReplacedColumnByAsyncServerMode { get; }
		string FieldName { get; }
		XafGridColumnWrapper CreateColumnWrapper(GridColumn column);
	}
	public interface IModelSynchronizer {
		IModelLayoutElement Model { get; }
		void ApplyModel(Component component);
		void SynchronizeModel(Component component);
	}
	public class GridColumnModelSynchronizer : IGridColumnModelSynchronizer {
		private IModelColumn model;
		private ITypeInfo objectTypeInfo;
		private bool isProtectedColumn;
		private bool allowSummaryChange = true;
		private bool isAsyncServerMode;
		private IMemberInfo memberInfo = null;
		private bool memberInfoIsCalculated = false;
		private bool modelMemberIsCalculated = false;
		private IModelMember modelMember = null;
		private bool? isReplacedColumnByAsyncServerMode = null;
		public GridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo)
			: this(modelColumn, objectTypeInfo, false, false) {
		}
		public GridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode)
			: this(modelColumn, objectTypeInfo, isAsyncServerMode, false) {
		}
		public GridColumnModelSynchronizer(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode, bool isProtectedColumn) {
			Guard.ArgumentNotNull(modelColumn, "modelColumn");
			this.model = modelColumn;
			this.objectTypeInfo = objectTypeInfo;
			this.isProtectedColumn = isProtectedColumn;
			this.isAsyncServerMode = isAsyncServerMode;
		}
		private string DefaultFieldName {
			get {
				if(MemberInfo != null) {
					return MemberInfo.BindingName;
				}
				else {
					return PropertyName;
				}
			}
		}
		public string FieldName {
			get {
				if(Model.ModelMember != null) {
					MediaDataObjectAttribute mediaDataObjectAttribute = Model.ModelMember.MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>(false);
					if(mediaDataObjectAttribute != null) {
						string mediaBindingPropertyName = DataAccessMode == CollectionSourceDataAccessMode.DataView ? mediaDataObjectAttribute.MediaDataDataViewBindingProperty : mediaDataObjectAttribute.MediaDataBindingProperty;
						return Model.ModelMember.MemberInfo.Owner.FindMember(MemberInfo.Name + "." + mediaBindingPropertyName).BindingName;
					}
					else {
						if(DataAccessMode == CollectionSourceDataAccessMode.DataView) {
							return Model.FieldName;
						}
						else {
							return DefaultFieldName;
						}
					}
				}
				else {
					return DefaultFieldName;
				}
			}
		}
		public IModelColumn Model {
			get { return model; }
		}
		public ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
		}
		public IMemberInfo MemberInfo {
			get {
				if(!memberInfoIsCalculated) {
					memberInfoIsCalculated = true;
					memberInfo = FindMemberInfoForColumn();
				}
				return memberInfo;
			}
		}
		public IModelMember ModelMember {
			get {
				if(!modelMemberIsCalculated && model != null) {
					modelMemberIsCalculated = true;
					if(objectTypeInfo != null && MemberInfo != null) {
						modelMember = model.ModelMember.ModelClass.FindMember(MemberInfo.Name);
					}
					if(modelMember == null) {
						modelMember = model.ModelMember;
					}
				}
				return modelMember;
			}
		}
		public string PropertyName {
			get {
				if(model != null)
					return model.PropertyName;
				return string.Empty;
			}
		}
		public bool AllowSummaryChange {
			get {
				return allowSummaryChange;
			}
			set {
				allowSummaryChange = value;
			}
		}
		public bool IsProtectedContentColumn {
			get {
				return isProtectedColumn;
			}
		}
		public CollectionSourceDataAccessMode DataAccessMode {
			get { return ((IModelListView)model.ParentView).DataAccessMode; }
		}
		public bool IsReplacedColumnByAsyncServerMode {
			get {
				if(!isReplacedColumnByAsyncServerMode.HasValue) {
					IMemberInfo memberInfo = objectTypeInfo.FindMember(model.PropertyName);
					isReplacedColumnByAsyncServerMode = isAsyncServerMode
						&& (memberInfo != null)
						&& (memberInfo.MemberType != typeof(Type))
						&& (model.FieldName != memberInfo.BindingName)
						&& SimpleTypes.IsClass(memberInfo.MemberType);
				}
				return isReplacedColumnByAsyncServerMode.Value;
			}
		}
		public void ApplyModel(Component column) {
			ResetModelDependentMembers();
			if(column is GridColumn) {
				GridColumnApplyModel((GridColumn)column);
			}
		}
		public void SynchronizeModel(Component column) {
			if(column is GridColumn) {
				GridColumnSynchronizeModel((GridColumn)column);
			}
		}
		public virtual XafGridColumnWrapper CreateColumnWrapper(GridColumn column) {
			return new XafGridColumnWrapper(column, this);
		}
		protected virtual void GridColumnApplyModel(GridColumn column) {
			CreateModelSynchronizer(column).ApplyModel();
		}
		protected virtual void GridColumnSynchronizeModel(GridColumn column) {
			CreateModelSynchronizer(column).SynchronizeModel();
		}
		protected virtual ModelSynchronizer CreateModelSynchronizer(GridColumn column) {
			return new ColumnWrapperModelSynchronizer(CreateColumnWrapper(column), Model, DataAccessMode, IsProtectedContentColumn);
		}
		private void ResetModelDependentMembers() {
			isReplacedColumnByAsyncServerMode = null;
			memberInfoIsCalculated = false;
			modelMemberIsCalculated = false;
			memberInfo = null;
			modelMember = null;
		}
		private IMemberInfo FindMemberInfoForColumn() {
			if(objectTypeInfo != null && model != null) {
				if(IsReplacedColumnByAsyncServerMode) {
					return objectTypeInfo.FindMember(model.FieldName);
				}
				return objectTypeInfo.FindMember(model.PropertyName);
			}
			else {
				return null;
			}
		}
		IModelLayoutElement IModelSynchronizer.Model {
			get {
				return Model;
			}
		}
	}
}

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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Forms {
	public abstract class SortingFormControllerBase : FormController {
		readonly ISnapControl control;
		IFieldPathService fieldPathService;
		SnapListFieldInfo listFieldInfo;
		FieldPathInfo fieldPathInfo;
		protected SortingFormControllerBase(SortingFormControllerBaseParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			SortList = new BindingList<SortField>();
			listFieldInfo = GetListFieldInfo();
			if (listFieldInfo == null)
				return;
			fieldPathInfo = GetFieldPathInfo();
			if (fieldPathInfo == null)
				return;
			ListDataMember = GetListDataMember();
			FillSortList();
		}
		protected abstract FieldPathInfo GetFieldPathInfo();
		protected abstract SnapListFieldInfo GetListFieldInfo();
		#region Properties
		public ISnapControl Control { get { return control; } }
		protected IFieldPathService FieldPathService {
			get {
				if (fieldPathService == null) {
					IFieldDataAccessService service = DocumentModel.GetService<IFieldDataAccessService>();
					if (service != null)
						fieldPathService = service.FieldPathService;
				}
				return fieldPathService;
			}
		}
		protected SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)Control.InnerControl.DocumentModel; } }
		protected SnapPieceTable PieceTable { get { return DocumentModel.ActivePieceTable; } }
		protected SnapListFieldInfo ListFieldInfo { get { return listFieldInfo; } }
		public string ListName { get { return ListFieldInfo.ParsedInfo.Name; } }
		public string ListDataMember { get; set; }
		protected FieldPathInfo FieldPathInfo { get { return fieldPathInfo; } }
		protected FieldPathDataMemberInfo FieldPath { get { return fieldPathInfo.DataMemberInfo; } }
		public BindingList<SortField> SortList { get; set; }
		protected virtual bool AllowEmptySortList { get { return false; } }
		#endregion
		protected void FillSortList() {
			GroupProperties groupProperties;
			GroupFieldInfo groupFieldInfo;
			string displayName;
			bool shouldAddDefaultSorting = !AllowEmptySortList;
			foreach(FieldDataMemberInfoItem item in FieldPath.Items) {
				if(item.Groups != null) {
					for(int g = 0; g < item.Groups.Count; g++) {
						groupProperties = item.Groups[g];
						shouldAddDefaultSorting &= groupProperties.HasGroupTemplates;
						for(int f = 0; f < groupProperties.GroupFieldInfos.Count; f++) {
							groupFieldInfo = groupProperties.GroupFieldInfos[f];
							displayName = GetDisplayName(groupFieldInfo.FieldName);
							SortList.Add(new SortField(displayName, groupFieldInfo.SortOrder, groupFieldInfo.FieldName, groupProperties.HasGroupTemplates, g, f));
						}
					}
				}
			}
			if(!shouldAddDefaultSorting)
				return;
			if(!CreateSortingBySelectedField())
				SortList.Add(new SortField());
		}
		bool CreateSortingBySelectedField() {
			SnapFieldInfo selectedfieldInfo = new ListFieldSelectionController(DocumentModel).FindDataField();
			IDataAccessService service = DocumentModel.GetService<IDataAccessService>();
			if (service == null)
				return false;
			SNDataInfo dataInfo = GetDataInfo(selectedfieldInfo);
			if (dataInfo == null)
				return false;
			DataMemberInfo dataMemberInfo = DataMemberInfo.Create(dataInfo.DataPaths);
			if (service.AllowGroupAndSort(dataInfo.Source, dataMemberInfo.ParentDataMemberInfo.DataMember, dataMemberInfo.ColumnName)) {
				string defaultFieldName = PieceTable.CreateFieldInstructionController(selectedfieldInfo.Field).GetArgumentAsString(0);
				string displayName = GetDisplayName(defaultFieldName);
				SortList.Add(new SortField(displayName, ColumnSortOrder.Ascending, defaultFieldName, false, 0, 0));
				return true;
			}
			return false;
		}
		protected InstructionController CreateInstructionControllerByFieldName(string fieldName) {
			Field field = PieceTable.FindFieldNearestToSelection(fieldName, false);
			return PieceTable.CreateFieldInstructionController(field);
		}
		public void RemoveEmptySortListElements() {
			var emptyItems = SortList.Where((p) => String.IsNullOrEmpty(p.DisplayName)).ToList();
			foreach (var item in emptyItems)
				SortList.Remove(item);
		}
		public void SwapSortListElements(int index1, int index2) {
			Algorithms.SwapElements(SortList, index1, index2);
		}
		public string GetFieldName(string dataMember) {
			return !string.IsNullOrEmpty(ListDataMember) ? dataMember.Substring(ListDataMember.Length + 1) : dataMember;
		}
		public string GetDisplayName(string fieldName) {
			return FieldsHelper.GetFieldDisplayName(DocumentModel.DataSourceDispatcher, listFieldInfo, fieldName, fieldName);
		}
		string GetListDataMember() {
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DocumentModel.DataSourceDispatcher, listFieldInfo);
			if (binding == null || (binding.DataSource == null && String.IsNullOrEmpty(binding.DataMember)))
				return null;
			return binding.DataMember;
		}
		protected SNDataInfo GetDataInfo(SnapFieldInfo fieldInfo) {
			if(fieldInfo == null)
				return null;
			IDataSourceDispatcher dataSourceDispatcher = DocumentModel.DataSourceDispatcher;
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(dataSourceDispatcher, fieldInfo);
			if(binding == null || (binding.DataSource == null && String.IsNullOrEmpty(binding.DataMember)))
				return null;
			string fieldName = binding.DataMember;
			Field parent = fieldInfo.Field.Parent;
			string dataMember = string.Empty;
			if(parent != null) {
				SnapFieldInfo parentFieldInfo = new SnapFieldInfo(PieceTable, parent);
				DesignBinding parentDesignBinding = FieldsHelper.GetFieldDesignBinding(dataSourceDispatcher, parentFieldInfo);
				dataMember = parentDesignBinding.DataMember;
				if(String.CompareOrdinal(dataMember, binding.DataMember) == 0)
					fieldName = String.Empty;
				else
					fieldName = !string.IsNullOrEmpty(dataMember) ? binding.DataMember.Substring(dataMember.Length + 1) : binding.DataMember;
			}
			string displayName = FieldsHelper.GetFieldDisplayName(dataSourceDispatcher, listFieldInfo, fieldName, fieldName);
			return new SNDataInfo(binding.DataSource, new[] { dataMember, fieldName }, displayName);
		}
	}
}

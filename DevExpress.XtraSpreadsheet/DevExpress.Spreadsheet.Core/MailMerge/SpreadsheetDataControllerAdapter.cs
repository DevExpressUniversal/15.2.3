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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using DevExpress.Data.Browsing;
#endif
#if !DXPORTABLE
using DevExpress.DataAccess.EntityFramework;
#endif
namespace DevExpress.XtraSpreadsheet {
	public class SpreadsheetDataControllerAdapter : OfficeDataControllerAdapterBase, IDataControllerCurrentSupport, IDataControllerData2 {
		#region Fields
		BaseGridController dataController;
		readonly Dictionary<string, bool> complexColumnNames;
		bool isGroupFooter;
		bool isGroupHeader;
		bool lastRowProcessed;
		int lastRow;
		FilterInfo filterInfo;
		List<DataControllerState> states;
		List<GroupInfo> currentGroupInfo;
		List<SpreadsheetParameter> parameters;
		#endregion
		public SpreadsheetDataControllerAdapter(BaseGridController dataController) {
			Guard.ArgumentNotNull(dataController, "dataController");
			this.complexColumnNames = new Dictionary<string, bool>();
			this.dataController = dataController;
			this.lastRowProcessed = false;
			this.filterInfo = null;
			this.parameters = new List<SpreadsheetParameter>();
			InitializeDataController();
		}
		#region Properties
		public override bool IsReady { get { return dataController.IsReady; } }
		public override int ListSourceRowCount { get { return dataController.ListSourceRowCount; } }
		public override int CurrentControllerRow { get { return dataController.CurrentControllerRow; } set { dataController.CurrentControllerRow = value; } }
		public override object DataSource { get { return dataController.DataSource; } set { dataController.DataSource = value; } }
		public override string DataMember { get { return dataController.DataMember; } set { dataController.DataMember = value; } }
		internal bool IsGroupFooter { get { return isGroupFooter; } }
		internal bool IsGroupHeader { get { return isGroupHeader; } }
		internal bool NeedProcessGroupRanges { get { return isGroupHeader || isGroupFooter; } }
		internal bool LastRowProcessed { get { return lastRowProcessed || !CurrentRowIsValid; } }
		internal bool IsSorted { get { return dataController.SortInfo.Count > 0; } }
		bool CurrentRowIsValid { get { return dataController.IsValidControllerRowHandle(dataController.CurrentControllerRow); } }
		public List<SpreadsheetParameter> Parameters { get { return parameters; } }
		#endregion
		protected internal virtual void ReplaceDataController(BaseGridController controller) {
			if (dataController != null)
				DisposeDataController();
			dataController = controller;
			InitializeDataController();
		}
		protected internal virtual void InitializeDataController() {
			dataController.CurrentClient = this;
			dataController.DataClient = this;
			SubscribeDataControllerEvents();
		}
		protected internal virtual void DisposeDataController() {
			UnsubscribeDataControllerEvents();
			dataController.Dispose();
		}
#if !SL
		public virtual void SetBindingContext(BindingContext bindingContext) {
			dataController.SetDataSource(bindingContext, DataSource, DataMember);
		}
#endif
		public override int GetColumnIndex(string name) {
			int index = dataController.Columns.GetColumnIndex(name);
			if (index < 0)
				index = dataController.DetailColumns.GetColumnIndex(name);
			if (index < 0) {
				if (complexColumnNames.ContainsKey(name))
					return -1;
				complexColumnNames.Add(name, true);
				dataController.RePopulateColumns();
				index = dataController.Columns.GetColumnIndex(name);
			}
			return index;
		}
		public override object GetCurrentRowValue(int columnIndex) {
			return dataController.GetCurrentRowValue(columnIndex);
		}
		public override object GetCurrentRow() {
			return dataController.CurrentControllerRowObject;
		}
		protected virtual void ListSourceChanged(object sender, EventArgs e) {
			RaiseDataSourceChanged();
		}
		protected virtual void SubscribeDataControllerEvents() {
			dataController.ListSourceChanged += ListSourceChanged;
		}
		protected virtual void UnsubscribeDataControllerEvents() {
			dataController.ListSourceChanged -= ListSourceChanged;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (dataController != null) {
					DisposeDataController();
					dataController = null;
				}
			}
			base.Dispose(disposing);
		}
		#region IDataControllerCurrentSupport Members
		public void OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			RaiseCurrentRowChangedEvent();
		}
		public void OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) {
			RaiseCurrentRowChangedEvent();
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties {
			get {
				return true;
			}
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection res = new ComplexColumnInfoCollection();
			foreach (string name in complexColumnNames.Keys) {
				if (name.Contains(".") && dataController.Columns[name] == null)
					res.Add(name);
			}
			return res;
		}
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter {
			get { return filterInfo != null; }
		}
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if (collection == null)
				return collection;
			foreach (string name in complexColumnNames.Keys) {
				if (collection.Find(name, false) == null)
					collection.Find(name, true);
			}
			return collection;
		}
		#endregion
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return null;
		}
		object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
			return null;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value) {
		}
		#endregion
		internal object GetCurrentRowValue(string columnName) {
			GetColumnIndex(columnName);
			if (dataController.IsGroupRowHandle(dataController.CurrentControllerRow))
				return dataController.GetListSourceRowValue(dataController.CurrentListSourceIndex, columnName);
			return dataController.GetCurrentRowValue(columnName);
		}
		internal string GetDisplayName(string name, bool needRelationDisplayName) {
			if (string.IsNullOrEmpty(name))
				return name;
			string[] names = name.Split(MailMergeProcessor.DividerString.ToCharArray());
			string result = string.Empty;
			IDisplayNameProvider displayNameProvider = DataSource as IDisplayNameProvider;
			if (displayNameProvider != null)
				return displayNameProvider.GetFieldDisplayName(names);
			for (int i = 0; i < names.Length; i++) {
				if (i != names.Length - 1 || needRelationDisplayName) {
					result += (i > 0 && !string.IsNullOrEmpty(result) ? MailMergeProcessor.DividerString : string.Empty) + dataController.GetRelationDisplayName(0, dataController.GetRelationIndex(0, names[i]));
					this.DataSource = GetCurrentRowValue(names[i]);
					continue;
				}
				foreach (DataColumnInfo column in dataController.Columns) {
					if (column.Name == names[i]) {
						result += (i > 0 && !string.IsNullOrEmpty(result) ? MailMergeProcessor.DividerString : string.Empty) + column.PropertyDescriptor.DisplayName;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(result))
				return name;
			return result;
		}
		#region Group
		internal void SetGroup(List<GroupInfo> groupInfo) {
			DataColumnSortInfo[] dataColumnSortInfo = GetDataColumnSortInfo(groupInfo);
			dataController.GroupSummary.Clear();
			dataController.UpdateSortGroup(dataColumnSortInfo, dataColumnSortInfo.Length, new SummarySortInfo[0]);
			isGroupHeader = true;
			isGroupFooter = false;
			lastRowProcessed = false;
			currentGroupInfo = groupInfo;
		}
		internal string GetCurrentHeaderFooterGroupedName() {
			return dataController.SortInfo[dataController.GetRowLevel(dataController.CurrentControllerRow)].ColumnInfo.Name;
		}
		DataColumnSortInfo[] GetDataColumnSortInfo(List<GroupInfo> groupInfo) {
			List<DataColumnSortInfo> result = new List<DataColumnSortInfo>();
			if (groupInfo == null)
				return result.ToArray();
			foreach (GroupInfo info in groupInfo) {
				if (string.IsNullOrEmpty(info.FieldName))
					continue;
				DataColumnInfo columnInfo = dataController.Columns[info.FieldName];
				if (columnInfo == null)
					continue;
				DataColumnSortInfo sortInfo = new DataColumnSortInfo(columnInfo, info.SortOrder);
				result.Add(sortInfo);
			}
			return result.ToArray();
		}
		#endregion
		internal void SetFilter(FilterInfo filterInfo) {
			if (filterInfo != null) {
				CriteriaOperator resultedCriteriaOperator = CriteriaOperator.Parse(filterInfo.Expression);
				XtraReports.Native.Parameters.ParametersValueSetter.Process(resultedCriteriaOperator, Parameters);
				dataController.FilterCriteria = resultedCriteriaOperator;
			}
			else {
				dataController.FilterCriteria = null;
				dataController.FilterExpression = string.Empty;
			}
			this.filterInfo = filterInfo;
		}
		#region GoToNextRow
		internal void GoToNextRow() {
			if (!isGroupFooter && !isGroupHeader) FromRow();
			else if (isGroupFooter) FromFooter();
			else if (isGroupHeader) FromHeader();
		}
		void FromRow() {
			lastRow = dataController.CurrentControllerRow;
			int current = dataController.GetParentRowHandle(dataController.CurrentControllerRow);
			int next = dataController.GetParentRowHandle(dataController.CurrentControllerRow + 1);
			if (next != current) {
				isGroupFooter = true;
				dataController.CurrentControllerRow = current;
			}
			else if (dataController.IsValidControllerRowHandle(dataController.CurrentControllerRow + 1))
				dataController.CurrentControllerRow++;
			else lastRowProcessed = true;
		}
		void FromFooter() {
			int nextGroup = dataController.GetNextSibling(lastRow);
			int currentParentGroup = dataController.GetParentRowHandle(dataController.CurrentControllerRow);
			int nextParent = dataController.GetParentRowHandle(nextGroup);
			int nextParentSibling = dataController.GetNextSibling(dataController.CurrentControllerRow);
			if (nextParent != currentParentGroup && dataController.IsGroupRowHandle(currentParentGroup))
				GoToNextHeaderFooter(currentParentGroup, false);
			else if (nextParent == dataController.CurrentControllerRow) {
				if (dataController.CurrentControllerRow != nextParentSibling)
					GoToNextHeaderFooter(nextParentSibling, true);
				else lastRowProcessed = true;
			}
			else if (dataController.IsGroupRowHandle(nextGroup) && nextGroup != dataController.CurrentControllerRow)
				GoToNextHeaderFooter(nextGroup, true);
			else lastRowProcessed = true;
		}
		void GoToNextHeaderFooter(int nextGroup, bool header) {
			dataController.CurrentControllerRow = nextGroup;
			isGroupHeader = header;
			isGroupFooter = !header;
		}
		void FromHeader() {
			int childIndex = dataController.GetControllerRowByGroupRow(dataController.CurrentControllerRow);
			if (dataController.GetParentRowHandle(childIndex) == dataController.CurrentControllerRow)
				isGroupHeader = false;
			else
				while (dataController.GetParentRowHandle(childIndex) != dataController.CurrentControllerRow)
					childIndex = dataController.GetParentRowHandle(childIndex);
			dataController.CurrentControllerRow = childIndex;
		}
		#endregion
		#region State
		internal void SaveCurrentState() {
			if (states == null)
				states = new List<DataControllerState>();
			states.Add(new DataControllerState(dataController, isGroupHeader, isGroupFooter, currentGroupInfo, filterInfo, lastRow));
		}
		internal void ReturnToLastState() {
			if (states == null || states.Count <= 0)
				return;
			DataControllerState lastState = states[states.Count - 1];
			dataController.DataMember = lastState.DataMember;
			dataController.DataSource = lastState.DataSource;
			SetFilter(lastState.FilterInfo);
			SetGroup(lastState.CurrentGroupInfo);
			this.isGroupFooter = lastState.IsGroupFooter;
			this.isGroupHeader = lastState.IsGroupHeader;
			dataController.CurrentControllerRow = lastState.CurrentRow;
			lastRowProcessed = false;
			lastRow = lastState.LastRow;
			states.Remove(lastState);
		}
		internal void ResetState() {
			isGroupHeader = false;
			isGroupFooter = false;
			lastRowProcessed = false;
			currentGroupInfo = null;
			dataController.CurrentControllerRow = 0;
			lastRow = 0;
		}
		#endregion
	}
	internal class SpreadsheetDataController : OfficeDataController {
		protected override IList GetListSource() {
#if !DXPORTABLE
			if(DataSource is EFDataSource) {
				IList result = ListBindingHelper.GetList(DataSource, DataMember) as IList;
				return result;
			}
#endif
			if (DataSource != null) {
				Type dataSourceType = DataSource.GetType();
				if (dataSourceType.IsGenericType()) {
					if (dataSourceType.GetGenericTypeDefinition() == typeof(HashSet<>)) {
						List<object> result = new List<object>();
						foreach (object o in (IEnumerable)DataSource) {
							result.Add(o);
						}
						return result;
					}
				}
			}
			return base.GetListSource();
		}
	}
}

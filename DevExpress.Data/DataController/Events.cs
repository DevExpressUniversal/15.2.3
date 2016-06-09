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

using DevExpress.Compatibility.System.Collections;
using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if !SILVERLIGHT
using System.Windows.Forms;
#else
using DevExpress.Xpf.Collections;
using DevExpress.Utils;
#endif
namespace DevExpress.Data {
	public interface IDataControllerVisualClient {
		int VisibleRowCount { get; }
		int TopRowIndex { get; }
		int PageRowCount { get; }
		void UpdateColumns();
		void UpdateRows(int topRowIndexDelta);
		void UpdateLayout();
		void UpdateRow(int controllerRowHandle);
		void UpdateRowIndexes(int newTopRowIndex);
		void UpdateTotalSummary();
		void UpdateScrollBar();
		void RequestSynchronization();
		bool IsInitializing { get; }
		void ColumnsRenewed();
		void RequireSynchronization(IDataSync dataSync);
	}
	public interface IDataControllerVisualClient2 : IDataControllerVisualClient {
		void TotalSummaryCalculated();
	}
	public class NullVisualClient : IDataControllerVisualClient {
		internal static NullVisualClient Default = new NullVisualClient();
		#region IDataControllerVisualClient Members
		int IDataControllerVisualClient.VisibleRowCount { get { return 0; } }
		int IDataControllerVisualClient.TopRowIndex { get { return 0; } }
		int IDataControllerVisualClient.PageRowCount { get { return 0; } }
		void IDataControllerVisualClient.UpdateColumns() { }
		void IDataControllerVisualClient.UpdateRows(int topRowIndexDelta) { }
		void IDataControllerVisualClient.UpdateLayout() { }
		void IDataControllerVisualClient.UpdateRow(int controllerRowHandle) { }
		void IDataControllerVisualClient.UpdateRowIndexes(int newTopRowIndex) { }
		void IDataControllerVisualClient.UpdateTotalSummary() { }
		void IDataControllerVisualClient.UpdateScrollBar() { }
		void IDataControllerVisualClient.RequestSynchronization() { }
		bool IDataControllerVisualClient.IsInitializing { get { return false; } }
		void IDataControllerVisualClient.ColumnsRenewed() {}
		void IDataControllerVisualClient.RequireSynchronization(IDataSync dataSync) { }
		#endregion
	}
	internal class NullThreadClient : IDataControllerThreadClient {
		internal static NullThreadClient Default = new NullThreadClient();
		void IDataControllerThreadClient.OnAsyncBegin() { }
		void IDataControllerThreadClient.OnAsyncEnd() { }
		void IDataControllerThreadClient.OnRowLoaded(int controllerRowHandle) { }
		void IDataControllerThreadClient.OnTotalsReceived() { }
	}
	internal class NullCurrentSupport : IDataControllerCurrentSupport {
		internal static NullCurrentSupport Default = new NullCurrentSupport();
	#region IDataControllerCurrentSupport Members
		void IDataControllerCurrentSupport.OnCurrentControllerRowChanged(CurrentRowEventArgs e) { }
		void IDataControllerCurrentSupport.OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) { }
		#endregion
	}
	public interface IRelationList {
		string GetRelationName(int index, int relationIndex);
		int RelationCount { get; }
		bool IsMasterRowEmpty(int index, int relationIndex);
		IList GetDetailList(int index, int relationIndex);
	}
	public interface IRelationListEx : IRelationList {
		int GetRelationCount(int index);
		string GetRelationDisplayName(int index, int relationIndex);
	}
	public interface IDataControllerRelationSupport {
		string GetRelationName(string name, int controllerRow, int relationIndex);
		string GetRelationDisplayName(string displayName, int controllerRow, int relationIndex);
		bool IsMasterRowEmpty(bool isEmpty, int controllerRow, int relationIndex);
		IList GetDetailList(int controllerRow, int relationIndex);
		int GetRelationCount(int relationCount, int controllerRow);
	}
	public interface IDataControllerThreadClient {
		void OnAsyncBegin();
		void OnAsyncEnd();
		void OnRowLoaded(int controllerRowHandle);
		void OnTotalsReceived();
	}
	[
#if !SL && !DXPORTABLE
	TypeConverter(typeof(DevExpress.Utils.Design.UnboundColumnTypeConverter)),
#endif
	DevExpress.Utils.Design.ResourceFinder(typeof(DevExpress.Data.ResFinder))
	]
	public enum UnboundColumnType { Bound, Integer, Decimal, DateTime, String, Boolean, Object }
	public class UnboundColumnInfo {
		string name;
		UnboundColumnType columnType;
		Type dataType;
		bool readOnly;
		string expression;
		bool requireValueConversion;
		bool visible;
		public UnboundColumnInfo(string name, UnboundColumnType columnType, bool readOnly) : this(name, columnType, readOnly, string.Empty) { }
		public UnboundColumnInfo(string name, UnboundColumnType columnType, bool readOnly, string expression) : this(name, columnType, readOnly, expression, true) { }
		public UnboundColumnInfo(string name, UnboundColumnType columnType, bool readOnly, string expression, bool visible) {
			this.visible = visible;
			this.expression = expression;
			this.name = name;
			this.columnType = columnType;
			this.readOnly = readOnly;
			this.dataType = GetDataType();
			this.requireValueConversion = !string.IsNullOrEmpty(Expression) && !DataType.Equals(typeof(object));
		}
		public string Expression { get { return expression; } }
		public UnboundColumnInfo() : this(string.Empty, UnboundColumnType.Object, true) { }
		public bool ReadOnly { get { return readOnly; } set { readOnly = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public string Name { get { return name; } set { name = value; } }
		public Type DataType { get { return dataType; } set { dataType = value; } }
		public UnboundColumnType ColumnType { 
			get { return columnType; }
			set {
				columnType = value;
				dataType = GetDataType();
			}
		}
		public bool RequireValueConversion {
			get { return requireValueConversion; }
			set { requireValueConversion = value; }
		}
		static Type[] dataTypes = new Type[] { typeof(object), typeof(int), typeof(Decimal), typeof(DateTime), typeof(string), typeof(bool), typeof(object) };
		protected Type GetDataType() {
			return dataTypes[(int)ColumnType];
		}
	}
	public class UnboundColumnInfoCollection : CollectionBase {
		public UnboundColumnInfoCollection() { }
		public UnboundColumnInfoCollection(UnboundColumnInfo[] infos) {
			AddRange(infos);
		}
		public int Add(UnboundColumnInfo info) {
			return List.Add(info);
		}
		public void AddRange(UnboundColumnInfo[] infos) {
			foreach(UnboundColumnInfo info in infos) Add(info);
		}
		public UnboundColumnInfo this[int index] { get { return (UnboundColumnInfo)List[index]; } }
	}
	public class DataControllerSortRowEventArgs : EventArgs {
		int listSourceRow1, listSourceRow2;
		int result;
		bool handled;
		public DataControllerSortRowEventArgs() : this(0, 0) { }
		public DataControllerSortRowEventArgs(int listSourceRow1, int listSourceRow2) {
			Setup(listSourceRow1, listSourceRow2);
		}
		public int ListSourceRow1 { get { return listSourceRow1; } }
		public int ListSourceRow2 { get { return listSourceRow2; } }
		public int Result { get { return result; } set { result = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		internal void Setup(int listSourceRow1, int listSourceRow2) {
			this.listSourceRow1 = listSourceRow1;
			this.listSourceRow2 = listSourceRow2;
			this.result = 0;
			this.handled = true;
		}
	}
	public class DataControllerSortCellEventArgs : DataControllerSortRowEventArgs {
		internal DataColumnInfo sortColumn;
		internal object value1, value2;
		public DataControllerSortCellEventArgs() : this(0, 0, null, null, null) { }
		public DataControllerSortCellEventArgs(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) : base(listSourceRow1, listSourceRow2) {
			Setup(listSourceRow1, listSourceRow2, value1, value2, sortColumn);
		}
		public DataColumnInfo SortColumn { get { return sortColumn; } }
		public object Value1 { get { return value1; } }
		public object Value2 { get { return value2; } }
		internal void Setup(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) {
			Setup(listSourceRow1, listSourceRow2);
			this.sortColumn = sortColumn;
			this.value1 = value1;
			this.value2 = value2;
		}
	}
	public delegate void DataControllerSortRowEventHandler(object sender, DataControllerSortRowEventArgs e);
	public delegate void DataControllerSortCellEventHandler(object sender, DataControllerSortCellEventArgs e);
	public class CustomSummaryEventArgs : EventArgs {
		CustomSummaryProcess summaryProcess;
		object totalValue;
		object fieldValue;
		int controllerRow; 
		int groupRowHandle, groupLevel;
		object item;
		internal DataController controller;
		bool totalValueReady = false;
		public CustomSummaryEventArgs() : this(0, null, null, 0, CustomSummaryProcess.Calculate, null, 0) { }
		public CustomSummaryEventArgs(int controllerRow, object totalValue, object fieldValue, int groupRowHandle, CustomSummaryProcess summaryProcess, object item, int groupLevel) {
			Setup(controllerRow, totalValue, fieldValue, null, summaryProcess, item);
			this.controller = null;
			this.groupRowHandle = groupRowHandle;
			this.groupLevel = groupLevel;
		}
		public object TotalValue {
			get { return totalValue; }
			set { totalValue = value; }
		}
		public bool TotalValueReady { get { return totalValueReady; } set { totalValueReady = value; } }
		public int GroupLevel { get { return groupLevel; } }
		public object Item { get { return item; } }
		public CustomSummaryProcess SummaryProcess { get { return summaryProcess; } }
		public int GroupRowHandle { get { return groupRowHandle; } }
		public object FieldValue { get { return fieldValue; } }
		public int RowHandle { get { return controllerRow; } } 
		public virtual bool IsGroupSummary { get { return GroupRowHandle < 0; } }
		public virtual bool IsTotalSummary { get { return GroupRowHandle == 0; } }
		protected internal void SetupSummaryProcess(CustomSummaryProcess summaryProcess) {
			this.summaryProcess = summaryProcess;
			this.totalValueReady = false;
		}
		protected internal void SetupCell(int controllerRow, object fieldValue) {
			this.controllerRow = controllerRow;
			this.fieldValue = fieldValue;
			this.totalValueReady = false;
		}
		protected internal void Setup(int controllerRow, object totalValue, object fieldValue, GroupRowInfo groupRow, CustomSummaryProcess summaryProcess, object item) {
			this.totalValueReady = false;
			this.groupRowHandle = 0;
			this.groupLevel = -1;
			this.controllerRow = controllerRow;
			this.fieldValue = fieldValue;
			this.summaryProcess = summaryProcess;
			this.item = item;
			this.totalValue = totalValue;
			if(groupRow != null) {
				this.groupRowHandle = groupRow.Handle;
				this.groupLevel = groupRow.Level;
			}
		}
		public object Row { get { return controller == null ? null : controller.GetRow(controllerRow); } }
		public object GetValue(string fieldName) {
			if(controller == null) return null;
			return controller.GetRowValue(controllerRow, fieldName);
		}
		public object GetGroupSummary(int groupRowHandle, object summaryItem) {
			if(controller == null) return null;
			Hashtable summary = controller.GetGroupSummary(groupRowHandle);
			return summary == null ? null : summary[summaryItem];
		}
	}
	public class CustomSummaryExistEventArgs : EventArgs {
		bool exists;
		int groupRowHandle, groupLevel;
		object item;
		public CustomSummaryExistEventArgs(int groupRowHandle, int groupLevel, object item) {
			this.item = item;
			this.groupRowHandle = groupRowHandle;
			this.groupLevel = groupLevel;
			this.exists = true;
		}
		protected internal CustomSummaryExistEventArgs(GroupRowInfo groupRow, object item) : this(0, -1, item) {
			if(groupRow != null) {
				this.groupRowHandle = groupRow.Handle;
				this.groupLevel = groupRow.Level;
			}
		}
		public object Item { get { return item; } }
		public virtual bool Exists {
			get { return exists; }
			set { exists = value; }
		}
		public int GroupLevel { get { return groupLevel; } }
		public virtual int GroupRowHandle { get { return groupRowHandle; } }
		public virtual bool IsGroupSummary { get { return GroupRowInfo.IsGroupRowHandle(GroupRowHandle); } }
		public virtual bool IsTotalSummary { get { return !IsGroupSummary; } }
	}
	public delegate void CustomSummaryEventHandler(object sender, CustomSummaryEventArgs e);
	public delegate void CustomSummaryExistEventHandler(object sender, CustomSummaryExistEventArgs e);
	public class SelectionChangedEventArgs : EventArgs {
		CollectionChangeAction action;
		int controllerRow;
		static SelectionChangedEventArgs refresh = null;
		internal static SelectionChangedEventArgs Refresh {
			get { 
				if(refresh == null) refresh = new SelectionChangedEventArgs();
				return refresh;
			}
		}
		public SelectionChangedEventArgs() : this(CollectionChangeAction.Refresh, DataController.InvalidRow) { }
		public SelectionChangedEventArgs(CollectionChangeAction action, int controllerRow) {
			this.action = action;
			this.controllerRow = controllerRow;
		}
		public CollectionChangeAction Action { get { return action; } }
		public int ControllerRow { get { return controllerRow; } }
	}
	public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
	public class ListSortInfo {
		string propertyName;
		ListSortDirection sortDirection;
		public ListSortInfo(string propertyName, ListSortDirection sortDirection) {
			this.sortDirection = sortDirection;
			this.propertyName = propertyName;
		}
		public string PropertyName { get { return propertyName; } }
		public ListSortDirection SortDirection { get { return sortDirection; } }
	}
	public class CollectionViewFilterSortGroupInfoChangedEventArgs {
		int groupCount;
		bool filterChanged;
		List<ListSortInfo> sortInfo;
		bool needRefresh;
		public List<ListSortInfo> SortInfo { get { return sortInfo; } private set { sortInfo = value; } }
		public int GroupCount { get { return groupCount; } private set { groupCount = value; } }
		public bool FilterChanged { get { return filterChanged; } private set { filterChanged = value; } }
		public bool NeedRefresh { get { return needRefresh; } private set { needRefresh = value; } }
		public CollectionViewFilterSortGroupInfoChangedEventArgs(List<ListSortInfo> sortInfo, int groupCount, bool filterChanged, bool needRefresh) {
			SortInfo = sortInfo;
			GroupCount = groupCount;
			FilterChanged = filterChanged;
			NeedRefresh = needRefresh;
		}
	}
	public delegate void CollectionViewFilterSortGroupInfoChangedEventHandler(object sender, CollectionViewFilterSortGroupInfoChangedEventArgs e);
	public interface IDataSync {
		event CollectionViewFilterSortGroupInfoChangedEventHandler FilterSortGroupInfoChanged;
		List<ListSortInfo> Sort { get; }
		int GroupCount { get; }
		bool AllowSyncSortingAndGrouping { get; set; }
		bool ResetCache();
		void Initialize();
		bool HasFilter { get; }
	}
}

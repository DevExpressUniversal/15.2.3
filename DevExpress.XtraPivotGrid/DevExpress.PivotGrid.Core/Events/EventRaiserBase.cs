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
using DevExpress.Data.PivotGrid;
using DevExpress.PivotGrid.QueryMode.Sorting;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid.Events {
	public delegate void EventEntryPoint(List<object> pars);
	public class PivotEventRecord {
		EventEntryPoint entryPoint;
		List<object> parList;
		public PivotEventRecord(EventEntryPoint entryPoint, params object[] pars) {
			this.entryPoint = entryPoint;
			this.parList = new List<object>();
			if(pars == null)
				return;
			foreach(object par in pars)
				this.parList.Add(par);
		}
		public void Raise() {
			entryPoint.Invoke(parList);
		}
	}
	public enum EventRaiserMode { Record, Raise };
	public class PivotGridEventsRecorderBase : IPivotGridEventsImplementorBase {
		readonly Queue<PivotEventRecord> eventList = new Queue<PivotEventRecord>();
		readonly IPivotGridEventsImplementorBase eventsImplementor;
		EventRaiserMode mode;
		protected bool IsInRecording { get { return Mode == EventRaiserMode.Record; } }
		protected IPivotGridEventsImplementorBase BaseImpl { get { return eventsImplementor; } }
		protected IPivotGridEventsImplementorBase CurrentImpl { get { return (IPivotGridEventsImplementorBase)this; } }
		protected Queue<PivotEventRecord> EventList { get { return eventList; } }
		protected EventRaiserMode Mode { get { return mode; } }
		public PivotGridEventsRecorderBase(IPivotGridEventsImplementorBase eventsImplementor) {
			this.eventsImplementor = eventsImplementor;
			this.mode = EventRaiserMode.Raise;
		}
		public void StartRecording() {
			mode = EventRaiserMode.Record;
		}
		public void FinishRecordingAndRaiseEvents() {
			mode = EventRaiserMode.Raise;
			RaiseEventList();
		}
		void RaiseEventList() {
			while(EventList.Count > 0)
				EventList.Dequeue().Raise();
		}
		protected virtual void EnsureIsNotRecording() { }
		protected void AddEventRecord(EventEntryPoint entryPoint, params object[] pars) {
			EventList.Enqueue(new PivotEventRecord(entryPoint, pars));
		}
		#region IPivotGridEventsImplementorBase Members
		void IPivotGridEventsImplementorBase.CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			BaseImpl.CalcCustomSummary(field, customSummaryInfo);
		}
		object IPivotGridEventsImplementorBase.CustomGroupInterval(PivotGridFieldBase field, object value) {
			return BaseImpl.CustomGroupInterval(field, value);
		}
		int? IPivotGridEventsImplementorBase.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return BaseImpl.GetCustomSortRows(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		object IPivotGridEventsImplementorBase.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return BaseImpl.GetUnboundValue(field, listSourceRowIndex, expValue);
		}
		int? IPivotGridEventsImplementorBase.QuerySorting(IQueryMemberProvider value0, IQueryMemberProvider value1, PivotGridFieldBase field, ICustomSortHelper helper) {
			return BaseImpl.QuerySorting(value0, value1, field, helper);
		}
		bool IPivotGridEventsImplementorBase.FieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			EnsureIsNotRecording();
			return BaseImpl.FieldAreaChanging(field, newArea, newAreaIndex);
		}
		bool IPivotGridEventsImplementorBase.FieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			EnsureIsNotRecording();
			return BaseImpl.FieldFilterChanging(field, filterType, showBlanks, values);
		}
		void IPivotGridEventsImplementorBase.LayoutChanged() {
			EnsureIsNotRecording();
			BaseImpl.LayoutChanged();
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, object value) {
			return BaseImpl.FieldValueDisplayText(field, value);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, IOLAPMember member) {
			return BaseImpl.FieldValueDisplayText(field, member);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			return BaseImpl.FieldValueDisplayText(item, defaultText);
		}
		string IPivotGridEventsImplementorBase.CustomCellDisplayText(PivotGridCellItem cellItem) {
			return BaseImpl.CustomCellDisplayText(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomCellValue(PivotGridCellItem cellItem) {
			return BaseImpl.CustomCellValue(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			EnsureIsNotRecording();
			return BaseImpl.CustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		void IPivotGridEventsImplementorBase.CustomChartDataSourceRows(IList<PivotChartDataSourceRowBase> rows) {
			EnsureIsNotRecording();
			BaseImpl.CustomChartDataSourceRows(rows);
		}
		bool IPivotGridEventsImplementorBase.CustomFilterPopupItems(PivotGridFilterItems items) {
			EnsureIsNotRecording();
			return BaseImpl.CustomFilterPopupItems(items);
		}
		bool IPivotGridEventsImplementorBase.CustomFieldValueCells(PivotVisualItemsBase items) {
			return BaseImpl.CustomFieldValueCells(items);
		}
		void IPivotGridEventsImplementorBase.OLAPQueryTimeout() {
			if(IsInRecording)
				AddEventRecord(OLAPQueryTimeoutEntryPoint);
			else
				BaseImpl.OLAPQueryTimeout();
		}
		void OLAPQueryTimeoutEntryPoint(List<object> pars) {
			CurrentImpl.OLAPQueryTimeout();
		}
		bool IPivotGridEventsImplementorBase.QueryException(Exception ex) {
			return BaseImpl.QueryException(ex);
		}
		bool IPivotGridEventsImplementorBase.BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			return BaseImpl.BeforeFieldValueChangeExpanded(item);
		}
		void IPivotGridEventsImplementorBase.BeginRefresh() {
			if(!IsInRecording)
				BaseImpl.BeginRefresh();
		}
		void IPivotGridEventsImplementorBase.EndRefresh() {
			if(!IsInRecording)
				BaseImpl.EndRefresh();
		}
		void IPivotGridEventsImplementorBase.DataSourceChanged() {
			if(IsInRecording)
				AddEventRecord(DataSourceChangedEntryPoint, null);
			else
				BaseImpl.DataSourceChanged();
		}
		void DataSourceChangedEntryPoint(List<object> pars) {
			CurrentImpl.DataSourceChanged();
		}
		void IPivotGridEventsImplementorBase.GroupFilterChanged(PivotGridGroup group) {
			if(IsInRecording)
				AddEventRecord(GroupFilterChangedEntryPoint, group);
			else
				BaseImpl.GroupFilterChanged(group);
		}
		void GroupFilterChangedEntryPoint(List<object> pars) {
			CurrentImpl.GroupFilterChanged((PivotGridGroup)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldFilterChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldFilterChangedEntryPoint, field);
			else
				BaseImpl.FieldFilterChanged(field);
		}
		void FieldFilterChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldFilterChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldAreaChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldAreaChangedEntryPoint, field);
			else
				BaseImpl.FieldAreaChanged(field);
		}
		protected void FieldAreaChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldAreaChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldExpandedInFieldsGroupChangedEntryPoint, field);
			else
				BaseImpl.FieldExpandedInFieldsGroupChanged(field);
		}
		void FieldExpandedInFieldsGroupChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldExpandedInFieldsGroupChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldWidthChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldWidthChangedEntryPoint, field);
			else
				BaseImpl.FieldWidthChanged(field);
		}
		void FieldWidthChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldWidthChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldUnboundExpressionChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldUnboundExpressionChangedEntryPoint, field);
			else
				BaseImpl.FieldUnboundExpressionChanged(field);
		}
		void FieldUnboundExpressionChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldUnboundExpressionChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldAreaIndexChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldAreaIndexChangedEntryPoint, field);
			else
				BaseImpl.FieldAreaIndexChanged(field);
		}
		void FieldAreaIndexChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldAreaIndexChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldVisibleChanged(PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(FieldVisibleChangedEntryPoint, field);
			else
				BaseImpl.FieldVisibleChanged(field);
		}
		protected void FieldVisibleChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldVisibleChanged((PivotGridFieldBase)pars[0]);
		}
		void IPivotGridEventsImplementorBase.FieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) {
			if(IsInRecording)
				AddEventRecord(FieldPropertyChangedEntryPoint, field, propertyName);
			else
				BaseImpl.FieldPropertyChanged(field, propertyName);
		}
		void FieldPropertyChangedEntryPoint(List<object> pars) {
			CurrentImpl.FieldPropertyChanged((PivotGridFieldBase)pars[0], (PivotFieldPropertyName)pars[1]);
		}
		void IPivotGridEventsImplementorBase.PrefilterCriteriaChanged() {
			if(IsInRecording)
				AddEventRecord(PrefilterCriteriaChangedEntryPoint);
			else
				BaseImpl.PrefilterCriteriaChanged();
		}
		void PrefilterCriteriaChangedEntryPoint(List<object> pars) {
			CurrentImpl.PrefilterCriteriaChanged();
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(IsInRecording)
				AddEventRecord(AfterFieldValueChangeExpandedEntryPoint, item);
			else
				BaseImpl.AfterFieldValueChangeExpanded(item);
		}
		void AfterFieldValueChangeExpandedEntryPoint(List<object> pars) {
			CurrentImpl.AfterFieldValueChangeExpanded((PivotFieldValueItem)pars[0]);
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) {
			if(IsInRecording)
				AddEventRecord(AfterFieldValueChangeNotExpandedEntryPoint, item, field);
			else
				BaseImpl.AfterFieldValueChangeNotExpanded(item, field);
		}
		void AfterFieldValueChangeNotExpandedEntryPoint(List<object> pars) {
			CurrentImpl.AfterFieldValueChangeNotExpanded((PivotFieldValueItem)pars[0], (PivotGridFieldBase)pars[1]);
		}
		#endregion
	}
	public class PivotGridEventRaiserBase : PivotGridEventsRecorderBase {
		public PivotGridEventRaiserBase(IPivotGridEventsImplementorBase eventsImplementor) : base(eventsImplementor) {
		}
		protected override void EnsureIsNotRecording() {
			if(Mode == EventRaiserMode.Record)
				throw new IncorrectAsyncOperationCallException();
		}
		void DeliverActionEntryPoint(List<object> pars) {
			((Action)pars[0])();
		}
		public void DoActionInMainThread(Action action) {
			if(IsInRecording)
				AddEventRecord(DeliverActionEntryPoint, action);
			else
				action();
		}
	}
}

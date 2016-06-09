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
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.DataCalculation;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
namespace DevExpress.XtraPivotGrid.Data {
	public abstract class OLAPDataSourceBase : QueryDataSource<OLAPCubeColumn>, IPivotOLAPDataSource, IOLAPHelpersOwner {
		OLAPMetadata metadata;
		public OLAPDataSourceBase() : this((PivotGridData)null) {
		}
		protected OLAPDataSourceBase(PivotGridData pivotGridData) : base(pivotGridData) {		
		}
		protected OLAPDataSourceBase(OLAPMetadata metadata) : base(null) {
			this.metadata = metadata;
		}
		internal override PivotDrillDownDataSource QueryDrilldown(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, List<string> customColumns) {
			if(DataArea.Count == 0 && !OptionsOLAP.UseDefaultMeasure)
				return null;
			return base.QueryDrilldown(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns);
		}
		protected override QueryColumns<OLAPCubeColumn> CreateCubeColumns() {
			return new OLAPCubeColumns(this);
		}
		protected override QueryAreas<OLAPCubeColumn> CreateAreas() {
			return new OLAPAreas(this);
		}
		protected override IQueryFilterHelper CreateFilterHelper() {
			if(Owner != null && Owner.OptionsOLAP.UsePrefilter)
				return new OLAPCriteriaFilterHelper(this);
			return new OLAPFilterHelper(this);
		}
		protected override UniqueValues<OLAPCubeColumn> CreateUniqueValues() {
			return new OLAPUniqueValues(this);
		}
		protected override bool IsFieldTypeCheckRequiredCore(PivotGridFieldBase field) {
			if(!Contains(field.FieldName))
				return true;
			OLAPCubeColumn column = CubeColumns[field.FieldName];
			return !column.HasCalculatedMembers && !column.OLAPFilterByUniqueName;
		}
		protected override bool IsAreaAllowedCore(PivotGridFieldBase field, PivotArea area) {
			OLAPCubeColumn column = CubeColumns[field.FieldName];
			if(column != null) {
				if(column.IsMeasure)
					return area == PivotArea.DataArea;
				else
					return area != PivotArea.DataArea;
			}
			if(OLAPHierarchy.GetIsMeasure(field))
				return area == PivotArea.DataArea;
			else
				return area != PivotArea.DataArea;
		}
		protected internal PivotGridData Data { get { return Owner as PivotGridData; } }
		protected internal OLAPMetadata OLAPMetadata {
			get {
				if(metadata == null) {
					metadata = CreateMetadata();
					metadata.RegisterOwner(this);
				}
				return metadata;
			}
		}
		protected abstract OLAPMetadata CreateMetadata();
		protected override QueryMetadata<OLAPCubeColumn> Metadata {
			get {
				return OLAPMetadata;
			}
		}
		protected internal OLAPHierarchies Hierarchies { get { return OLAPMetadata.Hierarchies; } }
		protected OLAPMetadataColumns MetadataColumns { get { return OLAPMetadata.Columns; } }
		protected List<OLAPCubeColumn> ColumnArea { get { return Areas.ColumnArea; } }
		protected List<OLAPCubeColumn> RowArea { get { return Areas.RowArea; } }
		protected List<OLAPCubeColumn> FilterArea { get { return Areas.FilterArea; } }
		protected new OLAPCubeColumns CubeColumns { get { return (OLAPCubeColumns)base.CubeColumns; } }
		protected new OLAPAreas Areas { get { return (OLAPAreas)base.Areas; } }
		protected PivotGridOptionsOLAP OptionsOLAP { get { return Owner.OptionsOLAP; } }
		protected internal virtual bool Connected { get { return Metadata.Connected; } }
		protected override PivotDataSourceCaps Capabilities {
			get { 
				if(Data.OptionsData.IsProcessExpressionOnSummaryLevel)
					return PivotDataSourceCaps.DenyExpandValuesAllowed | PivotDataSourceCaps.UnboundColumns;
				else
					return PivotDataSourceCaps.DenyExpandValuesAllowed;
			}
		}
		protected abstract bool RequirePopulateColumnsOnEmptyOnly();
		void IOLAPHelpersOwner.ClearState(bool layoutChanged) {
			CubeColumns.Clear();
			Areas.Clear();
			if(layoutChanged)
				RaiseLayoutChanged();
		}
		void IOLAPHelpersOwner.OnSuccessConnected() {
			OnSuccessConnected();
		}
		protected virtual void OnSuccessConnected() {
			RaiseConnectionChanged();
		}
		protected override void OnInitialized() {
			if(Metadata.Connected && CubeColumns.Count == 0 && Metadata.Connected && CubeColumns.Count == 0)
				OnSuccessConnected();
		}
		protected void RaiseConnectionChanged() {
			RaiseOLAPGroupsChanged();
			RaiseDataChanged();
		}
		protected internal virtual PivotOLAPKPIValue QueryKPIValue(string kpiName) {
			if(!Metadata.Connected)
				return null;
			return OLAPMetadata.QueryKPIValue(kpiName);
		}
		protected void EnsureColumnsPopulated(bool force) {
			if(MetadataColumns.Count == 0 && (RequirePopulateColumnsOnEmptyOnly() || (force || !IsDesignMode)))
				PopulateColumns();
			if(CubeColumns.Count == 0 && (RequirePopulateColumnsOnEmptyOnly() || (force || !IsDesignMode)))
				CreateCubeColumnsByMetadataColumns();
		}
		#region IPivotOLAPDataSource
		PivotOLAPDataSourceEventHandler olapGroupsChanged, olapQueryTimeout;
		event PivotOLAPDataSourceEventHandler IPivotOLAPDataSource.OLAPGroupsChanged {
			add { olapGroupsChanged += value; }
			remove { olapGroupsChanged -= value; }
		}
		event PivotOLAPDataSourceEventHandler IPivotOLAPDataSource.OLAPQueryTimeout {
			add { olapQueryTimeout += value; }
			remove { olapQueryTimeout -= value; }
		}
		void RaiseOLAPGroupsChanged() {
			if(olapGroupsChanged != null)
				olapGroupsChanged(this);
		}
		void RaiseOLAPQueryTimeout() {
			if(olapQueryTimeout != null)
				olapQueryTimeout(this);
		}
		string IPivotOLAPDataSource.FullConnectionString {
			get { return OLAPMetadata.FullConnectionString; }
			set { OLAPMetadata.FullConnectionString = value; }
		}
		bool IPivotOLAPDataSource.PopulateColumns() {
			bool result = OLAPMetadata.PopulateColumns(true, this);
			if(result && Metadata.Columns.Count > 0 && CubeColumns.Count == 0)
				CreateCubeColumnsByMetadataColumns();
			return result;
		}
		protected override sealed void PopulateColumns() {
			if(Metadata.Columns.Count > 0)
				((IOLAPHelpersOwner)this).ClearState(false);
			else
				Metadata.PopulateColumnsCore(null);
			if(Metadata.Columns.Count > 0 && CubeColumns.Count == 0)
				CreateCubeColumnsByMetadataColumns();
		}
		protected override PartialUpdaterBase<OLAPCubeColumn> UpdateCubeColumnsState(bool needExpand, CollapsedState row, CollapsedState column) {
			if(Metadata.Columns.Count != 0 && CubeColumns.Count == 0)
				CreateCubeColumnsByMetadataColumns();
			return base.UpdateCubeColumnsState(needExpand, row, column);
		}
		string IPivotOLAPDataSource.GetHierarchyName(string columnName) {
			return OLAPMetadata.GetHierarchyName(columnName);
		}
		string IPivotOLAPDataSource.CubeName { get { return OLAPMetadata.CubeName; } }
		string IPivotOLAPDataSource.CubeCaption { get { return OLAPMetadata.CubeCaption; } }
		DefaultBoolean IPivotOLAPDataSource.GetColumnIsAggregatable(string dimensionName) {
			return OLAPMetadata.GetColumnIsAggregatable(dimensionName);
		}
		List<string> IPivotOLAPDataSource.GetKPIList() {
			return OLAPMetadata.GetKPIList();
		}
		string IPivotOLAPDataSource.GetKPIName(string fieldName, PivotKPIType kpiType) {
			return OLAPMetadata.GetKPIName(fieldName, kpiType);
		}
		PivotKPIType IPivotOLAPDataSource.GetKPIType(string fieldName) {
			return OLAPMetadata.GetKPIType(fieldName);
		}
		PivotKPIGraphic IPivotOLAPDataSource.GetKPIGraphic(string fieldName) {
			return OLAPMetadata.GetKPIGraphic(fieldName);
		}
		PivotOLAPKPIMeasures IPivotOLAPDataSource.GetKPIMeasures(string kpiName) {
			return OLAPMetadata.GetKPIMeasures(kpiName);
		}
		PivotOLAPKPIValue IPivotOLAPDataSource.GetKPIValue(string kpiName) {
			return this.QueryKPIValue(kpiName);
		}
		PivotKPIGraphic IPivotOLAPDataSource.GetKPIServerDefinedGraphic(string kpiName, PivotKPIType kpiType) {
			return OLAPMetadata.GetKPIServerDefinedGraphic(kpiName, kpiType);
		}
		string IPivotOLAPDataSource.GetMeasureServerDefinedFormatString(string fieldName) {
			return OLAPMetadata.GetMeasureServerDefinedFormatString(fieldName);
		}
		IOLAPMember IPivotOLAPDataSource.GetMember(bool isColumn, int visibleIndex) {
			return (IOLAPMember)GetFieldValues(isColumn)[visibleIndex].Member;
		}
		IOLAPMember IPivotOLAPDataSource.GetMemberByValue(string fieldName, object value) {
			return OLAPMetadata.GetMemberByValue(CubeColumns[fieldName], value) as IOLAPMember;
		}
		IOLAPMember IPivotOLAPDataSource.GetMemberByUniqueName(string fieldName, object value) {
			return OLAPMetadata.Columns.GetMemberByUniqueLevelValue(fieldName, value);
		}
		IOLAPMember[] IPivotOLAPDataSource.GetUniqueMembers(string fieldName) {
			return OLAPMetadata.GetUniqueMembers(CubeColumns[fieldName]);
		}
		Dictionary<string, OLAPDataType> IPivotOLAPDataSource.GetProperties(string fieldName) {
			OLAPMetadataColumn column = MetadataColumns[fieldName];
			if(column == null)
				return null;
			return column.Properties;
		}
		string IPivotOLAPDataSource.GetDefaultSortProperty(string fieldName) {
			OLAPMetadataColumn column = MetadataColumns[fieldName];
			if(column == null)
				return null;
			return column.DefaultSortProperty;
		}
		string IPivotOLAPDataSource.GetDrillDownColumnName(string fieldName) {
			return OLAPMetadata.GetDrillDownColumnName(fieldName);
		}
		int IPivotOLAPDataSource.GetFieldHierarchyLevel(string fieldName) {
			return OLAPMetadata.GetFieldHierarchyLebel(fieldName);
		}
		bool IPivotOLAPDataSource.GetOlapIsUserHierarchy(string hierarchyName) {
			return OLAPMetadata.GetOlapIsUserHierarchy(hierarchyName);
		}
		void IPivotOLAPDataSource.Connect() {
			OLAPMetadata.Connect(true);
		}
		void IPivotOLAPDataSource.EnsureConnected() {
			if(!Connected)
				OLAPMetadata.Connect(true);
		}
		void IPivotOLAPDataSource.Disconnect() {
			Metadata.Disconnect();
		}
		bool IPivotOLAPDataSource.Connected { get { return Metadata.Connected; } }
		int[] IPivotOLAPDataSource.GetFieldNotExpandedIndexes(PivotGridFieldBase field) {
			OLAPMetadataColumn column = MetadataColumns[field.FieldName];
			if(column.Hierarchy.Structure != 2) 
				return new int[0];
			return GetFieldValues(field.Area == PivotArea.ColumnArea).GetAllNotExpandedIndexes(field.AreaIndex);
		}
		int IPivotOLAPDataSource.GetVisibleIndexByUniqueValues(bool isColumn, object[] values) {
			return GetFieldValues(isColumn).GetIndexUseULV(values);
		}
		#endregion
		#region IOLAPHelpersOwner Members
		OLAPAreas IOLAPHelpersOwner.Areas { get { return this.Areas; } }
		AreaFieldValues IOLAPHelpersOwner.ColumnValues { get { return ColumnValues; } }
		AreaFieldValues IOLAPHelpersOwner.RowValues { get { return RowValues; } }
		OLAPCubeColumns IOLAPHelpersOwner.CubeColumns { get { return this.CubeColumns; } }
		PivotGridOptionsOLAP IOLAPHelpersOwner.Options { get { return Owner.OptionsOLAP; } }
		IOLAPFilterHelper IOLAPHelpersOwner.FilterHelper { get { return (IOLAPFilterHelper)this.FilterHelper; } }
		OLAPUniqueValues IOLAPHelpersOwner.UniqueValues { get { return (OLAPUniqueValues)base.UniqueValues; } }
		#endregion
		#region IDisposable implementation
		void IDisposable.Dispose() {
			Owner = null;
		}
		#endregion
		bool IOLAPHelpersOwner.IsLocked { get { return Data == null ? false : Data.IsLocked; } }
		IOLAPMetadata IOLAPHelpersOwner.Metadata { get { return OLAPMetadata; } }
		Dictionary<string, Dictionary<string, string>> IOLAPHelpersOwner.GetExpressionNamesByHierarchies(CriteriaOperator criteria) {
			ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
			criteria.Accept(visitor);
			Dictionary<string, Dictionary<string, string>> newNames = new Dictionary<string, Dictionary<string, string>>();
			foreach(string name in visitor.ColumnNames) {
				string path, suffix;
				QueryMemberEvaluatorBase.GetPathAndSuffix(name, out path, out suffix);
				PivotGridFieldBase field = ((PivotGridData)Owner).GetFieldByNameOrDataControllerColumnName(path);
				string columnName = field == null ? path : CubeColumns.GetFieldCubeColumnsName(field);
				if(!CubeColumns.ContainsKey(columnName))
					continue;
				string parentName = columnName;
				if(CubeColumns[parentName].ParentColumn != null)
					parentName = (CubeColumns[parentName].Metadata.GetColumnHierarchy()[0]).UniqueName;
				Dictionary<string, string> dic = null;
				if(!newNames.TryGetValue(parentName, out dic)) {
					dic = new Dictionary<string, string>();
					newNames.Add(parentName, dic);
				}
				if(!dic.ContainsKey(path))
					dic.Add(path, columnName);
			}
			return newNames;
		}
		protected override OLAPCubeColumn CreateColumnCore(IQueryMetadataColumn column, PivotGridFieldBase field) {
			return new OLAPCubeColumn((OLAPMetadataColumn)column);
		}
		protected override bool EnsureFilterHelper() {
			bool res = Owner.OptionsOLAP.UsePrefilter != (FilterHelper.GetType() == typeof(OLAPCriteriaFilterHelper));
			if(res)
				FilterHelper = null;
			return base.EnsureFilterHelper() || res;
		}
		internal override List<object> QueryVisibleValues(IDataSourceHelpersOwner<OLAPCubeColumn> owner, OLAPCubeColumn column) {
			if(!Metadata.Connected)
				return new List<object>();
			column.EnsureColumnMembersLoaded();
			return base.QueryVisibleValues(owner, column);
		}
		protected override bool ChangeExpandedAll(bool isColumn, bool expanded) {
			IList<OLAPCubeColumn> area = Areas.GetArea(isColumn);
			if(expanded) {
				for(int i = 0; i < area.Count - 1; i++) {
					OLAPMetadata.EnsureMembersLoadedOnColumnExpand(area[i]);
				}
			}
			return base.ChangeExpandedAll(isColumn, expanded);
		}
		protected override bool IsUnboundExpressionValidCore(PivotGridFieldBase field) {
			return false;
		}
		internal override object[] QueryAvailableValues(IDataSourceHelpersOwner<OLAPCubeColumn> owner, OLAPCubeColumn column, bool deferUpdates, List<OLAPCubeColumn> customFilters) {
			if(!Metadata.Connected)
				return new object[0];
			column.EnsureColumnMembersLoaded();
			return base.QueryAvailableValues(owner, column, deferUpdates, customFilters);
		}
		protected override bool RaiseQueryException(QueryHandleableException raisedException) {
			if(raisedException.IsResponse && ((DevExpress.XtraPivotGrid.Data.IOLAPResponseException)raisedException.InnerException).IsQueryTimeout) {
				RaiseOLAPQueryTimeout();
				return true;
			}
			return base.RaiseQueryException(raisedException);
		}
		protected internal override string SaveColumns() {
			EnsureColumnsPopulated(false);
			return base.SaveColumns();
		}
		void CreateCubeColumnsByMetadataColumns() {
			foreach(KeyValuePair<string, MetadataColumnBase> pair in MetadataColumns)
				CubeColumns.Add(new OLAPCubeColumn((OLAPMetadataColumn)pair.Value));
		}
		protected override bool NeedRequestCalculationsFromServer(IList<AggregationLevel> calcs) {
			return base.NeedRequestCalculationsFromServer(calcs) || calcs.Any((f) => f.Any((s) => s.Any((r) => r.SummaryType.IsTopPercentage())));
		}
		internal void QueryMemberAttributes(OLAPCubeColumn column, OLAPMember[] members) {
			OLAPMetadata.QueryMembers(column, members);
		}
	}
}

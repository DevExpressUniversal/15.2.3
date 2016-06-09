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
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid;
using DevExpress.PivotGrid.QueryMode.Sorting;
namespace DevExpress.PivotGrid.QueryMode {
	public interface IQueryMetadataColumn {
		string DisplayFolder { get; }
		bool IsMeasure { get; }
		IQueryMetadataColumn ChildColumn { get; }
		IQueryMetadataColumn ParentColumn { get; }
		IQueryMetadata Owner { get; }
		bool IsParent(IQueryMetadataColumn child);
		List<IQueryMetadataColumn> GetColumnHierarchy();
		string Name { get; }
		string UniqueName { get; }
		bool? HasNullValues { get; set; }
		Type DataType { get; }
		void SaveToStream(IQueryMetadata metadata, TypedBinaryWriter writer);
		void RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader);
		Type SafeDataType { get; }
		QueryMember GetAllMember();
	}
	public abstract class QueryColumn {
		public const string MeasureFilteringNotSupported = "Measure filtering is not supported";
		#region Fields
		PivotSortOrder sortOrder;
		QueryColumn sortBySummary;
		int topValueCount;
		bool topValueShowOthers;
		PivotTopValueType topValueType;
		bool filtered;
		PivotSortMode sortMode;
		PivotSummaryType summaryType;
		readonly List<QueryMember> sortBySummaryMembers = new List<QueryMember>();
		object[] uniqueValueMembersCache;
		List<object> sortedUniqueValuesCache;
		IQueryMetadataColumn metadata;
		bool isDataSourceLevelCalculation;
		#endregion
		#region metadata
		public virtual string UniqueName {
			get { return Metadata.UniqueName; }
		}
		public IQueryMetadataColumn Metadata { get { return metadata; } }
		public string Name { get { return metadata.Name; } }
		public virtual bool IsMeasure { get { return metadata.IsMeasure; } }
		public virtual bool HasMeasureData { get { return IsMeasure; } }
		public bool SortBySummaryMembersExpanded { get; set; }
		#endregion
		#region Properties
		public virtual string ActualSortProperty { get { return null; } }
		public bool? HasNullValues { get { return Metadata.HasNullValues; } }
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set { summaryType = value; }
		}
		public PivotSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value)
					return;
				sortOrder = value;
				SortedUniqueValuesCache = null;
			}
		}
		public PivotSortMode SortMode {
			get { return sortMode; }
			set {
				if(SortMode == value)
					return;
				sortMode = value;
				SortedUniqueValuesCache = null;
			}
		}
		public QueryColumn SortBySummary {
			get { return sortBySummary; }
			set {
				if(value != null && !value.Metadata.IsMeasure)
					return;
				sortBySummary = value;
			}
		}
		public virtual bool IsSortedByAttribute {
			get {
				if(sortBySummary != null)
					return false;
				return sortMode == PivotSortMode.ID || sortMode == PivotSortMode.Key;
			}
		}
		public List<QueryMember> SortBySummaryMembers {
			get { return sortBySummaryMembers; }
		}
		public int TopValueCount {
			get { return topValueCount; }
			set {
				if(topValueCount == value)
					return;
				if(TopValueType == PivotTopValueType.Absolute && value >= 0)
					topValueCount = value;
				if(TopValueType == PivotTopValueType.Percent && value >= 0 && value <= 100)
					topValueCount = value;
				if(TopValueType == PivotTopValueType.Sum)
					topValueCount = value;
				UpdateTotalMember();
			}
		}
		public bool SortByLastEmpty { get { return TopValueCount == 0 && SortBySummary != null; } }
		public bool TopValueShowOthers { get { return topValueShowOthers; } set { topValueShowOthers = value; } }
		public PivotTopValueType TopValueType {
			get { return topValueType; }
			set {
				if(value != topValueType) {
					topValueType = value;
					if(value == PivotTopValueType.Percent && TopValueCount > 100)
						topValueCount = 100;
				}
			}
		}
		public abstract bool TopValueHiddenOthersShowedInTotal { get; }
		public bool Filtered {
			get { return filtered; }
			set {
				if(filtered == value)
					return;
				if(value && IsMeasure)
					throw new NotSupportedException(MeasureFilteringNotSupported);
				filtered = value;
				UpdateTotalMember();
			}
		}
		public bool IsDataSourceLevelCalculation {
			get { return isDataSourceLevelCalculation; }
			private set { isDataSourceLevelCalculation = value; }
		}
		public virtual bool HasCustomTotal {
			get { return TopValueCount != 0; }
		}
		QueryMember totalMember;
		public QueryMember TotalMember {
			get {
				if(totalMember == null)
					totalMember = new QueryVirtualMember(Metadata, true);
				return totalMember;
			}
		}
		public object[] UniqueValueMembersCache {
			get { return uniqueValueMembersCache; }
			set { uniqueValueMembersCache = value; }
		}
		public List<object> SortedUniqueValuesCache {
			get { return sortedUniqueValuesCache; }
			set { sortedUniqueValuesCache = value; }
		}
		#endregion
		protected internal QueryColumn(IQueryMetadataColumn metadata) {
			this.metadata = metadata;
			this.sortOrder = PivotSortOrder.Ascending;
		}
		public override string ToString() {
			return Name;
		}
		public override bool Equals(object obj) {
			QueryColumn b = obj as QueryColumn;
			if(b != null) {
				return Name == b.Name && Metadata.DataType == b.Metadata.DataType
					&& object.Equals(Metadata.ParentColumn, b.Metadata.ParentColumn);
			}
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return Name.GetHashCode();
		}
		public bool Equals(PivotGridFieldBase field) {
			return Equals(field, false);
		}
		protected virtual bool Equals(PivotGridFieldBase field, bool forceSort) {
			return
				!((field.IsColumnOrRow || forceSort) && SortOrder != field.SortOrder) &&
				SortMode == field.ActualSortMode &&
				TopValueCount == field.TopValueCount &&
				TopValueShowOthers == field.TopValueShowOthers &&
				TopValueType == field.TopValueType &&
				(field.Area != PivotArea.DataArea || SummaryType == field.SummaryType &&
													 IsDataSourceLevelCalculation == (!field.IsProcessOnSummaryLevel || field.UnboundExpressionMode == UnboundExpressionMode.UseAggregateFunctions));
		}
		public virtual void Assign(PivotGridFieldBase field, bool forceSort) {
			if(Equals(field, forceSort))
				return;
			SortOrder = field.SortOrder;
			SortMode = field.ActualSortMode;
			TopValueCount = field.TopValueCount;
			TopValueShowOthers = field.TopValueShowOthers;
			TopValueType = field.TopValueType;
			SummaryType = field.SummaryType;
			IsDataSourceLevelCalculation = !field.IsProcessOnSummaryLevel || field.UnboundExpressionMode == UnboundExpressionMode.UseAggregateFunctions;
			SortedUniqueValuesCache = null;
			UniqueValueMembersCache = null;
		}
		protected virtual void UpdateTotalMember() { }
		internal void SetMetadata(IQueryMetadataColumn metadataColumn) {
			metadata = metadataColumn;
		}
		internal abstract IComparer<TMember> GetByMemberComparer<TMember>(Func<object, string> customText) where TMember : IQueryMemberProvider;
	}
	public class NamedColumnList<T> : Dictionary<string, T> {
		readonly List<T> columnsList;
		protected List<T> ColumnsList { get { return columnsList; } }
		public NamedColumnList() {
			this.columnsList = new List<T>();
		}
		public new T this[string key] {
			get {
				if(string.IsNullOrEmpty(key))
					return default(T);
				T result;
				if(!TryGetValue(key, out result))
					return default(T);
				return result;
			}
		}
		public T this[int index] {
			get { return columnsList[index]; }
		}
		public int IndexOf(T column) {
			return columnsList.IndexOf(column);
		}
		public virtual new void Clear() {
			base.Clear();
			columnsList.Clear();
		}
	}
	public abstract class QueryColumns<TColumn> : NamedColumnList<TColumn> where TColumn : QueryColumn {
		readonly IDataSourceHelpersOwner<TColumn> owner;
		public QueryColumns(IDataSourceHelpersOwner<TColumn> owner)
			: base() {
			this.owner = owner;
		}
		public IDataSourceHelpersOwner<TColumn> Owner { get { return owner; } }
		public virtual void Add(TColumn column) {
			ColumnsList.Add(column);
			base.Add(column.UniqueName, column);
		}
		public TColumn this[PivotGridFieldBase field] {
			get {
				string key = GetFieldCubeColumnsName(field);
				if(string.IsNullOrEmpty(key))
					return null;
				TColumn result;
				if(!TryGetValue(key, out result))
					return null;
				return result;
			}
		}
		public bool TryGetValue(PivotGridFieldBase field, out TColumn result) {
			string key = GetFieldCubeColumnsName(field);
			if(string.IsNullOrEmpty(key)) {
				result = default(TColumn);
				return false;
			}
			return TryGetValue(key, out result) && result != null;
		}
		public bool ContainsKey(PivotGridFieldBase field) {
			return this[field] != null;
		}
		public void Remove(PivotGridFieldBase field) {
			Remove(GetFieldCubeColumnsName(field));
		}
		public abstract string GetFieldCubeColumnsName(PivotGridFieldBase field);
	}
}

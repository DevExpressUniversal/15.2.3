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
using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Xpo.DB;
namespace DevExpress.PivotGrid.ServerMode {
	class MetadataColumn : MetadataColumnBase, IGroupCriteriaConvertible, IDrillDownProvider, IQueryMetadataColumn {
		string columnName, alias, tableName, tableAlias;
		DBColumnType dbColumnType;
		string displayFolder, caption;
		public string ColumnName {
			get { return columnName; }
			set { columnName = value; }
		}
		public string Alias {
			get { return alias; }
			set { alias = value; }
		}
		public string TableName {
			get { return tableName; }
			set { tableName = value; }
		}
		public string TableAlias {
			get { return tableAlias; }
			set { tableAlias = value; }
		}
		public DBColumnType DBColumnType {
			get { return dbColumnType; }
			set { dbColumnType = value; }
		}
		public override string Name { get { return string.IsNullOrEmpty(alias) ? columnName : alias; } }
		public override string UniqueName { get { return Name; } }
		public override string DisplayFolder { get { return displayFolder; } }
		public override string Caption { get { return caption; } }
		public override bool IsMeasure { get { return false; } }
		internal MetadataColumn() {
		}
		public MetadataColumn(string name, string alias, string tableName, string tableAlias, Type type, string displayFolder, string caption) : base(type, null) {
			this.columnName = name;
			this.alias = alias;
			this.tableName = tableName;
			this.tableAlias = tableAlias;
			this.dbColumnType = DBColumn.GetColumnType(type, true);
			this.displayFolder = displayFolder;
			this.caption = caption;
		}
		public override string ToString() {
			return UniqueName;
		}
		protected override void SaveToStream(IQueryMetadata owner, Data.IO.TypedBinaryWriter writer) {
			base.SaveToStream(owner, writer);
			int flags = 0;
			if(ColumnName != null)
				flags = 1;
			if(Alias != null)
				flags = flags | 2;
			if(TableName != null)
				flags = flags | 4;
			if(TableAlias != null)
				flags = flags | 16;
			writer.Write((byte)flags);
			if((flags & 1) != 0)
				writer.Write(ColumnName);
			if((flags & 2) != 0)
				writer.Write(Alias);
			if((flags & 4) != 0)
				writer.Write(TableName);
			if((flags & 16) != 0)
				writer.Write(TableAlias);
			writer.Write((Int32)dbColumnType);
		}
		public override void RestoreFromStream(IQueryMetadata metadata, Data.IO.TypedBinaryReader reader) {
			base.RestoreFromStream(metadata, reader);
			int flags = reader.ReadByte();
			if((flags & 1) != 0)
				ColumnName = reader.ReadString();
			if((flags & 2) != 0)
				Alias = reader.ReadString();
			if((flags & 4) != 0)
				TableName = reader.ReadString();
			if((flags & 16) != 0)
				TableAlias = reader.ReadString();
			DBColumnType = (DBColumnType)reader.ReadInt32();
		}
		public virtual CriteriaOperator GetGroupCriteria() {
			return GetRawCriteria();
		}
		public virtual CriteriaOperator GetRawCriteria() {
			return (((ServerModeMetadata)Owner).Exec.CriteriaSyntax & CriteriaSyntax.ServerCriteria) != 0 ? (CriteriaOperator)new QueryOperand(ColumnName, TableAlias, DBColumnType) : new OperandProperty(ColumnName);
		}
		#region IDrillDownProvider
		string IDrillDownProvider.DrillDownName {
			get {
				if(!string.IsNullOrEmpty(Name))
					return Name;
				if(!string.IsNullOrEmpty(UniqueName))
					return UniqueName;
				return GetRawCriteria().ToString();
			}
		}
		string IDrillDownProvider.Name {
			get { return Name; }
		}
		Type IDrillDownProvider.DataType {
			get { return DataType; }
		}
		#endregion
		protected override QueryMember GetAllMember() {
			return new QueryVirtualMember(this, true);
		}
	}
	sealed class VirtualMetadataColumn : IQueryMetadataColumn, IGroupCriteriaConvertible, IDrillDownProvider {
		IQueryMetadataColumn column;
		IGroupCriteriaConvertible conv;
		public IQueryMetadataColumn Column { get { return column; } }
		public VirtualMetadataColumn(IQueryMetadataColumn column, IGroupCriteriaConvertible conv) {
			this.column = column;
			this.conv = conv;
		}
		public VirtualMetadataColumn(IQueryMetadataColumn column)
			: this(column, null) {
		}
		string IQueryMetadataColumn.DisplayFolder {
			get { return column.DisplayFolder; }
		}
		bool IQueryMetadataColumn.IsMeasure {
			get { return true; }
		}
		IQueryMetadataColumn IQueryMetadataColumn.ChildColumn {
			get { return column.ChildColumn; }
		}
		IQueryMetadataColumn IQueryMetadataColumn.ParentColumn {
			get { return column.ParentColumn; }
		}
		IQueryMetadata IQueryMetadataColumn.Owner {
			get { return column.Owner; }
		}
		QueryMember IQueryMetadataColumn.GetAllMember() {
			return column.GetAllMember();
		}
		bool IQueryMetadataColumn.IsParent(IQueryMetadataColumn child) {
			return column.IsParent(child);
		}
		List<IQueryMetadataColumn> IQueryMetadataColumn.GetColumnHierarchy() {
			return column.GetColumnHierarchy();
		}
		string IQueryMetadataColumn.Name {
			get { return column.Name; }
		}
		string IQueryMetadataColumn.UniqueName {
			get { return column.UniqueName; }
		}
		bool? IQueryMetadataColumn.HasNullValues {
			get {
				if(IsAggregated())
					return false;
				return column.HasNullValues;
			}
			set {
				if(IsAggregated())
					return;
				column.HasNullValues = value;
			}
		}
		bool IsAggregated() {
			ServerModeColumn serverColumn = conv as ServerModeColumn;
			if(serverColumn == null || !serverColumn.IsMeasure)
				return false;
			switch(serverColumn.SummaryType) {
				case Data.PivotGrid.PivotSummaryType.Count:
				case Data.PivotGrid.PivotSummaryType.StdDev:
				case Data.PivotGrid.PivotSummaryType.StdDevp:
				case Data.PivotGrid.PivotSummaryType.Sum:
				case Data.PivotGrid.PivotSummaryType.Var:
				case Data.PivotGrid.PivotSummaryType.Varp:
					return true;
				default:
					return false;
			}
		}
		Type IQueryMetadataColumn.DataType {
			get { return GetDataType(); }
		}
		Type GetDataType() {
			ServerModeColumn serverColumn = conv as ServerModeColumn;
			if(serverColumn != null && serverColumn.GroupInterval != XtraPivotGrid.PivotGroupInterval.Default)
				return DevExpress.PivotGrid.Utils.GroupIntervalHelper.GetValueType(serverColumn.GroupInterval);
			return column.DataType;
		}
		Type IQueryMetadataColumn.SafeDataType {
			get { return GetDataType(); }
		}
		void IQueryMetadataColumn.SaveToStream(IQueryMetadata metadata, TypedBinaryWriter writer) {
			column.SaveToStream(metadata, writer); 
		}
		void IQueryMetadataColumn.RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader) {
			column.RestoreFromStream(metadata, reader); 
		}
		internal void SetIConvertible(IGroupCriteriaConvertible column) {
			conv = column;
		}
		CriteriaOperator IGroupCriteriaConvertible.GetGroupCriteria() {
			return conv.GetGroupCriteria();
		}
		CriteriaOperator IRawCriteriaConvertible.GetRawCriteria() {
			return ((IRawCriteriaConvertible)column).GetRawCriteria();
		}
		#region IDrillDownProvider
		string IDrillDownProvider.DrillDownName {
			get { return ((IDrillDownProvider)column).DrillDownName; }
		}
		string IDrillDownProvider.Name {
			get { return ((IDrillDownProvider)column).Name; }
		}
		Type IDrillDownProvider.DataType {
			get { return ((IDrillDownProvider)column).DataType; }
		}
		#endregion
	}
}

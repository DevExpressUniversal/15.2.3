#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.DragDrop {
	public interface IDragObject {
		IDataSourceSchema DataSourceSchema { get; }
		string Caption { get; }
		int DataItemsCount { get; }
		bool IsDataField { get; }
		DragItem DragItem { get; }
		IDragSource DragSource { get; }
		bool SameDataItem(DataItem dataItem);
		bool SameDragGroup(DragGroup dragGroup);
		Measure GetMeasure(DataDashboardItem dashboardItem);
		IList<Dimension> GetDimensions();
		DragGroup GetGroup();
		bool IsGroup { get; }
	}
	public class DataFieldDragObject : IDragObject {
		readonly IDataSourceSchema dataSourceSchema;
		readonly DataField dataField;
		readonly IDragSource dragSource;
		public DataFieldDragObject(IDragSource dragSource, IDataSourceSchema dataSourceSchema, DataField dataField) {
			this.dragSource = dragSource;
			this.dataSourceSchema = dataSourceSchema;
			this.dataField = dataField;
		}
		OlapHierarchyDataField HierarchyDataField { get { return dataField as OlapHierarchyDataField; } }
		public DataField DataField { get { return dataField; } }
		IDataSourceSchema IDragObject.DataSourceSchema { get { return dataSourceSchema; } }
		bool IDragObject.IsDataField { get { return true; } }
		string IDragObject.Caption { get { return dataField.Caption; } }
		int IDragObject.DataItemsCount { get { return HierarchyDataField != null ? HierarchyDataField.GroupDataMembers.Count : 1; } }
		DragItem IDragObject.DragItem { get { return null; } }
		IDragSource IDragObject.DragSource { get { return dragSource; } }
		bool IDragObject.SameDataItem(DataItem dataItem) {
			return false;
		}
		bool IDragObject.SameDragGroup(DragGroup group) {
			return false;
		}
		bool IDragObject.IsGroup { get { return false; } }
		Measure IDragObject.GetMeasure(DataDashboardItem dashboardItem) {
			Measure measure;
			OlapDataField olapDataField = dataField as OlapDataField;
			if (olapDataField != null) {
				measure = new Measure(dataField.DataMember);
				switch (olapDataField.NodeType) {
					case DataNodeType.OlapKpi:
					case DataNodeType.OlapMeasure:
						if (olapDataField.DefaultNumericFormat != null) {
							measure.NumericFormat.CurrencyCultureName = olapDataField.DefaultNumericFormat.CurrencyCultureName;
							measure.NumericFormat.FormatType = olapDataField.DefaultNumericFormat.FormatType;
							measure.NumericFormat.IncludeGroupSeparator = olapDataField.DefaultNumericFormat.IncludeGroupSeparator;
							measure.NumericFormat.Precision = olapDataField.DefaultNumericFormat.Precision;
						}
						return measure;
					default:
						return null;
				}
			}
			else
				measure = new Measure(dataField.DataMember);
			DashboardWinHelper.SetCorrectSummaryType(measure, dataSourceSchema);
			return measure;
		}
		IList<Dimension> IDragObject.GetDimensions() {
			OlapDataField olapDataField = dataField as OlapDataField;
			if(olapDataField != null) {
				switch(olapDataField.NodeType){
					case DataNodeType.OlapDimension:
						return new List<Dimension> { new Dimension(dataField.DataMember) };
					case DataNodeType.OlapHierarchy:
						List<Dimension> result = new List<Dimension>();
						foreach(string dataMember in HierarchyDataField.GroupDataMembers) {
							Dimension dimension = new Dimension(dataMember, HierarchyDataField.GroupIndex);
							result.Add(dimension);
						}
						return result;
					default:
						return null;
				}
			}
			if(dataSourceSchema != null && dataSourceSchema.IsAggregateCalcField(dataField.DataMember))
				return null;
			return new List<Dimension> { new Dimension(dataField.DataMember) };
		}
		DragGroup IDragObject.GetGroup() {
			return null;
		}
	}
	public class DataItemDragObject : IDragObject {
		readonly DataItem dataItem;
		readonly string caption;
		readonly IDataSourceSchema dataSourceSchema;
		readonly DragItem dragItem;
		public DataItemDragObject(DragItem dragItem) {
			this.dragItem = dragItem;
			this.dataItem = dragItem.DataItem;
			this.caption = dragItem.ActualCaption;
			this.dataSourceSchema = dragItem.DataSourceSchema;
		}
		IDataSourceSchema IDragObject.DataSourceSchema { get { return dataSourceSchema; } }
		bool IDragObject.IsDataField { get { return false; } }
		string IDragObject.Caption { get { return caption; } }
		int IDragObject.DataItemsCount { get { return 1; } }
		public bool IsMeasure { get { return (dataItem as Measure) != null; } }
		public DataFieldType FieldType { get { return dataSourceSchema != null ? dataSourceSchema.GetFieldType(dataItem.DataMember) : DataFieldType.Unknown; } }
		DragItem IDragObject.DragItem { get { return dragItem; } }
		IDragSource IDragObject.DragSource { get { return dragItem.Group; } }
		bool IDragObject.SameDataItem(DataItem dataItem) {
			return this.dataItem == dataItem;
		}
		bool IDragObject.SameDragGroup(DragGroup group) {
			return dragItem.Group == group;
		}
		bool IDragObject.IsGroup { get { return false; } }
		Measure IDragObject.GetMeasure(DataDashboardItem dashboardItem) {
			Measure measure = DashboardWinHelper.ConvertToMeasure(dataItem);
			DashboardWinHelper.SetCorrectSummaryType(measure, dataSourceSchema);
			return measure;
		}
		IList<Dimension> IDragObject.GetDimensions() {
			Dimension dimension = dataItem as Dimension;
			if(dimension != null)
				return new List<Dimension> { dimension };
			return new List<Dimension> { new Dimension(dataItem.DataMember) };
		}
		DragGroup IDragObject.GetGroup() {
			return dragItem.Group;
		}
	}
	public class OlapDimensionDragObject : IDragObject {
		readonly Dimension dimension;
		readonly string caption;
		readonly IDataSourceSchema dataSourceSchema;
		readonly DragItem dragItem;
		public OlapDimensionDragObject(DragItem dragItem) {
			this.dragItem = dragItem;
			this.dimension = (Dimension)dragItem.DataItem;
			this.caption = dragItem.ActualCaption;
			this.dataSourceSchema = dragItem.DataSourceSchema;
		}
		IDataSourceSchema IDragObject.DataSourceSchema { get { return dataSourceSchema; } }
		bool IDragObject.IsDataField { get { return false; } }
		string IDragObject.Caption { get { return caption; } }
		int IDragObject.DataItemsCount { get { return 1; } }
		DragItem IDragObject.DragItem { get { return dragItem; } }
		IDragSource IDragObject.DragSource { get { return dragItem.Group; } }
		bool IDragObject.SameDataItem(DataItem dataItem) {
			return dimension == dataItem;
		}
		bool IDragObject.SameDragGroup(DragGroup group) {
			return dragItem.Group == group;
		}
		bool IDragObject.IsGroup { get { return false; } }
		Measure IDragObject.GetMeasure(DataDashboardItem dashboardItem) {
			return null;
		}
		IList<Dimension> IDragObject.GetDimensions() {
			return new List<Dimension> { dimension };
		}
		DragGroup IDragObject.GetGroup() {
			return dragItem.Group;
		}
	}
	public class OlapMeasureDragObject : IDragObject {
		readonly Measure measure;
		readonly string caption;
		readonly IDataSourceSchema dataSourceSchema;
		readonly DragItem dragItem;
		public OlapMeasureDragObject(DragItem dragItem) {
			this.dragItem = dragItem;
			this.measure = (Measure)dragItem.DataItem;
			this.caption = dragItem.ActualCaption;
			this.dataSourceSchema = dragItem.DataSourceSchema;
		}
		IDataSourceSchema IDragObject.DataSourceSchema { get { return dataSourceSchema; } }
		bool IDragObject.IsDataField { get { return false; } }
		string IDragObject.Caption { get { return caption; } }
		int IDragObject.DataItemsCount { get { return 1; } }
		DragItem IDragObject.DragItem { get { return dragItem; } }
		IDragSource IDragObject.DragSource { get { return dragItem.Group; } }
		bool IDragObject.SameDataItem(DataItem dataItem) {
			return measure == dataItem;
		}
		bool IDragObject.SameDragGroup(DragGroup group) {
			return dragItem.Group == group;
		}
		bool IDragObject.IsGroup { get { return false; } }
		Measure IDragObject.GetMeasure(DataDashboardItem dashboardItem) {
			return measure;
		}
		IList<Dimension> IDragObject.GetDimensions() {
			return null;
		}
		DragGroup IDragObject.GetGroup() {
			return dragItem.Group;
		}
	}
	public class OlapHierarchyDragObject : IDragObject {
		readonly List<Dimension> dimensions;
		readonly string caption;
		readonly IDataSourceSchema dataSourceSchema;
		readonly DragItem dragItem;
		public OlapHierarchyDragObject(DragItem dragItem) {
			this.dragItem = dragItem;
			dimensions = new List<Dimension>(dragItem.DataItems.Count);
			foreach(Dimension dim in dragItem.DataItems)
				dimensions.Add(dim);
			this.caption = dragItem.ActualCaption;
			this.dataSourceSchema = dragItem.DataSourceSchema;
		}
		IDataSourceSchema IDragObject.DataSourceSchema { get { return dataSourceSchema; } }
		bool IDragObject.IsDataField { get { return false; } }
		string IDragObject.Caption { get { return caption; } }
		int IDragObject.DataItemsCount { get { return dimensions.Count; } }
		DragItem IDragObject.DragItem { get { return dragItem; } }
		IDragSource IDragObject.DragSource { get { return dragItem.Group; } }
		bool IDragObject.SameDataItem(DataItem dataItem) {
			return dimensions[0] == dataItem;
		}
		bool IDragObject.SameDragGroup(DragGroup group) {
			return dragItem.Group == group;
		}
		bool IDragObject.IsGroup { get { return false; } }
		Measure IDragObject.GetMeasure(DataDashboardItem dashboardItem) {
			return null;
		}
		IList<Dimension> IDragObject.GetDimensions() {
			return dimensions;
		}
		DragGroup IDragObject.GetGroup() {
			return dragItem.Group;
		}
	}
	public class GroupDragObject : IDragObject {
		readonly IDataSourceSchema dataSourceSchema;
		readonly DragGroup group;
		public GroupDragObject(DragGroup group) {
			this.dataSourceSchema = group.ItemList[0].DataSourceSchema;
			this.group = group;
		}
		IDataSourceSchema IDragObject.DataSourceSchema { get { return dataSourceSchema; } }
		bool IDragObject.IsDataField { get { return false; } }
		string IDragObject.Caption { get { return String.Empty; } }
		int IDragObject.DataItemsCount { get { return 1; } }
		public DragGroup Group { get { return group; } }
		DragItem IDragObject.DragItem { get { return null; } }
		IDragSource IDragObject.DragSource { get { return group; } }
		bool IDragObject.SameDataItem(DataItem dataItem) {
			foreach(DragItem item in group.Items)
				if(item.DataItem == dataItem)
					return true;
			return false;
		}
		bool IDragObject.SameDragGroup(DragGroup group) { return this.group == group; }
		bool IDragObject.IsGroup { get { return true; } }
		Measure IDragObject.GetMeasure(DataDashboardItem dashboardItem) { return null; }
		IList<Dimension> IDragObject.GetDimensions() { return null; }
		DragGroup IDragObject.GetGroup() { return group; }
	}
}

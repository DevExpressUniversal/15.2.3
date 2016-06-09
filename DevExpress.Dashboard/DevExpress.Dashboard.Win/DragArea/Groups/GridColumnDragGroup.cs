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
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardWin.Native {
	public class GridColumnDragGroup : HolderCollectionDragGroup<GridColumnBase> {
		public bool AutoMode { get; set; }
		public GridColumnDragGroup(string optionsButtonImageName, GridColumnBase column, bool autoMode)
			: base(optionsButtonImageName, column) {
			AutoMode = autoMode;
		}
		protected override void SetOptionsButtonState(DragAreaButtonState state) {
			GridDimensionColumn dimensionColumn = Holder as GridDimensionColumn;
			if(!IsNewGroup && dimensionColumn != null && DataSource != null && DataSource.GetIsOlap())
				base.SetOptionsButtonState(DragAreaButtonState.Disabled);
			else
				base.SetOptionsButtonState(state);
		}
		IList<GridColumnBase> CreateGridColumns(IDragObject dragObject) {
			if (ItemList.Count > 1)
				return null;
			if (dragObject.DataItemsCount > 1) {
				IList<GridColumnBase> result = new List<GridColumnBase>();
				for (int i = 0; i < dragObject.DataItemsCount; i++) {
					result.Add(new GridDimensionColumn());
				}
				return result;
			}
			DataFieldDragObject dataField = dragObject as DataFieldDragObject;
			if (dataField != null)
				return CreateGridColumnsFromDataField(dataField);
			if ((dragObject as OlapMeasureDragObject) != null)
				return new List<GridColumnBase> { new GridMeasureColumn() };
			if ((dragObject as OlapDimensionDragObject) != null)
				return new List<GridColumnBase> { new GridDimensionColumn() };
			DataItemDragObject dataItemDragObject = dragObject as DataItemDragObject;
			if (dataItemDragObject != null)
				return CreateGridColumnsFromDataItemDragObject(dataItemDragObject);
			return null;
		}
		IList<GridColumnBase> CreateGridColumnsFromDataItemDragObject(DataItemDragObject dataItemDragObject) {
			if (AutoMode) {
				if (dataItemDragObject.IsMeasure)
					return new List<GridColumnBase> { new GridMeasureColumn() };
				else {
					GridDimensionColumn column = new GridDimensionColumn();
					if (dataItemDragObject.FieldType == DataFieldType.Custom)
						column.DisplayMode = GridDimensionColumnDisplayMode.Image;
					return new List<GridColumnBase> { column };
				}
			}
			return null;
		}
		IList<GridColumnBase> CreateGridColumnsFromDataField(DataFieldDragObject dataFieldDragObject) {
			OlapDataField olapDataField = dataFieldDragObject.DataField as OlapDataField;
			IDragObject dragObject = dataFieldDragObject;
			if (olapDataField != null) {
				IList<GridColumnBase> holders = base.CreateHolders(dataFieldDragObject);
				if (AutoMode || holders.Count > 1)
					return dragObject.GetMeasure(Section.Area.DashboardItem) != null ? 
							new List<GridColumnBase> { new GridMeasureColumn() } : new List<GridColumnBase> { new GridDimensionColumn() };
				return holders;
			}
			if(dragObject.DataSourceSchema != null && dragObject.DataSourceSchema.IsAggregateCalcField(dataFieldDragObject.DataField.DataMember))
				return new List<GridColumnBase> { new GridMeasureColumn() };
			DataFieldType fieldType = dataFieldDragObject.DataField.FieldType;
			if (AutoMode) {
				if (fieldType.IsNumericType())
					return new List<GridColumnBase> { new GridMeasureColumn() };
				else {
					GridDimensionColumn column = new GridDimensionColumn();
					if (fieldType == DataFieldType.Custom)
						column.DisplayMode = GridDimensionColumnDisplayMode.Image;
					return new List<GridColumnBase> { column };
				}
			}
			return null;
		}
		protected override string GetActualCaption(string caption) {
			return AutoMode ? DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAuto) : caption;
		}
		public override IList<GridColumnBase> CreateHolders(IDragObject dragObject) {
			IList<GridColumnBase> columns = CreateGridColumns(dragObject);
			return columns ?? base.CreateHolders(dragObject);
		}
		public override IList<DataItem> CreateDataItems(GridColumnBase targetColumn, IDragObject dragObject, int itemIndex) {
			IList<DataItem> dataItems = base.CreateDataItems(targetColumn, dragObject, itemIndex);
			GridDimensionColumn dimensionColumn = targetColumn as GridDimensionColumn;
			if(dimensionColumn != null)
				dataItems = new List<DataItem>(dragObject.GetDimensions());
			return dataItems;
		}
		protected override bool AcceptableDragObject(IDragObject dragObject) {
			if(dragObject.IsGroup || (!dragObject.IsGroup && AutoMode))
				return true;
			GridDimensionColumn dimensionColumn = Holder as GridDimensionColumn;
			if(dimensionColumn != null)
				return dragObject.GetDimensions() != null;
			return base.AcceptableDragObject(dragObject);
		}
	}
}

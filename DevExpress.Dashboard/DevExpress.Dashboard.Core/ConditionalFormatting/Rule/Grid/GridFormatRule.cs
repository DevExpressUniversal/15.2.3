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

using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon {
	public class GridItemFormatRuleCollection : FormatRuleCollection<GridItemFormatRule> {
		public GridItemFormatRuleCollection() : base() {
		}
		protected override IFormatRuleCollection CreateInstance() {
			return new GridItemFormatRuleCollection();
		}
	}
	public class GridItemFormatRule : CellsItemFormatRule {
		static DataItem GetDataItem(GridColumnBase column) {
			GridMeasureColumn measureColumn = column as GridMeasureColumn;
			if(measureColumn != null)
				return measureColumn.Measure;
			else {
				GridDimensionColumn dimensionColumn = column as GridDimensionColumn;
				return (dimensionColumn != null) ? dimensionColumn.Dimension : null;
			}
		}
		protected internal override bool EnableLevel { get { return true; } }
		public GridItemFormatRule()
			: this((DataItem)null) {
		}
		public GridItemFormatRule(GridColumnBase column) : this(column, null) {
		}
		public GridItemFormatRule(GridColumnBase column, GridColumnBase columnApplyTo)
			: this(GetDataItem(column), GetDataItem(columnApplyTo)) {
		}
		public GridItemFormatRule(DataItem item)
			: this(item, null) {
		}
		public GridItemFormatRule(DataItem item, DataItem itemApplyTo)
			: base(item, itemApplyTo) {
		}
		protected internal override DashboardItemFormatRule CreateInstance() {
			return new GridItemFormatRule();
		}
		protected internal override FormatRuleModelBase CreateModelInternal() {
			string applyByDataId = null;
			string applyToDataId = null;
			if(IsEvaluatorRequiredCondition) {
				Dimension dimension = GetAxisItem(false);
				if(dimension != null)
					applyByDataId = dimension.ActualId;
				applyToDataId = applyByDataId;
				if(LevelCore.ItemApplyTo != null)
					applyToDataId = LevelCore.ItemApplyTo.ActualId;
				else if(LevelCore.Item != null)
					applyToDataId = LevelCore.Item.ActualId;
			}
			else {
				applyByDataId = LevelCore.Item.ActualId;
				applyToDataId = LevelCore.ItemApplyTo.ActualId;
			}
			return new GridFormatRuleModel() {
				CalcByDataId = applyByDataId,
				ApplyToDataId = applyToDataId,
				ApplyToRow = ApplyToRow
			};
		}
		protected override Dimension GetAxisItem(bool isSecond) {
			if(isSecond) return null;
			IList<Dimension> axisItems = Context.GetAxisItems(isSecond);
			Dimension filterAxisPoint = FilterByEvaluatorRequiredDataMembers(axisItems);
			if(filterAxisPoint != null)
				return filterAxisPoint;
			bool checkDataItem = DataItem is Measure || IsEvaluatorRequiredCondition;
			return (axisItems.Count != 0 && checkDataItem) ? axisItems[axisItems.Count - 1] : null;
		}
		protected Dimension FilterByEvaluatorRequiredDataMembers(IList<Dimension> axisItems) {
			IEvaluatorRequired evaluatorRequired = Condition as IEvaluatorRequired;
			if(evaluatorRequired != null) {
				IEnumerable<string> filterDataMembers = evaluatorRequired.GetDataMembers();
				IList<Measure> measures = Context.GetMeasures();
				if(measures.Select(item => item.ActualId).Intersect(filterDataMembers).Count() > 0)
					return null;
				return FilterByEvaluatorRequiredDataMembers(axisItems, filterDataMembers);
			}
			return null;
		}
	}
}

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
using System.Text;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
namespace DevExpress.XtraPivotGrid {
	public interface IThreadSafeField {
		PivotArea Area { get; }
		int AreaIndex { get; }
		bool Visible { get; }
		string Name { get; }
		string FieldName { get; }
		string UnboundFieldName { get; }
		string PrefilterColumnName { get; }
		string Caption { get; }
		string DisplayFolder { get; }
		PivotSummaryType SummaryType { get; }
		PivotSortOrder SortOrder { get; }
		PivotSortMode SortMode { get; }
		PivotGridAllowedAreas AllowedAreas { get; }
		int GroupIndex { get; }
		IThreadSafeGroup Group { get; }
		bool TopValueShowOthers { get; }
		int TopValueCount { get; }
		bool RunningTotal { get; }
		bool ShowNewValues { get; }
		PivotTopValueType TopValueType { get; }
		PivotGroupInterval GroupInterval { get; }
		int GroupIntervalNumericRange { get; }
		string UnboundExpression { get; }
		bool ExpandedInFieldsGroup { get; }
		UnboundColumnType UnboundType { get; }
		Type DataType { get; }
		object Tag { get; }
	}
	public interface IThreadSafeFieldCollection {
		IThreadSafeField this[int index] { get; }
		IThreadSafeField this[string fieldName] { get; }
		int Count { get; }
		int GetVisibleFieldCount(PivotArea area);
	}
	public interface IThreadSafeGroup {
		int Index { get; }
		PivotArea Area { get; }
		string Caption { get; }
		string Hierarhcy { get; }
		bool Visible { get; }
		int VisibleCount { get; }
		IThreadSafeFieldCollection Fields { get; }
	}
	public interface IThreadSafeGroupCollection {
		IThreadSafeGroup this[int index] { get; }
		int Count { get; }
	}
	public interface IThreadSafeAccessible {
		IThreadSafeFieldCollection Fields { get; }
		IThreadSafeGroupCollection Groups { get; }
		int ColumnCount { get; }
		int RowCount { get; }
		IThreadSafeField GetFieldByArea(PivotArea area, int index);
		IThreadSafeField GetFieldByLevel(bool isColumn, int level);
		List<IThreadSafeField> GetFieldsByArea(PivotArea area);
		int GetFieldCountByArea(PivotArea area);
		string GetFieldValueDisplayText(IThreadSafeField field, int lastLevelIndex);
		string GetCellDisplayText(int columnIndex, int rowIndex);
		bool IsAsyncInProgress { get; }
	}
}

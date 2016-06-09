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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	public interface PivotLocation {
		Range Range { get; }
		Range PageRange { get; }
		Range ColumnRange { get; }
		Range RowRange { get; }
		Range DataRange { get; }
		Range WholeRange { get; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Utils;
	partial class NativePivotTable {
		Model.PivotTableLocation ModelLocation { get { return modelPivotTable.Location; } }
		#region PivotLocation Members
		public PivotLocation Location { get { return this; } }
		Range PivotLocation.Range {
			get { 
				CheckValid();
				return new NativeRange(ModelLocation.Range, nativeWorksheet);
			}
		}
		Range PivotLocation.PageRange {
			get {
				CheckValid();
				return CreateRangeCore(ModelLocation.PageFieldsRange);
			}
		}
		Range PivotLocation.ColumnRange {
			get {
				CheckValid();
				return CreateRangeCore(ModelLocation.TryGetColumnRange());
			}
		}
		Range PivotLocation.RowRange {
			get {
				CheckValid();
				Model.CellRange modelRange = ModelLocation.TryGetRowRange(modelPivotTable.RowFields.Count, modelPivotTable.DataFields.Count);
				return CreateRangeCore(modelRange);
			}
		}
		Range PivotLocation.DataRange {
			get {
				CheckValid();
				return CreateRangeCore(ModelLocation.TryGetDataRange());
			}
		}
		Range PivotLocation.WholeRange {
			get {
				CheckValid();
				return new NativeRange(ModelLocation.WholeRange, nativeWorksheet);
			}
		}
		NativeRange CreateRangeCore(Model.CellRangeBase modelRange) {
			if (modelRange == null)
				return null;
			return new NativeRange(modelRange, nativeWorksheet);
		}
		#endregion
	}
}

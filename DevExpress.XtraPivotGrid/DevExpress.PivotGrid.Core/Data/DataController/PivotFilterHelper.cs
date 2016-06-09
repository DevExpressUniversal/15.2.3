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

using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using System;
using DevExpress.XtraPivotGrid.Customization;
namespace DevExpress.Data.PivotGrid {
	public static class PivotFilterHelper {
		public static object[] GetUniqueColumnValues(PivotDataController controller, int column, bool includeFilteredOut, bool showBlanks) {
			return new FilterHelper(controller, controller.VisibleListSourceRows).GetUniqueColumnValues(column, -1, includeFilteredOut, false, null, showBlanks);
		}
		public static object[] GetUniqueAvailableColumnValues(PivotDataController controller, int column, bool deferUpdates) {
			return new AvailableValuesFilterHelper(controller).GetUniqueAvailableColumnValues(column, deferUpdates);
		}
	}
	class AvailableValuesFilterHelper : FilterHelper {
		int[] filteredRows;
		int count;
		public AvailableValuesFilterHelper(DataControllerBase controller)
			: base(controller, null) {
		}
		protected override int GetValuesCount(bool includeFilteredOut) {
			return this.count;
		}
		protected override int GetRow(bool includeFilteredOut, int index) {
			return this.filteredRows[index];
		}
		public object[] GetUniqueAvailableColumnValues(int column, bool deferUpdates) {
			this.filteredRows = ((PivotDataController)Controller).GetFilteredRows(column, out this.count, deferUpdates);
			return GetUniqueColumnValues(column, -1, false, false, null);
		}
	}
}

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
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Xpf.Grid;
namespace DevExpress.Xpf.Data {
	public interface IDataProviderEvents {
		object GetUnboundData(int listSourceRowIndex, string fieldName, object value);
		void SetUnboundData(int listSourceRowIndex, string fieldName, object value);
		void OnCustomSummaryExists(object sender, CustomSummaryExistEventArgs e);
		void OnCustomSummary(object sender, CustomSummaryEventArgs e);
		int? OnCompareSortValues(int listSourceRowIndex1, int listSourceRowIndex2, object value1, object value2, DataColumnInfo column, ColumnSortOrder sortOrder);
		int? OnCompareGroupValues(int listSourceRowIndex1, int listSourceRowIndex2, object value1, object value2, DataColumnInfo column);
		ExpressiveSortInfo.Cell GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order);
		ExpressiveSortInfo.Cell GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType);
		bool? OnCustomRowFilter(int listSourceRowIndex, bool fit);
		bool OnShowingGroupFooter(int rowHandle, int level);
		void OnBeforeSorting();
		void OnAfterSorting();
		void OnBeforeGrouping();
		void OnAfterGrouping();
		void SubstituteFilter(SubstituteFilterEventArgs args);
		void SubstituteSortInfo(SubstituteSortInfoEventArgs args);
	}
}

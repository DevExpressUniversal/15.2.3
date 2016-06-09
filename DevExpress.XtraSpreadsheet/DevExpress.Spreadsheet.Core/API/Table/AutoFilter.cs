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
using DevExpress.Office;
using System.Collections.Generic;
namespace DevExpress.Spreadsheet {
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	#region usings
	using DevExpress.Utils;
	using System.Collections.Generic;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
	using ModelDefinedNameCollection = DevExpress.XtraSpreadsheet.Model.DefinedNameCollection;
	using ModelDefinedNameDase = DevExpress.XtraSpreadsheet.Model.DefinedNameBase;
	using ModelStyleSheet = DevExpress.XtraSpreadsheet.Model.StyleSheet;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorkbookDataContext = DevExpress.XtraSpreadsheet.Model.WorkbookDataContext;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Office.Utils;
	#endregion
	public interface ExcelAutoFilter {
		void ApplyFilter();
		bool FilterMode { get; }
		ExcelAutoFilterCollection Filters { get; }
		Range Range { get; }
		void ShowAllData();
		TableSort Sort { get; }
	}
	public interface ExcelFilter {
		int Count { get; }
		object Criteria1 { get; }
		object Criteria2 { get; }
		bool On { get; }
		ExcelAutoFilterOperator Operator { get; set; }
	}
	public enum ExcelAutoFilterOperator {
		And = 1,		  
		Or,			   
		Top10Items,	   
		Bottom10Items,	
		Top10Percent,	 
		Bottom10Percent,  
		FilterCellColor,  
		FilterValues,	 
		FilterFontColor,  
		FilterIcon,	   
		FilterDynamic	 
	}
	public interface ExcelAutoFilterCollection : ISimpleCollection<ExcelFilter> {
	}
}

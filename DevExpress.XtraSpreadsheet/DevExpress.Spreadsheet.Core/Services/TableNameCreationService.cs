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
using System.Text;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office.Utils;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Services {
	#region ITableNameCreationService
	public interface ITableNameCreationService {
		string Prefix { get; set; }
		string GetNewNameForTableCopy(IList<string> existingTablesNames, string tableName);
		string GetNewTableName(IList<string> existingTablesNames);
	}
	#endregion
	#region ITableStyleNameCreationService
	public interface ITableStyleNameCreationService {
		string GetDuplicateTableStyleName(TableStyle style);
	}
	#endregion
	#region IPivotStyleNameCreationService
	public interface IPivotStyleNameCreationService {
		string GetDuplicatePivotStyleName(TableStyle style);
	}
	#endregion
	#region ICellStyleNameCreationService
	public interface ICellStyleNameCreationService {
		string GetDuplicateStyleName(string existingStyleName, IList<string> targetStyleNames);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Services.Implementation {
	#region TableNameCreationService
	public class TableNameCreationService : ITableNameCreationService {
		string currentPrefix;
		public TableNameCreationService() {
			this.currentPrefix = "Table";
		}
		public string Prefix { get { return currentPrefix; } set { currentPrefix = value; } }
		public string GetNewNameForTableCopy(IList<string> existingTablesNames, string tableName) {
			return tableName + existingTablesNames.Count.ToString();
		}
		public string GetNewTableName(IList<string> existingTablesNames) {
			string result = NextObjectNameGenerator.GetNameForNextObject(Prefix, existingTablesNames);
			return result;
		}
	}
	#endregion
	#region TableStyleNameCreationService
	public class TableStyleNameCreationService : ITableStyleNameCreationService {
		public string GetDuplicateTableStyleName(TableStyle style) {
			IList<string> targetStyleNames = style.DocumentModel.StyleSheet.TableStyles.GetExistingCustomTableStyleNames();
			return NextObjectNameGenerator.GetNameForDuplicate(style.Name.Name, targetStyleNames);
		}
	}
	#endregion
	#region PivotStyleNameCreationService
	public class PivotStyleNameCreationService : IPivotStyleNameCreationService {
		public string GetDuplicatePivotStyleName(TableStyle style) {
			IList<string> targetStyleNames = style.DocumentModel.StyleSheet.TableStyles.GetExistingCustomPivotStyleNames();
			return NextObjectNameGenerator.GetNameForDuplicate(style.Name.Name, targetStyleNames);
		}
	}
	#endregion
	#region CellStyleNameCreationService
	public class CellStyleNameCreationService : ICellStyleNameCreationService {
		public string GetDuplicateStyleName(string existingStyleName, IList<string> targetStyleNames) {
			return NextObjectNameGenerator.GetNameForDuplicate(existingStyleName, targetStyleNames);
		}
	}
	#endregion
}

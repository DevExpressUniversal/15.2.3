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
namespace DevExpress.XtraReports.Data {
	public static class SelectQueryBuilder {
		public static string BuildSelectCommand(string tableName, string tableSchemaName, string[] columnNames, string prefixLiteral, string suffixLiteral) {
			string wrappedTable = WrapObjectName(tableName, prefixLiteral, suffixLiteral);
			string objectName = !String.IsNullOrEmpty(tableSchemaName) ?
				string.Format("{0}.{1}", WrapObjectName(tableSchemaName, prefixLiteral, suffixLiteral), wrappedTable) : wrappedTable;
			return string.Format("Select {0} from {1}", BuildCoumns(columnNames, prefixLiteral, suffixLiteral), objectName);
		}
		static string BuildCoumns(string[] columnNames, string prefixLiteral, string suffixLiteral) {
			if(columnNames == null || columnNames.Length == 0)
				return "*";
			StringBuilder columnsBuilder = new StringBuilder(WrapObjectName(columnNames[0], prefixLiteral, suffixLiteral));
			for(int i = 1; i < columnNames.Length; i++) {
				columnsBuilder.Append(", ");
				columnsBuilder.Append(WrapObjectName(columnNames[i], prefixLiteral, suffixLiteral));
			}
			return columnsBuilder.ToString();
		}
		static string WrapObjectName(string name, string prefixLiteral, string suffixLiteral) {
			return string.Format("{0}{1}{2}", prefixLiteral, name, suffixLiteral);
		}
	}
}

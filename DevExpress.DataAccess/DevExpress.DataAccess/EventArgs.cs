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
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DataAccess {
	public class CustomFilterExpressionEventArgs : EventArgs {
		public string DataSourceComponentName { get; private set; }
		public string DataSourceName { get; private set; }
		public CriteriaOperator FilterExpression { get; set; }
		public string TableName { get; private set; }
		public CustomFilterExpressionEventArgs(string dataSourceComponentName, string dataSourceName, string tableName) {
			DataSourceComponentName = dataSourceComponentName;
			DataSourceName = dataSourceName;
			TableName = tableName;
		}
	}
	public delegate void CustomFilterExpressionEventHandler(object sender, CustomFilterExpressionEventArgs e);
	public class ValidateCustomSqlQueryEventArgs : EventArgs {
		public ValidateCustomSqlQueryEventArgs(CustomSqlQuery customSqlQuery, bool valid) {
			CustomSqlQuery = customSqlQuery;
			Valid = valid;
		}
		public CustomSqlQuery CustomSqlQuery { get; private set; }
		public bool Valid { get; set; }
		public string ExceptionMessage { get; set; }
	}
	public delegate void ValidateCustomSqlQueryEventHandler(object sender, ValidateCustomSqlQueryEventArgs e);
}

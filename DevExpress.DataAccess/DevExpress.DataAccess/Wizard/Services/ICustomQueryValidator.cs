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

using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
namespace DevExpress.DataAccess.Wizard.Services {
	public interface ICustomQueryValidator {
		bool Validate(DataConnectionParametersBase connectionParameters, string sql, ref string message);
	}
	public class CustomQueryValidator : ICustomQueryValidator {
		public virtual bool Validate(DataConnectionParametersBase connectionParameters, string sql, ref string message) {
			var result = CustomSqlQuery.IsSqlValid(sql);
			if(!result)
				message = DataAccessLocalizer.GetString(DataAccessStringId.CustomSqlQueryValidationException);
			return result;
		}
	}
	public static class QueryValidatorHelper {
		public static void Validate(this SqlQuery query, ICustomQueryValidator customQueryValidator, DataConnectionParametersBase dataConnectionParametersBase, DBSchema dbSchema) {
			Guard.ArgumentNotNull(query, "query");
			var customSqlQuery = query as CustomSqlQuery;
			if (customSqlQuery != null && customQueryValidator != null) {
				string message = null;
				if (!customQueryValidator.Validate(dataConnectionParametersBase, customSqlQuery.Sql, ref message)) {
					throw new CustomSqlQueryValidationException(message);
				}
			}
			else
				query.Validate(dbSchema);
		}
	}
}

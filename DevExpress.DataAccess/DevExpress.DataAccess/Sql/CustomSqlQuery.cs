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
using System.ComponentModel;
using System.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
#if !DXPORTABLE
using DevExpress.CodeParser;
using DevExpress.DataAccess.Native.Sql.SqlParser;
#endif
namespace DevExpress.DataAccess.Sql {
	public sealed class CustomSqlQuery : SqlQuery {
		class CustomSqlTypeConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if(sourceType == typeof(string))
					return false;
				return base.CanConvertFrom(context, sourceType);
			}
		}
		string sql;
		[Obsolete("Use CustomSqlQuery(string name, string sql) instead.")]
		public CustomSqlQuery(string sql) : this(string.Empty, sql) { }
		public CustomSqlQuery(string name, string sql) : base(name) { this.sql = sql; }
		public CustomSqlQuery() : this(string.Empty, string.Empty) { }
		internal CustomSqlQuery(CustomSqlQuery other) : base(other) { this.sql = other.sql; }
		[TypeConverter(typeof(CustomSqlTypeConverter))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.CustomSqlQuery.Sql")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("CustomSqlQuerySql")]
#endif
		[LocalizableCategory(DevExpress.DataAccess.Localization.DataAccessStringId.QueryPropertyGridCustomSqlCategoryName)]
		public string Sql { get { return sql; } set { sql = value; } }
		public override void Validate() {
			if(string.IsNullOrEmpty(this.sql)) {
				throw new SqlStringEmptyValidationException();
			}
#if !DXPORTABLE
			bool valid = IsSqlValid(sql);
			ValidateCustomSqlQueryEventArgs args = new ValidateCustomSqlQueryEventArgs(this, valid);
			if(!valid)
				args.ExceptionMessage = DataAccessLocalizer.GetString(DataAccessStringId.CustomSqlQueryValidationException);
			if(Owner != null && Owner.DataSource != null)
				Owner.DataSource.RaiseValidateCustomSqlQuery(args);
			if (!args.Valid)
				throw new CustomSqlQueryValidationException(args.ExceptionMessage); 
#endif
		}
#if !DXPORTABLE
		internal static bool IsSqlValid(string sql) {
			if(SqlDataSource.DisableCustomQueryValidation) 
				return true;
			string toLowersql = sql.ToLowerInvariant();
			return CheckSelectStatement(toLowersql) && CheckSeveralStatements(toLowersql) && CheckBlackList(toLowersql);
		}
		static bool CheckSelectStatement(string sql) {
			using (SourceStringReader sourceStringReader = new SourceStringReader(sql)) {
				using (CommonSqlScanner sqlScanner = new CommonSqlScanner(sourceStringReader)) {
					Token token = sqlScanner.Scan();
					while (token.Type != Tokens.EOF) {
						if(token.Type == Tokens.COMMENT)
							token = sqlScanner.Scan();
						else return token.Type == Tokens.SELECT;
					}
				}
			}
			return false;
		}
		static bool CheckSeveralStatements(string sql) {
			bool first = true;
			using (SourceStringReader sourceStringReader = new SourceStringReader(sql)) {
				using (CommonSqlScanner sqlScanner = new CommonSqlScanner(sourceStringReader)) {
					Token token = sqlScanner.Scan();
					while (token.Type != Tokens.EOF) {
						if (!first && token.Type != Tokens.COMMENT) {
							return false;
						}
						if (token.Type == Tokens.SEMICOLON) {
							first = false;
						}
						token = sqlScanner.Scan();
					}
				}
				return true;
			}
		}
		static readonly int[] blackList = {Tokens.DROP, Tokens.INTO, Tokens.UPDATE, Tokens.DELETE};
		static bool CheckBlackList(string sql) {
			using (SourceStringReader sourceStringReader = new SourceStringReader(sql)) {
				using (CommonSqlScanner sqlScanner = new CommonSqlScanner(sourceStringReader)) {
					Token token = sqlScanner.Scan();
					while (token.Type != Tokens.EOF) {
						if(blackList.Contains(token.Type))
							return false;
						token = sqlScanner.Scan();
					}
				}
			}
			return true;
		}
#endif
		public override void Validate(DBSchema schema) {
			Validate();
		}
		internal override bool EqualsCore(SqlQuery obj) {
			CustomSqlQuery other = (CustomSqlQuery)obj;
			return string.Equals(this.sql, other.sql, StringComparison.Ordinal);
		}
		internal override SqlQuery Clone() { return new CustomSqlQuery(this); }
	}
}

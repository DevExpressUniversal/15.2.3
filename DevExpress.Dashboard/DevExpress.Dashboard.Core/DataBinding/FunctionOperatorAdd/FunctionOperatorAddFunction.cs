#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.Xpo.DB;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public abstract class DashboardInternalCustomFunction : ICustomFunctionOperatorBrowsable {
		object ICustomFunctionOperator.Evaluate(params object[] operands) { return null; }
		public abstract string Name { get; }
		public abstract Type ResultType(params Type[] operands);
		FunctionCategory ICustomFunctionOperatorBrowsable.Category { get { return FunctionCategory.All; } }
		string ICustomFunctionOperatorBrowsable.Description { get { return string.Empty; } }
		int ICustomFunctionOperatorBrowsable.MaxOperandCount { get { return 1; } }
		int ICustomFunctionOperatorBrowsable.MinOperandCount { get { return 2; } }
		bool ICustomFunctionOperatorBrowsable.IsValidOperandCount(int count) { return true; }
		bool ICustomFunctionOperatorBrowsable.IsValidOperandType(int operandIndex, int operandCount, Type type) { return true; }
	}
	class FunctionOperatorAddFunction : DashboardInternalCustomFunction, ICustomFunctionOperatorFormattable {
		protected static Dictionary<Type, ISqlGeneratorFormatterAdd> Formatters;
		static FunctionOperatorAddFunction() {
			Formatters = new Dictionary<Type, ISqlGeneratorFormatterAdd>(new ProviderTypeEqualityComparer()) {
					{typeof(MSSqlConnectionProvider), new MSSQLFormatterAdd()}
#if !DXPORTABLE
				   ,{typeof(AccessConnectionProvider), new AccessFormatterAdd()}, 
					{typeof(AdvantageConnectionProvider), new AdvantageFormatterAdd()}, 
					{typeof(AsaConnectionProvider), new AsaFormatterAdd()}, 
					{typeof(AseConnectionProvider), new AseFormatterAdd()}, 
					{typeof(DB2ConnectionProvider), new DB2FormatterAdd()},
					{typeof(FirebirdConnectionProvider), new FirebirdFormatterAdd()},
					{typeof(MSSqlCEConnectionProvider), new MSSQLCEFormatterAdd()}, 
					{typeof(MySqlConnectionProvider), new MySqlFormatterAdd()}, 
					{typeof(ODPConnectionProvider), new OracleFormatterAdd()}, 
					{typeof(ODPManagedConnectionProvider), new OracleFormatterAdd()}, 
					{typeof(OracleConnectionProvider), new OracleFormatterAdd()}, 
					{typeof(PervasiveSqlConnectionProvider), new PervasiveFormatterAdd()},
					{typeof(PostgreSqlConnectionProvider), new PostgresFormatterAdd()}, 
					{typeof(SQLiteConnectionProvider), new SQLiteFormatterAdd()},
					{typeof(DataAccessTeradataConnectionProvider), new TeradataFormatterAdd()},
					{typeof(DataAccessBigQueryConnectionProvider), new BigQueryFormatterAdd()},
					{typeof(VistaDBConnectionProviderBase), new VistaDBFormatterAdd()}
#endif
			};
		}
		readonly FunctionOperatorTypeAdd operatorType;
		internal FunctionOperatorAddFunction(FunctionOperatorTypeAdd operatorType) {
			this.operatorType = operatorType;
		}
		internal FunctionOperatorTypeAdd OperatorType { get { return operatorType; } }
		public override string Name { get { return FunctionOperatorAdd.GetFunctionName(OperatorType); } }
		string ICustomFunctionOperatorFormattable.Format(Type providerType, params string[] operands) {
			ISqlGeneratorFormatterAdd formatter = null;
			if(Formatters.TryGetValue(providerType, out formatter)) {
				return formatter.FormatFunctionAdd(OperatorType, operands);
			}
			throw new NotSupportedException();
		}
		public override Type ResultType(params Type[] operands) {
			switch(OperatorType) {
				case FunctionOperatorTypeAdd.GetDateHour:					
				case FunctionOperatorTypeAdd.GetDateHourMinute:
				case FunctionOperatorTypeAdd.GetDateHourMinuteSecond:
				case FunctionOperatorTypeAdd.GetDateMonthYear:
				case FunctionOperatorTypeAdd.GetDateQuarterYear:
					return typeof(DateTime);
				case FunctionOperatorTypeAdd.GetQuarter:
				case FunctionOperatorTypeAdd.GetWeekOfYear:
					return typeof(int);
				default:
					return typeof(object);
			}
		}
	}
	class ProviderTypeEqualityComparer : IEqualityComparer<Type> {
		bool IEqualityComparer<Type>.Equals(Type x, Type y) {
			return x.IsSubclassOf(y) || y.IsSubclassOf(x) || x == y;
		}
		int IEqualityComparer<Type>.GetHashCode(Type obj) {
			Type baseProviderType = typeof(ConnectionProviderSql);
			Type inheritingProviderType =
				obj.GetBaseType() == baseProviderType ? obj :
				obj.GetBaseType().GetBaseType() == baseProviderType ? obj.GetBaseType() :
				obj.GetBaseType().GetBaseType().GetBaseType() == baseProviderType ? obj.GetBaseType().GetBaseType() : null;
			return inheritingProviderType == null ? 1 : inheritingProviderType.GetHashCode();
		}
	}
}

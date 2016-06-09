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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode.Queryable {
	abstract class EntityCriteriaHelper {
		static bool? IsDoubleOrDecimal(Type type) {
			TypeCode code = DXTypeExtensions.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type);
			switch(code) {
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Decimal:
					return false;
				default:
					return null;
			}
		}
		public const string SystemDataEntityAssemblyName = "System.Data.Entity";
		protected static Assembly GetAsssembly(string assemblyName) {
#if DXPORTABLE
			return null;
#else
			Assembly assembly = null;
			try {
				assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((asm) => asm.FullName.StartsWith(assemblyName));
			} catch { }
			if(assembly == null)
				Assembly.LoadFile(assemblyName);
			return assembly;
#endif
		}
		public static EntityCriteriaHelper Create(Assembly assembly) {
			if(assembly.FullName.StartsWith(EntityCriteriaHelper.SystemDataEntityAssemblyName))
				return new Entity4CriteriaHelper(assembly);
			else
				if(assembly.GetName().Version.Major >= 6)
					return new Entity6CriteriaHelper(assembly);
				else
					return new Entity5CriteriaHelper();
		}
		public const string TruncateTime = "TruncateTime", DatePart = "DatePart", CreateTime = "CreateTime", AddDays = "AddDays", AddHours = "AddHours", AddMilliseconds = "AddMilliseconds",
							AddSeconds = "AddSeconds", AddMinutes = "AddMinutes", AddMonths = "AddMonths", AddYears = "AddYears", DiffDays = "DiffDays", DiffHours = "DiffHours",
							DiffMilliSeconds = "DiffMilliSeconds", DiffMinutes = "DiffMinutes", DiffMonths = "DiffMonths", DiffSeconds = "DiffSeconds", DiffYears = "DiffYears",
							StringConvert = "StringConvert", SquareRoot = "SquareRoot";
		Dictionary<string, EntityMethod> recs = new Dictionary<string, EntityMethod>();
		class EntityMethod {
			readonly string name;
			readonly Type target;
			protected readonly Type[] typeArguments;
			public EntityMethod(string name, Type target, params Type[] typeArguments) {
				this.name = name;
				this.target = target;
				this.typeArguments = typeArguments;
			}
			public virtual Expression CreateCall(Expression[] arguments, EntityCriteriaHelper helper) {
				return CreateCallCore(arguments, typeArguments, helper);
			}
			protected Expression CreateCallCore(Expression[] arguments, Type[] typeArguments, EntityCriteriaHelper helper) {
				if(arguments.Length != typeArguments.Length)
					throw new ArgumentException(string.Format("EntityCriteriaHelper.{0} arguments.Length {1} : {2}", name, arguments.Length, typeArguments.Length));
				for(int i = 0; i < typeArguments.Length; i++)
					arguments[i] = CriteriaToExpressionConverterBase.ChangeType(arguments[i], typeArguments[i], helper, false, true);
				return Expression.Call(target, name, new Type[0], arguments);
			}
		}
		class AutoDoubleDecimalTypeEntityMethod : EntityMethod {
			public AutoDoubleDecimalTypeEntityMethod(string name, Type target, params Type[] typeArguments)
				: base(name, target, typeArguments) {
			}
			public override Expression CreateCall(Expression[] arguments, EntityCriteriaHelper helper) {
				Type[] types = new Type[typeArguments.Length];
				Array.Copy(typeArguments, types, types.Length);
				types[0] = IsDoubleOrDecimal(arguments[0].Type) == false ? typeof(decimal?) : typeof(double?);
				return CreateCallCore(arguments, types, helper);
			}
		}
		Assembly assembly;
		public int? DateFirst { get; set; }
		protected abstract Type DBFunctionsType { get; }
		protected abstract Type SqlFunctionsType { get; }
		public bool ListContainsSupported { get { return true; } }
		protected EntityCriteriaHelper(Assembly assembly) {
			this.assembly = assembly;
		}
		public Expression CreateMethodCall(string name, params Expression[] arguments) {
			if(recs.Count == 0)
				PopulateRecords(recs);
			EntityMethod method;
			return recs.TryGetValue(name, out method) ? method.CreateCall(arguments, this) : null;
		}
		public Expression ConvertExpressionToString(Expression condition) {
			if(IsDoubleOrDecimal(condition.Type).HasValue)
				return Expression.Call(CreateMethodCall(EntityCriteriaHelper.StringConvert, condition, Expression.Constant((int?)19, typeof(int?))),
					typeof(string).GetMethod("Trim", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null));
			else
				return condition;
		}
		public Expression ConvertFunctionOperator(FunctionOperator theOperator, Func<CriteriaOperator, Expression> func) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.AddDays:
					return CallEF(EntityCriteriaHelper.AddDays, theOperator.Operands, func);
				case FunctionOperatorType.AddHours:
					return CallEF(EntityCriteriaHelper.AddHours, theOperator.Operands, func);
				case FunctionOperatorType.AddMilliSeconds:
					return CallEF(EntityCriteriaHelper.AddMilliseconds, theOperator.Operands, func);
				case FunctionOperatorType.AddSeconds:
					return CallEF(EntityCriteriaHelper.AddSeconds, theOperator.Operands, func);
				case FunctionOperatorType.AddMinutes:
					return CallEF(EntityCriteriaHelper.AddMinutes, theOperator.Operands, func);
				case FunctionOperatorType.AddMonths:
					return CallEF(EntityCriteriaHelper.AddMonths, theOperator.Operands, func);
				case FunctionOperatorType.AddYears:
					return CallEF(EntityCriteriaHelper.AddYears, theOperator.Operands, func);
				case FunctionOperatorType.GetDate:
					return CallEF(EntityCriteriaHelper.TruncateTime, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffDay:
					return CallEF(EntityCriteriaHelper.DiffDays, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffHour:
					return CallEF(EntityCriteriaHelper.DiffHours, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffMilliSecond:
					return CallEF(EntityCriteriaHelper.DiffMilliSeconds, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffMinute:
					return CallEF(EntityCriteriaHelper.DiffMinutes, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffMonth:
					return CallEF(EntityCriteriaHelper.DiffMonths, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffSecond:
					return CallEF(EntityCriteriaHelper.DiffSeconds, theOperator.Operands, func);
				case FunctionOperatorType.DateDiffYear:
					return CallEF(EntityCriteriaHelper.DiffYears, theOperator.Operands, func);
				case FunctionOperatorType.GetDayOfYear:
					return CallEF(EntityCriteriaHelper.DatePart, func, new OperandValue("dayofyear"), theOperator.Operands[0]);
				case FunctionOperatorType.GetDayOfWeek: {
						Expression date = CallEF(EntityCriteriaHelper.DatePart, func, new OperandValue("dw"), theOperator.Operands[0]);
						return Expression.Modulo(!DateFirst.HasValue || DateFirst == 0 ? date : Expression.Add(date, Expression.Constant(DateFirst, date.Type)), Expression.Constant(7, date.Type));
					}
				case FunctionOperatorType.GetTimeOfDay: {
						CriteriaOperator op = theOperator.Operands[0];
						OperandValue val = op as OperandValue;
						if(!ReferenceEquals(null, val))
							return Expression.Constant(Convert.ToDateTime(val.Value).TimeOfDay);
						else
							return CallEF(EntityCriteriaHelper.CreateTime, func,
																		 new FunctionOperator(FunctionOperatorType.GetHour, theOperator.Operands[0]),
																		 new FunctionOperator(FunctionOperatorType.GetMinute, theOperator.Operands[0]),
																		 new FunctionOperator(FunctionOperatorType.GetSecond, theOperator.Operands[0]));
					}
				case FunctionOperatorType.Sqr:
					return CallEF(EntityCriteriaHelper.SquareRoot, theOperator.Operands, func);
				default:
					return null;
			}
		}
		Expression CallEF(string name, params Expression[] operands) {
			return CreateMethodCall(name, operands);
		}
		Expression CallEF(string name, Func<CriteriaOperator, Expression> func, params CriteriaOperator[] operands) {
			return CallEF(name, OperandsToExpressions(operands, func));
		}
		Expression CallEF(string name, IEnumerable<CriteriaOperator> operands, Func<CriteriaOperator, Expression> func) {
			return CallEF(name, OperandsToExpressions(operands, func));
		}
		Expression[] OperandsToExpressions(IEnumerable<CriteriaOperator> operands, Func<CriteriaOperator, Expression> func) {
			return operands.Select((op) => func(op)).ToArray();
		}
		void PopulateRecords(Dictionary<string, EntityMethod> recs) {
			recs.Add(TruncateTime, new EntityMethod(TruncateTime, DBFunctionsType, typeof(DateTime?)));
			recs.Add(CreateTime, new EntityMethod(CreateTime, DBFunctionsType, typeof(int?), typeof(int?), typeof(double?)));
			recs.Add(AddDays, new EntityMethod(AddDays, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(AddHours, new EntityMethod(AddHours, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(AddMilliseconds, new EntityMethod(AddMilliseconds, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(AddSeconds, new EntityMethod(AddSeconds, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(AddMinutes, new EntityMethod(AddMinutes, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(AddMonths, new EntityMethod(AddMonths, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(AddYears, new EntityMethod(AddYears, DBFunctionsType, typeof(DateTime?), typeof(int?)));
			recs.Add(DiffDays, new EntityMethod(DiffDays, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			recs.Add(DiffHours, new EntityMethod(DiffHours, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			recs.Add(DiffMilliSeconds, new EntityMethod(DiffMilliSeconds, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			recs.Add(DiffMinutes, new EntityMethod(DiffMinutes, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			recs.Add(DiffMonths, new EntityMethod(DiffMonths, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			recs.Add(DiffSeconds, new EntityMethod(DiffSeconds, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			recs.Add(DiffYears, new EntityMethod(DiffYears, DBFunctionsType, typeof(DateTime?), typeof(DateTime?)));
			if(SqlFunctionsType != null) {
				recs.Add(DatePart, new EntityMethod(DatePart, SqlFunctionsType, typeof(string), typeof(DateTime?)));
				recs.Add(StringConvert, new AutoDoubleDecimalTypeEntityMethod(StringConvert, SqlFunctionsType, typeof(double?), typeof(int?)));
				recs.Add(SquareRoot, new AutoDoubleDecimalTypeEntityMethod(SquareRoot, SqlFunctionsType, typeof(decimal?)));
			}
		}
		public void EnsureServerConstants(IEnumerable<CriteriaOperator> criterias, IQueryable queryable, IPivotCriteriaToExpressionConverter basic) {
			if(DateFirst != null)
				return;
			if(!HasDayOfWeekChecker.Check(criterias))
				return;
			IQueryable q = queryable.MakeGroupBy(basic, new OperandValue(0));
			DateFirst = Convert.ToInt32(LinqExpressionHelper.DoSelectSeveral(
													q,
													Expression.Parameter(q.ElementType, ""),
													new List<Expression>() {
															 basic.Convert(
																				Expression.Parameter(q.ElementType, ""), 
																				new FunctionOperator(FunctionOperatorType.GetDayOfWeek, new OperandValue(new DateTime(2000, 1, 2))))
														   }, true).ToArray()[0][0]) + 5;
			if(DateFirst < 0)
				DateFirst += 7;
			if(DateFirst > 6)
				DateFirst -= 7;
		}
	}
	class Entity4CriteriaHelper : EntityCriteriaHelper {
		const string EntityFunctionsTypeName = "System.Data.Objects.EntityFunctions";
		const string SqlFunctionsTypeName = "System.Data.Objects.SqlClient.SqlFunctions";
		readonly Type entityFunctionsType;
		readonly Type sqlFunctionsType;
		protected override Type DBFunctionsType {
			get { return entityFunctionsType; }
		}
		protected override Type SqlFunctionsType {
			get { return sqlFunctionsType; }
		}
		public Entity4CriteriaHelper(Assembly assembly)
			: base(assembly) {
			entityFunctionsType = assembly.GetType(EntityFunctionsTypeName);
			sqlFunctionsType = assembly.GetType(SqlFunctionsTypeName);
		}
	}
	class Entity5CriteriaHelper : Entity4CriteriaHelper {
		public Entity5CriteriaHelper() : base(GetAsssembly(SystemDataEntityAssemblyName)) { }
	}
	class Entity6CriteriaHelper : EntityCriteriaHelper {
		const string DbFunctionsTypeName = "System.Data.Entity.DbFunctions";
		const string SqlFunctionsTypeName = "System.Data.Entity.SqlServer.SqlFunctions";
		const string EntityAssemblyName = "EntityFramework";
		const string SqlServerAssemblyName = "EntityFramework.SqlServer";
		readonly Type dbFunctions;
		readonly Type sqlFunctionsType;
		protected override Type DBFunctionsType {
			get { return dbFunctions; }
		}
		protected override Type SqlFunctionsType {
			get { return sqlFunctionsType; }
		}
		public Entity6CriteriaHelper(Assembly assembly)
			: base(assembly) {
			dbFunctions = assembly.GetType(DbFunctionsTypeName);
			string sqlServerAssemblyFullName = assembly.FullName.Replace(EntityAssemblyName, SqlServerAssemblyName);
			Assembly sqlServerAssembly = null;
			try {
				sqlServerAssembly = GetAsssembly(sqlServerAssemblyFullName);
				sqlFunctionsType = sqlServerAssembly.GetType(SqlFunctionsTypeName);
			} catch { }
		}
	}
}

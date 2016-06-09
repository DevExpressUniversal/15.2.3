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

using System.Data;
using System;
using System.Reflection;
namespace DevExpress.Xpo.DB {	
	using DevExpress.Xpo.DB.Helpers;
	using DevExpress.Xpo.DB.Exceptions;
	using DevExpress.Data.Filtering;	
	using System.Collections.Generic;
	public class ODPConnectionProvider : BaseOracleConnectionProvider {
		public const string XpoProviderTypeString = "ODP";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "Oracle.DataAccess.Client.OracleException");
				return helper;
			}
		}
		public static string GetConnectionString(string server, string userid, string password) {
			return String.Format("{3}={4};Data Source={0};user id={1}; password={2};", server, userid, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new ODPConnectionProvider(connection, autoCreateOption);
		}
		static ODPConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("Oracle.DataAccess.Client.OracleConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
		}
		public static void Register() { }
		object oracleDbTypeVarchar2;
		object oracleDbTypeNVarchar2;
		object oracleDbTypeChar;
		SetPropertyValueDelegate setOracleDbType;
		GetPropertyValueDelegate getOracleDbType;
		SetPropertyValueDelegate setOracleCommandBindByName;
		readonly bool IsODP10;
		public ODPConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			Type odpConnectionType = connection.GetType();
			AssemblyName name = new AssemblyName(odpConnectionType.Assembly.FullName);
			IsODP10 = name.Version.Major > 9 
				|| ((name.Version.Major == 2 || name.Version.Major == 4) && name.Version.Minor >= 100);
		}
		protected override void PrepareDelegates() {
			Type oracleParameterType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleParameter");
			Type oracleDbTypeType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleDbType");
			Type oracleCommandType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleCommand");
			oracleDbTypeVarchar2 = Enum.Parse(oracleDbTypeType, "Varchar2");
			oracleDbTypeNVarchar2 = Enum.Parse(oracleDbTypeType, "NVarchar2");
			oracleDbTypeChar = Enum.Parse(oracleDbTypeType, "Char");
			ReflectConnectionHelper.CreatePropertyDelegates(oracleParameterType, "OracleDbType", out setOracleDbType, out getOracleDbType);
			setOracleCommandBindByName = ReflectConnectionHelper.CreateSetPropertyDelegate(oracleCommandType, "BindByName");
		}
		public static bool VarcharParameterFixEnabled = true;
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			if(value is Enum) param.Value = 0;
			if(value is Guid) {
				param.Value = value.ToString();
				setOracleDbType(param, oracleDbTypeChar);
			} else {
				param.Value = value;
			}
			param.ParameterName = name;
			if(VarcharParameterFixEnabled) {
				if(object.Equals(getOracleDbType(param), oracleDbTypeVarchar2))  
					setOracleDbType(param, oracleDbTypeNVarchar2);
			}
			return param;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.Boolean:
					return (bool)clientValue ? 1 : 0;
				case TypeCode.Byte:
					return (short)(byte)clientValue;
				case TypeCode.UInt16:
					return (int)(UInt16)clientValue;
				case TypeCode.SByte:
					return (int)(sbyte)clientValue;
				case TypeCode.UInt32:
					return (long)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (decimal)(UInt64)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override bool IsConnectionBroken(Exception e) {
			object numberObject;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out numberObject)) {
				int number = (int)numberObject;
				if(number == 0x311b || number == 3114 || number == 12152) {
					Connection.Close();
					return true;
				}
			}
			return base.IsConnectionBroken(e);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object numberObject;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out numberObject)) {
				int number = (int)numberObject;
				if(number == 0x388 || number == 0x3ae || number == 0x1996)
					return new SchemaCorrectionNeededException(e);
				if(number == 0x8f4 || number == 1)
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
			}
			return base.WrapException(e, query);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		public override IDbCommand CreateCommand() {
			IDbCommand cmd = base.CreateCommand();
			setOracleCommandBindByName(cmd, true);
			return cmd;
		}
		static public IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("Oracle.DataAccess", "Oracle.DataAccess.Client.OracleConnection", connectionString);
		}
		protected override bool IsFieldTypesNeeded { get { return true; } }
		ReflectionGetValuesHelperBase getValuesHelper;
		private ReflectionGetValuesHelperBase GetValuesHelper {
			get {
				if(getValuesHelper == null) {
					Type oracleDataReaderType = ConnectionHelper.GetType("Oracle.DataAccess.Client.OracleDataReader");
					Type oracleDecimalType = ConnectionHelper.GetType("Oracle.DataAccess.Types.OracleDecimal");
					getValuesHelper = (ReflectionGetValuesHelperBase)Activator.CreateInstance(typeof(ReflectionGetValuesHelper<,>).MakeGenericType(oracleDataReaderType, oracleDecimalType));
				}
				return getValuesHelper;
			}
		}
		protected override void GetValues(IDataReader reader, Type[] fieldTypes, object[] values) {
			if(GetValuesHelper.GetValues(reader, fieldTypes, values))
				return;
			base.GetValues(reader, fieldTypes, values);
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(IsODP10) {
				if(commandBuilderDeriveParametersHandler == null) {
					commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("Oracle.DataAccess", "Oracle.DataAccess.Client.OracleCommandBuilder");
				}
				commandBuilderDeriveParametersHandler(command);
			} else
				throw new NotSupportedException();
		}
		protected override SelectedData ExecuteSprocParametrized(string sprocName, params OperandValue[] parameters) {
			if(IsODP10) {
				return base.ExecuteSprocParametrized(sprocName, parameters);
			}
			using(IDbCommand command = CreateCommand()) {
				string[] listParams = new string[parameters.Length];
				for(int i = 0; i < parameters.Length; i++) {
					SprocParameter sprocParameter = (SprocParameter)parameters[i];
					if(sprocParameter.Direction != SprocParameterDirection.Input && sprocParameter.Direction != SprocParameterDirection.InputOutput)
						continue;
					listParams[i] = sprocParameter.ParameterName;
					command.Parameters.Add(CreateParameter(command, sprocParameter.Value, listParams[i]));
				}
				command.CommandText = string.Format("call \"{0}\"({1})", sprocName, string.Join(", ", listParams));
				List<SelectStatementResult> selectStatementResults = GetSelectedStatmentResults(command);
				return new SelectedData(selectStatementResults.ToArray());
			}
		}
		protected override SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			if(IsODP10) {
				return base.ExecuteSproc(sprocName, parameters);
			}
			using(IDbCommand command = CreateCommand()) {
				string[] listParams = new string[parameters.Length];
				for(int i = 0; i < parameters.Length; i++) {
					bool createParam = true;
					listParams[i] = GetParameterName(parameters[i], i, ref createParam);
					command.Parameters.Add(CreateParameter(command, parameters[i].Value, listParams[i]));
				}
				command.CommandText = string.Format("call \"{0}\"({1})", sprocName, string.Join(", ", listParams));
				List<SelectStatementResult> selectStatementResults = GetSelectedStatmentResults(command);
				return new SelectedData(selectStatementResults.ToArray());
			}
		}
	}
	public class ODPManagedConnectionProvider : BaseOracleConnectionProvider {
		public const string XpoProviderTypeString = "ODPManaged";
		ReflectConnectionHelper helper;
		ReflectConnectionHelper ConnectionHelper {
			get {
				if(helper == null)
					helper = new ReflectConnectionHelper(Connection, "Oracle.ManagedDataAccess.Client.OracleException");
				return helper;
			}
		}
		public ODPManagedConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
		}
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = CreateConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new ODPManagedConnectionProvider(connection, autoCreateOption);
		}
		public static string GetConnectionString(string server, string userid, string password) {
			return String.Format("{3}={4};Data Source={0};user id={1}; password={2};", server, userid, password, DataStoreBase.XpoProviderTypeParameterName, XpoProviderTypeString);
		}
		public static IDbConnection CreateConnection(string connectionString) {
			return ReflectConnectionHelper.GetConnection("Oracle.ManagedDataAccess", "Oracle.ManagedDataAccess.Client.OracleConnection", connectionString);
		}
		protected override IDbConnection CreateConnection() {
			return ConnectionHelper.GetConnection(ConnectionString);
		}
		static ODPManagedConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("Oracle.ManagedDataAccess.Client.OracleConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
		}
		public static void Register() { }
		object oracleDbTypeVarchar2;
		object oracleDbTypeNVarchar2;
		object oracleDbTypeChar;
		SetPropertyValueDelegate setOracleDbType;
		GetPropertyValueDelegate getOracleDbType;
		SetPropertyValueDelegate setOracleCommandBindByName;
		protected override void PrepareDelegates() {
			Type oracleParameterType = ConnectionHelper.GetType("Oracle.ManagedDataAccess.Client.OracleParameter");
			Type oracleDbTypeType = ConnectionHelper.GetType("Oracle.ManagedDataAccess.Client.OracleDbType");
			Type oracleCommandType = ConnectionHelper.GetType("Oracle.ManagedDataAccess.Client.OracleCommand");
			oracleDbTypeVarchar2 = Enum.Parse(oracleDbTypeType, "Varchar2");
			oracleDbTypeNVarchar2 = Enum.Parse(oracleDbTypeType, "NVarchar2");
			oracleDbTypeChar = Enum.Parse(oracleDbTypeType, "Char");
			ReflectConnectionHelper.CreatePropertyDelegates(oracleParameterType, "OracleDbType", out setOracleDbType, out getOracleDbType);
			setOracleCommandBindByName = ReflectConnectionHelper.CreateSetPropertyDelegate(oracleCommandType, "BindByName");
		}
		public override IDbCommand CreateCommand() {
			IDbCommand cmd = base.CreateCommand();
			setOracleCommandBindByName(cmd, true);
			return cmd;
		}
		public static bool VarcharParameterFixEnabled = true;
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			IDataParameter param = CreateParameter(command);
			if(value is Enum) param.Value = 0;
			if(value is Guid) {
				param.Value = value.ToString();
				setOracleDbType(param, oracleDbTypeChar);
			} else {
				param.Value = value;
			}
			param.ParameterName = name;
			if(VarcharParameterFixEnabled) {
				if(object.Equals(getOracleDbType(param), oracleDbTypeVarchar2))  
					setOracleDbType(param, oracleDbTypeNVarchar2);
			}
			return param;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.Boolean:
					return (bool)clientValue ? 1 : 0;
				case TypeCode.Byte:
					return (short)(byte)clientValue;
				case TypeCode.UInt16:
					return (int)(UInt16)clientValue;
				case TypeCode.SByte:
					return (int)(sbyte)clientValue;
				case TypeCode.UInt32:
					return (long)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (decimal)(UInt64)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override bool IsConnectionBroken(Exception e) {
			object numberObject;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out numberObject)) {
				int number = (int)numberObject;
				if(number == 0x311b || number == 3114 || number == 12152) {
					Connection.Close();
					return true;
				}
			}
			return base.IsConnectionBroken(e);
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			object numberObject;
			if(ConnectionHelper.TryGetExceptionProperty(e, "Number", out numberObject)) {
				int number = (int)numberObject;
				if(number == 0x388 || number == 0x3ae || number == 0x1996)
					return new SchemaCorrectionNeededException(e);
				if(number == 0x8f4 || number == 1)
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
			}
			return base.WrapException(e, query);
		}
		protected override bool IsFieldTypesNeeded { get { return true; } }
		ReflectionGetValuesHelperBase getValuesHelper;
		private ReflectionGetValuesHelperBase GetValuesHelper {
			get {
				if(getValuesHelper == null) {
					Type oracleDataReaderType = ConnectionHelper.GetType("Oracle.ManagedDataAccess.Client.OracleDataReader");
					Type oracleDecimalType = ConnectionHelper.GetType("Oracle.ManagedDataAccess.Types.OracleDecimal");
					getValuesHelper = (ReflectionGetValuesHelperBase)Activator.CreateInstance(typeof(ReflectionGetValuesHelper<,>).MakeGenericType(oracleDataReaderType, oracleDecimalType));
				}
				return getValuesHelper;
			}
		}
		protected override void GetValues(IDataReader reader, Type[] fieldTypes, object[] values) {
			if(GetValuesHelper.GetValues(reader, fieldTypes, values))
				return;
			base.GetValues(reader, fieldTypes, values);
		}
		ExecMethodDelegate commandBuilderDeriveParametersHandler;
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
			if(commandBuilderDeriveParametersHandler == null) {
				commandBuilderDeriveParametersHandler = ReflectConnectionHelper.GetCommandBuilderDeriveParametersDelegate("Oracle.ManagedDataAccess", "Oracle.ManagedDataAccess.Client.OracleCommandBuilder");
			}
			commandBuilderDeriveParametersHandler(command);
		}
	}
}
namespace DevExpress.Xpo.DB.Helpers {
	class ReflectionGetValuesHelperBase {
		public virtual bool GetValues(IDataReader reader, Type[] fieldTypes, object[] values) {
			return false;
		}
	}
	class ReflectionGetValuesHelper<R, OD> : ReflectionGetValuesHelperBase where R : IDataReader, IDataRecord {
		static readonly OD oracleDecimalMax;
		static readonly OD oracleDecimalMin;
		static readonly OD oracleDecimalZero;
		static readonly OD oracleDecimalTen;
		static readonly OD oracleDecimalMagicMantissaMax;
		static readonly OracleDataReaderGetOracleDecimalDelegate getOracleDecimal;
		static readonly OracleDecimalComparisonDelegate oDGreaterThan;
		static readonly OracleDecimalComparisonDelegate oDEquals;
		static readonly OracleDecimalOperationWithInt oDLog;
		static readonly OracleDecimalOperationWithInt oDTruncate;
		static readonly OracleDecimalOperationWithInt oDRound;
		static readonly OracleDecimalOperationWithInt oDPow;
		static readonly OracleDecimalOperation oDAbs;
		static readonly OracleDecimalOperation2 oDDivide;
		static readonly OracleDecimalToDouble oDToDouble;
		static readonly OracleDecimalToDecimal oDToDecimal;
		static ReflectionGetValuesHelper() {
			Type oracleDecimalType = typeof(OD);
			oracleDecimalMax = (OD)Activator.CreateInstance(oracleDecimalType, Decimal.MaxValue);
			oracleDecimalMin = (OD)Activator.CreateInstance(oracleDecimalType, Decimal.MinValue);
			oracleDecimalZero = (OD)Activator.CreateInstance(oracleDecimalType, Decimal.Zero);
			oracleDecimalTen = (OD)Activator.CreateInstance(oracleDecimalType, 10);
			oracleDecimalMagicMantissaMax = (OD)Activator.CreateInstance(oracleDecimalType, Decimal.MaxValue / (decimal)Math.Pow(10, 28));
			MethodInfo mi = typeof(R).GetMethod("GetOracleDecimal", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
			getOracleDecimal = (OracleDataReaderGetOracleDecimalDelegate)Delegate.CreateDelegate(typeof(OracleDataReaderGetOracleDecimalDelegate), null, mi);
			mi = oracleDecimalType.GetMethod("GreaterThan", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, oracleDecimalType }, null);
			oDGreaterThan = (OracleDecimalComparisonDelegate)Delegate.CreateDelegate(typeof(OracleDecimalComparisonDelegate), mi);
			mi = oracleDecimalType.GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, oracleDecimalType }, null);
			oDEquals = (OracleDecimalComparisonDelegate)Delegate.CreateDelegate(typeof(OracleDecimalComparisonDelegate), mi);
			mi = oracleDecimalType.GetMethod("Log", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, typeof(int) }, null);
			oDLog = (OracleDecimalOperationWithInt)Delegate.CreateDelegate(typeof(OracleDecimalOperationWithInt), mi);
			mi = oracleDecimalType.GetMethod("Truncate", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, typeof(int) }, null);
			oDTruncate = (OracleDecimalOperationWithInt)Delegate.CreateDelegate(typeof(OracleDecimalOperationWithInt), mi);
			mi = oracleDecimalType.GetMethod("Round", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, typeof(int) }, null);
			oDRound = (OracleDecimalOperationWithInt)Delegate.CreateDelegate(typeof(OracleDecimalOperationWithInt), mi);
			mi = oracleDecimalType.GetMethod("Pow", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, typeof(int) }, null);
			oDPow = mi == null ?  null : (OracleDecimalOperationWithInt)Delegate.CreateDelegate(typeof(OracleDecimalOperationWithInt), mi);
			mi = oracleDecimalType.GetMethod("Abs", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType }, null);
			oDAbs = (OracleDecimalOperation)Delegate.CreateDelegate(typeof(OracleDecimalOperation), mi);
			mi = oracleDecimalType.GetMethod("Divide", BindingFlags.Public | BindingFlags.Static, null, new Type[] { oracleDecimalType, oracleDecimalType }, null);
			oDDivide = (OracleDecimalOperation2)Delegate.CreateDelegate(typeof(OracleDecimalOperation2), mi);
			MethodInfo[] miList = oracleDecimalType.GetMethods(BindingFlags.Public | BindingFlags.Static);
			for(int i = 0; i < miList.Length; i++) {
				MethodInfo currentMi = miList[i];
				if(currentMi.Name == "op_Explicit") {
					if(currentMi.ReturnType == typeof(double)) {
						oDToDouble = (OracleDecimalToDouble)Delegate.CreateDelegate(typeof(OracleDecimalToDouble), currentMi);
					} else if(currentMi.ReturnType == typeof(decimal)) {
						oDToDecimal = (OracleDecimalToDecimal)Delegate.CreateDelegate(typeof(OracleDecimalToDecimal), currentMi);
					}
				}
			}
			if(oDToDecimal == null || oDToDouble == null)
				throw new InvalidOperationException("Methods 'ToDecimal' or 'ToDouble' not found.");
		}
		public override bool GetValues(IDataReader reader, Type[] fieldTypes, object[] values) {
			if(fieldTypes == null && !(reader is R)) {
				return false;
			}
			R oReader = (R)reader;
			for(int i = fieldTypes.Length - 1; i >= 0; i--) {
				if(oReader.IsDBNull(i)) {
					values[i] = DBNull.Value;
					continue;
				}
				if(fieldTypes[i].Equals(typeof(decimal))) {
					OD od = getOracleDecimal(oReader, i);
					if(oDGreaterThan(od, oracleDecimalMax) || oDGreaterThan(oracleDecimalMin, od))
						values[i] = oDToDouble(od);
					else
						if(oDEquals(od, oracleDecimalZero)) {
							values[i] = 0M;
						} else {
							int exp = (int)oDToDecimal(oDTruncate(oDLog(oDAbs(od), 10), 0));
							OD oPow = oDPow(oracleDecimalTen, exp);
							OD oMantissa =  oDDivide(od, oPow);
							oMantissa = oDTruncate(oMantissa, oDGreaterThan(oMantissa, oracleDecimalMagicMantissaMax) ? 27 : 28);
							values[i] = oDToDecimal(oMantissa) * (decimal)Math.Pow(10, exp);
						}
					continue;
				}
				values[i] = oReader.GetValue(i);
			}
			return true;
		}
		delegate OD OracleDataReaderGetOracleDecimalDelegate(R reader, int i);
		delegate bool OracleDecimalComparisonDelegate(OD left, OD right);
		delegate OD OracleDecimalOperationWithInt(OD od, int i);
		delegate OD OracleDecimalOperation(OD od);
		delegate OD OracleDecimalOperation2(OD od, OD od2);
		delegate double OracleDecimalToDouble(OD od);
		delegate decimal OracleDecimalToDecimal(OD od);
	}
}

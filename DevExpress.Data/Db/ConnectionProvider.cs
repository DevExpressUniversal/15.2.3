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
using System.Collections;
using System.Data;
using System.Globalization;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using System.Threading;
using DevExpress.Xpo.DB;
using System.IO;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress.Xpo.DB.Helpers {
	using DevExpress.Xpo.DB;
#if !SL
	using System.Data;
	using System.Reflection;
#if !CF
	using System.Reflection.Emit;
	using System.Data.Common;
#endif
	public delegate IDataStore DataStoreCreationFromConnectionDelegate(IDbConnection connection, AutoCreateOption autoCreateOption);
#endif
	public interface IDataStoreForTests : IDataStore {
		void ClearDatabase();
	}
	public delegate IDataStore DataStoreCreationFromStringDelegate(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect);
#if !SL
#if !CF
	public delegate void ExecMethodDelegate(object argument);
	public delegate object GetPropertyValueDelegate(object instance);
	public delegate void SetPropertyValueDelegate(object instance, object value);
#endif
	public class ReflectConnectionHelper {
		Type connectionType;
		ConstructorInfo connectionConstructor;
		Type exceptionType;
		public Type ConnectionType { get { return connectionType; } }
		public Type ExceptionType { get { return exceptionType; } }
		public ReflectConnectionHelper(IDbConnection connection, string exceptionTypeName) {
			connectionType = connection.GetType();
			connectionConstructor = connectionType.GetConstructor(new Type[0]);
			exceptionType = GetType(exceptionTypeName);
		}
		public static Type GetTypeFromAssembly(string assemblyName, string typeName) {
			return GetTypeFromAssembly(assemblyName, typeName, true);
		}
		public static Type GetTypeFromAssembly(string assemblyName, string typeName, bool throwException) {
			Type resultType = DevExpress.Xpo.Helpers.XPTypeActivator.GetType(assemblyName, typeName);
			if(resultType == null && throwException) throw new TypeLoadException("Could not load type " + typeName);
			return resultType;
		}
		public IDbConnection GetConnection(string connectionString) {
			IDbConnection connection = (IDbConnection)connectionConstructor.Invoke(new object[0]);
			connection.ConnectionString = connectionString;
			return connection;
		}
		public bool TryGetExceptionProperty(Exception e, string propertyName, out object value) {
			return TryGetExceptionProperty(e, propertyName, false, out value);
		}
		public bool TryGetExceptionProperty(Exception e, string propertyName, bool declaredOnly, out object value) {
			Type eType = e.GetType();
			if(eType.IsAssignableFrom(exceptionType)) {
				BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
				if(declaredOnly) flags |= BindingFlags.DeclaredOnly;
				PropertyInfo pi = eType.GetProperty(propertyName, flags);
				if(pi != null && pi.CanRead) {
					MethodInfo mi = pi.GetGetMethod();
					if(mi != null) {
						value = mi.Invoke(e, new object[0]);
						return true;
					}
				}
			}
			value = null;
			return false;
		}
		static object[] emptyList = new object[0];
		public bool TryGetExceptionProperties(Exception e, string[] propertyNameList, out object[] values) {
			return TryGetExceptionProperties(e, propertyNameList, null, out values);
		}
		public bool TryGetExceptionProperties(Exception e, string[] propertyNameList, bool[] declaredOnly, out object[] values) {
			Type eType = e.GetType();
			if(exceptionType.IsAssignableFrom(eType)) {
				bool propertyNotFound = false;
				values = new object[propertyNameList.Length];
				for(int i = 0; i < propertyNameList.Length; i++) {
					PropertyInfo pi;
					if(declaredOnly == null || !declaredOnly[i])
						pi = eType.GetProperty(propertyNameList[i], BindingFlags.Public | BindingFlags.Instance);
					else
						pi = eType.GetProperty(propertyNameList[i], BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					if(pi != null && pi.CanRead) {
						MethodInfo mi = pi.GetGetMethod();
						if(mi == null) {
							propertyNotFound = true;
							break;
						}
						values[i] = mi.Invoke(e, emptyList);
					}
				}
				if(!propertyNotFound) return true;
			}
			values = null;
			return false;
		}
		public Type GetType(string typeName) {
			return connectionType.GetAssembly().GetType(typeName, true, false);
		}
		public static IDbConnection GetConnection(string assemblyName, string typeName, string connectionString) {
			return GetConnection(assemblyName, typeName, connectionString, true);
		}
		public static IDbConnection GetConnection(string assemblyName, string typeName, string connectionString, bool throwException) {
			var con = GetConnection(assemblyName, typeName, throwException);
			con.ConnectionString = connectionString;
			return con;
		}
		public static IDbConnection GetConnection(string assemblyName, string typeName, bool throwException) {
			Type connectionType = GetTypeFromAssembly(assemblyName, typeName, throwException);
			if(connectionType == null)
				return null;
			return (IDbConnection)Activator.CreateInstance(connectionType);
		}
#if !CF
		public static ExecMethodDelegate GetCommandBuilderDeriveParametersDelegate(string assemblyName, string typeName) {
			Type instanceType = GetTypeFromAssembly(assemblyName, typeName);
			return CreateStaticMethodDelegate(instanceType, "DeriveParameters"); 
		}
#endif
		public static object CreateInstance(Type objectType, params object[] parameters) {
			Type[] types = new Type[parameters.Length];
			for(int i = 0; i < parameters.Length; i++) {
				types[i] = parameters[i].GetType();
			}
			ConstructorInfo ci = objectType.GetConstructor(types);
			if(ci == null) throw new InvalidOperationException("Constructor not found.");
			return ci.Invoke(parameters);
		}
		public static object GetPropertyValue(object instance, string propertyName) {
			return GetPropertyValue(instance, propertyName, false);
		}
		public static object GetPropertyValue(object instance, string propertyName, bool declaredOnly) {
			Type type = instance.GetType();
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			if(declaredOnly) flags |= BindingFlags.DeclaredOnly;
			PropertyInfo pi = type.GetProperty(propertyName, flags);
			if(pi != null && pi.CanRead) {
				MethodInfo mi = pi.GetGetMethod();
				if(mi != null) {
					return mi.Invoke(instance, emptyList);
				}
			}
			throw new InvalidOperationException(string.Format("Property '{0}' not found", propertyName));
		}
		public static void SetPropertyValue(object instance, string propertyName, object value) {
			SetPropertyValue(instance, propertyName, value, false);
		}
		public static void SetPropertyValue(object instance, string propertyName, object value, bool declaredOnly) {
			Type type = instance.GetType();
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			if(declaredOnly) flags |= BindingFlags.DeclaredOnly;
			PropertyInfo pi = type.GetProperty(propertyName, flags);
			if(pi != null && pi.CanWrite) {
				MethodInfo mi = pi.GetSetMethod();
				if(mi != null) {
					mi.Invoke(instance, new object[] { value });
					return;
				}
			}
			throw new InvalidOperationException(string.Format("Property '{0}' not found", propertyName));
		}
		public static object InvokeMethod(object instance, string methodName, object[] parameters, bool declaredOnly) {
			Type type = instance.GetType();
			return InvokeMethod(instance, type, methodName, parameters, declaredOnly);
		}
		public static object InvokeMethod(object instance, Type type, string methodName, object[] parameters, bool declaredOnly) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			if(declaredOnly) flags |= BindingFlags.DeclaredOnly;
			Type[] types = new Type[parameters.Length];
			for(int i = 0; i < parameters.Length; i++) {
				types[i] = parameters[i].GetType();
			}
			MethodInfo mi = type.GetMethod(methodName, flags, null, types, null);
			if(mi == null) throw new InvalidOperationException("Method not found");
			return mi.Invoke(instance, parameters);
		}
		public static object InvokeStaticMethod(Type type, string methodName, object[] parameters, bool declaredOnly) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
			if(declaredOnly) flags |= BindingFlags.DeclaredOnly;
			Type[] types = new Type[parameters.Length];
			for(int i = 0; i < parameters.Length; i++) {
				types[i] = parameters[i].GetType();
			}
			MethodInfo mi = type.GetMethod(methodName, flags, null, types, null);
			if(mi == null) throw new InvalidOperationException("Method not found");
			return mi.Invoke(null, parameters);
		}
		public static object GetCollectionFirstItem(IEnumerable collection) {
			IEnumerator enumerator = collection.GetEnumerator();
			if(enumerator.MoveNext()) {
				return enumerator.Current;
			}
			return null;
		}
		public delegate void CommandBuilderDeriveParametersHandler(IDbCommand command);
#if !CF
		abstract class GetHelperBase {
			public abstract GetPropertyValueDelegate CreateGetter(MethodInfo mi);
			public abstract SetPropertyValueDelegate CreateSetter(MethodInfo mi);
		}
		class GetHelper<T, V> : GetHelperBase {
			delegate V GetPropertyValueTemplate(T instance);
			delegate void SetPropertyValueTemplate(T instance, V value);
			public override GetPropertyValueDelegate CreateGetter(MethodInfo mi) {
				GetPropertyValueTemplate d = (GetPropertyValueTemplate)mi.CreateDelegate(typeof(GetPropertyValueTemplate), null);
				return delegate(object target) {
					return (object)d((T)target);
				};
			}
			public override SetPropertyValueDelegate CreateSetter(MethodInfo mi) {
				SetPropertyValueTemplate d = (SetPropertyValueTemplate)mi.CreateDelegate(typeof(SetPropertyValueTemplate), null);
				return delegate(object target, object value) {
					d((T)target, (V)value);
				};
			}
		}
		abstract class GetMethodHelperBase{
			public abstract ExecMethodDelegate CreateStaticMethodDelegate(MethodInfo mi);
		}
		class GetMethodHelper<T> : GetMethodHelperBase {
			delegate void GetMethodDelegateTemplate(T argument);
			public override ExecMethodDelegate CreateStaticMethodDelegate(MethodInfo mi) {
				GetMethodDelegateTemplate d = (GetMethodDelegateTemplate)mi.CreateDelegate(typeof(GetMethodDelegateTemplate));
				return delegate(object argument) {
					d((T)argument);
				};
			}
		}
		public static GetPropertyValueDelegate CreateGetPropertyDelegate(Type instanceType, string propertyName) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			PropertyInfo pi = instanceType.GetProperty(propertyName, flags);
			if(pi == null || !pi.CanRead) throw new InvalidOperationException("Property not found");
			MethodInfo mi = pi.GetGetMethod();
			GetHelperBase h = (GetHelperBase)Activator.CreateInstance(typeof(GetHelper<,>).MakeGenericType(instanceType, pi.PropertyType));
			return h.CreateGetter(mi);
		}
		public static SetPropertyValueDelegate CreateSetPropertyDelegate(Type instanceType, string propertyName) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			PropertyInfo pi = instanceType.GetProperty(propertyName, flags);
			if(pi == null || !pi.CanWrite) throw new InvalidOperationException("Property not found");
			MethodInfo mi = pi.GetSetMethod();
			GetHelperBase h = (GetHelperBase)Activator.CreateInstance(typeof(GetHelper<,>).MakeGenericType(instanceType, pi.PropertyType));
			return h.CreateSetter(mi);
		}
		public static void CreatePropertyDelegates(Type instanceType, string propertyName, out SetPropertyValueDelegate setProperty, out GetPropertyValueDelegate getProperty) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			PropertyInfo pi = instanceType.GetProperty(propertyName, flags);
			if(pi == null || !pi.CanWrite || !pi.CanRead) throw new InvalidOperationException("Property not found");
			MethodInfo miSet = pi.GetSetMethod();
			MethodInfo miGet = pi.GetGetMethod();
			GetHelperBase h = (GetHelperBase)Activator.CreateInstance(typeof(GetHelper<,>).MakeGenericType(instanceType, pi.PropertyType));
			setProperty = h.CreateSetter(miSet);
			getProperty = h.CreateGetter(miGet);
		}
		public static ExecMethodDelegate CreateStaticMethodDelegate(Type type, string methodName) {
			MethodInfo mi = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
			if (mi == null)
				throw new InvalidOperationException("Method not found");
			ParameterInfo[] parameters = mi.GetParameters();
			if(parameters.Length != 1)
				throw new InvalidOperationException("Method not found");
			GetMethodHelperBase h = (GetMethodHelperBase)Activator.CreateInstance(typeof(GetMethodHelper<>).MakeGenericType(parameters[0].ParameterType));
			return h.CreateStaticMethodDelegate(mi);
		}
#endif
	}
#endif
}
namespace DevExpress.Xpo.Helpers {
	public interface ICommandChannel {
		object Do(string command, object args);
	}
	public class CommandChannelHelper {
		public const string Message_CommandIsNotSupported = "Command '{0}' is not supported.";
		public const string Message_CommandIsNotSupportedEx = "Command '{0}' is not supported by {1}.";
		public const string Message_CommandWrongParameterSet = "Wrong parameter set for command '{0}'.";
		public const string Command_ExplicitBeginTransaction = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExplicitBeginTransaction";
		public const string Command_ExplicitCommitTransaction = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExplicitCommitTransaction";
		public const string Command_ExplicitRollbackTransaction = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExplicitRollbackTransaction";
		public const string Command_ExecuteStoredProcedure = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteStoredProcedure";
		public const string Command_ExecuteStoredProcedureParametrized = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteStoredProcedureParametrized";
		public const string Command_ExecuteNonQuerySQL = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteNonQuerySQL";
		public const string Command_ExecuteNonQuerySQLWithParams = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteNonQuerySQLWithParams";
		public const string Command_ExecuteScalarSQL = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteScalarSQL";
		public const string Command_ExecuteScalarSQLWithParams = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteScalarSQLWithParams";
		public const string Command_ExecuteQuerySQL = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteQuerySQL";
		public const string Command_ExecuteQuerySQLWithParams = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteQuerySQLWithParams";
		public const string Command_ExecuteQuerySQLWithMetadata = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteQuerySQLWithMetadata";
		public const string Command_ExecuteQuerySQLWithMetadataWithParams = "DevExpress.Xpo.Helpers.CommandChannelHelper.ExecuteQuerySQLWithMetadataWithParams";
		[Serializable]
		public class SprocQuery {
			[XmlAttribute]
			public string SprocName;
			[XmlElement(typeof(OperandValue))]
			[XmlElement(typeof(ConstantValue))]
			[XmlElement(typeof(ParameterValue))]
			[XmlElement(typeof(OperandParameter))]
			[XmlElement(typeof(SprocParameter))]
			public OperandValue[] Parameters;
			public SprocQuery() { }
			public SprocQuery(string sprocName, OperandValue[] parameters) {
				SprocName = sprocName;
				Parameters = parameters;
			}
		}
		public static SelectedData ExecuteSproc(ICommandChannel commandChannel, string sprocName, params OperandValue[] parameters) {
			return (SelectedData)commandChannel.Do(Command_ExecuteStoredProcedure, new SprocQuery(sprocName, parameters));
		}
		public static SelectedData ExecuteSprocParametrized(ICommandChannel commandChannel, string sprocName, params SprocParameter[] parameters) {
			return (SelectedData)commandChannel.Do(Command_ExecuteStoredProcedureParametrized, new SprocQuery(sprocName, parameters));
		}
		[Serializable]
		public class SqlQuery {
			[XmlAttribute]
			public string SqlCommand;
			public QueryParameterCollection Parameters;
			public string[] ParametersNames;
			public SqlQuery() { }
			public SqlQuery(string sqlCommand, QueryParameterCollection parameters, string[] parametersNames) {
				SqlCommand = sqlCommand;
				Parameters = parameters;
				ParametersNames = parametersNames;
			}
		}
		public static int ExecuteNonQuery(ICommandChannel commandChannel, string sql) {
			return (int)commandChannel.Do(Command_ExecuteNonQuerySQL, sql);
		}
		public static int ExecuteNonQueryWithParams(ICommandChannel commandChannel, string sqlCommand, QueryParameterCollection parameters, string[] parametersNames) {
			return (int)commandChannel.Do(Command_ExecuteNonQuerySQLWithParams, new SqlQuery(sqlCommand, parameters, parametersNames));
		}
		public static object ExecuteScalar(ICommandChannel commandChannel, string sql) {
			return commandChannel.Do(Command_ExecuteScalarSQL, sql);
		}
		public static object ExecuteScalarWithParams(ICommandChannel commandChannel, string sqlCommand, QueryParameterCollection parameters, string[] parametersNames) {
			return commandChannel.Do(Command_ExecuteScalarSQLWithParams, new SqlQuery(sqlCommand, parameters, parametersNames));
		}
		public static DevExpress.Xpo.DB.SelectedData ExecuteQuery(ICommandChannel commandChannel, string sql) {
			return (DevExpress.Xpo.DB.SelectedData)commandChannel.Do(Command_ExecuteQuerySQL, sql);
		}
		public static DevExpress.Xpo.DB.SelectedData ExecuteQueryWithParams(ICommandChannel commandChannel, string sqlCommand, QueryParameterCollection parameters, string[] parametersNames) {
			return (DevExpress.Xpo.DB.SelectedData)commandChannel.Do(Command_ExecuteQuerySQLWithParams, new SqlQuery(sqlCommand, parameters, parametersNames));
		}
		public static DevExpress.Xpo.DB.SelectedData ExecuteQueryWithMetadata(ICommandChannel commandChannel, string sql) {
			return (DevExpress.Xpo.DB.SelectedData)commandChannel.Do(Command_ExecuteQuerySQLWithMetadata, sql);
		}
		public static DevExpress.Xpo.DB.SelectedData ExecuteQueryWithMetadataWithParams(ICommandChannel commandChannel, string sqlCommand, QueryParameterCollection parameters, string[] parametersNames) {
			return (DevExpress.Xpo.DB.SelectedData)commandChannel.Do(Command_ExecuteQuerySQLWithMetadataWithParams, new SqlQuery(sqlCommand, parameters, parametersNames));
		}
	}
}
namespace DevExpress.Xpo.DB {
	using Compatibility.System;
	using DevExpress.Xpo.DB.Helpers;
	using System.Collections.Generic;
	[Serializable]
	public enum AutoCreateOption { DatabaseAndSchema, SchemaOnly, None, SchemaAlreadyExists };
	[Serializable]
	public enum UpdateSchemaResult { SchemaExists, FirstTableNotExists }
	[Serializable]
	public class ModificationResult {
		public ParameterValue[] Identities;
		public ModificationResult(ParameterValue[] identities) {
			this.Identities = identities;
		}
		public ModificationResult() : this(new ParameterValue[0]) { }
		public ModificationResult(List<ParameterValue> identities)
			: this(identities.ToArray()) { }
	}
	[Serializable]
	public class SelectStatementResultRow {
		[XmlArrayItem(typeof(System.Boolean))]
		[XmlArrayItem(typeof(System.Byte))]
		[XmlArrayItem(typeof(System.SByte))]
		[XmlArrayItem(typeof(System.Char))]
		[XmlArrayItem(typeof(System.Decimal))]
		[XmlArrayItem(typeof(System.Double))]
		[XmlArrayItem(typeof(System.Single))]
		[XmlArrayItem(typeof(System.Int32))]
		[XmlArrayItem(typeof(System.UInt32))]
		[XmlArrayItem(typeof(System.Int64))]
		[XmlArrayItem(typeof(System.UInt64))]
		[XmlArrayItem(typeof(System.Int16))]
		[XmlArrayItem(typeof(System.UInt16))]
		[XmlArrayItem(typeof(System.Guid))]
		[XmlArrayItem(typeof(String))]
		[XmlArrayItem(typeof(DateTime))]
		[XmlArrayItem(typeof(TimeSpan))]
		[XmlArrayItem(typeof(System.Byte[]))]
		[XmlArrayItem(typeof(NullValue))]
		public object[] XmlValues {
			get {
				object[] vals = (object[])Values.Clone();
				for(int i = 0; i < vals.Length; i++) {
					if(vals[i] == null)
						vals[i] = NullValue.Value;
					else {
						string stringVal = vals[i] as string;
						if(stringVal != null)
							vals[i] = OperandValue.FormatString(stringVal);
					}
				}
				return vals;
			}
			set {
				object[] vals = (object[])value.Clone();
				for(int i = 0; i < vals.Length; i++) {
					if(vals[i] is NullValue)
						vals[i] = null;
					else {
						string stringVal = vals[i] as string;
						if(stringVal != null)
							vals[i] = OperandValue.ReformatString(stringVal);
					}
				}
				Values = vals;
			}
		}
		[XmlIgnore]
		public object[] Values;
		public SelectStatementResultRow()
			: this(new object[0]) { }
		public SelectStatementResultRow(object[] values) {
			this.Values = values;
		}
		#region ICloneable Members
		public SelectStatementResultRow Clone() {
			object[] values = new object[Values.Length];
			Array.Copy(Values, values, values.Length);
			return new SelectStatementResultRow(values);
		}
		#endregion
	}
	[Serializable]
	public class SelectStatementResult {
		public SelectStatementResultRow[] Rows;
		public SelectStatementResult()
			: this(new SelectStatementResultRow[0]) { }
		public SelectStatementResult(SelectStatementResultRow[] rows) {
			this.Rows = rows;
		}
		public SelectStatementResult(ICollection rows) {
			SelectStatementResultRow[] result = new SelectStatementResultRow[rows.Count];
			int i = 0;
			foreach(object row in rows) {
				SelectStatementResultRow rrow = row as SelectStatementResultRow;
				if(rrow == null)
					rrow = new SelectStatementResultRow((object[])row);
				result[i] = rrow;
				i++;
			}
			this.Rows = result;
		}
		public SelectStatementResult(params object[] testData) : this((ICollection)(testData[0] is object[] ? testData : new object[] { testData })) { }
		#region ICloneable Members
		public SelectStatementResult Clone() {
			SelectStatementResultRow[] rows = new SelectStatementResultRow[Rows.Length];
			for(int i = 0; i < Rows.Length; i++)
				rows[i] = Rows[i].Clone();
			return new SelectStatementResult(rows);
		}
		#endregion
	}
	[Serializable]
	public class SelectedData {
		public SelectStatementResult[] ResultSet;
		public SelectedData(params SelectStatementResult[] resultSet) {
			this.ResultSet = resultSet;
		}
		public SelectedData() : this(null) { }
	}
	public interface IDataStore {
		UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables);
		SelectedData SelectData(params SelectStatement[] selects);
		ModificationResult ModifyData(params ModificationStatement[] dmlStatements);
		AutoCreateOption AutoCreateOption { get; }
	}
	public interface IDataStoreSchemaExplorer {
		string[] GetStorageTablesList(bool includeViews);
		DBTable[] GetStorageTables(params string[] tables);
	}
	public interface IDataStoreSchemaExplorerSp : IDataStoreSchemaExplorer {
		DBStoredProcedure[] GetStoredProcedures();
	}
	[Serializable]
	public class SprocParameter : OperandParameter {
		int? size;
		public int? Size {
			get { return size; }
			set { size = value; }
		}
		byte? precision;
		public byte? Precision {
			get { return precision; }
			set { precision = value; }
		}
		byte? scale;
		public byte? Scale {
			get { return scale; }
			set { scale = value; }
		}
		DBColumnType? dbType;
		public DBColumnType? DbType {
			get { return dbType; }
			set { dbType = value; }
		}
		SprocParameterDirection direction = SprocParameterDirection.Input;
		public SprocParameterDirection Direction {
			get { return direction; }
			set { direction = value; }
		}
		public SprocParameter() : this(null) { }
		public SprocParameter(string parameterName)
			: this(parameterName, null) {
		}
		public SprocParameter(string parameterName, object value)
			: base(parameterName, value) {
		}
		public SprocParameter(string parameterName, object value, int size)
			: this(parameterName, value) {
			this.size = size;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ 0x7315382 ^ size.GetHashCode() ^ precision.GetHashCode()
				^ scale.GetHashCode() ^ direction.GetHashCode() ^ dbType.GetHashCode();
		}
		public override bool Equals(object obj) {
			SprocParameter other = obj as SprocParameter;
			return !ReferenceEquals(other, null) && base.Equals(obj) && other.size == size 
				&& other.precision == precision && other.scale == scale && other.direction == direction
				&& other.dbType == dbType;
		}
		protected override CriteriaOperator CloneCommon() {
			return Clone();
		}
		public new OperandParameter Clone() {
			ICloneable cloneableValue = Value as ICloneable;
			if(cloneableValue != null)
				return new SprocParameter(ParameterName, cloneableValue.Clone()) {
					Direction = this.Direction,
					Size = this.Size,
					Scale = this.Scale,
					Precision = this.Precision
				};
			else
				return new SprocParameter(ParameterName, Value) {
					Direction = this.Direction,
					Size = this.Size,
					Scale = this.Scale,
					Precision = this.Precision
				};
		}
#if !SL
		public static System.Data.ParameterDirection GetDataParameterDirection(SprocParameterDirection direction) {
			switch(direction){
				case SprocParameterDirection.Input:
					return System.Data.ParameterDirection.Input;
				case SprocParameterDirection.InputOutput:
					return System.Data.ParameterDirection.InputOutput;
				case SprocParameterDirection.Output:
					return System.Data.ParameterDirection.Output;
				case SprocParameterDirection.ReturnValue:
					return System.Data.ParameterDirection.ReturnValue;
				default:
					throw new InvalidOperationException(direction.ToString());
			}
		}
#endif        
	}
	public enum SprocParameterDirection {
		Input = 1,
		Output = 2,
		InputOutput = 3,
		ReturnValue = 6,
	}
}
namespace DevExpress.Xpo.DB {
	using DevExpress.Xpo.DB.Helpers;
	using System.Collections.Generic;
	public abstract class DataStoreBase : DataStoreSerializedBase, IDataStoreForTests {
		AutoCreateOption _AutoCreateOption;
		public override AutoCreateOption AutoCreateOption { get { return this._AutoCreateOption; } }
		protected DataStoreBase(AutoCreateOption autoCreateOption) {
			this._AutoCreateOption = autoCreateOption;
		}
		void IDataStoreForTests.ClearDatabase() {
			lock(SyncRoot) {
				ProcessClearDatabase();
			}
		}
		protected abstract void ProcessClearDatabase();
		public void UpdateSchema(params DBTable[] tables) {
			UpdateSchema(false, tables);
		}
		protected abstract SelectStatementResult ProcessSelectData(SelectStatement select);
		protected sealed override SelectedData ProcessSelectData(SelectStatement[] selects) {
			SelectStatementResult[] result = new SelectStatementResult[selects.Length];
			for(int i = 0; i < selects.Length; ++i) {
				result[i] = ProcessSelectData(selects[i]);
			}
			return new SelectedData(result);
		}
		static IDictionary providersCreationByString = new Dictionary<string, object>();
		static IDictionary providersCreationByConnection = new Dictionary<string, object>(StringExtensions.ComparerInvariantCultureIgnoreCase);
#if !SL
		static Dictionary<string, ProviderFactory> providersFactory = new Dictionary<string, ProviderFactory>();
		public static ProviderFactory[] Factories {
			get {
				ProviderFactory[] result = new ProviderFactory[providersFactory.Count];
				int i = 0;
				foreach(KeyValuePair<string, ProviderFactory> factory in providersFactory) {
					result[i] = factory.Value;
					i++;
				}
				return result;
			}
		}
		public const string XpoProviderTypeParameterName = "XpoProvider";
		public static void RegisterDataStoreProvider(string providerKey, DataStoreCreationFromStringDelegate createFromStringDelegate) {
			lock(providersCreationByString.SyncRoot) {
				providersCreationByString[providerKey] = createFromStringDelegate;
			}
		}
		public static void RegisterDataStoreProvider(string connectionTypeShortName, DataStoreCreationFromConnectionDelegate createFromConnectionDelegate) {
			lock(providersCreationByConnection.SyncRoot) {
				providersCreationByConnection[connectionTypeShortName] = createFromConnectionDelegate;
			}
		}
		public static void RegisterFactory(ProviderFactory providerFactory) {
			lock(providersFactory) {			
				providersFactory[providerFactory.ProviderKey] = providerFactory;
			}
		}
		public static IDataStore QueryDataStore(IDbConnection connection, AutoCreateOption autoCreateOption) {
			Type connectionType = connection.GetType();
			DataStoreCreationFromConnectionDelegate fn = (DataStoreCreationFromConnectionDelegate)providersCreationByConnection[connectionType.FullName];
			if(fn == null)
				fn = (DataStoreCreationFromConnectionDelegate)providersCreationByConnection[connectionType.Name];
			if(fn == null)
				return null;
			IDataStore result = fn(connection, autoCreateOption);
			return result;
		}
		public static IDataStore QueryDataStore(string providerType, string connectionString, AutoCreateOption defaultAutoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			DataStoreCreationFromStringDelegate fn = (DataStoreCreationFromStringDelegate)providersCreationByString[providerType];
			if(fn == null) {
				objectsToDisposeOnDisconnect = null;
				return null;
			}
			return fn(connectionString, defaultAutoCreateOption, out objectsToDisposeOnDisconnect);
		}
#endif
	}
	public abstract class DataStoreSerializedBase : 
#if !SL && !DXPORTABLE
		MarshalByRefObject,
#endif
		IDataStore {
		public abstract object SyncRoot { get; }
		protected DataStoreSerializedBase() { }
		protected abstract UpdateSchemaResult ProcessUpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables);
		protected abstract SelectedData ProcessSelectData(params SelectStatement[] selects);
		protected abstract ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements);
		public virtual UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			lock(SyncRoot) {
				return ProcessUpdateSchema(dontCreateIfFirstTableNotExist, tables);
			}
		}
		public virtual SelectedData SelectData(params SelectStatement[] selects) {
			lock(SyncRoot) {
				return ProcessSelectData(selects);
			}
		}
		public virtual ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			lock(SyncRoot) {
				return ProcessModifyData(dmlStatements);
			}
		}
		public abstract AutoCreateOption AutoCreateOption { get; }
	}
}
#if !SL
namespace DevExpress.Xpo.DB {
	using DevExpress.Xpo.DB.Helpers;
	using System.Collections.Generic;
	using System.Data;
	using DevExpress.Xpo.Helpers;
	public interface ISqlDataStore : IDataStore {
		IDbConnection Connection { get; }
		IDbCommand CreateCommand();
	}
	public abstract class DataStoreForkBase :
#if !DXPORTABLE
		MarshalByRefObject,
#endif
		IDataStore, ICommandChannel {	
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			IDataStore provider = AcquireUpdateSchemaProvider();
			try {
				return provider.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
			} finally {
				ReleaseUpdateSchemaProvider(provider);
			}
		}
		public SelectedData SelectData(params SelectStatement[] selects) {
			IDataStore provider = AcquireReadProvider();
			try {
				return provider.SelectData(selects);
			} finally {
				ReleaseReadProvider(provider);
			}
		}
		public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			IDataStore provider = AcquireChangeProvider();
			try {
				return provider.ModifyData(dmlStatements);
			} finally {
				ReleaseChangeProvider(provider);
			}
		}
		public abstract AutoCreateOption AutoCreateOption { get; }
		public virtual IDataStore AcquireUpdateSchemaProvider() {
			return AcquireChangeProvider();
		}
		public virtual void ReleaseUpdateSchemaProvider(IDataStore provider) {
			ReleaseChangeProvider(provider);
		}
		public abstract IDataStore AcquireChangeProvider();
		public abstract void ReleaseChangeProvider(IDataStore provider);
		public abstract IDataStore AcquireReadProvider();
		public abstract void ReleaseReadProvider(IDataStore provider);
		object ICommandChannel.Do(string command, object args) {
			IDataStore provider = AcquireChangeProvider();
			try {
				ICommandChannel commandChannel = provider as ICommandChannel;
				if(commandChannel == null) {
					if(provider == null) {
						throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
					} else {
						throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, provider.GetType().FullName));
					}
				}
				return commandChannel.Do(command, args);
			} finally {
				ReleaseChangeProvider(provider);
			}
		}
	}
	public class DataStoreFork : DataStoreForkBase {
		public object SyncRoot { get { return this; } }
		readonly IDataStore[] providersList;
		int free;
		readonly ManualResetEvent providerFreeEvent = new ManualResetEvent(true);
		readonly AutoCreateOption autoCreateOption;
		public override AutoCreateOption AutoCreateOption { get { return this.autoCreateOption; } }
		public DataStoreFork(params IDataStore[] providers)
			: this((providers == null || providers.Length == 0) ? AutoCreateOption.SchemaAlreadyExists : providers[0].AutoCreateOption, providers) { }
		public DataStoreFork(AutoCreateOption autoCreateOption, params IDataStore[] providers) {
			if(providers == null || providers.Length == 0)
				throw new ArgumentNullException("providers");
			this.autoCreateOption = autoCreateOption;
			this.providersList = (IDataStore[])providers.Clone();
			free = providersList.Length;
		}
		public override IDataStore AcquireChangeProvider() {
			return AcquireReadProvider();
		}
		public override void ReleaseChangeProvider(IDataStore provider) {
			ReleaseReadProvider(provider);
		}
		public override IDataStore AcquireReadProvider() {
			for(; ; ) {
				lock(SyncRoot) {
					if(free > 0) {
						free--;
						return providersList[free];
					}
					providerFreeEvent.Reset();
				}
				providerFreeEvent.WaitOne();
			}
		}
		public override void ReleaseReadProvider(IDataStore provider) {
			lock(SyncRoot) {
				if(free == 0)
					providerFreeEvent.Set();
				providersList[free] = provider;
				free++;
			}
		}
	}
#if !CF && !DXPORTABLE
	public class DataStoreForkMultipleReadersSingleWriter : DataStoreFork {
		readonly IDataStore changesProvider;
		readonly ReaderWriterLock rwl = new ReaderWriterLock();
		public DataStoreForkMultipleReadersSingleWriter(IDataStore changesProvider, params IDataStore[] readProviders)
			: base(changesProvider.AutoCreateOption, readProviders) {
			this.changesProvider = changesProvider;
		}
		public override IDataStore AcquireChangeProvider() {
			rwl.AcquireWriterLock(Timeout.Infinite);
			return changesProvider;
		}
		public override void ReleaseChangeProvider(IDataStore provider) {
			rwl.ReleaseWriterLock();
		}
		public override IDataStore AcquireReadProvider() {
			rwl.AcquireReaderLock(Timeout.Infinite);
			return base.AcquireReadProvider();
		}
		public override void ReleaseReadProvider(IDataStore provider) {
			try {
				base.ReleaseReadProvider(provider);
			} finally {
				rwl.ReleaseReaderLock();
			}
		}
	}
#endif
	public class DataStoreSerialized : DataStoreSerializedBase {
		public override object SyncRoot { get { return this; } }
		readonly IDataStore _nested;
		public DataStoreSerialized(IDataStore nestedProvider) {
			this._nested = nestedProvider;
		}
		public IDataStore NestedDataStore { get { return _nested; } }
		protected override UpdateSchemaResult ProcessUpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			return NestedDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
		}
		protected override SelectedData ProcessSelectData(params SelectStatement[] selects) {
			return NestedDataStore.SelectData(selects);
		}
		protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements) {
			return NestedDataStore.ModifyData(dmlStatements);
		}
		public override AutoCreateOption AutoCreateOption { get { return NestedDataStore.AutoCreateOption; } }
	}
	public class DataStoreLogger : DataStoreSerialized, ICommandChannel {
		readonly System.IO.TextWriter _logWriter;
		public System.IO.TextWriter LogWriter { get { return _logWriter; } }
		public DataStoreLogger(IDataStore nestedProvider, System.IO.TextWriter logWriter)
			: base(nestedProvider) {
			this._logWriter = logWriter;
		}
		protected override UpdateSchemaResult ProcessUpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			string tablesList = string.Empty;
			foreach(DBTable table in tables) {
				if(tablesList.Length > 0)
					tablesList += ", ";
				tablesList += table.Name;
			}
			LogWriter.Write("{0:u} UpdateSchema for tables {1}, opt: {2}, result: ", DateTime.Now.ToUniversalTime(), tablesList, dontCreateIfFirstTableNotExist);
			try {
				UpdateSchemaResult result = base.ProcessUpdateSchema(dontCreateIfFirstTableNotExist, tables);
				LogWriter.WriteLine("{0}", result);
				return result;
			} catch(Exception e) {
				LogWriter.WriteLine("Exception:");
				LogWriter.WriteLine("{0}", e);
				throw;
			}
		}
		protected override SelectedData ProcessSelectData(params SelectStatement[] selects) {
			LogWriter.Write("{0:u} SelectData request with {1} queries:", DateTime.Now.ToUniversalTime(), selects.Length);
			foreach(SelectStatement st in selects) {
				LogWriter.WriteLine("{0} ;", QueryStatementToStringFormatter.GetString(st));
			}
			try {
				SelectedData gdr = base.ProcessSelectData(selects);
				for(int r = 0; r < gdr.ResultSet.Length; ++r) {
					SelectStatementResult result = gdr.ResultSet[r];
					LogWriter.WriteLine("result[{0}] {1} rows:", r, result.Rows.Length);
					int printedRows = Math.Min(5, result.Rows.Length);
					for(int i = 0; i < printedRows; ++i) {
						SelectStatementResultRow row = result.Rows[i];
						for(int j = 0; j < row.Values.Length; ++j) {
							if(j > 0) {
								LogWriter.Write("\t");
							} else {
								LogWriter.Write(" ");
							}
							LogWriter.Write(GetDisplayValue(row.Values[j]));
						}
						LogWriter.WriteLine();
					}
					if(printedRows < result.Rows.Length)
						LogWriter.WriteLine("  ...");
				}
				return gdr;
			} catch(Exception e) {
				LogWriter.WriteLine("Exception:");
				LogWriter.WriteLine("{0}", e);
				throw;
			}
		}
		protected override ModificationResult ProcessModifyData(params ModificationStatement[] dmlStatements) {
			LogWriter.WriteLine("{0:u} ModifyData:", DateTime.Now.ToUniversalTime());
			TaggedParametersHolder identities = new TaggedParametersHolder();
			int identitiesCount = 0;
			foreach(ModificationStatement s in dmlStatements) {
				LogWriter.WriteLine(" {0}", QueryStatementToStringFormatter.GetString(s, identities));
				InsertStatement ins = s as InsertStatement;
				if(ins != null && !ReferenceEquals(ins.IdentityParameter, null)) {
					ins.IdentityParameter.Value = "identity " + identitiesCount.ToString();
					++identitiesCount;
				}
			}
			LogWriter.Write(" result: ");
			try {
				ModificationResult result = base.ProcessModifyData(dmlStatements);
				if(result.Identities.Length == 0) {
					LogWriter.WriteLine("Ok.");
				} else {
					LogWriter.Write("Ok, identities returned: ");
					for(int i = 0; i < result.Identities.Length; ++i) {
						if(i > 0)
							LogWriter.Write(", ");
						LogWriter.Write(GetDisplayValue(result.Identities[i]));
					}
					LogWriter.WriteLine();
				}
				return result;
			} catch(Exception e) {
				LogWriter.WriteLine("Exception:");
				LogWriter.WriteLine("{0}", e);
				throw;
			}
		}
		protected virtual string GetDisplayValue(object obj) {
			if(obj == null)
				return "null";
			string str = obj as string;
			if(str != null) {
				str = str.Replace("\\", "\\\\");
				str = str.Replace("\n", "\\n");
				str = str.Replace("\r", "\\r");
				str = str.Replace("\t", "\\t");
				if(str.Length < 32) {
					return '"' + str + '"';
				} else {
					return '"' + str.Substring(0, 24) + "\"...";
				}
			}
			if(obj is char) {
				char ch = (char)obj;
				if(ch < 32) {
					if(ch == '\n') return "'\\n'";
					if(ch == '\r') return "'\\r'";
					if(ch == '\t') return "'\\t'";
					return "'\\" + ((int)ch).ToString() + "'";
				}
				return "'" + ch + "'";
			}
			if(obj is IConvertible) {
				return Convert.ToString(obj, System.Globalization.CultureInfo.InvariantCulture);
			}
			return obj.ToString();
		}
		object ICommandChannel.Do(string command, object args) {
			return DoInternal(command, args);
		}
		protected virtual object DoInternal(string command, object args) {
			ICommandChannel nestedCommandChannel = NestedDataStore as ICommandChannel;
			if(nestedCommandChannel == null) {
				if(NestedDataStore == null) {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, NestedDataStore.GetType().FullName));
				}
			}
			return nestedCommandChannel.Do(command, args);
		}
	}
	public class DataStoreLongrunnersWatch: IDataStore, IDataStoreSchemaExplorer, ICommandChannel {
		public readonly IDataStore Nested;
		public readonly int WatchDelay;
		public event EventHandler<LongrunnersReportEventArgs> LongrunnersDetected;
		Dictionary<TraceItem, object> traceItems = new Dictionary<TraceItem, object>();
		public DataStoreLongrunnersWatch(IDataStore nested, int watchDelay) {
			this.Nested = nested;
			this.WatchDelay = watchDelay;
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			return Nested.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
		}
		public SelectedData SelectData(params SelectStatement[] selects) {
			using(IDisposable i = Trace(selects)) {
				return Nested.SelectData(selects);
			}
		}
		public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			using(IDisposable i = Trace(dmlStatements)) {
				return Nested.ModifyData(dmlStatements);
			}
		}
		public AutoCreateOption AutoCreateOption {
			get { return Nested.AutoCreateOption; }
		}
		public string[] GetStorageTablesList(bool includeViews) {
			return ((IDataStoreSchemaExplorer)Nested).GetStorageTablesList(includeViews);
		}
		public DBTable[] GetStorageTables(params string[] tables) {
			return ((IDataStoreSchemaExplorer)Nested).GetStorageTables(tables);
		}
		public object Do(string command, object args) {
			return ((ICommandChannel)Nested).Do(command, args);
		}
		protected virtual TraceItem CreateTraceItem(BaseStatement[] stmts) {
			return new TraceItem(this, stmts);
		}
		IDisposable Trace(BaseStatement[] stmts) {
			TraceItem item = CreateTraceItem(stmts);
			return item;
		}
		void timer(object state) {
			EventHandler<LongrunnersReportEventArgs> handler = this.LongrunnersDetected;
			if(handler == null)
				return;
			LongrunnersReportEventArgs rpt;
			lock(traceItems) {
				TraceItem item = (TraceItem)state;
				if(item.Timer == null)
					return;
				rpt = new LongrunnersReportEventArgs();
				foreach(TraceItem ti in traceItems.Keys) {
					Call call = new Call();
					call.Statements = ti.Stmts;
					call.StartTime = ti.StartTime;
					call.Tag = ti.Tag;
					rpt.Calls.Add(call);
					if(ti.Timer != null) {
						ti.Timer.Dispose();
						ti.Timer = null;
					}
				}
			}
			rpt.Calls.Sort(callcomp);
			try {
				handler(this, rpt);
			} catch { }
		}
		static int callcomp(Call x, Call y) {
			return y.StartTime.CompareTo(x.StartTime);
		}
		protected class TraceItem: IDisposable {
			public readonly DataStoreLongrunnersWatch Owner;
			public readonly BaseStatement[] Stmts;
			public readonly DateTime StartTime;
			public Timer Timer;
			public object Tag;
			public TraceItem(DataStoreLongrunnersWatch owner, BaseStatement[] stmts) {
				this.Owner = owner;
				this.Stmts = stmts;
				this.StartTime = DateTime.Now;
				this.Timer = new Timer(owner.timer, this, owner.WatchDelay, Timeout.Infinite);
				lock(owner.traceItems) {
					owner.traceItems.Add(this, null);
				}
			}
			public void Dispose() {
				lock(Owner.traceItems) {
					Owner.traceItems.Remove(this);
					if(Timer != null) {
						Timer.Dispose();
						Timer = null;
					}
				}
			}
		}
		public class Call {
			public object Tag;
			public DateTime StartTime;
			public BaseStatement[] Statements;
		}
		public class LongrunnersReportEventArgs: EventArgs {
			public List<Call> Calls = new List<Call>();
			public DateTime ReportTime = DateTime.Now;
		}
	}
}
#endif //SL

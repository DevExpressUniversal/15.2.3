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
using System.Globalization;
using System.Collections;
using DevExpress.Xpo.DB;
using System.ComponentModel;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
#if !CF && !SL && !DXPORTABLE
using System.Runtime.Serialization;
#endif
namespace DevExpress.Xpo.DB.Exceptions {
	[Serializable]
	public class SchemaCorrectionNeededException : Exception {
		string sql;
#if !CF && !SL && !DXPORTABLE
		protected SchemaCorrectionNeededException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public SchemaCorrectionNeededException(string sql, Exception innerException)
			: base(DbRes.GetString(DbRes.ConnectionProvider_SchemaCorrectionNeeded, sql), innerException) {
			this.sql = sql;
		}
		public SchemaCorrectionNeededException(Exception innerException) : this(innerException.Message, innerException) { }
		public SchemaCorrectionNeededException(string sql) : this(sql, null) { }
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string Sql { get { return sql; } }
	}
	[Serializable]
	public class SqlExecutionErrorException : Exception {
		string sql;
		string parameters;
#if !CF && !SL && !DXPORTABLE
		protected SqlExecutionErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public SqlExecutionErrorException(string sql, string parameters, Exception innerException)
			:
			base(DbRes.GetString(DbRes.ConnectionProvider_SqlExecutionError, sql, parameters, innerException), innerException) {
			this.sql = sql;
			this.parameters = parameters;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string Sql { get { return sql; } }
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string Parameters { get { return parameters; } }
	}
	[Serializable]
	public class ConstraintViolationException : SqlExecutionErrorException {
#if !CF && !SL && !DXPORTABLE
		protected ConstraintViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public ConstraintViolationException(string sql, string parameters, Exception innerException)
			: base(sql, parameters, innerException) {
		}
	}
	[Serializable]
	public class UnableToOpenDatabaseException : Exception {
#if !CF && !SL && !DXPORTABLE
		protected UnableToOpenDatabaseException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public UnableToOpenDatabaseException(string connectionString, Exception innerException)
			: base(DbRes.GetString(DbRes.ConnectionProvider_UnableToOpenDatabase, connectionString, innerException), innerException) { }
	}
	[Serializable]
	public class UnableToCreateDBObjectException : Exception {
		string objectTypeName;
		string objectName;
		string parentObjectName;
#if !CF && !SL && !DXPORTABLE
		protected UnableToCreateDBObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public UnableToCreateDBObjectException(string objectTypeName, string objectName, string parentObjectName, Exception innerException)
			:
			base(DbRes.GetString(DbRes.ConnectionProvider_UnableToCreateDBObject, objectTypeName, objectName, parentObjectName, innerException.Message), innerException) {
			this.objectTypeName = objectTypeName;
			this.objectName = objectName;
			this.parentObjectName = parentObjectName;
		}
		public string ObjectTypeName { get { return objectTypeName; } set { objectTypeName = value; } }
		public string ObjectName { get { return objectName; } set { objectName = value; } }
		public string ParentObjectName { get { return parentObjectName; } set { parentObjectName = value; } }
	}
	[Serializable]
	public class PropertyTypeMappingMissingException : Exception {
		Type propertyType;
#if !CF && !SL && !DXPORTABLE
		protected PropertyTypeMappingMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public PropertyTypeMappingMissingException(Type objtype)
			: base(DbRes.GetString(DbRes.ConnectionProvider_TypeMappingMissing, objtype.ToString())) {
			this.propertyType = objtype;			
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public Type PropertyType { get { return propertyType; } }
	}
	[Serializable]
	public class LockingException : Exception {
#if !CF && !SL && !DXPORTABLE
		protected LockingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public LockingException()
			:
			base(DbRes.GetString(DbRes.ConnectionProvider_Locking)) {
		}
	}
}
namespace DevExpress.Xpo.Exceptions {
	[Serializable]
	public class ExceptionBundleException : Exception {
		Exception[] _Exceptions;
		public Exception[] Exceptions {
			get { return _Exceptions; }
			set { _Exceptions = value; }
		}
#if !CF && !SL && !DXPORTABLE
		protected ExceptionBundleException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public ExceptionBundleException(params Exception[] exceptions)
			: base(ExtractMessage(exceptions)) {
			_Exceptions = exceptions;
		}
		static string ExtractMessage(Exception[] exceptions) {
			if(exceptions == null)
				return string.Empty;
			string msg = exceptions.Length.ToString() + " exceptions thrown:";
			foreach(Exception e in exceptions) {
				msg += "\n" + e.Message + "\n";
			}
			return msg;
		}
		public override string ToString() {
			if(Exceptions == null)
				return base.ToString();
			string msg = Exceptions.Length.ToString() + " exceptions thrown:\n";
			foreach(Exception e in Exceptions) {
				msg += e.ToString() + "\n";
			}
			return msg;
		}
	}
}

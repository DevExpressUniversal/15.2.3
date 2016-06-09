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
using System.Collections.ObjectModel;
using DevExpress.PivotGrid.OLAP;
#if !SL
#if !DXPORTABLE
using System.Data.OleDb;
#endif
using DevExpress.PivotGrid.QueryMode;
#endif
namespace DevExpress.XtraPivotGrid.Data {
	public interface IOLAPResponseException {
		bool IsQueryTimeout { get; }
		Exception RaisedException { get; }
	}
	public class OLAPConnectionException : QueryHandleableException {
		public const string DefaultMessage = "Couldn't connect to the Analysis Services.";
		public OLAPConnectionException() : base() { }
		public OLAPConnectionException(string message) : base(message) { }
		public OLAPConnectionException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class OLAPException : QueryHandleableException {
		static IList<string> EmptyFieldNames = (IList<string>)null;
		static IList<string> GetFieldNames(string fieldName) {
			return new string[] { fieldName };
		}
		static IList<string> GetFieldNames(IList<OLAPMetadataColumn> columns) {
			List<string> res = new List<string>(columns.Count);
			foreach(OLAPMetadataColumn column in columns) {
				res.Add(column.UniqueName);
			}
			return res;
		}
		public const string InvalidTupleMembersCount = "Invalid tuple members count";
		public const string QueryDataException = "Failed to execute an MDX Query";
		public const string QueryMembersException = "Failed to retrieve member values";
		public const string QueryNullValuesException = "Failed to retrieve null values";
		public const string QuerySortedMembersException = "Failed to retrieve sorted member values";
		public const string QueryVisibleValuesException = "Failed to retrieve visible members";
		public const string LevelHasTooManyMembersException = "PivotGrid doesn't support levels with more than 2147483647 members";
		readonly string mdxQuery;
		readonly ReadOnlyCollection<string> fieldNames;
		readonly bool isQueryDrillDownException = false;
		public OLAPException() : base() { }
		public OLAPException(string message) : base(message) { }
		protected internal OLAPException(string message, Exception innerException)
			: base(message, innerException) { }
		protected internal OLAPException(string message, Exception innerException, string mdxQuery)
			: this(message, innerException, mdxQuery, EmptyFieldNames) { }
		protected internal OLAPException(string message, Exception innerException, string mdxQuery, string fieldName)
			: this(message, innerException, mdxQuery, GetFieldNames(fieldName)) { }
		protected internal OLAPException(string message, Exception innerException, string mdxQuery, IList<OLAPMetadataColumn> columns)
			: this(message, innerException, mdxQuery, GetFieldNames(columns)) {
		}
		protected internal OLAPException(string message, Exception innerException, string mdxQuery, IList<string> fieldNames)
			: base(message, innerException) {
			this.mdxQuery = mdxQuery;
			if(fieldNames != null)
				this.fieldNames = new ReadOnlyCollection<string>(fieldNames);
		}
		protected internal OLAPException(string message, Exception innerException, string mdxQuery, IList<string> fieldNames, bool isQueryDrillDownException) 
			: this(message, innerException, mdxQuery, fieldNames) {
			this.isQueryDrillDownException = isQueryDrillDownException;
		}
		public string MDXQuery { get { return mdxQuery; } }
		public ReadOnlyCollection<string> FieldNames { get { return fieldNames; } }
		public bool IsQueryDrillDownException { get { return isQueryDrillDownException; } }
	}
	public class OLAPKPIException : OLAPException {
		public const string QueryKPIValueException = "Failed to retrieve kpi value";
		string kpiName;
		public OLAPKPIException(Exception innerException, string mdxQuery, string kpiName) 
			: base(QueryKPIValueException, innerException, mdxQuery) {
			this.kpiName = kpiName;
		}
		public string KPIName { get { return kpiName; } }
	}
#if !DXPORTABLE
	public class XmlaErrorResponseException : Exception, IOLAPResponseException {
		static string GetMessage(DevExpress.PivotGrid.Xmla.XmlaErrors fault, Exception e) {
			if(e != null) return e.Message;
			if(fault != null && fault.Count > 0)
				return fault[0].Description;
			return OLAPConnectionException.DefaultMessage;
		}
		long errorCode;
		internal XmlaErrorResponseException(int errorCode)
			: base() {
			this.errorCode = errorCode;
		}
		internal XmlaErrorResponseException(DevExpress.PivotGrid.Xmla.XmlaErrors errors, Exception e) : base(GetMessage(errors, e), e) {
			if(errors != null && errors.Count > 0) {
				this.errorCode = Convert.ToInt64(errors[0].ErrorCode);
			}
		}
		public long ErrorCode {
			get { return this.errorCode; }
		}
		#region IOLAPResponseException Members
		bool IOLAPResponseException.IsQueryTimeout {
			get {
				return this.ErrorCode == 3238789169;
			}
		}
		Exception IOLAPResponseException.RaisedException {
			get { return this; }
		}
		#endregion
	}
#endif
#if !SL && !DXPORTABLE
	class OleDbErrorResponseException : Exception, IOLAPResponseException {
		internal OleDbErrorResponseException(OleDbException oleDbException)
			: base(oleDbException.Message, oleDbException) {
		}
#region IOLAPResponseException Members
		bool IOLAPResponseException.IsQueryTimeout {
			get {
				OleDbException innerException = (OleDbException)base.InnerException;
				return innerException.ErrorCode == -2147217900 || (innerException.ErrorCode == -2147467259
					&& innerException.Errors.Count > 0 && (innerException.Errors[0].NativeError == -1056178127 || innerException.Errors[0].NativeError == -1056571392 || innerException.Errors[0].NativeError == -1055129598));
			}
		}
		Exception IOLAPResponseException.RaisedException {
			get { return this.InnerException; }
		}
#endregion
	}
#endif
}

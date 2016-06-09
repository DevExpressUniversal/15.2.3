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
using System.Reflection;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP.AdoWrappers {
	public class BaseExceptionWrapper : Exception, IOLAPResponseException {
		public static Exception TryGetInnerException(Exception ex) {
			if(ex.InnerException == null)
				throw new Exception("No inner exception");
			return GetExceptionWrapper(ex.InnerException);
		}
		static Exception GetExceptionWrapper(Exception instance) {
			if(instance.GetType().Name == "AdomdErrorResponseException")
				return new AdomdErrorResponseException(instance);
			if(instance.GetType().Name == "AdomdConnectionException")
				return new AdomdConnectionException(instance);
			if(instance.GetType().Name == "XmlaException")
				return new XmlaException(instance);
			if(instance.GetType().Name == "AdomdUnknownResponseException" && instance.InnerException != null && instance.InnerException.GetType().Name == "XmlException")
				return new AdomdXmlException(instance);
			return instance;
		}
		readonly Exception instance;
		readonly Type instanceType;
		ErrorCollection errors;
		string fullErrorMessage;
		public BaseExceptionWrapper(Exception instance) : base() {
			this.instance = instance;
			this.instanceType = instance.GetType();
		}
		public override string Message {
			get {
				fullErrorMessage = string.Empty;
				if(Errors.Count > 0) {
					for(int i = Errors.Count - 1; i>=1; i--) {
						fullErrorMessage += Errors[i].Message + Environment.NewLine;
					}
					fullErrorMessage += Errors[0].Message;
				} else {
					fullErrorMessage = base.Message;
				}
				return fullErrorMessage;
			}
		}
		public new Exception InnerException {
			get {
				if(Instance.InnerException == null)
					return null;
				return GetExceptionWrapper(Instance.InnerException);
			}
		}
		protected Exception Instance { get { return instance; } }
		protected object GetPropertyValue(string propertyName, params object[] index) {
			try {
				return instanceType.InvokeMember(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty,
					null, instance, index);
			} catch(TargetInvocationException e) {
				throw e.InnerException;
			}
		}
		protected ErrorCollection Errors {
			get {
				if(errors == null)
					errors = new ErrorCollection(GetPropertyValue("Errors"));
				return errors;
			}
		}
		protected virtual bool IsQueryTimeoutException {
			get { return false; }
		}
		#region IOLAPResponseException Members
		bool IOLAPResponseException.IsQueryTimeout {
			get {
				return IsQueryTimeoutException;
			}
		}
		Exception IOLAPResponseException.RaisedException {
			get { return this.Instance; }
		}
		#endregion
	}
	public class AdomdXmlException : BaseExceptionWrapper {
		public AdomdXmlException(Exception instance) : base(instance) { }
	}
	public class AdomdErrorResponseException : BaseExceptionWrapper {
		public AdomdErrorResponseException(Exception instance)
			: base(instance) {
		}
		public int ErrorCode {
			get { return (int)GetPropertyValue("ErrorCode"); }
		}
		public bool IsTimeoutException() {
			return IsQueryTimeoutException;
		}
		protected override bool IsQueryTimeoutException {
			get { return this.ErrorCode == -1056178127 || this.ErrorCode == -1056571392 || this.ErrorCode == -1055129598; }
		}
	}
	public class AdomdConnectionException : BaseExceptionWrapper {
		public AdomdConnectionException(Exception instance)
			: base(instance) { }
	}
	public class XmlaException : BaseExceptionWrapper {
		public XmlaException(Exception instance)
			: base(instance) { }
		public bool IsIncorrectSessionException {
			get {
				object results = GetPropertyValue("Results");
				object value = results.GetType().InvokeMember("ContainsInvalidSessionError", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null,results, null);
				return (bool)value;
			}
		}
	}
}

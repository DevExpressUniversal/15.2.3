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
namespace DevExpress.XtraEditors.DXErrorProvider {
	public enum ErrorType { None, Default, Information, Warning, Critical, User1, User2, User3, User4, User5, User6, User7, User8, User9 };
	public interface IDXDataErrorInfo {
		void GetPropertyError(string propertyName, ErrorInfo info);
		void GetError(ErrorInfo info);
	}
	public class ErrorInfo {
		string errorText;
		ErrorType errorType;
		public ErrorInfo() {
			errorText = "";
			errorType = ErrorType.Default;
		}
		public ErrorInfo(string errorText, ErrorType errorType) {
			this.errorText = errorText;
			this.errorType = errorType;
		}
#if !SL
	[DevExpressDataLocalizedDescription("ErrorInfoErrorText")]
#endif
		public string ErrorText { get { return errorText; } set { errorText = value; } }
#if !SL
	[DevExpressDataLocalizedDescription("ErrorInfoErrorType")]
#endif
		public ErrorType ErrorType { get { return errorType; } set { errorType = value; } }
	}
}
namespace DevExpress.Utils {
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using DevExpress.XtraEditors.DXErrorProvider;
	public class ErrorInfo {
		public event EventHandler Changed;
		Dictionary<object, object> hash;
		string errorText;
		public ErrorInfo() {
			this.errorText = null;
			this.hash = new Dictionary<object, object>();
		}
		public virtual string this[object obj] {
			get {
				if (obj == null) return ErrorText;
				object val;
				Hash.TryGetValue(obj, out val);
				if (val != null) return val.ToString();
				return null;
			}
			set {
				if (obj == null) {
					ErrorText = value;
					return;
				}
				object val;
				Hash.TryGetValue(obj, out val);
				if (value != null && value.Length == 0) value = null;
				if (value == null && val == null) return;
				if (Object.Equals(val, value)) return;
				if (value == null) Hash.Remove(obj);
				else
					Hash[obj] = value;
				OnChanged();
			}
		}
		public virtual string ErrorText {
			get { return errorText; }
			set {
				if (ErrorText == value) return;
				errorText = value;
				OnChanged();
			}
		}
		public virtual bool HasErrors {
			get {
				return (ErrorText != null && ErrorText.Length > 0) || Hash.Count > 0;
			}
		}
		public virtual void ClearErrors() {
			bool changed = Hash.Count > 0 || (ErrorText != null && ErrorText.Length > 0);
			Hash.Clear();
			this.errorText = "";
			if (changed) OnChanged();
		}
		protected virtual void OnChanged() {
			if (Changed != null) Changed(this, EventArgs.Empty);
		}
		protected Dictionary<object, object> Hash { get { return hash; } }
	}
	public class ErrorInfoEx : ErrorInfo {
		ErrorType errorType;
		public ErrorInfoEx()
			: base() {
			this.errorType = ErrorType.Default;
		}
		public virtual ErrorType ErrorType {
			get { return errorType; }
			set {
				if (ErrorType == value) return;
				errorType = value;
				OnChanged();
			}
		}
		public override string this[object obj] {
			get {
				if (obj == null) {
					if (ErrorType == ErrorType.None) return null;
					return ErrorText;
				}
				object hashValue;
				Hash.TryGetValue(obj, out hashValue);
				ErrorItem val = hashValue as ErrorItem;
				if (val != null && val.ErrorType != ErrorType.None) return val.ErrorText;
				return null;
			}
			set {
				SetError(obj, value, ErrorType.Default);
			}
		}
		public virtual void SetError(object obj, string errorText, ErrorType errorType) {
			if (obj == null) {
				ErrorText = errorText;
				ErrorType = errorType;
				return;
			}
			object hashValue;
			Hash.TryGetValue(obj, out hashValue);
			ErrorItem val = hashValue as ErrorItem;
			if (errorText != null && errorText.Length == 0) errorText = null;
			if (errorText == null && val == null) return;
			if (val != null && String.Equals(val.ErrorText, errorText)) return;
			if (errorText == null || errorType == ErrorType.None) Hash.Remove(obj);
			else
				Hash[obj] = new ErrorItem(errorText, errorType);
			OnChanged();
		}
		public virtual void SetErrorType(object obj, ErrorType newErrorType) {
			object hashValue;
			Hash.TryGetValue(obj, out hashValue);
			ErrorItem val = hashValue as ErrorItem;
			if (val == null) {
				errorType = newErrorType;
				return;
			}
			if (val.ErrorType == newErrorType) return;
			val.ErrorType = newErrorType;
			OnChanged();
		}
		public virtual ErrorType GetErrorType(object obj) {
			object hashValue;
			Hash.TryGetValue(obj, out hashValue);
			ErrorItem val = hashValue as ErrorItem;
			if (val == null) return errorType;
			return val.ErrorType;
		}
		#region ErrorItem
		class ErrorItem {
			string errorText;
			ErrorType errorType;
			public ErrorItem(string errorText, ErrorType errorType) {
				this.errorText = errorText;
				this.errorType = errorType;
			}
			public string ErrorText { get { return errorText; } set { errorText = value; } }
			public ErrorType ErrorType { get { return errorType; } set { errorType = value; } }
		}
		#endregion
	}
}

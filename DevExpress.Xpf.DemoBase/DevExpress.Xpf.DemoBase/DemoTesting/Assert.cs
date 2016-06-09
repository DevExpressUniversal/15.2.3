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
using System.Runtime.Serialization;
namespace DevExpress.Xpf.DemoBase.DemoTesting {
	[Serializable]
	public class AssertionException : ApplicationException {
		protected AssertionException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
		public AssertionException(string message)
			: base(message) {
		}
	}
	#region assert
	public class Assert {
		public static readonly Assert Instance = new Assert();
		protected Assert() { }
		public void AreSame(object obj1, object obj2) {
			AreSame(obj1, obj2, string.Empty);
		}
		public void AreSame(object obj1, object obj2, string message) {
			IsTrueCore(AreSameCore(obj1, obj2), GetErrorMessage(message, string.Format("Expected: same as {0}\r\n  But was:  {1}\r\n", obj1, obj2)));
		}
		public void AreNotSame(object obj1, object obj2) {
			AreNotSame(obj1, obj2, string.Empty);
		}
		public void AreNotSame(object obj1, object obj2, string message) {
			IsTrueCore(!AreSameCore(obj1, obj2), GetErrorMessage(message, string.Format("Expected: not same as {0}\r\n  But was:  {1}\r\n", obj1, obj2)));
		}
		public void AreEqual(object obj1, object obj2) {
			AreEqual(obj1, obj2, string.Empty);
		}
		public void AreEqual(object obj1, object obj2, string message) {
			IsTrueCore(AreEqualCore(obj1, obj2), GetErrorMessage(message, string.Format("Expected: {0}\r\n  But was:  {1}\r\n", obj1, obj2)));
		}
		public void AreNotEqual(object obj1, object obj2) {
			AreNotEqual(obj1, obj2, string.Empty);
		}
		public void AreNotEqual(object obj1, object obj2, string message) {
			IsTrueCore(!AreEqualCore(obj1, obj2), GetErrorMessage(message, string.Format("Expected: not {0}\r\n  But was:  {1}\r\n", obj1, obj2)));
		}
		public void IsTrue(bool condition) {
			IsTrue(condition, string.Empty);
		}
		public void IsTrue(bool condition, string message) {
			AreEqual(true, condition, message);
		}
		void IsTrueCore(bool condition, string message) {
			if(!condition)
				Fail(message);
		}
		public void IsFalse(bool condition) {
			IsFalse(condition, string.Empty);
		}
		public void IsFalse(bool condition, string message) {
			AreEqual(false, condition, message);
		}
		public void IsNull(object obj) {
			IsNull(obj, string.Empty);
		}
		public void IsNull(object obj, string message) {
			IsTrue(obj == null, message);
		}
		public void IsNotNull(object obj) {
			IsNotNull(obj, string.Empty);
		}
		public void IsNotNull(object obj, string message) {
			IsTrue(obj != null, message);
		}
		public void Fail() {
			Fail(string.Empty);
		}
		public virtual void Fail(string message) {
			throw new AssertionException(message);
		}
		bool AreEqualCore(object obj1, object obj2) {
			return (obj1 == null && obj2 == null) || object.Equals(obj1, obj2);
		}
		bool AreSameCore(object obj1, object obj2) {
			return object.ReferenceEquals(obj1, obj2);
		}
		string GetErrorMessage(string customMessage, string assertMessage) {
			string res = "  ";
			if(!string.IsNullOrEmpty(customMessage)) {
				res += customMessage + "\r\n  ";
			}
			res += assertMessage;
			return res;
		}
	}
	public interface IErrorLog {
		void AddErrorToLog(string message);
	}
	public class AssertLog : Assert {
		readonly IErrorLog errorLog;
		public AssertLog(IErrorLog errorLog) {
			this.errorLog = errorLog;
		}
		public override void Fail(string message) {
			errorLog.AddErrorToLog(message);
		}
	}
	#endregion
}

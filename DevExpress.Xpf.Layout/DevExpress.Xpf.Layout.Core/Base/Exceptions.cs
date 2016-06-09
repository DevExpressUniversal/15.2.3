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
using System.Diagnostics;
using DevExpress.Xpf.Layout.Core.Platform;
namespace DevExpress.Xpf.Layout.Core.Base {
	public abstract class BaseException : Exception {
		public static readonly string ElementIsNull = "Element Is Null";
		public BaseException(string message)
			: base(message) {
		}
	}
	public class AffinityHelperException : BaseException {
		public static readonly string BaseTypeIsWrong = "Element Type Is Not Supported";
		protected AffinityHelperException(string message)
			: base(message) {
		}
		public static bool Assert(ILayoutElement element) {
			if(element is BaseLayoutElement) return true;
			throw new AffinityHelperException(BaseTypeIsWrong);
		}
		public static bool Assert(IView element) {
			if(element is BaseView) return true;
			throw new AffinityHelperException(BaseTypeIsWrong);
		}
	}
	public class AssertionException : BaseException {
		public static readonly string AssertionFails = "Assertion Fails";
		public AssertionException(string message)
			: base(message) {
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsTrue(bool condition) {
			if(!condition) throw new AssertionException(AssertionFails);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsFalse(bool condition) {
			if(condition) throw new AssertionException(AssertionFails);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsTrue(bool condition, string message) {
			if(!condition) throw new AssertionException(message);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsNotNull(object obj) {
			if(obj == null) throw new AssertionException(AssertionFails);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsNotNull(object obj, string message) {
			if(obj == null) throw new AssertionException(message);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsNull(object obj) {
			if(obj != null) throw new AssertionException(AssertionFails);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsNull(object obj, string message) {
			if(obj != null) throw new AssertionException(message);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void Is<T>(object obj) {
			if(!(obj is T)) throw new AssertionException(AssertionFails);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void Is<T>(object obj, string message) {
			if(!(obj is T)) throw new AssertionException(message);
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void IsNotDefault<T>(T value) where T : struct {
			if(value.Equals(default(T))) {
				throw new AssertionException(AssertionFails);
			}
		}
		[Conditional("DEBUG"), Conditional("DEBUGTEST")]
		public static void AreEqual<T>(T expected, T actual) {
			if(null == expected) {
				if(null != actual && !actual.Equals(expected)) {
					throw new AssertionException(AssertionFails);
				}
			}
			else if(!expected.Equals(actual)) {
				throw new AssertionException(AssertionFails);
			}
		}
	}
}

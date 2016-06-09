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
#if MVVM && !NETFX_CORE
namespace DevExpress.Mvvm.Native {
	public static class GuardHelper {
#else
namespace DevExpress.Utils {
	public static class Guard {
#endif
		public static void ArgumentNotNull(object value, string name) {
			if (Object.ReferenceEquals(value, null))
				ThrowArgumentNullException(name);
		}
		public static void ArgumentNonNegative(int value, string name) {
			if (value < 0)
				ThrowArgumentException(name, value);
		}
		public static void ArgumentPositive(int value, string name) {
			if (value <= 0)
				ThrowArgumentException(name, value);
		}
		public static void ArgumentNonNegative(float value, string name) {
			if (value < 0)
				ThrowArgumentException(name, value);
		}
		public static void ArgumentPositive(float value, string name) {
			if (value <= 0)
				ThrowArgumentException(name, value);
		}
		public static void ArgumentNonNegative(double value, string name) {
			if (value < 0)
				ThrowArgumentException(name, value);
		}
		public static void ArgumentPositive(double value, string name) {
			if (value <= 0)
				ThrowArgumentException(name, value);
		}
		public static void ArgumentIsNotNullOrEmpty(string value, string name) {
			if (string.IsNullOrEmpty(value))
				ThrowArgumentException(name, value);
		}
		public static TValue ArgumentMatchType<TValue>(object value, string name) {
			try {
				return (TValue)value;
			} catch(InvalidCastException e) {
				ThrowArgumentException(name, value, e);
				throw new InvalidOperationException();
			}
		}
		static void ThrowArgumentException(string propName, object val, Exception innerException = null) {
			string valueStr = 
				Object.ReferenceEquals(val, string.Empty) ? "String.Empty" :
				Object.ReferenceEquals(val, null) ? "null" :
				val.ToString();
			string s = String.Format("'{0}' is not a valid value for '{1}'", valueStr, propName);
			throw new ArgumentException(s, innerException);
		}
		static void ThrowArgumentNullException(string propName) {
			throw new ArgumentNullException(propName);
		}
	}
}

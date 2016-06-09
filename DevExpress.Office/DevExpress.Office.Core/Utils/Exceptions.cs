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
using DevExpress.Office.Localization;
namespace DevExpress.Office.Utils {
	#region Exceptions
	public static class Exceptions {
		public static void ThrowArgumentException(string propName, object val) {
			string valueStr = (val != null) ? val.ToString() : "null";
			string s = String.Format(OfficeLocalizer.GetString(OfficeStringId.Msg_IsNotValid), valueStr, propName);
			throw new ArgumentException(s);
		}
		public static void ThrowInternalException() {
			throw new Exception(OfficeLocalizer.GetString(OfficeStringId.Msg_InternalError));
		}
		public static void ThrowArgumentNullException(string propName) {
			throw new ArgumentNullException(propName);
		}
		public static void ThrowInvalidOperationException(OfficeStringId id) {
			throw new InvalidOperationException(OfficeLocalizer.GetString(id));
		}
		public static void ThrowInvalidOperationException(string message) {
			throw new InvalidOperationException(message);
		}
		public static void ThrowArgumentOutOfRangeException(string parameterName, string message) {
			throw new ArgumentOutOfRangeException(parameterName, message);
		}
		public static void ThrowArgumentOutOfRangeException(OfficeStringId id, string parameterName) {
			Exceptions.ThrowArgumentOutOfRangeException(parameterName, OfficeLocalizer.GetString(id));
		}
	}
	#endregion
}

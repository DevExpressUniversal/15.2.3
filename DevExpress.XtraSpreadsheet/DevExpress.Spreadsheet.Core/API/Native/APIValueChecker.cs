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
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	public static class ApiValueChecker {
		public static void CheckValue(int value, int minValue, int maxValue) {
			CheckValue(value, minValue, maxValue, CultureInfo.InvariantCulture);
		}
		public static void CheckValue(double value, double minValue, double maxValue, CultureInfo culture) {
			if (value < minValue || value > maxValue) {
				string[] args = new string[2] { minValue.ToString(culture), maxValue.ToString(culture) };
				ThrowError(XtraSpreadsheetStringId.Msg_IncorrectNumberRange, args);
			}
		}
		public static void CheckIndex(int index, int maxValue) {
			if (index < 0 || index > maxValue)
				ThrowError(XtraSpreadsheetStringId.Msg_ErrorIndexOutOfRange);
		}
		static void ThrowError(XtraSpreadsheetStringId id) {
			throw new ArgumentOutOfRangeException(XtraSpreadsheetLocalizer.GetString(id));
		}
		static void ThrowError(XtraSpreadsheetStringId id, string[] args) {
			string message = String.Format(XtraSpreadsheetLocalizer.GetString(id), args);
			throw new ArgumentOutOfRangeException(message);
		}
	}
}

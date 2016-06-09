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
namespace DevExpress.Office.Utils {
	public static class ValueChecker {
		public static void CheckValue(int value, int minValue, int maxValue) {
			if (value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format("Value out of range {0}...{1}", minValue, maxValue));
		}
		public static void CheckValue(int value, int minValue, int maxValue, string name) {
			if (value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format(name + " value out of range {0}...{1}", minValue, maxValue));
		}
		public static void CheckValue(long value, long minValue, long maxValue, string name) {
			if (value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format(name + " value out of range {0}...{1}", minValue, maxValue));
		}
		public static void CheckValue(double value, double minValue, double maxValue, string name) {
			if (value < minValue || value > maxValue)
				throw new ArgumentOutOfRangeException(string.Format(name + " value out of range {0}...{1}", minValue, maxValue));
		}
		public static void CheckLength(string value, int maxLength, string name) {
			if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
				throw new ArgumentException(string.Format("{0}: number of characters in this string MUST be less than or equal to {1}", name, maxLength));
		}
	}
}

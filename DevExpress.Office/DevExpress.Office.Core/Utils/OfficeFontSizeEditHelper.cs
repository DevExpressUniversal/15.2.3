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
using DevExpress.Office.Model;
namespace DevExpress.Office.Internal {
	#region OfficeFontSizeEditHelper
	public static class OfficeFontSizeEditHelper {
		#region TryGetValue
		public static bool TryGetValue(object editValue, out int value) {
			value = 0;
			if (editValue is int) {
				value = (int)editValue;
				return true;
			}
			else {
				string editText = editValue as string;
				if (!String.IsNullOrEmpty(editText) && Int32.TryParse(editText, out value))
					return true;
				else
					return false;
			}
		}
		#endregion
		public static bool TryGetHalfSizeValue(object editValue, out int value) {
			value = 0;
			if (editValue is int)  {
				value = (int)editValue*2;
				return true;
			}
			else {
				float editfloat = 0;
				string editText = String.Empty;
				if (editValue is float)
					editfloat = (float)editValue;
				else 
					editText = editValue as string;
				if ((!String.IsNullOrEmpty(editText) && float.TryParse(editText, NumberStyles.Float, CultureInfo.CurrentCulture, out editfloat)) || editfloat > 0  ) {
					if (editfloat * 10 % 5 == 0) {
						value = (int)(editfloat * 2);
						return true;
					}
					else
						return false;
				}
				else 
					return false;
			}
		}
		#region IsValidFontSize
		public static bool IsValidFontSize(int fontSize) {
			return fontSize >= PredefinedFontSizeCollection.MinFontSize && fontSize <= PredefinedFontSizeCollection.MaxFontSize;
		}
		#endregion
	}
	#endregion
}

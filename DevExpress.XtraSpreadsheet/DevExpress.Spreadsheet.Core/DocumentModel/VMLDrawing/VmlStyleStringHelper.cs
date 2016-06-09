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
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	static class VmlStyleStringHelper {
		static string[] Split(string style) {
			string[] parts = style.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			return parts;
		}
		public static string ExcludePart(this string style, params string[] partKey) {
			if(string.IsNullOrEmpty(style))
				return string.Empty;
			string[] parts = Split(style);
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < parts.Length; i++) {
				bool exclude = false;
				for(int j = 0; j < partKey.Length && !exclude; j++)
					exclude = parts[i].StartsWithInvariantCultureIgnoreCase(partKey[j]);
				if(!exclude) {
					if(sb.Length > 0) 
						sb.Append(";");
					sb.Append(parts[i]);
				}
			}
			return sb.ToString();
		}
		public static string ReplacePart(this string style, string partKey, string partValue) {
			string result = ExcludePart(style, partKey);
			if(!string.IsNullOrEmpty(partValue)) {
				if(!string.IsNullOrEmpty(result))
					result += ";";
				result += partKey + partValue;
			}
			return result;
		}
		public static bool HasPart(this string style, string part) {
			if(string.IsNullOrEmpty(part))
				return true;
			if(string.IsNullOrEmpty(style))
				return false;
			return style.IndexOfInvariantCultureIgnoreCase(part) != -1;
		}
	}
}

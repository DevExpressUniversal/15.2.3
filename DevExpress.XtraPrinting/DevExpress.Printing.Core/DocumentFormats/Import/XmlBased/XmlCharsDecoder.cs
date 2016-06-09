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
using System.Text.RegularExpressions;
namespace DevExpress.Office.Utils {
	#region XmlCharsDecoder
	public static class XmlCharsDecoder {
		static Regex xmlCharDecodingRegex = new Regex("_x(?<value>([\\da-fA-F]){4})_");
		public static string Decode(string val) {
			if (String.IsNullOrEmpty(val))
				return val;
			int underscoreIndex = val.IndexOf('_');
			if (underscoreIndex < 0)
				return val;
			MatchCollection matches = xmlCharDecodingRegex.Matches(val);
			if (matches == null || matches.Count <= 0)
				return val;
			for (int i = matches.Count - 1; i >= 0; i--) {
				Match match = matches[i];
				string hexValue = match.Groups["value"].Value;
				int value;
				if (Int32.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value)) {
					if (value <= 0x1f || value >= 0xffff) {
						val = val.Remove(match.Index, match.Length);
						val = val.Insert(match.Index, new String((char)value, 1));
					}
				}
			}
			return val;
		}
	}
	#endregion
}

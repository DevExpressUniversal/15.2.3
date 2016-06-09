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

using DevExpress.Utils;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.PivotGrid.OLAP {
	static class OLAPFormatHelper {
		static Dictionary<string, string> hash = new Dictionary<string, string>();
		public static string Intern(string format) {
			if(string.IsNullOrEmpty(format))
				return "{0}";
			string val;
			if(hash.TryGetValue(format, out val))
				return val;
			if(string.Compare("Currency", format, true) == 0)
				val = "c";
			else
				if(string.Compare("Percent", format, true) == 0)
					val = "0.00%";
				else
					if(string.Compare("Short Date", format, true) == 0)
						val = "d";
					else
						if(string.Compare("General Date", format, true) == 0)
							val = "g";
						else
							if(string.Compare("Long Date", format, true) == 0)
								val = "D";
							else
								if(string.Compare("Medium Date", format, true) == 0)
									val = "s";
								else
									if(string.Compare("Long Time", format, true) == 0)
										val = "T";
									else
										if(string.Compare("Medium Time", format, true) == 0)
											val = "u";
										else
											if(string.Compare("Short Time", format, true) == 0)
												val = "t";
											else
												if(string.Compare("General Number", format, true) == 0)
													val = "G";
												else
													if(string.Compare("Fixed", format, true) == 0)
														val = "F";
													else
														if(string.Compare("Standard", format, true) == 0)
															val = "N";
														else
															if(string.Compare("Scientific", format, true) == 0)
																val = "E";
															else
																val = format;
			val = "{0:" + val + "}";
			hash[format] = val;
			return val;
		}
		internal static string Format(OLAPCellValue val) {
			if(val.FormatString == null)
				return null;
			if(val.Locale == -1)
				return val.FormatString;
			if(val.Value is double) {
				if(double.IsPositiveInfinity((double)val.Value))
					return "1.#INF";
				if(double.IsNegativeInfinity((double)val.Value))
					return "-1.#INF";
			}
#if !DXPORTABLE
			CultureInfo culture = CultureInfo.GetCultureInfo(val.Locale);
#else
			CultureInfo culture = LanguageIdToCultureConverter.Convert(val.Locale);
#endif
			return string.Format(culture, val.FormatString, val.Value);
		}
		static Dictionary<string, string> intern = new Dictionary<string, string>();
		internal static string InternSimple(string str) {
			if(str == null)
				return null;
			string result;
			if(intern.TryGetValue(str, out result))
				return result;
			intern[str] = str;
			return str;
		}
	}
}

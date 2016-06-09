#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public static class PdfTextUtils {
		static readonly HashSet<char> rtlSet = new HashSet<char>();
		static PdfTextUtils() {
			AppendRTLRange((char)0x0590, (char)0x05FF); 
			AppendRTLRange((char)0xFB1D, (char)0xFB4F); 
			AppendRTLRange((char)0x0600, (char)0x06FF); 
			AppendRTLRange((char)0x0750, (char)0x077F); 
			AppendRTLRange((char)0x08A0, (char)0x08FF); 
			AppendRTLRange((char)0xFB50, (char)0xFDFF); 
			AppendRTLRange((char)0xFE70, (char)0xFEFF); 
			AppendRTLRange((char)0x0700, (char)0x074F); 
			AppendRTLRange((char)0x0780, (char)0x07BF); 
			AppendRTLRange((char)0x0800, (char)0x083F); 
			AppendRTLRange((char)0x0840, (char)0x085F); 
		}
		static void AppendRTLRange(char startChar, char endChar) {
			for (char c = startChar; c <= endChar; c++)
				rtlSet.Add(c);
		}
		public static bool HasRTLMark(string str) {
			foreach (char c in str)
				if (rtlSet.Contains(c))
					return true;
			return false;
		}
		public static PdfPoint RotatePoint(PdfPoint point, double angle) {
			if (angle == 0)
				return point;
			double x = point.X;
			double y = point.Y;
			double sin = Math.Sin(angle);
			double cos = Math.Cos(angle);
			return new PdfPoint(x * cos - y * sin, x * sin + y * cos);
		}
		public static double GetOrientedDistance(PdfPoint first, PdfPoint second, double angle) {
			if (angle == 0)
				return second.X - first.X;
			return (second.X - first.X) * Math.Cos(-angle) - (second.Y - first.Y) * Math.Sin(-angle);
		}
		public static bool AppendText(StringBuilder stringBuilder, bool rtl, string str) {
			int length = stringBuilder.Length;
			string lastChar = length == 0 ? String.Empty : stringBuilder[length - 1].ToString();
			if (lastChar == "\r" || lastChar == "\n" || str == "\r" || str == "\n" || str == "\r\n") {
				stringBuilder.Append(str);
				return rtl;
			} 
			else {
				if (rtl || HasRTLMark(str)) {
					stringBuilder.Insert(0, str);
					return true;
				}
				else
					stringBuilder.Append(str);
			}
			return false;
		}
		public static string Substring(string str, int offset, int length) {
			return HasRTLMark(str) ? str.Substring(str.Length - offset - length, length) : str.Substring(offset, length);
		}
		public static string Substring(string str, int offset) {
			return Substring(str, offset, str.Length - offset);
		}
	}
}

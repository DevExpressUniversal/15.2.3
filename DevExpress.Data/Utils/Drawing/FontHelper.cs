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
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
#else
using System.Drawing;
using System.Reflection;
using DevExpress.Compatibility.System.Drawing;
#endif
namespace DevExpress.Utils {
	public static class FontHelper {
		public static long GetFontHashCode(Font font) {
			long hi = (long)(((font.Size) * 100f)) << 32;
			return ((long)(GetFontHashCodeCore(font)) ^ hi);
		}
#if !SL && !DXPORTABLE
		static GetNativeFamily getNativeFamily;
		delegate IntPtr GetNativeFamily(FontFamily family);
		static GetNativeFamily NativeFamily {
			get {
				if(getNativeFamily == null) {
					try {
						getNativeFamily = (GetNativeFamily)Delegate.CreateDelegate(typeof(GetNativeFamily), null, typeof(FontFamily).GetProperty("NativeFamily", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true));
						getNativeFamily(FontFamily.GenericSansSerif);
					}
					catch {
						getNativeFamily = delegate(FontFamily f) { return new IntPtr(f.GetHashCode()); };
					}
				}
				return getNativeFamily;
			}
		}
#endif
		static long GetFontHashCodeCore(Font font) {
#if SL || DXPORTABLE
			return font.GetHashCode();
#else
			long nativeFamily64 = NativeFamily(font.FontFamily).ToInt64();
			int nativeFamily = ((int)(nativeFamily64 >> 32)) ^ (int)(nativeFamily64 & 0x00000000ffffffffL);
			int fontStyle = (int)font.Style, fontUnit = (int)font.Unit;
			uint fontSize = (uint)font.Size;
			long result = (((((int)nativeFamily) ^ ((((int)fontStyle) << 13) | (((int)fontStyle) >> 0x13))) ^ ((((int)fontUnit) << 0x1a) | (((int)fontUnit) >> 6))) ^ ((int)((((uint)fontSize) << 7) | (((uint)fontSize) >> 0x19))));
			result += (((long)nativeFamily) << 40);
			return result;
#endif
		}
		public static float GetLineSpacing(Font font) {
#if SL || DXPORTABLE
			return font.Size; 
#else
			return font.FontFamily.GetLineSpacing(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
#endif
		}
		public static float GetCellAscent(Font font) {
#if SL
			return font.FontInfo.GetAscent(font.SizeInPoints); 
#else
#if !DXPORTABLE
			return font.FontFamily.GetCellAscent(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
#else
			return font.SizeInPoints;
#endif
#endif
		}
		public static float GetCellDescent(Font font) {
#if SL
			return font.FontInfo.GetDescent(font.SizeInPoints); 
#else
#if !DXPORTABLE
			return font.FontFamily.GetCellDescent(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
#else
			return 0;
#endif
#endif
		}
		public static string GetFamilyName(Font font) {
			if(font == null)
				throw new ArgumentNullException("font");
			return GetFamilyName(font.FontFamily);
		}
		public static string GetFamilyName(FontFamily family) {
			if(family == null)
				throw new ArgumentNullException("family");
#if SL
			return family.ToString();
#else
			return family.Name;
#endif
		}
#if !SL && !DXPORTABLE
		static readonly System.Globalization.CultureInfo englishNeutralCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en");
		public static string GetEnglishFamilyName(Font font) {
			if(font == null)
				throw new ArgumentNullException("font");
			return GetEnglishFamilyName(font.FontFamily);
		}
		public static string GetEnglishFamilyName(FontFamily family) {
			if(family == null)
				throw new ArgumentNullException("family");
			return family.GetName(englishNeutralCulture.LCID);
		}
#endif
	}
}

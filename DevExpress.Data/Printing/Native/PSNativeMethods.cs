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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Drawing;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class PSNativeMethods {
#if SL || DXPORTABLE
		static public bool AspIsRunning {
			get { return false; }
		}
#else
		public interface IAspDetector {
			bool AspIsRunning { get; }
		}
		class AspDetector : IAspDetector {
			public bool AspIsRunning {
				get {
#if DEBUGTEST
					return HttpContextAccessor.Current != null;
#else
					return !Environment.UserInteractive || HttpContextAccessor.Current != null;
#endif
				}
			}
		}
		static IAspDetector aspDetector = new AspDetector();
		public static void SetAspDetector(IAspDetector detector) {
			aspDetector = detector;
		}
		static public bool AspIsRunning {
			get {
				return aspDetector.AspIsRunning;
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804")]
		public static void ForceCreateHandle(Control control) {
			IntPtr ignore = control.Handle;
		}
#endif
		public static Color ValidateBackgrColor(Color color) {
			if (color == DXSystemColors.Window || DXColor.IsTransparentOrEmpty(color))
				return DXColor.White;
			else
				return color;
		}
		static public PointF TranslatePointF(PointF val, float dx, float dy) {
			return new PointF(val.X + dx, val.Y + dy);
		}
		static public PointF TranslatePointF(PointF val, PointF pos) {
			return new PointF(val.X + pos.X, val.Y + pos.Y);
		}
		static public Array CombineCollections(ICollection items1, ICollection items2, Type type) {
			Array items = Array.CreateInstance(type, items1.Count + items2.Count);
			items1.CopyTo(items, 0);
			items2.CopyTo(items, items1.Count);
			return items;
		}
		public static bool IsNaN(float value) {
			try {
				return Single.IsNaN(value);
			} catch {
				return true;
			}
		}
		public static SizeF GetResolutionImageSize(Image img) {
			return GetResolutionImageSize(img, GraphicsDpi.DeviceIndependentPixel);
		}
		public static SizeF GetResolutionImageSize(Image img, float dpi) {
			if(img == null)
				throw new ArgumentNullException("img");
			SizeF size;
			lock(img) {
				size = new SizeF(img.Size.Width / img.HorizontalResolution, img.Size.Height / img.VerticalResolution);
			}
			return GraphicsUnitConverter.Convert(size, GraphicsDpi.Inch, dpi);
		}
		public static bool IsFloatType(Type type) {
			return (typeof(System.Decimal).Equals(type) ||
					typeof(System.Single).Equals(type) ||
					typeof(System.Double).Equals(type));
		}
		public static bool IsNumericalType(Type type) {
			return (typeof(System.Decimal).Equals(type) ||
					typeof(System.Single).Equals(type) ||
					typeof(System.Double).Equals(type) ||
					typeof(System.Int16).Equals(type) ||
					typeof(System.Int32).Equals(type) ||
					typeof(System.Int64).Equals(type) ||
					typeof(System.UInt16).Equals(type) ||
					typeof(System.UInt32).Equals(type) ||
					typeof(System.UInt64).Equals(type) ||
					typeof(System.Byte).Equals(type) ||
					typeof(System.SByte).Equals(type));
		}
		public static bool IsNullableNumericalType(Type type) {
			return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? IsNumericalType(Nullable.GetUnderlyingType(type)) : false;
		}
		public static bool ValueInsideBounds(float value, float lowBound, float highBound) {
			return !FloatsComparer.Default.FirstLessSecond(value, lowBound) && FloatsComparer.Default.FirstLessSecond(value, highBound);
		}
	}
}
namespace System.Collections.Generic {
	public static class IListExtentions {
		public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate) {
			for(int i = 0; i < list.Count; i++)
				if(predicate(list[i]))
					return i;
			return -1;
		}
		public static bool IsValidIndex<T>(this IList<T> array, int index) {
			return array != null && index >= 0 && index < array.Count;
		}
		public static int GetValidIndex<T>(this IList<T> array, int index) {
			return Math.Max(0, Math.Min(index, array.Count - 1));
		}
		public static bool TryGetValue<T>(this IList<T> array, int index, out T value) {
			if(IsValidIndex(array, index)) {
				value = array[index];
				return true;
			}
			value = default(T);
			return false;
		}
	}
}

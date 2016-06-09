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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, (L={Left},R={Right},T={Top},B={Bottom}) Dpi={Dpi}}")]
#endif
#if !DXPORTABLE
	[TypeConverter(typeof(DevExpress.XtraPrinting.Design.PaddingInfoTypeConverter))]
#endif
	public struct PaddingInfo {
#region static
		public static readonly PaddingInfo Empty = new PaddingInfo(GraphicsDpi.DeviceIndependentPixel);
		public static implicit operator PaddingInfo(int offset) {
			return new PaddingInfo(offset, offset, 0, 0);
		}
		public static bool operator !=(PaddingInfo pad1, PaddingInfo pad2) {
			return !(pad1 == pad2);
		}
		public static bool operator ==(PaddingInfo pad1, PaddingInfo pad2) {
			return pad1.Equals(pad2);
		}
		static float ValidateZeroRestrictedValue(float value, string paramName) {
			if (value < 0)
				throw new ArgumentOutOfRangeException(paramName);
			return value;
		}
#endregion
		float left;
		float right;
		float top;
		float bottom;
		float dpi;
		[
		RefreshProperties(RefreshProperties.All),
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PaddingInfo.All"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public int All {
			get {
				return Left == Top && Left == Right && Left == Bottom ? Left : -1;
			}
			set {
				Left = value;
				Top = value;
				Right = value;
				Bottom = value;
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("PaddingInfoLeft"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PaddingInfo.Left"),
		RefreshProperties(RefreshProperties.All),
		]
		public int Left {
			get { return (int)Math.Floor(left); }
			set { SetLeft(value); }
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("PaddingInfoRight"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PaddingInfo.Right"),
		RefreshProperties(RefreshProperties.All),
		]
		public int Right {
			get { return (int)Math.Floor(right); }
			set { SetRight(value); }
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("PaddingInfoTop"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PaddingInfo.Top"),
		RefreshProperties(RefreshProperties.All),
		]
		public int Top {
			get { return (int)Math.Floor(top); }
			set { SetTop(value); }
		}	 
		[
#if !SL
	DevExpressDataLocalizedDescription("PaddingInfoBottom"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PaddingInfo.Bottom"),
		RefreshProperties(RefreshProperties.All),
		]
		public int Bottom {
			get { return (int)Math.Floor(bottom); }
			set { SetBottom(value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public float Dpi {
			get { return dpi; }
			set {
				Update(value);
				dpi = value;
			}
		}
		[
		Browsable(false),
		]
		public bool IsEmpty {
			get { return left == 0 && right == 0 && top == 0 && bottom == 0; }
		}
		public PaddingInfo(int left, int right, int top, int bottom)
			: this(left, right, top, bottom, GraphicsDpi.DeviceIndependentPixel) {
		}
		public PaddingInfo(int left, int right, int top, int bottom, float dpi)
			: this((float)left, (float)right, (float)top, (float)bottom, dpi) {
		}
		PaddingInfo(float left, float right, float top, float bottom, float dpi) {
			this.left = 0;
			this.right = 0;
			this.top = 0;
			this.bottom = 0;
			this.dpi = dpi;
			SetLeft(left);
			SetRight(right);
			SetTop(top);
			SetBottom(bottom);
		}
		public PaddingInfo(float dpi)
			: this(0, 0, 0, 0, dpi) {
		}
		public PaddingInfo(int all, float dpi)
			: this(all, all, all, all, dpi) {
		}
		public PaddingInfo(PaddingInfo src, float dpi)
			: this(src.left, src.right, src.top, src.bottom, src.dpi) {
			Dpi = dpi;
		}
		public PaddingInfo(int left, int right, int top, int bottom, GraphicsUnit graphicsUnit)
			: this(left, right, top, bottom, GraphicsDpi.UnitToDpi(graphicsUnit)) {
		}
		public PaddingInfo(GraphicsUnit graphicsUnit)
			: this(GraphicsDpi.UnitToDpi(graphicsUnit)) {
		}
		void Update(float dpi) {
			if (dpi == 0)
				throw new ArgumentException("dpi");
			if (this.dpi != 0 && this.dpi != dpi) {
				left = ConvertToInt32(left, dpi);
				right = ConvertToInt32(right, dpi);
				top = ConvertToInt32(top, dpi);
				bottom = ConvertToInt32(bottom, dpi);
			}
		}
		public override bool Equals(object obj) {
			if (obj is PaddingInfo) {
				PaddingInfo padding = new PaddingInfo((PaddingInfo)obj, dpi);
				return left == padding.left && right == padding.right &&
					top == padding.top && bottom == padding.bottom;
			}
			return false;
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode((int)top, (int)left, (int)right, (int)bottom, (int)dpi);
		}
		public void RotatePaddingCounterclockwise(int numberOfTimes) {
			float tempLeft = 0;
			for (int i = 0; i < numberOfTimes; i++) {
				tempLeft = left;
				left = bottom;
				bottom = right;
				right = top;
				top = tempLeft;
			}
		}
		public RectangleF Deflate(RectangleF rect, float dpi) {
			float left = rect.Left + Convert(Left, dpi);
			float top = rect.Top + Convert(Top, dpi);
			return RectangleF.FromLTRB(left, top,
				Math.Max(left, rect.Right - Convert(Right, dpi)),
				Math.Max(top, rect.Bottom - Convert(Bottom, dpi)));
		}
		float Convert(float val, float toDpi) {
			return GraphicsUnitConverter.Convert(val, this.dpi, toDpi);
		}
		int ConvertToInt32(float val, float toDpi) {
			return System.Convert.ToInt32(Convert(val, toDpi));
		}
		public RectangleF Inflate(RectangleF rect, float dpi) {
			return RectangleF.FromLTRB(rect.Left - Convert(Left, dpi), rect.Top - Convert(Top, dpi),
				rect.Right + Convert(Right, dpi), rect.Bottom + Convert(Bottom, dpi));
		}
		public SizeF Inflate(SizeF size, float dpi) {
			return new SizeF(InflateWidth(size.Width, dpi), InflateHeight(size.Height, dpi));
		}
		public SizeF Deflate(SizeF size, float dpi) {
			return new SizeF(DeflateWidth(size.Width, dpi), DeflateHeight(size.Height, dpi));
		}
		public float InflateWidth(float width, float dpi) {
			return width + Convert(left + right, dpi);
		}
		public float DeflateWidth(float width, float dpi) {
			return width - Convert(left + right, dpi);
		}
		public float InflateHeight(float height, float dpi) {
			return height + Convert(top + bottom, dpi);
		}
		public float DeflateHeight(float height, float dpi) {
			return height - Convert(top + bottom, dpi);
		}
		public float InflateWidth(float width) {
			return width + (left + right);
		}
		public float DeflateWidth(float width) {
			return width - (left + right);
		}
		public float InflateHeight(float height) {
			return height + (top + bottom);
		}
		public float DeflateHeight(float height) {
			return height - (top + bottom);
		}
		internal PaddingInfo Scale(float scaleFactor) {
			return new PaddingInfo(
				MathMethods.Scale(left, scaleFactor),
				MathMethods.Scale(right, scaleFactor),
				MathMethods.Scale(top, scaleFactor),
				MathMethods.Scale(bottom, scaleFactor),
				dpi
				);
		}
		void SetLeft(float value) {
			left = ValidateZeroRestrictedValue(value, "Left");
		}
		void SetRight(float value) {
			right = ValidateZeroRestrictedValue(value, "Right");
		}
		void SetTop(float value) {
			top = ValidateZeroRestrictedValue(value, "Top");
		}
		void SetBottom(float value) {
			bottom = ValidateZeroRestrictedValue(value, "Bottom");
		}
	}
}

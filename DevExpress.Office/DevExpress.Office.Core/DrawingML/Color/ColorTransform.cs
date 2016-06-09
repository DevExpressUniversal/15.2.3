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

using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Model;
using System;
#if DXPORTABLE
using DevExpress.Compatibility.System.Drawing;
#else
using System.Drawing;
#endif
namespace DevExpress.Office.Drawing {
	#region ColorTransformCollection
	public class ColorTransformCollection : UndoableClonableCollection<ColorTransformBase> {
		readonly InvalidateProxy innerParent;
		public ColorTransformCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
			this.innerParent = new InvalidateProxy();
		}
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public Color ApplyTransform(Color color) {
			for (int i = 0; i < Count; i++)
				color = InnerList[i].ApplyTransform(color);
			return color;
		}
		public override ColorTransformBase GetCloneItem(ColorTransformBase item, IDocumentModelPart documentModelPart) {
			return item.Clone();
		}
		public override UndoableClonableCollection<ColorTransformBase> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new ColorTransformCollection(documentModelPart.DocumentModel);
		}
	}
	#endregion
	#region IColorTransformVisitor
	public interface IColorTransformVisitor {
		void Visit(TintColorTransform transform);
		void Visit(ShadeColorTransform transform);
		void Visit(ComplementColorTransform transform);
		void Visit(InverseColorTransform transform);
		void Visit(GrayscaleColorTransform transform);
		void Visit(AlphaColorTransform transform);
		void Visit(AlphaOffsetColorTransform transform);
		void Visit(AlphaModulationColorTransform transform);
		void Visit(HueColorTransform transform);
		void Visit(HueOffsetColorTransform transform);
		void Visit(HueModulationColorTransform transform);
		void Visit(LuminanceColorTransform transform);
		void Visit(LuminanceOffsetColorTransform transform);
		void Visit(LuminanceModulationColorTransform transform);
		void Visit(SaturationColorTransform transform);
		void Visit(SaturationOffsetColorTransform transform);
		void Visit(SaturationModulationColorTransform transform);
		void Visit(RedColorTransform transform);
		void Visit(RedOffsetColorTransform transform);
		void Visit(RedModulationColorTransform transform); 
		void Visit(GreenColorTransform transform);
		void Visit(GreenOffsetColorTransform transform);
		void Visit(GreenModulationColorTransform transform);
		void Visit(BlueColorTransform transform);
		void Visit(BlueOffsetColorTransform transform);
		void Visit(BlueModulationColorTransform transform);
		void Visit(GammaColorTransform transform);
		void Visit(InverseGammaColorTransform transform);
	}
	#endregion
	#region ColorTransformBase (abstract class)
	public abstract class ColorTransformBase {
		protected double ApplyInverseDefaultGamma(double normalRgb) {
			if (normalRgb < 0)
				return 0;
			if (normalRgb <= 0.04045)
				return normalRgb / 12.92;
			if (normalRgb < 1)
				return Math.Pow((normalRgb + 0.055) / 1.055, 2.4);
			return 1;
		}
		protected double ApplyDefaultGamma(double normalRgb) {
			if (normalRgb < 0)
				return 0;
			if (normalRgb <= 0.0031308)
				return normalRgb * 12.92;
			if (normalRgb < 1)
				return 1.055 * Math.Pow(normalRgb, 1.0 / 2.4) - 0.055;
			return 1;
		}
		protected double ToDoubleValue(int value) {
			return (double)value / 255.0;
		}
		protected int ToIntValue(double value) {
			return GetFixRGBValue((int)Math.Round(255 * value, 0));
		}
		protected int GetFixRGBValue(int rgb) {
			return rgb < 0 ? 0 : rgb > 255 ? 255 : rgb;
		}
		protected Color ApplyDefaultGamma(Color color) {
			int r = ToIntValue(ApplyDefaultGamma(ToDoubleValue(color.R)));
			int g = ToIntValue(ApplyDefaultGamma(ToDoubleValue(color.G)));
			int b = ToIntValue(ApplyDefaultGamma(ToDoubleValue(color.B)));
			return DXColor.FromArgb(r, g, b);
		}
		protected Color ApplyInverseDefaultGamma(Color color) {
			int r = ToIntValue(ApplyInverseDefaultGamma(ToDoubleValue(color.R)));
			int g = ToIntValue(ApplyInverseDefaultGamma(ToDoubleValue(color.G)));
			int b = ToIntValue(ApplyInverseDefaultGamma(ToDoubleValue(color.B)));
			return DXColor.FromArgb(r, g, b);
		}
		#region IColorTransform Members
		public abstract void Visit(IColorTransformVisitor visitor);
		public abstract ColorTransformBase Clone();
		public abstract Color ApplyTransform(Color color);
		#endregion
	}
	#endregion
	#region ColorTransformValueBase (abstract class)
	public abstract class ColorTransformValueBase : ColorTransformBase {
		readonly int value;
		protected ColorTransformValueBase(int value) {
			this.value = value;
		}
		protected int GetRGBFromValue() {
			double rgb = (double)Value/DrawingValueConstants.ThousandthOfPercentage;
			return ToIntValue(ApplyDefaultGamma(rgb));
		}
		protected double ApplyRGBOffset(double normalRgb, double offset) {
			return GetFixRGBNormalValue(normalRgb + offset);
		}
		protected double ApplyRGBModulation(double normalRgb, double modulation) {
			return GetFixRGBNormalValue(normalRgb * modulation);
		}
		protected double GetFixRGBNormalValue(double rgb) {
			return rgb < 0 ? 0 : rgb > 1 ? 1 : rgb;
		}
		protected int ApplyRGBOffset(int rgb) {
			double offset = (double)Value / DrawingValueConstants.ThousandthOfPercentage;
			return ToIntValue(ApplyDefaultGamma(ApplyRGBOffset(ApplyInverseDefaultGamma(ToDoubleValue(rgb)), offset)));
		}
		protected int ApplyRGBModulation(int rgb) {
			double modulation = (double)Value / DrawingValueConstants.ThousandthOfPercentage;
			return ApplyRGBModulationCore(rgb, modulation);
		}
		protected int ApplyRGBModulationCore(int rgb, double modulation) {
			return ToIntValue(ApplyDefaultGamma(ApplyRGBModulation(ApplyInverseDefaultGamma(ToDoubleValue(rgb)), modulation)));
		}
		public int Value { get { return value; } }
	}
	#endregion
	#region TintColorTransform
	public class TintColorTransform : ColorTransformValueBase { 
		public TintColorTransform(int value) : base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new TintColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			double normalTint = 1 - (double)Value / (double)DrawingValueConstants.ThousandthOfPercentage;
			int r = ToIntValue(ApplyDefaultGamma(ApplyTintCore(ApplyInverseDefaultGamma(ToDoubleValue(color.R)), normalTint)));
			int g = ToIntValue(ApplyDefaultGamma(ApplyTintCore(ApplyInverseDefaultGamma(ToDoubleValue(color.G)), normalTint)));
			int b = ToIntValue(ApplyDefaultGamma(ApplyTintCore(ApplyInverseDefaultGamma(ToDoubleValue(color.B)), normalTint)));
			return DXColor.FromArgb(r, g, b);
		}
		double ApplyTintCore(double normalRgb, double normalTint) {
			return normalTint > 0 ? normalRgb * (1 - normalTint) + normalTint : normalRgb * (1 + normalTint);
		}
		#region Equals
		public override bool Equals(object obj) {
			TintColorTransform other = obj as TintColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region ShadeColorTransform
	public class ShadeColorTransform : ColorTransformValueBase {
		public ShadeColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new ShadeColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			double normalShade = (double)Value / (double)DrawingValueConstants.ThousandthOfPercentage;
			int r = ApplyRGBModulationCore(color.R, normalShade);
			int g = ApplyRGBModulationCore(color.G, normalShade);
			int b = ApplyRGBModulationCore(color.B, normalShade);
			return DXColor.FromArgb(r, g, b);
		}
		#region Equals
		public override bool Equals(object obj) {
			ShadeColorTransform other = obj as ShadeColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region ComplementColorTransform
	public class ComplementColorTransform : ColorTransformBase {
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new ComplementColorTransform();
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).GetComplementColor().ToRgb(); 
		}
		#region Equals
		public override bool Equals(object obj) {
			return obj as ComplementColorTransform != null;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		#endregion
	}
	#endregion
	#region InverseColorTransform
	public class InverseColorTransform : ColorTransformBase {
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new InverseColorTransform();
		}
		public override Color ApplyTransform(Color color) {
			int r = ToIntValue(ApplyDefaultGamma(1 - (ApplyInverseDefaultGamma(ToDoubleValue(color.R)))));
			int g = ToIntValue(ApplyDefaultGamma(1 - (ApplyInverseDefaultGamma(ToDoubleValue(color.G)))));
			int b = ToIntValue(ApplyDefaultGamma(1 - (ApplyInverseDefaultGamma(ToDoubleValue(color.B)))));
			return DXColor.FromArgb(r, g, b);
		}
		#region Equals
		public override bool Equals(object obj) {
			return obj as InverseColorTransform != null;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		#endregion
	}
	#endregion
	#region GrayscaleColorTransform
	public class GrayscaleColorTransform : ColorTransformBase {
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new GrayscaleColorTransform();
		}
		public override Color ApplyTransform(Color color) {
			int gray = (int)Math.Round(0.3 * color.R + 0.59 * color.G + 0.11 * color.B + 0.5, 0);
			return DXColor.FromArgb(gray, gray, gray);
		}
		#region Equals
		public override bool Equals(object obj) {
			return obj as GrayscaleColorTransform != null;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		#endregion
	}
	#endregion
	#region AlphaColorTransform
	public class AlphaColorTransform : ColorTransformValueBase {
		#region Static Members
		public static AlphaColorTransform CreateFromA(int a) {
			int value = (int)(((double)a * DrawingValueConstants.ThousandthOfPercentage / 255.0));
			return new AlphaColorTransform(value);
		}
		#endregion
		public AlphaColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new AlphaColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(ToIntValue((double)Value / DrawingValueConstants.ThousandthOfPercentage), color.R, color.G, color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			AlphaColorTransform other = obj as AlphaColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region AlphaOffsetColorTransform
	public class AlphaOffsetColorTransform : ColorTransformValueBase {
		public AlphaOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new AlphaOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			double normalAlpha = ToDoubleValue(color.A) + (double)Value / DrawingValueConstants.ThousandthOfPercentage;
			return DXColor.FromArgb(ToIntValue(normalAlpha), color.R, color.G, color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			AlphaOffsetColorTransform other = obj as AlphaOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region AlphaModulationColorTransform
	public class AlphaModulationColorTransform : ColorTransformValueBase {
		public AlphaModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new AlphaModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			double normalAlpha = ToDoubleValue(color.A) * (double)Value / DrawingValueConstants.ThousandthOfPercentage;
			return DXColor.FromArgb(ToIntValue(normalAlpha), color.R, color.G, color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			AlphaModulationColorTransform other = obj as AlphaModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region HueColorTransform
	public class HueColorTransform : ColorTransformValueBase {
		public HueColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new HueColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplyHue(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			HueColorTransform other = obj as HueColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region HueOffsetColorTransform
	public class HueOffsetColorTransform : ColorTransformValueBase {
		public HueOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new HueOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplyHueOffset(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			HueOffsetColorTransform other = obj as HueOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region HueModulationColorTransform
	public class HueModulationColorTransform : ColorTransformValueBase {
		public HueModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new HueModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplyHueMod(Value).ToRgb();;
		}
		#region Equals
		public override bool Equals(object obj) {
			HueModulationColorTransform other = obj as HueModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region LuminanceColorTransform
	public class LuminanceColorTransform : ColorTransformValueBase {
		public LuminanceColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new LuminanceColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplyLuminance(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			LuminanceColorTransform other = obj as LuminanceColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region LuminanceOffsetColorTransform
	public class LuminanceOffsetColorTransform : ColorTransformValueBase {
		public LuminanceOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new LuminanceOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplyLuminanceOffset(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			LuminanceOffsetColorTransform other = obj as LuminanceOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region LuminanceModulationColorTransform
	public class LuminanceModulationColorTransform : ColorTransformValueBase {
		public LuminanceModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new LuminanceModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplyLuminanceMod(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			LuminanceModulationColorTransform other = obj as LuminanceModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region SaturationColorTransform
	public class SaturationColorTransform : ColorTransformValueBase {
		public SaturationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new SaturationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplySaturation(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			SaturationColorTransform other = obj as SaturationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region SaturationOffsetColorTransform
	public class SaturationOffsetColorTransform : ColorTransformValueBase {
		public SaturationOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new SaturationOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplySaturationOffset(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			SaturationOffsetColorTransform other = obj as SaturationOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region SaturationModulationColorTransform
	public class SaturationModulationColorTransform : ColorTransformValueBase {
		public SaturationModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new SaturationModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return ColorHSL.FromColorRGB(color).ApplySaturationMod(Value).ToRgb();
		}
		#region Equals
		public override bool Equals(object obj) {
			SaturationModulationColorTransform other = obj as SaturationModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region RedColorTransform
	public class RedColorTransform : ColorTransformValueBase {
		public RedColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new RedColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(GetRGBFromValue(), color.G, color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			RedColorTransform other = obj as RedColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region RedOffsetColorTransform
	public class RedOffsetColorTransform : ColorTransformValueBase {
		public RedOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new RedOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(ApplyRGBOffset(color.R), color.G, color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			RedOffsetColorTransform other = obj as RedOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region RedModulationColorTransform
	public class RedModulationColorTransform : ColorTransformValueBase {
		public RedModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new RedModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(ApplyRGBModulation(color.R), color.G, color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			RedModulationColorTransform other = obj as RedModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region GreenColorTransform
	public class GreenColorTransform : ColorTransformValueBase {
		public GreenColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new GreenColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(color.R, GetRGBFromValue(), color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			GreenColorTransform other = obj as GreenColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region GreenOffsetColorTransform
	public class GreenOffsetColorTransform : ColorTransformValueBase {
		public GreenOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new GreenOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(color.R, ApplyRGBOffset(color.G), color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			GreenOffsetColorTransform other = obj as GreenOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region GreenModulationColorTransform
	public class GreenModulationColorTransform : ColorTransformValueBase {
		public GreenModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new GreenModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(color.R, ApplyRGBModulation(color.G), color.B);
		}
		#region Equals
		public override bool Equals(object obj) {
			GreenModulationColorTransform other = obj as GreenModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region BlueColorTransform
	public class BlueColorTransform : ColorTransformValueBase {
		public BlueColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new BlueColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(color.R, color.G, GetRGBFromValue());
		}
		#region Equals
		public override bool Equals(object obj) {
			BlueColorTransform other = obj as BlueColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region BlueOffsetColorTransform
	public class BlueOffsetColorTransform : ColorTransformValueBase {
		public BlueOffsetColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new BlueOffsetColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(color.R, color.G, ApplyRGBOffset(color.B));
		}
		#region Equals
		public override bool Equals(object obj) {
			BlueOffsetColorTransform other = obj as BlueOffsetColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region BlueModulationColorTransform
	public class BlueModulationColorTransform : ColorTransformValueBase {
		public BlueModulationColorTransform(int value)
			: base(value) {
		}
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new BlueModulationColorTransform(Value);
		}
		public override Color ApplyTransform(Color color) {
			return DXColor.FromArgb(color.R, color.G, ApplyRGBModulation(color.B));
		}
		#region Equals
		public override bool Equals(object obj) {
			BlueModulationColorTransform other = obj as BlueModulationColorTransform;
			return other != null && other.Value == Value;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ Value;
		}
		#endregion
	}
	#endregion
	#region GammaColorTransform
	public class GammaColorTransform : ColorTransformBase {
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new GammaColorTransform();
		}
		public override Color ApplyTransform(Color color) {
			return ApplyDefaultGamma(color);
		}
		#region Equals
		public override bool Equals(object obj) {
			return obj as GammaColorTransform != null;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		#endregion
	}
	#endregion
	#region InverseGammaColorTransform
	public class InverseGammaColorTransform : ColorTransformBase {
		public override void Visit(IColorTransformVisitor visitor) {
			visitor.Visit(this);
		}
		public override ColorTransformBase Clone() {
			return new InverseGammaColorTransform();
		}
		public override Color ApplyTransform(Color color) {
			return ApplyInverseDefaultGamma(color);
		}
		#region Equals
		public override bool Equals(object obj) {
			return obj as InverseGammaColorTransform != null;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode();
		}
		#endregion
	}
	#endregion
}

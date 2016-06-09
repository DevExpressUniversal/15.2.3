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
using DevExpress.XtraScheduler.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	[TypeConverter(typeof(PrintColorConverterToStringConverter))]
	public class PrintColorConverter : UserInterfaceObject, ICloneable {
		#region static
		static readonly PrintColorConverter fullColor = new PrintColorConverter();
		static readonly PrintColorConverter grayScaleColor = new GrayScalePrintColorConverter();
		static readonly PrintColorConverter blackAndWhiteColor = new BlackAndWhitePrintColorConverter();
		public static PrintColorConverter BlackAndWhiteColor { get { return blackAndWhiteColor; } }
		public static PrintColorConverter GrayScaleColor { get { return grayScaleColor; } }
		public static PrintColorConverter FullColor { get { return fullColor; } }
		public static PrintColorConverter DefaultConverter { get { return FullColor; } }
		protected internal static int CalcLuma(Color color) {
			return (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B);
		}
		#endregion
		ApplyToFlags applyFlags;
		public PrintColorConverter()
			: this(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ColorConverterFullColor)) {
		}
		protected internal PrintColorConverter(string displayName)
			: base(null, displayName) {
			Initialize();
		}
		public bool ApplyToAllDayArea { get { return IsApplyTo(ApplyToFlags.AllDayArea); } set { SetFlag(ApplyToFlags.AllDayArea, value); } }
		public bool ApplyToAppointment { get { return IsApplyTo(ApplyToFlags.Appointment); } set { SetFlag(ApplyToFlags.Appointment, value); } }
		public bool ApplyToAppointmentStatus { get { return IsApplyTo(ApplyToFlags.AppointmentStatus); } set { SetFlag(ApplyToFlags.AppointmentStatus, value); } }
		public bool ApplyToCells { get { return IsApplyTo(ApplyToFlags.Cells); } set { SetFlag(ApplyToFlags.Cells, value); } }
		public bool ApplyToHeader { get { return IsApplyTo(ApplyToFlags.Header); } set { SetFlag(ApplyToFlags.Header, value); } }
		public bool ApplyToTimeRuler { get { return IsApplyTo(ApplyToFlags.TimeRuler); } set { SetFlag(ApplyToFlags.TimeRuler, value); } }
		internal ApplyToFlags ApplyFlags { get { return applyFlags; } set { applyFlags = value; } }
		protected internal virtual void Initialize() {
			applyFlags = ApplyToFlags.All;
		}
		internal bool IsApplyTo(ApplyToFlags flag) {
			return (applyFlags & flag) != 0;
		}
		internal void SetFlag(ApplyToFlags flag, bool isSet) {
			if (IsPredefinedConverter)
				ThrowReadOnlyException();
			if (isSet)
				applyFlags |= flag;
			else
				applyFlags &= ~flag;
		}
		protected internal virtual void ThrowReadOnlyException() {
			throw new Exception("You cannot modify predefined color converter. You should create a new one instead.");
		}
		public bool IsPredefinedConverter {
			get {
				return object.ReferenceEquals(this, FullColor) || object.ReferenceEquals(this, BlackAndWhiteColor) || object.ReferenceEquals(this, GrayScaleColor);
			}
		}
		public override bool Equals(object obj) {
			PrintColorConverter colorConverter = obj as PrintColorConverter;
			if (colorConverter == null)
				return false;
			if (Object.ReferenceEquals(colorConverter, this))
				return true;
			if (!base.Equals(obj))
				return false;
			return applyFlags == colorConverter.applyFlags;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public object Clone() {
			if (IsPredefinedConverter)
				return this;
			PrintColorConverter result = CreateInstance();
			result.Assign(this);
			return result;
		}
		protected virtual PrintColorConverter CreateInstance() {
			return new PrintColorConverter(this.DisplayName);
		}
		protected void Assign(PrintColorConverter src) {
			applyFlags = src.applyFlags;
		}
		#region Colors and images converters
		public virtual Color ConvertColor(Color color) {
			return color;
		}
		public Color ConvertCellBackColor(Color color) {
			return ApplyToCells ? ConvertColor(color) : Color.White;
		}
		public Color ConvertCellBorderColor(Color color) {
			return ApplyToCells ? ConvertColor(color) : Color.Black;
		}
		public Color ConvertCellBorderWeekColor(Color color) {
			color = Color.Black;
			return ApplyToCells ? ConvertColor(color) : color;
		}
		public Color ConvertCellBorderMonthColor(Color color) {
			return ConvertCellBorderWeekColor(color);
		}
		public Color ConvertAppointmentBackColor(Color color) {
			return ApplyToAppointment ? ConvertColor(color) : Color.White;
		}
		protected ColorBlend ConvertColorBlend(ColorBlend colorBlend) {
			Color[] colors = (Color[])colorBlend.Colors.Clone();
			int count = colors.Length;
			for (int i = 0; i < count; i++)
				colors[i] = ConvertColor(colors[i]);
			ColorBlend result = new ColorBlend(count);
			result.Colors = colors;
			result.Positions = (float[])colorBlend.Positions.Clone();
			return result;
		}
		public virtual Image GetConvertedImage(Image image) {
			Image imageClone = (Image)image.Clone();
			if (image.Tag != null) 
				imageClone.Tag = image.Tag;
			return imageClone;
		}
		#endregion
		#region Brushes converters
		public virtual Brush ConvertHatchBrush(HatchBrush hatchBrush) {
			Color foreColor = ConvertColor(hatchBrush.ForegroundColor);
			Color backColor = ConvertColor(hatchBrush.BackgroundColor);
			return new HatchBrush(hatchBrush.HatchStyle, foreColor, backColor);
		}
		public virtual Brush ConvertLinearGradientBrush(LinearGradientBrush linearGradientBrush) {
			ColorBlend interpolationColors = ConvertColorBlend(linearGradientBrush.InterpolationColors);
			Color[] linearColors = new Color[2] {
														ConvertColor(linearGradientBrush.LinearColors[0]),
														ConvertColor(linearGradientBrush.LinearColors[1])
													};
			LinearGradientBrush result = (LinearGradientBrush)linearGradientBrush.Clone();
			result.InterpolationColors = interpolationColors;
			result.LinearColors = linearColors;
			return result;
		}
		public virtual Brush ConvertPathGradientBrush(PathGradientBrush pathGradientBrush) {
			Color centerColor = ConvertColor(pathGradientBrush.CenterColor);
			ColorBlend interpolationColors = ConvertColorBlend(pathGradientBrush.InterpolationColors);
			Color[] surroundColors = (Color[])pathGradientBrush.SurroundColors.Clone();
			int count = surroundColors.Length;
			for (int i = 0; i < count; i++)
				surroundColors[i] = ConvertColor(surroundColors[i]);
			PathGradientBrush result = (PathGradientBrush)pathGradientBrush.Clone();
			result.CenterColor = centerColor;
			result.InterpolationColors = interpolationColors;
			result.SurroundColors = surroundColors;
			return result;
		}
		public virtual Brush ConvertSolidBrush(SolidBrush solidBrush) {
			return new SolidBrush(ConvertColor(solidBrush.Color));
		}
		public virtual Brush ConvertTextureBrush(TextureBrush textureBrush) {
			Image image = GetConvertedImage(textureBrush.Image);
			TextureBrush result = new TextureBrush(image);
			result.Transform = textureBrush.Transform;
			result.WrapMode = textureBrush.WrapMode;
			return result;
		}
		public virtual Brush ConvertBrush(Brush brush) {
			HatchBrush hatchBrush = brush as HatchBrush;
			if (hatchBrush != null)
				return ConvertHatchBrush(hatchBrush);
			LinearGradientBrush linearGradientBrush = brush as LinearGradientBrush;
			if (linearGradientBrush != null)
				return ConvertLinearGradientBrush(linearGradientBrush);
			PathGradientBrush pathGradientBrush = brush as PathGradientBrush;
			if (pathGradientBrush != null)
				return ConvertPathGradientBrush(pathGradientBrush);
			SolidBrush solidBrush = brush as SolidBrush;
			if (solidBrush != null)
				return ConvertSolidBrush(solidBrush);
			TextureBrush textureBrush = brush as TextureBrush;
			if (textureBrush != null)
				return ConvertTextureBrush(textureBrush);
			return null;
		}
		public virtual Brush ConvertAppointmentStatusBrush(Brush brush) {
			if (ApplyToAppointmentStatus)
				return ConvertBrush(brush);
			HatchBrush hatchBrush = brush as HatchBrush;
			if (hatchBrush == null)
				return new SolidBrush(Color.White);
			int foreLuma = CalcLuma(hatchBrush.ForegroundColor);
			int backLuma = CalcLuma(hatchBrush.BackgroundColor);
			return new HatchBrush(hatchBrush.HatchStyle, foreLuma < backLuma ? Color.Black : Color.White, foreLuma < backLuma ? Color.White : Color.Black);
		}
		#endregion
		#region Appearances converters
		public void ConvertAppearance(AppearanceObject appearance) {
			if (appearance.BackColor != Color.Empty)
				appearance.BackColor = ConvertColor(appearance.BackColor);
			if (appearance.BackColor2 != Color.Empty)
				appearance.BackColor2 = ConvertColor(appearance.BackColor2);
			if (appearance.BorderColor != Color.Empty)
				appearance.BorderColor = ConvertColor(appearance.BorderColor);
			if (appearance.ForeColor != Color.Empty)
				appearance.ForeColor = ConvertColor(appearance.ForeColor);
			if (appearance.Image != null)
				appearance.Image = GetConvertedImage(appearance.Image);
		}
		void ConvertToBlackOnWhiteAppearance(AppearanceObject appearance) {
			if (appearance.BackColor != Color.Empty)
				appearance.BackColor = Color.White;
			if (appearance.BackColor2 != Color.Empty)
				appearance.BackColor2 = Color.White;
			if (appearance.ForeColor != Color.Empty)
				appearance.ForeColor = Color.Black;
			if (appearance.BorderColor != Color.Empty)
				appearance.BorderColor = Color.Black;
			appearance.Image = null;
		}
		void ConvertToBlackAppearance(AppearanceObject appearance) {
			if (appearance.BackColor != Color.Empty)
				appearance.BackColor = Color.Black;
			if (appearance.BackColor2 != Color.Empty)
				appearance.BackColor2 = Color.Black;
			if (appearance.ForeColor != Color.Empty)
				appearance.ForeColor = Color.Black;
			if (appearance.BorderColor != Color.Empty)
				appearance.BorderColor = Color.Black;
			appearance.Image = null;
		}
		void ConvertToWhiteAppearance(AppearanceObject appearance) {
			if (appearance.BackColor != Color.Empty)
				appearance.BackColor = Color.White;
			if (appearance.BackColor2 != Color.Empty)
				appearance.BackColor2 = Color.White;
			if (appearance.ForeColor != Color.Empty)
				appearance.ForeColor = Color.White;
			if (appearance.BorderColor != Color.Empty)
				appearance.BorderColor = Color.White;
			appearance.Image = null;
		}
		public void ConvertAllDayAreaAppearance(AppearanceObject appearance) {
			if (ApplyToAllDayArea)
				ConvertAppearance(appearance);
			else
				ConvertToBlackOnWhiteAppearance(appearance);
		}
		public void ConvertAllDayAreaSeparatorAppearance(AppearanceObject appearance) {
			if (ApplyToAllDayArea)
				ConvertAppearance(appearance);
			else
				ConvertToBlackAppearance(appearance);
		}
		public void ConvertAppointmentAppearance(AppearanceObject appearance) {
			if (ApplyToAppointment)
				ConvertAppearance(appearance);
			else
				ConvertToBlackOnWhiteAppearance(appearance);
		}
		public void ConvertHeaderCaptionAppearance(AppearanceObject appearance) {
			if (ApplyToHeader)
				ConvertAppearance(appearance);
			else
				ConvertToBlackOnWhiteAppearance(appearance);
		}
		public void ConvertHeaderCaptionLineAppearance(AppearanceObject appearance) {
			if (ApplyToHeader)
				ConvertAppearance(appearance);
			else
				ConvertToWhiteAppearance(appearance);
		}
		public void ConvertTimeRulerAppearance(AppearanceObject appearance) {
			if (ApplyToTimeRuler)
				ConvertAppearance(appearance);
			else
				ConvertToBlackOnWhiteAppearance(appearance);
		}
		public void ConvertTimeRulerHourLineAppearance(AppearanceObject appearance) {
			if (ApplyToTimeRuler)
				ConvertAppearance(appearance);
			else
				ConvertToBlackAppearance(appearance);
		}
		public void ConvertTimeRulerLineAppearance(AppearanceObject appearance) {
			if (ApplyToTimeRuler)
				ConvertAppearance(appearance);
			else
				ConvertToBlackAppearance(appearance);
		}
		public void ConvertCellHeaderCaptionAppearance(AppearanceObject appearance) {
			if (ApplyToHeader)
				ConvertAppearance(appearance);
			else
				ConvertToBlackOnWhiteAppearance(appearance);
		}
		public void ConvertCellHeaderCaptionLineAppearance(AppearanceObject appearance) {
			if (ApplyToHeader)
				ConvertAppearance(appearance);
			else
				ConvertToWhiteAppearance(appearance);
		}
		#endregion
		public void ConvertCellAppearance(AppearanceObject appearanceObject) {
			appearanceObject.BackColor = ConvertCellBackColor(appearanceObject.BackColor);
			if (appearanceObject.BackColor2 != Color.Empty)
				appearanceObject.BackColor2 = ConvertCellBackColor(appearanceObject.BackColor2);
			appearanceObject.BorderColor = ConvertCellBorderColor(appearanceObject.BorderColor);
		}
	}
}

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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraReports.UI;
using Guard = DevExpress.Utils.Guard;
namespace DevExpress.Xpf.Reports.UserDesigner.FontProperties.Native {
	public sealed class FontProperties : BaseFontProperties {
		XRControl xrObject;
		IObjectTracker xrObjectTracker;
		public FontProperties(XRControl xrObject) {
			Guard.ArgumentNotNull(xrObject, "xrObject");
			this.xrObject = xrObject;
			Tracker.GetTracker(xrObject, out xrObjectTracker);
			xrObjectTracker.ObjectPropertyChanged += OnXRObjectPropertyChanged;
		}
		void OnXRObjectPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == ExpressionHelper.GetPropertyName((XRControl x) => x.Font)) {
				RaisePropertyChanged(() => FontFamily);
				RaisePropertyChanged(() => FontSize);
				RaiseFontStyleChanged();
				RaiseFontWeightChanged();
				RaiseTextDecorationsChanged();
			}
			if(e.PropertyName == ExpressionHelper.GetPropertyName((XRControl x) => x.ForeColor))
				RaisePropertyChanged(() => Foreground);
			if(e.PropertyName == ExpressionHelper.GetPropertyName((XRControl x) => x.TextAlignment)) {
				RaisePropertyChanged(() => TextHorizontalAlignment);
				RaisePropertyChanged(() => VerticalContentAlignment);
			}
		}
		public override FontFamily FontFamily {
			get { return new FontFamily(xrObject.Font.FontFamily.Name); }
			set { Tracker.Set(xrObject, x => x.Font, ConvertToFont(value.FamilyNames.First().Value, xrObject.Font)); }
		}
		public override float FontSize {
			get { return xrObject.Font.SizeInPoints; }
			set { Tracker.Set(xrObject, x => x.Font, ConvertToFont(value, xrObject.Font)); }
		}
		public override FontStyle FontStyle {
			get { return xrObject.Font.Italic ? FontStyles.Italic : FontStyles.Normal; }
			set { Tracker.Set(xrObject, x => x.Font, ConvertToFont(value == FontStyles.Italic, System.Drawing.FontStyle.Italic, xrObject.Font)); }
		}
		public override FontWeight FontWeight {
			get { return FontHelper.IsBoldToFontWeight(xrObject.Font.Bold); }
			set { Tracker.Set(xrObject, x => x.Font, ConvertToFont(FontHelper.FontWeightToIsBold(value), System.Drawing.FontStyle.Bold, xrObject.Font)); }
		}
		public override TextDecorationCollection TextDecorations {
			get { return FontHelper.FlagsToTextDecorations(xrObject.Font.Underline, xrObject.Font.Strikeout); }
			set {
				var fontStyle = WinFontHelper.ConvertToFontStyle(FontHelper.TextDecorationsToIsUnderline(value), System.Drawing.FontStyle.Underline, xrObject.Font.Style);
				fontStyle = WinFontHelper.ConvertToFontStyle(FontHelper.TextDecorationsToIsStrikethrough(value), System.Drawing.FontStyle.Strikeout, fontStyle);
				Tracker.Set(xrObject, x => x.Font, ConvertToFont(fontStyle, xrObject.Font));
			}
		}
		public override Brush Foreground {
			get { return new SolidColorBrush(Color.FromArgb(xrObject.ForeColor.A, xrObject.ForeColor.R, xrObject.ForeColor.G, xrObject.ForeColor.B)); }
			set {
				var color = value.ToSolidColorBrush().Color;
				Tracker.Set(xrObject, x => x.ForeColor, System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
			}
		}
		public override TextAlignment TextHorizontalAlignment {
			get { return ConvertToTextAlignment(xrObject.TextAlignment); }
			set { Tracker.Set(xrObject, x => x.TextAlignment, ConvertToXtraTextAlignment(value, xrObject.TextAlignment)); }
		}
		public override VerticalAlignment VerticalContentAlignment {
			get { return ConvertToVerticalAlignment(xrObject.TextAlignment); }
			set { Tracker.Set(xrObject, x => x.TextAlignment, ConvertToXtraTextAlignment(value, xrObject.TextAlignment)); }
		}
		#region Font methods
		static System.Drawing.Font ConvertToFont(bool fontStyleFlagState, System.Drawing.FontStyle fontStyleFlag, System.Drawing.Font baseFont) {
			return ConvertToFont(WinFontHelper.ConvertToFontStyle(fontStyleFlagState, fontStyleFlag, baseFont.Style), baseFont);
		}
		static System.Drawing.Font ConvertToFont(System.Drawing.FontStyle fontStyle, System.Drawing.Font baseFont) {
			return new System.Drawing.Font(baseFont.Name, baseFont.SizeInPoints, fontStyle, System.Drawing.GraphicsUnit.Point);
		}
		static System.Drawing.Font ConvertToFont(string familyName, System.Drawing.Font baseFont) {
			return new System.Drawing.Font(familyName, baseFont.SizeInPoints, baseFont.Style, System.Drawing.GraphicsUnit.Point);
		}
		static System.Drawing.Font ConvertToFont(float fontSize, System.Drawing.Font baseFont) {
			return new System.Drawing.Font(baseFont.Name, fontSize, baseFont.Style, System.Drawing.GraphicsUnit.Point);
		}
		#endregion
		#region Alignment methods
		static TextAlignment ConvertToTextAlignment(XtraPrinting.TextAlignment alignment) {
			switch(alignment) {
				case XtraPrinting.TextAlignment.BottomCenter: return TextAlignment.Center;
				case XtraPrinting.TextAlignment.MiddleCenter: return TextAlignment.Center;
				case XtraPrinting.TextAlignment.TopCenter: return TextAlignment.Center;
				case XtraPrinting.TextAlignment.BottomRight: return TextAlignment.Right;
				case XtraPrinting.TextAlignment.MiddleRight: return TextAlignment.Right;
				case XtraPrinting.TextAlignment.TopRight: return TextAlignment.Right;
				case XtraPrinting.TextAlignment.BottomLeft: return TextAlignment.Left;
				case XtraPrinting.TextAlignment.MiddleLeft: return TextAlignment.Left;
				case XtraPrinting.TextAlignment.TopLeft: return TextAlignment.Left;
				default: return TextAlignment.Justify;
			}
		}
		static XtraPrinting.TextAlignment ConvertToXtraTextAlignment(TextAlignment alignment, XtraPrinting.TextAlignment baseAlignment) {
			switch(alignment) {
				case TextAlignment.Center:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.BottomJustify: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.TopCenter;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.TopCenter;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.TopCenter;
						case XtraPrinting.TextAlignment.TopJustify: return XtraPrinting.TextAlignment.TopCenter;
						default: return XtraPrinting.TextAlignment.MiddleCenter;
					}
				case TextAlignment.Right:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.BottomJustify: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.TopRight;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.TopRight;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.TopRight;
						case XtraPrinting.TextAlignment.TopJustify: return XtraPrinting.TextAlignment.TopRight;
						default: return XtraPrinting.TextAlignment.MiddleRight;
					}
				case TextAlignment.Left:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.BottomLeft;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.BottomLeft;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.BottomLeft;
						case XtraPrinting.TextAlignment.BottomJustify: return XtraPrinting.TextAlignment.BottomLeft;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.TopLeft;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.TopLeft;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.TopLeft;
						case XtraPrinting.TextAlignment.TopJustify: return XtraPrinting.TextAlignment.TopLeft;
						default: return XtraPrinting.TextAlignment.MiddleLeft;
					}
				default:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.BottomJustify;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.BottomJustify;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.BottomJustify;
						case XtraPrinting.TextAlignment.BottomJustify: return XtraPrinting.TextAlignment.BottomJustify;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.TopJustify;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.TopJustify;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.TopJustify;
						case XtraPrinting.TextAlignment.TopJustify: return XtraPrinting.TextAlignment.TopJustify;
						default: return XtraPrinting.TextAlignment.MiddleJustify;
					}
			}
		}
		static VerticalAlignment ConvertToVerticalAlignment(XtraPrinting.TextAlignment alignment) {
			switch(alignment) {
				case XtraPrinting.TextAlignment.TopCenter: return VerticalAlignment.Top;
				case XtraPrinting.TextAlignment.TopRight: return VerticalAlignment.Top;
				case XtraPrinting.TextAlignment.TopLeft: return VerticalAlignment.Top;
				case XtraPrinting.TextAlignment.MiddleCenter: return VerticalAlignment.Center;
				case XtraPrinting.TextAlignment.MiddleRight: return VerticalAlignment.Center;
				case XtraPrinting.TextAlignment.MiddleLeft: return VerticalAlignment.Center;
				case XtraPrinting.TextAlignment.BottomCenter: return VerticalAlignment.Bottom;
				case XtraPrinting.TextAlignment.BottomRight: return VerticalAlignment.Bottom;
				case XtraPrinting.TextAlignment.BottomLeft: return VerticalAlignment.Bottom;
				default: return VerticalAlignment.Stretch;
			}
		}
		static XtraPrinting.TextAlignment ConvertToXtraTextAlignment(VerticalAlignment alignment, XtraPrinting.TextAlignment baseAlignment) {
			switch(alignment) {
				case VerticalAlignment.Bottom:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.MiddleCenter: return XtraPrinting.TextAlignment.BottomCenter;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.MiddleRight: return XtraPrinting.TextAlignment.BottomRight;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.BottomLeft;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.BottomLeft;
						case XtraPrinting.TextAlignment.MiddleLeft: return XtraPrinting.TextAlignment.BottomLeft;
						default: return XtraPrinting.TextAlignment.BottomJustify;
					}
				case VerticalAlignment.Top:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.TopCenter;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.TopCenter;
						case XtraPrinting.TextAlignment.MiddleCenter: return XtraPrinting.TextAlignment.TopCenter;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.TopRight;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.TopRight;
						case XtraPrinting.TextAlignment.MiddleRight: return XtraPrinting.TextAlignment.TopRight;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.TopLeft;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.TopLeft;
						case XtraPrinting.TextAlignment.MiddleLeft: return XtraPrinting.TextAlignment.TopLeft;
						default: return XtraPrinting.TextAlignment.TopJustify;
					}
				default:
					switch(baseAlignment) {
						case XtraPrinting.TextAlignment.BottomCenter: return XtraPrinting.TextAlignment.MiddleCenter;
						case XtraPrinting.TextAlignment.TopCenter: return XtraPrinting.TextAlignment.MiddleCenter;
						case XtraPrinting.TextAlignment.MiddleCenter: return XtraPrinting.TextAlignment.MiddleCenter;
						case XtraPrinting.TextAlignment.BottomRight: return XtraPrinting.TextAlignment.MiddleRight;
						case XtraPrinting.TextAlignment.TopRight: return XtraPrinting.TextAlignment.MiddleRight;
						case XtraPrinting.TextAlignment.MiddleRight: return XtraPrinting.TextAlignment.MiddleRight;
						case XtraPrinting.TextAlignment.BottomLeft: return XtraPrinting.TextAlignment.MiddleLeft;
						case XtraPrinting.TextAlignment.TopLeft: return XtraPrinting.TextAlignment.MiddleLeft;
						case XtraPrinting.TextAlignment.MiddleLeft: return XtraPrinting.TextAlignment.MiddleLeft;
						default: return XtraPrinting.TextAlignment.MiddleJustify;
					}
			}
		}
		#endregion
	}
}

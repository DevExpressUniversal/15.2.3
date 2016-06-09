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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.BrickCollection {
	abstract class BrickCreator : IBrickCreator {
		protected readonly PrintingSystemBase ps;
		protected readonly Dictionary<BrickStyleKey, BrickStyle> brickStyles;
		readonly Dictionary<IVisualBrick, IOnPageUpdater> onPageUpdaters;
		public BrickCreator(PrintingSystemBase ps, Dictionary<BrickStyleKey, BrickStyle> brickStyles, Dictionary<IVisualBrick, IOnPageUpdater> onPageUpdaters) {
			Guard.ArgumentNotNull(ps, "ps");
			Guard.ArgumentNotNull(brickStyles, "brickStyles");
			Guard.ArgumentNotNull(onPageUpdaters, "onPageUpdaters");
			this.ps = ps;
			this.brickStyles = brickStyles;
			this.onPageUpdaters = onPageUpdaters;
		}
		public abstract VisualBrick Create(UIElement source, UIElement parent);
		protected void InitializeBrickCore(UIElement source, UIElement parent, VisualBrick brick, IExportSettings exportSettings) {
			Rect rect = GetElementRect(source, parent);
			brick.Initialize(ps, GraphicsUnitConverter2.PixelToDoc(new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height)));
			brick.Url = exportSettings.Url;
			string tag = ExportSettings.GetElementTag(source);
			if(tag != null)
				brick.Value = tag;
			BrickStyleKey key = CreateBrickStyleKey(exportSettings);
			BrickStyle style = null;
			if(!brickStyles.TryGetValue(key, out style)) {
				style = CreateBrickStyle(key);
				brickStyles.Add(key, style);
			}
			brick.Style = style;
			if(exportSettings.MergeValue != null) {
				brick.SetAttachedValue(BrickAttachedProperties.MergeValue, exportSettings.MergeValue);
			}
			var onPageUpdater = exportSettings.OnPageUpdater;
			if(onPageUpdater != null) {
				Debug.Assert(!onPageUpdaters.ContainsKey(brick), "onPageUpdaters dictionary already contains entry for this brick.");
				onPageUpdaters[brick] = onPageUpdater;
			}
		}
		protected virtual Rect GetElementRect(UIElement element, UIElement parent) {
			return GetRelativeElementRect(element, parent);
		}
		static Rect GetRelativeElementRect(UIElement element, UIElement parent) {
			GeneralTransform transform = element.TransformToVisual(parent);
			return transform.TransformBounds(new Rect(new System.Windows.Point(0d, 0d), element.RenderSize));
		}
		static BrickStyleKey CreateBrickStyleKey(IExportSettings exportSettings) {
			BrickStyleKey styleKey = new BrickStyleKey();
			styleKey.BackColor = exportSettings.Background;
			styleKey.ForeColor = exportSettings.Foreground;
			styleKey.BorderColor = exportSettings.BorderColor;
			styleKey.BorderThickness = exportSettings.BorderThickness;
			styleKey.BorderDashStyle = exportSettings.BorderDashStyle;
			ITextExportSettings textExportSettings = exportSettings as ITextExportSettings;
			if(textExportSettings != null) {
				styleKey.FontFamily = textExportSettings.FontFamily;
				styleKey.FontStyle = textExportSettings.FontStyle;
				styleKey.FontWeight = textExportSettings.FontWeight;
				styleKey.FontSize = textExportSettings.FontSize;
				styleKey.Padding = textExportSettings.Padding;
				styleKey.HorizontalAlignment = textExportSettings.HorizontalAlignment;
				styleKey.VerticalAlignment = textExportSettings.VerticalAlignment;
				styleKey.TextWrapping = textExportSettings.TextWrapping;
				styleKey.TextDecorations = GetBrickTextDecorations(textExportSettings.TextDecorations);
				styleKey.TextTrimming = textExportSettings.TextTrimming;
			}
			return styleKey;
		}
		static BrickStyle CreateBrickStyle(BrickStyleKey styleKey) {
			BrickStyle style = BrickStyle.CreateDefault();
			style.BackColor = DrawingConverter.ToGdiColor(styleKey.BackColor);
			style.ForeColor = DrawingConverter.ToGdiColor(styleKey.ForeColor);
			style.BorderColor = DrawingConverter.ToGdiColor(styleKey.BorderColor);
			style.BorderStyle = BrickBorderStyle.Inset;
			style.BorderDashStyle = styleKey.BorderDashStyle;
			InitializeBorder(style, styleKey.BorderThickness);
			if(styleKey.FontFamily != null) {
				style.Font = DrawingConverter.CreateGdiFont(
					styleKey.FontFamily,
					styleKey.FontStyle,
					styleKey.FontWeight,
					styleKey.TextDecorations,
					(float)styleKey.FontSize);
				style.Padding = new PaddingInfo(
					(int)styleKey.Padding.Left,
					(int)styleKey.Padding.Right,
					(int)styleKey.Padding.Top,
					(int)styleKey.Padding.Bottom,
					System.Drawing.GraphicsUnit.Pixel);
				style.TextAlignment = DrawingConverter.ToXtraPrintingTextAlignment(styleKey.HorizontalAlignment, styleKey.VerticalAlignment);
				StringFormatFlags flags = StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox;
				if(styleKey.TextWrapping == TextWrapping.NoWrap)
					flags |= StringFormatFlags.NoWrap;
				style.StringFormat = BrickStringFormat.Create(style.TextAlignment, flags, DrawingConverter.ToStringTrimming(styleKey.TextTrimming));
			}
			return style;
		}
		static void InitializeBorder(BrickStyle style, Thickness thickness) {
			double[] lengths = { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom };
			double uniformLength = 0;
			foreach(double length in lengths) {
				if(length > 0) {
					uniformLength = length;
					break;
				}
			}
			if(uniformLength > 0) {
				foreach(double length in lengths) {
					if(length > 0 && length != uniformLength)
						throw new NotSupportedException("All rectangle borders must have the same thickness. In addition, any border can have a zero thickness, which means that this border is invisible.");
				}
			}
			style.BorderWidth = (float)uniformLength;
			BorderSide[] sides = { BorderSide.Left, BorderSide.Top, BorderSide.Right, BorderSide.Bottom };
			style.Sides = BorderSide.None;
			for(int i = 0; i < sides.Length; i++) {
				if(lengths[i] > 0)
					style.Sides |= sides[i];
			}
		}
		internal static BrickTextDecorations GetBrickTextDecorations(TextDecorationCollection decorations) {
			BrickTextDecorations brickTextDecorations = BrickTextDecorations.None;
#if SL
			if(decorations == TextDecorations.Underline) {
				brickTextDecorations |= BrickTextDecorations.Underline;
			}
#else
			if(decorations.Contains(TextDecorations.Underline.First())) {
				brickTextDecorations |= BrickTextDecorations.Underline;
			}
			if(decorations.Contains(TextDecorations.Strikethrough.First())) {
				brickTextDecorations |= BrickTextDecorations.Strikethrough;
			}
#endif
			return brickTextDecorations;
		}
	}
}

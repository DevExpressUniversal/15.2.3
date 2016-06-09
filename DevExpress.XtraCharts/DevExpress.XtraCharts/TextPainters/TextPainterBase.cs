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
using System.Drawing;
namespace DevExpress.XtraCharts.Native {
	public abstract class TextPainterBase {
		const int DefaultLinesCount = 2;
		string text;
		ITextPropertiesProvider propertiesProvider;
		SizeF textSize;
		int width;
		int height;
		RectangleF? bounds;
		Rectangle? roundedBounds;
		bool parseText;
		bool mixColorByHitTestState;
		StringInfo parsedText;
		public string Text { get { return text; } }
		public RectangleF Bounds {
			get {
				if(!bounds.HasValue)
					CalculateBounds();
				return bounds.Value;
			}
		}
		public ITextPropertiesProvider PropertiesProvider { get { return propertiesProvider; } }
		public Rectangle RoundedBounds {
			get {
				if(!roundedBounds.HasValue)
					CalculateBounds();
				return roundedBounds.Value;
			}
		}
		public int Width {
			get { return width; }
			protected set { width = value; }
		}
		public int Height {
			get { return height; }
			protected set { height = value; }
		}
		protected int BorderThickness {
			get { return propertiesProvider.Border == null ? 0 : propertiesProvider.Border.ActualThickness; }
		}
		protected Shadow Shadow {
			get { return propertiesProvider.Shadow; }
		}
		TextPainterBase(string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool parseText) {
			this.text = text;
			this.textSize = textSize;
			this.propertiesProvider = propertiesProvider;
			this.mixColorByHitTestState = mixColorByHitTestState;
			this.parseText = parseText;
		}
		public TextPainterBase(string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, TextMeasurer textMeasurer, int maxWidth, int maxHeight)
			: this(text, textSize, propertiesProvider, mixColorByHitTestState, true) {
			parsedText = new StringInfo(text, propertiesProvider, textSize, textMeasurer, maxWidth, maxHeight);
			this.width = MathUtils.Ceiling(parsedText.Bounds.Width);
			this.height = RoundHeight(parsedText.Bounds.Height);
			CorrectSizeByBorder();
		}
		public TextPainterBase(string text, SizeF textSize, ITextPropertiesProvider propertiesProvider, bool mixColorByHitTestState, bool parseText, TextMeasurer textMeasurer, int maxWidth, int maxLineCount, bool wordWrap)
			: this(text, textSize, propertiesProvider, mixColorByHitTestState, parseText) {
			if (parseText) {
				parsedText = new StringInfo(text, propertiesProvider, textSize, textMeasurer, maxWidth, maxLineCount, wordWrap);
				this.width = MathUtils.Ceiling(parsedText.Bounds.Width);
				this.height = RoundHeight(parsedText.Bounds.Height);
			}
			else if (maxWidth > 0 && maxWidth < textSize.Width) {
				int lines;
				SizeF size = textMeasurer.MeasureString(text, propertiesProvider.Font, maxWidth, propertiesProvider.Alignment, StringAlignment.Center, out lines);
				width = (int)Math.Ceiling(size.Width);
				if (maxLineCount > 0 && lines > maxLineCount)
					height = RoundHeight((size.Height / lines) * maxLineCount);
				else
					height = RoundHeight(size.Height);
			}
			else {
				this.width = MathUtils.Ceiling(textSize.Width);
				this.height = RoundHeight(textSize.Height);
			}
			CorrectSizeByBorder();
		}
		void CorrectSizeByBorder() {
			if (propertiesProvider.CorrectBoundsByBorder) {
				int doubleThickness = BorderThickness * 2;
				width += doubleThickness;
				height += doubleThickness;
			}
		}
		HitRegion CalculateHitRegion(RectangleF bounds) {
			VariousPolygon polygon = CalculateHitPolygon(bounds);
			return polygon == null ? new HitRegion(bounds) : new HitRegion(polygon);
		}
		public void Render(IRenderer renderer, HitTestController hitTestController) {
			Render(renderer, hitTestController, null, null, Color.Empty);
		}
		public void Render(IRenderer renderer, HitTestController hitTestController, object additionalObject) {
			Render(renderer, hitTestController, null, additionalObject, Color.Empty);
		}
		public void Render(IRenderer renderer, HitTestController hitTestController, object additionalHitObject, Color color) {
			Render(renderer, hitTestController, null, additionalHitObject, color);
		}
		public void Render(IRenderer renderer, HitTestController hitTestController, object additionalHitObject, Color color, Rectangle clipBounds) {
			RenderWithClipping(renderer, hitTestController, additionalHitObject, color, clipBounds);
		}
		public abstract void Render(IRenderer renderer, HitTestController hitTestController, IHitRegion hitRegion, object additionalHitObject, Color color);
		public abstract void RenderWithClipping(IRenderer renderer, HitTestController hitTestController, object additionalHitObject, Color color, Rectangle clipBounds);
		protected abstract RectangleF GetBounds();
		protected virtual VariousPolygon GetPolygon() { return null; }
		protected virtual VariousPolygon CalculateHitPolygon(RectangleF bounds) { return null; }
		protected virtual int RoundHeight(float height) {
			return MathUtils.Ceiling(height);
		}
		protected void ProcessHitTesting(IRenderer renderer, HitTestController hitTestController, IHitRegion hitRegion, object additionalHitObject) {
			renderer.ProcessHitTestRegion(hitTestController, propertiesProvider, additionalHitObject, hitRegion == null ? GetHitRegion() : hitRegion);
		}
		protected void RenderInternal(IRenderer renderer, HitTestController hitTestController, RectangleF rectangle, Color color, bool antialiasedBorder) {
			Color textColor = propertiesProvider.GetTextColor(color);
			if(!propertiesProvider.BackColor.IsEmpty)
				renderer.FillRectangle(rectangle, propertiesProvider.BackColor, propertiesProvider.FillStyle);
			RectangularBorder border = propertiesProvider.Border;
			if(border != null) {
				bool textAntialiasingEnabled = AntialiasingSupport.GetAntialiasingEnabled(propertiesProvider);
				if (antialiasedBorder && textAntialiasingEnabled)
					renderer.EnableAntialiasing(true);
				BorderHelper.RenderBorder(renderer, border, rectangle, propertiesProvider.State, propertiesProvider.Border.ActualThickness, propertiesProvider.GetBorderColor(color));
				if (antialiasedBorder && textAntialiasingEnabled)
					renderer.RestoreAntialiasing();
			}
			else if(!propertiesProvider.State.Normal && propertiesProvider.ChangeSelectedBorder) {
				Color hitTestColor = mixColorByHitTestState ? GraphicUtils.CorrectColorByHitTestState(textColor, propertiesProvider.State) :
					GraphicUtils.GetColor(textColor, propertiesProvider.State);
				renderer.EnableAntialiasing(true);
				renderer.DrawRectangle(rectangle, new Pen(hitTestColor, 1));
				renderer.RestoreAntialiasing();
			}
			if(parseText)
				foreach(LineInfo line in parsedText.Lines) {
					bool useTypographicStringFormat = line.TextBlocks.Count != 1;
					StringAlignment lineAligment = useTypographicStringFormat ? StringAlignment.Near : StringAlignment.Center;
					foreach(TextBlockInfo textBlock in line.TextBlocks) {
						NativeFont font = new NativeFont(textBlock.Font);
						renderer.DrawBoundedText(textBlock.Text, font, textBlock.FontColor, propertiesProvider, textBlock.Bounds, textBlock.Aligment, lineAligment, textSize.Height, useTypographicStringFormat);
						if (textBlock.IsHyperlink) {
							RectangleF bounds = textBlock.Bounds;
							bounds.Offset(renderer.Transform.OffsetX, renderer.Transform.OffsetY);
							renderer.ProcessHitTestRegion(hitTestController, propertiesProvider, new HyperlinkSource(textBlock.Hyperlink), CalculateHitRegion(bounds));
						}
						font.Dispose();
					}
				}
			else {
				NativeFont font = new NativeFont(propertiesProvider.Font);
				renderer.DrawBoundedText(text, font, textColor, propertiesProvider, rectangle, propertiesProvider.Alignment, StringAlignment.Center, textSize.Height);
				font.Dispose();
			}
		}
		protected void CalculateBounds() {
			bounds = GetBounds();
			roundedBounds = GraphicUtils.RoundRectangle(bounds.Value);
		}
		public HitRegion GetHitRegion() {
			VariousPolygon polygon = GetPolygon();
			return polygon == null ? new HitRegion(Bounds) : new HitRegion(polygon);
		}
		public abstract void Offset(double dx, double dy);
		public abstract void Rotate(double angle);
	}
}

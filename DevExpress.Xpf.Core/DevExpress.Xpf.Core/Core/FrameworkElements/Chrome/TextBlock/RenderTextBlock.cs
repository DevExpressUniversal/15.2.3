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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public class RenderTextBlock : FrameworkRenderElement {
		static readonly RenderRealTextBlock RealTextBlockTemplate = new RenderRealTextBlock();
		TextTrimming textTrimming;
		TextWrapping textWrapping = TextWrapping.NoWrap;
		TextAlignment textAlignment;
		TextDecorationCollection textDecorations;
		public TextTrimming TextTrimming {
			get { return textTrimming; }
			set { SetProperty(ref textTrimming, value); }
		}
		public TextWrapping TextWrapping {
			get { return textWrapping; }
			set { SetProperty(ref textWrapping, value); }
		}
		public TextAlignment TextAlignment {
			get { return textAlignment; }
			set { SetProperty(ref textAlignment, value); }
		}
		public TextDecorationCollection TextDecorations {
			get { return textDecorations; }
			set { SetProperty(ref textDecorations, value); }
		}
		public RenderTextBlock() {
			VerticalAlignment = VerticalAlignment.Center;
			ContentSpecificClipToBounds = true;
		}
		protected override void PreApplyTemplate(FrameworkRenderElementContext context) {
			base.PreApplyTemplate(context);
			var tbContext = (RenderTextBlockContext)context;
			var textWrapping = tbContext.TextWrapping ?? TextWrapping;
			var textTrimming = tbContext.TextTrimming ?? TextTrimming;
			var textAlignment = tbContext.TextAlignment ?? TextAlignment;
			var textDecorations = tbContext.TextDecorations ?? TextDecorations;
			string highlightedText = tbContext.HighlightedText;
			if (ShouldUseRealTextBlock(tbContext, textWrapping, textTrimming, highlightedText)) {
				tbContext.RenderMode = RenderTextBlockMode.TextBlock;
				var realTBContext = tbContext.Child;
				if (realTBContext == null) {
					realTBContext = (RenderRealTextBlockContext)RealTextBlockTemplate.CreateContext(context.Namescope, context.ElementHost);
					context.AddChild(realTBContext);
				}
				realTBContext.BeginInit();
				realTBContext.HighlightedText = highlightedText;
				realTBContext.HighlightedTextCriteria = tbContext.HighlightedTextCriteria;
				realTBContext.Text = tbContext.Text;
				realTBContext.TextWrapping = textWrapping;
				realTBContext.TextTrimming = textTrimming;
				realTBContext.TextAlignment = textAlignment;
				realTBContext.TextDecorations = textDecorations;
				realTBContext.EndInit();
				return;
			}
			if (tbContext.Typeface == null)
				tbContext.Typeface = CreateTypeface(tbContext);
			if (ShouldUseFastTextBlock(tbContext, textWrapping, textTrimming, highlightedText)) {
				InitializeGLyphRunContainer(tbContext);
				return;
			}
			InitializeFormattedTextContainer(tbContext);
		}
		static void InitializeFormattedTextContainer(RenderTextBlockContext tbContext) {
			if (tbContext.FormattedTextContainer == null) {
				tbContext.FormattedTextContainer = new FormattedTextContainer();
				tbContext.FormattedTextContainer.Initialize(tbContext);
				tbContext.RenderMode = RenderTextBlockMode.FormattedText;
				return;
			}
		}
		static void InitializeGLyphRunContainer(RenderTextBlockContext tbContext) {
			if (tbContext.GlyphRunContainer == null) {
				tbContext.GlyphRunContainer = new GlyphRunContainer();
				tbContext.GlyphRunContainer.Initialize(tbContext);
				tbContext.RenderMode = RenderTextBlockMode.GlyphRun;
			}
		}
		bool ShouldUseFastTextBlock(RenderTextBlockContext tbContext, TextWrapping textWrapping1, TextTrimming textTrimming1, string highlightedText) {
			return false;
		}
		public static bool ShouldUseRealTextBlock(RenderTextBlockContext tb, TextWrapping textWrapping, TextTrimming textTrimming, string highlightedText) {
			if (tb.ForceUseRealTextBlock)
				return true;
			if (!string.IsNullOrEmpty(highlightedText))
				return true;
			return textWrapping != TextWrapping.NoWrap;
		}
		protected override Size MeasureOverride(Size availableSize, IFrameworkRenderElementContext context) {
			var tbContext = (RenderTextBlockContext)context;
			if (tbContext.RenderMode == RenderTextBlockMode.TextBlock)
				return LayoutProvider.GridInstance.MeasureOverride(availableSize, context);
			if (tbContext.RenderMode == RenderTextBlockMode.GlyphRun) {
				Size measureSize = tbContext.GlyphRunContainer.Measure(availableSize, tbContext);
				if (!(availableSize.Width.LessThan(measureSize.Width) || availableSize.Height.LessThan(measureSize.Height))) 
					return measureSize;
				tbContext.RenderMode = RenderTextBlockMode.FormattedText;
				InitializeFormattedTextContainer(tbContext);
			}
			if (tbContext.RenderMode == RenderTextBlockMode.FormattedText)
				return tbContext.FormattedTextContainer.Measure(availableSize, tbContext);
			return availableSize;
		}
		protected override Size ArrangeOverride(Size finalSize, IFrameworkRenderElementContext context) {
			var tbContext = (RenderTextBlockContext)context;
			if (tbContext.RenderMode == RenderTextBlockMode.TextBlock)
				return LayoutProvider.GridInstance.ArrangeOverride(finalSize, context);
			if (tbContext.RenderMode == RenderTextBlockMode.FormattedText)
				return tbContext.FormattedTextContainer.Arrange(finalSize, tbContext);
			if (tbContext.RenderMode == RenderTextBlockMode.GlyphRun)
				return tbContext.GlyphRunContainer.Arrange(finalSize, tbContext);
			return base.ArrangeOverride(finalSize, context);
		}
		protected override bool CalcNeedToClipSlot(Size layoutSlotSize, FrameworkRenderElementContext context) {
			return false;
		}
		bool IsTextGreaterThan(FormattedText formattedText, Size finalSize) {
			if (formattedText.Width > finalSize.Width)
				return true;
			if (formattedText.Height > finalSize.Height + 1)
				return true;
			return false;
		}
		protected override void RenderOverride(DrawingContext dc, IFrameworkRenderElementContext context) {
			base.RenderOverride(dc, context);
			var tbContext = (RenderTextBlockContext)context;
			if (tbContext.RenderMode == RenderTextBlockMode.TextBlock)
				return;
			if (tbContext.RenderMode == RenderTextBlockMode.FormattedText)
				tbContext.FormattedTextContainer.Render(dc, tbContext);
			if (tbContext.RenderMode == RenderTextBlockMode.GlyphRun)
				tbContext.GlyphRunContainer.Render(dc, tbContext);
		}
		Typeface CreateTypeface(RenderTextBlockContext context) {
			FrameworkElement root = context.ElementHost.Parent;
			return new Typeface(TextBlock.GetFontFamily(root), TextBlock.GetFontStyle(root), TextBlock.GetFontWeight(root), TextBlock.GetFontStretch(root));
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderTextBlockContext(this);
		}
	}
	public interface IFormattedTextContainer {
		bool HasCollapsedLines { get; }
		void Initialize(RenderTextBlockContext context);
		void Render(DrawingContext dc, RenderTextBlockContext context);
		Size Measure(Size availableSize, RenderTextBlockContext tbContext);
		Size Arrange(Size finalSize, RenderTextBlockContext tbContext);
	}
	public class FormattedTextContainer : IFormattedTextContainer {
		static readonly Func<FormattedText, IEnumerator> getFormattedTextEnumerator;
		static FormattedTextContainer() {
			getFormattedTextEnumerator = Internal.ReflectionHelper.CreateInstanceMethodHandler<FormattedText, Func<FormattedText, IEnumerator>>(
				null,
				"GetEnumerator",
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		}
		Size desiredSize;
		FormattedText formattedText;
		public FormattedTextContainer() {
		}
		public object Trimming { get { return formattedText.Trimming; } }
		public void Initialize(RenderTextBlockContext context) {
			formattedText = CreateFormattedText(context);
			desiredSize = new Size(formattedText.Width, formattedText.Height);
		}
		TextAlignment GetTextAlignment(RenderTextBlockContext context) {
			return context.TextAlignment ?? ((RenderTextBlock)context.Factory).TextAlignment;
		}
		TextTrimming GetTexTrimming(RenderTextBlockContext context) {
			return context.TextTrimming ?? ((RenderTextBlock)context.Factory).TextTrimming;
		}
		public void Render(DrawingContext dc, RenderTextBlockContext context) {
			Rect renderRect = context.RenderRect;
			if (renderRect.Width.AreClose(0d) || renderRect.Height.AreClose(0d))
				return;
			var textTrimming = GetTexTrimming(context);
			dc.DrawText(formattedText, CalcAnchorPointForDrawText(context, textTrimming));
		}
		Point CalcAnchorPointForDrawText(RenderTextBlockContext context, TextTrimming textTrimming) {
			Rect renderRect = context.RenderRect;
			TextAlignment textAlignment = GetTextAlignment(context);
			if (textTrimming != TextTrimming.None)
				return new Point(0, 0);
			bool isRtl = IsRtl(context);
			bool isNativeLeft = textAlignment == TextAlignment.Left || textAlignment == TextAlignment.Justify;
			bool isLeft = (isNativeLeft && !isRtl) || (textAlignment == TextAlignment.Right && isRtl);
			bool isRight = (textAlignment == TextAlignment.Right && !isRtl) || (isNativeLeft && isRtl);
			if (isLeft)
				return new Point(0, 0);
			if (isRight)
				return new Point(renderRect.Width, 0);
			return new Point(renderRect.Width / 2, 0);
		}		
		public bool HasCollapsedLines {
			get {
				var enumerator = getFormattedTextEnumerator(formattedText);
				while (enumerator.MoveNext())
					if (((System.Windows.Media.TextFormatting.TextLine)enumerator.Current).HasCollapsed)
						return true;
				return false;
			}
		}
		public Size Measure(Size availableSize, RenderTextBlockContext tbContext) {
			return new Size(formattedText.Width, formattedText.Height);
		}
		public Size Arrange(Size finalSize, RenderTextBlockContext tbContext) {
			var textTrimming = GetTexTrimming(tbContext);
			if (textTrimming != TextTrimming.None) {
				formattedText.MaxTextWidth = finalSize.Width + (Math.Ceiling(formattedText.Width) - Math.Round(formattedText.Width));
				formattedText.MaxTextHeight = finalSize.Height + 1;
			}
			return finalSize;
		}
		FormattedText CreateFormattedText(RenderTextBlockContext context) {
			string value = context.Text;
			FrameworkElement root = context.ElementHost.Parent;
			var foreground = context.Foreground ?? TextBlock.GetForeground(root);
			var ft = new FormattedText(string.IsNullOrEmpty(value) ? " " : value, System.Globalization.CultureInfo.CurrentCulture, context.FlowDirection, context.Typeface, TextBlock.GetFontSize(root),
				foreground, new NumberSubstitution() { Substitution = NumberSubstitution.GetSubstitution(root) }, TextOptions.GetTextFormattingMode(root));
			ft.Trimming = context.TextTrimming ?? ((RenderTextBlock)context.Factory).TextTrimming;
			ft.TextAlignment = GetTextAlignment(context);
			if (context.TextDecorations != null)
				ft.SetTextDecorations(context.TextDecorations);
			return ft;
		}
		bool IsRtl(RenderTextBlockContext tbContext) {
			return tbContext.FlowDirection == System.Windows.FlowDirection.RightToLeft;
		}
	}
	public class GlyphRunContainer : IFormattedTextContainer {
		GlyphRun glyphRun;
		public GlyphRunContainer() {
		}
		public Size DesiredSize { get; private set; }
		public void Initialize(RenderTextBlockContext context) {
			GlyphTypeface glyphTypeface;
			context.Typeface.TryGetGlyphTypeface(out glyphTypeface);
			context.GlyphTypeface = glyphTypeface;
			glyphRun = CreateGlyphRun(context);
		}
		GlyphRun CreateGlyphRun(RenderTextBlockContext context) {
			string text = context.Text;
			if (string.IsNullOrEmpty(text)) 
				text = " ";
			int textLength = Math.Min(text.Length, Int16.MaxValue - 1);
			FrameworkElement root = context.ElementHost.Parent;
			double fontSize = TextBlock.GetFontSize(root);
			ushort[] glyphIndexes = new ushort[textLength];
			double[] advanceWidths = new double[textLength];
			double totalWidth = 0;
			for (int n = 0; n < textLength; n++) {
				ushort glyphIndex;
				context.GlyphTypeface.CharacterToGlyphMap.TryGetValue(text[n], out glyphIndex);
				glyphIndexes[n] = glyphIndex;
				double width = context.GlyphTypeface.AdvanceWidths[glyphIndex] * fontSize;
				advanceWidths[n] = width;
				totalWidth += width;
			}
			GlyphRun run = new GlyphRun(context.GlyphTypeface,
							bidiLevel: 0,
							isSideways: false,
							renderingEmSize: fontSize,
							glyphIndices: glyphIndexes,
							baselineOrigin: new Point(0, Math.Round(context.GlyphTypeface.Baseline * fontSize)),
							advanceWidths: advanceWidths,
							glyphOffsets: null,
							characters: null,
							deviceFontName: null,
							clusterMap: null,
							caretStops: null,
							language: null);
			DesiredSize = run.ComputeAlignmentBox().Size;
			return run;
		}
		public bool HasCollapsedLines { get { return false; } }
		public void Render(DrawingContext dc, RenderTextBlockContext context) {
			if (glyphRun == null)
				return;
			Rect renderRect = context.RenderRect;
			if (renderRect.Width.AreClose(0d) || renderRect.Height.AreClose(0d))
				return;
			var foreground = context.Foreground ?? context.Factory.Foreground ?? TextBlock.GetForeground(context.ElementHost.Parent);
			Point point = CalcAnchorPointForDrawText(context);
			dc.PushTransform(new TranslateTransform(point.X, point.Y));
			dc.DrawGlyphRun(foreground, glyphRun);
			dc.Pop();
		}
		Point CalcAnchorPointForDrawText(RenderTextBlockContext context) {
			var textAlignment = GetTextAlignment(context);
			bool isRtl = IsRtl(context);
			Rect renderRect = context.RenderRect;
			double width = Math.Max(0d, renderRect.Width - DesiredSize.Width);
			bool isNativeLeft = textAlignment == TextAlignment.Left || textAlignment == TextAlignment.Justify;
			bool isLeft = (isNativeLeft && !isRtl) || (textAlignment == TextAlignment.Right && isRtl);
			bool isRight = (textAlignment == TextAlignment.Right && !isRtl) || (isNativeLeft && isRtl);
			if (isLeft)
				return new Point(0, 0);
			if (isRight)
				return new Point(width, 0);
			return new Point(width / 2d, 0);
		}
		public Size Measure(Size availableSize, RenderTextBlockContext context) {
			return DesiredSize;
		}
		public Size Arrange(Size finalSize, RenderTextBlockContext tbContext) {
			return finalSize;
		}
		TextAlignment GetTextAlignment(RenderTextBlockContext context) {
			return context.TextAlignment ?? ((RenderTextBlock)context.Factory).TextAlignment;
		}
		TextTrimming GetTexTrimming(RenderTextBlockContext context) {
			return context.TextTrimming ?? ((RenderTextBlock)context.Factory).TextTrimming;
		}
		bool IsRtl(RenderTextBlockContext tbContext) {
			return tbContext.FlowDirection == System.Windows.FlowDirection.RightToLeft;
		}
	}
}

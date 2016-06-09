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
using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native.TOC;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Native {
	[BrickExporter(typeof(TableOfContentsLineBrickExporter))]
	public class TableOfContentsLineBrick : PanelBrick {
		readonly string caption;
		ITextGenerator textGenerator;
		float textDivisionRatio = 1f;
		internal TextBrick CaptionBrick {
			get { return (TextBrick)Bricks[0]; }
		}
		internal TextBrick PageNumberBrick {
			get { return (TextBrick)Bricks[1]; }
		}
		internal string Caption {
			get { return caption; }
		}
		public override string Text {
			get { return CaptionBrick.Text + PageNumberBrick.Text; }
			set { throw new NotSupportedException("Text"); }
		}
		public float TextDivisionRatio {
			get { return textDivisionRatio; }
			set {
				if(value <= 0f || value > 1f)
					throw new ArgumentOutOfRangeException("TextDivisionRatio");
				textDivisionRatio = value;
			}
		}
		float TextDivisionPosition {
			get { return CaptionBrick.Width * TextDivisionRatio; }
		}
		[XtraSerializableProperty]
		public char LeaderSymbol {
			get;
			set;
		}
		public override string BrickType {
			get {
				return BrickTypes.TableOfContentsLine;
			}
		}
		public TableOfContentsLineBrick() : this(NullBrickOwner.Instance) { }
		public TableOfContentsLineBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			Guard.ArgumentNotNull(brickOwner, "brickOwner");
			this.caption = brickOwner.Text;
		}
		TableOfContentsLineBrick(TableOfContentsLineBrick tocLineBrick)
			: base(tocLineBrick) {
			this.caption = tocLineBrick.Caption;
			this.textDivisionRatio = tocLineBrick.TextDivisionRatio;
		}
		public override object Clone() {
			return new TableOfContentsLineBrick(this);
		}
		protected virtual ITextGenerator CreateTextGenerator(Measurer measurer, GraphicsUnit graphicsUnit, char leaderSymbol, Font font, BrickStyle style, string caption, float textDivisionPosition) {
			return new TableOfContentsLineTextGenerator() {
				Measurer = measurer,
				GraphicsUnit = graphicsUnit,
				LeaderSymbol = leaderSymbol,
				Font = font,
				Style = style,
				Caption = caption,
				TextDivisionPosition = textDivisionPosition
			};
		}
		protected internal override void PerformLayout(IPrintingSystemContext context) {
			Page associatedPage = GetAssociatedPage(context.PrintingSystem.Pages);
			if(associatedPage == null || ReferenceEquals(BrickOwner, NullBrickOwner.Instance)) return;
			if(NavigationPair != BrickPagePair.Empty && string.IsNullOrEmpty(Url)) {
				var visualBrick = NavigationPair.GetBrick(context.PrintingSystem.Pages) as VisualBrick;
				if(visualBrick != null) {
					Url = DocumentMapTreeViewNodeHelper.GetTagByIndices(BrickPagePairHelper.IndicesFromArray(NavigationPair.BrickIndices), NavigationPair.PageIndex);
					CaptionBrick.Url = Url;
					PageNumberBrick.Url = Url;
					if(!string.IsNullOrEmpty(visualBrick.AnchorName) && visualBrick.AnchorName != Url) {
						System.Diagnostics.Debug.Assert(false);
						Tracer.TraceError(NativeSR.TraceSource, new Exception("The AnchorName property has already been assigned."));
					}
					visualBrick.AnchorName = Url;
				}
			}
			Debug.Assert(textGenerator != null);
			Measurer measurer = context.Measurer;
			string pageNumberText = associatedPage.PageNumber;
			SizeF sizePageNumberBrick = measurer.MeasureString(pageNumberText, PageNumberBrick.Font, Point.Empty, PageNumberBrick.StringFormat.Value, GraphicsUnit.Document);
			PageNumberBrick.Width = (float)Math.Ceiling(PageNumberBrick.Padding.InflateWidth(sizePageNumberBrick.Width, GraphicsDpi.Document));
			PageNumberBrick.X = Width - PageNumberBrick.Width;
			CaptionBrick.Width = Width - PageNumberBrick.Width;
			float maxCaptionBrickTextWidth = CaptionBrick.Padding.DeflateWidth(CaptionBrick.Width, GraphicsDpi.Document);
			CaptionBrick.Text = textGenerator.GenerateText(maxCaptionBrickTextWidth);
			PageNumberBrick.Text = pageNumberText;
			base.PerformLayout(context);
		}
		protected virtual Page GetAssociatedPage(PageList pages) {
			return NavigationPair != null ? NavigationPair.GetPage(pages) : null;
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			Page associatedPage = GetAssociatedPage(PrintingSystem.Pages);
			if(associatedPage == null || ReferenceEquals(BrickOwner, NullBrickOwner.Instance)) return;
			base.OnSetPrintingSystem(cacheStyle);
			Initialize();
			Measurer measurer = ((IPrintingSystemContext)PrintingSystem).Measurer;
			string pageNumberText = associatedPage.PageNumber;
			SizeF sizePageNumberBrick = measurer.MeasureString(pageNumberText, Style.Font, Point.Empty, Style.StringFormat.Value, GraphicsUnit.Document);
			float pageNumberWidth = (float)Math.Ceiling(PageNumberBrick.Padding.InflateWidth(sizePageNumberBrick.Width, GraphicsDpi.Document));
			float captionWidth;
			if(pageNumberWidth > Width) {
				pageNumberWidth = 0;
				captionWidth = 0;
				PageNumberBrick.IsVisible = false;
				CaptionBrick.IsVisible = false;
			} else
				captionWidth = Width - pageNumberWidth;
			float pageNumberX = captionWidth;
			captionWidth = CaptionBrick.Padding.DeflateWidth(captionWidth, GraphicsDpi.Document);
			float desiredHeight = measurer.MeasureString(Caption, Style.Font, captionWidth, Style.StringFormat.Value, GraphicsUnit.Document).Height;
			desiredHeight = CaptionBrick.Padding.InflateHeight(desiredHeight, GraphicsDpi.Document);
			desiredHeight += 1;
			if(desiredHeight > Height)
				Height = desiredHeight;
			float captionHeight = Height;
			float pageNumberHeight = Height;
			InitialRect = new RectangleF(InitialRect.X, InitialRect.Y, InitialRect.Width, Height);
			CaptionBrick.InitialRect = new RectangleF(CaptionBrick.InitialRect.X, CaptionBrick.InitialRect.Y, captionWidth, captionHeight);
			PageNumberBrick.InitialRect = new RectangleF(pageNumberX, PageNumberBrick.InitialRect.Y, pageNumberWidth, pageNumberHeight);
			float maxCaptionBrickTextWidth = CaptionBrick.Width;
			textGenerator = CreateTextGenerator(measurer, PrintingSystem.Graph.PageUnit, LeaderSymbol, Style.Font, Style, Caption, TextDivisionPosition);
			CaptionBrick.Text = textGenerator.GenerateText(maxCaptionBrickTextWidth);
			PageNumberBrick.Text = pageNumberText;
		}
		void Initialize() {
			if(Bricks.Count == 0) {
				Bricks.Add(new TextBrick()); 
				Bricks.Add(new TextBrick()); 
			}
			PaddingInfo paddingWithBorder = GetPaddingsWithBorder(Sides, (int)BorderWidth, Padding.Dpi);
			Style.StringFormat = new BrickStringFormat(Style.StringFormat.ChangeFormatFlags(Style.StringFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces), StringTrimming.Word);
			InitializeCaptionBrick(GetPaddingCaptionBrick(paddingWithBorder));
			InitializePageNumberBrick(GetPaddingPageNumberBrick(paddingWithBorder));
			Padding = new PaddingInfo(0, Padding.Dpi);
		}
		void InitializeCaptionBrick(PaddingInfo paddingWithBorder) {
			BrickStyle newStyle = new BrickStyle(Style) { TextAlignment = TextAlignment.BottomLeft, BorderWidth = 0f, Padding = new PaddingInfo(paddingWithBorder, CaptionBrick.Padding.Dpi), StringFormat = new BrickStringFormat(Style.StringFormat, StringAlignment.Near, StringAlignment.Far) };
			CaptionBrick.Style = (BrickStyle)newStyle.Clone();
			CaptionBrick.NavigationPair = NavigationPair;
			CaptionBrick.Target = Target;
		}
		void InitializePageNumberBrick(PaddingInfo paddingWithBorder) {
			BrickStyle newStyle = new BrickStyle(Style) { TextAlignment = XtraPrinting.TextAlignment.BottomRight, BorderWidth = 0f, Padding = new PaddingInfo(paddingWithBorder, PageNumberBrick.Padding.Dpi), StringFormat = new BrickStringFormat(Style.StringFormat, StringAlignment.Far, StringAlignment.Far) };
			PageNumberBrick.Style = (BrickStyle)newStyle.Clone();
			PageNumberBrick.NavigationPair = NavigationPair;
			PageNumberBrick.Target = Target;
		}
		PaddingInfo GetPaddingsWithBorder(BorderSide sides, int borderWidth, float dpi) {
			var padding = new PaddingInfo(Padding, dpi);
			if(sides.HasFlag(BorderSide.Left))
				padding.Left += borderWidth;
			if(sides.HasFlag(BorderSide.Right))
				padding.Right += borderWidth;
			if(sides.HasFlag(BorderSide.Top))
				padding.Top += borderWidth;
			if(sides.HasFlag(BorderSide.Bottom))
				padding.Bottom += borderWidth;
			return padding;
		}
		static PaddingInfo GetPaddingCaptionBrick(PaddingInfo paddingTocLineBrick) {
			return new PaddingInfo(paddingTocLineBrick.Left, 0, paddingTocLineBrick.Top, paddingTocLineBrick.Bottom, paddingTocLineBrick.Dpi);
		}
		static PaddingInfo GetPaddingPageNumberBrick(PaddingInfo paddingTocLineBrick) {
			return new PaddingInfo(0, paddingTocLineBrick.Right, paddingTocLineBrick.Top, paddingTocLineBrick.Bottom, paddingTocLineBrick.Dpi);
		}
	}
	[BrickExporter(typeof(TextCellExporter))]
	public class TextCell : CellWrapper {
		string text;
		internal string Text { get { return text; } }
		public TextCell(ITableCell innerCell, string text)
			: base(innerCell) {
			this.text = text;
		}
	}
}
namespace DevExpress.XtraPrinting.BrickExporters {
	public class TextCellExporter : CellWrapperExporter {
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			exportProvider.SetCellText((Brick as TextCell).Text, TableCell.Url);
			exportProvider.SetNavigationUrl((VisualBrick)((CellWrapper)Brick).InnerCell);
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			exportProvider.SetCellText((Brick as TextCell).Text, TableCell.Url);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			exportProvider.SetCellText((Brick as TextCell).Text, TableCell.Url);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			exportProvider.SetCellText((Brick as TextCell).Text, null);
		}
	}
}

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
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting {
	[TypeConverter("DevExpress.XtraPrinting.Design.HeaderFooterConverter," + AssemblyInfo.SRAssemblyPrintingDesign)]
	public class PageHeaderFooter {
		private PageArea pageHeader;
		private PageArea pageFooter;
		public bool IncreaseMarginsByContent { get; set; }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageHeaderFooterHeader"),
#endif
XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PageArea Header {
			get {
				if(pageHeader == null)
					pageHeader = new PageHeaderArea();
				return pageHeader;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageHeaderFooterFooter"),
#endif
XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public PageArea Footer {
			get {
				if(pageFooter == null)
					pageFooter = new PageFooterArea();
				return pageFooter;
			}
		}
		public PageHeaderFooter() {
		}
		public PageHeaderFooter(PageHeaderArea pageHeader, PageFooterArea pageFooter) {
			this.pageHeader = pageHeader;
			this.pageFooter = pageFooter;
		}
		public override string ToString() {
			return "";
		}
		internal SizeF MeasureMarginalHeader(BrickGraphics graph, Image[] images) {
			return pageHeader != null ? pageHeader.MeasureArea(graph, images) : SizeF.Empty;
		}
		internal SizeF MeasureMarginalFooter(BrickGraphics graph, Image[] images) {
			return pageFooter != null ? pageFooter.MeasureArea(graph, images) : SizeF.Empty;
		}
		internal void CreateMarginalHeader(BrickGraphics graph, Image[] images) {
			if(pageHeader != null)
				pageHeader.CreateArea(graph, images);
		}
		internal void CreateMarginalFooter(BrickGraphics graph, Image[] images) {
			if(pageFooter != null)
				pageFooter.CreateArea(graph, images);
		}
		internal bool ShouldSerialize() {
			return ShouldSerialize(pageHeader) || ShouldSerialize(pageFooter);
		}
		protected bool ShouldSerialize(PageArea pageArea) {
			return (pageArea != null && pageArea.ShouldSerialize());
		}
	}
	[TypeConverter("DevExpress.XtraPrinting.Design.PageAreaConverter," + AssemblyInfo.SRAssemblyPrintingDesign)]
	public class PageArea {
		protected StringCollection fContent;
		protected BrickList bricks;
		protected Font fFont;
		protected BrickAlignment fLineAlignment = BrickAlignment.Near;
		protected Image[] images;
		BrickGraphics graph;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageAreaContent"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.SimpleCollection, true, true, true)
		]
		public StringCollection Content {
			get { return fContent; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageAreaLineAlignment"),
#endif
XtraSerializableProperty]
		public BrickAlignment LineAlignment {
			get { return fLineAlignment; }
			set { fLineAlignment = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PageAreaFont"),
#endif
XtraSerializableProperty]
		public Font Font {
			get {
				if(fFont == null) SetFont(DefaultFont);
				return fFont;
			}
			set { SetFont(value); }
		}
		private Font DefaultFont {
			get {
#if SL
				return new Font(DevExpress.Utils.Internal.FontManager.DefaultFontFamilyName, 8f);
#else
				return System.Windows.Forms.Control.DefaultFont; 
#endif
			}
		}
		public PageArea(string[] content, Font font, BrickAlignment lineAligment)
			: base() {
			Initialize();
			this.fContent.AddRange(content);
			this.fLineAlignment = lineAligment;
			SetFont(font);
		}
		public PageArea()
			: base() {
			Initialize();
		}
		protected virtual void Initialize() {
			fContent = new StringCollection();
			bricks = new BrickList();
		}
		private void SetFont(Font value) {
			fFont = (Font)value.Clone();
		}
		public void CreateArea(BrickGraphics graph, Image[] images) {
			try {
				AddBricks(graph, images);
				foreach(Brick item in bricks)
					graph.DrawBrick(item);
			} finally {
				bricks.Clear();
			}
		}
		public IList<Brick> GetBricks(BrickGraphics graph, Image[] images) {
			return CreateBricks(graph, images);
		}
		public bool ShouldSerialize() {
			return (fContent.Count > 0 || Font.Equals(DefaultFont) == false || fLineAlignment != BrickAlignment.Near);
		}
		internal SizeF MeasureArea(BrickGraphics graph, Image[] images) {
			try {
				AddBricks(graph, images);
				foreach(Brick item in bricks)
					item.PerformLayout(graph.PrintingSystem);
				return bricks.Bounds.Size;
			} finally {
				bricks.Clear();
			}
		}
		private void AddBricks(BrickGraphics graph, Image[] images) {
			bricks.AddRange(CreateBricks(graph, images));
		}
		private IList<Brick> CreateBricks(BrickGraphics graph, Image[] images) {
			this.images = images;
			this.graph = graph;
			List<Brick> createdBricks = new List<Brick>();
			try {
				BrickAlignment[] alignments = new BrickAlignment[] { BrickAlignment.Near, BrickAlignment.Center, BrickAlignment.Far };
				for(int i = 0; i < Math.Min(3, fContent.Count); i++) {
					Brick brick = CreateBrick(fContent[i], alignments[i], fLineAlignment);
					if(brick != null)
						createdBricks.Add(brick);
				}
				return createdBricks;
			} finally {
				graph = null;
				images = null;
			}
		}
		private Brick CreateBrick(string s, BrickAlignment aligment, BrickAlignment lineAligment) {
			PageTableBrick brick = CreateTable(s);
			if (brick != null && brick.Rows.Count > 0) {
				brick.Alignment = aligment;
				brick.LineAlignment = lineAligment;
			} else
				brick = null;
			return brick;
		}
		private PageTableBrick CreateTable(string s) {
			if(s != null && s.Length > 0) {
				PageTableBrick table = new PageTableBrick();
				string[] items = Regex.Split(s, "\r\n");
				foreach(string item in items) {
					TableRow row = CreateRow(item);
					if(row != null && row.Bricks.Count > 0)
						table.Rows.Add(row);
				}
				return table;
			}
			return null;
		}
		private TableRow CreateRow(string s) {
			TableRow row = new TableRow();
			Regex r = new Regex(@"(\[[^\[]*\])");
			string[] items = r.Split(s);
			foreach(string item in items) {
				Brick brick = CreateCell(item);
				if(brick != null) row.Bricks.Add(brick);
			}
			return row;
		}
		private Brick CreateCell(string s) {
			if(s == null || s.Length == 0)
				return null;
			Match m = Regex.Match(s, @"\[Image (?<val>\d+)\]");
			if(m.Success) {
				string val = m.Groups["val"].Value;
				int index = Convert.ToInt32(val, 10);
				if(index < images.Length) {
					PageImageBrick imageBrick = new PageImageBrick();
					imageBrick.Image = images[index];
					imageBrick.Sides = BorderSide.None;
					imageBrick.Rect = new RectangleF(0, 0, imageBrick.Image.Width, imageBrick.Image.Height);
					return imageBrick;
				}
			} else {
#if SL
				return null;
#else
				PageInfoBrick brick = new PageInfoBrick();
				brick.Sides = BorderSide.None;
				brick.Font = Font;
				brick.AutoWidth = true;
				brick.PageInfo = ToPageInfo(s);
				brick.Rect = new RectangleF(PointF.Empty, graph.Measurer.MeasureString(Measurement.FontMeasureGlyph, Font, GetPageUnit()));
				brick.Format = ToStringFormat(brick.PageInfo, s);
				brick.Style.BackColor = Color.Transparent;
				return brick;
#endif
			}
			return null;
		}
		GraphicsUnit GetPageUnit() {
			return graph != null ? graph.PageUnit : GraphicsUnit.Pixel;
		}
		private string ToStringFormat(PageInfo pageInfo, string info) {
			return pageInfo.Equals(PageInfo.DateTime) ? ToStringFormat(info) :
				pageInfo.Equals(PageInfo.NumberOfTotal) ? PreviewLocalizer.GetString(PreviewStringId.SB_PageInfo) :
				pageInfo.Equals(PageInfo.None) ? info :
				"";
		}
		private string ToStringFormat(string info) {
			return info.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageDate)) ? "{0:d}" : "{0:t}";
		}
		private PageInfo ToPageInfo(string s) {
			if(s.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageNumber)))
				return PageInfo.Number;
			else if(s.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageNumberOfTotal)))
				return PageInfo.NumberOfTotal;
			else if(s.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageTotal)))
				return PageInfo.Total;
			else if(s.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageTime)) || s.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageDate)))
				return PageInfo.DateTime;
			else if(s.Equals(PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageUserName)))
				return PageInfo.UserName;
			else return PageInfo.None;
		}
		object XtraCreateContentItem(XtraItemEventArgs e) {
			return e.Item.Value;
		}
	}
	public class PageHeaderArea : PageArea {
		public PageHeaderArea(string[] content, Font font, BrickAlignment lineAligment)
			: base(content, font, lineAligment) {
		}
		public PageHeaderArea()
			: base() {
		}
	}
	public class PageFooterArea : PageArea {
		public PageFooterArea(string[] content, Font font, BrickAlignment lineAligment)
			: base(content, font, lineAligment) {
		}
		public PageFooterArea()
			: base() {
		}
	}
}

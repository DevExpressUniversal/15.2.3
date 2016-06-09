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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text.Internal;
using System.Resources;
using System.Collections;
using System.ComponentModel;
namespace DevExpress.Utils.Text {
	public static class StringParser2 {
		public static List<StringBlock> Parse(AppearanceObject appearance, Color hyperlinkColor, string text) {
			return StringParser.Parse(Color.Empty, Color.Empty, new HyperlinkSettings() { Color = hyperlinkColor, Underline = true }, appearance.Font.Size, text, false);
		}
		public static List<StringBlock> Parse(AppearanceObject appearance, HyperlinkSettings hyperlinkSettings, string text) {
			return StringParser.Parse(Color.Empty, Color.Empty, hyperlinkSettings, appearance.Font.Size, text, false);
		}
		public static List<StringBlock> Parse(AppearanceObject appearance, string text) {
			return Parse(appearance, Color.Empty, text);
		}
	}
	public class StringInfo {
		internal string matchString = null;
		internal int matchIndex = -1;
		AppearanceObject appearance;
		List<StringBlock> blocks;
		List<Rectangle> blockBounds;
		Rectangle bounds;
		internal Rectangle originalBounds;
		object context;
		internal bool roundTextHeight;
		internal bool simpleString;
		string sourceString;
		bool allowBaselineAlignment; 
		public StringInfo() : this(null, null, string.Empty) { }
		public StringInfo(List<StringBlock> blocks, List<Rectangle> blockBounds, string sourceString) {
			this.simpleString = false;
			this.appearance = null;
			this.originalBounds = Rectangle.Empty;
			this.sourceString = sourceString;
			this.blocks = blocks;
			this.blockBounds = blockBounds;
			this.bounds = Rectangle.Empty;
			this.allowBaselineAlignment = true;
			this.HyperlinkSettings = new HyperlinkSettings();
		}
		public Rectangle OriginalBounds { get { return originalBounds; } }
		public bool SimpleString { get { return simpleString; } }
		public HyperlinkSettings HyperlinkSettings { get; set; }
		public object Context { get { return context; } set { context = value; } }
		protected internal void Assign(List<StringBlock> blocks, List<Rectangle> blockBounds, string sourceString) {
			this.sourceString = sourceString;
			this.blocks = blocks;
			this.blockBounds = blockBounds;
		}
		public void UpdateAppearanceColors(Color foreColor, Color backColor, Color backColor2) {
			Appearance.ForeColor = foreColor;
			Appearance.BackColor = backColor;
			Appearance.BackColor2 = backColor2;
		}
		public void UpdateAppearanceColors(AppearanceObject source) {
			Appearance.ForeColor = source.GetForeColor();
			Appearance.BackColor = source.GetBackColor();
			Appearance.BackColor2 = source.GetBackColor2();
			Appearance.GradientMode = source.GetGradientMode();
			Appearance.Image = source.GetImage();
			Appearance.BorderColor = source.GetBorderColor();
		}
		public StringBlock GetLinkByPoint(Point pt) {
			if(Blocks == null)
				return null;
			for(int i = 0; i < Blocks.Count; i++) {
				if(Blocks[i].Type == StringBlockType.Link && BlocksBounds[i].Contains(pt))
					return Blocks[i];
			}
			return null;
		}
		public bool HasHyperlink {
			get {
				if(Blocks == null)
					return false;
				for(int i = 0; i < Blocks.Count; i++)
					if(Blocks[i].Type == StringBlockType.Link)
						return true;
				return false;
			}
		}
		public bool IsEqualsAppearance(AppearanceObject appearance) {
			if(appearance == null)
				return false;
			return Appearance.IsEqual(appearance);
		}
		protected internal AppearanceObject Appearance { 
			get {
				if(appearance == null) appearance = new AppearanceObject();
				return appearance; 
			} 
		}
		public Rectangle Bounds { get { return bounds; } }
		public string SourceString { get { return sourceString; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public List<StringBlock> Blocks { get { return blocks; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public List<Rectangle> BlocksBounds { get { return blockBounds; } }
		public bool IsEmpty { get { return Blocks == null || Blocks.Count == 0; } }
		internal void SetBounds(Rectangle bounds) { this.bounds = bounds; }
		internal void UpdateLocation(Point pt) {
			if (BlocksBounds == null) return;
			Point delta = new Point(pt.X - Bounds.X, pt.Y - Bounds.Y);
			for(int i = 0; i < BlocksBounds.Count; i++) {
				BlocksBounds[i] = new Rectangle(new Point(BlocksBounds[i].X + delta.X, BlocksBounds[i].Y + delta.Y), BlocksBounds[i].Size);
			}
		}
		internal void UpdateXLocation(HorzAlignment align, Rectangle bounds) {
			if(!StringPainter.IsValidSize(bounds) || align == HorzAlignment.Near || align == HorzAlignment.Default || Blocks == null || BlocksBounds == null) return;
			for(int n = 0; n < Blocks.Count; ) {
				int blockCount = GetLineBlockCount(n);
				int lineWidth = GetBlocksWidth(n, blockCount);
				int newX = StringPainter.CalcXByAlignment(bounds, lineWidth, align);
				for(int b = n; b < n + blockCount; b++) {
					Rectangle blockBounds = BlocksBounds[b];
					blockBounds.X = newX;
					BlocksBounds[b] = blockBounds;
					newX += blockBounds.Width;
				}
				n += blockCount;
			}
		}
		internal int GetLineBlockCount(int startIndex) {
			int count = 0;
			int lineNo = 0;
			for(int n = startIndex; n < Blocks.Count; n++, count++) {
				if(n == startIndex) {
					lineNo = Blocks[n].LineNumber;
					continue;
				}
				if(Blocks[n].LineNumber != lineNo) break;
			}
			return count;
		}
		internal int GetBlocksWidth(int startIndex, int count) {
			if(count < 2) return BlocksBounds[startIndex].Width;
			return BlocksBounds[startIndex + count - 1].Right - BlocksBounds[startIndex].Left;
		}
		internal void Assign(StringInfo info) {
			if (Blocks == null) {
				this.blocks = new List<StringBlock>();
				this.blockBounds = new List<Rectangle>();
			}
			Blocks.Clear();
			BlocksBounds.Clear();
			this.appearance = info.Appearance.Clone() as AppearanceObject;
			this.sourceString = info.SourceString;
			this.bounds = info.Bounds;
			this.simpleString = info.simpleString;
			this.originalBounds = info.originalBounds;
			this.allowBaselineAlignment = info.AllowBaselineAlignment;
			for(int n = 0; n < (info.Blocks == null ? 0 : info.Blocks.Count); n++) {
				Blocks.Add(info.Blocks[n]);
				BlocksBounds.Add(info.BlocksBounds[n]);
			}
		}
		public void SetLocation(Point pt) {
			SetLocation(pt.X, pt.Y);
		}
		public void SetLocation(int x, int y) {
			int dx = x - Bounds.X;
			int dy = y - Bounds.Y;
			Offset(dx, dy);
			SetBounds(new Rectangle(x, y, Bounds.Width, Bounds.Height));
		}
		public void Offset(int x, int y) {
			if(originalBounds.IsEmpty) return;
			this.originalBounds.Offset(x, y);
			SetBounds(new Rectangle(Bounds.X + x, Bounds.Y + y, Bounds.Width, Bounds.Height));
			if (BlocksBounds == null) return;
			for(int n = 0; n < BlocksBounds.Count; n++) {
				Rectangle r = BlocksBounds[n];
				r.Offset(x, y);
				BlocksBounds[n] = r;
			}
		}
		internal bool AllowBaselineAlignment { get { return allowBaselineAlignment; }  set { allowBaselineAlignment = value; }}
		internal void SetEmpty() {
			Assign(new List<StringBlock>(), new List<Rectangle>(), string.Empty);
		}
	}
	public class StringCalculateArgs {
		Graphics graphics;
		AppearanceObject appearance;
		TextOptions defaultTextOptions;
		string text;
		Rectangle bounds;
		XPaint painter;
		object context;
		bool allowSimpleString;
		bool allowBaselineAlignment; 
		public StringCalculateArgs(Graphics graphics, AppearanceObject appearance, string text, Rectangle bounds, XPaint painter) :
			this(graphics, appearance, (TextOptions)null, text, bounds, painter) { }
		public StringCalculateArgs(Graphics graphics, AppearanceObject appearance, TextOptions defaultTextOptions, string text, Rectangle bounds, XPaint painter) :
			this(graphics, appearance, defaultTextOptions, text, bounds, painter, null) { }
		public StringCalculateArgs(Graphics graphics, AppearanceObject appearance, string text, Rectangle bounds, object context) :
			this(graphics, appearance, null, text, bounds, null, context) { }
		public StringCalculateArgs(Graphics graphics, AppearanceObject appearance, TextOptions defaultTextOptions, string text, Rectangle bounds, XPaint painter, object context) {
			this.graphics = graphics;
			this.appearance = appearance;
			this.defaultTextOptions = defaultTextOptions;
			this.text = text;
			this.bounds = bounds;
			this.painter = painter;
			this.context = context;
			this.allowSimpleString = true;
			this.allowBaselineAlignment = true;
			HyperlinkSettings = new HyperlinkSettings();
		}
		public HyperlinkSettings HyperlinkSettings { get; set; }
		public Color HyperlinkColor { get { return HyperlinkSettings.Color; } set { HyperlinkSettings.Color = value; } }
		public bool RoundTextHeight { get; set; }
		public bool AllowSimpleString { get { return allowSimpleString; } set { allowSimpleString = value; } }
		public bool AllowBaselineAlignment { get { return allowBaselineAlignment; } set { allowBaselineAlignment = value; } }
		public Graphics Graphics { get { return graphics; } }
		public AppearanceObject Appearance { get { return appearance; } }
		public TextOptions DefaultTextOptions { get { return defaultTextOptions; } }
		public string Text { get { return text; } }
		public Rectangle Bounds { get { return bounds; } }
		public XPaint Painter { 
			get {
				if(painter == null) painter = XPaint.Graphics;
				return painter; 
			} 
		}
		public object Context { get { return context; } }
	}
	public class StringPainter {
		static StringPainter defaultPainter;
		public static StringPainter Default {
			get {
				if(defaultPainter == null) defaultPainter = new StringPainter();
				return defaultPainter; 
			}
		}
		bool IsSimpleStringCore(string text) {
			if(string.IsNullOrEmpty(text)) return true;
			if(text.IndexOf('<') < 0) return text.IndexOf('&') < 0;
			return false;
		}
		[ThreadStatic]
		static AppearanceObject dummy;
		protected static AppearanceObject Dummy {
			get {
				if(dummy == null) dummy = new AppearanceObject();
				return dummy;
			}
		}
		internal Font GetFont(Font font, StringFontSettings settings) {
			return ResourceCache.DefaultCache.GetFont(font, settings.Size < 1 ? font.Size : settings.Size,
				settings.IsStyleSet ? settings.Style : font.Style);
		}
		internal void SetFont(StringBlock block, Graphics graphics, Font font) {
			Graphics g = GraphicsInfo.Default.AddGraphics(graphics);
			try {
				Dummy.Font = font;
				block.SetFontInfo(font, Dummy.CalcDefaultTextSize(g).Height, DevExpress.Utils.Text.TextUtils.GetFontAscentHeight(g, font));
			}
			finally {
				GraphicsInfo.Default.ReleaseGraphics();
			}
		}
		public bool IsSimpleString(string text) { return IsSimpleStringCore(text); }
		public string RemoveFormat(string text) { return RemoveFormat(text, false); }
		public string RemoveFormat(string text, bool allowNewLine) {
			if(IsSimpleStringCore(text)) return text;
			List<StringBlock> strings = StringParser.Parse(12, text, allowNewLine);
			if(strings == null || strings.Count == 0) return string.Empty;
			if(strings.Count == 1 && (strings[0].Type == StringBlockType.Text || strings[0].Type == StringBlockType.Link)) return strings[0].Text;
			StringBuilder sb = new StringBuilder();
			int lastLineNumber = 0;
			foreach(StringBlock block in strings) {
				if(block.Type == StringBlockType.Text || block.Type == StringBlockType.Link) {
					if(lastLineNumber != block.LineNumber && allowNewLine) {
						sb.Append("\r\n");
						lastLineNumber = block.LineNumber;
					}
					sb.Append(block.Text);
				}
			}
			return sb.ToString();
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, string text, int maxWidth, XPaint painter) {
			return Calculate(graphics, appearance, text, new Rectangle(0, 0, maxWidth, 0), painter);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, string text, int maxWidth) {
			return Calculate(graphics, appearance, text, new Rectangle(0, 0, maxWidth, 0));
		}
		public StringInfo DrawString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds) {
			return DrawString(cache, appearance, text, bounds, null);
		}
		public StringInfo DrawString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds, TextOptions defaultOptions) {
			return DrawString(cache, appearance, text, bounds, defaultOptions, null);
		}
		public StringInfo DrawString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds, TextOptions defaultOptions, object context) {
			if(bounds.Width < 1 || bounds.Height < 1) {
				StringInfo res = new StringInfo();
				res.SetEmpty();
				return res;
			}
			StringInfo info = Calculate(cache.Graphics, appearance, defaultOptions, text, bounds, cache.Paint, context);
			DrawString(cache, info, defaultOptions);
			return info;
		}
		public void DrawString(GraphicsCache cache, StringInfo info) {
			DrawString(cache, info, null);
		}
		public void DrawString(GraphicsCache cache, StringInfo info, TextOptions defaultOptions) {
			if(info.IsEmpty && !info.SimpleString) return;
			StringFormat sf = info.Appearance.GetStringFormat(defaultOptions);
			if(info.SimpleString) {
				Rectangle textBounds = info.originalBounds;
				if(textBounds.Height < 1) textBounds.Height = info.Bounds.Height;
				if(textBounds.Width < 1) textBounds.Width = info.Bounds.Width;
				Color color = info.Appearance.GetForeColor();
				if(info.matchIndex > -1 && info.matchString != null) {
					DrawSimpleMatchString(cache, info.SourceString, info.Appearance.GetFont(), color, textBounds, sf, info, SystemColors.Highlight, SystemColors.HighlightText); 
				} else {
					DrawSimpleString(cache, info.SourceString, info.Appearance.GetFont(), color, textBounds, sf);
				}
				return;
			}
			StringAlignment prevAlignment = sf.Alignment;
			sf.Alignment = StringAlignment.Near;
			try {
				for(int n = 0; n < info.Blocks.Count; n++) {
					StringBlock sb = info.Blocks[n];
					Rectangle r = info.BlocksBounds[n];
					Color color = sb.FontSettings.Color;
					Color backColor = sb.FontSettings.BackColor;
					if(color.IsEmpty) color = info.Appearance.GetForeColor();
					if(!backColor.IsEmpty) cache.FillRectangle(backColor, r);
					DrawStringBlock(cache, info.Context, sf, sb, r, color);
				}
			}
			finally {
				sf.Alignment = prevAlignment;
			}
		}
		protected virtual Size CalcStringBlockSize(StringBlock block, Graphics g, XPaint painter, string s, StringFormat format) {
			return painter.CalcTextSizeInt(g, s, block.Font, format, 0);
		}
		protected virtual void DrawSimpleMatchString(GraphicsCache cache, string text, Font font, Color color, Rectangle rect, StringFormat format, StringInfo info, Color highlight, Color highlightText) {
			AppearanceObject app = new AppearanceObject();
			app.Font = font;
			app.ForeColor = color;
			cache.Paint.DrawMultiColorString(cache, rect, text, info.matchString, app, format, highlightText, highlight, false, info.matchIndex);
		}
		protected virtual void DrawSimpleString(GraphicsCache cache, string text, Font font, Color color, Rectangle rect, StringFormat format) {
			cache.DrawString(text, font, cache.GetSolidBrush(color), rect, format);
		}
		protected virtual void DrawStringBlock(GraphicsCache cache, object context, StringFormat format, StringBlock sb, Rectangle rect, Color color) {
			switch(sb.Type) {
				case StringBlockType.Text:
				case StringBlockType.Link:
					if(sb.MatchText != null && sb.MatchIndex > -1) {
						AppearanceObject app = new AppearanceObject();
						app.Font = sb.Font;
						app.ForeColor = color;
						cache.Paint.DrawMultiColorString(cache, rect, sb.Text, sb.MatchText, app, format, sb.MatchFore, sb.MatchBack, false, sb.MatchIndex);
					}
					else {
						cache.Paint.DrawString(cache, sb.Text, sb.Font, cache.GetSolidBrush(color), rect, format);
					}
					break;
				case StringBlockType.Image:
					Image i = GetImage(context, sb.ImageName);
					if (i != null) {
						Size iSize = sb.Size.IsEmpty ? i.Size : sb.Size;
						Point iLoc = Point.Empty;
						if(rect.Width < iSize.Width) {
							iLoc.X = (iSize.Width - rect.Width) / 2;
							iSize.Width = rect.Width;
						}
						cache.Paint.DrawImage(cache.Graphics, i, rect, iLoc.X, iLoc.Y, iSize.Width, iSize.Height, GraphicsUnit.Pixel, null);
					}
					else {
						cache.DrawRectangle(cache.GetPen(Color.Gray, 2), rect);
					}
					break;
			}
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, string text, Rectangle bounds) {
			return Calculate(graphics, appearance, appearance.GetTextOptions(), text, bounds);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, Color hyperlinkColor, string text, Rectangle bounds) {
			return Calculate(graphics, appearance, new HyperlinkSettings() { Color = hyperlinkColor }, text, bounds);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, HyperlinkSettings hyperlinkSettings, string text, Rectangle bounds) {
			return Calculate(graphics, appearance, appearance.GetTextOptions(), hyperlinkSettings, text, bounds);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, string text, Rectangle bounds, XPaint painter) {
			return Calculate(graphics, appearance, null, text, bounds, painter);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, Color hyperlinkColor, string text, Rectangle bounds, XPaint painter) {
			return Calculate(graphics, appearance, null, hyperlinkColor, text, bounds, painter);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, string text, Rectangle bounds) {
			return Calculate(graphics, appearance, textOptions, text, bounds, null);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, Color hyperlinkColor, string text, Rectangle bounds) {
			return Calculate(graphics, appearance, textOptions, new HyperlinkSettings() { Color = hyperlinkColor }, text, bounds, null);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, HyperlinkSettings hyperlinkSettings, string text, Rectangle bounds) {
			return Calculate(graphics, appearance, textOptions, hyperlinkSettings, text, bounds, null);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, string text, Rectangle bounds, XPaint painter) {
			return Calculate(graphics, appearance, textOptions, Color.Blue, text, bounds, painter);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, Color hyperlinkColor, string text, Rectangle bounds, XPaint painter) {
			return Calculate(graphics, appearance, textOptions, new HyperlinkSettings() { Color = hyperlinkColor }, text, bounds, painter);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, HyperlinkSettings hyperlinkSettings, string text, Rectangle bounds, XPaint painter) {
			return Calculate(new StringCalculateArgs(graphics, appearance, textOptions, text, bounds, painter) { HyperlinkSettings = hyperlinkSettings });
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, string text, Rectangle bounds, XPaint painter, object context) {
			return Calculate(new StringCalculateArgs(graphics, appearance, textOptions, text, bounds, painter, context));
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, Color hyperlinkColor, string text, Rectangle bounds, XPaint painter, object context) {
			return Calculate(new StringCalculateArgs(graphics, appearance, textOptions, text, bounds, painter, context) { HyperlinkColor = hyperlinkColor });
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, string text, int textWidth, XPaint painter, object context) {
			return Calculate(new StringCalculateArgs(graphics, appearance, textOptions, text, new Rectangle(0, 0, textWidth, 0), painter, context));
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, Color hyperlinkColor, string text, int textWidth, XPaint painter, object context) {
			return Calculate(graphics, appearance, textOptions, new HyperlinkSettings() { Color = hyperlinkColor }, text, textWidth, painter, context);
		}
		public StringInfo Calculate(Graphics graphics, AppearanceObject appearance, TextOptions textOptions, HyperlinkSettings hyperlinkSettings, string text, int textWidth, XPaint painter, object context) {
			return Calculate(new StringCalculateArgs(graphics, appearance, textOptions, text, new Rectangle(0, 0, textWidth, 0), painter, context) { HyperlinkSettings = hyperlinkSettings });
		}
		public virtual StringInfo Calculate(StringCalculateArgs e) {
			StringInfo info = new StringInfo();
			info.roundTextHeight = e.RoundTextHeight;
			info.HyperlinkSettings = e.HyperlinkSettings;
			info.AllowBaselineAlignment = e.AllowBaselineAlignment;
			info.Context = e.Context;
			info.Appearance.Assign(e.Appearance);
			info.Appearance.TextOptions.RightToLeft = e.Appearance.TextOptions.RightToLeft; 
			Rectangle bounds = e.Bounds;
			if(e.DefaultTextOptions != null) info.Appearance.TextOptions.UpdateDefaultOptions(e.DefaultTextOptions);
			if(e.AllowSimpleString && IsSimpleStringCore(e.Text)) {
				SetupSimpleString(info, e);
				return info;
			}
			List<StringBlock> strings = Parse(info.Appearance, info.HyperlinkSettings, e.Text);
			UpdateFont(e.Graphics, strings, info.Appearance.Font);
			List<StringBlock> blocks = new List<StringBlock>();
			List<Rectangle> blocksBounds = new List<Rectangle>();
			info.Assign(blocks, blocksBounds, e.Text);
			info.originalBounds = bounds;
			if(strings.Count == 0) return info;
			TextProcessInfo te = new TextProcessInfo();
			te.RoundTextHeight = e.RoundTextHeight;
			te.AllowMultiLine = info.Appearance.TextOptions.WordWrap == WordWrap.Wrap;
			te.CurrentPosition = bounds.Location;
			te.Graphics = e.Graphics;
			te.Info = info;
			te.Bounds = bounds;
			te.Paint = e.Painter == null ? XPaint.Graphics : e.Painter;
			te.Context = e.Context;
			int index = 0;
			foreach(StringBlock str in strings) {
				if(te.LineHeight == 0) te.LineHeight = GetBlockHeight(info.Context, str, te.RoundTextHeight);
				te.Block = str;
				ProcessBlock(te, strings, index);
				index++;
			}
			UpdateBaseLine(info);
			UpdateMaximumBounds(info, bounds);
			info.SetBounds(Rectangle.FromLTRB(bounds.X, bounds.Y, te.End.X, te.End.Y));
			UpdateBoundsByAppearance(info, bounds);
			return info;
		}
		void SetupSimpleString(StringInfo info, StringCalculateArgs e) {
			Size size = e.Painter.CalcTextSizeInt(e.Graphics, e.Text, info.Appearance.GetFont(), info.Appearance.GetStringFormat(), e.Bounds.Width);
			info.simpleString = true;
			info.SetBounds(new Rectangle(e.Bounds.Location, size));
			info.originalBounds = e.Bounds;
			info.Assign(null, null, e.Text);
			return;
		}
		void ProcessBlock(TextProcessInfo te, List<StringBlock> strings, int index) {
			switch(te.Block.Type) {
				case StringBlockType.Text:
				case StringBlockType.Link:
					ProcessText(te, strings, index); 
					return;
				case StringBlockType.Image:
					ProcessText(te, strings, index);
					return;
				default:
					throw new Exception(string.Format("Not implemented {0} block type",  te.Block.Type));
			}
		}
		protected Image GetImage(object context, string id) {
			if(string.IsNullOrEmpty(id)) return null;
			if(id[0] == '#') return ResourceImageProvider.Current.GetImage(context, id);
			IStringImageProvider ip = context as IStringImageProvider;
			if(ip != null) return ip.GetImage(id);
			return null;
		}
		Size GetBlockSize(object context, StringBlock block) {
			switch(block.Type) {
				case StringBlockType.Image:
					Size size = block.Size;
					if(size.IsEmpty) {
						Image i = GetImage(context, block.ImageName);
						return i == null ? Size.Empty : i.Size;
					}
					return size;
				default:
					throw new ArgumentException("Unsupported block type", "block.Type");
			}
		}
		int GetBlockHeight(object context, StringBlock block, bool roundTextHeight) {
			switch(block.Type) {
				case StringBlockType.Text:
				case StringBlockType.Link:
					int res = block.FontHeight;
					if(roundTextHeight && (res % 2) == 1) res++;
					return res;
				case StringBlockType.Image:
					return GetBlockSize(context, block).Height;
			}
			return 0;
		}
		class MatchInfo {
			public StringBlock Block;
			public int StartIndex;
			public string Text;
		}
		void ResetMatch(StringInfo info) {
			info.matchIndex = -1;
			info.matchString = null;
			if(info.Blocks != null) {
				foreach(StringBlock b in info.Blocks) b.SetMatchInfo(null, -1);
			}
		}
		public bool CheckApplyMatch(StringInfo info, bool caseInsensitive, string match) {
			ResetMatch(info);
			if(string.IsNullOrEmpty(match)) return false;
			if(info.SimpleString) {
				return CheckApplySimpleStringMatch(info, caseInsensitive, match);
			}
			var mc = FindMatch(info, true, match);
			if(mc == null || mc.Count < 1) return false;
			foreach(var m in mc) {
				m.Block.SetMatchInfo(m.Text, m.StartIndex);
				m.Block.SetMatchColorInfo(SystemColors.HighlightText, SystemColors.Highlight); 
			}
			return true;
		}
		protected bool CheckApplySimpleStringMatch(StringInfo info, bool caseInsensitive, string match) {
			if(string.IsNullOrEmpty(info.SourceString) || string.IsNullOrEmpty(match)) return false;
			string source = caseInsensitive ? info.SourceString.ToLower() : info.SourceString;
			if(caseInsensitive) match = match.ToLower();
			info.matchIndex = source.IndexOf(match);
			if(info.matchIndex > -1) info.matchString = match;
			return info.matchIndex > -1;
		}
		List<MatchInfo> FindMatch(StringInfo info, bool caseInsensitive, string match) {
			List<MatchInfo> mi = null;
			if(caseInsensitive) match = match.ToLower();
			for(int n = 0; n < info.Blocks.Count; n++) {
				StringBlock b = info.Blocks[n];
				if(b.Type != StringBlockType.Text) continue;
				for(int c = 0; c < b.Text.Length; c++) {
					char ch = b.Text[c];
					if(caseInsensitive) ch = Char.ToLower(ch);
					if(ch == match[0]) {
						if(mi == null) mi = new List<MatchInfo>();
						mi.Clear();
						if(Match(info, caseInsensitive, n, c, match, mi)) {
							return mi;
						}
					}
				}
			}
			return null;
		}
		bool Match(StringInfo info, bool caseInsensitive, int block, int cNumber, string match, List<MatchInfo> mi) {
			int position = 0;
			StringBuilder sb = new StringBuilder();
			for(int n = block; n < info.Blocks.Count; n++) {
				StringBlock b = info.Blocks[n];
				MatchInfo m = new MatchInfo() { Block = b, Text = "", StartIndex = cNumber };
				for(int c = cNumber; c < b.Text.Length; c++) {
					char targetChar = match[position];
					char ch = b.Text[c];
					if(caseInsensitive) ch = Char.ToLower(ch);
					if(targetChar != ch) return false;
					sb.Append(b.Text[c]);
					if(++position >= match.Length) {
						m.Text = sb.ToString();
						mi.Add(m);
						return true;
					}
				}
				m.Text = sb.ToString();
				if(!string.IsNullOrEmpty(m.Text)) {
					mi.Add(m);
				}
				sb.Clear();
				cNumber = 0;
			}
			return false;
		}
		public class TextProcessInfo {
			public TextProcessInfo() {
				this.LineHeight = this.LineNumber = 0;
				this.End = Point.Empty;
				this.IsNewLine = true;
				this.Paint = XPaint.Graphics;
			}
			public XPaint Paint;
			public object Context;
			public StringInfo Info;
			public StringBlock Block;
			public Graphics Graphics;
			public bool AllowMultiLine;
			public Rectangle Bounds;
			public Point End;
			public int LineHeight;
			public int LineNumber;
			public bool IsNewLine;
			public Point CurrentPosition;
			public int CurrentX {
				get { return CurrentPosition.X; }
				set { CurrentPosition.X = value; }
			}
			public int CurrentY {
				get { return CurrentPosition.Y; }
				set { CurrentPosition.Y = value; }
			}
			public bool RoundTextHeight { get; set; }
		}
		void UpdateMaximumBounds(StringInfo info, Rectangle bounds) {
			if(!IsValidSize(bounds)) return;
			for(int n = info.BlocksBounds.Count - 1; n >= 0; n--) {
				Rectangle block = info.BlocksBounds[n];
				if(block.Right > bounds.Right) {
					block.Width = bounds.Right - block.X;
					if(block.Width < 1) block = Rectangle.Empty;
				}
				if(block.Bottom > bounds.Bottom) {
					block.Height = bounds.Bottom - block.Y;
					if(block.Height < 1) block = Rectangle.Empty;
				}
				if(block.IsEmpty) {
					info.BlocksBounds.RemoveAt(n);
					info.Blocks.RemoveAt(n);
					continue;
				}
				info.BlocksBounds[n] = block;
			}
		}
		public void UpdateLocation(StringInfo info, Rectangle bounds) {
			if(bounds.Size != info.originalBounds.Size) {
				if(bounds.Size == info.Bounds.Size) {
					info.originalBounds.Size = bounds.Size;
					UpdateBoundsByAppearance(info, bounds);
				}
				else {
					Graphics g = GraphicsInfo.Default.AddGraphics(null);
					try {
						StringInfo newInfo = Calculate(new StringCalculateArgs(g, info.Appearance, null, info.SourceString, bounds, null, info.Context) { HyperlinkSettings = info.HyperlinkSettings, RoundTextHeight = info.roundTextHeight });
						info.Assign(newInfo);
					}
					finally {
						GraphicsInfo.Default.ReleaseGraphics();
					}
				}
			}
			UpdateLocation(info, bounds.Location);
		}
		public void UpdateLocation(StringInfo info, Point location) {
			if(location != info.originalBounds.Location) {
				UpdateBoundsByAppearance(info, new Rectangle(location, info.originalBounds.Size));
			}
		}
		HorzAlignment CheckRTLHAlignment(AppearanceObject appearance) {
			var res = appearance.HAlignment;
			if(appearance.TextOptions.RightToLeft) {
				switch(res) {
					case HorzAlignment.Default:
					case HorzAlignment.Near: return HorzAlignment.Far;
					case HorzAlignment.Far: return HorzAlignment.Near;
				}
			}
			return res;
		}
		void UpdateBoundsByAppearance(StringInfo info, Rectangle bounds) {
			int x = CalcXByAlignment(bounds, info.Bounds.Width, CheckRTLHAlignment(info.Appearance));
			int y = CalcYByAppearance(info.Appearance.TextOptions.VAlignment, bounds, info.Bounds.Height, false);
			if(bounds.Width < 1) x = bounds.X;
			if(bounds.Height < 1) y = bounds.Y;
			info.UpdateLocation(new Point(x, y));
			if(bounds.Width > 0) info.UpdateXLocation(CheckRTLHAlignment(info.Appearance), bounds);
			info.originalBounds = bounds;
		}
		int CalcYByAppearance(VertAlignment align, Rectangle bounds, int height, bool reverseHeight) {
			int delta = reverseHeight ? (height - bounds.Height) : (bounds.Height - height);
			int y = bounds.Y;
			if(delta < 1) return y;
			if(align == VertAlignment.Center || align == VertAlignment.Default) y += delta / 2;
			if(align == VertAlignment.Bottom) y += delta;
			return y;
		}
		internal static int CalcXByAlignment(Rectangle totalBounds, int textWidth, HorzAlignment align) {
			int delta = totalBounds.Width - textWidth;
			int x = totalBounds.X;
			if(delta < 1) return x;
			if(align == HorzAlignment.Center) x += delta / 2 + (delta % 2);
			if(align == HorzAlignment.Far) x += delta;
			return x;
		}
		bool StartsWithNbsp(string text) {
			return text.Length > 0 && text[0] == 0xa0;
		}
		internal void ProcessText(TextProcessInfo te, List<StringBlock> strings, int index) {
			int maxWidth = te.Bounds.Width;
			if(maxWidth <= 0) maxWidth = int.MaxValue;
			string drawText = te.Block.Text;
			bool textWasSplit = false;
			bool isImageWithText = false;
			while(drawText.Length > 0 && drawText[0] != '\0') {
				te.IsNewLine = te.Bounds.X == te.CurrentX || !te.AllowMultiLine;
				int textWidth = 0;
				string words = string.Empty;
				int connectedWidth = CalcConnectedWidth(te, strings, index);
				isImageWithText = te.Block.Type == StringBlockType.Image && connectedWidth > 0;
				if(te.Block.Type == StringBlockType.Text || te.Block.Type == StringBlockType.Link || isImageWithText) {
					int availableWidth = maxWidth - (te.CurrentX - te.Bounds.X);
					words = GetNextWords(te, drawText, availableWidth, out textWidth, connectedWidth);
					if(te.AllowMultiLine) {
						if(textWasSplit || ((words == "" || words[0] < ' ') && !StartsWithNbsp(drawText))) {
							textWasSplit = false;
							drawText = ProcessEmptyWord(te, drawText);
							continue;
						}
						if(words == "" && StartsWithNbsp(drawText)) {
							textWidth = GetStringWidth(te, drawText);
							words = drawText;
						}
						if(te.AllowMultiLine && !TextFit(te, textWidth, maxWidth, connectedWidth)) {
							if(isImageWithText) {
								if(te.CurrentX != te.Bounds.X) {
									textWasSplit = false;
									drawText = ProcessEmptyWord(te, drawText);
									continue;
								}
							}
							else {
								string fit = GetTextFitPart(te, drawText, availableWidth);
								if(fit != string.Empty) {
									words = fit;
									textWidth = GetStringWidth(te, words);
									textWasSplit = true;
								}
							}
						}
					}
					if(te.Block.Type == StringBlockType.Image)
						textWidth = GetBlockSize(te.Context, te.Block).Width;
				}
				else {
					words = te.Block.Text;
					textWidth = GetBlockSize(te.Context, te.Block).Width;
				}
				int blockHeight = GetBlockHeight(te.Context, te.Block, te.RoundTextHeight);
				if(te.IsNewLine)
					te.LineHeight = blockHeight;
				else
					te.LineHeight = Math.Max(blockHeight, te.LineHeight);
				Rectangle lastBlockBounds = CreateNewBlock(te, textWidth, words);
				if(!te.AllowMultiLine && maxWidth != int.MaxValue && te.CurrentX >= te.Bounds.X + maxWidth) {
					lastBlockBounds.Width = Math.Max(0, (te.Bounds.X + maxWidth) - lastBlockBounds.X);
					te.End.X = lastBlockBounds.Right;
					te.CurrentX = lastBlockBounds.Right;
					te.Info.BlocksBounds[te.Info.BlocksBounds.Count - 1] = lastBlockBounds;
					break;
				}
				te.End.X = Math.Min(te.End.X, te.Bounds.X + maxWidth);
				if((te.Block.Type != StringBlockType.Text && te.Block.Type != StringBlockType.Link) || words.Length == drawText.Length) break;
				drawText = drawText.Substring(words.Length, drawText.Length - words.Length);
			}
		}
		private string GetTextFitPart(TextProcessInfo te, string drawText, int maxWidth) {
			int startIndex = 0;
			int endIndex = drawText.Length;
			int index = 0;
			int width = 0;
			for(int i = 0; i < 32; i++) {
				if(endIndex - startIndex <= 1)
					break;
				index = (endIndex + startIndex) / 2;
				string calcText = drawText.Substring(0, index);
				width = GetStringWidth(te, calcText);
				if(width == maxWidth) {
					break;
				}
				if(width > maxWidth)
					endIndex = index;
				else
					startIndex = index;
			}
			if(width > maxWidth)
				index = startIndex;
			return drawText.Substring(0, index);
		}
		private bool TextFit(TextProcessInfo te, int textWidth, int maxWidth, int separatorWidth) {
			return (te.CurrentX - te.Bounds.X) + textWidth + separatorWidth <= maxWidth;
		}
		bool IsSeparator(StringBlock block) {
			return block.Text != null && block.Text.Length == 1 && char.GetUnicodeCategory(block.Text, 0) == System.Globalization.UnicodeCategory.OtherPunctuation;
		}
		private int CalcConnectedWidth(TextProcessInfo te, List<StringBlock> strings, int index) {
			int res = CalcSeparatorsWidth(te, strings, index);
			res += CalcNbspTextWidth(te, strings, index);
			return res;
		}
		private int CalcNbspTextWidth(TextProcessInfo te, List<StringBlock> strings, int index) {
			int res = 0;
			if(strings == null || index == strings.Count - 1)
				return res;
			StringBlock block = strings[index + 1];
			if(block.Text.Length > 0 && block.Text[0] == 0xa0) {
				StringBlock prev = te.Block;
				te.Block = block;
				res = GetStringWidth(te, block.Text);
				te.Block = prev;
			}
			return res;
		}
		private int CalcSeparatorsWidth(TextProcessInfo te, List<StringBlock> strings, int index) {
			if(strings == null)
				return 0;
			int res = 0;
			for(int i = index + 1; i < strings.Count; i++) { 
				if(!IsSeparator(strings[i]))
					break;
				StringBlock prev = te.Block;
				te.Block = strings[i];
				res += GetStringWidth(te, strings[i].Text);
				te.Block = prev;
			}
			return res;
		}
		Rectangle CreateNewBlock(TextProcessInfo te, int textWidth, string words) {
			StringBlock newBlock = new StringBlock();
			newBlock.Text = words;
			newBlock.SetBlock(te.Block);
			newBlock.LineNumber = te.LineNumber;
			newBlock.Link = te.Block.Link;
			te.Info.Blocks.Add(newBlock);
			Rectangle lastBlock = new Rectangle(te.CurrentPosition, new Size(textWidth, GetBlockHeight(te.Context, te.Block, te.RoundTextHeight)));
			te.Info.BlocksBounds.Add(lastBlock);
			UpdateBlockAscentHeight(newBlock, lastBlock);
			te.CurrentX += textWidth;
			te.End.Y = Math.Max(te.End.Y, te.CurrentY + te.LineHeight);
			te.End.X = Math.Max(te.End.X, te.CurrentX);
			return lastBlock;
		}
		void UpdateBlockAscentHeight(StringBlock newBlock, Rectangle lastBlock) {
			if(newBlock.Type == StringBlockType.Image) {
				if(lastBlock.Height > 0) newBlock.SetAscentHeight((int)(lastBlock.Height * 0.84f));
			}
		}
		void RemoveControlChars(ref string drawText) {
			while(drawText.Length > 0 && drawText[0] != (char)13 && drawText[0] < ' ')
				drawText = drawText.Remove(0, 1);
		}
		string ProcessEmptyWord(TextProcessInfo te, string drawText) {
			bool trimText = true;
			if(drawText.Length > 0 && drawText[0] == (char)13) {
				while(drawText.Length > 0 && drawText[0] == (char)13) {
					drawText = drawText.Remove(0, 1);
					RemoveControlChars(ref drawText);
					te.CurrentY += te.LineHeight;
				}
				trimText = false;
			}
			else {
				te.CurrentY += te.LineHeight;
			}
			RemoveControlChars(ref drawText);
			te.LineNumber++;
			te.CurrentX = te.Bounds.X;
			te.IsNewLine = true;
			if(trimText) drawText = drawText.TrimStart(' ', (char)160);
			return drawText;
		}
		void UpdateBaseLine(StringInfo info) {
			if(info.Blocks.Count < 2) return;
			int count;
			for(int n = 0; n < info.Blocks.Count;) {
				int ascentHeight;
				int lineHeight = GetLineHeight(info, n, out count, out ascentHeight, info.roundTextHeight);
				ascentHeight = Math.Min(ascentHeight, lineHeight);
				for(int l = n; l < n + count; l++) {
					Rectangle bounds = info.BlocksBounds[l];
					StringBlock b = info.Blocks[l];
					if (b.Type != StringBlockType.Text) {
						bounds = UpdateBlockBoundsByAlign(b, bounds, lineHeight);
					}
					else {
						VertAlignment valign = info.Appearance.GetTextOptions().VAlignment;
						if(valign == VertAlignment.Center || valign == VertAlignment.Default || !info.AllowBaselineAlignment) {
							bounds.Y += ascentHeight - info.Blocks[l].FontAscentHeight;
						}
						else {
							bounds.Y = CalcYByAppearance(valign, bounds, lineHeight, true);
						}
					}
					info.BlocksBounds[l] = bounds;
				}
				n += count;
			}
		}
		Rectangle UpdateBlockBoundsByAlign(StringBlock sb, Rectangle bounds, int lineHeight) {
			if(bounds.Height < lineHeight) {
				switch(sb.Alignment) {
					case StringBlockAlignment.Top:
						break;
					case StringBlockAlignment.Bottom:
						bounds.Y += (lineHeight - bounds.Height);
						break;
					case StringBlockAlignment.Center:
						bounds.Y += (lineHeight - bounds.Height) / 2;
						break;
				}
			}
			return bounds;
		}
		Rectangle UpdateYByAlignment(Rectangle rect, Size iSize, StringBlock sb) {
			switch(sb.Alignment) {
				case StringBlockAlignment.Top:
					rect.Height = iSize.Height;
					return rect;
				case StringBlockAlignment.Bottom:
					rect.Y = rect.Bottom - iSize.Height;
					rect.Height = iSize.Height;
					return rect;
				case StringBlockAlignment.Center:
					rect.Y += (rect.Height - iSize.Height) / 2;
					rect.Height = iSize.Height;
					return rect;
			}
			return rect;
		}
		int GetLineHeight(StringInfo info, int startIndex, out int count, out int ascentHeight, bool roundTextHeight) {
			count = 1;
			ascentHeight = info.Blocks[startIndex].FontAscentHeight;
			int lineHeight = GetBlockHeight(info.Context, info.Blocks[startIndex], roundTextHeight);
			int lineNo = info.Blocks[startIndex].LineNumber;
			for(int n = startIndex + 1; n < info.Blocks.Count; n++) {
				if(info.Blocks[n].LineNumber != lineNo) break;
				count++;
				lineHeight = Math.Max(lineHeight, GetBlockHeight(info.Context, info.Blocks[n], roundTextHeight));
				ascentHeight = Math.Max(ascentHeight, info.Blocks[n].FontAscentHeight);
			}
			return lineHeight;
		}
		[Obsolete("Use UseAltAlgorithm"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool UseAltAlghorithm = false;
		public static bool UseAltAlgorithm = false;
		string[] GetWordList(TextProcessInfo te, string drawText) {
			List<string> res = new List<string>();
			while(drawText != "") {
				string nextWord = GetNextWord(ref drawText);
				if(nextWord == "" || nextWord[0] < ' ') break;
				res.Add(nextWord);
			}
			return res.ToArray();
		}
		string GetWords(string[] array, int count) {
			return string.Join(string.Empty, array, 0, Math.Min(count, array.Length));
		}
		string GetNextWordsAlt(TextProcessInfo te, string drawText, int wordsWidth, out int width) {
			width = 0;
			if(!te.AllowMultiLine) {
				width = GetStringWidth(te, drawText);
				return drawText;
			}
			string[] list = GetWordList(te, drawText);
			StringBuilder res = new StringBuilder();
			if(list.Length > 6) {
				string sRes = "";
				int first = 0;
				int last = list.Length - 1;
				int lastFit = -1;
				while (first<=last) {
					int index = first + (last - first >> 1);
					string nextBlock = GetWords(list, index);
					int textWidth = GetStringWidth(te, nextBlock);
					if(textWidth < wordsWidth) {
						first = index + 1;
						lastFit = index;
						sRes = nextBlock;
						width = textWidth;
					}
					else {
						if(textWidth == wordsWidth) {
							width = textWidth;
							sRes = nextBlock;
							break;
						}
						last = index - 1;
					}
				}
				if(sRes != "") return sRes;
				if(te.IsNewLine) res.Append(list[0]);
			}
			else {
				for(int i = 0; i < list.Length; i++) {
					string nextWord = list[i];
					int textWidth = GetStringWidth(te, res + nextWord);
					if(textWidth > wordsWidth && (i > 0 || !te.IsNewLine)) break;
					res.Append(nextWord);
				}
			}
			if(width == 0 && res.Length > 0) width = GetStringWidth(te, res.ToString());
			return res.ToString();
		}
		string GetNextWords(TextProcessInfo te, string drawText, int wordsWidth, out int width, int separatorWidth) {
			wordsWidth -= separatorWidth;
			if(UseAltAlgorithm) return GetNextWordsAlt(te, drawText, wordsWidth, out width);
			if(!te.AllowMultiLine) {
				width = GetStringWidth(te, drawText);
				return drawText;
			}
			StringBuilder res = new StringBuilder(te.IsNewLine ? GetNextWord(ref drawText) : "");
			width = 0;
			string nextWord = "";
			while(drawText != "") {
				nextWord = GetNextWord(ref drawText);
				if(nextWord == "" || nextWord[0] < ' ') break;
				int textWidth = GetStringWidth(te, res + nextWord);
				if(textWidth > wordsWidth)
					break;
				width = textWidth;
				res.Append(nextWord);
			}
			if(width == 0 && res.Length > 0) width = GetStringWidth(te, res.ToString());
			return res.ToString();
		}
		int GetStringWidth(TextProcessInfo te, string drawText) {
			if(drawText.Length == 0) return 0;
			StringFormat format = te.Info.Appearance.GetStringFormat();
			StringAlignment halign = format.Alignment;
			StringFormatFlags oldFlags = format.FormatFlags;
			try {
				format.Alignment = StringAlignment.Near;
				format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				Size textSize = CalcStringBlockSize(te.Block, te.Graphics, te.Paint, drawText, format);
				return textSize.Width;
			}
			finally {
				format.FormatFlags = oldFlags;
				format.Alignment = halign;
			}
		}
		string GetNextWord(ref string drawText) {
			if(UseAltAlgorithm) return GetNextWordAlt(ref drawText);
			if(drawText.Length == 0) return "";
			int i = 1;
			while(i < drawText.Length && ((!char.IsWhiteSpace(drawText[i]) && drawText[i] >= ' ') || drawText[i] == 160)) 
				i++;
			string word = drawText.Substring(0, i);
			drawText = drawText.Substring(i, drawText.Length - i);
			return word;
		}
		string GetNextWordAlt(ref string drawText) {
			if(drawText.Length == 0) return "";
			int i = 1;
			while(i < drawText.Length && !char.IsWhiteSpace(drawText[i]) && drawText[i] >= ' ') {
				if(char.GetUnicodeCategory(drawText[i]) == System.Globalization.UnicodeCategory.OtherLetter) {
					if(i < drawText.Length - 1) {
						char ch = drawText[i + 1];
						if(char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter) break;
					}
				}
				i++;
			}
			string word = drawText.Substring(0, i);
			drawText = drawText.Substring(i, drawText.Length - i);
			return word;
		}
		public List<StringBlock> Parse(AppearanceObject appearance, string text) {
			return Parse(appearance, new HyperlinkSettings(), text);
		}
		public List<StringBlock> Parse(AppearanceObject appearance, HyperlinkSettings hyperlinkSettins, string text) {
			return ParseCore(appearance, hyperlinkSettins, text);
		}
		protected void UpdateFont(Graphics graphics, List<StringBlock> strings, Font font) {
			foreach(StringBlock str in strings) {
				SetFont(str, graphics, GetFont(font, str.FontSettings));
			}
		}
		protected internal virtual List<StringBlock> ParseCore(AppearanceObject appearance, HyperlinkSettings hyperlinkSettings, string text) {
			return StringParser2.Parse(appearance, hyperlinkSettings, text);
		}
		internal static bool IsValidSize(Rectangle bounds) {
			return bounds.Width > 0 && bounds.Height > 0;
		}
		protected internal virtual void ParseParameter(string parameter, out string name, out string value) {
			name = parameter;
			value = null;
			int eq = parameter.IndexOf('=');
			if(eq < 0) return;
			name = parameter.Substring(0, eq).ToLowerInvariant();
			value = parameter.Substring(eq + 1).ToLowerInvariant();
		}
	}
}

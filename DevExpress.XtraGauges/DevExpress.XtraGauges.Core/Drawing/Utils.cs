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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Text;
using DevExpress.Utils.Text.Internal;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Drawing {
	public class Utils_StringInfo {
		List<StringBlock> blocks;
		List<Rectangle> blockBounds;
		BaseTextAppearance appearance;
		Rectangle bounds;
		internal Rectangle originalBounds;
		internal bool simpleString;
		string sourceString;
		public Utils_StringInfo() : this(null, null, string.Empty) { }
		public Utils_StringInfo(List<StringBlock> blocks, List<Rectangle> blockBounds, string sourceString) {
			this.simpleString = false;
			this.originalBounds = Rectangle.Empty;
			this.sourceString = sourceString;
			this.blocks = blocks;
			this.blockBounds = blockBounds;
			this.bounds = Rectangle.Empty;
		}
		public bool SimpleString {
			get { return simpleString; }
		}
		public object Context { get; set; }
		public Rectangle Bounds { get { return bounds; } }
		public string SourceString { get { return sourceString; } }
		public List<StringBlock> Blocks { get { return blocks; } }
		public List<Rectangle> BlocksBounds { get { return blockBounds; } }
		public BaseTextAppearance Appearance {
			get {
				if(appearance == null) appearance = new BaseTextAppearance();
				return appearance;
			}
		}
		internal bool AllowBaselineAlignment;
		protected internal void Assign(List<StringBlock> blocks, List<Rectangle> blockBounds, string sourceString) {
			this.sourceString = sourceString;
			this.blocks = blocks;
			this.blockBounds = blockBounds;
		}
		public bool IsEmpty { get { return Blocks == null || Blocks.Count == 0; } }
		internal void SetBounds(Rectangle bounds) {
			this.bounds = bounds;
		}
		internal void UpdateLocation(Point pt) {
			if(BlocksBounds == null) return;
			Point delta = new Point(pt.X - Bounds.X, pt.Y - Bounds.Y);
			for(int i = 0; i < BlocksBounds.Count; i++)
				BlocksBounds[i] = new Rectangle(new Point(BlocksBounds[i].X + delta.X, BlocksBounds[i].Y + delta.Y), BlocksBounds[i].Size);
		}
		internal void UpdateXLocation(StringAlignment align, Rectangle bounds) {
			if(!Utils_StringPainter.IsValidSize(bounds) || align == StringAlignment.Near || Blocks == null || BlocksBounds == null) return;
			for(int n = 0; n < Blocks.Count; ) {
				int blockCount = GetLineBlockCount(n);
				int lineWidth = GetBlocksWidth(n, blockCount);
				int newX = Utils_StringPainter.CalcXByAlignment(bounds, lineWidth, align);
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
		internal void Assign(Utils_StringInfo info) {
			if(Blocks == null) {
				this.blocks = new List<StringBlock>();
				this.blockBounds = new List<Rectangle>();
			}
			Blocks.Clear();
			BlocksBounds.Clear();
			Appearance.Assign(info.Appearance);
			this.sourceString = info.SourceString;
			this.bounds = info.Bounds;
			this.simpleString = info.simpleString;
			this.originalBounds = info.originalBounds;
			this.AllowBaselineAlignment = info.AllowBaselineAlignment;
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
			this.originalBounds.Offset(x, y);
			SetBounds(new Rectangle(Bounds.X + x, Bounds.Y + y, Bounds.Width, Bounds.Height));
			if(BlocksBounds == null) return;
			for(int n = 0; n < BlocksBounds.Count; n++) {
				Rectangle r = BlocksBounds[n];
				r.Offset(x, y);
				BlocksBounds[n] = r;
			}
		}
		internal void SetEmpty() {
			Assign(new List<StringBlock>(), new List<Rectangle>(), string.Empty);
		}
	}
	public class Utils_StringCalculateArgs {
		Graphics graphics;
		string text;
		Rectangle bounds;
		object context;
		public Utils_StringCalculateArgs(Graphics graphics, string text, Rectangle bounds, object context) {
			this.graphics = graphics;
			this.text = text;
			this.bounds = bounds;
			this.context = context;
			this.AllowSimpleString = true;
			this.AllowBaselineAlignment = true;
		}
		public BaseTextAppearance Appearance { get; set; }
		public bool AllowSimpleString { get; set; }
		public bool AllowBaselineAlignment { get; set; }
		public Graphics Graphics { get { return graphics; } }
		public string Text { get { return text; } }
		public Rectangle Bounds { get { return bounds; } }
		public object Context { get { return context; } }
	}
	public class Utils_StringPainter {
		bool IsSimpleStringCore(string text) {
			if(string.IsNullOrEmpty(text)) return true;
			if(text.IndexOf('<') < 0) return true;
			return false;
		}
		internal Font GetFont(Font font, StringFontSettings settings) {
			return GetFont(font, settings.Size < 1 ? font.Size : settings.Size, settings.IsStyleSet ? settings.Style : font.Style);
		}
		static Font GetFont(Font font, float size, FontStyle style) {
			if(font.Style == style && font.Size == size) return font;
			string key = GetFontKey(font, style, size);
			object res = Fonts[key];
			if(res == null) {
				font = new Font(font.FontFamily, size, style, font.Unit);
				Fonts[key] = font;
				return font;
			}
			return res as Font;
		}
		static string GetFontKey(Font font, FontStyle style, float size) {
			return string.Format("{0}: {1} {2} {3}", font.FontFamily, size, font.Unit, style);
		}
		static Hashtable fonts;
		protected static Hashtable Fonts {
			get {
				if(fonts == null) fonts = new Hashtable();
				return fonts;
			}
		}
		internal void SetFont(StringBlock block, Graphics graphics, Font font) {
#if !DXPORTABLE
			block.SetFontInfo(font, DevExpress.Utils.Text.TextUtils.GetFontHeight(graphics, font), DevExpress.Utils.Text.TextUtils.GetFontAscentHeight(graphics, font));
#else
			block.SetFontInfo(font, (int)Math.Round(font.Size), (int)Math.Round(font.Size));
#endif
		}
		public void DrawString(Graphics g, Utils_StringInfo info) {
			if(info.IsEmpty && !info.SimpleString) return;
			StringFormat sf = info.Appearance.Format.NativeFormat;
			if(info.SimpleString) {
				Rectangle textBounds = info.originalBounds;
				if(textBounds.Height < 1) textBounds.Height = info.Bounds.Height;
				if(textBounds.Width < 1) textBounds.Width = info.Bounds.Width;
				Brush brush = info.Appearance.TextBrush.GetBrush(textBounds);
				DrawSimpleString(g, info.SourceString, info.Appearance.Font, brush, textBounds, sf);
				return;
			}
			StringAlignment prevAlignment = sf.Alignment;
			sf.Alignment = StringAlignment.Near;
			try {
				for(int n = 0; n < info.Blocks.Count; n++) {
					StringBlock sb = info.Blocks[n];
					Rectangle r = info.BlocksBounds[n];
					Color backColor = sb.FontSettings.BackColor;
					if(!backColor.IsEmpty) {
						using(var backBrush = new SolidBrush(backColor)) {
							g.FillRectangle(backBrush, r);
						}
					}
					Color color = sb.FontSettings.Color;
					using(var foreBrush = new SolidBrush(color)) {
						Brush brush = foreBrush;
						if(color.IsEmpty)
							brush = info.Appearance.TextBrush.GetBrush(r);
						DrawStringBlock(g, info.Context, sf, sb, r, brush);
					}
				}
			}
			finally {
				sf.Alignment = prevAlignment;
			}
		}
		protected Brush StringBrush { get; set; }
		protected virtual SizeF CalcStringBlockSize(StringBlock block, Graphics g, string s, StringFormat format) {
			return CalcTextSize(block.Font, g, s, format);
		}
		protected SizeF CalcTextSize(Font font, Graphics g, string s, StringFormat format) {
			SizeF size = g.MeasureString(s, font, 0, format);
			if(Math.Round((double)size.Width) != size.Width) size.Width++;
			return size;
		}
		protected virtual void DrawSimpleString(Graphics g, string text, Font font, Brush brush, Rectangle rect, StringFormat format) {
			g.DrawString(text, font, StringBrush, rect, format);
		}
		protected virtual void DrawStringBlock(Graphics g, object context, StringFormat format, StringBlock sb, Rectangle rect, Brush brush) {
			switch(sb.Type) {
				case StringBlockType.Text:
					g.DrawString(sb.Text, sb.Font, (brush == null) ? StringBrush : brush, rect, format);
					break;
				case StringBlockType.Image:
					Image image = ResourceImageProvider.Current.GetImage(context, sb.ImageName);
					if(image != null) {
						Size iSize = sb.Size.IsEmpty ? image.Size : sb.Size;
						g.DrawImage(image, rect, 0, 0, iSize.Width, iSize.Height, GraphicsUnit.Pixel, null);
					}
					else g.DrawRectangle(Pens.Gray, rect);
					break;
			}
		}
		public virtual Utils_StringInfo Calculate(Utils_StringCalculateArgs e) {
			Utils_StringInfo info = new Utils_StringInfo();
			info.AllowBaselineAlignment = e.AllowBaselineAlignment;
			info.Context = e.Context;
			info.Appearance.Assign(e.Appearance);
			Rectangle bounds = e.Bounds;
			if(e.AllowSimpleString && IsSimpleStringCore(e.Text)) {
				SetupSimpleString(info, e);
				return info;
			}
			List<StringBlock> strings = StringParser.Parse(info.Appearance.Font.Size, e.Text, false);
			UpdateFont(e.Graphics, strings, info.Appearance.Font);
			List<StringBlock> blocks = new List<StringBlock>();
			List<Rectangle> blocksBounds = new List<Rectangle>();
			info.Assign(blocks, blocksBounds, e.Text);
			info.originalBounds = bounds;
			if(strings.Count == 0) return info;
			TextProcessInfo te = new TextProcessInfo();
			te.AllowMultiLine = ((info.Appearance.Format.FormatFlags & StringFormatFlags.NoWrap) == 0);
			te.CurrentPosition = bounds.Location;
			te.Graphics = e.Graphics;
			te.Info = info;
			te.Bounds = bounds;
			te.Context = e.Context;
			foreach(StringBlock str in strings) {
				if(te.LineHeight == 0) te.LineHeight = GetBlockHeight(info.Context, str);
				te.Block = str;
				ProcessBlock(te);
			}
			UpdateBaseLine(info);
			UpdateMaximumBounds(info, bounds);
			info.SetBounds(Rectangle.FromLTRB(bounds.X, bounds.Y, te.End.X, te.End.Y));
			UpdateBoundsByAppearance(info, bounds);
			return info;
		}
		void SetupSimpleString(Utils_StringInfo info, Utils_StringCalculateArgs e) {
			SizeF size = CalcTextSize(info.Appearance.Font, e.Graphics, e.Text, info.Appearance.Format.NativeFormat);
			info.simpleString = true;
			info.SetBounds(new Rectangle(e.Bounds.Location, Size.Round(size)));
			info.originalBounds = e.Bounds;
			info.Assign(null, null, e.Text);
			return;
		}
		void ProcessBlock(TextProcessInfo te) {
			switch(te.Block.Type) {
				case StringBlockType.Text:
					ProcessText(te);
					return;
				case StringBlockType.Image:
					ProcessText(te);
					return;
				default:
					throw new Exception(string.Format("Not implemented {0} block type", te.Block.Type));
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
		int GetBlockHeight(object context, StringBlock block) {
			switch(block.Type) {
				case StringBlockType.Text:
					return block.FontHeight;
				case StringBlockType.Image:
					return GetBlockSize(context, block).Height;
			}
			return 0;
		}
		public class TextProcessInfo {
			public TextProcessInfo() {
				this.LineHeight = this.LineNumber = 0;
				this.End = Point.Empty;
				this.IsNewLine = true;
			}
			public object Context;
			public Utils_StringInfo Info;
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
		}
		void UpdateMaximumBounds(Utils_StringInfo info, Rectangle bounds) {
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
		void UpdateBoundsByAppearance(Utils_StringInfo info, Rectangle bounds) {
			int x = CalcXByAlignment(bounds, info.Bounds.Width, info.Appearance.Format.Alignment);
			int y = CalcYByAppearance(info.Appearance.Format.LineAlignment, bounds, info.Bounds.Height, false);
			if(bounds.Width < 1) x = bounds.X;
			if(bounds.Height < 1) y = bounds.Y;
			info.UpdateLocation(new Point(x, y));
			if(bounds.Width > 0) info.UpdateXLocation(info.Appearance.Format.Alignment, bounds);
			info.originalBounds = bounds;
		}
		internal int CalcYByAppearance(StringAlignment align, Rectangle bounds, int height, bool reverseHeight) {
			int delta = reverseHeight ? (height - bounds.Height) : (bounds.Height - height);
			int y = bounds.Y;
			if(delta < 1) return y;
			if(align == StringAlignment.Center) y += delta / 2 + (delta % 2);
			if(align == StringAlignment.Far) y += delta;
			return y;
		}
		internal static int CalcXByAlignment(Rectangle totalBounds, int textWidth, StringAlignment align) {
			int delta = totalBounds.Width - textWidth;
			int x = totalBounds.X;
			if(delta < 1) return x;
			if(align == StringAlignment.Center) x += delta / 2 + (delta % 2);
			if(align == StringAlignment.Far) x += delta;
			return x;
		}
		internal void ProcessText(TextProcessInfo te) {
			int maxWidth = te.Bounds.Width;
			if(maxWidth <= 0) maxWidth = int.MaxValue;
			string drawText = te.Block.Text;
			while(drawText.Length > 0 && drawText[0] != '\0') {
				te.IsNewLine = te.Bounds.X == te.CurrentX || !te.AllowMultiLine;
				int textWidth = 0;
				string words = string.Empty;
				if(te.Block.Type == StringBlockType.Text) {
					words = GetNextWords(te, drawText, maxWidth - (te.CurrentX - te.Bounds.X), out textWidth);
					if(te.AllowMultiLine) {
						if(words == "" || words[0] < ' ') {
							drawText = ProcessEmptyWord(te, drawText);
							continue;
						}
					}
				}
				else {
					words = te.Block.Text;
					textWidth = GetBlockSize(te.Context, te.Block).Width;
				}
				int blockHeight = GetBlockHeight(te.Context, te.Block);
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
				if(te.Block.Type != StringBlockType.Text || words.Length == drawText.Length) break;
				drawText = drawText.Substring(words.Length, drawText.Length - words.Length);
			}
		}
		Rectangle CreateNewBlock(TextProcessInfo te, int textWidth, string words) {
			StringBlock newBlock = new StringBlock();
			newBlock.Text = words;
			newBlock.SetBlock(te.Block);
			newBlock.LineNumber = te.LineNumber;
			te.Info.Blocks.Add(newBlock);
			Rectangle lastBlock = new Rectangle(te.CurrentPosition, new Size(textWidth, GetBlockHeight(te.Context, te.Block)));
			te.Info.BlocksBounds.Add(lastBlock);
			UpdateBlockAscentHeight(newBlock, lastBlock);
			te.CurrentX += textWidth;
			te.End.Y = Math.Max(te.End.Y, te.CurrentY + te.LineHeight);
			te.End.X = Math.Max(te.End.X, te.CurrentX);
			return lastBlock;
		}
		void UpdateBlockAscentHeight(StringBlock newBlock, Rectangle lastBlock) {
			if(newBlock.Type == StringBlockType.Image) {
				if(lastBlock.Height > 0) newBlock.SetAscentHeight((int)(lastBlock.Height * 0.6f));
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
			if(trimText) drawText = drawText.TrimStart();
			return drawText;
		}
		void UpdateBaseLine(Utils_StringInfo info) {
			if(info.Blocks.Count < 2) return;
			int count;
			for(int n = 0; n < info.Blocks.Count; ) {
				int ascentHeight;
				int lineHeight = GetLineHeight(info, n, out count, out ascentHeight);
				for(int l = n; l < n + count; l++) {
					Rectangle bounds = info.BlocksBounds[l];
					StringBlock b = info.Blocks[l];
					if(b.Type != StringBlockType.Text) {
						bounds = UpdateBlockBoundsByAlign(b, bounds, lineHeight);
					}
					else {
						StringAlignment valign = info.Appearance.Format.LineAlignment;
						if(valign == StringAlignment.Center || !info.AllowBaselineAlignment) {
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
		int GetLineHeight(Utils_StringInfo info, int startIndex, out int count, out int ascentHeight) {
			count = 1;
			ascentHeight = info.Blocks[startIndex].FontAscentHeight;
			int lineHeight = GetBlockHeight(info.Context, info.Blocks[startIndex]);
			int lineNo = info.Blocks[startIndex].LineNumber;
			for(int n = startIndex + 1; n < info.Blocks.Count; n++) {
				if(info.Blocks[n].LineNumber != lineNo) break;
				count++;
				lineHeight = Math.Max(lineHeight, GetBlockHeight(info.Context, info.Blocks[n]));
				ascentHeight = Math.Max(ascentHeight, info.Blocks[n].FontAscentHeight);
			}
			return lineHeight;
		}
		string GetNextWords(TextProcessInfo te, string drawText, int wordsWidth, out int width) {
			width = 0;
			if(!te.AllowMultiLine) {
				width = GetStringWidth(te, drawText);
				return drawText;
			}
			string res = te.IsNewLine ? GetNextWord(ref drawText) : "";
			string nextWord = "";
			while(drawText != "") {
				nextWord = GetNextWord(ref drawText);
				if(nextWord == "" || nextWord[0] < ' ') break;
				int textWidth = GetStringWidth(te, res + nextWord);
				if(textWidth > wordsWidth)
					break;
				width = textWidth;
				res += nextWord;
			}
			if(width == 0 && res.Length > 0) width = GetStringWidth(te, res);
			return res;
		}
		int GetStringWidth(TextProcessInfo te, string drawText) {
			if(drawText.Length == 0) return 0;
			StringFormat format = te.Info.Appearance.Format.NativeFormat;
			StringAlignment halign = format.Alignment;
			StringFormatFlags oldFlags = format.FormatFlags;
			try {
				format.Alignment = StringAlignment.Near;
				format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				SizeF textSize = CalcStringBlockSize(te.Block, te.Graphics, drawText, format);
				return Size.Round(textSize).Width;
			}
			finally {
				format.FormatFlags = oldFlags;
				format.Alignment = halign;
			}
		}
		string GetNextWord(ref string drawText) {
			if(drawText.Length == 0) return "";
			int i = 1;
			while(i < drawText.Length && !char.IsWhiteSpace(drawText[i]) && drawText[i] >= ' ')
				i++;
			string word = drawText.Substring(0, i);
			drawText = drawText.Substring(i, drawText.Length - i);
			return word;
		}
		protected void UpdateFont(Graphics graphics, List<StringBlock> strings, Font font) {
			foreach(StringBlock str in strings) {
				SetFont(str, graphics, GetFont(font, str.FontSettings));
			}
		}
		internal static bool IsValidSize(Rectangle bounds) {
			return bounds.Width > 0 && bounds.Height > 0;
		}
	}
}
namespace DevExpress.XtraGauges.Core.Animation {
	public interface IAnimationID { }
	public class Utils_AnimationInfo : IDisposable {
		int deltaTick;
		int frameCount;
		int prevFrame;
		int[] deltaTicks;
		long beginTick;
		long currentTick;
		object animationId;
		IAnimationID animObject;
		bool finalFrameDrawn = false, forceLastFrame = false;
		Action<Utils_AnimationInfo> method;
		public Utils_AnimationInfo(IAnimationID obj, object animationId, int deltaTick, int frameCount, Action<Utils_AnimationInfo> method) {
			this.animObject = obj;
			this.deltaTick = deltaTick;
			this.frameCount = frameCount;
			this.animationId = animationId;
			this.beginTick = -1;
			this.currentTick = -1;
			this.prevFrame = -1;
			this.method = method;
		}
		public virtual void Dispose() {
			this.method = null;
		}
		public virtual void ForceLastFrameStep() {
			this.forceLastFrame = true;
			FrameStep();
		}
		public void FrameStep() {
			if(method != null) method(this);
		}
		public bool IsFinalFrame { get { return CurrentFrame >= FrameCount - 1; } }
		public bool IsFinalFrameDrawn { get { return finalFrameDrawn; } set { finalFrameDrawn = value; } }
		public int DeltaTick { get { return deltaTick; } set { deltaTick = value; } }
		public int[] DeltaTicks { get { return deltaTicks; } set { deltaTicks = value; } }
		public int FrameCount { get { return frameCount; } set { frameCount = value; } }
		public long BeginTick { get { return beginTick; } set { beginTick = value; } }
		public long CurrentTick {
			get { return currentTick; }
			set {
				prevFrame = CurrentFrame;
				currentTick = value;
			}
		}
		public IAnimationID AnimatedObject { get { return animObject; } set { animObject = value; } }
		public object AnimationId { get { return animationId; } }
		public virtual int PrevFrame { get { return prevFrame; } }
		public int AnimationLength {
			get {
				if(DeltaTicks != null) {
					int summ = 0;
					for(int i = 0; i < DeltaTicks.Length; i++) summ += DeltaTicks[i];
					return summ;
				}
				return DeltaTick * FrameCount;
			}
		}
		int GetFrameByTicks() {
			long dt = CurrentTick - BeginTick;
			for(int i = 0; i < DeltaTicks.Length; i++) {
				dt -= DeltaTicks[i];
				if(dt < 0) return i;
			}
			return FrameCount - 1;
		}
		public virtual int CurrentFrame {
			get {
				if(forceLastFrame) return FrameCount - 1;
				if(DeltaTicks != null) return Math.Min(FrameCount, GetFrameByTicks());
				if(DeltaTick == 0) return 0;
				long dt = CurrentTick - BeginTick;
				return Math.Min(FrameCount, (int)(dt / DeltaTick));
			}
			set { frameCount = value; }
		}
	}
	public class Utils_AnimationInfoCollection : CollectionBase {
		class HashInfo {
			Hashtable hash = new Hashtable();
			public HashInfo(Utils_AnimationInfo info1, Utils_AnimationInfo info2) {
				hash[info1.AnimationId] = info1;
				hash[info2.AnimationId] = info2;
			}
			public Hashtable Hash { get { return hash; } }
			public Utils_AnimationInfo this[object animationId] {
				get { return Hash[animationId] as Utils_AnimationInfo; }
			}
			public object Remove(object animationId) {
				Hash.Remove(animationId);
				if(Hash.Count < 2 && Hash.Count > 0) {
					IEnumerator enu = Hash.Values.GetEnumerator();
					enu.MoveNext();
					return enu.Current;
				}
				return this;
			}
		}
		const int MaxAnimatingObjectCount = 100;
		Hashtable hash = new Hashtable();
		public int Add(Utils_AnimationInfo animInfo) {
			if(List.Count >= MaxAnimatingObjectCount) {
				Utils_AnimationInfo toRemove = this[0];
				if(!toRemove.IsFinalFrameDrawn) {
					toRemove.ForceLastFrameStep();
				}
				RemoveAt(0);
			}
			return List.Add(animInfo);
		}
		public Utils_AnimationInfo this[int index] { get { return List[index] as Utils_AnimationInfo; } }
		public void Remove(Utils_AnimationInfo animInfo) {
			List.Remove(animInfo);
		}
		public void Remove(IAnimationID animObj, object animId) {
			Utils_AnimationInfo info = this[animObj, animId];
			if(info != null) List.Remove(info);
		}
		public void Remove(IAnimationID animObj) {
			hash.Remove(animObj);
			for(int n = Count - 1; n >= 0; n--) {
				if(this[n].AnimatedObject == animObj) RemoveAt(n);
			}
		}
		public Utils_AnimationInfo this[IAnimationID animObj, object animId] {
			get {
				if(animObj == null) return null;
				object res = hash[animObj];
				Utils_AnimationInfo ani = res as Utils_AnimationInfo;
				if(ani != null) {
					if(Object.Equals(ani.AnimationId, animId)) return ani;
					return null;
				}
				HashInfo info = res as HashInfo;
				if(info != null) return info[animId];
				return null;
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			Utils_AnimationInfo item = (Utils_AnimationInfo)value;
			object animObject = hash[item.AnimatedObject];
			if(animObject == null) {
				hash[item.AnimatedObject] = item;
			}
			else {
				if(animObject is Utils_AnimationInfo) {
					hash[item.AnimatedObject] = new HashInfo(animObject as Utils_AnimationInfo, item);
				}
				else {
					HashInfo hashInfo = animObject as HashInfo;
					hashInfo.Hash[item.AnimationId] = item;
				}
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			Utils_AnimationInfo item = (Utils_AnimationInfo)value;
			object obj = hash[item.AnimatedObject];
			if(obj == null) return;
			if(obj is Utils_AnimationInfo) {
				hash.Remove(item.AnimatedObject);
			}
			else {
				HashInfo info = obj as HashInfo;
				hash[item.AnimatedObject] = info.Remove(item.AnimationId);
			}
			item.Dispose();
		}
	}
#if DXPORTABLE
	public class Utils_Animator {
		[ThreadStatic]
		static Utils_Animator current;
		public static Utils_Animator Current {
			get {
				if (current == null) current = new Utils_Animator();
				return current;
			}
		}
		public static void RemoveObject(IAnimationID obj) {
		}
		public void AddObject(IAnimationID obj, object animId, int deltaTick, int frameCount, Action<Utils_AnimationInfo> method) {
		}
	}
#else
	public class Utils_Animator {
		[ThreadStatic]
		static Utils_Animator current;
		public static Utils_Animator Current {
			get {
				if(current == null) current = new Utils_Animator();
				return current;
			}
		}
		public static void RemoveObject(IAnimationID obj) {
			if(current == null) return;
			Current.Animations.Remove(obj);
			Current.CheckTimerStop();
		}
		Utils_AnimationInfoCollection animations;
		Timer timer;
		Stopwatch stopwatch;
		Utils_Animator() {
			animations = new Utils_AnimationInfoCollection();
			timer = new Timer();
			timer.Interval = int.MaxValue;
			timer.Tick += new EventHandler(FrameStep);
			stopwatch = Stopwatch.StartNew();
		}
		protected Stopwatch Stopwatch { get { return stopwatch; } }
		public Utils_AnimationInfoCollection Animations { get { return animations; } }
		public void AddObject(IAnimationID obj, object animId, int deltaTick, int frameCount, Action<Utils_AnimationInfo> method) {
			animations.Remove(obj, animId);
			animations.Add(new Utils_AnimationInfo(obj, animId, deltaTick, frameCount, method));
			CheckTimerStart();
		}
		void RemoveObjectCore(IAnimationID obj, object animId) {
			animations.Remove(obj, animId);
			CheckTimerStop();
		}
		void RemoveObjectCore(IAnimationID obj) {
			animations.Remove(obj);
			CheckTimerStop();
		}
		void CheckTimerStart() {
			if((timer.Interval != 1 || !timer.Enabled) && animations.Count > 0) {
				timer.Interval = 1;
				timer.Start();
			}
		}
		void CheckTimerStop() {
			if((timer.Interval == 1 || timer.Enabled) && animations.Count == 0) {
				timer.Stop();
				timer.Interval = int.MaxValue;
			}
		}
		bool lockTimer = false;
		public void FrameStep(object sender, EventArgs e) {
			if(lockTimer) return;
			this.lockTimer = true;
			try {
				timer.Interval = int.MaxValue;
				long ticks = stopwatch.Elapsed.Ticks;
				for(int animIndex = 0; animIndex < animations.Count; animIndex++) {
					Utils_AnimationInfo animationInfo = animations[animIndex];
					if(animationInfo.BeginTick == -1) {
						animationInfo.BeginTick = ticks;
						animationInfo.CurrentTick = ticks;
						animationInfo.FrameStep();
						continue;
					}
					animationInfo.CurrentTick = ticks;
					if(animationInfo.PrevFrame != animationInfo.CurrentFrame) {
						animationInfo.FrameStep();
					}
					if(animationInfo.CurrentFrame >= animationInfo.FrameCount)
						animations.Remove(animationInfo);
				}
				CheckTimerStart();
			}
			finally {
				this.lockTimer = false;
			}
		}
		public Utils_AnimationInfo Get(IAnimationID obj, object animationId) {
			return animations[obj, animationId];
		}
	}
#endif
	}

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
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Native {
	public class JustifiedStringPainter : IDisposable {
		static public void DrawString(string text, IGraphics gr, Font font, Brush br, RectangleF rect, StringFormat sf, bool doClip) {
			using(JustifiedStringPainter painter = new JustifiedStringPainter(text, gr, rect, font, br, sf, doClip)) {
				painter.DrawJustifiedString();
			}
		}
		Font font;
		IGraphics gr;
		StringFormat stringFormat;
		Brush brush;
		RectangleF fitRect;
		RectangleF clipRect;
		StringAlignment lineAlignment;
		string text;
		bool doClip;
		Matrix identityMatrix = new Matrix();
		protected class WordInfo {
			public string text;
			public RectangleF rect;
			public bool lastInLine;
			public CharacterRange range;
		}
		protected JustifiedStringPainter(string text, IGraphics gr, RectangleF rect, Font font, Brush br, StringFormat sf, bool doClip) {
			this.gr = gr;
			this.clipRect = rect;
			this.font = font;
			this.stringFormat = (StringFormat)sf.Clone();
			this.text = text;
			this.brush = br;
			this.lineAlignment = sf.LineAlignment;
			stringFormat.Alignment = StringAlignment.Near;
			stringFormat.LineAlignment = StringAlignment.Near;
			this.doClip = doClip;
			fitRect = clipRect;
		}
		public void Dispose() {
			if(this.stringFormat != null) {
				this.stringFormat.Dispose();
				this.stringFormat = null;
			}
			if(this.identityMatrix != null) {
				this.identityMatrix.Dispose();
				this.identityMatrix = null;
			}
		}
		static bool IsCR(char ch) {
			return ch == '\n' || ch == '\r';
		}
		static bool IsWhiteSpace(char ch) {
			return ch == ' ' || ch == '\t' || IsCR(ch);
		}
		static bool IsLongHyphen(char ch) {
			return ch == '\x2013';
		}
		static CharacterRange[] GetCharacterRanges(string text) {
			List<CharacterRange> ranges = new List<CharacterRange>();
			int count = text.Length;
			int i = 0;
			bool leadingWhiteSpace = true;
			for(; ; ) {
				int begin = i;
				while(i < count && IsWhiteSpace(text[i])) {
					if(IsCR(text[i])) {
						leadingWhiteSpace = true;
						begin = i + 1;
					}
					i++;
				}
				if(leadingWhiteSpace) {
					leadingWhiteSpace = false;
				} else
					begin = i;
				if(i >= count)
					break;
				while(i < count && !IsWhiteSpace(text[i]))
					if(IsLongHyphen(text[i++]))
						break;
				ranges.Add(new CharacterRange(begin, i - begin));
				if(i >= count)
					break;
			}
			return ranges.ToArray();
		}
		static float CalcTotalWordsWidth(List<WordInfo> infos) {
			float width = 0;
			foreach(WordInfo info in infos)
				width += info.rect.Width;
			return width;
		}
		void RemeasureWords(List<WordInfo> infos) {
			for(int i = 0; i < infos.Count; i++) {
				SizeF size = gr.Measurer.MeasureString(infos[i].text, font, PointF.Empty, stringFormat, gr.PageUnit);
				infos[i].rect.Width = size.Width;
			}
		}
		void JustifyLine(List<WordInfo> infos) {
			if(infos.Count == 1) {
				infos[0].rect.Width = fitRect.Width;
				return;
			}
			RemeasureWords(infos);
			float gap = GetWordGap(infos);
			for(int i = 1; i < infos.Count; i++)
				infos[i].rect.X = infos[i - 1].rect.Right + gap;
		}
		float GetWordGap(List<WordInfo> infos) {
			WordInfo lastInfo = infos[infos.Count - 1];
			if(lastInfo.lastInLine && (stringFormat.FormatFlags & StringFormatFlags.DisplayFormatControl) == 0) {
				StringFormatFlags flags = stringFormat.FormatFlags;
				try {
					stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
					SizeF size = gr.Measurer.MeasureString(" ", font, PointF.Empty, stringFormat, gr.PageUnit);
					return size.Width;
				} finally {
					stringFormat.FormatFlags = flags;
				}
			}
			return Math.Max(0, fitRect.Width - CalcTotalWordsWidth(infos)) / (infos.Count - 1);
		}
		static void AddWordInfoToLines(WordInfo info, Dictionary<float, List<WordInfo>> ht) {
			float key = info.rect.Top;
			if(!ht.ContainsKey(key)) {
				List<WordInfo> line = new List<WordInfo>();
				line.Add(info);
				ht[key] = line;
			} else {
				ht[key].Add(info);
			}
		}
		void PerformJustification(WordInfo[] infos) {
			if(infos.Length <= 0)
				return;
			float height = infos[0].rect.Height;
			Dictionary<float, List<WordInfo>> ht = new Dictionary<float, List<WordInfo>>();
			for(int i = 0; i < infos.Length; i++)
				AddWordInfoToLines(infos[i], ht);
			foreach(float key in ht.Keys)
				JustifyLine(ht[key]);
		}
		bool IsLastWord(CharacterRange range) {
			if(range.First + range.Length >= text.Length)
				return true;
			int charIndex = range.First + range.Length;
			char ch;
			do {
				ch = text[charIndex];
				charIndex++;
			} while(IsWhiteSpace(ch) && !IsCR(ch) && charIndex < text.Length);
			return IsCR(ch) || charIndex >= text.Length;
		}
		void SplitWord(CharacterRange range, Dictionary<float, WordInfo> lines) {
			int count = range.Length;
			CharacterRange[] ranges = new CharacterRange[count];
			for(int i = 0; i < count; i++)
				ranges[i] = new CharacterRange(range.First + i, 1);
			Region[] regions = CalcRegions(ranges);
			for(int i = 0; i < count; i++) {
				RectangleF[] rects = regions[i].GetRegionScans(identityMatrix);
				if (rects == null || rects.Length == 0)
					continue;
				RectangleF rect = rects[0];
				float key = rect.Top;
				if(!lines.ContainsKey(key)) {
					WordInfo info = new WordInfo();
					info.rect = rect;
					info.range = new CharacterRange(ranges[i].First, ranges[i].Length);
					lines[key] = info;
				} else {
					WordInfo info = lines[key];
					info.rect = RectangleF.Union(info.rect, rect);
					info.range.Length++;
				}
			}
			foreach(float key in lines.Keys) {
				WordInfo info = lines[key];
				info.text = text.Substring(info.range.First, info.range.Length);
				info.lastInLine = IsLastWord(info.range);
			}
		}
		WordInfo[] CreateWordInfo(Region region, CharacterRange range) {
			RectangleF[] rects = region.GetRegionScans(identityMatrix);
			if(rects.Length == 0)
				return null;
			if(rects.Length == 1) {
				WordInfo[] infos = { new WordInfo() };
				infos[0].rect = rects[0];
				infos[0].text = text.Substring(range.First, range.Length);
				infos[0].lastInLine = IsLastWord(range);
				return infos;
			} else {
				Dictionary<float, WordInfo> lines = new Dictionary<float, WordInfo>();
				SplitWord(range, lines);
				WordInfo[] infos = new WordInfo[lines.Count];
				int i = 0;
				foreach(float key in lines.Keys) {
					infos[i] = lines[key];
					i++;
				}
				return infos;
			}
		}
		WordInfo[] CreateWordInfo(Region[] regions, CharacterRange[] ranges) {
			List<WordInfo> wordInfos = new List<WordInfo>();
			System.Diagnostics.Debug.Assert(ranges.Length == regions.Length);
			for(int i = 0; i < regions.Length; i++) {
				WordInfo[] infos = CreateWordInfo(regions[i], ranges[i]);
				if(infos != null && infos.Length > 0)
					wordInfos.AddRange(infos);
			}
			return wordInfos.ToArray();
		}
		static void CopyRanges(CharacterRange[] from, CharacterRange[] to, int startIndexFrom, int count) {
			for(int fromIndex = startIndexFrom, i = 0; i < count; i++, fromIndex++)
				to[i] = from[fromIndex];
		}
		Region[] MeasureCharacterRangesImpl(StringFormat sf) {
			RectangleF rect = fitRect;
			rect.Height = int.MaxValue;
			GraphicsUnit pageUnit = gr.PageUnit;
			using(Graphics measuringGrapnics = DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphicsFromHiResImage()) {
				measuringGrapnics.PageUnit = gr.PageUnit;
				return measuringGrapnics.MeasureCharacterRanges(text, font, rect, sf);
			}
		}
		Region[] CalcRegions(CharacterRange[] ranges) {
			using(StringFormat sf = (StringFormat)stringFormat.Clone()) {
				return CalcRegions(ranges, sf);
			}
		}
		Region[] CalcRegions(CharacterRange[] ranges, StringFormat sf) {
			if(ranges.Length <= 32) { 
				sf.SetMeasurableCharacterRanges(ranges);
				return MeasureCharacterRangesImpl(sf);
			} else {
				List<Region> regionsList = new List<Region>();
				CharacterRange[] smallRanges = new CharacterRange[32];
				int i;
				for(i = 0; i < ranges.Length; i += 32) {
					if(i + 32 <= ranges.Length) {
						CopyRanges(ranges, smallRanges, i, 32);
						sf.SetMeasurableCharacterRanges(smallRanges);
						regionsList.AddRange(MeasureCharacterRangesImpl(sf));
					}
				}
				int n = ranges.Length % 32;
				if(n != 0) {
					CharacterRange[] remainRanges = new CharacterRange[n];
					CopyRanges(ranges, remainRanges, i - 32, n);
					sf.SetMeasurableCharacterRanges(remainRanges);
					regionsList.AddRange(MeasureCharacterRangesImpl(sf));
				}
				return regionsList.ToArray();
			}
		}
		float MeasureTextHeight() {
			return gr.Measurer.MeasureString(text, font, fitRect.Width, stringFormat, gr.PageUnit).Height;
		}
		void AdjustWordsYPosition(WordInfo[] infos, float dy) {
			for(int i = 0; i < infos.Length; i++)
				infos[i].rect.Offset(0, dy);
		}
		void CenterWordsVertically(WordInfo[] infos, RectangleF rect) {
			RectangleF textRect = rect;
			textRect.Height = MeasureTextHeight();
			float dy = (rect.Height - textRect.Height) / 2;
			AdjustWordsYPosition(infos, dy);
		}
		void AlignWordsToBottom(WordInfo[] infos, RectangleF rect) {
			RectangleF textRect = rect;
			textRect.Height = MeasureTextHeight();
			float dy = rect.Height - textRect.Height;
			AdjustWordsYPosition(infos, dy);
		}
		void DoClip() {
			if(!this.doClip)
				return;
			RectangleF rect = this.clipRect;
			rect.Intersect(this.gr.ClipBounds);
			gr.ClipBounds = new RectangleF(this.gr.ClipBounds.X, rect.Y, this.gr.ClipBounds.Width, rect.Height);
		}
		protected void DrawJustifiedString() {
			if(text == null || text == String.Empty)
				return;
			CharacterRange[] ranges = GetCharacterRanges(text);
			if(ranges.Length <= 0)
				return;
			Region[] regions = CalcRegions(ranges);
			WordInfo[] infos = CreateWordInfo(regions, ranges);
			PerformJustification(infos);
			if(lineAlignment == StringAlignment.Center)
				CenterWordsVertically(infos, fitRect);
			else if(lineAlignment == StringAlignment.Far)
				AlignWordsToBottom(infos, fitRect);
			RectangleF oldClip = gr.ClipBounds;
			try {
				DoClip();
				for(int i = 0; i < infos.Length; i++)
					gr.DrawString(infos[i].text, font, brush, infos[i].rect.Location, stringFormat);
			} finally {
				gr.ClipBounds = oldClip;
			}
		}
	}
}

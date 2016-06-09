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
using System.Linq;
using System.Text;
using System.Windows.Documents;
namespace DevExpress.Xpf.DemoBase.Helpers.TextColorizer.Internal {
	class SLTextRange {
		public SLTextRange(TextPointer start, TextPointer end) {
			Start = start;
			End = end;
		}
		public TextPointer Start { get; private set; }
		public TextPointer End { get; private set; }
	}
	static class RichTextExtensions {
	}
	interface IRichTextPresenter {
		ICollection<Block> Blocks { get; }
		TextPointer ContentStart { get; }
		TextPointer ContentEnd { get; }
		void TextWidthMaxSet(double width);
		void Select(TextPointer start, TextPointer end);
	}
	class RichTextHelper {
		public static double SetText(IRichTextPresenter rtb, string text, CodeLanguage codeLanguage) {
			const double WidthCharacter = 7.5;
			try {
				if(rtb == null) return 0;
				rtb.Blocks.Clear();
				if(string.IsNullOrEmpty(text)) return 0;
				Paragraph paragraph = new Paragraph();
				CodeBlockPresenter presenter = new CodeBlockPresenter(codeLanguage);
				presenter.FillInlines(text, paragraph.Inlines);
				int index = 0, length = 0, currentIndex = 0;
				do {
					currentIndex = text.IndexOf('\r', index);
					length = Math.Max(length, currentIndex - index);
					index = currentIndex + 1;
				}
				while(currentIndex > -1);
				rtb.Blocks.Add(paragraph);
				rtb.Select(rtb.ContentStart, rtb.ContentStart);
				return length * WidthCharacter;
			} catch(Exception e) {
				throw new Exception(string.Format("Text = #{0}#", text), e);
			}
		}
		public static string GetText(IRichTextPresenter rtb, SLTextRange range = null) {
			StringBuilder text = new StringBuilder();
			foreach(Run run in GetRuns(rtb, range)) {
				int start;
				int end;
				GetRunOffsets(range, run, out start, out end);
				text.Append(run.Text.Substring(start, end - start));
			}
			return text.ToString();
		}
		public static SLTextRange FindText(IRichTextPresenter rtb, string text) {
			return FindText(rtb, text, null, StringComparison.OrdinalIgnoreCase);
		}
		public static SLTextRange FindText(IRichTextPresenter rtb, string text, SLTextRange range) {
			return FindText(rtb, text, range, StringComparison.OrdinalIgnoreCase);
		}
		public static SLTextRange FindText(IRichTextPresenter rtb, string text, StringComparison comparisionType) {
			return FindText(rtb, text, null, comparisionType);
		}
		public static SLTextRange FindText(IRichTextPresenter rtb, string text, SLTextRange range, StringComparison comparisionType) {
			if(string.IsNullOrEmpty(text)) return null;
			StringBuilder s = new StringBuilder(text);
			TextPointer textStart = null;
			LinkedListNode<Run> run = GetRuns(rtb, range).First;
			LinkedListNode<Run> savedRun = null;
			while(run != null) {
				int runTextStart;
				int runTextEnd;
				GetRunOffsets(range, run.Value, out runTextStart, out runTextEnd);
				int start;
				int end;
				if(FindSubstring(run.Value.Text, s, out start, out end, runTextStart, runTextEnd, comparisionType, textStart != null)) {
					if(textStart == null) {
						savedRun = run;
						textStart = run.Value.ContentStart.GetPositionAtOffset(start, LogicalDirection.Forward);
					}
					if(s.Length == 0) {
						return new SLTextRange(textStart, run.Value.ContentStart.GetPositionAtOffset(end, LogicalDirection.Backward));
					}
				} else {
					if(textStart != null) {
						run = savedRun;
						s = new StringBuilder(text);
						textStart = null;
					}
				}
				run = run.Next;
			}
			return null;
		}
		internal static void GetRunOffsets(SLTextRange range, Run run, out int runTextStart, out int runTextEnd) {
			runTextStart = 0;
			if(range != null && range.Start != null) {
				if(range.Start.CompareTo(run.ContentStart) <= 0)
					runTextStart = 0;
				else
					runTextStart = run.ContentStart.GetOffsetToPosition(range.Start);
			}
			int runLength = run.Text.Length;
			runTextEnd = runLength;
			if(range != null && range.End != null) {
				if(run.ContentEnd.CompareTo(range.End) <= 0)
					runTextEnd = 0;
				else
					runTextEnd = range.End.GetOffsetToPosition(run.ContentEnd);
				runTextEnd = runLength - runTextEnd;
			}
		}
		internal static LinkedList<Run> GetRuns(IRichTextPresenter rtb, SLTextRange range) {
			LinkedList<Run> list = new LinkedList<Run>();
			foreach(Block block in rtb.Blocks) {
				if(range != null) {
					if(range.Start != null && block.ElementEnd.CompareTo(range.Start) < 0) continue;
					if(range.End != null && block.ElementStart.CompareTo(range.End) > 0) break;
				}
				Paragraph paragraph = block as Paragraph;
				if(paragraph == null) continue;
				foreach(Inline inline in paragraph.Inlines) {
					if(range != null) {
						if(range.Start != null && inline.ElementEnd.CompareTo(range.Start) < 0) continue;
						if(range.End != null && inline.ElementStart.CompareTo(range.End) > 0) break;
					}
					Run run = inline as Run;
					if(run == null) continue;
					list.AddLast(run);
				}
			}
			return list;
		}
		public static bool FindSubstring(string text, StringBuilder substring, out int start, out int end, int textStart, int textEnd, StringComparison comparisionType, bool startsOnly) {
			if(text.Length == 0) {
				start = -1;
				end = -1;
				return false;
			}
			for(int ti = textStart; ti < (startsOnly ? textStart + 1 : textEnd); ++ti) {
				if(!CharEquals(text[ti], substring[0], comparisionType)) continue;
				int ti2 = ti;
				int si = 0;
				while(true) {
					++ti2;
					++si;
					if(si == substring.Length || ti2 == text.Length) {
						start = ti;
						end = ti2;
						substring.Remove(0, si);
						return true;
					}
					if(ti2 >= textEnd || !CharEquals(text[ti2], substring[si], comparisionType)) break;
				}
			}
			start = -1;
			end = -1;
			return false;
		}
		internal static bool CharEquals(char c1, char c2, StringComparison comparisionType) {
			switch(comparisionType) {
				case StringComparison.CurrentCultureIgnoreCase: return char.ToLower(c1) == char.ToLower(c2);
				case StringComparison.OrdinalIgnoreCase: return char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2);
				default: return c1 == c2;
			}
		}
	}
}

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
using System.Linq;
namespace DevExpress.XtraPrinting.Native.Navigation {
	public class TextBrickSelector : BrickSelector {
		readonly string text = String.Empty;
		readonly bool wholeWord = false;
		readonly bool matchCase = false;
		readonly IPrintingSystemContext context;
		public TextBrickSelector(string text, bool wholeWord, bool matchCase, IPrintingSystemContext context) {
			this.text = text;
			this.wholeWord = wholeWord;
			this.matchCase = matchCase;
			this.context = context;
		}
		public override bool CanSelect(Brick brick, RectangleF brickRect, RectangleF visibleRect) {
			VisualBrick visualBrick = brick as VisualBrick;
			if(visualBrick == null || !brick.IsVisible)
				return false;
#if !SILVERLIGHT
			bool useMeasurer = context != null && visualBrick is TextBrick;
			string visibleText = useMeasurer ? GetVisibleText(visualBrick, brickRect, visibleRect) : visualBrick.Text;
#else
			string visibleText = visualBrick.Text;
#endif
			if(String.IsNullOrEmpty(visibleText))
				return false;
			string selectingText = text;
			if(!matchCase) {
				visibleText = visibleText.ToLower();
				selectingText = selectingText.ToLower();
			}
			visibleText = ReplaceNoBreakSpace(visibleText);
			selectingText = ReplaceNoBreakSpace(selectingText);
			return wholeWord
				? StringContainsWord(visibleText, selectingText)
				: visibleText.IndexOf(selectingText) >= 0;
		}
		public string GetVisibleText(VisualBrick visualBrick, RectangleF brickRect, RectangleF visibleRect) {
			TextFormatter textFormatter = new TextFormatter(GraphicsUnit.Document, context.Measurer);
			string[] multilineText = textFormatter.FormatMultilineText(visualBrick.Text, visualBrick.Style.Font, visualBrick.Width, float.MaxValue, visualBrick.Style.StringFormat.Value);
			int startLineIndex = 0;
			if(FloatsComparer.Default.FirstLessSecond(brickRect.Top, visibleRect.Top)) { 
				for(int lineIndex = 0; lineIndex < multilineText.Length; lineIndex++) {
					float currentLinesHeight = TextFormatter.CalculateHeightOfLines(visualBrick.Style.Font, lineIndex + 1, GraphicsUnit.Document);
					if(currentLinesHeight > visibleRect.Top - brickRect.Top) {
						startLineIndex = lineIndex;
						break;
					}
				}
			}
			int endLineIndex = multilineText.Length - 1;
			if(FloatsComparer.Default.FirstGreaterSecond(brickRect.Bottom, visibleRect.Bottom)) { 
				for(int lineIndex = multilineText.Length - 1; lineIndex >= 0; lineIndex--) {
					float currentLinesHeight = TextFormatter.CalculateHeightOfLines(visualBrick.Style.Font, lineIndex + 1, GraphicsUnit.Document);
					if(currentLinesHeight <= visibleRect.Bottom - brickRect.Top) {
						endLineIndex = lineIndex;
						break;
					}
				}
			}
			return string.Join(Environment.NewLine, multilineText, startLineIndex, (endLineIndex - startLineIndex) + 1);
		}
		static bool StringContainsWord(string text, string searchingWord) {
			char[] separators = new char[] { ' ', '.', ',', '-', ':', ';', '"', '(', ')', '&', '@', '[', ']', '{', '}', '\r', '\n', '\t' };
			string[] items = text.Split(separators);
			return items.Contains(searchingWord);
		}
		static string ReplaceNoBreakSpace(string text) {
			if(string.IsNullOrEmpty(text))
				return text;
			return text.Replace("\u00A0", " ");
		}
	}
}

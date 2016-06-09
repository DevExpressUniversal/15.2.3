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
namespace DevExpress.XtraPrinting.Native.TOC {
	public interface ITextGenerator {
		string GenerateText(float maxTextWidth);
	}
	class TableOfContentsLineTextGenerator : ITextGenerator {
		ITextFormatter textFormatter;
		public Measurer Measurer {
			get;
			set;
		}
		public GraphicsUnit GraphicsUnit {
			get;
			set;
		}
		public char LeaderSymbol {
			get;
			set;
		}
		public Font Font {
			get;
			set;
		}
		public BrickStyle Style {
			get;
			set;
		}
		public string Caption {
			get;
			set;
		}
		public float TextDivisionPosition {
			get;
			set;
		}
		protected virtual ITextFormatter CreateTextFormatter(GraphicsUnit graphicsUnit, Measurer measurer) {
			return new TextFormatter(graphicsUnit, measurer);
		}
		public string GenerateText(float maxTextWidth) {
			string fullCaption = Caption + " ";
			StringFormat sf = Style.StringFormat.Value;
			sf.SetTabStops(0, Style.CalculateTabStops(Measurer));
			float actualTextWidth = Measurer.MeasureString(fullCaption, Font, PointF.Empty, sf, GraphicsUnit).Width;
			string[] tocTextLines = null;
			if(actualTextWidth > maxTextWidth) {
				textFormatter = CreateTextFormatter(GraphicsUnit, Measurer);
				tocTextLines = textFormatter.FormatMultilineText(fullCaption, Font, TextDivisionPosition, float.MaxValue, Style.StringFormat.Value);
				tocTextLines[tocTextLines.Length - 1] = GenerateLastLine(tocTextLines[tocTextLines.Length - 1], maxTextWidth);
			} else {
				tocTextLines = new string[] { GenerateLastLine(fullCaption, maxTextWidth) };
			}
			return string.Join(Environment.NewLine, tocTextLines);
		}
		string GenerateLastLine(string captionLine, float maxTextWidth) {
			float captionLineWidth = Measurer.MeasureString(captionLine, Font, PointF.Empty, Style.StringFormat.Value, GraphicsUnit).Width;
			float leaderAreaWidth = maxTextWidth - captionLineWidth;
			SizeF singleLeaderSymbolSize = Measurer.MeasureString(LeaderSymbol.ToString(), Font, PointF.Empty, Style.StringFormat.Value, GraphicsUnit);
			int leaderSymbolCount = Convert.ToInt32(Math.Floor(leaderAreaWidth / singleLeaderSymbolSize.Width));
			string leader = (leaderSymbolCount > 0) ? new string(LeaderSymbol, leaderSymbolCount) : string.Empty;
			return captionLine + leader;
		}
	}
}

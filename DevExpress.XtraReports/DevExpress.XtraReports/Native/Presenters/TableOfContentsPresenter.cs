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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.TOC;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native.Presenters {
	class TableOfContentsPresenter : ControlPresenter {
		protected XRTableOfContents TableOfContents {
			get { return (XRTableOfContents)control; }
		}
		public TableOfContentsPresenter(XRTableOfContents tableOfContents) : base(tableOfContents) { }
		public override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return (VisualBrick)new SubreportBrick(TableOfContents);
		}
	}
	class DesignTableOfContentsPresenter : TableOfContentsPresenter {
		Color ControlContourColor = Color.FromArgb(64, Color.Black);
		TableOfContentsLineTextGenerator textGenerator;
		TableOfContentsLineTextGenerator TextGenerator {
			get { return textGenerator ?? (textGenerator = new TableOfContentsLineTextGenerator() { Measurer = Measurement.Measurer, GraphicsUnit = GraphicsUnit.Document }); }
		}
		public DesignTableOfContentsPresenter(XRTableOfContents tableOfContents) : base(tableOfContents) { }
		public override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new PanelBrick(TableOfContents);
		}
		public override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			brick.PrintingSystem = ps;
			int index = 0;
			BrickStyle panelStyle = new XRControlStyle(TableOfContents.Dpi) { Sides = BorderSide.Bottom, BorderColor = ControlContourColor, BorderWidth = 1 };
			TableOfContents.ForEachLevel((level, levelBounds) => {
				PanelBrick levelBrick = new PanelBrick(TableOfContents) { Style = panelStyle };
				VisualBrickHelper.SetBrickBounds(levelBrick, levelBounds, TableOfContents.Dpi);
				float indent = (level as XRTableOfContentsLevel) != null ? ((XRTableOfContentsLevel)level).GetIndent(Math.Max(0, index - 1)) : 0f;
				RectangleF levelBrickBounds = new RectangleF(indent, 0, TableOfContents.WidthF - indent, level.Height);
				LabelBrick childBrick = new LabelBrick() { Style = TableOfContents.GetLevelStyle(level) };
				VisualBrickHelper.SetBrickBounds(childBrick, levelBrickBounds, TableOfContents.Dpi);
				if(ReferenceEquals(level, TableOfContents.LevelTitle)) {
					childBrick.Text = ((XRTableOfContentsTitle)level).Text;
					childBrick.Style.StringFormat = childBrick.Style.StringFormat.ChangeFormatFlags(StringFormatFlags.MeasureTrailingSpaces);
				} else {
					childBrick.HorzAlignment = Utils.HorzAlignment.Near;
					childBrick.VertAlignment = Utils.VertAlignment.Bottom;
					childBrick.Style.StringFormat = childBrick.Style.StringFormat.ChangeFormatFlags(StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.MeasureTrailingSpaces);
					string text = ReferenceEquals(level, TableOfContents.LevelDefault) ? "Level (Default)" : "Level " + index;
					childBrick.Text = GetLevelText(text, ((XRTableOfContentsLevel)level).LeaderSymbol, levelBrickBounds.Width, childBrick.Style);
				}
				index++;
				levelBrick.Bricks.Add(childBrick);
				brick.Bricks.Add(levelBrick);
				return false;
			});
		}
		string GetLevelText(string caption, char leaderSymbol, float levelWidth, BrickStyle style) {
			TextGenerator.Caption = caption;
			TextGenerator.Font = style.Font;
			TextGenerator.LeaderSymbol = leaderSymbol;
			TextGenerator.Style = style;
			const string pageNumberPlaceholder = " #";
			StringFormat stringFormat = style.StringFormat.Value;
			float pageNumberPlaceholderWidthInDocuments = TextGenerator.Measurer.MeasureString(pageNumberPlaceholder, style.Font, PointF.Empty, stringFormat, GraphicsUnit.Document).Width;
			float widthInDocuments = GraphicsUnitConverter.Convert(style.Padding.DeflateWidth(levelWidth), TableOfContents.Dpi, GraphicsDpi.Document);
			return TextGenerator.GenerateText(widthInDocuments - pageNumberPlaceholderWidthInDocuments) + pageNumberPlaceholder;
		}
	}
}

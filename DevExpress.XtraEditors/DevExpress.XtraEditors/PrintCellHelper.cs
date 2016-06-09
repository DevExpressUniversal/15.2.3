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
using System.Text;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraEditors {
	public class PrintCellHelperInfo {
		Point cell;
		Color lineColor;
		IPrintingSystem ps;
		object editValue;
		AppearanceObject appearance;
		string displayText;
		Rectangle rect;
		IBrickGraphics graph;
		HorzAlignment alignmentCore;
		String formatString;
		bool printHLines;
		bool printVLines;
		DevExpress.XtraPrinting.BorderSide sides;
		public PrintCellHelperInfo(Color lineColor, IPrintingSystem ps, object editValue, AppearanceObject appearance, string displayText, Rectangle rect, IBrickGraphics graphics) : this(lineColor, ps, editValue, appearance, displayText, rect, graphics, HorzAlignment.Default, true, true, String.Empty) { }
		public PrintCellHelperInfo(Color lineColor, IPrintingSystem ps, object editValue, AppearanceObject appearance, string displayText, Rectangle rect, IBrickGraphics graphics, HorzAlignment alignment) : this(lineColor, ps, editValue, appearance, displayText, rect, graphics, alignment, true, true, String.Empty) { }
		public PrintCellHelperInfo(Color lineColor, IPrintingSystem ps, object editValue, AppearanceObject appearance, string displayText, Rectangle rect, IBrickGraphics graphics, HorzAlignment alignment, bool printHLines, bool printVLines) : this(lineColor, ps, editValue, appearance, displayText, rect, graphics, alignment, printHLines, printVLines, String.Empty) { }
		public PrintCellHelperInfo(Color lineColor, IPrintingSystem ps, object editValue, AppearanceObject appearance, string displayText, Rectangle rect, IBrickGraphics graphics, HorzAlignment alignment, bool printHLines, bool printVLines, string textvalueFormatString) : this(lineColor, ps, editValue, appearance, displayText, rect, graphics, alignment, printHLines, printVLines, textvalueFormatString, DevExpress.XtraPrinting.BorderSide.None) { }
		public PrintCellHelperInfo(Color lineColor, IPrintingSystem ps, object editValue, AppearanceObject appearance, string displayText, Rectangle rect, IBrickGraphics graphics, HorzAlignment alignment, bool printHLines, bool printVLines, string textvalueFormatString, DevExpress.XtraPrinting.BorderSide sides) :
			this(new Point(-1, -1), lineColor, ps, editValue, appearance, displayText, rect, graphics, alignment, printHLines, printVLines, textvalueFormatString, sides) { }
		public PrintCellHelperInfo(Point cell, Color lineColor, IPrintingSystem ps, object editValue, AppearanceObject appearance, string displayText, Rectangle rect, IBrickGraphics graphics, HorzAlignment alignment, bool printHLines, bool printVLines, string textvalueFormatString, DevExpress.XtraPrinting.BorderSide sides) {
			this.cell = cell;
			this.sides = sides;
			this.lineColor = lineColor;
			this.ps = ps;
			this.editValue = editValue;
			this.appearance = appearance;
			this.displayText = displayText;
			this.rect = rect;
			graph = graphics;
			alignmentCore = alignment;
			this.printHLines = printHLines;
			this.printVLines = printVLines;
			this.formatString = textvalueFormatString;
		}
		public Point Cell { get { return cell; } }
		public DevExpress.XtraPrinting.BorderSide Sides { get { return sides; } set { sides = value; } }
		public Color LineColor { get { return lineColor; } set { lineColor = value; } }
		public IPrintingSystem PS { get { return ps; } set { ps = value; } }
		public object EditValue { get { return editValue; } set { editValue = value; } }
		[Obsolete("Use Appearance"), System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public AppearanceObject Appearace { get { return Appearance; } set { Appearance = value; } }
		public AppearanceObject Appearance { get { return appearance; } set { appearance = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public Rectangle Rectangle { get { return rect; } set { rect = value; } }
		public IBrickGraphics Graph { get { return graph; } set { graph = value; } }
		public HorzAlignment HAlignment { get { return alignmentCore; } set { alignmentCore = value; } }
		public bool PrintHLines { get { return printHLines; } set { printHLines = value; } }
		public bool PrintVLines { get { return printVLines; } set { printVLines = value; } }
		public string TextValueFormatString { get { return formatString; } set { formatString = value; } }
		internal DevExpress.XtraPrinting.BorderSide GetSides() {
			if(Sides != DevExpress.XtraPrinting.BorderSide.None) return Sides;
			return RepositoryItem.GetBorderSides(PrintHLines, PrintVLines);
		}
	}
}

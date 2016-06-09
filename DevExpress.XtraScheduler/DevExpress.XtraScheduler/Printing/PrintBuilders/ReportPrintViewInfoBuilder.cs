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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	class ReportPrintViewInfoBuilder : PrintViewInfoBuilder {
		int horizontalSpaceSize;
		int lineHeight;
		TableViewInfo table;
		AppearanceObject headerAppearance;
		AppearanceObject textAppearance;
		AppearanceObject captionAppearance;
		public ReportPrintViewInfoBuilder(SchedulerPrintStyle printStyle, SchedulerControl control, GraphicsInfo gInfo)
			: base(printStyle, control, gInfo) {
			horizontalSpaceSize = 10;
			lineHeight = 8;
			table = new TableViewInfo();
			CreateAppearanceObjects();
		}
		protected internal virtual TableViewInfo Table { get { return table; } }
		protected AppearanceObject HeaderAppearance { get { return headerAppearance; } }
		protected AppearanceObject TextAppearance { get { return textAppearance; } }
		protected AppearanceObject CaptionAppearance { get { return captionAppearance; } }
		public override IPrintableObjectViewInfo CreateViewInfo(Rectangle pageBounds) {
			return null;
		}
		protected void CreateAppearanceObjects() {
			CreateHeaderAppearance();
			CreateTextAppearance();
			CreateCaptionAppearance();
		}
		protected void CreateHeaderAppearance() {
			headerAppearance = new AppearanceObject();
			headerAppearance.TextOptions.HAlignment = HorzAlignment.Center;
			headerAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			headerAppearance.Font = PrintStyle.HeadingsFont;
			headerAppearance.ForeColor = Color.Black;
		}
		protected void CreateTextAppearance() {
			textAppearance = new AppearanceObject();
			textAppearance.Font = PrintStyle.AppointmentFont;
			textAppearance.ForeColor = Color.Black;
			textAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			textAppearance.TextOptions.HAlignment = HorzAlignment.Near;
			textAppearance.TextOptions.VAlignment = VertAlignment.Top;
		}
		protected RowPrintViewInfo CreateEmptyRow() {
			RowPrintViewInfo emptyRow = new RowPrintViewInfo();
			BlankCellPrintViewInfo verticalSpaceCell = new BlankCellPrintViewInfo(0, textAppearance.FontHeight);
			emptyRow.AddCell(verticalSpaceCell);
			return emptyRow;
		}
		protected RowPrintViewInfo CreatePaddingDownRow() {
			RowPrintViewInfo paddingDownRow = new RowPrintViewInfo();
			BlankCellPrintViewInfo verticalSpaceCell = new BlankCellPrintViewInfo(0, textAppearance.FontHeight / 2);
			paddingDownRow.AddCell(verticalSpaceCell);
			return paddingDownRow;
		}
		protected virtual void AddLine() {
			RowPrintViewInfo lineRow = CreateLineRow();
			Table.AddRow(lineRow);
		}
		protected RowPrintViewInfo CreateLineRow() {
			RowPrintViewInfo lineRow = new RowPrintViewInfo();
			SolidLineCellPrintViewInfo lineCell = new SolidLineCellPrintViewInfo(lineHeight);
			lineRow.AddCell(lineCell);
			return lineRow;
		}
		protected virtual void AddEmptyRow() {
			RowPrintViewInfo emptyRow = CreateEmptyRow();
			Table.AddRow(emptyRow);
		}
		protected internal virtual void AddPaddingDownRow() {
			RowPrintViewInfo paddingDownRow = CreatePaddingDownRow();
			Table.AddRow(paddingDownRow);
		}
		protected internal virtual RowPrintViewInfo CreateRow(SchedulerStringId captionId, string content) {
			return CreateRow(SchedulerLocalizer.GetString(captionId), content);
		}
		protected internal virtual RowPrintViewInfo CreateRow(string caption, string content) {
			RowPrintViewInfo row = new RowPrintViewInfo();
			TextCellPrintViewInfo captionTextCell = new TextCellPrintViewInfo(caption, CaptionAppearance);
			TextCellPrintViewInfo bodyTextCell = new TextCellPrintViewInfo(content, TextAppearance);
			BlankCellPrintViewInfo horizontalSpaceCell = new BlankCellPrintViewInfo(horizontalSpaceSize, 0);
			row.AddCell(captionTextCell);
			row.AddCell(horizontalSpaceCell);
			row.AddCell(bodyTextCell);
			return row;
		}
		protected internal virtual RowPrintViewInfo CreateContentRow(string content) {
			RowPrintViewInfo row = new RowPrintViewInfo();
			TextCellPrintViewInfo bodyTextCell = new TextCellPrintViewInfo(content, TextAppearance);
			row.AddCell(bodyTextCell);
			return CreateTextRow(content, TextAppearance);
		}
		protected internal virtual RowPrintViewInfo CreateHeaderRow(string header) {
			return CreateTextRow(header, HeaderAppearance);
		}
		protected internal virtual void AddPageBreak() {
			RowPrintViewInfo row = new RowPrintViewInfo();
			PageBreakCellPrintViewInfo cell = new PageBreakCellPrintViewInfo();
			row.AddCell(cell);
			Table.AddRow(row);
		}
		void CreateCaptionAppearance() {
			captionAppearance = new AppearanceObject();
			Font font = PrintStyle.AppointmentFont;
			captionAppearance.Font = new Font(font.FontFamily, font.Size, font.Style | FontStyle.Bold, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
			captionAppearance.ForeColor = Color.Black;
			captionAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			captionAppearance.TextOptions.HAlignment = HorzAlignment.Near;
			captionAppearance.TextOptions.VAlignment = VertAlignment.Top;
		}
		RowPrintViewInfo CreateTextRow(string text, AppearanceObject appearance) {
			RowPrintViewInfo row = new RowPrintViewInfo();
			TextCellPrintViewInfo bodyTextCell = new TextCellPrintViewInfo(text, appearance);
			row.AddCell(bodyTextCell);
			return row;
		}
	}
}

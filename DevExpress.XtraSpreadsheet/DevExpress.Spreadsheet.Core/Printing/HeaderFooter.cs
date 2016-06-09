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

using DevExpress.Office.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.XtraSpreadsheet.Printing {
	public partial class PrintingDocumentExporter {
		#region Fields
		static Font defaultHeaderFooterFont = new Font("Calibri", 11f, FontStyle.Regular);
		HeaderFooterLayout currentHeaderFooterLayout;
		HeaderFooterPageCounter headerFooterPageCounter = new HeaderFooterPageCounter();
		int currentHeaderFooterPageIndex;
		#endregion
		void UpdateCurrentHeaderFooterLayout(Worksheet sheet, int totalPages) {
			HeaderFooterLayout layout = documentLayout.GetHeaderFooterLayout(sheet.Name);
			if (layout != null && layout != currentHeaderFooterLayout)
				layout.UpdateTotalPageTag(totalPages);
			this.currentHeaderFooterLayout = layout;
			this.currentHeaderFooterPageIndex = headerFooterPageCounter.GetPageIndex(sheet);
		}
		void DrawHeader(int pageIndex) {
			if (currentHeaderFooterLayout == null)
				return;
			HeaderFooterLayoutItem header = currentHeaderFooterLayout.GetActualHeader(pageIndex);
			DrawHeaderFooterCore(header, TextAlignment.TopLeft, TextAlignment.TopCenter, TextAlignment.TopRight);
		}
		void DrawFooter(int pageIndex) {
			if (currentHeaderFooterLayout == null)
				return;
			HeaderFooterLayoutItem footer = currentHeaderFooterLayout.GetActualFooter(pageIndex);
			DrawHeaderFooterCore(footer, TextAlignment.BottomLeft, TextAlignment.BottomCenter, TextAlignment.BottomRight);
		}
		void DrawHeaderFooterCore(HeaderFooterLayoutItem headerFooterItem, TextAlignment left, TextAlignment center, TextAlignment right) {
			if (headerFooterItem == null)
				return;
			Rectangle headerFooterBounds = currentHeaderFooterLayout.TotalBounds;
			headerFooterBounds = new Rectangle(offset.X + headerFooterBounds.X, offset.Y + headerFooterBounds.Y, headerFooterBounds.Width, headerFooterBounds.Height);
			headerFooterBounds = DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(headerFooterBounds);
			DrawHeaderFooterBrick(headerFooterItem.GetFormattedLeftPlainText(currentHeaderFooterPageIndex), headerFooterBounds, left);
			DrawHeaderFooterBrick(headerFooterItem.GetFormattedCenterPlainText(currentHeaderFooterPageIndex), headerFooterBounds, center);
			DrawHeaderFooterBrick(headerFooterItem.GetFormattedRightPlainText(currentHeaderFooterPageIndex), headerFooterBounds, right);
		}
		void DrawHeaderFooterBrick(string plainText, Rectangle bounds, TextAlignment alignment) {
			if (String.IsNullOrEmpty(plainText))
				return;
			TextBrick brick = new TextBrick();
			brick.NoClip = false;
			brick.BackColor = Color.Transparent;
			brick.Text = plainText;
			brick.Font = defaultHeaderFooterFont;
			brick.StringFormat = BrickStringFormat.Create(alignment, true);
			brick.Rect = bounds;
			printingSystem.Graph.DrawBrick(brick);
		}
	}
}

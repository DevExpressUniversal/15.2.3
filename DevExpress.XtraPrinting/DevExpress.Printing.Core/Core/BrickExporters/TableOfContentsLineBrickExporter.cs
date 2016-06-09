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
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using System.Drawing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrinting.BrickExporters {
	class TableOfContentsLineBrickExporter : RowBrickExporter {
		TableOfContentsLineBrick TableOfContentsLineBrick { get { return Brick as TableOfContentsLineBrick; } }
		protected override void ValidateInnerData(BrickViewData[] innerData, ExportContext exportContext) {
			ITableCell tableCell = innerData[0].TableCell;
			if(!IsPagedExport(exportContext) && ReferenceEquals(tableCell, TableOfContentsLineBrick.PageNumberBrick)) {
				innerData[0].TableCell = new TextCell(tableCell, string.Empty);
			} else if(!IsPagedExport(exportContext) && ReferenceEquals(tableCell, TableOfContentsLineBrick.CaptionBrick)) {
				innerData[0].TableCell = new TextCell(tableCell, GetCaptionText());
			} else if(IsPagedExport(exportContext) && (exportContext is XlsExportContext || exportContext is HtmlExportContext) &&
				ReferenceEquals(tableCell, TableOfContentsLineBrick.CaptionBrick)) {
				innerData[0].TableCell = new TextCell(tableCell, GetCaptionText());
			}
		}
		static bool IsPagedExport(ExportContext exportContext) {
			return exportContext is XlsExportContext ? ((XlsExportContext)exportContext).XlsExportOptions.IsMultiplePaged :
				exportContext is RtfExportContext ? ((RtfExportContext)exportContext).IsPageByPage :
				exportContext is HtmlExportContext ? ((HtmlExportContext)exportContext).IsPageExport :
				false;
		}
		string GetCaptionText() {
			char[] charsToTrim = { TableOfContentsLineBrick.LeaderSymbol };
			return TableOfContentsLineBrick.CaptionBrick.Text.TrimEnd(charsToTrim);
		}
	}
}

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

using DevExpress.Web;
using DevExpress.Web.Export;
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.Internal;
	[ToolboxItem(false)]
	public class MVCxCardViewExporter : ASPxCardViewExporter {
		public MVCxCardViewExporter(MVCxCardView cardView)
			: base(cardView) {
		}
		protected override GridLinkBase GetPrintableLink() {
			return new MVCxCardViewLink(this);
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.Mvc;
	using DevExpress.XtraPrinting;
	using System.Drawing;
	public class MVCxCardViewLink : CardViewLink {
		public MVCxCardViewLink(MVCxCardViewExporter exporter)
			: base(exporter) {
		}
		protected override void CreateFirstPrinter(ASPxGridBase grid) {
			var graph = PrintingSystemBase != null ? PrintingSystemBase.Graph : null;
			Printers.Push(new MVCxCardViewPrinter((MVCxCardViewExporter)Exporter, graph));
		}
	}
	public class MVCxCardViewPrinter : CardViewPrinter {
		public MVCxCardViewPrinter(MVCxCardViewExporter exporter, BrickGraphics graph)
			: base(exporter, graph) {
		}
		public override CardViewPrintInfoBase CreatePrintInfoItem(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int colIndex, int rowIndex) {
			if(layoutItem is CardViewCommandLayoutItem)
				return null;
			if(layoutItem is CardViewLayoutGroup)
				return new CardViewGroupPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			if(layoutItem is CardViewTabbedLayoutGroup)
				return new CardViewTabbedGroupPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			var column = layoutItem is CardViewColumnLayoutItem ? ((CardViewColumnLayoutItem)layoutItem).Column : null;
			if(column == null)
				return new CardViewTextPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			var adapter = column.ColumnAdapter as MVCxGridDataColumnAdapter;
			if(adapter != null) {
				if(adapter.ColumnType == (int)MVCxCardViewColumnType.BinaryImage)
					return new MVCxCardViewImagePrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
				if(adapter.ColumnType == (int)MVCxCardViewColumnType.CheckBox)
					return new CardViewCheckPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
			}
			return new CardViewTextPrintInfo(printInfoOwner, layoutItem, colIndex, rowIndex);
		}
	}
	public class MVCxCardViewImagePrintInfo : CardViewImagePrintInfo {
		public MVCxCardViewImagePrintInfo(CardViewGroupPrintInfoBase printInfoOwner, LayoutItemBase layoutItem, int startCol, int startRow)
			: base(printInfoOwner, layoutItem, startCol, startRow) {
		}
		protected override Size GetDataObjectSize(BrickGraphics graph, CardViewStyleHelper styleHelper, int maxCellWidth) {
			var imageExportSettings = PrintingHelper.GetColumnExportProperties(Column) as IImageExportSettings;
			if(imageExportSettings == null)
				return Size.Empty;
			var textBuilder = Printer.TextBuilder;
			var width = imageExportSettings.Width < maxCellWidth ? imageExportSettings.Width : maxCellWidth;
			var style = styleHelper.GetCellStyle(graph, Column, CardIndex, false, textBuilder.GetColumnDisplayControlDefaultAlignment(Column), false, true);
			return styleHelper.CalcImageSize(width, imageExportSettings.Height, graph, style, false);
		}
	}
}

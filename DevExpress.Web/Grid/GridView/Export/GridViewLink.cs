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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.Export {
	public class GridViewLink : GridLinkBase {
		ASPxGridView grid;
		ASPxGridViewExporter exporter;
		GridExportStyleHelper styleHelper;
		public GridViewLink(ASPxGridViewExporter exporter)
			: base(exporter) {
		}
		protected ASPxGridView Grid {
			get {
				if(grid == null)
					grid = (ASPxGridView)GridBase;
				return grid;
			}
		}
		protected ASPxGridViewExporter Exporter {
			get {
				if(exporter == null)
					exporter = (ASPxGridViewExporter)ExporterBase;
				return exporter;
			}
		}
		protected internal GridExportStyleHelper StyleHelper {
			get {
				if(styleHelper == null)
					styleHelper = new GridExportStyleHelper(Exporter.Styles);
				return styleHelper;
			}
		}
		protected override void CreateFirstPrinter(ASPxGridBase grid) {
			AddPrinter((ASPxGridView)grid, 0);
		}
		protected virtual void AddPrinter(ASPxGridView grid, int detailGridIndent) {
			Printers.Push(new GridViewPrinter(Exporter, grid, PrintingSystemBase.Graph, StyleHelper, OnDrawDetailGrid, Printers.Count, detailGridIndent));
		}
		public override void CreateArea(string areaName, IBrickGraphics graph) {
			if(areaName == SR.DetailHeader)
				CreateDetailHeader((BrickGraphics)graph);
			if(areaName == SR.Detail)
				CreateDetail((BrickGraphics)graph);
		}
		protected override void CreateDetailHeader(BrickGraphics graph) {
			((GridViewPrinter)ActivePrinter).CreateDetailHeader();
		}
		protected override void CreateDetail(BrickGraphics graph) {
			((GridViewPrinter)ActivePrinter).CreateDetail();
		}
		protected void OnDrawDetailGrid(ASPxGridView detailGrid, int indent) {
			AddPrinter(detailGrid, indent);
			AddSubreport(new PointF(0, Exporter.DetailVerticalOffset));
			RemovePrinter();
		}
	}
}

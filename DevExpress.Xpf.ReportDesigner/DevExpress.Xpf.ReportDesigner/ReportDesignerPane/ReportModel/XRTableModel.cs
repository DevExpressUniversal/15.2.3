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
using System.Linq;
using System.Windows.Media;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
#if NEWTABLE
	public class XRTableModel : XRContainerModelBase<XRTable, XRTableRowModel, XRTableDiagramItem_NewTable> {
#else
	public class XRTableModel : XRContainerModelBase<XRTable, XRTableRowModel, XRTableDiagramItem> {
#endif
		protected internal XRTableModel(XRControlModelFactory.ISource<XRTable> factory, ImageSource icon) 
			: base(factory, icon) { }
		protected override void InitializeNewXRControl(XRTable xrTable, XtraReportModel model) {
			base.InitializeNewXRControl(xrTable, model);
			float tableCellWidth = xrTable.WidthF / 3;
			var tableCellHeight = xrTable.HeightF;
			if(xrTable.Rows.Count == 0) {
				var xrTableRow = new XRTableRow() { WidthF = xrTable.WidthF, HeightF = tableCellHeight };
				xrTableRow.Controls.AddRange(new[] {
					new XRTableCell() { WidthF = tableCellWidth, HeightF = tableCellHeight },
					new XRTableCell() { WidthF = tableCellWidth, HeightF = tableCellHeight },
					new XRTableCell() { WidthF = tableCellWidth, HeightF = tableCellHeight }
				});
				xrTable.Controls.Add(xrTableRow);
			} else {
				tableCellWidth = xrTable.WidthF / xrTable.Rows[0].Cells.Count;
				xrTable.Rows[0].WidthF = xrTable.WidthF;
				xrTable.Rows[0].HeightF = tableCellHeight;
				foreach(XRTableCell cell in xrTable.Rows[0].Cells) {
					cell.WidthF = tableCellWidth;
					cell.HeightF = tableCellHeight;
				}   
			}
		}
	}
}

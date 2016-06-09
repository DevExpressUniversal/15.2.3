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
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Export;
	using DevExpress.Web.Mvc.Internal;
	public delegate void GridViewGetExportDetailGridViewEventHandler(object sender, GridViewExportDetailRowEventArgs e);
	public class GridViewExportDetailRowEventArgs: EventArgs {
		public GridViewExportDetailRowEventArgs(int rowIndex, object dataItem) {
			RowIndex = rowIndex;
			DataItem = dataItem;
			DetailGridViews = new List<GridViewExtension>();
		}
		public int RowIndex { get; private set; }
		public object DataItem { get; private set; }
		public List<GridViewExtension> DetailGridViews { get; private set; }
	}
	[ToolboxItem(false)]
	public class MVCxGridViewExporter : ASPxGridViewExporter {
		MVCxGridView gridView;
		static readonly object getExportDetailGridViews = new object();
		public MVCxGridViewExporter(MVCxGridView gridView)
			: base() {
			this.gridView = gridView;
			GetExportDetailGridViewsHandlerHash = new Dictionary<string, GridViewGetExportDetailGridViewEventHandler>();
		}
		protected override ASPxGridBase GetGrid() {
			return this.gridView;
		}
		protected override GridLinkBase GetPrintableLink() {
			return new MVCxGridViewLink(this);
		}
		internal IDictionary<string, GridViewGetExportDetailGridViewEventHandler> GetExportDetailGridViewsHandlerHash { get; private set; }
		internal List<GridViewExtension> GetExportDetailGridViews(string gridID, int rowIndex, object dataItem) {
			if(!GetExportDetailGridViewsHandlerHash.ContainsKey(gridID))
				return new List<GridViewExtension>();
			var e = new GridViewExportDetailRowEventArgs(rowIndex, dataItem);
			GetExportDetailGridViewsHandlerHash[gridID](this, e);
			return e.DetailGridViews;
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web;
	using DevExpress.Web.Export;
	using DevExpress.Web.Data;
	using DevExpress.XtraPrinting;
	public class MVCxGridViewLink: GridViewLink {
		public MVCxGridViewLink(MVCxGridViewExporter exporter)
			: base(exporter) {
		}
		protected override void AddPrinter(ASPxGridView grid, int detailGridIndent) {
			Printers.Push(new MVCxGridViewPrinter(Exporter, grid, PrintingSystemBase.Graph, StyleHelper, OnDrawDetailGrid, Printers.Count, detailGridIndent));
		}
	}
	public class MVCxGridViewPrinter: GridViewPrinter {
		public MVCxGridViewPrinter(ASPxGridViewExporter exporter, ASPxGridView grid, BrickGraphics graph, GridExportStyleHelper styleHelper, GridViewPrinterDrawDetailGrid onDrawDetailGrid, int level, int indent)
			: base(exporter, grid, graph, styleHelper, onDrawDetailGrid, level, indent) {
		}
		public new MVCxGridViewExporter Exporter { get { return (MVCxGridViewExporter)base.Exporter; } }
		protected override void DrawDetailRow(int rowIndex) {
			if (!WillDetailRowDrawn(rowIndex))
				return;
			int indent = GetGroupLevelOffSetByRowIndex(rowIndex) + (1 + Level) * Exporter.DetailHorizontalOffset;
			List<GridViewExtension> detailGridViewExtensions = Exporter.GetExportDetailGridViews(Grid.ClientID, rowIndex, DataProxy.GetRowForTemplate(rowIndex));
			detailGridViewExtensions.Sort(DoDetailGridCompare);
			foreach (GridViewExtension extension in detailGridViewExtensions) {
				Exporter.GetExportDetailGridViewsHandlerHash[extension.Control.ClientID] = extension.Settings.SettingsExport.GetExportDetailGridViews;
				extension.PrepareControl();
				extension.LoadPostData();
				var grid = (ASPxGridView)extension.Control;
				if (Exporter.ExportEmptyDetailGrid || grid.VisibleRowCount > 0) {
					GridViewExtension.RaiseBeforeExportEvent(extension);
					DrawDetailGrid(grid, indent);
				}
				extension.DisposeControl();
			}
		}
		bool WillDetailRowDrawn(int rowIndex) {
			return Grid.SettingsDetail.ExportMode != GridViewDetailExportMode.None && DataProxy.GetRowType(rowIndex) == WebRowType.Data && 
				(Grid.DetailRows.IsVisible(rowIndex) || Grid.SettingsDetail.ExportMode == GridViewDetailExportMode.All);
		}
		int DoDetailGridCompare(GridViewExtension g1, GridViewExtension g2) {
			return Comparer<int>.Default.Compare(g1.Settings.SettingsDetail.ExportIndex, g2.Settings.SettingsDetail.ExportIndex);
		}
	}
}

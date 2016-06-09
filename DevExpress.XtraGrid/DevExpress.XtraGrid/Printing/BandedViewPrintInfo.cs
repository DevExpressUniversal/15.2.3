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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraGrid.Views.Printing {
	public class BandedGridViewPrintInfo : GridViewPrintInfo {
		PrintBandInfoCollection bands = new PrintBandInfoCollection();
		int bandRowHeight, bandsRowCount;
		public BandedGridViewPrintInfo(PrintInfoArgs args) : base(args) { }
		protected int BandRowHeight { get { return bandRowHeight; } }
		protected int BandsRowCount { get { return bandsRowCount; } }
		protected new BandedGridViewInfo ViewViewInfo { get { return base.ViewViewInfo as BandedGridViewInfo; } }
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		protected PrintBandInfoCollection Bands { get { return bands; } }
		protected override void CreateColumns() {
			CreateBands();
			base.CreateColumns();
		}
		protected override void CreateInfo() {
			base.CreateInfo();
		}
		protected override BaseViewAppearanceCollection CreatePrintAppearance() { return new BandedViewPrintAppearances(View);	}
		public new BandedViewPrintAppearances AppearancePrint { get { return base.AppearancePrint as BandedViewPrintAppearances; } }
		public override void Initialize() {
			base.Initialize();
			Bricks.Add("BandPanel", AppearancePrint.BandPanel, BorderSide.All, AppearancePrint.BandPanel.BorderColor, 1);
		}
		protected override void UpdateAppearances() {
			base.UpdateAppearances();
			if(!View.OptionsPrint.UsePrintStyles) {
				AppearanceDefaultInfo[] info = View.BaseInfo.GetDefaultPrintAppearance();
				if(View.IsSkinned) {
					AppearancePrint.BandPanel.Assign(Find("BandPanel", info));
				}
			}
		}
		protected override int HeaderY { get { return View.OptionsPrint.PrintBandHeader ? BandsRowCount * BandRowHeight : 0; } }
		public override void PrintHeader(DevExpress.XtraPrinting.IBrickGraphics graph) {
			PrintBandHeader(graph);
			base.PrintHeader(graph);
		}
		protected virtual void PrintBandHeader(DevExpress.XtraPrinting.IBrickGraphics graph) {
			if(!View.OptionsPrint.PrintBandHeader) return;
			Rectangle r = Rectangle.Empty;
			Point indent = new Point(Indent, 0);
			bool usePrintStyles = View.OptionsPrint.UsePrintStyles;
			SetDefaultBrickStyle(graph, Bricks["BandPanel"]);
			foreach(PrintBandInfo band in Bands) {
				r = band.Bounds;
				r.Offset(indent);
				if(!usePrintStyles && band.Band != null) {
					AppearanceObject temp = new AppearanceObject();
					AppearanceHelper.Combine(temp, new AppearanceObject[] { band.Band.AppearanceHeader, AppearancePrint.BandPanel});
					SetDefaultBrickStyle(graph, Bricks.Create(temp, BorderSide.All, temp.BorderColor, 1));
				}
				DrawTextBrick(graph, band.Band.GetTextCaption(), r);
			}
		}
		protected override void CreatePrintColumnCollection() {
		}
		protected virtual void OnBandInfoAdded(PrintBandInfo bandInfo) {
			this.fMaxRowWidth = Math.Max(bandInfo.Bounds.Right, this.fMaxRowWidth);
			if(bandInfo.Band.HasChildren || bandInfo.Band.Columns.VisibleColumnCount == 0) return;
			GridBandRowCollection rows = View.GetBandRows(bandInfo.Band);
			if(rows == null || rows.Count == 0) return;
			AddBandColumns(bandInfo, rows);
		}
		protected int RowHeightInternal() {
			return CurrentRowHeight == 0 ? headerRowHeight : CurrentRowHeight;
		}
		protected int CalcBandColumnY(object colInfo) {
			PrintColumnInfo pci = colInfo as PrintColumnInfo;
			BandedGridColumn column = colInfo as BandedGridColumn;
			BandedGridColumn bgc = null;
			if(pci != null) bgc = pci.Column as BandedGridColumn;
			if(column != null) bgc = colInfo as BandedGridColumn;
			int result = 0;
			if(bgc == null) return pci.RowIndex * RowHeightInternal();
			else {
				if(bgc.OwnerBand != null) {
					Hashtable ht = new Hashtable();
					foreach(BandedGridColumn current in bgc.OwnerBand.Columns) {
						int currentHeight = current.RowCount * RowHeightInternal();
						if(bgc.RowIndex > current.RowIndex) {
							if(!ht.Contains(current.RowIndex)) {
								result += currentHeight;
								ht.Add(current.RowIndex, currentHeight);
							} else {
								if((int)ht[current.RowIndex] < currentHeight) {
									result -= (int)ht[current.RowIndex];
									result += currentHeight;
									ht[current.RowIndex] = currentHeight;
								}
							}
						}
					}
				}
			}
			return result;
		}
		protected virtual void AddBandColumns(PrintBandInfo bandInfo, GridBandRowCollection rows) {
			int lastRight;
			for(int n = 0; n < rows.Count; n++) {
				GridBandRow row = rows[n];
				lastRight = bandInfo.Bounds.X;
				foreach(BandedGridColumn column in row.Columns) {
					if(column.VisibleIndex < 0 || column.OptionsColumn.Printable == DevExpress.Utils.DefaultBoolean.False) continue;
					bool isLastRow = n == rows.Count - 1;
					lastRight = AddBandColumn(column, lastRight, CalcBandColumnY(column), n, isLastRow, row, bandInfo.Bounds.Right);
				}
				this.fMaxRowWidth = Math.Max(lastRight, this.fMaxRowWidth);
			}
		}
		protected virtual int AddBandColumn(BandedGridColumn column, int lastRight, int lastBottom, int rowIndex, bool isLastRow, GridBandRow row, int maxRight) {
			PrintColumnInfo col = new PrintColumnInfo(column);
			int rowCount = CalcColumnRowCount(column, rowIndex, isLastRow, row);
			col.RowCount = rowCount;
			col.RowIndex = rowIndex;
			int width = Math.Max(0, Math.Min(column.VisibleWidth, maxRight - lastRight));
			col.Bounds = new Rectangle(lastRight, lastBottom, width, rowCount * this.headerRowHeight);
			Columns.Add(col);
			return col.Bounds.Right;
		}
		protected virtual int CalcColumnRowCount(BandedGridColumn column, int rowIndex, bool isLastRow, GridBandRow row) {
			return 1;
		}
		protected override void UpdateWidthCache(bool push) {
			base.UpdateWidthCache(push);
			UpdateBandsWidthCache(push, View.Bands);
		}
		protected virtual void UpdateBandsWidthCache(bool push, GridBandCollection bands) {
			foreach(GridBand band in bands) {
				if(!band.Visible) continue;
				UpdateWidthCache(band, push);
				if(band.HasChildren) UpdateBandsWidthCache(push, band.Children);
			}
		}
		protected override void UpdateWidthCache(object obj, bool push) {
			base.UpdateWidthCache(obj, push);
			GridBand band = obj as GridBand;
			if(band == null) return;
			if(push) {
				WidthCache[band] = new Point(band.Width, band.VisibleWidth);
			} else {
				if(!WidthCache.ContainsKey(band)) return;
				Point p = (Point)WidthCache[band];
				band.SetWidthCore(p.X);
				band.SetVisibleWidthCore(p.Y);
			}
		}
		protected virtual void CreateBands() {
			this.headerRowHeight = Math.Max(CalcStyleHeight(AppearancePrint.HeaderPanel), View.ColumnPanelRowHeight) + 4;
			this.bandRowHeight = Math.Max(CalcStyleHeight(AppearancePrint.HeaderPanel), View.BandPanelRowHeight) + 4;
			this.bandsRowCount = ViewViewInfo.CalcBandRowCount();
			Bands.Clear();
			AddBands(View.Bands, 0, 0);
		}
		protected virtual void AddBands(GridBandCollection bandCollection, int rowIndex, int lastRight) {
			foreach(GridBand band in bandCollection) {
				if(!band.Visible) continue;
				lastRight = AddBand(band, lastRight, rowIndex);
			}
		}
		protected virtual int AddBand(GridBand band, int lastRight, int rowIndex) {
			PrintBandInfo bandInfo = new PrintBandInfo(band);
			int rowCount = band.RowCount;
			bool hasVisibleChildren = band.HasChildren && band.Children.VisibleBandCount > 0;
			if(!hasVisibleChildren && band.AutoFillDown) {
				rowCount = BandsRowCount - rowIndex;
			}
			bandInfo.Bounds = new Rectangle(lastRight, rowIndex * BandRowHeight, band.VisibleWidth, rowCount * BandRowHeight);
			Bands.Add(bandInfo);
			if(hasVisibleChildren) {
				AddBands(band.Children, rowIndex + rowCount, lastRight);
			}
			OnBandInfoAdded(bandInfo);
			return bandInfo.Bounds.Right;
		}
	}
	public class PrintBandInfoCollection : CollectionBase {
		public PrintBandInfo this[int index] { get { return List[index] as PrintBandInfo; } }
		public int Add(PrintBandInfo band) { return List.Add(band); }
	}
	public class PrintBandInfo {
		public GridBand Band;
		public Rectangle Bounds;
		public PrintBandInfo(GridBand band) {
			this.Band = band;
			this.Bounds = Rectangle.Empty;
		}
	}
}

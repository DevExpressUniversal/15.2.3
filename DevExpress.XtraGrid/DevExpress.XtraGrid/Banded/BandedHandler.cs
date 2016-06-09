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
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.BandedGrid.Customization;
using DevExpress.XtraGrid.Menu;
namespace DevExpress.XtraGrid.Views.BandedGrid.Handler {
	public class BandedGridHandler : GridHandler {
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public new BandedGridViewInfo ViewInfo { get { return base.ViewInfo as BandedGridViewInfo; } }
		public new BandedGridHitInfo DownPointHitInfo { 
			get { return base.DownPointHitInfo as BandedGridHitInfo; }
			set { base.DownPointHitInfo = value;
			}
		}
		public BandedGridHandler(BandedGridView gridView) : base(gridView) {
		}
		protected override GridDragManager CreateDragManager() { return new BandedGridDragManager(View); }
		public override void DoClickAction(BaseHitInfo hitInfo) {
			BandedGridHitInfo hit = hitInfo as BandedGridHitInfo;
			if(hit.HitTest == BandedGridHitTest.Band) {
				if(View.GridControl.IsDesignMode) 
					View.GridControl.SetComponentsSelected(new object[] {hit.Band});
			}
			base.DoClickAction(hitInfo);
		}
		protected override bool CanResizeObject(GridHitInfo hit) {
			BandedGridHitInfo hi = hit as BandedGridHitInfo;
			if(hi.HitTest == BandedGridHitTest.BandEdge) {
				if(View.CanResizeBand(hi.Band)) {
					this.fSizingCursor = Cursors.SizeWE;
					return true;
				}
			}
			return base.CanResizeObject(hit);
		}
		GridBand CheckIfResizeBand(GridHitInfo hit) {
			BandedGridHitInfo bhit = hit as BandedGridHitInfo;
			if(bhit.HitTest == BandedGridHitTest.BandEdge) return bhit.Band;
			if(bhit.HitTest != BandedGridHitTest.ColumnEdge) return null;
			GridBandRow row = View.GetBandRows(bhit.Column.OwnerBand).FindRow(bhit.Column);
			if(row == null) return null;
			int index = row.Columns.IndexOf(bhit.Column);
			if(index == row.Columns.Count - 1) {
				return bhit.Column.OwnerBand;
			}
			return null;
		}
		protected override void DoStartResize(GridHitInfo hit) {
			BandedGridHitInfo hi = hit as BandedGridHitInfo;
			GridBand resizeBand = CheckIfResizeBand(hit);
			if(resizeBand != null) {
				DoStartBandSizing(resizeBand, hi.HitPoint);
				return;
			}
			base.DoStartResize(hi);
		}
		protected virtual void DoStartBandSizing(GridBand band, Point p) {
			if(!View.CanResizeBand(band)) return;
			Painter.HideSizerLine();
			Painter.CurrentSizerPos = Painter.StartSizerPos = p.X;
			View.SetStateCore(BandedGridState.BandSizing);
			Painter.ReSizingObject = band;
			Painter.ShowSizerLine();
			this.fSizingCursor = Cursors.SizeWE;
			View.SetCursor(this.fSizingCursor);
		}
		protected override void DoSizing(Point p) {
			int newPosition = -10000;
			if(View.State == BandedGridState.BandSizing) {
				GridBand band = Painter.ReSizingObject as GridBand;
				if(band == null) return;
				GridBandInfoArgs info = ViewInfo.BandsInfo.FindBand(band);
				newPosition = ValidateColumnSizingPosition(p.X, ViewInfo.ViewRects.BandPanel, info, View.GetBandMinWidth(band));
				if(newPosition != -10000 && newPosition != Painter.CurrentSizerPos) {
					Painter.HideSizerLine();
					Painter.CurrentSizerPos = newPosition;
					Painter.ShowSizerLine();
				}
				return;
			}
			base.DoSizing(p);
		}
		private int ValidateBandSizingPosition() {
			throw new NotImplementedException();
		}
		protected override object CanStartDragObject(GridHitInfo hi, Point offs) {
			if((offs.X > DragDeltaStart || offs.Y > DragDeltaStart) && DownPointHitInfo.Band != null && View.State == BandedGridState.BandDown) {
				if(View.CanDragBand(DownPointHitInfo.Band)) return DownPointHitInfo.Band;
			}
			return base.CanStartDragObject(hi, offs);
		}
		public override void DoStartDragObject(object drag, Size size) {
			if(drag is GridBand) {
				if(!View.RaiseDragObjectStart(new DragObjectStartEventArgs(drag))) return;
				View.SetStateCore(BandedGridState.BandDragging);
				if(View.CustomizationForm != null)
					(View.CustomizationForm as BandedCustomizationForm).ShowBands();
				DragMasterStart(drag, DownPointHitInfo.HitPoint, ((BandedGridPainter)View.Painter).GetBandDragBitmap(ViewInfo, drag as GridBand, size, false, (drag as GridBand).RowCount, false));
				return;
			}
			if(drag is GridColumn) {
				if(View.CustomizationForm != null)
					(View.CustomizationForm as BandedCustomizationForm).ShowColumns();
			}
			base.DoStartDragObject(drag, size);
		}
		protected override GridViewMenu CreateMenuEx(GridViewMenu menu) {
			if(menu == null) {
				if(View.OptionsMenu.EnableColumnMenu) {
					if(DownPointHitInfo.InBandPanel && DownPointHitInfo.Band != null) {
						menu = new GridViewBandMenu(View);
						menu.Init(DownPointHitInfo.Band);
					}
				}
			}
			return menu;
		}
	}
}

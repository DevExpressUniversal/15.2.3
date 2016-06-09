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
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.XtraGrid.Views.BandedGrid.Drawing {
	public class BandedGridViewDrawArgs : GridViewDrawArgs {
		public BandedGridViewDrawArgs(GraphicsCache graphicsCache, BandedGridViewInfo viewInfo, Rectangle bounds) : base(graphicsCache, viewInfo, bounds) {
		}
		public virtual new BandedGridViewInfo ViewInfo { get { return base.ViewInfo as BandedGridViewInfo; } }
	}
	public class BandedGridPainter : GridPainter {
		public BandedGridPainter(BandedGridView gridView) : base(gridView) {
		}
		public new BandedGridView View { get { return base.View as BandedGridView; } }
		public new BandedGridElementsPainter ElementsPainter { get { return base.ElementsPainter as BandedGridElementsPainter; } }
		protected override void DrawColumnPanel(GridViewDrawArgs e) {
			DrawBandPanel(e as BandedGridViewDrawArgs);
			base.DrawColumnPanel(e);
		}
		protected virtual void DrawBandIndicator(BandedGridViewDrawArgs e) {
			if(e.ViewInfo.BandsInfo.Count > 0) {
				GridBandInfoArgs bi = e.ViewInfo.BandsInfo[0];
				if(bi.Type == GridColumnInfoType.Indicator) {
					DrawIndicator(e, bi.Bounds, GridControl.InvalidRowHandle, -1, e.ViewInfo.PaintAppearance.BandPanel, IndicatorKind.Band, bi);
				}
			}
		}
		protected override void DrawColumnPanelBackground(GridViewDrawArgs e) {
			BandedGridViewInfo viewInfo = e.ViewInfo as BandedGridViewInfo;
			Rectangle r = e.ViewInfo.ViewRects.ColumnPanel;
			if(r.Right > e.ViewInfo.ViewRects.DataRectRight) 
				r.Width -= (r.Right - e.ViewInfo.ViewRects.DataRectRight);
			viewInfo.PaintAppearance.HeaderPanelBackground.DrawBackground(e.Cache, r);
		}
		protected virtual void DrawBandPanel(BandedGridViewDrawArgs e) {
			Graphics g = e.Graphics;
			if(!View.OptionsView.ShowBands || !IsNeedDrawRect(e, e.ViewInfo.ViewRects.BandPanelActual)) return;
			BandedGridViewInfo viewInfo = e.ViewInfo as BandedGridViewInfo;
			Rectangle r = e.ViewInfo.ViewRects.BandPanelActual;
			viewInfo.PaintAppearance.BandPanelBackground.DrawBackground(e.Cache, r);
			DrawBandIndicator(e);
			bool isRightToLeft = e.ViewInfo.IsRightToLeft;
			GraphicsClipState clipState = null;
			if(e.ViewInfo.ViewRects.IndicatorWidth > 0) {
				Rectangle clipR = e.ViewInfo.ViewRects.Scroll;
				if(!isRightToLeft) {
					clipR.X += e.ViewInfo.ViewRects.IndicatorWidth;
				}
				clipR.Width -= e.ViewInfo.ViewRects.IndicatorWidth;
				clipState = e.Cache.ClipInfo.SaveAndSetClip(clipR, true, true);
			}
			foreach(GridBandInfoArgs bi in e.ViewInfo.BandsInfo) {
				Rectangle biBounds = bi.Bounds;
				biBounds.Height = e.ViewInfo.ViewRects.BandPanel.Height;
				GridClipInfo cli = PrepareClipInfo(e, bi, biBounds);
				if(cli.ShouldPaint)
					DrawBand(e, bi);
				Rectangle fr = Rectangle.Empty;
				if(bi.Band != null && bi.Band == e.ViewInfo.FixedLeftBand) {
					fr = bi.Bounds;
					fr.X = bi.Bounds.Right;
					if(isRightToLeft) fr.X = bi.Bounds.X - e.ViewInfo.FixedLineWidth;
					fr.Width = e.ViewInfo.FixedLineWidth;
					fr.Height = e.ViewInfo.ViewRects.BandPanel.Height;
				}
				if(bi.Band != null && bi.Band == e.ViewInfo.FixedRightBand) {
					fr = bi.Bounds;
					fr.X -= e.ViewInfo.FixedLineWidth;
					if(isRightToLeft) fr.X = bi.Bounds.Right;
					fr.Width = e.ViewInfo.FixedLineWidth;
					fr.Height = e.ViewInfo.ViewRects.BandPanel.Height;
				}
				if(!fr.IsEmpty) {
					e.Cache.Paint.FillRectangle(e.Graphics, e.ViewInfo.PaintAppearance.FixedLine.GetBackBrush(e.Cache), fr);
				}
				cli.Restore(e);
			}
			e.Cache.ClipInfo.RestoreClipRelease(clipState);
		}
		protected virtual void DrawBand(BandedGridViewDrawArgs e, GridBandInfoArgs bi) {
			if(bi.Type == GridColumnInfoType.Indicator) return;
			e.ViewInfo.CalcBandInfoState(bi);
			DrawBandCore(e, bi);
			if(bi.HasChildren) {
				foreach(GridBandInfoArgs chBi in bi.Children) {
					DrawBand(e, chBi);
				}
			}
		}
		protected virtual void DrawBandCore(BandedGridViewDrawArgs e, GridBandInfoArgs bi) {
			Rectangle r = bi.Bounds;
			bi.Cache = e.Cache;
			try {
				BandHeaderCustomDrawEventArgs cs = new BandHeaderCustomDrawEventArgs(e.Cache, ElementsPainter.Band, bi);
				cs.SetDefaultDraw(() => {
					ElementsPainter.Band.DrawObject(bi);
				});
				View.RaiseCustomDrawBandHeader(cs);
				cs.DefaultDraw();
			}
			finally {
				bi.Cache = null;
			}
		}
		public virtual void DrawBandDrag(GraphicsCache cache, BandedGridViewInfo viewInfo, GridBand band, Rectangle bounds, bool pressed, bool customization, HeaderPositionKind kind = HeaderPositionKind.Special) {
			GridBandInfoArgs bi = new GridBandInfoArgs(band);
			bi.HeaderPosition = kind;
			bi.CustomizationForm = customization;
			bi.Bounds = bounds;
			bi.AllowEffects = false;
			viewInfo.CalcBandInfoState(bi);
			bi.SetAppearance(viewInfo.PaintAppearance.BandPanel);
			ElementsPainter.Band.CalcObjectBounds(bi);
			bi.State = pressed ? ObjectState.Pressed : ObjectState.Normal;
			BandHeaderCustomDrawEventArgs cs = new BandHeaderCustomDrawEventArgs(null, ElementsPainter.Band, bi);
			DrawObject(cache, ElementsPainter.Band, bi, new CustomEventCall(View.RaiseCustomDrawBandHeader), cs);
		}
		public virtual Bitmap GetBandDragBitmap(BandedGridViewInfo viewInfo, GridBand band, Size bandSize, bool pressed, int rowCount, bool customization) {
			if(bandSize.Width == 0) {
				bandSize.Width = band.VisibleWidth;
				if(bandSize.Width > 200) bandSize.Width = 200;
				if(bandSize.Width < 100) bandSize.Width = 100;
			}
			Rectangle bounds = new Rectangle(0, 0, bandSize.Width, viewInfo.BandRowHeight * rowCount);
			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
			Graphics g = Graphics.FromImage(bmp);
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(g, Rectangle.Empty), new DevExpress.Utils.Paint.XPaint());
			DrawBandDrag(cache, viewInfo, band, bounds, pressed, customization);
			g.Dispose();
			return bmp;
		}
		protected override GridClipInfo PrepareClipInfo(GridViewDrawArgs e, GridColumnInfoArgs ci, Rectangle bounds) {
			BandedGridClipInfo cli = new BandedGridClipInfo(e.Cache);
			cli.Prepare(e, ci, bounds);
			return cli;
		}
		protected virtual BandedGridClipInfo PrepareClipInfo(BandedGridViewDrawArgs e, GridBandInfoArgs bi, Rectangle bounds) {
			BandedGridClipInfo cli = new BandedGridClipInfo(e.Cache);
			cli.PrepareBand(e, bi, bounds);
			return cli;
		}
		public class BandedGridClipInfo : GridClipInfo {
			public BandedGridClipInfo(GraphicsCache cache) : base(cache) {
			}
			public virtual void PrepareBand(BandedGridViewDrawArgs e, GridBandInfoArgs bi, Rectangle bounds) {
				if(e.ViewInfo.HasFixedColumns && bi.Fixed != FixedStyle.Left) {
					if(bi.Type == GridColumnInfoType.Indicator || bi.Type == GridColumnInfoType.BehindColumn) return;
					Rectangle clipR = CheckRectangle(e.ViewInfo.UpdateFixedRange(bounds, bi));
					if(clipR.Width == 0) {
						this.ShouldPaint = false;
					}
					else {
						if(clipR != bounds) {
							ClipState = Cache.ClipInfo.SaveAndSetClip(clipR);
							this.Clipped = true;
						}
					}
				}
			}
		}
		protected override void CalcSizingPoints(ref Point start, ref Point end) {
			BandedGridViewInfo viewInfo = View.ViewInfo as BandedGridViewInfo;
			int startX, startY, endX, endY;
			startY = endX = endY = startX = -10000;
			if(View.State == BandedGridState.BandSizing) {
				GridBandInfoArgs info = viewInfo.BandsInfo.FindBand(ReSizingObject as GridBand);
				startX = fCurrentSizerPos;
				startY = info == null ? viewInfo.ViewRects.BandPanel.Y : info.Bounds.Y;
				endX = fCurrentSizerPos;
				endY = viewInfo.ViewRects.Rows.Bottom;
				if(startX > viewInfo.ViewRects.BandPanel.Right) return;
				if(startX < viewInfo.ViewRects.BandPanel.Left + viewInfo.ViewRects.IndicatorWidth)
					return;
			}
			if(startX == -10000) {
				base.CalcSizingPoints(ref start, ref end);
				return;
			}
			start = new Point(startX, startY);
			end = new Point(endX, endY);
		}
	}
	public class BandedGridElementsPainter : GridElementsPainter {
		HeaderObjectPainter band;
		public BandedGridElementsPainter(BaseView view) : base(view) {
			this.band = CreateBandPainter();
		}
		public HeaderObjectPainter Band { get { return band; } }
		protected virtual HeaderObjectPainter CreateBandPainter() { return LookAndFeelPainterHelper.GetPainter(ElementsStyle).Header; }
	}
	public class BandedGridStyle3DElementsPainter : BandedGridElementsPainter {
		public BandedGridStyle3DElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Style3D; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Style3DIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.Style3D)); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new Border3DSunkenPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new Style3DButtonObjectPainter()); }
	}
	public class BandedGridUltraFlatElementsPainter : BandedGridElementsPainter {
		public BandedGridUltraFlatElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.UltraFlat; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new UltraFlatIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(BorderStyles.UltraFlat)); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new SimpleBorderPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new GridUltraFlatButtonPainter()); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new GridSpecialTopRowIndentUltraFlatPainter(); }
	}
	public class BandedGridWindowsXPElementsPainter : BandedGridElementsPainter {
		public BandedGridWindowsXPElementsPainter(BaseView view) : base(view) {
		}
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.WindowsXP; } }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new XPIndicatorObjectPainter(); }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new GridFilterButtonPainter(new WindowsXPEditorButtonPainter()); } 
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new GridGroupFooterCellPainter(new TextFlatBorderPainter()); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new GridWindowsXPButtonPainter()); }
	}
	public class BandedGridMixedXPElementsPainter : BandedGridElementsPainter {
		public BandedGridMixedXPElementsPainter(BaseView view) : base(view) { }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new GridFilterPanelPainter(new WindowsXPEditorButtonPainter(), new WindowsXPCheckObjectPainter()); }
	}
	public class BandedGridOffice2003ElementsPainter : BandedGridElementsPainter {
		public BandedGridOffice2003ElementsPainter(BaseView view) : base(view) { }
		protected override ActiveLookAndFeelStyle ElementsStyle { get { return ActiveLookAndFeelStyle.Office2003; } }
		protected override ObjectPainter CreateHeaderFilterButtonPainter() { return new Office2003GridFilterButtonPainter(); } 
		protected override ObjectPainter CreateHeaderSmartFilterButtonPainter() { return new GridSmartOffice2003FilterButtonPainter();	}
		protected override FooterCellPainter CreateGroupFooterCellPainter() { return new Office2003FooterCellPainter(); }
		protected override FooterPanelPainter CreateGroupFooterPanelPainter() { return new GridGroupFooterPanelPainter(new Office2003FooterPanelObjectPainter()); }
		protected override GridFilterPanelPainter CreateFilterPanelPainter() { return new Office2003GridFilterPanelPainter(); }
		protected override GridGroupPanelPainter CreateGroupPanelPainter() { return new Office2003GridGroupPanelPainter(); }
		protected override GridGroupRowPainter CreateGroupRowPainter() { return new Office2003GridGroupRowPainter(this); }
		protected override IndicatorObjectPainter CreateIndicatorPainter() { return new Office2003IndicatorObjectPainter(); }
		protected override ObjectPainter CreateSpecialTopRowPainter() { return new GridTopNewItemRowIndentOffice2003Painter(); }
	}
}

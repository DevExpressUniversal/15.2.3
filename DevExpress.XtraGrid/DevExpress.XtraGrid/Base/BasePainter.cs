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
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
namespace DevExpress.XtraGrid.Views.Base {
	public class ViewDrawArgs : GraphicsInfoArgs {
		BaseViewInfo viewInfo;
		public ViewDrawArgs(GraphicsCache graphicsCache, BaseViewInfo viewInfo, Rectangle bounds) : base(graphicsCache, bounds) {
			this.viewInfo = viewInfo;
		}
		public virtual BaseViewInfo ViewInfo { get { return viewInfo; } }
	}
	public abstract class BaseViewPainter {
		protected DevExpress.Utils.Paint.XPaint fPaint;
		protected DevExpress.Utils.Paint.Clipping fClip;
		protected const int fNoSizerCoord = -10000;
		protected int fStartSizerPos = fNoSizerCoord, fCurrentSizerPos = fNoSizerCoord;
		bool sizerLineVisible = false;
		BaseView view;
		public BaseViewPainter(BaseView view) {
			this.view = view;
			this.fClip = new DevExpress.Utils.Paint.Clipping();
			this.fPaint = DevExpress.Utils.Paint.XPaint.CreateDefaultPainter();
		}
		public BaseView View { get { return view; } }
		public virtual DevExpress.Utils.Paint.XPaint Paint { get { return fPaint; } }
		public virtual void Draw(ViewDrawArgs e) {
			if(View.IsDesignMode) DrawDesignMode(e);
		}
		public virtual void StyleFillRectangle(GraphicsCache cache, AppearanceObject appearance, Rectangle bounds) {
			appearance.DrawBackground(cache, bounds);
		}
		public virtual void StyleFillRectangle(GraphicsCache cache, AppearanceObject appearance, Brush brush, Rectangle bounds) {
			cache.Paint.FillRectangle(cache.Graphics, brush, bounds);
		}
		public virtual void StyleFillRectangle(GraphicsCache cache, AppearanceObject appearance, Color brushColor, Rectangle bounds) {
			cache.Paint.FillRectangle(cache.Graphics, cache.GetSolidBrush(brushColor), bounds);
		}
		public virtual void StyleDrawRectangle(GraphicsCache cache, AppearanceObject appearance, Rectangle bounds) {
			cache.Paint.DrawRectangle(cache.Graphics,  appearance.GetBackPen(cache), bounds);
		}
		public virtual void StyleDrawString(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle bounds) {
			appearance.DrawString(cache, text, bounds);
		}
		protected virtual void DrawDesignMode(ViewDrawArgs e) {
			if(!View.GridControl.LevelTree.HasChildren) return;
			string viewType = "";
			string name = View.Name;
			if(View == View.GridControl.MainView) {
				viewType = Properties.Resources.MainViewCaption;
			} else {
				viewType = View.LevelDefaultName;
			}
			if(name == "") name = "view";
			string text = string.Format("{0}: {1}", viewType, name);
			Font font = new Font(AppearanceObject.DefaultFont.FontFamily, 20);
			Brush brush = new SolidBrush(Color.FromArgb(100, SystemColors.ControlText));
			Size size = e.Graphics.MeasureString(text, font, 0).ToSize();
			size.Width += 1;
			size.Height += 1;
			Rectangle r = View.ViewRect;
			r.X = r.X + (r.Width - size.Width) / 2;
			r.Y = r.Y + (r.Height - size.Height) / 2;
			if(r.Width < size.Width) r.X = View.ViewRect.X + 3;
			r.Size = size;
			Control selector = null;
			foreach(Control ctrl in View.GridControl.Controls) {
				if(ctrl.Name == "LevelSelector") {
					selector = ctrl;
					break;
				}
			}
			if(selector != null) {
				if(r.Bottom > selector.Top)
					r.Y = selector.Top - r.Height - 10;
				if(r.Y < View.ViewRect.Y + 3) r.Y = View.ViewRect.Y + 3;
			}
			e.Graphics.DrawString(text, font, brush, r);
			brush.Dispose();
			font.Dispose();
		}
		public virtual int CalcMaxTextHeight(Graphics g, AppearanceObject[] appearances) {
			int result = 0;
			for(int i = 0; i < appearances.Length; i++) {
				int h = Convert.ToInt32(appearances[i].CalcTextSize(g, "Wg", 0).Height);
				result = Math.Max(result, h) ;
			}
			return result;
		}
		protected bool IsNeedDrawRect(ViewDrawArgs e, Rectangle r) {
			return e.Cache.IsNeedDrawRectEx(r);
		}
		protected bool PtInRect(Rectangle r, Point pt) {
			return (!r.IsEmpty && r.Contains(pt));
		}
		protected virtual void DrawTabControl(ViewDrawArgs e) {
			if(!e.ViewInfo.ShowTabControl) return;
			Rectangle maxBounds = e.Cache.ClipInfo.MaximumBounds;
			e.Cache.ClipInfo.MaximumBounds = e.ViewInfo.Bounds;
			e.ViewInfo.TabControl.ViewInfo.HeaderInfo.FillTransparentBackground = View.IsZoomedView;
			e.ViewInfo.TabControl.Draw(e.PaintArgs, e.Cache);
			e.Cache.ClipInfo.MaximumBounds = maxBounds;
		}
		protected virtual void DrawBorder(ViewDrawArgs e, Rectangle r) {
			BorderObjectInfoArgs info = e.ViewInfo.CreateBorderInfo(r);
			info.Cache = e.Cache;
			e.ViewInfo.BorderPainter.DrawObject(info);
			DrawTabControl(e);
		}
		protected void DrawIndents(ViewDrawArgs e, IndentInfoCollection indents, ObjectInfoArgs sourceObject) {
			for(int n = 0; n < indents.Count; n++) {
				DrawIndentCore(e, indents[n], sourceObject);
			}
		}
		protected virtual void DrawLines(ViewDrawArgs e, IndentInfoCollection indents) {
			foreach(IndentInfo indent in indents) { DrawLine(e, indent); }
		}
		protected virtual bool IsAllowDrawIndent(IndentInfo indent) { return true; }
		protected virtual void DrawLine(ViewDrawArgs e, IndentInfo indent) {
			if(!IsAllowDrawIndent(indent)) return;
			if(indent.Appearance != null) {
				e.Paint.FillRectangle(e.Graphics, indent.Appearance.GetBackBrush(e.Cache), indent.Bounds);
			} else {
				e.Paint.FillRectangle(e.Graphics, SystemBrushes.Window, indent.Bounds);
			}
		}
		protected virtual void DrawIndentCore(ViewDrawArgs e, IndentInfo indent, ObjectInfoArgs sourceObject) {
			if(indent.Appearance != null) {
				indent.Appearance.DrawBackground(e.Cache, indent.Bounds);
			} else {
				e.Cache.Paint.FillRectangle(e.Graphics, SystemBrushes.Window, indent.Bounds);
			}
		}
		public bool IsSizerLineVisible { get { return sizerLineVisible; } }
		public virtual void HideSizerLine() {
			if(this.sizerLineVisible)
				DrawSizerLine();
			this.sizerLineVisible = false;
		}
		public virtual void ShowSizerLine() {
			if(!sizerLineVisible)
				DrawSizerLine();
			this.sizerLineVisible = true;
		}
		public virtual void StopSizing() {
			HideSizerLine();
			fStartSizerPos = fCurrentSizerPos = fNoSizerCoord;
			View.ResetCursor();
		}
		public int StartSizerPos { get { return fStartSizerPos; } set { fStartSizerPos = value; } }
		public int CurrentSizerPos { get { return fCurrentSizerPos; } set { fCurrentSizerPos = value; } }
		public virtual void DrawSizerLine() { }
		protected virtual void DrawObject(GraphicsCache cache, ObjectPainter painter, ObjectInfoArgs args, CustomEventCall eventCall, CustomDrawEventArgs eventArgs) {
			args.Cache = cache;
			if(eventCall != null) {
				eventArgs.SetupCache(args.Cache);
				eventArgs.SetDefaultDraw(() => {
					painter.DrawObject(args);
				});
				eventCall(eventArgs);
				eventArgs.DefaultDraw();
			} 
			if(eventArgs == null || !eventArgs.Handled) 
				painter.DrawObject(args);
		}
		public delegate void CustomEventCall(EventArgs e);
	}
	public abstract class ColumnViewPainter : BaseViewPainter {
		public ColumnViewPainter(BaseView view) : base(view) { }
		public new ColumnView View { get { return base.View as ColumnView; } }
		protected virtual void DrawViewCaption(ViewDrawArgs e) {
			ColumnViewInfo viewInfo = e.ViewInfo as ColumnViewInfo;
			if(viewInfo.ViewCaptionBounds.IsEmpty || !IsNeedDrawRect(e, viewInfo.ViewCaptionBounds)) return;
			StyleObjectInfoArgs panel = new StyleObjectInfoArgs(e.Cache, viewInfo.ViewCaptionBounds, viewInfo.PaintAppearance.ViewCaption.Clone() as AppearanceObject, ObjectState.Normal);
			viewInfo.ViewCaptionPainter.DrawObject(panel);
		}
		protected virtual void DrawFilterPanel(ViewDrawArgs e) {
			ColumnViewInfo viewInfo = e.ViewInfo as ColumnViewInfo;
			if(viewInfo.FilterPanel.Bounds.IsEmpty) return;
			if(!IsNeedDrawRect(e, viewInfo.FilterPanel.Bounds)) return;
			viewInfo.FilterPanel.FilterActive = View.ActiveFilterEnabled;
			viewInfo.FilterPanel.TextState = CalcFilterTextState(e);
			viewInfo.FilterPanel.CustomizeButtonInfo.State = CalcFilterCustomizeButtonState(e);
			viewInfo.FilterPanel.MRUButtonInfo.State = CalcFilterMRUButtonState(e);
			viewInfo.FilterPanel.CloseButtonInfo.State = CalcFilterCloseButtonState(e);
			viewInfo.FilterPanel.ActiveButtonInfo.State = CalcFilterActiveButtonState(e);
			viewInfo.FilterPanel.DisplayText = View.FilterPanelText;
			CustomDrawObjectEventArgs cs = new CustomDrawObjectEventArgs(e.Cache, viewInfo.FilterPanelPainter, viewInfo.FilterPanel, viewInfo.FilterPanel.Appearance);
			cs.SetDefaultDraw(() => {
				ObjectPainter.DrawObject(e.Cache, viewInfo.FilterPanelPainter, viewInfo.FilterPanel);
			});
			View.RaiseCustomDrawFilterPanel(cs);
			cs.DefaultDraw();
		}
		protected virtual ObjectState CalcFilterTextState(ViewDrawArgs e) { return ObjectState.Normal; }
		protected virtual ObjectState CalcFilterMRUButtonState(ViewDrawArgs e) { return ObjectState.Normal; }
		protected virtual ObjectState CalcFilterCustomizeButtonState(ViewDrawArgs e) { return ObjectState.Normal; }
		protected virtual ObjectState CalcFilterCloseButtonState(ViewDrawArgs e) { return ObjectState.Normal; }
		protected virtual ObjectState CalcFilterActiveButtonState(ViewDrawArgs e) { return ObjectState.Normal; }
	}
}

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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Export {
	public abstract class BaseExportLink {
		protected BaseView fView;
		protected IExportProvider fProvider;
		protected bool fExportAll = true;
		bool exportCellsAsDisplayText = true;
		BaseAppearanceCollection exportAppearance = null;
		public BaseExportLink(BaseView view, IExportProvider provider) {	
			this.fView = view;
			this.fProvider = provider;
			fProvider.ProviderProgress += new ProviderProgressEventHandler(Provider_Progress);
		}
		protected abstract BaseAppearanceCollection CreateAppearanceCollectionInstance();
		protected virtual BaseAppearanceCollection CreateAppearance() {
			BaseAppearanceCollection res = CreateAppearanceCollectionInstance();
			res.Assign(View.ViewInfo.PaintAppearance);
			UpdateAppearanceScheme(res);
			return res;
		}
		protected virtual void UpdateAppearanceScheme(BaseAppearanceCollection coll) {
		}
		public BaseAppearanceCollection ExportAppearance { 
			get { 
				if(exportAppearance == null) exportAppearance = CreateAppearance();
				return exportAppearance; 
			} 
		}
		void Provider_Progress(object sender, ProviderProgressEventArgs e) {
			OnProgress(e.Position, ExportPhase.Provider);
		}
		protected void OnProgress(int position, ExportPhase phase) {
			if(Progress != null)
				Progress(this, new ProgressEventArgs(position, phase));
		}
		protected void CreateBorders() {
			int cacheWidth = GetCacheWidth();
			int cacheHeight = GetCacheHeight();
			if(cacheWidth == 0 || cacheHeight == 0)
				return;
			for(int i = 0; i < cacheWidth; i++) {
				ExportCacheCellStyle style = Provider.GetCellStyle(i, cacheHeight - 1);
				if(style.BottomBorder.Width > 0)
					continue;
				style.BottomBorder.Width = 1;
				Provider.SetCellStyle(i, cacheHeight - 1, style);
			}
			for(int i = 0; i < cacheHeight; i++) {
				ExportCacheCellStyle style = Provider.GetCellStyle(cacheWidth - 1, i);
				if(style.RightBorder.Width > 0)
					continue;
				style.RightBorder.Width = 1;
				Provider.SetCellStyle(cacheWidth - 1, i, style);
			}
		}
		protected StringAlignment HorzAlignmentToStringAlignment(HorzAlignment alignment) {
			StringAlignment result = StringAlignment.Near;	
			switch(alignment) {
				case HorzAlignment.Near:
					result = StringAlignment.Near;
					break;
				case HorzAlignment.Center:
					result = StringAlignment.Center;
					break;
				case HorzAlignment.Far:
					result = StringAlignment.Far;
					break;
			};
			return result;
		}
		Color CheckColor(Color color, Color defaultColor) {
			if(color == Color.Empty) color = defaultColor;
			return color;
		}
		protected ExportCacheCellStyle GridStyleToExportStyle(AppearanceObject style, AppearanceObject vertBorderStyle, AppearanceObject horzBorderStyle) {			
			ExportCacheCellStyle result = GetDefaultStyle();
			if(style != null) {
				result.BkColor = CheckColor(style.BackColor, Color.White);
				result.BrushStyle_ = style.Options.UseBackColor ? BrushStyle.Solid : BrushStyle.Clear;
				result.TextColor = CheckColor(style.ForeColor, Color.Black);
				result.TextFont = style.Font.Clone() as Font;
				result.TextAlignment = HorzAlignmentToStringAlignment(style.HAlignment);
				if(vertBorderStyle != null) {
					result.LeftBorder.Color_ = vertBorderStyle.BackColor;
					result.RightBorder.Color_ = vertBorderStyle.BackColor;
				}
				if(horzBorderStyle != null) {
					result.TopBorder.Color_ = horzBorderStyle.BackColor;
					result.BottomBorder.Color_ = horzBorderStyle.BackColor;
				}
				if(result.BkColor.ToArgb() == result.LeftBorder.Color_.ToArgb())
					result.LeftBorder.Color_ = ControlPaint.Light(result.LeftBorder.Color_);
				if(result.BkColor.ToArgb() == result.RightBorder.Color_.ToArgb())
					result.RightBorder.Color_ = ControlPaint.Light(result.RightBorder.Color_);
				if(result.BkColor.ToArgb() == result.TopBorder.Color_.ToArgb())
					result.TopBorder.Color_ = ControlPaint.Light(result.TopBorder.Color_);
				if(result.BkColor.ToArgb() == result.BottomBorder.Color_.ToArgb())
					result.BottomBorder.Color_ = ControlPaint.Light(result.BottomBorder.Color_);
			}
			return result;
		}
		protected virtual ExportCacheCellStyle GetDefaultStyle() {
			ExportCacheCellStyle result = Provider.GetDefaultStyle();
			return result;
		}
		protected abstract bool TestView();
		protected abstract void DoExport();
		protected abstract int GetCacheWidth();
		protected abstract int GetCacheHeight();
		public virtual void Copy(BaseExportLink link) {
			exportCellsAsDisplayText = link.ExportCellsAsDisplayText;
		}
		public void ExportTo(bool doCommit) {
			if(Provider != null && View != null) {
				if(TestView())
					DoExport();
				CreateBorders();
				if(doCommit) 
					Provider.Commit();
			}			
		}
		public bool ExportAll {
			get {
				return fExportAll;
			}
			set {
				fExportAll = value;
			}
		}
		public bool ExportCellsAsDisplayText {
			get { return exportCellsAsDisplayText; }
			set { exportCellsAsDisplayText = value; }
		}
		public BaseView View {
			get {
				return fView;
			}
		}
		public IExportProvider Provider {
			get {
				return fProvider;
			}
		}
		public event ProgressEventHandler Progress;
	}
	public enum ExportPhase {
		Link,
		Provider
	}
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);
	public class ProgressEventArgs : EventArgs {
		int position;
		ExportPhase phase;
		public ProgressEventArgs(int position, ExportPhase phase) : base() {
			this.position = position;
			this.phase = phase;
		}
		public int Position { get { return position; } }
		public ExportPhase Phase { get { return phase; } }
	}
}

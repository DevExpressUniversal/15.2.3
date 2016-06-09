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

using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RecentControlPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			RecentControlViewInfo viewInfo = info.ViewInfo as RecentControlViewInfo;
			if(viewInfo.ShowTitle)
				DrawTitle(info);
			if(viewInfo.ShowSplitter)
				DrawSplitter(info);
			DrawPanels(info);
		}
		protected void DrawSplitter(ControlGraphicsInfoArgs info) {
			RecentControlViewInfo viewInfo = info.ViewInfo as RecentControlViewInfo;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, viewInfo.GetSplitterInfo());
		}
		protected virtual void DrawTitle(ControlGraphicsInfoArgs info) {
			RecentControlViewInfo viewInfo = info.ViewInfo as RecentControlViewInfo;
			viewInfo.PaintAppearanceTitleCaption.DrawString(info.Cache, viewInfo.RecentControl.Title, viewInfo.TitleCaptionBounds);
		}
		protected void DrawPanels(ControlGraphicsInfoArgs info) {
			RecentControlViewInfo viewInfo = info.ViewInfo as RecentControlViewInfo;
			viewInfo.RecentControl.MainPanel.Painter.Draw(info.Cache, viewInfo.RecentControl.MainPanel.ViewInfo);
			viewInfo.RecentControl.ContentPanel.Painter.Draw(info.Cache, viewInfo.RecentControl.ContentPanel.ViewInfo);
		}
	}
	public class RecentPanelPainterBase {
		public virtual void Draw(GraphicsCache cache, RecentPanelViewInfoBase panelInfoBase) {
			DrawBackground(cache, panelInfoBase);
			if(!cache.Graphics.ClipBounds.IntersectsWith(panelInfoBase.PanelContentRect)) return;
			GraphicsClipState state = cache.ClipInfo.SaveAndSetClip(panelInfoBase.PanelContentRect);
			try {
				if(panelInfoBase.ShowCaption)
					DrawGlyph(cache, panelInfoBase);
				DrawCaption(cache, panelInfoBase);
				DrawItems(cache, panelInfoBase);
			}
			finally {
				cache.ClipInfo.RestoreClip(state);
				state.Dispose();
			}
			DrawSelection(cache, panelInfoBase);
		}
		void DrawSelection(GraphicsCache cache, RecentPanelViewInfoBase panelInfoBase) {
			if(panelInfoBase.Panel.RecentControl.GetViewInfo().DesignTimeManager.IsComponentSelected(panelInfoBase.Panel))
				panelInfoBase.Panel.RecentControl.GetViewInfo().DesignTimeManager.DrawSelection(cache, panelInfoBase.Bounds, Color.Magenta);
		}
		void DrawGlyph(GraphicsCache cache, RecentPanelViewInfoBase panelInfoBase) {
			if(panelInfoBase.Panel.Glyph == null) return;
			cache.Graphics.DrawImage(panelInfoBase.Panel.Glyph, panelInfoBase.GlyphBounds.Location);
		}
		void DrawItems(GraphicsCache cache, RecentPanelViewInfoBase panelInfoBase) {
			foreach(RecentItemBase item in panelInfoBase.Panel.Items) {
				if(!item.Visible) continue;
				item.Painter.Draw(cache, item.ViewInfo);
			}
		}
		void DrawCaption(GraphicsCache cache, RecentPanelViewInfoBase panelInfoBase) {
			panelInfoBase.PaintAppearance.DrawString(cache, panelInfoBase.Panel.Caption, panelInfoBase.CaptionBounds);
		}
		void DrawBackground(GraphicsCache cache, RecentPanelViewInfoBase panelInfoBase) {
		}
	}
	public class RecentStackPanelPainter : RecentPanelPainterBase {
	}
}

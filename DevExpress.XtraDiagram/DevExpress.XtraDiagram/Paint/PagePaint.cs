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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Paint {
	#region PaintArgs
	public class DiagramPageObjectInfoArgs : ObjectInfoArgs {
		Rectangle pageRect;
		bool drawGrid;
		bool allowCache;
		UserLookAndFeel lookAndFeel;
		DiagramAppearanceObject paintAppearance;
		DiagramGridObjectInfoArgs gridDrawArgs;
		DiagramGridPainter gridPainter;
		DiagramPaintCache paintCache;
		public DiagramPageObjectInfoArgs() {
			this.paintAppearance = null;
			this.lookAndFeel = null;
			this.paintCache = null;
			this.gridDrawArgs = null;
			this.gridPainter = null;
			this.allowCache = false;
		}
		public virtual void Initialize(DiagramControlViewInfo viewInfo) {
			this.pageRect = viewInfo.PageDisplayRect;
			this.drawGrid = viewInfo.AllowDrawGrid;
			this.lookAndFeel = viewInfo.LookAndFeel;
			this.paintCache = viewInfo.PaintCache;
			this.gridDrawArgs = viewInfo.GridDrawArgs;
			this.gridPainter = viewInfo.GridPainter;
			this.gridDrawArgs.Initialize(viewInfo);
			this.allowCache = viewInfo.AllowPageCache();
		}
		public virtual void Clear() {
			this.paintAppearance = null;
			this.lookAndFeel = null;
			this.paintCache = null;
			this.gridDrawArgs.Cache = null;
			this.gridDrawArgs.Clear();
			this.gridDrawArgs = null;
			this.gridPainter = null;
		}
		public DiagramAppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public Rectangle PageRect { get { return pageRect; } }
		public bool AllowCache { get { return allowCache; } }
		public bool DrawGrid { get { return drawGrid; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public DiagramPaintCache PaintCache { get { return paintCache; } }
		public DiagramGridObjectInfoArgs GridDrawArgs { get { return gridDrawArgs; } }
		public DiagramGridPainter GridPainter { get { return gridPainter; } }
	}
	#endregion
	#region Painter
	public class DiagramPagePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramPageObjectInfoArgs args = (DiagramPageObjectInfoArgs)e;
			base.DrawObject(e);
			DrawPage(args);
		}
		protected virtual void DrawPage(DiagramPageObjectInfoArgs args) {
			Rectangle rect = GetPageRect(args);
			if(args.AllowCache) {
				args.Graphics.DrawImageUnscaled(args.PaintCache.GetPageImage(args, rect, DrawPageImage), rect.Location);
			}
			else {
				DrawPageImage(args, rect);
			}
		}
		protected virtual void DrawPageImage(DiagramPageObjectInfoArgs drawArgs, Rectangle bounds) {
			SkinElementInfo element = GetPageElementInfo(drawArgs);
			element.Bounds = bounds;
			ObjectPainter.DrawObject(drawArgs.Cache, SkinElementPainter.Default, element);
			if(drawArgs.DrawGrid) {
				ObjectPainter.DrawObject(drawArgs.Cache, drawArgs.GridPainter, drawArgs.GridDrawArgs);
			}
		}
		protected virtual SkinElementInfo GetPageElementInfo(DiagramPageObjectInfoArgs args) {
			SkinElementInfo pageElementInfo = new SkinElementInfo(GetPageElement(args.LookAndFeel), GetPageRect(args));
			pageElementInfo.ImageIndex = 1;
			return pageElementInfo;
		}
		public virtual SkinElement GetPageElement(UserLookAndFeel lookAndFeel) {
			return PrintingSkins.GetSkin(lookAndFeel)[PrintingSkins.SkinBorderPage];
		}
		protected virtual Rectangle GetPageRect(DiagramPageObjectInfoArgs args) {
			Rectangle pageRect = args.PageRect;
			pageRect.Width++;
			pageRect.Height++;
			return pageRect;
		}
	}
	#endregion
}

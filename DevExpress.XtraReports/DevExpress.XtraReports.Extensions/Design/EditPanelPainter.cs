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
using System.Text;
using DevExpress.Utils.Drawing;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	public class EditPanelPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			DrawBorders(e, SystemColors.ControlDarkDark);
		}
		static void DrawBorders(ObjectInfoArgs e, Color color) {
			DrawBordersCore(e.Graphics, e.Cache.GetSolidBrush(color), ((EditPanelViewInfo)e).BorderBounds);
		}
		static void DrawBordersCore(Graphics gr, Brush brush, Rectangle rect) {
			gr.FillRectangle(brush, new Rectangle(rect.X, rect.Y, rect.Width, 1));
			gr.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1));
			gr.FillRectangle(brush, new Rectangle(rect.X, rect.Y, 1, rect.Height));
			gr.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height));
		}
	}
	public class EditPanelPainterSkin : ObjectPainter {
		UserLookAndFeel lookAndFeel;
		public EditPanelPainterSkin(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			SkinHelperBase helper = new SkinHelperBase(SkinProductId.Reports);
			helper.DrawSkinElement(lookAndFeel, e.Cache, "PageBorder", ((EditPanelViewInfo)e).BorderBounds);
		}
	}
	public class EditPanelViewInfo : ObjectInfoArgs {
		Margins borders;
		public Rectangle BorderBounds {
			get {
				return RectHelper.InflateRect(Bounds, borders.Left, borders.Top, borders.Right, borders.Bottom);
			}
		}
		public EditPanelViewInfo(Rectangle bounds, Margins borders) {
			Bounds = bounds;
			this.borders = borders;
		}
	}
}

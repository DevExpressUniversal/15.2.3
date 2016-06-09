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
using System.Linq;
using System.Text;
using System.Drawing;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.XtraBars.Painters {
	public class OfficeXPPrimitivesPainter : PrimitivesPainter {
		public OfficeXPPrimitivesPainter(BarManagerPaintStyle paintStyle)
			: base(paintStyle) {
		}
		protected override MenuHeaderPainter CreateDefaultMenuHeaderPainter() {
			return new MenuHeaderOfficeXPPainter();
		}
		public override void DrawLinkHighLightedBackground(BarLinkPaintArgs e, Rectangle r, BarLinkEmptyBorder border, BarLinkState realState, Brush backBrush) {
			DrawLinkHighlightedFrame(e, ref r, border);
			PaintHelper.FillRectangle(e.Graphics, GetLinkHighlightedBackBrush(e, realState, backBrush), r);
		}
		protected virtual Brush GetLinkHighlightedBackBrush(BarLinkPaintArgs e, BarLinkState state, Brush backBrush) {
			Color clr = e.LinkInfo.GetItemAppearance(state).BackColor;
			return clr.IsEmpty ? backBrush : e.LinkInfo.GetBackBrush(e.Cache, e.LinkInfo.Bounds);
		}
	}
	public class MenuHeaderOfficeXPPainter : MenuHeaderPainter {
		public override void DrawObject(BarLinkPaintArgs e) {
			e.Graphics.FillRectangle(Brushes.Brown, e.LinkInfo.Bounds);
		}
		public override int CalcElementHeight(Graphics g, BarHeaderLinkViewInfo linkInfo, Size captionSize, int res) {
			return res;
		}
	}
}

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
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraReports.Design {
	[BrickExporter(typeof(DesignPivotBrickExporter))]
	public class DesignPivotBrick : TextBrick {
		static Color selectColor { get { return Color.FromArgb(75, 0, 0, 0); } }
		static Color hotColor { get { return Color.FromArgb(75, 255, 255, 255); } }
		Color hitTestColor = Color.Empty;
		internal Color HitTestColor { get { return hitTestColor; } }
		public void MarkAsSelected() {
			hitTestColor = selectColor;
		}
		public void MarkAsHot() {
			if(hitTestColor == Color.Empty)
				hitTestColor = hotColor;
		}
	}
	public class DesignPivotBrickExporter : TextBrickExporter {
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			base.DrawClientContent(gr, clientRect);
			Color hitTestColor = (Brick as DesignPivotBrick).HitTestColor;
			if(hitTestColor != Color.Empty) {
				Brush brush = new SolidBrush(hitTestColor);
				RectangleF rect = VisualBrick.Padding.Inflate(clientRect, GraphicsDpi.Document);
				gr.FillRectangle(brush, rect);
				brush.Dispose();
			}
		}
	}
}

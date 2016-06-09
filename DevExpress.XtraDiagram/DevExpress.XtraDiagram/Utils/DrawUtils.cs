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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Utils {
	public class DrawUtils {
		public static void DrawSizeGrip(ObjectInfoArgs e, Rectangle rect, Color color, Color cornerColor) {
			ClearRect(e, rect);
			e.Graphics.DrawRectangle(e.Cache.GetPen(color), rect);
			e.Graphics.DrawRectCorners(e.Cache.GetSolidBrush(cornerColor), rect);
		}
		public static void ClearRect(ObjectInfoArgs e, Rectangle rect) {
			e.Graphics.FillRectangle(Brushes.White, rect);
		}
		public static void ClearCircle(ObjectInfoArgs e, Rectangle rect) {
			e.Graphics.FillEllipse(Brushes.White, rect);
		}
		public static void DrawRadialCloudEffect(ObjectInfoArgs e, Color color, Rectangle rect, int alpha = 40) {
			Color cloudColor = Color.FromArgb(alpha, color);
			e.Graphics.DrawEllipse(e.Cache.GetPen(cloudColor, 2), rect);
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(cloudColor), rect.X + rect.Width / 2, rect.Y - 1, 1, 1);
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(cloudColor), rect.Right + 1, rect.Y + rect.Height / 2, 1, 1);
		}
		public static void DrawRectangularCloudEffect(ObjectInfoArgs e, Color color, Rectangle rect, int alpha = 120) {
			FillRectangle(e, Rectangle.Inflate(rect, 0, -1), Color.FromArgb(alpha, color));
			FillRectangle(e, Rectangle.Inflate(rect, -1, 0), Color.FromArgb(alpha, color));
			FillRectangle(e, Rectangle.Inflate(rect, 0, -2), color);
			FillRectangle(e, Rectangle.Inflate(rect, -2, 0), color);
			FillRectangle(e, Rectangle.Inflate(rect, -1, -1), color);
		}
		public static void FillRectangle(ObjectInfoArgs e, Rectangle rect, Color color, int xOffset = 1, int yOffset = 1) {
			Rectangle displayRect = rect;
			displayRect.Offset(xOffset, yOffset);
			displayRect.Width -= xOffset;
			displayRect.Height -= yOffset;
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(color), displayRect);
		}
	}
}

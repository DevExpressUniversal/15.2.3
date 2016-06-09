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
#if !SL
using System.Drawing.Drawing2D;
#endif
using System.Linq;
using System.Text;
namespace DevExpress.XtraRichEdit.Layout {
	   #if !SL
	public class CommentPainter {
		public GraphicsPath CreatePathRoundedRectangle(Rectangle rect, int radius) {
			radius = CalculateRadius(radius, rect.Height, rect.Width);
			int diameter = radius * 2;
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
			path.AddLine(rect.Left + radius, rect.Top, rect.Right - radius, rect.Top);
			path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
			path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom - radius);
			path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
			path.AddLine(rect.Left + radius, rect.Bottom, rect.Right - radius, rect.Bottom);
			path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
			path.AddLine(rect.Left, rect.Top + radius, rect.Left, rect.Bottom - radius);
			path.CloseFigure();
			return path;
		}
		int CalculateRadius(int radius, int height, int width) {
			int result = radius;
			if (result > (int)(height / 2))
				result = (int)(height / 2);
			if (result > (int)(width / 2))
				result = (int)(width / 2);
			return result;
		}
	}
#endif
}

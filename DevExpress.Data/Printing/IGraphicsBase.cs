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
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
using DevExpress.Xpf.Drawing.Drawing2D;
using DevExpress.Xpf.Windows.Forms;
using DevExpress.XtraPrinting.Stubs;
using Brush = DevExpress.Xpf.Drawing.Brush;
#else
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting {
	public interface IGraphicsBase {
		RectangleF ClipBounds { get; set; }
		GraphicsUnit PageUnit { get; set; }
		SmoothingMode SmoothingMode { get; set; }
		void FillRectangle(Brush brush, RectangleF bounds);
		void FillRectangle(Brush brush, float x, float y, float width, float height);
		void DrawString(string s, Font font, Brush brush, RectangleF bounds, StringFormat format);
		void DrawString(string s, Font font, Brush brush, RectangleF bounds);
		void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format);
		void DrawString(string s, Font font, Brush brush, PointF point);
		void DrawLine(Pen pen, PointF pt1, PointF pt2);
		void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
		void DrawLines(Pen pen, PointF[] points);
		void DrawImage(Image image, RectangleF rect);
		void DrawImage(Image image, RectangleF rect, Color underlyingColor);
		void DrawImage(Image image, Point point);
		void DrawCheckBox(RectangleF rect, CheckState state);
		void DrawRectangle(Pen pen, RectangleF bounds);
		void DrawPath(Pen pen, GraphicsPath path);
		void FillPath(Brush brush, GraphicsPath path);
		void DrawEllipse(Pen pen, RectangleF rect);
		void DrawEllipse(Pen pen, float x, float y, float width, float height);
		void FillEllipse(Brush brush, RectangleF rect);
		void FillEllipse(Brush brush, float x, float y, float width, float height);
		SizeF MeasureString(string text, Font font, GraphicsUnit graphicsUnit);
		SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit graphicsUnit);
#if SL
		SizeF MeasureString(string p, Font font, int p_2, StringFormat sf, GraphicsUnit unit);
#else
		SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit graphicsUnit);
		SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit graphicsUnit);
#endif
		void ResetTransform();
		void RotateTransform(float angle);
		void RotateTransform(float angle, MatrixOrder order);
		void ScaleTransform(float sx, float sy);
		void ScaleTransform(float sx, float sy, MatrixOrder order);
		void TranslateTransform(float dx, float dy);
		void TranslateTransform(float dx, float dy, MatrixOrder order);
		void SaveTransformState();
		void ApplyTransformState(MatrixOrder order, bool removeState);
		Brush GetBrush(Color color);
	}
}

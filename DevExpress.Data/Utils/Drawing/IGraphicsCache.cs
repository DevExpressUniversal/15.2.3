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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Utils.Drawing {
	public interface IGraphicsCache : IDisposable {
		void Clear();
		Matrix TransformMatrix { get; }
		Point Offset { get; }
		Rectangle CalcRectangle(Rectangle r);
		Rectangle CalcClipRectangle(Rectangle r);
		void ResetMatrix();
		bool IsNeedDrawRect(Rectangle r);
		Graphics Graphics { get; }
		Font GetFont(Font font, FontStyle fontStyle);
		Brush GetSolidBrush(Color color);
		Pen GetPen(Color color);
		Pen GetPen(Color color, int width);
		Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode);
		Brush GetGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode, int blendCount);
		void FillRectangle(Brush brush, Rectangle rect);
		void FillRectangle(Brush brush, RectangleF rect);
		void FillRectangle(Color color, Rectangle rect);
		void DrawVString(string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat, int angle);
		void DrawString(string text, Font font, Brush foreBrush, Rectangle bounds, StringFormat strFormat);
		SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth);
		SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth, int maxHeight);
		SizeF CalcTextSize(string text, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped);
		void DrawRectangle(Pen pen, Rectangle r);
	}
}

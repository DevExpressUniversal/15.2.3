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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.Utils;
using PlatformPoint = System.Windows.Point;
using PlatformSize = System.Windows.Size;
namespace DevExpress.XtraDiagram.Extensions {
	public static class GraphicsPathExtensions {
		public static void TranslateTransform(this GraphicsPath path, Point pt) {
			TranslateTransform(path, pt.X, pt.Y);
		}
		public static void TranslateTransform(this GraphicsPath path, float x, float y) {
			Matrix matrix = new Matrix();
			matrix.Translate(x, y);
			path.Transform(matrix);
		}
		public static void RotateTransform(this GraphicsPath path, float angle, Point point) {
			Matrix matrix = new Matrix();
			matrix.RotateAt(angle, point);
			path.Transform(matrix);
		}
		public static void AddString(this GraphicsPath path, string content, Font font, Point loc, StringFormat format) {
			path.AddString(content, font.FontFamily, (int)font.Style, font.Size * 1.4f, loc, format);
		}
		public static void AddString(this GraphicsPath path, string content, Font font, Rectangle rect, StringFormat format) {
			path.AddString(content, font.FontFamily, (int)font.Style, font.Size * 1.4f, rect, format);
		}
		public static void AddArc(this GraphicsPath path, PlatformPoint start, PlatformPoint end, PlatformSize size, SpinDirection direction) {
			if(MathUtils.IsEquals(size.Width, 0)) size.Width = 1;
			if(MathUtils.IsEquals(size.Height, 0)) size.Height = 1;
			ArcDrawParams arcPar = PlatformPointUtils.GetArcParams(start, end, size, direction);
			path.AddArc((float)arcPar.Rect.X, (float)arcPar.Rect.Y, (float)arcPar.Rect.Width, (float)arcPar.Rect.Height, arcPar.Angle, arcPar.SweepAngle);
		}
	}
}

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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public static class GraphicsUtils {
		public static GRect2D ConvertRect(Rect rect) {
			int left = MathUtils.StrongRound(rect.Left);
			int top = MathUtils.StrongRound(rect.Top);
			return new GRect2D(left, top, MathUtils.StrongRound(rect.Right) - left, MathUtils.StrongRound(rect.Bottom) - top);
		}
		public static Rect MakeRect(Point center, double width, double height) {
			return new Rect(center.X - 0.5 * width, center.Y - 0.5 * height, width, height);
		}
		public static Rect UnionRect(Rect rect1, Rect rect2) {
			if (rect1.IsEmpty)
				return rect2;
			if (rect2.IsEmpty)
				return rect1;
			rect1.Union(rect2);
			return rect1;
		}
		public static bool IsThicknessEmpty(Thickness thickness) {
			return thickness.Left == 0 && thickness.Top == 0 && thickness.Right == 0 && thickness.Bottom == 0;
		}
		public static bool IsSimpleBorder(Border border) {
			return ((border.Child is TextBlock) || border.Child == null) && 
				IsThicknessEmpty(border.Margin) &&
				IsThicknessEmpty(border.BorderThickness) &&
				border.LayoutTransform.IsIdentity() &&
				border.RenderTransform.IsIdentity();
		}		
	}
}

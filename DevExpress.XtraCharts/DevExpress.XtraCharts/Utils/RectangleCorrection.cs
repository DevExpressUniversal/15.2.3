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

using System.Drawing;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Native {
	public class RectangleCorrection {
		public static RectangleCorrection Combine(RectangleCorrection originalCorrection, RectangleCorrection newCorrection) {
			if (originalCorrection == null)
				return newCorrection;
			originalCorrection.Add(newCorrection);
			return originalCorrection;
		}
		readonly Rectangle bounds;
		int left = 0;
		int right = 0;
		int top = 0;
		int bottom = 0;
		public int Left { get { return left; } }
		public int Right { get { return right; } }
		public int Top { get { return top; } }
		public int Bottom { get { return bottom; } }
		public bool ShouldCorrect { get { return left > 0 || right > 0 || top > 0 || bottom > 0; } }
		public RectangleCorrection(Rectangle bounds) {
			this.bounds = bounds;
		}
		public Rectangle Correct(Rectangle rect) {
			rect.X += left;
			rect.Width -= left + right;
			rect.Y += top;
			rect.Height -= top + bottom;
			return rect;
		}
		public void Add(RectangleCorrection correction) {
			left += correction.left;
			right += correction.right;
			top += correction.top;
			bottom += correction.bottom;
		}
		public void Update(Rectangle b) {
			UpdateWidth(b);
			UpdateHeight(b);
		}
		public void UpdateWidth(Rectangle b) {
			if (b.Width > 0) {
				int correction = bounds.Left - b.Left;
				if (correction > left)
					left = correction;
				correction = b.Right - bounds.Right;
				if (correction > right)
					right = correction;
			}
		}
		public void UpdateHeight(Rectangle b) {
			if (b.Height > 0) {
				int correction = bounds.Top - b.Top;
				if (correction > top)
					top = correction;
				correction = b.Bottom - bounds.Bottom;
				if (correction > bottom)
					bottom = correction;
			}
		}
		public void Update(IEnumerable<Rectangle> list) {
			foreach (Rectangle item in list)
				Update(item);
		}
		public override string ToString() {
			return string.Format("Left={0},Right={1},Top={2},Bottom={3}", new object[] { Left, Right, Top, Bottom });
		}
	}
}

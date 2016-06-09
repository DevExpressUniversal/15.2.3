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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using D3D;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing.DirectX {
	public static class D3DMath {
		public static Vector2d CalcNormal(Vector2d p1, Vector2d p2) {
			Vector2d delta = p2 - p1;
			if(delta.X == 0.0)
				return new Vector2d(1.0, 0.0);
			double ratio = delta.Y / delta.X;
			double y = 1.0 / Math.Sqrt(ratio * ratio + 1);
			double x = -ratio * y;
			return new Vector2d(x, y);
		}
		public static bool Compare(float v1, float v2, float epsilon) {
			return Math.Abs(v1 - v2) < epsilon;
		}
	}
	public static class D3DUtils {
		public static bool CheckForD3DErrors(int d3dResult, string methodName, bool throException) {
			if(d3dResult != 0) {
				if(throException)
					throw new Exception(string.Format("Calling D3D {0} failed : {1} memory: {2}", methodName, d3dResult, GC.GetTotalMemory(false)>>20));
				return false;
			}  
			return true;
		}
		[CLSCompliant(false)]
		public static float[] GetRGBAColor(int color) {
			float a = (color >> 24 & 255) / 255.0f;
			float g = (color >> 8 & 255) / 255.0f;
			float r = (color >> 16 & 255) / 255.0f;
			float b = (color & 255) / 255.0f;
			return new float[4] { r, g, b, a };
		}
		public static Size RoundPow2Size(Size origin) {
			int i, j, S;
			for(S = 0, i = 0; origin.Width > S; i++)
				S = Convert.ToInt32(Math.Pow(2, i));
			for(S = 0, j = 0; origin.Height > S; j++)
				S = Convert.ToInt32(Math.Pow(2, j));
			return new Size(Convert.ToInt32(Math.Pow(2, i - 1)), Convert.ToInt32(Math.Pow(2, j - 1)));
		}
		public static Rectangle ValidateClipRect(Rectangle clipRect, Size viewPortSize) {
			Rectangle rect = RectUtils.ValidateNegative(clipRect);
			if(rect.X + rect.Width > viewPortSize.Width)
				rect.Width = Math.Max(0, viewPortSize.Width - rect.X);
			if(rect.Y + rect.Height > viewPortSize.Height)
				rect.Height = Math.Max(0, viewPortSize.Height - rect.Y);
			return rect;
		}
	}
}

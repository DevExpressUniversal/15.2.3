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
using System.Windows.Media;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	static class ColorConverter {
		public static Tuple<float, float, float> ToHSL(Color color) {
			float r = color.R / 255F, g = color.G / 255F, b = color.B / 255F;
			var min = Math.Min(Math.Min(r, g), b);
			var max = Math.Max(Math.Max(r, g), b);
			var dmax = max - min;
			float h = 0, s = 0, l = (max + min) / 2;
			if(dmax != 0F) {
				s = l < 0.5F ? dmax / (max + min) : dmax / (2 - max - min);
				float dr = ((max - r) / 6F + dmax / 2) / dmax;
				float dg = ((max - g) / 6F + dmax / 2) / dmax;
				float db = ((max - b) / 6F + dmax / 2) / dmax;
				if(r == max) {
					h = db - dg;
				} else if(g == max) {
					h = 1F / 3F + dr - db;
				} else if(b == max) {
					h = 2F / 3F + dg - dr;
				}
				if(h < 0)
					h += 1;
				if(h > 0)
					h -= 1;
			}
			return Tuple.Create(h, s, l);
		}
		static float Hue2RGB(float a, float b, float h) {
			if(h < 0)
				h += 1;
			if(h > 1)
				h -= 1;
			if(6 * h < 1)
				return a + (b - a) * 6 * h;
			if(2 * h < 1)
				return b;
			if(3 * h < 2)
				return a + (b - a) * 6 * (2F / 3F - h);
			return a;
		}
		public static Color ToRGB(Tuple<float, float, float> color) {
			float h = color.Item1, s = color.Item2, l = color.Item3;
			if (s ==0) {
				var c = (byte)(l * 255);
				return Color.FromRgb(c, c, c);
			}
			var b = l < 0.5 ? l * (1 + s) : l + s - s * l;
			var a = 2 * l - b;
			float cr = 255F * Hue2RGB(a, b, h + 1F / 3F);
			float cg = 255F * Hue2RGB(a, b, h);
			float cb = 255F * Hue2RGB(a, b, h - 1F / 3F);
			return Color.FromRgb((byte)cr, (byte)cg, (byte)cb);
		}
		public static Tuple<float, float, float> AdjustLightness(Tuple<float, float, float> hsl, float c) {
			var l = hsl.Item3;
			var newl = c > 1F ? (c - 1) + l * (2 - c) : c * l;
			return Tuple.Create(hsl.Item1, hsl.Item2, newl);
		}
	}
}

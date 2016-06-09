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
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Native {
	public class MathMethods {
		static public SizeF Scale(SizeF val, double ratio) {
			return new SizeF((float)(ratio * val.Width), (float)(ratio * val.Height));
		}
		static public SizeF Scale(SizeF val, float ratio) {
			return Scale(val, (double)ratio);
		}
		static public PointF Scale(PointF val, double ratio) {
			return new PointF((float)(ratio * val.X), (float)(ratio * val.Y));
		}
		static public PointF Scale(PointF val, float ratio) {
			return Scale(val, (double)ratio);
		}
		static public RectangleF Scale(RectangleF val, double ratio) {
			return new RectangleF((float)(ratio * val.X), (float)(ratio * val.Y), (float)(ratio * val.Width), (float)(ratio * val.Height));
		}
		static public RectangleF Scale(RectangleF val, float ratio) {
			return Scale(val, (double)ratio);
		}
		static public double Scale(double val, double ratio) {
			return val * ratio;
		}
		static public float Scale(float val, double ratio) {
			return (float)Scale((double)val, ratio);
		}
		public static SizeF ZoomInto(SizeF outer, SizeF inner) {
			SizeF result = new SizeF();
			float innerRatio = (float)inner.Width / (float)inner.Height;
			float outerRatio = (float)outer.Width / (float)outer.Height;
			if (innerRatio < outerRatio) {
				result.Height = outer.Height;
				result.Width = outer.Height * innerRatio;
			}
			else {
				result.Width = outer.Width;
				result.Height = outer.Width / innerRatio;
			}
			return result;
		}
	}
}

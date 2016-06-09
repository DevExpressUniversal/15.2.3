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
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfRenderingPatch {
		readonly Color leftBottom;
		readonly Color leftTop;
		readonly Color rightTop;
		readonly Color rightBottom;
		public abstract double Width { get; }
		public abstract double Height { get; }
		protected PdfRenderingPatch(Color[] colors) { 
			leftBottom = colors[0];
			leftTop = colors[1];
			rightTop = colors[2];
			rightBottom = colors[3];
		}
		public Color CalculateColor(double u, double v) {
			return Color.FromArgb(Convert.ToByte((1 - v) * ((1 - u) * leftBottom.R + u * rightBottom.R) + v * ((1 - u) * leftTop.R + u * rightTop.R)), 
								  Convert.ToByte((1 - v) * ((1 - u) * leftBottom.G + u * rightBottom.G) + v * ((1 - u) * leftTop.G + u * rightTop.G)), 
								  Convert.ToByte((1 - v) * ((1 - u) * leftBottom.B + u * rightBottom.B) + v * ((1 - u) * leftTop.B + u * rightTop.B)));	
		}
		public abstract Point CalculatePoint(double u, double v);
	}
}

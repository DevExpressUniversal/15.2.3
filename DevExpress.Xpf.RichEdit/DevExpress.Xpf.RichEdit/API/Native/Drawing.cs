﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.Windows.Media;
using DevExpress.XtraRichEdit;
namespace DevExpress.Xpf.RichEdit {
	public class RichEditPen : RichEditPenBase {
		Color color;
		public RichEditPen(Color color, int thickness)
			: base(thickness) {
			this.color = color;
		}
		public RichEditPen(Color color)
			: this(color, 1) {
		}
		public Color Color { get { return color; } set { color = value; } }
		internal override System.Drawing.Color GetColor() {
			return System.Drawing.Color.FromArgb(Color.A, Color.R, Color.G, Color.B);
		}
		internal override System.Drawing.Pen CreatePlatformIndependentPen() {
			System.Drawing.Pen result = new System.Drawing.Pen(GetColor(), Thickness);
			if (DashStyle != RichEditDashStyle.Solid)
				result.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
			switch (DashStyle) {
				case RichEditDashStyle.Dash:
					result.DashPattern = new float[] { 3, 1.5f };
					break;
				case RichEditDashStyle.DashDot:
					result.DashPattern = new float[] { 3, 1.5f, 1.5f, 1.5f };
					break;
				case RichEditDashStyle.DashDotDot:
					result.DashPattern = new float[] { 3, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f };
					break;
				case RichEditDashStyle.Dot:
					result.DashPattern = new float[] { 1, 1.5f };
					break;
			}
			return result;
		}
	}
	public class RichEditBrush : RichEditBrushBase {
		Color color;
		public RichEditBrush(Color color) {
			this.color = color;
		}
		public Color Color { get { return color; } set { color = value; } }
		internal override System.Drawing.Color GetColor() {
			return System.Drawing.Color.FromArgb(Color.A, Color.R, Color.G, Color.B);
		}
	}
}

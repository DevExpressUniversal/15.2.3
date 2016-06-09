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
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Base;
using System.IO;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Drawing {
	public class MouseEventArgsInfo {
		int x;
		int y;
		public MouseEventArgsInfo(Point p) { x = p.X; y = p.Y; }
		public MouseEventArgsInfo(int x, int y) { this.x = x; this.y = y; }
		public int X {
			get { return x; }
			set { x = value; }
		}
		public int Y {
			get { return y; }
			set { y = value; }
		}
	}
	public enum GradientBrushType { Default, Horizontal, Vertical, ForwardDiagonal, BackwardDiagonal, Linear, Radial }
	public sealed class GradientStop : ISupportAcceptOrder {
		int acceptOrderCore;
		Color colorCore;
		float offsetCore;
		public GradientStop() { }
		public GradientStop(Color color, float offset)
			: this() {
			this.colorCore = color;
			this.offsetCore = offset;
			this.acceptOrderCore = 0;
		}
		public Color Color {
			get { return colorCore; }
			set { colorCore = value; }
		}
		public float Offset {
			get { return offsetCore; }
			set { offsetCore = value; }
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return acceptOrderCore; }
			set { acceptOrderCore = value; }
		}
	}
	public sealed class GradientStopCollection : BaseChangeableList<GradientStop> { }
	public enum RenderMethod { Default, ToGraphics, ToStream }
	public class PaintInfo : BaseObject {
		RenderMethod methodCore;
		Graphics graphicsCore;
		Stream streamCore;
		public PaintInfo(Graphics g)
			: base() {
			this.graphicsCore = g;
		}
		public PaintInfo(IRenderingContext context)
			: base() {
			if(context != null) {
				this.graphicsCore = context.Graphics;
				this.streamCore = context.Stream;
				this.methodCore = (Stream != null) ? RenderMethod.ToStream : RenderMethod.Default;
			}
		}
		public PaintInfo(PaintInfo info)
			: base() {
			if(info != null) {
				this.graphicsCore = info.graphicsCore;
				this.streamCore = info.streamCore;
				this.methodCore = info.methodCore;
			}
		}
		protected override void OnCreate() {
			this.methodCore = RenderMethod.Default;
		}
		protected override void OnDispose() {
			this.graphicsCore = null;
			this.streamCore = null;
		}
		public Graphics Graphics {
			get { return graphicsCore; }
		}
		public Stream Stream {
			get { return streamCore; }
		}
		public RenderMethod Method {
			get { return methodCore; }
		}
	}
}

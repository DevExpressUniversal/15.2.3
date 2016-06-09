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

using System.Windows.Media;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public class RenderPathContext : FrameworkRenderElementContext {
		Geometry data;
		Stretch? stretch;
		Brush fill;
		Brush stroke;
		double? strokeThickness;
		PenLineCap? strokeStartLineCap;
		PenLineCap? strokeEndLineCap;
		PenLineCap? strokeDashCap;
		PenLineJoin? strokeLineJoin;
		double? strokeMiterLimit;
		double? strokeDashOffset;
		DoubleCollection strokeDashArray;
		public Geometry Data {
			get { return data; }
			set { SetProperty(ref data, value.With(x => (Geometry)x.GetCurrentValueAsFrozen())); }
		}		
		public Stretch? Stretch {
			get { return stretch; }
			set { SetProperty(ref stretch, value); }
		}
		public Brush Fill {
			get { return fill; }
			set { SetProperty(ref fill, value.With(x => (Brush)x.GetCurrentValueAsFrozen())); }
		}
		public Brush Stroke {
			get { return stroke; }
			set { SetProperty(ref stroke, value.With(x => (Brush)x.GetCurrentValueAsFrozen())); }
		}
		public double? StrokeThickness {
			get { return strokeThickness; }
			set { SetProperty(ref strokeThickness, value); }
		}
		public PenLineCap? StrokeStartLineCap {
			get { return strokeStartLineCap; }
			set { SetProperty(ref strokeStartLineCap, value); }
		}
		public PenLineCap? StrokeEndLineCap {
			get { return strokeEndLineCap; }
			set { SetProperty(ref strokeEndLineCap, value); }
		}
		public PenLineCap? StrokeDashCap {
			get { return strokeDashCap; }
			set { SetProperty(ref strokeDashCap, value); }
		}
		public PenLineJoin? StrokeLineJoin {
			get { return strokeLineJoin; }
			set { SetProperty(ref strokeLineJoin, value); }
		}
		public double? StrokeMiterLimit {
			get { return strokeMiterLimit; }
			set { SetProperty(ref strokeMiterLimit, value); }
		}
		public double? StrokeDashOffset {
			get { return strokeDashOffset; }
			set { SetProperty(ref strokeDashOffset, value); }
		}
		public DoubleCollection StrokeDashArray {
			get { return strokeDashArray; }
			set { SetProperty(ref strokeDashArray, value); }
		}
		public Geometry ActualData { get { return Data ?? Factory.Data ?? Geometry.Empty; } }
		public Stretch ActualStretch { get { return Stretch.HasValue ? Stretch.Value : Factory.Stretch; } }
		public Brush ActualFill { get { return Fill ?? Factory.Fill; } }
		public Brush ActualStroke { get { return Stroke ?? Factory.Stroke; } }
		public double ActualStrokeThickness { get { return StrokeThickness.HasValue ? StrokeThickness.Value : Factory.StrokeThickness; } }
		public PenLineCap ActualStrokeStartLineCap { get { return StrokeStartLineCap.HasValue ? StrokeStartLineCap.Value : Factory.StrokeStartLineCap; } }
		public PenLineCap ActualStrokeEndLineCap { get { return StrokeEndLineCap.HasValue ? StrokeEndLineCap.Value : Factory.StrokeEndLineCap; } }
		public PenLineCap ActualStrokeDashCap { get { return StrokeDashCap.HasValue ? StrokeDashCap.Value : Factory.StrokeDashCap; } }
		public PenLineJoin ActualStrokeLineJoin { get { return StrokeLineJoin.HasValue ? StrokeLineJoin.Value : Factory.StrokeLineJoin; } }
		public double ActualStrokeMiterLimit { get { return StrokeMiterLimit.HasValue ? StrokeMiterLimit.Value : Factory.StrokeMiterLimit; } }
		public double ActualStrokeDashOffset { get { return StrokeDashOffset.HasValue ? StrokeDashOffset.Value : Factory.StrokeDashOffset; } }
		public DoubleCollection ActualStrokeDashArray { get { return StrokeDashArray ?? Factory.StrokeDashArray; } }
		public new RenderPath Factory { get { return base.Factory as RenderPath; } }
		public RenderPathContext(RenderPath factory)
			: base(factory) {
		}
		public Pen Pen { get; internal set; }
		public Geometry RenderedGeometry { get; internal set; }
		public Matrix? StretchMatrix { get; internal set; }
	}
}

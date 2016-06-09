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
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map {
	public abstract class MapShape : MapShapeBase, IMapItemStyleProvider {
		Shape shape;
		protected override IMapItemStyleProvider StyleProvider {
			get { return shape != null ? this : null; }
		}
		internal MapItemPresenter ShapeCanvas {
			get { return Container as MapItemPresenter; }
		}
		protected internal Shape Shape { 
			get { return shape; }
		}
		#region IMapItemStyleProvider Members
		Brush IMapItemStyleProvider.Fill {
			get { return shape.Fill; }
			set { shape.Fill = value; }
		}
		Brush IMapItemStyleProvider.Stroke {
			get { return shape.Stroke; }
			set { shape.Stroke = value; }
		}
		Visibility IMapItemStyleProvider.Visibility {
			get { return shape.Visibility; }
			set { shape.Visibility = value; }
		}
		double IMapItemStyleProvider.StrokeThickness {
			get { return shape.StrokeThickness; }
			set { shape.StrokeThickness = value; }
		}
		DoubleCollection IMapItemStyleProvider.StrokeDashArray {
			get { return shape.StrokeDashArray; }
			set { shape.StrokeDashArray = value; }
		}
		PenLineCap IMapItemStyleProvider.StrokeDashCap {
			get { return shape.StrokeDashCap; }
			set { shape.StrokeDashCap = value; }
		}
		double IMapItemStyleProvider.StrokeDashOffset {
			get { return shape.StrokeDashOffset; }
			set { shape.StrokeDashOffset = value; }
		}
		PenLineCap IMapItemStyleProvider.StrokeEndLineCap {
			get { return shape.StrokeEndLineCap; }
			set { shape.StrokeEndLineCap = value; }
		}
		PenLineCap IMapItemStyleProvider.StrokeStartLineCap {
			get { return shape.StrokeStartLineCap; }
			set { shape.StrokeStartLineCap = value; }
		}
		PenLineJoin IMapItemStyleProvider.StrokeLineJoin {
			get { return shape.StrokeLineJoin; }
			set { shape.StrokeLineJoin = value; }
		}
		double IMapItemStyleProvider.StrokeMiterLimit {
			get { return shape.StrokeMiterLimit; }
			set { shape.StrokeMiterLimit = value; }
		}
		Effect IMapItemStyleProvider.Effect {
			get { return shape.Effect; }
			set { shape.Effect = value; }
		}
		#endregion
		protected override void UpdateToolTipPattern() {
			Title.ToolTipPattern = ToolTipPattern;
		}
		protected internal override void OnItemChanged(object shape) {
			this.shape = shape as Shape;
			if (Shape != null)
				Shape.Stretch = Stretch.Fill;
			base.OnItemChanged(shape);
		}
	}
}

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

using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Shape.Native;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraPrinting.Shape {
	public class ShapeStar : ShapePolygonBase {
		float concavity = 50F;
		internal override PreviewStringId ShapeStringId {
			get { return PreviewStringId.Shapes_Star; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeStarStarPointCount"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeStar.StarPointCount"),
		DefaultValue(3),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public int StarPointCount {
			get { return base.NumberOfSides; }
			set { base.NumberOfSides = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("ShapeStarConcavity"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.Shape.ShapeStar.Concavity"),
		DefaultValue(50F),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public float Concavity {
			get { return concavity; }
			set { concavity = ShapeHelper.ValidatePercentageValue(value, "Concavity"); }
		}
		public ShapeStar() {
		}
		ShapeStar(ShapeStar source)
			: base(source) {
			concavity = source.Concavity;
		}
		protected override ShapeBase CloneShape() {
			return new ShapeStar(this);
		}
		protected internal override PointF[] CreatePoints(RectangleF bounds, int angle) {
			PointF[] outerVertexes = base.CreatePointsCore(bounds, 0, 1);
			float inscribedCircleRadius = (float)Math.Cos(AngleStep / 2);
			PointF[] innerVertexes = base.CreatePointsCore(bounds, AngleStep / 2, (1 - (concavity / 100f)) * inscribedCircleRadius);
			PointF[] vertexes = new PointF[NumberOfSides * 2];
			int count = NumberOfSides;
			for(int i = 0; i < count; i++) {
				vertexes[2 * i] = outerVertexes[i];
				vertexes[2 * i + 1] = innerVertexes[i];
			}
			return vertexes;
		}
	}
}

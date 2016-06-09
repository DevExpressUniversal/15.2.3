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
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Shape;
namespace DevExpress.XtraReports.Native.Templates {
	public class Star {
		#region inner classes
		class FivePointRegularStar : ShapeStar {
			public PointF[] GetPoints() {
				this.NumberOfSides = 5;
				this.Concavity = 52.78F;
				return CreatePoints(new Rectangle(0, 0, 2, 2), 0);
			}
		}
		#endregion
		#region static
		public static Star Create(float ratingValue, int i, float size) {
			int integerPart = (int)ratingValue;
			float fractionalPart = ratingValue - integerPart;
			int startIndexFractionalStars = fractionalPart != 0 ? integerPart + 1 : -1;
			int startEmptyFractionalStars = startIndexFractionalStars == -1 ? integerPart : integerPart + 1;
			if ((i >= 0) && (i < integerPart)) {
				return new FilledStar(size);
			}
			if ((i >= integerPart) && (i < startIndexFractionalStars)) {
				return new PartiallyFilledStar(size, fractionalPart);
			}
			if (i >= startEmptyFractionalStars) {
				return new Star(size);
			}
			throw new NotImplementedException();
		}
		static PointF[] singleRadiusBaseStarPoints;
		static Star() {
			singleRadiusBaseStarPoints = new FivePointRegularStar().GetPoints();
		}
		#endregion
		PointF[] baseStarPoints;
		protected float size;
		public Star(float size) {
			this.size = size;
		}
		PointF[] BasePoints {
			get {
				if (baseStarPoints == null) {
					CalcBaseStarPoints();
				}
				return baseStarPoints;
			}
		}
		void CalcBaseStarPoints() {
			baseStarPoints = new PointF[singleRadiusBaseStarPoints.Length];
			baseStarPoints = (PointF[])singleRadiusBaseStarPoints.Clone();
			float r = this.size / 2;
			Matrix matrix = new Matrix();
			matrix.Scale(r, r);
			matrix.TransformPoints(baseStarPoints);
		}
		public virtual void Draw(Graphics g, Color starColor, float offsetX) {
			Pen pen = new Pen(starColor);
			g.DrawPolygon(pen, OffsetBasePoints(offsetX));
		}
		protected PointF[] OffsetBasePoints(float offset) {
			List<PointF> starsPoints = new List<PointF>();
			foreach (PointF starPoint in BasePoints) {
				starsPoints.Add(new PointF(starPoint.X + offset, starPoint.Y));
			}
			return starsPoints.ToArray();
		}
	}
	public class FilledStar : Star {
		public FilledStar(float size)
			: base(size) {
		}
		public override void Draw(Graphics g, Color starColor, float offsetX) {
			base.Draw(g, starColor, offsetX);
			SolidBrush solidBrush = new SolidBrush(starColor);
			Region fillRegion = null;
			try {
				GraphicsPath path = new GraphicsPath();
				path.AddPolygon(OffsetBasePoints(offsetX));
				fillRegion = new Region(path);
				path.Dispose();
				FillRegion(g, solidBrush, fillRegion, offsetX);
			}
			finally {
				fillRegion.Dispose();
				solidBrush.Dispose();
			}
		}
		protected virtual void FillRegion(Graphics g, SolidBrush solidBrush, Region fillRegion, float offsetX) {
			g.FillRegion(solidBrush, fillRegion);
		}
	}
	public class PartiallyFilledStar : FilledStar {
		float fractionalPart;
		public PartiallyFilledStar(float size, float fractionalPart) : base(size) {
			this.fractionalPart = fractionalPart;
		}
		protected override void FillRegion(Graphics g, SolidBrush solidBrush, Region fillRegion, float offsetX) {
			fillRegion.Intersect(new RectangleF(offsetX, 0, size * fractionalPart, size));
			g.FillRegion(solidBrush, fillRegion);
		}
	}
}

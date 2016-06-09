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
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public class HitRegionContainer : IDisposable {
		IHitRegion underlying;
		public IHitRegion Underlying { get { return underlying; } }
		public HitRegionContainer() : this(new HitRegion()) {
		}
		public HitRegionContainer(IHitRegion underlying) {
			if(underlying == null)
				throw new ArgumentNullException("underlying");
			this.underlying = underlying;
		}
		public void Union(HitRegionContainer container) {
			if(container != null)
				Union((IHitRegion)container.Underlying.Clone());
		}
		public void Union(IHitRegion hitRegion) {
			if(hitRegion != null)
				underlying = new HitRegionUnionExpression(underlying, hitRegion);
		}
		public void Intersect(HitRegionContainer container) {
			if(container != null)
				Intersect((IHitRegion)container.Underlying.Clone());
		}
		public void Intersect(IHitRegion hitRegion) {
			if(hitRegion != null)
				underlying = new HitRegionIntersectionExpression(underlying, hitRegion);
		}
		public void Exclude(HitRegionContainer container) {
			if(container != null)
				Exclude((IHitRegion)container.Underlying.Clone());
		}
		public void Exclude(IHitRegion hitRegion) {
			if(hitRegion != null)
				underlying = new HitRegionExclusionExpression(underlying, hitRegion);
		}
		public void Xor(HitRegionContainer container) {
			if(container != null)
				Xor((IHitRegion)container.Underlying.Clone());
		}
		public void Xor(IHitRegion hitRegion) {
			if(hitRegion != null)
				underlying = new HitRegionXorExpression(underlying, hitRegion);
		}
		public void Dispose() {
			if(underlying != null) {	
				underlying.Dispose();
				underlying = null;
			}
		}
	}
	public class HitRegion : IHitRegion {
		Bitmap bitmap;
		RectangleF rect;
		GraphicsPath path;
		bool ContainsRect { get { return !rect.IsEmpty; } }
		bool ContainsPath { get { return path != null; } }		
		public RectangleF Rect { get { return rect; } }
		public GraphicsPath Path { get { return path; } }
		public bool IsEmpty { get { return rect.IsEmpty && path == null; } }
		public Bitmap Bitmap { get { return bitmap; } }
		public HitRegion() : this(RectangleF.Empty, null) {
		}
		public HitRegion(RectangleF rect) : this(rect, null) {
		}
		public HitRegion(GraphicsPath path) : this(RectangleF.Empty, path) {
		}
		public HitRegion(VariousPolygon polygon) : this(Rectangle.Empty, polygon.GetPath()) {
		}
		public HitRegion(RectangleF rect, GraphicsPath path) {
			this.rect = rect;
			this.path = path;
			CreateBitmap();
		}
		void CreateBitmap() {
			if (ContainsRect && ContainsPath) {
				bitmap = new Bitmap(MathUtils.StrongRound(Math.Abs(rect.X) + rect.Width), MathUtils.StrongRound(Math.Abs(rect.Y) + rect.Height));
				using (Graphics gr = Graphics.FromImage(bitmap)) {
					gr.SetClip(rect);
					gr.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					gr.FillPath(Brushes.Black, path);
				}
			}
		}
		public bool IsVisible(PointF hitPoint) {
			if (ContainsRect) {
				if (ContainsPath) {
					int x = MathUtils.StrongRound(hitPoint.X);
					int y = MathUtils.StrongRound(hitPoint.Y);
					if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
						return false;
					return bitmap.GetPixel(x, y).ToArgb() != Color.White.ToArgb();
				}
				return rect.Contains(hitPoint);
			}
			else if (ContainsPath)
				return path.IsVisible(hitPoint);
			else
				return false;
		}
		public Region GetGDIRegion() {
			if(ContainsRect)
				return new Region(rect);
			else if(ContainsPath)
				return new Region(path);
			else {
				Region region = new Region();
				region.MakeEmpty();
				return region;
			}
		}
		public void Serialize(IHitRegionSerializer serializer) {
			if (ContainsPath)
				serializer.SerializePath(path);
			else if (ContainsRect)
				serializer.SerializeRectangle(rect);
			else
				serializer.SerializeEmptyRegion();
		}
		public void Dispose() {
			if(path != null) {
				path.Dispose();
				path = null;
			}
			if (bitmap != null) {
				bitmap.Dispose();
				bitmap = null;
			}
		}
		public object Clone() {
			if (ContainsRect) {
				if (ContainsPath)
					return new HitRegion(rect, (GraphicsPath)path.Clone());
				return new HitRegion(rect);
			}
			else if (ContainsPath)
				return new HitRegion((GraphicsPath)path.Clone());
			else
				return new HitRegion();
		}
	}
	public abstract class HitRegionExpressionBase : IHitRegion {
		IHitRegion leftOperand;
		IHitRegion rightOperand;
		public IHitRegion LeftOperand { get { return leftOperand; } }
		public IHitRegion RightOperand { get { return rightOperand; } }
		public HitRegionExpressionBase(IHitRegion leftOperand, IHitRegion rightOperand) {
			this.leftOperand = leftOperand;
			this.rightOperand = rightOperand;
		}
		protected abstract void ApplyOperationToGDIRegion(Region leftRegion, Region rightRegion);
		protected abstract HitRegionExpressionBase Clone(IHitRegion leftOperand, IHitRegion rightOperand);
		public abstract bool IsVisible(PointF hitPoint);
		public Region GetGDIRegion() {
			Region leftRegion = LeftOperand.GetGDIRegion();
			using(Region rightRegion = RightOperand.GetGDIRegion())
				ApplyOperationToGDIRegion(leftRegion, rightRegion);
			return leftRegion;
		}
		public abstract void Serialize(IHitRegionSerializer serializer);
		public void Dispose() {
			if(leftOperand != null) {
				leftOperand.Dispose();
				leftOperand = null;
			}
			if(rightOperand != null) {
				rightOperand.Dispose();
				rightOperand = null;
			}
		}
		public object Clone() {
			return Clone((IHitRegion)leftOperand.Clone(), (IHitRegion)rightOperand.Clone());
		}
	}
	public class HitRegionUnionExpression : HitRegionExpressionBase {
		public HitRegionUnionExpression(IHitRegion leftOperand, IHitRegion rightOperand) : base(leftOperand, rightOperand) {
		}
		protected override HitRegionExpressionBase Clone(IHitRegion leftOperand, IHitRegion rightOperand) {
			return new HitRegionUnionExpression(leftOperand, rightOperand);
		}
		protected override void ApplyOperationToGDIRegion(Region leftRegion, Region rightRegion) {
			leftRegion.Union(rightRegion);
		}
		public override bool IsVisible(PointF hitPoint) {
			return LeftOperand.IsVisible(hitPoint) || RightOperand.IsVisible(hitPoint);
		}
		public override void Serialize(IHitRegionSerializer serializer) {
			serializer.SerializeUnionExpression(LeftOperand, RightOperand);
		}
	}
	public class HitRegionIntersectionExpression : HitRegionExpressionBase {
		public HitRegionIntersectionExpression(IHitRegion leftOperand, IHitRegion rightOperand) : base(leftOperand, rightOperand) {
		}
		protected override HitRegionExpressionBase Clone(IHitRegion leftOperand, IHitRegion rightOperand) {
			return new HitRegionIntersectionExpression(leftOperand, rightOperand);
		}
		protected override void ApplyOperationToGDIRegion(Region leftRegion, Region rightRegion) {
			leftRegion.Intersect(rightRegion);
		}
		public override bool IsVisible(PointF hitPoint) {
			return LeftOperand.IsVisible(hitPoint) && RightOperand.IsVisible(hitPoint);
		}
		public override void Serialize(IHitRegionSerializer serializer) {
			serializer.SerializeIntersectExpression(LeftOperand, RightOperand);
		}
	}
	public class HitRegionExclusionExpression : HitRegionExpressionBase {
		public HitRegionExclusionExpression(IHitRegion leftOperand, IHitRegion rightOperand) : base(leftOperand, rightOperand) {
		}
		protected override HitRegionExpressionBase Clone(IHitRegion leftOperand, IHitRegion rightOperand) {
			return new HitRegionExclusionExpression(leftOperand, rightOperand);
		}
		protected override void ApplyOperationToGDIRegion(Region leftRegion, Region rightRegion) {
			leftRegion.Exclude(rightRegion);
		}
		public override bool IsVisible(PointF hitPoint) {
			return LeftOperand.IsVisible(hitPoint) && !RightOperand.IsVisible(hitPoint);
		}
		public override void Serialize(IHitRegionSerializer serializer) {
			serializer.SerializeExcludeExpression(LeftOperand, RightOperand);
		}
	}
	public class HitRegionXorExpression : HitRegionExpressionBase {
		public HitRegionXorExpression(IHitRegion leftOperand, IHitRegion rightOperand) : base(leftOperand, rightOperand) {
		}
		protected override HitRegionExpressionBase Clone(IHitRegion leftOperand, IHitRegion rightOperand) {
			return new HitRegionXorExpression(leftOperand, rightOperand);
		}
		protected override void ApplyOperationToGDIRegion(Region leftRegion, Region rightRegion) {
			leftRegion.Xor(rightRegion);
		}
		public override bool IsVisible(PointF hitPoint) {
			return LeftOperand.IsVisible(hitPoint) ^ RightOperand.IsVisible(hitPoint);
		}
		public override void Serialize(IHitRegionSerializer serializer) {
			serializer.SerializeXorExpression(LeftOperand, RightOperand);
		}
	}
}

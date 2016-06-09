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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.XtraMap.Native;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap.Drawing {
	public enum TemplateGeometryType {
		Circle,
		Square,
		Diamond,
		Triangle,
		InvertedTriangle,
		Plus,
		Cross,
		Star5,
		Star6,
		Star8,
		Pentagon,
		Hexagon
	}
	public abstract class MapItemGeometryBase : MapDisposableObject, IMapItemGeometry {
		public MapRect Bounds { get; set; }
		public virtual IList<MapUnit> GetPoints() {
			return new List<MapUnit>();
		}
	}
	public class UnitGeometry : MapItemGeometryBase, IUnitGeometry {
		const UnitGeometryType DefaultType = UnitGeometryType.Areal;
		UnitGeometryType type = DefaultType;
		public MapUnit[] Points { get; set; }
		public UnitGeometryType Type { get { return type; } set { type = value; } }
		public override IList<MapUnit> GetPoints() {
			return new List<MapUnit>(Points);
		}
	}
	public class MultiLineUnitGeometry : UnitGeometry {
		public IList<MapUnit[]> Segments { get; set; }
		public override IList<MapUnit> GetPoints() {
			List<MapUnit> points = new List<MapUnit>();
			foreach(MapUnit[] segment in Segments)
				points.AddRange(segment);
			return points;
		}
	}
	public class PathUnitGeometry : UnitGeometry{
		public IList<MapUnit[]> InnerContours { get; set; }
		public override IList<MapUnit> GetPoints() {
			List<MapUnit> points = new List<MapUnit>();
			points.AddRange(Points);
			foreach(MapUnit[] contour in InnerContours)
				points.AddRange(contour);
			return points;
		}
	}
	public class ImageGeometry : MapItemGeometryBase, IImageGeometry {
		public const byte DefaultTransparency = 0;
		Image image;
		bool storingInPool = true;
		byte transparency = DefaultTransparency;
		bool isExternalImage = false;
		bool alignImage = false;
		ImageCachePriority cachePriority = ImageCachePriority.Normal;
		public Image Image {
			get { return image; }
		}
		public RectangleF ImageRect { get; set; }
		public RectangleF ClipRect { get; set; }
		public bool StoringInPool {
			get { return storingInPool; }
			set {
				if(value == storingInPool)
					return;
				storingInPool = value;
			}
		}
		public byte Transparency {
			get { return transparency; }
			set {
				if (transparency == value)
					return;
				transparency = value;
			}
		}
		public ImageCachePriority CachePriority {
			get { return cachePriority; }
			set {
				if (cachePriority == value)
					return;
				cachePriority = value;
			}
		}
		public bool AlignImage {
			get { return alignImage;  }
			set { alignImage = value; }
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			DisposeImage();
		}
		void DisposeImage() {
			if (this.image != null && !isExternalImage) {
				this.image.Dispose();
				this.image = null;
			}
		}
		public void SetImage(Image image, bool isExternal) {
			if (Object.Equals(this.image, image))
				return;
			DisposeImage();
			isExternalImage = isExternal;
			this.image = image;
		} 
	}
	public class GeometryBase : IGeometry {
		public IList<MapPoint[]> Points { get; set; }
	}
	public class PolylineGeometry : GeometryBase {
		public PolylineGeometry(IList<MapPoint[]> points) {
			Points = points;
		}
	}
	public class PolygonGeometry : PolylineGeometry {
		public PolygonGeometry(IList<MapPoint[]> points)
			: base(points) {
		}
	}
	[CLSCompliant(false)]
	public abstract class TemplateGeometryBase {
		protected const double Size = 1.0;
		MapUnit[] points;
		public MapUnit[] Points {
			get {
				if(points == null)
					points = CreateTemplate();
				return points;
			}
			protected set { points = value; }
		}
		protected abstract MapUnit[] CreateTemplate();
		public static TemplateGeometryBase CreateTemplate(TemplateGeometryType templateGeometryType) {
			switch(templateGeometryType) {
				case TemplateGeometryType.Circle:
					return CircleTemplateGeometry.Instance;
				case TemplateGeometryType.Square:
					return SquareTemplateGeometry.Instance;
				case TemplateGeometryType.Diamond:
					return DiamondTemplateGeometry.Instance;
				case TemplateGeometryType.Triangle:
					return TriangleTemplateGeometry.Instance;
				case TemplateGeometryType.InvertedTriangle:
					return InvertedTriangleTemplateGeometry.Instance;
				case TemplateGeometryType.Plus:
					return PlusTemplateGeometry.Instance;
				case TemplateGeometryType.Cross:
					return CrossTemplateGeometry.Instance;
				case TemplateGeometryType.Star5:
					return Star5TemplateGeometry.Instance;
				case TemplateGeometryType.Star6:
					return Star6TemplateGeometry.Instance;
				case TemplateGeometryType.Star8:
					return Star8TemplateGeometry.Instance;
				case TemplateGeometryType.Pentagon:
					return PentagonTemplateGeometry.Instance;
				case TemplateGeometryType.Hexagon:
					return HexagonTemplateGeometry.Instance;
			}
			return CircleTemplateGeometry.Instance;
		}
	}
	[CLSCompliant(false)]
	public class CircleTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new CircleTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			double radius = Size / 2.0;
			double step = Math.PI / 45.0;
			List<MapUnit> circle = new List<MapUnit>();
			for(double r = 0.0; r < 2.0 * Math.PI; r += step) {
				MapUnit p = new MapUnit(radius + radius * Math.Cos(r), radius + radius * Math.Sin(r));
				circle.Add(p);
			}
			return circle.ToArray();
		}
	}
	[CLSCompliant(false)]
	public class SquareTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new SquareTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return new MapUnit[]{ new MapUnit(0.0, 0.0),
								  new MapUnit(Size , 0.0),
								  new MapUnit(Size ,Size),
								  new MapUnit(0.0, Size)};
		}
	}
	[CLSCompliant(false)]
	public class DiamondTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new DiamondTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return MapUtils.CalcPolygon(MathUtils.Degree2Radian(-90), 4, new MapUnit(Size / 2, Size / 2), Size / 2);
		}
	}
	[CLSCompliant(false)]
	public class TriangleTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new TriangleTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return MapUtils.CalcPolygon(MathUtils.Degree2Radian(-90), 3, new MapUnit(Size / 2, Size * 2 / 3), Size * 2 / 3);
		}
	}
	[CLSCompliant(false)]
	public class InvertedTriangleTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new InvertedTriangleTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return MapUtils.RotateVectors(180, new MapUnit(Size / 2, Size / 2), TriangleTemplateGeometry.Instance.Points);
		}
	}
	[CLSCompliant(false)]
	public class PlusTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new PlusTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			const double widthRatio = 0.4;
			double semiSize = Size / 2.0;
			double semiWidth = semiSize * widthRatio;
			MapUnit[] points = new MapUnit[12];
			points[0] = new MapUnit(-semiWidth + semiSize, 0);
			points[1] = new MapUnit(semiWidth + semiSize, 0);
			points[2] = new MapUnit(semiWidth + semiSize, -semiWidth + semiSize);
			points[3] = new MapUnit(Size, -semiWidth + semiSize);
			points[4] = new MapUnit(Size, semiWidth + semiSize);
			points[5] = new MapUnit(semiWidth + semiSize, semiWidth + semiSize);
			points[6] = new MapUnit(semiWidth + semiSize, Size);
			points[7] = new MapUnit(-semiWidth + semiSize, Size);
			points[8] = new MapUnit(-semiWidth + semiSize, semiWidth + semiSize);
			points[9] = new MapUnit(0, semiWidth + semiSize);
			points[10] = new MapUnit(0, -semiWidth + semiSize);
			points[11] = new MapUnit(-semiWidth + semiSize, -semiWidth + semiSize);
			return points;
		}
	}
	[CLSCompliant(false)]
	public class CrossTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new CrossTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return MapUtils.RotateVectors(45.0, new MapUnit(Size / 2, Size / 2), PlusTemplateGeometry.Instance.Points);
		}
	}
	[CLSCompliant(false)]
	public class PentagonTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new PentagonTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return MapUtils.CalcPolygon(MathUtils.Degree2Radian(-90), 5, new MapUnit(Size / 2, Size * (1 - Math.Sqrt(5) / 5)), Size * (1 - Math.Sqrt(5) / 5));
		}
	}
	[CLSCompliant(false)]
	public class HexagonTemplateGeometry : TemplateGeometryBase {
		static readonly TemplateGeometryBase instance = new HexagonTemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
		protected override MapUnit[] CreateTemplate() {
			return MapUtils.CalcPolygon(MathUtils.Degree2Radian(-90), 6, new MapUnit(Size / 2, Size / 2), Size / 2);
		}
	}
	[CLSCompliant(false)]
	public abstract class StarGeometryBase : TemplateGeometryBase {
		protected abstract int StarPointCount { get; }
		protected override MapUnit[] CreateTemplate() {
			int pointCount = StarPointCount * 2;
			double extSize = Size / 2.0;
			double intSize = extSize * (StarPointCount <= 3 ? 0.25 : 0.43);
			MapUnit center = new MapUnit(Size / 2, Size / 2);
			MapUnit[] extPoints = MapUtils.CalcPolygon(MathUtils.Degree2Radian(-90), StarPointCount, center, extSize);
			MapUnit[] intPoints = MapUtils.CalcPolygon(MathUtils.Degree2Radian(-90) + Math.PI / StarPointCount, StarPointCount, center, intSize);
			MapUnit[] points = new MapUnit[pointCount];
			for(int i = 0; i < pointCount; i++)
				points[i] = i % 2 == 0 ? extPoints[i / 2] : intPoints[i / 2];
			return points;
		}
	}
	[CLSCompliant(false)]
	public class Star5TemplateGeometry : StarGeometryBase {
		protected override int StarPointCount { get { return 5; } }
		static readonly TemplateGeometryBase instance = new Star5TemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
	}
	[CLSCompliant(false)]
	public class Star6TemplateGeometry : StarGeometryBase {
		protected override int StarPointCount { get { return 6; } }
		static readonly TemplateGeometryBase instance = new Star6TemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
	}
	[CLSCompliant(false)]
	public class Star8TemplateGeometry : StarGeometryBase {
		protected override int StarPointCount { get { return 8; } }
		static readonly TemplateGeometryBase instance = new Star8TemplateGeometry();
		public static TemplateGeometryBase Instance { get { return instance; } }
	}
}

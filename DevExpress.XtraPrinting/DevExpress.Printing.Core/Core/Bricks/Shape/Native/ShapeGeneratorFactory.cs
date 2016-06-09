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

using DevExpress.XtraPrinting;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Localization;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Shape.Native {
	#region ShapeFactory
	public static class ShapeFactory {
		class SampleShapeDrawingInfo : IShapeDrawingInfo {
			public static readonly SampleShapeDrawingInfo Instance = new SampleShapeDrawingInfo();
			int angle;
			float IShapeDrawingInfo.LineWidth { get { return 1; } }
			DashStyle IShapeDrawingInfo.LineStyle { get { return DashStyle.Solid; } }
			public int Angle { get { return angle; } set { angle = value; } }
			bool IShapeDrawingInfo.Stretch { get { return false; } }
			Color IShapeDrawingInfo.FillColor { get { return Color.Transparent; } }
			Color IShapeDrawingInfo.ForeColor { get { return Color.Black; } }
			SampleShapeDrawingInfo() { }
		}
		static List<PreviewStringId> nameIds = new List<PreviewStringId>();
		static Dictionary<string, IShapeFactory> shapesHT = new Dictionary<string, IShapeFactory>();
		static Dictionary<string, Type> shapeTypesHT = new Dictionary<string, Type>();
		internal static readonly IShapeFactory DefaultFactory = new ShapePrototypeFactory(new DevExpress.XtraPrinting.Shape.ShapeEllipse());
		static ImageList sampleImages = new ImageList();
		public static PreviewStringId[] ShapeNamesIds {
			get { return nameIds.ToArray(); }
		}
		public static ImageList SampleImages {
			get { return sampleImages; }
		}
		static ShapeFactory() {
			sampleImages.ImageSize = new Size(16, 16);
			RegisterFactories();
		}
		public static ShapeBase Create(IShapeBaseOwner shapeOwner, string shapeInvariantName) {
			IShapeFactory factory = shapesHT[shapeInvariantName] as IShapeFactory;
			if(factory == null)
				factory = DefaultFactory;
			return factory.CreateShape(shapeOwner);
		}
		public static ShapeBase CreateByType(string shapeName) {
			return (ShapeBase)Activator.CreateInstance(shapeTypesHT[shapeName]);
		}
		public static ShapeBase CloneShape(ShapeBase shape) {
			return (ShapeBase)((ICloneable)shape).Clone();
		}
		static void RegisterFactories() {
			RegisterShapeFactory(new ShapePrototypeFactory(new DevExpress.XtraPrinting.Shape.ShapeRectangle()), PreviewStringId.Shapes_Rectangle);
			RegisterShapeFactory(DefaultFactory, PreviewStringId.Shapes_Ellipse);
			DevExpress.XtraPrinting.Shape.ShapeArrow arrowPrototype = new DevExpress.XtraPrinting.Shape.ShapeArrow();
			RegisterShapeFactory(new ShapePrototypeAngleFactory(arrowPrototype, ShapeHelper.TopAngle), PreviewStringId.Shapes_TopArrow);
			RegisterShapeFactory(new ShapePrototypeAngleFactory(arrowPrototype, ShapeHelper.RightAngle), PreviewStringId.Shapes_RightArrow);
			RegisterShapeFactory(new ShapePrototypeAngleFactory(arrowPrototype, ShapeHelper.BottomAngle), PreviewStringId.Shapes_BottomArrow);
			RegisterShapeFactory(new ShapePrototypeAngleFactory(arrowPrototype, ShapeHelper.LeftAngle), PreviewStringId.Shapes_LeftArrow);
			DevExpress.XtraPrinting.Shape.ShapePolygon polygonPrototype = new DevExpress.XtraPrinting.Shape.ShapePolygon();
			polygonPrototype.NumberOfSides = 3;
			RegisterShapeFactory(new ShapePrototypeFactory(polygonPrototype), PreviewStringId.Shapes_Triangle);
			polygonPrototype.NumberOfSides = 4;
			RegisterShapeFactory(new ShapePrototypeFactory(polygonPrototype), PreviewStringId.Shapes_Square);
			polygonPrototype.NumberOfSides = 5;
			RegisterShapeFactory(new ShapePrototypeFactory(polygonPrototype), PreviewStringId.Shapes_Pentagon);
			polygonPrototype.NumberOfSides = 6;
			RegisterShapeFactory(new ShapePrototypeFactory(polygonPrototype), PreviewStringId.Shapes_Hexagon);
			polygonPrototype.NumberOfSides = 8;
			RegisterShapeFactory(new ShapePrototypeFactory(polygonPrototype), PreviewStringId.Shapes_Octagon);
			DevExpress.XtraPrinting.Shape.ShapeStar starPrototype = new DevExpress.XtraPrinting.Shape.ShapeStar();
			starPrototype.StarPointCount = 3;
			RegisterShapeFactory(new ShapePrototypeFactory(starPrototype), PreviewStringId.Shapes_ThreePointStar);
			starPrototype.StarPointCount = 4;
			RegisterShapeFactory(new ShapePrototypeFactory(starPrototype), PreviewStringId.Shapes_FourPointStar);
			starPrototype.StarPointCount = 5;
			RegisterShapeFactory(new ShapePrototypeFactory(starPrototype), PreviewStringId.Shapes_FivePointStar);
			starPrototype.StarPointCount = 6;
			RegisterShapeFactory(new ShapePrototypeFactory(starPrototype), PreviewStringId.Shapes_SixPointStar);
			starPrototype.StarPointCount = 8;
			RegisterShapeFactory(new ShapePrototypeFactory(starPrototype), PreviewStringId.Shapes_EightPointStar);
			DevExpress.XtraPrinting.Shape.ShapeLine linePrototype = new DevExpress.XtraPrinting.Shape.ShapeLine();
			RegisterShapeFactory(new ShapePrototypeAngleFactory(linePrototype, 0), PreviewStringId.Shapes_VerticalLine);
			RegisterShapeFactory(new ShapePrototypeAngleFactory(linePrototype, ShapeHelper.RightAngle), PreviewStringId.Shapes_HorizontalLine);
			RegisterShapeFactory(new ShapePrototypeAngleFactory(linePrototype, ShapeHelper.RightAngle / 2), PreviewStringId.Shapes_SlantLine);
			RegisterShapeFactory(new ShapePrototypeAngleFactory(linePrototype, -ShapeHelper.RightAngle / 2), PreviewStringId.Shapes_BackslantLine);
			RegisterShapeFactory(new ShapePrototypeFactory(new DevExpress.XtraPrinting.Shape.ShapeCross()), PreviewStringId.Shapes_Cross);
			RegisterShapeFactory(new ShapePrototypeFactory(new DevExpress.XtraPrinting.Shape.ShapeBracket()), PreviewStringId.Shapes_Bracket);
			RegisterShapeFactory(new ShapePrototypeFactory(new DevExpress.XtraPrinting.Shape.ShapeBrace()), PreviewStringId.Shapes_Brace);
			RegisterShapeType(typeof(ShapeArrow), PreviewStringId.Shapes_Arrow);
			RegisterShapeType(typeof(ShapePolygon), PreviewStringId.Shapes_Polygon);
			RegisterShapeType(typeof(ShapeStar), PreviewStringId.Shapes_Star);
			RegisterShapeType(typeof(ShapeLine), PreviewStringId.Shapes_Line);
		}
		static void RegisterShapeType(Type shapeType, PreviewStringId shapeStringId) {
			string shapeName = ShapeHelper.GetInvariantName(shapeStringId);
			shapeTypesHT[shapeName] = shapeType;
		}
		static void RegisterShapeFactory(IShapeFactory factory, PreviewStringId shapeStringId) {
			string shapeName = ShapeHelper.GetInvariantName(shapeStringId);
			RegisterSampleBitmap(factory);
			nameIds.Add(shapeStringId);
			shapesHT[shapeName] = factory;
			RegisterShapeType(factory.ShapeType, shapeStringId);
		}
		static void RegisterSampleBitmap(IShapeFactory factory) {
			using(ShapeBrick shapeForSample = new ShapeBrick()) {
				ShapeBase shape = factory.CreateShape(shapeForSample);
				SampleShapeDrawingInfo.Instance.Angle = shapeForSample.Angle;
				Bitmap sampleBitmap = new Bitmap(sampleImages.ImageSize.Width, sampleImages.ImageSize.Height);
				using(PrintingSystemBase ps = new PrintingSystemBase()) {
					using(GdiGraphics gdiGraphics = new ImageGraphics(sampleBitmap, ps)) {
						SizeF clientSize = new SizeF(sampleImages.ImageSize.Width - 2, sampleImages.ImageSize.Height - 2);
						clientSize = GraphicsUnitConverter.PixelToDoc(clientSize);
						using(GdiHashtable gdi = new GdiHashtable()) {
							ShapeHelper.DrawShapeContent(shape, gdiGraphics, new RectangleF(new PointF(1, 1), clientSize), SampleShapeDrawingInfo.Instance, gdi);
						}
					}
				}
				sampleImages.Images.Add(sampleBitmap);
			}
		}
	}
	#endregion
	#region ShapeFactories
	public interface IShapeFactory {
		ShapeBase CreateShape(IShapeBaseOwner shapeOwner);
		Type ShapeType { get; }
	}
	public class ShapePrototypeFactory : IShapeFactory {
		ShapeBase prototype;
		Type IShapeFactory.ShapeType { get { return prototype.GetType(); } }
		public ShapePrototypeFactory(ShapeBase prototype) {
			this.prototype = ShapeFactory.CloneShape(prototype);
		}
		ShapeBase IShapeFactory.CreateShape(IShapeBaseOwner shapeOwner) {
			ConfigureShapeOwner(shapeOwner);
			return ShapeFactory.CloneShape(prototype);
		}
		protected virtual void ConfigureShapeOwner(IShapeBaseOwner shapeOwner) {
		}
	}
	public class ShapePrototypeAngleFactory : ShapePrototypeFactory {
		int angle;
		public ShapePrototypeAngleFactory(ShapeBase prototype, int angle)
			: base(prototype) {
			this.angle = angle;
		}
		protected override void ConfigureShapeOwner(IShapeBaseOwner shapeOwner) {
			shapeOwner.Angle = angle;
		}
	}
	#endregion
}

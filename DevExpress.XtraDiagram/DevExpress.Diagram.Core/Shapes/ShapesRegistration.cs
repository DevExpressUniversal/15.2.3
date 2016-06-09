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
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Shapes.Native;
using System.Collections.ObjectModel;
namespace DevExpress.Diagram.Core {
	public static partial class BasicShapes {
		static DiagramStencil Stencil { get { return DiagramToolboxRegistrator.GetStencil(StencilId); } }
		public const string StencilId = "BasicShapes";
		public static ShapeDescription Rectangle { get { return Stencil.GetShape("Rectangle"); } }
		public static ShapeDescription Ellipse { get { return Stencil.GetShape("Ellipse"); } }
		internal static ShapeDescription CreateEllipseShape() {
			return ShapeDescription.Create("Ellipse", DiagramControlStringId.BasicShapes_Ellipse_Name, 
				() => new Size(100,75), GetEllipsePoints, GetEllipseConnectionPoints, isQuick: true, styleId: GetStyleId("Ellipse"));
		}
		public static ShapeDescription Triangle { get { return Stencil.GetShape("Triangle"); } }
		internal static ShapeDescription CreateTriangleShape() {
			return ShapeDescription.Create("Triangle", DiagramControlStringId.BasicShapes_Triangle_Name, 
				() => new Size(100,90), GetTrianglePoints, GetTriangleConnectionPoints, GetTriangleParameters(), isQuick: true, styleId: GetStyleId("Triangle"));
		}
		public static ShapeDescription RightTriangle { get { return Stencil.GetShape("RightTriangle"); } }
		internal static ShapeDescription CreateRightTriangleShape() {
			return ShapeDescription.Create("RightTriangle", DiagramControlStringId.BasicShapes_RightTriangle_Name, 
				() => new Size(100,75), GetRightTrianglePoints, GetRightTriangleConnectionPoints, GetRightTriangleParameters(), isQuick: true, styleId: GetStyleId("RightTriangle"));
		}
		public static ShapeDescription Pentagon { get { return Stencil.GetShape("Pentagon"); } }
		internal static ShapeDescription CreatePentagonShape() {
			return ShapeDescription.Create("Pentagon", DiagramControlStringId.BasicShapes_Pentagon_Name, 
				() => new Size(100,100), GetPentagonPoints, GetPentagonConnectionPoints, isQuick: true, styleId: GetStyleId("Pentagon"));
		}
		public static ShapeDescription Hexagon { get { return Stencil.GetShape("Hexagon"); } }
		internal static ShapeDescription CreateHexagonShape() {
			return ShapeDescription.Create("Hexagon", DiagramControlStringId.BasicShapes_Hexagon_Name, 
				() => new Size(100,100), GetHexagonPoints, GetHexagonConnectionPoints, isQuick: true, styleId: GetStyleId("Hexagon"));
		}
		public static ShapeDescription Heptagon { get { return Stencil.GetShape("Heptagon"); } }
		internal static ShapeDescription CreateHeptagonShape() {
			return ShapeDescription.Create("Heptagon", DiagramControlStringId.BasicShapes_Heptagon_Name, 
				() => new Size(100,100), GetHeptagonPoints, GetHeptagonConnectionPoints, styleId: GetStyleId("Heptagon"));
		}
		public static ShapeDescription Octagon { get { return Stencil.GetShape("Octagon"); } }
		internal static ShapeDescription CreateOctagonShape() {
			return ShapeDescription.Create("Octagon", DiagramControlStringId.BasicShapes_Octagon_Name, 
				() => new Size(100,100), GetOctagonPoints, GetOctagonConnectionPoints, styleId: GetStyleId("Octagon"));
		}
		public static ShapeDescription Decagon { get { return Stencil.GetShape("Decagon"); } }
		internal static ShapeDescription CreateDecagonShape() {
			return ShapeDescription.Create("Decagon", DiagramControlStringId.BasicShapes_Decagon_Name, 
				() => new Size(100,100), GetDecagonPoints, GetDecagonConnectionPoints, styleId: GetStyleId("Decagon"));
		}
		public static ShapeDescription Can { get { return Stencil.GetShape("Can"); } }
		internal static ShapeDescription CreateCanShape() {
			return ShapeDescription.Create("Can", DiagramControlStringId.BasicShapes_Can_Name, 
				() => new Size(70,80), GetCanPoints, GetCanConnectionPoints, GetCanParameters(), styleId: GetStyleId("Can"));
		}
		public static ShapeDescription Parallelogram { get { return Stencil.GetShape("Parallelogram"); } }
		internal static ShapeDescription CreateParallelogramShape() {
			return ShapeDescription.Create("Parallelogram", DiagramControlStringId.BasicShapes_Parallelogram_Name, 
				() => new Size(100,75), GetParallelogramPoints, GetParallelogramConnectionPoints, GetParallelogramParameters(), styleId: GetStyleId("Parallelogram"));
		}
		public static ShapeDescription Trapezoid { get { return Stencil.GetShape("Trapezoid"); } }
		internal static ShapeDescription CreateTrapezoidShape() {
			return ShapeDescription.Create("Trapezoid", DiagramControlStringId.BasicShapes_Trapezoid_Name, 
				() => new Size(100,75), GetTrapezoidPoints, GetTrapezoidConnectionPoints, GetTrapezoidParameters(), styleId: GetStyleId("Trapezoid"));
		}
		public static ShapeDescription Diamond { get { return Stencil.GetShape("Diamond"); } }
		internal static ShapeDescription CreateDiamondShape() {
			return ShapeDescription.Create("Diamond", DiagramControlStringId.BasicShapes_Diamond_Name, 
				() => new Size(100,75), GetDiamondPoints, GetDiamondConnectionPoints, styleId: GetStyleId("Diamond"));
		}
		public static ShapeDescription Cross { get { return Stencil.GetShape("Cross"); } }
		public static ShapeDescription Chevron { get { return Stencil.GetShape("Chevron"); } }
		internal static ShapeDescription CreateChevronShape() {
			return ShapeDescription.Create("Chevron", DiagramControlStringId.BasicShapes_Chevron_Name, 
				() => new Size(80,80), GetChevronPoints, GetChevronConnectionPoints, GetChevronParameters(), styleId: GetStyleId("Chevron"));
		}
		public static ShapeDescription Cube { get { return Stencil.GetShape("Cube"); } }
		internal static ShapeDescription CreateCubeShape() {
			return ShapeDescription.Create("Cube", DiagramControlStringId.BasicShapes_Cube_Name, 
				() => new Size(100,75), GetCubePoints, GetCubeConnectionPoints, GetCubeParameters(), GetCubeEditorBounds, styleId: GetStyleId("Cube"));
		}
		public static ShapeDescription Star4 { get { return Stencil.GetShape("Star4"); } }
		internal static ShapeDescription CreateStar4Shape() {
			return ShapeDescription.Create("Star4", DiagramControlStringId.BasicShapes_Star4_Name, 
				() => new Size(100,100), GetStar4Points, GetStar4ConnectionPoints, GetStar4Parameters(), styleId: GetStyleId("Star4"));
		}
		public static ShapeDescription Star5 { get { return Stencil.GetShape("Star5"); } }
		internal static ShapeDescription CreateStar5Shape() {
			return ShapeDescription.Create("Star5", DiagramControlStringId.BasicShapes_Star5_Name, 
				() => new Size(100,100), GetStar5Points, GetStar5ConnectionPoints, GetStar5Parameters(), styleId: GetStyleId("Star5"));
		}
		public static ShapeDescription Star6 { get { return Stencil.GetShape("Star6"); } }
		internal static ShapeDescription CreateStar6Shape() {
			return ShapeDescription.Create("Star6", DiagramControlStringId.BasicShapes_Star6_Name, 
				() => new Size(100,100), GetStar6Points, GetStar6ConnectionPoints, GetStar6Parameters(), styleId: GetStyleId("Star6"));
		}
		public static ShapeDescription Star7 { get { return Stencil.GetShape("Star7"); } }
		internal static ShapeDescription CreateStar7Shape() {
			return ShapeDescription.Create("Star7", DiagramControlStringId.BasicShapes_Star7_Name, 
				() => new Size(100,100), GetStar7Points, GetStar7ConnectionPoints, GetStar7Parameters(), styleId: GetStyleId("Star7"));
		}
		public static ShapeDescription Star16 { get { return Stencil.GetShape("Star16"); } }
		internal static ShapeDescription CreateStar16Shape() {
			return ShapeDescription.Create("Star16", DiagramControlStringId.BasicShapes_Star16_Name, 
				() => new Size(100,100), GetStar16Points, GetStar16ConnectionPoints, GetStar16Parameters(), styleId: GetStyleId("Star16"));
		}
		public static ShapeDescription Star24 { get { return Stencil.GetShape("Star24"); } }
		internal static ShapeDescription CreateStar24Shape() {
			return ShapeDescription.Create("Star24", DiagramControlStringId.BasicShapes_Star24_Name, 
				() => new Size(100,100), GetStar24Points, GetStar24ConnectionPoints, GetStar24Parameters(), styleId: GetStyleId("Star24"));
		}
		public static ShapeDescription Star32 { get { return Stencil.GetShape("Star32"); } }
		internal static ShapeDescription CreateStar32Shape() {
			return ShapeDescription.Create("Star32", DiagramControlStringId.BasicShapes_Star32_Name, 
				() => new Size(100,100), GetStar32Points, GetStar32ConnectionPoints, GetStar32Parameters(), styleId: GetStyleId("Star32"));
		}
		public static ShapeDescription RoundedRectangle { get { return Stencil.GetShape("RoundedRectangle"); } }
		internal static ShapeDescription CreateRoundedRectangleShape() {
			return ShapeDescription.Create("RoundedRectangle", DiagramControlStringId.BasicShapes_RoundedRectangle_Name, 
				() => new Size(100,75), GetRoundedRectanglePoints, GetRoundedRectangleConnectionPoints, GetRoundedRectangleParameters(), styleId: GetStyleId("RoundedRectangle"));
		}
		public static ShapeDescription SingleSnipCornerRectangle { get { return Stencil.GetShape("SingleSnipCornerRectangle"); } }
		internal static ShapeDescription CreateSingleSnipCornerRectangleShape() {
			return ShapeDescription.Create("SingleSnipCornerRectangle", DiagramControlStringId.BasicShapes_SingleSnipCornerRectangle_Name, 
				() => new Size(100,75), GetSingleSnipCornerRectanglePoints, GetSingleSnipCornerRectangleConnectionPoints, GetSingleSnipCornerRectangleParameters(), styleId: GetStyleId("SingleSnipCornerRectangle"));
		}
		public static ShapeDescription SnipSameSideCornerRectangle { get { return Stencil.GetShape("SnipSameSideCornerRectangle"); } }
		internal static ShapeDescription CreateSnipSameSideCornerRectangleShape() {
			return ShapeDescription.Create("SnipSameSideCornerRectangle", DiagramControlStringId.BasicShapes_SnipSameSideCornerRectangle_Name, 
				() => new Size(100,75), GetSnipSameSideCornerRectanglePoints, GetSnipSameSideCornerRectangleConnectionPoints, GetSnipSameSideCornerRectangleParameters(), styleId: GetStyleId("SnipSameSideCornerRectangle"));
		}
		public static ShapeDescription SnipDiagonalCornerRectangle { get { return Stencil.GetShape("SnipDiagonalCornerRectangle"); } }
		internal static ShapeDescription CreateSnipDiagonalCornerRectangleShape() {
			return ShapeDescription.Create("SnipDiagonalCornerRectangle", DiagramControlStringId.BasicShapes_SnipDiagonalCornerRectangle_Name, 
				() => new Size(100,75), GetSnipDiagonalCornerRectanglePoints, GetSnipDiagonalCornerRectangleConnectionPoints, GetSnipDiagonalCornerRectangleParameters(), styleId: GetStyleId("SnipDiagonalCornerRectangle"));
		}
		public static ShapeDescription SingleRoundCornerRectangle { get { return Stencil.GetShape("SingleRoundCornerRectangle"); } }
		internal static ShapeDescription CreateSingleRoundCornerRectangleShape() {
			return ShapeDescription.Create("SingleRoundCornerRectangle", DiagramControlStringId.BasicShapes_SingleRoundCornerRectangle_Name, 
				() => new Size(100,75), GetSingleRoundCornerRectanglePoints, GetSingleRoundCornerRectangleConnectionPoints, GetSingleRoundCornerRectangleParameters(), styleId: GetStyleId("SingleRoundCornerRectangle"));
		}
		public static ShapeDescription RoundSameSideCornerRectangle { get { return Stencil.GetShape("RoundSameSideCornerRectangle"); } }
		internal static ShapeDescription CreateRoundSameSideCornerRectangleShape() {
			return ShapeDescription.Create("RoundSameSideCornerRectangle", DiagramControlStringId.BasicShapes_RoundSameSideCornerRectangle_Name, 
				() => new Size(100,75), GetRoundSameSideCornerRectanglePoints, GetRoundSameSideCornerRectangleConnectionPoints, GetRoundSameSideCornerRectangleParameters(), styleId: GetStyleId("RoundSameSideCornerRectangle"));
		}
		public static ShapeDescription RoundDiagonalCornerRectangle { get { return Stencil.GetShape("RoundDiagonalCornerRectangle"); } }
		internal static ShapeDescription CreateRoundDiagonalCornerRectangleShape() {
			return ShapeDescription.Create("RoundDiagonalCornerRectangle", DiagramControlStringId.BasicShapes_RoundDiagonalCornerRectangle_Name, 
				() => new Size(100,75), GetRoundDiagonalCornerRectanglePoints, GetRoundDiagonalCornerRectangleConnectionPoints, GetRoundDiagonalCornerRectangleParameters(), styleId: GetStyleId("RoundDiagonalCornerRectangle"));
		}
		public static ShapeDescription SnipAndRoundSingleCornerRectangle { get { return Stencil.GetShape("SnipAndRoundSingleCornerRectangle"); } }
		internal static ShapeDescription CreateSnipAndRoundSingleCornerRectangleShape() {
			return ShapeDescription.Create("SnipAndRoundSingleCornerRectangle", DiagramControlStringId.BasicShapes_SnipAndRoundSingleCornerRectangle_Name, 
				() => new Size(100,75), GetSnipAndRoundSingleCornerRectanglePoints, GetSnipAndRoundSingleCornerRectangleConnectionPoints, GetSnipAndRoundSingleCornerRectangleParameters(), styleId: GetStyleId("SnipAndRoundSingleCornerRectangle"));
		}
		public static ShapeDescription SnipCornerRectangle { get { return Stencil.GetShape("SnipCornerRectangle"); } }
		internal static ShapeDescription CreateSnipCornerRectangleShape() {
			return ShapeDescription.Create("SnipCornerRectangle", DiagramControlStringId.BasicShapes_SnipCornerRectangle_Name, 
				() => new Size(100,75), GetSnipCornerRectanglePoints, GetSnipCornerRectangleConnectionPoints, GetSnipCornerRectangleParameters(), styleId: GetStyleId("SnipCornerRectangle"));
		}
		public static ShapeDescription RoundCornerRectangle { get { return Stencil.GetShape("RoundCornerRectangle"); } }
		internal static ShapeDescription CreateRoundCornerRectangleShape() {
			return ShapeDescription.Create("RoundCornerRectangle", DiagramControlStringId.BasicShapes_RoundCornerRectangle_Name, 
				() => new Size(100,75), GetRoundCornerRectanglePoints, GetRoundCornerRectangleConnectionPoints, GetRoundCornerRectangleParameters(), styleId: GetStyleId("RoundCornerRectangle"));
		}
		public static ShapeDescription SnipAndRoundCornerRectangle { get { return Stencil.GetShape("SnipAndRoundCornerRectangle"); } }
		internal static ShapeDescription CreateSnipAndRoundCornerRectangleShape() {
			return ShapeDescription.Create("SnipAndRoundCornerRectangle", DiagramControlStringId.BasicShapes_SnipAndRoundCornerRectangle_Name, 
				() => new Size(100,75), GetSnipAndRoundCornerRectanglePoints, GetSnipAndRoundCornerRectangleConnectionPoints, GetSnipAndRoundCornerRectangleParameters(), styleId: GetStyleId("SnipAndRoundCornerRectangle"));
		}
		public static ShapeDescription Plaque { get { return Stencil.GetShape("Plaque"); } }
		internal static ShapeDescription CreatePlaqueShape() {
			return ShapeDescription.Create("Plaque", DiagramControlStringId.BasicShapes_Plaque_Name, 
				() => new Size(100,75), GetPlaquePoints, GetPlaqueConnectionPoints, GetPlaqueParameters(), styleId: GetStyleId("Plaque"));
		}
		public static ShapeDescription Frame { get { return Stencil.GetShape("Frame"); } }
		internal static ShapeDescription CreateFrameShape() {
			return ShapeDescription.Create("Frame", DiagramControlStringId.BasicShapes_Frame_Name, 
				() => new Size(100,75), GetFramePoints, GetFrameConnectionPoints, GetFrameParameters(), useBackgroundAsForeground: true, styleId: GetStyleId("Frame"));
		}
		public static ShapeDescription FrameCorner { get { return Stencil.GetShape("FrameCorner"); } }
		internal static ShapeDescription CreateFrameCornerShape() {
			return ShapeDescription.Create("FrameCorner", DiagramControlStringId.BasicShapes_FrameCorner_Name, 
				() => new Size(100,100), GetFrameCornerPoints, GetFrameCornerConnectionPoints, GetFrameCornerParameters(), GetFrameCornerEditorBounds, useBackgroundAsForeground: true, styleId: GetStyleId("FrameCorner"));
		}
		public static ShapeDescription LShape { get { return Stencil.GetShape("LShape"); } }
		internal static ShapeDescription CreateLShapeShape() {
			return ShapeDescription.Create("LShape", DiagramControlStringId.BasicShapes_LShape_Name, 
				() => new Size(100,100), GetLShapePoints, GetLShapeConnectionPoints, GetLShapeParameters(), GetLShapeEditorBounds, useBackgroundAsForeground: true, styleId: GetStyleId("LShape"));
		}
		public static ShapeDescription DiagonalStripe { get { return Stencil.GetShape("DiagonalStripe"); } }
		internal static ShapeDescription CreateDiagonalStripeShape() {
			return ShapeDescription.Create("DiagonalStripe", DiagramControlStringId.BasicShapes_DiagonalStripe_Name, 
				() => new Size(100,100), GetDiagonalStripePoints, GetDiagonalStripeConnectionPoints, GetDiagonalStripeParameters(), GetDiagonalStripeEditorBounds, useBackgroundAsForeground: true, styleId: GetStyleId("DiagonalStripe"));
		}
		public static ShapeDescription Donut { get { return Stencil.GetShape("Donut"); } }
		internal static ShapeDescription CreateDonutShape() {
			return ShapeDescription.Create("Donut", DiagramControlStringId.BasicShapes_Donut_Name, 
				() => new Size(100,100), GetDonutPoints, GetDonutConnectionPoints, GetDonutParameters(), GetDonutEditorBounds, useBackgroundAsForeground: true, styleId: GetStyleId("Donut"));
		}
		public static ShapeDescription NoSymbol { get { return Stencil.GetShape("NoSymbol"); } }
		internal static ShapeDescription CreateNoSymbolShape() {
			return ShapeDescription.Create("NoSymbol", DiagramControlStringId.BasicShapes_NoSymbol_Name, 
				() => new Size(100,100), GetNoSymbolPoints, GetNoSymbolConnectionPoints, GetNoSymbolParameters(), GetNoSymbolEditorBounds, useBackgroundAsForeground: true, styleId: GetStyleId("NoSymbol"));
		}
		public static ShapeDescription LeftParenthesis { get { return Stencil.GetShape("LeftParenthesis"); } }
		public static ShapeDescription RightParenthesis { get { return Stencil.GetShape("RightParenthesis"); } }
		public static ShapeDescription LeftBrace { get { return Stencil.GetShape("LeftBrace"); } }
		public static ShapeDescription RightBrace { get { return Stencil.GetShape("RightBrace"); } }
	}
	public static partial class BasicFlowchartShapes {
		static DiagramStencil Stencil { get { return DiagramToolboxRegistrator.GetStencil(StencilId); } }
		public const string StencilId = "BasicFlowchartShapes";
		public static ShapeDescription Process { get { return Stencil.GetShape("Process"); } }
		public static ShapeDescription Decision { get { return Stencil.GetShape("Decision"); } }
		public static ShapeDescription Subprocess { get { return Stencil.GetShape("Subprocess"); } }
		public static ShapeDescription StartEnd { get { return Stencil.GetShape("StartEnd"); } }
		internal static ShapeDescription CreateStartEndShape() {
			return ShapeDescription.Create("StartEnd", DiagramControlStringId.BasicFlowchartShapes_StartEnd_Name, 
				() => new Size(100,37.5), GetStartEndPoints, GetStartEndConnectionPoints, isQuick: true, styleId: GetStyleId("StartEnd"));
		}
		public static ShapeDescription Document { get { return Stencil.GetShape("Document"); } }
		public static ShapeDescription Data { get { return Stencil.GetShape("Data"); } }
		internal static ShapeDescription CreateDataShape() {
			return ShapeDescription.Create("Data", DiagramControlStringId.BasicFlowchartShapes_Data_Name, 
				() => new Size(100,75), GetDataPoints, GetDataConnectionPoints, isQuick: true, styleId: GetStyleId("Data"));
		}
		public static ShapeDescription Database { get { return Stencil.GetShape("Database"); } }
		internal static ShapeDescription CreateDatabaseShape() {
			return ShapeDescription.Create("Database", DiagramControlStringId.BasicFlowchartShapes_Database_Name, 
				() => new Size(100,75), GetDatabasePoints, GetDatabaseConnectionPoints, styleId: GetStyleId("Database"));
		}
		public static ShapeDescription ExternalData { get { return Stencil.GetShape("ExternalData"); } }
		internal static ShapeDescription CreateExternalDataShape() {
			return ShapeDescription.Create("ExternalData", DiagramControlStringId.BasicFlowchartShapes_ExternalData_Name, 
				() => new Size(100,75), GetExternalDataPoints, GetExternalDataConnectionPoints, styleId: GetStyleId("ExternalData"));
		}
		public static ShapeDescription Custom1 { get { return Stencil.GetShape("Custom1"); } }
		internal static ShapeDescription CreateCustom1Shape() {
			return ShapeDescription.Create("Custom1", DiagramControlStringId.BasicFlowchartShapes_Custom1_Name, 
				() => new Size(100,75), GetCustom1Points, GetCustom1ConnectionPoints, styleId: GetStyleId("Custom1"));
		}
		public static ShapeDescription Custom2 { get { return Stencil.GetShape("Custom2"); } }
		internal static ShapeDescription CreateCustom2Shape() {
			return ShapeDescription.Create("Custom2", DiagramControlStringId.BasicFlowchartShapes_Custom2_Name, 
				() => new Size(100,75), GetCustom2Points, GetCustom2ConnectionPoints, styleId: GetStyleId("Custom2"));
		}
		public static ShapeDescription Custom3 { get { return Stencil.GetShape("Custom3"); } }
		internal static ShapeDescription CreateCustom3Shape() {
			return ShapeDescription.Create("Custom3", DiagramControlStringId.BasicFlowchartShapes_Custom3_Name, 
				() => new Size(100,75), GetCustom3Points, GetCustom3ConnectionPoints, styleId: GetStyleId("Custom3"));
		}
		public static ShapeDescription Custom4 { get { return Stencil.GetShape("Custom4"); } }
		internal static ShapeDescription CreateCustom4Shape() {
			return ShapeDescription.Create("Custom4", DiagramControlStringId.BasicFlowchartShapes_Custom4_Name, 
				() => new Size(100,75), GetCustom4Points, GetCustom4ConnectionPoints, styleId: GetStyleId("Custom4"));
		}
		public static ShapeDescription OnPageReference { get { return Stencil.GetShape("OnPageReference"); } }
		public static ShapeDescription OffPageReference { get { return Stencil.GetShape("OffPageReference"); } }
	}
	public static partial class ArrowShapes {
		static DiagramStencil Stencil { get { return DiagramToolboxRegistrator.GetStencil(StencilId); } }
		public const string StencilId = "ArrowShapes";
		public static ShapeDescription SimpleArrow { get { return Stencil.GetShape("SimpleArrow"); } }
		public static ShapeDescription SimpleDoubleArrow { get { return Stencil.GetShape("SimpleDoubleArrow"); } }
		public static ShapeDescription ModernArrow { get { return Stencil.GetShape("ModernArrow"); } }
		public static ShapeDescription FlexibleArrow { get { return Stencil.GetShape("FlexibleArrow"); } }
		public static ShapeDescription BentArrow { get { return Stencil.GetShape("BentArrow"); } }
		public static ShapeDescription UTurnArrow { get { return Stencil.GetShape("UTurnArrow"); } }
		public static ShapeDescription SharpBentArrow { get { return Stencil.GetShape("SharpBentArrow"); } }
		public static ShapeDescription CurvedRightArrow { get { return Stencil.GetShape("CurvedRightArrow"); } }
		public static ShapeDescription CurvedLeftArrow { get { return Stencil.GetShape("CurvedLeftArrow"); } }
		public static ShapeDescription NotchedArrow { get { return Stencil.GetShape("NotchedArrow"); } }
		public static ShapeDescription StripedArrow { get { return Stencil.GetShape("StripedArrow"); } }
		public static ShapeDescription BlockArrow { get { return Stencil.GetShape("BlockArrow"); } }
		public static ShapeDescription CircularArrow { get { return Stencil.GetShape("CircularArrow"); } }
		public static ShapeDescription QuadArrow { get { return Stencil.GetShape("QuadArrow"); } }
		public static ShapeDescription LeftRightUpArrow { get { return Stencil.GetShape("LeftRightUpArrow"); } }
		public static ShapeDescription LeftRightArrowBlock { get { return Stencil.GetShape("LeftRightArrowBlock"); } }
		public static ShapeDescription QuadArrowBlock { get { return Stencil.GetShape("QuadArrowBlock"); } }
	}
	public static partial class SDLDiagramShapes {
		static DiagramStencil Stencil { get { return DiagramToolboxRegistrator.GetStencil(StencilId); } }
		public const string StencilId = "SDLDiagramShapes";
		public static ShapeDescription Start { get { return Stencil.GetShape("Start"); } }
		public static ShapeDescription VariableStart { get { return Stencil.GetShape("VariableStart"); } }
		public static ShapeDescription Procedure { get { return Stencil.GetShape("Procedure"); } }
		public static ShapeDescription VariableProcedure { get { return Stencil.GetShape("VariableProcedure"); } }
		public static ShapeDescription CreateRequest { get { return Stencil.GetShape("CreateRequest"); } }
		public static ShapeDescription Alternative { get { return Stencil.GetShape("Alternative"); } }
		public static ShapeDescription Document { get { return Stencil.GetShape("Document"); } }
		public static ShapeDescription Return { get { return Stencil.GetShape("Return"); } }
		public static ShapeDescription Decision1 { get { return Stencil.GetShape("Decision1"); } }
		public static ShapeDescription MessageFromUser { get { return Stencil.GetShape("MessageFromUser"); } }
		public static ShapeDescription PrimitiveFromCallControl { get { return Stencil.GetShape("PrimitiveFromCallControl"); } }
		public static ShapeDescription Decision2 { get { return Stencil.GetShape("Decision2"); } }
		public static ShapeDescription MessageToUser { get { return Stencil.GetShape("MessageToUser"); } }
		public static ShapeDescription PrimitiveToCallControl { get { return Stencil.GetShape("PrimitiveToCallControl"); } }
		public static ShapeDescription Save { get { return Stencil.GetShape("Save"); } }
		public static ShapeDescription OnPageReference { get { return Stencil.GetShape("OnPageReference"); } }
		public static ShapeDescription OffPageReference { get { return Stencil.GetShape("OffPageReference"); } }
		public static ShapeDescription DiskStorage { get { return Stencil.GetShape("DiskStorage"); } }
		public static ShapeDescription DividedProcess { get { return Stencil.GetShape("DividedProcess"); } }
		public static ShapeDescription DividedEvent { get { return Stencil.GetShape("DividedEvent"); } }
		public static ShapeDescription Terminator { get { return Stencil.GetShape("Terminator"); } }
	}
	public static partial class SoftwareIcons {
		static DiagramStencil Stencil { get { return DiagramToolboxRegistrator.GetStencil(StencilId); } }
		public const string StencilId = "SoftwareIcons";
		public static ShapeDescription Back { get { return Stencil.GetShape("Back"); } }
		public static ShapeDescription Forward { get { return Stencil.GetShape("Forward"); } }
		public static ShapeDescription Expand { get { return Stencil.GetShape("Expand"); } }
		public static ShapeDescription Collapse { get { return Stencil.GetShape("Collapse"); } }
		public static ShapeDescription Add { get { return Stencil.GetShape("Add"); } }
		public static ShapeDescription Remove { get { return Stencil.GetShape("Remove"); } }
		public static ShapeDescription ZoomIn { get { return Stencil.GetShape("ZoomIn"); } }
		public static ShapeDescription ZoomOut { get { return Stencil.GetShape("ZoomOut"); } }
		public static ShapeDescription Lock { get { return Stencil.GetShape("Lock"); } }
		public static ShapeDescription Permission { get { return Stencil.GetShape("Permission"); } }
		public static ShapeDescription Sort { get { return Stencil.GetShape("Sort"); } }
		public static ShapeDescription Filter { get { return Stencil.GetShape("Filter"); } }
		public static ShapeDescription Tools { get { return Stencil.GetShape("Tools"); } }
		public static ShapeDescription Properties { get { return Stencil.GetShape("Properties"); } }
		public static ShapeDescription Calendar { get { return Stencil.GetShape("Calendar"); } }
		public static ShapeDescription Document { get { return Stencil.GetShape("Document"); } }
		public static ShapeDescription Database { get { return Stencil.GetShape("Database"); } }
		public static ShapeDescription HardDrive { get { return Stencil.GetShape("HardDrive"); } }
		public static ShapeDescription Network { get { return Stencil.GetShape("Network"); } }
	}
	public static partial class DecorativeShapes {
		static DiagramStencil Stencil { get { return DiagramToolboxRegistrator.GetStencil(StencilId); } }
		public const string StencilId = "DecorativeShapes";
		public static ShapeDescription LightningBolt { get { return Stencil.GetShape("LightningBolt"); } }
		public static ShapeDescription Moon { get { return Stencil.GetShape("Moon"); } }
		public static ShapeDescription Wave { get { return Stencil.GetShape("Wave"); } }
		public static ShapeDescription DoubleWave { get { return Stencil.GetShape("DoubleWave"); } }
		public static ShapeDescription VerticalScroll { get { return Stencil.GetShape("VerticalScroll"); } }
		public static ShapeDescription HorizontalScroll { get { return Stencil.GetShape("HorizontalScroll"); } }
		public static ShapeDescription Heart { get { return Stencil.GetShape("Heart"); } }
		public static ShapeDescription DownRibbon { get { return Stencil.GetShape("DownRibbon"); } }
		public static ShapeDescription UpRibbon { get { return Stencil.GetShape("UpRibbon"); } }
		public static ShapeDescription Cloud { get { return Stencil.GetShape("Cloud"); } }
	}
	public static partial class ArrowDescriptions {
		public static ArrowDescription Open90 { get { return GetArrow("Open90"); } }
		public static ArrowDescription Filled90 { get { return GetArrow("Filled90"); } }
		public static ArrowDescription ClosedDot { get { return GetArrow("ClosedDot"); } }
		public static ArrowDescription FilledDot { get { return GetArrow("FilledDot"); } }
		public static ArrowDescription OpenFletch { get { return GetArrow("OpenFletch"); } }
		public static ArrowDescription FilledFletch { get { return GetArrow("FilledFletch"); } }
		public static ArrowDescription Diamond { get { return GetArrow("Diamond"); } }
		public static ArrowDescription FilledDiamond { get { return GetArrow("FilledDiamond"); } }
		public static ArrowDescription ClosedDiamond { get { return GetArrow("ClosedDiamond"); } }
		public static ArrowDescription IndentedFilledArrow { get { return GetArrow("IndentedFilledArrow"); } }
		public static ArrowDescription OutdentedFilledArrow { get { return GetArrow("OutdentedFilledArrow"); } }
		public static ArrowDescription FilledSquare { get { return GetArrow("FilledSquare"); } }
		public static ArrowDescription ClosedASMEArrow { get { return GetArrow("ClosedASMEArrow"); } }
		public static ArrowDescription FilledDoubleArrow { get { return GetArrow("FilledDoubleArrow"); } }
		public static ArrowDescription ClosedDoubleArrow { get { return GetArrow("ClosedDoubleArrow"); } }
	}
	internal static class ShapeRegistratorHelper {
		readonly static Dictionary<string, DiagramControlStringId> arrowStringId;
		readonly static Dictionary<string, DiagramControlStringId> categoryStringId;
		readonly static Dictionary<string, Dictionary<string, DiagramControlStringId>> shapeStringId;
		readonly static Dictionary<string, Dictionary<string, Func<ShapeDescription>>> codedShapeFactory;
		readonly static Dictionary<string, Type> shapeOwners;
		static ShapeRegistratorHelper() {
			arrowStringId = new Dictionary<string, DiagramControlStringId>();
			categoryStringId = new Dictionary<string, DiagramControlStringId>();
			shapeStringId = new Dictionary<string, Dictionary<string, DiagramControlStringId>>();
			codedShapeFactory = new Dictionary<string, Dictionary<string, Func<ShapeDescription>>>();
			shapeOwners = new Dictionary<string, Type>();
			PopulateStringIdTable();
			PopulateArrowStringIdTable();
			PopulateShapeFactory();
			RegisterCategories();
		}
		static void PopulateStringIdTable() {
				categoryStringId.Add("BasicShapes", DiagramControlStringId.BasicShapes_Name);
				shapeStringId.Add("BasicShapes", new Dictionary<string, DiagramControlStringId>());
				shapeStringId["BasicShapes"].Add("Rectangle", DiagramControlStringId.BasicShapes_Rectangle_Name);
				shapeStringId["BasicShapes"].Add("Ellipse", DiagramControlStringId.BasicShapes_Ellipse_Name);
				shapeStringId["BasicShapes"].Add("Triangle", DiagramControlStringId.BasicShapes_Triangle_Name);
				shapeStringId["BasicShapes"].Add("RightTriangle", DiagramControlStringId.BasicShapes_RightTriangle_Name);
				shapeStringId["BasicShapes"].Add("Pentagon", DiagramControlStringId.BasicShapes_Pentagon_Name);
				shapeStringId["BasicShapes"].Add("Hexagon", DiagramControlStringId.BasicShapes_Hexagon_Name);
				shapeStringId["BasicShapes"].Add("Heptagon", DiagramControlStringId.BasicShapes_Heptagon_Name);
				shapeStringId["BasicShapes"].Add("Octagon", DiagramControlStringId.BasicShapes_Octagon_Name);
				shapeStringId["BasicShapes"].Add("Decagon", DiagramControlStringId.BasicShapes_Decagon_Name);
				shapeStringId["BasicShapes"].Add("Can", DiagramControlStringId.BasicShapes_Can_Name);
				shapeStringId["BasicShapes"].Add("Parallelogram", DiagramControlStringId.BasicShapes_Parallelogram_Name);
				shapeStringId["BasicShapes"].Add("Trapezoid", DiagramControlStringId.BasicShapes_Trapezoid_Name);
				shapeStringId["BasicShapes"].Add("Diamond", DiagramControlStringId.BasicShapes_Diamond_Name);
				shapeStringId["BasicShapes"].Add("Cross", DiagramControlStringId.BasicShapes_Cross_Name);
				shapeStringId["BasicShapes"].Add("Chevron", DiagramControlStringId.BasicShapes_Chevron_Name);
				shapeStringId["BasicShapes"].Add("Cube", DiagramControlStringId.BasicShapes_Cube_Name);
				shapeStringId["BasicShapes"].Add("Star4", DiagramControlStringId.BasicShapes_Star4_Name);
				shapeStringId["BasicShapes"].Add("Star5", DiagramControlStringId.BasicShapes_Star5_Name);
				shapeStringId["BasicShapes"].Add("Star6", DiagramControlStringId.BasicShapes_Star6_Name);
				shapeStringId["BasicShapes"].Add("Star7", DiagramControlStringId.BasicShapes_Star7_Name);
				shapeStringId["BasicShapes"].Add("Star16", DiagramControlStringId.BasicShapes_Star16_Name);
				shapeStringId["BasicShapes"].Add("Star24", DiagramControlStringId.BasicShapes_Star24_Name);
				shapeStringId["BasicShapes"].Add("Star32", DiagramControlStringId.BasicShapes_Star32_Name);
				shapeStringId["BasicShapes"].Add("RoundedRectangle", DiagramControlStringId.BasicShapes_RoundedRectangle_Name);
				shapeStringId["BasicShapes"].Add("SingleSnipCornerRectangle", DiagramControlStringId.BasicShapes_SingleSnipCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("SnipSameSideCornerRectangle", DiagramControlStringId.BasicShapes_SnipSameSideCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("SnipDiagonalCornerRectangle", DiagramControlStringId.BasicShapes_SnipDiagonalCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("SingleRoundCornerRectangle", DiagramControlStringId.BasicShapes_SingleRoundCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("RoundSameSideCornerRectangle", DiagramControlStringId.BasicShapes_RoundSameSideCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("RoundDiagonalCornerRectangle", DiagramControlStringId.BasicShapes_RoundDiagonalCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("SnipAndRoundSingleCornerRectangle", DiagramControlStringId.BasicShapes_SnipAndRoundSingleCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("SnipCornerRectangle", DiagramControlStringId.BasicShapes_SnipCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("RoundCornerRectangle", DiagramControlStringId.BasicShapes_RoundCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("SnipAndRoundCornerRectangle", DiagramControlStringId.BasicShapes_SnipAndRoundCornerRectangle_Name);
				shapeStringId["BasicShapes"].Add("Plaque", DiagramControlStringId.BasicShapes_Plaque_Name);
				shapeStringId["BasicShapes"].Add("Frame", DiagramControlStringId.BasicShapes_Frame_Name);
				shapeStringId["BasicShapes"].Add("FrameCorner", DiagramControlStringId.BasicShapes_FrameCorner_Name);
				shapeStringId["BasicShapes"].Add("LShape", DiagramControlStringId.BasicShapes_LShape_Name);
				shapeStringId["BasicShapes"].Add("DiagonalStripe", DiagramControlStringId.BasicShapes_DiagonalStripe_Name);
				shapeStringId["BasicShapes"].Add("Donut", DiagramControlStringId.BasicShapes_Donut_Name);
				shapeStringId["BasicShapes"].Add("NoSymbol", DiagramControlStringId.BasicShapes_NoSymbol_Name);
				shapeStringId["BasicShapes"].Add("LeftParenthesis", DiagramControlStringId.BasicShapes_LeftParenthesis_Name);
				shapeStringId["BasicShapes"].Add("RightParenthesis", DiagramControlStringId.BasicShapes_RightParenthesis_Name);
				shapeStringId["BasicShapes"].Add("LeftBrace", DiagramControlStringId.BasicShapes_LeftBrace_Name);
				shapeStringId["BasicShapes"].Add("RightBrace", DiagramControlStringId.BasicShapes_RightBrace_Name);
				categoryStringId.Add("BasicFlowchartShapes", DiagramControlStringId.BasicFlowchartShapes_Name);
				shapeStringId.Add("BasicFlowchartShapes", new Dictionary<string, DiagramControlStringId>());
				shapeStringId["BasicFlowchartShapes"].Add("Process", DiagramControlStringId.BasicFlowchartShapes_Process_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Decision", DiagramControlStringId.BasicFlowchartShapes_Decision_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Subprocess", DiagramControlStringId.BasicFlowchartShapes_Subprocess_Name);
				shapeStringId["BasicFlowchartShapes"].Add("StartEnd", DiagramControlStringId.BasicFlowchartShapes_StartEnd_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Document", DiagramControlStringId.BasicFlowchartShapes_Document_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Data", DiagramControlStringId.BasicFlowchartShapes_Data_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Database", DiagramControlStringId.BasicFlowchartShapes_Database_Name);
				shapeStringId["BasicFlowchartShapes"].Add("ExternalData", DiagramControlStringId.BasicFlowchartShapes_ExternalData_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Custom1", DiagramControlStringId.BasicFlowchartShapes_Custom1_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Custom2", DiagramControlStringId.BasicFlowchartShapes_Custom2_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Custom3", DiagramControlStringId.BasicFlowchartShapes_Custom3_Name);
				shapeStringId["BasicFlowchartShapes"].Add("Custom4", DiagramControlStringId.BasicFlowchartShapes_Custom4_Name);
				shapeStringId["BasicFlowchartShapes"].Add("OnPageReference", DiagramControlStringId.BasicFlowchartShapes_OnPageReference_Name);
				shapeStringId["BasicFlowchartShapes"].Add("OffPageReference", DiagramControlStringId.BasicFlowchartShapes_OffPageReference_Name);
				categoryStringId.Add("ArrowShapes", DiagramControlStringId.ArrowShapes_Name);
				shapeStringId.Add("ArrowShapes", new Dictionary<string, DiagramControlStringId>());
				shapeStringId["ArrowShapes"].Add("SimpleArrow", DiagramControlStringId.ArrowShapes_SimpleArrow_Name);
				shapeStringId["ArrowShapes"].Add("SimpleDoubleArrow", DiagramControlStringId.ArrowShapes_SimpleDoubleArrow_Name);
				shapeStringId["ArrowShapes"].Add("ModernArrow", DiagramControlStringId.ArrowShapes_ModernArrow_Name);
				shapeStringId["ArrowShapes"].Add("FlexibleArrow", DiagramControlStringId.ArrowShapes_FlexibleArrow_Name);
				shapeStringId["ArrowShapes"].Add("BentArrow", DiagramControlStringId.ArrowShapes_BentArrow_Name);
				shapeStringId["ArrowShapes"].Add("UTurnArrow", DiagramControlStringId.ArrowShapes_UTurnArrow_Name);
				shapeStringId["ArrowShapes"].Add("SharpBentArrow", DiagramControlStringId.ArrowShapes_SharpBentArrow_Name);
				shapeStringId["ArrowShapes"].Add("CurvedRightArrow", DiagramControlStringId.ArrowShapes_CurvedRightArrow_Name);
				shapeStringId["ArrowShapes"].Add("CurvedLeftArrow", DiagramControlStringId.ArrowShapes_CurvedLeftArrow_Name);
				shapeStringId["ArrowShapes"].Add("NotchedArrow", DiagramControlStringId.ArrowShapes_NotchedArrow_Name);
				shapeStringId["ArrowShapes"].Add("StripedArrow", DiagramControlStringId.ArrowShapes_StripedArrow_Name);
				shapeStringId["ArrowShapes"].Add("BlockArrow", DiagramControlStringId.ArrowShapes_BlockArrow_Name);
				shapeStringId["ArrowShapes"].Add("CircularArrow", DiagramControlStringId.ArrowShapes_CircularArrow_Name);
				shapeStringId["ArrowShapes"].Add("QuadArrow", DiagramControlStringId.ArrowShapes_QuadArrow_Name);
				shapeStringId["ArrowShapes"].Add("LeftRightUpArrow", DiagramControlStringId.ArrowShapes_LeftRightUpArrow_Name);
				shapeStringId["ArrowShapes"].Add("LeftRightArrowBlock", DiagramControlStringId.ArrowShapes_LeftRightArrowBlock_Name);
				shapeStringId["ArrowShapes"].Add("QuadArrowBlock", DiagramControlStringId.ArrowShapes_QuadArrowBlock_Name);
				categoryStringId.Add("SDLDiagramShapes", DiagramControlStringId.SDLDiagramShapes_Name);
				shapeStringId.Add("SDLDiagramShapes", new Dictionary<string, DiagramControlStringId>());
				shapeStringId["SDLDiagramShapes"].Add("Start", DiagramControlStringId.SDLDiagramShapes_Start_Name);
				shapeStringId["SDLDiagramShapes"].Add("VariableStart", DiagramControlStringId.SDLDiagramShapes_VariableStart_Name);
				shapeStringId["SDLDiagramShapes"].Add("Procedure", DiagramControlStringId.SDLDiagramShapes_Procedure_Name);
				shapeStringId["SDLDiagramShapes"].Add("VariableProcedure", DiagramControlStringId.SDLDiagramShapes_VariableProcedure_Name);
				shapeStringId["SDLDiagramShapes"].Add("CreateRequest", DiagramControlStringId.SDLDiagramShapes_CreateRequest_Name);
				shapeStringId["SDLDiagramShapes"].Add("Alternative", DiagramControlStringId.SDLDiagramShapes_Alternative_Name);
				shapeStringId["SDLDiagramShapes"].Add("Document", DiagramControlStringId.SDLDiagramShapes_Document_Name);
				shapeStringId["SDLDiagramShapes"].Add("Return", DiagramControlStringId.SDLDiagramShapes_Return_Name);
				shapeStringId["SDLDiagramShapes"].Add("Decision1", DiagramControlStringId.SDLDiagramShapes_Decision1_Name);
				shapeStringId["SDLDiagramShapes"].Add("MessageFromUser", DiagramControlStringId.SDLDiagramShapes_MessageFromUser_Name);
				shapeStringId["SDLDiagramShapes"].Add("PrimitiveFromCallControl", DiagramControlStringId.SDLDiagramShapes_PrimitiveFromCallControl_Name);
				shapeStringId["SDLDiagramShapes"].Add("Decision2", DiagramControlStringId.SDLDiagramShapes_Decision2_Name);
				shapeStringId["SDLDiagramShapes"].Add("MessageToUser", DiagramControlStringId.SDLDiagramShapes_MessageToUser_Name);
				shapeStringId["SDLDiagramShapes"].Add("PrimitiveToCallControl", DiagramControlStringId.SDLDiagramShapes_PrimitiveToCallControl_Name);
				shapeStringId["SDLDiagramShapes"].Add("Save", DiagramControlStringId.SDLDiagramShapes_Save_Name);
				shapeStringId["SDLDiagramShapes"].Add("OnPageReference", DiagramControlStringId.SDLDiagramShapes_OnPageReference_Name);
				shapeStringId["SDLDiagramShapes"].Add("OffPageReference", DiagramControlStringId.SDLDiagramShapes_OffPageReference_Name);
				shapeStringId["SDLDiagramShapes"].Add("DiskStorage", DiagramControlStringId.SDLDiagramShapes_DiskStorage_Name);
				shapeStringId["SDLDiagramShapes"].Add("DividedProcess", DiagramControlStringId.SDLDiagramShapes_DividedProcess_Name);
				shapeStringId["SDLDiagramShapes"].Add("DividedEvent", DiagramControlStringId.SDLDiagramShapes_DividedEvent_Name);
				shapeStringId["SDLDiagramShapes"].Add("Terminator", DiagramControlStringId.SDLDiagramShapes_Terminator_Name);
				categoryStringId.Add("SoftwareIcons", DiagramControlStringId.SoftwareIcons_Name);
				shapeStringId.Add("SoftwareIcons", new Dictionary<string, DiagramControlStringId>());
				shapeStringId["SoftwareIcons"].Add("Back", DiagramControlStringId.SoftwareIcons_Back_Name);
				shapeStringId["SoftwareIcons"].Add("Forward", DiagramControlStringId.SoftwareIcons_Forward_Name);
				shapeStringId["SoftwareIcons"].Add("Expand", DiagramControlStringId.SoftwareIcons_Expand_Name);
				shapeStringId["SoftwareIcons"].Add("Collapse", DiagramControlStringId.SoftwareIcons_Collapse_Name);
				shapeStringId["SoftwareIcons"].Add("Add", DiagramControlStringId.SoftwareIcons_Add_Name);
				shapeStringId["SoftwareIcons"].Add("Remove", DiagramControlStringId.SoftwareIcons_Remove_Name);
				shapeStringId["SoftwareIcons"].Add("ZoomIn", DiagramControlStringId.SoftwareIcons_ZoomIn_Name);
				shapeStringId["SoftwareIcons"].Add("ZoomOut", DiagramControlStringId.SoftwareIcons_ZoomOut_Name);
				shapeStringId["SoftwareIcons"].Add("Lock", DiagramControlStringId.SoftwareIcons_Lock_Name);
				shapeStringId["SoftwareIcons"].Add("Permission", DiagramControlStringId.SoftwareIcons_Permission_Name);
				shapeStringId["SoftwareIcons"].Add("Sort", DiagramControlStringId.SoftwareIcons_Sort_Name);
				shapeStringId["SoftwareIcons"].Add("Filter", DiagramControlStringId.SoftwareIcons_Filter_Name);
				shapeStringId["SoftwareIcons"].Add("Tools", DiagramControlStringId.SoftwareIcons_Tools_Name);
				shapeStringId["SoftwareIcons"].Add("Properties", DiagramControlStringId.SoftwareIcons_Properties_Name);
				shapeStringId["SoftwareIcons"].Add("Calendar", DiagramControlStringId.SoftwareIcons_Calendar_Name);
				shapeStringId["SoftwareIcons"].Add("Document", DiagramControlStringId.SoftwareIcons_Document_Name);
				shapeStringId["SoftwareIcons"].Add("Database", DiagramControlStringId.SoftwareIcons_Database_Name);
				shapeStringId["SoftwareIcons"].Add("HardDrive", DiagramControlStringId.SoftwareIcons_HardDrive_Name);
				shapeStringId["SoftwareIcons"].Add("Network", DiagramControlStringId.SoftwareIcons_Network_Name);
				categoryStringId.Add("DecorativeShapes", DiagramControlStringId.DecorativeShapes_Name);
				shapeStringId.Add("DecorativeShapes", new Dictionary<string, DiagramControlStringId>());
				shapeStringId["DecorativeShapes"].Add("LightningBolt", DiagramControlStringId.DecorativeShapes_LightningBolt_Name);
				shapeStringId["DecorativeShapes"].Add("Moon", DiagramControlStringId.DecorativeShapes_Moon_Name);
				shapeStringId["DecorativeShapes"].Add("Wave", DiagramControlStringId.DecorativeShapes_Wave_Name);
				shapeStringId["DecorativeShapes"].Add("DoubleWave", DiagramControlStringId.DecorativeShapes_DoubleWave_Name);
				shapeStringId["DecorativeShapes"].Add("VerticalScroll", DiagramControlStringId.DecorativeShapes_VerticalScroll_Name);
				shapeStringId["DecorativeShapes"].Add("HorizontalScroll", DiagramControlStringId.DecorativeShapes_HorizontalScroll_Name);
				shapeStringId["DecorativeShapes"].Add("Heart", DiagramControlStringId.DecorativeShapes_Heart_Name);
				shapeStringId["DecorativeShapes"].Add("DownRibbon", DiagramControlStringId.DecorativeShapes_DownRibbon_Name);
				shapeStringId["DecorativeShapes"].Add("UpRibbon", DiagramControlStringId.DecorativeShapes_UpRibbon_Name);
				shapeStringId["DecorativeShapes"].Add("Cloud", DiagramControlStringId.DecorativeShapes_Cloud_Name);
		}
		static void PopulateArrowStringIdTable() {
				arrowStringId.Add("Open90", DiagramControlStringId.Arrow_Open90);
				arrowStringId.Add("Filled90", DiagramControlStringId.Arrow_Filled90);
				arrowStringId.Add("ClosedDot", DiagramControlStringId.Arrow_ClosedDot);
				arrowStringId.Add("FilledDot", DiagramControlStringId.Arrow_FilledDot);
				arrowStringId.Add("OpenFletch", DiagramControlStringId.Arrow_OpenFletch);
				arrowStringId.Add("FilledFletch", DiagramControlStringId.Arrow_FilledFletch);
				arrowStringId.Add("Diamond", DiagramControlStringId.Arrow_Diamond);
				arrowStringId.Add("FilledDiamond", DiagramControlStringId.Arrow_FilledDiamond);
				arrowStringId.Add("ClosedDiamond", DiagramControlStringId.Arrow_ClosedDiamond);
				arrowStringId.Add("IndentedFilledArrow", DiagramControlStringId.Arrow_IndentedFilledArrow);
				arrowStringId.Add("OutdentedFilledArrow", DiagramControlStringId.Arrow_OutdentedFilledArrow);
				arrowStringId.Add("FilledSquare", DiagramControlStringId.Arrow_FilledSquare);
				arrowStringId.Add("ClosedASMEArrow", DiagramControlStringId.Arrow_ClosedASMEArrow);
				arrowStringId.Add("FilledDoubleArrow", DiagramControlStringId.Arrow_FilledDoubleArrow);
				arrowStringId.Add("ClosedDoubleArrow", DiagramControlStringId.Arrow_ClosedDoubleArrow);
		}
		static void PopulateShapeFactory() {
				codedShapeFactory.Add("BasicShapes", new Dictionary<string, Func<ShapeDescription>>());
				codedShapeFactory["BasicShapes"].Add("Ellipse", BasicShapes.CreateEllipseShape);
				codedShapeFactory["BasicShapes"].Add("Triangle", BasicShapes.CreateTriangleShape);
				codedShapeFactory["BasicShapes"].Add("RightTriangle", BasicShapes.CreateRightTriangleShape);
				codedShapeFactory["BasicShapes"].Add("Pentagon", BasicShapes.CreatePentagonShape);
				codedShapeFactory["BasicShapes"].Add("Hexagon", BasicShapes.CreateHexagonShape);
				codedShapeFactory["BasicShapes"].Add("Heptagon", BasicShapes.CreateHeptagonShape);
				codedShapeFactory["BasicShapes"].Add("Octagon", BasicShapes.CreateOctagonShape);
				codedShapeFactory["BasicShapes"].Add("Decagon", BasicShapes.CreateDecagonShape);
				codedShapeFactory["BasicShapes"].Add("Can", BasicShapes.CreateCanShape);
				codedShapeFactory["BasicShapes"].Add("Parallelogram", BasicShapes.CreateParallelogramShape);
				codedShapeFactory["BasicShapes"].Add("Trapezoid", BasicShapes.CreateTrapezoidShape);
				codedShapeFactory["BasicShapes"].Add("Diamond", BasicShapes.CreateDiamondShape);
				codedShapeFactory["BasicShapes"].Add("Chevron", BasicShapes.CreateChevronShape);
				codedShapeFactory["BasicShapes"].Add("Cube", BasicShapes.CreateCubeShape);
				codedShapeFactory["BasicShapes"].Add("Star4", BasicShapes.CreateStar4Shape);
				codedShapeFactory["BasicShapes"].Add("Star5", BasicShapes.CreateStar5Shape);
				codedShapeFactory["BasicShapes"].Add("Star6", BasicShapes.CreateStar6Shape);
				codedShapeFactory["BasicShapes"].Add("Star7", BasicShapes.CreateStar7Shape);
				codedShapeFactory["BasicShapes"].Add("Star16", BasicShapes.CreateStar16Shape);
				codedShapeFactory["BasicShapes"].Add("Star24", BasicShapes.CreateStar24Shape);
				codedShapeFactory["BasicShapes"].Add("Star32", BasicShapes.CreateStar32Shape);
				codedShapeFactory["BasicShapes"].Add("RoundedRectangle", BasicShapes.CreateRoundedRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SingleSnipCornerRectangle", BasicShapes.CreateSingleSnipCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SnipSameSideCornerRectangle", BasicShapes.CreateSnipSameSideCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SnipDiagonalCornerRectangle", BasicShapes.CreateSnipDiagonalCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SingleRoundCornerRectangle", BasicShapes.CreateSingleRoundCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("RoundSameSideCornerRectangle", BasicShapes.CreateRoundSameSideCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("RoundDiagonalCornerRectangle", BasicShapes.CreateRoundDiagonalCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SnipAndRoundSingleCornerRectangle", BasicShapes.CreateSnipAndRoundSingleCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SnipCornerRectangle", BasicShapes.CreateSnipCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("RoundCornerRectangle", BasicShapes.CreateRoundCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("SnipAndRoundCornerRectangle", BasicShapes.CreateSnipAndRoundCornerRectangleShape);
				codedShapeFactory["BasicShapes"].Add("Plaque", BasicShapes.CreatePlaqueShape);
				codedShapeFactory["BasicShapes"].Add("Frame", BasicShapes.CreateFrameShape);
				codedShapeFactory["BasicShapes"].Add("FrameCorner", BasicShapes.CreateFrameCornerShape);
				codedShapeFactory["BasicShapes"].Add("LShape", BasicShapes.CreateLShapeShape);
				codedShapeFactory["BasicShapes"].Add("DiagonalStripe", BasicShapes.CreateDiagonalStripeShape);
				codedShapeFactory["BasicShapes"].Add("Donut", BasicShapes.CreateDonutShape);
				codedShapeFactory["BasicShapes"].Add("NoSymbol", BasicShapes.CreateNoSymbolShape);
				codedShapeFactory.Add("BasicFlowchartShapes", new Dictionary<string, Func<ShapeDescription>>());
				codedShapeFactory["BasicFlowchartShapes"].Add("StartEnd", BasicFlowchartShapes.CreateStartEndShape);
				codedShapeFactory["BasicFlowchartShapes"].Add("Data", BasicFlowchartShapes.CreateDataShape);
				codedShapeFactory["BasicFlowchartShapes"].Add("Database", BasicFlowchartShapes.CreateDatabaseShape);
				codedShapeFactory["BasicFlowchartShapes"].Add("ExternalData", BasicFlowchartShapes.CreateExternalDataShape);
				codedShapeFactory["BasicFlowchartShapes"].Add("Custom1", BasicFlowchartShapes.CreateCustom1Shape);
				codedShapeFactory["BasicFlowchartShapes"].Add("Custom2", BasicFlowchartShapes.CreateCustom2Shape);
				codedShapeFactory["BasicFlowchartShapes"].Add("Custom3", BasicFlowchartShapes.CreateCustom3Shape);
				codedShapeFactory["BasicFlowchartShapes"].Add("Custom4", BasicFlowchartShapes.CreateCustom4Shape);
				codedShapeFactory.Add("ArrowShapes", new Dictionary<string, Func<ShapeDescription>>());
				codedShapeFactory.Add("SDLDiagramShapes", new Dictionary<string, Func<ShapeDescription>>());
				codedShapeFactory.Add("SoftwareIcons", new Dictionary<string, Func<ShapeDescription>>());
				codedShapeFactory.Add("DecorativeShapes", new Dictionary<string, Func<ShapeDescription>>());
		}
		internal static DiagramControlStringId GetCategoryStringId(string categoryId) {
			return categoryStringId[categoryId];
		}
		internal static DiagramControlStringId GetShapeStringId(string categoryId, string shapeId) {
			return shapeStringId[categoryId][shapeId];
		}
		internal static ShapeDescription CreateShape(string categoryId, string shapeId) {
			return codedShapeFactory[categoryId][shapeId]();
		}
		internal static bool IsTemplateShape(string categoryId, string shapeId) {
			Dictionary<string, Func<ShapeDescription>> categoryFactory;
			if(codedShapeFactory.TryGetValue(categoryId, out categoryFactory))
				return !codedShapeFactory[categoryId].ContainsKey(shapeId);
			return true;
		}
		internal static DiagramControlStringId GetArrowStringId(string arrowId) {
			return arrowStringId[arrowId];
		}
		internal static Type GetShapeOwner(string categoryId) {
			if(!shapeOwners.ContainsKey(categoryId)) {
				throw new ArgumentException(categoryId);
			}
			return shapeOwners[categoryId];
		}
		static void RegisterCategories() {
				RegisterShapeOwner("BasicShapes", typeof(BasicShapes));
				RegisterShapeOwner("BasicFlowchartShapes", typeof(BasicFlowchartShapes));
				RegisterShapeOwner("ArrowShapes", typeof(ArrowShapes));
				RegisterShapeOwner("SDLDiagramShapes", typeof(SDLDiagramShapes));
				RegisterShapeOwner("SoftwareIcons", typeof(SoftwareIcons));
				RegisterShapeOwner("DecorativeShapes", typeof(DecorativeShapes));
		}
		static void RegisterShapeOwner(string categoryId, Type ownerType) {
			shapeOwners[categoryId] = ownerType;
		}
	}
}

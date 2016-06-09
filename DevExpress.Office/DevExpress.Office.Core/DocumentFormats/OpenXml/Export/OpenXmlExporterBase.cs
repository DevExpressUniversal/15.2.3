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
using DevExpress.Office.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Utils;
namespace DevExpress.Office.OpenXml.Export {
	public abstract class OpenXmlExporterBase {
		#region TileFlipTypeTable
		public static readonly Dictionary<TileFlipType, string> TileFlipTypeTable = CreateTileFlipTypeTable();
		static Dictionary<TileFlipType, string> CreateTileFlipTypeTable() {
			Dictionary<TileFlipType, string> result = new Dictionary<TileFlipType, string>();
			result.Add(TileFlipType.None, "none");
			result.Add(TileFlipType.Horizontal, "x");
			result.Add(TileFlipType.Vertical, "y");
			result.Add(TileFlipType.Both, "xy");
			return result;
		}
		#endregion
		#region GradientTypeTable
		public static readonly Dictionary<GradientType, string> GradientTypeTable = CreateGradientTypeTable();
		static Dictionary<GradientType, string> CreateGradientTypeTable() {
			Dictionary<GradientType, string> result = new Dictionary<GradientType, string>();
			result.Add(GradientType.Circle, "circle");
			result.Add(GradientType.Rectangle, "rect");
			result.Add(GradientType.Shape, "shape");
			return result;
		}
		#endregion
		#region DrawingPatternTypeTable
		public static readonly Dictionary<DrawingPatternType, string> DrawingPatternTypeTable = CreateDrawingPatternTypeTable();
		static Dictionary<DrawingPatternType, string> CreateDrawingPatternTypeTable() {
			Dictionary<DrawingPatternType, string> result = new Dictionary<DrawingPatternType, string>();
			result.Add(DrawingPatternType.Cross, "cross");
			result.Add(DrawingPatternType.DashedDownwardDiagonal, "dashDnDiag");
			result.Add(DrawingPatternType.DashedHorizontal, "dashHorz");
			result.Add(DrawingPatternType.DashedUpwardDiagonal, "dashUpDiag");
			result.Add(DrawingPatternType.DashedVertical, "dashVert");
			result.Add(DrawingPatternType.DiagonalBrick, "diagBrick");
			result.Add(DrawingPatternType.DiagonalCross, "diagCross");
			result.Add(DrawingPatternType.Divot, "divot");
			result.Add(DrawingPatternType.DarkDownwardDiagonal, "dkDnDiag");
			result.Add(DrawingPatternType.DarkHorizontal, "dkHorz");
			result.Add(DrawingPatternType.DarkUpwardDiagonal, "dkUpDiag");
			result.Add(DrawingPatternType.DarkVertical, "dkVert");
			result.Add(DrawingPatternType.DownwardDiagonal, "dnDiag");
			result.Add(DrawingPatternType.DottedDiamond, "dotDmnd");
			result.Add(DrawingPatternType.DottedGrid, "dotGrid");
			result.Add(DrawingPatternType.Horizontal, "horz");
			result.Add(DrawingPatternType.HorizontalBrick, "horzBrick");
			result.Add(DrawingPatternType.LargeCheckerBoard, "lgCheck");
			result.Add(DrawingPatternType.LargeConfetti, "lgConfetti");
			result.Add(DrawingPatternType.LargeGrid, "lgGrid");
			result.Add(DrawingPatternType.LightDownwardDiagonal, "ltDnDiag");
			result.Add(DrawingPatternType.LightHorizontal, "ltHorz");
			result.Add(DrawingPatternType.LightUpwardDiagonal, "ltUpDiag");
			result.Add(DrawingPatternType.LightVertical, "ltVert");
			result.Add(DrawingPatternType.NarrowHorizontal, "narHorz");
			result.Add(DrawingPatternType.NarrowVertical, "narVert");
			result.Add(DrawingPatternType.OpenDiamond, "openDmnd");
			result.Add(DrawingPatternType.Percent10, "pct10");
			result.Add(DrawingPatternType.Percent20, "pct20");
			result.Add(DrawingPatternType.Percent25, "pct25");
			result.Add(DrawingPatternType.Percent30, "pct30");
			result.Add(DrawingPatternType.Percent40, "pct40");
			result.Add(DrawingPatternType.Percent5, "pct5");
			result.Add(DrawingPatternType.Percent50, "pct50");
			result.Add(DrawingPatternType.Percent60, "pct60");
			result.Add(DrawingPatternType.Percent70, "pct70");
			result.Add(DrawingPatternType.Percent75, "pct75");
			result.Add(DrawingPatternType.Percent80, "pct80");
			result.Add(DrawingPatternType.Percent90, "pct90");
			result.Add(DrawingPatternType.Plaid, "plaid");
			result.Add(DrawingPatternType.Shingle, "shingle");
			result.Add(DrawingPatternType.SmallCheckerBoard, "smCheck");
			result.Add(DrawingPatternType.SmallConfetti, "smConfetti");
			result.Add(DrawingPatternType.SmallGrid, "smGrid");
			result.Add(DrawingPatternType.SolidDiamond, "solidDmnd");
			result.Add(DrawingPatternType.Sphere, "sphere");
			result.Add(DrawingPatternType.Trellis, "trellis");
			result.Add(DrawingPatternType.UpwardDiagonal, "upDiag");
			result.Add(DrawingPatternType.Vertical, "vert");
			result.Add(DrawingPatternType.Wave, "wave");
			result.Add(DrawingPatternType.WideDownwardDiagonal, "wdDnDiag");
			result.Add(DrawingPatternType.WideUpwardDiagonal, "wdUpDiag");
			result.Add(DrawingPatternType.Weave, "weave");
			result.Add(DrawingPatternType.ZigZag, "zigZag");
			return result;
		}
		#endregion
		#region RectangleAlignTypeTable
		public static readonly Dictionary<RectangleAlignType, string> RectangleAlignTypeTable = CreateRectangleAlignTypeTable();
		static Dictionary<RectangleAlignType, string> CreateRectangleAlignTypeTable() {
			Dictionary<RectangleAlignType, string> result = new Dictionary<RectangleAlignType, string>();
			result.Add(RectangleAlignType.TopLeft, "tl");
			result.Add(RectangleAlignType.Top, "t");
			result.Add(RectangleAlignType.TopRight, "tr");
			result.Add(RectangleAlignType.Left, "l");
			result.Add(RectangleAlignType.Center, "ctr");
			result.Add(RectangleAlignType.Right, "r");
			result.Add(RectangleAlignType.BottomLeft, "bl");
			result.Add(RectangleAlignType.Bottom, "b");
			result.Add(RectangleAlignType.BottomRight, "br");
			return result;
		}
		#endregion
		#region CompressionStateTable
		public static Dictionary<CompressionState, string> CompressionStateTable = CreateCompressionStateTable();
		static Dictionary<CompressionState, string> CreateCompressionStateTable() {
			Dictionary<CompressionState, string> result = new Dictionary<CompressionState, string>();
			result.Add(CompressionState.Email, "email");
			result.Add(CompressionState.HighQualityPrinting, "hqprint");
			result.Add(CompressionState.None, "none");
			result.Add(CompressionState.Print, "print");
			result.Add(CompressionState.Screen, "screen");
			return result;
		}
		#endregion
		#region Export effects
		#region BlendModeTable
		public static Dictionary<BlendMode, string> BlendModeTable = GetBlendModeTable();
		static Dictionary<BlendMode, string> GetBlendModeTable() {
			Dictionary<BlendMode, string> result = new Dictionary<BlendMode, string>();
			result.Add(BlendMode.Darken, "darken");
			result.Add(BlendMode.Lighten, "lighten");
			result.Add(BlendMode.Multiply, "mult");
			result.Add(BlendMode.Overlay, "over");
			result.Add(BlendMode.Screen, "screen");
			return result;
		}
		#endregion
		#region PresetShadowTypeTable
		public static readonly Dictionary<PresetShadowType, string> PresetShadowTypeTable = CreatePresetShadowTypeTable();
		static Dictionary<PresetShadowType, string> CreatePresetShadowTypeTable() {
			Dictionary<PresetShadowType, string> result = new Dictionary<PresetShadowType, string>();
			result.Add(PresetShadowType.TopLeftDrop, "shdw1");
			result.Add(PresetShadowType.TopRightDrop, "shdw2");
			result.Add(PresetShadowType.BackLeftPerspective, "shdw3");
			result.Add(PresetShadowType.BackRightPerspective, "shdw4");
			result.Add(PresetShadowType.BottomLeftDrop, "shdw5");
			result.Add(PresetShadowType.BottomRightDrop, "shdw6");
			result.Add(PresetShadowType.FrontLeftPerspective, "shdw7");
			result.Add(PresetShadowType.FrontRightPerspective, "shdw8");
			result.Add(PresetShadowType.TopLeftSmallDrop, "shdw9");
			result.Add(PresetShadowType.TopLeftLargeDrop, "shdw10");
			result.Add(PresetShadowType.BackLeftLongPerspective, "shdw11");
			result.Add(PresetShadowType.BackRightLongPerspective, "shdw12");
			result.Add(PresetShadowType.TopLeftDoubleDrop, "shdw13");
			result.Add(PresetShadowType.BottomRightSmallDrop, "shdw14");
			result.Add(PresetShadowType.FrontLeftLongPerspective, "shdw15");
			result.Add(PresetShadowType.FrontRightLongPerspective, "shdw16");
			result.Add(PresetShadowType.OuterBox3d, "shdw17");
			result.Add(PresetShadowType.InnerBox3d, "shdw18");
			result.Add(PresetShadowType.BackCenterPerspective, "shdw19");
			result.Add(PresetShadowType.FrontBottomShadow, "shdw20");
			return result;
		}
		#endregion
		#region DrawingEffectContainerTypeTable
		public static Dictionary<DrawingEffectContainerType, string> DrawingEffectContainerTypeTable = GetDrawingEffectContainerTypeTable();
		static Dictionary<DrawingEffectContainerType, string> GetDrawingEffectContainerTypeTable() {
			Dictionary<DrawingEffectContainerType, string> result = new Dictionary<DrawingEffectContainerType, string>();
			result.Add(DrawingEffectContainerType.Sibling, "sib");
			result.Add(DrawingEffectContainerType.Tree, "tree");
			return result;
		}
		#endregion
		#endregion
		#region StrokeAlignmentTable
		public static Dictionary<OutlineStrokeAlignment, string> StrokeAlignmentTable = CreateStrokeAlignmentTable();
		static Dictionary<OutlineStrokeAlignment, string> CreateStrokeAlignmentTable() {
			Dictionary<OutlineStrokeAlignment, string> result = new Dictionary<OutlineStrokeAlignment, string>();
			result.Add(OutlineStrokeAlignment.None, "none");
			result.Add(OutlineStrokeAlignment.Center, "ctr");
			result.Add(OutlineStrokeAlignment.Inset, "in");
			return result;
		}
		#endregion
		#region EndCapStyleTable
		public static Dictionary<OutlineEndCapStyle, string> EndCapStyleTable = CreateEndCapStyleTable();
		static Dictionary<OutlineEndCapStyle, string> CreateEndCapStyleTable() {
			Dictionary<OutlineEndCapStyle, string> result = new Dictionary<OutlineEndCapStyle, string>();
			result.Add(OutlineEndCapStyle.Flat, "flat");
			result.Add(OutlineEndCapStyle.Round, "rnd");
			result.Add(OutlineEndCapStyle.Square, "sq");
			return result;
		}
		#endregion
		#region CompoundTypeTable
		public static Dictionary<OutlineCompoundType, string> CompoundTypeTable = CreateCompoundTypeTable();
		static Dictionary<OutlineCompoundType, string> CreateCompoundTypeTable() {
			Dictionary<OutlineCompoundType, string> result = new Dictionary<OutlineCompoundType, string>();
			result.Add(OutlineCompoundType.Single, "sng");
			result.Add(OutlineCompoundType.Double, "dbl");
			result.Add(OutlineCompoundType.ThickThin, "thickThin");
			result.Add(OutlineCompoundType.ThinThick, "thinThick");
			result.Add(OutlineCompoundType.Triple, "tri");
			return result;
		}
		#endregion
		#region PresetDashTable
		public static Dictionary<OutlineDashing, string> PresetDashTable = CreatePresetDashTable();
		static Dictionary<OutlineDashing, string> CreatePresetDashTable() {
			Dictionary<OutlineDashing, string> result = new Dictionary<OutlineDashing, string>();
			result.Add(OutlineDashing.Solid, "solid");
			result.Add(OutlineDashing.Dash, "dash");
			result.Add(OutlineDashing.DashDot, "dashDot");
			result.Add(OutlineDashing.Dot, "dot");
			result.Add(OutlineDashing.LongDash, "lgDash");
			result.Add(OutlineDashing.LongDashDot, "lgDashDot");
			result.Add(OutlineDashing.LongDashDotDot, "lgDashDotDot");
			result.Add(OutlineDashing.SystemDash, "sysDash");
			result.Add(OutlineDashing.SystemDashDot, "sysDashDot");
			result.Add(OutlineDashing.SystemDashDotDot, "sysDashDotDot");
			result.Add(OutlineDashing.SystemDot, "sysDot");
			return result;
		}
		#endregion
		#region HeadTailSizeTable
		public static Dictionary<OutlineHeadTailSize, string> HeadTailSizeTable = CreateHeadTailSizeTable();
		static Dictionary<OutlineHeadTailSize, string> CreateHeadTailSizeTable() {
			Dictionary<OutlineHeadTailSize, string> result = new Dictionary<OutlineHeadTailSize, string>();
			result.Add(OutlineHeadTailSize.Large, "lg");
			result.Add(OutlineHeadTailSize.Medium, "med");
			result.Add(OutlineHeadTailSize.Small, "sm");
			return result;
		}
		#endregion
		#region HeadTailTypeTable
		public static Dictionary<OutlineHeadTailType, string> HeadTailTypeTable = CreateHeadTailTypeTable();
		static Dictionary<OutlineHeadTailType, string> CreateHeadTailTypeTable() {
			Dictionary<OutlineHeadTailType, string> result = new Dictionary<OutlineHeadTailType, string>();
			result.Add(OutlineHeadTailType.None, "none");
			result.Add(OutlineHeadTailType.Arrow, "arrow");
			result.Add(OutlineHeadTailType.Diamond, "diamond");
			result.Add(OutlineHeadTailType.Oval, "oval");
			result.Add(OutlineHeadTailType.StealthArrow, "stealth");
			result.Add(OutlineHeadTailType.TriangleArrow, "triangle");
			return result;
		}
		#endregion
		#region PresetCameraTypeTable
		public static Dictionary<PresetCameraType, string> PresetCameraTypeTable = CreatePresetCameraTypeTable();
		static Dictionary<PresetCameraType, string> CreatePresetCameraTypeTable() {
			Dictionary<PresetCameraType, string> result = new Dictionary<PresetCameraType, string>();
			result.Add(PresetCameraType.None, "none");
			result.Add(PresetCameraType.LegacyObliqueTopLeft, "legacyObliqueTopLeft");
			result.Add(PresetCameraType.LegacyObliqueTop, "legacyObliqueTop");
			result.Add(PresetCameraType.LegacyObliqueTopRight, "legacyObliqueTopRight");
			result.Add(PresetCameraType.LegacyObliqueLeft, "legacyObliqueLeft");
			result.Add(PresetCameraType.LegacyObliqueFront, "legacyObliqueFront");
			result.Add(PresetCameraType.LegacyObliqueRight, "legacyObliqueRight");
			result.Add(PresetCameraType.LegacyObliqueBottomLeft, "legacyObliqueBottomLeft");
			result.Add(PresetCameraType.LegacyObliqueBottom, "legacyObliqueBottom");
			result.Add(PresetCameraType.LegacyObliqueBottomRight, "legacyObliqueBottomRight");
			result.Add(PresetCameraType.LegacyPerspectiveTopLeft, "legacyPerspectiveTopLeft");
			result.Add(PresetCameraType.LegacyPerspectiveTop, "legacyPerspectiveTop");
			result.Add(PresetCameraType.LegacyPerspectiveTopRight, "legacyPerspectiveTopRight");
			result.Add(PresetCameraType.LegacyPerspectiveLeft, "legacyPerspectiveLeft");
			result.Add(PresetCameraType.LegacyPerspectiveFront, "legacyPerspectiveFront");
			result.Add(PresetCameraType.LegacyPerspectiveRight, "legacyPerspectiveRight");
			result.Add(PresetCameraType.LegacyPerspectiveBottomLeft, "legacyPerspectiveBottomLeft");
			result.Add(PresetCameraType.LegacyPerspectiveBottom, "legacyPerspectiveBottom");
			result.Add(PresetCameraType.LegacyPerspectiveBottomRight, "legacyPerspectiveBottomRight");
			result.Add(PresetCameraType.OrthographicFront, "orthographicFront");
			result.Add(PresetCameraType.IsometricTopUp, "isometricTopUp");
			result.Add(PresetCameraType.IsometricTopDown, "isometricTopDown");
			result.Add(PresetCameraType.IsometricBottomUp, "isometricBottomUp");
			result.Add(PresetCameraType.IsometricBottomDown, "isometricBottomDown");
			result.Add(PresetCameraType.IsometricLeftUp, "isometricLeftUp");
			result.Add(PresetCameraType.IsometricLeftDown, "isometricLeftDown");
			result.Add(PresetCameraType.IsometricRightUp, "isometricRightUp");
			result.Add(PresetCameraType.IsometricRightDown, "isometricRightDown");
			result.Add(PresetCameraType.IsometricOffAxis1Left, "isometricOffAxis1Left");
			result.Add(PresetCameraType.IsometricOffAxis1Right, "isometricOffAxis1Right");
			result.Add(PresetCameraType.IsometricOffAxis1Top, "isometricOffAxis1Top");
			result.Add(PresetCameraType.IsometricOffAxis2Left, "isometricOffAxis2Left");
			result.Add(PresetCameraType.IsometricOffAxis2Right, "isometricOffAxis2Right");
			result.Add(PresetCameraType.IsometricOffAxis2Top, "isometricOffAxis2Top");
			result.Add(PresetCameraType.IsometricOffAxis3Left, "isometricOffAxis3Left");
			result.Add(PresetCameraType.IsometricOffAxis3Right, "isometricOffAxis3Right");
			result.Add(PresetCameraType.IsometricOffAxis3Bottom, "isometricOffAxis3Bottom");
			result.Add(PresetCameraType.IsometricOffAxis4Left, "isometricOffAxis4Left");
			result.Add(PresetCameraType.IsometricOffAxis4Right, "isometricOffAxis4Right");
			result.Add(PresetCameraType.IsometricOffAxis4Bottom, "isometricOffAxis4Bottom");
			result.Add(PresetCameraType.ObliqueTopLeft, "obliqueTopLeft");
			result.Add(PresetCameraType.ObliqueTop, "obliqueTop");
			result.Add(PresetCameraType.ObliqueTopRight, "obliqueTopRight");
			result.Add(PresetCameraType.ObliqueLeft, "obliqueLeft");
			result.Add(PresetCameraType.ObliqueRight, "obliqueRight");
			result.Add(PresetCameraType.ObliqueBottomLeft, "obliqueBottomLeft");
			result.Add(PresetCameraType.ObliqueBottom, "obliqueBottom");
			result.Add(PresetCameraType.ObliqueBottomRight, "obliqueBottomRight");
			result.Add(PresetCameraType.PerspectiveFront, "perspectiveFront");
			result.Add(PresetCameraType.PerspectiveLeft, "perspectiveLeft");
			result.Add(PresetCameraType.PerspectiveRight, "perspectiveRight");
			result.Add(PresetCameraType.PerspectiveAbove, "perspectiveAbove");
			result.Add(PresetCameraType.PerspectiveBelow, "perspectiveBelow");
			result.Add(PresetCameraType.PerspectiveAboveLeftFacing, "perspectiveAboveLeftFacing");
			result.Add(PresetCameraType.PerspectiveAboveRightFacing, "perspectiveAboveRightFacing");
			result.Add(PresetCameraType.PerspectiveContrastingLeftFacing, "perspectiveContrastingLeftFacing");
			result.Add(PresetCameraType.PerspectiveContrastingRightFacing, "perspectiveContrastingRightFacing");
			result.Add(PresetCameraType.PerspectiveHeroicLeftFacing, "perspectiveHeroicLeftFacing");
			result.Add(PresetCameraType.PerspectiveHeroicRightFacing, "perspectiveHeroicRightFacing");
			result.Add(PresetCameraType.PerspectiveHeroicExtremeLeftFacing, "perspectiveHeroicExtremeLeftFacing");
			result.Add(PresetCameraType.PerspectiveHeroicExtremeRightFacing, "perspectiveHeroicExtremeRightFacing");
			result.Add(PresetCameraType.PerspectiveRelaxed, "perspectiveRelaxed");
			result.Add(PresetCameraType.PerspectiveRelaxedModerately, "perspectiveRelaxedModerately");
			return result;
		}
		#endregion
		#region LightRigDirectionTable
		public static Dictionary<LightRigDirection, string> LightRigDirectionTable = CreateLightRigDirectionTable();
		static Dictionary<LightRigDirection, string> CreateLightRigDirectionTable() {
			Dictionary<LightRigDirection, string> result = new Dictionary<LightRigDirection, string>();
			result.Add(LightRigDirection.None, "none");
			result.Add(LightRigDirection.Bottom, "b");
			result.Add(LightRigDirection.BottomLeft, "bl");
			result.Add(LightRigDirection.BottomRight, "br");
			result.Add(LightRigDirection.Left, "l");
			result.Add(LightRigDirection.Right, "r");
			result.Add(LightRigDirection.Top, "t");
			result.Add(LightRigDirection.TopLeft, "tl");
			result.Add(LightRigDirection.TopRight, "tr");
			return result;
		}
		#endregion
		#region LightRigPresetTable
		public static Dictionary<LightRigPreset, string> LightRigPresetTable = CreateLightRigPresetTable();
		static Dictionary<LightRigPreset, string> CreateLightRigPresetTable() {
			Dictionary<LightRigPreset, string> result = new Dictionary<LightRigPreset, string>();
			result.Add(LightRigPreset.None, "none");
			result.Add(LightRigPreset.LegacyFlat1, "legacyFlat1");
			result.Add(LightRigPreset.LegacyFlat2, "legacyFlat2");
			result.Add(LightRigPreset.LegacyFlat3, "legacyFlat3");
			result.Add(LightRigPreset.LegacyFlat4, "legacyFlat4");
			result.Add(LightRigPreset.LegacyNormal1, "legacyNormal1");
			result.Add(LightRigPreset.LegacyNormal2, "legacyNormal2");
			result.Add(LightRigPreset.LegacyNormal3, "legacyNormal3");
			result.Add(LightRigPreset.LegacyNormal4, "legacyNormal4");
			result.Add(LightRigPreset.LegacyHarsh1, "legacyHarsh1");
			result.Add(LightRigPreset.LegacyHarsh2, "legacyHarsh2");
			result.Add(LightRigPreset.LegacyHarsh3, "legacyHarsh3");
			result.Add(LightRigPreset.LegacyHarsh4, "legacyHarsh4");
			result.Add(LightRigPreset.ThreePt, "threePt");
			result.Add(LightRigPreset.Balanced, "balanced");
			result.Add(LightRigPreset.Soft, "soft");
			result.Add(LightRigPreset.Harsh, "harsh");
			result.Add(LightRigPreset.Flood, "flood");
			result.Add(LightRigPreset.Contrasting, "contrasting");
			result.Add(LightRigPreset.Morning, "morning");
			result.Add(LightRigPreset.Sunrise, "sunrise");
			result.Add(LightRigPreset.Sunset, "sunset");
			result.Add(LightRigPreset.Chilly, "chilly");
			result.Add(LightRigPreset.Freezing, "freezing");
			result.Add(LightRigPreset.Flat, "flat");
			result.Add(LightRigPreset.TwoPt, "twoPt");
			result.Add(LightRigPreset.Glow, "glow");
			result.Add(LightRigPreset.BrightRoom, "brightRoom");
			return result;
		}
		#endregion
		#region PresetMaterialTypeTable
		public static Dictionary<PresetMaterialType, string> PresetMaterialTypeTable = CreatePresetMaterialTypeTable();
		static Dictionary<PresetMaterialType, string> CreatePresetMaterialTypeTable() {
			Dictionary<PresetMaterialType, string> result = new Dictionary<PresetMaterialType, string>();
			result.Add(PresetMaterialType.None, "none");
			result.Add(PresetMaterialType.LegacyMatte, "legacyMatte");
			result.Add(PresetMaterialType.LegacyPlastic, "legacyPlastic");
			result.Add(PresetMaterialType.LegacyMetal, "legacyMetal");
			result.Add(PresetMaterialType.LegacyWireframe, "legacyWireframe");
			result.Add(PresetMaterialType.Matte, "matte");
			result.Add(PresetMaterialType.Plastic, "plastic");
			result.Add(PresetMaterialType.Metal, "metal");
			result.Add(PresetMaterialType.WarmMatte, "warmMatte");
			result.Add(PresetMaterialType.TranslucentPowder, "translucentPowder");
			result.Add(PresetMaterialType.Powder, "powder");
			result.Add(PresetMaterialType.DarkEdge, "dkEdge");
			result.Add(PresetMaterialType.SoftEdge, "softEdge");
			result.Add(PresetMaterialType.Clear, "clear");
			result.Add(PresetMaterialType.Flat, "flat");
			result.Add(PresetMaterialType.SoftMetal, "softmetal");
			return result;
		}
		#endregion
		#region PresetBevelTypeTable
		public static Dictionary<PresetBevelType, string> PresetBevelTypeTable = CreatePresetBevelTypeTable();
		static Dictionary<PresetBevelType, string> CreatePresetBevelTypeTable() {
			Dictionary<PresetBevelType, string> result = new Dictionary<PresetBevelType, string>();
			result.Add(PresetBevelType.None, "none");
			result.Add(PresetBevelType.RelaxedInset, "relaxedInset");
			result.Add(PresetBevelType.Circle, "circle");
			result.Add(PresetBevelType.Slope, "slope");
			result.Add(PresetBevelType.Cross, "cross");
			result.Add(PresetBevelType.Angle, "angle");
			result.Add(PresetBevelType.SoftRound, "softRound");
			result.Add(PresetBevelType.Convex, "convex");
			result.Add(PresetBevelType.CoolSlant, "coolSlant");
			result.Add(PresetBevelType.Divot, "divot");
			result.Add(PresetBevelType.Riblet, "riblet");
			result.Add(PresetBevelType.HardEdge, "hardEdge");
			result.Add(PresetBevelType.ArtDeco, "artDeco");
			return result;
		}
		#endregion
	}
}

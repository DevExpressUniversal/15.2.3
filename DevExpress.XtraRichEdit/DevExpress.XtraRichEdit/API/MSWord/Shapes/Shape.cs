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
using System.CodeDom.Compiler;
using System.Collections;
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region Shape
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Shape : IWordObject {
		Adjustments Adjustments { get; }
		MsoAutoShapeType AutoShapeType { get; set; }
		CalloutFormat Callout { get; }
		int ConnectionSiteCount { get; }
		MsoTriState Connector { get; }
		ConnectorFormat ConnectorFormat { get; }
		FillFormat Fill { get; }
		GroupShapes GroupItems { get; }
		float Height { get; set; }
		MsoTriState HorizontalFlip { get; }
		float Left { get; set; }
		LineFormat Line { get; }
		MsoTriState LockAspectRatio { get; set; }
		string Name { get; set; }
		ShapeNodes Nodes { get; }
		float Rotation { get; set; }
		PictureFormat PictureFormat { get; }
		ShadowFormat Shadow { get; }
		TextEffectFormat TextEffect { get; }
		TextFrame TextFrame { get; }
		ThreeDFormat ThreeD { get; }
		float Top { get; set; }
		MsoShapeType Type { get; }
		MsoTriState VerticalFlip { get; }
		object Vertices { get; }
		MsoTriState Visible { get; set; }
		float Width { get; set; }
		int ZOrderPosition { get; }
		Hyperlink Hyperlink { get; }
		WdRelativeHorizontalPosition RelativeHorizontalPosition { get; set; }
		WdRelativeVerticalPosition RelativeVerticalPosition { get; set; }
		int LockAnchor { get; set; }
		WrapFormat WrapFormat { get; }
		OLEFormat OLEFormat { get; }
		Range Anchor { get; }
		LinkFormat LinkFormat { get; }
		void Apply();
		void Delete();
		Shape Duplicate();
		void Flip(MsoFlipCmd FlipCmd);
		void IncrementLeft(float Increment);
		void IncrementRotation(float Increment);
		void IncrementTop(float Increment);
		void PickUp();
		void RerouteConnections();
		void ScaleHeight(float Factor, MsoTriState RelativeToOriginalSize, MsoScaleFrom Scale);
		void ScaleWidth(float Factor, MsoTriState RelativeToOriginalSize, MsoScaleFrom Scale);
		void Select(ref object Replace);
		void SetShapesDefaultProperties();
		ShapeRange Ungroup();
		void ZOrder(MsoZOrderCmd ZOrderCmd);
		InlineShape ConvertToInlineShape();
		Frame ConvertToFrame();
		void Activate();
		string AlternativeText { get; set; }
		Script Script { get; }
		MsoTriState HasDiagram { get; }
		IMsoDiagram Diagram { get; }
		MsoTriState HasDiagramNode { get; }
		DiagramNode DiagramNode { get; }
		MsoTriState Child { get; }
		Shape ParentGroup { get; }
		CanvasShapes CanvasItems { get; }
		int ID { get; }
		void CanvasCropLeft(float Increment);
		void CanvasCropTop(float Increment);
		void CanvasCropRight(float Increment);
		void CanvasCropBottom(float Increment);
		string RTF { set; }
		int LayoutInCell { get; set; }
		MsoTriState HasChart { get; }
		bool Dummy1 { get; }
		float LeftRelative { get; set; }
		float TopRelative { get; set; }
		float WidthRelative { get; set; }
		float HeightRelative { get; set; }
		WdRelativeHorizontalSize RelativeHorizontalSize { get; set; }
		WdRelativeVerticalSize RelativeVerticalSize { get; set; }
		SoftEdgeFormat SoftEdge { get; }
		GlowFormat Glow { get; }
		ReflectionFormat Reflection { get; }
	}
	#endregion
	#region Shapes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface Shapes : IWordObject, IEnumerable {
		int Count { get; }
		Shape this[object Index] { get; } 
		Shape AddCallout(MsoCalloutType Type, float Left, float Top, float Width, float Height, ref object Anchor);
		Shape AddConnector(MsoConnectorType Type, float BeginX, float BeginY, float EndX, float EndY);
		Shape AddCurve(ref object SafeArrayOfPoints, ref object Anchor);
		Shape AddLabel(MsoTextOrientation Orientation, float Left, float Top, float Width, float Height, ref object Anchor);
		Shape AddLine(float BeginX, float BeginY, float EndX, float EndY, ref object Anchor);
		Shape AddPicture(string FileName, ref object LinkToFile, ref object SaveWithDocument, ref object Left, ref object Top, ref object Width, ref object Height, ref object Anchor);
		Shape AddPolyline(ref object SafeArrayOfPoints, ref object Anchor);
		Shape AddShape(int Type, float Left, float Top, float Width, float Height, ref object Anchor);
		Shape AddTextEffect(MsoPresetTextEffect PresetTextEffect, string Text, string FontName, float FontSize, MsoTriState FontBold, MsoTriState FontItalic, float Left, float Top, ref object Anchor);
		Shape AddTextbox(MsoTextOrientation Orientation, float Left, float Top, float Width, float Height, ref object Anchor);
		FreeformBuilder BuildFreeform(MsoEditingType EditingType, float X1, float Y1);
		ShapeRange Range(ref object Index);
		void SelectAll();
		Shape AddOLEObject(ref object ClassType, ref object FileName, ref object LinkToFile, ref object DisplayAsIcon, ref object IconFileName, ref object IconIndex, ref object IconLabel, ref object Left, ref object Top, ref object Width, ref object Height, ref object Anchor);
		Shape AddOLEControl(ref object ClassType, ref object Left, ref object Top, ref object Width, ref object Height, ref object Anchor);
		Shape AddDiagram(MsoDiagramType Type, float Left, float Top, float Width, float Height, ref object Anchor);
		Shape AddCanvas(float Left, float Top, float Width, float Height, ref object Anchor);
		Shape AddChart(XlChartType Type, ref object Left, ref object Top, ref object Width, ref object Height, ref object Anchor);
	}
	#endregion
	#region WdRelativeHorizontalPosition
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRelativeHorizontalPosition {
		wdRelativeHorizontalPositionMargin,
		wdRelativeHorizontalPositionPage,
		wdRelativeHorizontalPositionColumn,
		wdRelativeHorizontalPositionCharacter,
		wdRelativeHorizontalPositionLeftMarginArea,
		wdRelativeHorizontalPositionRightMarginArea,
		wdRelativeHorizontalPositionInnerMarginArea,
		wdRelativeHorizontalPositionOuterMarginArea
	}
	#endregion
	#region WdRelativeVerticalPosition
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRelativeVerticalPosition {
		wdRelativeVerticalPositionMargin,
		wdRelativeVerticalPositionPage,
		wdRelativeVerticalPositionParagraph,
		wdRelativeVerticalPositionLine,
		wdRelativeVerticalPositionTopMarginArea,
		wdRelativeVerticalPositionBottomMarginArea,
		wdRelativeVerticalPositionInnerMarginArea,
		wdRelativeVerticalPositionOuterMarginArea
	}
	#endregion
	#region WdRelativeHorizontalSize
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRelativeHorizontalSize {
		wdRelativeHorizontalSizeMargin,
		wdRelativeHorizontalSizePage,
		wdRelativeHorizontalSizeLeftMarginArea,
		wdRelativeHorizontalSizeRightMarginArea,
		wdRelativeHorizontalSizeInnerMarginArea,
		wdRelativeHorizontalSizeOuterMarginArea
	}
	#endregion
	#region WdRelativeVerticalSize
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdRelativeVerticalSize {
		wdRelativeVerticalSizeMargin,
		wdRelativeVerticalSizePage,
		wdRelativeVerticalSizeTopMarginArea,
		wdRelativeVerticalSizeBottomMarginArea,
		wdRelativeVerticalSizeInnerMarginArea,
		wdRelativeVerticalSizeOuterMarginArea
	}
	#endregion
}

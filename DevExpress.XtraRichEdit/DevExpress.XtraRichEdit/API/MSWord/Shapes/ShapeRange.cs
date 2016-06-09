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
	#region ShapeRange
	[GeneratedCode("Suppress FxCop check", "")]
	public interface ShapeRange : IWordObject, IEnumerable {
		int Count { get; }
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
		Range Anchor { get; }
		Shape this[object Index] { get; } 
		void Align(MsoAlignCmd Align, int RelativeTo);
		void Apply();
		void Delete();
		void Distribute(MsoDistributeCmd Distribute, int RelativeTo);
		ShapeRange Duplicate();
		void Flip(MsoFlipCmd FlipCmd);
		void IncrementLeft(float Increment);
		void IncrementRotation(float Increment);
		void IncrementTop(float Increment);
		Shape Group();
		void PickUp();
		Shape Regroup();
		void RerouteConnections();
		void ScaleHeight(float Factor, MsoTriState RelativeToOriginalSize, MsoScaleFrom Scale);
		void ScaleWidth(float Factor, MsoTriState RelativeToOriginalSize, MsoScaleFrom Scale);
		void Select(ref object Replace);
		void SetShapesDefaultProperties();
		ShapeRange Ungroup();
		void ZOrder(MsoZOrderCmd ZOrderCmd);
		Frame ConvertToFrame();
		InlineShape ConvertToInlineShape();
		void Activate();
		string AlternativeText { get; set; }
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
}

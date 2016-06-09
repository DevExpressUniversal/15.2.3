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
	#region InlineShape
	[GeneratedCode("Suppress FxCop check", "")]
	public interface InlineShape : IWordObject {
		Borders Borders { get; set; }
		Range Range { get; }
		LinkFormat LinkFormat { get; }
		Field Field { get; }
		OLEFormat OLEFormat { get; }
		WdInlineShapeType Type { get; }
		Hyperlink Hyperlink { get; }
		float Height { get; set; }
		float Width { get; set; }
		float ScaleHeight { get; set; }
		float ScaleWidth { get; set; }
		MsoTriState LockAspectRatio { get; set; }
		LineFormat Line { get; }
		FillFormat Fill { get; }
		PictureFormat PictureFormat { get; set; }
		void Activate();
		void Reset();
		void Delete();
		void Select();
		Shape ConvertToShape();
		HorizontalLineFormat HorizontalLineFormat { get; }
		Script Script { get; }
		int OWSAnchor { get; }
		TextEffectFormat TextEffect { get; set; }
		string AlternativeText { get; set; }
		bool IsPictureBullet { get; }
		GroupShapes GroupItems { get; }
		MsoTriState HasChart { get; }
		bool Dummy1 { get; }
		SoftEdgeFormat SoftEdge { get; }
		GlowFormat Glow { get; }
		ReflectionFormat Reflection { get; }
		ShadowFormat Shadow { get; }
	}
	#endregion
	#region InlineShapes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface InlineShapes : IWordObject, IEnumerable {
		int Count { get; }
		InlineShape this[int Index] { get; }
		InlineShape AddPicture(string FileName, ref object LinkToFile, ref object SaveWithDocument, ref object Range);
		InlineShape AddOLEObject(ref object ClassType, ref object FileName, ref object LinkToFile, ref object DisplayAsIcon, ref object IconFileName, ref object IconIndex, ref object IconLabel, ref object Range);
		InlineShape AddOLEControl(ref object ClassType, ref object Range);
		InlineShape New(Range Range);
		InlineShape AddHorizontalLine(string FileName, ref object Range);
		InlineShape AddHorizontalLineStandard(ref object Range);
		InlineShape AddPictureBullet(string FileName, ref object Range);
		InlineShape AddChart(XlChartType Type, ref object Range);
	}
	#endregion
	#region WdInlineShapeType
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdInlineShapeType {
		wdInlineShapeChart = 12,
		wdInlineShapeDiagram = 13,
		wdInlineShapeEmbeddedOLEObject = 1,
		wdInlineShapeHorizontalLine = 6,
		wdInlineShapeLinkedOLEObject = 2,
		wdInlineShapeLinkedPicture = 4,
		wdInlineShapeLinkedPictureHorizontalLine = 8,
		wdInlineShapeLockedCanvas = 14,
		wdInlineShapeOLEControlObject = 5,
		wdInlineShapeOWSAnchor = 11,
		wdInlineShapePicture = 3,
		wdInlineShapePictureBullet = 9,
		wdInlineShapePictureHorizontalLine = 7,
		wdInlineShapeScriptAnchor = 10,
		wdInlineShapeSmartArt = 15
	}
	#endregion
}

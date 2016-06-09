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
using System.Collections;
using System.CodeDom.Compiler;
using DevExpress.API.Mso;
namespace DevExpress.XtraRichEdit.API.Word {
	#region CanvasShapes
	[GeneratedCode("Suppress FxCop check", "")]
	public interface CanvasShapes : IWordObject, IEnumerable {
		int Count { get; }
		Shape this[object Index] { get; } 
		Shape AddCallout(MsoCalloutType Type, float Left, float Top, float Width, float Height);
		Shape AddConnector(MsoConnectorType Type, float BeginX, float BeginY, float EndX, float EndY);
		Shape AddCurve(ref object SafeArrayOfPoints);
		Shape AddLabel(MsoTextOrientation Orientation, float Left, float Top, float Width, float Height);
		Shape AddLine(float BeginX, float BeginY, float EndX, float EndY);
		Shape AddPicture(string FileName, ref object LinkToFile, ref object SaveWithDocument, ref object Left, ref object Top, ref object Width, ref object Height);
		Shape AddPolyline(ref object SafeArrayOfPoints);
		Shape AddShape(int Type, float Left, float Top, float Width, float Height);
		Shape AddTextEffect(MsoPresetTextEffect PresetTextEffect, string Text, string FontName, float FontSize, MsoTriState FontBold, MsoTriState FontItalic, float Left, float Top);
		Shape AddTextbox(MsoTextOrientation Orientation, float Left, float Top, float Width, float Height);
		FreeformBuilder BuildFreeform(MsoEditingType EditingType, float X1, float Y1);
		ShapeRange Range(ref object Index);
		void SelectAll();
	}
	#endregion
}

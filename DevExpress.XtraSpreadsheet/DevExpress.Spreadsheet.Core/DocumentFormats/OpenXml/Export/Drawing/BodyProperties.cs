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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter : IDrawingTextAutoFitVisitor, IDrawingText3DVisitor {
		#region AnchoringTypeTable
		internal static Dictionary<DrawingTextAnchoringType, string> AnchoringTypeTable = CreateAnchoringTypeTable();
		static Dictionary<DrawingTextAnchoringType, string> CreateAnchoringTypeTable() {
			Dictionary<DrawingTextAnchoringType, string> result = new Dictionary<DrawingTextAnchoringType, string>();
			result.Add(DrawingTextAnchoringType.Top, "t");
			result.Add(DrawingTextAnchoringType.Center, "ctr");
			result.Add(DrawingTextAnchoringType.Bottom, "b");
			result.Add(DrawingTextAnchoringType.Justified, "just");
			result.Add(DrawingTextAnchoringType.Distributed, "dist");
			return result;
		}
		#endregion
		#region HorizontalOverflowTypeTable
		internal static Dictionary<DrawingTextHorizontalOverflowType, string> HorizontalOverflowTypeTable = CreateHorizontalOverflowTypeTable();
		static Dictionary<DrawingTextHorizontalOverflowType, string> CreateHorizontalOverflowTypeTable() {
			Dictionary<DrawingTextHorizontalOverflowType, string> result = new Dictionary<DrawingTextHorizontalOverflowType, string>();
			result.Add(DrawingTextHorizontalOverflowType.Clip, "clip");
			result.Add(DrawingTextHorizontalOverflowType.Overflow, "overflow");
			return result;
		}
		#endregion
		#region VerticalOverflowTypeTable
		internal static Dictionary<DrawingTextVerticalOverflowType, string> VerticalOverflowTypeTable = CreateVerticalOverflowTypeTable();
		static Dictionary<DrawingTextVerticalOverflowType, string> CreateVerticalOverflowTypeTable() {
			Dictionary<DrawingTextVerticalOverflowType, string> result = new Dictionary<DrawingTextVerticalOverflowType, string>();
			result.Add(DrawingTextVerticalOverflowType.Clip, "clip");
			result.Add(DrawingTextVerticalOverflowType.Overflow, "overflow");
			result.Add(DrawingTextVerticalOverflowType.Ellipsis, "ellipsis");
			return result;
		}
		#endregion
		#region VerticalTextTypeTable
		internal static Dictionary<DrawingTextVerticalTextType, string> VerticalTextTypeTable = CreateVerticalTextTypeTable();
		static Dictionary<DrawingTextVerticalTextType, string> CreateVerticalTextTypeTable() {
			Dictionary<DrawingTextVerticalTextType, string> result = new Dictionary<DrawingTextVerticalTextType, string>();
			result.Add(DrawingTextVerticalTextType.Horizontal, "horz");
			result.Add(DrawingTextVerticalTextType.Vertical, "vert");
			result.Add(DrawingTextVerticalTextType.Vertical270, "vert270");
			result.Add(DrawingTextVerticalTextType.WordArtVertical, "wordArtVert");
			result.Add(DrawingTextVerticalTextType.EastAsianVertical, "eaVert");
			result.Add(DrawingTextVerticalTextType.MongolianVertical, "mongolianVert");
			result.Add(DrawingTextVerticalTextType.VerticalWordArtRightToLeft, "wordArtVertRtl");
			return result;
		}
		#endregion
		#region TextWrappingTypeTable
		internal static Dictionary<DrawingTextWrappingType, string> TextWrappingTypeTable = CreateTextWrappingTypeTable();
		static Dictionary<DrawingTextWrappingType, string> CreateTextWrappingTypeTable() {
			Dictionary<DrawingTextWrappingType, string> result = new Dictionary<DrawingTextWrappingType, string>();
			result.Add(DrawingTextWrappingType.None, "none");
			result.Add(DrawingTextWrappingType.Square, "square");
			return result;
		}
		#endregion
		protected internal void GenerateBodyPropertiesContent(DrawingTextBodyProperties properties) {
			DrawingTextInset inset = properties.Inset;
			WriteStartElement("bodyPr", DrawingMLNamespace);
			try {
				ITextBodyOptions options = properties.Options;
				WriteIntValue("rot", properties.Rotation, options.HasRotation);
				WriteOptionalBoolValue("spcFirstLastPara", properties.ParagraphSpacing, options.HasParagraphSpacing);
				WriteEnumValue("vertOverflow", properties.VerticalOverflow, VerticalOverflowTypeTable, options.HasVerticalOverflow);
				WriteEnumValue("horzOverflow", properties.HorizontalOverflow, HorizontalOverflowTypeTable, options.HasHorizontalOverflow);
				WriteEnumValue("vert", properties.VerticalText, VerticalTextTypeTable, options.HasVerticalText);
				WriteEnumValue("wrap", properties.TextWrapping, TextWrappingTypeTable, options.HasTextWrapping);
				WriteIntEmuValue("lIns", inset.Left, inset.Options.HasLeft);
				WriteIntEmuValue("tIns", inset.Top, inset.Options.HasTop);
				WriteIntEmuValue("rIns", inset.Right, inset.Options.HasRight);
				WriteIntEmuValue("bIns", inset.Bottom, inset.Options.HasBottom);
				WriteIntValue("numCol", properties.NumberOfColumns, options.HasNumberOfColumns);
				WriteIntEmuValue("spcCol", properties.SpaceBetweenColumns, options.HasSpaceBetweenColumns);
				WriteOptionalBoolValue("rtlCol", properties.RightToLeftColumns, options.HasRightToLeftColumns);
				WriteOptionalBoolValue("fromWordArt", properties.FromWordArt, options.HasFromWordArt);
				WriteEnumValue("anchor", properties.Anchor, AnchoringTypeTable, options.HasAnchor);
				WriteOptionalBoolValue("anchorCtr", properties.AnchorCenter, options.HasAnchorCenter);
				WriteOptionalBoolValue("forceAA", properties.ForceAntiAlias, options.HasForceAntiAlias);
				WriteBoolValue("upright", properties.UprightText, DrawingTextBodyInfo.DefaultInfo.UprightText);
				WriteOptionalBoolValue("compatLnSpc", properties.CompatibleLineSpacing, options.HasCompatibleLineSpacing);
				GenerateAutoFitContent(properties.AutoFit);
				GenerateScene3DContent(properties.Scene3D);
				GenerateText3DContent(properties.Text3D);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateAutoFitContent(IDrawingTextAutoFit autoFit) {
			autoFit.Visit(this);
		}
		void GenerateText3DContent(IDrawingText3D text3d) {
			text3d.Visit(this);
		}
		#region IDrawingTextAutoFitVisitor Members
		void IDrawingTextAutoFitVisitor.VisitAutoFitNone() {
			GenerateAutoFitTag("noAutofit");
		}
		void IDrawingTextAutoFitVisitor.VisitAutoFitShape() {
			GenerateAutoFitTag("spAutoFit");
		}
		void IDrawingTextAutoFitVisitor.Visit(DrawingTextNormalAutoFit autoFit) {
			WriteStartElement("normAutofit", DrawingMLNamespace);
			try {
				WriteIntValue("fontScale", autoFit.FontScale, DrawingTextNormalAutoFit.DefaultFontScale);
				WriteIntValue("lnSpcReduction", autoFit.LineSpaceReduction, 0);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateAutoFitTag(string tagName) {
			WriteStartElement(tagName, DrawingMLNamespace);
			WriteEndElement();
		}
		#endregion
		#region IDrawingText3DVisitor Members
		void IDrawingText3DVisitor.Visit(Shape3DProperties text3d) {
			GenerateShape3DContent(text3d);
		}
		void IDrawingText3DVisitor.Visit(DrawingText3DFlatText text3d) {
			WriteStartElement("flatTx", DrawingMLNamespace);
			try {
				WriteLongValue("z", Workbook.UnitConverter.ModelUnitsToEmuL(text3d.ZCoordinate), 0);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}

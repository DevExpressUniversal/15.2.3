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
using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter : IDrawingEffectVisitor {
		protected void GenerateContainerEffectContent(ContainerEffect effect) {
			string tagName = effect.HasEffectsList ? "effectLst" : "effectDag";
			GenerateContainerEffectContent(tagName, effect);
		}
		protected internal virtual void GenerateContainerEffectContent(string tagName, ContainerEffect container) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				if (!container.HasEffectsList) {
					WriteEnumValue("type", container.Type, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.DrawingEffectContainerTypeTable, DrawingEffectContainerType.Sibling);
					WriteStringValue("name", container.Name, !String.IsNullOrEmpty(container.Name));
				}
				GenerateDrawingEffectCollectionContent(container.Effects);
			} finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateDrawingEffectCollectionContent(DrawingEffectCollection effects) {
			effects.ForEach(ExportEffect);
		}
		void ExportEffect(IDrawingEffect effect) {
			effect.Visit(this);
		}
		#region IDrawingEffectVisitor Members
		void GenerateEmptyEffectContent(string attr) {
			WriteStartElement(attr, DrawingMLNamespace);
			WriteEndElement();
		}
		void IDrawingEffectVisitor.AlphaCeilingEffectVisit() {
			GenerateEmptyEffectContent("alphaCeiling");
		}
		void IDrawingEffectVisitor.AlphaFloorEffectVisit() {
			GenerateEmptyEffectContent("alphaFloor");
		}
		void IDrawingEffectVisitor.GrayScaleEffectVisit() {
			GenerateEmptyEffectContent("grayscl");
		}
		void GenerateEffectRequiredIntValueContent(string tagName, string attr, int value) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				WriteIntValue(attr, value);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(AlphaBiLevelEffect drawingEffect) {
			GenerateEffectRequiredIntValueContent("alphaBiLevel", "thresh", drawingEffect.Thresh);
		}
		void IDrawingEffectVisitor.Visit(AlphaInverseEffect drawingEffect) {
			WriteStartElement("alphaInv", DrawingMLNamespace);
			try {
				if (!drawingEffect.Color.IsEmpty)
					GenerateDrawingColorContent(drawingEffect.Color);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(AlphaModulateEffect drawingEffect) {
			WriteStartElement("alphaMod", DrawingMLNamespace);
			try {
				GenerateContainerEffectContent("cont", drawingEffect.Container);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(AlphaModulateFixedEffect drawingEffect) {
			WriteStartElement("alphaModFix", DrawingMLNamespace);
			try {
				WriteIntValue("amt", drawingEffect.Amount, DrawingValueConstants.ThousandthOfPercentage);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(AlphaOutsetEffect drawingEffect) {
			WriteStartElement("alphaOutset", DrawingMLNamespace);
			try {
				long radius = drawingEffect.Radius;
				WriteLongValue("rad", radius, 0);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(AlphaReplaceEffect drawingEffect) {
			GenerateEffectRequiredIntValueContent("alphaRepl", "a", drawingEffect.Alpha);
		}
		void IDrawingEffectVisitor.Visit(BiLevelEffect drawingEffect) {
			GenerateEffectRequiredIntValueContent("biLevel", "thresh", drawingEffect.Thresh);
		}
		void IDrawingEffectVisitor.Visit(BlendEffect drawingEffect) {
			WriteStartElement("blend", DrawingMLNamespace);
			try {
				WriteStringValue("blend", DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.BlendModeTable[drawingEffect.BlendMode]);
				GenerateContainerEffectContent("cont", drawingEffect.Container);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(BlurEffect drawingEffect) {
			WriteStartElement("blur", DrawingMLNamespace);
			try {
				long radius = drawingEffect.Radius;
				WriteLongValue("rad", radius, 0);
				if (!drawingEffect.Grow)
					WriteBoolValue("grow", drawingEffect.Grow);
			} finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingColorContentFromEffect(DrawingColor color, string attr) {
			WriteStartElement(attr, DrawingMLNamespace);
			try {
				GenerateDrawingColorContent(color);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(ColorChangeEffect drawingEffect) {
			WriteStartElement("clrChange", DrawingMLNamespace);
			try {
				GenerateDrawingColorContentFromEffect(drawingEffect.ColorFrom, "clrFrom");
				GenerateDrawingColorContentFromEffect(drawingEffect.ColorTo, "clrTo");
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(ContainerEffect drawingEffect) {
			GenerateContainerEffectContent("cont", drawingEffect);
		}
		void IDrawingEffectVisitor.Visit(DuotoneEffect drawingEffect) {
			WriteStartElement("duotone", DrawingMLNamespace);
			try {
				GenerateDrawingColorContent(drawingEffect.FirstColor);
				GenerateDrawingColorContent(drawingEffect.SecondColor);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(Effect drawingEffect) {
			WriteStartElement("effect", DrawingMLNamespace);
			try {
				WriteStringValue("ref", drawingEffect.Reference);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(FillEffect drawingEffect) {
			WriteStartElement("fill", DrawingMLNamespace);
			try {
				GenerateDrawingFillContent(drawingEffect.Fill);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(FillOverlayEffect drawingEffect) {
			WriteStartElement("fillOverlay", DrawingMLNamespace);
			try {
				WriteStringValue("blend", DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.BlendModeTable[drawingEffect.BlendMode]);
				GenerateDrawingFillContent(drawingEffect.Fill);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(GlowEffect drawingEffect) {
			WriteStartElement("glow", DrawingMLNamespace);
			try {
				long radius = drawingEffect.Radius;
				WriteLongValue("rad", radius, 0);
				GenerateDrawingColorContent(drawingEffect.Color);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(HSLEffect drawingEffect) {
			WriteStartElement("hsl", DrawingMLNamespace);
			try {
				WriteIntValue("hue", drawingEffect.Hue, 0);
				WriteIntValue("sat", drawingEffect.Saturation, 0);
				WriteIntValue("lum", drawingEffect.Luminance, 0);
			} finally {
				WriteEndElement();
			}
		}
		void WriteOffsetShadowInfo(OffsetShadowInfo info) {
			long distance = info.Distance;
			WriteLongValue("dist", distance, 0);
			WriteIntValue("dir", info.Direction, 0);
		}
		void IDrawingEffectVisitor.Visit(InnerShadowEffect drawingEffect) {
			WriteStartElement("innerShdw", DrawingMLNamespace);
			try {
				long blurRadius = drawingEffect.BlurRadius;
				WriteLongValue("blurRad", blurRadius, 0);
				WriteOffsetShadowInfo(drawingEffect.OffsetShadow);
				GenerateDrawingColorContent(drawingEffect.Color);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(LuminanceEffect drawingEffect) {
			WriteStartElement("lum", DrawingMLNamespace);
			try {
				WriteIntValue("bright", drawingEffect.Bright, 0);
				WriteIntValue("contrast", drawingEffect.Contrast, 0);
			} finally {
				WriteEndElement();
			}
		}
		void WriteSkewAnglesInfo(SkewAnglesInfo info) {
			WriteIntValue("kx", info.Horizontal, 0);
			WriteIntValue("ky", info.Vertical, 0);
		}
		void WriteScalingFactorInfo(ScalingFactorInfo info) {
			WriteIntValue("sx", info.Horizontal, DrawingValueConstants.ThousandthOfPercentage);
			WriteIntValue("sy", info.Vertical, DrawingValueConstants.ThousandthOfPercentage);
		}
		void WriteOuterShadowEffectInfo(OuterShadowEffectInfo info) {
			long blurRadius = info.BlurRadius;
			WriteLongValue("blurRad", blurRadius, 0);
			WriteOffsetShadowInfo(info.OffsetShadow);
			WriteScalingFactorInfo(info.ScalingFactor);
			WriteSkewAnglesInfo(info.SkewAngles);
			WriteEnumValue("algn", info.ShadowAlignment, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.RectangleAlignTypeTable, RectangleAlignType.Bottom);
			if (!info.RotateWithShape)
				WriteBoolValue("rotWithShape", info.RotateWithShape);
		}
		void IDrawingEffectVisitor.Visit(OuterShadowEffect drawingEffect) {
			WriteStartElement("outerShdw", DrawingMLNamespace);
			try {
				WriteOuterShadowEffectInfo(drawingEffect.Info);
				GenerateDrawingColorContent(drawingEffect.Color);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(PresetShadowEffect drawingEffect) {
			WriteStartElement("prstShdw", DrawingMLNamespace);
			try {
				WriteEnumValue("prst", drawingEffect.Type, DevExpress.Office.OpenXml.Export.OpenXmlExporterBase.PresetShadowTypeTable);
				WriteOffsetShadowInfo(drawingEffect.OffsetShadow);
				GenerateDrawingColorContent(drawingEffect.Color);
			} finally {
				WriteEndElement();
			}
		}
		void WriteReflectionOpacityInfo(ReflectionOpacityInfo info) {
			WriteIntValue("stA", info.StartOpacity, DrawingValueConstants.ThousandthOfPercentage);
			WriteIntValue("stPos", info.StartPosition, 0);
			WriteIntValue("endA", info.EndOpacity, 0);
			WriteIntValue("endPos", info.EndPosition, DrawingValueConstants.ThousandthOfPercentage);
			WriteIntValue("fadeDir", info.FadeDirection, DrawingValueConstants.MaxFixedAngle);
		}
		void IDrawingEffectVisitor.Visit(ReflectionEffect drawingEffect) {
			WriteStartElement("reflection", DrawingMLNamespace);
			try {
				WriteReflectionOpacityInfo(drawingEffect.ReflectionOpacity);
				WriteOuterShadowEffectInfo(drawingEffect.OuterShadowEffectInfo);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(RelativeOffsetEffect drawingEffect) {
			WriteStartElement("relOff", DrawingMLNamespace);
			try {
				WriteIntValue("tx", drawingEffect.OffsetX, 0);
				WriteIntValue("ty", drawingEffect.OffsetY, 0);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(SoftEdgeEffect drawingEffect) {
			WriteStartElement("softEdge", DrawingMLNamespace);
			try {
				long radius = drawingEffect.Radius;
				WriteLongValue("rad", radius, 0);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(SolidColorReplacementEffect drawingEffect) {
			GenerateDrawingColorContentFromEffect(drawingEffect.Color, "clrRepl");
		}
		void IDrawingEffectVisitor.Visit(TintEffect drawingEffect) {
			WriteStartElement("tint", DrawingMLNamespace);
			try {
				WriteIntValue("hue", drawingEffect.Hue, 0);
				WriteIntValue("amt", drawingEffect.Amount, 0);
			} finally {
				WriteEndElement();
			}
		}
		void IDrawingEffectVisitor.Visit(TransformEffect drawingEffect) {
			WriteStartElement("xfrm", DrawingMLNamespace);
			try {
				WriteScalingFactorInfo(drawingEffect.ScalingFactor);
				WriteSkewAnglesInfo(drawingEffect.SkewAngles);
				long horizontalShift = drawingEffect.CoordinateShift.Horizontal;
				WriteLongValue("tx", horizontalShift, 0);
				long verticalShift = drawingEffect.CoordinateShift.Vertical;
				WriteLongValue("ty", verticalShift, 0);
			} finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}

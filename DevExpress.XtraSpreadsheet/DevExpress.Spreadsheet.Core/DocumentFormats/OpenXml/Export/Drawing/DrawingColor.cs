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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Import.OpenXml;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter : IColorTransformVisitor {
		#region Export colors
		protected internal virtual void GenerateDrawingColorContent(DrawingColor color) {
			switch (color.ColorType) {
				case DrawingColorType.Rgb:
					ExportRgbColor(color);
					break;
				case DrawingColorType.Scheme:
					ExportShemeColor(color);
					break;
				case DrawingColorType.System:
					ExportSystemColor(color);
					break;
				case DrawingColorType.Preset:
					ExportPresetColor(color);
					break;
				case DrawingColorType.ScRgb:
					ExportScRgbColor(color);
					break;
				case DrawingColorType.Hsl:
					ExportHslColor(color);
					break;
			}
		}
		protected internal virtual void ExportShemeColor(DrawingColor color) {
			WriteStartElement("schemeClr", DrawingMLNamespace);
			try {
				ExportSchemeColorAttributes(color);
				ExportColorTransformation(color.Transforms);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void ExportSystemColor(DrawingColor color) {
			WriteStartElement("sysClr", DrawingMLNamespace);
			try {
				ExportSystemColorAttributes(color.Info.SystemColor);
				ExportColorTransformation(color.Transforms);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void ExportRgbColor(DrawingColor color) {
			WriteStartElement("srgbClr", DrawingMLNamespace);
			try {
				ExportRgbColorAttributes(color.OriginalColor.Rgb);
				ExportColorTransformation(color.Transforms);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void ExportPresetColor(DrawingColor color) {
			WriteStartElement("prstClr", DrawingMLNamespace);
			try {
				WriteStringValue("val", color.OriginalColor.Preset);
				ExportColorTransformation(color.Transforms);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void ExportScRgbColor(DrawingColor color) {
			WriteStartElement("scrgbClr", DrawingMLNamespace);
			try {
				ExportScRgbColorAttributes(color.OriginalColor.ScRgb);
				ExportColorTransformation(color.Transforms);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void ExportHslColor(DrawingColor color) {
			WriteStartElement("hslClr", DrawingMLNamespace);
			try {
				ExportHslColorAttributes(color.OriginalColor.Hsl);
				ExportColorTransformation(color.Transforms);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportSchemeColorAttributes(DrawingColor color) {
			string value;
			if (SchemeColorDestination.schemeColorTable.TryGetValue(color.Info.SchemeColor, out value))
				WriteStringValue("val", value);
		}
		void ExportSystemColorAttributes(SystemColorValues color) {
			string value;
			if (SystemColorDestination.systemColorTable.TryGetValue(color, out value))
				WriteStringValue("val", value);
		}
		void ExportRgbColorAttributes(Color color) {
			byte r = color.R;
			byte g = color.G;
			byte b = color.B;
			WriteStringValue("val", String.Format("{0:X2}", r) + String.Format("{0:X2}", g) + String.Format("{0:X2}", b));
		}
		void ExportScRgbColorAttributes(ScRGBColor color) {
			WriteIntValue("r", color.ScR);
			WriteIntValue("g", color.ScG);
			WriteIntValue("b", color.ScB);
		}
		void ExportHslColorAttributes(ColorHSL color) {
			WriteIntValue("hue", color.Hue);
			WriteIntValue("sat", color.Saturation);
			WriteIntValue("lum", color.Luminance);
		}
		void ExportColorTransformation(ColorTransformCollection transforms) {
			int count = transforms.Count;
			for (int i = 0; i < count; i++) 
				transforms[i].Visit(this);
		}
		void ExportColorTransformMember(string name, int transformation) {
			WriteStartElement(name, DrawingMLNamespace);
			try {
				WriteIntValue("val", transformation);
			}
			finally {
				WriteEndElement();
			}
		}
		void ExportColorTransformMember(string name) {
			WriteStartElement(name, DrawingMLNamespace);
			WriteEndElement();
		}
		#endregion
		public void Visit(TintColorTransform transform) {
			ExportColorTransformMember("tint", transform.Value);
		}
		public void Visit(ShadeColorTransform transform) {
			ExportColorTransformMember("shade", transform.Value);
		}
		public void Visit(ComplementColorTransform transform) {
			ExportColorTransformMember("comp");
		}
		public void Visit(InverseColorTransform transform) {
			ExportColorTransformMember("inv");
		}
		public void Visit(GrayscaleColorTransform transform) {
			ExportColorTransformMember("gray");
		}
		public void Visit(AlphaColorTransform transform) {
			ExportColorTransformMember("alpha", transform.Value);
		}
		public void Visit(AlphaOffsetColorTransform transform) {
			ExportColorTransformMember("alphaOff", transform.Value);
		}
		public void Visit(AlphaModulationColorTransform transform) {
			ExportColorTransformMember("alphaMod", transform.Value);
		}
		public void Visit(HueColorTransform transform) {
			ExportColorTransformMember("hue", transform.Value);
		}
		public void Visit(HueOffsetColorTransform transform) {
			ExportColorTransformMember("hueOff", transform.Value);
		}
		public void Visit(HueModulationColorTransform transform) {
			ExportColorTransformMember("hueMod", transform.Value);
		}
		public void Visit(LuminanceColorTransform transform) {
			ExportColorTransformMember("lum", transform.Value);
		}
		public void Visit(LuminanceOffsetColorTransform transform) {
			ExportColorTransformMember("lumOff", transform.Value);
		}
		public void Visit(LuminanceModulationColorTransform transform) {
			ExportColorTransformMember("lumMod", transform.Value);
		}
		public void Visit(SaturationColorTransform transform) {
			ExportColorTransformMember("sat", transform.Value);
		}
		public void Visit(SaturationOffsetColorTransform transform) {
			ExportColorTransformMember("satOff", transform.Value);
		}
		public void Visit(SaturationModulationColorTransform transform) {
			ExportColorTransformMember("satMod", transform.Value);
		}
		public void Visit(RedColorTransform transform) {
			ExportColorTransformMember("red", transform.Value);
		}
		public void Visit(RedOffsetColorTransform transform) {
			ExportColorTransformMember("redOff", transform.Value);
		}
		public void Visit(RedModulationColorTransform transform) {
			ExportColorTransformMember("redMod", transform.Value);
		}
		public void Visit(GreenColorTransform transform) {
			ExportColorTransformMember("green", transform.Value);
		}
		public void Visit(GreenOffsetColorTransform transform) {
			ExportColorTransformMember("greenOff", transform.Value);
		}
		public void Visit(GreenModulationColorTransform transform) {
			ExportColorTransformMember("greenMod", transform.Value);
		}
		public void Visit(BlueColorTransform transform) {
			ExportColorTransformMember("blue", transform.Value);
		}
		public void Visit(BlueOffsetColorTransform transform) {
			ExportColorTransformMember("blueOff", transform.Value);
		}
		public void Visit(BlueModulationColorTransform transform) {
			ExportColorTransformMember("blueMod", transform.Value);
		}
		public void Visit(GammaColorTransform transform) {
			ExportColorTransformMember("gamma");
		}
		public void Visit(InverseGammaColorTransform transform) {
			ExportColorTransformMember("invGamma");
		}
	}
}

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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
namespace DevExpress.Office.Import.OpenXml {
	#region DrawingColorDestinationBase
	public abstract class DrawingColorDestinationBase : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		protected static void AddDrawingColorHandlers(ElementHandlerTable<DestinationAndXmlBasedImporter> table) {
			table.Add("hslClr", OnHSLColors);
			table.Add("prstClr", OnPresetColors);
			table.Add("schemeClr", OnSchemeColors);
			table.Add("scrgbClr", OnPercentageRGBColors);
			table.Add("srgbClr", OnHexRGBColors);
			table.Add("sysClr", OnSystemColors);
		}
		static DrawingColorDestinationBase GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingColorDestinationBase)importer.PeekDestination();
		}
		protected static DrawingColor GetColor(DestinationAndXmlBasedImporter importer) {
			DrawingColorDestinationBase destination = GetThis(importer);
			destination.HasColor = true;
			return destination.Color;
		}
		#region Handlers
		static Destination OnHSLColors(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new HSLColorDestination(importer, GetColor(importer));
		}
		static Destination OnPresetColors(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new PresetColorDestination(importer, GetColor(importer));
		}
		static Destination OnSchemeColors(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new SchemeColorDestination(importer, GetColor(importer));
		}
		static Destination OnPercentageRGBColors(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new PercentageRGBColorDestination(importer, GetColor(importer));
		}
		static Destination OnHexRGBColors(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new HexRGBColorDestination(importer, GetColor(importer));
		}
		static Destination OnSystemColors(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new SystemColorDestination(importer, GetColor(importer));
		}
		#endregion
		#endregion
		#region Fields
		readonly DrawingColor color;
		bool hasColor;
		#endregion
		protected DrawingColorDestinationBase(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer) {
			this.color = color;
		}
		#region Properties
		protected virtual DrawingColor Color { get { return color; } }
		protected virtual bool HasColor { get { return hasColor; } set { hasColor = value; } }
		#endregion
	}
	#endregion
	#region DrawingColorDestination
	public class DrawingColorDestination : DrawingColorDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			AddDrawingColorHandlers(result);
			return result;
		}
		#endregion
		public DrawingColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region DrawingColorPropertiesDestination classes
	#region DrawingColorPropertiesDestinationBase (abstract class)
	public abstract class DrawingColorPropertiesDestinationBase : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("alpha", OnAlpha);
			result.Add("alphaMod", OnAlphaModulation);
			result.Add("alphaOff", OnAlphaOffset);
			result.Add("blue", OnBlue);
			result.Add("blueMod", OnBlueModification);
			result.Add("blueOff", OnBlueOffset);
			result.Add("comp", OnComplement);
			result.Add("gamma", OnGamma);
			result.Add("gray", OnGray);
			result.Add("green", OnGreen);
			result.Add("greenMod", OnGreenModification);
			result.Add("greenOff", OnGreenOffset);
			result.Add("hue", OnHue);
			result.Add("hueMod", OnHueModulate);
			result.Add("hueOff", OnHueOffset);
			result.Add("inv", OnInverse);
			result.Add("invGamma", OnInverseGamma);
			result.Add("lum", OnLuminance);
			result.Add("lumMod", OnLuminanceModulation);
			result.Add("lumOff", OnLuminanceOffset);
			result.Add("red", OnRed);
			result.Add("redMod", OnRedModulation);
			result.Add("redOff", OnRedOffset);
			result.Add("sat", OnSaturation);
			result.Add("satMod", OnSaturationModulation);
			result.Add("satOff", OnSaturationOffset);
			result.Add("shade", OnShade);
			result.Add("tint", OnTint);
			return result;
		}
		static DrawingColorPropertiesDestinationBase GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingColorPropertiesDestinationBase)importer.PeekDestination();
		}
		static Destination OnAlpha(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationAlpha(importer, GetThis(importer).color);
		}
		static Destination OnAlphaModulation(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationAlphaModulation(importer, GetThis(importer).color);
		}
		static Destination OnAlphaOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationAlphaOffset(importer, GetThis(importer).color);
		}
		static Destination OnBlue(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationBlue(importer, GetThis(importer).color);
		}
		static Destination OnBlueModification(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationBlueModification(importer, GetThis(importer).color);
		}
		static Destination OnBlueOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationBlueOffset(importer, GetThis(importer).color);
		}
		static Destination OnComplement(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationComplement(importer, GetThis(importer).color);
		}
		static Destination OnGamma(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationGamma(importer, GetThis(importer).color);
		}
		static Destination OnGray(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationGray(importer, GetThis(importer).color);
		}
		static Destination OnGreen(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationGreen(importer, GetThis(importer).color);
		}
		static Destination OnGreenModification(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationGreenModification(importer, GetThis(importer).color);
		}
		static Destination OnGreenOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationGreenOffset(importer, GetThis(importer).color);
		}
		static Destination OnHue(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationHue(importer, GetThis(importer).color);
		}
		static Destination OnHueModulate(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationHueModulate(importer, GetThis(importer).color);
		}
		static Destination OnHueOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationHueOffset(importer, GetThis(importer).color);
		}
		static Destination OnInverse(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationInverse(importer, GetThis(importer).color);
		}
		static Destination OnInverseGamma(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationInverseGamma(importer, GetThis(importer).color);
		}
		static Destination OnLuminance(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationLuminance(importer, GetThis(importer).color);
		}
		static Destination OnLuminanceModulation(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationLuminanceModulation(importer, GetThis(importer).color);
		}
		static Destination OnLuminanceOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationLuminanceOffset(importer, GetThis(importer).color);
		}
		static Destination OnRed(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationRed(importer, GetThis(importer).color);
		}
		static Destination OnRedModulation(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationRedModulation(importer, GetThis(importer).color);
		}
		static Destination OnRedOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationRedOffset(importer, GetThis(importer).color);
		}
		static Destination OnSaturation(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationSaturation(importer, GetThis(importer).color);
		}
		static Destination OnSaturationModulation(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationSaturationModulation(importer, GetThis(importer).color);
		}
		static Destination OnSaturationOffset(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationSaturationOffset(importer, GetThis(importer).color);
		}
		static Destination OnShade(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationShade(importer, GetThis(importer).color);
		}
		static Destination OnTint(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorTransformDestinationTint(importer, GetThis(importer).color);
		}
		#endregion
		readonly DrawingColor color;
		readonly DrawingColorModelInfo colorModelInfo;
		protected DrawingColorPropertiesDestinationBase(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer) {
			this.color = color;
			this.colorModelInfo = new DrawingColorModelInfo();
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public DrawingColor Color { get { return color; } }
		protected DrawingColorModelInfo ColorModelInfo { get { return colorModelInfo; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			SetColorPropertyValue(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			color.AssignInfo(colorModelInfo);
		}
		protected abstract void SetColorPropertyValue(XmlReader reader);
	}
	#endregion
	#region PresetColorDestination
	public class PresetColorDestination : DrawingColorPropertiesDestinationBase {
		public PresetColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override void SetColorPropertyValue(XmlReader reader) {
			ColorModelInfo.Preset = Importer.ReadAttribute(reader, "val");
		}
	}
	#endregion
	#region PercentageRGBColorDestination
	public class PercentageRGBColorDestination : DrawingColorPropertiesDestinationBase {
		public PercentageRGBColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override void SetColorPropertyValue(XmlReader reader) {
			int scR = Importer.GetIntegerValue(reader, "r");
			int scG = Importer.GetIntegerValue(reader, "g");
			int scB = Importer.GetIntegerValue(reader, "b");
			if ((scR == int.MinValue) || (scG == int.MinValue) || (scB == int.MinValue))
				Importer.ThrowInvalidFile();
			ColorModelInfo.ScRgb = new ScRGBColor(scR, scG, scB);
		}
	}
	#endregion
	#region HexRGBColorDestination
	public class HexRGBColorDestination : DrawingColorPropertiesDestinationBase {
		public HexRGBColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override void SetColorPropertyValue(XmlReader reader) {
			ColorModelInfo.Rgb = DrawingColorModelInfo.SRgbToRgb(Importer.ReadAttribute(reader, "val"));
		}
	}
	#endregion
	#region SystemColorDestination
	public class SystemColorDestination : DrawingColorPropertiesDestinationBase {
		#region Static Members
		public static Dictionary<SystemColorValues, string> systemColorTable = CreateSystemColorTable();
		static Dictionary<SystemColorValues, string> CreateSystemColorTable() {
			Dictionary<SystemColorValues, string> result = new Dictionary<SystemColorValues, string>();
			result.Add(SystemColorValues.Sc3dDkShadow, "3dDkShadow");
			result.Add(SystemColorValues.Sc3dLight, "3dLight");
			result.Add(SystemColorValues.ScActiveBorder, "activeBorder");
			result.Add(SystemColorValues.ScActiveCaption, "activeCaption");
			result.Add(SystemColorValues.ScAppWorkspace, "appWorkspace");
			result.Add(SystemColorValues.ScBackground, "background");
			result.Add(SystemColorValues.ScBtnFace, "btnFace");
			result.Add(SystemColorValues.ScBtnHighlight, "btnHighlight");
			result.Add(SystemColorValues.ScBtnShadow, "btnShadow");
			result.Add(SystemColorValues.ScBtnText, "btnText");
			result.Add(SystemColorValues.ScCaptionText, "captionText");
			result.Add(SystemColorValues.ScGradientActiveCaption, "gradientActiveCaption");
			result.Add(SystemColorValues.ScGradientInactiveCaption, "gradientInactiveCaption");
			result.Add(SystemColorValues.ScGrayText, "grayText");
			result.Add(SystemColorValues.ScHighlight, "highlight");
			result.Add(SystemColorValues.ScHighlightText, "highlightText");
			result.Add(SystemColorValues.ScHotLight, "hotLight");
			result.Add(SystemColorValues.ScInactiveBorder, "inactiveBorder");
			result.Add(SystemColorValues.ScInactiveCaption, "inactiveCaption");
			result.Add(SystemColorValues.ScInactiveCaptionText, "inactiveCaptionText");
			result.Add(SystemColorValues.ScInfoBk, "infoBk");
			result.Add(SystemColorValues.ScInfoText, "infoText");
			result.Add(SystemColorValues.ScMenu, "menu");
			result.Add(SystemColorValues.ScMenuBar, "menuBar");
			result.Add(SystemColorValues.ScMenuHighlight, "menuHighlight");
			result.Add(SystemColorValues.ScMenuText, "menuText");
			result.Add(SystemColorValues.ScScrollBar, "scrollBar");
			result.Add(SystemColorValues.ScWindow, "window");
			result.Add(SystemColorValues.ScWindowFrame, "windowFrame");
			result.Add(SystemColorValues.ScWindowText, "windowText");
			return result;
		}
		#endregion
		public SystemColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override void SetColorPropertyValue(XmlReader reader) {
			SystemColorValues systemColor = Importer.GetWpEnumValue<SystemColorValues>(reader, "val", systemColorTable, SystemColorValues.Empty);
			if (systemColor == SystemColorValues.Empty)
				Importer.ThrowInvalidFile();
			ColorModelInfo.SystemColor = systemColor;
		}
	}
	#endregion
	#region HSLColorDestination
	public class HSLColorDestination : DrawingColorPropertiesDestinationBase {
		public HSLColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override void SetColorPropertyValue(XmlReader reader) {
			int hue = Importer.GetIntegerValue(reader, "hue");
			int sat = Importer.GetIntegerValue(reader, "sat");
			int lum = Importer.GetIntegerValue(reader, "lum");
			if ((hue == int.MinValue) || (sat == int.MinValue) || (lum == int.MinValue))
				Importer.ThrowInvalidFile();
			ColorModelInfo.Hsl = new ColorHSL(hue, sat, lum);
		}
	}
	#endregion
	#region SchemeColorDestination
	public class SchemeColorDestination : DrawingColorPropertiesDestinationBase {
		public static Dictionary<SchemeColorValues, string> schemeColorTable = CreateSchemeColorTable();
		static Dictionary<SchemeColorValues, string> CreateSchemeColorTable() {
			Dictionary<SchemeColorValues, string> result = new Dictionary<SchemeColorValues, string>();
			result.Add(SchemeColorValues.Accent1, "accent1");
			result.Add(SchemeColorValues.Accent2, "accent2");
			result.Add(SchemeColorValues.Accent3, "accent3");
			result.Add(SchemeColorValues.Accent4, "accent4");
			result.Add(SchemeColorValues.Accent5, "accent5");
			result.Add(SchemeColorValues.Accent6, "accent6");
			result.Add(SchemeColorValues.Background1, "bg1");
			result.Add(SchemeColorValues.Background2, "bg2");
			result.Add(SchemeColorValues.Dark1, "dk1");
			result.Add(SchemeColorValues.Dark2, "dk2");
			result.Add(SchemeColorValues.FollowedHyperlink, "folHlink");
			result.Add(SchemeColorValues.Hyperlink, "hlink");
			result.Add(SchemeColorValues.Light1, "lt1");
			result.Add(SchemeColorValues.Light2, "lt2");
			result.Add(SchemeColorValues.Style, "phClr");
			result.Add(SchemeColorValues.Text1, "tx1");
			result.Add(SchemeColorValues.Text2, "tx2");
			return result;
		}
		public SchemeColorDestination(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		protected override void SetColorPropertyValue(XmlReader reader) {
			SchemeColorValues schemeColor = Importer.GetWpEnumValue<SchemeColorValues>(reader, "val", schemeColorTable, SchemeColorValues.Empty);
			if (schemeColor == SchemeColorValues.Empty)
				Importer.ThrowInvalidFile();
			ColorModelInfo.SchemeColor = schemeColor;
		}
	}
	#endregion
	#endregion
}

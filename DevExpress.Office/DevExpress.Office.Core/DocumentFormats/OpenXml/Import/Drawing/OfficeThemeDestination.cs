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

using System.Xml;
using DevExpress.Office;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils;
using System;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Import.OpenXml {
	#region OfficeThemeDestination
	public class OfficeThemeDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("themeElements", OnThemeElements);
			return result;
		}
		static Destination OnThemeElements(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeElementsDestination(importer);
		}
		#endregion
		string name;
		public OfficeThemeDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			name = Importer.ReadAttribute(reader, "name");
			if(!String.IsNullOrEmpty(name))
			   Importer.DocumentModel.OfficeTheme.Name = name;
		}
	}
	#endregion
	#region OfficeThemeElementsDestination
	public class OfficeThemeElementsDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("clrScheme", OnColorScheme);
			result.Add("fontScheme", OnFontScheme);
			result.Add("fmtScheme", OnFormatScheme);
			return result;
		}
		static Destination OnColorScheme(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorSchemeDestination(importer);
		}
		static Destination OnFontScheme(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeFontSchemeDestination(importer);
		}
		static Destination OnFormatScheme(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeFormatSchemeDestination(importer);
		}
		#endregion
		public OfficeThemeElementsDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region OfficeThemeColorSchemeDestination
	public class OfficeThemeColorSchemeDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("dk1", OnColorDark1);
			result.Add("lt1", OnColorLight1);
			result.Add("dk2", OnColorDark2);
			result.Add("lt2", OnColorLight2);
			result.Add("accent1", OnColorAccent1);
			result.Add("accent2", OnColorAccent2);
			result.Add("accent3", OnColorAccent3);
			result.Add("accent4", OnColorAccent4);
			result.Add("accent5", OnColorAccent5);
			result.Add("accent6", OnColorAccent6);
			result.Add("hlink", OnColorHyperlink);
			result.Add("folHlink", OnColorFollowedHyperlink);
			return result;
		}
		static Destination OnColorDark1(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Dark1);
		}
		static Destination OnColorDark2(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Dark2);
		}
		static Destination OnColorLight1(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Light1);
		}
		static Destination OnColorLight2(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Light2);
		}
		static Destination OnColorAccent1(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Accent1);
		}
		static Destination OnColorAccent2(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Accent2);
		}
		static Destination OnColorAccent3(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Accent3);
		}
		static Destination OnColorAccent4(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Accent4);
		}
		static Destination OnColorAccent5(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Accent5);
		}
		static Destination OnColorAccent6(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Accent6);
		}
		static Destination OnColorHyperlink(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.Hyperlink);
		}
		static Destination OnColorFollowedHyperlink(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeColorDestination(importer, ThemeColorIndex.FollowedHyperlink);
		}
		#endregion
		public OfficeThemeColorSchemeDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.OfficeTheme.Colors.Name = Importer.ReadAttribute(reader, "name");
		}
	}
	#endregion
	#region OfficeThemeColorDestination
	public class OfficeThemeColorDestination : DrawingColorDestination {
		readonly ThemeColorIndex colorIndex;
		public OfficeThemeColorDestination(DestinationAndXmlBasedImporter importer, ThemeColorIndex colorIndex)
			: base(importer, new DrawingColor(importer.ActualDocumentModel)) {
			this.colorIndex = colorIndex;
		}
		public override void ProcessElementClose(XmlReader reader) {
		   Importer.DocumentModel.OfficeTheme.Colors.SetDrawingColor(colorIndex, Color);
		}
	}
	#endregion
	#region OfficeThemeFontSchemeDestination
	public class OfficeThemeFontSchemeDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("majorFont", OnMajorFont);
			result.Add("minorFont", OnMinorFont);
			return result;
		}
		static OfficeThemeFontSchemeDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (OfficeThemeFontSchemeDestination)importer.PeekDestination();
		}
		static Destination OnMajorFont(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeFontCollectionSchemeDestination(importer, GetThis(importer).MajorFont);
		}
		static Destination OnMinorFont(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeFontCollectionSchemeDestination(importer, GetThis(importer).MinorFont);
		}
		#endregion
		public OfficeThemeFontSchemeDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal ThemeFontSchemePart MajorFont { get { return Importer.DocumentModel.OfficeTheme.FontScheme.MajorFont; } }
		protected internal ThemeFontSchemePart MinorFont { get { return Importer.DocumentModel.OfficeTheme.FontScheme.MinorFont; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.OfficeTheme.FontScheme.Name = Importer.ReadAttribute(reader, "name");
		}
	}
	#endregion
	#region OfficeThemeFontCollectionSchemeDestination
	public class OfficeThemeFontCollectionSchemeDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("latin", OnLatin);
			result.Add("ea", OnEastAsian);
			result.Add("cs", OnComplexScript);
			result.Add("font", OnFont);
			return result;
		}
		static OfficeThemeFontCollectionSchemeDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (OfficeThemeFontCollectionSchemeDestination)importer.PeekDestination();
		}
		static Destination OnLatin(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			OfficeThemeFontCollectionSchemeDestination destination = GetThis(importer);
			destination.hasLatin = true;
			return new DrawingTextFontDestination(importer, destination.Latin);
		}
		static Destination OnEastAsian(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			OfficeThemeFontCollectionSchemeDestination destination = GetThis(importer);
			destination.hasEastAsian = true;
			return new DrawingTextFontDestination(importer, destination.EastAsian);
		}
		static Destination OnComplexScript(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			OfficeThemeFontCollectionSchemeDestination destination = GetThis(importer);
			destination.hasComplexScript = true;
			return new DrawingTextFontDestination(importer, destination.ComplexScript);
		}
		static Destination OnFont(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeSupplementalFontSchemeDestination(importer, GetThis(importer).fontPart);
		}
		#endregion
		readonly ThemeFontSchemePart fontPart;
		bool hasLatin = false;
		bool hasEastAsian = false;
		bool hasComplexScript = false;
		public OfficeThemeFontCollectionSchemeDestination(DestinationAndXmlBasedImporter importer, ThemeFontSchemePart fontPart)
			: base(importer) {
			Guard.ArgumentNotNull(fontPart, "ThemeFontSchemePart");
			this.fontPart = fontPart;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DrawingTextFont Latin { get { return fontPart.Latin; } }
		protected internal DrawingTextFont EastAsian { get { return fontPart.EastAsian; } }
		protected internal DrawingTextFont ComplexScript { get { return fontPart.ComplexScript; } }
		protected internal Dictionary<string, string> Fonts { get { return fontPart.SupplementalFonts; } }
		public override void ProcessElementClose(XmlReader reader) {
			fontPart.IsValid =
				hasLatin && hasEastAsian && hasComplexScript &&
				!String.IsNullOrEmpty(Latin.Typeface) && EastAsian.Typeface != null && ComplexScript.Typeface != null;
		}
	}
	#endregion
	#region OfficeThemeSupplementalFontSchemeDestination
	public class OfficeThemeSupplementalFontSchemeDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		readonly ThemeFontSchemePart fontPart;
		public OfficeThemeSupplementalFontSchemeDestination(DestinationAndXmlBasedImporter importer, ThemeFontSchemePart fontPart)
			: base(importer) {
			Guard.ArgumentNotNull(fontPart, "ThemeFontSchemePart");
			this.fontPart = fontPart;
		}
		public override void ProcessElementClose(XmlReader reader) {
			string script = Importer.ReadAttribute(reader, "script");
			string typeface = Importer.ReadAttribute(reader, "typeface");
			if (String.IsNullOrEmpty(script) && String.IsNullOrEmpty(typeface))
				Importer.ThrowInvalidFile();
			fontPart.AddSupplementalFont(script, typeface);
		}
	}
	#endregion
	#region OfficeThemeFormatSchemeDestination
	public class OfficeThemeFormatSchemeDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("bgFillStyleLst", OnBackgroundFillStyleList);
			result.Add("fillStyleLst", OnFillStyleList);
			result.Add("effectStyleLst", OnEffectStyleList);
			result.Add("lnStyleLst", OnLineStyleList);
			return result;
		}
		static Destination OnBackgroundFillStyleList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeFillStyleListDestination(importer, importer.DocumentModel.OfficeTheme.FormatScheme.BackgroundFillStyleList);
		}
		static Destination OnFillStyleList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeFillStyleListDestination(importer, importer.DocumentModel.OfficeTheme.FormatScheme.FillStyleList);
		}
		static Destination OnEffectStyleList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeEffectStyleListDestination(importer);
		}
		static Destination OnLineStyleList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OfficeThemeLineStyleListDestination(importer);
		}
		#endregion
		public OfficeThemeFormatSchemeDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.OfficeTheme.FormatScheme.Name = Importer.ReadAttribute(reader, "name");
		}
	}
	#endregion
	#region OfficeThemeFillStyleListDestination
	public class OfficeThemeFillStyleListDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("gradFill", OnGradientFill);
			result.Add("solidFill", OnSolidFill);
			result.Add("pattFill", OnPatternFill);
			result.Add("noFill", OnNoFill);
			result.Add("blipFill", OnPictureFill);
			result.Add("grpFill", OnGroupFill);
			return result;
		}
		static T GetFill<T>(DestinationAndXmlBasedImporter importer, T fill) where T : IDrawingFill {
			OfficeThemeFillStyleListDestination destination = (OfficeThemeFillStyleListDestination)importer.PeekDestination();
			destination.list.Add(fill);
			return fill;
		}
		static Destination OnGradientFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingGradientFill fill = GetFill(importer, new DrawingGradientFill(importer.ActualDocumentModel));
			return new DrawingGradientFillDestination(importer, fill);
		}
		static Destination OnSolidFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingSolidFill fill = GetFill(importer, new DrawingSolidFill(importer.ActualDocumentModel));
			return new DrawingColorDestination(importer, fill.Color);
		}
		static Destination OnPatternFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingPatternFill fill = GetFill(importer, new DrawingPatternFill(importer.ActualDocumentModel));
			return new DrawingPatternFillDestination(importer, fill);
		}
		static Destination OnNoFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			GetFill(importer, DrawingFill.None);
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		static Destination OnPictureFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingBlipFill fill = GetFill(importer, new DrawingBlipFill(importer.ActualDocumentModel));
			return new DrawingBlipFillDestination(importer, fill);
		}
		static Destination OnGroupFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			GetFill(importer, DrawingFill.Group);
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		#endregion
		readonly List<IDrawingFill> list;
		public OfficeThemeFillStyleListDestination(DestinationAndXmlBasedImporter importer, List<IDrawingFill> list)
			: base(importer) {
			Guard.ArgumentNotNull(list, "list");
			this.list = list;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region OfficeThemeEffectStyleListDestination
	public class OfficeThemeEffectStyleListDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("effectStyle", OnEffectStyle);
			return result;
		}
		static Destination OnEffectStyle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingEffectStyle style = new DrawingEffectStyle(importer.ActualDocumentModel);
			importer.DocumentModel.OfficeTheme.FormatScheme.EffectStyleList.Add(style);
			return new OfficeThemeEffectStyleDestination(importer, style);
		}
		#endregion
		public OfficeThemeEffectStyleListDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region OfficeThemeEffectStyleDestination
	public class OfficeThemeEffectStyleDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("effectDag", OnEffectDag);
			result.Add("effectLst", OnEffectList);
			result.Add("scene3d", OnEffectScene3D);
			result.Add("sp3d", OnEffectSp3D);
			return result;
		}
		static OfficeThemeEffectStyleDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (OfficeThemeEffectStyleDestination)importer.PeekDestination();
		}
		static Destination OnEffectDag(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsDAGDestination(importer, GetThis(importer).style.ContainerEffect);
		}
		static Destination OnEffectList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsListDestination(importer, GetThis(importer).style.ContainerEffect);
		}
		static Destination OnEffectScene3D(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new Scene3DPropertiesDestination(importer, GetThis(importer).style.Scene3DProperties);
		}
		static Destination OnEffectSp3D(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new Shape3DPropertiesDestination(importer, GetThis(importer).style.Shape3DProperties);
		}
		#endregion
		readonly DrawingEffectStyle style;
		public OfficeThemeEffectStyleDestination(DestinationAndXmlBasedImporter importer, DrawingEffectStyle style)
			: base(importer) {
			Guard.ArgumentNotNull(style, "DrawingEffectStyle");
			this.style = style;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region OfficeThemeLineStyleListDestination
	public class OfficeThemeLineStyleListDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("ln", OnLineStyle);
			return result;
		}
		static Destination OnLineStyle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			Outline lineStyle = new Outline(importer.ActualDocumentModel);
			importer.DocumentModel.OfficeTheme.FormatScheme.LineStyleList.Add(lineStyle);
			return new OutlineDestination(importer, lineStyle);
		}
		#endregion
		public OfficeThemeLineStyleListDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
}

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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.OpenXml.Export;
namespace DevExpress.Office.Import.OpenXml {
	#region DrawingBlipFillDestination
	public class DrawingBlipFillDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("blip", OnBlip);
			result.Add("srcRect", OnSourceRectangle);
			result.Add("stretch", OnStretch);
			result.Add("tile", OnTile);
			return result;
		}
		static DrawingBlipFillDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingBlipFillDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingBlipFill fill;
		public DrawingBlipFillDestination(DestinationAndXmlBasedImporter importer, DrawingBlipFill fill)
			: base(importer) {
			this.fill = fill;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnBlip(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingBlipDestination(importer, GetThis(importer).fill.Blip);
		}
		static Destination OnSourceRectangle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingBlipFill fill = GetThis(importer).fill;
			return new RelativeRectangleDestination(importer,
				delegate(RectangleOffset value) { fill.SourceRectangle = value; });
		}
		static Destination OnStretch(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingBlipFillStretchDestination(importer, GetThis(importer).fill);
		}
		static Destination OnTile(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingBlipFillTileDestination(importer, GetThis(importer).fill);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			fill.BeginUpdate();
			fill.Dpi = Importer.GetIntegerValue(reader, "dpi", 0);
			fill.RotateWithShape = Importer.GetWpSTOnOffValue(reader, "rotWithShape", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			fill.EndUpdate();
		}
	}
	#endregion
	#region DrawingBlipFillStretchDestination
	public class DrawingBlipFillStretchDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("fillRect", OnFillRectangle);
			return result;
		}
		static DrawingBlipFillStretchDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingBlipFillStretchDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingBlipFill fill;
		public DrawingBlipFillStretchDestination(DestinationAndXmlBasedImporter importer, DrawingBlipFill fill)
			: base(importer) {
			this.fill = fill;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnFillRectangle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingBlipFill fill = GetThis(importer).fill;
			return new RelativeRectangleDestination(importer,
				delegate(RectangleOffset value) { fill.FillRectangle = value; });
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			fill.Stretch = true;
		}
	}
	#endregion
	#region DrawingBlipFillTileDestination
	public class DrawingBlipFillTileDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		static readonly Dictionary<string, RectangleAlignType> tileAlignTable = Utils.DictionaryUtils.CreateBackTranslationTable<RectangleAlignType>(OpenXmlExporterBase.RectangleAlignTypeTable);
		static readonly Dictionary<string, TileFlipType> tileFlipTable = Utils.DictionaryUtils.CreateBackTranslationTable<TileFlipType>(OpenXmlExporterBase.TileFlipTypeTable);
		readonly DrawingBlipFill fill;
		public DrawingBlipFillTileDestination(DestinationAndXmlBasedImporter importer, DrawingBlipFill fill)
			: base(importer) {
			this.fill = fill;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			fill.Stretch = false;
			fill.TileAlign = Importer.GetWpEnumValue(reader, "algn", tileAlignTable, RectangleAlignType.TopLeft);
			fill.TileFlip = Importer.GetWpEnumValue(reader, "flip", tileFlipTable, TileFlipType.None);
			fill.ScaleX = Importer.GetIntegerValue(reader, "sx", 0);
			fill.ScaleY = Importer.GetIntegerValue(reader, "sy", 0);
			fill.OffsetX = Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(Importer.GetLongValue(reader, "tx", 0));
			fill.OffsetY = Importer.DocumentModel.UnitConverter.EmuToModelUnitsL(Importer.GetLongValue(reader, "ty", 0));
		}
	}
	#endregion
	#region DrawingBlipDestination
	public class DrawingBlipDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("alphaBiLevel", OnAlphaBiLevelEffect);
			result.Add("alphaCeiling", OnAlphaCeilingEffect);
			result.Add("alphaFloor", OnAlphaFloorEffect);
			result.Add("alphaInv", OnAlphaInverseEffect);
			result.Add("alphaMod", OnAlphaModulateEffect);
			result.Add("alphaModFix", OnAlphaModulateFixedEffect);
			result.Add("alphaRepl", OnAlphaReplaceEffect);
			result.Add("alphaOutset", OnAlphaOutsetEffect);
			result.Add("biLevel", OnBiLevelEffect);
			result.Add("blend", OnBlendEffect);
			result.Add("blur", OnBlurEffect);
			result.Add("clrChange", OnColorChangeEffect);
			result.Add("clrRepl", OnSolidColorReplacement);
			result.Add("duotone", OnDuotoneEffect);
			result.Add("fillOverlay", OnFillOverlayEffect);
			result.Add("grayscl", OnGrayScaleEffect);
			result.Add("hsl", OnHueSaturationLuminanceEffect);
			result.Add("lum", OnLuminanceEffect);
			result.Add("tint", OnTintEffect);
			result.Add("extLst", OnExtensionList);
			return result;
		}
		static DrawingBlipDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingBlipDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingBlip blip;
		public DrawingBlipDestination(DestinationAndXmlBasedImporter importer, DrawingBlip blip)
			: base(importer) {
			this.blip = blip;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DrawingBlip Blip { get { return blip; } }
		protected internal DrawingEffectCollection Effects { get { return blip.Effects; } }
		#endregion
		#region Handlers
		static Destination OnAlphaBiLevelEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaBiLevelEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaCeilingEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaCeilingEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaFloorEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaFloorEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaInverseEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaInverseEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaModulateEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaModulateEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaModulateFixedEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaModulateFixedEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaReplaceEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaReplaceEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaOutsetEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaOutsetEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnBiLevelEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new BiLevelEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnBlendEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new BlendEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnBlurEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new BlurEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnColorChangeEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorChangeEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnSolidColorReplacement(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new SolidColorReplacementDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnDuotoneEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DuotoneEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnFillOverlayEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new FillOverlayEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnGrayScaleEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new GrayScaleEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnHueSaturationLuminanceEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new HSLEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnLuminanceEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new LuminanceEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnTintEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new TintEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnExtensionList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			blip.CompressionState = Importer.GetWpEnumValue(reader, "cstate", OpenXmlExporterBase.CompressionStateTable, CompressionState.None);
			string embedRelationId = reader.GetAttribute("embed", Importer.RelationsNamespace);
			if (!String.IsNullOrEmpty(embedRelationId))
				blip.Image = GetEmbededImage(blip.DocumentModel, embedRelationId);
			else {
				string linkRelationId = reader.GetAttribute("link", Importer.RelationsNamespace);
				if (!String.IsNullOrEmpty(linkRelationId))
					SetExternalOfficeImage(blip, linkRelationId);
			}
		}
		OfficeImage GetEmbededImage(IDocumentModel documentModel, string relationId) {
			return GetImageCore(Importer.LookupImageByRelationId(blip.DocumentModel, relationId, Importer.DocumentRootFolder));
		}
		OfficeImage GetImageCore(OfficeImage image) {
			return image != null ? image : UriBasedOfficeImageBase.CreatePlaceHolder(Importer.DocumentModel, 28, 28);
		}
		void SetExternalOfficeImage(DrawingBlip blip, string relationId) {
			OpenXmlRelation relation = Importer.LookupExternalRelationById(relationId);
			if (relation == null)
				return;
			blip.Image = GetImageCore(Importer.LookupExternalImageByRelation(relation, Importer.DocumentRootFolder));
			blip.SetExternal(relation.Target);
		}
	}
	#endregion
}

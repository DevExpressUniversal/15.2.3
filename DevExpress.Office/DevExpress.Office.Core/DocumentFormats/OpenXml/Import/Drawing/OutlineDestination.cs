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
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.OpenXml.Export;
namespace DevExpress.Office.Import.OpenXml {
	#region OutlineDestination
	public class OutlineDestination : DrawingFillPartDestinationBase {
		#region Static Members
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("bevel", OnLineJoinBevel);
			result.Add("headEnd", OnHeadEndStyle);
			result.Add("miter", OnMiterLineJoin);
			result.Add("prstDash", OnPresetDash);
			result.Add("round", OnRoundLineJoin);
			result.Add("tailEnd", OnTailEndStyle);
			AddFillPartHandlers(result);
			return result;
		}
		#endregion
		static OutlineDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (OutlineDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnLineJoinBevel(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineJoinBevelDestination(importer, GetThis(importer).outline);
		}
		static Destination OnHeadEndStyle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineHeadEndStyleDestination(importer, GetThis(importer).outline);
		}
		static Destination OnMiterLineJoin(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineMiterLineJoinDestination(importer, GetThis(importer).outline);
		}
		static Destination OnPresetDash(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlinePresetDashDestination(importer, GetThis(importer).outline);
		}
		static Destination OnRoundLineJoin(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineRoundLineJoinDestination(importer, GetThis(importer).outline);
		}
		static Destination OnTailEndStyle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineTailEndStyleDestination(importer, GetThis(importer).outline);
		}
		#endregion
		#endregion
		readonly Outline outline;
		public OutlineDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer) {
			this.outline = outline;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			outline.BeginUpdate();
			outline.EndCapStyle = Importer.GetWpEnumValue(reader, "cap", OpenXmlExporterBase.EndCapStyleTable, OutlineInfo.DefaultInfo.EndCapStyle);
			OutlineCompoundType? compoundType = Importer.GetWpEnumOnOffNullValue(reader, "cmpd", OpenXmlExporterBase.CompoundTypeTable);
			if (compoundType.HasValue)
				outline.CompoundType = compoundType.Value;
			outline.StrokeAlignment = Importer.GetWpEnumValue(reader, "algn", OpenXmlExporterBase.StrokeAlignmentTable, OutlineInfo.DefaultInfo.StrokeAlignment);
			int? width = Importer.GetIntegerNullableValue(reader, "w");
			if (width.HasValue) {
				int widthValue = width.Value;
				DrawingValueChecker.CheckOutlineWidth(widthValue, "outlineWidth");
				outline.Width = Importer.DocumentModel.UnitConverter.EmuToModelUnits(widthValue);
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (Fill != null)
				outline.Fill = Fill;
			outline.EndUpdate();
		}
	}
	#endregion
	#region OutlinePropertiesDestinationBase
	public class OutlinePropertiesDestinationBase : LeafElementDestination<DestinationAndXmlBasedImporter> {
		readonly Outline outline;
		public OutlinePropertiesDestinationBase(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer) {
			this.outline = outline;
		}
		protected Outline Outline { get { return outline; } }
	}
	#endregion
	#region OutlineJoinBevelDestination
	public class OutlineJoinBevelDestination : OutlinePropertiesDestinationBase {
		public OutlineJoinBevelDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer, outline) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Outline.JoinStyle = LineJoinStyle.Bevel;
		}
	}
	#endregion
	#region OutlineHeadEndStyleDestination
	public class OutlineHeadEndStyleDestination : OutlinePropertiesDestinationBase {
		public OutlineHeadEndStyleDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer, outline) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Outline.HeadLength = Importer.GetWpEnumValue(reader, "len", OpenXmlExporterBase.HeadTailSizeTable, OutlineInfo.DefaultInfo.HeadLength);
			Outline.HeadType = Importer.GetWpEnumValue(reader, "type", OpenXmlExporterBase.HeadTailTypeTable, OutlineInfo.DefaultInfo.HeadType);
			Outline.HeadWidth = Importer.GetWpEnumValue(reader, "w", OpenXmlExporterBase.HeadTailSizeTable, OutlineInfo.DefaultInfo.HeadWidth);
		}
	}
	#endregion
	#region OutlineMiterLineJoinDestination
	public class OutlineMiterLineJoinDestination : OutlinePropertiesDestinationBase {
		public OutlineMiterLineJoinDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer, outline) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Outline.JoinStyle = LineJoinStyle.Miter;
			int miterLimit = Importer.GetIntegerValue(reader, "lim", OutlineInfo.DefaultInfo.MiterLimit);
			if (miterLimit < 0)
				Importer.ThrowInvalidFile();
			Outline.MiterLimit = miterLimit;
		}
	}
	#endregion
	#region OutlinePresetDashDestination
	public class OutlinePresetDashDestination : OutlinePropertiesDestinationBase {
		public OutlinePresetDashDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer, outline) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Outline.Dashing = Importer.GetWpEnumValue(reader, "val", OpenXmlExporterBase.PresetDashTable, OutlineInfo.DefaultInfo.Dashing);
		}
	}
	#endregion
	#region OutlineRoundLineJoinDestination
	public class OutlineRoundLineJoinDestination : OutlinePropertiesDestinationBase {
		public OutlineRoundLineJoinDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer, outline) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Outline.JoinStyle = LineJoinStyle.Round;
		}
	}
	#endregion
	#region OutlineTailEndStyleDestination
	public class OutlineTailEndStyleDestination : OutlinePropertiesDestinationBase {
		public OutlineTailEndStyleDestination(DestinationAndXmlBasedImporter importer, Outline outline)
			: base(importer, outline) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Outline.TailLength = Importer.GetWpEnumValue(reader, "len", OpenXmlExporterBase.HeadTailSizeTable, OutlineInfo.DefaultInfo.TailLength);
			Outline.TailType = Importer.GetWpEnumValue(reader, "type", OpenXmlExporterBase.HeadTailTypeTable, OutlineInfo.DefaultInfo.TailType);
			Outline.TailWidth = Importer.GetWpEnumValue(reader, "w", OpenXmlExporterBase.HeadTailSizeTable, OutlineInfo.DefaultInfo.TailWidth);
		}
	}
	#endregion
}

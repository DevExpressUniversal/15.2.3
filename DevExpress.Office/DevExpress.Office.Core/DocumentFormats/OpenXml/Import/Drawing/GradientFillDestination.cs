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
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.OpenXml.Export;
namespace DevExpress.Office.Import.OpenXml {
	#region DrawingGradientFillDestination
	public class DrawingGradientFillDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		static readonly Dictionary<string, TileFlipType> tileFlipTable = Utils.DictionaryUtils.CreateBackTranslationTable<TileFlipType>(OpenXmlExporterBase.TileFlipTypeTable);
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("gsLst", OnGradientStopList);
			result.Add("lin", OnLinearGradient);
			result.Add("path", OnPathGradient);
			result.Add("tileRect", OnTileRectangle);
			return result;
		}
		static DrawingGradientFillDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingGradientFillDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingGradientFill fill;
		public DrawingGradientFillDestination(DestinationAndXmlBasedImporter importer, DrawingGradientFill fill)
			: base(importer) {
			this.fill = fill;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnGradientStopList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingGradientStopListDestination(importer, GetThis(importer).fill);
		}
		static Destination OnLinearGradient(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingGradientLinearDestination(importer, GetThis(importer).fill);
		}
		static Destination OnPathGradient(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingGradientPathDestination(importer, GetThis(importer).fill);
		}
		static Destination OnTileRectangle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingGradientFill fill = GetThis(importer).fill;
			return new RelativeRectangleDestination(importer,
				delegate(RectangleOffset value) { fill.TileRect = value; });
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			fill.BeginUpdate();
			fill.Flip = Importer.GetWpEnumValue(reader, "flip", tileFlipTable, TileFlipType.None);
			fill.RotateWithShape = Importer.GetWpSTOnOffValue(reader, "rotWithShape", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			fill.EndUpdate();
		}
	}
	#endregion
	#region DrawingGradientPathDestination
	public class DrawingGradientPathDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		static readonly Dictionary<string, GradientType> gradientTypeTable = Utils.DictionaryUtils.CreateBackTranslationTable<GradientType>(OpenXmlExporterBase.GradientTypeTable);
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("fillToRect", OnFillRectangle);
			return result;
		}
		static DrawingGradientPathDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingGradientPathDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingGradientFill fill;
		public DrawingGradientPathDestination(DestinationAndXmlBasedImporter importer, DrawingGradientFill fill)
			: base(importer) {
			this.fill = fill;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnFillRectangle(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingGradientFill fill = GetThis(importer).fill;
			return new RelativeRectangleDestination(importer,
				delegate(RectangleOffset value) { fill.FillRect = value; });
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			fill.GradientType = Importer.GetWpEnumValue<GradientType>(reader, "path", gradientTypeTable, GradientType.Circle);
		}
	}
	#endregion
	#region DrawingGradientLinearDestination
	public class DrawingGradientLinearDestination : LeafElementDestination<DestinationAndXmlBasedImporter> {
		readonly DrawingGradientFill fill;
		public DrawingGradientLinearDestination(DestinationAndXmlBasedImporter importer, DrawingGradientFill fill)
			: base(importer) {
			this.fill = fill;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			fill.GradientType = GradientType.Linear;
			fill.Angle = Importer.DocumentModel.UnitConverter.AdjAngleToModelUnits(Importer.GetWpSTIntegerValue(reader, "ang", 0));
			fill.Scaled = Importer.GetWpSTOnOffValue(reader, "scaled", true);
		}
	}
	#endregion
	#region DrawingGradientStopListDestination
	public class DrawingGradientStopListDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("gs", OnGradientStop);
			return result;
		}
		static DrawingGradientStopListDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingGradientStopListDestination)importer.PeekDestination();
		}
		#endregion
		readonly DrawingGradientFill fill;
		public DrawingGradientStopListDestination(DestinationAndXmlBasedImporter importer, DrawingGradientFill fill)
			: base(importer) {
			this.fill = fill;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnGradientStop(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingGradientFill fill = GetThis(importer).fill;
			DrawingGradientStop gradientStop = new DrawingGradientStop(fill.DocumentModel);
			fill.GradientStops.Add(gradientStop);
			return new DrawingGradientStopDestination(importer, gradientStop);
		}
		#endregion
	}
	#endregion
	#region DrawingGradientStopDestination
	public class DrawingGradientStopDestination : DrawingColorDestination {
		readonly DrawingGradientStop gradientStop;
		public DrawingGradientStopDestination(DestinationAndXmlBasedImporter importer, DrawingGradientStop gradientStop)
			: base(importer, gradientStop.Color) {
			this.gradientStop = gradientStop;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			this.gradientStop.Position = Importer.GetWpSTIntegerValue(reader, "pos");
		}
	}
	#endregion
}

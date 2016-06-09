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
using DevExpress.Office.Import.OpenXml;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml{
	#region PictureDestination
	public class PictureDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("nvPicPr", OnNonVisualPictureProperties);
			result.Add("blipFill", OnBlipFillProperties);
			result.Add("spPr", OnShapeProperties);
			result.Add("AlternateContent", OnAlternateContent);
			result.Add("Choice", OnChoice);
			result.Add("Fallback", OnFallback);
			return result;
		}
		#endregion
		static int refs;
		readonly Picture picture;
		readonly DrawingObject drawingObjectInfo;
		public PictureDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObjectInfo)
			: base(importer) {
			this.drawingObjectInfo = drawingObjectInfo;
			this.drawingObjectInfo.EndUpdate();
			picture = new Picture(this.drawingObjectInfo);
			picture.BeginUpdate();
			refs = 0;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static PictureDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PictureDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnNonVisualPictureProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualPicturePropertiesDestination(importer, GetThis(importer).picture);
		}
		static Destination OnBlipFillProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingBlipFillDestination(importer, GetThis(importer).picture.PictureFill);
		}
		static Destination OnShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ShapePropertiesDestination(importer, GetThis(importer).picture.ShapeProperties);
		}
		static Destination OnAlternateContent(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		static Destination OnChoice(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		static Destination OnFallback(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			if (refs == 0) {
				picture.IsPublished = Importer.GetWpSTOnOffValue(reader, "fPublished", false);
				string macro = Importer.ReadAttribute(reader, "macro");
				if (String.IsNullOrEmpty(macro))
					macro = "";
				picture.Macro = macro;
			}
			refs++;
		}
		public override void ProcessElementClose(XmlReader reader) {
			refs--;
			if (refs != 0)
				return;
			picture.EndUpdate();
			Importer.CurrentDrawingObjectsContainer.DrawingObjects.Add(picture);
			drawingObjectInfo.BeginUpdate();
			picture.CheckRotationAndSwapBox();
		}
	}
	#endregion
	#region NonVisualPicturePropertiesDestination
	public class NonVisualPicturePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cNvPr", OnNonVisualDrawingProps);
			result.Add("cNvPicPr", OnNonVisualPictureProperties);
			return result;
		}
		#endregion
		readonly Picture picture;
		public NonVisualPicturePropertiesDestination(SpreadsheetMLBaseImporter importer, Picture picture)
			: base(importer) {
			this.picture = picture;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualPicturePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualPicturePropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnNonVisualPictureProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualPictureDrawingPropertiesDestination(importer, GetThis(importer).picture);
		}
		static Destination OnNonVisualDrawingProps(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualDrawingProps(importer, GetThis(importer).picture.DrawingObject.Properties);
		}
		#endregion
	}
	#endregion
	#region NonVisualPictureDrawingPropertiesDestination
	public class NonVisualPictureDrawingPropertiesDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("picLocks", OnPictureLocking);
			result.Add("extLst", OnExtensionList);
			return result;
		}
		#endregion
		readonly Picture picture;
		public NonVisualPictureDrawingPropertiesDestination(SpreadsheetMLBaseImporter importer, Picture picture)
			: base(importer) {
			this.picture = picture;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualPictureDrawingPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualPictureDrawingPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPictureLocking(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PictureLockingDestination(importer, GetThis(importer).picture);
		}
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			picture.Properties.PreferRelativeResize = Importer.GetWpSTOnOffValue(reader, "preferRelativeResize", true);
		}
	}
	#endregion
	#region CommonDrawingLockingDestination
	public class CommonDrawingLockingDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		readonly ICommonDrawingLocks commonDrawingLocks;
		public CommonDrawingLockingDestination(SpreadsheetMLBaseImporter importer, ICommonDrawingLocks commonDrawingLocks)
			: base(importer) {
			this.commonDrawingLocks = commonDrawingLocks;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			commonDrawingLocks.NoChangeAspect = Importer.GetWpSTOnOffValue(reader, "noChangeAspect", false);
			commonDrawingLocks.NoGroup = Importer.GetWpSTOnOffValue(reader, "noGrp", false);
			commonDrawingLocks.NoMove = Importer.GetWpSTOnOffValue(reader, "noMove", false);
			commonDrawingLocks.NoSelect = Importer.GetWpSTOnOffValue(reader, "noSelect", false);
		}
	}
	#endregion
	#region DrawingLockingDestination
	public class DrawingLockingDestination : CommonDrawingLockingDestination {
		readonly IDrawingLocks drawingLocks;
		public DrawingLockingDestination(SpreadsheetMLBaseImporter importer, IDrawingLocks drawingLocks)
			: base(importer, drawingLocks) {
			this.drawingLocks = drawingLocks;
		}
		#region Overrides of CommonDrawingLockingDestination
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			drawingLocks.NoAdjustHandles = Importer.GetWpSTOnOffValue(reader, "noAdjustHandles", false);
			drawingLocks.NoChangeArrowheads = Importer.GetWpSTOnOffValue(reader, "noChangeArrowheads", false);
			drawingLocks.NoChangeShapeType = Importer.GetWpSTOnOffValue(reader, "noChangeShapeType", false);
			drawingLocks.NoEditPoints = Importer.GetWpSTOnOffValue(reader, "noEditPoints", false);
			drawingLocks.NoResize = Importer.GetWpSTOnOffValue(reader, "noResize", false);
			drawingLocks.NoRotate = Importer.GetWpSTOnOffValue(reader, "noRot", false);
		}
		#endregion
	}
	#endregion
	#region PictureLockingDestination
	public class PictureLockingDestination : DrawingLockingDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("extLst", OnExtensionList);
			return result;
		}
		#endregion
		Picture picture;
		public PictureLockingDestination(SpreadsheetMLBaseImporter importer, Picture picture)
			: base(importer, picture.Locks) {
			this.picture = picture;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			picture.Locks.NoCrop = Importer.GetWpSTOnOffValue(reader, "noCrop", false);
		}
	}
	#endregion
	#region NonVisualDrawingProps
	public class NonVisualDrawingProps : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("extLst", OnExtensionList);
			result.Add("hlinkClick", OnClickHyperLink);
			return result;
		}
		static NonVisualDrawingProps GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualDrawingProps)importer.PeekDestination();
		}
		#endregion
		IDrawingObjectNonVisualProperties drawingObjectNonVisualProperties;
		public NonVisualDrawingProps(SpreadsheetMLBaseImporter importer, IDrawingObjectNonVisualProperties drawingObjectNonVisualProperties)
			: base(importer) {
			this.drawingObjectNonVisualProperties = drawingObjectNonVisualProperties;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		static Destination OnClickHyperLink(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ClickHyperLinkDestination(importer, GetThis(importer).drawingObjectNonVisualProperties);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			int id = Importer.GetIntegerValue(reader, "id", int.MinValue);
			Guard.ArgumentNonNegative(id, "DrawingObjectId");
			drawingObjectNonVisualProperties.Id = id;
			string name = Importer.ReadAttribute(reader, "name");
			Guard.ArgumentNotNull(name, "DrawingObjectName");
			drawingObjectNonVisualProperties.Name = name;
			string desctiption = Importer.ReadAttribute(reader, "descr");
			if(desctiption == null)
				desctiption = "";
			drawingObjectNonVisualProperties.Description = desctiption;
			drawingObjectNonVisualProperties.Hidden = Importer.GetWpSTOnOffValue(reader, "hidden", false);
		}
	}
	#endregion
	#region ClickHyperLinkDestination
	public class ClickHyperLinkDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("extLst", OnExtensionList);
			return result;
		}
		#endregion
		IDrawingObjectNonVisualProperties drawingObjectNonVisualProperties;
		public ClickHyperLinkDestination(SpreadsheetMLBaseImporter importer, IDrawingObjectNonVisualProperties drawingObjectNonVisualProperties)
			: base(importer) {
			this.drawingObjectNonVisualProperties = drawingObjectNonVisualProperties;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		#endregion
		#region Handlers
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			string relationId = reader.GetAttribute("id", Importer.RelationsNamespace);
			if (String.IsNullOrEmpty(relationId))
				return;
			OpenXmlRelation relation = Importer.DocumentRelations.LookupRelationById(relationId);
			drawingObjectNonVisualProperties.HyperlinkClickUrl = relation.Target;
			if (!String.IsNullOrEmpty(relation.TargetMode))
				drawingObjectNonVisualProperties.HyperlinkClickIsExternal = true;
			string tgtFrame = Importer.ReadAttribute(reader, "tgtFrame");
			if (String.IsNullOrEmpty(tgtFrame))
				tgtFrame = "";
			drawingObjectNonVisualProperties.HyperlinkClickTargetFrame = tgtFrame;
			string tooltip = Importer.ReadAttribute(reader, "tooltip");
			if (String.IsNullOrEmpty(tooltip))
				tooltip = "";
			drawingObjectNonVisualProperties.HyperlinkClickTooltip = tooltip;
		}
	}
	#endregion
	#region HoverHyperLinkDestination
	#endregion
	#region HyperlinkSoundDestination
	#endregion
	#region BlipFillPropertiesDestination
	#endregion
	#region RectangleBlipFillDestination
	#endregion
	#region StretchBlipFillDestination
	#endregion
	#region BlipPictureDestination
	#endregion
}

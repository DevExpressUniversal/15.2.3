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
using DevExpress.Office.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region GroupShapeDestination
	public class GroupShapeDestination : CommonDrawingObjectsDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("grpSpPr", OnGroupShapeProperties);
			result.Add("nvGrpSpPr", OnNonVisualGroupShapeProperties);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		readonly DrawingObject drawingObject;
		readonly GroupShape groupShape;
		readonly IDrawingObjectsContainer previousContainer;
		public GroupShapeDestination(SpreadsheetMLBaseImporter importer, DrawingObject drawingObject)
			: base(importer, null) {
			this.drawingObject = drawingObject;
			this.drawingObject.EndUpdate();
			groupShape = new GroupShape(this.drawingObject);
			GroupShape.BeginUpdate();
			previousContainer = importer.CurrentDrawingObjectsContainer;
			importer.CurrentDrawingObjectsContainer = GroupShape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public GroupShape GroupShape { get { return groupShape; } }
		#endregion
		#region Handlers
		static Destination OnGroupShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GroupShapePropertiesDestination(importer, GetThis(importer).GroupShape.GroupShapeProperties);
		}
		static Destination OnNonVisualGroupShapeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualGroupShapePropertiesDestination(importer, GetThis(importer).GroupShape);
		}
		#endregion
		static GroupShapeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GroupShapeDestination)importer.PeekDestination();
		}
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementClose(XmlReader reader) {
			GroupShape.EndUpdate();
			Importer.CurrentDrawingObjectsContainer = previousContainer;
			if(GroupShape.DrawingObjects.Count != 0)
				Importer.CurrentDrawingObjectsContainer.DrawingObjects.Add(GroupShape);
			drawingObject.BeginUpdate();
		}
		#endregion
		#region Overrides of CommonDrawingObjectsDestination
		protected override DrawingObject GetDrawingObject() {
			DrawingObject newDrawingObject = new DrawingObject(Importer.CurrentSheet);
			newDrawingObject.BeginUpdate();
			return newDrawingObject;
		}
		#endregion
	}
	#endregion
	#region GroupShapePropertiesDestination
	public class GroupShapePropertiesDestination : DrawingFillDestinationBase {
		public static Dictionary<OpenXmlBlackWhiteMode, string> BlackWhiteModeTable = CreateBlackWhiteModeTable();
		static Dictionary<OpenXmlBlackWhiteMode, string> CreateBlackWhiteModeTable() {
			Dictionary<OpenXmlBlackWhiteMode, string> result = new Dictionary<OpenXmlBlackWhiteMode, string>();
			result.Add(OpenXmlBlackWhiteMode.Auto, "auto");
			result.Add(OpenXmlBlackWhiteMode.Black, "black");
			result.Add(OpenXmlBlackWhiteMode.BlackGray, "blackGray");
			result.Add(OpenXmlBlackWhiteMode.BlackWhite, "blackWhite");
			result.Add(OpenXmlBlackWhiteMode.Clr, "clr");
			result.Add(OpenXmlBlackWhiteMode.Gray, "gray");
			result.Add(OpenXmlBlackWhiteMode.GrayWhite, "grayWhite");
			result.Add(OpenXmlBlackWhiteMode.Hidden, "hidden");
			result.Add(OpenXmlBlackWhiteMode.InvGray, "invGray");
			result.Add(OpenXmlBlackWhiteMode.LtGray, "ltGray");
			result.Add(OpenXmlBlackWhiteMode.White, "white");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("xfrm", OnXfrm);
			result.Add("effectDag", OnEffectGraph);
			result.Add("effectLst", OnEffectList);
			result.Add("scene3d", OnScene3DProperties);
			AddFillHandlers(result);
			return result;
		}
		#endregion
		#region Handlers
		static GroupShapePropertiesDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (GroupShapePropertiesDestination)importer.PeekDestination();
		}
		static Destination OnXfrm(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new GroupXfrmDestination((SpreadsheetMLBaseImporter)importer, GetThis(importer).groupShapeProperties.Transform2D);
		}
		static Destination OnEffectGraph(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsDAGDestination(importer, GetThis(importer).groupShapeProperties.EffectStyle.ContainerEffect);
		}
		static Destination OnEffectList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsListDestination(importer, GetThis(importer).groupShapeProperties.EffectStyle.ContainerEffect);
		}
		static Destination OnScene3DProperties(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new Scene3DPropertiesDestination(importer, GetThis(importer).groupShapeProperties.EffectStyle.Scene3DProperties);
		}
		#endregion
		#region Fields
		readonly GroupShapeProperties groupShapeProperties;
		#endregion
		public GroupShapePropertiesDestination(DestinationAndXmlBasedImporter importer, GroupShapeProperties groupShapeProperties)
			: base(importer) {
			this.groupShapeProperties = groupShapeProperties;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			groupShapeProperties.BlackAndWhiteMode = Importer.GetWpEnumValue(reader, "bwMode", BlackWhiteModeTable, OpenXmlBlackWhiteMode.Empty);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(Fill != null)
				groupShapeProperties.Fill = Fill;
		}
	}
	#endregion
	#region NonVisualGroupShapePropertiesDestination
	public class NonVisualGroupShapePropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cNvGrpSpPr", OnNonVisualGroupShapeDrawingProperties);
			result.Add("cNvPr", OnNonVisualDrawingProperties);
			return result;
		}
		#endregion
		readonly GroupShape groupShape;
		public NonVisualGroupShapePropertiesDestination(SpreadsheetMLBaseImporter importer, GroupShape groupShape)
			: base(importer) {
			this.groupShape = groupShape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualGroupShapePropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualGroupShapePropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnNonVisualGroupShapeDrawingProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualGroupShapeDrawingPropertiesDestination(importer, GetThis(importer).groupShape);
		}
		static Destination OnNonVisualDrawingProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new NonVisualDrawingProps(importer, GetThis(importer).groupShape.DrawingObject.Properties);
		}
		#endregion
	}
	#endregion
	#region NonVisualGroupShapeDrawingPropertiesDestination
	public class NonVisualGroupShapeDrawingPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("grpSpLocks", OnGroupShapeLocks);
			return result;
		}
		#endregion
		readonly GroupShape groupShape;
		public NonVisualGroupShapeDrawingPropertiesDestination(SpreadsheetMLBaseImporter importer, GroupShape groupShape)
			: base(importer) {
			this.groupShape = groupShape;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static NonVisualGroupShapeDrawingPropertiesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (NonVisualGroupShapeDrawingPropertiesDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnGroupShapeLocks(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GroupShapeLocksDestination(importer, GetThis(importer).groupShape);
		}
		#endregion
	}
	#endregion
	#region GroupShapeLocksDestination
	public class GroupShapeLocksDestination : CommonDrawingLockingDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		readonly GroupShape groupShape;
		public GroupShapeLocksDestination(SpreadsheetMLBaseImporter importer, GroupShape groupShape)
			: base(importer, groupShape.Locks) {
			this.groupShape = groupShape;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#region Overrides of ElementDestination<SpreadsheetMLBaseImporter>
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			groupShape.Locks.NoResize = Importer.GetWpSTOnOffValue(reader, "noResize", false);
			groupShape.Locks.NoRotate = Importer.GetWpSTOnOffValue(reader, "noRot", false);
			groupShape.Locks.NoUngroup = Importer.GetWpSTOnOffValue(reader, "noUngrp", false);
		}
		#endregion
	}
	#endregion
	#region GroupXfrmDestination
	public class GroupXfrmDestination : XfrmDestination {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("chExt", OnChildExtents);
			result.Add("chOff", OnChildOffset);
			AddCommonHandlers(result);
			return result;
		}
		#endregion
		readonly GroupTransform2D groupXfrm;
		public GroupXfrmDestination(SpreadsheetMLBaseImporter importer, GroupTransform2D groupXfrm)
			: base(importer, groupXfrm.MainTransform) {
			this.groupXfrm = groupXfrm;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		static GroupXfrmDestination GetThisGroupXlfrm(SpreadsheetMLBaseImporter importer) {
			return (GroupXfrmDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnChildExtents(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Transform2D xfrm = GetThisGroupXlfrm(importer).groupXfrm.ChildTransform;
			return new ExtentsDestination(importer, xfrm);
		}
		static Destination OnChildOffset(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Transform2D xfrm = GetThisGroupXlfrm(importer).groupXfrm.ChildTransform;
			return new OffsetDestination(importer, xfrm);
		}
		#endregion
	}
	#endregion
}

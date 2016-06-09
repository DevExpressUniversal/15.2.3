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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using System.Collections.Generic;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ShapePropertiesDestinationBase (abstract class)
	public abstract class ShapePropertiesDestinationBase : DrawingFillDestinationBase {
		#region Handler Table
		protected static void AddCommonHandlers(ElementHandlerTable<DestinationAndXmlBasedImporter> table) {
			table.Add("ln", OnOutline);
			table.Add("effectDag", OnEffectGraph);
			table.Add("effectLst", OnEffectList);
			table.Add("sp3d", OnShape3DProperties);
			table.Add("scene3d", OnScene3DProperties);
			AddFillHandlers(table);
		}
		static ShapePropertiesDestinationBase GetThis(DestinationAndXmlBasedImporter importer) {
			return (ShapePropertiesDestinationBase)importer.PeekDestination();
		}
		#endregion
		readonly ShapeProperties shapeProperties;
		protected ShapePropertiesDestinationBase(DestinationAndXmlBasedImporter importer, ShapeProperties shapeProperties)
			: base(importer) {
			this.shapeProperties = shapeProperties;
		}
		#region Properties
		protected internal ShapeProperties ShapeProperties { get { return shapeProperties; } }
		#endregion
		#region Handlers
		static Destination OnOutline(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OutlineDestination(importer, GetThis(importer).shapeProperties.Outline);
		}
		static Destination OnEffectGraph(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsDAGDestination(importer, GetThis(importer).shapeProperties.EffectStyle.ContainerEffect);
		}
		static Destination OnEffectList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingEffectsListDestination(importer, GetThis(importer).shapeProperties.EffectStyle.ContainerEffect);
		}
		static Destination OnShape3DProperties(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new Shape3DPropertiesDestination(importer, GetThis(importer).shapeProperties.EffectStyle.Shape3DProperties);
		}
		static Destination OnScene3DProperties(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new Scene3DPropertiesDestination(importer, GetThis(importer).shapeProperties.EffectStyle.Scene3DProperties);
		}
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			if (Fill != null)
				shapeProperties.Fill = Fill;
		}
	}
	#endregion 
}

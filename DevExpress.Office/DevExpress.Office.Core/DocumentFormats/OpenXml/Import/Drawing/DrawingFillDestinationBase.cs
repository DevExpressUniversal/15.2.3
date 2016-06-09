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
using System.Collections.Generic;
using DevExpress.Office.Drawing;
namespace DevExpress.Office.Import.OpenXml {
	#region DrawingFillPartDestinationBase (abstract class)
	public abstract class DrawingFillPartDestinationBase : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Handler Table
		protected static void AddFillPartHandlers(ElementHandlerTable<DestinationAndXmlBasedImporter> table) {
			table.Add("gradFill", OnGradientFill);
			table.Add("solidFill", OnSolidFill);
			table.Add("pattFill", OnPatternFill);
			table.Add("noFill", OnNoFill);
		}
		#endregion
		static DrawingFillPartDestinationBase GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingFillPartDestinationBase)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnGradientFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingFillPartDestinationBase destination = GetThis(importer);
			DrawingGradientFill fill = new DrawingGradientFill(importer.ActualDocumentModel);
			destination.fill = fill;
			return new DrawingGradientFillDestination(importer, fill);
		}
		static Destination OnSolidFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingFillPartDestinationBase destination = GetThis(importer);
			DrawingSolidFill fill = new DrawingSolidFill(importer.ActualDocumentModel);
			destination.fill = fill;
			return new DrawingColorDestination(importer, fill.Color);
		}
		static Destination OnPatternFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingFillPartDestinationBase destination = GetThis(importer);
			DrawingPatternFill fill = new DrawingPatternFill(importer.ActualDocumentModel);
			destination.fill = fill;
			return new DrawingPatternFillDestination(importer, fill);
		}
		static Destination OnNoFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			GetThis(importer).fill = DrawingFill.None;
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		#endregion
		IDrawingFill fill;
		protected DrawingFillPartDestinationBase(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		protected internal IDrawingFill Fill { get { return fill; } set { fill = value; } }
	}
	#endregion
	#region DrawingFillDestinationBase (abstract class)
	public abstract class DrawingFillDestinationBase : DrawingFillPartDestinationBase {
		#region Handler Table
		protected static void AddFillHandlers(ElementHandlerTable<DestinationAndXmlBasedImporter> table) {
			table.Add("blipFill", OnPictureFill);
			table.Add("grpFill", OnGroupFill);
			AddFillPartHandlers(table);
		}
		#endregion
		static DrawingFillDestinationBase GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingFillDestinationBase)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPictureFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			DrawingFillDestinationBase destination = GetThis(importer);
			DrawingBlipFill fill = new DrawingBlipFill(importer.ActualDocumentModel);
			destination.Fill = fill;
			return new DrawingBlipFillDestination(importer, fill);
		}
		static Destination OnGroupFill(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			GetThis(importer).Fill = DrawingFill.Group;
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		#endregion
		protected DrawingFillDestinationBase(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
	}
	#endregion
}

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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.OpenXml;
using System.IO;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	public class AltChunkInfo {
		string relationId;
		PieceTable pieceTable;
		DocumentLogPosition logPosition;
		public AltChunkInfo(string relationId, DocumentLogPosition logPosition, PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.relationId = relationId;
			this.logPosition = logPosition;
			this.pieceTable = pieceTable;
		}
		public string RelationId { get { return relationId; }}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public PieceTable PieceTable { get { return pieceTable; } }
	}
	#region AltChunkDestination
	public class AltChunkDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("altChunkPr", OnAltChunkProperties);
			return result;
		}
		string relId;		
		public AltChunkDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static AltChunkDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (AltChunkDestination)importer.PeekDestination();
		}
		protected internal virtual AltChunkPropertiesDestination CreateAltChunkPropertiesDestination() {
			return new AltChunkPropertiesDestination(Importer);
		}
		protected static Destination OnAltChunkProperties(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer).CreateAltChunkPropertiesDestination();
		}		
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("id", OpenXmlExporter.RelsNamespace);
			if (String.IsNullOrEmpty(id))
				return;
			this.relId = id;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (String.IsNullOrEmpty(relId))
				return;
			OpenXmlImporter openXmlImporter = (OpenXmlImporter)Importer;
			openXmlImporter.AddAltChunkInfo(new AltChunkInfo(relId, openXmlImporter.Position.LogPosition, openXmlImporter.Position.PieceTable));
		}	   
	}
	#endregion
	#region AltChunkPropertiesDestination
	public class AltChunkPropertiesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		public AltChunkPropertiesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
	}
	#endregion
}

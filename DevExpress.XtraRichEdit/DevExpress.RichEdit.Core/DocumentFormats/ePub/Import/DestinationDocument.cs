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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.EPub {
	#region DocumentDestination
	public class DocumentDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("manifest", OnDocumentManifest);
			result.Add("spine", OnDocumentSpine);
			return result;
		}
		public DocumentDestination(EPubImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDocumentManifest(EPubImporter importer, XmlReader reader) {
			return new DocumentManifestDestination(importer);
		}
		static Destination OnDocumentSpine(EPubImporter importer, XmlReader reader) {
			return new DocumentSpineDestination(importer);
		}
	}
	#endregion
	#region DocumentManifestDestination
	public class DocumentManifestDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("item", OnDocumentItem);
			return result;
		}
		public DocumentManifestDestination(EPubImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnDocumentItem(EPubImporter importer, XmlReader reader) {
			return new DocumentItemDestination(importer);
		}
	}
	#endregion
	#region DocumentItemDestination
	public class DocumentItemDestination : LeafElementDestination {
		public DocumentItemDestination(EPubImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("id");
			string fileName = reader.GetAttribute("href");
			string mediaType = reader.GetAttribute("media-type");
			if (mediaType == "application/xhtml+xml" && !String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(id))
				Importer.HtmlFiles.Add(id, fileName);
			if (mediaType == "text/xml" && !String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(id))
				Importer.XmlFiles.Add(id, fileName);
			if (mediaType == "application/x-dtbncx+xml" && !String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(id))
				Importer.XmlFiles.Add(id, fileName);
		}
	}
	#endregion
	#region DocumentSpineDestination
	public class DocumentSpineDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("itemref", OnItemReferenceItem);
			return result;
		}
		public DocumentSpineDestination(EPubImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			Importer.TableOfContentsPartId = reader.GetAttribute("toc");
		}
		static Destination OnItemReferenceItem(EPubImporter importer, XmlReader reader) {
			return new DocumentItemReferenceDestination(importer);
		}
	}
	#endregion
	#region DocumentItemReferenceDestination
	public class DocumentItemReferenceDestination : LeafElementDestination {
		public DocumentItemReferenceDestination(EPubImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string id = reader.GetAttribute("idref");
			Importer.OrderedHtmlFiles.Add(id);
		}
	}
	#endregion
}

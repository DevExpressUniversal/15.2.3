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

#if OPENDOCUMENT
using System.Collections.Generic;
using DevExpress.Office;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	public delegate Destination ElementHandler(OpenDocumentWorkbookImporter importer, XmlReader reader);
	public class ElementHandlerTable : Dictionary<string, ElementHandler> {
	}
	#region ElementDestination
	public abstract class ElementDestination : Destination {
		protected ElementDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		internal virtual new OpenDocumentWorkbookImporter Importer { get { return (OpenDocumentWorkbookImporter)base.Importer; } }
		protected internal abstract ElementHandlerTable ElementHandlerTable { get; }
		protected override Destination ProcessCurrentElement(XmlReader reader) {
			string localName = reader.LocalName;
			ElementHandler handler;
			if (ElementHandlerTable.TryGetValue(localName, out handler))
				return handler(Importer, reader);
			else
				return null;
		}
		public override void ProcessElementOpen(XmlReader reader) {
		}
		public override void ProcessElementClose(XmlReader reader) {
		}
		public override bool ProcessText(XmlReader reader) {
			return true;
		}
	}
	#endregion
	#region LeafElementDestination
	public abstract class LeafElementDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = new ElementHandlerTable();
		protected LeafElementDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
}
#endif

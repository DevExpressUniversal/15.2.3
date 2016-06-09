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
using DevExpress.Office;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region DocumentDestination
	public class DocumentContentDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("automatic-styles", OnAutoStyles);
			result.Add("body", OnBody);
			result.Add("font-face-decls", OnFontFaceDecls);
			result.Add("styles", OnStyles);
			return result;
		}
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public DocumentContentDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		#region Handlers
		static Destination OnAutoStyles(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new AutoStylesDestination(importer);
		}
		static Destination OnBody(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new BodyDestination(importer);
		}
		static Destination OnFontFaceDecls(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new FontFaceDeclsDestination(importer);
		}
		static Destination OnStyles(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new StylesDestination(importer);
		}
		#endregion
	}
	#endregion
}
#endif

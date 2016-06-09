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
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.XtraRichEdit.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region DocumentDestination
	public class DocumentDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("body", OnBody);
			result.Add("background", OnBackground);
			result.Add("version", OnVersion);
			return result;
		}
		public DocumentDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnBody(WordProcessingMLBaseImporter importer, XmlReader reader) {
			((OpenXmlImporter)importer).CheckVersion();
			return new BodyDestination(importer);
		}
		static Destination OnBackground(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentBackgroundDestination(importer);
		}
		static Destination OnVersion(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateVersionDestination(reader); 
		}
	}
	#endregion
	#region DocumentBackgroundDestination
	public class DocumentBackgroundDestination : LeafElementDestination {
		public DocumentBackgroundDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color color = Importer.GetWpSTColorValue(reader, "color");
			if (color != DXColor.Empty) {
				Importer.DocumentModel.DocumentProperties.PageBackColor = color;
			}
#if THEMES_EDIT
			OpenXmlImportHelper helper = new OpenXmlImportHelper();
			Importer.DocumentModel.DocumentProperties.PageBackColorModelInfo = helper.SaveColorModelInfo(Importer, reader, "color");
#endif
		}
	}
	#endregion
	#region DocumentVersionDestination
	public class DocumentVersionDestination : LeafElementDestination {
		public DocumentVersionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return ElementHandlerTable.Empty; } }
	}
	#endregion
}

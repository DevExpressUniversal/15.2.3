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
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region DocumentDestination
	public class DocumentDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("body", OnBody);
			result.Add("styles", OnStyles);
			result.Add("lists", OnLists);
			result.Add("docPr", OnDocPr);
			result.Add("bgPict", OnBackgroundPictureInformation);
			return result;
		}
		public DocumentDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnBody(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.BodyDestination(importer);
		}
		static Destination OnStyles(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.StylesDestination(importer);
		}
		static Destination OnLists(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.NumberingsDestination(importer);
		}
		static Destination OnDocPr(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DocumentSettingsDestination(importer);
		}
		static Destination OnBackgroundPictureInformation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new BackgroundPictureInformationDestination(importer);
		}
	}
	#endregion
}

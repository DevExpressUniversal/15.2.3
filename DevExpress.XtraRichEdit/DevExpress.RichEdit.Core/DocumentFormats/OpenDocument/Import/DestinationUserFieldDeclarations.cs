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
using DevExpress.XtraRichEdit.Internal;
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region UserFieldDeclarationsDestination
	class UserFieldDeclarationsDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("user-field-decl", OnUserFieldDeclaration);
			FieldHandlers.AddFieldHandlers(result);
			return result;
		}
		public UserFieldDeclarationsDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnUserFieldDeclaration(OpenDocumentTextImporter importer, XmlReader reader) {
			return new UserFieldDeclarationDestination(importer);
		}
	}
	#endregion
	#region UserFieldDeclarationDestination
	class UserFieldDeclarationDestination : LeafElementDestination {
		public UserFieldDeclarationDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = ImportHelper.GetTextStringAttribute(reader, "name");
			if (String.IsNullOrEmpty(name))
				return;
			string stringValue = reader.GetAttribute("string-value", OpenDocumentHelper.OfficeNamespace);
			if (stringValue != null) {
				Importer.DocumentModel.Variables.Add(name, stringValue);
				return;
			}
			string stringFloatValue = reader.GetAttribute("value", OpenDocumentHelper.OfficeNamespace);
			float floatValue = 0;
			if (float.TryParse(stringFloatValue, out floatValue)) {
				Importer.DocumentModel.Variables.Add(name, floatValue);
			}
			string stringDateValue = reader.GetAttribute("date-value", OpenDocumentHelper.OfficeNamespace);
			DateTime dateValue = DateTime.MinValue;
			if (DateTime.TryParse(stringDateValue, out dateValue)) {
				Importer.DocumentModel.Variables.Add(name, dateValue);
			}
		}
	}
	#endregion
}

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

using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ConditionalFormattingRuleExtLstDestination
	public class ConditionalFormattingRuleExtDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		string uri;
		ConditionalFormattingCreatorData creatorData;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("id", OnIdentificator);
			return result;
		}
		static ConditionalFormattingRuleExtDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingRuleExtDestination)importer.PeekDestination();
		}
		#endregion
		public ConditionalFormattingRuleExtDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingCreatorData creatorData)
			: base(importer) {
			this.creatorData = creatorData;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		ConditionalFormattingCreatorData CreatorData { get { return creatorData; } }
		public string Uri { get { return uri; } protected set { uri = value; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			Uri = Importer.ReadAttribute(reader, "uri");
		}
		void IdValueAction(string value) {
			CreatorData.ExtId = value;
		}
		static Destination OnIdentificator(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingRuleExtDestination self = GetThis(importer);
			if(!String.IsNullOrEmpty(self.Uri) && (StringExtensions.CompareInvariantCultureIgnoreCase(self.Uri, ConditionalFormattingRuleDestination.ExtUri) == 0)) {
				return new ConditionalFormattingFormulaDestination(importer, self.IdValueAction);
			}
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
	}
	#endregion
}

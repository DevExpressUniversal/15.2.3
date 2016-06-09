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
using DevExpress.Office.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ConditionalFormattingColorScaleDestination
	public class ConditionalFormattingColorScaleDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		const int appendToObjectList = -1000000;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cfvo", OnValueObject);
			result.Add("color", OnColor);
			return result;
		}
		static ConditionalFormattingColorScaleDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingColorScaleDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		ConditionalFormattingCreatorInitData initData;
		ConditionalFormattingColorScaleCreatorData creatorData;
		#endregion
		internal ConditionalFormattingColorScaleDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingCreatorInitData initData)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(initData, "initData");
			this.initData = initData;
			this.creatorData = initData.Details as ConditionalFormattingColorScaleCreatorData;
		}
		internal ConditionalFormattingColorScaleDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingColorScaleCreatorData creatorData)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(creatorData, "creatorData");
			this.initData = null;
			this.creatorData = creatorData;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal ConditionalFormattingCreatorInitData InitData { get { return initData; } }
		internal ConditionalFormattingColorScaleCreatorData CreatorData { get { return creatorData; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			int valuesCount = CreatorData.ValueObjects.Count;
			int colorsCount = CreatorData.Colors.Count;
			if((valuesCount > 1) && (valuesCount < 4) && (valuesCount == colorsCount))
				Importer.ConditionalFormattingCreatorCollection.Add(CreatorData);
		}
		static Destination OnValueObject(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConditionalFormattingValueObjectDestination(importer, GetThis(importer).CreatorData, appendToObjectList);
		}
		void ColorAction(ColorModelInfo value) {
			CreatorData.Colors.Add(value);
		}
		static Destination OnColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingColorScaleDestination self = GetThis(importer);
			return new ConditionalFormattingColorDestination(importer, self.ColorAction);
		}
	}
	#endregion
}

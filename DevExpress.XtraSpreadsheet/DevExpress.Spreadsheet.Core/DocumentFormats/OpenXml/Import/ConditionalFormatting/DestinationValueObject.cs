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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ConditionalFormattingValueObjectDestination
	public class ConditionalFormattingValueObjectDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		protected internal const string ErrorMessage = "ctfo@type invalid value";
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static readonly Dictionary<string, ConditionalFormattingValueObjectType> ObjectTypeTable = CreateObjectTypeTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			result.Add("extLst", OnFutureExtension);
			return result;
		}
		static ConditionalFormattingValueObjectDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingValueObjectDestination)importer.PeekDestination();
		}
		static Dictionary<string, ConditionalFormattingValueObjectType> CreateObjectTypeTable() {
			Dictionary<string, ConditionalFormattingValueObjectType> result = new Dictionary<string, ConditionalFormattingValueObjectType>();
			result.Add("autoMax", ConditionalFormattingValueObjectType.AutoMax);
			result.Add("autoMin", ConditionalFormattingValueObjectType.AutoMin);
			result.Add("formula", ConditionalFormattingValueObjectType.Formula);
			result.Add("max", ConditionalFormattingValueObjectType.Max);
			result.Add("min", ConditionalFormattingValueObjectType.Min);
			result.Add("num", ConditionalFormattingValueObjectType.Num);
			result.Add("percent", ConditionalFormattingValueObjectType.Percent);
			result.Add("percentile", ConditionalFormattingValueObjectType.Percentile);
			return result;
		}
		#endregion
		#region Fields
		int objectIndex;
		ConditionalFormattingValueObjectType objectType;
		bool isGreaterOrEqual;
		string value;
		ConditionalFormattingCreatorData creatorData;
		#endregion
		protected internal ConditionalFormattingValueObjectDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingCreatorData creatorData, int objectIndex)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(creatorData, "creatorData");
			this.creatorData = creatorData;
			ObjectIndex = objectIndex;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal ConditionalFormattingCreatorData CreatorData { get { return creatorData; } }
		public int ObjectIndex { get { return objectIndex; } protected set { objectIndex = value; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			objectType = Importer.GetWpEnumValue<ConditionalFormattingValueObjectType>(reader, "type", ObjectTypeTable, ConditionalFormattingValueObjectType.Unknown);
			value = Importer.ReadAttribute(reader, "val");
			isGreaterOrEqual = Importer.GetWpSTOnOffValue(reader, "gte", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(objectType == ConditionalFormattingValueObjectType.Unknown)
				Exceptions.ThrowInvalidOperationException(ErrorMessage);
			ConditionalFormattingValueObject valueObject;
			CultureInfo culture = Importer.DocumentModel.Culture;
			double d;
			const NumberStyles allowedStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
			if(double.TryParse(value, allowedStyle, culture, out d))
				valueObject = new ConditionalFormattingValueObject(creatorData.Sheet, objectType, d, isGreaterOrEqual);
			else
				valueObject = new ConditionalFormattingValueObject(creatorData.Sheet, objectType, value, isGreaterOrEqual);
			if(ObjectIndex < 0)
				creatorData.ValueObjects.Add(valueObject);
			else
				creatorData.ValueObjects[ObjectIndex] = valueObject;
		}
		void FormulaAction(string value) {
			this.value = value;
		}
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConditionalFormattingFormulaDestination(importer, GetThis(importer).FormulaAction);
		}
		static Destination OnFutureExtension(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
	}
	#endregion
}

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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region default attributes values
	public static class OpenXmlDefaultIconSetConditionalFormattingValues {
		public static bool CustomIconSet = false;
		public static bool Reverse = false;
		public static bool Percent = true;
		public static bool ReverseOfCustomIconSet = false; 
		public static bool ShowValue = true;
		public static IconSetType IconSet = IconSetType.TrafficLights13;
	}
	#endregion
	#region ConditionalFormattingIconSetDestination
	public class ConditionalFormattingIconSetDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		const int appentToObjectList = -1000000;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cfIcon", OnCustomIcon);
			result.Add("cfvo", OnValueObject);
			return result;
		}
		static ConditionalFormattingIconSetDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingIconSetDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		int objectIndex;
		const string UnexpectedIconChildMessage = "iconSet/cfIcon can not be here";
		ConditionalFormattingCreatorInitData initData;
		ConditionalFormattingIconSetCreatorData creatorData;
		#endregion
		internal ConditionalFormattingIconSetDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingCreatorInitData initData)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(initData, "initData");
			this.initData = initData;
			this.creatorData = initData.Details as ConditionalFormattingIconSetCreatorData;
			objectIndex = appentToObjectList;
		}
		internal ConditionalFormattingIconSetDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingIconSetCreatorData creatorData)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(creatorData, "creatorData");
			this.initData = null;
			this.creatorData = creatorData;
			objectIndex = 0;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal ConditionalFormattingCreatorInitData InitData { get { return initData; } }
		internal ConditionalFormattingIconSetCreatorData CreatorData { get { return creatorData; } }
		#endregion
		void PrepareCreatorInfo(ConditionalFormattingIconSetCreatorData data) {
		}
		void ParseAttributes(XmlReader reader, bool useDefaults) {
			bool? customValue = Importer.GetWpSTOnOffNullValue(reader, "custom");
			if(customValue.HasValue)
				CreatorData.CustomIconSet = customValue.Value;
			else
				if(useDefaults)
					CreatorData.CustomIconSet = OpenXmlDefaultIconSetConditionalFormattingValues.CustomIconSet;
			bool? percentValue = Importer.GetWpSTOnOffNullValue(reader, "percent");
			if(percentValue.HasValue)
				CreatorData.Percent = percentValue.Value;
			else
				if(useDefaults)
					CreatorData.Percent = OpenXmlDefaultIconSetConditionalFormattingValues.Percent;
			if(CreatorData.CustomIconSet)
				CreatorData.Reverse = OpenXmlDefaultIconSetConditionalFormattingValues.ReverseOfCustomIconSet; 
			else {
				bool? reversedValue = Importer.GetWpSTOnOffNullValue(reader, "reverse");
				if(reversedValue.HasValue)
					CreatorData.Reverse = reversedValue.Value;
				else
					if(useDefaults)
						CreatorData.Reverse = OpenXmlDefaultIconSetConditionalFormattingValues.Reverse;
			}
			bool? showValue = Importer.GetWpSTOnOffNullValue(reader, "showValue");
			if(showValue.HasValue)
				CreatorData.ShowValue = showValue.Value;
			else
				if(useDefaults)
					CreatorData.ShowValue = OpenXmlDefaultIconSetConditionalFormattingValues.ShowValue;
			string iconSetValue = Importer.ReadAttribute(reader, "iconSet");
			if(!String.IsNullOrEmpty(iconSetValue))
				CreatorData.IconSet = Importer.GetWpEnumValueCore(iconSetValue, OpenXmlExporter.IconSetTypeTable, OpenXmlDefaultIconSetConditionalFormattingValues.IconSet);
			else
				if(useDefaults)
					CreatorData.IconSet = OpenXmlDefaultIconSetConditionalFormattingValues.IconSet;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bool isMainPart = InitData != null;
			if(isMainPart)
				PrepareCreatorInfo(CreatorData);
			ParseAttributes(reader, isMainPart);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if((InitData != null) && (CreatorData.ValueObjects.Count > 0))
				Importer.ConditionalFormattingCreatorCollection.Add(CreatorData);
		}
		static Destination OnValueObject(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingIconSetDestination self = GetThis(importer);
			return new ConditionalFormattingValueObjectDestination(importer, self.CreatorData, self.objectIndex++);
		}
		static Destination OnCustomIcon(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingIconSetCreatorData actualData = GetThis(importer).CreatorData;
			if(!actualData.CustomIconSet)
				Exceptions.ThrowInvalidOperationException(UnexpectedIconChildMessage); 
			return new ConditionalFormattingCustomIconDestination(importer, actualData);
		}
	}
	#endregion
}

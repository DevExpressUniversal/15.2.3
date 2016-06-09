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

using System.Diagnostics.CodeAnalysis;
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using System;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region default attributes values
	public static class OpenXmlDefaultDataBarConditionalFormattingValues {
		public static int MinLength = 10;
		public static int MaxLength = 90;
		public static bool ShowValue = true;
		public static bool HaveBorder = false;
		public static bool GradientFill = true;
		public static bool NegativeUseSameColor = false;
		public static bool NegativeUseSameBorderColor = true;
		public static ConditionalFormattingDataBarDirection Direction = ConditionalFormattingDataBarDirection.Context;
		public static ConditionalFormattingDataBarAxisPosition AxisPosition = ConditionalFormattingDataBarAxisPosition.Automatic;
	}
	#endregion
	#region ConditionalFormattingDataBarDestination
	public class ConditionalFormattingDataBarDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		const int appendToObjectList = -1000000;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static readonly Dictionary<string, ConditionalFormattingDataBarDirection> DirectionTable = CreateDirectionTable();
		static readonly Dictionary<string, ConditionalFormattingDataBarAxisPosition> AxisPositionTable = CreateAxisPositionTable();
		static readonly ColorModelInfo InvalidColor = new ColorModelInfo();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("axisColor", OnAxisColor);
			result.Add("borderColor", OnBorderColor);
			result.Add("cfvo", OnValueObject);
			result.Add("color", OnColor);
			result.Add("fillColor", OnColor);
			result.Add("negativeFillColor", OnNegativeFillColor);
			result.Add("negativeBorderColor", OnNegativeBorderColor);
			return result;
		}
		static Dictionary<string, ConditionalFormattingDataBarDirection> CreateDirectionTable() {
			Dictionary<string, ConditionalFormattingDataBarDirection> result = new Dictionary<string, ConditionalFormattingDataBarDirection>();
			result.Add("context", ConditionalFormattingDataBarDirection.Context);
			result.Add("leftToRight", ConditionalFormattingDataBarDirection.LeftToRight);
			result.Add("rightToLeft", ConditionalFormattingDataBarDirection.RightToLeft);
			return result;
		}
		static Dictionary<string, ConditionalFormattingDataBarAxisPosition> CreateAxisPositionTable() {
			Dictionary<string, ConditionalFormattingDataBarAxisPosition> result = new Dictionary<string, ConditionalFormattingDataBarAxisPosition>();
			result.Add("automatic", ConditionalFormattingDataBarAxisPosition.Automatic);
			result.Add("middle", ConditionalFormattingDataBarAxisPosition.Middle);
			result.Add("none", ConditionalFormattingDataBarAxisPosition.None);
			return result;
		}
		static ConditionalFormattingDataBarDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ConditionalFormattingDataBarDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		int objectIndex;
		ConditionalFormattingCreatorInitData initData;
		ConditionalFormattingDataBarCreatorData creatorData;
		#endregion
		internal ConditionalFormattingDataBarDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingCreatorInitData initData)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(initData, "initData");
			this.initData = initData;
			this.creatorData = initData.Details as ConditionalFormattingDataBarCreatorData;
			objectIndex = appendToObjectList;
		}
		internal ConditionalFormattingDataBarDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingDataBarCreatorData creatorData)
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
		internal ConditionalFormattingDataBarCreatorData CreatorData { get { return creatorData; } }
		#endregion
		void PrepareCreatorInfo(ConditionalFormattingDataBarCreatorData data) {
			data.AxisColor = InvalidColor;
			data.BorderColor = InvalidColor;
			data.NegativeBorderColor = InvalidColor;
			data.NegativeFillColor = InvalidColor;
		}
		#region Read dataBar attributes
		internal static void ReadAttributes(SpreadsheetMLBaseImporter importer, XmlReader reader, ConditionalFormattingDataBarCreatorData data, bool useDefaults) {
			int minLengthValue = importer.GetWpSTIntegerValue(reader, "minLength", -1);
			if(minLengthValue >= 0)
				data.MinLength = minLengthValue;
			else if(useDefaults)
				data.MinLength = OpenXmlDefaultDataBarConditionalFormattingValues.MinLength;
			int maxLengthValue = importer.GetWpSTIntegerValue(reader, "maxLength", -1);
			if(maxLengthValue >= 0)
				data.MaxLength = maxLengthValue;
			else if(useDefaults)
				data.MaxLength = OpenXmlDefaultDataBarConditionalFormattingValues.MaxLength;
			bool? showValue = importer.GetWpSTOnOffNullValue(reader, "showValue");
			if(showValue.HasValue)
				data.ShowValue = showValue.Value;
			else if(useDefaults)
				data.ShowValue = OpenXmlDefaultDataBarConditionalFormattingValues.ShowValue;
			bool? borderValue = importer.GetWpSTOnOffNullValue(reader, "border");
			if(borderValue.HasValue)
				data.HaveBorder = borderValue.Value;
			else if(useDefaults)
				data.HaveBorder = OpenXmlDefaultDataBarConditionalFormattingValues.HaveBorder;
			bool? gradientValue = importer.GetWpSTOnOffNullValue(reader, "gradient");
			if(gradientValue.HasValue)
				data.GradientFill = gradientValue.Value;
			else if(useDefaults)
				data.GradientFill = OpenXmlDefaultDataBarConditionalFormattingValues.GradientFill;
			bool? negValueSameColor = importer.GetWpSTOnOffNullValue(reader, "negativeBarColorSameAsPositive");
			if(negValueSameColor.HasValue)
				data.NegativeUseSameColor = negValueSameColor.Value;
			else if(useDefaults)
				data.NegativeUseSameColor = OpenXmlDefaultDataBarConditionalFormattingValues.NegativeUseSameColor;
			bool? negVauleBorderSameColor = importer.GetWpSTOnOffNullValue(reader, "negativeBarBorderColorSameAsPositive");
			if(negVauleBorderSameColor.HasValue)
				data.NegativeUseSameBorderColor = negVauleBorderSameColor.Value;
			else if(useDefaults)
				data.NegativeUseSameBorderColor = OpenXmlDefaultDataBarConditionalFormattingValues.NegativeUseSameBorderColor;
			string dirValue = importer.ReadAttribute(reader, "direction");
			if(String.IsNullOrEmpty(dirValue)) {
				if(useDefaults)
					data.Direction = OpenXmlDefaultDataBarConditionalFormattingValues.Direction;
			}
			else
				data.Direction = importer.GetWpEnumValueCore(dirValue, DirectionTable, OpenXmlDefaultDataBarConditionalFormattingValues.Direction);
			string axisValue = importer.ReadAttribute(reader, "axisPosition");
			if(String.IsNullOrEmpty(axisValue)) {
				if(useDefaults)
					data.AxisPosition = OpenXmlDefaultDataBarConditionalFormattingValues.AxisPosition;
			}
			else
				data.AxisPosition = importer.GetWpEnumValueCore(axisValue, AxisPositionTable, OpenXmlDefaultDataBarConditionalFormattingValues.AxisPosition);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			bool isMainPart = initData != null;
			if(isMainPart)
				PrepareCreatorInfo(CreatorData); 
			ReadAttributes(Importer, reader, CreatorData, isMainPart);
		}
		public override void ProcessElementClose(XmlReader reader) {
			int valuesCount = CreatorData.ValueObjects.Count;
			if((InitData != null) && (valuesCount) > 1 && (CreatorData.Colors.Count > 0)) {
				Importer.ConditionalFormattingCreatorCollection.Add(CreatorData);
			}
		}
		static Destination OnValueObject(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingDataBarDestination self = GetThis(importer);
			return new ConditionalFormattingValueObjectDestination(importer, self.CreatorData, self.objectIndex++);
		}
		void ColorAction(ColorModelInfo value) {
			CreatorData.Colors.Add(value);
		}
		static Destination OnColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingDataBarDestination self = GetThis(importer);
			return new ConditionalFormattingColorDestination(importer, self.ColorAction);
		}
		void AxisColorAction(ColorModelInfo value) {
			CreatorData.AxisColor = value;
		}
		static Destination OnAxisColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingDataBarDestination self = GetThis(importer);
			ConditionalFormattingDataBarCreatorData actualData = self.CreatorData;
			if(actualData.AxisPosition == ConditionalFormattingDataBarAxisPosition.None)
				Exceptions.ThrowInvalidOperationException("dataBar/borderColor can not be here"); 
			return new ConditionalFormattingColorDestination(importer, self.AxisColorAction);
		}
		void BorderColorAction(ColorModelInfo value) {
			CreatorData.BorderColor = value;
		}
		static Destination OnBorderColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingDataBarDestination self = GetThis(importer);
			ConditionalFormattingDataBarCreatorData actualData = self.CreatorData;
			if(!actualData.HaveBorder)
				Exceptions.ThrowInvalidOperationException("dataBar/borderColor can not be here"); 
			return new ConditionalFormattingColorDestination(importer, self.BorderColorAction);
		}
		void NegativeBorderColorAction(ColorModelInfo value) {
			CreatorData.NegativeBorderColor = value;
		}
		static Destination OnNegativeBorderColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingDataBarDestination self = GetThis(importer);
			ConditionalFormattingDataBarCreatorData actualData = self.CreatorData;
			if(!actualData.HaveBorder || actualData.NegativeUseSameBorderColor)
				Exceptions.ThrowInvalidOperationException("dataBar/negativeBorderColor can not be here"); 
			return new ConditionalFormattingColorDestination(importer, self.NegativeBorderColorAction);
		}
		void NegativeFillColorAction(ColorModelInfo value) {
			CreatorData.NegativeFillColor = value;
		}
		static Destination OnNegativeFillColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ConditionalFormattingDataBarDestination self = GetThis(importer);
			ConditionalFormattingDataBarCreatorData actualData = self.CreatorData;
			if(actualData.NegativeUseSameColor)
				Exceptions.ThrowInvalidOperationException("dataBar/negativeFillColor can not be here"); 
			return new ConditionalFormattingColorDestination(importer, self.NegativeFillColorAction);
		}
	}
	#endregion
}

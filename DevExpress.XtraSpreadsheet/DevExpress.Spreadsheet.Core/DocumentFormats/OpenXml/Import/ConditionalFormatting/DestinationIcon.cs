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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ConditionalFormattingIconDestination
	public class ConditionalFormattingCustomIconDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Fields
		internal const IconSetType invalidIconSetValue = (IconSetType)Int32.MaxValue;
		const string InvalidIconSetMessage = "cfIcon@iconSet does not exist or has invalid value";
		const string InvalidIconIdValue = "cfIcon@iconId does not exist or has invalid value";
		ConditionalFormattingIconSetCreatorData creatorData;
		IconSetType iconSet;
		int iconIndex;
		#endregion
		public ConditionalFormattingCustomIconDestination(SpreadsheetMLBaseImporter importer, ConditionalFormattingIconSetCreatorData creatorData)
			: base(importer) {
			Guard.ArgumentNotNull(importer, "importer");
			Guard.ArgumentNotNull(creatorData, "creatorData");
			this.creatorData = creatorData;
		}
		#region Properties
		internal ConditionalFormattingIconSetCreatorData CreatorData { get { return creatorData; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			iconSet = Importer.GetWpEnumValue<IconSetType>(reader, "iconSet", OpenXmlExporter.IconSetTypeTable, invalidIconSetValue);
			iconIndex = Importer.GetWpSTIntegerValue(reader, "iconId", -1);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if(iconSet == invalidIconSetValue)
				Exceptions.ThrowInvalidOperationException(InvalidIconSetMessage); 
			int expectedPointsNumber;
			if(!IconSetConditionalFormatting.expectedPointsNumberTable.TryGetValue(iconSet, out expectedPointsNumber))
				expectedPointsNumber = 0;
			if((iconIndex < 0) || ((iconSet != IconSetType.None) && (iconIndex >= expectedPointsNumber)))
				Exceptions.ThrowInvalidOperationException(InvalidIconIdValue); 
			CreatorData.CustomIcons.Add(new ConditionalFormattingCustomIcon(iconSet, iconIndex));
		}
	}
	#endregion
}

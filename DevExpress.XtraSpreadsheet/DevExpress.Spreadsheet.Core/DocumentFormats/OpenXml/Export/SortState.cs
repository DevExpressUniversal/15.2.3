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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static Dictionary<SortMethod, string> SortMethodTable = CreateSortMethodTable();
		internal static Dictionary<SortBy, string> SortByTable = CreateSortByTable();
		internal static Dictionary<IconSetType, string> IconSetTypeTable = CreateIconSetTypeTable();
		static Dictionary<SortMethod, string> CreateSortMethodTable() {
			Dictionary<SortMethod, string> result = new Dictionary<SortMethod, string>();
			result.Add(SortMethod.None, "none");
			result.Add(SortMethod.Stroke, "stroke");
			result.Add(SortMethod.PinYin, "pinYin");
			return result;
		}
		static Dictionary<SortBy, string> CreateSortByTable() {
			Dictionary<SortBy, string> result = new Dictionary<SortBy, string>();
			result.Add(SortBy.Value, "value");
			result.Add(SortBy.CellColor, "cellColor");
			result.Add(SortBy.FontColor, "fontColor");
			result.Add(SortBy.Icon, "icon");
			return result;
		}
		static Dictionary<IconSetType, string> CreateIconSetTypeTable() {
			Dictionary<IconSetType, string> result = new Dictionary<IconSetType, string>();
			result.Add(IconSetType.None, "NoIcons");
			result.Add(IconSetType.Arrows3, "3Arrows");
			result.Add(IconSetType.ArrowsGray3, "3ArrowsGray");
			result.Add(IconSetType.Flags3, "3Flags");
			result.Add(IconSetType.TrafficLights13, "3TrafficLights1");
			result.Add(IconSetType.TrafficLights23, "3TrafficLights2");
			result.Add(IconSetType.Signs3, "3Signs");
			result.Add(IconSetType.Symbols3, "3Symbols");
			result.Add(IconSetType.Symbols23, "3Symbols2");
			result.Add(IconSetType.Stars3, "3Stars");
			result.Add(IconSetType.Triangles3, "3Triangles");
			result.Add(IconSetType.Arrows4, "4Arrows");
			result.Add(IconSetType.ArrowsGray4, "4ArrowsGray");
			result.Add(IconSetType.RedToBlack4, "4RedToBlack");
			result.Add(IconSetType.Rating4, "4Rating");
			result.Add(IconSetType.TrafficLights4, "4TrafficLights");
			result.Add(IconSetType.Arrows5, "5Arrows");
			result.Add(IconSetType.ArrowsGray5, "5ArrowsGray");
			result.Add(IconSetType.Rating5, "5Rating");
			result.Add(IconSetType.Quarters5, "5Quarters");
			result.Add(IconSetType.Boxes5, "5Boxes");
			return result;
		}
		#endregion
		protected internal virtual void GenerateSortStateContent(SortState sortState) {
			if (!ShouldExportSortState(sortState))
				return;
			WriteShStartElement("sortState");
			try {
				GenerateSortStateAttributesContent(sortState);
				GenerateSortConditionsContent(sortState.SortConditions);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldExportSortState(SortState sortState) {
			return sortState.SortRange != null;
		}
		protected internal virtual void GenerateSortStateAttributesContent(SortState sortState) {
			SortStateInfo defaultItem = Workbook.Cache.SortStateInfoCache.DefaultItem;
			WriteStringValue("ref", sortState.SortRange.ToString());
			if (sortState.SortMethod != defaultItem.SortMethod)
				WriteStringValue("sortMethod", SortMethodTable[sortState.SortMethod]);
			if (sortState.SortByColumns != defaultItem.SortByColumns)
				WriteBoolValue("columnSort", sortState.SortByColumns);
			if (sortState.CaseSensitive != defaultItem.CaseSensitive)
				WriteBoolValue("caseSensitive", sortState.CaseSensitive);
		}
		protected internal virtual void GenerateSortConditionsContent(SortConditionCollection sortConditions) {
			if (sortConditions.Count == 0)
				return;
			sortConditions.ForEach(GenerateSortConditionContent);
		}
		protected internal virtual void GenerateSortConditionContent(SortCondition sortCondition) {
			Guard.ArgumentNotNull(sortCondition, "sortCondition");
			WriteShStartElement("sortCondition");
			try {
				SortConditionInfo defaultItem = Workbook.Cache.SortConditionInfoCache.DefaultItem;
				if (sortCondition.Descending != defaultItem.Descending)
					WriteBoolValue("descending", sortCondition.Descending);
				if (sortCondition.SortBy != defaultItem.SortBy)
					WriteStringValue("sortBy", SortByTable[sortCondition.SortBy]);
				WriteStringValue("ref", sortCondition.SortReference.ToString());
				if (!String.IsNullOrEmpty(sortCondition.CustomList))
					WriteStringValue("customList", sortCondition.CustomList);
				if (sortCondition.IconSet != defaultItem.IconSet)
					WriteStringValue("iconSet", IconSetTypeTable[sortCondition.IconSet]);
				if (sortCondition.IconId != defaultItem.IconId)
					WriteIntValue("iconId", sortCondition.IconId);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}

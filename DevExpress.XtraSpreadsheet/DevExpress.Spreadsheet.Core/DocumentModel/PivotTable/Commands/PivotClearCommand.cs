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

using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Utils;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCreateCommand
	public class PivotClearCommand : PivotTableTransactedCommand {
		public PivotClearCommand(PivotTable pivotTable, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
		}
		bool TableCompact { get { return PivotTable.Compact; } }
		protected internal override void ExecuteCore() {
			PivotTable.ClearAllKeyFields(ErrorHandler); 
			ClearTableProperties();
			ClearFieldProperties();
			ClearUnusedCacheFieldItems();
			PivotTable.Filters.Clear();
			PivotTable.Formats.Clear();
			PivotTable.ConditionalFormats.Clear();
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void ClearTableProperties() {
			if (PivotTable.HasAlignmentFormats)
				PivotTable.ApplyAlignmentFormats = false;
			if (PivotTable.HasBorderFormats)
				PivotTable.ApplyBorderFormats = false;
			if (PivotTable.HasFontFormats)
				PivotTable.ApplyFontFormats = false;
			if (PivotTable.HasNumberFormats)
				PivotTable.ApplyNumberFormats = false;
			if (PivotTable.HasPatternFormats)
				PivotTable.ApplyPatternFormats = false;
			if (PivotTable.HasWidthHeightFormats)
				PivotTable.ApplyWidthHeightFormats = true;
			PivotTable.CompactData = TableCompact;
			PivotTable.OutlineData = TableCompact;
			PivotTable.DataCaption = PivotTable.DefaultDataCaption;
			PivotTable.GrandTotalCaption = null;
			PivotTable.RowHeaderCaption = null;
			PivotTable.ColHeaderCaption = null;
			PivotTable.Tag = null;
			PivotTable.VisualTotals = true;
			PivotTable.MdxSubqueries = false;
		}
		void ClearFieldProperties() {
			foreach (PivotField field in PivotTable.Fields) {
				bool allDrilled = field.AllDrilled;
				bool defaultAttributeDrillState = field.DefaultAttributeDrillState;
				field.Info.CopyFrom(DocumentModel.Cache.PivotFieldCache.DefaultItem);
				if (PivotTable.Outline && !TableCompact) {
					field.Outline = true;
					field.Compact = false;
				}
				else {
					field.Outline = TableCompact;
					field.Compact = TableCompact;
				}
				field.AllDrilled = allDrilled;
				field.DefaultAttributeDrillState = defaultAttributeDrillState;
				field.Name = null;
				field.UniqueMemberProperty = null;
				field.SubtotalCaption = null;
				field.PivotArea.Clear();
				field.MeasureFilter = false;
				field.Items.UnhideAll();
			}
		}
		void ClearUnusedCacheFieldItems() {
			foreach (PivotCacheField cacheField in PivotTable.Cache.CacheFields) {
				PivotCacheSharedItemsCollection sharedItems = cacheField.SharedItems;
				for (int i = sharedItems.Count - 1; i >= 0; i--) {
					IPivotCacheRecordValue value = sharedItems[i];
					if (value.IsUnusedItem)
						cacheField.SharedItems.Remove(value);
				}
			}
		}
	}
	#endregion
}

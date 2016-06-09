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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ResetRangeCommand
	public class ResetRangeCommand : MailMergeCommand {
		#region static
		static List<string> GetDetailLevelsList(int index) {
			List<string> result = new List<string>();
			string stringIndex = index.ToString();
			result.Add(MailMergeDefinedNames.DetailLevel + stringIndex);
			result.Add(MailMergeDefinedNames.DetailDataMember + stringIndex);
			return result;
		}
		#endregion
		#region fields
		List<string> selectedItems;
		CellRange selectedRange;
		#endregion
		public ResetRangeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.MailMergeResetRange; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeResetRangeCommandDescription; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_MailMergeResetRangeCommand; } }
		public override string ImageName { get { return "ResetRange"; } }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				foreach (string rangeName in selectedItems) {
					ActiveSheet.RemoveDefinedName(rangeName);
					ActiveSheet.Workbook.RemoveDefinedName(rangeName);
				}
				Control.InnerControl.RaiseUpdateUI();
				Control.InnerControl.Owner.Redraw();
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			selectedItems = GetRanges();
			state.Enabled = selectedItems != null && selectedItems.Count > 0;
		}
		List<string> GetRanges() {
			selectedRange = ActiveSheet.Selection.SelectedRanges[0];
			List<string> result = new List<string>();
			MailMergeOptions options = new MailMergeOptions(DocumentModel);
			if (RangeIsIntersected(options.HeaderRange))
				result.Add(MailMergeDefinedNames.HeaderRange);
			if (RangeIsIntersected(options.FooterRange))
				result.Add(MailMergeDefinedNames.FooterRange);
			if (!RangeIsIntersected(options.DetailRange))
				return result;
			List<string> groupAddinDefinedNames = GroupAddins(options.DetailRange, options, true);
			if (groupAddinDefinedNames.Count > 0)
				return groupAddinDefinedNames;
			List<CellRange> outerDetails = GetLevelIncludesSelectedRange(options.DetailRange as CellRange, options);
			if (outerDetails.Count == 0 || outerDetails.Contains(options.DetailRange as CellRange)) {
				result.Add(MailMergeDefinedNames.DetailRange);
				result.AddRange(GroupAddins(options.DetailRange, options, false));
				for (int i = 0; i < options.DetailLevels.Count; i++) {
					result.AddRange(GetDetailLevelsList(i));
					result.AddRange(GroupAddins(options.DetailLevels[i], options, false));
				}
				return result;
			}
			else foreach (CellRange outerDetail in outerDetails) {
					List<CellRangeBase> childRanges = options.GetChildRanges(outerDetail, false);
					if (childRanges.Count > 0)
						foreach (CellRange detailLevel in childRanges) {
							groupAddinDefinedNames = GroupAddins(detailLevel, options, true);
							if (groupAddinDefinedNames.Count > 0)
								return groupAddinDefinedNames;
							result.AddRange(GetDetailLevelsList(options.DetailLevels.IndexOf(detailLevel)));
							result.AddRange(GroupAddins(detailLevel, options, false));
						}
					else {
						groupAddinDefinedNames = GroupAddins(outerDetail, options, true);
						if (groupAddinDefinedNames.Count > 0)
							return groupAddinDefinedNames;
						result.AddRange(GetDetailLevelsList(options.DetailLevels.IndexOf(outerDetail)));
						result.AddRange(GroupAddins(outerDetail, options, false));
					}
				}
			return result;
		}
		List<String> GroupAddins(CellRangeBase range, MailMergeOptions options, bool includeSelectedRange) {
			List<String> result = new List<string>();
			List<GroupInfo> detailRangeGroupInfo = options.GetGroupInfo(range);
			if (detailRangeGroupInfo.Count > 0) {
				foreach (GroupInfo info in detailRangeGroupInfo) {
					if (info.Header != null && (info.Header.Includes(selectedRange) || !includeSelectedRange))
						result.Add(MailMergeDefinedNames.GroupHeader + info.DefinedName.Substring(MailMergeDefinedNames.GroupName.Length));
					if (info.Footer != null && (info.Footer.Includes(selectedRange) || !includeSelectedRange))
						result.Add(MailMergeDefinedNames.GroupFooter + info.DefinedName.Substring(MailMergeDefinedNames.GroupName.Length));
					if (!includeSelectedRange)
						result.Add(info.DefinedName);
				}
			}
			return result;
		}
		bool RangeIsIntersected(CellRangeBase range) {
			return range != null && selectedRange.Intersects(range);
		}
		List<CellRange> GetLevelIncludesSelectedRange(CellRange detailLevel, MailMergeOptions options) {
			List<CellRange> result = new List<CellRange>();
			if (detailLevel.Includes(selectedRange)){
				foreach (CellRange childRange in options.GetChildRanges(detailLevel, false)) {
					foreach (CellRange nestedLevel in GetLevelIncludesSelectedRange(childRange, options))
						if(!result.Contains(nestedLevel))
							result.Add(nestedLevel);
				}
				if (result.Count == 0)
					result.Add(detailLevel);
			}
			return result;
		}
	}
	#endregion
}

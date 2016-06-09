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
using DevExpress.Data;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	public class MailMergeOptions {
		#region fields
		readonly char[] divider = ";".ToCharArray();
		readonly string dotDivider = ".";
		bool horizontalMode;
		DocumentModel templateBook;
		MailMergeMode mailMergeMode;
		CellRangeBase detailRange;
		CellRangeBase headerRange;
		CellRangeBase footerRange;
		DetailList<CellRangeBase> detailLevels;
		DetailList<string> dataMembers;
		Dictionary<CellRangeBase, List<GroupInfo>> groupedRanges;
		Dictionary<CellRangeBase, string> rangeNames;
		int groupInfoCount;
		List<FilterInfo> filterInfo;
		bool isEmpty;
		#endregion
		public MailMergeOptions(DocumentModel templateBook) {
			this.templateBook = templateBook;
			horizontalMode = false;
			detailRange = null;
			headerRange = null;
			footerRange = null;
			detailLevels = new DetailList<CellRangeBase>();
			dataMembers = new DetailList<string>();
			groupedRanges = new Dictionary<CellRangeBase, List<GroupInfo>>();
			rangeNames = new Dictionary<CellRangeBase, string>();
			filterInfo = new List<FilterInfo>();
			groupInfoCount = 0;
			isEmpty = true;
			GetOptions();
		}
		#region Properties
		public MailMergeMode MailMergeMode { get { return mailMergeMode; } }
		public bool HorizontalMode { get { return horizontalMode; } }
		public bool HasGroup { get { return groupInfoCount > 0; } }
		public bool IsEmpty { get { return isEmpty; } }
		public CellRangeBase DetailRange { get { return detailRange; } }
		public CellRangeBase HeaderRange { get { return headerRange; } }
		public CellRangeBase FooterRange { get { return footerRange; } }
		public DetailList<CellRangeBase> DetailLevels { get { return detailLevels; } }
		public DetailList<string> DataMembers { get { return dataMembers; } }
		#endregion
		void GetOptions() {
			ValidateDocument();
			if (isEmpty)
				return;
			mailMergeMode = GetMailMergeMode();
			horizontalMode = GetHorizontalMode();
			GetReportOptions();
		}
		void ValidateDocument() {
			if (templateBook.DefinedNames.Count == 0 && templateBook.ActiveSheet.DefinedNames.Count == 0)
				return;
			if(templateBook.DefinedNames.Contains(MailMergeDefinedNames.MailMergeMode) || templateBook.ActiveSheet.DefinedNames.Contains(MailMergeDefinedNames.DetailRange))
				isEmpty = false;
			isEmpty = false; 
		}
		void ProcessDefinedName(string name) {
			if (name.Contains(MailMergeDefinedNames.DetailLevel)) {
				int index = int.Parse(name.Substring(MailMergeDefinedNames.DetailLevel.Length));
				CellRangeBase detailLevel = GetDetailRange(name);
				string dataMember = GetExpressionString(MailMergeDefinedNames.DetailDataMember + index.ToString());
				if (dataMember != null) {
					detailLevels.AddInSortOrder(index, detailLevel);
					dataMembers.AddInSortOrder(index, dataMember);
				}
			}
			else if (name.Contains(MailMergeDefinedNames.FilterField)) {
				FilterInfo info = GetFilterInfo(name);
				if (info != null)
					filterInfo.Add(info);
			}
		}
		void GetReportOptions() {
			GetHeaderFooterRanges();
			if (!templateBook.ActiveSheet.DefinedNames.Contains(MailMergeDefinedNames.DetailRange))
				return;
			detailRange = GetDetailRange(MailMergeDefinedNames.DetailRange);
			try {
				foreach (DefinedNameBase definedName in templateBook.ActiveSheet.DefinedNames)
					ProcessDefinedName(definedName.Name);
			}
			catch {
				ClearDetailLevels();
			}
			if (detailLevels.Count > 0 && !detailLevels.IsContiniousZeroOrder)
				ClearDetailLevels();
		}
		void ClearDetailLevels() {
			detailLevels.Clear();
			dataMembers.Clear();
		}
		MailMergeMode GetMailMergeMode() {
			if (templateBook.DefinedNames.Contains(MailMergeDefinedNames.MailMergeMode)) {
				string mode = "\""+GetExpressionString(MailMergeDefinedNames.MailMergeMode, templateBook.DefinedNames)+"\"";
				switch (mode) {
					case MailMergeConstants.DocumentsMode:
						return MailMergeMode.DocumentsMode;
					case MailMergeConstants.OneDocumentMode:
						return MailMergeMode.OneDocumentMode;
					case MailMergeConstants.OneSheetMode:
						return MailMergeMode.OneSheetMode;
					default:
						return MailMergeMode.OneSheetMode;
				}
			}
			else
				return MailMergeMode.OneSheetMode;
		}
		string GetExpressionString(string definedName) {
			return GetExpressionString(definedName, templateBook.ActiveSheet.DefinedNames);
		}
		string GetExpressionString(string definedName, DefinedNameCollection dinedNames) {
			if (!dinedNames.Contains(definedName))
				return null;
			DefinedNameBase definedN = dinedNames[definedName];
			VariantValue value = definedN.EvaluateToSimpleValue(templateBook.DataContext);
			value = value.ToText(templateBook.DataContext);
			if (value.IsText)
				return value.GetTextValue(templateBook.SharedStringTable);
			return string.Empty;
		}
		bool GetHorizontalMode() {
			if (templateBook.DefinedNames.Contains(MailMergeDefinedNames.HorizontalMode)) {
				VariantValue value = templateBook.DefinedNames[MailMergeDefinedNames.HorizontalMode].Evaluate(templateBook.DataContext);
				if (value.IsBoolean)
					return value.BooleanValue;
			}
			return false;
		}
		CellRangeBase GetDetailRange(string definedName) {
			DefinedName detailDefinedName = templateBook.ActiveSheet.DefinedNames[definedName] as DefinedName;
			CellRangeBase result = detailDefinedName.GetReferencedRange() ?? detailDefinedName.Evaluate(templateBook.DataContext).CellRangeValue;
			if (!string.IsNullOrEmpty(detailDefinedName.Comment)) {
				foreach (string groupDefinedName in detailDefinedName.Comment.Split(divider)) {
					if (string.IsNullOrEmpty(groupDefinedName))
						continue;
					GroupInfo groupInfo = GetGroupInfo(groupDefinedName.Trim());
					if (groupedRanges.ContainsKey(result))
						groupedRanges[result].Add(groupInfo);
					else groupedRanges.Add(result, new List<GroupInfo> { groupInfo });
					groupInfoCount++;
				}
			}
			rangeNames.Add(result, definedName);
			return result;
		}
		GroupInfo GetGroupInfo(string definedName) {
			DefinedName groupName = templateBook.ActiveSheet.DefinedNames[definedName] as DefinedName;
			string name = GetExpressionString(definedName);
			bool descending = String.IsNullOrEmpty(groupName.Comment) ? false : groupName.Comment.Equals(ColumnSortOrder.Descending.ToString(), StringComparison.OrdinalIgnoreCase);
			CellRangeBase header = GetRangeBySortFieldName(MailMergeDefinedNames.GroupHeader, definedName);
			CellRangeBase footer = GetRangeBySortFieldName(MailMergeDefinedNames.GroupFooter, definedName);
			return new GroupInfo(name, header, footer, descending, definedName);
		}
		public CellRangeBase GetRangeByDefinedName(string nameOfDefinedName) {
			CellRangeBase range = null;
			if(templateBook.ActiveSheet.DefinedNames.Contains(nameOfDefinedName)) {
				DefinedName definedName = templateBook.ActiveSheet.DefinedNames[nameOfDefinedName] as DefinedName;
				if(definedName == null)
					return null;
				range = definedName.GetReferencedRange() ?? definedName.Evaluate(templateBook.DataContext).CellRangeValue;
			}
			return range;
		}
		CellRangeBase GetRangeBySortFieldName(string definedTemplate, string sortFieldName) {
			CellRangeBase range = null;
			foreach (DefinedName definedName in templateBook.ActiveSheet.DefinedNames) {
				if (definedName.Name.Contains(definedTemplate) && definedName.Comment == sortFieldName) {
					range = definedName.GetReferencedRange();
					break;
				}
			}
			return range;
		}
		public List<GroupInfo> GetGroupInfoList() {
			List<GroupInfo> result = new List<GroupInfo>();
			foreach (KeyValuePair<CellRangeBase, List<GroupInfo>> groupedRangeInfo in groupedRanges)
				if (groupedRangeInfo.Value != null)
					result.AddRange(groupedRangeInfo.Value);
			return result;
		}
		FilterInfo GetFilterInfo(string definedName) {
			string expressionString = GetExpressionString(definedName);
			if (string.IsNullOrEmpty(expressionString))
				return null;
			string[] expressions = expressionString.Split(divider);
			string dataMember = expressions.Length > 1 ? expressions[0].Trim() : string.Empty;
			string expression = expressions[expressions.Length - 1].Trim();
			return new FilterInfo(dataMember, expression, definedName);
		}
		public FilterInfo GetFilterInfoByDataMember(string dataMember) {
			foreach (FilterInfo info in filterInfo)
				if (info.DataMember == dataMember )
					return info;
			return null;
		}
		public FilterInfo CreateFilterInfo(string dataMember, string expression) {
			FilterInfo result = new FilterInfo(dataMember, expression, MailMergeDefinedNames.FilterField + filterInfo.Count.ToString());
			filterInfo.Add(result);
			return result;
		}
		void GetHeaderFooterRanges() {
			headerRange = GetRangeByDefinedName(MailMergeDefinedNames.HeaderRange);
			footerRange = GetRangeByDefinedName(MailMergeDefinedNames.FooterRange);
		}
		public List<CellRangeBase> GetChildRanges(CellRange parentRange, bool excludeGrandChild){
			if (parentRange == detailRange && !excludeGrandChild)
				return detailLevels;
			List<CellRangeBase> tempDetailLevels = new List<CellRangeBase>();
			List<CellRangeBase> resultDetailLevels = new List<CellRangeBase>();
			foreach (CellRangeBase detailLevel in detailLevels)
				if (parentRange.Includes(detailLevel) && parentRange != detailLevel)
					tempDetailLevels.Add(detailLevel);
			for (int i = 0; i < tempDetailLevels.Count; i++)
				if (!LevelIsInner(tempDetailLevels, tempDetailLevels[i]))
					resultDetailLevels.Add(tempDetailLevels[i]);
			return resultDetailLevels;
		}
		public CellRangeBase GetParentRange(CellRangeBase childRange) {
			if (!detailLevels.Contains(childRange))
				return null;
			return GetParentForSomeRange(childRange);
		}
		CellRangeBase GetParentForSomeRange(CellRangeBase childRange) {
			List<CellRangeBase> tempDetailLevels = new List<CellRangeBase>();
			foreach (CellRangeBase detailLevel in detailLevels)
				if (detailLevel.Includes(childRange) && childRange != detailLevel)
					tempDetailLevels.Add(detailLevel);
			for (int i = 0; i < tempDetailLevels.Count; i++)
				if (!LevelIsOuter(tempDetailLevels, tempDetailLevels[i]))
					return tempDetailLevels[i];
			return detailRange;
		}
		bool LevelIsInner(List<CellRangeBase> tempDetailLevels, CellRangeBase level) {
			foreach (CellRangeBase detailLevel in tempDetailLevels)
				if (detailLevel.Includes(level) && detailLevel != level)
					return true;
			return false;
		}
		bool LevelIsOuter(List<CellRangeBase> tempDetailLevels, CellRangeBase level) {
			foreach (CellRangeBase detailLevel in tempDetailLevels)
				if (level.Includes(detailLevel) && detailLevel != level)
					return true;
			return false;
		}
		public bool IsGroupedRange(CellRangeBase range) {
			return groupedRanges.ContainsKey(range);
		}
		public List<GroupInfo> GetGroupInfo(CellRangeBase range) {
			if (range != null && groupedRanges.Count > 0 && groupedRanges.ContainsKey(range))
				return groupedRanges[range];
			return new List<GroupInfo>();
		}
		public GroupInfo GetGroupInfo(CellRangeBase range, string fieldName) {
			foreach (GroupInfo info in groupedRanges[range])
				if (info.FieldName == fieldName)
					return info;
			return null;
		}
		public List<GroupInfo> GetGroupInfoFromPosition(CellPosition position) {
			return GetGroupInfo(GetRangeFromPosition(position));
		}
		public List<EditGroupInfo> GetEditableGroupInfo(CellPosition position) {
			List<GroupInfo> groupInfo = GetGroupInfoFromPosition(position);
			List<EditGroupInfo> result = new List<EditGroupInfo>();
			if (groupInfo != null)
				for (int i = 0; i < groupInfo.Count; i++)
					result.Add(new EditGroupInfo(groupInfo[i], i));
			return result;
		}
		public GroupInfo CreateGroupInfo(string name, CellRangeBase header, CellRangeBase footer, ColumnSortOrder sortOrder) {
			string definedName = MailMergeDefinedNames.GroupName + groupInfoCount.ToString();
			groupInfoCount++;
			return new GroupInfo(name, header, footer, sortOrder == ColumnSortOrder.Descending, definedName);
		}
		public string GetDataMemberFromPosition(CellPosition position) {
			CellRangeBase range = GetRangeFromPosition(position);
			if (range != null && range != detailRange)
				return dataMembers[detailLevels.IndexOf(range)];
			return string.Empty;
		}
		public string GetFullDataMemberFromPosition(CellPosition position) {
			CellRangeBase range = GetRangeFromPosition(position);
			string dataMember = string.Empty;
			if (range != null && range != detailRange)
				dataMember = dataMembers[detailLevels.IndexOf(range)];
			range = GetParentRange(range);
			while (range != null && range != detailRange) {
				dataMember = dataMembers[detailLevels.IndexOf(range)] + (!string.IsNullOrEmpty(dataMember) ? dotDivider : string.Empty) + dataMember;
				range = GetParentRange(range);
			}
			if(range == detailRange && !string.IsNullOrEmpty(dataMember))
				dataMember = templateBook.MailMergeDataMember + (String.IsNullOrEmpty(templateBook.MailMergeDataMember) ? String.Empty : dotDivider) + dataMember;
			if (range == null || string.IsNullOrEmpty(dataMember))
				dataMember = templateBook.MailMergeDataMember;
			return dataMember;
		}
		CellRangeBase GetRangeFromPosition(CellPosition position) {
			if (detailRange != null && RangeContainsPosition(detailRange, position)){
				if (detailLevels.Count > 0) {
					CellRangeBase range = GetParentForSomeRange(new CellRange(detailRange.Worksheet, position, position));
					if (range != null)
						return range;
				}
				return detailRange;
			}
			return null;
		}
		public string GetRangeNameFromPosition(CellPosition position) {
			CellRangeBase range = GetRangeFromPosition(position);
			if (range != null)
				return rangeNames[range];
			return null;
		}
		public CellRange GetDetailedRangeFromSomeRange(CellRange someRange) {
			if (detailRange == null || !detailRange.Includes(someRange))
				return null;
			if (detailLevels.Count <= 0)
				return detailRange as CellRange;
			CellRange range = GetParentForSomeRange(someRange) as CellRange;
			if (range != null && range != detailRange)
				return range;
			return null;
		}
		bool RangeContainsPosition(CellRangeBase range, CellPosition position) {
			return range.TopLeft.Column <= position.Column && position.Column <= range.BottomRight.Column & range.TopLeft.Row <= position.Row &&  position.Row <= range.BottomRight.Row;
		}
	}
}

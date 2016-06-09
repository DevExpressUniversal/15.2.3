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
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region RtfListId
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct RtfListId  {
		int m_value;
		internal RtfListId(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is RtfListId) && (this.m_value == ((RtfListId)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(RtfListId id1, RtfListId id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(RtfListId id1, RtfListId id2) {
			return id1.m_value != id2.m_value;
		}
		public static explicit operator int(RtfListId id) {
			return id.m_value;
		}
	}
	#endregion
	public enum RtfNumberingListType {
		Unknown,
		Hybrid,
		Simple		
	}
	#region RtfNumberingList
	public class RtfNumberingList {
		#region Fields
		RtfListId id;
		RtfListId parentStyleId;
		string name;
		string styleName;
		List<RtfListLevel> levels = new List<RtfListLevel>();
		RtfNumberingListType numberingListType = RtfNumberingListType.Unknown;
		#endregion
		#region Properties
		public RtfListId Id { get { return id; } set { id = value; } }
		public RtfListId ParentStyleId { get { return parentStyleId; } set { parentStyleId = value; } }
		public string Name { get { return name; } set { name = value; } }
		public string StyleName { get { return styleName; } set { styleName = value; } }
		public List<RtfListLevel> Levels { get { return levels; } }
		public RtfNumberingListType NumberingListType { get { return numberingListType; } set { numberingListType = value; } }
		#endregion
	}
	#endregion
	#region RtfListTable
	public class RtfListTable : List<RtfNumberingList> {
	}
	#endregion
	#region ListTableDestination
	public class ListTableDestination : DestinationBase {
		#region Fields
		RtfNumberingList currentList;
		#endregion
		#region CreateKeywordTable
		static KeywordTranslatorTable keywordHT = CreateKeywordTable();
		static KeywordTranslatorTable CreateKeywordTable() {
			KeywordTranslatorTable table = new KeywordTranslatorTable();
			table.Add("list", OnListKeyword);
			table.Add("listid", OnListIdKeyword);
			table.Add("listtemplateid", OnListTemplateIdKeyword);
			table.Add("liststyleid", OnListStyleIdKeyword);
			table.Add("liststylename", OnListStyleNameKeyword);
			table.Add("listname", OnListNameKeyword);
			table.Add("listhybrid", OnListHybridKeyword);
			table.Add("listrestarthdn", OnListRestartAtEachSectionKeyword);
			table.Add("listsimple", OnListSimpleKeyword);
			table.Add("listlevel", OnListLevelKeyword);
			return table;
		}
		#endregion
		public ListTableDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		#region Properties
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return keywordHT; } }
		public RtfNumberingList CurrentList { get { return currentList; } set { currentList = value; } }
		#endregion
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			TryToHandleFinishOfListNameDestination(nestedDestination);
			TryToHandleFinishOfListStyleNameDestination(nestedDestination);
			TryToHandleFinishOfListLevelDestination(nestedDestination);
		}
		protected internal virtual void TryToHandleFinishOfListNameDestination(DestinationBase nestedDestination) {
			ListTableDestination currentDestination = (ListTableDestination)Importer.Destination;
			ListNameDestination destination = nestedDestination as ListNameDestination;
			if (destination != null)
				currentDestination.CurrentList.Name = destination.Value;
		}
		protected internal virtual void TryToHandleFinishOfListStyleNameDestination(DestinationBase nestedDestination) {
			ListTableDestination currentDestination = (ListTableDestination)Importer.Destination;
			ListStyleNameDestination destination = nestedDestination as ListStyleNameDestination;
			if (destination != null)
				currentDestination.CurrentList.StyleName = destination.Value;
		}
		protected internal virtual void TryToHandleFinishOfListLevelDestination(DestinationBase nestedDestination) {
			ListTableDestination currentDestination = (ListTableDestination)Importer.Destination;
			ListLevelDestination destination = nestedDestination as ListLevelDestination;
			if (destination != null) {
				RtfListLevel level = destination.Level;
				currentDestination.CurrentList.Levels.Add(level);
			}
		}
		#region Keyword handlers
		static void OnListKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListTableDestination destination = (ListTableDestination)importer.Destination;
			destination.CurrentList = new RtfNumberingList();
			importer.DocumentProperties.ListTable.Add(destination.CurrentList);
		}
		static void OnListIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListTableDestination destination = (ListTableDestination)importer.Destination;
			if (destination.CurrentList != null && hasParameter)
				destination.CurrentList.Id = new RtfListId(parameterValue);
		}
		static void OnListTemplatIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListStyleIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListTableDestination destination = (ListTableDestination)importer.Destination;
			if (destination.CurrentList != null && hasParameter)
				destination.CurrentList.ParentStyleId = new RtfListId(parameterValue);
		}
		static void OnListStyleNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ListStyleNameDestination(importer);
		}
		static void OnListNameKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			importer.Destination = new ListNameDestination(importer);
		}
		static void OnListHybridKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListTableDestination destination = (ListTableDestination)importer.Destination;
			destination.CurrentList.NumberingListType = RtfNumberingListType.Hybrid;
		}
		static void OnListRestartAtEachSectionKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListTemplateIdKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListSimpleKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
		}
		static void OnListLevelKeyword(RtfImporter importer, int parameterValue, bool hasParameter) {
			ListLevelDestination destination = new ListLevelDestination(importer);
			importer.Position.CharacterFormatting.BeginUpdate();
			importer.Position.CharacterFormatting.ResetUse(CharacterFormattingOptions.Mask.UseAll);
			importer.Position.CharacterFormatting.EndUpdate();
			importer.Destination = destination;
		}
		#endregion
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			TranslateKeywordHandler translator;
			if (KeywordHT.TryGetValue(keyword, out translator)) {
				translator(Importer, parameterValue, hasParameter);
				return true;
			}
			return false;
		}
		protected override void ProcessCharCore(char ch) {
		}
		protected override DestinationBase CreateClone() {
			ListTableDestination clone = new ListTableDestination(Importer);
			clone.CurrentList = this.CurrentList;
			return clone;
		}
	}
	#endregion
}

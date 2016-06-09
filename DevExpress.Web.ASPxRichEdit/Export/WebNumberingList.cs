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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebAbstractNumberingList : IHashtableProvider {
		public WebAbstractNumberingList(AbstractNumberingList abstractNumberingList, CharacterFormattingBaseExporter characterPropertiesExporter, ParagraphFormattingBaseExporter paragraphPropertiesExporter, ListLevelPropertiesExporter listLevelPropertiesExporter) {
			AbstractNumberingList = abstractNumberingList;
			CharacterPropertiesExporter = characterPropertiesExporter;
			ParagraphPropertiesExporter = paragraphPropertiesExporter;
			ListLevelPropertiesExporter = listLevelPropertiesExporter;
		}
		protected CharacterFormattingBaseExporter CharacterPropertiesExporter { get; set; }
		protected ParagraphFormattingBaseExporter ParagraphPropertiesExporter { get; set; }
		protected ListLevelPropertiesExporter ListLevelPropertiesExporter { get; set; }
		public AbstractNumberingList AbstractNumberingList { get; set; }
		public void FillHashtable(System.Collections.Hashtable result) {
			var levelsCollection = new HashtableCollection<WebNumberingListLevel>();
			for(var i = 0; i < AbstractNumberingList.Levels.Count; i++) {
				levelsCollection.Add(new WebNumberingListLevel(AbstractNumberingList.Levels[i], CharacterPropertiesExporter, ParagraphPropertiesExporter, ListLevelPropertiesExporter));
			}
			result["deleted"] = AbstractNumberingList.Deleted;
			result["id"] = AbstractNumberingList.Id;
			result["levels"] = levelsCollection.ToHashtableCollection();
		}
	}
	public class WebNumberingList : IHashtableProvider {
		public WebNumberingList(NumberingList numberingList, CharacterFormattingBaseExporter characterPropertiesExporter, ParagraphFormattingBaseExporter paragraphPropertiesExporter, ListLevelPropertiesExporter listLevelPropertiesExporter) {
			NumberingList = numberingList;
			CharacterPropertiesExporter = characterPropertiesExporter;
			ParagraphPropertiesExporter = paragraphPropertiesExporter;
			ListLevelPropertiesExporter = listLevelPropertiesExporter;
		}
		protected CharacterFormattingBaseExporter CharacterPropertiesExporter { get; set; }
		protected ParagraphFormattingBaseExporter ParagraphPropertiesExporter { get; set; }
		protected ListLevelPropertiesExporter ListLevelPropertiesExporter { get; set; }
		protected NumberingList NumberingList { get; set; }
		public void FillHashtable(System.Collections.Hashtable result) {
			var levelsCollection = new HashtableCollection<IHashtableProvider>();
			for(var i = 0; i < NumberingList.Levels.Count; i++) {
				var level = NumberingList.Levels[i];
				var overrideLevel = level as OverrideListLevel;
				if(overrideLevel != null)
					levelsCollection.Add(new WebNumberingOverrideListLevel(overrideLevel, CharacterPropertiesExporter, ParagraphPropertiesExporter, ListLevelPropertiesExporter));
				else
					levelsCollection.Add(new WebNumberingListReferenceLevel((NumberingListReferenceLevel)level));
			}
			result["alIndex"] = ((IConvertToInt<AbstractNumberingListIndex>)NumberingList.AbstractNumberingListIndex).ToInt();
			result["id"] = NumberingList.Id;
			result["levels"] = levelsCollection.ToHashtableCollection();
		}
	}
	public class WebNumberingListLevel : IHashtableProvider {
		public WebNumberingListLevel(XtraRichEdit.Model.ListLevel listLevel, CharacterFormattingBaseExporter characterPropertiesExporter, ParagraphFormattingBaseExporter paragraphPropertiesExporter, ListLevelPropertiesExporter listLevelPropertiesExporter) {
			ListLevel = listLevel;
			CharacterPropertiesExporter = characterPropertiesExporter;
			ParagraphPropertiesExporter = paragraphPropertiesExporter;
			ListLevelPropertiesExporter = listLevelPropertiesExporter;
		}
		protected CharacterFormattingBaseExporter CharacterPropertiesExporter { get; set; }
		protected ParagraphFormattingBaseExporter ParagraphPropertiesExporter { get; set; }
		protected ListLevelPropertiesExporter ListLevelPropertiesExporter { get; set; }
		protected ListLevel ListLevel { get; set; }
		public void FillHashtable(System.Collections.Hashtable result) {
			FillHashtableCore(result);
		}
		protected virtual void FillHashtableCore(System.Collections.Hashtable result) {
			CharacterPropertiesExporter.RegisterItem(ListLevel.CharacterProperties.Index, ListLevel.DocumentModel.Cache.CharacterFormattingCache[ListLevel.CharacterProperties.Index]);
			ParagraphPropertiesExporter.RegisterItem(ListLevel.ParagraphProperties.Index, ListLevel.DocumentModel.Cache.ParagraphFormattingCache[ListLevel.ParagraphProperties.Index]);
			ListLevelPropertiesExporter.RegisterItem(ListLevel.ListLevelProperties.Index, ListLevel.ListLevelProperties);
			result["characterPropertiesCacheIndex"] = ListLevel.CharacterProperties.Index;
			result["paragraphPropertiesCacheIndex"] = ListLevel.ParagraphProperties.Index;
			result["listLevelPropertiesCacheIndex"] = ListLevel.ListLevelProperties.Index;
		}
	}
	public class WebNumberingOverrideListLevel : WebNumberingListLevel {
		public WebNumberingOverrideListLevel(XtraRichEdit.Model.OverrideListLevel listLevel, CharacterFormattingBaseExporter characterPropertiesExporter, ParagraphFormattingBaseExporter paragraphPropertiesExporter, ListLevelPropertiesExporter listLevelPropertiesExporter)
			: base(listLevel, characterPropertiesExporter, paragraphPropertiesExporter, listLevelPropertiesExporter) { }
		protected new OverrideListLevel ListLevel { get { return (OverrideListLevel)base.ListLevel; } }
		protected override void FillHashtableCore(System.Collections.Hashtable result) {
			base.FillHashtableCore(result);
			result["newStart"] = ListLevel.NewStart;
			result["overrideStart"] = ListLevel.OverrideStart;
		}
	}
	public class WebNumberingListReferenceLevel : IHashtableProvider {
		public WebNumberingListReferenceLevel(NumberingListReferenceLevel listLevel) {
			ListLevel = listLevel;
		}
		protected NumberingListReferenceLevel ListLevel { get; set; }
		public void FillHashtable(System.Collections.Hashtable result) {
			result["newStart"] = ListLevel.NewStart;
			result["overrideStart"] = ListLevel.OverrideStart;
			result["level"] = ListLevel.Level;
		}
	}
}

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Web.ASPxRichEdit.Export;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class TabParagraphWebCommandBase : WebRichEditStateBasedCommand<IntervalCommandState> {
		public TabParagraphWebCommandBase(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModelCore(IntervalCommandState stateObject) {
			TabInfo tabInfo = TabInfoExporter.FromHashtable(stateObject.Value as Hashtable);
			var paragraphIndex = DocumentModel.MainPieceTable.FindParagraphIndex(stateObject.Position);
			var endPosition = stateObject.Position + stateObject.Length;
			while(true) {
				Paragraph paragraph = DocumentModel.MainPieceTable.Paragraphs[paragraphIndex];
				ModifyParagraph(paragraph, tabInfo);
				paragraphIndex++;
				if(paragraph.EndLogPosition + 1 >= endPosition) break;
			}
		}
		protected abstract void ModifyParagraph(Paragraph paragraph, TabInfo tab);
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.ParagraphTabsAllowed;
		}
	}
	public class InsertTabToParagraphWebCommand : TabParagraphWebCommandBase {
		public InsertTabToParagraphWebCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.InsertTabToParagraph; } }
		protected override bool IsEnabled() { return true; }
		protected override void ModifyParagraph(Paragraph paragraph, TabInfo tab) {
			TabFormattingInfo tabs = paragraph.Tabs.GetTabs();
			if(!tabs.Contains(tab)) {
				tabs.Add(tab);
				paragraph.Tabs.SetTabs(tabs);
			}
		}
	}
	public class DeleteTabAtParagraphWebCommand : TabParagraphWebCommandBase {
		public DeleteTabAtParagraphWebCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.DeleteTabAtParagraph; } }
		protected override bool IsEnabled() { return true; }
		protected override void ModifyParagraph(Paragraph paragraph, TabInfo tab) {
			TabFormattingInfo tabs = paragraph.GetTabs();
			if(tabs.Contains(tab)) {
				TabFormattingInfo paragraphTabs = paragraph.Tabs.GetTabs();
				paragraphTabs.Remove(tab);
				TabFormattingInfo styleTabs = paragraph.ParagraphStyle.GetTabs();
				if(styleTabs.Contains(tab)) {
					tab = new TabInfo(tab.Position, tab.Alignment, tab.Leader, true);
					paragraphTabs.Add(tab);
				}
				paragraph.Tabs.SetTabs(paragraphTabs);
			}
		}
	}
}
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebTabsProperties : IHashtableProvider {
		TabInfo tabInfo;
		public WebTabsProperties(TabInfo tab) {
			tabInfo = tab;
		}
		public void FillHashtable(Hashtable result) {
			TabInfoExporter.FillHashtable(result, tabInfo);
		}
	}
	public class WebTabsCollection : HashtableCollection<WebTabsProperties> {
		public WebTabsCollection(TabProperties tabs) {
			int count = tabs.Info.Count;
			for (int i = 0; i < count; i++)
				Add(new WebTabsProperties(tabs[i]));
		}
		public WebTabsCollection() {
		}
	}
}

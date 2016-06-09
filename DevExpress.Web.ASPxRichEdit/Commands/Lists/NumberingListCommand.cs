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

using DevExpress.Web.ASPxRichEdit.Export;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class NumberingListCommand : WebRichEditLoadModelCommandBase {
		public NumberingListCommand(WebRichEditLoadModelCommandBase parentCommand, DocumentModel documentModel)
			: base(parentCommand, new Hashtable(), documentModel) { }
		public override CommandType Type { get { return CommandType.NumberingLists; } }
		protected override bool IsEnabled() { return true; }
		protected CharacterFormattingBaseExporter CharacterPropertiesExporter { get; private set; }
		protected ParagraphFormattingBaseExporter ParagraphPropertiesExporter { get; private set; }
		protected ListLevelPropertiesExporter ListLevelPropertiesExporter { get; private set; }
		protected internal bool SkipTemplates { get; set; }
		protected override void FillHashtable(Hashtable result) {
			InitializeExporters();
			result.Add("abstractNumberingLists", GetAbstractNumberingListCollection());
			result.Add("numberingLists", GetNumberingListCollection());
			if(!SkipTemplates)
				result.Add("abstractNumberingListTemplates", GetAbstractNumberingListTemplatesCollection());
			result.Add("characterPropertiesCache", CharacterPropertiesExporter.Cache);
			result.Add("paragraphPropertiesCache", ParagraphPropertiesExporter.Cache);
			result.Add("listLevelPropertiesCache", ListLevelPropertiesExporter.Cache);
		}
		protected void InitializeExporters() {
			CharacterPropertiesExporter = new CharacterFormattingBaseExporter(DocumentModel, WorkSession.FontInfoCache);
			ParagraphPropertiesExporter = new ParagraphFormattingBaseExporter(DocumentModel);
			ListLevelPropertiesExporter = new ListLevelPropertiesExporter();
		}
		protected IEnumerable<Hashtable> GetAbstractNumberingListTemplatesCollection() {
			var collection = new HashtableCollection<WebAbstractNumberingList>();
			foreach(var abstractNumberingList in WorkSession.NumberingListTemplates)
				collection.Add(new WebAbstractNumberingList(abstractNumberingList, CharacterPropertiesExporter, ParagraphPropertiesExporter, ListLevelPropertiesExporter));
			return collection.ToHashtableCollection();
		}
		protected IEnumerable<Hashtable> GetAbstractNumberingListCollection() {
			var collection = new HashtableCollection<WebAbstractNumberingList>();
			foreach(var abstractNumberingList in DocumentModel.AbstractNumberingLists)
				collection.Add(new WebAbstractNumberingList(abstractNumberingList, CharacterPropertiesExporter, ParagraphPropertiesExporter, ListLevelPropertiesExporter));
			return collection.ToHashtableCollection();
		}
		protected IEnumerable<Hashtable> GetNumberingListCollection() {
			var collection = new HashtableCollection<WebNumberingList>();
			foreach(var numberingList in DocumentModel.NumberingLists)
				collection.Add(new WebNumberingList(numberingList, CharacterPropertiesExporter, ParagraphPropertiesExporter, ListLevelPropertiesExporter));
			return collection.ToHashtableCollection();
		}
	}
}

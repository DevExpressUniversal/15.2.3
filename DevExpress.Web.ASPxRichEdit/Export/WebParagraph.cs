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

using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebParagraph : IHashtableProvider {
		int mergedParagraphFormattingCacheIndex = -1;
		int paragraphPropertiesIndex = -1;
		int paragraphStyleIndex;
		public WebParagraph(RichEditServerResponse serverResponse) {
			Guard.ArgumentNotNull(serverResponse, "serverResponse");
			serverResponse.Paragraphs.Add(this);
		}
		public DocumentLogPosition LogPosition { get; protected internal set; }
		public DocumentLogPosition EndLogPosition { get; protected internal set; }
		public int Length { get { return EndLogPosition - LogPosition + 1; } }
		public int ParagraphPropertiesIndex { get { return paragraphPropertiesIndex; } protected internal set { paragraphPropertiesIndex = value; } }
		public int MergedParagraphFormattingCacheIndex { get { return mergedParagraphFormattingCacheIndex; } protected internal set { mergedParagraphFormattingCacheIndex = value; } }
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } protected internal set { paragraphStyleIndex = value; } }
		public int NumberingListIndex { get; protected internal set; }
		public int ListLevelIndex { get; protected internal set; }
		public WebTabsCollection Tabs { get; protected internal set; }
		public void FillHashtable(Hashtable result) {
			result.Add("maskedParagraphPropertiesIndex", paragraphPropertiesIndex);
			result.Add("mergedParagraphFormattingCacheIndex", mergedParagraphFormattingCacheIndex);
			result.Add("paragraphStyleIndex", paragraphStyleIndex);
			result.Add("logPosition", ((IConvertToInt<DocumentLogPosition>)LogPosition).ToInt());
			result.Add("length", Length);
			if (Tabs != null && Tabs.Count > 0)
				result.Add("tabs", Tabs.ToHashtableCollection());
			result.Add("listIndex", NumberingListIndex);
			result.Add("listLevelIndex", ListLevelIndex);
		}
	}
	public class ParagraphHashtableCollection : HashtableCollection<WebParagraph> {
	}
}

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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.Web.ASPxRichEdit.Export {
	#region WebParagraphStyle
	public class WebParagraphStyle : WebStyleBase<ParagraphStyle> {
		WebTabsCollection tabs;
		string linkedStyleName;
		bool hasLinkedStyle;
		bool autoUpdate;
		string nextParagraphStyleName;
		int numberingListIndex;
		int listLevelIndex;
		public int CharacterPropertiesCacheIndex { get; private set; }
		public int ParagraphPropertiesCacheIndex { get; private set; }
		public WebParagraphStyle(ParagraphStyle modelStyle)
			: base(modelStyle) {
		}
		protected override void Initialize(ParagraphStyle modelStyle) {
			CharacterPropertiesCacheIndex = modelStyle.CharacterProperties.Index;
			ParagraphPropertiesCacheIndex = modelStyle.ParagraphProperties.Index;
			this.hasLinkedStyle = modelStyle.HasLinkedStyle;
			if (modelStyle.HasLinkedStyle)
				this.linkedStyleName = modelStyle.LinkedStyle.StyleName;
			if (modelStyle.NextParagraphStyle != null)
				this.nextParagraphStyleName = modelStyle.NextParagraphStyle.StyleName;
			this.autoUpdate = modelStyle.AutoUpdate;
			this.tabs = new WebTabsCollection(modelStyle.Tabs);
			this.numberingListIndex = ((IConvertToInt<NumberingListIndex>)modelStyle.GetOwnNumberingListIndex()).ToInt();
			this.listLevelIndex = modelStyle.GetOwnListLevelIndex();
		}
		protected override void FillHashtableCore(Hashtable result) {
			result["characterPropertiesCacheIndex"] = CharacterPropertiesCacheIndex;
			result["paragraphPropertiesCacheIndex"] = ParagraphPropertiesCacheIndex;
			result["tabs"] = tabs.ToHashtableCollection();
			result["linkedStyleName"] = linkedStyleName;
			result["hasLinkedStyle"] = hasLinkedStyle;
			result["nextParagraphStyleName"] = nextParagraphStyleName;
			result["autoUpdate"] = autoUpdate;
			result["numberingListIndex"] = numberingListIndex;
			result["listLevelIndex"] = listLevelIndex;
		}
		protected override bool IsDefaultStyle(ParagraphStyle modelStyle) {
			return modelStyle.DocumentModel.ParagraphStyles.DefaultItem == modelStyle;
		}
	}
	public class WebParagraphStyleCollection : HashtableCollection<WebParagraphStyle> { }
	#endregion
}

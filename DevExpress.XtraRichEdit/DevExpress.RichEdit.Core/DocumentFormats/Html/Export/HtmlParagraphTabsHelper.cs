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
using System.Collections.Specialized;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.XtraRichEdit.Export.Html {
	#region HtmlParagraphTabsHelper
	public class HtmlParagraphTabsHelper {
		readonly DocumentModelUnitConverter unitConverter;
		readonly HtmlStyleHelper styleHelper;
		public HtmlParagraphTabsHelper(DocumentModelUnitConverter unitConverter, HtmlStyleHelper styleHelper) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			Guard.ArgumentNotNull(styleHelper, "styleHelper");
			this.unitConverter = unitConverter;
			this.styleHelper = styleHelper;
		}
		public DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		public HtmlStyleHelper HtmlStyleHelper { get { return styleHelper; } }
		public string CreateTabPlaceholder(int width, int spaceWidth, int fillCharacterWidth, char fillCharacter) {
			int fillCharacterCount = 0;
			width -= spaceWidth;
			if (width > 0)
				fillCharacterCount = width / fillCharacterWidth;
			return new String(fillCharacter, fillCharacterCount) + " ";
		}
		public string CreateTabStops(Paragraph paragraph) {
			TabFormattingInfo tabs = paragraph.GetTabs();
			StringCollection tabAttributes = new StringCollection();
			int count = tabs.Count;
			for (int i = 0; i < count; i++)
				tabAttributes.Add(CreateTabStopAttribute(tabs[i]));
			return HtmlStyleHelper.CreateCssStyle(tabAttributes, ' ');
		}
		internal string CreateTabStopAttribute(TabInfo tab) {
			if (tab.Deleted)
				return String.Empty;
			StringCollection attributes = new StringCollection();
			attributes.Add(GetHtmlTabInfoAlign(tab.Alignment));
			attributes.Add(GetHtmlTabInfoLeader(tab.Leader));
			attributes.Add(HtmlStyleHelper.GetHtmlSizeInPoints(UnitConverter.ModelUnitsToPointsF(tab.Position)));
			return HtmlStyleHelper.CreateCssStyle(attributes, ' ');
		}
		internal string CreateMsoTabStopAttributeValue(int tabCount, TabLeaderType leader) {
			StringCollection attributes = new StringCollection();
			attributes.Add(tabCount.ToString());
			attributes.Add(GetHtmlTabInfoLeader(leader));
			return HtmlStyleHelper.CreateCssStyle(attributes, ' ');
		}
		internal string GetHtmlTabInfoAlign(TabAlignmentType align) {
			switch (align) {
				default:
				case TabAlignmentType.Left:
					return "left";
				case TabAlignmentType.Right:
					return "right";
				case TabAlignmentType.Center:
					return "center";
				case TabAlignmentType.Decimal:
					return "decimal";
			}
		}
		internal string GetHtmlTabInfoLeader(TabLeaderType leader) {
			switch (leader) {
				default:
				case TabLeaderType.EqualSign:
				case TabLeaderType.None:
					return String.Empty;
				case TabLeaderType.Dots:
					return "dotted";
				case TabLeaderType.Underline:
					return "lined";
				case TabLeaderType.Hyphens:
					return "dashed";
				case TabLeaderType.MiddleDots:
					return "middot";
				case TabLeaderType.ThickLine:
					return "heavy";
			}
		}
		internal char GetFillCharacter(TabLeaderType leader) {
			switch (leader) {
				default:
				case TabLeaderType.EqualSign:
				case TabLeaderType.None:
					return Characters.NonBreakingSpace;
				case TabLeaderType.Dots:
					return Characters.Dot;
				case TabLeaderType.Underline:
					return Characters.Underscore;
				case TabLeaderType.Hyphens:
					return Characters.Dash;
				case TabLeaderType.MiddleDots:
					return Characters.MiddleDot;
				case TabLeaderType.ThickLine:
					return Characters.Underscore;
			}
		}
		internal int GetFillCharacterWidth(FontInfo fontInfo, TabLeaderType leader) {
			switch (leader) {
				default:
				case TabLeaderType.EqualSign:
				case TabLeaderType.None:
					return fontInfo.NonBreakingSpaceWidth;
				case TabLeaderType.Dots:
					return fontInfo.DotWidth;
				case TabLeaderType.Underline:
					return fontInfo.UnderscoreWidth;
				case TabLeaderType.Hyphens:
					return fontInfo.DashWidth;
				case TabLeaderType.MiddleDots:
					return fontInfo.MiddleDotWidth;
				case TabLeaderType.ThickLine:
					return fontInfo.UnderscoreWidth;
			}
		}
	}
	#endregion
}

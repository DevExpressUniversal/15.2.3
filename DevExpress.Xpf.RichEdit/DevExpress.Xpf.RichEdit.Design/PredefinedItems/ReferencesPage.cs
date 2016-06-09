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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
	public static partial class BarInfos {
		#region TableOfContents
		public static BarInfo ReferencesTableOfContents { get { return referencesTableOfContents; } }
		static readonly BarInfo referencesTableOfContents = new BarInfo(
			String.Empty,
			"References",
			"Table of Contents",
			new BarInfoItems(
				new string[] { "ReferencesInsertTableOfContents", "ReferencesAddParagraphsToTableOfContents", "ReferencesUpdateTableOfContents" },
				new BarItemInfo[] { BarItemInfos.Button,
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "FormatParagraphSetBodyTextLevel", "FormatParagraphSetHeading1Level", "FormatParagraphSetHeading2Level", "FormatParagraphSetHeading3Level", "FormatParagraphSetHeading4Level", "FormatParagraphSetHeading5Level", "FormatParagraphSetHeading6Level", "FormatParagraphSetHeading7Level", "FormatParagraphSetHeading8Level", "FormatParagraphSetHeading9Level" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
					BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageReferences",
			"Caption_GroupTableOfContents"
		);
		#endregion
		#region Captions
		public static BarInfo Captions { get { return captions; } }
		static readonly BarInfo captions = new BarInfo(
			String.Empty,
			"References",
			"Captions",
			new BarInfoItems(
				new string[] { "ReferencesInsertCaptionPlaceholder", "ReferencesInsertTableOfFiguresPlaceholder", "ReferencesUpdateTableOfCaptions" },
				new BarItemInfo[] { new BarSubItemInfo(new BarInfoItems(
						new string[] { "ReferencesInsertFiguresCaption", "ReferencesInsertTablesCaption", "ReferencesInsertEquationsCaption" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
				new BarSubItemInfo(new BarInfoItems(
						new string[] { "ReferencesInsertTableOfFigures", "ReferencesInsertTableOfTables", "ReferencesInsertTableOfEquations" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
				BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageReferences",
			"Caption_GroupCaptions"
		);
		#endregion
	}
}

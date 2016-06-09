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
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public enum CommandType { 
		Unknown = 0,
		FixedLengthText = 1,
		DocumentProperties = 2,
		Sections = 3,
		Styles = 4,
		ControlOptions = 5,
		OpenDocument = 6,
		SaveDocument = 7,
		NewDocument = 8,
		SaveAsDocument = 9,
		DeleteRuns = -10,
		InsertSimpleRun = -11,
		InsertParagraph = -12,
		InsertSection = -13,
		ChangeCharacterProperties = -14,
		ChangeParagraphProperties = -15,
		ChangeSectionProperties = -16,
		ChangeInlineObjectProperties = -17,
		ChangeTextBuffer = -18,
		DelayedPrint = -19,
		InsertInlinePicture = -20,
		Start = 21,
		ApplyCharacterStyle = -22,
		ApplyParagraphStyle = -23,
		NumberingLists = 24,
		ApplyNumberingList = -25,
		ChangeCharacterPropertiesUseValue = -26,
		ChangeParagraphPropertiesUseValue = -27,
		DeleteTabAtParagraph = -28,
		InsertTabToParagraph = -29,
		AddNumberingList = -30,
		DeleteNumberingList = -31,
		AddAbstractNumberingList = -32,
		DeleteAbstractNumberingList = -33,
		ChangeListLevelCharacterProperties = -34,
		ChangeListLevelParagraphProperties = -35,
		ChangeListLevelProperties = -36,
		ChangeIOverrideListLevel = -37,
		FieldLists = 38,
		CreateStyleLink = -39,
		DeleteStyleLink = -40,
		FieldUpdate = -41,
		LoadInlinePictures = -42,
		InsertField = -43,
		ReloadDocument = 44,
		UpdateInlinePictures = -45,
		HyperlinkInfoChanged = -46,
		DeleteField = -47,
		LoadDocument = 48,
		ChangeDefaultTabWidth = -49,
		MergeSections = -50,
		MergeParagraphs = -51,
		Bookmarks = 52,
		ChangePageColor = -53,
		CreateHeader = -54,
		CreateFooter = -55,
		ChangeHeaderIndex = -56,
		ChangeFooterIndex = -57,
		SaveMergedDocument = -58,
		LoadHeaders = 59,
		LoadFooters = 60,
		LoadStringResources = 61,
		CreateBookmark = -62,
		DeleteBookmark = -63,
		ChangeDifferentOddAndEvenPages = -64,
		LoadSubDocument = 65,
		CreateTable = -66,
		RemoveTable = -67,
		ShiftTableStartPosition = -68,
		ChangeTableCell = -69,
		ChangeTableCellProperty = -70,
		ChangeTableRow = -71,
		ChangeTableRowProperty = -72,
		ChangeTable = -73,
		ChangeTableProperty = -74,
		SplitTableCellHorizontally = -75,
		MergeTableCellHorizontally = -76,
		InsertTableRow = -77,
		RemoveTableRow = -78,
		RemoveTableCell = -79,
		InsertTableCell = -80,
		ApplyTableStyle = -81
	}
}

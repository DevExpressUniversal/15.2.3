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
		#region WriteAndInsertFields
		public static BarInfo MailingsWriteAndInsertFields { get { return mailingsWriteAndInsertFields; } }
		static readonly BarInfo mailingsWriteAndInsertFields = new BarInfo(
			String.Empty,
			"Mail Merge",
			"Write & Insert Fields",
			new BarInfoItems(
				new string[] { "MailMergeInsertFieldPlaceholder" },
				new BarItemInfo[] { new InsertMergeFieldsBarSubItemInfo() }
			),
			String.Empty,
			String.Empty,
			"Caption_PageMailings",
			"Caption_GroupWriteInsertFields"
		);
		#endregion
		#region PreviewResults
		public static BarInfo MailingsPreviewResults { get { return mailingsPreviewResults; } }
		static readonly BarInfo mailingsPreviewResults = new BarInfo(
			String.Empty,
			"Mail Merge",
			"Preview Results",
			new BarInfoItems(
				new string[] { "MailMergeViewMergedData", "MailMergeShowAllFieldCodes", "MailMergeShowAllFieldResults", "MailMergeFirstDataRecord", "MailMergePreviousDataRecord", "MailMergeNextDataRecord", "MailMergeLastDataRecord", "MailMergeSaveDocumentAs" },
				new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageMailings",
			"Caption_GroupPreviewResults"
		);
		#endregion
	}
}

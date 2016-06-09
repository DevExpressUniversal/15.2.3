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
		#region Protect
		public static BarInfo ReviewProtection { get { return reviewProtection; } }
		static readonly BarInfo reviewProtection = new BarInfo(
			String.Empty,
			"Review",
			"Protect",
			new BarInfoItems(
				new string[] { "ReviewProtectDocument", "ReviewEditPermissionRange", "ReviewUnprotectDocument" },
				new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageReview",
			"Caption_GroupDocumentProtection"
		);
		#endregion
		#region Proofing
		public static BarInfo ReviewProofing { get { return reviewProofing; } }
		static readonly BarInfo reviewProofing = new BarInfo(
			String.Empty,
			"Review",
			"Proofing",
			new BarInfoItems(
				new string[] { "CheckSpelling" },
				new BarItemInfo[] { BarItemInfos.Button }
			),
			String.Empty,
			String.Empty,
			"Caption_PageReview",
			"Caption_GroupDocumentProofing"
		);
		#endregion
		#region Comment
		public static BarInfo ReviewComment { get { return reviewComment; } }
		static readonly BarInfo reviewComment = new BarInfo(
			String.Empty,
			"Review",
			"Comment",
			new BarInfoItems(
				new string[] { "ReviewNewComment", "ReviewDeleteCommentsPlaceholder", "ReviewPreviousComment", "ReviewNextComment"},
				new BarItemInfo[] { BarItemInfos.Button, new BarSubItemInfo(new BarInfoItems(
						new string[] { "ReviewDeleteOneComment", "ReviewDeleteAllCommentsShown", "ReviewDeleteAllComments" },
						new BarItemInfo[] { BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph, BarItemInfos.ButtonLargeRibbonGlyph})),
						BarItemInfos.Button, BarItemInfos.Button }
				),
				String.Empty,
				String.Empty,
				"Caption_PageReview",
				"Caption_GroupComment"
				);
		#endregion
		#region Tracking
		public static BarInfo ReviewTracking { get { return reviewTracking; } }
		static readonly BarInfo reviewTracking = new BarInfo(
			String.Empty,
			"Review",
			"Tracking",
			new BarInfoItems(
				new string[] { "ReviewViewComment", "ReviewReviewers", "ReviewReviewingPane" },
				new BarItemInfo[] { BarItemInfos.Check, new ReviewersSubItemInfo(), BarItemInfos.Check }
				),
				String.Empty,
				String.Empty,
				"Caption_PageReview",
				"Caption_GroupTracking"
				);
		#endregion
	}
}

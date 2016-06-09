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
using System.Text;
namespace DevExpress.XtraRichEdit.Native {
	public static class ParagraphPropertiesHelper {
		public static void ForEach(IParagraphPropertiesActions actions) {
			actions.AlignmentAction();
			actions.FirstLineIndentAction();
			actions.FirstLineIndentTypeAction();
			actions.LeftIndentAction();
			actions.LineSpacingAction();
			actions.LineSpacingTypeAction();
			actions.RightIndentAction();
			actions.SpacingAfterAction();
			actions.SpacingBeforeAction();
			actions.SuppressHyphenationAction();
			actions.SuppressLineNumbersAction();
			actions.ContextualSpacingAction();
			actions.PageBreakBeforeAction();
			actions.BeforeAutoSpacingAction();
			actions.AfterAutoSpacingAction();
			actions.KeepWithNextAction();
			actions.KeepLinesTogetherAction();
			actions.WidowOrphanControlAction();
			actions.OutlineLevelAction();
			actions.BackColorAction();
			actions.ShadingAction();
			actions.FramePropertiesAction();
			actions.LeftBorderAction();
			actions.RightBorderAction();
			actions.TopBorderAction();
			actions.BottomBorderAction();
		}
	}
	public interface IParagraphPropertiesActions {
		void AlignmentAction();
		void FirstLineIndentAction();
		void FirstLineIndentTypeAction();
		void LeftIndentAction();
		void LineSpacingAction();
		void LineSpacingTypeAction();
		void RightIndentAction();
		void SpacingAfterAction();
		void SpacingBeforeAction();
		void SuppressHyphenationAction();
		void SuppressLineNumbersAction();
		void ContextualSpacingAction();
		void PageBreakBeforeAction();
		void BeforeAutoSpacingAction();
		void AfterAutoSpacingAction();
		void KeepWithNextAction();
		void KeepLinesTogetherAction();
		void WidowOrphanControlAction();
		void OutlineLevelAction();
		void BackColorAction();
		void ShadingAction();
		void FramePropertiesAction();
		void LeftBorderAction();
		void RightBorderAction();
		void TopBorderAction();
		void BottomBorderAction();
	}
}

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
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region EnsureCaretVisibleHorizontallyCommand
	public class EnsureCaretVisibleHorizontallyCommand : RichEditCaretBasedCommand {
		public EnsureCaretVisibleHorizontallyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EnsureCaretVisibleHorizontally; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EnsureCaretVisibleHorizontallyDescription; } }
		#endregion
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			UpdateCaretPosition(DocumentLayoutDetailsLevel.Character);
			Debug.Assert(CaretPosition.PageViewInfo != null);
			if (CaretPosition.PageViewInfo == null)
				return;
			Rectangle logicalVisibleBounds = ActiveView.CreateLogicalRectangle(CaretPosition.PageViewInfo, ActiveView.Bounds);
			if (CaretPosition.LayoutPosition.DetailsLevel >= DocumentLayoutDetailsLevel.Character) {
				Rectangle caretBounds = CaretPosition.CalculateCaretBounds();
				if (!IsCaretVisible(logicalVisibleBounds, caretBounds))
					ScrollToMakeCaretVisible(logicalVisibleBounds, caretBounds);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = true;
			state.Visible = true;
			state.Checked = false;
		}
		protected internal bool IsCaretVisible(Rectangle logicalVisibleBounds, Rectangle caretBounds) {
			Debug.Assert(CaretPosition.PageViewInfo != null);
			return logicalVisibleBounds.Contains(caretBounds);
		}
		protected internal virtual int CalculateOptimalCaretHorizontalPosition(Rectangle logicalVisibleBounds, Rectangle caretBounds) {
			Rectangle optimalLogicalVisibleBounds = logicalVisibleBounds;
			optimalLogicalVisibleBounds.X = caretBounds.X - 2 * logicalVisibleBounds.Width / 3;
			Rectangle columnBounds = CaretPosition.LayoutPosition.Column.Bounds;
			CommentCaretPosition position = CaretPosition as CommentCaretPosition;
			if (position != null)
				return position.LayoutPosition.Page.Bounds.Right;
			if (optimalLogicalVisibleBounds.X < columnBounds.X)
				optimalLogicalVisibleBounds.X = columnBounds.X;
			Rectangle pageBounds = CaretPosition.LayoutPosition.Page.Bounds;
			if (optimalLogicalVisibleBounds.Right > pageBounds.Right)
				optimalLogicalVisibleBounds.X = pageBounds.Right - optimalLogicalVisibleBounds.Width;
			return optimalLogicalVisibleBounds.X;
		}
		protected internal virtual void ScrollToMakeCaretVisible(Rectangle logicalVisibleBounds, Rectangle caretBounds) {
			int optimalHorizontalPosition = CalculateOptimalCaretHorizontalPosition(logicalVisibleBounds, caretBounds);
			IScrollBarAdapter scrollBarAdapter = ActiveView.HorizontalScrollController.ScrollBarAdapter;
			if (scrollBarAdapter.Value != optimalHorizontalPosition) {
				scrollBarAdapter.Value = optimalHorizontalPosition;
				scrollBarAdapter.ApplyValuesToScrollBar();
				ActiveView.PageViewInfoGenerator.LeftInvisibleWidth = ActiveView.HorizontalScrollController.GetLeftInvisibleWidth();
				ActiveView.OnHorizontalScroll();
			}
		}
	}
	#endregion
}

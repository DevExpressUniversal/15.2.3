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
using System.ComponentModel;
using System.Reflection;
using DevExpress.Utils.Commands;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ScrollVerticallyByPhysicalOffsetCommandBase (abstract class)
	public abstract class ScrollVerticallyByPhysicalOffsetCommandBase : RichEditMenuItemSimpleCommand {
		bool updateScrollBarBeforeExecution = true;
		bool executedSuccessfully;
		protected ScrollVerticallyByPhysicalOffsetCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public abstract int AbsolutePhysicalVerticalOffset { get; }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public bool UpdateScrollBarBeforeExecution { get { return updateScrollBarBeforeExecution; } set { updateScrollBarBeforeExecution = value; } }
		protected internal virtual RichEditView View { get { return ActiveView; } }
		internal bool ExecutedSuccessfully { get { return executedSuccessfully; } }
		#endregion
		protected internal override void ExecuteCore() {
			View.CheckExecutedAtUIThread();
			if (UpdateScrollBarBeforeExecution) {
				View.VerticalScrollController.UpdateScrollBar();
			}
			int offset = AbsolutePhysicalVerticalOffset;
			ScrollByPhysicalHeightCalculator calculator = CreateScrollDeltaCalculator(offset < 0);
			long delta = calculator.CalculateScrollDelta(Math.Abs(offset));
			if (delta == 0)
				return;
			long previousValue = View.VerticalScrollController.ScrollBarAdapter.Value;
			GeneratePagesToEnsureScrollingSuccessfull(delta * Math.Sign(offset));
			View.VerticalScrollController.ScrollByTopInvisibleHeightDelta(delta * Math.Sign(offset));
			View.OnVerticalScroll();
			this.executedSuccessfully = previousValue != View.VerticalScrollController.ScrollBarAdapter.Value;
		}
		protected internal virtual void GeneratePagesToEnsureScrollingSuccessfull(long delta) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
		protected internal virtual ScrollByPhysicalHeightCalculator CreateScrollDeltaCalculator(bool scrollUp) {
			if (scrollUp)
				return new ScrollUpByPhysicalHeightCalculator(View);
			else
				return new ScrollDownByPhysicalHeightCalculator(View);
		}
	}
	#endregion
	#region ScrollVerticallyByPhysicalOffsetCommand
	public class ScrollVerticallyByPhysicalOffsetCommand : ScrollVerticallyByPhysicalOffsetCommandBase {
		int physicalOffset;
		public ScrollVerticallyByPhysicalOffsetCommand(IRichEditControl control)
			: base(control) {
			this.physicalOffset = DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(50);
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollVerticallyByPhysicalOffsetCommandPhysicalOffset")]
#endif
public int PhysicalOffset { get { return physicalOffset; } set { physicalOffset = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollVerticallyByPhysicalOffsetCommandAbsolutePhysicalVerticalOffset")]
#endif
public override int AbsolutePhysicalVerticalOffset { get { return PhysicalOffset; } }
	}
	#endregion
	#region ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand
	public class ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand : ScrollVerticallyByPhysicalOffsetCommand {
		public ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void GeneratePagesToEnsureScrollingSuccessfull(long delta) {
			long newTopInvisibleHeight = CalculateNewTopInvisibleHeight(delta);
			PageViewInfoGenerator pageViewInfoGenerator = View.PageViewInfoGenerator;
			if (newTopInvisibleHeight <= pageViewInfoGenerator.TotalHeight - pageViewInfoGenerator.VisibleHeight + 1)
				return;
			View.ResetPages(PageGenerationStrategyType.RunningHeight);
			pageViewInfoGenerator.TopInvisibleHeight = newTopInvisibleHeight;
			View.GeneratePages();
			Debug.Assert(newTopInvisibleHeight < pageViewInfoGenerator.TotalHeight);
		}
		protected internal virtual long CalculateNewTopInvisibleHeight(long delta) {
			return View.PageViewInfoGenerator.TopInvisibleHeight + delta;
		}
	}
	#endregion
	#region ScrollToAbsoluteTopInvisibleHeightPositionCommand
	public class ScrollToAbsoluteTopInvisibleHeightPositionCommand : ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand {
		long newTopInvisibleHeight;
		public ScrollToAbsoluteTopInvisibleHeightPositionCommand(IRichEditControl control)
			: base(control) {
		}
		public long NewTopInvisibleHeight { get { return newTopInvisibleHeight; } set { newTopInvisibleHeight = value; } }
		public long CurrentTopInvisibleHeight { get { return View.PageViewInfoGenerator.TopInvisibleHeight; } }
		protected internal override void ExecuteCore() {
			View.CheckExecutedAtUIThread();
			if (UpdateScrollBarBeforeExecution)
				View.VerticalScrollController.UpdateScrollBar();
			GeneratePagesToEnsureScrollingSuccessfull(0);
			View.VerticalScrollController.ScrollToAbsolutePosition(CalculateNewTopInvisibleHeight(0));
			View.OnVerticalScroll();
		}
		protected internal override long CalculateNewTopInvisibleHeight(long delta) {
			return newTopInvisibleHeight;
		}
	}
	#endregion
	#region ScrollVerticallyByPixelOffsetCommand (abstract class)
	public class ScrollVerticallyByPixelOffsetCommand : ScrollVerticallyByPhysicalOffsetCommandBase {
		int pixelOffset = 1;
		public ScrollVerticallyByPixelOffsetCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollVerticallyByPixelOffsetCommandPixelOffset")]
#endif
public int PixelOffset { get { return pixelOffset; } set { pixelOffset = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollVerticallyByPixelOffsetCommandAbsolutePhysicalVerticalOffset")]
#endif
public override int AbsolutePhysicalVerticalOffset {
			get {
				DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
				return unitConverter.PixelsToLayoutUnits(PixelOffset);
			}
		}
	}
	#endregion
	#region ScrollVerticallyByLogicalOffsetCommand
	public class ScrollVerticallyByLogicalOffsetCommand : ScrollVerticallyByPhysicalOffsetCommandBase {
		int logicalOffset;
		public ScrollVerticallyByLogicalOffsetCommand(IRichEditControl control)
			: base(control) {
			this.logicalOffset = DocumentModel.UnitConverter.DocumentsToModelUnits(150);
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollVerticallyByLogicalOffsetCommandLogicalOffset")]
#endif
		public int LogicalOffset { get { return logicalOffset; } set { logicalOffset = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollVerticallyByLogicalOffsetCommandAbsolutePhysicalVerticalOffset")]
#endif
		public override int AbsolutePhysicalVerticalOffset { get { return (int)Math.Round(LogicalOffset * ActiveView.ZoomFactor); } } 
	}
	#endregion
	#region ScrollHorizontallyByPhysicalOffsetCommand
	public class ScrollHorizontallyByPhysicalOffsetCommand : RichEditMenuItemSimpleCommand {
		int physicalOffset;
		bool executedSuccessfully;
		public ScrollHorizontallyByPhysicalOffsetCommand(IRichEditControl control)
			: base(control) {
			this.physicalOffset = DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(50);
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollHorizontallyByPhysicalOffsetCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollHorizontallyByPhysicalOffsetCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ScrollHorizontallyByPhysicalOffsetCommandPhysicalOffset")]
#endif
		public int PhysicalOffset { get { return physicalOffset; } set { physicalOffset = value; } }
		protected internal virtual RichEditView View { get { return ActiveView; } }
		internal bool ExecutedSuccessfully { get { return executedSuccessfully; } }
		protected internal override void ExecuteCore() {
			View.CheckExecutedAtUIThread();
			PerformScroll();
			View.OnHorizontalScroll();
		}
		protected internal virtual void PerformScroll() {
			long previousValue = View.HorizontalScrollController.ScrollBarAdapter.Value;
			View.HorizontalScrollController.ScrollByLeftInvisibleWidthDelta(PhysicalOffset);
			this.executedSuccessfully = (previousValue != View.HorizontalScrollController.ScrollBarAdapter.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
	}
	#endregion
	#region ScrollToAbsoluteLeftInvisibleWidthPositionCommand
	public class ScrollToAbsoluteLeftInvisibleWidthPositionCommand : ScrollHorizontallyByPhysicalOffsetCommand {
		long newLeftInvisibleWidth;
		public ScrollToAbsoluteLeftInvisibleWidthPositionCommand(IRichEditControl control)
			: base(control) {
		}
		public long NewLeftInvisibleWidth { get { return newLeftInvisibleWidth; } set { newLeftInvisibleWidth = value; } }
		public long CurrentLeftInvisibleWidth { get { return View.PageViewInfoGenerator.LeftInvisibleWidth; } }
		protected internal override void PerformScroll() {
			View.HorizontalScrollController.ScrollToAbsolutePosition(NewLeftInvisibleWidth);
		}
	}
	#endregion
}

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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout;
using System.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Commands {
	#region ZoomCommand
	public class ZoomCommand : ZoomCommandBase {
		double zoomFactor;
		public ZoomCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Zoom; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ZoomDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.Zoom; } }
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<double> valueState = state as IValueBasedCommandUIState<double>;
			if (valueState != null)
				zoomFactor = valueState.Value;
			base.ForceExecute(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<double> valueState = state as IValueBasedCommandUIState<double>;
			if (valueState != null)
				valueState.EditValue = (double)ZoomFactor;
		}
		protected internal override float CalculateNewZoomFactor(float oldZoomFactor) {
			return (float)zoomFactor;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<double> state = new DefaultValueBasedCommandUIState<double>();
			state.Value = ZoomFactor;
			return state;
		}
	}
	#endregion
	#region ZoomPercentCommand
	public class ZoomPercentCommand : ZoomCommandBase {
		double zoomPercent;
		public ZoomPercentCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Zoom; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ZoomDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ZoomPercent; } }
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<double> valueState = state as IValueBasedCommandUIState<double>;
			if (valueState != null)
				zoomPercent = valueState.Value;
			base.ForceExecute(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<double> valueState = state as IValueBasedCommandUIState<double>;
			if (valueState != null)
				valueState.EditValue = (double)(100 * ZoomFactor);
		}
		protected internal override float CalculateNewZoomFactor(float oldZoomFactor) {
			return (float)(zoomPercent / 100.0);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<double> state = new DefaultValueBasedCommandUIState<double>();
			state.Value = ZoomFactor * 100;
			return state;
		}
	}
	#endregion
	public abstract class FitToPageCommandBase : ZoomCommandBase {
		protected FitToPageCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected CaretPosition CaretPosition { get { return ActiveView.CaretPosition; } }
		protected PageCollection Pages { get { return ActiveView.FormattingController.PageController.Pages; } }
		protected internal override void ExecuteCore() {
			base.ExecuteCore();
			ScrollToPage();
		}
		void ScrollToPage() {
			ActiveView.ScrollToPage(GetVisiblePage().PageIndex);
		}
		protected internal override float CalculateNewZoomFactor(float oldZoomFactor) {
			Rectangle pageBounds = GetVisiblePage().Bounds;
			Rectangle availableBounds = GetAvailableBounds();
			float heightZoomFactor = CalculateZoomFactor(pageBounds.Height, availableBounds.Height);
			float widthZoomFactor = CalculateZoomFactor(pageBounds.Width, availableBounds.Width);
			return GetPreferredZoomFactor(heightZoomFactor, widthZoomFactor);
		}
		float CalculateZoomFactor(float pageSide, float viewSide) {
			float zoomFactor = viewSide / pageSide;
			float result = Math.Max(Options.Behavior.MinZoomFactor, zoomFactor);
			return Math.Min(Options.Behavior.MaxZoomFactor, result);
		}
		Page GetVisiblePage() {
			if (CaretPosition != null && CaretPosition.LayoutPosition != null && CaretPosition.LayoutPosition.Page != null)
				return CaretPosition.LayoutPosition.Page;
			return Pages[0];
		}
		Rectangle GetAvailableBounds() {
			Rectangle bounds = ActiveView.Bounds;
			if (Control.UseSkinMargins) {
				bounds.Width -= Control.SkinLeftMargin + Control.SkinRightMargin;
				bounds.Height -= Control.SkinTopMargin + Control.SkinBottomMargin;
			}
			return bounds;
		}
		protected abstract float GetPreferredZoomFactor(float heightZoomFactor, float widthZoomFactor);
	}
	public class FitToPageCommand : FitToPageCommandBase {
		public FitToPageCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FitToPage; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FitToPageDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FitToPage; } }
		protected override float GetPreferredZoomFactor(float heightZoomFactor, float widthZoomFactor) {
			return Math.Min(heightZoomFactor, widthZoomFactor);
		}
	}
	public class FitWidthCommand : FitToPageCommandBase {
		public FitWidthCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FitWidth; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FitWidthDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FitWidth; } }
		protected override float GetPreferredZoomFactor(float heightZoomFactor, float widthZoomFactor) {
			return widthZoomFactor;
		}
	}
	public class FitHeightCommand : FitToPageCommandBase {
		public FitHeightCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FitHeight; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FitHeightDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FitHeight; } }
		protected override float GetPreferredZoomFactor(float heightZoomFactor, float widthZoomFactor) {
			return heightZoomFactor;
		}
	}
}

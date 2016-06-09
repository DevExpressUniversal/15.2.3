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

using DevExpress.Pdf.Native;
using System;
using System.Threading;
namespace DevExpress.Pdf.Drawing {
	public class PdfLinkAnnotationStateController : PdfAnnotationStateController, IPdfViewerValueEditingCallBack {
		readonly string hintText;
		readonly PdfLinkAnnotation linkAnnotation;
		readonly PdfInteractiveOperation interactiveOperation;
		Timer start;
		bool shown;
		protected override bool InteractionSupported { get { return true; } }
		public PdfLinkAnnotationStateController(PdfDocumentStateController documentStateController, PdfAnnotationState state)
			: base(documentStateController, state) {
			this.linkAnnotation = ((PdfLinkAnnotation)state.Annotation);
			this.interactiveOperation = new PdfInteractiveOperation(linkAnnotation.Action, linkAnnotation.Destination);
			PdfUriAction uriAction = linkAnnotation.Action as PdfUriAction;
			if (uriAction != null) {
				hintText = uriAction.Uri;
			}
		}
		void StopTimer() {
			if (start != null) {
				start.Dispose();
				start = null;
			}
		}
		void OnViewerSinchronizationCallback(object context) {
			IPdfViewerController viewerController = DocumentStateController.ViewerController as IPdfViewerController;
			if (viewerController != null)
				viewerController.ValueEditingController.ShowTooltip(
					new PdfStickyNoteEditSettings(State, hintText, null));
		}
		void OnTimerTick(object context) {
			shown = true;
			StopTimer();
			IPdfViewerController viewerController = DocumentStateController.ViewerController as IPdfViewerController;
			if (viewerController != null)
				viewerController.SynchronizationContext.Post(OnViewerSinchronizationCallback, null);
		}
		public override void MouseMove(PdfMouseAction action) {
			base.MouseMove(action);
			bool containsActionPoint = ContainsActionPoint(action);
			if (shown) {
				if (!containsActionPoint) {
					HideEditor();
					shown = false;
					StopTimer();
				}
			}
			else if (containsActionPoint && !String.IsNullOrEmpty(hintText)) {
				StopTimer();
				start = new Timer(OnTimerTick, null, 1000, Timeout.Infinite);
			}
		}
		public override void MouseUp(PdfMouseAction action) {
			base.MouseUp(action);
			if (ContainsActionPoint(action)) {
				DocumentStateController.ExecuteInteractiveOperation(interactiveOperation);
			}
		}
		public override void ExecuteEnterAction() {
			base.ExecuteEnterAction();
			DocumentStateController.ExecuteInteractiveOperation(interactiveOperation);
		}
		public void PostEditor(object value) {
		}
		public PdfValidationError ValidateEditor(object value) {
			return null;
		}
		public void HideEditor() {
			DocumentStateController.ViewerController.ValueEditingController.HideTooltip();
		}
	}
}

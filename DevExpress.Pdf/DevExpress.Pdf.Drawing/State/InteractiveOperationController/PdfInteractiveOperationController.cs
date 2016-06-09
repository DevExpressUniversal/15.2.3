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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfInteractiveOperationController : IPdfInteractiveOperationController {
		readonly PdfDocumentStateController documentStateController;
		readonly IPdfViewerNavigationController navigationController;
		readonly IPdfViewerActionController viewerActionController;
		readonly IList<PdfPage> pages;
		bool isContinueExecuting = true;
		public PdfInteractiveOperationController(PdfDocumentStateController controller) {
			documentStateController = controller;
			pages = documentStateController.DocumentState.Document.Pages;
			if (controller != null && controller.ViewerController != null) {
				navigationController = controller.ViewerController.NavigationController;
				viewerActionController = controller.ViewerController.ActionController;
			}
		}
		public void ExecuteInteractiveOperation(PdfInteractiveOperation interactiveOperation) {
			if (navigationController != null && interactiveOperation != null) {
				PdfDestination destination = interactiveOperation.Destination;
				if (destination != null)
					((IPdfInteractiveOperationController)this).ShowDocumentPosition(destination.CreateTarget(pages));
				isContinueExecuting = true;
				ExecuteAllActions(interactiveOperation.Action, new List<PdfAction>());
			}
		}
		void ExecuteAllActions(PdfAction action, List<PdfAction> executedAction) {
			if (action == null || executedAction.Contains(action) || !isContinueExecuting)
				return;
			ExecuteAction(action);
			executedAction.Add(action);
			if (action.Next != null)
				foreach (PdfAction nextAction in action.Next)
					ExecuteAllActions(nextAction, executedAction);
		}
		void ExecuteAction(PdfAction action) {
			action.Execute(this, pages);
		}
		void SetTabNavigationPage(int pageIndex) {
			PdfTabNavigationController tabNavigationController = documentStateController.TabNavigationController;
			if (tabNavigationController.LastPageIndex != pageIndex) {
				tabNavigationController.LastPageIndex = pageIndex;
				documentStateController.FocusedAnnotation = null;
			}
		}
		void ResetField(PdfWidgetAnnotationStateController controller) {
			controller.Reset();
		}
		void IPdfInteractiveOperationController.GoToFirstPage() {
			navigationController.GoToFirstPage();
			SetTabNavigationPage(navigationController.CurrentPageNumber - 1);
		}
		void IPdfInteractiveOperationController.GoToLastPage() {
			navigationController.GoToLastPage();
			SetTabNavigationPage(navigationController.CurrentPageNumber - 1);
		}
		void IPdfInteractiveOperationController.GoToNextPage() {
			navigationController.GoToNextPage();
			SetTabNavigationPage(navigationController.CurrentPageNumber - 1);
		}
		void IPdfInteractiveOperationController.GoToPreviousPage() {
			navigationController.GoToPreviousPage();
			SetTabNavigationPage(navigationController.CurrentPageNumber - 1);
		}
		void IPdfInteractiveOperationController.OpenReferencedDocument(string documentPath, PdfTarget target, bool openInNewWindow) {
			PdfActionRequestResult openResult;
			try {
				if (target == null)
					target = new PdfTarget( PdfTargetMode.XYZ, 0);
				openResult = viewerActionController.OpenDocument(documentPath, openInNewWindow, target);
			}
			catch {
				openResult = PdfActionRequestResult.Cancel;
			}
			if (openResult == PdfActionRequestResult.Ok) {
				isContinueExecuting = false;
			}
		}
		void IPdfInteractiveOperationController.OpenUri(string uriValue) {
			Uri uri = null;
			try {
				uri = new UriBuilder(uriValue).Uri;
			}
			catch { }
			if (uri != null && (viewerActionController.UriAccessRequest(uri) == PdfActionRequestResult.Ok))
				PdfProcessorLauncher.Launch(uriValue);
		}
		void IPdfInteractiveOperationController.Print() {
			viewerActionController.RunPrintDialog();
		}
		void IPdfInteractiveOperationController.ResetFormExcludingFields(IEnumerable<PdfInteractiveFormField> fields) {
			PerfomeResetFields((controller, interactiveFormField) => {
				if (!ContainsField(fields, interactiveFormField))
					ResetField(controller);
			});
		}
		void IPdfInteractiveOperationController.ResetFields(IEnumerable<PdfInteractiveFormField> fields) {
			PerfomeResetFields((controller, interactiveFormField) => {
				if (ContainsField(fields, interactiveFormField))
					ResetField(controller);
			});
		}
		void IPdfInteractiveOperationController.ResetForm() {
			PerfomeResetFields((controller, interactiveFormField) => { ResetField(controller); });
		}
		void IPdfInteractiveOperationController.ShowDocumentPosition(PdfTarget target) {
			int targetPageIndex = target.PageIndex;
			if (targetPageIndex >= 0 && targetPageIndex < pages.Count) {
				navigationController.ShowDocumentPosition(target);
				SetTabNavigationPage(target.PageIndex);
			}
		}
		void PerfomeResetFields(Action<PdfWidgetAnnotationStateController, PdfInteractiveFormField> action) {
			for (int i = 0; i < pages.Count; i++) {
				foreach (PdfAnnotationStateController annotationController in documentStateController.GetAnnotationStateControllers(i)) {
					PdfWidgetAnnotationStateController controller = annotationController as PdfWidgetAnnotationStateController;
					PdfWidgetAnnotation widgetAnnotation = annotationController.State.Annotation as PdfWidgetAnnotation;
					PdfInteractiveFormField interactiveFormField = widgetAnnotation == null ? null : widgetAnnotation.InteractiveFormField;
					if (interactiveFormField != null && interactiveFormField.Flags.HasFlag(PdfInteractiveFormFieldFlags.PushButton))
						continue;
					if (controller != null) {
						action(controller, interactiveFormField);
					}
				}
			}
		}
		bool ContainsField(IEnumerable<PdfInteractiveFormField> fields, PdfInteractiveFormField interactiveFormField) {
			foreach (PdfInteractiveFormField formField in fields)
				if (formField == interactiveFormField)
					return true;
			return false;
		}
	}
}

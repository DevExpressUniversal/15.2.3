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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfTabNavigationStrategy {
		readonly PdfDocumentStateController documentStateController;
		protected abstract int Step { get; }
		protected abstract int StartPageIndex { get; }
		protected int PageCount { get { return documentStateController.DocumentState.Document.Pages.Count; } }
		protected PdfAnnotationStateController FirstAnnotationStateController { get { return GetFirstAnnotationStateController(-1); } }
		protected PdfAnnotationStateController GetNextAnnotationStateController(PdfAnnotationStateController current) {
			PdfAnnotationState annotationState = current == null ? null : current.State;
			if (annotationState == null)
				return FirstAnnotationStateController;
			int pageIndex = annotationState.PageIndex;
			IList<PdfAnnotationStateController> controllers = documentStateController.GetAnnotationStateControllers(pageIndex);
			if (controllers != null) {
				int idx = controllers.IndexOf(current);
				if (idx >= 0) {
					idx += Step;
					return idx >= 0 && idx < controllers.Count ? controllers[idx] : GetPageFirstAnnotationStateController(GetNextPageWithAnnotations(pageIndex));
				}
			}
			return GetPageFirstAnnotationStateController(GetNextPageWithAnnotations(pageIndex));
		}
		protected PdfTabNavigationStrategy(PdfDocumentStateController documentStateController) {
			this.documentStateController = documentStateController;
		}
		public void TabNext(int lastPageIndex) {
			PdfAnnotationStateController currentFocus = documentStateController.FocusedAnnotation != null ? GetNextAnnotationStateController(documentStateController.FocusedAnnotation) : GetFirstAnnotationStateController(lastPageIndex);
			if (currentFocus != null) {
				PdfAnnotationStateController start = currentFocus;
				while (!currentFocus.TabStop) {
					currentFocus = GetNextAnnotationStateController(currentFocus);
					if (currentFocus == start)
						break;
				}
				if (currentFocus != null && currentFocus.TabStop) {
					PdfAnnotationState state = currentFocus.State;
					int pageIndex = state.PageIndex;
					documentStateController.TabNavigationController.LastPageIndex = pageIndex;
					documentStateController.ViewerController.NavigationController.ShowRectangleOnPage(PdfRectangleAlignMode.Center, pageIndex, state.Rectangle);
					documentStateController.FocusedAnnotation = currentFocus;
				}
			}
		}
		protected abstract PdfAnnotationStateController GetPageFirstAnnotationStateController(IList<PdfAnnotationStateController> page);
		protected IList<PdfAnnotationStateController> GetNextPageWithAnnotations(int currentPageIndex) {
			int startPageIndex = currentPageIndex + Step;
			int pageIndex = startPageIndex;
			int pageCount = PageCount;
			if (pageIndex >= pageCount || pageIndex < 0)
				pageIndex = StartPageIndex;
			int step = Step;
			if (pageCount == 0)
				return null;
			do {
				IList<PdfAnnotationStateController> page = documentStateController.GetAnnotationStateControllers(pageIndex);
				if (page != null && page.Count > 0)
					return page;
				pageIndex += step;
				if (pageIndex < 0 || pageIndex >= pageCount)
					pageIndex = StartPageIndex;
			} while (pageIndex != startPageIndex);
			return null;
		}
		protected PdfAnnotationStateController GetFirstAnnotationStateController(int lastPageIndex) {
			int idx = (lastPageIndex == -1 ? StartPageIndex : lastPageIndex) - Step;
			IList<PdfAnnotationStateController> firstPage = GetNextPageWithAnnotations(idx);
			if (firstPage != null)
				return GetPageFirstAnnotationStateController(firstPage);
			return null;
		}
	}
}

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
namespace DevExpress.Pdf.Drawing {
	public class PdfMarkupAnnotationStateController : PdfAnnotationStateController, IPdfViewerValueEditingCallBack {
		bool shown;
		public PdfMarkupAnnotationStateController(PdfDocumentStateController documentStateController, PdfAnnotationState state) : base(documentStateController, state) {
		}
		public void HideEditor() {
			DocumentStateController.ViewerController.ValueEditingController.HideTooltip();
		}
		protected void ShowEditor() {
			PdfMarkupAnnotation markupAnnotation = (PdfMarkupAnnotation)State.Annotation;
			DocumentStateController.ViewerController.ValueEditingController.ShowTooltip(
				new PdfStickyNoteEditSettings(State, markupAnnotation.Contents, markupAnnotation.Title));
		}
		public override void MouseMove(PdfMouseAction action) {
			base.MouseMove(action);
			bool containsActionPoint = ContainsActionPoint(action);
			if (shown) {
				if (!containsActionPoint) {
					HideEditor();
					shown = false;
				}
			}
			else if (containsActionPoint && !String.IsNullOrEmpty(State.Annotation.Contents)) {
				shown = true;
				ShowEditor();
			}
		}
		void IPdfViewerValueEditingCallBack.PostEditor(object value) {
		}
		PdfValidationError IPdfViewerValueEditingCallBack.ValidateEditor(object value) {
			return null;
		}
	}
}

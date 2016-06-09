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
	public class PdfAnnotationStateController {
		public static PdfAnnotationStateController Create(PdfDocumentStateController documentStateController, PdfAnnotationState annotationState, IDictionary<PdfWidgetAnnotation, PdfWidgetAppearanceBuilder> builders) {
			PdfAnnotation annotation = annotationState.Annotation;
			if (annotation is PdfMarkupAnnotation)
				return new PdfMarkupAnnotationStateController(documentStateController, annotationState);
			PdfLinkAnnotation linkAnnotation = annotation as PdfLinkAnnotation;
			if (linkAnnotation != null)
				return new PdfLinkAnnotationStateController(documentStateController, annotationState);
			PdfWidgetAnnotation widgetAnnotation = annotation as PdfWidgetAnnotation;
			if (widgetAnnotation != null && !(widgetAnnotation.InteractiveFormField is PdfSignatureFormField)) {
				PdfButtonFormField buttonField = widgetAnnotation.InteractiveFormField as PdfButtonFormField;
				if (buttonField != null)
					if (buttonField.Flags.HasFlag(PdfInteractiveFormFieldFlags.Radio))
						return new PdfRadioButtonAnnotationStateController(documentStateController, annotationState);
					else if (buttonField.Flags.HasFlag(PdfInteractiveFormFieldFlags.PushButton))
						return new PdfButtonAnnotationStateController(documentStateController, annotationState);
					else
						return new PdfCheckBoxAnnotationStateController(documentStateController, annotationState);
				PdfWidgetAppearanceBuilder builder = null;
				builders.TryGetValue(widgetAnnotation, out builder);
				return new PdfTextBasedWidgetAnnotationStateController(documentStateController, annotationState, builder);
			}
			return new PdfAnnotationStateController(documentStateController, annotationState);
		}
		readonly PdfDocumentStateController documentStateController;
		readonly PdfAnnotationState state;
		protected virtual bool InteractionSupported { get { return false; } }
		protected PdfDocumentStateController DocumentStateController { get { return documentStateController; } }
		protected virtual bool ReadOnly {
			get {
				PdfAnnotation annotation = state == null ? null : state.Annotation;
				return (documentStateController != null && documentStateController.ViewerController != null && documentStateController.ViewerController.ReadOnly) ||
					(annotation != null && annotation.Flags.HasFlag(PdfAnnotationFlags.ReadOnly));
			}
		}
		protected bool AnnotationVisible {
			get {
				PdfAnnotation annotation = state == null ? null : state.Annotation;
				return !annotation.Flags.HasFlag(PdfAnnotationFlags.Hidden) && !annotation.Flags.HasFlag(PdfAnnotationFlags.NoView);
			}
		}
		public PdfAnnotationState State { get { return state; } }
		public bool TabStop {
			get {
				PdfAnnotation annotation = state == null ? null : state.Annotation;
				return annotation != null && !ReadOnly && AnnotationVisible && InteractionSupported;
			}
		}
		protected PdfAnnotationStateController(PdfDocumentStateController documentStateController, PdfAnnotationState state) {
			this.documentStateController = documentStateController;
			this.state = state;
		}
		public bool ContainsActionPoint(PdfMouseAction action) {
			return state.Rectangle.Contains(action.DocumentPosition.Point);
		}
		public virtual void MouseMove(PdfMouseAction action) {
			if (action.Button != PdfMouseButton.Left) {
				bool containsActionPoint = ContainsActionPoint(action);
				if (state.State == PdfAnnotationAppearanceState.Normal) {
					if (containsActionPoint)
						state.State = PdfAnnotationAppearanceState.Rollover;
				}
				else if (!containsActionPoint)
					state.State = PdfAnnotationAppearanceState.Normal;
			}
		}
		public virtual void MouseDown(PdfMouseAction action) {
			if (ContainsActionPoint(action))
				state.State = PdfAnnotationAppearanceState.Down;
		}
		public virtual void MouseUp(PdfMouseAction action) {
			if (state.State == PdfAnnotationAppearanceState.Down && ContainsActionPoint(action))
				state.State = PdfAnnotationAppearanceState.Rollover;
			else
				state.State = PdfAnnotationAppearanceState.Normal;
		}
		public virtual void SetFocus(bool focus) {
			state.Focused = focus;
			documentStateController.ViewerController.NavigationController.Invalidate(PdfDocumentStateChangedFlags.Annotation, state.PageIndex);
		}
		public virtual void ExecuteEnterAction() {
		}
	}
}

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

namespace DevExpress.Pdf.Drawing {
	public abstract class PdfWidgetAnnotationStateController : PdfAnnotationStateController {
		readonly PdfWidgetAnnotation annotation;
		readonly PdfInteractiveOperation interactiveOperation;
		readonly PdfFormData formData;
		protected PdfFormData FormData { get { return formData; } }
		protected PdfWidgetAnnotation Annotation { get { return annotation; } }
		protected override bool InteractionSupported { get { return true; } }
		protected override bool ReadOnly {
			get {
				PdfInteractiveFormField field = annotation == null ? null : annotation.InteractiveFormField;
				return base.ReadOnly || field == null || field.Flags.HasFlag(PdfInteractiveFormFieldFlags.ReadOnly);
			}
		}
		protected PdfWidgetAnnotationStateController(PdfDocumentStateController documentStateController, PdfAnnotationState state)
			: base(documentStateController, state) {
			this.annotation = (PdfWidgetAnnotation)state.Annotation;
			this.interactiveOperation = new PdfInteractiveOperation(annotation.Action);
			PdfInteractiveFormField field = annotation.InteractiveFormField;
			if (field != null && !field.Flags.HasFlag(PdfInteractiveFormFieldFlags.PushButton))
				this.formData = documentStateController.DocumentState.FormData[field.FullName];
		}
		protected virtual void SetValue(object value) {
			if (formData != null)
				formData.Value = value;
		}
		public void Reset() {
			if (formData != null)
				formData.Reset();
		}
		public void SetEditorValue(object value) {
			if (!ReadOnly)
				SetValue(value);
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
	}
}

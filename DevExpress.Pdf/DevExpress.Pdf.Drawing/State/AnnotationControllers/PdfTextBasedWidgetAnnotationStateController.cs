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
namespace DevExpress.Pdf.Drawing {
	public class PdfTextBasedWidgetAnnotationStateController : PdfWidgetAnnotationStateController, IPdfViewerValueEditingCallBack {
		readonly PdfWidgetAppearanceBuilder appearanceBuilder;
		protected virtual bool EditorCanBeOpened { 
			get {
				PdfInteractiveFormField field = Annotation.InteractiveFormField;
				return AnnotationVisible && !((field != null && field is PdfChoiceFormField && !field.Flags.HasFlag(PdfInteractiveFormFieldFlags.Combo) && ReadOnly)); } 
		}
		public PdfTextBasedWidgetAnnotationStateController(PdfDocumentStateController documentStateController, PdfAnnotationState state, PdfWidgetAppearanceBuilder appearanceBuilder)
			: base(documentStateController, state) {
			this.appearanceBuilder = appearanceBuilder;
		}
		public override void SetFocus(bool focus) {
			base.SetFocus(focus);
			if (focus)
				OnGotFocus();
			else
				OnLostFocus();
		}
		protected virtual void ShowEditor(int pageIndex) {
			DocumentStateController.DataSelector.HideCaret();
			DocumentStateController.DataSelector.ClearSelection();
			if (appearanceBuilder != null) {
				State.EditorVisible = true;
				DocumentStateController.ViewerController.ValueEditingController.ShowEditor(appearanceBuilder.GetEditorAppearanceSettings(State, ReadOnly), this);
			}
		}
		protected void OnGotFocus() {
			if (EditorCanBeOpened)
				ShowEditor(State.PageIndex);
		}
		protected void OnLostFocus() {
			((IPdfViewerValueEditingCallBack)this).HideEditor();
		}
		protected override void SetValue(object value) {
			PdfListBoxEditValue listBoxValue = value as PdfListBoxEditValue;
			base.SetValue(listBoxValue == null ? value : listBoxValue.SelectedValues);
			if (listBoxValue != null) {
				PdfChoiceFormField choice = Annotation.InteractiveFormField as PdfChoiceFormField;
				if (choice != null)
					choice.SetTopIndex(listBoxValue.TopIndex, appearanceBuilder != null ? new Action[] { () => appearanceBuilder.RebuildAppearance() } : null);
			}
			DocumentStateController.TextSearch.ClearCache(State.PageIndex);
		}
		void IPdfViewerValueEditingCallBack.PostEditor(object value) {
			SetEditorValue(value);
		}
		PdfValidationError IPdfViewerValueEditingCallBack.ValidateEditor(object value) {
			return null;
		}
		void IPdfViewerValueEditingCallBack.HideEditor() {
			State.EditorVisible = false;
			DocumentStateController.ViewerController.ValueEditingController.CloseEditor();
			DocumentStateController.ViewerController.NavigationController.Invalidate(PdfDocumentStateChangedFlags.Annotation, State.PageIndex);
		}
	}
}

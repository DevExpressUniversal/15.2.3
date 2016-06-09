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
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class PdfAnnotationState {
		readonly PdfPageState pageState;
		readonly PdfAnnotation annotation;
		PdfRectangle rectangle;
		PdfAnnotationAppearanceState state = PdfAnnotationAppearanceState.Normal;
		bool focused;
		bool editorVisible;
		public bool Focused {
			get { return focused; }
			set { focused = value; }
		}
		public bool EditorVisible {
			get { return editorVisible; }
			set { editorVisible = value; }
		}
		public int PageIndex { get { return pageState == null ? -1 : pageState.PageIndex; } }
		public PdfAnnotation Annotation { get { return annotation; } }
		public PdfRectangle Rectangle {
			get {
				if (rectangle == null) {
					PdfRectangle cropBox = pageState.Page.CropBox;
					double xOffset = cropBox.Left;
					double yOffset = cropBox.Bottom;
					PdfRectangle annotationRect = annotation.Rect;
					rectangle = new PdfRectangle(annotationRect.Left - xOffset, annotationRect.Bottom - yOffset, annotationRect.Right - xOffset, annotationRect.Top - yOffset);
				}
				return rectangle;
			}
		}
		public PdfAnnotationAppearanceState State {
			get { return state; }
			set {
				if (annotation.Appearance != null && annotation.Appearance.Rollover == null && value == PdfAnnotationAppearanceState.Rollover)
					value = PdfAnnotationAppearanceState.Normal;
				if (state != value) {
					state = value;
					RaiseStateChanged();
				}
			}
		}
		public PdfAnnotationState(PdfPageState pageState, PdfAnnotation annotation) {
			this.pageState = pageState;
			this.annotation = annotation;
		}
		public void Draw(PdfViewerCommandInterpreter interpreter) {
			PdfWidgetAnnotation widget = annotation as PdfWidgetAnnotation;
			if (!editorVisible || widget == null || (!(widget.InteractiveFormField is PdfTextFormField) && !(widget.InteractiveFormField is PdfChoiceFormField))) {
				PdfDocumentState documentState = pageState == null ? null : pageState.DocumentState;
				if (documentState != null && documentState.HighlightFormFields) {
					Color highlightColor = documentState.HighlightedFormFieldColor;
					interpreter.DrawAnnotation(Annotation, state, PdfRGBColor.Create(highlightColor.R / 255.0, highlightColor.G / 255.0, highlightColor.B / 255.0));
				}   
				else
					interpreter.DrawAnnotation(Annotation, state, null);
				if ((widget == null || widget.InteractiveFormField is PdfButtonFormField) && Focused) {
					PdfRectangle annotationRect = annotation.Rect;
					interpreter.SaveGraphicsState();
					interpreter.SetLineWidth(0.1);
					interpreter.SetLineStyle(PdfLineStyle.CreateDashed(new double[] { 0.1 }, 0));
					interpreter.SetColorForStrokingOperations(new PdfColor(0, 0, 0));
					interpreter.AppendRectangle(annotationRect.Left - 1, annotationRect.Bottom - 1, annotationRect.Width + 2, annotationRect.Height + 2);
					interpreter.TransformAndStrokePaths();
					interpreter.RestoreGraphicsState();
				}
			}
		}
		public void RaiseStateChanged() {
			pageState.RaiseDocumentStateChanged();
		}
	}
}

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
namespace DevExpress.Pdf.Drawing {
	public class PdfChoiceWidgetAppearanceBuilder : PdfWidgetAppearanceBuilder {
		const double itemMargin = 1;
		public static PdfColor SelectionForeColor = new PdfColor(0, 0, 0);
		public static PdfColor SelectionBackColor = new PdfColor(0.6, 0.75686, 0.8549);
		readonly PdfChoiceFormField choiceField;
		bool IsCombo { get { return choiceField.Flags.HasFlag(PdfInteractiveFormFieldFlags.Combo); } }
		double ItemHeight { get { return itemMargin + FontSize; } }
		public PdfChoiceWidgetAppearanceBuilder(PdfChoiceFormField field, PdfDocumentState documentState) : base(field, documentState) {
			choiceField = field;
			if (IsCombo) {
				IList<string> values = field.SelectedValues;
				if (values != null && values.Count > 0)
					CalculateActualFontSize(Annotation.ContentRectangle, values[0]);
			}
		}
		public override PdfEditorSettings GetEditorAppearanceSettings(PdfAnnotationState annotationState, bool readOnly) {
			return IsCombo ?
				(PdfEditorSettings)new PdfComboBoxSettings(annotationState, choiceField, FontData, ActualFontSize, readOnly) :
				(PdfEditorSettings)new PdfListBoxSettings(annotationState, choiceField, FontData, ActualFontSize, readOnly);
		}
		protected override void FillMarkedContent(IList<PdfCommand> markedContentCommands) {
			if (IsCombo) {
				IList<string> selectedValues = choiceField.SelectedValues;
				if (selectedValues != null && selectedValues.Count > 0) {
					PdfRectangle contentRectangle = Annotation.ContentRectangle;
					IList<PdfOptionsFormFieldOption> options = choiceField.Options;
					if (options != null)
						foreach (PdfOptionsFormFieldOption opt in options)
							if (opt.Text.Equals(selectedValues[0])) {
								DrawTextBox(markedContentCommands, contentRectangle, opt.ExportText);
								return;
							}
					DrawTextBox(markedContentCommands, contentRectangle, selectedValues[0]);
				}
			}
			else {
				IList<string> selectedValues = choiceField.SelectedValues;
				PdfRectangle contentRectangle = Annotation.ContentRectangle;
				double itemHeight = ItemHeight;
				PdfChoiceFormField field = choiceField;
				IList<PdfOptionsFormFieldOption> options = field.Options;
				int topIndex = field.TopIndex;
				if (options != null)
					for (int i = 0; i < options.Count; i++)
						if (i >= topIndex) {
							double bottom = contentRectangle.Top - itemHeight * (1 + i - topIndex);
							bool selected = selectedValues != null && selectedValues.Contains(options[i].Text);
							PdfRectangle itemBox = new PdfRectangle(contentRectangle.Left, bottom, contentRectangle.Right, bottom + itemHeight);
							if (selected)
								FillRectangle(markedContentCommands, itemBox, SelectionBackColor);
							DrawTextBox(markedContentCommands, itemBox, options[i].ExportText, selected ? SelectionForeColor : null);
						}
			}
		}
	}
}

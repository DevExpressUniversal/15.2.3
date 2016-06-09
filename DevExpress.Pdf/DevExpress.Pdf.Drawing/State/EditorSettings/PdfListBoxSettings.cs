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
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class PdfListBoxSettings : PdfChoiceEditorSettings {
		readonly bool multiselect;
		readonly IList<string> selectedValues;
		IList<int> selectedIndices;
		public override PdfEditorType EditorType { get { return PdfEditorType.ListBox; } }
		public Color SelectionForeColor { get { return PdfEditorSettings.ToGDIPlusColor(PdfChoiceWidgetAppearanceBuilder.SelectionForeColor); } }
		public Color SelectionBackColor { get { return PdfEditorSettings.ToGDIPlusColor(PdfChoiceWidgetAppearanceBuilder.SelectionBackColor); } }
		public override object EditValue { get { return selectedValues; } }
		public IList<int> SelectedIndices {
			get {
				if (selectedIndices != null)
					return selectedIndices;
				selectedIndices = new List<int>();
				IList<PdfOptionsFormFieldOption> values = Values;
				if (values != null) {
					int count = values.Count;
					foreach (string s in selectedValues)
						for (int i = 0; i < count; i++)
							if (values[i].Text.Equals(s))
								selectedIndices.Add(i);
				}
				return selectedIndices;
			}
		}
		public bool Multiselect { get { return multiselect; } }
		public PdfListBoxSettings(PdfAnnotationState state, PdfChoiceFormField field, PdfEditableFontData fontData, double fontSize, bool readOnly)
			: base(state, field, fontData, fontSize, readOnly) {
			multiselect = field.Flags.HasFlag(PdfInteractiveFormFieldFlags.MultiSelect);
			selectedValues = field.SelectedValues == null ? new List<string>() : field.SelectedValues;
		}
	}
}

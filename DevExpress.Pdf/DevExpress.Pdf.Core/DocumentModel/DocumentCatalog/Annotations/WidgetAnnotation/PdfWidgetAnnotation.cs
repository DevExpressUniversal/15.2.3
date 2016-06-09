#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf {
	public class PdfWidgetAnnotation : PdfAnnotation {
		internal const string Type = "Widget";
		const string parentDictionaryKey = "Parent";
		const string appearanceCharacteristicsDictionaryKey = "MK";
		const string actionsDictionaryKey = "AA";
		readonly PdfAnnotationHighlightingMode highlightingMode;
		readonly PdfWidgetAppearanceCharacteristics appearanceCharacteristics;
		readonly PdfAction action;
		readonly PdfAdditionalActions actions;
		readonly PdfAnnotationBorderStyle borderStyle;
		PdfInteractiveFormField interactiveFormField;
		public PdfAnnotationHighlightingMode HighlightingMode { get { return highlightingMode; } }
		public PdfWidgetAppearanceCharacteristics AppearanceCharacteristics { get { return appearanceCharacteristics; } }
		public PdfAction Action { get { return action; } }
		public PdfAnnotationActions Actions { get { return actions == null ? null : actions.AnnotationActions; } }
		public PdfAnnotationBorderStyle BorderStyle { get { return borderStyle; } }
		public PdfInteractiveFormField InteractiveFormField {
			get { return ResolveInteractiveFormField(); }
			internal set { interactiveFormField = value; }
		}
		internal PdfColor BackgroundColor { get { return appearanceCharacteristics == null ? null : appearanceCharacteristics.BackgroundColor ; } }
		internal string OnAppearanceName {
			get {
				PdfAnnotationAppearances appearance = Appearance;
				if (appearance != null) {
					IList<string> names = appearance.Normal.GetNames(null);
					if (names.Count == 1)
						return names[0];
					if (names.Count > 1)
						return names[0] == PdfButtonFormField.OffStateName ? names[1] : names[0];
				}
				return PdfButtonFormField.OffStateName;
			}
		}
		internal PdfRectangle ContentRectangle {
			get {
				double borderWidth;
				if (borderStyle == null) {
					PdfAnnotationBorder border = Border;
					borderWidth = border == null ? 0 : border.LineWidth;
				}
				else {
					borderWidth = borderStyle.Width;
					string styleName = borderStyle.StyleName;
					if (styleName == "B" || styleName == "I")
						borderWidth *= 2;
				}
				PdfRectangle rect = DefaultForm.BBox;
				double right = rect.Width - borderWidth;
				double top = rect.Height - borderWidth;
				return new PdfRectangle(borderWidth, borderWidth, right > borderWidth ? right : borderWidth + 1, top > borderWidth ? top : borderWidth + 1);
			}
		}
		protected override string AnnotationType { get { return Type; } }
		internal PdfWidgetAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			highlightingMode = dictionary.GetAnnotationHighlightingMode();
			PdfReaderDictionary mk = dictionary.GetDictionary(appearanceCharacteristicsDictionaryKey);
			if (mk != null)
				appearanceCharacteristics = new PdfWidgetAppearanceCharacteristics(page, mk);
			action = dictionary.GetAction(PdfDictionary.DictionaryActionKey);
			actions = dictionary.GetAdditionalActions(this);
			borderStyle = PdfAnnotationBorderStyle.Parse(dictionary);
		}
		internal PdfInteractiveFormField ResolveInteractiveFormField() {
			if (interactiveFormField == null)
				interactiveFormField = DocumentCatalog.ResolveUnparsedInteractiveFormFields(new PdfObjectReference(ObjectNumber));
			return interactiveFormField;
		}
		protected internal override PdfForm GetHighlightedAppearanceForm(PdfAnnotationAppearanceState state, PdfRGBColor highlightColor) {
			PdfForm form = base.GetHighlightedAppearanceForm(state, highlightColor);
			if (interactiveFormField != null && interactiveFormField.ShouldHighlight) {
				PdfRectangle rect = Rect;
				double width = rect.Width;
				double height = rect.Height;
				form = form == null ? new PdfForm(DocumentCatalog, new PdfRectangle(0, 0, width, height)) : new PdfForm(form);
				IList<PdfCommand> commands = form.Commands;
				int count = commands.Count;
				for (int i = 0; i < count; i++) {
					if (commands[i] is PdfSetRGBColorSpaceForNonStrokingOperationsCommand) {
						commands.Insert(i + 1, new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(highlightColor.R, highlightColor.G, highlightColor.B));
						return form;
					}
				}
				commands.Insert(0, new PdfRestoreGraphicsStateCommand());
				commands.Insert(0, new PdfFillPathUsingNonzeroWindingNumberRuleCommand());
				commands.Insert(0, new PdfAppendRectangleCommand(0, 0, width, height));
				commands.Insert(0, new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(highlightColor.R, highlightColor.G, highlightColor.B));
				commands.Insert(0, new PdfSaveGraphicsStateCommand());
			}
			return form;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			ResolveInteractiveFormField();
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.AddEnumName(PdfDictionary.DictionaryAnnotationHighlightingModeKey, highlightingMode);
			if (appearanceCharacteristics != null)
				dictionary.Add(appearanceCharacteristicsDictionaryKey, appearanceCharacteristics.ToWritableObject(collection));
			dictionary.Add(PdfDictionary.DictionaryActionKey, action);
			if (!dictionary.ContainsKey(PdfAdditionalActions.DictionaryAdditionalActionsKey))
				dictionary.Add(PdfAdditionalActions.DictionaryAdditionalActionsKey, actions);
			dictionary.Add(PdfAnnotationBorderStyle.DictionaryKey, borderStyle);
			if (interactiveFormField != null)
				interactiveFormField.FillDictionary(dictionary);
			return dictionary;
		}
	}
}

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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[Flags]
	public enum PdfAnnotationFlags {
		None = 0x000, Invisible = 0x001, Hidden = 0x002, Print = 0x004, NoZoom = 0x008, NoRotate = 0x010, NoView = 0x020, ReadOnly = 0x040, Locked = 0x080, ToggleNoView = 0x100, LockedContents = 0x200
	}
	public abstract class PdfAnnotation : PdfObject {
		internal const string DictionaryType = "Annot";
		internal const string PageDictionaryKey = "P";
		const string rectDictionaryKey = "Rect";
		const string contentsDictionaryKey = "Contents";
		const string nameDictionaryKey = "NM";
		const string modifiedDictionaryKey = "M";
		const string flagsDictionaryKey = "F";
		const string appearanceNameDictionaryKey = "AS";
		const string appearanceDictionaryKey = "AP";
		const string borderDictionaryKey = "Border";
		const string colorDictionaryKey = "C";
		const string structParentDictionaryKey = "StructParent";
		internal static PdfAnnotation Parse(PdfPage page, PdfReaderDictionary dictionary) {
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string subType = dictionary.GetName(PdfDictionary.DictionarySubtypeKey);
			if ((type != null && type != DictionaryType) || subType == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (subType) {
				case PdfTextAnnotation.Type:
					return new PdfTextAnnotation(page, dictionary);
				case PdfLinkAnnotation.Type:
					return new PdfLinkAnnotation(page, dictionary);
				case PdfFreeTextAnnotation.Type:
					return new PdfFreeTextAnnotation(page, dictionary);
				case PdfLineAnnotation.Type:
					return new PdfLineAnnotation(page, dictionary);
				case PdfSquareAnnotation.Type:
					return new PdfSquareAnnotation(page, dictionary);
				case PdfCircleAnnotation.Type:
					return new PdfCircleAnnotation(page, dictionary);
				case PdfPolygonAnnotation.Type:
					return new PdfPolygonAnnotation(page, dictionary);
				case PdfPolyLineAnnotation.Type:
					return new PdfPolyLineAnnotation(page, dictionary);
				case PdfTextMarkupAnnotation.HighlightType:
					return new PdfTextMarkupAnnotation(page, PdfTextMarkupAnnotationType.Highlight, dictionary);
				case PdfTextMarkupAnnotation.UnderlineType:
					return new PdfTextMarkupAnnotation(page, PdfTextMarkupAnnotationType.Underline, dictionary);
				case PdfTextMarkupAnnotation.SquigglyType:
					return new PdfTextMarkupAnnotation(page, PdfTextMarkupAnnotationType.Squiggly, dictionary);
				case PdfTextMarkupAnnotation.StrikeOutType:
					return new PdfTextMarkupAnnotation(page, PdfTextMarkupAnnotationType.StrikeOut, dictionary);
				case PdfRubberStampAnnotation.Type:
					return new PdfRubberStampAnnotation(page, dictionary);
				case PdfCaretAnnotation.Type:
					return new PdfCaretAnnotation(page, dictionary);
				case PdfInkAnnotation.Type:
					return new PdfInkAnnotation(page, dictionary);
				case PdfPopupAnnotation.Type:
					return new PdfPopupAnnotation(page, dictionary);
				case PdfFileAttachmentAnnotation.Type:
					return new PdfFileAttachmentAnnotation(page, dictionary);
				case PdfSoundAnnotation.Type:
					return new PdfSoundAnnotation(page, dictionary);
				case PdfMovieAnnotation.Type:
					return new PdfMovieAnnotation(page, dictionary);
				case PdfWidgetAnnotation.Type:
					return new PdfWidgetAnnotation(page, dictionary);
				case PdfScreenAnnotation.Type:
					return new PdfScreenAnnotation(page, dictionary);
				case PdfPrinterMarkAnnotation.Type:
					return new PdfPrinterMarkAnnotation(page, dictionary);
				case PdfTrapNetAnnotation.Type:
					return new PdfTrapNetAnnotation(page, dictionary);
				case PdfWatermarkAnnotation.Type:
					return new PdfWatermarkAnnotation(page, dictionary);
				case Pdf3dAnnotation.Type:
					return new Pdf3dAnnotation(page, dictionary);
				case PdfRedactAnnotation.Type:
					return new PdfRedactAnnotation(page, dictionary);
				case PdfRichMediaAnnotation.Type:
					return new PdfRichMediaAnnotation(page, dictionary);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		internal static PdfColor ParseColor(PdfReaderDictionary dictionary, string key) {
			IList<object> colorArray = dictionary.GetArray(key);
			if (colorArray == null)
				return null;
			int length = colorArray.Count;
			if (length == 0)
				return null;
			if (length != 1 && length != 3 && length != 4)
				PdfDocumentReader.ThrowIncorrectDataException();
			double[] components = new double[length];
			for (int i = 0; i < length; i++) {
				double value = PdfDocumentReader.ConvertToDouble(colorArray[i]);
				if (value < 0)
					value = 0;
				else if (value > 1)
					value = 1;
				components[i] = value;
			}
			return new PdfColor(components);
		}
		readonly PdfDocumentCatalog documentCatalog;
		readonly PdfPage page;
		PdfRectangle rect;
		string contents;
		string name;
		DateTimeOffset? modified;
		PdfAnnotationFlags flags;
		PdfAnnotationBorder border;
		PdfColor color;
		int? structParent;
		PdfOptionalContent optionalContent;
		PdfAnnotationAppearances appearance;
		string appearanceName;
		PdfReaderDictionary dictionary;
		public PdfPage Page { get { return page; } }
		public PdfRectangle Rect {
			get {
				Ensure();
				return rect;
			}
		}
		public string Contents {
			get {
				Ensure();
				return contents;
			}
		}
		public string Name {
			get {
				Ensure();
				return name;
			}
		}
		public DateTimeOffset? Modified {
			get {
				Ensure();
				return modified;
			}
		}
		public PdfAnnotationFlags Flags {
			get {
				Ensure();
				return flags;
			}
		}
		public PdfAnnotationBorder Border {
			get {
				Ensure();
				return border;
			}
		}
		public PdfColor Color {
			get {
				Ensure();
				return color;
			}
		}
		public int? StructParent {
			get {
				Ensure();
				return structParent;
			}
		}
		public PdfOptionalContent OptionalContent {
			get {
				Ensure();
				return optionalContent;
			}
		}
		public PdfAnnotationAppearances Appearance {
			get {
				Ensure();
				return appearance;
			}
		}
		public string AppearanceName {
			get {
				Ensure();
				return appearanceName;
			}
			set {
				Ensure();
				appearanceName = value;
			}
		}
		internal PdfForm DefaultForm {
			get {
				CreateAppearanceIfNotExist();
				return Appearance.Normal.DefaultForm;
			}
		}
		protected PdfDocumentCatalog DocumentCatalog { get { return documentCatalog; } }
		protected abstract string AnnotationType { get; }
		protected PdfAnnotation(PdfPage page, PdfRectangle rect) {
			this.page = page;
			this.documentCatalog = page.DocumentCatalog;
			this.rect = rect;
			this.border = new PdfAnnotationBorder(new object[] { 0, 0, 0 });
		}
		protected PdfAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			this.documentCatalog = dictionary.Objects.DocumentCatalog;
			this.page = page;
			this.dictionary = dictionary;
		}
		protected void Ensure() {
			if (dictionary != null) {
				ResolveDictionary(dictionary);
				dictionary = null;
			}
		}
		internal void CreateAppearanceIfNotExist() {
			Ensure();
			if (appearance == null) {
				PdfRectangle rect = Rect;
				appearance = new PdfAnnotationAppearances(documentCatalog, new PdfRectangle(0, 0, rect.Width, rect.Height));
			}
		}
		internal void UpdateTransformation() {
			CreateAppearanceIfNotExist();
			PdfRectangle rect = Rect;
			double width = rect.Width;
			double height = rect.Height;
			switch (page == null ? 0 : page.Rotate) {
				case 90:
					appearance.UpdateDefaultFormTransformation(new PdfRectangle(0, 0, height, width), new PdfTransformationMatrix(0, 1, -1, 0, width, 0));
					break;
				case 180:
					appearance.UpdateDefaultFormTransformation(new PdfRectangle(0, 0, width, height), new PdfTransformationMatrix(-1, 0, 0, -1, width, height));
					break;
				case 270:
					appearance.UpdateDefaultFormTransformation(new PdfRectangle(0, 0, height, width), new PdfTransformationMatrix(0, -1, 1, 0, 0, height));
					break;
				default:
					appearance.UpdateDefaultFormTransformation(new PdfRectangle(0, 0, width, height), new PdfTransformationMatrix());
					break;
			}
		}
		internal PdfForm GetAppearanceForm(PdfAnnotationAppearanceState state) {
			CreateAppearanceIfNotExist();
			PdfAnnotationAppearance annotationAppearanceState;
			switch (state) {
				case PdfAnnotationAppearanceState.Down:
					annotationAppearanceState = appearance.Down ?? appearance.Normal;
					break;
				case PdfAnnotationAppearanceState.Rollover:
					annotationAppearanceState = appearance.Rollover ?? appearance.Normal;
					break;
				default:
					annotationAppearanceState = appearance.Normal;
					break;
			}
			if (annotationAppearanceState == null)
				return null;
			PdfForm form = null;
			if (!String.IsNullOrEmpty(appearanceName)) {
				IDictionary<string, PdfForm> forms = annotationAppearanceState.Forms;
				if (forms != null && !annotationAppearanceState.Forms.TryGetValue(appearanceName, out form))
					form = null;
			}
			if (form == null && appearanceName != PdfButtonFormField.OffStateName)
				form = annotationAppearanceState.DefaultForm;
			if (form != null) {
				IList<PdfCommand> commands = form.Commands;
				if (commands.Count == 0)
					GenerateAppearanceCommands(commands);
			}
			return form;
		}
		protected virtual void ResolveDictionary(PdfReaderDictionary dictionary) {
			rect = dictionary.GetRectangle(rectDictionaryKey);
			contents = dictionary.GetString(contentsDictionaryKey);
			if (rect == null || (page == null && dictionary.GetObjectReference(PageDictionaryKey) != null))
				PdfDocumentReader.ThrowIncorrectDataException();
			name = dictionary.GetString(nameDictionaryKey);
			modified = dictionary.GetDate(modifiedDictionaryKey);
			flags = (PdfAnnotationFlags)(dictionary.GetInteger(flagsDictionaryKey) ?? 0);
			appearanceName = dictionary.GetName(appearanceNameDictionaryKey);
			object ap;
			if (dictionary.TryGetValue(appearanceDictionaryKey, out ap)) {
				appearance = dictionary.Objects.GetAnnotationAppearances(ap, rect);
				IList<string> names = appearance.Names;
				if (!flags.HasFlag(PdfAnnotationFlags.Invisible) && String.IsNullOrEmpty(appearanceName) && names.Count > 1 && !names.Contains(PdfAnnotationAppearances.NormalAppearanceDictionaryKey))
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			IList<object> borderArray = dictionary.GetArray(borderDictionaryKey);
			border = (borderArray == null || borderArray.Count == 0) ? new PdfAnnotationBorder() : new PdfAnnotationBorder(borderArray);
			color = ParseColor(dictionary, colorDictionaryKey);
			structParent = dictionary.GetInteger(structParentDictionaryKey);
			optionalContent = dictionary.GetOptionalContent();
		}
		protected virtual void GenerateAppearanceCommands(IList<PdfCommand> commands) {
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(PageDictionaryKey, page);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(DictionaryType));
			dictionary.Add(PdfDictionary.DictionarySubtypeKey, new PdfName(AnnotationType));
			dictionary.Add(rectDictionaryKey, rect);
			dictionary.Add(contentsDictionaryKey, contents, null);
			dictionary.Add(nameDictionaryKey, name, null);
			dictionary.AddNullable(modifiedDictionaryKey, modified);
			dictionary.Add(flagsDictionaryKey, (int)flags, (int)PdfAnnotationFlags.None);
			dictionary.Add(appearanceDictionaryKey, appearance);
			dictionary.AddName(appearanceNameDictionaryKey, appearanceName);
			if (!border.IsDefault)
				dictionary.Add(borderDictionaryKey, border.ToWritableObject());
			dictionary.Add(colorDictionaryKey, color);
			dictionary.AddNullable(structParentDictionaryKey, structParent);
			dictionary.Add(PdfOptionalContent.DictionaryKey, optionalContent);
			return dictionary;
		}
		protected internal virtual PdfForm GetHighlightedAppearanceForm(PdfAnnotationAppearanceState state, PdfRGBColor highlightColor) {
			return GetAppearanceForm(state);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			Ensure();
			return CreateDictionary(collection);
		}
	}
}

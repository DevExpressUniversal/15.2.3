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
	[PdfDefaultField(PdfFreeTextAnnotationIntent.FreeText)]
	public enum PdfFreeTextAnnotationIntent {
		FreeText,
		FreeTextCallout,
		[PdfFieldName("FreeTextTypewriter", "FreeTextTypeWriter")]
		FreeTextTypewriter
	}
	public class PdfFreeTextAnnotation : PdfMarkupAnnotation {
		internal const string Type = "FreeText";
		const string defaultStyleDictionaryKey = "DS";
		const string calloutDictionaryKey = "CL";
		const string calloutLineEndingStyleDictionaryKey = "LE";
		readonly PdfCommandList appearanceCommands;
		readonly PdfTextJustification textJustification;
		readonly string defaulStyle;
		readonly PdfAnnotationCallout callout;
		readonly PdfFreeTextAnnotationIntent freeTextIntent;
		readonly PdfAnnotationBorderEffect borderEffect;
		readonly PdfRectangle padding;
		readonly PdfAnnotationBorderStyle borderStyle;
		readonly PdfAnnotationLineEndingStyle calloutLineEndingStyle;
		public IEnumerable<PdfCommand> AppearanceCommands { get { return appearanceCommands; } }
		public PdfTextJustification TextJustification { get { return textJustification; } }
		public string DefaultStyle { get { return defaulStyle; } }
		public PdfAnnotationCallout Callout { get { return callout; } }
		public PdfFreeTextAnnotationIntent FreeTextIntent { get { return freeTextIntent; } }
		public PdfAnnotationBorderEffect BorderEffect { get { return borderEffect; } }
		public PdfRectangle Padding { get { return padding; } }
		public PdfAnnotationBorderStyle BorderStyle { get { return borderStyle; } }
		public PdfAnnotationLineEndingStyle CalloutLineEndingStyle { get { return calloutLineEndingStyle; } }
		protected override string AnnotationType { get { return Type; } }
		internal PdfFreeTextAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			appearanceCommands = dictionary.GetAppearance(page.Resources);
			if (appearanceCommands == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			textJustification = dictionary.GetTextJustification();
			defaulStyle = dictionary.GetString(defaultStyleDictionaryKey);
			IList<object> ca = dictionary.GetArray(calloutDictionaryKey);
			if (ca != null && ca.Count > 0)
				callout = new PdfAnnotationCallout(ca);
			freeTextIntent = PdfEnumToStringConverter.Parse<PdfFreeTextAnnotationIntent>(Intent);
			borderEffect = PdfAnnotationBorderEffect.Parse(dictionary);
			padding = dictionary.GetPadding(Rect);
			borderStyle = PdfAnnotationBorderStyle.Parse(dictionary);
			calloutLineEndingStyle = dictionary.GetEnumName<PdfAnnotationLineEndingStyle>(calloutLineEndingStyleDictionaryKey);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(PdfDictionary.DictionaryAppearanceKey, appearanceCommands.ToByteArray(Page.Resources));
			dictionary.Add(PdfDictionary.DictionaryJustificationKey, PdfEnumToValueConverter.Convert(textJustification));
			dictionary.Add(defaultStyleDictionaryKey, defaulStyle, null);
			if (callout != null)
				dictionary.Add(calloutDictionaryKey, callout.ToWritableObject());
			if (borderEffect != null)
				dictionary.Add(PdfAnnotationBorderEffect.DictionaryKey, borderEffect.ToWritableObject());
			dictionary.Add(PdfDictionary.DictionaryPaddingKey, padding);
			dictionary.Add(PdfAnnotationBorderStyle.DictionaryKey, borderStyle);
			dictionary.AddEnumName(calloutLineEndingStyleDictionaryKey, calloutLineEndingStyle);
			return dictionary;
		}
	}
}

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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfWidgetAnnotationTextPosition.NoIcon)]
	public enum PdfWidgetAnnotationTextPosition {
		[PdfFieldValue(0)]
		NoIcon = 0,
		[PdfFieldValue(1)]
		NoCaption,
		[PdfFieldValue(2)]
		CaptionBelowTheIcon,
		[PdfFieldValue(3)]
		CaptionAboveTheIcon,
		[PdfFieldValue(4)]
		CaptionToTheRightOfTheIcon,
		[PdfFieldValue(5)]
		CaptionToTheLeftOfTheIcon,
		[PdfFieldValue(6)]
		CaptionOverlaidDirectlyOnTheIcon
	}
	public class PdfWidgetAppearanceCharacteristics {
		const string rotationAngleDictionaryKey = "R";
		const string borderColorDictionaryKey = "BC";
		const string backgroundColorDictionaryKey = "BG";
		const string captionDictionaryKey = "CA";
		const string rolloverCaptionDictionaryKey = "RC";
		const string alternateCaptionDictionaryKey = "AC";
		const string normalIconDictionaryKey = "I";
		const string rolloverIconDictionaryKey = "RI";
		const string alternateIconDictionaryKey = "IX";
		const string iconFitDictionaryKey = "IF";
		const string textPositionDictionaryKey = "TP";
		readonly int rotationAngle;
		readonly PdfColor borderColor;
		readonly PdfColor backgroundColor;
		readonly string caption;
		readonly string rolloverCaption;
		readonly string alternateCaption;
		readonly PdfXObject normalIcon;
		readonly PdfXObject rolloverIcon;
		readonly PdfXObject alternateIcon;
		readonly PdfIconFit iconFit;
		readonly PdfWidgetAnnotationTextPosition textPosition;
		public int RotationAngle { get { return rotationAngle; } }
		public PdfColor BorderColor { get { return borderColor; } }
		public PdfColor BackgroundColor { get { return backgroundColor; } }
		public string Caption { get { return caption; } }
		public string RolloverCaption { get { return rolloverCaption; } }
		public string AlternateCaption { get { return alternateCaption; } }
		public PdfXObject NormalIcon { get { return normalIcon; } }
		public PdfXObject RolloverIcon { get { return rolloverIcon; } }
		public PdfXObject AlternateIcon { get { return alternateIcon; } }
		public PdfIconFit IconFit { get { return iconFit; } }
		public PdfWidgetAnnotationTextPosition TextPosition { get { return textPosition; } }
		internal PdfWidgetAppearanceCharacteristics(PdfPage page, PdfReaderDictionary dictionary) {
			rotationAngle = PdfPageTreeNode.NormalizeRotate(dictionary.GetInteger(rotationAngleDictionaryKey) ?? 0);
			if (!PdfPageTreeNode.CheckRotate(rotationAngle))
				PdfDocumentReader.ThrowIncorrectDataException();
			borderColor = PdfAnnotation.ParseColor(dictionary, borderColorDictionaryKey);
			backgroundColor = PdfAnnotation.ParseColor(dictionary, backgroundColorDictionaryKey);
			caption = dictionary.GetString(captionDictionaryKey);
			rolloverCaption = dictionary.GetString(rolloverCaptionDictionaryKey);
			alternateCaption = dictionary.GetString(alternateCaptionDictionaryKey);
			PdfObjectCollection objects = dictionary.Objects;
			object value;
			normalIcon = dictionary.TryGetValue(normalIconDictionaryKey, out value) ? objects.GetXObject(value, null, null) : null;
			rolloverIcon = dictionary.TryGetValue(rolloverIconDictionaryKey, out value) ? objects.GetXObject(value, null, null) : null;
			alternateIcon = dictionary.TryGetValue(alternateIconDictionaryKey, out value) ? objects.GetXObject(value, null, null) : null;
			PdfReaderDictionary iconFitDictionary = dictionary.GetDictionary(iconFitDictionaryKey);
			if (iconFitDictionary != null)
				iconFit = new PdfIconFit(iconFitDictionary);
			textPosition = PdfEnumToValueConverter.Parse<PdfWidgetAnnotationTextPosition>(dictionary.GetInteger(textPositionDictionaryKey), PdfWidgetAnnotationTextPosition.NoIcon);
		}
		internal PdfDictionary ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(rotationAngleDictionaryKey, rotationAngle, 0);
			dictionary.Add(borderColorDictionaryKey, borderColor);
			dictionary.Add(backgroundColorDictionaryKey, backgroundColor);
			dictionary.AddASCIIString(captionDictionaryKey, caption);
			dictionary.AddASCIIString(rolloverCaptionDictionaryKey, rolloverCaption);
			dictionary.AddASCIIString(alternateCaptionDictionaryKey, alternateCaption);
			dictionary.Add(normalIconDictionaryKey, normalIcon);
			dictionary.Add(rolloverIconDictionaryKey, rolloverIcon);
			dictionary.Add(alternateIconDictionaryKey, alternateIcon);
			dictionary.Add(iconFitDictionaryKey, iconFit);
			dictionary.Add(textPositionDictionaryKey, PdfEnumToValueConverter.Convert(textPosition), (int)PdfWidgetAnnotationTextPosition.NoIcon);
			return dictionary;
		}
	}
}

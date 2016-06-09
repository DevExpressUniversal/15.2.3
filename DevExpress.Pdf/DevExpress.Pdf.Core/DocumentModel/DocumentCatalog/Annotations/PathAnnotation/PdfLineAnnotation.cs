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
	[PdfDefaultField(PdfLineAnnotationIntent.None, false)]
	public enum PdfLineAnnotationIntent { None, LineArrow, LineDimension };
	[PdfDefaultField(PdfLineAnnotationCaptionPosition.Inline)]
	public enum PdfLineAnnotationCaptionPosition { Inline, Top };
	public class PdfLineAnnotation : PdfUnclosedPathAnnotation {
		internal const string Type = "Line";
		const string leaderLinesLengthDictionaryKey = "LL";
		const string leaderLineExtensionsLengthDictionaryKey = "LLE";
		const string showCaptionDictionaryKey = "Cap";
		const string leaderLineOffsetDictionaryKey = "LLO";
		const string captionPositionDictionaryKey = "CP";
		const string captionOffsetsDictionaryKey = "CO";
		readonly double leaderLinesLength;
		readonly double leaderLineExtensionsLength;
		readonly bool showCaption;
		readonly PdfLineAnnotationIntent lineIntent;
		readonly double leaderLineOffset;
		readonly PdfLineAnnotationCaptionPosition captionPosition;
		readonly double horizontalCaptionOffset;
		readonly double verticalCaptionOffset;
		public double LeaderLinesLength { get { return leaderLinesLength; } }
		public double LeaderLineExtensionsLength { get { return leaderLineExtensionsLength; } }
		public bool ShowCaption { get { return showCaption; } }
		public PdfLineAnnotationIntent LineIntent { get { return lineIntent; } }
		public double LeaderLineOffset { get { return leaderLineOffset; } }
		public PdfLineAnnotationCaptionPosition CaptionPosition { get { return captionPosition; } }
		public double HorizontalCaptionOffset { get { return horizontalCaptionOffset; } }
		public double VerticalCaptionOffset { get { return verticalCaptionOffset; } }
		protected override string AnnotationType { get { return Type; } }
		protected override string VerticesDictionaryKey { get { return "L"; } }
		internal PdfLineAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			leaderLinesLength = dictionary.GetNumber(leaderLinesLengthDictionaryKey) ?? 0;
			leaderLineExtensionsLength = dictionary.GetNumber(leaderLineExtensionsLengthDictionaryKey) ?? 0;
			showCaption = dictionary.GetBoolean(showCaptionDictionaryKey) ?? false;
			lineIntent = PdfEnumToStringConverter.Parse<PdfLineAnnotationIntent>(Intent);
			leaderLineOffset = dictionary.GetNumber(leaderLineOffsetDictionaryKey) ?? 0;
			IList<PdfPoint> vertices = Vertices;
			if (vertices != null && vertices.Count != 2 || (leaderLineExtensionsLength != 0 && leaderLinesLength == 0) || leaderLineOffset < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			captionPosition = PdfEnumToStringConverter.Parse<PdfLineAnnotationCaptionPosition>(dictionary.GetName(captionPositionDictionaryKey));
			IList<object> captionOffsets = dictionary.GetArray(captionOffsetsDictionaryKey);
			if (captionOffsets != null) {
				if (captionOffsets.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				horizontalCaptionOffset = PdfDocumentReader.ConvertToDouble(captionOffsets[0]);
				verticalCaptionOffset = PdfDocumentReader.ConvertToDouble(captionOffsets[1]);
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(leaderLinesLengthDictionaryKey, leaderLinesLength, 0);
			dictionary.Add(leaderLineExtensionsLengthDictionaryKey, leaderLineExtensionsLength, 0);
			dictionary.Add(showCaptionDictionaryKey, showCaption, false);
			dictionary.Add(leaderLineOffsetDictionaryKey, leaderLineOffset, 0);
			dictionary.AddName(captionPositionDictionaryKey, PdfEnumToStringConverter.Convert<PdfLineAnnotationCaptionPosition>(captionPosition));
			if (horizontalCaptionOffset != 0 || verticalCaptionOffset != 0)
				dictionary.Add(captionOffsetsDictionaryKey, new double[] { horizontalCaptionOffset, verticalCaptionOffset });
			return dictionary;
		}
	}
}

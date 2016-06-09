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
	[PdfDefaultField(PdfAnnotationBorderEffectStyle.NoEffect)]
	public enum PdfAnnotationBorderEffectStyle { 
		[PdfFieldName("S")]
		NoEffect,
		[PdfFieldName("C")]
		CloudyEffect
	}
	public class PdfAnnotationBorderEffect {
		internal const string DictionaryKey = "BE";
		const string styleKey = "S";
		const string intensityKey = "I";
		internal static PdfAnnotationBorderEffect Parse(PdfReaderDictionary dictionary) {
			PdfReaderDictionary borderEffectDicionary = dictionary.GetDictionary(DictionaryKey);
			return borderEffectDicionary == null ? null : new PdfAnnotationBorderEffect(borderEffectDicionary);
		}
		readonly PdfAnnotationBorderEffectStyle style;
		readonly double intensity;
		public PdfAnnotationBorderEffectStyle Style { get { return style; } }
		public double Intensity { get { return intensity; } }
		PdfAnnotationBorderEffect(PdfReaderDictionary dictionary) {
			style = dictionary.GetEnumName<PdfAnnotationBorderEffectStyle>(styleKey);
			double? i = dictionary.GetNumber(intensityKey);
			if (i.HasValue) {
				intensity = i.Value;
				if (style != PdfAnnotationBorderEffectStyle.CloudyEffect || intensity < 0 || intensity > 2)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
		}
		internal PdfWriterDictionary ToWritableObject() {
			PdfWriterDictionary result = new PdfWriterDictionary(null);
			result.AddEnumName(styleKey, style);
			result.Add(intensityKey, intensity);
			return result;
		}
	}
}

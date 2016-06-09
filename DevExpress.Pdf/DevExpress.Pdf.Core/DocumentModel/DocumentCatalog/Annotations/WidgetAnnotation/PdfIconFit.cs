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
	[PdfDefaultField(PdfIconScalingCircumstances.Always)]
	public enum PdfIconScalingCircumstances {
		[PdfFieldName("A")]
		Always,
		[PdfFieldName("B")]
		BiggerThanAnnotationRectangle,
		[PdfFieldName("S")]
		SmallerThanAnnotationRectangle,
		[PdfFieldName("N")]
		Never
	};
	[PdfDefaultField(PdfIconScalingType.Proportional)]
	public enum PdfIconScalingType {
		[PdfFieldName("A")]
		Anamorphic,
		[PdfFieldName("P")]
		Proportional
	}
	public class PdfIconFit : PdfObject {
		const string scalingCircumstancesDictionaryKey = "SW";
		const string scalingTypeDictionaryKey = "S";
		const string positionDictionaryKey = "A";
		const string fitToAnnotationBoundsDictionaryKey = "FB";
		const double defaultPosition = 0.5;
		static double ConvertToPosition(object value) {
			double position = PdfDocumentReader.ConvertToDouble(value);
			if (position < 0 || position > 1)
				PdfDocumentReader.ThrowIncorrectDataException();
			return position;
		}
		readonly PdfIconScalingCircumstances scalingCircumstances;
		readonly PdfIconScalingType scalingType;
		readonly double horizontalPosition;
		readonly double verticalPosition;
		readonly bool fitToAnnotationBounds;
		public PdfIconScalingCircumstances ScalingCircumstances { get { return scalingCircumstances; } }
		public PdfIconScalingType ScalingType { get { return scalingType; } }
		public double HorizontalPosition { get { return horizontalPosition; } }
		public double VerticalPosition { get { return verticalPosition; } }
		public bool FitToAnnotationBounds { get { return fitToAnnotationBounds; } }
		internal PdfIconFit(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			scalingCircumstances = PdfEnumToStringConverter.Parse<PdfIconScalingCircumstances>(dictionary.GetName(scalingCircumstancesDictionaryKey));
			scalingType = PdfEnumToStringConverter.Parse<PdfIconScalingType>(dictionary.GetName(scalingTypeDictionaryKey));
			IList<object> positions = dictionary.GetArray(positionDictionaryKey);
			if (positions == null) {
				horizontalPosition = defaultPosition;
				verticalPosition = defaultPosition;
			}
			else {
				if (positions.Count != 2)
					PdfDocumentReader.ThrowIncorrectDataException();
				horizontalPosition = ConvertToPosition(positions[0]);
				verticalPosition = ConvertToPosition(positions[1]);
			}
			fitToAnnotationBounds = dictionary.GetBoolean(fitToAnnotationBoundsDictionaryKey) ?? false;
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.AddEnumName(scalingCircumstancesDictionaryKey, scalingCircumstances);
			dictionary.AddEnumName(scalingTypeDictionaryKey, scalingType);
			if (horizontalPosition != defaultPosition || verticalPosition != defaultPosition)
				dictionary.Add(positionDictionaryKey, new object[] { horizontalPosition, verticalPosition });
			dictionary.Add(fitToAnnotationBoundsDictionaryKey, fitToAnnotationBounds, false);
			return dictionary;
		}
	}
}

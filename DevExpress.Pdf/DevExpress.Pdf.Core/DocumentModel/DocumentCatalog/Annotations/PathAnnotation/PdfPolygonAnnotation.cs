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
	[PdfDefaultField(PdfPolygonAnnotationIntent.None, false)]
	public enum PdfPolygonAnnotationIntent { None, PolygonCloud, PolygonDimension };
	public class PdfPolygonAnnotation : PdfPathAnnotation {
		internal const string Type = "Polygon";
		readonly PdfAnnotationBorderEffect borderEffect;
		readonly PdfPolygonAnnotationIntent polygonIntent;
		public PdfAnnotationBorderEffect BorderEffect { get { return borderEffect; } }
		public PdfPolygonAnnotationIntent PolygonIntent { get { return polygonIntent; } }
		protected override string AnnotationType { get { return Type; } }
		internal PdfPolygonAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			borderEffect = PdfAnnotationBorderEffect.Parse(dictionary);
			polygonIntent = PdfEnumToStringConverter.Parse<PdfPolygonAnnotationIntent>(Intent);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			if (borderEffect != null)
				dictionary.Add(PdfAnnotationBorderEffect.DictionaryKey, borderEffect.ToWritableObject());
			return dictionary;
		}
	}
}

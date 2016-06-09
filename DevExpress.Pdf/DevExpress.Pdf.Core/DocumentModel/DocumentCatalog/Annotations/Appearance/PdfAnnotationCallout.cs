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
	public class PdfAnnotationCallout {
		readonly PdfPoint startPoint;
		readonly PdfPoint? kneePoint;
		readonly PdfPoint endPoint;
		public PdfPoint StartPoint { get { return startPoint; } }
		public PdfPoint? KneePoint { get { return kneePoint; } }
		public PdfPoint EndPoint { get { return endPoint; } }
		internal PdfAnnotationCallout(IList<object> array) {
			switch (array.Count) {
				case 4:
					startPoint = PdfDocumentReader.CreatePoint(array, 0);
					endPoint = PdfDocumentReader.CreatePoint(array, 2);
					break;
				case 6:
					startPoint = PdfDocumentReader.CreatePoint(array, 0);
					kneePoint = PdfDocumentReader.CreatePoint(array, 2);
					endPoint = PdfDocumentReader.CreatePoint(array, 4);
					break;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
		}
		internal IList<double> ToWritableObject() {
			List<double> result = new List<double>();
			result.Add(startPoint.X);
			result.Add(startPoint.Y);
			if (kneePoint.HasValue) {
				result.Add(kneePoint.Value.X);
				result.Add(kneePoint.Value.Y);
			}
			result.Add(endPoint.X);
			result.Add(endPoint.Y);
			return result;
		}
	}
}

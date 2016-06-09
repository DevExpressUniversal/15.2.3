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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfLuminositySoftMask : PdfCustomSoftMask {
		internal const string Name = "Luminosity";
		const string backdropColorDictionaryKey = "BC";
		readonly PdfColor backdropColor;
		public PdfColor BackdropColor { get { return backdropColor; } }
		protected override string ActualName { get { return Name; } }
		internal PdfLuminositySoftMask(PdfReaderDictionary dictionary) : base(dictionary) {
			PdfColorSpace colorSpace = TransparencyGroup.Group.ColorSpace;
			if (colorSpace == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<object> backdropColorArray = dictionary.GetArray(backdropColorDictionaryKey);
			if (backdropColorArray != null) {
				int count = backdropColorArray.Count;
				if (count != colorSpace.ComponentsCount)
					PdfDocumentReader.ThrowIncorrectDataException();
				double[] components = new double[count];
				for (int i = 0; i < count; i++)
					components[i] = PdfDocumentReader.ConvertToDouble(backdropColorArray[i]);
				backdropColor = new PdfColor(components);
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(backdropColorDictionaryKey, backdropColor);
			return dictionary;
		}
	}
}

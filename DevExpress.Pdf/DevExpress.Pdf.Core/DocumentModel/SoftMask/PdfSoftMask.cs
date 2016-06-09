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
	public abstract class PdfSoftMask : PdfObject {
		internal const string SoftMaskTypeDictionaryKey = "S";
		internal const string DictionaryType = "Mask";
		internal static PdfSoftMask Create(PdfDocumentCatalog documentCatalog, object value) {
			PdfObjectCollection objects = documentCatalog.Objects;
			PdfObjectReference reference = value as PdfObjectReference;
			if (reference != null)
				return objects.ResolveObject<PdfSoftMask>(reference.Number, () => Create(documentCatalog, objects.GetObjectData(reference.Number)));
			PdfName name = value as PdfName;
			if (name != null) {
				if (name.Name != PdfEmptySoftMask.Name)
					PdfDocumentReader.ThrowIncorrectDataException();
				return PdfEmptySoftMask.Instance;
			}
			PdfReaderDictionary softMaskDictionary = value as PdfReaderDictionary;
			if (softMaskDictionary == null) 
				PdfDocumentReader.ThrowIncorrectDataException();
			string type = softMaskDictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string softMaskType = softMaskDictionary.GetName(SoftMaskTypeDictionaryKey);
			if ((type != null && type != DictionaryType) || softMaskType == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (softMaskType) {
				case PdfLuminositySoftMask.Name:
					return new PdfLuminositySoftMask(softMaskDictionary);
				case PdfAlphaSoftMask.Name:
					return new PdfAlphaSoftMask(softMaskDictionary);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		protected PdfSoftMask(PdfObjectCollection objects) {
		}
		protected internal abstract bool IsSame(PdfSoftMask softMask);
		protected internal abstract object Write(PdfObjectCollection objects);
	}
}

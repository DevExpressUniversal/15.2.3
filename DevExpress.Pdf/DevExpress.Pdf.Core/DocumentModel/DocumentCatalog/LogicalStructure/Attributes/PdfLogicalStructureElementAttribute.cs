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
	public abstract class PdfLogicalStructureElementAttribute : PdfObject {
		protected const string OwnerKey = "O";
		static PdfLogicalStructureElementAttribute ParseAttribute(PdfObjectCollection collection, object value) {
			PdfReaderDictionary dictionary = collection.TryResolve(value) as PdfReaderDictionary;
			if (dictionary == null)
				return null;
			PdfObjectReference reference = value as PdfObjectReference;
			return reference == null ? Parse(dictionary) : collection.GetLogicalStructureElementAttributes(reference.Number);
		}
		internal static PdfLogicalStructureElementAttribute Parse(PdfReaderDictionary dictionary) {
			string ownerString = dictionary.GetName(OwnerKey);
			switch (ownerString) {
				case PdfLayoutLogicalStructureElementAttribute.Owner:
					return PdfLayoutLogicalStructureElementAttribute.ParseAttribute(dictionary);
				case PdfListLogicalStructureElementAttribute.Owner:
					return new PdfListLogicalStructureElementAttribute(dictionary);
				case PdfPrintFieldLogicalStructureElementAttribute.Owner:
					return new PdfPrintFieldLogicalStructureElementAttribute(dictionary);
				case PdfTableLogicalStructureElementAttribute.Owner:
					return new PdfTableLogicalStructureElementAttribute(dictionary);
				default:
					return new PdfCustomLogicalStructureElementAttribute(ownerString, dictionary);
			}
		}
		internal static PdfLogicalStructureElementAttribute[] Parse(PdfObjectCollection collection, object value) {
			PdfLogicalStructureElementAttribute attribute = ParseAttribute(collection, value);
			if (attribute != null)
				return new PdfLogicalStructureElementAttribute[] { attribute };
			value = collection.TryResolve(value);
			if (value == null)
				return null;
			IList<object> list = value as IList<object>;
			if (list == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			int count = list.Count;
			PdfLogicalStructureElementAttribute[] result = new PdfLogicalStructureElementAttribute[count];
			for (int i = 0; i < count; i++) {
				attribute = ParseAttribute(collection, list[i]);
				if (attribute == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				result[i] = attribute;
			}
			return result;
		}
		protected PdfLogicalStructureElementAttribute(int objectNumber) : base(objectNumber) { 
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return CreateDictionary(collection);
		}
		protected abstract PdfWriterDictionary CreateDictionary(PdfObjectCollection collection);
	}
}

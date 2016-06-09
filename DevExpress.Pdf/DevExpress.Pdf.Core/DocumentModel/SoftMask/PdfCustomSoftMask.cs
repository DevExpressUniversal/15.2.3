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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public abstract class PdfCustomSoftMask : PdfSoftMask{
		const string transparencyGroupDictionaryKey = "G";
		const string transferFunctionDictionaryKey = "TR";
		readonly PdfGroupForm transparencyGroup;
		readonly PdfFunction transferFunction;
		public PdfGroupForm TransparencyGroup { get { return transparencyGroup; } }
		public PdfFunction TransferFunction { get { return transferFunction; } }
		protected abstract string ActualName { get; }
		protected PdfCustomSoftMask(PdfObjectCollection objects, PdfGroupForm transparencyGroup, PdfFunction transferFunction) : base(objects) {
			if (transparencyGroup == null)
				throw new ArgumentNullException("transparencyGroup");
			this.transparencyGroup = transparencyGroup;
			this.transferFunction = transferFunction ?? PdfPredefinedFunction.Identity;
		}
		protected PdfCustomSoftMask(PdfReaderDictionary dictionary) : base(dictionary.Objects) {
			object value;
			PdfObjectCollection objects = dictionary.Objects;
			if (!dictionary.TryGetValue(transparencyGroupDictionaryKey,out value))
				PdfDocumentReader.ThrowIncorrectDataException();
			transparencyGroup = objects.GetXObject(value, null, PdfForm.Type) as PdfGroupForm;
			if (transparencyGroup == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			transferFunction = dictionary.TryGetValue(transferFunctionDictionaryKey, out value) ? PdfFunction.Parse(objects, value, false) : PdfPredefinedFunction.Identity;
		}
		protected internal override bool IsSame(PdfSoftMask softMask) {
			return false;
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return objects.AddObject(this);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(PdfSoftMask.DictionaryType));
			dictionary.Add(PdfSoftMask.SoftMaskTypeDictionaryKey, new PdfName(ActualName));
			dictionary.Add(transparencyGroupDictionaryKey, objects.AddObject(transparencyGroup));
			if (transferFunction != PdfPredefinedFunction.Identity)
				dictionary.Add(transferFunctionDictionaryKey, transferFunction.Write(objects));
			return dictionary;
		}
	}
}

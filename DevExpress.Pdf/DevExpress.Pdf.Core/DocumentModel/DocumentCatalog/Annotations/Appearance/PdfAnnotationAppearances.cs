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
	public class PdfAnnotationAppearances : PdfObject {
		internal const string NormalAppearanceDictionaryKey = "N";
		const string rolloverAppeaeranceDictinaryKey = "R";
		const string downAppearanceDictionaryKey = "D";
		readonly PdfAnnotationAppearance normal;
		readonly PdfAnnotationAppearance rollover;
		readonly PdfAnnotationAppearance down;
		readonly PdfForm form;
		public PdfAnnotationAppearance Normal { get { return normal; } }
		public PdfAnnotationAppearance Rollover { get { return rollover; } }
		public PdfAnnotationAppearance Down { get { return down; } }
		public PdfForm Form { get { return form; } }
		internal IList<string> Names {
			get {
				List<string> names = normal.GetNames(NormalAppearanceDictionaryKey);
				if (rollover != null)
					names.AddRange(rollover.GetNames(rolloverAppeaeranceDictinaryKey));
				if (down != null)
					names.AddRange(down.GetNames(downAppearanceDictionaryKey));
				return names;
			}
		}
		internal PdfAnnotationAppearances(PdfDocumentCatalog documentCatalog, PdfRectangle bBox) {
			normal = new PdfAnnotationAppearance(documentCatalog, bBox);
		}
		internal PdfAnnotationAppearances(PdfReaderDictionary dictionary, PdfRectangle parentBBox) : base (dictionary.Number) {
			normal = PdfAnnotationAppearance.Parse(dictionary, NormalAppearanceDictionaryKey);
			if (normal == null) {
				if (parentBBox == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				normal = new PdfAnnotationAppearance(dictionary.Objects.DocumentCatalog, parentBBox);
			}
			rollover = PdfAnnotationAppearance.Parse(dictionary, rolloverAppeaeranceDictinaryKey);
			down = PdfAnnotationAppearance.Parse(dictionary, downAppearanceDictionaryKey);
		}
		internal PdfAnnotationAppearances(PdfForm form) : base(form.ObjectNumber) {
			this.form = form;
		}
		internal void UpdateDefaultFormTransformation(PdfRectangle bBox, PdfTransformationMatrix matrix) {
			normal.UpdateDefaultFormTransformation(bBox, matrix);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			if (form != null)
				return form.ToWritableObject(objects);
			PdfDictionary dictionary = new PdfDictionary();
			dictionary.Add(NormalAppearanceDictionaryKey, normal.ToWritableObject(objects));
			if (rollover != null)
				dictionary.Add(rolloverAppeaeranceDictinaryKey, rollover.ToWritableObject(objects));
			if (down != null)
				dictionary.Add(downAppearanceDictionaryKey, down.ToWritableObject(objects));
			return dictionary;
		}
	}
}

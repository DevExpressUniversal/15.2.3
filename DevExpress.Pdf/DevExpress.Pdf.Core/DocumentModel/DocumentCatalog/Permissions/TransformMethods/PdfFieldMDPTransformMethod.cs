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
namespace DevExpress.Pdf.Native {
	public enum PdfFormFieldPermission { All, Include, Exclude }
	public class PdfFieldMDPTransformMethod : PdfTransformMethod {
		const string actionDictionaryKey = "Action";
		const string fieldDictionaryKey = "Fields";
		readonly PdfFormFieldPermission formPermission;
		readonly IList<string> fields;
		public PdfFormFieldPermission FormPermission { get { return formPermission; } }
		public IList<string> Fields { get { return fields; } }
		protected override string ValidVersion { get { return "1.2"; } }
		public PdfFieldMDPTransformMethod(PdfReaderDictionary dictionary) : base(dictionary) {
				PdfDocumentReader.ThrowIncorrectDataException();
			formPermission = dictionary.GetEnumName<PdfFormFieldPermission>(actionDictionaryKey);
			fields = dictionary.GetArray<string>(fieldDictionaryKey, (o) => PdfDocumentReader.ConvertToString(o as byte[]));
			if ((formPermission == PdfFormFieldPermission.Exclude || formPermission == PdfFormFieldPermission.Include) && fields == null)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
	}
}

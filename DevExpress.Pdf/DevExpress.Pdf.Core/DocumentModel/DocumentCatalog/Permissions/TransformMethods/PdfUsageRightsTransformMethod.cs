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
	public enum PdfDocumentRights { Create, Delete, Modify, Copy, Import, Export, Online, SummaryView }
	public enum PdfAnnotsRights { Create, Delete, Modify, Copy, Import, Export, Online, SummaryView }
	public enum PdfFormFieldRights { Add, Delete, Import, Export, SubmitStandalone, SpawnTemplate, BarcodePlaintext, Online }
	public enum PdfEmbeddedFilesRights { Create, Delete, Modify, Import }
	public enum PdfSignatureRights { Modify }
	public class PdfUsageRightsTransformMethod : PdfTransformMethod {
		const string documentDictionaryKey = "Document";
		const string messageDictionaryKey = "Msg";
		const string annotsDictionaryKey = "Annots";
		const string formDictionaryKey = "Form";
		const string signatureDictionaryKey = "Signature";
		const string allowPermissionsDictionaryKey = "P";
		static T ConvertToEnum<T>(object value) where T : struct {
			PdfName name = value as PdfName;
			if (name == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfEnumToStringConverter.Parse<T>(name.Name);
		}
		readonly IList<PdfDocumentRights> documentRights;
		readonly string message;
		readonly IList<PdfAnnotsRights> annotsRights;
		readonly IList<PdfFormFieldRights> formFieldRights;
		readonly IList<PdfSignatureRights> signatureRights;
		readonly IList<PdfEmbeddedFilesRights> embeddedFilesRights;
		readonly bool allowPermissions;
		public IList<PdfDocumentRights> DocumentRights { get { return documentRights; } }
		public string Message { get { return message; } }
		public IList<PdfAnnotsRights> AnnotsRights { get { return annotsRights; } }
		public IList<PdfFormFieldRights> FormFieldRights { get { return formFieldRights; } }
		public IList<PdfSignatureRights> SignatureRights { get { return signatureRights; } }
		public IList<PdfEmbeddedFilesRights> EmbeddedFilesRights { get { return embeddedFilesRights; } }
		public bool AllowPermissions { get { return allowPermissions; } }
		protected override string ValidVersion { get { return "2.2"; } }
		public PdfUsageRightsTransformMethod(PdfReaderDictionary dictionary) : base(dictionary) {
			documentRights = dictionary.GetArray<PdfDocumentRights>(documentDictionaryKey, o => ConvertToEnum<PdfDocumentRights>(o));
			message = dictionary.GetString(messageDictionaryKey);
			annotsRights = dictionary.GetArray<PdfAnnotsRights>(documentDictionaryKey, o => ConvertToEnum<PdfAnnotsRights>(o));
			formFieldRights = dictionary.GetArray<PdfFormFieldRights>(documentDictionaryKey, o => ConvertToEnum<PdfFormFieldRights>(o));
			signatureRights = dictionary.GetArray<PdfSignatureRights>(documentDictionaryKey, o => ConvertToEnum<PdfSignatureRights>(o));
			embeddedFilesRights = dictionary.GetArray<PdfEmbeddedFilesRights>(documentDictionaryKey, o => ConvertToEnum<PdfEmbeddedFilesRights>(o));
			allowPermissions = dictionary.GetBoolean(allowPermissionsDictionaryKey) ?? false;
		}
	}
}

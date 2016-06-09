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
using System.Security;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfEncryptionOptions {
		SecureString userPassword;
		SecureString ownerPassword;
		PdfDocumentPrintingPermissions printingPermissions = PdfDocumentPrintingPermissions.Allowed;
		PdfDocumentDataExtractionPermissions dataExtractionPermissions = PdfDocumentDataExtractionPermissions.Allowed;
		PdfDocumentModificationPermissions modificationPermissions = PdfDocumentModificationPermissions.Allowed;
		PdfDocumentInteractivityPermissions interactivityPermissions = PdfDocumentInteractivityPermissions.Allowed;
		public PdfDocumentPrintingPermissions PrintingPermissions {
			get { return printingPermissions; }
			set { printingPermissions = value; }
		}
		public PdfDocumentDataExtractionPermissions DataExtractionPermissions {
			get { return dataExtractionPermissions; }
			set { dataExtractionPermissions = value; }
		}
		public PdfDocumentModificationPermissions ModificationPermissions {
			get { return modificationPermissions; }
			set { modificationPermissions = value; }
		}
		public PdfDocumentInteractivityPermissions InteractivityPermissions {
			get { return interactivityPermissions; }
			set { interactivityPermissions = value; }
		}
		public SecureString UserPassword {
			get { return userPassword; }
			set { userPassword = value; }
		}
		public SecureString OwnerPassword {
			get { return ownerPassword; }
			set { ownerPassword = value; }
		}
		internal long PermissionsValue {
			get {
				long result = 0xFFFFF0C0;
				switch (printingPermissions) {
					case PdfDocumentPrintingPermissions.LowQuality:
						result |= (int)PdfDocumentPermissionsFlags.Printing;
						break;
					case PdfDocumentPrintingPermissions.Allowed:
						result |= (int)(PdfDocumentPermissionsFlags.HighQualityPrinting | PdfDocumentPermissionsFlags.Printing);
						break;
				}
				switch (dataExtractionPermissions) {
					case PdfDocumentDataExtractionPermissions.Accessibility:
						result |= (int)PdfDocumentPermissionsFlags.Accessibility;
						break;
					case PdfDocumentDataExtractionPermissions.Allowed:
						result |= (int)(PdfDocumentPermissionsFlags.Accessibility | PdfDocumentPermissionsFlags.DataExtraction);
						break;
				}
				switch (interactivityPermissions) {
					case PdfDocumentInteractivityPermissions.FormFillingAndSigning:
						result |= (int)PdfDocumentPermissionsFlags.FormFilling;
						break;
					case PdfDocumentInteractivityPermissions.Allowed:
						result |= (int)(PdfDocumentPermissionsFlags.FormFilling | PdfDocumentPermissionsFlags.ModifyingFormFieldsAndAnnotations);
						break;
				}
				switch (modificationPermissions) {
					case PdfDocumentModificationPermissions.DocumentAssembling:
						result |= (int)PdfDocumentPermissionsFlags.DocumentAssembling;
						break;
					case PdfDocumentModificationPermissions.Allowed:
						result |= (int)(PdfDocumentPermissionsFlags.DocumentAssembling | PdfDocumentPermissionsFlags.Modifying);
						break;
				}
				return result;
			}
		}
	}
}

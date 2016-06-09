#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.IO;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Spreadsheet;
using System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Import {
	#region OpenXmlDocumentImporterOptions
	public class OpenXmlDocumentImporterOptions : DocumentImporterOptions {
		string encryptionPassword;
		CalculationModeOverride overrideCalculationMode = CalculationModeOverride.None;
		#region Properties
		protected internal override DocumentFormat Format { get { return DocumentFormat.OpenXml; } }
		#region EncryptionPassword
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("OpenXmlDocumentImporterOptionsEncryptionPassword")]
#endif
		public string EncryptionPassword {
			get { return encryptionPassword; }
			set {
				if (encryptionPassword == value)
					return;
				string oldValue = encryptionPassword;
				encryptionPassword  = value;
				OnChanged("EncryptionPassword", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeEncryptionPassword() {
			return !String.IsNullOrEmpty(EncryptionPassword);
		}
		protected internal virtual void ResetEncryptionPassword() {
			EncryptionPassword = String.Empty;
		}
		#endregion
		#region #OverrideCalculationMode
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("OpenXmlDocumentImporterOptionsOverrideCalculationMode"),
#endif
		DefaultValue(CalculationModeOverride.None)]
		public CalculationModeOverride OverrideCalculationMode {
			get { return overrideCalculationMode; }
			set {
				if (overrideCalculationMode != value) {
					CalculationModeOverride oldValue = overrideCalculationMode;
					overrideCalculationMode = value;
					OnChanged("OverrideCalculationMode", oldValue, overrideCalculationMode);
				}
			}
		}
		#endregion
		#endregion
		public override void Reset() {
			base.Reset();
			ResetEncryptionPassword();
			OverrideCalculationMode = CalculationModeOverride.None;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			OpenXmlDocumentImporterOptions options = value as OpenXmlDocumentImporterOptions;
			if (options != null) {
				this.EncryptionPassword = options.EncryptionPassword;
				this.OverrideCalculationMode = options.OverrideCalculationMode;
			}
		}
	}
	#endregion
	#region OpenXmlDocumentImporter
	public class OpenXmlDocumentImporter : IImporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_OpenXmlFiles), "xlsx");
		#region IDocumentImporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.OpenXml; } }
		public IImporterOptions SetupLoading() {
			return new OpenXmlDocumentImporterOptions();
		}
		public bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.LoadDocumentOpenXmlContent(stream, (OpenXmlDocumentImporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
	#region XltxDocumentImporterOptions
	public class XltxDocumentImporterOptions : OpenXmlDocumentImporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xltx; } }
	}
	#endregion
	#region XltxDocumentImporter
	public class XltxDocumentImporter : IImporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_XltxFiles), "xltx");
		#region IDocumentImporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xltx; } }
		public IImporterOptions SetupLoading() {
			return new XltxDocumentImporterOptions();
		}
		public bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.LoadDocumentXltxContent(stream, (XltxDocumentImporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
}

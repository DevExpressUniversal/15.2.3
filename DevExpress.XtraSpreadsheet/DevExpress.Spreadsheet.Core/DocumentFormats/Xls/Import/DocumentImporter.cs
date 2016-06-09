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
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Spreadsheet;
namespace DevExpress.XtraSpreadsheet.Import {
	#region CalculationModeOverride
	public enum CalculationModeOverride {
		None = -1,
		AutomaticExceptTables = 0,
		Manual = 1,
		Automatic = 2
	}
	#endregion
	#region XlsDocumentImporterOptions
	public class XlsDocumentImporterOptions : DocumentImporterOptions {
		#region Fields
		string password = string.Empty;
		bool createDefaultDocumentOnContentError = true;
		bool validateFormula = false;
		CalculationModeOverride overrideCalculationMode = CalculationModeOverride.None;
		#endregion
		#region Properties
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xls; } }
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("XlsDocumentImporterOptionsPassword"),
#endif
		DefaultValue("")]
		public string Password
		{
			get { return password; }
			set {
				if (value == null)
					value = string.Empty;
				if (password != value) {
					string oldPassword = password;
					password = value;
					OnChanged("Password", oldPassword, password);
				}
			}
		}
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("XlsDocumentImporterOptionsValidateFormula"),
#endif
		DefaultValue(false)]
		public bool ValidateFormula
		{
			get { return validateFormula; }
			set {
				if (validateFormula != value) {
					bool oldValue = validateFormula;
					validateFormula = value;
					OnChanged("ValidateFormula", oldValue, validateFormula);
				}
			}
		}
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("XlsDocumentImporterOptionsOverrideCalculationMode"),
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
		protected internal bool CreateDefaultDocumentOnContentError {
			get { return createDefaultDocumentOnContentError; }
			set { createDefaultDocumentOnContentError = value; }
		}
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			Password = string.Empty;
			CreateDefaultDocumentOnContentError = true;
			ValidateFormula = false;
			OverrideCalculationMode = CalculationModeOverride.None;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			XlsDocumentImporterOptions options = value as XlsDocumentImporterOptions;
			if(options != null) {
				Password = options.Password;
				CreateDefaultDocumentOnContentError = options.CreateDefaultDocumentOnContentError;
				ValidateFormula = options.ValidateFormula;
				OverrideCalculationMode = options.OverrideCalculationMode;
			}
		}
	}
	#endregion
	#region XlsDocumentImporter
	public class XlsDocumentImporter : IImporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_DocFiles), "xls");
		#region IDocumentImporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xls; } }
		public IImporterOptions SetupLoading() {
			return new XlsDocumentImporterOptions();
		}
		public bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.LoadDocumentXlsContent(stream, (XlsDocumentImporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
	#region XltDocumentImporterOptions
	public class XltDocumentImporterOptions : XlsDocumentImporterOptions {
		protected internal override DocumentFormat Format { get { return DocumentFormat.Xlt; } }
	}
	#endregion
	#region XltDocumentImporter
	public class XltDocumentImporter : IImporter<DocumentFormat, bool> {
		internal static readonly FileDialogFilter filter = new FileDialogFilter(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_XltFiles), "xlt");
		#region IDocumentImporter Members
		public FileDialogFilter Filter { get { return filter; } }
		public DocumentFormat Format { get { return DocumentFormat.Xlt; } }
		public IImporterOptions SetupLoading() {
			return new XltDocumentImporterOptions();
		}
		public bool LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			DocumentModel model = (DocumentModel)documentModel;
			model.InternalAPI.LoadDocumentXltContent(stream, (XltDocumentImporterOptions)options);
			return true;
		}
		#endregion
	}
	#endregion
}

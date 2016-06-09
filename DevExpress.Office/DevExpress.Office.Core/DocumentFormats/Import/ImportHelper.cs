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
using System.Collections.Generic;
using System.IO;
using System.Text;
using DevExpress.Office.Import;
using DevExpress.Office.Localization;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Office.Internal {
	#region ImportExportHelper (abstract class)
	public abstract class ImportExportHelper {
		readonly IDocumentModel documentModel;
		protected ImportExportHelper(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public IDocumentModel DocumentModel { get { return documentModel; } }
		protected string CreateFilterString(FileDialogFilterCollection filters) {
			return filters.CreateFilterString();
		}
		public abstract void ThrowUnsupportedFormatException();
	}
	#endregion
	#region ImportHelper<TFormat, TResult> (abstract class)
	public abstract class ImportHelper<TFormat, TResult> : ImportExportHelper {
		protected ImportHelper(IDocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal abstract TFormat UndefinedFormat { get; }
		protected internal abstract TFormat FallbackFormat { get; }
		public TResult Import(Stream stream, TFormat format, string sourceUri, IImportManagerService<TFormat, TResult> importManagerService) {
			return Import(stream, format, sourceUri, importManagerService, null);
		}
		public TResult Import(Stream stream, TFormat format, string sourceUri, IImportManagerService<TFormat, TResult> importManagerService, Encoding encoding) {
			IImporter<TFormat, TResult> importer = importManagerService.GetImporter(format);
			return Import(stream, sourceUri, importer, encoding);
		}
		public TResult Import(Stream stream, string sourceUri, IImporter<TFormat, TResult> importer, Encoding encoding) {
			if (importer == null)
				ThrowUnsupportedFormatException();
			IImporterOptions options = importer.SetupLoading();
			IImporterOptions predefinedOptions = GetPredefinedOptions(importer.Format);
			if (predefinedOptions != null)
				options.CopyFrom(predefinedOptions);
			options.SourceUri = sourceUri;
			if (encoding != null)
				ApplyEncoding(options, encoding);
			return importer.LoadDocument(DocumentModel, stream, options);
		}
		public IImporter<TFormat, TResult> AutodetectImporter(string fileName, IImportManagerService<TFormat, TResult> importManagerService) {
			return AutodetectImporter(fileName, importManagerService, true);
		}
		public IImporter<TFormat, TResult> AutodetectImporter(string fileName, IImportManagerService<TFormat, TResult> importManagerService, bool useFormatFallback) {
			List<IImporter<TFormat, TResult>> importers = importManagerService.GetImporters();
			if (importers.Count <= 0)
				return null;
			return ChooseImporter(fileName, -1, importers, useFormatFallback);
		}
		public TResult ImportFromFileAutodetectFormat(string fileName, IImportManagerService<TFormat, TResult> importManagerService) {
			IImporter<TFormat, TResult> importer = AutodetectImporter(fileName, importManagerService);
			if (importer == null)
				return default(TResult);
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				return Import(stream, importer.Format, fileName, importManagerService);
			}
		}
		public ImportSource<TFormat, TResult> InvokeImportDialog(IWin32Window parent, IImportManagerService<TFormat, TResult> importManagerService) {
#if !DXPORTABLE
			if (importManagerService == null)
				ThrowUnsupportedFormatException();
			List<IImporter<TFormat, TResult>> importers = importManagerService.GetImporters();
			if (importers.Count <= 0)
				ThrowUnsupportedFormatException();
			FileDialogFilterCollection filters = CreateImportFilters(GetLoadDocumentDialogFileFilters(importManagerService));
			OpenFileDialog dialog = CreateOpenFileDialog(filters);
			if (!ShowOpenFileDialog(dialog, parent))
				return null;
			IImporter<TFormat, TResult> importer = ChooseImporter(GetFileName(dialog), dialog.FilterIndex - 1, importers, true); 
			if (importer == null)
				ThrowUnsupportedFormatException();
			return new ImportSource<TFormat, TResult>(GetFileStorage(dialog), GetFileName(dialog), importer);
#else
			return null;
#endif
		}
#if !DXPORTABLE
		protected internal virtual bool ShowOpenFileDialog(OpenFileDialog dialog, IWin32Window parent) {
#if !SL
			return dialog.ShowDialog(parent) == DialogResult.OK;
#else
			return dialog.ShowDialog() == true;
#endif
		}
#endif
		protected internal virtual FileDialogFilterCollection GetLoadDocumentDialogFileFilters(IImportManagerService<TFormat, TResult> importManagerService) {
			List<IImporter<TFormat, TResult>> importers = importManagerService.GetImporters();
			FileDialogFilterCollection result = new FileDialogFilterCollection();
			foreach (IImporter<TFormat, TResult> importer in importers)
				result.Add(importer.Filter);
			return result;
		}
		protected internal IImporter<TFormat, TResult> ChooseImporter(string fileName, int filterIndex, List<IImporter<TFormat, TResult>> importers, bool useFormatFallback) {
			IImporter<TFormat, TResult> result = ChooseImporterByFileName(fileName, importers);
			if (result == null)
				result = ChooseImporterByFilterIndex(filterIndex, importers);
			if (result == null) {
				TFormat format = useFormatFallback ? FallbackFormat : UndefinedFormat;
				result = ChooseImporterByFormat(format, importers);
			}
			return result;
		}
		protected IImporter<TFormat, TResult> ChooseImporterByFileName(string fileName, List<IImporter<TFormat, TResult>> importers) {
			string extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
			if (String.IsNullOrEmpty(extension))
				return null;
			int count = importers.Count;
			for (int i = 0; i < count; i++) {
				FileExtensionCollection extensions = importers[i].Filter.Extensions;
				if (extensions.IndexOf(extension) >= 0)
					return importers[i];
			}
			return null;
		}
		protected IImporter<TFormat, TResult> ChooseImporterByFormat(TFormat format, List<IImporter<TFormat, TResult>> importers) {
			if (format.Equals(UndefinedFormat))
				return null;
			int count = importers.Count;
			for (int i = 0; i < count; i++) {
				if (format.Equals(importers[i].Format))
					return importers[i];
			}
			return null;
		}
		protected IImporter<TFormat, TResult> ChooseImporterByFilterIndex(int filterIndex, List<IImporter<TFormat, TResult>> importers) {
			if (filterIndex >= 0 && filterIndex < importers.Count)
				return importers[filterIndex];
			else
				return null;
		}
#if !DXPORTABLE
		protected internal virtual OpenFileDialog CreateOpenFileDialog(FileDialogFilterCollection filters) {
			OpenFileDialog result = new OpenFileDialog();
			result.Filter = CreateFilterString(filters);
			result.FilterIndex = 2; 
			result.Multiselect = false;
#if !SL
			result.RestoreDirectory = true;
			result.CheckFileExists = true;
			result.CheckPathExists = true;
			result.SupportMultiDottedExtensions = true;
			result.AddExtension = false;
			result.DereferenceLinks = true;
			result.ValidateNames = true;
#endif
			return result;
		}
		protected internal virtual FileDialogFilterCollection CreateImportFilters(FileDialogFilterCollection filters) {
			FileDialogFilter allSupportedFilesFilter = new FileDialogFilter();
			allSupportedFilesFilter.Description = OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_AllSupportedFiles);
			FileDialogFilterCollection result = new FileDialogFilterCollection();
			result.Add(FileDialogFilter.AllFiles);
			result.Add(allSupportedFilesFilter);
			int count = filters.Count;
			for (int i = 0; i < count; i++) {
				FileDialogFilter filter = filters[i];
				if (filter.Extensions.Count > 0) {
					result.Add(filter);
					allSupportedFilesFilter.Extensions.AddRange(filter.Extensions);
				}
			}
			return result;
		}
#if !SL
		string GetFileStorage(OpenFileDialog dialog) {
			return dialog.FileName;
		}
		string GetFileName(OpenFileDialog dialog) {
			return dialog.FileName;
		}
#else
		Stream GetFileStorage(OpenFileDialog dialog) {
			return dialog.File.OpenRead();
		}
		string GetFileName(OpenFileDialog dialog) {
			return dialog.File.Name;
		}
#endif
#endif
		protected internal abstract IImporterOptions GetPredefinedOptions(TFormat format);
		protected internal abstract void ApplyEncoding(IImporterOptions options, Encoding encoding);
	}
#endregion
}

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
using DevExpress.Office.Export;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Office.Internal {
	public interface IDocumentSaveOptions<TFormat> {
		string DefaultFileName { get; set; }
		string CurrentFileName { get; set; }
		TFormat DefaultFormat { get; set; }
		TFormat CurrentFormat { get; set; }
	}
	public delegate List<IExporter<TFormat, TResult>> ExportersCalculator<TFormat, TResult>(IExportManagerService<TFormat, TResult> exportManagerService);
	#region ExportHelper<TFormat, TResult> (abstract class)
	public abstract class ExportHelper<TFormat, TResult> : ImportExportHelper {
		protected ExportHelper(IDocumentModel documentModel)
			: base(documentModel) {
		}
		public TResult Export(Stream stream, TFormat format, string targetUri, IExportManagerService<TFormat, TResult> exportManagerService) {
			return Export(stream, format, targetUri, exportManagerService, null);
		}
		public TResult Export(Stream stream, TFormat format, string targetUri, IExportManagerService<TFormat, TResult> exportManagerService, Encoding encoding) {
			IExporter<TFormat, TResult> exporter = exportManagerService.GetExporter(format);
			if (exporter == null)
				ThrowUnsupportedFormatException();
			IExporterOptions options = exporter.SetupSaving();
			IExporterOptions predefinedOptions = GetPredefinedOptions(format);
			if (predefinedOptions != null)
				options.CopyFrom(predefinedOptions);
			options.TargetUri = targetUri;
			if (encoding != null)
				ApplyEncoding(options, encoding);
			PreprocessContentBeforeExport(format);
			return exporter.SaveDocument(DocumentModel, stream, options);
		}
		public ExportTarget<TFormat, TResult> InvokeExportDialog(IWin32Window parent, IExportManagerService<TFormat, TResult> exportManagerService) {
			return InvokeExportDialog(parent, exportManagerService, null);
		}
		public ExportTarget<TFormat, TResult> InvokeExportDialog(IWin32Window parent, IExportManagerService<TFormat, TResult> exportManagerService, ExportersCalculator<TFormat, TResult> exportersCollector) {
			return InvokeExportDialog(parent, exportManagerService, exportersCollector, null);
		}
		public ExportTarget<TFormat, TResult> InvokeExportDialog(IWin32Window parent, IExportManagerService<TFormat, TResult> exportManagerService, ExportersCalculator<TFormat, TResult> exportersCollector, IDocumentSaveOptions<TFormat> options) {
#if !DXPORTABLE
			if (exportManagerService == null)
				ThrowUnsupportedFormatException();
			List<IExporter<TFormat, TResult>> exporters = !Object.ReferenceEquals(exportersCollector, null) ? exportersCollector(exportManagerService) : exportManagerService.GetExporters();
			if (exporters.Count <= 0)
				ThrowUnsupportedFormatException();
			FileDialogFilterCollection filters = CreateExportFilters(exporters);
			int currentFilterIndex = CalculateCurrentFilterIndex(exporters, options);
			SaveFileDialog dialog = CreateSaveFileDialog(filters, currentFilterIndex, options);
			if (!ShowSaveFileDialog(dialog, parent))
				return null;
			IExporter<TFormat, TResult> exporter = ChooseExporter(GetFileName(dialog), dialog.FilterIndex - 1, exporters); 
			if (exporter == null)
				ThrowUnsupportedFormatException();
			return new ExportTarget<TFormat, TResult>(GetFileStorage(dialog), exporter);
#else
			return null;
#endif
		}
#if !DXPORTABLE
		protected internal virtual bool ShowSaveFileDialog(SaveFileDialog dialog, IWin32Window parent) {
#if !SL
			return dialog.ShowDialog(parent) == DialogResult.OK;
#else
			return dialog.ShowDialog() == true;
#endif
		}
		protected internal virtual SaveFileDialog CreateSaveFileDialog(FileDialogFilterCollection filters, int currentFilterIndex) {
			return CreateSaveFileDialog(filters, currentFilterIndex, null);
		}
		internal virtual SaveFileDialog CreateSaveFileDialog(FileDialogFilterCollection filters, int currentFilterIndex, IDocumentSaveOptions<TFormat> options) {
			SaveFileDialog result = new SaveFileDialog();
			result.Filter = CreateFilterString(filters);
#if !SL
			result.RestoreDirectory = true;
			result.CheckFileExists = false;
			result.CheckPathExists = true;
			result.OverwritePrompt = true;
			result.DereferenceLinks = true;
			result.ValidateNames = true;
			result.AddExtension = false;
#endif
			result.FilterIndex = 1;
			if (filters.Count > 0 && currentFilterIndex < filters.Count) {
				result.FilterIndex = 1 + currentFilterIndex; 
				if (filters[currentFilterIndex].Extensions.Count > 0) {
					result.DefaultExt = filters[currentFilterIndex].Extensions[0];
#if !SL
					result.AddExtension = true;
#endif
				}
			}
			string fileName = options != null ? GetFileNameForSaving(options) : GetFileNameForSaving();
			string directoryName = GetDirectoryName(fileName);
			fileName = GetFileName(fileName);
			SetFileName(result, fileName);
			SetDirectoryName(result, directoryName);
			return result;
		}
#endif
		protected internal virtual string GetFileName(string fileName) {
#if !SL
			fileName = Path.GetFileNameWithoutExtension(fileName);
#endif
			return fileName;
		}
		protected internal virtual string GetDirectoryName(string fileName) {
#if !SL
			if (String.IsNullOrEmpty(fileName))
				return String.Empty;
			string directoryName = Path.GetDirectoryName(fileName);
			if(string.IsNullOrEmpty(directoryName))
				return String.Empty;
			return Path.GetFullPath(Path.GetDirectoryName(fileName));
#else
			return "";
#endif
		}
		protected internal virtual string GetFileNameForSaving(IDocumentSaveOptions<TFormat> options) {
			return GetFileNameForSaving();
		}
		protected internal abstract string GetFileNameForSaving();
		protected IExporter<TFormat, TResult> ChooseExporter(string fileName, int filterIndex, List<IExporter<TFormat, TResult>> exporters) {
			IExporter<TFormat, TResult> result = ChooseExporterByFileName(fileName, exporters);
			if (result == null)
				result = ChooseExporterByFilterIndex(filterIndex, exporters);
			return result;
		}
		protected IExporter<TFormat, TResult> ChooseExporterByFileName(string fileName, List<IExporter<TFormat, TResult>> exporters) {
			string extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
			if (String.IsNullOrEmpty(extension))
				return null;
			int count = exporters.Count;
			for (int i = 0; i < count; i++) {
				FileExtensionCollection extensions = exporters[i].Filter.Extensions;
				if (extensions.IndexOf(extension) >= 0)
					return exporters[i];
			}
			return null;
		}
		protected IExporter<TFormat, TResult> ChooseExporterByFilterIndex(int filterIndex, List<IExporter<TFormat, TResult>> exporters) {
			if (filterIndex >= 0 && filterIndex < exporters.Count)
				return exporters[filterIndex];
			else
				return null;
		}
		protected internal virtual FileDialogFilterCollection CreateExportFilters(List<IExporter<TFormat, TResult>> exporters) {
			FileDialogFilterCollection result = new FileDialogFilterCollection();
			int count = exporters.Count;
			for (int i = 0; i < count; i++) {
				FileDialogFilter filter = exporters[i].Filter;
				if (filter.Extensions.Count > 0)
					result.Add(filter);
			}
			return result;
		}
		protected internal virtual int CalculateCurrentFilterIndex(List<IExporter<TFormat, TResult>> exporters) {
			return CalculateCurrentFilterIndex(exporters, null);
		}
		internal virtual int CalculateCurrentFilterIndex(List<IExporter<TFormat, TResult>> exporters, IDocumentSaveOptions<TFormat> options) {
			TFormat documentFormat = options != null ? GetCurrentDocumentFormat(options) : GetCurrentDocumentFormat();
			int count = exporters.Count;
			for (int i = 0; i < count; i++)
				if (Object.Equals(exporters[i].Format, documentFormat))
					return i;
			return 0;
		}
		protected internal virtual TFormat GetCurrentDocumentFormat(IDocumentSaveOptions<TFormat> options) {
			return GetCurrentDocumentFormat();
		}
		protected internal abstract TFormat GetCurrentDocumentFormat();
#if !DXPORTABLE
#if !SL
		string GetFileStorage(SaveFileDialog dialog) {
			return dialog.FileName;
		}
		string GetFileName(SaveFileDialog dialog) {
			return dialog.FileName;
		}
		void SetFileName(SaveFileDialog dialog, string fileName) {
			dialog.FileName = fileName;
		}
		void SetDirectoryName(SaveFileDialog dialog, string directoryName) {
			dialog.InitialDirectory = directoryName;
		}
#else
		Stream GetFileStorage(SaveFileDialog dialog) {
			return dialog.OpenFile();
		}
		string GetFileName(SaveFileDialog dialog) {
			return dialog.SafeFileName;
		}
		void SetFileName(SaveFileDialog dialog, string fileName) {
		}
		void SetDirectoryName(SaveFileDialog dialog, string directoryName) {
		}
#endif
#endif
		protected internal abstract IExporterOptions GetPredefinedOptions(TFormat format);
		protected internal abstract void PreprocessContentBeforeExport(TFormat format);
		protected internal abstract void ApplyEncoding(IExporterOptions options, Encoding encoding);
	}
#endregion
}

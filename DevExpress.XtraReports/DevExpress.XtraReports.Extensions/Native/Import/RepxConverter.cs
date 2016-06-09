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

using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.IO;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraPrinting.Design;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Import {
	public class RepxConverter : ConverterBase {
		protected override void ConvertInternal(XtraReport report) {
			using(Stream stream = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				ConvertCore(stream, report);
			}
		}
		protected void ConvertCore(Stream source, XtraReport report) {
			ClearTargetReportBands();
			((System.ComponentModel.ISupportInitialize)report).BeginInit();
			try {
				ClearHost();
				VsComponentLoadHelper componentLoader = new VsComponentLoadHelper();
				XtraReport sourceReport = LoadReport(source, report);
				Hashtable components = sourceReport != null ? componentLoader.GetComponentsFromFields(sourceReport) :
					componentLoader.GetComponentsFromReport(report);
				foreach(IComponent component in components.Keys) {
					string name = ComponentLoadHelper.GetComponentName(component, (string)components[component], designerHost);
					designerHost.Container.Add(component, name);
				}
				CleanUpInvalidComponents();
			} finally {
				((System.ComponentModel.ISupportInitialize)report).EndInit();
			}
		}
		protected virtual XtraReport LoadReport(Stream source, XtraReport report) {
			return DevExpress.XtraReports.Native.ReportHelper.LoadReportLayout(report, source);
		}
		void CleanUpInvalidComponents() {
			IComponent[] components = new IComponent[designerHost.Container.Components.Count];
			designerHost.Container.Components.CopyTo(components, 0);
			foreach(IComponent component in components)
				if(string.IsNullOrEmpty(component.GetType().Assembly.Location))
					designerHost.Container.Remove(component);
		}
		protected override void ResetProperties() {
			fTargetReport.DataSource = null;
			fTargetReport.DataAdapter = null;
		}
		void ClearHost() {
			while(designerHost.Container.Components.Count > 1) {
				IComponent component = designerHost.Container.Components[designerHost.Container.Components.Count - 1];
				if(component != designerHost.RootComponent)
					designerHost.DestroyComponent(component);
			}
		}
	}
	public class XRImporterBase {
		public class ImportFormat {
			public string[] Extensions { get; set; }
			public string Description { get; set; }
			public Func<ConverterBase> CreateConverter { get; set; }
		}
		readonly static OpenFileDialog ofd;
		static XRImporterBase() {
			ofd = new OpenFileDialog() { Title = "Import Report" };
		}
		public bool ConvertTo(XtraReport targetReport) {
			if(targetReport == null)
				return false;
			List<ImportFormat> importFormats = new List<ImportFormat>();
			FillImportFormats(importFormats);
			ofd.Filter = CreateFilter(importFormats);
			if(DialogRunner.ShowDialog(ofd) != DialogResult.OK)
				return false;
			ImportFormat selectedFormat = null;
			string extension = Path.GetExtension(ofd.FileName);
			foreach(ImportFormat format in importFormats) {
				if(Array.Exists<string>(format.Extensions, item => item.Equals(extension, StringComparison.OrdinalIgnoreCase))) {
					selectedFormat = format;
					break;
				}
			}
			if(selectedFormat != null) {
				try {
					DevExpress.XtraPrinting.Native.CursorStorage.SetCursor(Cursors.WaitCursor);
					selectedFormat.CreateConverter().Convert(ofd.FileName, targetReport);
				} finally {
					DevExpress.XtraPrinting.Native.CursorStorage.RestoreCursor();
				}
			}
			return true;
		}
		protected virtual void FillImportFormats(IList<ImportFormat> formats) {
			formats.Add(new ImportFormat() { Extensions = new string[] { ".repx" }, Description = "DevExpress XtraReports", CreateConverter = () => new RepxConverter() });
		}
		static string CreateFilter(IList<ImportFormat> formats) {
			List<ImportFormat> importFormats = new List<ImportFormat>(formats);
			if(formats.Count > 1) {
				List<string> allExtensions = new List<string>();
				foreach(ImportFormat format in importFormats) 
					allExtensions.AddRange(format.Extensions);
				importFormats.Insert(0, new ImportFormat() { Extensions = allExtensions.ToArray(), Description = "All Supported Formats" });
			}
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < importFormats.Count; i++) {
				ImportFormat format = importFormats[i];
				sb.Append(format.Description);
				sb.Append(" (*");
				sb.Append(string.Join(", *", format.Extensions));
				sb.Append(")|*");
				sb.Append(string.Join(";*", format.Extensions));
				if(i < importFormats.Count - 1) 
					sb.Append("|");
			}
			return sb.ToString();
		}
#if DEBUGTEST
		public static string Test_CreateFilter(IList<ImportFormat> formats) {
			return CreateFilter(formats);
		}
		public IList<ImportFormat> Test_GetImportFormats() {
			List<ImportFormat> formats = new List<ImportFormat>();
			FillImportFormats(formats);
			return formats;
		}
#endif
	}
}
namespace DevExpress.XtraReports.Design {
	using DevExpress.Utils.Localization;
	using DevExpress.XtraEditors;
	public class CustomLocalizer : XtraEditors.Controls.Localizer, IDisposable {
		XtraLocalizer<XtraEditors.Controls.StringId> localizer;
		Dictionary<XtraEditors.Controls.StringId, string> strings = new Dictionary<XtraEditors.Controls.StringId, string>();
		public CustomLocalizer() {
			localizer = XtraEditors.Controls.Localizer.Active;
			XtraEditors.Controls.Localizer.Active = this;
		}
		public void SetLocalizedString(XtraEditors.Controls.StringId id, string value) {
			strings[id] = value;
		}
		public override string GetLocalizedString(XtraEditors.Controls.StringId id) {
			string value;
			return strings.TryGetValue(id, out value) ? value : localizer.GetLocalizedString(id);
		}
		void IDisposable.Dispose() {
			XtraEditors.Controls.Localizer.Active = localizer;
		}
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2.Win {
	public class XafReportVerbsManager : IDisposable {
		public const string ExportLayoutLocalizedTextItemName = "ExportLayout";
		public const string ImportLayoutLocalizedTextItemName = "ImportLayout";
		private const string defaultExt = ".repx";
		private string rootDirectory;
		private ComponentDesigner designer;
		public XafReportVerbsManager() {
			rootDirectory = PathHelper.GetApplicationFolder();
		}
		private static SaveFileDialog CreateSaveFileDialog(string fileName, string defaultDirectory) {
			SaveFileDialog fileDialog = new SaveFileDialog();
			fileDialog.Filter = DialogFilter;
			fileDialog.InitialDirectory = GetInitialDirectory(fileName, defaultDirectory);
			fileDialog.FileName = fileName;
			fileDialog.Title = CaptionHelper.GetLocalizedText("Captions", "ExportLayout");
			return fileDialog;
		}
		private static string DialogFilter {
			get {
				return String.Format("{0} (*{2})|*{3}|" +
					"{1} (*.*)|*.*",
					CaptionHelper.GetLocalizedText("Captions", "ReportFiles"),
					CaptionHelper.GetLocalizedText("Captions", "AllFiles"), defaultExt, defaultExt);
			}
		}
		private static string GetInitialDirectory(string fileName, string defaultDirectory) {
			try {
				string s = Path.GetDirectoryName(fileName);
				return !string.IsNullOrEmpty(s) ? s : defaultDirectory;
			}
			catch {
				return defaultDirectory;
			}
		}
		private DesignerVerb GetVerbByText(DesignerVerbCollection verbsCollection, string text) {
			foreach(DesignerVerb verb in verbsCollection)
				if(verb.Text == text) {
					return verb;
				}
			return null;
		}
		private void ExportLayoutVerbHandler(object sender, EventArgs e) {
			Guard.ArgumentNotNull(designer.Component, "designer.Component");
			Guard.ArgumentNotNull(designer, "designer");
			ExportLayout((XtraReport)designer.Component, designer);
		}
		private void ImportLayoutVerbHandler(object sender, EventArgs e) {
			Guard.ArgumentNotNull(designer.Component, "designer.Component");
			ImportLayout((XtraReport)designer.Component, designer);
		}
		private IWin32Window GetDesignerWindow() {
			IServiceProvider provider = designer.Component != null ? designer.Component.Site : null;
			if(provider != null) {
				System.Windows.Forms.Design.IUIService serv = provider.GetService(typeof(System.Windows.Forms.Design.IUIService)) as System.Windows.Forms.Design.IUIService;
				if(serv != null) {
					return serv.GetDialogOwnerWindow();
				}
			}
			return null;
		}
		protected string RunOpenEditor() {
			using(OpenFileDialog fileDialog = new OpenFileDialog()) {
				fileDialog.Filter = DialogFilter;
				fileDialog.InitialDirectory = rootDirectory;
				fileDialog.Title = CaptionHelper.GetLocalizedText("Captions", "ImportLayout");
				return DialogRunner.ShowDialog(fileDialog, GetDesignerWindow()) == DialogResult.OK ?
					fileDialog.FileName : string.Empty;
			}
		}
		protected string RunSaveEditor(string defaultUrl) {
			using(SaveFileDialog fileDialog = CreateSaveFileDialog(defaultUrl, rootDirectory)) {
				if(DialogRunner.ShowDialog(fileDialog, GetDesignerWindow()) == DialogResult.OK) {
					return FileHelper.SetValidExtension(fileDialog.FileName, defaultExt, new string[] { defaultExt });
				}
				return string.Empty;
			}
		}
		protected virtual void ExportLayout(XtraReport report, ComponentDesigner designer) {
			string fileName = RunSaveEditor(report.DisplayName);
			if(!string.IsNullOrEmpty(fileName)) {
				report.SaveLayout(fileName);
			}
		}
		protected virtual void ImportLayout(XtraReport report, ComponentDesigner designer) {
			string fileName = RunOpenEditor();
			if(!string.IsNullOrEmpty(fileName)) {
				CreateCustomRepxConverterEventArgs args = new CreateCustomRepxConverterEventArgs();
				if(CreateCustomRepxConverter != null) {
					CreateCustomRepxConverter(this, args);
				}
				if(args.Converter == null) {
					args.Converter = new XafRepxConverter();
				}
				args.Converter.Convert(fileName, report);
			}
		}
		public virtual void Attach(ComponentDesigner designer) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
			DesignerVerb wizardVerb = GetVerbByText(designer.Verbs, DesignSR.Verb_ReportWizard);
			designer.Verbs.Remove(wizardVerb);
			designer.Verbs.Add(new DesignerVerb(CaptionHelper.GetLocalizedText("Captions", ExportLayoutLocalizedTextItemName), new EventHandler(ExportLayoutVerbHandler)));
			designer.Verbs.Add(new DesignerVerb(CaptionHelper.GetLocalizedText("Captions", ImportLayoutLocalizedTextItemName), new EventHandler(ImportLayoutVerbHandler)));
		}
		public virtual void Dispose() {
			designer = null;
		}
		public event EventHandler<CreateCustomRepxConverterEventArgs> CreateCustomRepxConverter;
	}
}

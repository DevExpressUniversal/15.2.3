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

using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.Utils.Serializing;
using System.Runtime.InteropServices;
using System.IO.Compression;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Control;
using DevExpress.Utils;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Exports;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.XLS;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.XtraPrinting {
	public class ResFinder { }
}
namespace DevExpress.XtraPrinting {
	[
	DefaultPropertyAttribute("Links"),
	Designer("DevExpress.XtraPrinting.Design.PrintingSystemDesigner," + AssemblyInfo.SRAssemblyPrintingDesign),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "PrintingSystem.bmp"),
	Description("Encapsulates report generation."),
	ToolboxItem(false)
	]
	public class PrintingSystem : PrintingSystemBase, IDocumentSource {
		void ILink.CreateDocument() {
			if(Links.Count > 0)
				Links[0].CreateDocument();
		}
		void ILink.CreateDocument(bool buildPagesInBackground) {
			if(Links.Count > 0)
				Links[0].CreateDocument(buildPagesInBackground);
		}
		IPrintingSystem ILink.PrintingSystem {
			get { return this; }
		}
		PrintingSystemBase IDocumentSource.PrintingSystemBase {
			get { return this; }
		}
		#region Fields & Properties
		PreviewFormContainer formContainer;
		LinkCollection links;
		internal PreviewFormContainer FormContainer {
			get {
				if(formContainer == null)
					formContainer = new PreviewFormContainer(this);
				return formContainer;
			}
		}
		protected override IPrintingSystemExtenderBase CreateExtender() {
			return new PrintingSystemExtenderWin(this);
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public override DevExpress.XtraPrinting.Drawing.Watermark Watermark {
			get {
				return base.Watermark;
			}
		}
		[Category(NativeSR.CatPrinting),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraPrinting.Design.LinkItemsEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor))
		]
		public LinkCollection Links {
			get {
				if(links == null)
					links = new LinkCollection(this);
				return links;
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new XtraPageSettings PageSettings {
			get {
				return (XtraPageSettings)base.PageSettings;
			}
		}
		protected override XtraPageSettingsBase CreatePageSettings() {
			return new XtraPageSettings(this);
		}
		[Browsable(false)]
		public virtual Preview.PrintPreviewFormEx PreviewFormEx {
			get {
				return FormContainer.PreviewForm;
			}
		}
		[Browsable(false)]
		public virtual Preview.PrintPreviewRibbonFormEx PreviewRibbonFormEx {
			get {
				return FormContainer.PreviewRibbonForm;
			}
		}
		#endregion
		#region Trial
		public static void About() {
		}
		#endregion
		static PrintingSystem() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
			BrickResolver.EnsureStaticConstructor();
		}
		public PrintingSystem()
			: this(null) {
		}
		public PrintingSystem(System.ComponentModel.IContainer container)
			: base(container) {
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				StartDispose();
				if(links != null) {
					for(int i = 0; i < links.Count; i++)
						links[i].Dispose();
				}
				if(formContainer != null) {
					formContainer.Dispose();
					formContainer = null;
				}
				base.Dispose(disposing);
				EndDispose();
			} else 
				base.Dispose(disposing);
		}
		internal override void FinalizeLink(LinkBase link) {
			RemoveCommandHandler(link as ICommandHandler);
			base.FinalizeLink(link);
		}
		internal void ShowPreviewDialog(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			PreviewFormEx.ShowDialog(owner, lookAndFeel);
			PreviewFormEx.Dispose();
		}
		internal void ShowPreview(UserLookAndFeel lookAndFeel) {
			PreviewFormEx.Show(lookAndFeel);
		}
		public DialogResult PrintDlg() {
			return this.Extend().PrintDlg();
		}
		public bool PageSetup() {
			return this.Extend().ShowPageSetup(null, null);
		}
		public void Print() {
			Print(string.Empty);
		}
		public void Print(string printerName) {
			this.Extend().Print(printerName);
		}
	}
}
#if DEBUGTEST
namespace DevExpress.XtraPrinting.Tests {
	using DevExpress.XtraPrinting.InternalAccess;
	using NUnit.Framework;
	public class PrintingSystemHelper : PrintingSystemBaseHelper {
		protected override PrintingSystemBase CreatePrintingSystemCore() {
			PrintingSystemBase printingSystem = new PrintingSystem();
			printingSystem.PageSettings.AssignDefaultPageSettings();
			return printingSystem;
		}
		public static new PrintingSystem CreatePrintingSystem() {
			return (PrintingSystem)new PrintingSystemHelper().CreatePrintingSystemCore();
		}
		public void SaveIndependentPagesAndCheckVirtualDocument(PrintingSystemBase initialPrintingSystem) {
			using(MemoryStream stream = new MemoryStream()) {
				initialPrintingSystem.SaveIndependentPages(stream);
				CheckVirtualDocument(initialPrintingSystem, stream);
			}
		}
		public void CheckVirtualDocument(PrintingSystemBase initialPrintingSystem, Stream independentPagesStream) {
			HtmlExportOptions options = new HtmlExportOptions();
			options.ExportMode = HtmlExportMode.SingleFilePageByPage;
			options.ExportWatermarks = false;
			using(PrintingSystemBase onePagePrintingSystem = CreatePrintingSystemCore()) {
				for(int i = 0; i < initialPrintingSystem.Pages.Count; i++) {
					onePagePrintingSystem.LoadVirtualDocument(independentPagesStream);
					onePagePrintingSystem.Document.LoadPage(i);
					options.PageRange = (i + 1).ToString();
					using(Stream initial = new MemoryStream(), restored = new MemoryStream()) {
						initialPrintingSystem.ExportToHtml(initial, options);
						onePagePrintingSystem.ExportToHtml(restored, options);
						FileAssert.AreEqual(initial, restored, "invalid independent pages operation");
					}
				}
			}
		}
	}
}
#endif

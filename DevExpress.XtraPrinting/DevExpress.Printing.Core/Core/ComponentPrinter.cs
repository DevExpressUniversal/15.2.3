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
using System.Text;
using DevExpress.XtraPrintingLinks;
using System.Reflection;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting {
	public abstract class ComponentPrinterBase: ComponentExporter {
		protected ComponentPrinterBase(IPrintable component)
			: base(component) {
		}
		protected ComponentPrinterBase(IPrintable component, PrintingSystemBase printingSystem)
			: base(component, printingSystem) {
		}
		public abstract void Print();
		public abstract void PrintDialog();
		public abstract Form ShowPreview(object lookAndFeel);
		public abstract Form ShowPreview(IWin32Window owner, object lookAndFeel);
		public abstract Form ShowRibbonPreview(object lookAndFeel);
		public abstract Form ShowRibbonPreview(IWin32Window owner, object lookAndFeel);
		public abstract PageSettings PageSettings { get; }
		protected override PrintableComponentLinkBase CreateLink() {
			return base.CreateLink();
		}
		public static PageSettings GetDefaultPageSettings() {
			PageSettings defaultSettings = new PrintDocument().DefaultPageSettings;
			try {
				PaperSize ignore = defaultSettings.PaperSize;
			} catch {
				defaultSettings.PaperSize = CreateLetterPaperSize();
				defaultSettings.Margins = XtraPageSettingsBase.DefaultMargins;
			}
			return defaultSettings;
		}
		static PaperSize CreateLetterPaperSize() {
			return new PaperSize("Custom", 850, 1100); 
		}
		public static bool IsPrintingAvailable(bool throwException) {
			return ComponentPrinterDynamic.IsPrintingAvailable_(throwException);
		}
	}
	public class ComponentPrinterDynamic: ComponentPrinterBase {
		class SingletonContainer {
			static SingletonContainer() {
			}
			static System.Reflection.Assembly printingAssembly = DevExpress.Data.Utils.AssemblyCache.LoadDXAssembly(AssemblyInfo.SRAssemblyPrinting);
			public static System.Reflection.Assembly GetPrintingAssembly(bool throwException) {
				if(throwException && printingAssembly == null)
					throw new Exception(AssemblyInfo.SRAssemblyPrinting + " isn't found.");
				return printingAssembly;
			}
		}
		protected override PrintableComponentLinkBase CreateLink() {
			Type type = GetType("DevExpress.XtraPrinting.PrintableComponentLink", false);
			return type != null ? (PrintableComponentLinkBase)Activator.CreateInstance(type) : base.CreateLink();
		}
		static Type GetType(string typeName, bool throwException) {
			System.Reflection.Assembly printingAssembly = SingletonContainer.GetPrintingAssembly(false);
			if(printingAssembly != null)
				return printingAssembly.GetType(typeName);
			return GetTypeOfficially(typeName, throwException);
		}
		static Type GetTypeOfficially(string typeName, bool throwException) {
			return Type.GetType(string.Format("{0}, {1}", typeName, AssemblyInfo.SRAssemblyPrinting), throwException);
		}
		internal static bool IsPrintingAvailable_(bool throwException) {
			return SingletonContainer.GetPrintingAssembly(throwException) != null;
		}
		public override PageSettings PageSettings {
			get {
				System.Reflection.Assembly printingAssembly = SingletonContainer.GetPrintingAssembly(true);
				Type type = printingAssembly.GetType("DevExpress.XtraPrinting.PrintingSystemBaseExtentions");
				IPrintingSystemExtenderBase extender = type.InvokeMember("Extend", 
					BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
					null, null, new object[] { this.PrintingSystemBase }) as IPrintingSystemExtenderBase;
				return extender.PageSettings;
			}
		}
		public ComponentPrinterDynamic(IPrintable component)
			: base(component) {
		}
		public ComponentPrinterDynamic(IPrintable component, PrintingSystemBase printingSystem)
			: base(component, printingSystem) {
		}
		public override void Print() {
			InvokeLinkMethod("Print", new object[] { "" });
		}
		public override void PrintDialog() {
			InvokeLinkMethod("PrintDlg", new object[] { });
		}
		public override Form ShowPreview(object lookAndFeel) {
			return ShowPreviewCore("ShowPreview", "PreviewForm", lookAndFeel);
		}
		public override Form ShowPreview(IWin32Window owner, object lookAndFeel) {
			return ShowPreviewCore("ShowPreview", "PreviewForm", owner, lookAndFeel);
		}
		public override Form ShowRibbonPreview(object lookAndFeel) {
			return ShowPreviewCore("ShowRibbonPreview", "PreviewRibbonForm", lookAndFeel);
		}
		public override Form ShowRibbonPreview(IWin32Window owner, object lookAndFeel) {
			return ShowPreviewCore("ShowRibbonPreview", "PreviewRibbonForm", owner, lookAndFeel);
		}
		Form ShowPreviewCore(string method, string form, object lookAndFeel) {
			object tool = CreatePrintTool();
			tool.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, tool, new object[] { lookAndFeel });
			return tool.GetType().InvokeMember(form, BindingFlags.GetProperty, null, tool, null) as Form;
		}
		Form ShowPreviewCore(string method, string form, IWin32Window owner, object lookAndFeel) {
			object tool = CreatePrintTool();
			tool.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, tool, new object[] { owner, lookAndFeel });
			return tool.GetType().InvokeMember(form, BindingFlags.GetProperty, null, tool, null) as Form;
		}
		object CreatePrintTool() {
			Type type = GetType("DevExpress.XtraPrinting.Links.LinkPrintTool", true);
			return Activator.CreateInstance(type, this.LinkBase);
		}
		void InvokeLinkMethod(string memberName, object[] args) {
			if(IsPrintingAvailable(true))
				LinkBase.GetType().InvokeMember(memberName, BindingFlags.InvokeMethod, null, LinkBase, args);
		}
	}
	public class PrintToolBase {
		PrintingSystemBase printingSystem;
		public PrinterSettings PrinterSettings {
			get {
				return PrintingSystem.Extender.PrinterSettings;
			}
		}
		public PrintingSystemBase PrintingSystem {
			get {
				return printingSystem;
			}
			protected set {
				printingSystem = value;
				ExtendPrintingSystem(printingSystem);
			}
		}
		public PrintToolBase(PrintingSystemBase printingSystem) {
			PrintingSystem = printingSystem;
		}
		protected virtual void ExtendPrintingSystem(PrintingSystemBase printingSystem) {
			if(!(printingSystem.Extender is PrintingSystemExtenderPrint))
				printingSystem.Extender = new PrintingSystemExtenderPrint(printingSystem);
		}
		public void Print() {
			Print(string.Empty);
		}
		public void Print(string printerName) {
			if(printingSystem != null) {
				BeforePrint();
				printingSystem.Extender.Print(printerName);
			}
		}
		protected virtual void BeforePrint() {
		}
	}
}

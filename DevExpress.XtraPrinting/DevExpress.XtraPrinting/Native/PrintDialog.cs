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
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Globalization;
namespace DevExpress.XtraPrinting.Native {
	[System.ComponentModel.ToolboxItem(false)]
	public sealed class PrintDialog : CommonDialog {
		#region Native
		delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
		interface PRINTDLG {
			int Flags { get; set; }
			IntPtr hDC { get; set; }
			IntPtr hDevMode { get; set; }
			IntPtr hDevNames { get; set; }
			IntPtr hInstance { get; set; }
			IntPtr hPrintTemplate { get; set; }
			IntPtr hSetupTemplate { get; set; }
			IntPtr hwndOwner { get; set; }
			IntPtr lCustData { get; set; }
			WndProc lpfnPrintHook { get; set; }
			WndProc lpfnSetupHook { get; set; }
			string lpPrintTemplateName { get; set; }
			string lpSetupTemplateName { get; set; }
			int lStructSize { get; set; }
			short nCopies { get; set; }
			short nFromPage { get; set; }
			short nMaxPage { get; set; }
			short nMinPage { get; set; }
			short nToPage { get; set; }
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		class PRINTDLGEX {
			public int lStructSize;
			public IntPtr hwndOwner;
			public IntPtr hDevMode;
			public IntPtr hDevNames;
			public IntPtr hDC;
			public int Flags;
			public int Flags2;
			public int ExclusionFlags;
			public int nPageRanges;
			public int nMaxPageRanges;
			public IntPtr pageRanges;
			public int nMinPage;
			public int nMaxPage;
			public int nCopies;
			public IntPtr hInstance;
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpPrintTemplateName;
			public WndProc lpCallback;
			public int nPropertyPages;
			public IntPtr lphPropertyPages;
			public int nStartPage;
			public int dwResultAction;
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		class PRINTDLG_32 : PRINTDLG {
			private int m_lStructSize;
			private IntPtr m_hwndOwner;
			private IntPtr m_hDevMode;
			private IntPtr m_hDevNames;
			private IntPtr m_hDC;
			private int m_Flags;
			private short m_nFromPage;
			private short m_nToPage;
			private short m_nMinPage;
			private short m_nMaxPage;
			private short m_nCopies;
			private IntPtr m_hInstance;
			private IntPtr m_lCustData;
			private WndProc m_lpfnPrintHook;
			private WndProc m_lpfnSetupHook;
			private string m_lpPrintTemplateName;
			private string m_lpSetupTemplateName;
			private IntPtr m_hPrintTemplate;
			private IntPtr m_hSetupTemplate;
			public int lStructSize {
				get { return m_lStructSize; }
				set { m_lStructSize = value; }
			}
			public IntPtr hwndOwner {
				get { return m_hwndOwner; }
				set { m_hwndOwner = value; }
			}
			public IntPtr hDevMode {
				get { return m_hDevMode; }
				set { m_hDevMode = value; }
			}
			public IntPtr hDevNames {
				get { return m_hDevNames; }
				set { m_hDevNames = value; }
			}
			public IntPtr hDC {
				get { return m_hDC; }
				set { m_hDC = value; }
			}
			public int Flags {
				get { return m_Flags; }
				set { m_Flags = value; }
			}
			public short nFromPage {
				get { return m_nFromPage; }
				set { m_nFromPage = value; }
			}
			public short nToPage {
				get { return m_nToPage; }
				set { m_nToPage = value; }
			}
			public short nMinPage {
				get { return m_nMinPage; }
				set { m_nMinPage = value; }
			}
			public short nMaxPage {
				get { return m_nMaxPage; }
				set { m_nMaxPage = value; }
			}
			public short nCopies {
				get { return m_nCopies; }
				set { m_nCopies = value; }
			}
			public IntPtr hInstance {
				get { return m_hInstance; }
				set { m_hInstance = value; }
			}
			public IntPtr lCustData {
				get { return m_lCustData; }
				set { m_lCustData = value; }
			}
			public WndProc lpfnPrintHook {
				get { return m_lpfnPrintHook; }
				set { m_lpfnPrintHook = value; }
			}
			public WndProc lpfnSetupHook {
				get { return m_lpfnSetupHook; }
				set { m_lpfnSetupHook = value; }
			}
			public string lpPrintTemplateName {
				get { return m_lpPrintTemplateName; }
				set { m_lpPrintTemplateName = value; }
			}
			public string lpSetupTemplateName {
				get { return m_lpSetupTemplateName; }
				set { m_lpSetupTemplateName = value; }
			}
			public IntPtr hPrintTemplate {
				get { return m_hPrintTemplate; }
				set { m_hPrintTemplate = value; }
			}
			public IntPtr hSetupTemplate {
				get { return m_hSetupTemplate; }
				set { m_hSetupTemplate = value; }
			}
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		class PRINTDLG_64 : PRINTDLG {
			private int m_lStructSize;
			private IntPtr m_hwndOwner;
			private IntPtr m_hDevMode;
			private IntPtr m_hDevNames;
			private IntPtr m_hDC;
			private int m_Flags;
			private short m_nFromPage;
			private short m_nToPage;
			private short m_nMinPage;
			private short m_nMaxPage;
			private short m_nCopies;
			private IntPtr m_hInstance;
			private IntPtr m_lCustData;
			private WndProc m_lpfnPrintHook;
			private WndProc m_lpfnSetupHook;
			private string m_lpPrintTemplateName;
			private string m_lpSetupTemplateName;
			private IntPtr m_hPrintTemplate;
			private IntPtr m_hSetupTemplate;
			public int lStructSize {
				get { return m_lStructSize; }
				set { m_lStructSize = value; }
			}
			public IntPtr hwndOwner {
				get { return m_hwndOwner; }
				set { m_hwndOwner = value; }
			}
			public IntPtr hDevMode {
				get { return m_hDevMode; }
				set { m_hDevMode = value; }
			}
			public IntPtr hDevNames {
				get { return m_hDevNames; }
				set { m_hDevNames = value; }
			}
			public IntPtr hDC {
				get { return m_hDC; }
				set { m_hDC = value; }
			}
			public int Flags {
				get { return m_Flags; }
				set { m_Flags = value; }
			}
			public short nFromPage {
				get { return m_nFromPage; }
				set { m_nFromPage = value; }
			}
			public short nToPage {
				get { return m_nToPage; }
				set { m_nToPage = value; }
			}
			public short nMinPage {
				get { return m_nMinPage; }
				set { m_nMinPage = value; }
			}
			public short nMaxPage {
				get { return m_nMaxPage; }
				set { m_nMaxPage = value; }
			}
			public short nCopies {
				get { return m_nCopies; }
				set { m_nCopies = value; }
			}
			public IntPtr hInstance {
				get { return m_hInstance; }
				set { m_hInstance = value; }
			}
			public IntPtr lCustData {
				get { return m_lCustData; }
				set { m_lCustData = value; }
			}
			public WndProc lpfnPrintHook {
				get { return m_lpfnPrintHook; }
				set { m_lpfnPrintHook = value; }
			}
			public WndProc lpfnSetupHook {
				get { return m_lpfnSetupHook; }
				set { m_lpfnSetupHook = value; }
			}
			public string lpPrintTemplateName {
				get { return m_lpPrintTemplateName; }
				set { m_lpPrintTemplateName = value; }
			}
			public string lpSetupTemplateName {
				get { return m_lpSetupTemplateName; }
				set { m_lpSetupTemplateName = value; }
			}
			public IntPtr hPrintTemplate {
				get { return m_hPrintTemplate; }
				set { m_hPrintTemplate = value; }
			}
			public IntPtr hSetupTemplate {
				get { return m_hSetupTemplate; }
				set { m_hSetupTemplate = value; }
			}
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
		public class PRINTPAGERANGE {
			public int nFromPage;
			public int nToPage;
		}
		const int GMEM_ZEROINIT = 0x40;
		[System.Security.SecuritySafeCritical]
		static IntPtr GlobalAlloc(int uFlags, int dwBytes) { return GlobalAlloc_(uFlags, dwBytes); }
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "GlobalAlloc")]
		static extern IntPtr GlobalAlloc_(int uFlags, int dwBytes);
		[System.Security.SecuritySafeCritical]
		static IntPtr GlobalFree(HandleRef handle) { return GlobalFree_(handle); }
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "GlobalFree")]
		static extern IntPtr GlobalFree_(HandleRef handle);
		[System.Security.SecuritySafeCritical]
		static int PrintDlgEx([In, Out] PRINTDLGEX lppdex) { return PrintDlgEx_(lppdex); }
		[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "PrintDlgEx")]
		static extern int PrintDlgEx_([In, Out] PRINTDLGEX lppdex);
		[System.Security.SecuritySafeCritical]
		static bool PrintDlg_32([In, Out] PRINTDLG_32 lppd) { return PrintDlg_32_(lppd); }
		[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "PrintDlg")]
		static extern bool PrintDlg_32_([In, Out] PRINTDLG_32 lppd);
		[System.Security.SecuritySafeCritical]
		static bool PrintDlg_64([In, Out] PRINTDLG_64 lppd) { return PrintDlg_64_(lppd); }
		[DllImport("comdlg32.dll", CharSet = CharSet.Auto, SetLastError = true, EntryPoint = "PrintDlg")]
		static extern bool PrintDlg_64_([In, Out] PRINTDLG_64 lppd);
		static bool PrintDlg([In, Out] PRINTDLG lppd) {
			if(IntPtr.Size == 4) {
				PRINTDLG_32 printdlg_32 = lppd as PRINTDLG_32;
				if(printdlg_32 == null) {
					throw new NullReferenceException("PRINTDLG data is null");
				}
				return PrintDlg_32(printdlg_32);
			}
			PRINTDLG_64 printdlg_64 = lppd as PRINTDLG_64;
			if(printdlg_64 == null) {
				throw new NullReferenceException("PRINTDLG data is null");
			}
			return PrintDlg_64(printdlg_64);
		}
		#endregion
		const int printRangeMask = 0x400003;
		const int START_PAGE_GENERAL = -1;
		const int PD_ALLPAGES = 0;
		const int PD_COLLATE = 0x10;
		const int PD_CURRENTPAGE = 0x400000;
		const int PD_DISABLEPRINTTOFILE = 0x80000;
		const int PD_ENABLEPRINTHOOK = 0x1000;
		const int PD_ENABLEPRINTTEMPLATE = 0x4000;
		const int PD_ENABLEPRINTTEMPLATEHANDLE = 0x10000;
		const int PD_ENABLESETUPHOOK = 0x2000;
		const int PD_ENABLESETUPTEMPLATE = 0x8000;
		const int PD_ENABLESETUPTEMPLATEHANDLE = 0x20000;
		const int PD_EXCLUSIONFLAGS = 0x1000000;
		const int PD_HIDEPRINTTOFILE = 0x100000;
		const int PD_NOCURRENTPAGE = 0x800000;
		const int PD_NONETWORKBUTTON = 0x200000;
		const int PD_NOPAGENUMS = 8;
		const int PD_NOSELECTION = 4;
		const int PD_NOWARNING = 0x80;
		const int PD_PAGENUMS = 2;
		const int PD_PRINTSETUP = 0x40;
		const int PD_PRINTTOFILE = 0x20;
		const int PD_RETURNDC = 0x100;
		const int PD_RETURNDEFAULT = 0x400;
		const int PD_RETURNIC = 0x200;
		const int PD_SELECTION = 1;
		const int PD_SHOWHELP = 0x800;
		const int PD_USEDEVMODECOPIES = 0x40000;
		const int PD_USEDEVMODECOPIESANDCOLLATE = 0x40000;
		const int PD_USELARGETEMPLATE = 0x10000000;
		const int PD_RESULT_APPLY = 2;
		const int PD_RESULT_CANCEL = 0;
		const int PD_RESULT_PRINT = 1;
		static PermissionSet allPrintingAndUnmanagedCode = null;
		static PrintingPermission safePrinting = null;
		static System.Resources.ResourceManager resources = null;
		bool allowCurrentPage;
		bool allowPages;
		bool allowPrintToFile;
		bool allowSelection;
		PrintDocument printDocument;
		bool printToFile;
		PrinterSettings settings;
		bool showHelp;
		bool showNetwork;
		bool useEXDialog;
		public bool AllowCurrentPage {
			get { return allowCurrentPage; }
			set { allowCurrentPage = value; }
		}
		public bool AllowPrintToFile {
			get { return allowPrintToFile; }
			set { allowPrintToFile = value; }
		}
		public bool AllowSelection {
			get { return allowSelection; }
			set { allowSelection = value; }
		}
		public bool AllowSomePages {
			get { return allowPages; }
			set { allowPages = value; }
		}
		public PrintDocument Document {
			get { return printDocument; }
			set {
				printDocument = value;
				if(printDocument == null)
					settings = new PrinterSettings();
				else
					settings = printDocument.PrinterSettings;
			}
		}
		PageSettings PageSettings {
			get {
				if(Document == null)
					return PrinterSettings.DefaultPageSettings;
				return Document.DefaultPageSettings;
			}
		}
		public PrinterSettings PrinterSettings {
			get {
				if(settings == null) 
					settings = new PrinterSettings();
				return settings;
			}
			set {
				if(value != PrinterSettings) {
					settings = value;
					printDocument = null;
				}
			}
		}
		public bool PrintToFile {
			get { return printToFile; }
			set { printToFile = value; }
		}
		public bool ShowHelp {
			get { return showHelp; }
			set { showHelp = value; }
		}
		public bool ShowNetwork {
			get { return showNetwork; }
			set { showNetwork = value; }
		}
		public bool UseEXDialog {
			get { return useEXDialog; }
			set { useEXDialog = value; }
		}
		bool SuppressExDialog {
			get { return (!UseEXDialog || (Environment.OSVersion.Platform != PlatformID.Win32NT)) || (Environment.OSVersion.Version.Major < 5); }
		}
		public PrintDialog() {
			Reset();
		}
		static PRINTDLG CreatePRINTDLG() {
			PRINTDLG structure = null;
			if(IntPtr.Size == 4)
				structure = new PRINTDLG_32();
			else
				structure = new PRINTDLG_64();
			structure.lStructSize = Marshal.SizeOf(structure);
			structure.hwndOwner = IntPtr.Zero;
			structure.hDevMode = IntPtr.Zero;
			structure.hDevNames = IntPtr.Zero;
			structure.Flags = 0;
			structure.hDC = IntPtr.Zero;
			structure.nFromPage = 1;
			structure.nToPage = 1;
			structure.nMinPage = 0;
			structure.nMaxPage = 624;
			structure.nCopies = 1;
			structure.hInstance = IntPtr.Zero;
			structure.lCustData = IntPtr.Zero;
			structure.lpfnPrintHook = null;
			structure.lpfnSetupHook = null;
			structure.lpPrintTemplateName = null;
			structure.lpSetupTemplateName = null;
			structure.hPrintTemplate = IntPtr.Zero;
			structure.hSetupTemplate = IntPtr.Zero;
			return structure;
		}
		static PRINTDLGEX CreatePRINTDLGEX() {
			PRINTDLGEX structure = new PRINTDLGEX();
			structure.lStructSize = Marshal.SizeOf(structure);
			structure.hwndOwner = IntPtr.Zero;
			structure.hDevMode = IntPtr.Zero;
			structure.hDevNames = IntPtr.Zero;
			structure.hDC = IntPtr.Zero;
			structure.Flags = 0;
			structure.Flags2 = 0;
			structure.ExclusionFlags = 0;
			structure.nPageRanges = 0;
			structure.nMaxPageRanges = 1;
			structure.pageRanges = GlobalAlloc(GMEM_ZEROINIT, structure.nMaxPageRanges * Marshal.SizeOf(typeof(PRINTPAGERANGE)));
			structure.nMinPage = 0;
			structure.nMaxPage = 624;
			structure.nCopies = 1;
			structure.hInstance = IntPtr.Zero;
			structure.lpPrintTemplateName = null;
			structure.nPropertyPages = 0;
			structure.lphPropertyPages = IntPtr.Zero;
			structure.nStartPage = START_PAGE_GENERAL;
			structure.dwResultAction = 0;
			return structure;
		}
		int GetFlags() {
			int num = 0;
			if(SuppressExDialog)
				num |= PD_ENABLEPRINTHOOK;
			if(!allowCurrentPage)
				num |= PD_NOCURRENTPAGE;
			if(!allowPages)
				num |= PD_NOPAGENUMS;
			if(!allowPrintToFile)
				num |= PD_DISABLEPRINTTOFILE;
			if(!allowSelection)
				num |= PD_NOSELECTION;
			num |= (int)PrinterSettings.PrintRange;
			if(printToFile)
				num |= PD_PRINTTOFILE;
			if(showHelp)
				num |= PD_SHOWHELP;
			if(!showNetwork)
				num |= PD_NONETWORKBUTTON;
			if(PrinterSettings.Collate)
				num |= PD_COLLATE;
			return num;
		}
		public override void Reset() {
			allowCurrentPage = false;
			allowPages = false;
			allowPrintToFile = true;
			allowSelection = false;
			printDocument = null;
			printToFile = false;
			settings = null;
			showHelp = false;
			showNetwork = true;
		}
		protected override bool RunDialog(IntPtr hwndOwner) {
			SafePrinting.Demand();
			WndProc hookProcPtr = new WndProc(HookProc);
			if(SuppressExDialog) {
				PRINTDLG printdlg = CreatePRINTDLG();
				return ShowPrintDialog(hwndOwner, hookProcPtr, printdlg);
			}
			PRINTDLGEX data = CreatePRINTDLGEX();
			return ShowPrintDialog(hwndOwner, data);
		}
		[SecuritySafeCritical]
		bool ShowPrintDialog(IntPtr hwndOwner, PRINTDLGEX data) {
			bool result;
			data.Flags = GetFlags();
			data.nCopies = PrinterSettings.Copies;
			data.hwndOwner = hwndOwner;
			AllPrintingAndUnmanagedCode.Assert();
			try {
				if(PageSettings == null) {
					data.hDevMode = PrinterSettings.GetHdevmode();
				} else {
					data.hDevMode = PrinterSettings.GetHdevmode(PageSettings);
				}
				data.hDevNames = PrinterSettings.GetHdevnames();
			} catch(InvalidPrinterException) {
				data.hDevMode = IntPtr.Zero;
				data.hDevNames = IntPtr.Zero;
			} finally {
				CodeAccessPermission.RevertAssert();
			}
			try {
				if(AllowSomePages) {
					if((PrinterSettings.FromPage < PrinterSettings.MinimumPage) || (PrinterSettings.FromPage > PrinterSettings.MaximumPage))
						throw new ArgumentException(SRGetString("PDpageOutOfRange", "FromPage"));
					if((PrinterSettings.ToPage < PrinterSettings.MinimumPage) || (PrinterSettings.ToPage > PrinterSettings.MaximumPage))
						throw new ArgumentException(SRGetString("PDpageOutOfRange", "ToPage"));
					if(PrinterSettings.ToPage < PrinterSettings.FromPage)
						throw new ArgumentException(SRGetString("PDpageOutOfRange", "FromPage"));
					PRINTPAGERANGE pageRange = new PRINTPAGERANGE();
					pageRange.nFromPage = PrinterSettings.FromPage;
					pageRange.nToPage = PrinterSettings.ToPage;
					Marshal.StructureToPtr(pageRange, data.pageRanges, false);
					data.nPageRanges = 1;
					data.nMinPage = PrinterSettings.MinimumPage;
					data.nMaxPage = PrinterSettings.MaximumPage;
				}
				data.Flags &= ~(PD_NONETWORKBUTTON | PD_SHOWHELP);
				if(PrintDlgEx(data) < 0 || (data.dwResultAction == 0)) {
					return false;
				}
				AllPrintingAndUnmanagedCode.Assert();
				try {
					UpdatePrinterSettings(data.hDevMode, data.hDevNames, (short)data.nCopies, data.Flags, PrinterSettings, PageSettings);
				} finally {
					CodeAccessPermission.RevertAssert();
				}
				PrintToFile = (data.Flags & PD_PRINTTOFILE) != 0;
				PrinterSettings.PrintToFile = PrintToFile;
				if(AllowSomePages) {
					PRINTPAGERANGE pageRange = new PRINTPAGERANGE();
					Marshal.PtrToStructure(data.pageRanges, pageRange);
					PrinterSettings.FromPage = pageRange.nFromPage;
					PrinterSettings.ToPage = pageRange.nToPage;
				}
				if(((data.Flags & PD_USEDEVMODECOPIES) == 0) && (Environment.OSVersion.Version.Major >= 6)) {
					PrinterSettings.Copies = (short)data.nCopies;
					PrinterSettings.Collate = (data.Flags & PD_COLLATE) == PD_COLLATE;
				}
				result = data.dwResultAction == PD_RESULT_PRINT;
			} finally {
				if(data.hDevMode != IntPtr.Zero) {
					GlobalFree(new HandleRef(data, data.hDevMode));
				}
				if(data.hDevNames != IntPtr.Zero) {
					GlobalFree(new HandleRef(data, data.hDevNames));
				}
				if(data.pageRanges != IntPtr.Zero) {
					GlobalFree(new HandleRef(data, data.pageRanges));
				}
			}
			return result;
		}
		bool ShowPrintDialog(IntPtr hwndOwner, WndProc hookProcPtr, PRINTDLG data) {
			bool result;
			data.Flags = GetFlags();
			data.nCopies = PrinterSettings.Copies;
			data.hwndOwner = hwndOwner;
			data.lpfnPrintHook = hookProcPtr;
			AllPrintingAndUnmanagedCode.Assert();
			try {
				if(PageSettings == null) {
					data.hDevMode = PrinterSettings.GetHdevmode();
				} else {
					data.hDevMode = PrinterSettings.GetHdevmode(PageSettings);
				}
				data.hDevNames = PrinterSettings.GetHdevnames();
			} catch(InvalidPrinterException) {
				data.hDevMode = IntPtr.Zero;
				data.hDevNames = IntPtr.Zero;
			} finally {
				CodeAccessPermission.RevertAssert();
			}
			try {
				if(AllowSomePages) {
					if((PrinterSettings.FromPage < PrinterSettings.MinimumPage) || (PrinterSettings.FromPage > PrinterSettings.MaximumPage))
						throw new ArgumentException(SRGetString("PDpageOutOfRange", "FromPage"));
					if((PrinterSettings.ToPage < PrinterSettings.MinimumPage) || (PrinterSettings.ToPage > PrinterSettings.MaximumPage))
						throw new ArgumentException(SRGetString("PDpageOutOfRange", "ToPage"));
					if(PrinterSettings.ToPage < PrinterSettings.FromPage)
						throw new ArgumentException(SRGetString("PDpageOutOfRange", "FromPage"));
					data.nFromPage = (short)PrinterSettings.FromPage;
					data.nToPage = (short)PrinterSettings.ToPage;
					data.nMinPage = (short)PrinterSettings.MinimumPage;
					data.nMaxPage = (short)PrinterSettings.MaximumPage;
				}
				if(!PrintDlg(data))
					return false;
				AllPrintingAndUnmanagedCode.Assert();
				try {
					UpdatePrinterSettings(data.hDevMode, data.hDevNames, data.nCopies, data.Flags, settings, PageSettings);
				} finally {
					CodeAccessPermission.RevertAssert();
				}
				PrintToFile = (data.Flags & PD_PRINTTOFILE) != 0;
				PrinterSettings.PrintToFile = PrintToFile;
				if(AllowSomePages) {
					PrinterSettings.FromPage = data.nFromPage;
					PrinterSettings.ToPage = data.nToPage;
				}
				if(((data.Flags & PD_USEDEVMODECOPIES) == 0) && (Environment.OSVersion.Version.Major >= 6)) {
					PrinterSettings.Copies = data.nCopies;
					PrinterSettings.Collate = (data.Flags & PD_COLLATE) == PD_COLLATE;
				}
				result = true;
			} finally {
				GlobalFree(new HandleRef(data, data.hDevMode));
				GlobalFree(new HandleRef(data, data.hDevNames));
			}
			return result;
		}
		static void UpdatePrinterSettings(IntPtr hDevMode, IntPtr hDevNames, short copies, int flags, PrinterSettings settings, PageSettings pageSettings) {
			settings.SetHdevmode(hDevMode);
			settings.SetHdevnames(hDevNames);
			if(pageSettings != null)
				pageSettings.SetHdevmode(hDevMode);
			if(settings.Copies == 1)
				settings.Copies = copies;
			settings.PrintRange = ((PrintRange)flags) & (PrintRange.CurrentPage | PrintRange.SomePages | PrintRange.Selection);
		}
		string SRGetString(string name, params object[] args) {
			if(resources == null)
				resources = new System.Resources.ResourceManager("System.Windows.Forms", typeof(System.Windows.Forms.Control).Assembly);
			string format = resources.GetString(name);
			if((args == null) || (args.Length <= 0))
				return format;
			return string.Format(CultureInfo.CurrentCulture, format, args);
		}
		static PermissionSet AllPrintingAndUnmanagedCode {
			get {
				if(allPrintingAndUnmanagedCode == null) {
					PermissionSet set = new PermissionSet(PermissionState.None);
					set.SetPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
					set.SetPermission(new PrintingPermission(PrintingPermissionLevel.AllPrinting));
					allPrintingAndUnmanagedCode = set;
				}
				return allPrintingAndUnmanagedCode;
			}
		}
		public static CodeAccessPermission SafePrinting {
			get {
				if(safePrinting == null) 
					safePrinting = new PrintingPermission(PrintingPermissionLevel.SafePrinting);
				return safePrinting;
			}
		}
	}
}

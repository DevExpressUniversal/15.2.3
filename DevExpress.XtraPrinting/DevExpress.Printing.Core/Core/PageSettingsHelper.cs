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
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	public class PageSettingsHelper {
		#region static
		static PageSettingsHelper instance;
		public static PageSettingsHelper Instance {
			get {
				if(instance == null)
					instance = new PageSettingsHelper();
				return instance;
			}
			set {
				instance = value;
			}
		}
		public static PageSettings DefaultPageSettings {
			get {
				return Instance.DefaultSettings;
			}
		}
		public static PaperSize CreateLetterPaperSize() {
			return new PaperSize("Custom", 850, 1100); 
		}
		[System.Security.SecuritySafeCritical]
		public static Margins GetMinMargins(PageSettings pageSettings) {
			IntPtr hdc = CreateDC(pageSettings.PrinterSettings.PrinterName);
			if(hdc != IntPtr.Zero) {
				try {
					using(Graphics gr = Graphics.FromHdc(hdc)) {
						Margins minMargins = DeviceCaps.GetMinMargins(gr);
						return pageSettings.Landscape ? ConvertToLandscape(minMargins) : minMargins;
					}
				} finally {
					Win32.DeleteDC(hdc);
				}
			}
			return XtraPageSettingsBase.DefaultMinMargins;
		}
		[System.Security.SecuritySafeCritical]
		static IntPtr CreateDC(string printerName) {
			try {
				if(!string.IsNullOrEmpty(printerName))
					return Win32.CreateDC(null, printerName, IntPtr.Zero, IntPtr.Zero);
			} catch(AccessViolationException) {
			}
			return IntPtr.Zero;
		}
		static Margins ConvertToLandscape(Margins margins) {
			return new Margins(margins.Top, margins.Bottom, margins.Right, margins.Left);
		}
		public static void SetDefaultPageSettings(PrinterSettings printerSettings, PageSettings pageSettings) {
			typeof(PrinterSettings).InvokeMember("defaultPageSettings",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField, null,
				printerSettings, new object[] { pageSettings });
		}
		public static bool PrinterExists {
			get {
				try {
					return PrinterSettings.InstalledPrinters.Count > 0;
				} catch { return false; }
			}
		}
		public static void ChangePageSettings(PageSettings pageSettings, PaperSize paperSize, ReadonlyPageData pageData) {
			try {
				if(paperSize != null)
					pageSettings.PaperSize = paperSize;
				pageSettings.Landscape = pageData.Landscape;
				pageSettings.Margins = pageData.Margins;
			} catch { }
		}
		public static void SetPrinterName(PrinterSettings sets, string printerName) {
			foreach(string item in PrinterSettings.InstalledPrinters) {
				if(Object.Equals(item, printerName)) {
					sets.PrinterName = printerName;
					return;
				}
			}
		}
		#endregion
		PageSettings defaultSettings;
		public virtual PageSettings DefaultSettings {
			get {
				if(defaultSettings == null) {
					defaultSettings = new PrintDocument().DefaultPageSettings;
					try {
						PaperSize ignore = defaultSettings.PaperSize;
					} catch {
						defaultSettings.PaperSize = CreateLetterPaperSize();
						defaultSettings.Margins = XtraPageSettingsBase.DefaultMargins;
					}
				}
				return defaultSettings;
			}
		}
	}
}

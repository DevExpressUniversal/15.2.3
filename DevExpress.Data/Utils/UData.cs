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

#if SILVERLIGHT
extern alias Platform;
#endif
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;
namespace DevExpress.Utils.About {
#if DXCommon
	public enum ProductInfoStage { Beta, Trial, Registered, Free };
#else
	internal enum ProductInfoStage { Beta, Trial, Registered, Free };
#endif
	[Flags]
	public enum ProductKind : long {
		Default =			   0L,
		DXperienceWin = 1L << 0,
		XtraReports = 1L << 4,
		DemoWin =			   1L << 13, 
		XPO = 1L << 15,
		DXperienceASP =		 1L << 25,
		XAF =				   1L << 28,
		DXperienceWPF =		 1L << 38,
		DXperienceSliverlight = 1L << 39,
		LightSwitchReports =	1L << 45,
		Dashboard =		 1L << 47,
		CodedUIWin =			   1L << 48,
		Snap =				  1L << 49,
		Docs = 1L << 55,
		XtraReportsWpf = 1L << 57,
		XtraReportsSL = 1L << 58,
		XtraReportsWeb = 1L << 59,
		XtraReportsWin = 1L << 60,
		FreeOffer = 1L << 62, 
		DXperiencePro = DXperienceWin | XtraReports | Snap,
		DXperienceEnt = DXperiencePro | XPO | DXperienceASP | DXperienceWPF | DXperienceSliverlight | Docs,
		DXperienceUni = DXperienceEnt | XAF | DXperienceWPF | DXperienceSliverlight| Dashboard,
	};
	public class ProductStringInfoWin : ProductStringInfo {
		public ProductStringInfoWin(string productName)
			: base(ProductInfoHelper.PlatformWinForms, productName) {
		}
	}
	public class ProductStringInfo {
		string name, platform;
		public ProductStringInfo(string name) : this("", name) {}
		public ProductStringInfo(string platform, string name) {
			this.name = name;
			this.platform = platform;
		}
		public string ProductName { get {return name;} }
		public string ProductPlatform { get {return platform;} }
	}
	public class ProductInfoHelper {
		public static ProductStringInfo GetProductInfo(ProductKind kind) {
			if(LocalizationHelper.IsJapanese) return new ProductStringInfo("XtraGrid for WinForms", "Japanese Edition");
			string dx = "DXperience";
			string dxu = "DXperience Universal";
			switch (kind)
			{
				case ProductKind.DemoWin: return new ProductStringInfo(dx, "Demo for Windows Forms");
				case ProductKind.DXperienceSliverlight: return new ProductStringInfo("Silverlight Subscription", "DXperience Silverlight");
				case ProductKind.DXperienceWPF: return new ProductStringInfo("WPF Subscription", "DXperience WPF");
				case ProductKind.DXperienceWin: return new ProductStringInfo("WinForms Subscription", "DXperience WinForms");
				case ProductKind.Dashboard: return new ProductStringInfo("Universal Subscription", "DevExpress Dashboard");
				case ProductKind.CodedUIWin: return new ProductStringInfo(dxu, "Coded UI Support for WinForms Controls");
				case ProductKind.DXperienceASP: return new ProductStringInfo(dx, "DXperience ASP.NET");
				case ProductKind.LightSwitchReports: return new ProductStringInfo(dx, "Reporting for LightSwitch");
				case ProductKind.Snap: return new ProductStringInfo(dx, "Snap by DevExpress");
				case ProductKind.XPO: return new ProductStringInfo(dx, "eXpress Persistent Objects");
				case ProductKind.XAF: return new ProductStringInfo(dxu, "eXpressApp Framework");				
				case ProductKind.XtraReports: return new ProductStringInfo(dx, "Cross-Platform Reporting Solution");
				case ProductKind.DXperienceEnt: return new ProductStringInfo(dx, "enterprise");
				case ProductKind.DXperiencePro: return new ProductStringInfo(dx, "professional");
				case ProductKind.DXperienceUni: return new ProductStringInfo(dx, "universal");
				case ProductKind.Docs: return new ProductStringInfo(dxu, "Document Server");
				case ProductKind.FreeOffer: return new ProductStringInfo(dx, "Free Offer");
			}
			return new ProductStringInfo(string.Format("{0}", kind));
		}
		public const string PlatformFreeOffer = "Free Controls";
		public const string PlatformWinForms = "WinForms Subscription";
		public const string PlatformDashboard = "DevExpress Dashboard";
		public const string PlatformUniversal = "Universal Subscription";
		public const string WinGrid = "The XtraGrid Suite";
		public const string WinEditors = "The XtraEditors Suite";
		public const string WinDiagram = "The XtraDiagram Suite";
		public const string WinVGrid = "The XtraVerticalGrid Suite";
		public const string WinRichEdit = "The XtraRichEdit Suite";
		public const string WinSpellChecker = "The XtraSpellChecker";
		public const string WinScheduler = "The XtraScheduler Suite";
		public const string WinSnap = "Snap Reports";
		public const string WinCharts = "The XtraCharts Suite";
		public const string WinTreeList = "The XtraTreeList Suite";
		public const string WinPivotGrid = "The XtraPivotGrid Suite";
		public const string WinXtraBars = "The XtraBars Suite";
		public const string WinMaps = "The XtraMaps Suite";
		public const string WinSpreadsheet = "The XtraSpreadsheet Suite";
		public const string WinNavBar = "The XtraNavBar Suite";
		public const string WinLayoutControl = "The XtraLayout Suite";
		public const string WinPrinting = "The XtraPrinting Library";
		public const string WinGauge = "The XtraGauge Suite";
		public const string WinPdfViewer = "PDF Viewer";
		public const string WinWizard = "XtraWizard";
		public const string WinReports = "Reporting Solution";
		public const string WinXPO = "eXpress Persistent Objects";
		public const string WinMVVM = "Application Infrastructural MVVM Solution";
	}
}
namespace DevExpress.Utils {
	public class LocalizationHelper {
		const string ciFlag = "ci:";
		public static void SetCurrentCulture(string[] arguments) {
			string info = null;
			foreach(string line in arguments)
				if(line.IndexOf(ciFlag) == 0) info = line.Substring(ciFlag.Length);
			if(string.IsNullOrEmpty(info)) {
				System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
				System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
			} else {
				CultureInfo ci = null;
				if(info.StartsWith("ja")) ci = new CultureInfo(1041);
				if(info.StartsWith("ru")) ci = new CultureInfo(1049);
				if(ci != null) {
					System.Threading.Thread.CurrentThread.CurrentCulture = ci;
					System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
				}
			}
		}
		public static bool IsJapanese { get { return System.Threading.Thread.CurrentThread.CurrentCulture.LCID == 1041; } }
	}
}

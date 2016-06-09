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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Utils.About;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using Microsoft.Win32;
namespace DevExpress.Web.Design {
	public class AboutDialogHelperBase {
		public const string SubscriptionProductName = "DevExpress ASP.NET Subscription";
		public const string FreeProductName = "DevExpress Free Controls";
		protected static bool IsTrial(Type type) {
			return LicenseUtils.IsTrial(type);
		}
		protected static bool IsExpired(Type type) {
			return LicenseUtils.IsExpired(type);
		}
		protected static string GetSerialNumber(Type type, ProductKind[] kinds) {
			string sn = "";
			return sn;
		}
		protected static void ShowAboutForm(IServiceProvider provider, Type type, ProductKind kind) {
			ShowAboutForm(provider, type, new ProductKind[] { kind });
		}
		protected static void ShowAboutForm(IServiceProvider provider, Type type, ProductKind[] kinds) {
			ShowAboutControlWindow(type, kinds, provider);
		}
		static Xpf.LicenseState GetLicenseState(Type type) {
			if(!IsTrial(type))
				return Xpf.LicenseState.Licensed;
			else if(IsExpired(type))
				return Xpf.LicenseState.TrialExpired;
			return Xpf.LicenseState.Trial;
		}
		static string GerVersion() {
			Version ver = new Version(AssemblyInfo.Version);
			return string.Format("{0}.{1}", AssemblyInfo.MarketingVersion, ver.Build);
		}
		static string GetProductName(Type type, ProductKind[] kinds) {
			return !IsTrial(type) && new List<ProductKind>(kinds).Contains(ProductKind.FreeOffer) ? FreeProductName : SubscriptionProductName;
		}
		public static bool? ShowAboutControlWindow(Type type, ProductKind[] kinds, IServiceProvider provider) {
			var aif = new DevExpress.Xpf.AboutInfo() {
				RegistrationCode = GetSerialNumber(type, kinds),
				ProductName = GetProductName(type, kinds),
				LicenseState = GetLicenseState(type),
				Version = GerVersion()
			};
			AboutWindow aWindow = new AboutWindow() {
				Content = new DevExpress.Xpf.ControlAbout(aif)
			};
			return DesignUtils.ShowDialog(provider, aWindow);
		}
		public static bool? ShowAboutToolWindow(string productName, string descrLine1, string descrLine2, string eulaLine, string eulaUri, Form owner) {
			var aif = new DevExpress.Xpf.AboutToolInfo() {
				LicenseState = Xpf.LicenseState.Licensed,
				ProductDescriptionLine1 = descrLine1,
				ProductDescriptionLine2 = descrLine2,
				ProductName = productName,
				ProductEULA = eulaLine,
				ProductEULAUri = eulaUri,
				Version = GerVersion()
			};
			var aWindow = new AboutWindow() {
				Content = new DevExpress.Xpf.ToolAbout(aif)
			};
			if(owner != null) {
				aWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
				var wih = new System.Windows.Interop.WindowInteropHelper(aWindow);
				wih.Owner = owner.Handle;
				return aWindow.ShowDialog();
			}
			return aWindow.ShowDialog();
		}
		public static bool ShouldShowTrialAbout(Type type) {
			return false;
		}
	}
	public class AboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxWebControl), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxWebControl))) 
				ShowAbout(provider);
		}
		public static void ShowMenuAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(DevExpress.Web.ASPxMenuBase), ProductKind.DXperienceASP);
		}
		public static void ShowMenuTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(DevExpress.Web.ASPxMenuBase))) 
				ShowMenuAbout(provider);
		}
		public static void ShowSiteMapAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(DevExpress.Web.ASPxSiteMapControlBase), ProductKind.DXperienceASP);
		}
		public static void ShowSiteMapTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(DevExpress.Web.ASPxSiteMapControlBase))) 
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.Design {
	public class EditorsAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxEditBase), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxEditBase))) 
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.Design {
	public class GridViewAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxGridView), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxGridView))) 
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.ASPxSpellChecker.Design {
	public sealed class SpellCheckerAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxSpellChecker), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxSpellChecker))) 
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class HtmlEditorAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxHtmlEditor), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxHtmlEditor))) 
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.ASPxTreeList.Design {
	public class TreeListAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxTreeList), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxTreeList)))
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.ASPxSpreadsheet.Design {
	public class SpreadsheetAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxSpreadsheet), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxSpreadsheet)))
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.ASPxRichEdit.Design {
	public class RichEditAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxRichEdit), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxRichEdit)))
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class PivotGridAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxPivotGrid), ProductKind.DXperienceASP);
		}
		public static void ShowASPxPivotCustomizationControlAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxPivotCustomizationControl), ProductKind.DXperienceASP);
		}
		static void ShowTrialAbout(Type type, IServiceProvider provider) {
			if(ShouldShowTrialAbout(type)) 
				ShowAbout(provider);
		}
		public static void ShowASPxPivotGridTrialAbout(IServiceProvider provider) {
			ShowTrialAbout(typeof(ASPxPivotGrid), provider);
		}
		public static void ShowASPxPivotCustomizationControlTrialAbout(IServiceProvider provider) {
			ShowTrialAbout(typeof(ASPxPivotCustomizationControl), provider);
		}
	}
}
namespace DevExpress.Web.ASPxGauges.Design {
	public class GaugeControlAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxGaugeControl), ProductKind.DXperienceASP);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if(ShouldShowTrialAbout(typeof(ASPxGaugeControl))) 
				ShowAbout(provider);
		}
	}
}
namespace DevExpress.XtraReports.Web.Design {
	using DevExpress.XtraReports.UI;
	public class XtraReportAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(XtraReport), ProductKind.XtraReports);
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
#if !DEBUG
			if(ShouldShowTrialAbout(typeof(XtraReport))) 
				ShowAbout(provider);
#endif
		}
	}
}

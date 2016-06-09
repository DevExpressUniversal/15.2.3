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
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using System.Resources;
using System.Globalization;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraWizard.Localization {
	[ToolboxItem(false)]
	public class WizardLocalizer : XtraLocalizer<WizardStringId> {
		static WizardLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<WizardStringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<WizardStringId> Active { 
			get { return XtraLocalizer<WizardStringId>.Active; }
			set { XtraLocalizer<WizardStringId>.Active = value; }
		}
		public override XtraLocalizer<WizardStringId> CreateResXLocalizer() {
			return new WizardResLocalizer();
		}
		public static XtraLocalizer<WizardStringId> CreateDefaultLocalizer() { return new WizardResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(WizardStringId.CancelText, "Cancel");
			AddString(WizardStringId.FinishText, "&Finish");
			AddString(WizardStringId.HelpText, "&Help");
			AddString(WizardStringId.NextText, "&Next >");
			AddString(WizardStringId.PreviousText, "< &Back");
			AddString(WizardStringId.WelcomePageTitleText, "Welcome to the sample wizard");
			AddString(WizardStringId.WelcomePageProceedText, "To continue, click Next");
			AddString(WizardStringId.WelcomePageIntroductionText, "This wizard guides you through several steps");
			AddString(WizardStringId.CompletionPageTitleText, "Completing the sample wizard");
			AddString(WizardStringId.CompletionPageProceedText, "To close this wizard, click Finish");
			AddString(WizardStringId.CompletionPageFinishText, "You have successfully completed the sample wizard");
			AddString(WizardStringId.InteriorPageTitleText, "Wizard Page");
			AddString(WizardStringId.PageDescriptionText, "Here is a desciption of this page");
			AddString(WizardStringId.WizardTitle, "Wizard Title");
			AddString(WizardStringId.CaptionError, "Error");
		}
		#endregion
	}
	public class WizardResLocalizer : WizardLocalizer {
		ResourceManager manager = null;
		public WizardResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraWizard.LocalizationRes", typeof(WizardResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		public override string GetLocalizedString(WizardStringId id) {
			string resStr = "WizardStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = "";
			return ret;
		}
	}
	#region enum WizardStringId
	public enum WizardStringId {
		CancelText,
		FinishText,
		HelpText,
		NextText,
		PreviousText,
		WelcomePageProceedText,
		CompletionPageProceedText,
		WelcomePageTitleText,
		CompletionPageTitleText,
		WelcomePageIntroductionText,
		CompletionPageFinishText,
		InteriorPageTitleText,
		PageDescriptionText,
		WizardTitle,
		CaptionError
	}
	#endregion
}

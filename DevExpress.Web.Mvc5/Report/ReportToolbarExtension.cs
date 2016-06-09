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

using System.Web.Mvc;
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class ReportToolbarExtension : ExtensionBase {
		protected internal new MVCxReportToolbar Control {
			get { return (MVCxReportToolbar)base.Control; }
		}
		public new ReportToolbarSettings Settings {
			get { return (ReportToolbarSettings)base.Settings; }
		}
		public ReportToolbarExtension(SettingsBase settings)
			: base(settings) {
		}
		public ReportToolbarExtension(SettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxReportToolbar();
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.SetReportViewerClientID(Settings.ReportViewerName);
			Control.ItemSpacing = Settings.ItemSpacing;
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.CaptionSettings.Assign(Settings.CaptionSettings);
			Control.MenuStyle.CopyFrom(Settings.MenuStyle);
			Control.Images.CopyFrom(Settings.Images);
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.ShowDefaultButtons = Settings.ShowDefaultButtons;
			Control.Items.Assign(Settings.Items);
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
		}
		protected internal override void DisposeControl() {
		}
	}
}

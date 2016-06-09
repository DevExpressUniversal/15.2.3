#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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

using System.ComponentModel;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarEditorCaptionSettings : EditorCaptionSettings {
		const string
			ShowColonName = "ShowColon",
			RequiredMarkDisplayModeName = "RequiredMarkDisplayMode";
		const bool ShowColonDefault = false;
		const EditorRequiredMarkMode RequiredMarkDisplayModeDefault = EditorRequiredMarkMode.Hidden;
		public ReportToolbarEditorCaptionSettings(IPropertiesOwner owner)
			: base(owner) {
			base.ShowColon = ShowColonDefault;
			base.RequiredMarkDisplayMode = RequiredMarkDisplayModeDefault;
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarEditorCaptionSettingsShowColon")]
#endif
		[DefaultValue(ShowColonDefault)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		public new bool ShowColon {
			get { return GetBoolProperty(ShowColonName, ShowColonDefault); }
			set { SetBoolProperty(ShowColonName, ShowColonDefault, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarEditorCaptionSettingsRequiredMarkDisplayMode")]
#endif
		[DefaultValue(RequiredMarkDisplayModeDefault)]
		[NotifyParentProperty(true)]
		[AutoFormatEnable]
		public new EditorRequiredMarkMode RequiredMarkDisplayMode {
			get { return (EditorRequiredMarkMode)GetEnumProperty(RequiredMarkDisplayModeName, RequiredMarkDisplayModeDefault); }
			set { SetEnumProperty(RequiredMarkDisplayModeName, RequiredMarkDisplayModeDefault, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as ReportToolbarEditorCaptionSettings;
			if(src != null) {
				ShowColon = src.ShowColon;
				RequiredMarkDisplayMode = src.RequiredMarkDisplayMode;
			}
		}
	}
}

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
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum ProgressBarDisplayMode { 
		Percentage = 0, 
		Position = 1, 
		Custom = 2
	}
	public class ProgressBarSettings : PropertiesBase, IPropertiesOwner {
		public const string DefaultDisplayFormatString = "0.##";
		public ProgressBarSettings()
			: base() {
		}
		public ProgressBarSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal ASPxProgressBarBase ProgressBar {
			get { return Owner as ASPxProgressBarBase; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarSettingsDisplayFormatString"),
#endif
		DefaultValue(DefaultDisplayFormatString), NotifyParentProperty(true), AutoFormatDisable]
		public string DisplayFormatString {
			get { return GetStringProperty("DisplayFormatString", DefaultDisplayFormatString); }
			set {
				SetStringProperty("DisplayFormatString", DefaultDisplayFormatString, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarSettingsDisplayMode"),
#endif
		DefaultValue(ProgressBarDisplayMode.Percentage), NotifyParentProperty(true), AutoFormatDisable]
		public ProgressBarDisplayMode DisplayMode {
			get { return (ProgressBarDisplayMode)GetEnumProperty("DisplayMode", ProgressBarDisplayMode.Percentage); }
			set {
				SetEnumProperty("DisplayMode", ProgressBarDisplayMode.Percentage, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarSettingsShowPosition"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable,
		Localizable(true)]
		public bool ShowPosition {
			get { return GetBoolProperty("ShowPosition", true); }
			set {
				SetBoolProperty("ShowPosition", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is ProgressBarSettings) {
					ProgressBarSettings settings = source as ProgressBarSettings;
					DisplayFormatString = settings.DisplayFormatString;
					DisplayMode = settings.DisplayMode;
					ShowPosition = settings.ShowPosition;
				}
			} finally {
				EndUpdate();
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
}

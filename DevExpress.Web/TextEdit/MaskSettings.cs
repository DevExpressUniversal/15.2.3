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
using System.ComponentModel;
using System.Text;
using DevExpress.Web;
namespace DevExpress.Web {
	public enum MaskIncludeLiteralsMode {
		All = 1,
		None = 2,
		DecimalSymbol = 3
	}
	public class MaskSettings: PropertiesBase {
		public const char DefaultPromptChar = '_';
		bool isDateTimeOnly = false;
		public MaskSettings(IPropertiesOwner owner)
			: base(owner) {			
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MaskSettingsAllowMouseWheel"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool AllowMouseWheel {
			get { return GetBoolProperty("AllowMouseWheel", true); }
			set { SetBoolProperty("AllowMouseWheel", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MaskSettingsMask"),
#endif
		DefaultValue(""), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.MaskExpressionUITypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Mask {
			get { return GetStringProperty("Mask", ""); }
			set {
				if(value != Mask) {
					SetStringProperty("Mask", "", value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MaskSettingsPromptChar"),
#endif
		DefaultValue(DefaultPromptChar), NotifyParentProperty(true)]
		public char PromptChar {
			get { return (char)GetObjectProperty("PromptChar", DefaultPromptChar); }
			set {
				if(value == 0)
					value = DefaultPromptChar;
				SetObjectProperty("PromptChar", DefaultPromptChar, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MaskSettingsIncludeLiterals"),
#endif
		DefaultValue(MaskIncludeLiteralsMode.All), NotifyParentProperty(true)]
		public MaskIncludeLiteralsMode IncludeLiterals {
			get { return (MaskIncludeLiteralsMode)GetEnumProperty("IncludeLiterals", MaskIncludeLiteralsMode.All); }
			set { SetEnumProperty("IncludeLiterals", MaskIncludeLiteralsMode.All, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MaskSettingsErrorText"),
#endif
		DefaultValue(""), NotifyParentProperty(true)]
		public string ErrorText {
			get { return GetStringProperty("ErrorText", ""); }
			set { SetStringProperty("ErrorText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MaskSettingsShowHints"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowHints {
			get { return GetBoolProperty("ShowHints", false); }
			set { SetBoolProperty("ShowHints", false, value); }
		}
		protected internal bool IsDateTimeOnly {
			get { return isDateTimeOnly; }
			set { isDateTimeOnly = value; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			MaskSettings srcMaskSettings = source as MaskSettings;
			if(srcMaskSettings != null) {
				AllowMouseWheel = srcMaskSettings.AllowMouseWheel;
				Mask = srcMaskSettings.Mask;
				ErrorText = srcMaskSettings.ErrorText;
				PromptChar = srcMaskSettings.PromptChar;
				IncludeLiterals = srcMaskSettings.IncludeLiterals;
				ShowHints = srcMaskSettings.ShowHints;
			}
		}
		public override string ToString() {
			return string.Empty;
		}
	}
}

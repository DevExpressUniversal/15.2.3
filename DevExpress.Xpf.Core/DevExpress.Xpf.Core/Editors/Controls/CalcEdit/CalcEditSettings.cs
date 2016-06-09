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

using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Markup;
using System;
#if !SL
using DevExpress.Xpf.Editors.Settings.Extension;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors.Settings {
	public class CalcEditSettings : PopupBaseEditSettings {
		public static readonly DependencyProperty IsPopupAutoWidthProperty;
		public static readonly DependencyProperty PrecisionProperty;
		static CalcEditSettings() {
			Type ownerType = typeof(CalcEditSettings);
			IsPopupAutoWidthProperty = DependencyPropertyManager.Register("IsPopupAutoWidth", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrecisionProperty = DependencyPropertyManager.Register("Precision", typeof(int), ownerType, new FrameworkPropertyMetadata(6));
		}
		public bool IsPopupAutoWidth {
			get { return (bool)GetValue(IsPopupAutoWidthProperty); }
			set { SetValue(IsPopupAutoWidthProperty, value); }
		}
		public int Precision {
			get { return (int)GetValue(PrecisionProperty); }
			set { SetValue(PrecisionProperty, value); }
		}
		protected override void AssignToEditCore(IBaseEdit edit) {
			base.AssignToEditCore(edit);
			PopupCalcEdit calc = edit as PopupCalcEdit;
			if (calc == null)
				return;
			SetValueFromSettings(IsPopupAutoWidthProperty, () => calc.IsPopupAutoWidth = IsPopupAutoWidth);
			SetValueFromSettings(PrecisionProperty, () => calc.Precision = Precision);
		}
	}
}
#if !SL
	namespace DevExpress.Xpf.Editors.Settings.Extension {
		public class CalcEditSettingsExtension : PopupBaseSettingsExtension {
			public bool IsPopupAutoWidth { get; set; }
			public int Precision { get; set; }
			public CalcEditSettingsExtension() {
				IsPopupAutoWidth = (bool)PopupCalcEdit.IsPopupAutoWidthProperty.DefaultMetadata.DefaultValue;
				Precision = (int)PopupCalcEdit.PrecisionProperty.DefaultMetadata.DefaultValue;
			}
			protected override void Assign(PopupBaseEditSettings settings) {
				base.Assign(settings);
				((CalcEditSettings)settings).IsPopupAutoWidth = IsPopupAutoWidth;
				((CalcEditSettings)settings).Precision = Precision;
			}
			protected override PopupBaseEditSettings CreatePopupBaseEditSettings() {
				CalcEditSettings calcSettings = new CalcEditSettings();
				Assign(calcSettings);
				return calcSettings;
			}
		}
	}
#endif

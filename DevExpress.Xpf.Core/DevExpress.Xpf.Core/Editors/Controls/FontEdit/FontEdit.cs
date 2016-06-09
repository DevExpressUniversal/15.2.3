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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	public class FontEdit : ComboBoxEdit {
		#region static
		public static readonly DependencyProperty FontProperty;
		public static readonly DependencyProperty AllowConfirmFontUseDialogProperty;
		static FontEdit() {
			Type ownerType = typeof(FontEdit);
			FontProperty = DependencyPropertyManager.Register("Font", typeof(FontFamily), ownerType, new FrameworkPropertyMetadata(null, (obj, e) => ((FontEdit)obj).OnFontChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue), (obj, baseValue) => { return ((FontEdit)obj).CoerceFont((FontFamily)baseValue); }));
			AllowConfirmFontUseDialogProperty = DependencyPropertyManager.Register("AllowConfirmFontUseDialog", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
#if !SL
			AutoCompleteProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(true));
			ValidateOnTextInputProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
#endif
		}
		#endregion
		#region ctor
		public FontEdit() {
			this.SetDefaultStyleKey(typeof(FontEdit));
		}
		#endregion
		#region properties
		public FontFamily Font {
			get { return (FontFamily)GetValue(FontProperty); }
			set { SetValue(FontProperty, value); }
		}
		public bool AllowConfirmFontUseDialog {
			get { return (bool)GetValue(AllowConfirmFontUseDialogProperty); }
			set { SetValue(AllowConfirmFontUseDialogProperty, value); }
		}
		protected new FontEditStrategy EditStrategy {
			get { return base.EditStrategy as FontEditStrategy; }
			set { base.EditStrategy = value; }
		}
		#endregion
		#region methods
		protected override EditStrategyBase CreateEditStrategy() {
			return new FontEditStrategy(this);
		}
		protected internal new FontEditSettings Settings { get { return base.Settings as FontEditSettings; } }
		protected virtual void OnFontChanged(FontFamily oldFont, FontFamily newFont) {
			EditStrategy.FontChanged(oldFont, newFont);
		}
		object CoerceFont(FontFamily baseValue) {
			return EditStrategy.CoerceFont(baseValue);
		}
		protected override void OnLoadedInternal() {
			base.OnLoadedInternal();
			SetItemsSource();
		}
		void SetItemsSource() {
			if (ItemsSource == null && GetBindingExpression(ItemsSourceProperty) == null)
				ItemsSource = FontEditSettings.CachedFonts;
		}
		#endregion
	}
}

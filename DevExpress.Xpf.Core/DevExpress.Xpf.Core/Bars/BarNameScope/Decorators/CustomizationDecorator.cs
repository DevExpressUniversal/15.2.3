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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Bars.Native {
	public interface ICustomizationService {
		BarManagerCustomizationHelper CustomizationHelper { get; }
		bool IsCustomizationMode { get; set; }		
		bool IsPreparedToQuickCustomizationMode { get; set; }
		bool ShowCustomizationMenu(object target);
		void HideCustomizationMenu(object target);
		void ShowCustomizationForm();		
		void Select(BarItemLinkControl lControl);
		void CloseCustomizationForm();
		void HideCustomizationForm();
		void RestoreCustomizationForm();
	}
	class CustomizationService : ICustomizationService {
		CustomizationDecorator decorator;
		public CustomizationService(CustomizationDecorator decorator) {
			this.decorator = decorator;
		}
		bool ICustomizationService.IsPreparedToQuickCustomizationMode {
			get { return decorator.Return(x => x.CustomizationHelper.IsPreparedToQuickCustomizationMode, () => false); }
			set { decorator.Do(x => x.CustomizationHelper.IsPreparedToQuickCustomizationMode = value); }
		}
		bool ICustomizationService.IsCustomizationMode {
			get { return decorator.Return(x => x.CustomizationHelper.IsCustomizationMode, () => false); }
			set { if (decorator == null) return; decorator.CustomizationHelper.IsCustomizationMode = value; }
		}
		void ICustomizationService.ShowCustomizationForm() {
			if (decorator == null) return;
			decorator.CustomizationHelper.ShowCustomizationForm();
		}
		void ICustomizationService.CloseCustomizationForm() {
			if (decorator == null) return;
			decorator.CustomizationHelper.CloseCustomizationForm();
		}
		void ICustomizationService.HideCustomizationMenu(object target) {
			if (decorator == null) return;
			if (target == null) {
				decorator.CustomizationHelper.HideCustomizationMenus();
				return;
			}
			var bControl = target as BarControl;
			if (bControl != null) {
				if (bControl.Bar.Return(x => x.AllowCustomizationMenu, () => false)) {
					decorator.CustomizationHelper.HideToolbarsCustomizationMenu(bControl);
					return;
				}					
			}
			var tButton = target as BarQuickCustomizationButton;
			if (tButton != null) {
				if (decorator.CustomizationHelper.IsCustomizationMode) {
					decorator.CustomizationHelper.HideCustomizationMenu();
					return;
				}
				bControl = LayoutHelper.FindParentObject<BarControl>(tButton);
				decorator.CustomizationHelper.HideCustomizationMenu(tButton);
				return;
			}
			var iControl = target as BarItemLinkControl;
			if (iControl != null) {
				decorator.CustomizationHelper.HideItemCustomizationMenu(iControl);
			}
		}
		bool ICustomizationService.ShowCustomizationMenu(object target) {
			if (decorator == null) return false;
			var bControl = target as BarControl;
			if (bControl != null) {
				if (bControl.Bar.Return(x => x.AllowCustomizationMenu, () => false))
					return decorator.CustomizationHelper.ShowToolbarsCustomizationMenu(bControl);
			}
			var tButton = target as BarQuickCustomizationButton;
			if (tButton != null) {
				if (decorator.CustomizationHelper.IsCustomizationMode)
					return false;
				bControl = LayoutHelper.FindParentObject<BarControl>(tButton);
				return decorator.CustomizationHelper.ShowCustomizationMenu(bControl.With(x => x.Bar), tButton);
			}
			var iControl = target as BarItemLinkControl;
			if (iControl != null) {
				return decorator.CustomizationHelper.ShowItemCustomizationMenu(iControl);
			}
			return false;
		}
		bool customizationFormHidden = false;
		void ICustomizationService.HideCustomizationForm() {
			if (decorator == null) return;
			customizationFormHidden = true;
			decorator.CustomizationHelper.CustomizationForm.Do(x=>x.IsOpen = false);
		}
		void ICustomizationService.RestoreCustomizationForm() {
			if (decorator == null || !decorator.CustomizationHelper.IsCustomizationMode || !customizationFormHidden) return;
			customizationFormHidden = false;
			decorator.CustomizationHelper.CustomizationForm.Do(x=>x.IsOpen = true);
		}
		void ICustomizationService.Select(BarItemLinkControl lControl) {
			if (decorator == null || !decorator.CustomizationHelper.IsCustomizationMode || BarManagerCustomizationHelper.IsInCustomizationMenu(lControl)) return;
			decorator.CustomizationHelper.SelectedLinkControl = lControl;
		}
		BarManagerCustomizationHelper ICustomizationService.CustomizationHelper {
			get { return decorator.With(x => x.CustomizationHelper); }
		}
	}
	public class CustomizationDecorator : IBarNameScopeDecorator {
		static CustomizationDecorator() { }
		BarNameScope scope;
		BarManagerCustomizationHelper customizationHelper;
		public BarManagerCustomizationHelper CustomizationHelper { get { return customizationHelper ?? (customizationHelper = CreateCustomizationHelper()); } }
		protected virtual BarManagerCustomizationHelper CreateCustomizationHelper() {
			return new BarManagerCustomizationHelper();
		}
		public void TODO_SetAllowCustomization(bool newValue) {
			CustomizationHelper.AllowCustomization = newValue;
		}
		#region IBarNameScopeDecorator Members
		void IBarNameScopeDecorator.Attach(BarNameScope scope) {
			this.scope = scope;
			CustomizationHelper.Scope = scope;
		}
		void IBarNameScopeDecorator.Detach() {
			this.scope = null;
			CustomizationHelper.CloseCustomizationForm();
			CustomizationHelper.Scope = null;
		}
		#endregion
	}
}

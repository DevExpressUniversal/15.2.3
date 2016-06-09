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
using System.Windows;
using DevExpress.Xpf.Docking.Base;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class OptionsPanel : psvControl {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty AllowInvisbleItemsInternalProperty;
		static OptionsPanel() {
			var dProp = new DependencyPropertyRegistrator<OptionsPanel>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("AllowInvisbleItemsInternal", ref AllowInvisbleItemsInternalProperty, true,
				(dObj, ea) => ((OptionsPanel)dObj).OnAllowInvisibleItemsInternalChanged((bool)ea.NewValue));
		}
		#endregion static
		protected DevExpress.Xpf.Editors.CheckEdit PartCheckShowAll { get; private set; }
		public OptionsPanel() {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartCheckShowAll = GetTemplateChild("PART_CheckShowAll") as DevExpress.Xpf.Editors.CheckEdit;
			if(PartCheckShowAll != null) {
				PartCheckShowAll.Content = DockingLocalizer.GetString(DockingStringId.CheckBoxShowInvisibleItems);
				if(Container != null) {
					BindingHelper.SetBinding(PartCheckShowAll, Editors.CheckEdit.IsCheckedProperty, Container, DockLayoutManager.ShowInvisibleItemsProperty, System.Windows.Data.BindingMode.TwoWay);
					BindingHelper.SetBinding(this, AllowInvisbleItemsInternalProperty, Container, "ShowInvisibleItemsInCustomizationForm");
				}
			}
			Focusable = false;
		}
		protected override void OnDispose() {
			BindingHelper.ClearBinding(this, AllowInvisbleItemsInternalProperty);
			if(PartCheckShowAll != null)
				BindingHelper.ClearBinding(PartCheckShowAll, Editors.CheckEdit.IsCheckedProperty);
			base.OnDispose();
		}
		public void OnAllowInvisibleItemsInternalChanged(bool value) {
			if(Container != null) {
				PartCheckShowAll.Visibility = VisibilityHelper.Convert(value);
				if(value)
					BindingHelper.SetBinding(PartCheckShowAll, Editors.CheckEdit.IsCheckedProperty, Container, DockLayoutManager.ShowInvisibleItemsProperty, System.Windows.Data.BindingMode.TwoWay);
				else
					BindingHelper.ClearBinding(PartCheckShowAll, Editors.CheckEdit.IsCheckedProperty);
			}
		}
	}
}

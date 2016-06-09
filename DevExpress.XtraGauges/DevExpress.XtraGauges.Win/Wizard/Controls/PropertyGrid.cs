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
using System.Windows.Forms;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Wizard {
	public enum DesignerPropertyGridMode {
		PropertyView,
		DataBindingView
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class DesignerPropertyGrid : DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx {
		IComponent serviceProviderCore;
		GaugeDesignerControl designerControlCore;
		MenuItem menuItemReset;
		ContextMenu propertyGridContextMenu;
		Dictionary<Type, BasePropertyGridObjectWrapper> wrappersCore = null;
		DesignerPropertyGridMode modeCore;
		public DesignerPropertyGrid(DesignerPropertyGridMode mode)
			: base() {
			InitializeContextMenu();
			this.modeCore = mode;
			this.wrappersCore = new Dictionary<Type, BasePropertyGridObjectWrapper>();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				designerControlCore = null;
				if(Wrappers != null) {
					Wrappers.Clear();
					wrappersCore = null;
				}
			}
			base.Dispose(disposing);
		}
		internal void SetDesignerControl(GaugeDesignerControl designer) {
			this.designerControlCore = designer;
			this.serviceProviderCore = FindDTServiceProvider();
		}
		protected DesignerPropertyGridMode Mode {
			get { return modeCore; }
		}
		protected Dictionary<Type, BasePropertyGridObjectWrapper> Wrappers {
			get { return wrappersCore; }
		}
		protected GaugeDesignerControl DesignerControl {
			get { return designerControlCore; }
		}
		public IComponent ServiceProvider {
			get { return serviceProviderCore; }
		}
		protected override object GetService(Type serviceType) {
			object service = null;
			if((ServiceProvider != null) && (ServiceProvider.Site != null)) 
				service = ServiceProvider.Site.GetService(serviceType);
			return (service != null) ? service : base.GetService(serviceType);
		}
		public void SetSelectedPrimitive(object primitive) {
			SelectedObject = GetObjectWrapper(primitive);
		}
		protected BasePropertyGridObjectWrapper GetObjectWrapper(object obj) {
			if(obj == null || !(obj is ISupportPropertyGridWrapper)) return null;
			return (Mode == DesignerPropertyGridMode.PropertyView) ?
				GetPropertyViewObjectWrapperCore(obj as ISupportPropertyGridWrapper) :
				GetDataBindingViewObjectWrapperCore(obj as ISupportPropertyGridWrapper);
		}
		BasePropertyGridObjectWrapper GetDataBindingViewObjectWrapperCore(ISupportPropertyGridWrapper obj) {
			Type wrapperType = typeof(BindableObjectPropertyGridWrapper);
			return CreateObjectWrapper(obj, wrapperType);
		}
		BasePropertyGridObjectWrapper GetPropertyViewObjectWrapperCore(ISupportPropertyGridWrapper obj) {
			Type wrapperType = obj.PropertyGridWrapperType;
			return CreateObjectWrapper(obj, wrapperType);
		}
		BasePropertyGridObjectWrapper CreateObjectWrapper(ISupportPropertyGridWrapper obj, Type wrapperType) {
			BasePropertyGridObjectWrapper wrapper = null;
			if(Wrappers.ContainsKey(wrapperType)) wrapper = Wrappers[wrapperType];
			else {
				wrapper = (BasePropertyGridObjectWrapper)Activator.CreateInstance(wrapperType, false);
				if(wrapper != null) Wrappers.Add(wrapperType, wrapper);
			}
			if(wrapper != null) {
				wrapper.SetWrappedObject(obj);
				wrapper.SetPropertyGrid(this);
			}
			return wrapper;
		}
		IComponent FindDTServiceProvider() {
			if(DesignerControl == null) return null;
			IComponent serviceProvider = null;
			GaugeDesignerForm form = DesignerControl.FindForm() as GaugeDesignerForm;
			if(form != null) serviceProvider = form.Gauge;
			return serviceProvider;
		}
		protected void InitializeContextMenu() {
			this.menuItemReset = new MenuItem();
			this.menuItemReset.Enabled = false;
			this.menuItemReset.Index = 0;
			this.menuItemReset.Text = "Reset";
			this.menuItemReset.Click += OMenuItemResetClick;
			this.propertyGridContextMenu = new ContextMenu();
			this.propertyGridContextMenu.MenuItems.AddRange(
					new MenuItem[] { this.menuItemReset }
				);
			this.propertyGridContextMenu.Popup += OnContextMenuPopup;
			ContextMenu = propertyGridContextMenu;
		}
		void OMenuItemResetClick(object sender, EventArgs e) {
			object oldValue = SelectedGridItem.Value;
			ResetSelectedProperty();
			OnPropertyValueChanged(new PropertyValueChangedEventArgs(SelectedGridItem, oldValue));
		}
		void OnContextMenuPopup(object sender, EventArgs e) {
			menuItemReset.Enabled = CanResetSelectedItems();
		}
		bool CanResetSelectedItems() {
			if(SelectedObject == null || SelectedGridItem == null) return false;
			try {
				return SelectedGridItem.PropertyDescriptor.ShouldSerializeValue(SelectedObject)
					&& SelectedGridItem.PropertyDescriptor.CanResetValue(SelectedObject);
			}
			catch { return false; }
		}
	}
}

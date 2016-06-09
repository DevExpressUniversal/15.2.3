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
using DevExpress.XtraGauges.Core.Base;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Model;
namespace DevExpress.XtraGauges.Win.Data {
	public class BaseBindableProvider : BaseObject {
		IBindableComponent componentCore;
		BindingContext bindingContextCore = null;
		ControlBindingsCollection dataBindingsCore = null;
		public BaseBindableProvider(IBindableComponent component)
			: base() {
			this.componentCore = component;
		}
		protected override void OnCreate() { }
		protected override void OnDispose() {
			if(dataBindingsCore != null) {
				dataBindingsCore.Clear();
				dataBindingsCore = null;
			}
			bindingContextCore = null;
			componentCore = null;
		}
		protected IBindableComponent Component {
			get { return componentCore; }
		}
		public ControlBindingsCollection DataBindings {
			get {
				if(dataBindingsCore == null) dataBindingsCore = new ControlBindingsCollection(Component);
				return dataBindingsCore;
			}
		}
		public BindingContext BindingContext {
			get {
				if(bindingContextCore != null) {
					return bindingContextCore;
				}
				IBindableComponent parentComponent = GetParentBindableComponent();
				if(parentComponent != null) {
					return parentComponent.BindingContext;
				}
				return null;
			}
			set {
				if(bindingContextCore == value) return;
				this.bindingContextCore = value;
				OnBindingContextChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnBindingContextChanged() {
			if(bindingContextCore != null) {
				this.UpdateBindings(this.BindingContext);
			}
		}
		public void UpdateBindings(BindingContext context) {
			if(dataBindingsCore == null) return;
			for(int i = 0; i < DataBindings.Count; i++) {
				BindingContext.UpdateBinding(context, this.DataBindings[i]);
			}
		}
		IBindableComponent GetParentBindableComponent() {
			BaseGaugeModel model = BaseGaugeModel.Find(Component);
			if(model != null && model.Owner!=null) {
				return model.Owner.Container as IBindableComponent;
			}
			return null;
		}
	}
}

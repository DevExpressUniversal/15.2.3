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
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Base;
using System.Web.UI;
using DevExpress.XtraGauges.Core.Model;
namespace DevExpress.Web.ASPxGauges.Data {
	public class BaseBindableProvider : BaseObject {
		IElement<IRenderableElement> ownerCore;
		EventHandler dataBindingCore;
		public BaseBindableProvider(IElement<IRenderableElement> component)
			: base() {
			this.ownerCore = component;
		}
		protected override void OnCreate() { }
		protected override void OnDispose() {
			dataBindingCore = null;
			ownerCore = null;
		}
		protected IElement<IRenderableElement> Owner {
			get { return ownerCore; }
		}
		public void DataBind() {
			RaiseDataBinding(EventArgs.Empty);
		}
		protected void RaiseDataBinding(EventArgs e) {
			if(dataBindingCore != null) dataBindingCore(Owner, e);
		}
		public event EventHandler DataBinding {
			add { dataBindingCore += value; }
			remove { dataBindingCore -= value; }
		}
		public Control BindingContainer {
			get { return GetGaugeContainer(Owner); }
		}
		Control GetGaugeContainer(IElement<IRenderableElement> e) {
			BaseGaugeModel model = BaseGaugeModel.Find(e);
			if(model != null && model.Owner != null) {
				return model.Owner.Container as Control;
			}
			return null;
		}
	}
}

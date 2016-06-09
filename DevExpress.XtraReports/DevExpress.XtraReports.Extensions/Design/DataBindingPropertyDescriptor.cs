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
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.Data;
namespace DevExpress.XtraReports.Design {
	public class DataBindingPropertyDescriptor : PropertyDescriptor {
		public DataBindingPropertyDescriptor(PropertyDescriptor propertyDescriptor, Attribute[] attrs) : base(propertyDescriptor, attrs) { }
		public override Type ComponentType {
			get { return typeof(XRBindingCollection); }
		}
		public override Type PropertyType {
			get { return typeof(DataBinding); }
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public override bool CanResetValue(object component) {
			DataBinding dataBinding = GetBinding(component as XRBindingCollection);
			return dataBinding.Binding.IsNull == false;
		}
		public override void ResetValue(object component) {
			XRBindingCollection bindings = component as XRBindingCollection;
			new DataBinding(bindings.Control, null, null, string.Empty, Name).SetIntoDataBindings();
		}
		public override object GetValue(object component) {
			return GetBinding(component as XRBindingCollection);
		}
		DataBinding GetBinding(XRBindingCollection bindings) {
			XRBinding xrBinding = bindings[Name];
			return xrBinding == null ? new DataBinding(bindings.Control, null, null, String.Empty, Name) :
				new DataBinding(bindings.Control, DesignBindingHelper.CreateDesignBinding(xrBinding), xrBinding.FormatString, Name);
		}
		public override void SetValue(object component, object value) {
		}
	}
}

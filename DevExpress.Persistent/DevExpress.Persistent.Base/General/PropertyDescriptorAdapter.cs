#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
namespace DevExpress.Persistent.Base.General {
	public class PropertyDescriptorAdapter : System.ComponentModel.PropertyDescriptor {
		private IPropertyDescriptor propertyDesc;
		public PropertyDescriptorAdapter(IPropertyDescriptor propertyDesc)
			: base(propertyDesc.Name, null) {
			this.propertyDesc = propertyDesc;
		}
		public IPropertyDescriptor PropertyDesc {
			get { return propertyDesc; }
		}
		public override bool CanResetValue(object component) {
			return true;
		}
		public override Type ComponentType {
			get { return typeof(IPropertyBag); }
		}
		public override object GetValue(object component) {
			return ((IPropertyBag)component)[propertyDesc.Name];
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override Type PropertyType {
			get { return propertyDesc.ValueType; }
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
			((IPropertyBag)component)[propertyDesc.Name] = value;
		}
		public override bool ShouldSerializeValue(object component) {
			return true;
		}
		public static PropertyDescriptorCollection CreatePropertyDescriptorCollection(IList<IPropertyDescriptor> properties) {
			System.ComponentModel.PropertyDescriptor[] result = new System.ComponentModel.PropertyDescriptor[properties.Count];
			for (int i = 0; i < properties.Count; i++) {
				result[i] = new PropertyDescriptorAdapter(properties[i]);
			}
			return new System.ComponentModel.PropertyDescriptorCollection(result);
		}
	}
}

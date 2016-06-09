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

using System.Collections.Generic;
namespace DevExpress.Persistent.Base.General {
	public class PropertyBagImpl {
		private IPropertyBag owner;
		private IPropertyDescriptorContainer descriptors;
		private Dictionary<string, IPropertyValue> propertiesByName;
		private void AddValue(IPropertyValue val) {
			propertiesByName[val.Descriptor.Name] = val;
		}
		public PropertyBagImpl(IPropertyBag owner) {
			this.owner = owner;
		}
		public void Init() {
			propertiesByName = new Dictionary<string, IPropertyValue>();
			foreach (IPropertyValue val in owner.PropertyValues) {
				AddValue(val);
			}
			if (Descriptors == null) return;
			foreach (PropertyDescriptorAdapter desc in Descriptors.PropertyDescriptors) {
				if (!propertiesByName.ContainsKey(desc.Name)) {
					AddValue(owner.CreateValue(desc.PropertyDesc));
				}
			}
		}
		public IPropertyDescriptorContainer Descriptors {
			get { return descriptors; }
			set { 
				descriptors = value;
				propertiesByName = null;
			}
		}
		public object this[string name] {
			get {
				if(propertiesByName == null)
					Init();
				return propertiesByName[name].Value;
			}
			set {
				if(propertiesByName == null)
					Init();
				propertiesByName[name].Value = value;
			}
		}
		public System.ComponentModel.PropertyDescriptorCollection GetItemProperties(System.ComponentModel.PropertyDescriptor[] listAccessors) {
			return Descriptors.PropertyDescriptors;
		}
		public string GetListName(System.ComponentModel.PropertyDescriptor[] listAccessors) {
			return "Properties";
		}
	}
}

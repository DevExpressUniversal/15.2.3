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
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	public class PropertyDescriptor : BaseObject, IPropertyDescriptor {
		private PropertyDescriptorImpl propertyDescriptor;
		private string code;
		public PropertyDescriptor(Session session)
			: base(session) {
			this.propertyDescriptor = new PropertyDescriptorImpl();
		}
		public PropertyDescriptor(Session session, string name, System.Type valueType)
			: this(session) {
			this.propertyDescriptor = new PropertyDescriptorImpl(name, valueType);
			this.code = name.Substring(0, Math.Min(4, name.Length));
		}
		[Association("PropertyBagDescriptor-PropertyDescriptor")]
		public XPCollection<PropertyBagDescriptor> PropertyBags {
			get { return GetCollection<PropertyBagDescriptor>("PropertyBags"); }
		}
		[Indexed(Unique = true)]
		[Size(4)]
		[RuleRequiredField("PropertyDescriptor Code required", "Save", "")]
		[RuleUniqueValue("PropertyDescriptor Code is unique", "Save", "The 'Code' property must have a unique value")]
		public string Code {
			get { return code; }
			set { SetPropertyValue("Code", ref code, value); }
		}
		#region IPropertyDescriptor
		public string Name {
			get { return propertyDescriptor.Name; }
			set {
				string oldValue = propertyDescriptor.Name;
				propertyDescriptor.Name = value;
				OnChanged("Name", oldValue, propertyDescriptor.Name);
			}
		}
		public string Description {
			get { return propertyDescriptor.Description; }
			set {
				string oldValue = propertyDescriptor.Description;
				propertyDescriptor.Description = value;
				OnChanged("Description", oldValue, propertyDescriptor.Description);
			}
		}
		public string Type {
			get { return propertyDescriptor.Type; }
			set {
				string oldValue = propertyDescriptor.Type;
				propertyDescriptor.Type = value;
				OnChanged("Type", oldValue, propertyDescriptor.Type);
			}
		}
		public Type ValueType {
			get { return propertyDescriptor.ValueType; }
		}
		#endregion
	}
}

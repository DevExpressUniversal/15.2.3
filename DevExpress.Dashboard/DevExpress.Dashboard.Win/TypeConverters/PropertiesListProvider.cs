#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NamesList = System.Collections.Generic.List<string>;
namespace DevExpress.DashboardWin.Native {
	class PropertiesListProvider<T> where T : class {
		readonly NamesList defaultExcludedPropNames = new NamesList();
		readonly ITypeDescriptorContext context;
		readonly Attribute[] attributes;
		readonly T value;
		protected T Value { get { return value; } }
		protected ITypeDescriptorContext Context { get { return context; } }
		public PropertiesListProvider(ITypeDescriptorContext context, T value, Attribute[] attributes, IEnumerable<string> defaultExcludedPropNames = null) {
			if(defaultExcludedPropNames != null)
				this.defaultExcludedPropNames.AddRange(defaultExcludedPropNames);
			this.value = value;
			this.context = context;
			this.attributes = attributes;
		}
		public PropertyDescriptorCollection GetProperties() {
			NamesList excludedPropsNames = new NamesList(defaultExcludedPropNames);
			FillExcluded(excludedPropsNames);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(Value, attributes);
			List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor property in properties)
				if(!excludedPropsNames.Contains(property.Name))
					newProperties.Add(property);
			return new PropertyDescriptorCollection(newProperties.ToArray());
		}
		protected virtual void FillExcluded(NamesList propNames) {
		}
	}
}

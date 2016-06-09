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
using System.Web;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Design {
	public class SiteMapNodeTypeConverter : TypeConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection oldPropCollection = TypeDescriptor.GetProperties(context.Instance);
			PropertyDescriptorCollection newPropCollection = new PropertyDescriptorCollection(null);
			if(oldPropCollection != null) {
				newPropCollection.Add(oldPropCollection["Title"]);
				newPropCollection["Title"].SetValue(context.Instance, oldPropCollection["Title"].GetValue(context.Instance));
				newPropCollection.Add(oldPropCollection["Url"]);
				newPropCollection["Url"].SetValue(context.Instance, oldPropCollection["Url"].GetValue(context.Instance));
				newPropCollection.Add(oldPropCollection["Description"]);
				newPropCollection["Description"].SetValue(context.Instance, oldPropCollection["Description"].GetValue(context.Instance));
			}
			return newPropCollection;
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			if(context.Instance is SiteMapNode)
				return true;
			else
				return false;
		}
	}
	public class SiteMapDataSourceIDConverter : HierarchicalDataSourceIDConverter {
		protected override bool IsValidDataSource(IComponent component) {
			return (component is SiteMapDataSource);
		}
	}
	public class SiteMapDataSourceConverter : HierarchicalDataSourceConverter {
		protected override bool IsValidDataSource(IComponent component) {
			if(base.IsValidDataSource(component))
				return (component is SiteMapNodeCollection);
			else
				return false;
		}
	}
	public class NodeIndexConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType) {
			if((sourceType == typeof(string)) && (context != null))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if((value.GetType() == typeof(string)) && (context != null)) {
				List<int> indexes = NodeIndexUtils.CreateNodeIndexList(context);
				int index = int.Parse((string)value);
				if(indexes.Contains(index))
					return index;
				else
					return -1;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context != null)
				return new StandardValuesCollection(NodeIndexUtils.CreateNodeIndexList(context));
			else
				return base.GetStandardValues(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return (context != null);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
}

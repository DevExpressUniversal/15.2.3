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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils.Design;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class SqlQueryCollectionTypeConverter : CollectionTypeConverter {
		class QueryPropertyDescriptor : SimplePropertyDescriptor {
			class QueryExpandableObjectConverter : ExpandableObjectConverter {
				class QueryNamePropertyDescriptor : SimplePropertyDescriptor {
					readonly PropertyDescriptor parent;
					readonly IServiceProvider serviceProvider;
					public QueryNamePropertyDescriptor(PropertyDescriptor parent, IServiceProvider serviceProvider)
						: base(
							parent.ComponentType,
							parent.Name,
							parent.PropertyType,
							(Attribute[])(new ArrayList(parent.Attributes).ToArray(typeof(Attribute)))
						) {
							this.parent = parent;
						this.serviceProvider = serviceProvider;
					}
					public override object GetValue(object component) {
						return this.parent.GetValue(component);
					}
					public override void SetValue(object component, object value) {
						var designerHost = serviceProvider != null ? (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost)) : null;
						var componentChangeService = serviceProvider != null ? (IComponentChangeService) serviceProvider.GetService(typeof (IComponentChangeService)) : null;
						var transaction = designerHost != null ? designerHost.CreateTransaction() : null;
						SqlQuery query = (SqlQuery)component;
						string newName = (string)value;
						string oldName = query.Name;
						if(componentChangeService != null)
							componentChangeService.OnComponentChanging(query.DataSource, null);
						try {
							this.parent.SetValue(component, value);
							query.DataSource.UpdateRelations(oldName, newName);
							query.DataSource.RenameResultSchemaPart(oldName, newName);
							if(componentChangeService != null)
								componentChangeService.OnComponentChanged(query.DataSource, null, null, null);
							if(transaction != null)
								transaction.Commit();
						}
						catch(Exception) {
							if(transaction != null)
								transaction.Cancel();
							throw;
						}
					}
				}
				public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
					PropertyDescriptorCollection props = base.GetProperties(context, value, attributes);
					List<PropertyDescriptor> result = new List<PropertyDescriptor>(props.Count);
					foreach(PropertyDescriptor pd in props)
						result.Add(pd.Name != "Name" ? pd : new QueryNamePropertyDescriptor(pd, context));
					return new PropertyDescriptorCollection(result.ToArray(), true);
				}
			}
			int index;
			public QueryPropertyDescriptor(int index)
				: base(
					typeof(SqlQueryCollection),
					string.Format("[{0}]", index),
					typeof(SqlQuery),
					new Attribute[] { 
						new TypeConverterAttribute(typeof(QueryExpandableObjectConverter)),
						new EditorAttribute(typeof(QueryEditor), typeof(UITypeEditor))
					}
				) 
			{
				this.index = index;
			}
			public override object GetValue(object component) {
				SqlQueryCollection collection = (SqlQueryCollection)component;
				return collection[this.index];
			}
			public override void SetValue(object component, object value) {
				SqlQueryCollection collection = (SqlQueryCollection)component;
				collection[this.index] = (SqlQuery)value;
			}
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			SqlQueryCollection collection = (SqlQueryCollection)value;
			PropertyDescriptor[] result = new QueryPropertyDescriptor[collection.Count];
			for(int i = 0; i < result.Length; i++)
				result[i] = new QueryPropertyDescriptor(i);
			return new PropertyDescriptorCollection(result);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) { return true; }
	}
}

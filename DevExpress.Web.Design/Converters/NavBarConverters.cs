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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.Design;
namespace DevExpress.Web.Design {
	public class DataSourceViewChildSchemaConverter : DataSourceViewSchemaConverter {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context, Type typeFilter) {
			if(context == null)
				return base.GetStandardValues(context, typeFilter);
			var instance = context.Instance as IDataSourceViewSchemaAccessor;
			if(instance == null)
				return base.GetStandardValues(context, typeFilter);
			var schema = instance.DataSourceViewSchema as IDataSourceViewSchema;
			if(schema != null) {
				var children = schema.GetChildren();
				if(children != null && (children.Length > 0))
					return GetFields(children[0], typeFilter);
			}
			return base.GetStandardValues(context, typeFilter);
		}
		private TypeConverter.StandardValuesCollection GetFields(IDataSourceViewSchema dataSourceViewSchema,
			Type typeFilter) {
			string[] destinationArray = null;
			if(dataSourceViewSchema != null) {
				IDataSourceFieldSchema[] fields = dataSourceViewSchema.GetFields();
				string[] sourceArray = new string[fields.Length];
				int index = 0;
				for(int i = 0; i < fields.Length; i++) {
					if(((typeFilter != null) && (fields[i].DataType == typeFilter)) || (typeFilter == null)) {
						sourceArray[index] = fields[i].Name;
						index++;
					}
				}
				destinationArray = new string[index];
				Array.Copy(sourceArray, destinationArray, index);
			}
			if(destinationArray == null)
				destinationArray = new string[0];
			Array.Sort(destinationArray, System.Collections.Comparer.Default);
			return new TypeConverter.StandardValuesCollection(destinationArray);
		}
	}
}

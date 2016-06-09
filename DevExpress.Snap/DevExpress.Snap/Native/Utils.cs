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
using System.Drawing.Design;
using System.ComponentModel.Design;
using DevExpress.Snap.Core;
using DevExpress.Utils.UI;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.API;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Data;
namespace DevExpress.Snap.Native {
	public class SNTypeDescriptionProvider : TypeDescriptionProvider {
		readonly string dataMember;
		readonly string dataSourceName;
		readonly IDataSourceDispatcher dataSourceDispatcher;
		readonly DevExpress.DataAccess.Native.Data.FilterInfoCollection collection;
		public SNTypeDescriptionProvider()
			: this(null, null, null) {
		}
		public SNTypeDescriptionProvider(string dataSourceName, IDataSourceDispatcher dataSourceDispatcher, string dataMember) {
			this.dataMember = dataMember;
			this.dataSourceName = dataSourceName;
			this.dataSourceDispatcher = dataSourceDispatcher;
		}
		public SNTypeDescriptionProvider(DevExpress.DataAccess.Native.Data.FilterInfoCollection collection) {
			this.collection = collection;
		}
		public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args) {
			IItemsContainer itemsContainer = (IItemsContainer)provider.GetService(typeof(IItemsContainer));
			if (objectType == typeof(CalculatedField)) {
				CalculatedFieldCollection calculatedFields = new CalculatedFieldCollection();
				foreach (CalculatedField item in itemsContainer.Items) {
					calculatedFields.Add(item);
				}
				return new CalculatedField(NameHelper.GetCalculatedFieldName(dataSourceDispatcher.GetDataSource(dataSourceName), calculatedFields, dataMember), dataMember) { DataSourceName = dataSourceName, DataSourceDispatcher = dataSourceDispatcher };
			}
			if (objectType == typeof(Parameter)) {
				List<IParameter> parameters = new List<IParameter>();
				foreach (IParameter item in itemsContainer.Items) {
					parameters.Add(item);
				}
				return new Parameter() { Name = NameHelper.GetParameterName(parameters) };
			}
			if (objectType == typeof(FilterInfo)) {
				return new FilterInfo() { Owner = collection };
			}
			return base.CreateInstance(provider, objectType, argTypes, args);
		}
	}
	public class ParametersOwner {
		readonly ParameterCollection parameters;
		public ParametersOwner(ParameterCollection parameters) {
			this.parameters = parameters;
		}
		public ParameterCollection Parameters {
			get { return parameters; }
		}
	}
}

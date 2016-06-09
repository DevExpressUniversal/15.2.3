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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.Utils.Design;
using DevExpress.Data;
#if !SILVERLIGHT
using System;
#endif
namespace DevExpress.XtraReports.Parameters {
	[ListBindable(BindableSupport.No)]
	[TypeConverter(typeof(CollectionTypeConverter))]
	public class ParameterCollection : Collection<Parameter>, IEnumerable<IParameter> {
		public Parameter this[string parameterName] {
			get { return GetByName(parameterName); }
		}
		internal virtual bool IsLoading {
			get { return false; }
		}
		public void AddRange(Parameter[] parameters) {
			foreach(Parameter parameter in parameters)
				Add(parameter);
		}
		internal Parameter GetByName(string parameterName) {
			return Items.FirstOrDefault(parameter => parameter.Name == parameterName);
		}
		protected override void InsertItem(int index, Parameter item) {
			if(Contains(item))
				return;
			base.InsertItem(index, item);
			item.Owner = this;
		}
#if !SILVERLIGHT
		protected internal virtual string Serialize(object value) {
			return string.Empty;
		}
		protected internal virtual bool Deserialize(string value, string typeName, out object result) {
			result = DBNull.Value;
			return false;
		}
#endif
		#region IEnumerable<IParameter> Members
		IEnumerator<IParameter> IEnumerable<IParameter>.GetEnumerator() {
			return Items.Cast<IParameter>().GetEnumerator();
		}
		#endregion
	}
}

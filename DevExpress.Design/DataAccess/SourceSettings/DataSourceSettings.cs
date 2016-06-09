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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	class DataSourceSettings : IDataSourceSettings {
		IEnumerable<IDataSourceProperty> propertiesSource;
		public DataSourceSettings(IEnumerable<IDataSourceProperty> source) {
			this.propertiesSource = source;
		}
		IDictionary<DataSourcePropertyCodeName, IDataSourceProperty> properties;
		public IDataSourceProperty this[DataSourcePropertyCodeName codeName] {
			get {
				if(properties == null) {
					properties = new Dictionary<DataSourcePropertyCodeName, IDataSourceProperty>();
					foreach(IDataSourceProperty property in propertiesSource)
						properties.Add(codeName, property);
				}
				IDataSourceProperty result;
				return properties.TryGetValue(codeName, out result) ? result : null; 
			}
		}
		#region IEnumerable
		public IEnumerator<IDataSourceProperty> GetEnumerator() {
			return propertiesSource.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return propertiesSource.GetEnumerator();
		}
		#endregion IEnumerable
		public static readonly IDataSourceSettings Empty = new EmptyDataSourceSettings();
		class EmptyDataSourceSettings : DataSourceSettings {
			public EmptyDataSourceSettings()
				: base(new IDataSourceProperty[] { }) {
			}
		}
	}
}

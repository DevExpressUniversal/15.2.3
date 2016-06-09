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
using System.ComponentModel.Design;
using DevExpress.Snap.Core.API;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.Snap.Design {
	public class DataSourceInfoCollectionEditor : CollectionEditor {
		readonly string DefaultDataSourceName = "Data Source {0}";
		public DataSourceInfoCollectionEditor(Type type)
			: base(type) {
		}
		DataSourceInfoCollection DataSources { get { return Context.PropertyDescriptor.GetValue(Context.Instance) as DataSourceInfoCollection; } }
		protected override object CreateInstance(Type itemType) {
			Debug.Assert(itemType == typeof(DataSourceInfo));
			int index = 1;
			string name;
			do {
				name = String.Format(DefaultDataSourceName, index);
				index++;
			} while (!CheckDataSourceName(name));
			return new DataSourceInfo(name, null);
		}
		bool CheckDataSourceName(string name) {
			foreach (DataSourceInfo info in DataSources) {
				if (info.DataSourceName == name)
					return false;
			}
			return true;
		}
	}
}

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

using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
namespace DevExpress.Map.Native {
	public static class DataHelper {
		static BindingContext bindingContext = new BindingContext();
		public static IList GetList(object dataSource, string dataMember) {
			if(dataMember == null)
				dataMember = String.Empty;
			bool useContext = !((dataSource is DataSet) && dataMember.Length == 0);
			return DevExpress.Data.Helpers.MasterDetailHelper.GetDataSource((useContext ? bindingContext : null), dataSource, dataMember);
		}
		public static StringCollection GetColumnNames(IList listSource) {
			List<String> names = new List<String>();
			try {
				PropertyDescriptorCollection properties = GetItemProperties(listSource, "");
				if(properties != null) {
					for(int i = 0; i < properties.Count; i++)
						names.Add(properties[i].Name);
				}
			}
			catch {
			}
			names.Sort();
			StringCollection result = new StringCollection();
			result.AddRange(names.ToArray());
			return result;
		}
		static PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember) {
			BindingManagerBase mgr = GetBindingManager(dataSource, dataMember);
			return (mgr != null) ? mgr.GetItemProperties() : null;
		}
		static BindingManagerBase GetBindingManager(object dataSource, string dataMember) {
			BindingManagerBase mgr = null;
			if(dataSource != null) {
				try {
					if(dataMember.Length > 0)
						mgr = bindingContext[dataSource, dataMember];
					else
						mgr = bindingContext[dataSource];
				}
				catch { }
			}
			return mgr;
		}
	}
}

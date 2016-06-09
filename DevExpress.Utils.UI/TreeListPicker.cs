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
using System.Drawing;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraReports.Design {
	public class TreeListPicker : DesignTreeListBindingPicker {
		#region inner classes
		class XtraTreeListPickManager : TreeListPickManager {
			public XtraTreeListPickManager() {
			}
			protected override IPropertiesProvider CreateProvider() {
				IDataContextService service = GetDataContextService();
				DataContextOptions options = new DataContextOptions(true, true);
				return service != null ? new CustomDataSortedPropertiesProvider(service.CreateDataContext(options), service) :
					base.CreateProvider();
			}
		}
		class CustomDataSortedPropertiesProvider : DataSortedPropertiesProvider {
			public CustomDataSortedPropertiesProvider(DataContext dataContext, IDataContextService serv) : 
				base(dataContext, serv) { 
			}
			protected override bool CanProcessProperty(IPropertyDescriptor property) {
				if(!base.CanProcessProperty(property))
					return false;
				Type propertyType = GetPropertyType(property);
				return (typeof(IList).IsAssignableFrom(propertyType) && !propertyType.IsArray) || DataContext.IsComplexType(propertyType);
			}
		}
		#endregion
		public TreeListPicker()
			: base(new XtraTreeListPickManager()) {
		}
		protected override bool CanCloseDropDown(XtraListNode node) {
			DataMemberListNodeBase dataMemberNode = node as DataMemberListNodeBase;
			if(dataMemberNode == null || dataMemberNode.IsDataSourceNode)
				return false;
			return dataMemberNode.Property == null || ListTypeHelper.IsListType(dataMemberNode.Property.PropertyType);
		}
	}
}

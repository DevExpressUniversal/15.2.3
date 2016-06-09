#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.ExpressApp.DC;
namespace DevExpress.Persistent.Base.ReportsV2 {
	public class ViewPropertyDescriptorProvider : ArrayList, ITypedList {
		ViewPropertiesCollection properties;
		PropertyDescriptorCollection displayProps;
		private string listName;
		public ViewPropertyDescriptorProvider(string listName, ViewPropertiesCollection properties) {
			this.listName = listName;
			this.properties = properties;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			return listAccessors != null && listAccessors.Length > 0 ? new PropertyDescriptorCollection(null) : DisplayProps;
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return listName;
		}
		internal PropertyDescriptorCollection DisplayProps {
			get {
				if(displayProps == null)
					displayProps = Properties;
				return displayProps;
			}
		}
		PropertyDescriptorCollection Properties {
			get {
				PropertyDescriptorCollection props = new PropertyDescriptorCollection(null);
				foreach(ViewProperty prop in properties) {
					try {
						props.Add(new ReportsDataSourcePropertyDescriptor(prop.DisplayName));
					}
					catch {
						if(!DataSourceBase.IsDesignMode)
							throw;
					}
				}
				return props;
			}
		}
	}
}

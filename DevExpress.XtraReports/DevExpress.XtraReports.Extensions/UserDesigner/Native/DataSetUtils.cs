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
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Data;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
namespace DevExpress.XtraReports.UserDesigner.Native
{
	public class DataSetDesigner : ComponentDesigner {
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			Attribute[] attributes = new Attribute[] { BrowsableAttribute.No, DesignerSerializationVisibilityAttribute.Hidden };
			AddAttributes(properties, "Tables", attributes);
			AddAttributes(properties, "Relations", attributes);
			AddAttributes(properties, "Namespace", attributes);
		}
		internal static void AddAttributes(IDictionary properties, string propertyName, Attribute[] attributes) {
			PropertyDescriptor pd = (PropertyDescriptor)properties[propertyName];
			if(pd != null)
				properties[propertyName] = TypeDescriptor.CreateProperty(pd.ComponentType, pd, attributes);
		}
	}
	public class DataAdapterDesigner : ComponentDesigner {
		public DataAdapterDesigner() { }
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(!(Component.Site is DevExpress.XtraReports.Serialization.XRDesignSite) && Component is System.Data.IDataAdapter) {
				Attribute[] attributes = new Attribute[] { BrowsableAttribute.No };
				DataSetDesigner.AddAttributes(properties, "SelectCommand", attributes);
				DataSetDesigner.AddAttributes(properties, "DeleteCommand", attributes);
				DataSetDesigner.AddAttributes(properties, "UpdateCommand", attributes);
				DataSetDesigner.AddAttributes(properties, "InsertCommand", attributes);
				DataSetDesigner.AddAttributes(properties, "TableMappings", attributes);
			}
		}
	}
}

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

using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
namespace DevExpress.ExpressApp.Web {
	[Browsable(false)]
	[PersistChildren(false), ParseChildren(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafWebDataSource)]
	[ToolboxBitmap(typeof(DevExpress.Persistent.Base.ResFinder), "Resources.CollectionDataSource.ico")]
	[ToolboxBitmap24("DevExpress.Persistent.Base.Resources.CollectionDataSource_24x24.png, DevExpress.Persistent.Base" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix)]
	[ToolboxBitmap32("DevExpress.Persistent.Base.Resources.CollectionDataSource_32x32.png, DevExpress.Persistent.Base" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix)]
	[Designer("DevExpress.ExpressApp.Design.XafWebDesignDataSourceDesigner, DevExpress.ExpressApp.Design" + XafApplication.CurrentVersion + XafAssemblyInfo.AssemblyNamePostfix)]
	public class XafWebDesignDataSource : DataSourceControl {
		protected override DataSourceView GetView(string viewName) {
			return null;
		}
		[Category("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("DevExpress.ExpressApp.ReportsV2.Win.ReportDataTypeConverterForDesigner, DevExpress.ExpressApp.ReportsV2.Win" + XafApplication.CurrentVersion + XafAssemblyInfo.AssemblyNamePostfix)]
		[XtraSerializableProperty(-1)]
		public string ObjectTypeName {
			get;
			set;
		}
	}
	public interface IModelCustomUserControlViewItemWeb : IModelViewItem {
		[Category("Data")]
		[Required()]
		string CustomControlPath { get; set; }
	}
}

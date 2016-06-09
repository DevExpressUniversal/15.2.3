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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Office.Options;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetDataSourceWizardOptions
	[TypeConverter("DevExpress.XtraSpreadsheet.Design.DataSourceWizardTypeConverter, " + AssemblyInfo.SRAssemblySpreadsheetDesign)]
	public class SpreadsheetDataSourceWizardOptions : SpreadsheetNotificationOptions {
		#region Consts
		const DataSourceTypes defaultDataSourceTypes = DataSourceTypes.All;
		const bool defaultAlwaysSaveCredentials = false;
		const bool defaultDisableNewConnections = false;
		const bool defaultEnableCustomSql = false;
		const bool defaultQueryBuilderLight = false;
		#endregion
		#region Fields
		DataSourceTypes dataSourceTypes = defaultDataSourceTypes;
		bool alwaysSaveCredentials = defaultAlwaysSaveCredentials;
		bool disableNewConnections = defaultDisableNewConnections;
		bool enableCustomSql = defaultEnableCustomSql;
		bool queryBuilderLight = defaultQueryBuilderLight;
		#endregion
		#region Properties
		#region DataSourceTypes
		[
		Browsable(false),
		DefaultValue(defaultDataSourceTypes)]
		public DataSourceTypes DataSourceTypes {
			get {
				return dataSourceTypes;
			}
			set {
				if (dataSourceTypes == value)
					return;
				DataSourceTypes oldValue = dataSourceTypes;
				dataSourceTypes = value;
				OnChanged("DataSourceTypes", oldValue, value);
			}
		}
		#endregion
		#region AlwaysSaveCredentials
		[ DefaultValue(defaultAlwaysSaveCredentials), NotifyParentProperty(true)]
		public bool AlwaysSaveCredentials {
			get { return alwaysSaveCredentials; }
			set {
				if (alwaysSaveCredentials == value)
					return;
				bool oldValue = alwaysSaveCredentials;
				alwaysSaveCredentials = value;
				OnChanged("AlwaysSaveCredentials", oldValue, value);
			}
		}
		#endregion
		#region DisableNewConnections
		[ DefaultValue(defaultDisableNewConnections), NotifyParentProperty(true)]
		public bool DisableNewConnections {
			get { return disableNewConnections; }
			set {
				if (disableNewConnections == value)
					return;
				bool oldValue = disableNewConnections;
				disableNewConnections = value;
				OnChanged("DisableNewConnections", oldValue, value);
			}
		}
		#endregion
		#region EnableCustomSql
		[ DefaultValue(defaultEnableCustomSql), NotifyParentProperty(true)]
		public bool EnableCustomSql {
			get { return enableCustomSql; }
			set {
				if (enableCustomSql == value)
					return;
				bool oldValue = enableCustomSql;
				enableCustomSql = value;
				OnChanged("EnableCustomSql", oldValue, value);
			}
		}
		#endregion
		#region QueryBuilderLight
		[ DefaultValue(defaultQueryBuilderLight), NotifyParentProperty(true)]
		public bool QueryBuilderLight {
			get { return queryBuilderLight; }
			set {
				if (queryBuilderLight == value)
					return;
				bool oldValue = queryBuilderLight;
				queryBuilderLight = value;
				OnChanged("QueryBuilderLight", oldValue, value);
			}
		}
		#endregion
		#endregion
		#region Overrides of SpreadsheetNotificationOptions
		protected override void ResetCore() {
			DataSourceTypes = defaultDataSourceTypes;
			AlwaysSaveCredentials = defaultAlwaysSaveCredentials;
			DisableNewConnections = defaultDisableNewConnections;
			EnableCustomSql = defaultEnableCustomSql;
			QueryBuilderLight = defaultQueryBuilderLight;
		}
		#endregion
		protected internal void CopyFrom(SpreadsheetDataSourceWizardOptions source) {
			Guard.ArgumentNotNull(source, "source");
			dataSourceTypes = source.dataSourceTypes;
			alwaysSaveCredentials = source.alwaysSaveCredentials;
			disableNewConnections = source.disableNewConnections;
			enableCustomSql = source.enableCustomSql;
			queryBuilderLight = source.queryBuilderLight;
		}
	}
	#endregion
}

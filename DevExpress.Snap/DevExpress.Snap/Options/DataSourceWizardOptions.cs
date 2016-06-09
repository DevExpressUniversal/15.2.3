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

using System.ComponentModel;
using DevExpress.Office.Options;
using DevExpress.DataAccess.UI.Wizard;
namespace DevExpress.Snap.Options {
	public class DataSourceWizardOptions {
		static readonly SqlWizardSettings sqlWizardSettingsDefault = new SqlWizardSettings();
		#region Fields
		const DataSourceTypes defaultDataSourceTypes = DataSourceTypes.All;
		DataSourceTypes dataSourceTypes = defaultDataSourceTypes;
		readonly SqlWizardSettings sqlWizardSettings;
		#endregion
		public DataSourceWizardOptions() {
			this.sqlWizardSettings = new SqlWizardSettings();
		}
		#region DataSourceTypes
		[
		Browsable(false),
		DefaultValue(defaultDataSourceTypes)]
		public DataSourceTypes DataSourceTypes { get { return dataSourceTypes; } set { dataSourceTypes = value; } }
		#endregion
		#region SqlWizardSettings
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SqlWizardSettings SqlWizardSettings { get { return sqlWizardSettings; } }
		#endregion
		public void Reset() {
			DataSourceTypes = defaultDataSourceTypes;
			SqlWizardSettings.Reset();
		}
		bool ShouldSerializeSqlWizardSettings() {
			return !sqlWizardSettings.Equals(sqlWizardSettingsDefault);
		}
		public override bool Equals(object obj) {
			var other = obj as DataSourceWizardOptions;
			if(other == null)
				return false;
			return DataSourceTypes == other.DataSourceTypes && SqlWizardSettings.Equals(other.SqlWizardSettings);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}

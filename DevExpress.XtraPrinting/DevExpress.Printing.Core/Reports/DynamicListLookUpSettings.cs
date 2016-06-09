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
using System.Drawing.Design;
using DevExpress.Data.Design;
using DevExpress.Printing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Parameters {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.DynamicListLookUpSettings")]
	public class DynamicListLookUpSettings : LookUpSettings {
		[
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		Editor("DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.DynamicListLookUpSettings.DataSource")
		]
		public override object DataSource { get; set; }
		[
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		Editor("DevExpress.XtraReports.Design.DataAdapterEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraReports.Design.DataAdapterConverter," + AssemblyInfo.SRAssemblyReports),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.DynamicListLookUpSettings.DataAdapter")
		]
		public override object DataAdapter { get; set; }
		[
		XtraSerializableProperty,
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.DataContainerDataMemberEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraReports.Design.DataMemberTypeConverter," + AssemblyInfo.SRAssemblyReports),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.DynamicListLookUpSettings.DataMember")
		]
		public override string DataMember { get; set; }
		[
		XtraSerializableProperty,
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.DataContainerFieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraReports.Design.FieldNameConverter," + AssemblyInfo.SRAssemblyReports),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.DynamicListLookUpSettings.ValueMember")
		]
		public string ValueMember { get; set; }
		[
		XtraSerializableProperty,
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.DataContainerFieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraReports.Design.FieldNameConverter," + AssemblyInfo.SRAssemblyReports),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.DynamicListLookUpSettings.DisplayMember")
		]
		public string DisplayMember { get; set; }
	}
}

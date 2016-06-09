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
using DevExpress.XtraReports.Native;
#if !SL
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Printing;
using DevExpress.Data.Design;
#endif
namespace DevExpress.XtraReports.Parameters {
	public abstract class LookUpSettings : IDataContainer
#if !SL
		, IObject
#endif
 {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Parameter Parameter { get; set; }
		protected internal virtual void SyncParameterType(Type type) {
		}
#if !SL
		#region IObject Members
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DevExpress.Utils.Serializing.XtraSerializableProperty(-1)]
		public string ObjectType {
			get {
				Type type = GetType();
				return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
			}
		}
		#endregion
#endif
		[
		XtraSerializableProperty,
		DefaultValue(""),
#if !SL
		Editor("DevExpress.XtraReports.Design.LookUpSettingsFilterStringEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		TypeConverter("DevExpress.XtraReports.Design.TextPropertyTypeConverter," + AssemblyInfo.SRAssemblyReports),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.Parameters.LookUpSettings.FilterString")
		]
		public string FilterString { get; set; }
		#region IDataContainer
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract object DataSource { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract string DataMember { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract object DataAdapter { get; set; }
		object IDataContainer.GetEffectiveDataSource() {
			return DataSource;
		}
		object IDataContainer.GetSerializableDataSource() {
			return DataSource;
		}
		#endregion
		protected LookUpSettings() {
			FilterString = string.Empty;
		}
	}
}

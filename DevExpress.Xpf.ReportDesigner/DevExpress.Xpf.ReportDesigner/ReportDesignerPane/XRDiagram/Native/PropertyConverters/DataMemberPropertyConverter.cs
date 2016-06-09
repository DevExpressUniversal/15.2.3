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

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.PropertyConverters {
	public class DataMemberPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(string); } }
		public Type VirtualPropertyType { get { return typeof(BindingData); } }
		public object Convert(object value, object owner) {
			var dataMember = value as string;
			if(dataMember == null)
				return null;
		IDataContainerBase realComponent;
			if(owner is DiagramItem) {
				var model = ((DiagramItem)owner).GetValue(XRModelBase.XRModelProperty) as XRModelBase;
				realComponent = model.XRObject as IDataContainerBase;
			} else {
				realComponent = owner as IDataContainerBase;
			}
			if(realComponent == null)
				return null;
			return new BindingData(realComponent.DataSource, dataMember);
		}
		public object ConvertBack(object value) {
			return (value as BindingData).Return(x => x.Member, () => null);
		}
	}
	public class FieldMemberPropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType {
			get { return typeof(string); }
		}
		public Type VirtualPropertyType {
			get { return typeof(BindingData); }
		}
		public object Convert(object value, object owner) {
			return null;
		}
		public object ConvertBack(object value) {
			return null;
		}
	}
}

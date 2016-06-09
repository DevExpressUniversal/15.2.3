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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions {
	[DebuggerDisplay("{GetType()}")]
	[TypeConverter(typeof(BindingSettingsTypeConverter))]
	public class BindingSettings {
		class BindingSettingsTypeConverter : ExpandableObjectConverter {
			static readonly string DataPropertyName = ExpressionHelper.GetPropertyName((BindingSettings x) => x.Data);
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
				var allProperties = base.GetProperties(context, value, attributes);
				if(((BindingSettings)value).Data != null) return allProperties;
				var dataProperty = allProperties[DataPropertyName];
				return new PropertyDescriptorCollection(dataProperty == null ? new PropertyDescriptor[] { } : new PropertyDescriptor[] { dataProperty });
			}
		}
		readonly XRBindingCollection bindings;
		readonly string propertyName;
		public BindingSettings(XRBindingCollection bindings, string propertyName) {
			this.bindings = bindings;
			this.propertyName = propertyName;
		}
		[NotifyParentProperty(true)]
		[Display(AutoGenerateField = false)]
		public BindingData Data {
			get {
				var xrBinding = bindings[propertyName];
				if(xrBinding == null) return null;
				if(xrBinding.Parameter != null) return new BindingData(xrBinding.Parameter, string.Empty);
				return new BindingData(xrBinding.DataSource, xrBinding.DataMember);
			}
			set { SetBinding(value, FormatString); }
		}
		[NotifyParentProperty(true)]
		public string FormatString {
			get { return bindings[propertyName].With(x => x.FormatString); }
			set { SetBinding(Data, value); }
		}
		void SetBinding(BindingData data, string formatString) {
			if(data == null) 
				bindings.Control.ClearBindingAndInvalidate(propertyName);
			 else
				bindings.Control.SetDataBindingAndInvalidate(data.GetXRBinding(propertyName, formatString));
		}
		public override string ToString() {
			return string.Empty;
		}
		#region Equality
		public override int GetHashCode() {
			return propertyName.GetHashCode();
		}
		public static bool operator ==(BindingSettings a, BindingSettings b) {
			bool aIsNull = ReferenceEquals(a, null);
			bool bIsNull = ReferenceEquals(b, null);
			if(aIsNull && bIsNull) return true;
			if(aIsNull || bIsNull) return false;
			return Equals(a.bindings, b.bindings) && string.Equals(a.propertyName, b.propertyName, StringComparison.Ordinal);
		}
		public static bool operator !=(BindingSettings a, BindingSettings b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			return this == obj as BindingSettings;
		}
		#endregion
	}
}

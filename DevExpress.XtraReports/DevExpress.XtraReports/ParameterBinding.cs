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
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.XtraReports.UI {
	[TypeConverter("DevExpress.XtraReports.Design.ParameterBindingConverter, " + AssemblyInfo.SRAssemblyReportsExtensionsFull)]
	public class ParameterBinding : IDataContainerBase {
		Parameter parameter;
		object dataSource;
		string dataMember;
		[DXDisplayName(typeof(DevExpress.XtraReports.ResFinder), "DevExpress.XtraReports.UI.ParameterBinding.ParameterName")]
		[TypeConverter(typeof(ParameterNameConverter))]
		[XtraSerializableProperty]
		public string ParameterName { get; set; }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Reference)]
		public Parameter Parameter { get { return parameter; } internal set { parameter = value; } }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Reference)]
		public object DataSource {
			get {
				if(Parameter != null && RootReport != null)
					return ParametersDataSource;
				return dataSource != null ? dataSource : ParentDataSource;
			}
			internal set { dataSource = value; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty]
		public string DataMember { get { return dataMember; } internal set { dataMember = value; } }
		internal bool IsEmpty { get { return Parameter == null && (DataSource == null || string.IsNullOrEmpty(DataMember)); } }
		internal ParameterBindingCollection Collection { get; set; }
		internal XRSubreport Owner { get { return Collection != null ? Collection.Parent : null; } }
		internal object ParametersDataSource { get { return Owner.RootReport.ParametersDataSource; } }
		internal object SerializeDataSource { get { return dataSource != ParentDataSource ? dataSource : null; } }
		XtraReport RootReport { get { return Owner != null ? Owner.RootReport : null; } }
		XtraReportBase Report { get { return Owner != null ? Owner.Report : null; } }
		object ParentDataSource { get { return Report != null ? Report.GetEffectiveDataSource() : null; } }
		string IDataContainerBase.DataMember {
			get { return DataMember; }
			set { }
		}
		object IDataContainerBase.DataSource {
			get { return DataSource; }
			set { DataSource = value; }
		}
		public ParameterBinding() {
		}
		public ParameterBinding(string parameterName, Parameter parameter) {
			this.ParameterName = parameterName;
			this.parameter = parameter;
		}
		public ParameterBinding(string parameterName, object dataSource, string dataMember) {
			this.ParameterName = parameterName;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		bool ShouldSerializeDataSource() {
			return SerializeDataSource != null && Parameter == null;
		}
		bool ShouldSerializeDataMember() {
			return !string.IsNullOrEmpty(DataMember);
		}
		internal void AdjustDataSourse() {
			if(dataSource == ParentDataSource || dataSource == null) {
				dataSource = null;
			}
		}
	}
	public class ParameterBindingCollection : Collection<ParameterBinding> {
		XRSubreport parent;
		internal XRSubreport Parent { get { return parent; } }
		public ParameterBindingCollection(XRSubreport parent) {
			this.parent = parent;
		}
		protected override void InsertItem(int index, ParameterBinding item) {
			base.InsertItem(index, item);
			item.Collection = this;
		}
	}
}
namespace DevExpress.XtraReports.Native {
	using System.Linq;
	public class ParameterBindingHelper {
		ParameterCollection parameters;
		ParameterBindingCollection bindings;
		public ParameterBindingHelper(ParameterCollection parameters, ParameterBindingCollection bindings) {
			this.parameters = parameters;
			this.bindings = bindings;
		}
		public ParameterBinding[] GetMissedBindings() {
			List<ParameterBinding> list = new List<ParameterBinding>();
			foreach(IParameter parameter in parameters)
				if(!string.IsNullOrEmpty(parameter.Name) && !bindings.Any(b => b.ParameterName == parameter.Name))
					list.Add(new ParameterBinding(parameter.Name, null, null));
			return list.ToArray();
		}
		public ParameterBinding[] GetOddBindings() {
			List<ParameterBinding> list = new List<ParameterBinding>();
			foreach(ParameterBinding binding in bindings)
				if(!Enumerable.Any<IParameter>(parameters, p => binding.ParameterName == p.Name))
					list.Add(binding);
			return list.ToArray();
		}
	}
}
namespace DevExpress.XtraReports.Design {
	public class ParameterNameConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return value is string ? value : base.ConvertFrom(context, culture, value);
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return GetStandardValuesCore(context) ?? base.GetStandardValues(context);
		}
		StandardValuesCollection GetStandardValuesCore(ITypeDescriptorContext context) {
			if(context == null)
				return null;
			ParameterBinding binding = context.Instance as ParameterBinding;
			if(binding == null
				|| binding.Owner == null
				|| binding.Owner.ReportSource == null)
				return null;
			ParameterCollection parameterCollection = binding.Owner.ReportSource.Parameters;
			List<string> parameters = new List<string>(parameterCollection.Count);
			foreach(IParameter parameter in parameterCollection)
				parameters.Add(parameter.Name);
			return new StandardValuesCollection(parameters);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null
				&& context.Instance != null
				&& context.Instance is ParameterBinding;
		}
	}
}

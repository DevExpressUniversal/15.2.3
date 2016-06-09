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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Data;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native {
	public class UniqueDataSourceEnumerator {
		public event ExceptionEventHandler ExceptionOccurred;
		public IEnumerable<object> EnumerateDataSources(XtraReport report, bool includeSubreports) {
			List<object> dataSources = new List<object>();
			var enumerator = new DataContainerEnumerator { IncludeSubreports = includeSubreports };
			enumerator.ExceptionOccurred += enumerator_ExceptionOccurred;
			foreach(IDataContainerBase dataContainer in enumerator.EnumerateDataContainers(report)) {
				if(dataContainer.DataSource == null || dataSources.Contains(dataContainer.DataSource))
					continue;
				dataSources.Add(dataContainer.DataSource);
				yield return dataContainer.DataSource;
			}
			foreach(var dataSource in report.ComponentStorage.Where(BindingHelper.IsDataSource)) {
				if(dataSources.Contains(dataSource))
					continue;
				dataSources.Add(dataSource);
				yield return dataSource;
			}
		}
		void enumerator_ExceptionOccurred(object sender, ExceptionEventArgs args) {
			if(ExceptionOccurred != null) {
				ExceptionOccurred(this, args);
			} else {
				args.Handled = true;
			}
		}
	}
	public class UniqueDataContainerEnumerator {
		public IEnumerable<IDataContainerBase> EnumerateDataContainers(XtraReport report, bool includeSubreports) {
			List<MultiKey> dataSources = new List<MultiKey>();
			DataContainerEnumerator enumerator = new DataContainerEnumerator() { IncludeSubreports = includeSubreports };
			foreach(IDataContainerBase dataContainer in enumerator.EnumerateDataContainers(report)) {
				if(dataContainer.DataSource == null) continue;
				MultiKey key = dataContainer is IDataContainer && ((IDataContainer)dataContainer).DataAdapter != null ?
					new MultiKey(dataContainer.DataSource, GetDataMember(dataContainer), ((IDataContainer)dataContainer).DataAdapter) :
					new MultiKey(dataContainer.DataSource, GetDataMember(dataContainer));
				if(dataSources.Contains(key)) continue;
				dataSources.Add(key);
				yield return dataContainer;
			}
		}
		static string GetDataMember(IDataContainerBase dataContainer) {
			return dataContainer.DataSource is IListAdapter ? string.Empty : dataContainer.DataMember;
		}
	}
	public class DataContainerEnumerator {
		public event ExceptionEventHandler ExceptionOccurred;
		public bool IncludeSubreports { get; set; }
		public IEnumerable<IDataContainerBase> EnumerateDataContainers(XtraReport report) {
			Guard.ArgumentNotNull(report, "report");
			foreach(Band band in report.EnumBandsRecursive()) {
				if(band is IDataContainerBase)
					yield return (IDataContainerBase)band;
				foreach(XRControl conrtol in new NestedComponentEnumerator(band.Controls)) {
					if(conrtol is IDataContainerBase)
						yield return (IDataContainerBase)conrtol;
					if(IncludeSubreports && conrtol is SubreportBase)
						foreach(IDataContainerBase dataContainer in EnumerateSubreport(conrtol as SubreportBase))
							yield return dataContainer;
					foreach(XRBinding binding in conrtol.DataBindings)
						yield return binding;
					if(conrtol is XRSubreport)
						foreach(ParameterBinding binding in ((XRSubreport)conrtol).ParameterBindings)
							yield return binding;
				}
			}
			foreach(var formattingRule in report.FormattingRules)
				yield return formattingRule;
			foreach(var calculatedField in report.CalculatedFields) {
				yield return calculatedField;
			}
		}
		protected virtual IEnumerable<IDataContainerBase> EnumerateSubreport(SubreportBase subreport) {
			Guard.ArgumentNotNull(subreport, "subreport");
			try {
				subreport.ForceReportSource();
			} catch(Exception e) {
				var handled = false;
				if(ExceptionOccurred != null) {
					var eventArgs = new ExceptionEventArgs(e);
					ExceptionOccurred(this, eventArgs);
					handled = eventArgs.Handled;
				}
				if(!handled)
					throw;
			}
			if(subreport.ReportSource != null) {
				foreach(var dataContainer in EnumerateDataContainers(subreport.ReportSource))
					yield return dataContainer;
			}
		}
	}
	public class NestedParameterCollectorBase {
		protected readonly List<IParameterSupplier> parameterSuppliers = new List<IParameterSupplier>();
		protected IEnumerable<Parameter> GetParameters(IParameterSupplier parameterSupplier) {
			if(parameterSupplier == null || parameterSuppliers.Contains(parameterSupplier))
				yield break;
			parameterSuppliers.Add(parameterSupplier);
			foreach(var parameter in parameterSupplier.GetParameters())
				yield return parameter;
		}
	}
	public class NestedParameterCollector : NestedParameterCollectorBase {
		public virtual IEnumerable<Parameter> EnumerateParameters(XtraReport report) {
			Guard.ArgumentNotNull(report, "report");
			parameterSuppliers.Clear();
			DataContainerEnumerator enumerator = new ParameterContainerEnumerator() { IncludeSubreports = true };
			foreach(IDataContainerBase dataContainer in enumerator.EnumerateDataContainers(report)) {
				foreach(Parameter param in GetParameters(dataContainer as IParameterSupplier))
					yield return param;
				foreach(Parameter param in GetParameters(dataContainer.DataSource as IParameterSupplier))
					yield return param;
			}
		}
	}
	public class ParameterContainerEnumerator : DataContainerEnumerator {
		class BoundParameterFilter : IDataContainerBase, IParameterSupplier {
			IDataContainerBase container;
			XRSubreport subreport;
			IParameterSupplier ParameterSupplier { get { return (IParameterSupplier)container; } }
			public BoundParameterFilter(IDataContainerBase container, XRSubreport subreport) {
				this.container = container;
				this.subreport = subreport;
			}
			public ICollection<Parameter> GetParameters() {
				return ParameterSupplier.GetParameters().Where(parameter => HasNoParameterBindings(parameter)).ToArray();
			}
			public IEnumerable<IParameter> GetIParameters() {
				return ParameterSupplier.GetIParameters().Where(parameter => HasNoParameterBindings(parameter));
			}
			bool HasNoParameterBindings(IParameter parameter) {
				var binding = subreport.ParameterBindings.FirstOrDefault(b => b.ParameterName == parameter.Name);
				return binding == null || binding.IsEmpty;
			}
			string IDataContainerBase.DataMember {
				get { return container.DataMember; }
				set { container.DataMember = value; }
			}
			object IDataContainerBase.DataSource {
				get { return container.DataSource; }
				set { container.DataSource = value; }
			}
			public override int GetHashCode() {
				return container.GetHashCode();
			}
			public override bool Equals(object obj) {
				return obj is BoundParameterFilter && ((BoundParameterFilter)obj).container == container;
			}
		}
		protected override IEnumerable<IDataContainerBase> EnumerateSubreport(SubreportBase subreport) {
			foreach(IDataContainerBase item in base.EnumerateSubreport(subreport)) {
				if(item == subreport.ReportSource && subreport is XRSubreport)
					yield return new BoundParameterFilter(item, (XRSubreport)subreport);
				else 
					yield return item;
			}
		}
	}
}

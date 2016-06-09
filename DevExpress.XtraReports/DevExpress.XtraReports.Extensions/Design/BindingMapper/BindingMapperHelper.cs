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
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design.BindingMapper {
	public class BindingMapperHelper {
		#region Nested Types
		enum MappingCondition { Always, IfContainsInvalidBindings }
		#endregion
		public void ProcessReport(XtraReport report) {
			ProcessReportCore(report, MappingCondition.Always);
		}
		public void ProcessReportIfContainsInvalidBindings(XtraReport report) {
			ProcessReportCore(report, MappingCondition.IfContainsInvalidBindings);
		}
		void ProcessReportCore(XtraReport report, MappingCondition mappingCondition) {
			IServiceProvider serviceProvider = report.Site as IServiceProvider;
			var bindingMapInfos = CollectBindingMapInfos(report, serviceProvider);
			if(mappingCondition == MappingCondition.Always || mappingCondition == MappingCondition.IfContainsInvalidBindings && bindingMapInfos.Any(x => !x.IsValid))
				MapBindings(report, serviceProvider, bindingMapInfos);
		}
		void MapBindings(XtraReport report, IServiceProvider serviceProvider, IList<BindingMapInfo> bindingMapInfos) {
			using(BindingMapperForm form = CreateBindingMapperForm(report, serviceProvider, bindingMapInfos)) {
				form.SetLookAndFeel(serviceProvider);
				if(DialogRunner.ShowDialog(form, serviceProvider) == DialogResult.OK) {
					bool showOnlyInvalidBinding = form.ShowOnlyInvalidBindings;
					IDesignerHost designerHost = serviceProvider.GetService<IDesignerHost>();
					using(DesignerTransaction transaction = designerHost.CreateTransaction("Edit bindings")) {
						try {
							IComponentChangeService componentChangeService = designerHost.GetService<IComponentChangeService>();
							foreach(BindingMapInfo bindingMapInfo in bindingMapInfos) {
								if(bindingMapInfo.IsChecked) {
									if(showOnlyInvalidBinding && bindingMapInfo.IsValid)
										continue;
									AcceptChanges(bindingMapInfo, componentChangeService);
								}
							}
						} catch {
							transaction.Cancel();
						} finally {
							transaction.Commit();
						}
					}
				}
			}
		}
		protected virtual BindingMapperForm CreateBindingMapperForm(XtraReport report, IServiceProvider serviceProvider, IList<BindingMapInfo> bindingMapInfos) {
			return new BindingMapperForm(report, serviceProvider, bindingMapInfos);
		}
		static IList<BindingMapInfo> CollectBindingMapInfos(XtraReport report, IServiceProvider serviceProvider) {
			List<BindingMapInfo> bindingMapInfos = new List<BindingMapInfo>();
			NestedComponentEnumerator enumerator = new NestedComponentEnumerator(new[] { report });
			IDataContextService serv = serviceProvider.GetService<IDataContextService>();
			XRDataContextBase dataContext = serv.CreateDataContext(new DataContextOptions(true, true)) as XRDataContextBase;
			while(enumerator.MoveNext()) {
				XRControl control = enumerator.Current;
				string controlName = string.Format("{0} {{{1}}}", control.Name, GetTypeName(control.GetType()));
				foreach(XRBinding binding in control.DataBindings) {
					BindingInfo source = new XRBindingInfo(serviceProvider, binding);
					string propertyName = GetPropertyName(control, binding.PropertyName);
					if(binding.IsValidDataSource(dataContext)) {
						DesignBindingInfo destination = new DesignBindingInfo(serviceProvider, binding.DataSource, binding.DataMember);
						bindingMapInfos.Add(new BindingMapInfo(false, true, controlName, propertyName, source, destination, control));
					} else {
						DesignBindingInfo destination = new DesignBindingInfo(serviceProvider);
						bindingMapInfos.Add(new BindingMapInfo(false, false, controlName, propertyName, source, destination, control));
					}
				}
				IDataContainer dataContainer = control as IDataContainer;
				if(dataContainer != null && !string.IsNullOrEmpty(dataContainer.DataMember)) {
					object dataSource = dataContainer.GetEffectiveDataSource();
					string propertyName = GetPropertyName(control, XRComponentPropertyNames.DataMember);
					BindingInfo source = new DataMemberInfo(serviceProvider, dataContainer);
					if(dataContext.IsDataMemberValid(dataSource, dataContainer.DataMember)) {
						DesignBindingInfo destination = new DesignDataMemberInfo(serviceProvider, dataSource, dataContainer.DataMember);
						bindingMapInfos.Add(new BindingMapInfo(false, true, controlName, propertyName, source, destination, control));
					} else {
						DesignBindingInfo destination = new DesignDataMemberInfo(serviceProvider);
						bindingMapInfos.Add(new BindingMapInfo(false, false, controlName, propertyName, source, destination, control));
					}
				}
			}
			return bindingMapInfos;
		}
		static string GetTypeName(Type type) {
			DisplayNameAttribute attribute = GetAttribute<DisplayNameAttribute>(TypeDescriptor.GetAttributes(type));
			return (attribute != null) ? attribute.DisplayName : type.Name;
		}
		static string GetPropertyName(object component, string propertyName) {
			PropertyDescriptor property = TypeDescriptor.GetProperties(component)[propertyName];
			if(property != null) {
				DisplayNameAttribute attribute = GetAttribute<DisplayNameAttribute>(property.Attributes);
				if(attribute != null)
					return attribute.DisplayName;
			}
			return propertyName;
		}
		static T GetAttribute<T>(AttributeCollection attributes) where T : Attribute {
			return attributes[typeof(T)] as T;
		}
		static void AcceptChanges(BindingMapInfo bindingMapInfo, IComponentChangeService componentChangeService) {
			PropertyDescriptor property = XRAccessor.GetPropertyDescriptor(bindingMapInfo.Control, bindingMapInfo.Source.PropertyName);
			componentChangeService.OnComponentChanging(bindingMapInfo.Control, property);
			bindingMapInfo.Source.AssignFrom(bindingMapInfo.Destination.DesignBinding);
			componentChangeService.OnComponentChanged(bindingMapInfo.Control, property, null, null);
		}
	}
}

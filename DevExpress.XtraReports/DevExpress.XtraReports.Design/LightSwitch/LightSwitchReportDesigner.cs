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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design.LightSwitch {
	public class LightSwitchReportDesigner : _ReportDesigner {		
		#region inner classes
		class LSDataContextServiceDesign : DesignTimeDataContextService {
			public LSDataContextServiceDesign(IDesignerHost host) : base(host) {
			}
			protected override PropertyDescriptor[] FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext) {
				PropertyDescriptor[] filteredProperties = base.FilterProperties(properties, dataSource, dataMember, dataContext);
				return LSPropertiesFilter.FilterProperties(filteredProperties, dataSource, dataMember, dataContext);
			}
		}
		class ReportTabControlLS : ReportTabControl {
			public ReportTabControlLS(ReportDesigner reportDesigner)
				: base(reportDesigner) {
			}
			protected override void AddScriptsPage() {
			}
		}
		class PropertyFilterService : IPropertyFilterService {
			public void PreFilterProperties(System.Collections.IDictionary properties, object component) {
				LightSwitchReportDesigner.RemoveValues(properties, 
					XRComponentPropertyNames.DataAdapter,
					XRComponentPropertyNames.Scripts
					);
				if(properties.Contains(XRComponentPropertyNames.DataSource)) {
					PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor)properties[XRComponentPropertyNames.DataSource];
					properties[XRComponentPropertyNames.DataSource] = XRAccessor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor,
							new Attribute[] { new EditorAttribute("DevExpress.XtraReports.Design.LightSwitch.DataSourceEditorLS," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(UITypeEditor)) });
				}
			}
			public void PreFilterEvents(IDictionary events, object component) {
				LightSwitchReportDesigner.RemoveValues(events,
					XRControl.EventNames.PreviewClick,
					XRControl.EventNames.PreviewDoubleClick,
					XRControl.EventNames.PreviewMouseMove,
					XRControl.EventNames.PreviewMouseDown,
					XRControl.EventNames.PreviewMouseUp
					);
			}
		}
		class LSNewParameterEditorForm : VSNewParameterEditorForm {
			public LSNewParameterEditorForm(IDesignerHost designerHost) : base(designerHost) { }
			[Editor(typeof(DataSourceEditorLS), typeof(UITypeEditor)), TypeConverter(typeof(DevExpress.Data.Design.DataSourceConverter))]
			public override object DataSource {
				get {
					return base.DataSource;
				}
				set {
					base.DataSource = value;
				}
			}
			protected override string[] GetDynamicLookUpParameterPropertyNames() {
				return new[] { "DataSource", "ValueMember", "DisplayMember" };
			}
		}
		#endregion
		protected override void OnAbout() {
		}
		public override TypePickerBase CreateReportSourcePicker() {
			return new ReportSourcePickerVS(typeof(LightSwitchReport));
		}
		protected override void ValidateLicence() {
		}
		protected override void InitializeCore(IComponent component) {
			base.InitializeCore(component);
			OnResetToolbox();
		}
		protected override IPropertyFilterService CreatePropertyFilterService() {
			return new PropertyFilterService();
		}
		protected override void PreFilterProperties(System.Collections.IDictionary properties) {
			base.PreFilterProperties(properties);
			RemoveValues(properties, 
				XRComponentPropertyNames.ScriptSecurityPermissions,
				XRComponentPropertyNames.ScriptReferences,
				XRComponentPropertyNames.ScriptLanguage,
				XRComponentPropertyNames.XmlDataPath,
				XRComponentPropertyNames.DataSourceSchema
				);
		}
		protected override void PreFilterEvents(System.Collections.IDictionary events) {
			base.PreFilterEvents(events);
			RemoveValues(events,
				XRControl.EventNames.ParametersRequestBeforeShow,
				XRControl.EventNames.ParametersRequestSubmit,
				XRControl.EventNames.ParametersRequestValueChanged,
				XRControl.EventNames.DesignerLoaded,
				XRControl.EventNames.PrintProgress,
				XRControl.EventNames.SaveComponents
				);
		}
		static void RemoveValues(System.Collections.IDictionary events, params string[] keys) {
			foreach(string key in keys)
				if(events.Contains(key)) events.Remove(key);
		}
		protected override void FillReportToolShell(IServiceProvider srvProvider, IToolShell toolShell) {
			toolShell.AddToolItem(new ReportFieldListItem(srvProvider, "Field List"));
			toolShell.AddToolItem(new ReportExplorerToolItem(srvProvider, "Report Explorer"));
			toolShell.AddToolItem(new GroupAndSortToolItem(srvProvider, "Group and Sort"));
			toolShell.AddToolItem(new ReportFormattingBar(srvProvider));
			toolShell.AddToolItem(new ReportMenu(srvProvider));
		}
		protected override ReportTabControl CreateReportTabControl() {
			return new ReportTabControlLS(this);
		}
		protected override IDataContextService CreateDataContextService() {
			return new LSDataContextServiceDesign(fDesignerHost);
		}
		protected override NewParameterEditorForm CreateNewParameterEditorForm() {
			return new LSNewParameterEditorForm(fDesignerHost);
		}
	}
}

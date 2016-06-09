#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb.Native;
using DevExpress.Design.VSIntegration;
using DevExpress.Web.Design;
using DevExpress.Web.Design.WebClientUIControl;
using DevExpress.Web.WebClientUIControl.Internal;
using EnvDTE;
namespace DevExpress.DashboardWeb.Design {
	public class ASPxDashboardViewerDesigner : ASPxWebUIControlDesigner, IServiceProvider {
		const string csFormatString = "typeof({0})";
		const string vbFormatString = "GetType({0})";
		const string DashboardSourcePropertyName = "DashboardSource";
		const string DashboardTypePropertyName = "DashboardType";
		const string DashboardTypeNamePropertyName = "DashboardTypeName";
		const string DashboardXmlFilePropertyName = "DashboardXmlFile";
		public static object GetBindingValue(string expression) {
			try {
				string typeName = expression;
				if(expression == null)
					return null;
				string csCodeStrart = string.Format(csFormatString, "");
				csCodeStrart = csCodeStrart.Substring(0, csCodeStrart.Length - 1);
				if(expression.StartsWith(csCodeStrart))
					typeName = expression.Substring(csCodeStrart.Length, expression.Length - csCodeStrart.Length - 1);
				string vbCodeStrart = string.Format(vbFormatString, "");
				vbCodeStrart = vbCodeStrart.Substring(0, vbCodeStrart.Length - 1);
				if(expression.StartsWith(vbCodeStrart))
					typeName = expression.Substring(vbCodeStrart.Length, expression.Length - vbCodeStrart.Length - 1);
				return Type.GetType(typeName);
			} catch {
			}
			return expression;
		}
		static string GetExpression(ProjectItem projectItem, string typeName) {
			if(projectItem != null && projectItem.FileCodeModel != null) {
				switch(projectItem.FileCodeModel.Language.ToUpper()) {
					case CodeModelLanguageConstants.vsCMLanguageCSharp:
					case "{E6FDF8BF-F3D1-11D4-8576-0002A516ECE8}":
						return string.Format(csFormatString, typeName);
					case CodeModelLanguageConstants.vsCMLanguageVB:
						return string.Format(vbFormatString, typeName);
				}
			} else if(projectItem != null && projectItem.FileCodeModel == null)
				return string.Format(csFormatString, typeName);
			return null;
		}
		ASPxDashboardViewer Viewer { get { return (ASPxDashboardViewer)Component; } }
		[
		Bindable(true),
		TypeConverter(typeof(DashboardSourceTypeConverter)),
		Editor("DevExpress.DashboardWin.Design.DashboardSourceUrlEditor," + AssemblyInfo.SRAssemblyDashboardWinDesign, typeof(UITypeEditor)),
		DefaultValue("")
		]
		public object DashboardSource {
			get {
				DataBinding binding = DataBindings[DashboardSourcePropertyName];
				if(binding != null && !string.IsNullOrEmpty(binding.Expression))
					return GetBindingValue(binding.Expression);
				object value = Viewer.DashboardSource;
				string str = value as string;
				if(str == null)
					return value;
				if(!DashboardSourceTypeConverter.IsXmlPath(str) && !File.Exists(str))
					return DashboardSourceTypeConverter.DashboardSourceFromString(Viewer, str, this) ?? value;
				return value;
			}
			set {
				Type type = value as Type;
				string str = value as string;
				if(type == null)
					type = DashboardSourceTypeConverter.DashboardSourceFromString(Viewer, str, this) as Type;
				if(type != null) {
					string name = type.FullName;
					DataBinding binding = DataBindings[DashboardSourcePropertyName];
					string expression = GetExpression((ProjectItem)GetService(typeof(ProjectItem)), name);
					if(binding == null)
						binding = new DataBinding(DashboardSourcePropertyName, typeof(object), expression);
					else
						binding.Expression = expression;
					DataBindings.Add(binding);
#pragma warning disable 0618
					OnBindingsCollectionChanged(DashboardSourcePropertyName);				   
#pragma warning restore 0618
				} else {
					base.DataBindings.Remove(DashboardSourcePropertyName);
#pragma warning disable 0618
					OnBindingsCollectionChanged(DashboardSourcePropertyName);
#pragma warning restore 0618
					Viewer.DashboardSource = str;
					PropertyChanged(DashboardTypePropertyName);
					PropertyChanged(DashboardTypeNamePropertyName);
					PropertyChanged(DashboardXmlFilePropertyName);
					PropertyChanged(DashboardSourcePropertyName);
				}
			}
		}
		public override void OnComponentChanged(object sender, System.ComponentModel.Design.ComponentChangedEventArgs ce) {
			if(ce.Member == null || ce.Member.Name != DashboardSourcePropertyName || !string.IsNullOrEmpty(Viewer.DashboardSource as string))
				base.OnComponentChanged(sender, ce);
			else
				UpdateDesignTimeHtml();
		}
		static ASPxDashboardViewerDesigner() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
		}
		public ASPxDashboardViewerDesigner() : base() {
		}
		public override void Initialize(System.ComponentModel.IComponent component) {
			base.Initialize(component);
		}
		protected override ClientControlsDesignModeInfo CreateClientControlsDesignModeInfo() {
			ClientControlsDesignModeInfo info = null;
			object source = null;
			IDataBindingsAccessor accessor = Viewer;
			DataBinding binding = accessor.DataBindings[DashboardSourcePropertyName];
			if(binding != null && !string.IsNullOrEmpty(binding.Expression))
				source = GetBindingValue(binding.Expression);
			if(source == null)
				source = DashboardSource;
			string sourceStr = source as string;
			if(source != null) {
				string xmlPath = DesignUtils.MapPath(DesignerHost, sourceStr ?? string.Empty);
				if(!string.IsNullOrEmpty(xmlPath) && (DashboardSourceTypeConverter.IsXmlPath(xmlPath) || File.Exists(xmlPath))) {
					bool haveFile = true;
					try {
						FileStream stream = File.OpenRead(xmlPath);
						stream.Dispose();
					}
					catch {
						haveFile = false;
					}
					if(haveFile)
						info = ASPxDashboardViewerDesignModeInfoGenerator.GetDashboardXmlSpecifiedInfo(xmlPath);
					else
						info = ASPxDashboardViewerDesignModeInfoGenerator.GetDashboardXmlNotFoundInfo();
				}
				else {
					Type type = source as Type;
					if(type == null || !typeof(Dashboard).IsAssignableFrom(type))
						if(type == null && !IsProjectType(sourceStr))
							info = ASPxDashboardViewerDesignModeInfoGenerator.GetDashboardClassNameInvalidInfo();
						else
							info = ASPxDashboardViewerDesignModeInfoGenerator.GetDashboardClassSpecifiedInfo(source.ToString());
					else
						info = ASPxDashboardViewerDesignModeInfoGenerator.GetDashboardClassSpecifiedInfo(type.Name);
				}
			}
			else
				info = ASPxDashboardViewerDesignModeInfoGenerator.GetDashboardNotSpecifiedInfo();
			return info;
		}
		bool IsProjectType(string typeName) {
			if(string.IsNullOrEmpty(typeName))
				return false;
			return Enumerable.Contains(new DTEServiceBase(this).GetClassesInfo(typeof(Dashboard), new string[] { }), typeName);
		}
		public override bool HasClientSideEvents() {
			return true;
		}
		public override bool HasCommonDesigner() {
			return false;
		}
		public override bool IsThemableControl() {
			return false;
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxDashboardViewerDesignerActionList(this);
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(
				AssemblyInfo.SRAssemblyChartsCore,
				AssemblyInfo.SRAssemblyData,
				AssemblyInfo.SRAssemblyDataAccess,
				AssemblyInfo.SRAssemblyMapCore,
				AssemblyInfo.SRAssemblyOfficeCore,
				AssemblyInfo.SRAssemblyPivotGridCore,
				AssemblyInfo.SRAssemblyPrintingCore,
				AssemblyInfo.SRAssemblyRichEditCore,
				AssemblyInfo.SRAssemblySparklineCore,
				AssemblyInfo.SRAssemblyUtils,				
				AssemblyInfo.SRAssemblyXpo,
				AssemblyInfo.SRAssemblyCharts,
				AssemblyInfo.SRAssemblyGaugesCore,
				AssemblyInfo.SRAssemblyGaugesPresets,
				AssemblyInfo.SRAssemblyGaugesWin,
				AssemblyInfo.SRAssemblyMap,
				AssemblyInfo.SRAssemblyReports,				
				AssemblyInfo.SRAssemblyDashboardCore);
		}
		public override void ShowAbout() {
			WebDashboardAboutDialogHelper.ShowAbout(Component.Site);
		}
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		protected override void PreFilterProperties(IDictionary properties) {
			SetDesignProperty(properties, DashboardSourcePropertyName);
			base.PreFilterProperties(properties);
		}
		void SetDesignProperty(IDictionary properties, string propName) {
			PropertyDescriptor propDesc = CreateProperty(propName, new Attribute[] { CategoryAttribute.Design } );
			if(propDesc != null)
				properties[propName] = propDesc;
		}
		PropertyDescriptor CreateProperty(string propName, Attribute[] attributes) {
			PropertyInfo property = this.GetType().GetProperty(propName);
			return (property != null) ? TypeDescriptor.CreateProperty(this.GetType(), propName, property.PropertyType, attributes) : null;
		}
	}
}

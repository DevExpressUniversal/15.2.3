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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Design;
using DevExpress.Design.Entity;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking.Design;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using EnvDTE;
using System.Reflection;
namespace DevExpress.XtraReports.Design {
	public class _XRPivotGridDesigner : XRPivotGridDesigner {
		public _XRPivotGridDesigner() : base() { }
		protected override void ValidateDataAdapter() {
			new VSDataContainerDataAdapterHelper(this).ValidateDataAdapter();
		}
	}
	public class _XRCrossBandControlDesigner : XRCrossBandControlDesigner {
		public _XRCrossBandControlDesigner() : base() { }
	}
	public class _XRCrossBandLineDesigner : XRCrossBandLineDesigner {
		public _XRCrossBandLineDesigner() : base() { }
	}
	public class _BandDesigner : BandDesigner {
		public _BandDesigner() : base() { }
	}
	public class _XRControlDesigner : XRControlDesigner {
		public _XRControlDesigner() : base() { }
	}
	public class _XRLabelDesigner : XRLabelDesigner {
		public _XRLabelDesigner() : base() { }
	}
	public class _XRCrossBandBoxDesigner : XRCrossBandBoxDesigner {
		public _XRCrossBandBoxDesigner() : base() { }
	}
	public class _XRCheckBoxDesigner : XRCheckBoxDesigner {
		public _XRCheckBoxDesigner() : base() { }
	}
	public static class SubreportDesignerHelper {
		public static void ShowReportSourceDesigner(XtraReport reportSource, IServiceProvider provider) {
			if(reportSource != null) {
				CodeElement codeElement = GetCodeElement(reportSource.GetType().FullName, provider);
				if(codeElement != null) {
					Window wind = codeElement.ProjectItem.Open(Constants.vsViewKindPrimary);
					if(wind != null)
						wind.Visible = true;
				}
			}
		}
		static CodeElement GetCodeElement(string typeName, IServiceProvider provider) {
			ProjectItem projectItem = (ProjectItem)provider.GetService(typeof(ProjectItem));
			if(projectItem != null) {
				Project project = projectItem.ContainingProject;
				projectItem = null;
				CodeType[] codeElements = ProjectHelper.FindCodeElements(project.ProjectItems, typeName);
				foreach(CodeElement codeElement in codeElements) {
					if(typeName.Equals(codeElement.FullName))
						return codeElement;
				}
			}
			return null;
		}
	}
	[MouseTarget(typeof(CustomSubreportMouseTarget))]
	public class _XRSubreportDesigner : XRSubreportDesigner {
		protected override string[] GetFilteredProperties() {
			List<string> names = new List<string>(base.GetFilteredProperties());
			names.Add("ReportSourceUrl");
			return names.ToArray();
		}
	}
	class CustomSubreportMouseTarget : SubreportMouseTarget {
		public CustomSubreportMouseTarget(XRControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override void HandleDoubleClick(object sender, BandMouseEventArgs e) {
			SubreportDesignerHelper.ShowReportSourceDesigner(((XRSubreport)XRControl).ReportSource, Host);
		}
	}
	public class _XRPageBreakDesigner : XRPageBreakDesigner {
		public _XRPageBreakDesigner() : base() { }
	}
	public class _XRPageInfoDesigner : XRPageInfoDesigner {
		public _XRPageInfoDesigner() : base() { }
	}
	public class _XRPictureBoxDesigner : XRPictureBoxDesigner {
		[
		SRCategory(DevExpress.XtraReports.Localization.ReportStringId.CatAppearance),
		Bindable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Editor(typeof(Design.ImageFileNameEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue("")
		]
		public string ImageUrl {
			get { return PictureBox.ImageUrl; }
			set {
				string projectDirectory = GetProjectFullName();
				if(projectDirectory.Length > 0 && value.StartsWith(projectDirectory))
					value = "~" + value.Remove(0, projectDirectory.Length);
				PictureBox.ImageUrl = value;
			}
		}
		public _XRPictureBoxDesigner() : base() { }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			string projectDirectory = GetProjectFullName();
			if(projectDirectory != String.Empty) {
				System.Reflection.PropertyInfo pi = component.GetType().GetProperty("SourceDirectory",
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				if(pi != null)
					pi.SetValue(component, projectDirectory, null);
			}
		}
		string GetProjectFullName() {
			if(DesignerHost != null) {
				IDTEService svc = (IDTEService)DesignerHost.GetService(typeof(IDTEService));
				if(svc != null) return svc.ProjectFullName;
			}
			return String.Empty;
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(ProjectHelper.IsWebProject(DesignerHost))
				DevExpress.XtraReports.Native.PropInfoAccessor.SetDesignProperty(this, properties, XRComponentPropertyNames.ImageUrl);
		}
	}
	public class _WinControlContainerDesigner : WinControlContainerDesigner {
		public _WinControlContainerDesigner() : base() { }
	}
	public class _PrintableComponentContainerDesigner : PrintableComponentContainerDesigner {
		public _PrintableComponentContainerDesigner() : base() { }
	}
	public class _XRTableDesigner : XRTableDesigner {
		public _XRTableDesigner() : base() { }
	}
	public class _XRTableRowDesigner : XRTableRowDesigner {
		public _XRTableRowDesigner() : base() { }
	}
	public class _XRTableCellDesigner : XRTableCellDesigner {
		public _XRTableCellDesigner() : base() { }
	}
	public class _XRTableOfContentsDesigner : XRTableOfContentsDesigner {
		public _XRTableOfContentsDesigner() : base() { }
	}
	public class _XRRichTextBoxDesigner : XRRichTextBoxDesigner {
		public _XRRichTextBoxDesigner() : base() { }
	}
	public class _XRRichTextDesigner : XRRichTextDesigner {
		public _XRRichTextDesigner() : base() { }
	}
	public class _XRLineDesigner : XRLineDesigner {
		public _XRLineDesigner() : base() { }
	}
	public class _XRSparklineDesigner : XRSparklineDesigner {
		public _XRSparklineDesigner() : base() { }
	}
	public class _XRGaugeDesigner : XRGaugeDesigner {
		public _XRGaugeDesigner() : base() { }
	}
	public class _XRPanelDesigner : XRPanelDesigner {
		public _XRPanelDesigner() : base() { }
	}
	public class _XRChartDesigner : XRChartDesigner {
		public _XRChartDesigner() : base() { }
		protected override void ValidateDataAdapter() {
			new VSChartDataAdapterHelper(this).ValidateDataAdapter();
		}
	}
	public class _ComponentReportDesigner : ComponentReportDesigner {
		public _ComponentReportDesigner()
			: base() {
		}
		protected override void PreFilterProperties(IDictionary properties) {
		}
		protected override Attribute[] CreateFilterPropertyAttributes() {
			List<Attribute> attributes = new List<Attribute>(base.CreateFilterPropertyAttributes());
			attributes.Add(new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
			return attributes.ToArray();
		}
	}
	public class _DetailReportBandDesigner : DetailReportBandDesigner {
		public _DetailReportBandDesigner() : base() { }
		protected override void ValidateDataAdapter() {
			new VSDataContainerDataAdapterHelper(this).ValidateDataAdapter();
		}
		protected override string[] GetFilteredProperties() {
			return new string[0];
		}
	}
	public class _XRBarCodeDesigner : XRBarCodeDesigner {
		public _XRBarCodeDesigner() : base() { }
	}
	public class _XRShapeDesigner : XRShapeDesigner {
		public _XRShapeDesigner() : base() { }
	}
	public class _XRZipCodeDesigner : XRZipCodeDesigner {
		public _XRZipCodeDesigner() : base() { }
	}
	class XRDesignPanelDesigner : DevExpress.Utils.Design.BaseControlDesignerSimple {
		public override void InitializeNewComponent(IDictionary dictionary) {
			base.InitializeNewComponent(dictionary);
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			foreach(IComponent component in designerHost.Container.Components) {
				IDesignControl designControl = component as IDesignControl;
				if(designControl != null && designControl.XRDesignPanel == null)
					designControl.XRDesignPanel = Component as XRDesignPanel;
			}
		}
	}
	public class DesignControlContainerDesigner : ContainerControlDesigner {
		protected override bool DrawGrid {
			get { return false; }
		}
		public override bool CanParent(Control control) {
			return false;
		}
	}
	public class PropertyGridDockPanelDesigner : DevExpress.XtraBars.Docking.Design.DockPanelDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			DXDisplayNameAttribute.UseResourceManager = false;
		}
	}
}

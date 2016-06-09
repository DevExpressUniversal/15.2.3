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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.FontProperties;
using DevExpress.Xpf.Reports.UserDesigner.Layout;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportExplorer;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public abstract class XRComponentModelBase : XRObjectModelBase, IReportExplorerItem {
		protected XRComponentModelBase(XRControlModelFactory.ISource<IComponent> source, ImageSource icon)
			: base(source.XRObject, source.FactoryData) {
			if(DesignSite == null)
				throw new InvalidOperationException();
			this.icon = icon;
			this.report = source.Owner;
			this.PropertyChanged += (s, e) => OnThisPropertyChanged(e);
			var componentNameProperty = XRObject.GetType().GetProperty("Name");
			if(componentNameProperty != null) {
				this.nameProperty = CreateXRPropertyModel("Name", () => (string)componentNameProperty.GetValue(XRObject, null), x => XRObject.Site.Name = x, XRObject);
				this.nameProperty.ValueChanged += (s, e) => {
					RaisePropertyChanged(() => Name);
					Report.DiagramItem.Diagram.InvalidateRenderLayer();
				};
			}
		}
		protected virtual void OnThisPropertyChanged(PropertyChangedEventArgs e) {
		}
		protected override void OnAttachItem() {
			DesignSite.OnAddToDesignerHost();
		}
		protected override void OnDetachItem() {
			DesignSite.OnRemoveFromDesignerHost();
		}
		readonly XtraReportModelBase report;
		public XtraReportModelBase Report { get { return report; } }
		public new IComponent XRObject { get { return (IComponent)base.XRObject; } }
		protected DesignSite DesignSite { get { return (DesignSite)XRObject.Site; } }
		readonly ImageSource icon;
		public ImageSource Icon { get { return icon; } }
		public static ImageSource GetDefaultIcon(string iconName) { return ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(XRControlModelBase).Assembly, string.Format("Images/Toolbox16x16/{0}.png", iconName))); }
		public virtual bool HasDataBindings { get { return false; } }
		readonly XRPropertyModel<string> nameProperty;
		[ParenthesizePropertyName(true)]
		[SRCategory(ReportStringId.CatDesign)]
		public string Name {
			get { return nameProperty.With(x => x.Value); }
			set { nameProperty.Do(x => x.Value = value); }
		}
		string IReportExplorerItem.TypeString { get { return XRObject.GetType().Name; } }
		protected override void InitializeXRObjectIfNeeded(object xrObject, ModelFactoryData factoryData) {
			base.InitializeXRObjectIfNeeded(xrObject, factoryData);
			var xrControl = (IComponent)xrObject;
			if(xrControl.Site != null) return;
			var reportModel = (XtraReportModel)factoryData.FactoryOwner;
			DesignSite.EnableDesignMode(xrControl, factoryData.SourceIsNew, reportModel.DesignerHost);
		}
		protected override DiagramItem CreateDiagramItem() {
			return new XRDiagramLogicItem();
		}
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return nameProperty == null ? base.GetEditableProperties() : base.GetEditableProperties().InjectProperty(this, x => x.Name);
		}
	}
}

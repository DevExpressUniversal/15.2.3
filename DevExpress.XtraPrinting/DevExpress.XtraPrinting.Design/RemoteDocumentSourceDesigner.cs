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
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.ReportServer.Printing;
using DevExpress.Skins;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Design {
	public class RemoteDocumentSourceDesigner : BaseComponentDesignerSimple {
		object reportName;
		object userName;
		object password;
		DesignerVerbCollection verbs;
		RemoteDocumentSource DocumentSource { get { return (RemoteDocumentSource)Component; } }
		public override DesignerVerbCollection Verbs {
			get {
				if(verbs == null) {
					verbs = new DesignerVerbCollection();
					verbs.Add(new DesignerVerb("About", OnAbout));
					DXSmartTagsHelper.CreateDefaultVerbs(this, verbs);
				}
				return verbs;
			}
		}
		#region designTime properties
		[Editor(typeof(RemoteDocumentSourceEditor2), typeof(UITypeEditor)),
		DefaultValue(null),
		TypeConverter(typeof(ReportNameConverter))]
		public object DesignTimeReportName {
			get {
				return reportName;
			}
			internal set {
				reportName = value;
			}
		}
		[DefaultValue(null)]
		public object UserName {
			get {
				return userName;
			}
			internal set {
				userName = value;
			}
		}
		[DefaultValue(null)]
		public object Password {
			get {
				return password;
			}
			internal set {
				password = value;
			}
		}
		#endregion
		void OnAbout(object sender, EventArgs e) {
			PrintingSystem.About();
		}
		protected override DXAboutActionList GetAboutAction() { return new DXAboutActionList(Component, new MethodInvoker(PrintingSystem.About)); }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Insert(0, new ReportServerConnectionActionList(DocumentSource));
		}
		protected override void PreFilterProperties(System.Collections.IDictionary properties) {
			base.PreFilterProperties(properties);
			properties["DesignTimeReportName"] = TypeDescriptor.CreateProperty(
				typeof(RemoteDocumentSourceDesigner),
				"DesignTimeReportName",
				typeof(object),
				new DisplayNameAttribute("ReportName"),
				new CategoryAttribute(NativeSR.CatPrinting),
				new DesignOnlyAttribute(true));
			properties["UserName"] = TypeDescriptor.CreateProperty(
				typeof(RemoteDocumentSourceDesigner),
				"UserName",
				typeof(object),
				new DesignOnlyAttribute(true),
				new BrowsableAttribute(false));
			properties["Password"] = TypeDescriptor.CreateProperty(
				typeof(RemoteDocumentSourceDesigner),
				"Password",
				typeof(object),
				new DesignOnlyAttribute(true),
				new BrowsableAttribute(false));
		}
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			RemoteDocumentSourceEditor2 editor = new RemoteDocumentSourceEditor2();
			var reportNameDescriptor = TypeDescriptor.GetProperties(DocumentSource).Find("DesignTimeReportName", true);
			DesignTimeReportName = editor.EditValue(new DevExpress.XtraPrinting.Native.Lines.TypeDescriptorContext(Component.Site, reportNameDescriptor, DocumentSource), Component.Site, null);
		}
	}
	class ReportServerConnectionActionList : DesignerActionList {
		DesignerActionUIService designerActionUIService;
		public RemoteDocumentSource RemoteDocumentSource { get { return (RemoteDocumentSource)Component; } }
		public ReportServerConnectionActionList(RemoteDocumentSource source)
			: base(source) {
				designerActionUIService = source.Site.GetService<DesignerActionUIService>();
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			result.Add(new DesignerActionPropertyItem("ServiceUri", "Service Uri", NativeSR.CatPrinting));
			result.Add(new DesignerActionPropertyItem("AuthenticationType", "Authentication type", NativeSR.CatPrinting));
			result.Add(new DesignerActionPropertyItem("ReportName", "Report Name", NativeSR.CatPrinting));
			return result;
		}
		[Editor(typeof(RemoteDocumentSourceEditor2), typeof(UITypeEditor)),
		TypeConverter(typeof(ReportNameConverter)),
		DefaultValue(null)]
		public object ReportName {
			get {
				return TypeDescriptor.GetProperties(RemoteDocumentSource).Find("DesignTimeReportName", true).GetValue(RemoteDocumentSource);
			}
			set {
				TypeDescriptor.GetProperties(RemoteDocumentSource).Find("DesignTimeReportName", true).SetValue(RemoteDocumentSource, value);
				designerActionUIService.Refresh(RemoteDocumentSource);
			}
		}
		[TypeConverter(typeof(EnumConverter))]
		[DefaultValue(DevExpress.ReportServer.Printing.AuthenticationType.None)]
		public AuthenticationType AuthenticationType {
			get {
				return (AuthenticationType)TypeDescriptor.GetProperties(RemoteDocumentSource).Find("AuthenticationType", true).GetValue(RemoteDocumentSource);
			}
			set {
				TypeDescriptor.GetProperties(RemoteDocumentSource).Find("AuthenticationType", true).SetValue(RemoteDocumentSource, value);
			}
		}
		[DefaultValue(null)]
		[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public string ServiceUri {
			get {
				return (string)TypeDescriptor.GetProperties(RemoteDocumentSource).Find("ServiceUri", true).GetValue(RemoteDocumentSource);
			}
			set {
				try {
					TypeDescriptor.GetProperties(RemoteDocumentSource).Find("ServiceUri", true).SetValue(RemoteDocumentSource, value);
				} catch(Exception e) {
					ShowError(e);
				} finally {
					designerActionUIService.Refresh(RemoteDocumentSource);
				}
			}
		}
		void ShowError(Exception e) {
			DevExpress.XtraEditors.XtraMessageBox.Show(RemoteDocumentSource.Site.GetService<IDesignerHost>().GetOwnerWindow(), e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		[DefaultValue(null)]
		public string EndpointConfigurationPrefix {
			get {
				return (string)TypeDescriptor.GetProperties(RemoteDocumentSource).Find("EndpointConfigurationPrefix", true).GetValue(RemoteDocumentSource);
			}
			set {
				TypeDescriptor.GetProperties(RemoteDocumentSource).Find("EndpointConfigurationPrefix", true).SetValue(RemoteDocumentSource, value);
			}
		}
	}
	public class ReportNameConverter : TypeConverter {
		const string none = "(none)";
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			var reportName = value as string;
			if(string.IsNullOrEmpty(reportName))
				return none;
			return reportName;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
}

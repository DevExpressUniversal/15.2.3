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
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Design.Reports;
using DevExpress.Web.Design.Reports.Viewer;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Web.Design {
	public class ReportViewerDesigner : ASPxWebControlDesigner {
		#region inner classes
		static class Names {
			public static string
				AutoSize = GetViewerPropertyName(x => x.AutoSize),
				Width = GetViewerPropertyName(x => x.Width),
				Height = GetViewerPropertyName(x => x.Height),
				FileStore = GetDesignerPropertyName(x => x.FileStore);
			static string GetViewerPropertyName(Expression<Func<ReportViewer, object>> propertyExpression) {
				return Names.GetPropertyName(propertyExpression);
			}
			static string GetDesignerPropertyName(Expression<Func<ReportViewerDesigner, object>> propertyExpression) {
				return Names.GetPropertyName(propertyExpression);
			}
			static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression) {
				return ExpressionHelper.GetPropertyName(propertyExpression);
			}
		}
		#endregion
		static readonly FileStore fileStore = new FileStore();
		ReportViewer reportViewer;
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public FileStore FileStore {
			get { return fileStore; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			fileStore.SetServiceProvider(GetService<IDesignerHost>());
			reportViewer = (ReportViewer)component;
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			XtraReportsAssembliesInsurer.EnsureControlReferences(DesignerHost);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ReportViewerActionList(this);
		}
		public override void ShowAbout() {
			XtraReportAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(e.Member != null && e.Member.Name == Names.AutoSize && reportViewer.AutoSize) {
				SetComponentProperty(Names.Width, Unit.Empty);
				SetComponentProperty(Names.Height, Unit.Empty);
			}
			base.OnComponentChanged(sender, e);
			RemoveObsoleteAttributes();
			if(e.Member != null && e.Member.Name == Names.FileStore) {
				fileStore.SaveNewValues();
			}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			PropInfoAccessor.SetDesignProperty(this, properties, Names.FileStore);
			base.PreFilterProperties(properties);
		}
		protected override string GetEmptyDesignTimeHtmlInternal() {
			var placeHolder = new WebControl(HtmlTextWriterTag.Div);
			ReportViewer.AssignStyles(reportViewer, placeHolder);
			if(reportViewer.BorderStyle == BorderStyle.None || (reportViewer.BorderStyle == BorderStyle.NotSet && reportViewer.BorderWidth == Unit.Empty)) {
				placeHolder.BorderStyle = BorderStyle.Dashed;
				placeHolder.BorderColor = Color.Silver;
				placeHolder.BorderWidth = 1;
			}
			placeHolder.Style.Add("font", "messagebox");
			const string ReportAssignHint = "To set up a report to display, please use the Report property.";
			const string AutoSizeHint = "<br>&nbsp;To manually change the viewer's height and width, set the AutoSize property to False.";
			var literalContent = string.Format(
				"<span style=\"font-weight:bold\">&nbsp;{0}</span> - {1}<br>&nbsp;{2}{3}",
				reportViewer.GetType().Name,
				reportViewer.Site.Name,
				ReportAssignHint,
				reportViewer.AutoSize ? AutoSizeHint : string.Empty);
			placeHolder.Controls.Add(new LiteralControl(literalContent));
			using(var stringWriter = new StringWriter()) {
				placeHolder.RenderControl(new HtmlTextWriter(stringWriter));
				return stringWriter.ToString();
			}
		}
		void RemoveObsoleteAttributes() {
			Tag.SetDirty(true);
			Tag.RemoveAttribute("BorderColor");
			Tag.RemoveAttribute("BorderWidth");
			Tag.RemoveAttribute("BorderStyle");
		}
		void SetComponentProperty(string propertyName, object value) {
			TypeDescriptor.GetProperties(reportViewer)[propertyName].SetValue(reportViewer, value);
		}
	}
}

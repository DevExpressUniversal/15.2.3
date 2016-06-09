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

using System.ComponentModel;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
namespace DevExpress.Web.Mvc {
	using DevExpress.PivotGrid.Printing;
	using DevExpress.Utils.Serializing;
	using DevExpress.Web.Internal;
	using DevExpress.Web.ASPxPivotGrid;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	public interface IViewContext {
		ViewContext ViewContext { get; }
	}
	[ToolboxItem(false)]
	public class MVCxPivotGrid: ASPxPivotGrid, IViewContext {
		bool isLoading = true;
		ViewContext viewContext;
		public MVCxPivotGrid()
			: this(null) {
		}
		protected internal MVCxPivotGrid(ViewContext viewContext)
			: base() {
			this.isLoading = false;
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
		}
		public new MVCxWebPrintAppearance StylesPrint { get { return (MVCxWebPrintAppearance)base.StylesPrint; } }
		protected override WebPrintAppearance CreatePrintStyles() {
			return new MVCxWebPrintAppearance();
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public override bool IsLoading() { 
			return isLoading; 
		}
		public override bool Initialized {
			get { return true; }
		}
		public override bool IsCallback {
			get { return !string.IsNullOrEmpty(MvcUtils.CallbackName) && MvcUtils.CallbackName == ID; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIds.LayoutIdColumns), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new MVCxPivotGridFieldCollection Fields { get { return (MVCxPivotGridFieldCollection)base.Fields; } }
		protected internal MVCxPivotPopupFilterControl FilterControl {
			get { return PrefilterPopup != null ? ((MVCxPivotWebFilterControlPopup)PrefilterPopup).FilterControl : null; }
		}
		protected internal string PivotCustomizationExtensionName { get; set; }
		protected internal new int InitialPageSize { get { return base.InitialPageSize; } set { base.InitialPageSize = value; } }
		protected internal PivotChartDataSource ChartDataSource { get { return ChartDataSourceView.ChartDataSource; } }
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(CallbackRouteValues != null)
				stb.Append(localVarName + ".callbackUrl=\"" + Utils.GetUrl(CallbackRouteValues) + "\";\n");
			if (CustomActionRouteValues != null)
				stb.Append(localVarName + ".customActionUrl=\"" + Utils.GetUrl(CustomActionRouteValues) + "\";\n");
			if(!string.IsNullOrEmpty(PivotCustomizationExtensionName))
				stb.Append(localVarName + ".pivotCustomizationExtensionName=\"" + PivotCustomizationExtensionName + "\";\n");
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientPivotGrid";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxPivotGrid), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxPivotGrid), Utils.PivotGridScriptResourceName);
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected internal void ResetRequireDataUpdate() {
			RequireDataUpdate = false;
		}
		protected override PivotGridHtmlCustomizationFieldsPopup CreateCustomizationFieldsPopup() {
			return new MVCxPivotGridHtmlCustomizationFieldsPopup(this);
		}
		protected override Web.ASPxPivotGrid.Internal.PivotWebFilterControlPopup CreatePrefilterPopup() {
			return new MVCxPivotWebFilterControlPopup(this);
		}
		protected override PivotGridWebData CreateData() {
			return new PivotGridMvcData(this);
		}
		#region IViewContext Members
		public ViewContext ViewContext { get { return viewContext; } }
		#endregion
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public class PivotGridMvcData: PivotGridWebData {
		public PivotGridMvcData(MVCxPivotGrid pivotGrid)
			: base(pivotGrid) {
		}
		public new MVCxPivotGridFieldCollection Fields { get { return base.Fields as MVCxPivotGridFieldCollection; } }
		protected override XtraPivotGrid.PivotGridFieldCollectionBase CreateFieldCollection() {
			return new MVCxPivotGridFieldCollection(this);
		}
	}
	[ToolboxItem(false)]
	public class MVCxPivotGridHtmlCustomizationFieldsPopup: PivotGridHtmlCustomizationFieldsPopup {
		public MVCxPivotGridHtmlCustomizationFieldsPopup(ASPxPivotGrid.ASPxPivotGrid pivotGrid)
			: base(pivotGrid) {
		}
		protected override PivotGridHtmlCustomizationFields CreateCustomizationFields() {
			return new MVCxPivotGridHtmlCustomizationFields(PivotGrid);
		}
	}
	[ToolboxItem(false)]
	public class MVCxPivotGridHtmlCustomizationFields: PivotGridHtmlCustomizationFields {
		public MVCxPivotGridHtmlCustomizationFields(ISupportsFieldsCustomization control)
			: base(control) {
		}
		protected override void CreateExcel2007HeaderRunTime() {
			base.CreateExcel2007HeaderRunTime();
			if(PivotGrid.IsCallback && TreeView != null)
				((IPostBackDataHandler)TreeView).LoadPostData("", Request.Params);
		}
	}
}

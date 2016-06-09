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
using System.Text;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Data;
	using DevExpress.Web.Mvc.Internal;
	[ToolboxItem(false)]
	public class MVCxGridLookup: ASPxGridLookup {
		ViewContext viewContext;
		ModelMetadata metadata;
		public MVCxGridLookup()
			: this(null) {
		}
		protected internal MVCxGridLookup(ViewContext viewContext)
			: this(viewContext, null) {
		}
		protected internal MVCxGridLookup(ViewContext viewContext, ModelMetadata metadata)
			: base() {
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext;
			this.metadata = metadata;
		}
		[Browsable(false)]
		public new MVCxGridLookupWrapper GridView { get { return (MVCxGridLookupWrapper)base.GridView; } }
		public new GridLookupProperties Properties { get { return base.Properties; } }
		public bool ShowModelErrors { get; set; }
		protected ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(MVCxGridLookup), Utils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxGridLookup), Utils.GridLookupScriptResourceName);
		}
		protected internal new void PrepareGridView() {
			base.PrepareGridView();
		}
		protected internal new void ForceCreateControlHierarchy() {
			base.ForceCreateControlHierarchy();
		}
		protected internal new void SetGridViewSelectionMode(GridLookupSelectionMode selectionMode) {
			base.SetGridViewSelectionMode(selectionMode);
		}
		protected override ASPxGridView CreateGridView() {
			return new MVCxGridLookupWrapper(Properties, new GridLookupViewModel(this));
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientGridLookup";
		}
		protected override bool IsCustomValidationEnabled {
			get { return ShowModelErrors || base.IsCustomValidationEnabled; }
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new MVCxGridLookupControl(this, Metadata);
		}
		protected override EditPropertiesBase CreateProperties() {
			return new MVCxGridLookupProperties(this);
		}
	}
	public class MVCxGridLookupControl : GridLookupControl {
		public MVCxGridLookupControl(MVCxGridLookup edit, ModelMetadata metadata)
			: base(edit) {
			Metadata = metadata;
		}
		protected ModelMetadata Metadata { get; set; }
		protected override void PrepareInputControl(InputControl input) {
			base.PrepareInputControl(input);
			ExtensionsHelper.SetUnobtrusiveValidationAttributes(input, Edit.ID, Metadata);
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	using DevExpress.Web.Data;
	public class MVCxGridLookupWrapper: MVCxGridView, IGridViewOwnedLookup {
		GridLookupProperties glpProperties;
		GridLookupViewModel customOperationViewModel;
		public MVCxGridLookupWrapper(GridLookupProperties properties, GridLookupViewModel customOperationViewModel)
			: base() {
			this.glpProperties = properties;
			this.customOperationViewModel = customOperationViewModel;
			ClientIDHelper.EnableClientIDGeneration(this);
		}
		protected internal override GridViewModel CustomOperationViewModel { get { return customOperationViewModel; } }
		protected override bool AllowFireFocusedOrSelectedRowChangedOnClient { get { return false; } }
		protected override void CreateControlHierarchy() {
			this.CheckUsingEndlessPaging();
			base.CreateControlHierarchy();
		}
		protected override void BeforeRender() {
			((IGridViewOwnedLookup)this).SaveSelectionState();
			base.BeforeRender();
		}
		protected override GridCallbackArgumentsReader GetCreateCallbackArgumentReader(string eventArgument) {
			var callbackArgumentReader = base.GetCreateCallbackArgumentReader(eventArgument);
			this.SaveCallbackCommand(callbackArgumentReader.CallbackArguments);
			return callbackArgumentReader;
		}
		protected override void InitializeClientObjectScript(StringBuilder stb, string localVarName, string clientID) {
			base.InitializeClientObjectScript(stb, localVarName, clientID);
			this.InitializeAdditionalClientObjectScript(stb, localVarName, clientID);
		}
		protected override void EnsurePreRender() {
			this.SettingsBehavior.AllowSelectByRowClick = !SettingsBehavior.AllowSelectSingleRowOnly; 
			base.EnsurePreRender();
		}
		protected override object GetCallbackResult() {
			((IGridViewOwnedLookup)this).SaveSelectionState();
			return base.GetCallbackResult();
		}
		protected override WebDataProxy CreateDataProxy() {
			return new MVCxGridLookupWrapperDataProxy(this, this, this);
		}
		protected override WebDataSelection CreateSelection(WebDataProxy proxy) {
			var dataProxy = proxy as MVCxGridLookupWrapperDataProxy;
			if(dataProxy == null)
				return base.CreateSelection(proxy);
			return new GridLookupWrapperSelection(dataProxy);
		}
		#region IGridViewOwnedLookup Members
		FilterExpressionCreatorBase IGridViewOwnedLookup.FilterExpressionCreator { get; set; }
		GridLookupProperties IGridViewOwnedLookup.GLPProperties { get { return glpProperties; } }
		string IGridViewOwnedLookup.CallbackCommand { get; set; }
		string[] IGridViewOwnedLookup.CallbackCommandArgs { get; set; }
		string IGridViewOwnedLookup.SavedSelectionState { get; set; }
		#endregion
	}
	public class MVCxSelectionStrategyOwner: SelectionStrategyOwner {
		public MVCxSelectionStrategyOwner(GridLookupProperties glpProperties)
			: base(glpProperties) {
		}
		public override bool HasDataSource { get { return base.HasDataSource || GridViewWrapper.EnableCustomOperations; } }
		protected new MVCxGridLookupWrapper GridViewWrapper { get { return (MVCxGridLookupWrapper)base.GridViewWrapper; } }
	}
}

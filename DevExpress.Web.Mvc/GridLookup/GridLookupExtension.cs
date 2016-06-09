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

using DevExpress.Data.Linq;
using DevExpress.Web.Mvc.Internal;
using System;
using System.Collections;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	public class GridLookupExtension : EditorExtension {
		public GridLookupExtension(GridLookupSettings settings)
			: base(settings) {
		}
		public GridLookupExtension(GridLookupSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public GridLookupExtension(GridLookupSettings settings, ViewContext viewContext, ModelMetadata metadata)
			: base(settings, viewContext, metadata) {
		}
		public static T[] GetSelectedValues<T>(string name) {
			return EditorValueProvider.GetValue<T[]>(name);
		}
		GridViewOwnedLookupExtension gridViewExtension;
		protected GridViewOwnedLookupExtension GridViewExtension {
			get {
				if(gridViewExtension == null)
					gridViewExtension = new GridViewOwnedLookupExtension(Control.GridView, Settings.GridViewSettings, ViewContext);
				return gridViewExtension;
			}
		}
		protected internal new MVCxGridLookup Control {
			get { return (MVCxGridLookup)base.Control; }
		}
		protected internal new GridLookupSettings Settings {
			get { return (GridLookupSettings)base.Settings; }
		}
		protected override EditPropertiesBase Properties {
			get { return Control.Properties; }
		}
		protected internal override bool IsCallback() {
			return Control.GridView.IsCallback;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.ForceCreateControlHierarchy();
			GridViewExtension.PrepareControl();
			PrepareValidationForEditor(Settings.ShowModelErrors);
		}
		protected override void AssignInitialProperties() {
			GridViewExtension.ApplyInitialProperties();
			Control.PrepareGridView();
			base.AssignInitialProperties();
			Control.ShowModelErrors = Settings.ShowModelErrors;
			Control.AnimationType = Settings.AnimationType;
			Control.DataBinding += Settings.DataBinding;
			Control.DataBound += Settings.DataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			GridViewExtension.ApplyRenderProperties();
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			GridViewExtension.LoadPostData();
		}
		protected override void ProcessCallback(string callbackArgument) {
			if(Control.GridView.IsCallback) {
				GridViewExtension.ProcessCallback(callbackArgument);
				return;
			}
			base.ProcessCallback(callbackArgument);
		}
		protected override void RenderCallbackResult() {
			if(Control.GridView.IsCallback) {
				GridViewExtension.RenderCallbackResult();
				return;
			}
			base.RenderCallbackResult();
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxGridLookup(ViewContext, Metadata);
		}
		public GridLookupExtension SetEditErrorText(string message) {
			GridViewExtension.SetEditErrorText(message);
			return this;
		}
		public GridLookupExtension BindList(object dataObject) {
			GridViewExtension.Bind(dataObject);
			return this;
		}
		public GridLookupExtension BindToXML(string fileName) {
			GridViewExtension.BindToXML(fileName, string.Empty, string.Empty);
			return this;
		}
		public GridLookupExtension BindToXML(string fileName, string xPath) {
			GridViewExtension.BindToXML(fileName, xPath, string.Empty);
			return this;
		}
		public GridLookupExtension BindToXML(string fileName, string xPath, string transformFileName) {
			GridViewExtension.BindToXML(fileName, xPath, transformFileName);
			return this;
		}
		public GridLookupExtension BindToLINQ(Type contextType, string tableName) {
			GridViewExtension.BindToLINQ(contextType.FullName, tableName);
			return this;
		}
		public GridLookupExtension BindToLINQ(string contextTypeName, string tableName) {
			GridViewExtension.BindToLINQ(contextTypeName, tableName, null);
			return this;
		}
		public GridLookupExtension BindToLINQ(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			GridViewExtension.BindToLINQ(contextType.FullName, tableName, selectingMethod);
			return this;
		}
		public GridLookupExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			GridViewExtension.BindToLINQ(contextTypeName, tableName, selectingMethod, null);
			return this;
		}
		public GridLookupExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			GridViewExtension.BindToLINQ(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public GridLookupExtension BindToEF(Type contextType, string tableName) {
			GridViewExtension.BindToEF(contextType.FullName, tableName);
			return this;
		}
		public GridLookupExtension BindToEF(string contextTypeName, string tableName) {
			GridViewExtension.BindToEF(contextTypeName, tableName, null);
			return this;
		}
		public GridLookupExtension BindToEF(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			GridViewExtension.BindToEF(contextType.FullName, tableName, selectingMethod);
			return this;
		}
		public GridLookupExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			GridViewExtension.BindToEF(contextTypeName, tableName, selectingMethod, null);
			return this;
		}
		public GridLookupExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			GridViewExtension.BindToEF(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public GridLookupExtension BindToCustomData(GridLookupViewModel viewModel) {
			GridViewExtension.BindToCustomData(viewModel);
			Control.IncrementalFilteringMode = viewModel.IncrementalFilteringMode;
			if(!string.IsNullOrEmpty(viewModel.TextFormatString))
				Control.TextFormatString = viewModel.TextFormatString;
			return this;
		}
		public static GridLookupViewModel GetViewModel(string name) {
			return GridLookupViewModel.Load(name + GridLookupViewModel.GridPostfix);
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public class GridViewOwnedLookupExtension: GridViewExtension {
		public GridViewOwnedLookupExtension(MVCxGridView grid, GridViewSettings gridSettings, ViewContext viewContext)
			: base(gridSettings, viewContext) {
			Control = grid;
		}
		protected internal new MVCxGridView Control {
			get { return base.Control; }
			set { base.Control = value; } 
		}
		protected internal void ApplyInitialProperties() {
			AssignInitialProperties();
		}
		protected internal void ApplyRenderProperties() {
			AssignRenderProperties();
		}
		protected override void AssignInitialProperties() {
			if(Control == null)
				return;
			base.AssignInitialProperties();
		}
		protected internal new void RenderCallbackResult() {
			base.RenderCallbackResult();
		}
		protected internal new void ProcessCallback(string callbackArgument) {
			base.ProcessCallback(callbackArgument);
		}
		protected override void ValidateSettings() { }
		protected override ASPxWebControl CreateControl() {
			return null;
		}
	}
}

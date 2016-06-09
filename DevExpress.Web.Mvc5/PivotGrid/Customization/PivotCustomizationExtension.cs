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
using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.Mvc.UI;
	public class PivotCustomizationExtension: ExtensionBase {
		PivotGridSettings pivotGridSettings;
		public PivotCustomizationExtension(PivotGridSettings pivotGridSettings)
			: base(pivotGridSettings) {
		}
		public PivotCustomizationExtension(PivotGridSettings pivotGridSettings, ViewContext viewContext)
			: base(pivotGridSettings, viewContext) {
		}
		protected internal PivotCustomizationExtension(PivotGridSettings pivotGridSettings, Action<ExtensionBase> onBeforeCreateControl)
			: base(pivotGridSettings, null, null, onBeforeCreateControl) {
		}
		protected override void ApplySettings(SettingsBase pivotGridSettings) {
			this.pivotGridSettings = (PivotGridSettings)pivotGridSettings;
			base.Settings = this.pivotGridSettings.PivotCustomizationExtensionSettings;
		}
		protected internal new MVCxPivotCustomizationControl Control {
			get { return (MVCxPivotCustomizationControl)base.Control; }
		}
		MVCxPivotGrid pivotGrid;
		protected internal MVCxPivotGrid PivotGrid {
			get {
				if (Settings != null && pivotGrid == null)
					pivotGrid = PivotGridExtension.CreatePivotGridControl(PivotGridSettings);
				return pivotGrid;
			}
			set { pivotGrid = value; }
		}
		protected internal new PivotCustomizationExtensionSettings Settings { 
			get { return (PivotCustomizationExtensionSettings)base.Settings; } 
		}
		protected internal PivotGridSettings PivotGridSettings { get { return pivotGridSettings; } }
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.Layout = Settings.Layout;
			Control.AllowedLayouts = Settings.AllowedLayouts;
			Control.DeferredUpdates = Settings.DeferredUpdates;
			Control.AllowSort = Settings.AllowSort;
			Control.AllowFilter = Settings.AllowFilter;
			Control.Visible = Settings.Visible;
			Control.ASPxPivotGridID = PivotGridSettings.Name;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.RegisterInPivotGrid();
			PivotGrid.EnsureRefreshData();
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			Control.CustomizationFields.ReadPostData();
		}
		public PivotCustomizationExtension Bind(object dataObject) {
			BindToDataSource(dataObject);
			return this;
		}
		public PivotCustomizationExtension BindToOLAP(string connectionString) {
			PivotGrid.OLAPConnectionString = connectionString;
			return this;
		}
		protected internal override bool IsBindInternalRequired() {
			return false;
		}
		protected override void BindToDataSource(object dataSource) {
			PivotGrid.DataSource = dataSource;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxPivotCustomizationControl(PivotGrid);
		}
		protected internal override void DisposeControl() {
			PivotGrid.Dispose();
			base.DisposeControl();
		}
	}
}

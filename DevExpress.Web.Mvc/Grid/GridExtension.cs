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

using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public abstract class GridExtensionBase : ExtensionBase {
		public GridExtensionBase(GridSettingsBase settings)
			: base(settings) {
		}
		public GridExtensionBase(GridSettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxGridBase Control {
			get { return (ASPxGridBase)base.Control; }
			protected set { base.Control = value; }
		}
		protected internal new GridSettingsBase Settings {
			get { return (GridSettingsBase)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnableCallbackCompression = Settings.EnableCallbackCompression;
			Control.EnablePagingCallbackAnimation = Settings.EnablePagingCallbackAnimation;
			Control.EnablePagingGestures = Settings.EnablePagingGestures;
			Control.ClientVisible = Settings.ClientVisible;
			Control.KeyFieldName = Settings.KeyFieldName;
			Control.ImagesFilterControl.Assign(Settings.ImagesFilterControl);
			Control.StylesFilterControl.Assign(Settings.StylesFilterControl);
			Control.RightToLeft = Settings.RightToLeft;
			Control.ClientLayout += Settings.ClientLayout;
			Control.DataBound += Settings.DataBound;
			Control.DataBinding += Settings.DataBinding;
			Control.FilterControlOperationVisibility += Settings.FilterControlOperationVisibility;
			Control.FilterControlParseValue += Settings.FilterControlParseValue;
			Control.FilterControlCustomValueDisplayText += Settings.FilterControlCustomValueDisplayText;
			Control.FilterControlCriteriaValueEditorInitialize += Settings.FilterControlCriteriaValueEditorInitialize;
			Control.FilterControlCriteriaValueEditorCreate += Settings.FilterControlCriteriaValueEditorCreate;
			Control.FilterControlColumnsCreated += Settings.FilterControlColumnsCreated;
		}
		protected internal override bool IsCallback() {
			return base.IsCallback() || IsFilterControlCallback();
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			Control.LayoutChanged();
		}
		protected internal override ICallbackEventHandler CallbackEventHandler {
			get { return IsExistFilterControl ? FilterControl : base.CallbackEventHandler; }
		}
		protected bool IsExistFilterControl {
			get { return IsFilterControlCallback() && FilterControl != null && !FilterControl.IsApplyCalled; }
		}
		protected abstract MVCxPopupFilterControl FilterControl { get; }
	}
}

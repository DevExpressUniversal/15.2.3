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
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using DevExpress.Utils;
	using DevExpress.Web;
	public abstract class GridSettingsBase : SettingsBase {
		public GridSettingsBase() {
			EnableCallbackCompression = true;
			EnablePagingGestures = AutoBoolean.Auto;
			ImagesFilterControl = new FilterControlImages(null);
			StylesFilterControl = new FilterControlStyles(null);
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public object CustomDataActionRouteValues { get; set; }
		public bool ClientVisible { get { return ClientVisibleInternal; } set { ClientVisibleInternal = value; } }
		public new AppearanceStyle ControlStyle { get { return (AppearanceStyle)base.ControlStyle; } }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnableCallbackCompression { get; set; }
		public bool EnablePagingCallbackAnimation { get; set; }
		public AutoBoolean EnablePagingGestures { get; set; }
		[Browsable(false)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		public string KeyFieldName { get; set; }
		public DefaultBoolean RightToLeft { get { return RightToLeftInternal; } set { RightToLeftInternal = value; } }
		public FilterControlImages ImagesFilterControl { get; private set; }
		public FilterControlStyles StylesFilterControl { get; private set; }
		public EventHandler BeforeGetCallbackResult { get { return BeforeGetCallbackResultInternal; } set { BeforeGetCallbackResultInternal = value; } }
		public ASPxClientLayoutHandler ClientLayout { get { return ClientLayoutInternal; } set { ClientLayoutInternal = value; } }
		public EventHandler DataBinding { get; set; }
		public EventHandler DataBound { get; set; }
		public FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility { get; set; }
		public FilterControlParseValueEventHandler FilterControlParseValue { get; set; }
		public FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText { get; set; }
		public FilterControlCriteriaValueEditorInitializeEventHandler FilterControlCriteriaValueEditorInitialize { get; set; }
		public FilterControlCriteriaValueEditorCreateEventHandler FilterControlCriteriaValueEditorCreate { get; set; }
		public FilterControlColumnsCreatedEventHandler FilterControlColumnsCreated { get; set; }
		protected override AppearanceStyleBase CreateControlStyle() {
			return new AppearanceStyle();
		}
	}
}

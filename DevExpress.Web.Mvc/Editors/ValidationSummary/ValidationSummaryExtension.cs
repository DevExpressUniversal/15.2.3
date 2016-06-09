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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	public class ValidationSummaryExtension : ExtensionBase {
		public ValidationSummaryExtension(ValidationSummarySettings settings)
			: base(settings) {
		}
		public ValidationSummaryExtension(ValidationSummarySettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxValidationSummary Control {
			get { return (MVCxValidationSummary)base.Control; }
		}
		protected internal new ValidationSummarySettings Settings {
			get { return (ValidationSummarySettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.HeaderText = Settings.HeaderText;
			Control.ValidationGroup = Settings.ValidationGroup;
			Control.ShowErrorsInEditors = Settings.ShowErrorsInEditorsInternal;
			Control.ShowErrorAsLink = Settings.ShowErrorAsLink;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.LinkStyle.CopyFrom(Settings.LinkStyle);
			Control.RenderMode = Settings.RenderMode;
			Control.RightToLeft = Settings.RightToLeft;
		}
		protected internal override void PrepareControlProperties() {
			base.PrepareControlProperties();
			if (IsSimpleIDsRenderModeSupported() && string.IsNullOrEmpty(Control.ID))
				Control.ID = "dxValidationSummary";
		}
		protected override bool IsSimpleIDsRenderModeSupported() {
			return Control.ClientSideEvents.IsEmpty() && string.IsNullOrEmpty(Control.ClientInstanceName);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.LoadErrors();
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxValidationSummary(ViewContext);
		}
	}
}

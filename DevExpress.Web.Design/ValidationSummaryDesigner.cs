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
using DevExpress.Web.Internal;
using DevExpress.Web.Design;
using System.ComponentModel.Design;
using System;
using DevExpress.Utils.About;
namespace DevExpress.Web.Design {
	public class ASPxValidationSummaryDesigner : ASPxWebControlDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			EnsureReferences("DevExpress.Web" + AssemblyInfo.VSuffix);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ValidationSummaryDesignerActionList(this);
		}
		public override void ShowAbout() {
			ValidationSummaryAboutDialogHelper.ShowAbout(Component.Site);
		}
	}
	public class ValidationSummaryAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxValidationSummary), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (ShouldShowTrialAbout(typeof(ASPxValidationSummary)))
				ShowAbout(provider);
		}
	}
	public class ValidationSummaryDesignerActionList : ASPxWebControlDesignerActionList {
		public ValidationSummaryDesignerActionList(ASPxValidationSummaryDesigner designer)
			: base(designer) {
		}
		protected ASPxValidationSummary ValidationSummary {
			get { return (ASPxValidationSummary)Designer.Component; }
		}
		public ValidationSummaryRenderMode RenderMode {
			get { return ValidationSummary.RenderMode; }
			set {
				ValidationSummary.RenderMode = value;
				ValidationSummary.PropertyChanged("RenderMode");
			}
		}
		public bool ShowErrorsInEditors {
			get { return ValidationSummary.ShowErrorsInEditors; }
			set {
				ValidationSummary.ShowErrorsInEditors = value;
				ValidationSummary.PropertyChanged("ShowErrorsInEditors");
			}
		}
		public string ValidationGroup {
			get { return ValidationSummary.ValidationGroup; }
			set {
				ValidationSummary.ValidationGroup = value;
				ValidationSummary.PropertyChanged("ValidationGroup");
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("ValidationGroup", "Validation Group"));
			collection.Add(new DesignerActionPropertyItem("RenderMode", "Render Mode"));
			collection.Add(new DesignerActionPropertyItem("ShowErrorsInEditors", "Show Errors in Editors"));
			return collection;
		}
	}
}

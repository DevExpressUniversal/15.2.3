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
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxHeadlineDesigner : ASPxWebControlDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		protected override string GetBaseProperty() {
			return "ContentText";
		}
		public override bool HasClientSideEvents() {
			return false;
		}
		public override bool HasCommonDesigner() {
			return false;
		}
	}
	public class HeadlineDataBindingHandler: DataBindingHandler {
		public HeadlineDataBindingHandler()
			: base() {
		}
		public override void DataBindControl(IDesignerHost designerHost, Control control) {
			ASPxHeadline headline = control as ASPxHeadline;
			if (headline == null)
				return;
			DataBinding bindingText = ((IDataBindingsAccessor)control).DataBindings["ContentText"];
			if (bindingText != null)
				headline.ContentText = StringResources.SampleDataBoundText;
			bindingText = ((IDataBindingsAccessor)control).DataBindings["HeaderText"];
			if (bindingText != null)
				headline.HeaderText = StringResources.SampleDataBoundText;
			bindingText = ((IDataBindingsAccessor)control).DataBindings["TailText"];
			if (bindingText != null)
				headline.TailText = StringResources.SampleDataBoundText;
		}
	}
}

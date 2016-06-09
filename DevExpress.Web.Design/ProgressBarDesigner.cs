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
using System.Collections.Generic;
using System.Text;
using DevExpress.Web.Design;
using System.ComponentModel;
using DevExpress.Web.Internal;
using System.Web.UI.Design;
using System.Web.UI;
using System.ComponentModel.Design;
using DevExpress.Web;
namespace DevExpress.Web.Design {
	public class ASPxProgressBarDesigner : ASPxEditDesignerBase {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ProgressBarDesignerActionList(this);
		}
	}
	public class ProgressBarDesignerActionList : ASPxWebControlDesignerActionList {
		public ProgressBarDesignerActionList(ASPxProgressBarDesigner designer)
			: base(designer) { }
		protected ASPxProgressBar ProgressBar {
			get { return (ASPxProgressBar)Designer.Component; }
		}
		public ProgressBarDisplayMode DisplayMode {
			get { return ProgressBar.DisplayMode; }
			set {
				ProgressBar.DisplayMode = value;
				ProgressBar.PropertyChanged("DisplayMode");
			}
		}
		public Decimal Minimum {
			get { return ProgressBar.Minimum; }
			set {
				ProgressBar.Minimum = value;
				ProgressBar.PropertyChanged("Minimum");
			}
		}
		public Decimal Maximum {
			get { return ProgressBar.Maximum; }
			set {
				ProgressBar.Maximum = value;
				ProgressBar.PropertyChanged("Maximum");
			}
		}
		public Decimal Position {
			get { return ProgressBar.Position; }
			set {
				ProgressBar.Position = value;
				ProgressBar.PropertyChanged("Position");
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("DisplayMode", "DisplayMode"));
			collection.Add(new DesignerActionPropertyItem("Minimum", "Minimum"));
			collection.Add(new DesignerActionPropertyItem("Maximum", "Maximum"));
			collection.Add(new DesignerActionPropertyItem("Position", "Position"));
			return collection;
		}
	}
	public class ProgressBarDataBindingHandler : DataBindingHandler {
		public ProgressBarDataBindingHandler()
			: base() {
		}
		public override void DataBindControl(IDesignerHost designerHost, Control control) {
			ASPxProgressBar progressBar = control as ASPxProgressBar;
			if (progressBar == null)
				return;
			DataBinding bindingText = ((IDataBindingsAccessor)control).DataBindings["Position"];
			if (bindingText != null)
				progressBar.Position = 50;
		}
	}
}

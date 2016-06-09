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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraRichEdit.UI;
namespace DevExpress.XtraRichEdit.Design {
	#region RichEditRibbonCommandBarComponentDesigner
	public class RichEditRibbonCommandBarComponentDesigner : RibbonCommandBarComponentBaseDesigner {
		public RichEditRibbonCommandBarComponentDesigner() {
		}
		public new RichEditRibbonCommandBarComponent CommandBar { get { return base.CommandBar as RichEditRibbonCommandBarComponent; } }
		protected override void InitializeNewCommandBar() {
			base.InitializeNewCommandBar();
			CommandBar.RichEditControl = GetRichEdit();
		}
		protected internal RichEditControl GetRichEdit() {
			return ComponentFinder.FindComponentOfType<RichEditControl>(Component.Site);
		}
		protected override void OnComponentAdded(object sender, ComponentEventArgs e) {
			base.OnComponentAdded(sender, e);
			if (CommandBar.RichEditControl == null) {
				RichEditControl control = e.Component as RichEditControl;
				if (control != null)
					CommandBar.RichEditControl = control;
			}
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(RichEditControl.About));
		}
	}
	#endregion
}

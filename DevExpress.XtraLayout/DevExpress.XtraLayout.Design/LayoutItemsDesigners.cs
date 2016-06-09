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
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.HitInfo;
#if DXWhidbey
using System.Windows.Forms.Design.Behavior;
#endif
namespace DevExpress.XtraLayout.DesignTime {
	public class BaseLayoutComponentDesigner : ComponentDesigner {
		protected ArrayList components = new ArrayList();
		public BaseLayoutComponentDesigner() {
		}
	}
	public class LayoutControlItemDesigner : BaseLayoutComponentDesigner {
		public LayoutControlItemDesigner() {
		}
		public new LayoutControlItem Component {
			get {
				return (LayoutControlItem)base.Component;
			}
		}
		public override ICollection AssociatedComponents {
			get {
				components.Clear();
				if(Component.Control != null) components.Add(Component.Control);
				return components;
			}
		}
	}
	public class LayouytControlGroupDesigner : BaseLayoutComponentDesigner {
		public LayouytControlGroupDesigner() {
		}
		public new LayoutControlGroup Component {
			get {
				return (LayoutControlGroup)base.Component;
			}
		}
		public override ICollection AssociatedComponents {
			get {
				return Component.Items;
			}
		}
	}
	public class TabbedGroupDesigner : ComponentDesigner {
		public TabbedGroupDesigner() {
		}
		public new TabbedControlGroup Component {
			get {
				return (TabbedControlGroup)base.Component;
			}
		}
		public override ICollection AssociatedComponents {
			get {
				return Component.TabPages;
			}
		}
	}
}

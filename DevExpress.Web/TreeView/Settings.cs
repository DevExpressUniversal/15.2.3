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
using DevExpress.Web;
using System.ComponentModel;
namespace DevExpress.Web {
	public enum TreeViewNodeImagePosition { Left, Right }
	public enum TreeViewLoadingPanelMode { ShowAsPopup, ShowNearNode, Disabled }
	public class TreeViewSettingsLoadingPanel : SettingsLoadingPanel {
		public TreeViewSettingsLoadingPanel(ASPxTreeView treeView)
			: base(treeView) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewSettingsLoadingPanelMode"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(TreeViewLoadingPanelMode.ShowNearNode)]
		public TreeViewLoadingPanelMode Mode {
			get { return (TreeViewLoadingPanelMode)GetEnumProperty("Mode", TreeViewLoadingPanelMode.ShowNearNode); }
			set {
				SetEnumProperty("Mode", TreeViewLoadingPanelMode.ShowNearNode, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TreeViewSettingsLoadingPanel src = source as TreeViewSettingsLoadingPanel;
			if(src != null) 
				Mode = src.Mode;
		}
	}
}

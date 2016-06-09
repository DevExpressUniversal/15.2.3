#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Native;
using System.Collections.Generic;
namespace DevExpress.DashboardCommon.Layout {
	public enum DashboardPaneType {
		Group,
		Item
	}
	public class DashboardPane : IDashboardLayoutNode {
		public static DashboardPane DefaultRootPane {
			get {
				DashboardPane pane = new DashboardPane();
				pane.Panes = new List<DashboardPane>();
				return pane;
			}
		}
		public string Name { get; set; }
		public double Size { get; set; }
		public DashboardLayoutGroupOrientation? Orientation { get; set; }
		public DashboardPaneType Type { get; set; }
		public IList<DashboardPane> Panes { get; set; }
		bool IDashboardLayoutNode.IsGroup { get { return Type == DashboardPaneType.Group; } }
		double IDashboardLayoutNode.Weight { get { return Size; } set { Size = value; } }
		string IDashboardLayoutNode.DashboardItemName { get { return Name; } set { Name = value; } }		
		DashboardLayoutGroupOrientation? IDashboardLayoutNode.Orientation {get { return Orientation; } set { Orientation = value; } }
		IEnumerable<IDashboardLayoutNode> IDashboardLayoutNode.ChildNodes { get { return Panes; }		}
		void IDashboardLayoutNode.AddChildNode(IDashboardLayoutNode node) {
			Panes.Add((DashboardPane)node);
		}
	}
}

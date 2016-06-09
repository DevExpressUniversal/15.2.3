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

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.Data;
using System.Windows.Automation;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.GridData;
namespace DevExpress.Xpf.Grid.Automation {
	public class ColumnHeaderAutomationPeerBase : GridControlVirtualElementAutomationPeerBase {
		ColumnBase column;
		GridColumnHeader header;
		public ColumnHeaderAutomationPeerBase(DataControlBase dataControl, GridColumnHeader header) : base(dataControl) {
			this.header = header;
			this.column = header.Column;
		}
		public ColumnHeaderAutomationPeerBase(DataControlBase dataControl, ColumnBase column)
			: base(dataControl) {
			this.column = column;
		}
		public GridColumnHeader ColumnHeader { get { return header; } }
		public ColumnBase Column { get { return column; } }
		public IColumnInfo ColumnInfo { get { return Column; } }
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.HeaderItem;
		}
		protected override string GetNameCore() {
			return ColumnInfo == null ? "" : ColumnInfo.FieldName;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			FrameworkElement header = ColumnHeader ?? GetFrameworkElement();
			return header == null ? null : DataControlAutomationPeerBase.GetUIChildrenCore(header, DataControl.AutomationPeer);
		}
		protected override FrameworkElement GetFrameworkElement() {
			FrameworkElement element = LayoutHelper.FindElement(DataControl, delegate(FrameworkElement current) {
				if(current is BaseGridColumnHeader)
					return ((BaseGridColumnHeader)current).Column == Column;
				return false;
			});
			return element;
		}
		public override object GetPattern(PatternInterface patternInterface) { return null; }
#if SL
		protected override string GetLocalizedControlTypeCore() {
			return string.Empty;
		}
#endif
	}
	public class ColumnHeaderAutomationPeer : ColumnHeaderAutomationPeerBase, IDockProvider {
		public ColumnHeaderAutomationPeer(GridControl gridControl, GridColumnHeader header)
			: base(gridControl, header) {
		}
		public ColumnHeaderAutomationPeer(GridControl gridControl, GridColumn column)
			: base(gridControl, column) {
		}
		public new GridControl DataControl { get { return base.DataControl as GridControl; } }
		public new GridColumn Column { get { return base.Column as GridColumn; } }
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Dock)
				return this;
			return null;
		}
		#region IDockProvider Members
		public DockPosition DockPosition {
			get {
				return Column != null && Column.IsGrouped ? DockPosition.Top : DockPosition.None;
			}
		}
		protected bool ShouldDockItem(DockPosition dockPosition) {
			if(dockPosition != DockPosition.Top && dockPosition != DockPosition.None) return false;
			return dockPosition != DockPosition;
		}
		public void SetDockPosition(DockPosition dockPosition) {
			if(!ShouldDockItem(dockPosition)) return;
			if(dockPosition == DockPosition.Top)
				DataControl.GroupBy(Column);
			else
				DataControl.UngroupBy(Column);
		}
		#endregion
	}
}

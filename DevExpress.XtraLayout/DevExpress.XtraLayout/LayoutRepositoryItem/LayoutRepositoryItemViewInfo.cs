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
using System.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraLayout.ViewInfo;
namespace DevExpress.XtraLayout {
	public class LayoutRepositoryItemViewInfo : LayoutControlItemViewInfo {
		RepositoryItem repositoryItemCore;
		BaseEditViewInfo repItemInfoCore;
		public virtual RepositoryItem RepositoryItem {
			get { return repositoryItemCore; }
			set { repositoryItemCore = value; }
		}
		public BaseEditViewInfo RepositoryItemViewInfo {
			get { return repItemInfoCore ?? (repItemInfoCore = repositoryItemCore.CreateViewInfo()); }
			set { repItemInfoCore = value; }
		}
		public new LayoutRepositoryItem Owner {
			get { return (LayoutRepositoryItem)base.Owner; }
		}
		public LayoutRepositoryItemViewInfo(LayoutRepositoryItem owner)
			: base(owner) {
			this.repositoryItemCore = Owner.RepositoryItem;
		}
		public override Rectangle ClientArea {
			get { return base.ClientArea; }
			set { base.ClientArea = value; }
		}
		protected bool IsRightToLeft {
			get { return Owner.IsRTL; }
		}
		protected override void CalcViewInfoCore() {
			if(repositoryItemCore == null) return;
			base.CalcViewInfoCore();
			UpdateRepositoryItemViewInfo();
		}
		protected virtual void UpdateRepositoryItemViewInfo() {
			Rectangle clientArea = ClientAreaRelativeToControl;
			bool usePreferredWidth = (RepositoryItemViewInfo is IHeightAdaptable);
			RepositoryItemViewInfo.Bounds = !usePreferredWidth || Owner.Owner is LayoutControl ? clientArea :
				new Rectangle(clientArea.Left, clientArea.Top, Math.Min(Owner.EditorPreferredWidth, clientArea.Width), clientArea.Height);
			RepositoryItemViewInfo.FillBackground = !RepositoryItemViewInfo.Item.AllowFocusedAppearance;
			RepositoryItemViewInfo.InplaceType = InplaceType.Grid;
			RepositoryItemViewInfo.CalcViewInfo(null);
		}
		public override object Clone() {
			LayoutRepositoryItemViewInfo cloneInfo = (LayoutRepositoryItemViewInfo)base.Clone();
			cloneInfo.repItemInfoCore = (repItemInfoCore != null) ? (BaseEditViewInfo)repItemInfoCore.Clone() : null;
			return cloneInfo;
		}
	}
}

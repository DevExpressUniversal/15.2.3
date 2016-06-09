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
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Container;
namespace DevExpress.XtraLayout {
	public class LayoutRepositoryItemPainter : LayoutControlItemPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			LayoutRepositoryItemViewInfo vi = e as LayoutRepositoryItemViewInfo;
			if(vi!=null && !vi.Owner.ActualItemVisibility) return;
			base.DrawObject(e);
			DrawFieldValue(vi);
		}
		protected virtual void DrawFieldValue(LayoutRepositoryItemViewInfo viewInfo) {
			viewInfo.RepositoryItemViewInfo.PaintAppearance.DrawBackground(viewInfo.Cache, viewInfo.ClientAreaRelativeToControl);
			EditorContainer editorContainer = viewInfo.OwnerILayoutControl.GetEditorContainer();
			LayoutControlContainerHelper ch = new LayoutControlContainerHelper(editorContainer);
			viewInfo.RepositoryItemViewInfo.FillBackground = false;
			ch.DrawCellEdit(viewInfo, viewInfo.RepositoryItem, viewInfo.RepositoryItemViewInfo);
		}
	}
	public class LayoutRepositoryItemSkinPainter : LayoutControlItemSkinPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			LayoutRepositoryItemViewInfo vi = e as LayoutRepositoryItemViewInfo;
			if(vi!=null && !vi.Owner.ActualItemVisibility) return;
			base.DrawObject(e);
			DrawFieldValue(vi);
		}
		protected virtual void DrawFieldValue(LayoutRepositoryItemViewInfo viewInfo) {
			viewInfo.RepositoryItemViewInfo.PaintAppearance.DrawBackground(viewInfo.Cache, viewInfo.ClientAreaRelativeToControl);
			EditorContainer editorContainer = viewInfo.OwnerILayoutControl.GetEditorContainer();
			LayoutControlContainerHelper ch = new LayoutControlContainerHelper(editorContainer);
			viewInfo.RepositoryItemViewInfo.FillBackground = false;
			ch.DrawCellEdit(viewInfo, viewInfo.RepositoryItem, viewInfo.RepositoryItemViewInfo);
		}
	}
}

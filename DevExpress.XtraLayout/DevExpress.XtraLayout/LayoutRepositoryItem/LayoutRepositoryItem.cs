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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraLayout {
	public class LayoutRepositoryItem : LayoutControlItem {
		object editValueCore;
		int editorPreferredWidth = 0;
		RepositoryItem repositoryItemCore;
		public LayoutRepositoryItem()
			: this(DefaultRepositoryItem) {
		}
		public LayoutRepositoryItem(RepositoryItem editor) {
			this.repositoryItemCore = editor;
		}
		protected override void Dispose(bool disposing) {
			repositoryItemCore = null;
			base.Dispose(disposing);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RepositoryItem RepositoryItem {
			get { return repositoryItemCore; }
			set { repositoryItemCore = value; }
		}
		[XtraSerializableProperty()]
		public int EditorPreferredWidth {
			get { return CalcScaledWidth(editorPreferredWidth); }
			set { SetEditorPreferredWidthCore(CalcDeScaledWidth(value)); }
		}
		public void ResetEditorPreferredWidth() {
			SetEditorPreferredWidthCore(Math.Min(20, editorPreferredWidth));
		}
		void SetEditorPreferredWidthCore(int value) {
			if(editorPreferredWidth == value) return;
			using(new SafeBaseLayoutItemChanger(this)) {
				editorPreferredWidth = value;
				ShouldArrangeTextSize = true;
				shouldResetBorderInfoCore = true;
				ComplexUpdate(true, true);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object EditValue {
			get { return editValueCore; }
			set {
				editValueCore = value;
				(ViewInfo as LayoutRepositoryItemViewInfo).RepositoryItemViewInfo.EditValue = value;
			}
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "LayoutRepositoryItem"; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ControlName {
			get { return base.ControlName; }
			set { base.ControlName = value; }
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new LayoutRepositoryItemViewInfo(this);
		}
		protected override Size GetContentMinMaxSize(bool getMin) {
			LayoutRepositoryItemViewInfo viewInfo = ViewInfo as LayoutRepositoryItemViewInfo;
			Size bestSize = CalcFieldBestSize(null, viewInfo, EditorPreferredWidth);
			if(getMin) return bestSize;
			else {
				if(RepositoryItem is RepositoryItemMemoEdit || RepositoryItem is RepositoryItemPictureEdit) return Size.Empty;
				else return new Size(0, bestSize.Height);
			}
		}
		Size CalcFieldBestSize(Graphics g, LayoutRepositoryItemViewInfo viewInfo, int iWidth) {
			int iEditorAutoHeight = CalcFieldAutoHeight(g, viewInfo.RepositoryItemViewInfo, iWidth);
			return new Size(iWidth, iEditorAutoHeight);
		}
		int cachedAutoHeightCore = -1;
		int CalcFieldAutoHeight(Graphics g, BaseEditViewInfo editViewInfo, int width) {
			if(cachedAutoHeightCore == -1) {
				GraphicsInfo gInfo = new GraphicsInfo();
				try {
					gInfo.AddGraphics(g);
					IHeightAdaptable ah = editViewInfo as IHeightAdaptable;
					if(ah != null) {
						this.cachedAutoHeightCore = ah.CalcHeight(gInfo.Cache, width);
					}
					else {
						this.cachedAutoHeightCore = editViewInfo.CalcMinHeight(gInfo.Graphics);
					}
				}
				finally {
					gInfo.ReleaseGraphics();
					gInfo = null;
				}
			}
			return Math.Max(16, cachedAutoHeightCore);
		}
		protected internal override void SetShouldUpdateViewInfo() {
			base.SetShouldUpdateViewInfo();
			this.cachedAutoHeightCore = -1;
		}
		static RepositoryItem DefaultRepositoryItem = new RepositoryItemTextEdit();
	}
}

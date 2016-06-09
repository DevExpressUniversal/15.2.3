﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Design {
	public partial class TileItemElementsCollectionEditorForm : XtraForm {
		static TileItemElementsCollectionEditorForm() {
			SkinManager.EnableFormSkins();
		}
		public TileItemElementsCollectionEditorForm(ISite site, ITileItem sourceItem) {
			InitializeComponent();
			this.site = site;
			this.sourceItem = sourceItem;
			WindowsFormsDesignTimeSettings.ApplyDesignSettings(this);
		}
		ISite site;
		ITileItem sourceItem;
		public void Init<T>() where T : TileItem {
			if(sourceItem.Frames.Count > 0)
				frameIndex = sourceItem.CurrentFrameIndex;
			Content.Assign<T>(site, sourceItem);
		}
		public void Init() {
			if(sourceItem.Frames.Count > 0)
				frameIndex = sourceItem.CurrentFrameIndex;
			Content.Assign(site, sourceItem);
		}
		int frameIndex;
		#region Handlers
		void btnOK_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		protected override void OnClosing(CancelEventArgs e) {
			Content.OnClosing();
			base.OnClosing(e);
		}
		#endregion
		#region GetValue
		public TileItemElementCollection GetValue() {
			return Content.Elements;
		}
		public int GetFrameIndex() {
			return frameIndex;
		}
		#endregion
		TileItemElementsCollectionEditorControl Content { get { return tileItemElementsCollectionEditorControl1; } }
	}
}

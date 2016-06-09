#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor
{
	public partial class UnusableNodesForm : DevExpress.XtraEditors.XtraForm {
		public UnusableNodesForm(Dictionary<string, string> unusableNodesXmls) {
			InitializeComponent();
			FillUnusableNodes(unusableNodesXmls);
			Image unusableNodesImage = ImageLoader.Instance.GetImageInfo("ModelEditor_ShowUnusableNodes").Image;
			if(unusableNodesImage != null) {
				this.Icon = Icon.FromHandle(new Bitmap(unusableNodesImage).GetHicon());
			}
		}
		public void FillUnusableNodes(Dictionary<string, string> unusableNodesXmls) {
			if(unusableNodesXmls.Count >= 1) {
				foreach(KeyValuePair<string, string> unusableNodesXml in unusableNodesXmls) {
					if(string.IsNullOrEmpty(unusableNodesXml.Key)) {
						this.defaultLanguageMemoEdit.Text = unusableNodesXml.Value;
					}
					else {
						XtraTabPage tabPage = unusableNodesTab.TabPages.Add();
						tabPage.Text = unusableNodesXml.Key;
						MemoEdit edit = new MemoEdit();
						tabPage.Controls.Add(edit);
						edit.Dock = System.Windows.Forms.DockStyle.Fill;
						edit.Properties.ReadOnly = true;
						edit.Text = unusableNodesXml.Value;
					}
				}
			}
		}
	}
}

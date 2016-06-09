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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Data.XtraReports.Wizard;
using System.ComponentModel;
namespace DevExpress.XtraReports.Wizards3.Views {
	[ToolboxItem(false)]
	public partial class SummaryEditorsContainer : UserControl {
		ColumnInfoSummaryOptions[] datasource;
		public ColumnInfoSummaryOptions[] Datasource {
			get {
				return datasource;
			}
			set {
				datasource = value;
				FillSummaries();
			}
		}
		public SummaryEditorsContainer() {
			InitializeComponent();
		}
		void FillSummaries() {
			ResetEditors();
			if (datasource == null)
				return;
			foreach (var op in datasource) {
				var control = new SummaryOptionsItem(op);
				control.Dock = DockStyle.Top;
				var item = editorsContainerGroup.AddItem(string.Empty, control);
				item.TextVisible = false;
			}
			SetHeaderPadding();
		}
		void SetHeaderPadding() {
			summaryOptionsHeader1.Padding = new Padding(0, 0, layoutEditors.Width - layoutEditors.ClientWidth, 6);
		}
		protected override void OnPaint(PaintEventArgs e) {
			var rectangle = new Rectangle(200, 0, 1, this.Height);
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(UserLookAndFeel.Default)[CommonSkins.SkinLabelLineVert], rectangle);
			ObjectPainter.DrawObject(new GraphicsCache(e), SkinElementPainter.Default, info);
			rectangle = new Rectangle(0, 30, this.Width, 1);
			info = new SkinElementInfo(CommonSkins.GetSkin(UserLookAndFeel.Default)[CommonSkins.SkinLabelLine], rectangle);
			ObjectPainter.DrawObject(new GraphicsCache(e), SkinElementPainter.Default, info);
		}
		void ResetEditors() {
			editorsContainerGroup.Clear();
		}
	}
}

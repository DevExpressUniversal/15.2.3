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

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.Data.Utils;
using DevExpress.Design.TypePickEditor;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.DashboardWin.Design {
	public class DashboardSourceEditor : DashboardSourceEditorBase {
		const string LoadDashboardString = "Load Dashboard...";
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new DashboardSourceTypePickerTreeView(typeof(Dashboard));
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new DashboardSourcePickerPanel(picker, LoadDashboardString);
		}
	}
	class DashboardSourceTypePickerTreeView : DashboardSourceTypePickerTreeViewBase {
		const int FirstUriIndex = 1;
		public DashboardSourceTypePickerTreeView(Type type) : base(type) {
		}
		protected override void PushNodes(IDesignerHost designerHost) {
			base.PushNodes(designerHost);
			IDashboardListService dashboardListService = designerHost.GetService<IDashboardListService>();
			if(dashboardListService != null)
				foreach(Uri uri in dashboardListService)
					PushUriNode(uri, Nodes);
		}
		protected override TreeListNode PushCurrentValue(object currentValue, TreeListNode node, IDesignerHost designerHost) {
			IDashboardListService dashboardListService = designerHost.GetService<IDashboardListService>();
			if(dashboardListService != null && currentValue != null && node == null) {
				Uri uri = currentValue as Uri;
				if(uri != null) {
					dashboardListService.Add(uri);
					node = PushUriNode(uri, Nodes);
				} else {
					string str = currentValue as string;
					if(str != null) {
						Type type = null;
						try {
							type = Type.GetType(str);
						} catch { }
						if(type != null) {
							node = FindNodeByValue(Nodes, type);
						}
					}
				}
			}
			return node;
		}
		public PickerNode PushUriNode(Uri uri, TreeListNodes owner) {
			return InsertTaggedNode(FirstUriIndex, uri.AbsolutePath, uri.GetType(), uri, owner);
		}
	}
	class DashboardSourcePickerPanel : TypePickerPanel {
		static Color GetLinkColor(UserLookAndFeel lookAndFeel) {
			Color linkColor = Color.Empty;
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				Skin skin = EditorsSkins.GetSkin(lookAndFeel);
				linkColor = skin.Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor);
			}
			if(linkColor.IsEmpty)
				linkColor = Color.FromArgb(0x28, 0x59, 0xAE);
			return linkColor;
		}
		static LinkLabel CreateHyperLink() {
			LinkLabel linkLabel = new LinkLabel() {
				Bounds = new Rectangle(new Point(20, 4), new Size(200, 15)),
				AutoSize = true,
				BackColor = Color.Transparent,
				LinkBehavior = LinkBehavior.HoverUnderline
			};
			return linkLabel;
		}
		static PictureBox CreatePictureBox() {
			Bitmap bitmap = BitmapStorage.LoadBitmap("LoadDashboard_Color");
			bitmap.MakeTransparent(Color.Magenta);
			return new PictureBox() {
				Image = bitmap,
				BackColor = Color.Transparent,
				Width = bitmap.Width,
				Height = bitmap.Height,
				Location = new Point(4, 4),
				AccessibleRole = AccessibleRole.Graphic
			};
		}
		LinkLabel hyperLink;
		ControlUserLookAndFeel userLookAndFeel;
		protected DashboardSourceTypePickerTreeView TreeView {
			get { return (DashboardSourceTypePickerTreeView)base.treeView; }
		}
		public DashboardSourcePickerPanel(TypePickerTreeView treeView, string hyperLinkText)
			: base(treeView) {
			hyperLink = CreateHyperLink();
			hyperLink.Text = hyperLinkText;
			hyperLink.LinkClicked += OnHyperLinkClick;
			Panel bottomPanel = new Panel() {
				Height = Math.Max(24, hyperLink.GetPreferredSize(Size.Empty).Height + 2),
				Dock = DockStyle.Bottom,
				BorderStyle = BorderStyle.None
			};
			bottomPanel.Controls.AddRange(new Control[] { hyperLink, CreatePictureBox() });
			userLookAndFeel = new ControlUserLookAndFeel(this);
			PopupSeparatorLine separatorLine = new PopupSeparatorLine(userLookAndFeel) { Dock = DockStyle.Bottom };
			Height += (bottomPanel.Height + separatorLine.Height);
			Controls.AddRange(new Control[] { separatorLine, bottomPanel });
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(userLookAndFeel != null) {
					userLookAndFeel.Dispose();
					userLookAndFeel = null;
				}
				if(hyperLink != null) {
					hyperLink.LinkClicked -= OnHyperLinkClick;
					hyperLink.Dispose();
					hyperLink = null;
				}
			}
			base.Dispose(disposing);
		}
		public override void Initialize(ITypeDescriptorContext context, IServiceProvider provider, object currentValue) {
			base.Initialize(context, provider, currentValue);
			ILookAndFeelService lookAndFeelService = provider.GetService<ILookAndFeelService>();
			if(lookAndFeelService != null) {
				userLookAndFeel.ParentLookAndFeel = lookAndFeelService.LookAndFeel;
				Color linkColor = GetLinkColor(lookAndFeelService.LookAndFeel);
				hyperLink.VisitedLinkColor = hyperLink.ActiveLinkColor = hyperLink.LinkColor = linkColor;
			}
			hyperLink.Enabled = true;
		}
		protected virtual void OnHyperLinkClick(object sender, LinkLabelLinkClickedEventArgs e) {
			try {
				Uri uri = DashboardFileChooser.ChooseUri(null);
				TreeView.PushUriNode(uri, TreeView.Nodes);
				IDashboardListService dashboardListService = Provider.GetService<IDashboardListService>();
				dashboardListService.Add(uri);
			} catch { 
			}
		}
	}
}

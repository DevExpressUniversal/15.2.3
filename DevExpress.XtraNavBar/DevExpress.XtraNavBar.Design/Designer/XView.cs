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
using System.Windows.Forms;
using DevExpress.XtraNavBar;
namespace DevExpress.XtraNavBar.Frames {
	public class XView {
		private NavBarControl navBar;
		public XView(NavBarControl navBar) : this(navBar, false) {}
		public XView(NavBarControl navBar, bool temp) {
			this.navBar = navBar;
			navBar.BeginUpdate();
			InitImages();
			InitView(temp);
			Refresh();
			navBar.EndUpdate();
		}
		private void InitView(bool temp) {
			navBar.Groups.Add(SetGroup(NavBarGroupStyle.SmallIconsText, "Local", 0));
			navBar.Groups.Add(SetGroup(NavBarGroupStyle.LargeIconsText, "News", 1));
			navBar.Items.Add(SetItem("Inbox", 2));
			navBar.Items.Add(SetItem("OutBox", 3));
			navBar.Items.Add(SetItem("Sent Items", 4, false));
			navBar.Items.Add(SetItem("Deleted Items", 5));
			navBar.Items.Add(SetItem("Drafts", 6));
			navBar.Items.Add(SetItem("Temp", 7));
			navBar.Items.Add(SetItem("News", 8));
			AddItemLinks(0, new int[] {0, 1, 2, 3, 4});
			AddItemLinks(1, new int[] {6});
			if(temp) {
				navBar.Groups.Add(SetGroup(NavBarGroupStyle.SmallIconsText, "Temp", 7));
				AddItemLinks(2, new int[] {5});
			}
		}
		public void Refresh() {
			navBar.Groups[0].Expanded = true;
			navBar.Groups[1].Expanded = true;
			navBar.ActiveGroup = navBar.Groups[0];
		} 
		private NavBarGroup SetGroup(NavBarGroupStyle groupStyle, string caption, int index) {
			NavBarGroup group = new NavBarGroup();
			group.Caption = caption;
			group.GroupCaptionUseImage = groupStyle != NavBarGroupStyle.LargeIconsText ? NavBarImage.Small : NavBarImage.Large;
			group.GroupStyle = groupStyle;
			group.LargeImageIndex = group.SmallImageIndex = index;
			return group;
		}
		private NavBarItem SetItem(string caption, int index) {
			return SetItem(caption, index, true);
		}
		private NavBarItem SetItem(string caption, int index, bool enabled) {
			NavBarItem item = new NavBarItem();
			item.Caption = caption;
			item.LargeImageIndex = item.SmallImageIndex = index;
			item.Enabled = enabled;
			return item;
		}
		private void AddItemLinks(int groupIndex, int[] indexes) {
			foreach(int i in indexes)
				navBar.Groups[groupIndex].ItemLinks.Add(navBar.Items[i]);
		}
		private ImageList NewImageList(int x, string s) {
			ImageList iml = new ImageList();
			iml.ImageSize = new System.Drawing.Size(x, x);
			try {
				System.IO.Stream str = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraNavBar.Design.Images." + s + ".bmp");
				iml.Images.AddStrip(Bitmap.FromStream(str));
			} catch {}
			iml.TransparentColor = System.Drawing.Color.Magenta;
			return iml;
		}
		private void InitImages() {
			navBar.LargeImages = NewImageList(32, "Large");
			navBar.SmallImages = NewImageList(16, "Small");	
		}
	}
}

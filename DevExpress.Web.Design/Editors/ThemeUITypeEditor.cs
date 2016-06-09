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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ThemeUITypeEditorInfo {
		public ASPxWebControl Control { get; set; }
		public IWindowsFormsEditorService SmartTagEditorService { get; set; }
	}
	public class ThemeUITypeEditor : UITypeEditor {
		internal static readonly Size ThemeEditorSize = new Size(256, 420);
		internal static readonly Size ColorImageSize = new Size(54, 22);
		internal static readonly Font ItemFont = new Font("Segoe UI", 10);
		internal static readonly Font SelectedItemFont = new Font(ItemFont, FontStyle.Bold);
		internal static readonly int VerticalSpacing = 2;
		internal static readonly int HorizontalSpacing = 8;
		internal static Dictionary<string, Color> ThemeColors = new Dictionary<string, Color>();
		static ThemeUITypeEditor() {
			ThemeColors.Add("Default", Color.FromArgb(220, 220, 220));
			ThemeColors.Add("DevEx", Color.FromArgb(205, 204, 245));
			ThemeColors.Add("Metropolis", Color.FromArgb(127, 127, 127));
			ThemeColors.Add("MetropolisBlue", Color.FromArgb(0, 114, 198));
			ThemeColors.Add("Office2010Blue", Color.FromArgb(181, 204, 229));
			ThemeColors.Add("Office2010Black", Color.FromArgb(71, 71, 71));
			ThemeColors.Add("Office2010Silver", Color.FromArgb(231, 234, 239));
			ThemeColors.Add("Office2003Blue", Color.FromArgb(181, 206, 242));
			ThemeColors.Add("Office2003Olive", Color.FromArgb(212, 222, 182));
			ThemeColors.Add("Office2003Silver", Color.FromArgb(209, 209, 224));
			ThemeColors.Add("Moderno", Color.FromArgb(18, 121, 192));
			ThemeColors.Add("Mulberry", Color.FromArgb(191, 78, 106));
			ThemeColors.Add("iOS", Color.FromArgb(213, 213, 216));
			ThemeColors.Add("Aqua", Color.FromArgb(150, 185, 236));
			ThemeColors.Add("Glass", Color.FromArgb(200, 221, 232));
			ThemeColors.Add("BlackGlass", Color.FromArgb(77, 90, 99));
			ThemeColors.Add("PlasticBlue", Color.FromArgb(80, 102, 179));
			ThemeColors.Add("RedWine", Color.FromArgb(138, 14, 60));
			ThemeColors.Add("SoftOrange", Color.FromArgb(240, 111, 48));
			ThemeColors.Add("Youthful", Color.FromArgb(156, 197, 36));
		}
		static ImageList ImageList;
		static ListView ThemeEditorControl;
		private static ImageList GetImageList(List<string> themes) {
			if(ImageList == null) {
				ImageList = new ImageList();
				ImageList.ColorDepth = ColorDepth.Depth24Bit;
				ImageList.ImageSize = ColorImageSize;
				using(MemoryStream memoryStream = new MemoryStream()) {
					foreach(string theme in themes) {
						Bitmap bmp = new Bitmap(ColorImageSize.Width, ColorImageSize.Height);
						Graphics graphics = Graphics.FromImage(bmp);
						Rectangle rect = new Rectangle(HorizontalSpacing, VerticalSpacing, bmp.Size.Width - 1 - 2 * HorizontalSpacing, bmp.Size.Height - 1 - 2 * VerticalSpacing);
						graphics.FillRectangle(new SolidBrush(ThemeColors[theme]), rect);
						ImageList.Images.Add(theme, bmp);
					}
				}
			}
			return ImageList;
		}
		internal static List<string> GetThemes() {
			List<string> themes = new List<string>(ThemesProvider.GetThemes(true));
			themes.Insert(0, ThemesProvider.DefaultTheme);
			return themes;
		}
		private ListView GetThemeEditorControl(ThemeUITypeEditorInfo info) {
			List<string> themes = GetThemes();
			if(ThemeEditorControl == null) {
				ThemeEditorControl = new ListView();
				ThemeEditorControl.SmallImageList = GetImageList(themes);
				ThemeEditorControl.View = System.Windows.Forms.View.SmallIcon;
				ThemeEditorControl.Font = ItemFont;
				foreach(string theme in themes) {
					ListViewItem item = new ListViewItem(theme, theme);
					ThemeEditorControl.Items.Add(item);
				}
				ThemeEditorControl.Click += new EventHandler(delegate(object sender, EventArgs e) {
					ListView listView = (ListView)sender;
					ThemeUITypeEditorInfo editorInfo = listView.Tag as ThemeUITypeEditorInfo;
					if(editorInfo == null || editorInfo.Control == null) return;
					if(listView.SelectedItems.Count > 0 && editorInfo.Control.EnableTheming) {
						editorInfo.Control.EnableTheming = false;
						PropertyDescriptor themeProperty = TypeDescriptor.GetProperties(typeof(ASPxWebControl))[ThemableControlBuilder.ThemeAttributeName];
						themeProperty.SetValue(editorInfo.Control, listView.SelectedItems[0].Text);
						editorInfo.Control.EnableTheming = true;
					}
					if(editorInfo.SmartTagEditorService != null)
						editorInfo.SmartTagEditorService.CloseDropDown();
				});
				ThemeEditorControl.Size = ThemeEditorSize;
				ThemeEditorControl.BorderStyle = BorderStyle.None;
				ThemeEditorControl.HotTracking = false;
				ThemeEditorControl.MultiSelect = false;
				ThemeEditorControl.HideSelection = true;
			}
			ThemeEditorControl.Tag = info;
			foreach(ListViewItem listItem in ThemeEditorControl.Items) {
				listItem.Selected = (listItem.Text == info.Control.Theme);
				listItem.Font = listItem.Selected ? SelectedItemFont : ItemFont;
			}
			return ThemeEditorControl;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			ThemeUITypeEditorInfo info = new ThemeUITypeEditorInfo();
			if(context.Instance is DesignerActionList)
				info.Control = ((DesignerActionList)context.Instance).Component as ASPxWebControl;
			else
				info.Control = context.Instance as ASPxWebControl;
			if(info.Control != null) {
				info.SmartTagEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(info.SmartTagEditorService != null)
					info.SmartTagEditorService.DropDownControl(GetThemeEditorControl(info));
			}
			return base.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
	}
}

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
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraEditors;
using DevExpress.Utils.Menu;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Customization;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Customization.Templates;
using DevExpress.Utils.Text;
namespace DevExpress.XtraLayout.Customization
{
	public class DragHeaderPainter {
	   	DevExpress.LookAndFeel.BaseLookAndFeelPainters painters;
		public DragHeaderPainter(UserLookAndFeel owner, ActiveLookAndFeelStyle activeStyle) {
			painters = DevExpress.LookAndFeel.LookAndFeelPainterHelper.GetPainter(owner, activeStyle);
		}
		public void Paint(Graphics g, BaseLayoutItem item, ObjectState itemState, Rectangle bounds, AppearanceObject appearance) {
			if(item == null || g == null) return;
			GraphicsCache cache = new GraphicsCache(g);
			Image itemTypeIcon = null;
			LayoutClassificationArgs arg = LayoutClassifier.Default.Classify(item);
			if(arg.IsGroup) {
				itemTypeIcon = LayoutControlImageStorage.Default.DragHeaderPainter.Images[1];
			} else if(arg.IsTabbedGroup) {
				itemTypeIcon = LayoutControlImageStorage.Default.DragHeaderPainter.Images[2];
			}
			if(item is IFixedLayoutControlItem) {
				itemTypeIcon = (item as IFixedLayoutControlItem).CustomizationImage;
			}
			if(itemTypeIcon == null) {
				itemTypeIcon = LayoutControlImageStorage.Default.DragHeaderPainter.Images[0];
			}
			if(item.Tag is TemplateManager) {
				itemTypeIcon = null;
			}
			int imageWigth = 16;
			int indent = (bounds.Height - imageWigth) / 2;
			if(itemTypeIcon == null) imageWigth = indent = 0;
			HeaderObjectInfoArgs info = new HeaderObjectInfoArgs();
			info.Bounds = bounds;
			if(item is IFixedLayoutControlItem) {
				info.Caption = (item as IFixedLayoutControlItem).CustomizationName;
			} else
				info.Caption = StringPainter.Default.RemoveFormat(item.CustomizationFormText);
			info.Graphics = g;
			info.State = itemState;
			if(appearance != null) info.SetAppearance(appearance);
			info.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.None;
			painters.Header.CalcObjectBounds(info);
			info.CaptionRect = new Rectangle(new Point(info.CaptionRect.X + imageWigth + 2 * indent, info.CaptionRect.Y), new Size(info.CaptionRect.Width - imageWigth - 2 * indent, info.CaptionRect.Height));
			painters.Header.DrawObject(info);
			if(itemTypeIcon!=null)cache.Graphics.DrawImage(itemTypeIcon, new Rectangle(bounds.X + indent, bounds.Y + indent, imageWigth, imageWigth), 0, 0, itemTypeIcon.Size.Width, itemTypeIcon.Size.Height, System.Drawing.GraphicsUnit.Pixel);
			itemTypeIcon = null;
		}
	}
}

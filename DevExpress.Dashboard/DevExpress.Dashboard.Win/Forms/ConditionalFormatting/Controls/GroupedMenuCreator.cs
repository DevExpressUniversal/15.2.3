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
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.LookAndFeel;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	abstract class GroupedMenuCreator<TGroup, TType> where TGroup: struct where TType: struct  {
		const bool DefaultAllowAllUp = false;
		const int DefaultItemGroupIndex = 1;
		readonly FormatConditionColorScheme scheme;
		readonly List<BarItem> items = new List<BarItem>();
		readonly Dictionary<BarItem, TType> barItemTypes = new Dictionary<BarItem, TType>();
		BarCheckItem selectedItem = null;
		public BarCheckItem SelectedItem {
			get { return selectedItem; }
			private set {
				if(value == selectedItem) return;
				if(value == null) {
					this.selectedItem.AllowAllUp = true;
					this.selectedItem.Checked = false;
					this.selectedItem.AllowAllUp = DefaultAllowAllUp;
				}
				this.selectedItem = value;
				if(value != null)
					this.selectedItem.Checked = true;
			}
		}
		public TType SelectedType {
			get {
				if(selectedItem != null)
					return GetType(selectedItem);
				return default(TType);
			}
			set {
				foreach(BarItem barItem in items) {
					BarCheckItem barCheckItem = barItem as BarCheckItem;
					if(barCheckItem != null && object.Equals(value, GetType(barCheckItem))) {
						SelectedItem = barCheckItem;
						return;
					}
				}
				SelectedItem = null;
			}
		}
		protected FormatConditionColorScheme Scheme { get { return scheme; } } 
		protected GroupedMenuCreator(UserLookAndFeel lookAndFeel) {
			this.scheme = DashboardWinHelper.IsDarkScheme(lookAndFeel) ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light;
			if(!typeof(TGroup).IsEnum || !typeof(TType).IsEnum)
				throw new ArgumentException("GroupedMenuCreator: TGroup/TType must be an enumerated type");
		}
		public TType GetType(BarItem barItem) {
			return barItemTypes[barItem];
		}
		void SetType(BarItem barItem, TType type) {
			barItemTypes.Add(barItem, type);
		}
		protected abstract Image GetIconImage(TType type);
		protected abstract IList<TType> GetTypes(TGroup group);
		protected abstract string GetDescription();
		protected abstract string GetGroupCaption(TGroup group);
		protected abstract string GetTypeCaption(TType type);
		protected abstract string GetTypeDescription(TType type);
		protected abstract int GetItemsCount(TType type);
		public IList<BarItem> Initialize(Action<TType, BarItem> itemClick) {
			return Initialize((barItem) => itemClick(GetType(barItem), barItem));
		}
		public IList<BarItem> Initialize(Action<BarItem> itemClick) {
			items.Clear();
			barItemTypes.Clear();
			foreach(TGroup group in Enum.GetValues(typeof(TGroup))) {
				IList<TType> types = GetTypes(group);
				BarHeaderItem barHeaderItem = new BarHeaderItem();
				items.Add(barHeaderItem);
				barHeaderItem.Caption = GetGroupCaption(group);
				barHeaderItem.MultiColumn = DefaultBoolean.True;
				barHeaderItem.OptionsMultiColumn.ColumnCount = GetItemsCount(types[0]);
				barHeaderItem.OptionsMultiColumn.ImageHorizontalAlignment = Utils.Drawing.ItemHorizontalAlignment.Left;
				barHeaderItem.OptionsMultiColumn.UseMaxItemWidth = DefaultBoolean.False;
				foreach(TType type in types) {
					Image image = GetIconImage(type);
					if(image != null) {
						BarCheckItem rangeIconItem = new BarCheckItem();
						rangeIconItem.AllowAllUp = DefaultAllowAllUp;
						rangeIconItem.Caption = GetTypeCaption(type);
						rangeIconItem.Glyph = image;
						rangeIconItem.GroupIndex = DefaultItemGroupIndex;
						rangeIconItem.ButtonStyle = BarButtonStyle.Check;
						rangeIconItem.ItemClickFireMode = BarItemEventFireMode.Immediate;
						rangeIconItem.ItemClick += (sender, e) => {
							if(e.Item != this.selectedItem) {
								this.selectedItem = (BarCheckItem)e.Item;
								itemClick(this.selectedItem);
							}
						};
						rangeIconItem.SuperTip = new SuperToolTip();
						rangeIconItem.SuperTip.AllowHtmlText = DefaultBoolean.True;
						rangeIconItem.SuperTip.Items.Add(string.Format("<b>{0}", rangeIconItem.Caption));
						rangeIconItem.SuperTip.Items.Add(GetDescription());
						SetType(rangeIconItem, type);
						items.Add(rangeIconItem);
					}
				}
			}
			return items;
		}
		protected void DrawBorder(Graphics g, Rectangle bounds) {
			Color penColor = StyleSettingsContainerPainter.GetDefaultBorderColor(scheme);
			using(Pen pen = new Pen(penColor)) {
				g.DrawRectangle(pen, new Rectangle(bounds.Location, new Size(bounds.Width - 1, bounds.Height - 1)));
			}
		}
	}
}

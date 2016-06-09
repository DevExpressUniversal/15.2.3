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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Model {
	#region BorderInfoRepository
	public class BorderInfoRepository {
		#region Fields
		static readonly Dictionary<BorderLineStyle, Underline> underlineTable = CreateUnderlineTable();
		readonly List<BorderInfo> items;
		readonly DocumentModelUnitConverter unitConverter;
		int currentItemIndex;
		#endregion
		public BorderInfoRepository(DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
			this.items = new List<BorderInfo>();
			this.currentItemIndex = 1;
			PopulateItems();
		}
		#region Properties
		public List<BorderInfo> Items { get { return items; } }
		public int CurrentItemIndex {
			get { return currentItemIndex; }
			set {
				if (currentItemIndex == value)
					return;
				currentItemIndex = value;
				RaiseUpdateUI();
			}
		}
		public BorderInfo CurrentItem { get { return Items[CurrentItemIndex]; } }
		#endregion
		EventHandler onUpdateUI;
		protected internal event EventHandler UpdateUI { add { onUpdateUI += value; } remove { onUpdateUI -= value; } }
		protected internal virtual void RaiseUpdateUI() {
			if (onUpdateUI != null)
				onUpdateUI(this, EventArgs.Empty);
		}
		static Dictionary<BorderLineStyle, Underline> CreateUnderlineTable() {
			Dictionary<BorderLineStyle, Underline> result = new Dictionary<BorderLineStyle, Underline>();
			result.Add(BorderLineStyle.None, Underline.UnderlineNone);
			result.Add(BorderLineStyle.Nil, Underline.UnderlineNone);
			result.Add(BorderLineStyle.Single, new UnderlineSingle());
			result.Add(BorderLineStyle.Dotted, new UnderlineDotted());
			result.Add(BorderLineStyle.DashSmallGap, new UnderlineDashSmallGap());
			result.Add(BorderLineStyle.Dashed, new UnderlineDashed());
			result.Add(BorderLineStyle.DotDash, new UnderlineDashDotted());
			result.Add(BorderLineStyle.DotDotDash, new UnderlineDashDotDotted());
			result.Add(BorderLineStyle.Double, new UnderlineDouble());
			result.Add(BorderLineStyle.Wave, new UnderlineWave());
			result.Add(BorderLineStyle.DoubleWave, new UnderlineDoubleWave());
			return result;
		}
		protected internal virtual void PopulateItems() {
			AddItem(BorderLineStyle.None, 0);
			AddItem(BorderLineStyle.Single, 10); 
			AddItem(BorderLineStyle.Dotted, 10); 
			AddItem(BorderLineStyle.DashSmallGap, 10); 
			AddItem(BorderLineStyle.Dashed, 10); 
			AddItem(BorderLineStyle.DotDash, 10); 
			AddItem(BorderLineStyle.DotDotDash, 10); 
			AddItem(BorderLineStyle.Double, 10); 
			AddItem(BorderLineStyle.Triple, 10); 
			AddItem(BorderLineStyle.ThickThinSmallGap, 60); 
			AddItem(BorderLineStyle.ThinThickSmallGap, 60); 
			AddItem(BorderLineStyle.ThinThickThinSmallGap, 60); 
			AddItem(BorderLineStyle.ThickThinMediumGap, 60); 
			AddItem(BorderLineStyle.ThinThickMediumGap, 60); 
			AddItem(BorderLineStyle.ThinThickThinMediumGap, 60); 
			AddItem(BorderLineStyle.ThickThinLargeGap, 60); 
			AddItem(BorderLineStyle.ThinThickLargeGap, 60); 
			AddItem(BorderLineStyle.ThinThickThinLargeGap, 60); 
			AddItem(BorderLineStyle.Wave, 15); 
			AddItem(BorderLineStyle.DoubleWave, 15); 
			AddItem(BorderLineStyle.DashDotStroked, 60); 
			AddItem(BorderLineStyle.ThreeDEmboss, 60); 
			AddItem(BorderLineStyle.ThreeDEngrave, 60); 
			AddItem(BorderLineStyle.Inset, 15); 
			AddItem(BorderLineStyle.Outset, 15); 
			AddItem(BorderLineStyle.Disabled, 0);
		}
		protected internal virtual void AddItem(BorderLineStyle lineStyle, int widthInTwips) {
			BorderInfo borderInfo = new BorderInfo();
			borderInfo.Color = DXColor.Black;
			borderInfo.Width = unitConverter.TwipsToModelUnits(widthInTwips);
			borderInfo.Style = lineStyle;
			Items.Add(borderInfo);
		}
		public int GetItemIndexByLineStyle(BorderLineStyle style) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				if (items[i].Style == style) {
					return i;
				}
			}
			return -1;
		}
		public BorderInfo GetItemByLineStyle(BorderLineStyle style) {
			int index = GetItemIndexByLineStyle(style);
			if (index < 0)
				return null;
			else
				return items[index];
		}
		protected internal static Underline GetUnderlineByBorderLineStyle(BorderLineStyle style) {
			Underline result;
			if (!underlineTable.TryGetValue(style, out result))
				return new UnderlineSingle();
			else
				return result;
		}
	}
	#endregion
}

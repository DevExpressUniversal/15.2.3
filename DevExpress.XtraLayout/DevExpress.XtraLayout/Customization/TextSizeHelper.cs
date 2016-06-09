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
using DevExpress.XtraLayout;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Utils;
using System.Collections;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Text;
using System.Collections.Generic;
namespace DevExpress.XtraLayout.Customization {
	public class TextAlignManager {
		ILayoutControl control;
		TextHelperDispatcher dispatcher;
		public TextAlignManager(ILayoutControl control) {
			this.control = control;
			dispatcher = new TextHelperDispatcher(this);
		}
		public void Reset() {
			dispatcher.Reset();
		}
		public void Reset(List<BaseLayoutItem> affectedItems) {
			dispatcher.Reset(affectedItems);
		}
		public SizeF Scale {
			get { return control.AutoScaleFactor; }
		}
		public ILayoutControl Owner { get { return control; } }
		public TextAlignMode GetAllowedAlignType(BaseLayoutItem item) {
			TextAlignMode result = dispatcher.GetAvailableAlignType(item);
			return result;
		}
		public int GetTextWidth(BaseLayoutItem item) {
			int result = dispatcher.GetTextWidth(item);
			return result;
		}
		public int GetTextHeight(BaseLayoutItem item) {
			int result = dispatcher.GetTextAutoSize(item, item.PaintAppearanceItemCaption).Height;
			return result;
		}
		public bool GetAlignHiddenText(BaseLayoutItem item) {
			bool result = dispatcher.GetAlignHiddenText(item);
			return result;
		}
		public int GetTextToControlDistance(BaseLayoutItem item) {
			int result = dispatcher.GetTextToControlDistance(item);
			return result;
		}
		public SizeF GetTextAutoSize(LayoutControlItem item, AppearanceObject appObject) {
			SizeF result = SizeF.Empty;
			GraphicsInfo ginfo = new GraphicsInfo();
			try {
				ginfo.AddGraphics(null);
				SizeF size = dispatcher.CalcItemTextSize(item, ginfo, appObject);
				result = dispatcher.CalcLabelSize(item, size);
			} catch {
				result = dispatcher.GetTextAutoSize(item, appObject);
			} finally {
				ginfo.ReleaseGraphics();
			}
			return result;
		}
		internal int GetTextAutoSizeCalcCount() {
			return dispatcher.textAutoSizeCalcCount;
		}
	}
	public class GlobalMaxSizeCalculator : BaseVisitor {
		protected int maxSize;
		protected TextHelperDispatcher dispatcher;
		public GlobalMaxSizeCalculator(TextHelperDispatcher dispatcher) {
			this.dispatcher = dispatcher;
		}
		public void Reset() {
			maxSize = -1;
		}
		public int GlobalMaxSize {
			get { return maxSize; }
		}
		public override void Visit(BaseLayoutItem item) {
			int result = dispatcher.GetTextAutoWidth(item);
			TextAlignMode allowedMode = dispatcher.GetAvailableAlignType(item);
			if(result > maxSize && allowedMode == TextAlignMode.AlignInLayoutControl && item.TextVisible) { maxSize = result; }
		}
	}
	public class TextHelperDispatcher {
		TextAlignManager owner;
		GlobalMaxSizeCalculator calculator;
		Hashtable alignTypeCache, textToControlDistanceCache, localAlignedWidthCache, globalAlignedWidthCache, autoSizeCache;
		public TextHelperDispatcher(TextAlignManager manager) {
			owner = manager;
			calculator = new GlobalMaxSizeCalculator(this);
			alignTypeCache = new Hashtable();
			textToControlDistanceCache = new Hashtable();
			localAlignedWidthCache = new Hashtable();
			globalAlignedWidthCache = new Hashtable();
			autoSizeCache = new Hashtable();
		}
		public void Reset() {
			RemoveIfExistAllHashtable(null);
		}
		public void Reset(List<BaseLayoutItem> affectedItems) {
			foreach(BaseLayoutItem bli in affectedItems) {
				RemoveIfExistAllHashtable(bli);
			}
		}
		void RemoveIfExistAllHashtable(BaseLayoutItem bli) {
			RemoveIfExist(alignTypeCache, bli);
			RemoveIfExist(textToControlDistanceCache, bli);
			RemoveIfExist(autoSizeCache, bli);
			RemoveIfExist(localAlignedWidthCache, bli);
			RemoveIfExist(globalAlignedWidthCache, bli);
		}
		void RemoveIfExist(Hashtable hashTable, BaseLayoutItem bli) {
			if(bli == null) { hashTable.Clear(); return; }
			if(hashTable.ContainsKey(bli)) hashTable.Remove(bli);
		}
		protected BaseLayoutItem GetNonDefaultOptions(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			LayoutControlItem citem = item as LayoutControlItem;
			TabbedGroup tabbedGroup = item as TabbedGroup;
			if(group != null) {
				if(group.OptionsItemText != null && group.OptionsItemText.TextAlignMode == TextAlignModeGroup.UseParentOptions) {
					if(group.Parent != null) return GetNonDefaultOptions(group.Parent);
					else
						return group;
				} else
					return group;
			}
			if(citem != null) {
				if(citem.TextAlignMode == TextAlignModeItem.UseParentOptions) {
					if(citem.Parent != null) return GetNonDefaultOptions(citem.Parent);
				} else
					return citem;
			}
			if(tabbedGroup != null && tabbedGroup.Parent != null) return GetNonDefaultOptions(tabbedGroup.Parent);
			return item;
		}
		public bool GetAlignHiddenText(BaseLayoutItem item) {
			BaseLayoutItem eitem = GetNonDefaultOptions(item);
			LayoutGroup group = eitem as LayoutGroup;
			if(group != null) { return group.OptionsItemText.AlignControlsWithHiddenText; }
			return false;
		}
		public int GetTextAutoWidth(BaseLayoutItem item) {
			int result = GetTextAutoSize(item,item.PaintAppearanceItemCaption).Width;
			return result;
		}
		internal int textAutoSizeCalcCount = 0;
		public Size GetTextAutoSize(BaseLayoutItem item,AppearanceObject appObject) {
			if(autoSizeCache.Contains(item)) return (Size)autoSizeCache[item];
			textAutoSizeCalcCount++;
			Size result = Size.Empty;
			IFixedLayoutControlItem fixedItem = item as IFixedLayoutControlItem;
			LayoutControlItem citem = item as LayoutControlItem;
			bool ignoreTextSize = (fixedItem != null && fixedItem.AllowClipText);
			if(!ignoreTextSize && citem != null && GetAvailableAlignType(citem) != TextAlignMode.CustomSize) {
				if(citem.TextVisible) {
					GraphicsInfo ginfo = new GraphicsInfo();
					try {
						ginfo.AddGraphics(null);
						SizeF textSize = CalcItemTextSize(citem, ginfo, appObject);
						result = CalcLabelSize(citem, textSize);
					}
					finally { ginfo.ReleaseGraphics(); }
				}
			}
			autoSizeCache.Add(item, result);
			return result;
		}
		protected internal Size CalcLabelSize(LayoutControlItem citem, SizeF textSize) {
			Size labelSize = Size.Round(textSize);
			if(ImageCollection.IsImageExists(citem.Image, citem.Images, citem.ImageIndex)) {
				Size imageSize = citem.CalcItemImageSize(citem.Image, citem.Images, citem.ImageIndex);
				labelSize = citem.CalcItemTextAndImageSize(labelSize, imageSize, citem.ImageAlignment);
			}
			return labelSize;
		}
		protected internal SizeF CalcItemTextSize(LayoutControlItem item, GraphicsInfo ginfo , AppearanceObject appObject) {
			SizeF textSize = SizeF.Empty;
			AppearanceObject appearance = appObject;
			using (StringFormat sf = (StringFormat)appearance.GetStringFormat().Clone()) {
				if (appearance.TextOptions.HotkeyPrefix == HKeyPrefix.None && appearance.Options.UseTextOptions) sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
				else sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				if(!item.AllowHtmlStringInCaption) textSize = appearance.CalcTextSize(ginfo.Graphics, sf, item.Text, int.MaxValue);
				else textSize = item.ViewInfo.GetStringInfo(ginfo.Graphics,1000).Bounds.Size;
			}
			return textSize;
		}
		public int GetTextLocalAlignedWidth(BaseLayoutItem item) {
			if(item.Parent == null) return 0;
			else {
				if(localAlignedWidthCache.Contains(item)) return (int)localAlignedWidthCache[item];
				LayoutGroup group = item.Parent;
				int maxSize = GetTextAlignedWidthCore(group, false);
				localAlignedWidthCache.Add(item, maxSize);
				return maxSize;
			}
		}
		public int GetTextAlignedWidthRecursive(BaseLayoutItem item) {
			if(item.Parent == null) return 0;
			else {
				if(localAlignedWidthCache.Contains(item)) return (int)localAlignedWidthCache[item];
				LayoutGroup group = GetTopLayoutGroupForTextAlign(item);
				int maxSize = GetTextAlignedWidthCore(group, true);
				localAlignedWidthCache.Add(item, maxSize);
				return maxSize;
			}
		}
		protected LayoutGroup GetTopLayoutGroupForTextAlign(BaseLayoutItem item) {
			LayoutGroup result = item is LayoutGroup ? (LayoutGroup)item : item.Parent;
			while(result.Parent != null && result.OptionsItemText.TextAlignMode != TextAlignModeGroup.AlignWithChildren) {
				result = result.Parent;
			}
			return result;
		}
		private int GetTextAlignedWidthCore(LayoutGroup group, bool recursive) {
			int maxSize = 0;
			foreach(BaseLayoutItem tItem in new ArrayList(group.Items)) {
				TextAlignMode alignType = GetAvailableAlignType(tItem);
				if(alignType == TextAlignMode.AutoSize || alignType == TextAlignMode.CustomSize || !tItem.TextVisible && !recursive )
					continue;
				LayoutGroup tGroup = tItem as LayoutGroup;
				TabbedGroup tTabbedGroup = tItem as TabbedGroup;
				int tempSize = 0;
				if(tGroup != null && recursive && tGroup.OptionsItemText.TextAlignMode == TextAlignModeGroup.UseParentOptions) { tempSize = GetTextAlignedWidthCore(tGroup, true); }
				if(tTabbedGroup != null && recursive) {
					foreach(LayoutGroup tab in tTabbedGroup.TabPages) {
						if(tab.OptionsItemText.TextAlignMode == TextAlignModeGroup.UseParentOptions) tempSize = Math.Max(tempSize, GetTextAlignedWidthCore(tab, true));
					}
				}
				if(tempSize == 0) {
					tempSize = GetTextAutoWidth(tItem);
				}
				if(tempSize > maxSize)
					maxSize = tempSize;
			}
			return maxSize;
		}
		protected int GetTextGlobalAlignedWidth(LayoutGroup calculationRoot) {
			calculator.Reset();
			if(calculationRoot!=null) {
				if(globalAlignedWidthCache.Contains(calculationRoot))
					return (int)globalAlignedWidthCache[calculationRoot];
				else {
					calculationRoot.Accept(calculator);
					globalAlignedWidthCache.Add(calculationRoot, calculator.GlobalMaxSize);
				}
			}
			return calculator.GlobalMaxSize;
		}
		public int GetTextWidth(BaseLayoutItem item) {
			TextAlignMode alignType = GetAvailableAlignType(item);
			if(ShouldReturnZeroTextToControlDistance(item, GetNonDefaultOptions(item))) return 0;
			switch(alignType) {
				case TextAlignMode.AutoSize:
					return GetTextAutoWidth(item);
				case TextAlignMode.CustomSize:
					return -1;
				case TextAlignMode.AlignInGroups:
					return GetTextLocalAlignedWidth(item);
				case TextAlignMode.AlignInGroupsRecursive:
					return GetTextAlignedWidthRecursive(item);
				case TextAlignMode.AlignInLayoutControl:
					LayoutGroup calculationRoot = GetCalculationRoot(item);
					return GetTextGlobalAlignedWidth(calculationRoot);
			}
			return 0;
		}
		LayoutGroup GetCalculationRoot(BaseLayoutItem item) {
			LayoutGroup g = item.Parent;
			if(!(item is LayoutGroup) && g==null) return owner.Owner.RootGroup;
			while(g.Parent!=null) g=g.Parent;
			return g;
		}
		public int GetTextToControlDistance(BaseLayoutItem item) {
			if(item == null) return -1;
			if(textToControlDistanceCache.Contains(item)) return (int)textToControlDistanceCache[item];
			else {
				int result = GetTextToControlDistanceCore(item);
				textToControlDistanceCache.Add(item, result);
				return result;
			}
		}
		protected bool ShouldReturnZeroTextToControlDistance(BaseLayoutItem item, BaseLayoutItem eitem) {
			LayoutControlItem citem = eitem as LayoutControlItem;
			LayoutGroup groupCore = eitem as LayoutGroup;
			if(!item.TextVisible) {
				if(!GetAlignHiddenText(eitem))
					return true;
				else
					if(citem != null) {
						if(citem.TextAlignMode == TextAlignModeItem.AutoSize || citem.TextAlignMode == TextAlignModeItem.CustomSize)
							return true;
					}
				if(groupCore != null) {
					if(groupCore.OptionsItemText.TextAlignMode == TextAlignModeGroup.AutoSize || groupCore.OptionsItemText.TextAlignMode == TextAlignModeGroup.CustomSize)
						return true;
				}
			}
			return false;
		}
		protected int GetTextToControlDistanceCore(BaseLayoutItem item) {
			BaseLayoutItem eitem = GetNonDefaultOptions(item);
			LayoutControlItem citem = eitem as LayoutControlItem;
			LayoutGroup groupCore = eitem as LayoutGroup;
			if(ShouldReturnZeroTextToControlDistance(item, eitem)) return 0;
			if(citem != null) {
				return -1;
			}
			if(groupCore != null) {
				return CalcScaled(owner.Scale.Width, groupCore.OptionsItemText.GetTextToControlDistance(item));
			}
			return 0;
		}
		int CalcScaled(float factor, int size) {
			return (int)Math.Round((double)(factor * size));
		}
		public TextAlignMode GetAvailableAlignType(BaseLayoutItem item) {
			if(alignTypeCache.Contains(item)) return (TextAlignMode)alignTypeCache[item];
			else {
				TextAlignMode result = GetAvailableAlignTypeCore(item);
				alignTypeCache.Add(item, result);
				return result;
			}
		}
		protected TextAlignMode GetAvailableAlignTypeCore(BaseLayoutItem item) {
			BaseLayoutItem eitem = GetNonDefaultOptions(item);
			LayoutControlItem citem = eitem as LayoutControlItem;
			LayoutGroup groupCore = eitem as LayoutGroup;
			if(citem != null) {
				switch(citem.TextAlignMode) {
					case TextAlignModeItem.AutoSize: return TextAlignMode.AutoSize;
					case TextAlignModeItem.CustomSize: return TextAlignMode.CustomSize;
				}
			}
			if(groupCore != null && groupCore.OptionsItemText != null) {
				switch(groupCore.OptionsItemText.TextAlignMode) {
					case TextAlignModeGroup.AlignLocal: return TextAlignMode.AlignInGroups;
					case TextAlignModeGroup.AutoSize: return TextAlignMode.AutoSize;
					case TextAlignModeGroup.CustomSize: return TextAlignMode.CustomSize;
					case TextAlignModeGroup.AlignWithChildren: return TextAlignMode.AlignInGroupsRecursive;
					case TextAlignModeGroup.UseParentOptions:
						if(groupCore.Parent == null && groupCore.Owner != null && groupCore.Owner.OptionsItemText != null)
							return groupCore.Owner.OptionsItemText.TextAlignMode;
						else return TextAlignMode.CustomSize;
				}
			}
			return TextAlignMode.CustomSize;
		}
	}
}

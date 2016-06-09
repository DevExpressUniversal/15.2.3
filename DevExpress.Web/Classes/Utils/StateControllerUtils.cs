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

using System.Collections.Generic;
using System.Text;
#if ASP
using System.Web.UI;
#else
using DevExpress.vNext.Internal;
#endif
namespace DevExpress.Web.Internal {
	public class StateScriptRenderHelper {
		private IUrlResolutionService urlResolutionService = null;
		private string name = "";
		private List<StateScriptRenderHelperItem> items = new List<StateScriptRenderHelperItem>();
		public StateScriptRenderHelper(IUrlResolutionService urlResolutionService, string name) {
			this.urlResolutionService = urlResolutionService;
			this.name = name;
		}
		public IUrlResolutionService UrlResolutionService {
			get { return urlResolutionService; }
		}
		public string Name {
			get { return name; }
		}
		public List<StateScriptRenderHelperItem> Items {
			get { return items; }
		}
		public void AddStyle(AppearanceStyleBase style, string itemName, bool enabled) {
			AddStyle(style, itemName, new string[0], enabled);
		}
		public void AddStyle(AppearanceStyleBase style, string itemName, string[] postfixes, bool enabled) {
			AddStyle(style, itemName, postfixes, new string[0], new string[0], enabled);
		}
		public void AddStyle(AppearanceStyleBase style, string itemName, string[] postfixes, object itemImageObject, string imagePostfix, bool enabled) {
			AddStyle(style, itemName, postfixes, new object[1] { itemImageObject }, new string[1] { imagePostfix }, enabled);
		}
		public void AddStyle(AppearanceStyleBase style, string itemName, string[] postfixes, object[] itemImageObjects, string[] imagePostfixes, bool enabled) {
			AppearanceStyleBase[] styles = new AppearanceStyleBase[] { style };
			AddStyles(styles, itemName, postfixes, itemImageObjects, imagePostfixes, enabled);
		}
		public void AddStyles(AppearanceStyleBase[] styles, string itemName, string[] postfixes, object[] itemImageObjects, string[] imagePostfixes, bool enabled) {
			StateScriptRenderHelperItem item = FindItem(styles, postfixes, imagePostfixes, enabled);
			if(item == null)
				Items.Add(new StateScriptRenderHelperItem(styles, itemName, postfixes, itemImageObjects, imagePostfixes, enabled));
			else
				item.AddItem(itemName, itemImageObjects);
		}
		protected StateScriptRenderHelperItem FindItem(AppearanceStyleBase[] styles, string[] postfixes, string[] imagePostfixes, bool enabled) {
			for (int i = 0; i < Items.Count; i++) {
				if(Items[i].IsEquals(UrlResolutionService, styles, postfixes, imagePostfixes, enabled))
					return Items[i];
			}
			return null;
		}
		public void GetCreateHoverScript(StringBuilder stb) {
			GetCreateScript(stb, "ASPx.AddHoverItems", GetItems(true));
			GetCreateScript(stb, "ASPx.RemoveHoverItems", GetItems(false));
		}
		public void GetCreatePressedScript(StringBuilder stb) {
			GetCreateScript(stb, "ASPx.AddPressedItems", GetItems(true));
			GetCreateScript(stb, "ASPx.RemovePressedItems", GetItems(false));
		}
		public void GetCreateSelectedScript(StringBuilder stb) {
			GetCreateScript(stb, "ASPx.AddSelectedItems", GetItems(true));
			GetCreateScript(stb, "ASPx.RemoveSelectedItems", GetItems(false));
		}
		public void GetCreateDisabledScript(StringBuilder stb) {
			GetCreateScript(stb, "ASPx.AddDisabledItems", GetItems(true));
			GetCreateScript(stb, "ASPx.RemoveDisabledItems", GetItems(false));
		}
		protected List<StateScriptRenderHelperItem> GetItems(bool enabled) {
			List<StateScriptRenderHelperItem> list = new List<StateScriptRenderHelperItem>();
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].Enabled == enabled) 
					list.Add(Items[i]);
			}
			return list;
		}
		protected void GetCreateScript(StringBuilder stb, string clientMethod, List<StateScriptRenderHelperItem> items) {
			if(items.Count == 0) return;
			stb.Append(clientMethod);
			stb.Append("(");
			stb.Append(HtmlConvertor.ToScript(Name));
			for(int i = 0; i < items.Count; i++) {
				stb.Append(",");
				if(i == 0)
					stb.Append('[');
				GetCreateItemScript(items[i], stb);
			}
			stb.Append("]);\n");
		}
		protected void GetCreateItemScript(StateScriptRenderHelperItem item, StringBuilder stb) {
			stb.Append('[');
			if(item.Enabled) {
				stb.Append(item.GetCssClassScript(UrlResolutionService));
				stb.Append("," + item.GetCssTextScript(UrlResolutionService));
				stb.Append("," + item.GetItemNamesScript());
				if(item.HasPostfixes()) 
					stb.Append("," + item.GetPostfixesScript());
				if(item.HasImageUrls()) {
					stb.Append("," + item.GetItemImageObjectsScript());
					stb.Append("," + item.GetImagePostfixScript());
				}
			}
			else {
				stb.Append(item.GetItemNamesScript());
				if(item.HasPostfixes())
					stb.Append("," + item.GetPostfixesScript());
			}
			stb.Append(']');
		}
	}
	public class StateScriptRenderHelperItem {
		private List<AppearanceStyleBase> styles = new List<AppearanceStyleBase>();
		private List<string> itemNames = new List<string>();
		private List<object[]> itemImageObjects = new List<object[]>();
		private string[] postfixes = new string[] { };
		private string[] imagePostfixes = new string[] { };
		private bool enabled = true;
		public StateScriptRenderHelperItem(AppearanceStyleBase[] styles, string itemName, string[] postfixes,
			object[] itemImageObjects, string[] imagePostfixes, bool enabled) {
			this.styles.AddRange(styles);
			this.postfixes = postfixes;
			this.imagePostfixes = imagePostfixes;
			this.enabled = enabled;
			AddItem(itemName, itemImageObjects);
		}
		public List<AppearanceStyleBase> Styles {
			get { return styles; }
		}
		public List<string> ItemNames {
			get { return itemNames; }
		}
		public List<object[]> ItemImageObjects {
			get { return itemImageObjects; }
		}
		public string[] Postfixes {
			get { return postfixes; }
		}
		public string[] ImagePostfixes {
			get { return imagePostfixes; }
		}
		public bool Enabled {
			get { return enabled; }
		}
		public void AddItem(string name, object[] imageObjects) {
			ItemNames.Add(name);
			ItemImageObjects.Add(imageObjects);
		}
		protected bool IsStylesEquals(IUrlResolutionService urlResolutionService) {
			if(Styles.Count == 0) return true;
			AppearanceStyleBase style = Styles[0];
			for(int i = 1; i < Styles.Count; i++) {
				if(!CommonUtils.AreEqualsStyles(urlResolutionService, style, Styles[i]))
					return false;
			}
			return true;
		}
		protected string GetStyleCssClass(AppearanceStyleBase style) {
			return (style != null) ? style.CssClass : "";
		}
		protected string GetStyleCssText(AppearanceStyleBase style, IUrlResolutionService urlResolutionService) {
			string cssText = (style != null) ? style.GetStyleAttributes(urlResolutionService).Value : "";
			return (cssText != null) ? cssText : "";
		}
		public string GetCssClassScript(IUrlResolutionService urlResolutionService) {
			List<string> cssClasses = new List<string>();
			if(IsStylesEquals(urlResolutionService))
				cssClasses.Add(GetStyleCssClass(Styles[0]));
			else { 
				for(int i = 0; i < Styles.Count; i++)
					cssClasses.Add(GetStyleCssClass(Styles[i]));
			}
			return HtmlConvertor.ToJSON(cssClasses);
		}
		public string GetCssTextScript(IUrlResolutionService urlResolutionService) {
			List<string> cssTexts = new List<string>();
			if(IsStylesEquals(urlResolutionService))
				cssTexts.Add(GetStyleCssText(Styles[0], urlResolutionService));
			else {
				for(int i = 0; i < Styles.Count; i++)
					cssTexts.Add(GetStyleCssText(Styles[i], urlResolutionService));
			}
			return HtmlConvertor.ToJSON(cssTexts);
		}
		public string GetItemNamesScript() {
			return HtmlConvertor.ToJSON(ItemNames);
		}
		protected internal bool HasPostfixes() {
			return Postfixes.Length > 0 || HasImageUrls();
		}
		public string GetPostfixesScript() {
			if(HasPostfixes())
				return (Postfixes.Length > 0 ? HtmlConvertor.ToJSON(Postfixes) : "");
			return string.Empty;
		}
		public string GetItemImageObjectsScript() {
			if (HasImageUrls()) {
				List<object> list = new List<object>();
				foreach (object[] images in ItemImageObjects)
					list.Add(images);
				return HtmlConvertor.ToJSON(list);
			}
			return string.Empty;
		}
		public string GetImagePostfixScript() {
			if (HasImageUrls())
				return HtmlConvertor.ToJSON(ImagePostfixes);
			return string.Empty;
		}
		protected internal bool HasImageUrls() {
			foreach (object[] images in ItemImageObjects) {
				for(int i = 0; i < images.Length; i++) {
					if(images[i] != null)
						return true;
				}
			}
			return false;
		}
		public bool IsEquals(IUrlResolutionService urlResolutionService, AppearanceStyleBase[] styles, string[] postfixes, string[] imagePostfixes, bool enabled) {
			if(Enabled == enabled && Styles.Count == styles.Length
				&& CommonUtils.AreEqualsArrays(Postfixes, postfixes)
				&& CommonUtils.AreEqualsArrays(ImagePostfixes, imagePostfixes)) {
				for(int i = 0; i < Styles.Count; i++) {
					if(!CommonUtils.AreEqualsStyles(urlResolutionService, Styles[i], styles[i]))
						return false;
				}
				return true;
			}
			return false;
		}
	}
}

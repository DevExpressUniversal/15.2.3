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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebFontInfoCache {
		#region Default Fonts
		public static readonly WebFontInfo[] DefaultFonts = new WebFontInfo[] {
			new WebFontInfo("Angsana New", true) {			CssString="'Angsana New', serif"						},
			new WebFontInfo("Arial", true) {				  CssString="Arial, Helvetica, sans-serif"				},
			new WebFontInfo("Arial Black", true) {			CssString="'Arial Black', Gadget, sans-serif"		  },
			new WebFontInfo("Batang", true) {				 CssString="Batang, 바탕, serif"						},
			new WebFontInfo("Book Antiqua", true) {		   CssString="'Book Antiqua', serif"					   },
			new WebFontInfo("Bookman Old Style", true) {	  CssString="'Bookman Old Style', serif"				  },
			new WebFontInfo("Calibri", true) {				CssString="Calibri, sans-serif"						},
			new WebFontInfo("Cambria", true) {				CssString="Cambria, serif"							 },
			new WebFontInfo("Candara", true) {				CssString="Candara, sans-serif"						},
			new WebFontInfo("Century", true) {				CssString="Century, serif"							 },
			new WebFontInfo("Century Gothic", true) {		 CssString="'Century Gothic', sans-serif"				},
			new WebFontInfo("Century Schoolbook", true) {	 CssString="'Century Schoolbook', serif"				 },
			new WebFontInfo("Comic Sans MS", true) {		  CssString="'Comic Sans MS', fantasy, cursive, sans-serif" },
			new WebFontInfo("Consolas", true) {			   CssString="Consolas, monospace"						},
			new WebFontInfo("Constantia", true) {			 CssString="Constantia, serif"						  },
			new WebFontInfo("Corbel", true) {				 CssString="Corbel, sans-serif"						 },
			new WebFontInfo("Cordia New", true) {			 CssString="'Cordia New', sans-serif"					},
			new WebFontInfo("Courier New", true) {			CssString="'Courier New', Courier, monospace"					},
			new WebFontInfo("DaunPenh", true) {			   CssString="DaunPenh, sans-serif"					   },
			new WebFontInfo("Dotum", true) {				  CssString="Dotum, 돋움, sans-serif"					},
			new WebFontInfo("FangSong", true) {			   CssString="FangSong, serif"							},
			new WebFontInfo("Franklin Gothic Book", true) {   CssString="'Franklin Gothic Book', sans-serif"		  },
			new WebFontInfo("Franklin Gothic Medium", true) { CssString="'Franklin Gothic Medium', sans-serif"		},
			new WebFontInfo("Garamond", true) {			   CssString="Garamond, serif"							},
			new WebFontInfo("Gautami", true) {				CssString="Gautami, sans-serif"						},
			new WebFontInfo("Georgia", true) {				CssString="Georgia, serif"							 },
			new WebFontInfo("Gill Sans MT", true) {		   CssString="'Gill Sans MT', sans-serif"				  },
			new WebFontInfo("Gulim", true) {				  CssString="Gulim, 굴림, sans-serif"					},
			new WebFontInfo("GungSuh", true) {				CssString="GungSuh, serif"							 },
			new WebFontInfo("Impact", true) {				 CssString="Impact, Charcoal, sans-serif"				  },
			new WebFontInfo("Iskoola Pota", true) {		   CssString="'Iskoola Pota', sans-serif"				  },
			new WebFontInfo("KaiTi", true) {				  CssString="KaiTi, sans-serif"						  },
			new WebFontInfo("Kalinga", true) {				CssString="Kalinga, sans-serif"						},
			new WebFontInfo("Kartika", true) {				CssString="Kartika, sans-serif"						},
			new WebFontInfo("Latha", true) {				  CssString="Latha, sans-serif"						  },
			new WebFontInfo("Lucida Console", true) {		 CssString="'Lucida Console', Monaco, monospace"		},
			new WebFontInfo("Lucida Sans", true) {			CssString="'Lucida Sans', sans-serif"				   },
			new WebFontInfo("Lucida Sans Unicode", true) {	CssString="'Lucida Sans Unicode', 'Lucida Grande', sans-serif" },
			new WebFontInfo("Malgun Gothic", true) {		  CssString="'Malgun Gothic', '맑은 고딕', sans-serif"	 },
			new WebFontInfo("Mangal", true) {				 CssString="Mangal, sans-serif"						 },
			new WebFontInfo("Meiryo", true) {				 CssString="Meiryo, メイリオ, sans-serif"				  },
			new WebFontInfo("Microsoft JhengHei", true) {	 CssString="'Microsoft JhengHei', 微軟正黑體, sans-serif" },
			new WebFontInfo("Microsoft YaHei", true) {		CssString="'Microsoft YaHei', 微软雅黑, sans-serif"	  },
			new WebFontInfo("MingLiU", true) {				CssString="MingLiU, 細明體, serif"					  },
			new WebFontInfo("MingLiU_HKSCS", true) {		  CssString="MingLiU_HKSCS, 細明體_HKSCS, serif"		  },
			new WebFontInfo("MS Gothic", true) {			  CssString="'MS Gothic', 'ＭＳ ゴシック', sans-serif"	   },
			new WebFontInfo("MS Mincho", true) {			  CssString="'MS Mincho', 'ＭＳ 明朝', serif"			  },
			new WebFontInfo("MS PGothic", true) {			 CssString="'MS PGothic', 'ＭＳ Ｐゴシック', sans-serif"	},
			new WebFontInfo("MS PMincho", true) {			 CssString="'MS PMincho', 'ＭＳ Ｐ明朝', serif"		   },
			new WebFontInfo("Nyala", true) {				  CssString="Nyala, sans-serif"						  },
			new WebFontInfo("Palatino Linotype", true) {	  CssString="'Palatino Linotype', 'Book Antiqua', Palatino, serif" },
			new WebFontInfo("PMingLiU", true) {			   CssString="PMingLiU, 新細明體, serif"				   },
			new WebFontInfo("PMingLiU-ExtB", true) {		  CssString="PMingLiU-ExtB, 新細明體-ExtB, 新細明體-ExtB, PMingLiU, serif" },
			new WebFontInfo("Raavi", true) {				  CssString="Raavi, sans-serif"						  },
			new WebFontInfo("Rockwell", true) {			   CssString="Rockwell, serif"							},
			new WebFontInfo("Segoe UI", true) {			   CssString="'Segoe UI', sans-serif"					  },
			new WebFontInfo("Segoe UI Light", true) {		 CssString="'Segoe UI Light', sans-serif"				},
			new WebFontInfo("Shruti", true) {				 CssString="Shruti, sans-serif"						 },
			new WebFontInfo("SimHei", true) {				 CssString="SimHei, 黑体, sans-serif"				   },
			new WebFontInfo("SimSun", true) {				 CssString="SimSun, 宋体, serif"						},
			new WebFontInfo("SimSun-ExtB", true) {			CssString="SimSun-ExtB, serif"						 },
			new WebFontInfo("Sylfaen", true) {				CssString="Sylfaen, serif"							 },
			new WebFontInfo("Tahoma", true) {				 CssString="Tahoma, Geneva, sans-serif"						 },
			new WebFontInfo("Times", true) {				  CssString="Times, 'Times New Roman', serif"			},
			new WebFontInfo("Times New Roman", true) {		CssString="'Times New Roman', Times, serif"					},
			new WebFontInfo("Trebuchet MS", true) {		   CssString="'Trebuchet MS', Helvetica, sans-serif"				  },
			new WebFontInfo("Tunga", true) {				  CssString="Tunga, sans-serif"						  },
			new WebFontInfo("TW Cen MT", true) {			  CssString="'TW Cen MT', sans-serif"					 },
			new WebFontInfo("Verdana", true) {				CssString="Verdana, Geneva, sans-serif"						},
			new WebFontInfo("Vrinda", true) {				 CssString="Vrinda, sans-serif"						 }
		};
		#endregion
		public WebFontInfoCache() {
			InnerCacheTable = new ConcurrentDictionary<WebFontInfo, int>();
			foreach(var item in DefaultFonts)
				AppendItem(item);
		}
		protected internal void CopyFrom(WebFontInfoCache source) {
			InnerCacheTable.Clear();
			foreach(var item in source.InnerCacheTable) {
				InnerCacheTable.AddOrUpdate(new WebFontInfo(item.Key.Name, item.Key.CanBeSet, item.Key.CssString), item.Value, (fi, ind) => ind);
			}
		}
		protected internal ConcurrentDictionary<WebFontInfo, int> InnerCacheTable { get; private set; }
		public int AddItem(string fontName) {
			return AddItem(new WebFontInfo(fontName, false));
		}
		public int AddItem(WebFontInfo fontInfo) {
			if(fontInfo == null)
				return -1;
			int index;
			if(InnerCacheTable.TryGetValue(fontInfo, out index))
				return index;
			return AppendItem(fontInfo);
		}
		public WebFontInfo GetItem(int index) {
			foreach(var item in InnerCacheTable) {
				if (item.Value == index)
					return item.Key;
			}
			return null;
		}
		int AppendItem(WebFontInfo fontInfo) {
			var index = InnerCacheTable.Count;
			return InnerCacheTable.AddOrUpdate(fontInfo, index, (fi, ind) => ind);
		}
		public void FillHashtable(System.Collections.Hashtable result, int startIndex) {
			if(startIndex < InnerCacheTable.Count) {
				var hashTable = new Hashtable();
				foreach(var item in InnerCacheTable) {
					if(item.Value >= startIndex) {
						var itemHashtable = new Hashtable();
						item.Key.FillHashtable(itemHashtable);
						hashTable[item.Value] = itemHashtable;
					}
				}
				result.Add("fontInfoCache", hashTable);
			}
		}
		public IEnumerable<WebFontInfo> GetCustomItems() {
			var defaultItemsCount = DefaultFonts.Length;
			return InnerCacheTable.Where(kv => kv.Value >= defaultItemsCount).OrderBy(kv => kv.Value).Select(kv => kv.Key);
		}
	}
}

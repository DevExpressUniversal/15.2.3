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
using DevExpress.Utils;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public sealed class DXWebAttributeCollection {
		DXStateBag bag;
		DXCssStyleCollection styleCollection;
		public int Count {
			get { return bag.Count; }
		}
		public DXCssStyleCollection CssStyle {
			get {
				if(styleCollection == null)
					styleCollection = new DXCssStyleCollection(bag);
				return styleCollection;
			}
		}
		public string this[string key] {
			get {
				if(styleCollection != null && DXWebStringUtil.EqualsIgnoreCase(key, "style"))
					return styleCollection.Value;
				return bag[key] as string;
			}
			set { Add(key, value); }
		}
		public ICollection Keys {
			get { return bag.Keys; }
		}
		public DXWebAttributeCollection(DXStateBag bag) {
			this.bag = bag;
		}
		public void Add(string key, string value) {
			if(styleCollection != null && StringExtensions.CompareInvariantCultureIgnoreCase(key, "style") == 0)
				styleCollection.Value = value;
			else
				bag[key] = value;
		}
		public void AddAttributes(DXHtmlTextWriter writer) {
			if(bag.Count > 0) {
				IDictionaryEnumerator enumerator = bag.GetEnumerator();
				while(enumerator.MoveNext()) {
					StateItem item = enumerator.Value as StateItem;
					if(item != null) {
						string str = item.Value as string;
						string key = enumerator.Key as string;
						if(key != null && str != null)
							writer.AddAttribute(key, str, true);
					}
				}
			}
		}
		public void Clear() {
			bag.Clear();
			if(styleCollection != null)
				styleCollection.Clear();
		}
		public override bool Equals(object o) {
			DXWebAttributeCollection attributes = o as DXWebAttributeCollection;
			if(attributes == null)
				return false;
			if(attributes.Count != bag.Count)
				return false;
			foreach(DictionaryEntry entry in bag)
				if(this[(string)entry.Key] != attributes[(string)entry.Key])
					return false;
			return true;
		}
		public override int GetHashCode() {
			DXHashCodeCombiner combiner = new DXHashCodeCombiner();
			foreach(DictionaryEntry entry in bag) {
				combiner.AddObject(entry.Key);
				combiner.AddObject(entry.Value);
			}
			return combiner.CombinedHash32;
		}
		public void Remove(string key) {
			if(styleCollection != null && DXWebStringUtil.EqualsIgnoreCase(key, "style"))
				styleCollection.Clear();
			else
				bag.Remove(key);
		}
		public void Render(DXHtmlTextWriter writer) {
			if(bag.Count > 0) {
				IDictionaryEnumerator enumerator = bag.GetEnumerator();
				while(enumerator.MoveNext()) {
					StateItem item = enumerator.Value as StateItem;
					if(item != null) {
						string str = item.Value as string;
						string key = enumerator.Key as string;
						if(key != null && str != null)
							writer.WriteAttribute(key, str, true);
					}
				}
			}
		}
	}
}

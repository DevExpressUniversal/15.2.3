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
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class HtmlEditorClientState {
		private ASPxHtmlEditor htmlEditor;
		private Dictionary<string, object> fieldsNameValueCollection = new Dictionary<string, object>();
		private const string ActiveViewFieldName = "ActiveView";
		private const string CurrentWidthFieldName = "CurrentWidth";
		private const string CurrentHeightFieldName = "CurrentHeight";
		private const string IsPercentWidthFieldName = "IsPercentWidth";
		private const string ForeColorPaletteName = "ForeColorPalette";
		private const string BackColorPaletteName = "BackColorPalette";
		private const string RibbonName = "Ribbon";
		public HtmlEditorClientState(ASPxHtmlEditor htmlEditor) {
			this.htmlEditor = htmlEditor;
		}
		public HtmlEditorView ActiveView {
			get { return (HtmlEditorView)this.fieldsNameValueCollection[ActiveViewFieldName]; }
			set { this.fieldsNameValueCollection[ActiveViewFieldName] = value; }
		}
		public int CurrentWidth {
			get { return (int)this.fieldsNameValueCollection[CurrentWidthFieldName]; }
			set { this.fieldsNameValueCollection[CurrentWidthFieldName] = value; }
		}
		public bool IsPercentWidth {
			get { return (int)this.fieldsNameValueCollection[IsPercentWidthFieldName] == 1; }
			set { this.fieldsNameValueCollection[IsPercentWidthFieldName] = (value) ? 1 : 0; }
		}
		public int CurrentHeight {
			get { return (int)this.fieldsNameValueCollection[CurrentHeightFieldName]; }
			set { this.fieldsNameValueCollection[CurrentHeightFieldName] = value; }
		}
		public string ForeColorPalette {
			get { return fieldsNameValueCollection.ContainsKey(ForeColorPaletteName) ? (string)this.fieldsNameValueCollection[ForeColorPaletteName] : ""; }
			set { this.fieldsNameValueCollection[ForeColorPaletteName] = value; }
		}
		public string BackColorPalette {
			get { return fieldsNameValueCollection.ContainsKey(BackColorPaletteName) ? (string)this.fieldsNameValueCollection[BackColorPaletteName] : ""; }
			set { this.fieldsNameValueCollection[BackColorPaletteName] = value; }
		}
		public string Ribbon {
			get { return fieldsNameValueCollection.ContainsKey(RibbonName) ? (string)this.fieldsNameValueCollection[RibbonName] : ""; }
			set { this.fieldsNameValueCollection[RibbonName] = value; }
		}
		public string GetSerializableStateString() {
			SaveClientState();
			return ToString();
		}
		public bool Load(string state) {
			return DictionarySerializer.Deserialize(state, this.fieldsNameValueCollection, ParseFieldValue);
		}
		public override string ToString() {
			return DictionarySerializer.Serialize(this.fieldsNameValueCollection);
		}
		private ASPxHtmlEditor HtmlEditor {
			get { return this.htmlEditor; }
		}
		private object ParseFieldValue(string name, string value) {
			switch(name) {
				case ActiveViewFieldName:
					return Enum.Parse(typeof(HtmlEditorView), value);
				case CurrentWidthFieldName:
					return Int32.Parse(value);
				case CurrentHeightFieldName:
					return Int32.Parse(value);
				case IsPercentWidthFieldName:
					return Int32.Parse(value);
				case ForeColorPaletteName:
					return value;
				case BackColorPaletteName:
					return value;
				case RibbonName:
					return value;
				default:
					throw new ArgumentException();
			}
		}
		private void SaveClientState() {
			string ribbonState = Ribbon;
			this.fieldsNameValueCollection.Clear();
			ActiveView = HtmlEditor.ActiveView;
			CurrentWidth = Convert.ToInt32(HtmlEditor.Width.Value);
			CurrentHeight = Convert.ToInt32(HtmlEditor.Height.Value);
			IsPercentWidth = HtmlEditor.Width.Type == UnitType.Percentage;
			SaveUserPalettes();
			Ribbon = ribbonState;
		}
		private void SaveUserPalettes() {
			foreach(HtmlEditorToolbar toolbar in HtmlEditor.Toolbars)
				foreach(HtmlEditorToolbarItem item in toolbar.Items.GetVisibleItems()) {
					ToolbarColorButtonBase colorButton = item as ToolbarColorButtonBase;
					if(colorButton != null) {
						if(item.CommandName == ToolbarFontColorButton.DefaultCommandName)
							ForeColorPalette = colorButton.ColorNestedControlProperties.GetSerializedCustomColorTableItems();
						if(item.CommandName == ToolbarBackColorButton.DefaultCommandName)
							BackColorPalette = colorButton.ColorNestedControlProperties.GetSerializedCustomColorTableItems();
					}
				}
		}
	}
}

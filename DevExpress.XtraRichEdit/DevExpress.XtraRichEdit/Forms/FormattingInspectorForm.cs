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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using System.Reflection;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class FormattingInspectorForm : XtraForm {
		RichEditControl control;
		public FormattingInspectorForm() {
			InitializeComponent();
		}
		#region Properties
		protected internal PropertyInfo[] CharacterPropertyInfo { get { return typeof(CharacterProperties).GetProperties(); } }
		protected internal PropertyInfo[] ParagraphPropertyInfo { get { return typeof(ParagraphProperties).GetProperties(); } }
		#endregion
		delegate void AddPropertiesDelegate<T>(string name, T properties);
		public RichEditControl Control { get { return control; } set { control = value; } }
		void FillProperties() {
			Selection selection = Control.DocumentModel.Selection;
			DocumentModelPosition startPos = selection.Interval.NormalizedStart;
			TextRunBase range = selection.PieceTable.Runs[startPos.RunIndex];
			Paragraph paragraph = selection.PieceTable.Paragraphs[startPos.ParagraphIndex];
			listBoxControl1.Items.Clear();
			listBoxControl1.Items.Add("RangePropeprties : ");
			FillPropertiesCore(CharacterPropertyInfo, range.CharacterProperties, AddProperties);
			listBoxControl1.Items.Add("CharacterStylePropeprties : ");
			FillCharacterStyleProperties(range.CharacterStyle);
			listBoxControl1.Items.Add("ParagraphPropeprties : ");
			FillPropertiesCore(ParagraphPropertyInfo, paragraph.ParagraphProperties, AddProperties);
			listBoxControl1.Items.Add("ParagraphStylePropeprties : ");
			FillParagraphSyleProperties(paragraph.ParagraphStyle);
		}
		private void FillPropertiesCore<T>(PropertyInfo[] propertyInfo, T properties, AddPropertiesDelegate<T> addPropertiesDelegate) {
			string usePrefix = "Use";
			foreach (PropertyInfo property in propertyInfo) {
				if (property.Name.StartsWith(usePrefix) && property.Name != "UseVal")
					addPropertiesDelegate(property.Name.Substring(usePrefix.Length), properties);
			}
		}
		void FillCharacterStyleProperties(CharacterStyle characterStyle) {
			listBoxControl1.Items.Add("\tStyleName: " + characterStyle.StyleName);
			FillPropertiesCore(CharacterPropertyInfo, characterStyle.CharacterProperties, AddProperties);
			if (characterStyle.Parent != null)
				FillCharacterStyleProperties(characterStyle.Parent);
		}
		void FillParagraphSyleProperties(ParagraphStyle paragraphStyle) {
			listBoxControl1.Items.Add(String.Format("\tStyleName: {0}", paragraphStyle.StyleName));
			FillPropertiesCore(ParagraphPropertyInfo, paragraphStyle.ParagraphProperties, AddProperties);
			FillPropertiesCore(CharacterPropertyInfo, paragraphStyle.CharacterProperties, AddProperties);
			if (paragraphStyle.Parent != null)
				FillParagraphSyleProperties(paragraphStyle.Parent);
		}
		void AddProperties(string propertyName, CharacterProperties characterProperties) {
			bool useProperty = (bool)characterProperties.GetType().GetProperty("Use" + propertyName).GetValue(characterProperties, null);
			if (useProperty) {
				object propertyValue = characterProperties.GetType().GetProperty(propertyName).GetValue(characterProperties, null);
				listBoxControl1.Items.Add(String.Format("\t\t{0} : {1}", propertyName, propertyValue));
			}
		}
		void AddProperties(string propertyName, ParagraphProperties paragraphProperties) {
			bool useProperty = (bool)paragraphProperties.GetType().GetProperty("Use" + propertyName).GetValue(paragraphProperties, null);
			if (useProperty) {
				object propertyValue = paragraphProperties.GetType().GetProperty(propertyName).GetValue(paragraphProperties, null);
				listBoxControl1.Items.Add(String.Format("\t\t{0} : {1}", propertyName, propertyValue));
			}
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Application.Idle += new EventHandler(Application_Idle);
		}
		void Application_Idle(object sender, EventArgs e) {
			FillProperties();
		}
		protected override void OnClosing(CancelEventArgs e) {
			Application.Idle -= new EventHandler(Application_Idle);
			base.OnClosing(e);
		}
	}
}

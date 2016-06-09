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
using System.Globalization;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.Export.Xl {
	#region XlRichTextRun
	public class XlRichTextRun {
		string text;
		public XlRichTextRun(string text, XlFont font) {
			Text = text;
			Font = font;
		}
		public string Text {
			get { return text; }
			set {
				Guard.ArgumentIsNotNullOrEmpty(value, "Text");
				text = value;
			}
		}
		public XlFont Font { get; set; }
		internal int FontIndex { get; set; }
		public override bool Equals(object obj) {
			XlRichTextRun other = obj as XlRichTextRun;
			if(other == null)
				return false;
			if(Text != other.Text)
				return false;
			if(Font != null)
				return Font.Equals(other.Font);
			return other.Font == null;
		}
		public override int GetHashCode() {
			int result = Text.GetHashCode();
			if(Font != null)
				result = result ^ Font.GetHashCode();
			return result;
		}
	}
	#endregion
	#region IXlString
	public interface IXlString {
		string Text { get; }
		bool IsPlainText { get; }
	}
	#endregion
	#region XlRichTextString
	public class XlRichTextString : IXlString {
		readonly List<XlRichTextRun> runs = new List<XlRichTextRun>();
		public IList<XlRichTextRun> Runs { get { return runs; } }
		public string Text {
			get {
				StringBuilder sb = new StringBuilder();
				foreach(XlRichTextRun run in runs)
					sb.Append(run.Text);
				return sb.ToString();
			}
			set {
				runs.Clear();
				if(!string.IsNullOrEmpty(value))
					runs.Add(new XlRichTextRun(value, null));
			}
		}
		public bool IsPlainText {
			get {
				if(runs.Count == 0)
					return true;
				return (runs.Count == 1) && (runs[0].Font == null);
			}
		}
		public override bool Equals(object obj) {
			XlRichTextString other = obj as XlRichTextString;
			if(other == null)
				return false;
			int count = Runs.Count;
			if(count != other.Runs.Count)
				return false;
			for(int i = 0; i < count; i++) {
				if(!Runs[i].Equals(other.Runs[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			int count = Runs.Count;
			int result = count;
			for(int i = 0; i < count; i++)
				result = result ^ Runs[i].GetHashCode();
			return result;
		}
	}
	#endregion
}

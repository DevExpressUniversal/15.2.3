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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class TitleElement : UserControl {
		string title;
		string prefix;
		Color titleColor;
		Color prefixColor;
		public string Title {
			get { return title; }
			set {
				title = value.ToUpper();
				UpdateText();
			}
		}
		public string Prefix {
			get { return prefix; }
			set {
				prefix = value.ToUpper();
				UpdateText();
			}
		}
		public Color TitleColor {
			get { return titleColor; }
			set {
				titleColor = value;
				UpdateText();
			}
		}
		public Color PrefixColor {
			get { return prefixColor; }
			set {
				prefixColor = value;
				UpdateText();
			}
		}
		public string FullText {
			get { return titleLabel.Text; }
		}
		public TitleElement() {
			InitializeComponent();
		}
		void UpdateText() {
			string newPrefix = prefix;
			if (!prefixColor.IsEmpty)
				newPrefix = string.Format(@"<color={0},{1},{2}>{3}</color>", prefixColor.R, prefixColor.G, prefixColor.B, prefix);
			string newTitle = title;
			if (!titleColor.IsEmpty)
				newTitle = string.Format(@"<color={0},{1},{2}>{3}</color>", titleColor.R, titleColor.G, titleColor.B, title);
			titleLabel.Text = newPrefix + newTitle;
		}
		#region ShouldSerialize
		bool ShouldSerializeTitleColor() {
			return !titleColor.IsEmpty;
		}
		bool ShouldSerializePrefixColor() {
			return !prefixColor.IsEmpty;
		}
		bool ShouldSerializePrefix() {
			return !string.IsNullOrEmpty(prefix);
		}
		bool ShouldSerializeTitle() {
			return !string.IsNullOrEmpty(title);
		}
		#endregion
	}
}

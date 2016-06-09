#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.API.Native;
using System.Text.RegularExpressions;
using DevExpress.Utils.Frames;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public partial class XafInfoPanel : DevExpress.XtraEditors.XtraUserControl {
		private static Regex urlRegex = new Regex(@"((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|ftp[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?½╗ôöæÆ]))", RegexOptions.IgnoreCase);
		private Image infoImage;
		private bool useHtmlString = false;
		private void CreateHyperlinks() {
			Document doc = richEditControl1.Document;
			doc.BeginUpdate();
			DocumentRange[] urls = richEditControl1.Document.FindAll(urlRegex);
			for(int i = urls.Length - 1; i >= 0; i--) {
				if(RangeHasHyperlink(urls[i]))
					continue;
				Hyperlink hyperlink = doc.Hyperlinks.Create(urls[i]);
				hyperlink.NavigateUri = doc.GetText(hyperlink.Range);
			}
			doc.EndUpdate();
		}
		private bool RangeHasHyperlink(DocumentRange documentRange) {
			foreach(Hyperlink h in richEditControl1.Document.Hyperlinks) {
				if(documentRange.Contains(h.Range.Start))
					return true;
			}
			return false;
		}
		public bool UseHtmlString {
			get { return useHtmlString; }
			set { useHtmlString = value; }
		}
		public override string Text {
			set {
				richEditControl1.BeginUpdate();
				richEditControl1.ResetText();
				Table table = richEditControl1.Document.Tables.Create(richEditControl1.Document.Range.Start, 1, 2);
				table.Borders.Bottom.LineStyle = TableBorderLineStyle.None;
				table.Borders.Left.LineStyle = TableBorderLineStyle.None;
				table.Borders.Right.LineStyle = TableBorderLineStyle.None;
				table.Borders.Top.LineStyle = TableBorderLineStyle.None;
				table.Borders.InsideVerticalBorder.LineStyle = TableBorderLineStyle.None;
				table.Cell(0, 0).RightPadding = 3;
				table.Cell(0, 0).LeftPadding = 0;
				table.Cell(0, 0).BottomPadding = 0;
				table.Cell(0, 1).LeftPadding = 0;
				table.Cell(0, 1).BottomPadding = 0;
				table.BottomPadding = 1;
				table.PreferredWidthType = WidthType.Auto;
				Image image = InfoImage;
				if(image != null) {
					Image localImage = image.Clone() as Image;
					richEditControl1.Document.Images.Insert(table[0, 0].Range.Start, localImage);
				}
				if(useHtmlString) {
					richEditControl1.Document.InsertHtmlText(table[0, 1].Range.Start, value);
				}
				else {
					richEditControl1.Document.InsertText(table[0, 1].Range.Start, value);
					CreateHyperlinks();
				}
				var cp = richEditControl1.Document.BeginUpdateCharacters(richEditControl1.Document.CreateRange(richEditControl1.Document.Range.End, 1));
				cp.FontSize = 0.5f;
				richEditControl1.Document.EndUpdateCharacters(cp);
				richEditControl1.Document.CaretPosition = richEditControl1.Document.Range.Start;
				richEditControl1.ScrollToCaret();
				richEditControl1.EndUpdate();
				this.Height = richEditControl1.Height;
			}
			get {
				return richEditControl1.HtmlText;
			}
		}
		public override Color BackColor {
			get {
				return richEditControl1.Views.SimpleView.BackColor;
			}
			set {
				richEditControl1.Views.SimpleView.BackColor = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image InfoImage {
			get {
				if(infoImage == null) {
					infoImage = ImageLoader.Instance.GetImageInfo("Action_AboutInfo_32x32").Image;
				}
				return infoImage;
			}
			set { infoImage = value; }
		}
		public XafInfoPanel() {
			InitializeComponent();
		}
	}
}

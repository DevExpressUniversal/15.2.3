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
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Services {
	public interface IToolTipService {
		ToolTipControlInfo CalculateToolTipInfo(Point point);
	}
}
namespace DevExpress.Services.Implementation {
	using DevExpress.XtraRichEdit.Services;
	using DevExpress.XtraRichEdit;
	using DevExpress.XtraRichEdit.Layout;
	using DevExpress.XtraRichEdit.Model;
	using DevExpress.XtraRichEdit.Mouse;
	using System.Windows.Forms;
	using DevExpress.XtraRichEdit.Localization;
	using DevExpress.XtraRichEdit.Export.PlainText;
	using DevExpress.XtraRichEdit.Export;
	using DevExpress.XtraRichEdit.Internal;
	public class ToolTipService : IToolTipService {
		readonly RichEditControl control;
		public ToolTipService(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public RichEditControl Control { get { return control; } }
		DocumentModel DocumentModel { get { return Control.DocumentModel; } }
		public ToolTipControlInfo CalculateToolTipInfo(Point point) {
			RichEditMouseHandler mouseHandler = Control.InnerControl.MouseHandler as RichEditMouseHandler;
			if (mouseHandler == null)
				return null;
			object activeObject = mouseHandler.ActiveObject;
			if (activeObject == null)
				return null;
			ToolTipControlInfo info = CalculateToolTipInfo(activeObject);
			if (info != null)
				info.ToolTipPosition = CalculateToolTipLocation(Control.GetPhysicalPoint(point));
			return info;
		}
		protected internal virtual ToolTipControlInfo CalculateToolTipInfo(object activeObject) {
			Comment comment = activeObject as Comment;
			if (comment != null) {
				return CalculateToolTipInfo(comment);
			}
			CommentViewInfo commentViewInfo = activeObject as CommentViewInfo;
			if (commentViewInfo != null) {
				return CalculateToolTipInfo(commentViewInfo);
			}
			Field field = activeObject as Field;
			if (field == null || field.IsCodeView || !DocumentModel.ActivePieceTable.IsHyperlinkField(field))
				return null;
			HyperlinkInfo hyperlinkInfo = DocumentModel.ActivePieceTable.HyperlinkInfos[field.Index];
			return CalculateToolTipInfo(hyperlinkInfo);
		}
		protected Point CalculateToolTipLocation(Point physicalPoint) {
			Point screenPosition = Control.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(physicalPoint, Control.DpiX, Control.DpiY);
			return Control.PointToScreen(screenPosition);
		}
		public ToolTipControlInfo CalculateToolTipInfo(Comment comment) { 
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = comment.Content;
			result.ToolTipLocation = ToolTipLocation.RightTop;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip(comment);
			return result;
		}
		SuperToolTip CreateSuperTip(Comment comment) {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipTitleItem header = new ToolTipTitleItem();
			RichEditToolTipHelper helper = new RichEditToolTipHelper();
			header.Text = helper.GetCommentToolTipHeader(comment);
			superTip.Items.Add(header);
			ToolTipItem text = new ToolTipItem();
			text.Text = helper.GetCommentToolTipText(comment);
			superTip.Items.Add(text);
			return superTip;
		}
		public ToolTipControlInfo CalculateToolTipInfo(CommentViewInfo commentViewInfo) {
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = commentViewInfo.Comment.Content;
			result.ToolTipLocation = ToolTipLocation.RightTop;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip(commentViewInfo);
			return result;
		}
		SuperToolTip CreateSuperTip(CommentViewInfo commentViewInfo) {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipTitleItem header = new ToolTipTitleItem();
			RichEditToolTipHelper helper = new RichEditToolTipHelper();
			header.Text = helper.GetCommentToolTipHeader(commentViewInfo);
			superTip.Items.Add(header);
			ToolTipItem text = new ToolTipItem();
			text.Text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ClickToComment);
			superTip.Items.Add(text);
			return superTip;
		}
		public ToolTipControlInfo CalculateToolTipInfo(HyperlinkInfo hyperlinkInfo) {
			ToolTipControlInfo result = new ToolTipControlInfo();
			result.Object = hyperlinkInfo;
			if (!Control.Options.Hyperlinks.ShowToolTip)
				return result;
			result.ToolTipLocation = ToolTipLocation.RightTop;
			result.ToolTipType = ToolTipType.SuperTip;
			result.SuperTip = CreateSuperTip(hyperlinkInfo);
			return result;
		}
		SuperToolTip CreateSuperTip(HyperlinkInfo hyperlinkInfo) {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipItem text = new ToolTipItem();
			text.Text = hyperlinkInfo.GetActualToolTip();
			superTip.Items.Add(text);
			ToolTipTitleItem footer = new ToolTipTitleItem();
			footer.Text = GetHyperlinkToolTipFooter();
			superTip.Items.Add(footer);
			return superTip;
		}
		string GetHyperlinkToolTipFooter() {
			KeysConverter converter = new KeysConverter();
			string keys = (string)converter.ConvertTo(Control.Options.Hyperlinks.ModifierKeys, typeof(String));
			string[] keyList = keys.Split('+');
			if (keyList[keyList.Length - 1] == Keys.None.ToString())
				keys = String.Join("+", keyList, 0, keyList.Length - 1);
			else
				keys = String.Join("+", keyList);
			string footer = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_ClickToFollowHyperlink);
			if (String.IsNullOrEmpty(keys))
				return footer;
			return String.Format("{0} + {1}", keys, footer);
		}
	}
}

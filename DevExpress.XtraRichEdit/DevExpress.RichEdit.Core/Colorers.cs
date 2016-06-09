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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Internal {
	#region CommentColorer
	public class CommentColorer {
		int colorIndex;
		CommentOptions options;
		Dictionary<string, Color> commentColors = new Dictionary<string, Color>();
		public CommentColorer(CommentOptions options) {
			this.options = options;
		}
		Dictionary<string, Color> CommentColors { get { return commentColors; } }
		CommentOptions Options { get { return options; } }
		public void Reset() {
			commentColors.Clear();
		}
		public Color GetColor(Comment comment) {
			if (Options.ShouldSerializeColor())
				return Options.Color;
			string id = comment.Name;
			Color color = DXColor.Red;
			if (CommentColors.ContainsKey(id))
				color = CommentColors[id];
			else {
				color = GetColor();
				CommentColors.Add(id, color);
			}
			return color;
		}
		Color GetColor() {
			if (colorIndex >= CommentOptions.GetDefaultColorsLength())
				colorIndex = 0;
			Color result = CommentOptions.GetColor(colorIndex);
			colorIndex++;
			return result;
		}
	}
	#endregion
	#region RangePermissionColorer
	public class RangePermissionColorer {
		Dictionary<string, Color> rangeColors = new Dictionary<string, Color>();
		Dictionary<string, Color> RangeColors { get { return rangeColors; } }
		public void Reset() {
			rangeColors.Clear();
		}
		public Color GetColor(RangePermission rangePermission) {
			string id = rangePermission.UserName;
			if (!String.IsNullOrEmpty(rangePermission.Group))
				id += String.Format(":{0}", rangePermission.Group);
			Color color = DXColor.Red;
			if (RangeColors.ContainsKey(id))
				color = RangeColors[id];
			else {
				color = RangePermissionOptions.GetColor();
				RangeColors.Add(id, color);
			}
			return color;
		}
	}
	#endregion
}

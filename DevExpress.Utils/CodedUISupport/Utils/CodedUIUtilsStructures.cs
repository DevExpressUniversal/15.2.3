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
using System.Drawing.Drawing2D;
namespace DevExpress.Utils.CodedUISupport {
	[Serializable]
	public struct FormElementInfo {
		public string Name;
		public FormElements ElementType;
	}
	public enum FormElements : int {
		Unknown,
		Caption,
		Button
	}
	[Serializable]
	public class AppearanceObjectSerializable {
		public AppearanceObjectSerializable(AppearanceObject appearence) {
			this.Font = CodedUIUtils.ConvertToString(appearence.Font);
			this.FontHeight = appearence.FontHeight;
			this.ForeColor = CodedUIUtils.ConvertToString(appearence.ForeColor);
			this.BackColor = CodedUIUtils.ConvertToString(appearence.BackColor);
			this.BackColor2 = CodedUIUtils.ConvertToString(appearence.BackColor2);
			this.BorderColor = CodedUIUtils.ConvertToString(appearence.BorderColor);
			this.GradientMode = appearence.GradientMode;
			this.HAlignment = CodedUIUtils.ConvertToString(appearence.HAlignment);
			this.TextOptions = new TextOptionsSerializable(appearence.TextOptions);
		}
		public string Font;
		public int FontHeight;
		public string ForeColor;
		public string BackColor;
		public string BackColor2;
		public string BorderColor;
		public LinearGradientMode GradientMode;
		public string HAlignment;
		public TextOptionsSerializable TextOptions;
	}
	[Serializable]
	public class TextOptionsSerializable {
		public TextOptionsSerializable(TextOptions textOptions) {
			this.HAlignment = CodedUIUtils.ConvertToString(textOptions.HAlignment);
			this.HotkeyPrefix = CodedUIUtils.ConvertToString(textOptions.HotkeyPrefix);
			this.Trimming = CodedUIUtils.ConvertToString(textOptions.Trimming);
			this.VAlignment = CodedUIUtils.ConvertToString(textOptions.VAlignment);
			this.WordWrap = CodedUIUtils.ConvertToString(textOptions.WordWrap);
		}
		public string HAlignment;
		public string HotkeyPrefix;
		public string Trimming;
		public string VAlignment;
		public string WordWrap;
	}
}

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

using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Forms {
	#region HyperlinkFormControllerParameters
	public class HyperlinkFormControllerParameters : FormControllerParameters {
		readonly IHyperlinkViewInfo hyperlinkInfo;
		public HyperlinkFormControllerParameters(ISpreadsheetControl control, IHyperlinkViewInfo hyperlinkInfo)
			: base(control) {
				Guard.ArgumentNotNull(hyperlinkInfo, "hyperlinkInfo");
			this.hyperlinkInfo = hyperlinkInfo;
		}
		public IHyperlinkViewInfo HyperlinkInfo { get { return hyperlinkInfo; } }
	}
	#endregion
	#region HyperlinkFormController
	public class HyperlinkFormController : FormController {
		readonly IHyperlinkViewInfo hyperlink;
		readonly ISpreadsheetControl control;
		string displayText;
		string tooltipText;
		string targetUri;
		bool isExternal;
		public HyperlinkFormController(HyperlinkFormControllerParameters parameters) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.hyperlink = parameters.HyperlinkInfo;
			this.control = parameters.Control;
			InitializeController();
		}
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public string TooltipText { get { return tooltipText; } set { tooltipText = value; } }
		public string TargetUri { get { return targetUri; } set { targetUri = value; } }
		public bool IsExternal { get { return isExternal; } set { isExternal = value; } }
		public ISpreadsheetControl Control { get { return control; } }
		public IHyperlinkViewInfo Hyperlink { get { return hyperlink; } }
		protected internal virtual void InitializeController() {
			DisplayText = Hyperlink.DisplayText;
			TooltipText = Hyperlink.TooltipText;
			TargetUri = Hyperlink.TargetUri;
			IsExternal = Hyperlink.IsExternal;
		}
		public override void ApplyChanges() {
			Control.BeginUpdate();
			try {
				ApplyDisplayText();
				ApplyTooltipText();
				ApplyTargetUri();
				ApplyIsExternal();
			}
			finally {
				Control.EndUpdate();
			}
		}
		void ApplyDisplayText() {
			if(Hyperlink.DisplayText != DisplayText)
				Hyperlink.DisplayText = DisplayText;
		}
		void ApplyTooltipText() {
			if(Hyperlink.TooltipText != TooltipText)
				Hyperlink.TooltipText = TooltipText;
		}
		void ApplyTargetUri() {
			if(Hyperlink.TargetUri != TargetUri)
				Hyperlink.SetTargetUriWithoutHistory(TargetUri);
		}
		void ApplyIsExternal() {
			if(Hyperlink.IsExternal != IsExternal)
				Hyperlink.IsExternal = IsExternal;
		}
	}
	#endregion
}

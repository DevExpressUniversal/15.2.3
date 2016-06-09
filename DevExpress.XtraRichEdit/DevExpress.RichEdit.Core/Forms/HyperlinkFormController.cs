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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using System.Collections.Generic;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	#region HyperlinkFormControllerParameters
	public class HyperlinkFormControllerParameters : FormControllerParameters {
		readonly HyperlinkInfo hyperlinkInfo;
		readonly RunInfo runInfo;
		string initialTextToDisplay;
		string textToDisplay;
		internal HyperlinkFormControllerParameters(IRichEditControl control, HyperlinkInfo hyperlinkInfo, RunInfo runInfo)
			: base(control) {
			Guard.ArgumentNotNull(hyperlinkInfo, "hyperlinkInfo");
			Guard.ArgumentNotNull(runInfo, "runInfo");
			this.hyperlinkInfo = hyperlinkInfo;
			this.runInfo = runInfo;
		}
		internal HyperlinkInfo HyperlinkInfo { get { return hyperlinkInfo; } }
		internal RunInfo RunInfo { get { return runInfo; } }
		internal string TextToDisplay { get { return textToDisplay; } set { textToDisplay = value; } }
		protected internal string InitialTextToDisplay { get { return initialTextToDisplay; } set { initialTextToDisplay = value; } }
		internal TextToDisplaySource TextSource {
			get {
				if (textToDisplay == initialTextToDisplay && initialTextToDisplay != null)
					return TextToDisplaySource.ExistingText;
				else
					return TextToDisplaySource.NewText;
			}
		}
	}
	#endregion
	public enum HyperlinkUriType {
		None,
		Url,
		Anchor
	}
	#region HyperlinkFormController
	public class HyperlinkFormController : FormController {
		#region Fields
		string navigateUri;
		string anchor;
		string toolTip;
		string target;
		HyperlinkUriType uriType;
		readonly HyperlinkFormControllerParameters controllerParameters;
		readonly HyperlinkInfo hyperlinkInfo;
		readonly IRichEditControl control;
		readonly RunInfo runInfo;
		#endregion
		public HyperlinkFormController(HyperlinkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
			this.control = controllerParameters.Control;
			this.hyperlinkInfo = controllerParameters.HyperlinkInfo;
			this.runInfo = controllerParameters.RunInfo;
			InitializeController();
		}
		#region Properties
		protected internal HyperlinkInfo HyperlinkInfo { get { return hyperlinkInfo; } }
		public HyperlinkUriType UriType { get { return uriType; } set { uriType = value; } }
		public string TextToDisplay { get { return controllerParameters.TextToDisplay; } set { controllerParameters.TextToDisplay = value; } }
		public string NavigateUri { get { return navigateUri; } set { navigateUri = value; } }
		public string Anchor { get { return anchor; } set { anchor = value; } }
		public string ToolTip { get { return toolTip; } set { toolTip = value; } }
		public string Target { get { return target; } set { target = value; } }
		protected internal TextToDisplaySource TextSource { get { return controllerParameters.TextSource; } }
		public IRichEditControl Control { get { return control; } }
		#endregion
		protected internal virtual void InitializeController() {
			if (CanUseNavigateUri(HyperlinkInfo.NavigateUri, HyperlinkInfo.Anchor)) {
				NavigateUri = HyperlinkInfo.CreateUrl();
				if(String.IsNullOrEmpty(NavigateUri))
					NavigateUri = "http://";
				UriType = HyperlinkUriType.Url;
			}
			else {
				Anchor = HyperlinkInfo.Anchor;
				UriType = HyperlinkUriType.Anchor;
			}
			ToolTip = HyperlinkInfo.ToolTip;
			Target = HyperlinkInfo.Target;
			if (runInfo.NormalizedEnd > runInfo.NormalizedStart)
				TextToDisplay = CanChangeDisplayText() ? GetText(runInfo.NormalizedStart, runInfo.NormalizedEnd).Trim() : String.Empty;
			controllerParameters.InitialTextToDisplay = TextToDisplay;
		}
		bool CanUseNavigateUri(string navigateUri, string anchor) {
			return !String.IsNullOrEmpty(navigateUri) || (String.IsNullOrEmpty(anchor) || !IsBookmarkExists(anchor));
		}
		bool IsBookmarkExists(string name) {
			List<Bookmark> bookmarks = Control.InnerControl.DocumentModel.GetBookmarks();
			int count = bookmarks.Count;
			for (int i = 0; i < count; i++) {
				if (name.Equals(bookmarks[i].Name))
					return true;
			}
			return false;
		}
		public override void ApplyChanges() {
			if (UriType == HyperlinkUriType.Anchor)
				SetAnchorCore();
			else
				SetNavigateUriCore();
			HyperlinkInfo.ToolTip = ToolTip;
			HyperlinkInfo.Target = Target;
			HyperlinkInfo.Visited = false;
			ApplyTextToDisplay();
		}
		public virtual void ApplyChanges(bool useNavigateUri) {
			if (useNavigateUri)
				SetNavigateUriCore();
			else
				SetAnchorCore();
			HyperlinkInfo.ToolTip = ToolTip;
			HyperlinkInfo.Target = Target;
			HyperlinkInfo.Visited = false;
			ApplyTextToDisplay();
		}
		public virtual bool CanChangeDisplayText() {
			return runInfo.Start.ParagraphIndex == runInfo.End.ParagraphIndex && !control.InnerControl.DocumentModel.Selection.PieceTable.HasInlinePicture(runInfo);
		}
		string GetText(DocumentModelPosition start, DocumentModelPosition end) {
			IVisibleTextFilter filter = start.PieceTable.VisibleTextFilter;
			return start.PieceTable.GetFilteredPlainText(start, end, filter.IsRunVisible);
		}
		protected internal virtual void SetNavigateUriCore() {
			string url = !String.IsNullOrEmpty(NavigateUri) ? NavigateUri.Trim() : String.Empty;
			int anchorStartIndex = url.LastIndexOf('#');
			string anchor = anchorStartIndex > 0 ? url.Substring(anchorStartIndex + 1) : String.Empty;
			HyperlinkInfo.Anchor = anchor;
			string navigateUri = String.IsNullOrEmpty(anchor) ? url : url.Remove(anchorStartIndex);
			HyperlinkInfo.NavigateUri = HyperlinkUriHelper.EnsureUriIsValid(navigateUri);
		}
		protected internal virtual void SetAnchorCore() {
			HyperlinkInfo.NavigateUri = String.Empty;
			HyperlinkInfo.Anchor = Anchor;
		}
		protected internal virtual void ApplyTextToDisplay() {
			if (!String.IsNullOrEmpty(TextToDisplay) || TextSource == TextToDisplaySource.ExistingText)
				return;
			TextToDisplay = !String.IsNullOrEmpty(NavigateUri) ? NavigateUri : Anchor;
		}
	}
	#endregion
}

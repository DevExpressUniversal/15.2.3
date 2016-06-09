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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Utils.Localization;
using System.Windows.Markup;
using System.Resources;
using System.Globalization;
namespace DevExpress.Xpf.DocumentViewer {
	public enum DocumentViewerStringId {
		#region Messages
		#endregion
		#region Captions
		BarCaption,
		CommandOpenCaption,
		CommandCloseCaption,
		CommandZoomInCaption,
		CommandZoomOutCaption,
		CommandZoomCaption,
		CommandPreviousPageCaption,
		CommandNextPageCaption,
		CommandZoom10Caption,
		CommandZoom25Caption,
		CommandZoom50Caption,
		CommandZoom75Caption,
		CommandZoom100Caption,
		CommandZoom125Caption,
		CommandZoom150Caption,
		CommandZoom200Caption,
		CommandZoom400Caption,
		CommandZoom500Caption,
		CommandSetActualSizeZoomModeCaption,
		CommandSetPageLevelZoomModeCaption,
		CommandSetFitWidthZoomModeCaption,
		CommandSetFitVisibleZoomModeCaption,
		CommandClockwiseRotateCaption,
		CommandCounterClockwiseRotateCaption,
		CommandPreviousViewCaption,
		CommandNextViewCaption,
		#endregion
		#region Hints
		CommandOpenDescription,
		CommandCloseDescription,
		CommandZoomInDescription,
		CommandZoomOutDescription,
		CommandZoomDescription,
		CommandPreviousPageDescription,
		CommandNextPageDescription,
		#endregion
		#region Ribbon
		FileRibbonGroupCaption,
		ZoomRibbonGroupCaption,
		NavigationRibbonGroupCaption,
		RotateRibbonGroupCaption,
		#endregion
		#region Search
		MessageSettingsCaption,
		MessageShowFindTextCaption,
		MessageShowFindTextHintCaption,
		FindControlCaseSensitive,
		FindControlWholeWordsOnly,
		FindControlPrevious,
		FindControlNext,
		FindControlClose,
		FindControlSearchCaption,
		#endregion
		#region DocumentMap
		DocumentMapExpandCurrentCaption,
		DocumentMapExpandTopLevelCaption,
		DocumentMapCollapseTopLevelCaption,
		DocumentMapGoToCaption,
		#endregion
		OpenFileDialogTitle,
		DocumentViewerInfiniteHeightExceptionMessage,
	}
	public class DocumentViewerLocalizer : DXLocalizer<DocumentViewerStringId> {
		static DocumentViewerLocalizer() {
			if(GetActiveLocalizerProvider() == null)
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DocumentViewerStringId>(new DocumentViewerControlResXLocalizer()));
		}
		public static new XtraLocalizer<DocumentViewerStringId> Active {
			get { return XtraLocalizer<DocumentViewerStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<DocumentViewerStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DocumentViewerStringId>(value));
					RaiseActiveChanged();
				}
				else {
					XtraLocalizer<DocumentViewerStringId>.Active = value;
				}
			}
		}
		public static XtraLocalizer<DocumentViewerStringId> CreateDefaultLocalizer() {
			return new DocumentViewerLocalizer();
		}
		public static string GetString(DocumentViewerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<DocumentViewerStringId> CreateResXLocalizer() {
			return new DocumentViewerControlResXLocalizer();
		}
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		protected override void PopulateStringTable() {
			AddString(DocumentViewerStringId.BarCaption, "Document Viewer");
			AddString(DocumentViewerStringId.CommandOpenCaption, "Open");
			AddString(DocumentViewerStringId.CommandCloseCaption, "Close");
			AddString(DocumentViewerStringId.CommandZoomInCaption, "Zoom In");
			AddString(DocumentViewerStringId.CommandZoomOutCaption, "Zoom Out");
			AddString(DocumentViewerStringId.CommandZoomCaption, "Zoom");
			AddString(DocumentViewerStringId.CommandPreviousPageCaption, "Previous");
			AddString(DocumentViewerStringId.CommandNextPageCaption, "Next");
			AddString(DocumentViewerStringId.CommandZoom10Caption, "10%");
			AddString(DocumentViewerStringId.CommandZoom25Caption, "25%");
			AddString(DocumentViewerStringId.CommandZoom50Caption, "50%");
			AddString(DocumentViewerStringId.CommandZoom75Caption, "75%");
			AddString(DocumentViewerStringId.CommandZoom100Caption, "100%");
			AddString(DocumentViewerStringId.CommandZoom125Caption, "125%");
			AddString(DocumentViewerStringId.CommandZoom150Caption, "150%");
			AddString(DocumentViewerStringId.CommandZoom200Caption, "200%");
			AddString(DocumentViewerStringId.CommandZoom400Caption, "400%");
			AddString(DocumentViewerStringId.CommandZoom500Caption, "500%");
			AddString(DocumentViewerStringId.CommandSetActualSizeZoomModeCaption, "Actual Size");
			AddString(DocumentViewerStringId.CommandSetPageLevelZoomModeCaption, "Zoom to Page Level");
			AddString(DocumentViewerStringId.CommandSetFitWidthZoomModeCaption, "Fit Width");
			AddString(DocumentViewerStringId.CommandSetFitVisibleZoomModeCaption, "Fit Visible");
			AddString(DocumentViewerStringId.CommandClockwiseRotateCaption, "Clockwise rotate");
			AddString(DocumentViewerStringId.CommandCounterClockwiseRotateCaption, "Counterclockwise rotate");
			AddString(DocumentViewerStringId.CommandPreviousViewCaption, "Previous view");
			AddString(DocumentViewerStringId.CommandNextViewCaption, "Next view");
			AddString(DocumentViewerStringId.CommandOpenDescription, "Open a document.");
			AddString(DocumentViewerStringId.CommandCloseDescription, "Close a document.");
			AddString(DocumentViewerStringId.CommandZoomInDescription, "Zoom in to get a close-up view of the document.");
			AddString(DocumentViewerStringId.CommandZoomOutDescription, "Zoom out to see more of the page at a reduces size.");
			AddString(DocumentViewerStringId.CommandZoomDescription, "Change the zoom level of the document.");
			AddString(DocumentViewerStringId.CommandPreviousPageDescription, "Show previous page.");
			AddString(DocumentViewerStringId.CommandNextPageDescription, "Show next page.");
			AddString(DocumentViewerStringId.NavigationRibbonGroupCaption, "Navigation");
			AddString(DocumentViewerStringId.ZoomRibbonGroupCaption, "Zoom");
			AddString(DocumentViewerStringId.FileRibbonGroupCaption, "File");
			AddString(DocumentViewerStringId.RotateRibbonGroupCaption, "Rotate");
			AddString(DocumentViewerStringId.MessageSettingsCaption, "Settings");
			AddString(DocumentViewerStringId.MessageShowFindTextCaption, "Find");
			AddString(DocumentViewerStringId.MessageShowFindTextHintCaption, "Find Text");
			AddString(DocumentViewerStringId.FindControlCaseSensitive, "Case Sensitive");
			AddString(DocumentViewerStringId.FindControlWholeWordsOnly, "Whole Words Only");
			AddString(DocumentViewerStringId.FindControlPrevious, "Previous");
			AddString(DocumentViewerStringId.FindControlNext, "Next");
			AddString(DocumentViewerStringId.FindControlClose, "Close");
			AddString(DocumentViewerStringId.FindControlSearchCaption, "Search: ");
			AddString(DocumentViewerStringId.OpenFileDialogTitle, "Open");
			AddString(DocumentViewerStringId.DocumentMapExpandCurrentCaption, "Expand current bookmark");
			AddString(DocumentViewerStringId.DocumentMapExpandTopLevelCaption, "Expand top level bookmark");
			AddString(DocumentViewerStringId.DocumentMapCollapseTopLevelCaption, "Collapse top level bookmark");
			AddString(DocumentViewerStringId.DocumentMapGoToCaption, "Go to bookmark");
			AddString(DocumentViewerStringId.DocumentViewerInfiniteHeightExceptionMessage, "The document viewer should have a finite height.");
		}
	}
	public class DocumentViewerControlResXLocalizer : DXResXLocalizer<DocumentViewerStringId> {
		ResourceManager resourceManager;
		public DocumentViewerControlResXLocalizer()
			: base(new DocumentViewerLocalizer()) {
				resourceManager = new ResourceManager("DevExpress.Xpf.DocumentViewer.LocalizationRes", typeof(DocumentViewerControlResXLocalizer).Assembly);
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return resourceManager ?? (resourceManager = new ResourceManager("DevExpress.Xpf.DocumentViewer.LocalizationRes", typeof(DocumentViewerControlResXLocalizer).Assembly));
		}
		public override string GetLocalizedString(DocumentViewerStringId id) {
			return resourceManager.GetString("DocumentViewerStringId." + id) ?? string.Empty;
		}
	}
	public class DocumentViewerControlLocalizedStringExtension : MarkupExtension {
		readonly DocumentViewerStringId stringID;
		public DocumentViewerControlLocalizedStringExtension(DocumentViewerStringId stringID) {
			this.stringID = stringID;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return DocumentViewerLocalizer.GetString(stringID);
		}
	}
}

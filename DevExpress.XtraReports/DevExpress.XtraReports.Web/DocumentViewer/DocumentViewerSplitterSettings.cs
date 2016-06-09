#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.ComponentModel;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerSplitterSettings : PropertiesBase, IPropertiesOwner {
		const string
			ParametersPanelCollapsedName = "ParametersPanelCollapsed",
			DocumentMapCollapsedName = "DocumentMapCollapsed",
			ToolbarCollapsedName = "ToolbarCollapsed",
			SidePaneVisibleName = "SidePaneVisible",
			SidePanePositionName = "SidePanePosition",
			DocumentMapAutoHeightName = "DocumentMapAutoHeight";
		const bool
			DefaultCollapsed = false,
			DocumentMapAutoHeightDefault = false;
		const DocumentViewerSidePanePosition DefaultSidePanePosition = DocumentViewerSidePanePosition.Right;
		internal const bool DefaultVisible = true;
		public DocumentViewerSplitterSettings(IPropertiesOwner owner)
			: base(owner) {
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterSettingsParametersPanelCollapsed")]
#endif
		[DefaultValue(DefaultCollapsed)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public bool ParametersPanelCollapsed {
			get { return GetBoolProperty(ParametersPanelCollapsedName, DefaultCollapsed); }
			set { SetBoolProperty(ParametersPanelCollapsedName, DefaultCollapsed, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterSettingsDocumentMapCollapsed")]
#endif
		[DefaultValue(DefaultCollapsed)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public bool DocumentMapCollapsed {
			get { return GetBoolProperty(DocumentMapCollapsedName, DefaultCollapsed); }
			set { SetBoolProperty(DocumentMapCollapsedName, DefaultCollapsed, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterSettingsSidePaneVisible")]
#endif
		[DefaultValue(DefaultVisible)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public bool SidePaneVisible {
			get { return GetBoolProperty(SidePaneVisibleName, DefaultVisible); }
			set { SetBoolProperty(SidePaneVisibleName, DefaultVisible, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterSettingsSidePanePosition")]
#endif
		[DefaultValue(DefaultSidePanePosition)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public DocumentViewerSidePanePosition SidePanePosition {
			get { return (DocumentViewerSidePanePosition)GetEnumProperty(SidePanePositionName, DefaultSidePanePosition); }
			set { SetEnumProperty(SidePanePositionName, DefaultSidePanePosition, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerSplitterSettingsDocumentMapAutoHeight")]
#endif
		[DefaultValue(false)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public bool DocumentMapAutoHeight {
			get { return GetBoolProperty(DocumentMapAutoHeightName, DocumentMapAutoHeightDefault); }
			set { SetBoolProperty(DocumentMapAutoHeightName, DocumentMapAutoHeightDefault, value); }
		}
		#region IPropertiesOwner Members
		public void Changed(PropertiesBase properties) {
			base.Changed();
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as DocumentViewerSplitterSettings;
			if(src == null)
				return;
			ParametersPanelCollapsed = src.ParametersPanelCollapsed;
			DocumentMapCollapsed = src.DocumentMapCollapsed;
			SidePaneVisible = src.SidePaneVisible;
			SidePanePosition = src.SidePanePosition;
			DocumentMapAutoHeight = src.DocumentMapAutoHeight;
		}
	}
}

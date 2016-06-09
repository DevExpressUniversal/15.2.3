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

using System.ComponentModel;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerDocumentMapSettings : PropertiesBase, IPropertiesOwner {
		const string
			ShowTreeLinesName = "ShowTreeLines",
			AllowSelectNodeName = "AllowSelectNode",
			EnableAnimationName = "EnableAnimation",
			EnableHotTrackName = "EnableHotTrack";
		const bool DefaultEnableHotTrack = true;
		public DocumentViewerDocumentMapSettings(IPropertiesOwner owner) : base(owner) { }
		[Category("Appearance")]
		[DefaultValue(ReportDocumentMap.DefaultShowTreeLines)]
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		public bool ShowTreeLines {
			get { return GetBoolProperty(ShowTreeLinesName, ReportDocumentMap.DefaultShowTreeLines); }
			set { SetBoolProperty(ShowTreeLinesName, ReportDocumentMap.DefaultShowTreeLines, value); }
		}
		[Category("Appearance")]
		[DefaultValue(ReportDocumentMap.DefaultAllowSelectNode)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public bool AllowSelectNode {
			get { return GetBoolProperty(AllowSelectNodeName, ReportDocumentMap.DefaultAllowSelectNode); }
			set { SetBoolProperty(AllowSelectNodeName, ReportDocumentMap.DefaultAllowSelectNode, value); }
		}
		[Category("Behavior")]
		[DefaultValue(ReportDocumentMap.DefaultEnableAnimation)]
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		public bool EnableAnimation {
			get { return GetBoolProperty(EnableAnimationName, ReportDocumentMap.DefaultEnableAnimation); }
			set { SetBoolProperty(EnableAnimationName, ReportDocumentMap.DefaultEnableAnimation, value); }
		}
		[Category("Behavior")]
		[DefaultValue(DefaultEnableHotTrack)]
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		public bool EnableHotTrack {
			get { return GetBoolProperty(EnableHotTrackName, DefaultEnableHotTrack); }
			set { SetBoolProperty(EnableHotTrackName, DefaultEnableHotTrack, value); }
		}
		#region IPropertiesOwner Members
		public void Changed(PropertiesBase properties) {
			base.Changed();
		}
		#endregion
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as DocumentViewerDocumentMapSettings;
			if(src == null)
				return;
			AllowSelectNode = src.AllowSelectNode;
			EnableAnimation = src.EnableAnimation;
			EnableHotTrack = src.EnableHotTrack;
			ShowTreeLines = src.ShowTreeLines;
		}
	}
}

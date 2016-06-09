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
using System.Windows.Media;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public class XRTextModelBase<TXRControl, TDiagramItem> : XRControlModelBase<TXRControl, TDiagramItem>
		where TXRControl : XRControl
		where TDiagramItem : XRDiagramTextItem, new() {
		protected internal XRTextModelBase(XRControlModelFactory.ISource<TXRControl> source, ImageSource icon)
			: base(source, icon) {
			Text = CreateTextPropertyModel();
		}
		protected virtual XRPropertyModel<string> CreateTextPropertyModel() {
			return CreateXRPropertyModel(() => XRObject.Text, () => XRObject.Text, text => Tracker.Set(XRObject, x => x.Text, text));
		}
		public XRPropertyModel<string> Text { get; private set; }
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			BindDiagramItem(XRDiagramTextItem.TextAlignmentProperty, () => FontProperties.TextHorizontalAlignment, FontProperties);
			BindDiagramItem(XRDiagramTextItem.TextDecorationsProperty, () => FontProperties.TextDecorations, FontProperties);
			BindDiagramItem(XRDiagramTextItem.VerticalContentAlignmentProperty, () => FontProperties.VerticalContentAlignment, FontProperties);
			BindDiagramItem(XRDiagramTextItem.TextProperty, () => Text.Value, Text);
		}
	}
	public class XRContainerTextModelBase<TXRControl, TItemModel, TDiagramItem> : XRContainerModelBase<TXRControl, TItemModel, TDiagramItem>
		where TXRControl : XRControl
		where TItemModel : XRControlModelBase
		where TDiagramItem : XRDiagramTextContainer, new() {
		protected internal XRContainerTextModelBase(XRControlModelFactory.ISource<TXRControl> source, ImageSource icon)
			: base(source, icon) {
			Text = CreateTextPropertyModel();
		}
		protected virtual XRPropertyModel<string> CreateTextPropertyModel() {
			return CreateXRPropertyModel(() => XRObject.Text, () => XRObject.Text, text => Tracker.Set(XRObject, x => x.Text, text));
		}
		public XRPropertyModel<string> Text { get; private set; }
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			BindDiagramItem(XRDiagramTextItem.TextAlignmentProperty, () => FontProperties.TextHorizontalAlignment, FontProperties);
			BindDiagramItem(XRDiagramTextItem.TextDecorationsProperty, () => FontProperties.TextDecorations, FontProperties);
			BindDiagramItem(XRDiagramTextItem.VerticalContentAlignmentProperty, () => FontProperties.VerticalContentAlignment, FontProperties);
			BindDiagramItem(XRDiagramTextItem.TextProperty, () => Text.Value, Text);
		}
	}
}

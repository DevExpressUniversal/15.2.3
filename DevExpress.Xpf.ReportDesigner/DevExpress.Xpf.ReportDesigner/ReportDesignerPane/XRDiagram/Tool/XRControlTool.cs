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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Data.Helpers;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.Toolbox;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public abstract class XRControlToolBase : XRToolBase {
		public XRControlToolBase(Type type, XRDiagramControl diagram)
			: base(diagram) {
			XRObjectType = type;
		}
		public Type XRObjectType { get; private set; }
	}
	public class XRControlTool<T> : XRControlToolBase where T : XRControl, new() {
		static Func<XRDiagramControl, Size> WrapSize(Size size) {
			return _ => size;
		}
		readonly Func<XRDiagramControl, Size> getSizeFunc;
		public XRControlTool(XRDiagramControl diagram, Size size) : this(diagram, WrapSize(size)) {
		}
		public XRControlTool(XRDiagramControl diagram, Func<XRDiagramControl, Size> getSizeFunc = null) : base(typeof(T), diagram) {
			this.getSizeFunc = getSizeFunc;
		}
		public override Size DefaultItemSize { get { return getSizeFunc == null ? XRControlModelBase.DefaultSize : getSizeFunc(((IXRTool)this).Diagram); } }
		public override string ToolName {
			get {
				return typeof(T).GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>()
					.Select(x => SplitStringHelper.SplitPascalCaseString(x.DisplayName)).FirstOrDefault()
					?? typeof(T).Name;
			}
		}
		public override string ToolId { get { return "XRControlTool"; } }
		protected override DiagramItem CreateItemOverride(XRDiagramControl diagram) {
			var xrControl = new T();
			return diagram.ItemFactory(xrControl);
		}
	}
}

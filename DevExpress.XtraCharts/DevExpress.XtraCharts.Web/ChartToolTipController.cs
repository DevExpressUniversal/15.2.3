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
using System.ComponentModel.Design.Serialization;
using System.Web.UI.WebControls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Web {
	public enum ChartImagePosition {
		Left,
		Top,
		Right,
		Bottom
	}
	public enum ToolTipOpenMode {
		OnHover,
		OnClick
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ChartToolTipController : ChartElement {
		bool showImage = true;
		bool showText = true;
		ToolTipOpenMode openMode = ToolTipOpenMode.OnHover;
		ChartImagePosition imagePosition = ChartImagePosition.Left;
		internal bool ShouldSerializeProperties { get { return ShouldSerialize(); } }
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartToolTipControllerShowImage"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowImage {
			get { return showImage; }
			set {
				if (value != showImage) {
					SendNotification(new ElementWillChangeNotification(this));
					showImage = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartToolTipControllerShowText"),
#endif
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ShowText {
			get { return showText; }
			set {
				if (value != showText) {
					SendNotification(new ElementWillChangeNotification(this));
					showText = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartToolTipControllerImagePosition"),
#endif
		XtraSerializableProperty
		]
		public ChartImagePosition ImagePosition {
			get { return imagePosition; }
			set {
				if (value != imagePosition) {
					SendNotification(new ElementWillChangeNotification(this));
					imagePosition = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartToolTipControllerOpenMode"),
#endif
		XtraSerializableProperty
		]
		public ToolTipOpenMode OpenMode {
			get { return openMode; }
			set {
				if (value != openMode) {
					SendNotification(new ElementWillChangeNotification(this));
					openMode = value;
					RaiseControlChanged();
				}
			}
		}
		public ChartToolTipController() : base() {
		}
		public ChartToolTipController(ChartElement owner) : base(owner) {
		}
		#region ShouldSerialize
		bool ShouldSerializeImagePosition() {
			return imagePosition != ChartImagePosition.Left;
		}
		bool ShouldSerializeShowImage() {
			return !showImage;
		}
		bool ShouldSerializeShowText() {
			return !showText;
		}
		bool ShouldSerializeOpenMode() {
			return openMode != ToolTipOpenMode.OnHover;
		}
		protected override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeImagePosition()
				|| ShouldSerializeShowImage() || ShouldSerializeShowText() || ShouldSerializeOpenMode();
		}
		#endregion
		public override void Assign(ChartElement obj) {
			ChartToolTipController toolTipController = obj as ChartToolTipController;
			if (toolTipController != null) {
				imagePosition = toolTipController.imagePosition;
				showText = toolTipController.showText;
				showImage = toolTipController.showImage;
				openMode = toolTipController.openMode;
			}
		}
		public override string ToString() {
			return string.Format("({0})", GetType().Name);
		}
		protected override ChartElement CreateObjectForClone() {
			return new ChartToolTipController();
		}
	}
}

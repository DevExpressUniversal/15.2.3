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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Spreadsheet;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TitleOptions
	public class TitleOptions : ICloneable<TitleOptions>, ISupportsCopyFrom<TitleOptions>, IChartTextOwnerEx {
		#region Fields
		readonly IChart parent;
		bool overlay;
		IChartText text;
		LayoutOptions layout;
		ShapeProperties shapeProperties;
		TextProperties textProperties;
		#endregion
		public TitleOptions(IChart parent) {
			this.parent = parent;
			this.overlay = false;
			this.text = ChartText.Empty;
			this.layout = new LayoutOptions(parent);
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		public IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public bool Visible { get { return text.TextType != ChartTextType.None; } }
		public LayoutOptions Layout { get { return layout; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		#region Overlay
		public bool Overlay {
			get { return overlay; }
			set {
				if(overlay == value)
					return;
				SetOverlay(value); 
			}
		}
		void SetOverlay(bool value) {
			TitleOptionsOverlayPropertyChangedHistoryItem historyItem = new TitleOptionsOverlayPropertyChangedHistoryItem(DocumentModel, this, overlay, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetOverlayCore(bool value) {
			overlay = value;
			Parent.Invalidate();
		}
		#endregion
		#region Text
		public IChartText Text { 
			get { return text; } 
			set {
				if(value == null)
					value = ChartText.Empty;
				if(text.Equals(value))
					return;
				if (value.TextType == ChartTextType.Auto)
					value = CreateAutoText();
				SetText(value); 
			} 
		}
		void SetText(IChartText value) {
			ChartTextPropertyChangedHistoryItem historyItem = new ChartTextPropertyChangedHistoryItem(DocumentModel, this, text, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTextCore(IChartText value) {
			text = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region ICloneable<TitleOptions> Members
		public TitleOptions Clone() {
			TitleOptions result = new TitleOptions(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<TitleOptions> Members
		public void CopyFrom(TitleOptions value) {
			Guard.ArgumentNotNull(value, "value");
			Overlay = value.Overlay;
			this.layout.CopyFrom(value.layout);
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
			Text = value.text.CloneTo(Parent);
		}
		#endregion
		protected virtual IChartText CreateAutoText() {
			return ChartText.Auto;
		}
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			TextProperties.ResetToStyle();
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			Text.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Text.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region ChartTitleOptions
	public class ChartTitleOptions : TitleOptions, ICloneable<ChartTitleOptions>, ISupportsCopyFrom<ChartTitleOptions> {
		public ChartTitleOptions(IChart parent)
			: base(parent) {
		}
		#region ICloneable<ChartTitleOptions> Members
		public new ChartTitleOptions Clone() {
			ChartTitleOptions result = new ChartTitleOptions(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ChartTitleOptions> Members
		public void CopyFrom(ChartTitleOptions value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
		}
		#endregion
		protected override IChartText CreateAutoText() {
			return new ChartTitleAutoText(Parent);
		}
	}
	#endregion
	#region AxisTitleOptions
	public class AxisTitleOptions : TitleOptions, ICloneable<AxisTitleOptions>, ISupportsCopyFrom<AxisTitleOptions> {
		public AxisTitleOptions(IChart parent)
			: base(parent) {
		}
		#region ICloneable<AxisTitleOptions> Members
		public new AxisTitleOptions Clone() {
			AxisTitleOptions result = new AxisTitleOptions(Parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<AxisTitleOptions> Members
		public void CopyFrom(AxisTitleOptions value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
		}
		#endregion
		protected override IChartText CreateAutoText() {
			return AxisTitleAutoText.Instance;
		}
	}
	#endregion
}

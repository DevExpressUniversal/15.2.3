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
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UserDesigner
{
	using DevExpress.XtraReports.UI;
	public class FilterComponentPropertiesEventArgs : EventArgs {
		IComponent component;
		IDictionary properties;
		public IComponent Component { get { return component; } }
		public IDictionary Properties { get { return properties; } }
		internal FilterComponentPropertiesEventArgs(IComponent component, IDictionary properties) {
			this.component = component;
			this.properties = properties;
		}
	}
	public class FilterControlPropertiesEventArgs : EventArgs {
		XRControl control;
		IDictionary properties;
		public XRControl Control { get { return control; } }
		public IDictionary Properties { get { return properties; } }
		internal FilterControlPropertiesEventArgs(XRControl control, IDictionary properties) {
			this.control = control;
			this.properties = properties;
		}
	}
	public delegate void FilterControlPropertiesEventHandler(object sender, FilterControlPropertiesEventArgs e);
	public class DesignerLoadedEventArgs : EventArgs {
		IDesignerHost designerHost = null;
		public DesignerLoadedEventArgs(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		public IDesignerHost DesignerHost { get { return designerHost; }
		}
	}
	public delegate void DesignerLoadedEventHandler(object sender, DesignerLoadedEventArgs e);
}
namespace DevExpress.XtraReports.UI {
	using System.Windows.Forms;
	using System.Collections.Generic;
	using DevExpress.XtraPrinting.HtmlExport.Controls;
	using DevExpress.XtraPrinting.Native;
	using DevExpress.Utils;
	using DevExpress.Data.Browsing;
	using DevExpress.XtraReports.Native.Data;
	class XRControlStyleEventArgs : EventArgs {
		XRControlStyle style;
		public XRControlStyle Style {
			get { return style; }
		}
		public XRControlStyleEventArgs(XRControlStyle style) {
			this.style = style;
		}
	}
	public class SaveComponentsEventArgs : EventArgs {
		IList components;
		public IList Components {
			get { return components; }
		}
		internal SaveComponentsEventArgs(IList components) {
			this.components = components;
		}
	}
	public class GetValueEventArgs : EventArgs, IDisposable {
		object val;
		bool isSet;
		object dataSource;
		string dataMember;
		XRDataContextBase dataContext;
		public XtraReport Report { get; private set; }
		public object Row { get; private set; }
		internal bool IsSet { get { return isSet; } }
		public object Value {
			get { return val; }
			set { 
				val = value;
				isSet = true;
			}
		}
		internal GetValueEventArgs(XtraReport report, object dataSource, string dataMember, object row) {
			Report = report;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			Row = row;
		}
		public object GetColumnValue(string columnName) {
			if(dataContext == null)
				dataContext = new XRDataContextBase(Report.CalculatedFields, true);
			DataBrowser br = dataContext.GetDataBrowser(dataSource, dataMember, true);
			if(br == null) return DBNull.Value;
			PropertyDescriptorCollection props = br.GetItemProperties();
			PropertyDescriptor prop = props[columnName];
			return prop != null && prop.ComponentType.IsAssignableFrom(Row.GetType()) ? prop.GetValue(Row) : DBNull.Value;
		}
		#region IDisposable Members
		void IDisposable.Dispose() {
			if(dataContext != null) {
				dataContext.Dispose();
				dataContext = null;
			}
		}
		#endregion
	}
	public delegate void GetValueEventHandler(object sender, GetValueEventArgs e);
	public class SummaryGetResultEventArgs : EventArgs {
		object result;
		bool handled;
		IList calculatedValues;
		public SummaryGetResultEventArgs(IList calculatedValues) {
			this.calculatedValues = calculatedValues;
		}
		public IList CalculatedValues { get { return calculatedValues; }
		}
		public object Result { get { return result; } set { result = value; } 
		}
		public bool Handled { get { return handled; } set { handled = value; } 
		}
	}
	public delegate void SummaryGetResultHandler(object sender, SummaryGetResultEventArgs e);
	public class PrintOnPageEventArgs : PrintEventArgs {
		int pageIndex;
		int pageCount;
		public int PageIndex { get { return pageIndex; } }
		public int PageCount { get { return pageCount; } }
		public PrintOnPageEventArgs(int pageIndex, int pageCount) {
			this.pageIndex = pageIndex;
			this.pageCount = pageCount;
		}
	}
	public delegate void PrintOnPageEventHandler(object sender, PrintOnPageEventArgs e);
	public class BandEventArgs : EventArgs {
		private Band band;
		public Band Band { get { return band;} 
		}
		public BandEventArgs(Band band) {
			this.band = band;
		}
	}
	public delegate void BandEventHandler(object sender, BandEventArgs e);
	public class ChangeEventArgs : EventArgs {
		private object oldValue;
		private object newValue;
		public object OldValue { get { return oldValue; } 
		}
		public object NewValue { get { return newValue; } 
		}
		public ChangeEventArgs(object oldValue, object newValue) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
	public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);
	public class BindingEventArgs : EventArgs {
		XRBinding binding;
		object value;
		public object Value {
			get { return value; }
			set { this.value = value; }
		}
		public XRBinding Binding {
			get { return binding; }
		}
		public BindingEventArgs(XRBinding binding, object value) {
			this.binding = binding;
			this.value = value;
		}
	}
	public delegate void BindingEventHandler(object sender, BindingEventArgs e);
	public class DataSourceRowEventArgs : EventArgs {
		private int currentRow;
		private int rowCount;
		public int CurrentRow { get { return currentRow; }
		}
		public int RowCount { get { return rowCount; }
		}
		public DataSourceRowEventArgs(int currentRow, int rowCount) {
			this.currentRow = currentRow;
			this.rowCount = rowCount;
		}
	}
	public delegate void DataSourceRowEventHandler(object sender, DataSourceRowEventArgs e);
	public class DrawEventArgs : EventArgs {
		private RectangleF bounds; 
		private IGraphics gr;
		private VisualBrick brick;
		public RectangleF Bounds { get { return bounds;} }
		public IGraphics UniGraphics { get { return gr;} }
		public VisualBrick Brick { get { return brick; } }
		public DrawEventArgs(IGraphics gr, RectangleF bounds, VisualBrick brick) {
			this.bounds = bounds;
			this.gr = gr;
			this.brick = brick;
		}
		public PageVisualBricksEnumerator GetPageVisualBricksEnumerator() {
			return new PageVisualBricksEnumerator(GetVisualBricks());
		}
		VisualBrick[] GetVisualBricks() {
			BrickEnumerator brickEnumerator = gr.DrawingPage.GetEnumerator();
			List<VisualBrick> bricks = new List<VisualBrick>();
			while(brickEnumerator.MoveNext()) {
				VisualBrick visualBrick = brickEnumerator.Current as VisualBrick;
				if(visualBrick != null)
					bricks.Add(visualBrick);
			}
			return bricks.ToArray();
		}
	}
	public delegate void DrawEventHandler(object sender, DrawEventArgs e);
	public class HtmlEventArgs : EventArgs 
	{
		private IScriptContainer scriptContainer;
		private DXHtmlContainerControl contentCell;
		private VisualBrick brick;
		public DXHtmlContainerControl ContentCell { get { return contentCell; } }
		public IScriptContainer ScriptContainer { get { return scriptContainer; } }
		public VisualBrick Brick { get { return brick; } }
		internal HtmlEventArgs(IScriptContainer scriptContainer, DXHtmlContainerControl contentCell, VisualBrick brick) {
			this.scriptContainer = scriptContainer;
			this.contentCell = contentCell;
			this.brick = brick;
		}
	}
	public delegate void HtmlEventHandler(object sender, HtmlEventArgs e);
	public class PreviewMouseEventArgs : EventArgs 
	{
		DevExpress.XtraPrinting.ChangeEventArgs changeArgs;
		private VisualBrick brick;
		private Control previewControl;
		public Control PreviewControl { 
			get {
				if(previewControl == null)
					previewControl = changeArgs.ValueOf(DevExpress.XtraPrinting.SR.PreviewControl) as Control;
				return previewControl;
			}
		}
		public VisualBrick Brick { get { return brick; } }
		public MouseButtons MouseButton { get { return (MouseButtons)changeArgs.ValueOf(DevExpress.XtraPrinting.SR.MouseButtons); } }
		public PreviewMouseEventArgs(VisualBrick brick, DevExpress.XtraPrinting.ChangeEventArgs changeArgs) {
			this.brick = brick;
			this.changeArgs = changeArgs;
		}
		public Rectangle GetBrickScreenBounds() {
			object bounds = changeArgs.ValueOf(DevExpress.XtraPrinting.SR.BrickScreenBounds);
			return bounds is Rectangle ? (Rectangle)bounds : Rectangle.Empty;
		}
	}
	public delegate void PreviewMouseEventHandler(object sender, PreviewMouseEventArgs e);
	public class TextFormatEventArgs : EventArgs {
		string text;
		object value;
		string format;
		public string Text { get { return text; } set { text = value; } }
		public object Value { get { return value; } }
		public string Format { get { return format; } }
		internal TextFormatEventArgs(string text, string format, object value) {
			this.text = text;
			this.format = format;
			this.value = value;
		}
	}
	public delegate void TextFormatEventHandler(object sender, TextFormatEventArgs e);
	#region Group Sorting Summary
	public class GroupSortingSummaryRowChangedEventArgs : EventArgs {
		object row;
		object fieldValue;
		public GroupSortingSummaryRowChangedEventArgs(object row, object fieldValue) {
			this.row = row;
			this.fieldValue = fieldValue;
		}
		public object Row {
			get { return row; }
		}
		public object FieldValue {
			get { return fieldValue; }
		}
	}
	public delegate void GroupSortingSummaryRowChangedEventHandler(object sender, GroupSortingSummaryRowChangedEventArgs e);
	public class GroupSortingSummaryGetResultEventArgs : EventArgs {
		object result;
		bool handled;
		object[] calculatedValues;
		public GroupSortingSummaryGetResultEventArgs(object[] calculatedValues) {
			this.calculatedValues = calculatedValues;
		}
		public object[] CalculatedValues {
			get { return calculatedValues; }
		}
		public object Result {
			get { return result; }
			set { result = value; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
	public delegate void GroupSortingSummaryGetResultEventHandler(object sender, GroupSortingSummaryGetResultEventArgs e);
	#endregion
}

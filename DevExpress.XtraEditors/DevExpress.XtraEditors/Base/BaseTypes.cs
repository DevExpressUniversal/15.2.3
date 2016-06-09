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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Design;
namespace DevExpress.XtraEditors.Controls {
	public enum PopupBorderStyles {
		Default = 0,
		Simple = 1,
		Flat = 2,
		Style3D = 3,
		NoBorder = 4
	}
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum FilterButtonShowMode { Default, Button, SmartTag }
	public enum DateOnError { Undo, Today, NullDate }
	public enum ProgressViewStyle {Solid, Broken};
	public enum ProgressKind {Horizontal, Vertical};
	public enum ShowDropDown { Never = 0, SingleClick = 1, DoubleClick = 2 }
	public enum DetailLevel { Minimum, Full}
	public enum SpinStyles { Horizontal, Vertical };
	public enum TextEditStyles { Standard, HideTextEditor, DisableTextEditor }
	public enum InplaceType { Standalone, Grid, Bars }
	public enum EditValueChangedFiringMode { Default, Buffered }
	public enum ResizeMode { Default, LiveResize, FrameResize }
	public class QueryDisplayTextEventArgs : EventArgs {
		object editValue;
		string displayText;
		public QueryDisplayTextEventArgs(object editValue, string displayText) {
			this.editValue = editValue;
			this.displayText = displayText;
		}
		public object EditValue { get { return editValue; } }
		public string DisplayText {
			get { return displayText; }
			set { displayText = value; }
		}
	}
	public class QueryResultValueEventArgs : EventArgs {
		object fValue;
		public QueryResultValueEventArgs(object fValue) {
			this.fValue = fValue;
		}
		public object Value {
			get { return fValue; }
			set { fValue = value; }
		}
	}
	public class CloseUpEventArgs : EventArgs {
		PopupCloseMode closeMode;
		bool acceptValue;
		object fValue;
		public CloseUpEventArgs(object val) : this(val, true) {}
		public CloseUpEventArgs(object val, bool accept) : this(val, accept, accept ? PopupCloseMode.Normal : PopupCloseMode.Cancel)  { }
		public CloseUpEventArgs(object val, bool accept, PopupCloseMode closeMode) {
			this.closeMode = closeMode;
			this.fValue = val;
			this.acceptValue = accept;
		}
		public bool AcceptValue {
			get { return acceptValue; }
			set { acceptValue = value; }
		}
		public object Value { 
			get { return fValue; }
			set { fValue = value; }
		}
		public PopupCloseMode CloseMode { get { return closeMode; } }
	}
	public class ClosedEventArgs : EventArgs {
		PopupCloseMode closeMode;
		public ClosedEventArgs(PopupCloseMode closeMode) {
			this.closeMode = closeMode;
		}
		public PopupCloseMode CloseMode	{ get { return closeMode; } }
	}
	public class ChangingEventArgs : CancelEventArgs {
		object oldValue;
		object newValue;
		public ChangingEventArgs(object oldValue, object newValue) : this(oldValue, newValue, false) { }
		public ChangingEventArgs(object oldValue, object newValue, bool cancel) : base(cancel) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public object OldValue { 
			get { return oldValue; }
			protected set { oldValue = value; }
		}
		public object NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
	}
	public class ButtonPressedEventArgs : EventArgs {
		EditorButton button;
		public ButtonPressedEventArgs(EditorButton button) {
			this.button = button;
		}
		public EditorButton Button { get { return button; } }
	}
	public class SpinEventArgs : EventArgs {
		bool _handled;
		bool _spinUp;
		public SpinEventArgs(bool isSpinUp) {
			this._spinUp = isSpinUp;
			this._handled = false;
		}
		public bool Handled {
			get { return _handled; }
			set { this._handled = value; }
		}
		public bool IsSpinUp {
			get { return _spinUp; }
			set { this._spinUp = value; }
		}
	}
	public class BeforeShowMenuEventArgs : EventArgs {
		DXPopupMenu _menu;
		Point location;
		bool restoreMenu = false;
		public BeforeShowMenuEventArgs(DXPopupMenu menu, Point location) : this(menu) {
			this.location = location;
		}
		public BeforeShowMenuEventArgs(DXPopupMenu menu) {
			this._menu = menu;
			this.location = new Point(-1, -1);
		}
		public Point Location { get { return location; } set { location = value; } }
		public DXPopupMenu Menu {
			get { return _menu; }
		}
		public bool RestoreMenu {
			get { return restoreMenu; }
			set { restoreMenu = value; }
		}
	}
	public class QueryCheckStatesEventArgs : EventArgs {
		protected CheckState fCheckState;
		protected object fValue = null;
		bool _handled;
		public QueryCheckStatesEventArgs(object fValue, CheckState checkState) {
			this.fCheckState = checkState;
			this.fValue = fValue;
			this._handled = false;
		}
		public bool Handled {
			get { return _handled; }
			set { _handled = value; }
		}
	}
	public class QueryCheckStateByValueEventArgs : QueryCheckStatesEventArgs {
		public QueryCheckStateByValueEventArgs(object fValue) : base(fValue, CheckState.Indeterminate) {
		}
		public CheckState CheckState {
			get { return fCheckState; }
			set { fCheckState = value; }
		}
		public object Value { get { return fValue; } }
	}
	public class QueryValueByCheckStateEventArgs : QueryCheckStatesEventArgs {
		public QueryValueByCheckStateEventArgs(CheckState state) : base(null, state) {
		}
		public CheckState CheckState { get { return fCheckState; } }
		public object Value {
			get { return fValue; }
			set { fValue = value; }
		}
	}
	public class CustomDisplayTextEventArgs : EventArgs {
		object value;
		string displayText;
		public CustomDisplayTextEventArgs(object value, string displayText) { 
			this.value = value;
			this.displayText = displayText;
		}
		public object Value { get { return value; } }
		public string DisplayText { 
			get { return displayText; }
			set {
				if(value == null) value = string.Empty;
				displayText = value;
			}
		}
	}
	public class QueryProcessKeyEventArgs : EventArgs {
		bool isNeededKey;
		Keys keyData;
		public QueryProcessKeyEventArgs(bool isNeededKey, Keys keyData) {
			this.isNeededKey = isNeededKey;
			this.keyData = keyData;
		}
		public Keys KeyData { get { return keyData; } }
		public bool IsNeededKey {
			get { return isNeededKey; }
			set { isNeededKey = value; }
		}
	}
	public class ConvertEditValueEventArgs : EventArgs {
		bool handled;
		object fValue;
		public ConvertEditValueEventArgs() : this(null) { }
		public ConvertEditValueEventArgs(object fValue) {
			this.handled = false;
			this.fValue = fValue;
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public object Value {
			get { return fValue; }
			set { fValue = value; }
		}
		protected internal void Initialize(object fValue) {
			this.fValue = fValue;
			this.handled = false;
		}
	}
	public class EditorValueException : Exception {
		string errorText;
		Exception sourceException;
		public EditorValueException(Exception sourceException, string errorText) {
			this.sourceException = sourceException;
			this.errorText = errorText;
		}
		public string ErrorText { get { return errorText; } set { errorText = value; } }
		public Exception SourceException { get { return sourceException; } set { sourceException = value; } }
	}
	public enum ExceptionMode { DisplayError, ThrowException, NoAction, Ignore} ;
	public class ExceptionEventArgs : EventArgs {
		string errorText;
		string windowCaption;
		Exception exception;
		ExceptionMode exceptionMode;
		public ExceptionEventArgs(string errorText, Exception exception) : 
			this(errorText, Localizer.Active.GetLocalizedString(StringId.CaptionError), exception, ExceptionMode.DisplayError) { }
		public ExceptionEventArgs(string errorText, string windowCaption, Exception exception, ExceptionMode exceptionMode) {
			this.errorText = errorText;
			this.windowCaption = windowCaption;
			this.exception = exception;
			this.exceptionMode = exceptionMode;
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		public string WindowCaption {
			get { return windowCaption; }
			set { windowCaption = value; }
		}
		public Exception Exception { get { return exception; } }
		public ExceptionMode ExceptionMode {
			get { return exceptionMode; }
			set { exceptionMode = value; }
		}
	}
	public class InvalidValueExceptionEventArgs : ExceptionEventArgs {
		object fValue;
		public InvalidValueExceptionEventArgs(string errorText, Exception exception, object fValue) : 
			this(errorText, Localizer.Active.GetLocalizedString(StringId.CaptionError), exception, ExceptionMode.DisplayError, fValue) { }
		public InvalidValueExceptionEventArgs(string errorText, string windowCaption, Exception exception, ExceptionMode exceptionMode, object fValue) :
			base(errorText, windowCaption, exception, exceptionMode) {
			this.fValue = fValue;
		}
		public object Value { get { return fValue; } }
	}
	public class BaseContainerValidateEditorEventArgs : EventArgs {
		object fValue;
		bool valid;
		string errorText;
		public BaseContainerValidateEditorEventArgs(object fValue) {
			this.fValue = fValue;
			this.valid = true;
			this.errorText = "";
		}
		public object Value {
			get { return fValue; }
			set { fValue = value; }
		}
		public bool Valid {
			get { return valid; }
			set { valid = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
		protected internal void TryValidateViaAnnotationAttributes(object value, Data.Utils.AnnotationAttributes annotationAttributes) {
			if(!CanValidateEditorViaAnnotationAttributes()) 
				return;
			if(Valid && annotationAttributes != null) {
				string errorMessage;
				if(!annotationAttributes.TryValidateValue(value, out errorMessage)) {
					Valid = false;
					ErrorText = errorMessage;
					return;
				}
				if(CanValidateRowViaAnnotationAttributes()) {
					object row = GetRowObject();
					if(object.ReferenceEquals(null, row))
						return;
					if(!annotationAttributes.TryValidateValue(value, row, out errorMessage)) {
						Valid = false;
						ErrorText = errorMessage;
					}
				}
			}
		}
		protected virtual bool CanValidateEditorViaAnnotationAttributes() { return true; }
		protected virtual bool CanValidateRowViaAnnotationAttributes() { return false; }
		protected virtual object GetRowObject() { return null; }
	}
	public class BaseEditValidatingEventArgs : CancelEventArgs {
		BaseEdit editor;
		public BaseEditValidatingEventArgs(BaseEdit editor, bool cancel)
			: base(cancel) {
			this.editor = editor;
		}
		public object Value { get; set; }
		public string ErrorText { get; set; }
		protected internal void TryValidateViaAnnotationAttributes(object value, Data.Utils.AnnotationAttributes annotationAttributes) {
			if(Cancel || !editor.CanValidateValueViaAnnotationAttributes())
				return;
			if(annotationAttributes != null) {
				string errorMessage;
				if(!annotationAttributes.TryValidateValue(value, out errorMessage)) {
					Cancel = true;
					ErrorText = errorMessage;
					return;
				}
				if(editor.CanValidateObjectViaAnnotationAttributes()) {
					object row = editor.GetObjectToValidate();
					if(object.ReferenceEquals(null, row))
						return;
					if(!annotationAttributes.TryValidateValue(value, row, out errorMessage)) {
						Cancel = true;
						ErrorText = errorMessage;
					}
				}
			}
		}
	}
	public class ChangeEventArgs : EventArgs {
		protected string fName = string.Empty;
		protected object fValue = null;
		protected bool fCancel = false;
		public string Name {
			get { return fName; }
		}
		public object Value {
			get { return fValue; }
		}
		public ChangeEventArgs() {
		}
		public ChangeEventArgs(string name, object value) {
			this.fName = name;
			this.fValue = value;
		}
	}
	public class DoValidateEventArgs: EventArgs {
		DoValidateEventArgs() {}
		static DoValidateEventArgs empty = new DoValidateEventArgs();
		new public static DoValidateEventArgs Empty { get { return empty; } } 
	}
	public class CustomDrawButtonEventArgs : EventArgs {		
		ObjectPainter painterCore;
		EditorButtonObjectInfoArgs infoCore;
		MethodInvoker defaultDraw;
		public CustomDrawButtonEventArgs(EditorButtonObjectInfoArgs info, ObjectPainter painter) {
			infoCore = info;
			painterCore = painter;
		}
		internal void SetDefaultDraw(MethodInvoker defaultDraw) {
			this.defaultDraw = defaultDraw;
		}
		public EditorButtonObjectInfoArgs Info { get { return infoCore; } }
		public EditorButton Button { get { return infoCore.Button; } }											  
		public ObjectState State { get { return infoCore.State; } }
		public GraphicsCache GraphicsCache { get { return infoCore.Cache; } }
		public Graphics Graphics { get { return infoCore.Graphics; } }
		public System.Drawing.Rectangle Bounds { get { return infoCore.Bounds; } }
		public EditorButtonPainter Painter { get { return painterCore as EditorButtonPainter; } }
		public Color GetForeColor() { return Painter.GetForeColor(infoCore); }
		public void GraphicsClear() {
			GraphicsClipState clipState = GraphicsCache.ClipInfo.SaveClip();
			GraphicsCache.ClipInfo.SetClip(Bounds);
			Graphics.Clear(infoCore.BackAppearance.GetBackColor());
			GraphicsCache.ClipInfo.RestoreClipRelease(clipState);
		}
		public void DefaultDraw() {
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		public bool Handled { get; set; }
	}
	public delegate void CustomDisplayTextEventHandler(object sender, CustomDisplayTextEventArgs e);
	public delegate void CustomDrawButtonEventHandler(object sender, CustomDrawButtonEventArgs e);
	public delegate void BaseContainerValidateEditorEventHandler(object sender, BaseContainerValidateEditorEventArgs e);
	public delegate void InvalidValueExceptionEventHandler(object sender, InvalidValueExceptionEventArgs e);
	public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);
	public delegate void ConvertEditValueEventHandler(object sender, ConvertEditValueEventArgs e);
	public delegate void QueryCheckStateByValueEventHandler(object sender, QueryCheckStateByValueEventArgs e);
	public delegate void QueryValueByCheckStateEventHandler(object sender, QueryValueByCheckStateEventArgs e);
	public delegate void CloseUpEventHandler(object sender, CloseUpEventArgs e);
	public delegate void ClosedEventHandler(object sender, ClosedEventArgs e);
	public delegate void ChangingEventHandler(object sender, ChangingEventArgs e);
	public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);
	public delegate void ButtonPressedEventHandler(object sender, ButtonPressedEventArgs e);
	public delegate void SpinEventHandler(object sender, SpinEventArgs e);
	public delegate void BeforeShowMenuEventHandler(object sender, BeforeShowMenuEventArgs e);
	public delegate void QueryResultValueEventHandler(object sender, QueryResultValueEventArgs e);
	public delegate void QueryDisplayTextEventHandler(object sender, QueryDisplayTextEventArgs e);
	public delegate void QueryProcessKeyEventHandler(object sender, QueryProcessKeyEventArgs e);
}

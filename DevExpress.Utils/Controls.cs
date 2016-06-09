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
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Data.Utils;
using DevExpress.Utils.Helpers;
namespace DevExpress.Utils.Controls {
	public class ControlPresenterEditor : UITypeEditor {
		public ControlPresenterEditor() : base() { 
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(provider == null || context == null)
				return value;
			ControlPresenter presenter = context.Instance as ControlPresenter;
			if(presenter == null)
				return value;
			IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if(editorService == null)
				return value;
			using(SelectedControlEditor editor = new SelectedControlEditor(editorService, presenter.Controls, presenter.SelectedControl)) {
				editorService.DropDownControl(editor);
				return editor.SelectedControl;
			}
		}
	}
	public class SelectedControlEditor : ListBox {
		ControlCollection controls;
		IWindowsFormsEditorService editorService;
		public SelectedControlEditor(IWindowsFormsEditorService editorService, ControlCollection controls, Control selectedControl) {
			this.controls = controls;
			this.editorService = editorService;
			foreach(Control control in EditorControls) {
				Items.Add(control.Name);
			}
			SelectedIndex = selectedControl == null? -1: Items.IndexOf(selectedControl.Name);
			SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
		}
		protected virtual void OnSelectedIndexChanged(object sender, EventArgs e) {
			EditorService.CloseDropDown();
		}
		IWindowsFormsEditorService EditorService { get { return editorService; } }
		public ControlCollection EditorControls { get { return controls; } }
		public Control SelectedControl { 
			get { 
				return SelectedIndex < 0? null: EditorControls[SelectedIndex];
			} 
		}
	}
	[ToolboxItem(false)]
	public class ControlPresenter : ContainerControl {
		private static readonly object selectedControlChanged = new object();
		Control selectedControl;
		[Editor(typeof(ControlPresenterEditor), typeof(UITypeEditor))]
		public Control SelectedControl {
			get { return selectedControl; }
			set {
				if(SelectedControl == value)
					return;
				selectedControl = value;
				OnSelectedControlChanged();
			}
		}
		int selectedControlIndex = -1;
		public int SelectedControlIndex {
			get { return selectedControlIndex; }
			set {
				if(SelectedControlIndex == value)
					return;
				value = Math.Min(value, Controls.Count - 1);
				selectedControlIndex = value;
				OnSelectedControlIndexChanged();
			}
		}
		protected virtual void OnSelectedControlIndexChanged() {
			SelectedControl = SelectedControlIndex == -1 ? null : Controls[SelectedControlIndex];
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			SelectedControl = e.Control;
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			if(SelectedControl != e.Control)
				return;
			SelectedControl = Controls.Count > 0 ? Controls[0] : null;
		}
		protected virtual void UpdateControlsVisibility() {
			foreach(Control ctrl in Controls) {
				ctrl.Visible = ctrl == SelectedControl;
			}
		}
		protected virtual void OnSelectedControlChanged() {
			UpdateControlsVisibility();
			SelectedControlIndex = SelectedControl == null? -1: Controls.IndexOf(SelectedControl);
			RaiseSelectedControlChanged();
		}
		protected virtual void RaiseSelectedControlChanged() {
			EventHandler handler = Events[selectedControlChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public event EventHandler SelectedControlChanged {
			add { Events.AddHandler(selectedControlChanged, value); }
			remove { Events.RemoveHandler(selectedControlChanged, value); }
		}
	}
	public interface IControlIterator : IDisposable {
		object GetNextObject(object currentObject);
		bool IsLastObject(object currentObject);
		object GetFirstObject();
	}
	[ToolboxItem(false)]
	public class PanelBase : XtraPanel {
		const int WM_CAPTURECHANGED = 0x215;
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			if(m.Msg == WM_CAPTURECHANGED) {
				if(m.LParam == this.Handle) 
					OnGotCapture();
				else
					OnLostCapture();
			}
			base.WndProc(ref m);
		}
		protected virtual void OnLostCapture() { }
		protected virtual void OnGotCapture() { }
	}
	[ToolboxItem(false)]
	public class ControlBase : Control {
#if DEBUGTEST
		public ControlBase() {
			System.Diagnostics.Debug.Assert(!IsHandleCreated);
			base.DestroyHandle();
		}
#endif
		const int WM_CAPTURECHANGED = 0x215;
		protected override void WndProc ( ref System.Windows.Forms.Message m ) {
			if(m.Msg == WM_CAPTURECHANGED) {
				if(m.LParam == this.Handle) 
					OnGotCapture();
				else
					OnLostCapture();
			}
			base.WndProc(ref m);
		}
		bool isRightToLeft = false;
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			CheckRightToLeft();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			CheckRightToLeft();
		}
		protected void CheckRightToLeft() {
			bool newRightToLeft = WindowsFormsSettings.GetIsRightToLeft(this);
			if(newRightToLeft == this.isRightToLeft) return;
			this.isRightToLeft = newRightToLeft;
			OnRightToLeftChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsRightToLeft { get { return isRightToLeft; } }
		protected virtual void OnRightToLeftChanged() {
		}
		protected virtual void OnLostCapture() { }
		protected virtual void OnGotCapture() { }
		protected virtual bool GetValidationCanceled() {
			return GetValidationCanceled(this);
		}
		public static bool GetCanProcessMnemonic(Control control) {
			if(control == null) return false;
			System.Reflection.MethodInfo mi = typeof(Control).GetMethod("CanProcessMnemonic", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(mi != null) return (bool)mi.Invoke(control, null);
			return true;
		}
		public static Control GetValidationCanceledSource(Control control) {
			if(control == null || !GetValidationCanceled(control)) return null;
			Control res = control;
			while(res.Parent != null) {
				if(!GetValidationCanceled(res.Parent)) return res;
				res = res.Parent;
			}
			return res;
		}
		public static bool GetValidationCanceled(Control control) {
			if(control == null) return false;
			System.Reflection.PropertyInfo pi = typeof(Control).GetProperty("ValidationCancelled", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(pi != null) return (bool)pi.GetValue(control, null);
			return true;
		}
		public static void ResetValidationCanceled(Control control) {
			if(control == null) return;
			System.Reflection.PropertyInfo pi = typeof(Control).GetProperty("ValidationCancelled", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(pi != null) pi.SetValue(control, false, null);
		}
		public static Control GetUnvalidatedControl(Control control) {
			if(control == null) return null;
			ContainerControl container = control as ContainerControl;
			if(container == null) container = control.GetContainerControl() as ContainerControl;
			if(container == null) container = control.FindForm();
			if(container == null) return null;
			System.Reflection.FieldInfo field = typeof(ContainerControl).GetField("unvalidatedControl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(field != null) return field.GetValue(container) as Control;
			return null;
		}
		public static bool IsUnvalidatedControlIsParent(Control control) {
			Control unvalidated = GetUnvalidatedControl(control);
			if(unvalidated == null) return false;
			while(control != null) {
				if(unvalidated == control) return true;
				control = control.Parent;
			}
			return false;
		}
		static MethodInfo xClear = null;
		[Obsolete("Use ClearPreferredSizeCache"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void ClearPrefferedSizeCache(Control control) {
			ClearPreferredSizeCache(control);
		}
		public static void ClearPreferredSizeCache(Control control) {
#if DXWhidbey
			if(xClear == null) {
				Type type = typeof(Control).Assembly.GetType("System.Windows.Forms.Layout.CommonProperties", false);
				if(type != null) xClear = type.GetMethod("xClearPreferredSizeCache", BindingFlags.NonPublic | BindingFlags.Static);
			}
			if(xClear != null) xClear.Invoke(null, new object[] { control });
#endif
		}
	}
	public interface IXtraResizableControl {
		Size MinSize { get; }
		Size MaxSize { get; }
		event EventHandler Changed;
		bool IsCaptionVisible { get; }
	}
	public interface IDXFocusController {
		bool FocusOnMouseDown { get; set; }
	}
	public class ImageHelper {
		public static Image LoadImageFromFileEx(string file) {
			FileStream stream = File.OpenRead(file);
			MemoryStream ms = new MemoryStream();
			byte[] buffer = new Byte[stream.Length];
			stream.Read(buffer, 0, (int)stream.Length);
			stream.Close();
			ms.Write(buffer, 0, buffer.Length);
			ms.Seek(0, SeekOrigin.Begin);
			Image image = ImageTool.ImageFromStream(ms);
			return image;
		}
		public static Image MakeTransparent(Image image) { return MakeTransparent(image, true); }
		public static Image MakeTransparent(Image image, bool forceFormat) {
			if(image == null) return image;
			if(!forceFormat && (ImageFormat.Emf.Equals(image.RawFormat) || ImageFormat.Gif.Equals(image.RawFormat) || ImageFormat.Png.Equals(image.RawFormat)
					|| ImageFormat.Icon.Equals(image.RawFormat) || ImageFormat.Wmf.Equals(image.RawFormat))) return image;
			if(image.PixelFormat == PixelFormat.Format32bppArgb ||
				image.PixelFormat == PixelFormat.Format32bppPArgb ||
				image.PixelFormat == PixelFormat.Format64bppArgb ||
				image.PixelFormat == PixelFormat.Format64bppPArgb) return image;
			if(ImageFormat.Icon.Equals(image.RawFormat)) return image;
			Bitmap bitmap = image.Clone() as Bitmap;
			if(bitmap != null)
				bitmap.MakeTransparent();
			return bitmap as Image;
		}
		[Obsolete("Use ResourceImageHelper.CreateCursorFromResources")]
		public static Cursor CreateCursorFromResources(string name, System.Reflection.Assembly asm) {
			return ResourceImageHelper.CreateCursorFromResources(name, asm);
		}
		[Obsolete("Use ResourceImageHelper.CreateImageListFromResources")]
		public static ImageList CreateImageListFromResources(string name, System.Reflection.Assembly asm, Size size) {
			return ResourceImageHelper.CreateImageListFromResources(name, asm, size, Color.Magenta);
		}
		[Obsolete("Use ResourceImageHelper.CreateImageListFromResources")]
		public static ImageList CreateImageListFromResources(string name, System.Reflection.Assembly asm, Size size, Color transparent) {
			return ResourceImageHelper.CreateImageListFromResources(name, asm, size, transparent, ColorDepth.Depth8Bit); 
		}
		public static ImageCollection CreateImageCollectionFromResources(string name, System.Reflection.Assembly asm, Size size) {
			return CreateImageCollectionFromResources(name, asm, size, Color.Empty);
		}
		public static ImageCollection CreateImageCollectionFromResources(string name, System.Reflection.Assembly asm, Size size, Color transparent) {
			Bitmap image = ResourceImageHelper.CreateBitmapFromResources(name, asm);
			return CreateImageCollectionCore(image, size, transparent);
		}
		public static ImageCollection CreateImageCollectionFromResourcesEx(string name, System.Reflection.Assembly asm, Size size) {
			return CreateImageCollectionFromResourcesEx(name, asm, size, Color.Empty);
		}
		public static ImageCollection CreateImageCollectionFromResourcesEx(string name, System.Reflection.Assembly asm, Size size, Color transparent) {
			Image image = ResourceImageHelper.CreateImageFromResourcesEx(name, asm);
			return CreateImageCollectionCore(image, size, transparent);
		}
		public static ImageCollection CreateImageCollectionCore(Image image, Size size, Color transparent) {
			if(object.Equals(image.RawFormat, ImageFormat.Png)) {
				Bitmap bmp = new Bitmap(image.Width, image.Height, (image.PixelFormat & PixelFormat.Alpha) != 0 ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
				bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
				Graphics g = Graphics.FromImage(bmp);
				g.CompositingMode = CompositingMode.SourceCopy;
				g.DrawImageUnscaled(image, Point.Empty);
				g.Dispose();
				image = bmp;
			}
			ImageCollection images = new ImageCollection();
			images.ImageSize = size;
			images.TransparentColor = transparent;
			images.AddImageStrip(image);
			return images;
		}
		[Obsolete("Use ResourceImageHelper.FillImageListFromResources")]
		public static void FillImageListFromResources(ImageList images, string name, System.Reflection.Assembly asm, Color transparent) {
			ResourceImageHelper.FillImageListFromResources(images, name, asm, transparent);
		}
		[Obsolete("Use ResourceImageHelper.FillImageListFromResources")]
		public static void FillImageListFromResources(ImageList images, string name, Type type) {
			ResourceImageHelper.FillImageListFromResources(images, name, type.Assembly);
		}
		[Obsolete("Use ResourceImageHelper.FillImageListFromResources")]
		public static void FillImageListFromResources(ImageList images, string name, System.Reflection.Assembly asm) {
			ResourceImageHelper.FillImageListFromResources(images, name, asm);
		}
		[Obsolete("Use ResourceImageHelper.CreateImageListFromResources")]
		public static ImageList CreateImageListFromResources(string name, System.Reflection.Assembly asm, Size size, Color transparent, ColorDepth depth) {
			return ResourceImageHelper.CreateImageListFromResources(name, asm, size, transparent, depth);
		}
		[Obsolete("Use ResourceImageHelper.CreateImageFromResources")]
		public static Image CreateImageFromResources(string name, System.Reflection.Assembly asm) {
			return ResourceImageHelper.CreateBitmapFromResources(name, asm);
		}
		[Obsolete("Use ResourceImageHelper.CreateBitmapFromResources")]
		public static Bitmap CreateBitmapFromResources(string name, Type type) {
			return ResourceImageHelper.CreateBitmapFromResources(name, type);
		}
		[Obsolete("Use ResourceImageHelper.CreateBitmapFromResources")]
		public static Bitmap CreateBitmapFromResources(string name, System.Reflection.Assembly asm) {
			return ResourceImageHelper.CreateBitmapFromResources(name, asm);
		}
		[Obsolete("Use ResourceImageHelper.CreateIconFromResources")]
		public static Icon CreateIconFromResources(string name, Type type) {
			return ResourceImageHelper.CreateIconFromResources(name, type);
		}
		[Obsolete("Use ResourceImageHelper.CreateIconFromResources")]
		public static Icon CreateIconFromResources(string name, System.Reflection.Assembly asm) {
			return ResourceImageHelper.CreateIconFromResources(name, asm);
		}
		[Obsolete("Use BitmapCreator.CreateBitmap")]
		public static Bitmap CreateBitmap(Image original, Color backColor) {
			return BitmapCreator.CreateBitmap(original, backColor);
		}
	}
	public enum EventType { KeyDown, KeyUp, KeyPress, Click, DoubleClick, MouseDown, MouseUp, MouseMove, MouseWheel, ProcessKey, Resize, MouseEnter, MouseLeave, LostCapture, ActivateApp, DeactivateApp,GestureNotify };
	public enum EventResult { None, Handled };
	public enum ExpandButtonMode { Normal, Inverted };
	public enum NeedKeyType { Enter, Escape, Tab, Dialog };
	public abstract class BaseHandler : IDisposable {
		bool mouseHere;
		public BaseHandler() {
			this.mouseHere = false;
		}
		public virtual void Dispose() {
		}
		public virtual bool MouseHere { 
			get { return mouseHere; }
			set { 
				if(MouseHere == value) return;
				mouseHere = value;
				if(value) 
					OnMouseEnter(EventArgs.Empty);
				else
					OnMouseLeave(EventArgs.Empty);
			}
		}
		public virtual EventResult ProcessEvent(EventType etype, object args) {
			EventResult res = EventResult.None;
			switch(etype) {
				case EventType.KeyDown : OnKeyDown(args as KeyEventArgs); break;
				case EventType.KeyUp : OnKeyUp(args as KeyEventArgs); break;
				case EventType.KeyPress : OnKeyPress(args as KeyPressEventArgs); break;
				case EventType.Click : OnClick(args as MouseEventArgs); break;
				case EventType.DoubleClick : OnDoubleClick(args as MouseEventArgs); break;
				case EventType.MouseEnter : UpdateMouseHere(args as MouseEventArgs); break;
				case EventType.MouseLeave : UpdateMouseHere(new Point(-10000, -10000), false); break;
				case EventType.MouseDown : if(OnMouseDown(args as MouseEventArgs)) res = EventResult.Handled;
					break;
				case EventType.MouseUp : if(OnMouseUp(args as MouseEventArgs)) res = EventResult.Handled; 
					break;
				case EventType.MouseMove : if(OnMouseMove(args as MouseEventArgs)) res = EventResult.Handled;
					break;
				case EventType.MouseWheel : if(OnMouseWheel(args as MouseEventArgs)) res = EventResult.Handled;
					break;
				case EventType.ProcessKey: if(ProcessKey(args as KeyEventArgs)) res = EventResult.Handled;
					break;
				case EventType.LostCapture : OnLostCapture(); break;
				case EventType.Resize: OnResize((Rectangle)args); break;
				default:
					throw new Exception("Unregistered eventType " + etype.ToString());
			}
			return res;
		}
		protected abstract Rectangle ClientBounds { get; }
		protected virtual void OnLostCapture() { }
		protected virtual void OnKeyDown(KeyEventArgs e) { }
		protected virtual void OnKeyUp(KeyEventArgs e) { }
		protected virtual void OnKeyPress(KeyPressEventArgs e) { }
		protected virtual bool OnMouseDown(MouseEventArgs e) { return false; }
		protected virtual bool OnMouseUp(MouseEventArgs e) { return false; }
		protected virtual bool OnMouseMove(MouseEventArgs e) { 
			UpdateMouseHere(e);
			return false; 
		}
		protected virtual bool OnMouseWheel(MouseEventArgs e) { return false; }
		protected virtual void OnClick(MouseEventArgs e) { }
		protected virtual void OnDoubleClick(MouseEventArgs e) { }
		protected virtual void OnMouseEnter(EventArgs e) { }
		protected virtual void OnMouseLeave(System.EventArgs e) { }
		protected virtual void OnResize(Rectangle clientRect) { }
		public virtual bool ProcessKey(KeyEventArgs keys) { return false; }
		public virtual bool NeedKey(NeedKeyType keyType) { return false; }
		public virtual bool RequireMouse(MouseEventArgs e) { return false; }
		public virtual void UpdateMouseHere(Point p, bool val) {
			MouseHere = val;
		}
		protected void UpdateMouseHere(MouseEventArgs e) {
			UpdateMouseHere(new Point(e.X, e.Y), ClientBounds.Contains(e.X, e.Y));
		}
	}
	public interface IDXVisualControl222 {
		BaseHandler Handler { get; }
		ObjectPainter Painter { get; }
		ObjectInfoArgs ViewInfo { get; }
		Rectangle Bounds { get; }
		object Owner { get; } 
		IDXVisualControl222 Parent { get; }
	}
	public class VisualControlCollection : CollectionBase {
		public IDXVisualControl222 this[int index] { get { return (IDXVisualControl222)List[index]; } }
		public IDXVisualControl222 this[Point point] {
			get {
				int count = Count;
				Rectangle lastBounds = Rectangle.Empty;
				IDXVisualControl222 res = null;
				for(int n = 0; n < count; n++) {
					IDXVisualControl222 vc = this[n];
					if(!vc.Bounds.IsEmpty && vc.Bounds.Contains(point)) {
						if(lastBounds.IsEmpty || lastBounds.Contains(vc.Bounds)) {
							lastBounds = vc.Bounds;
							res = vc;
						}
					}
				}
				return res;
			}
		}
		public void Add(IDXVisualControl222 control) {
			List.Add(control);
		}
		public void Remove(IDXVisualControl222 control) {
			List.Remove(control);
		}
		public virtual void Draw(PaintEventArgs e) {
			if(Count == 0) return;
			GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e));
			try {
				Draw(cache);
			}
			finally {
				cache.Dispose();
			}
		}
		public virtual void Draw(GraphicsCache cache) {
			Rectangle clipRect = cache.PaintArgs == null ? Rectangle.Empty : cache.PaintArgs.ClipRectangle;
			for(int n = 0; n < Count; n++) {
				IDXVisualControl222 ctrl = this[n];
				DrawControl(cache, ctrl, clipRect);
			}
		}
		protected virtual void DrawControl(GraphicsCache cache, IDXVisualControl222 ctrl, Rectangle clipRect) {
			if(ctrl.Bounds.IsEmpty || ctrl.ViewInfo == null ||
				!clipRect.IsEmpty && !clipRect.IntersectsWith(ctrl.Bounds)) return;
			ObjectInfoArgs args = ctrl.ViewInfo;
			try {
				args.Cache = cache;
				ctrl.Painter.DrawObject(args);
			}
			finally {
				args.Cache = null;
			}
		}
		public virtual bool Contains(IDXVisualControl222 control) { return List.Contains(control); }
		public IDXVisualControl222[] GetVisibleControls() {
			ArrayList list = null;
			for(int n = 0; n < Count; n++) {
				IDXVisualControl222 vc = this[n];
				if(!vc.Bounds.IsEmpty) {
					if(list == null) list = new ArrayList();
					list.Add(vc);
				}
			}
			return list == null ? null : list.ToArray(typeof(IDXVisualControl222)) as IDXVisualControl222[];
		}
		public IDXVisualControl222[] GetControlsAtPoint(Point point) {
			IDXVisualControl222[] res = null;
			int count = Count;
			for(int n = 0; n < count; n++) {
				IDXVisualControl222 vc = this[n];
				if(!vc.Bounds.IsEmpty && vc.Bounds.Contains(point)) {
					IDXVisualControl222[] temp = res;
					res = new IDXVisualControl222[res == null ? 1 : res.Length + 1];
					if(temp != null) Array.Copy(temp, res, temp.Length);
					res[res.Length - 1] = vc;
				}
			}
			return res;
		}
	}
	public interface IDXVisualControl {
		Rectangle Bounds { get; set; }
		void ProcessEvent(ProcessEventEventArgs e);
		void Draw(GraphicsCache cache);
		IDXVisualContainer VisualContainer { get; set; }
	}
	public interface IDXVisualContainer {
		VisualControlCollection VisualControls { get; }
		void Invalidate(IDXVisualControl control);
		IDXVisualControl FocusedControl { get ; }
		IDXVisualControl MouseCapture { get; }
	}
	public class ProcessEventEventArgs : EventArgs {
		EventType eventType;
		EventResult result;
		object eventArgs;
		public ProcessEventEventArgs(EventType eventType, object eventArgs) {
			this.eventType = eventType;
			this.eventArgs = eventArgs;
			this.result = EventResult.None;
		}
		public EventType EventType { get { return eventType; } }
		public EventResult Result {
			get { return result; }
			set { result = value; }
		}
		public bool Handled {
			get { return Result == EventResult.Handled; }
			set { Result = value ? EventResult.Handled : EventResult.None; }
		}
		public object EventArgs { get { return eventArgs; } }
	}
	public delegate void ProcessEventEventHandler(object sender, ProcessEventEventArgs e);
}
namespace DevExpress.Utils {
	public class HideException : Exception {
	}
}

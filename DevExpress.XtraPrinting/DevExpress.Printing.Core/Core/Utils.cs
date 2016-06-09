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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Printing;
namespace DevExpress.XtraPrinting.Native {
	public static class FileHelper {
		public static string SetValidExtension(string fileName, string primaryExt, string[] extensions) {
			foreach(string extension in extensions) {
				if(String.Compare(Path.GetExtension(fileName), extension, true) == 0)
					return fileName;
			}
			return fileName + primaryExt;
		}
		public static void CopyDirectory(string src, string dest) {
			List<string> dirs = new List<string>(Directory.GetDirectories(src, "*", SearchOption.AllDirectories));
			dirs.Add(src);
			foreach(string dir in dirs) {
				string destDir = dir.Replace(src, dest);
				if(!Directory.Exists(destDir))
					Directory.CreateDirectory(destDir);
				string[] files = Directory.GetFiles(dir);
				foreach(string file in files)
					CopyFile(file, file.Replace(src, dest));
			}
		}
		public static void CopyFile(string src, string dest) {
			if(File.Exists(dest))
				File.SetAttributes(dest, FileAttributes.Normal);
			File.Copy(src, dest, true);
		}
	}
	public class NativeSR {
		[Obsolete("Use CatPrinting instead")]
		public const string
			CatPrintPreview = "Print Preview";
		public const string
			CatAction = "Action",
			CatDocumentCreation = "Document Creation",
			CatPrinting = "Printing",
			CatBehavior = "Behavior",
			CatReportArea = "Report Area",
			CatAppearance = "Appearance",
			CatPropertyChanged = "Property Changed",
			CatPageLayout = "Page Layout",
			CatHeadersFooters = "HeadersFooters",
			CatPrintOptions = "Print Options",
			CatReportService = "Report Service",
			CatReportServer = "Report Server",
			PrintingSystem = "PrintingSystemBase",
			RegistryPath = "Software\\Developer Express\\XtraReports\\",
			CheckFileName = "Core.Images.check.gif",
			CheckGreyFileName = "Core.Images.check_grey.gif",
			UncheckFileName = "Core.Images.uncheck.gif",
			BlankFileName = "Core.Images.blank.gif",
			TraceSource = "DXperience.Reporting",
			TraceSourceTests = "DXperience.Reporting.Tests",
			InfoString = "This application was created using the trial version of the XtraReports.";
	}
	public static class BrowserIdentStrings {
		public const string IE = "ie";
		public const string Netscape = "netscape";
		public const string FireFox = "firefox";
		public const string Opera = "opera";
		public const string Safari = "safari";
	}
	public abstract class SeparatorAdjuster {
		public void Adjust(IList sourceItems) {
			IList items = GetVisibleItems(sourceItems);
			if(items.Count == 0) return;
			RemoveDoubleSeparators(items);
			SetSeparatorVisisbility(items[0], false);
			SetSeparatorVisisbility(items[items.Count - 1], false);
			foreach(object item in items)
				Adjust(GetItems(item));
		}
		protected virtual IList GetItems(object item) {
			return new object[] { };
		}
		private void RemoveDoubleSeparators(IList items) {
			for(int i = items.Count - 1; i > 0; i--) {
				object item = items[i];
				object prevItem = items[i - 1];
				if(IsSeparator(item) && IsSeparator(prevItem)) {
					SetVisibility(item, false);
					items.RemoveAt(i);
				}
			}
		}
		protected abstract bool IsSeparator(object item);
		protected abstract bool IsVisible(object item);
		protected abstract void SetVisibility(object item, bool visible);
		private IList GetVisibleItems(IList sourceItems) {
			ArrayList items = new ArrayList();
			foreach(object item in sourceItems) {
				SetSeparatorVisisbility(item, true);
				if(IsVisible(item)) items.Add(item);
			}
			return items;
		}
		private void SetSeparatorVisisbility(object item, bool visible) {
			if(IsSeparator(item))
				SetVisibility(item, visible);
		}
	}
	public class ToolbarSeparatorAdjuster : SeparatorAdjuster {
		protected override bool IsSeparator(object item) {
			return ((ToolBarButton)item).Style == ToolBarButtonStyle.Separator;
		}
		protected override bool IsVisible(object item) {
			return ((ToolBarButton)item).Visible;
		}
		protected override void SetVisibility(object item, bool visible) {
			((ToolBarButton)item).Visible = visible;
		}
	}
	public class MenuItemSeparatorAdjuster : SeparatorAdjuster {
		protected override IList GetItems(object item) {
			return ((MenuItem)item).MenuItems;
		}
		protected override bool IsSeparator(object item) {
			return ((MenuItem)item).Text == "-";
		}
		protected override bool IsVisible(object item) {
			return ((MenuItem)item).Visible;
		}
		protected override void SetVisibility(object item, bool visible) {
			((MenuItem)item).Visible = visible;
		}
	}
	public static class StringHelper {
		public static void ValidateFormatString(string formatString) {
			string ignore = String.Format(formatString, String.Empty);
		}
		public static string GetNonEmptyValue(params string[] values) {
			foreach(string value in values)
				if(!string.IsNullOrEmpty(value))
					return value;
			return string.Empty;
		}
	}
	public static class HotkeyPrefixHelper {
		public static object PreprocessHotkeyPrefixesInObject(object original, BrickViewData data, TextExportMode textExportMode) {
			if(!(data.TableCell.TextValue is string) && data.TableCell.TextValue != null && textExportMode == TextExportMode.Value)
				return (original is double) ? original : data.TableCell.TextValue;
			if(original is string)
				return PreprocessHotkeyPrefixesInString((string)original, data.Style);
			if(original is ITextLayoutTable)
				PreprocessHotkeyPrefixesInTextLayoutTable(((ITextLayoutTable)original), data.Style);
			return original;
		}
		public static string PreprocessHotkeyPrefixesInString(string original, BrickStyle style) {
			if(style == null)
				return original;
			return PreprocessHotkeyPrefixesInString(original, style.StringFormat.HotkeyPrefix);
		}
		public static void PreprocessHotkeyPrefixesInTextLayoutTable(ITextLayoutTable table, BrickStyle style) {
			if(table == null)
				return;
			int rowCount = table.Count;
			for(int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				table[rowIndex] = PreprocessHotkeyPrefixesInString(table[rowIndex], style);
			}
		}
		public static string PreprocessHotkeyPrefixesInString(string original, HotkeyPrefix hotkeyPrefix) {
			if(hotkeyPrefix == HotkeyPrefix.None || String.IsNullOrEmpty(original))
				return original;
			StringBuilder resultBuilder = new StringBuilder();
			int length = original.Length;
			const int NormalState = 0;
			const int LastAmpersandState = 1;
			int state = NormalState;
			for(int i = 0; i < length; i++) {
				char currentChar = original[i];
				switch(state) {
					case NormalState:
						if(currentChar == '&') {
							state = LastAmpersandState;
						}
						else {
							resultBuilder.Append(currentChar);
						}
						break;
					case LastAmpersandState:
						state = NormalState;
						resultBuilder.Append(currentChar);
						break;
				}
			}
			return resultBuilder.ToString();
		}
	}
	public class ResourceImages {
		public static readonly Image Triangle = ResourceImageHelper.CreateImageFromResources("DevExpress.Printing.Core.Images.Triangle.gif", System.Reflection.Assembly.GetExecutingAssembly());
	}
	public class SmartConvertHelper : ConvertHelper {
		SimpleConverter converter;
		protected override SimpleConverter Converter { get { return converter; } set { converter = value; } }
		public SmartConvertHelper() {
		}
	}
	public class DialogRunner {
		static DialogRunner instance = new DialogRunner();
		public static DialogRunner Instance {
			get { return instance; }
			set { instance = value; }
		}
		private class DummyWin32Window : IWin32Window {
			IntPtr handle = IntPtr.Zero;
			IntPtr IWin32Window.Handle {
				get { return handle; }
			}
			public DummyWin32Window(IntPtr handle) {
				this.handle = handle;
			}
		}
		public static DialogResult ShowDialog(CommonDialog dlg) {
			return ShowDialog(dlg, null);
		}
		public static DialogResult ShowDialog(System.Windows.Forms.Form form) {
			return ShowDialog(form, (IWin32Window)null);
		}
		public static DialogResult ShowDialog(System.Windows.Forms.Form form, IWin32Window owner) {
			if(owner == null)
				owner = instance.GetDefaultOwnerWindow();
			return instance.RunDialog(form, owner);
		}
		public static DialogResult ShowDialog(System.Windows.Forms.Form form, IServiceProvider provider) {
			return instance.RunDialog(form, provider);
		}
		public static DialogResult ShowDialog(CommonDialog dlg, IWin32Window owner) {
			if(owner == null)
				owner = instance.GetDefaultOwnerWindow();
			return instance.RunDialog(dlg, owner);
		}
		public static IWin32Window GetOwnerWindow() {
			IntPtr hwnd = Win32.GetActiveWindow();
			return hwnd != IntPtr.Zero ? new DummyWin32Window((IntPtr)hwnd) : null;
		}
		protected virtual IWin32Window GetDefaultOwnerWindow() {
			return GetOwnerWindow();
		}
		protected virtual DialogResult RunDialog(CommonDialog dialog, IWin32Window ownerWindow) {
			FileDialog fileDialog = dialog as FileDialog;
			if(fileDialog != null)
				fileDialog.RestoreDirectory = true;
			return dialog.ShowDialog(ownerWindow);
		}
		protected virtual DialogResult RunDialog(System.Windows.Forms.Form form, IWin32Window ownerWindow) {
			return form.ShowDialog(ownerWindow);
		}
		protected virtual DialogResult RunDialog(System.Windows.Forms.Form form, IServiceProvider provider) {
			System.Windows.Forms.Design.IUIService serv = provider.GetService(typeof(System.Windows.Forms.Design.IUIService)) as System.Windows.Forms.Design.IUIService;
			if(serv != null)
				return serv.ShowDialog(form);
			form.StartPosition = FormStartPosition.CenterScreen;
			return form.ShowDialog();
		}
	}
	public class PagePreviewPainterBase : IDisposable {
		protected const int padding = 10;
		protected const int shadowWidth = 3;
		protected Brush grayBrush = new SolidBrush(Color.Gray);
		protected Brush whiteBrush = new SolidBrush(Color.White);
		protected Brush blackBrush = new SolidBrush(Color.Black);
		protected Pen borderPen = new Pen(Color.DarkCyan);
		public virtual void Dispose() {
			grayBrush.Dispose();
			whiteBrush.Dispose();
			blackBrush.Dispose();
			borderPen.Dispose();
		}
		protected virtual void DrawPage(Graphics gr, int w, int h) {
			gr.FillRectangle(grayBrush, 0, 0, w, h);
			Rectangle pageRect = new Rectangle(padding, padding, w - 2 * padding, h - 2 * padding);
			Rectangle shadowRect = pageRect;
			shadowRect.Offset(shadowWidth, shadowWidth);
			gr.FillRectangle(blackBrush, shadowRect);
			gr.DrawRectangle(borderPen, shadowRect);
			gr.FillRectangle(whiteBrush, pageRect);
			gr.DrawRectangle(borderPen, pageRect);
		}
		protected virtual void DrawImage(Graphics gr, int w, int h) {
			DrawPage(gr, w, h);
		}
		public void GenerateImage(PictureBox pic) {
			int width = pic.ClientRectangle.Width;
			int height = pic.ClientRectangle.Height;
			Bitmap bmp = new Bitmap(width, height);
			Graphics gr = Graphics.FromImage(bmp);
			using(gr) {
				DrawImage(gr, width, height);
			}
			pic.Image = bmp;
		}
	}
	public class ZoomEditController {
		int minZoomFactor;
		int maxZoomFactor;
		string zoomStringFormat;
		public ZoomEditController(int minZoomFactor, int maxZoomFactor, string zoomStringFormat) {
			this.minZoomFactor = minZoomFactor;
			this.maxZoomFactor = maxZoomFactor;
			this.zoomStringFormat = zoomStringFormat;
		}
		public bool IsValidZoomValue(object value, ref string message) {
			if(!IsValidInput(value)) {
				message = PreviewLocalizer.GetString(PreviewStringId.Msg_InvalidMeasurement);
				return false;
			}
			if(!IsValidRange(value)) {
				message = string.Format(PreviewLocalizer.GetString(PreviewStringId.Msg_IncorrectZoomFactor), minZoomFactor, maxZoomFactor);
				return false;
			}
			return true;
		}
		public string GetDigits(object input) {
			return Regex.Replace(input.ToString(), @"[^0123456789]*", "");
		}
		public float GetZoomValue(object input) {
			return Convert.ToSingle(GetDigits(input)) / 100F;
		}
		bool IsValidInput(object input) {
			return Regex.IsMatch(input.ToString(), @"^[ ]*[0123456789]+[ ]*[%]?[ ]*$");
		}
		bool IsValidRange(object input) {
			int zoomFactor;
			return int.TryParse(GetDigits(input), out zoomFactor) && 
				minZoomFactor <= zoomFactor && zoomFactor <= maxZoomFactor;
		}
	}
	public static class ProcessLaunchHelper {
		public static Process StartProcess(string path) {
			return StartProcess(path, true);
		}
		public static Process StartProcess(string path, bool waitForInputIdle) {
			Process process = new Process();
			try {
				process.StartInfo.FileName = path;
				process.Start();
				if(waitForInputIdle)
					process.WaitForInputIdle();
			}
			catch { }
			return process;
		}
	}
	public static class IEnumerableExtensions {
		public static IEnumerable<T> ConvertAll<T>(this IEnumerable en, Converter<object, T> converter) {
			foreach(object o in en)
				yield return converter(o);
		}
		public static IEnumerable<TOutput> ConvertAll<TInput, TOutput>(this IEnumerable<TInput> en, Converter<TInput, TOutput> converter) {
			foreach(TInput o in en)
				yield return converter(o);
		}
		public static void ForEach<T>(this IEnumerable<T> en, Action<T> action) {
			foreach(var item in en) {
				action(item);
			}
		}
	}
	public static class PrintingSettings {
		static bool verticalContentSplittingNewBehavior = false;
		static bool multiColumnDownThenAcrossNewBehavior = true;
		static bool allowCustomUrlSchema = true;
		static bool newExcelExport = true;
		static bool useNewSingleFileRtfExport = true;
		static bool passPdfDrawingExceptions = false;
		[Obsolete]
		public static bool MultiColumnDownThenAcrossNewBehavior {
			get { return multiColumnDownThenAcrossNewBehavior; }
			set { multiColumnDownThenAcrossNewBehavior = value; }
		}
		public static bool VerticalContentSplittingNewBehavior {
			get { return verticalContentSplittingNewBehavior; }
			set { verticalContentSplittingNewBehavior = value; }
		}
		public static bool AllowCustomUrlScheme {
			get { return allowCustomUrlSchema; }
			set { allowCustomUrlSchema = value; }
		}
		public static bool NewExcelExport {
			get { return newExcelExport; }
			set { newExcelExport = value; }
		}
		public static bool UseNewSingleFileRtfExport {
			get { return useNewSingleFileRtfExport; }
			set { useNewSingleFileRtfExport = value; }
		}
		public static bool PassPdfDrawingExceptions {
			get { return passPdfDrawingExceptions; }
			set { passPdfDrawingExceptions = value; }
		}
	}
}
namespace DevExpress.XtraPrinting.Design {
	public class MySite : ISite {
		IServiceProvider sp;
		IComponent comp;
		public MySite(IServiceProvider sp, IComponent comp) {
			this.sp = sp;
			this.comp = comp;
		}
		IComponent ISite.Component {
			get { return comp; }
		}
		IContainer ISite.Container {
			get {
				return sp.GetService(typeof(IContainer)) as IContainer;
			}
		}
		bool ISite.DesignMode {
			get { return false; }
		}
		string ISite.Name {
			get { return null; }
			set { }
		}
		object IServiceProvider.GetService(Type t) {
			return (sp != null) ? sp.GetService(t) : null;
		}
	}
}
namespace DevExpress.XtraPrinting.Preview {
	using System.Threading;
	using DevExpress.XtraPrinting.Native;
	public interface IWaitIndicator {
		object Show(string description);
		bool Hide(object result);
	}
	public interface ICancellationService {
		event EventHandler StateChanged;
		CancellationTokenSource TokenSource { get; }
	}
	public interface IBackgroundService {
		void PerformAction();
	}
	public static class WaitIndicatorExtensions {
		public static object TryShow(this IWaitIndicator serv, string description) {
			return serv != null ? serv.Show(description) : null;
		}
		public static bool TryHide(this IWaitIndicator serv, object result) {
			return serv != null ? serv.Hide(result) : false;
		}
	}
	public static class CancellationServiceExtensions {
		public static bool CanBeCanceled(this ICancellationService serv) {
			return serv != null && serv.TokenSource != null && CanBeCanceled(serv.TokenSource.Token);
		}
		static bool CanBeCanceled(CancellationToken token) {
			return token.CanBeCanceled && !token.IsCancellationRequested;
		}
		public static bool IsCancellationRequested(this ICancellationService serv) {
			return serv != null && serv.TokenSource != null && serv.TokenSource.IsCancellationRequested;
		}
		public static bool TryRegister(this ICancellationService serv, Action callback, bool useSynchronizationContext) {
			if(serv != null && serv.TokenSource != null) {
				serv.TokenSource.Token.Register(callback, useSynchronizationContext);
				return true;
			}
			return false;
		}
	}
	public class CancellationService : ICancellationService, IDisposable {
		public event EventHandler StateChanged;
		void RaiseStateChanged() {
			if(StateChanged != null) StateChanged(this, EventArgs.Empty);
		}
		CancellationTokenSource tokenSource;
		public CancellationTokenSource TokenSource {
			get {
				return tokenSource;
			}
		}
		public void ResetTokenSource() {
			DisposeTokenSource();
			tokenSource = new CancellationTokenSource();
			tokenSource.Token.Register(RaiseStateChanged);
			RaiseStateChanged();
		}
		public void DisposeTokenSource() {
			if(tokenSource != null) {
				tokenSource.Cancel();
				tokenSource.Dispose();
				tokenSource = null;
			}
		}
		void IDisposable.Dispose() {
			DisposeTokenSource();
		}
	}
	public class FormLayout : IXtraSerializable, IDisposable {
		Form form = null;
		Rectangle bounds = Rectangle.Empty;
		FormWindowState windowState = FormWindowState.Normal;
		bool lockSaving;
		public FormLayout(Form form) {
			this.form = form;
			SubscribeEvents(form);
		}
		[XtraSerializableProperty()]
		public Rectangle Bounds {
			get { return bounds; }
			set { bounds = value; }
		}
		[XtraSerializableProperty()]
		public FormWindowState FormWindowState {
			get { return windowState; }
			set { windowState = value; }
		}
		void SubscribeEvents(Form form) {
			if(form == null)
				return;
			form.SizeChanged += new EventHandler(form_SizeChanged);
			form.LocationChanged += new EventHandler(form_LocationChanged);
		}
		void UnsubscribeEvents(Form form) {
			if(form == null)
				return;
			form.SizeChanged -= new EventHandler(form_SizeChanged);
			form.LocationChanged -= new EventHandler(form_LocationChanged);
		}
		void SaveFormBounds() {
			if(form == null || lockSaving)
				return;
			if(form.WindowState == FormWindowState.Normal)
				bounds = form.Bounds;
		}
		void form_LocationChanged(object sender, EventArgs e) {
			SaveFormBounds();
		}
		void form_SizeChanged(object sender, EventArgs e) {
			SaveFormBounds();
		}
		public virtual void SaveFormLayout() {
			if(form == null) 
				return;
			windowState = form.WindowState;
		}
		public virtual void RestoreFormLayout() {
			if(form == null) 
				return;
			lockSaving = true;
			try {
				if(!bounds.IsEmpty) {
					AdjustBoundsByVisibleScreen();
					form.Bounds = bounds;
				}
			form.WindowState = windowState;
		}
			finally {
				lockSaving = false;
			}
		}
		void AdjustBoundsByVisibleScreen() {
			Rectangle lastScreenBounds = Screen.GetBounds(bounds);
			if(bounds.X > lastScreenBounds.Right) {
				int missingScreensCount = bounds.X / lastScreenBounds.Width;
				bounds.X = bounds.X - lastScreenBounds.Width * missingScreensCount;
			}
			if(bounds.Y > lastScreenBounds.Bottom) {
				int missingScreensCount = bounds.Y / lastScreenBounds.Height;
				bounds.Y = bounds.Y - lastScreenBounds.Height * missingScreensCount;
			}
		}
		~FormLayout() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(form != null) {
					UnsubscribeEvents(form);
					form = null;
				}
			}
		}
		#region IXtraSerializable members
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		#endregion
	}
}
namespace DevExpress.Utils {
	public class NaturalStringComparer : IComparer<string>, IDisposable { 
		struct Info {
			public string[] strs;
			public int[] ints;
			public int Length { get { return strs.Length; } }
			public static bool Eq(Info x, Info y, int i) {
				return x.strs[i] == y.strs[i];
			}
		}
		Regex regex = new Regex("(\\d+)");
		Dictionary<string, Info> cache;
		public NaturalStringComparer() {
			cache = new Dictionary<string, Info>();
		}
		public NaturalStringComparer(int cacheSize) {
			cache = new Dictionary<string, Info>(cacheSize);
		}
		public int Compare(string x, string y) {
			if(x == y) return 0;
			if(x == null) return 1;
			if(y == null) return -1;
			Info xx = Split(x);
			Info yy = Split(y);
			int minLength = Math.Min(xx.Length, yy.Length);
			for(int i = 0; i < minLength; i++) {
				if(!Info.Eq(xx, yy, i))
					return CompareStringPart(xx, yy, i);
			}
			return IntCompare(xx.Length, yy.Length);
		}
		Info Split(string x) {
			Info result;
			if(!cache.TryGetValue(x, out result)) {
				result.strs = regex.Split(x);
				result.ints = new int[result.strs.Length / 2];
				int n;
				for(int i = 1, j = 0; i < result.strs.Length; i += 2, j++) {
					int.TryParse(result.strs[i], out n);
					result.ints[j] = n;
				}
				cache[x] = result;
			}
			return result;
		}
		int CompareStringPart(Info x, Info y, int i) {
			if(i % 2 == 1) {
				int j = i / 2; 
				return IntCompare(x.ints[j], y.ints[j]);
			} else {
				return StringCompare(x.strs[i], y.strs[i]);
			}
		}
		static int IntCompare(int x, int y) {
			return x == y ? 0 : (x > y ? 1 : -1);
		}
		protected virtual int StringCompare(string x, string y) {
			return string.Compare(x, y, StringComparison.CurrentCulture);
		}
		public void Dispose() {
			cache.Clear();
			cache = null;
		}
	}
}

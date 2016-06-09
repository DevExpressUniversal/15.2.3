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

using System.Xml;
using System.Windows.Forms;
using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using System.Collections;
namespace DevExpress.Tutorials {
	public class SimpleXmlReaderBase {
		private XmlNodeReader GetXmlNodeReader(string xmlFileName, Type type) {
			if(xmlFileName == null) return null;
			XmlDocument doc = new XmlDocument();
			try {
				if(type == null) {
					string realXmlFileName = FilePathUtils.FindFilePath(xmlFileName, false);
					if(realXmlFileName == string.Empty) {
						Stream st = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(xmlFileName);
						return null;
					}
					using(FileStream stream = File.OpenRead(realXmlFileName)) {
						doc.Load(realXmlFileName);
					}
				} else {
					using(Stream stream = type.Assembly.GetManifestResourceStream(xmlFileName)) {
						doc.Load(stream);
					}
				}
			} catch(Exception e) {
				MessageBox.Show(e.Message + ", " + e.GetType().ToString());
				return null;
			}
			return new XmlNodeReader(doc);
		}
		public bool ProcessXml(string xmlFileName, Type type) {
			XmlNodeReader reader = GetXmlNodeReader(xmlFileName, type);
			if(reader == null) return false;
			while(reader.Read()) {
				switch(reader.NodeType) {
					case XmlNodeType.Element :
						ProcessStartElement(reader);
						break;
					case XmlNodeType.EndElement :
						ProcessEndElement(reader);
						break;
					case XmlNodeType.Text :
						ProcessText(reader);
						break;
				}
			}
			reader.Close();
			return true;
		}
		protected virtual void ProcessStartElement(XmlNodeReader reader) {
		}
		protected virtual void ProcessEndElement(XmlNodeReader reader) {
		}
		protected virtual void ProcessText(XmlNodeReader reader) {
		}
		protected string RemoveLineBreaks(string s) {
			string temp = s.Replace("\n", string.Empty);
			temp = temp.Replace("\r\n", string.Empty);
			temp = temp.Replace(Environment.NewLine, string.Empty);
			return temp;
		}
	}
	public class FilePathUtils {
		public static string FindFilePath(string path, bool showError) {
			string s = "\\";
			for(int i = 0 ; i <= 10 ; i++) {
				if(File.Exists(Application.StartupPath + s + path)) 
					return Application.StartupPath + s + path;
				else 
					s += "..\\";
			} 
			if(showError)
				MessageBox.Show("File " + path + " is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
			return string.Empty;
		}
	}
	public class ControlIterator {
		Control startControl;
		public ControlIterator(Control startControl) {
			this.startControl = startControl;
		}
		public void ProcessControls() {
			InitProcessing();
			ProcessControls(startControl);
		}
		private void ProcessControls(Control startControl) {
			foreach(Control control in startControl.Controls) {
				ProcessControl(control);
				if(control.HasChildren)
					ProcessControls(control);
			}
		}
		protected virtual void ProcessControl(Control control) {
		}
		protected virtual void InitProcessing() {
		}
	}
	public class ControlImageCapturer {
		[System.Runtime.InteropServices.DllImport("USER32.dll")]
		internal static extern IntPtr GetDC(IntPtr dc);
		[System.Runtime.InteropServices.DllImport("USER32.dll")]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[System.Runtime.InteropServices.DllImport("USER32.dll")]
		internal static extern IntPtr GetDesktopWindow();
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern bool DeleteObject(IntPtr hObject);
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern IntPtr CreateSolidBrush(int color);
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		internal static extern IntPtr CreatePatternBrush(IntPtr hBitmap);
		public static Bitmap GetControlBitmap(Control control, Bitmap pattern) {
			int width = control.Width;
			int height = control.Height;
			if(control is Form) {
				width = control.ClientRectangle.Width;
				height = control.ClientRectangle.Height;
			}
			IntPtr hdc = GetDC(control.Handle);
			IntPtr compDC = CreateCompatibleDC(hdc);
			IntPtr compHBmp = CreateCompatibleBitmap(hdc, width, height);
			IntPtr prev = SelectObject(compDC, compHBmp);
			IntPtr brush = IntPtr.Zero, prevBrush = IntPtr.Zero;
			if(pattern != null) {
				brush = CreatePatternBrush(pattern.GetHbitmap());
				prevBrush = SelectObject(compDC, brush);
			}
			Point pt = new Point(0, 0);
			BitBlt(compDC, 0, 0, width, height, hdc, pt.X, pt.Y, 0x00C000CA);
			SelectObject(compDC, prev);
			if(prevBrush != IntPtr.Zero)
				SelectObject(compDC, prevBrush);
			ReleaseDC(control.Handle, hdc);
			NativeMethods.DeleteDC(compDC);
			Bitmap bmp = Bitmap.FromHbitmap(compHBmp);
			DeleteObject(compHBmp);
			if(brush != IntPtr.Zero)
				DeleteObject(brush);
			return bmp;
		}
	}
	public class ColorUtils {
		[DllImport("user32.dll")]
		internal static extern int GetSysColor(int colorIndex);
		public static Color GetGradientActiveCaptionColor() {
			int COLOR_GRADIENTACTIVECAPTION = 27;
			int colorValue = GetSysColor(COLOR_GRADIENTACTIVECAPTION);
			return ColorTranslator.FromWin32(colorValue);
		}
	}
	public class CodeLineShifter {
		public static string ShiftLeftToFit(string code) {
			if(code == string.Empty) return code;
			StringReader reader = new StringReader(code);
			string currentString = reader.ReadLine();
			int minWhiteSpaceCount = GetLeftWhiteSpaceCount(currentString);
			while((currentString = reader.ReadLine()) != null) {
				if(IsIgnoreString(currentString)) continue;
				if(minWhiteSpaceCount > GetLeftWhiteSpaceCount(currentString))
					minWhiteSpaceCount = GetLeftWhiteSpaceCount(currentString);
			}
			return ShiftLeftBy(code, minWhiteSpaceCount);
		}
		private static bool IsIgnoreString(string s) {
			if(s == string.Empty) return true;
			if(s.StartsWith("using")) return true;
			if(s.StartsWith("Imports")) return true;
			return false;
		}
		private static int GetLeftWhiteSpaceCount(string s) {
			int oldLength = s.Length;
			string newS = s.TrimStart(null);
			return oldLength - newS.Length;
		}
		public static string ShiftLeftBy(string code, int offset) {
			StringReader reader = new StringReader(code);
			string currentString;
			string result = string.Empty;
			while((currentString = reader.ReadLine()) != null) {
				if(currentString != string.Empty)
					result += currentString.Substring(offset);
				result += "\r\n";
			}
			return result;
		}
	}
	public class ControlUtils {
		public static void CenterControlInParent(Control ctrl) {
			CenterControlInParentCustom(ctrl, true, true);
		}
		public static void CenterControlInParentCustom(Control ctrl, bool centerHorz, bool centerVert) {
			Control parent  = ctrl.Parent;
			if(centerHorz)
				ctrl.Left = (parent.Width - ctrl.Width) / 2;
			if(centerVert)
				ctrl.Top = (parent.Height - ctrl.Height) / 2;
		}
		public static void HorzAlignControlInParent(Control ctrl, int boundsOffset) {
			ctrl.Left = boundsOffset;
			ctrl.Width = ctrl.Parent.Width - boundsOffset * 2;
		}
		public static void VertAlignControlInParent(Control ctrl, int boundsOffset) {
			ctrl.Top = boundsOffset;
			ctrl.Height = ctrl.Parent.Height - boundsOffset * 2;
		}
		public static void AlignControlInParent(Control ctrl, int boundsOffset) {
			HorzAlignControlInParent(ctrl, boundsOffset);
			VertAlignControlInParent(ctrl, boundsOffset);
		}
		public static void UpdateLabelHeight(Label label) {
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Near;
			format.FormatFlags &= ~StringFormatFlags.NoWrap;
			SizeF labelSize = SizeF.Empty;
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				labelSize = ginfo.Graphics.MeasureString(GetCorrectNewLineString(label.Text), label.Font, label.Width, format);
			}
			finally {
				ginfo.ReleaseGraphics();
			}
			label.Height = Convert.ToInt32(labelSize.Height);
		}
		static string GetCorrectNewLineString(string s) {
			int index = s.IndexOf("\r");
			if(index != -1 && s.IndexOf("\r\n") != index)
				return s.Replace("\r", "\r\n");
			return s;
		}
		private static System.Drawing.Rectangle GetCurrentScreenBounds(Point location) {
			Rectangle rect = SystemInformation.WorkingArea;
			if(SystemInformation.MonitorCount > 1)
				rect = Screen.FromPoint(location).Bounds;
			return rect;
		}
		public static void UpdateFrmToFitScreen(Form frm) {
			Rectangle currentScreenBounds = GetCurrentScreenBounds(frm.Location);
			if(frm.Top + frm.Height > currentScreenBounds.Top + currentScreenBounds.Height) 
				frm.Top = currentScreenBounds.Top + currentScreenBounds.Height - frm.Height;
			if(frm.Left + frm.Width > currentScreenBounds.X + currentScreenBounds.Width)
				frm.Left = currentScreenBounds.X + currentScreenBounds.Width - frm.Width;
		}
		public static bool ControlHasInvisibleParent(Control control) {
			Control tmpCtrl = control;
			while(tmpCtrl.Parent != null) {
				if(!tmpCtrl.Parent.Visible)
					return true;
				tmpCtrl = tmpCtrl.Parent;
			}
			return false;
		}
		public static Control GetControlByName(string name, Control parent) {
			FindControlByNameIterator iterator = new FindControlByNameIterator(name, parent);
			iterator.ProcessControls();
			return iterator.ControlResult;
		}
	}
	public class FindControlByNameIterator : ControlIterator {
		string name;
		Control controlResult;
		public FindControlByNameIterator(string name, Control startControl) : base(startControl) {
			this.name = name;
			this.controlResult = null;
		}
		protected override void ProcessControl(Control control) {
			if(control.Name == name)
				controlResult = control;
		}
		public Control ControlResult { get { return controlResult; } }
	}
	public class TutorialHelper {
		const string endDescription = "[end description]";
		public static void InitFont(DevExpress.XtraEditors.ImageListBoxControl ilb) {
			int imageWidth = 20; int imageHeight = 16; int offset = 1;
			ImageList fontImageList = new ImageList();
			fontImageList.ImageSize = new Size(imageWidth, imageHeight);
			Rectangle rect = new Rectangle(offset, offset, imageWidth - offset * 2, imageHeight - offset * 2);
			ilb.BeginUpdate();
			try {
				ilb.Items.Clear();
				ilb.ImageList = fontImageList;
				int imageCount = 0;
				for (int i = 0; i < FontFamily.Families.Length; i++)  {
					try {
						Font fontSample = new Font(FontFamily.Families[i].Name, 8);
						string fontName = FontFamily.Families[i].Name; 
						Image fontImage = new Bitmap(imageWidth, imageHeight);
						using(Graphics g = Graphics.FromImage(fontImage)) {
							g.FillRectangle(Brushes.White, rect);
							g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
							g.DrawString("abc", fontSample, Brushes.Black, offset, offset);
							g.DrawRectangle(Pens.Black, rect);
						}
						fontImageList.Images.Add(fontImage);
						ilb.Items.Add(fontName, imageCount++);
					} catch {}
				}
			} finally { ilb.CancelUpdate();; }
		}
		public static string[] GetDescriptionsFromResource(string name, System.Reflection.Assembly asm) {
			Stream stream = asm.GetManifestResourceStream(name);
			StreamReader file = new StreamReader(stream);
			ArrayList strings = new ArrayList();
			string composedString = "";
			while(!file.EndOfStream) {
				string s = file.ReadLine();
				if(composedString != "" && s != endDescription)
					composedString += "\r\n";
				if(s != endDescription)
					composedString += s;
				else {
					strings.Add(composedString);
					composedString = "";
				}
			}
			file.Close();
			return (string[])strings.ToArray(typeof(string));
		}
	}
}

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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using System.Xml;
using System.IO;
using System.Text;
namespace DevExpress.Tutorials {
	public class WhatsThisInfoReader : SimpleXmlReaderBase {
		WhatsThisController controller;
		string currentNodeName;
		string controlName, windowCaption, description, code, memberlist, dtImage;
		string hintText;
		const string NodeControl = "controlentry";
		const string NodeHint = "hint";
		const string NodeDescription = "description";
		const string NodeName = "name";
		const string NodeWindowCaption = "windowcaption";
		const string NodeMemberList = "memberlist";
		const string NodeDTImage = "dtimage";
		const string NodeHintText = "hinttext";
		const string NodeNewLine = "newline";
		public WhatsThisInfoReader(WhatsThisController controller) {
			this.controller = controller;
			this.currentNodeName = string.Empty;
			ResetData();
		}
		private void ResetData() {
			controlName = windowCaption = description = code = memberlist = dtImage = string.Empty;
			hintText = string.Empty;
		}
		protected override void ProcessStartElement(XmlNodeReader reader) {
			if(reader.Name == NodeNewLine) return;
			currentNodeName = reader.Name;
		}
		protected override void ProcessEndElement(XmlNodeReader reader) {
			if(reader.Name == NodeControl) {
				WhatsThisParams popupInfo = new WhatsThisParams(windowCaption, description, memberlist, code, dtImage);
				controller.Collection.Add(new WhatsThisControlEntry(controlName, popupInfo));
				ResetData();
			}
			if(reader.Name == NodeNewLine) {
				description += Environment.NewLine;
				hintText += Environment.NewLine;
			}
			if(reader.Name == NodeHint) {
				controller.Hints.Add(controlName, hintText);
				ResetData();
			}
		}
		protected override void ProcessText(XmlNodeReader reader) {
			string readValue = RemoveLineBreaks(reader.Value);
			switch(currentNodeName) {
				case NodeWindowCaption: 
					windowCaption = readValue;
					break;
				case NodeDescription:
					description += readValue;
					break;
				case NodeMemberList:
					memberlist = readValue;
					break;
				case NodeDTImage:
					dtImage = readValue;
					break;
				case NodeName:
					controlName = readValue;
					break;
				case NodeHintText:
					hintText += readValue;
					break;
			}
		}
	}
	public class ControlCodeEntry {
		string controlName, code;
		public ControlCodeEntry(string controlName, string code) {
			this.controlName = controlName;
			this.code = code;
		}
		public string ControlName { get { return controlName; } }
		public string Code {
			get { return code; }
			set { code = value; }
		}
	}
	public class ControlCodes : CollectionBase {
		public ControlCodeEntry this[int index] {
			get { return List[index] as ControlCodeEntry; }
		}
		public string GetCodeByControlName(string controlName) {
			int index = IndexOf(controlName);
			if(index == -1) return string.Empty;
			return this[index].Code;
		}
		public void AddListener(string controlName) {
			if(ContainsListener(controlName)) return;
			List.Add(new ControlCodeEntry(controlName, string.Empty));
		}
		public void RemoveListener(string controlName) {
			if(!ContainsListener(controlName)) return;
			RemoveAt(IndexOf(controlName));
		}
		private int IndexOf(string controlName) {
			for(int i = 0; i < Count; i++) {
				if(this[i].ControlName == controlName)
					return i;
			}
			return -1;
		}
		private bool ContainsListener(string controlName) {
			if(IndexOf(controlName) == -1) return false;
			return true;
		}
	}
	public class WhatsThisSourceFileReader {
		WhatsThisController controller;
		string commentString;
		ControlCodes listenerControlCodes;
		string[] separatedCodeFileNames;
	  bool skipLine;
	  static string SkipTag = "skip";
		public WhatsThisSourceFileReader(WhatsThisController controller, string codeFileNames, string commentString) {
			this.controller = controller;
			if(codeFileNames != null)
				this.separatedCodeFileNames = codeFileNames.Split(new char[] {';'}, 10);
			else 
				this.separatedCodeFileNames = new string[] { string.Empty };
			this.commentString = commentString;
			this.listenerControlCodes = new ControlCodes();
		 this.skipLine = false;
			controller.Collection.ResetControlCodes();
		}
		public void ProcessFiles() {
			foreach(string codeFileName in separatedCodeFileNames)
				ProcessFile(codeFileName);
		}
		private void ProcessFile(string codeFileName) {
			if(codeFileName == string.Empty) return;
			string realFilePath = FilePathUtils.FindFilePath(codeFileName, false);
			if(!File.Exists(realFilePath)) return;
			FileStream file = File.OpenRead(realFilePath);
			byte[] bytesRead = new byte[file.Length];
			file.Read(bytesRead, 0, bytesRead.Length);
			string fileString = Encoding.ASCII.GetString(bytesRead);
			StringReader reader = new StringReader(fileString);
			ProcessFileString(reader);
			file.Close();
		}
		private void ProcessFileString(StringReader reader) {
			string currentString = string.Empty;
			while(currentString != null) {
				currentString = reader.ReadLine();
				if(StringCommented(currentString)) {
					ProcessCommentedString(currentString);
					continue;
				}
				if(skipLine)
					continue;
				foreach(ControlCodeEntry entry in listenerControlCodes)
					AddCodeLine(entry, currentString);
			}
		}
		private bool StringCommented(string s) {
			if(s == null) return false;
			string tmp = s.TrimStart(null);
			if(tmp.StartsWith(commentString) 
				&& tmp.Length > 1 && tmp[1] != '~') 
				return true;
			return false;
		}
		private void AddCodeLine(ControlCodeEntry entry, string currentString) {
			entry.Code += currentString + "\r\n";
		}
	  private string GetSkipTag(bool opening) {
		 string start = "</";
		 if(opening)
			start = "<";
		 return start + SkipTag + ">";
	  }
		private void ProcessCommentedString(string s) {
		 CheckSkipLine(s);
		 foreach(WhatsThisControlEntry entry in controller.Collection) {
				if(s.IndexOf(entry.OpeningTag) != -1)
					listenerControlCodes.AddListener(entry.ControlName);
				if(s.IndexOf(entry.ClosingTag) != -1) {
					WhatsThisParams popupInfo = controller.Collection[entry.ControlName].PopupInfo;
					if(popupInfo.Code != string.Empty) 
						popupInfo.Code += commentString + "..\r\n"; 
					popupInfo.Code += CodeLineShifter.ShiftLeftToFit(listenerControlCodes.GetCodeByControlName(entry.ControlName));
					listenerControlCodes.RemoveListener(entry.ControlName);
				}
		 }
	  }
	  private void CheckSkipLine(string s) {
		 if(s.IndexOf(GetSkipTag(true)) != -1)
			skipLine = true;
		 if(s.IndexOf(GetSkipTag(false)) != -1)
			skipLine = false;
	  }
	}
	public class WhatsThisHintEntry {
		string controlName;
		string hintText;
		public WhatsThisHintEntry(string controlName, string hintText) {
			this.controlName = controlName;
			this.hintText = hintText;
		}
		public string ControlName { get { return controlName; } }
		public string HintText { get { return hintText; } }
	}
	public class WhatsThisHintCollection : CollectionBase {
		public WhatsThisHintEntry this[int index] {
			get { return List[index] as WhatsThisHintEntry; }
		}
		public void Add(string controlName, string hintText) {
			List.Add(new WhatsThisHintEntry(controlName, hintText));
		}
		public string GetHintTextByControl(string controlName) {
			foreach(WhatsThisHintEntry entry in this) {
				if(entry.ControlName == controlName)
					return entry.HintText;
			}
			return string.Empty;
		}
	}
	public class WhatsThisControlEntry {
		string controlName;
		WhatsThisParams popupInfo;
		Bitmap controlBitmap;
		Rectangle controlScreenRect;
		Control control;
		public WhatsThisControlEntry(string controlName, WhatsThisParams popupInfo) {
			this.controlName = controlName;
			this.popupInfo = popupInfo;
			this.controlBitmap = null;
			this.controlScreenRect = Rectangle.Empty;
			this.control = null;
		}
		public string ControlName { get { return controlName; } }
		public WhatsThisParams PopupInfo { get { return popupInfo; } }
		public Control Control {
			get { return control; }
			set { control = value; }
		}
		public Bitmap ControlBitmap {
			get { return controlBitmap; }
			set { controlBitmap = value; }
		}
		public Rectangle ControlScreenRect {
			get { return controlScreenRect; }
			set { controlScreenRect = value; }
		}
		public string OpeningTag {
			get { return "<" + controlName + ">"; }
		}
		public string ClosingTag {
			get { return "</" + controlName + ">"; }
		}
	}
	public class WhatsThisControlsCollection : CollectionBase {
		public WhatsThisControlsCollection() {
		}
		public void Add(WhatsThisControlEntry entry) {
			List.Add(entry);
		}
		public WhatsThisControlEntry this[int index] {
			get { return List[index] as WhatsThisControlEntry; }
		}
		public WhatsThisControlEntry this[string controlName] {
			get { 
				foreach(WhatsThisControlEntry entry in this) {
					if(entry.ControlName == controlName)
						return entry;
				}
				return null;
			}
		}
		public bool HasControl(string controlName) {
			foreach(WhatsThisControlEntry entry in this) {
				if(entry.ControlName.Equals(controlName))
					return true;
			}
			return false;
		}
		public WhatsThisParams PopupInfoByControlName(string controlName) {
			foreach(WhatsThisControlEntry entry in this) {
				if(entry.ControlName.Equals(controlName))
					return entry.PopupInfo;
			}
			return null;
		}
		public void SetControlBitmapBounds(Bitmap bmp, Control control) {
			foreach(WhatsThisControlEntry entry in this) {
				if(entry.ControlName.Equals(control.Name)) {
					entry.ControlScreenRect = control.RectangleToScreen(new Rectangle(0, 0, control.Width, control.Height));
					entry.ControlBitmap = bmp;
				}
			}
		}
		public void UpdateControlRects(int offsetX, int offsetY) {
			foreach(WhatsThisControlEntry entry in this)
				entry.ControlScreenRect = new Rectangle(entry.ControlScreenRect.Left + offsetX, entry.ControlScreenRect.Y + offsetY, entry.ControlScreenRect.Width, entry.ControlScreenRect.Height);
		}
		public void ResetControlCodes() {
			foreach(WhatsThisControlEntry entry in this)
				entry.PopupInfo.Code = string.Empty;
		}
	}
	public interface IWhatsThisProvider {
		ImageShaderBase CurrentShader { get; }
		bool HintVisible { get; set; }
		FormTutorialInfo TutorialInfo { get; }
		UserControl CurrentModule { get; }		
	}
	public class WhatsThisController {
		WhatsThisControlsCollection collection;
		WhatsThisControlsCollection registeredVisibleControls;
		WhatsThisHintCollection hints;
		FrmWhatsThisBase whatsThisForm;
		IWhatsThisProvider frm;
		Bitmap whatsThisModuleBitmap;
		ColoredHint coloredToolTip;
		public WhatsThisController(IWhatsThisProvider frm) {
			this.whatsThisForm = null;
			this.collection = new WhatsThisControlsCollection();
			this.registeredVisibleControls = new WhatsThisControlsCollection();
			this.hints = new WhatsThisHintCollection();
			this.frm = frm;
			this.whatsThisModuleBitmap = null;
			this.coloredToolTip = new ColoredHint();
		}
		public WhatsThisControlsCollection Collection { get { return collection; } }
		public WhatsThisControlsCollection RegisteredVisibleControls { get { return registeredVisibleControls; } }
		public WhatsThisHintCollection Hints { get { return hints; } }
		public Bitmap WhatsThisModuleBitmap { get { return whatsThisModuleBitmap; } }
		public UserControl CurrentModule { get { return frm.CurrentModule; } }
		public void UpdateWhatsThisInfo(string xmlFileName, string codeFileNames) {
			UpdateWhatsThisInfo(xmlFileName, codeFileNames, null);
		}
		public void UpdateWhatsThisInfo(string xmlFileName, string codeFileNames, Type type) {
			collection.Clear();
			hints.Clear();
			if(string.IsNullOrEmpty(xmlFileName)) return;
			WhatsThisInfoReader infoReader = new WhatsThisInfoReader(this);
			infoReader.ProcessXml(xmlFileName, type);
			AssignHintHandlers();
			WhatsThisSourceFileReader sourceReader = new WhatsThisSourceFileReader(this, codeFileNames, frm.TutorialInfo.SourceFileComment);
			sourceReader.ProcessFiles();
		}
		private void AssignHintHandlers() {
			foreach(WhatsThisHintEntry entry in Hints) {
				Control ctrl = ControlUtils.GetControlByName(entry.ControlName, frm.CurrentModule);
				if(ctrl != null) {
					ctrl.MouseEnter += new EventHandler(HintControlMouseEnter);
					ctrl.MouseLeave += new EventHandler(HintControlMouseLeave);
					PopupBaseEdit pEdit = ctrl as PopupBaseEdit;
					if(pEdit != null)
						pEdit.Popup += new EventHandler(pEdit_Popup);
				}
			}
		}
		void pEdit_Popup(object sender, EventArgs e) {
			HideHint();
		}
		private void HintControlMouseEnter(object sender, EventArgs e) {
			Control ctrl = sender as Control;
			coloredToolTip.ColoredHintText = hints.GetHintTextByControl(ctrl.Name);
		 if(coloredToolTip.ColoredHintText != string.Empty) {
			coloredToolTip.ShowAtControl(ctrl);
			frm.HintVisible = true;
		 }
		}
		private void HintControlMouseLeave(object sender, EventArgs e) {
			HideHint();
		}
	  public void HideHint() {
		 coloredToolTip.HideTip();
		 frm.HintVisible = false;
	  }
		public void UpdateRegisteredVisibleControls() {
			registeredVisibleControls.Clear();
			foreach(WhatsThisControlEntry entry in Collection) {
			Control ctrl = GetVisibleControlByName(entry.ControlName);
			if(ctrl != null) {
			   entry.Control = ctrl;
			   registeredVisibleControls.Add(entry);
			}
			}
		}
		public void UpdateWhatsThisBitmaps() {
			whatsThisModuleBitmap = ControlImageCapturer.GetControlBitmap(frm.CurrentModule, null);
			whatsThisModuleBitmap = frm.CurrentShader.ShadeBitmap(whatsThisModuleBitmap);
			WhatsThisControlImageProcessor capturer = new WhatsThisControlImageProcessor(frm.CurrentModule, this, Graphics.FromImage(whatsThisModuleBitmap));
			capturer.ProcessControls();
		}
	  public Control GetVisibleControlByName(string controlName) {
		 VisibleControlFinder finder = new VisibleControlFinder(controlName, frm.CurrentModule);
		 finder.ProcessControls();
		 return finder.Result;
	  }
	  public bool ControlExists(string controlName) {
		 ControlFinder finder = new ControlFinder(controlName, frm.CurrentModule);
		 finder.ProcessControls();
		 return finder.Exists;
	  }
		public bool IsWhatsThisInfoValid() {
			if(collection.Count == 0) 
				return false;
			int errorControlCount = 0;
			foreach(WhatsThisControlEntry entry in collection) {
				if(!ControlExists(entry.ControlName)) {
					MessageBox.Show("The control " + entry.ControlName + " doesn't exist on the current module", "What's This functionality error");
					errorControlCount++;
				}
			}
			if(errorControlCount > 0) 
				return false;
			return true;
		}
		public bool IsControlRegistered(string controlName) {
			return collection.HasControl(controlName);
		}
		public bool TryToShowWhatsThis(Control control) {
			if(control != null && control.GetType().Equals(typeof(DevExpress.XtraEditors.TextBoxMaskBox))) control = control.Parent;
			string controlName = GetControlName(control);   
			if(control == whatsThisForm) return false;
			if(whatsThisForm != null && whatsThisForm.Contains(control)) return false;
			if(!IsControlRegistered(controlName)) {
				HideCodeContainer();
				return false;
			}
			if(NeedToShowCodeContainer(controlName))
				ShowCodeContainer(control, controlName);
			return true;
		}
		bool NeedToShowCodeContainer(string controlName) {
			if(whatsThisForm == null) 
				return true;
			if(whatsThisForm.ControlName != controlName) 
				return true;
			if(!whatsThisForm.Visible && whatsThisForm.ControlName == controlName)
				return true;
			return false;
		}
		string GetControlName(Control control) {
		 #region Support for XtraBars
		 #endregion
			return control.Name;
		}
		void ShowCodeContainer(Control control, string controlName) {
			WhatsThisParams popupInfo = GetWhatsThisParams(controlName);
			if(whatsThisForm != null && whatsThisForm.Visible) HideCodeContainer();
			whatsThisForm = GetWhatsThisFormByPopupInfo(popupInfo);
			whatsThisForm.Location = control.PointToScreen(new Point(0, control.Height));
			whatsThisForm.StartPosition = FormStartPosition.Manual;
			whatsThisForm.Show(controlName, GetWhatsThisParams(controlName), frm.TutorialInfo.SourceFileType);
		}
		void HideCodeContainer() {
			if(whatsThisForm != null)
				whatsThisForm.Close();
		}
		public WhatsThisParams GetWhatsThisParams(string controlName) {
			return collection.PopupInfoByControlName(controlName);
		}
		private FrmWhatsThisBase GetWhatsThisFormByPopupInfo(WhatsThisParams popupInfo) {
			if(popupInfo.Code == string.Empty) return new FrmWhatsThisTextOnly();
			else return new FrmWhatsThis();
		}
	}
	public class WhatsThisParams {
		string fCaption, fDescription, fCode;
		string fMemberList, fDTImage;
		public WhatsThisParams(string caption, string description, string memberList, string code, string dtImage) {
			fCode = code;
			fCaption = caption;
			fDescription = description;
			fMemberList = memberList;
			fDTImage = dtImage;
		}
		public string Code { get { return fCode; } set { fCode = value; } }
		public string Caption { get { return fCaption; } }
		public string Description { get { return fDescription; } }
		public string MemberList { get { return fMemberList; } }
		public string DTImage { get { return fDTImage; } }
	}
	public class WhatsThisControlImageProcessor : ControlIterator {
		WhatsThisController controller;
		Graphics g;
		public WhatsThisControlImageProcessor(Control startControl, WhatsThisController controller, Graphics g) 
			: base(startControl) {
			this.controller = controller;
			this.g = g;
		}
		protected override void ProcessControl(Control control) {
		 if(ControlUtils.ControlHasInvisibleParent(control)) return;
			if(!controller.RegisteredVisibleControls.HasControl(control.Name)) return;
			Bitmap bmp = ControlImageCapturer.GetControlBitmap(control, null);
			Point imageLocation = controller.CurrentModule.PointToClient(control.PointToScreen(new Point(0, 0)));
			g.DrawImage(bmp, imageLocation.X, imageLocation.Y, bmp.Width, bmp.Height);
			controller.Collection.SetControlBitmapBounds(bmp, control);
		}
	}
   public class VisibleControlFinder : ControlIterator {
	  string controlName;
	  Control result;
	  public VisibleControlFinder(string controlName, Control startControl) : base(startControl) {
		 this.controlName = controlName;
		 this.result = null;
	  }
	  protected override void ProcessControl(Control control) {
		 if(control.Name == controlName && !ControlUtils.ControlHasInvisibleParent(control) && control.Visible)
			result = control;
	  }
	  public Control Result {
		 get { return result; }
	  }
   }
   public class ControlFinder : ControlIterator {
	  string controlName;
	  bool exists;
	  public ControlFinder(string controlName, Control startControl) : base(startControl) {
		 this.controlName = controlName;
		 this.exists = false;
	  }
	  protected override void ProcessControl(Control control) {
		 if(control.Name == controlName)
			exists = true;
	  }
	  public bool Exists {
		 get { return exists; }
	  }
   }
}

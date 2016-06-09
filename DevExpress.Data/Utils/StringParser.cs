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
using System.Text;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Utils.Text.Internal;
using System.Resources;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Utils.Text {
	public interface IStringImageProvider {
		Image GetImage(string id);
	}
	public class ResourceImageProvider {
		[ThreadStatic]
		static ResourceImageProvider current;
		public static ResourceImageProvider Current {
			get {
				if(current == null) current = new ResourceImageProvider();
				return current;
			}
		}
		Dictionary<string, Image> images;
		protected Dictionary<string, Image> Images {
			get {
				if (images == null) images = new Dictionary<string, Image>();
				return images;
			}
		}
		ResourceManager resourceManager;
		public ResourceManager ResourceManager {
			get {
				if(resourceManager == null) resourceManager = GetDefaultResourceManager();
				return resourceManager; 
			}
			set { resourceManager = value; }
		}
		protected ResourceManager GetDefaultResourceManager() {
#if DXPORTABLE
			return null;
#else
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
			if(asm == null) return null;
			string[] resources = asm.GetManifestResourceNames();
			foreach(string s in resources) {
				string lv = s.ToLowerInvariant();
				if(lv.EndsWith(".resources.resources") || lv.Equals("resources.resources")) {
					string resName = s.Substring(0, s.Length - ".resources".Length);
					return new ResourceManager(resName, asm);
				}
			}
			return null;
#endif
		}
		protected Image GetResourceImage(object context, string id) {
#if DXPORTABLE
			return null;
#else
			ResourceManager manager = ResourceManager;
			if(manager != null) return manager.GetObject(id) as Image;
			return null;
#endif
		}
		public Image GetImage(object context, string id) {
			if(string.IsNullOrEmpty(id) || context == null) return null;
			if(id[0] == '#') id = id.Substring(1);
			Image res;
			Images.TryGetValue(id, out res);
			if(res != null) return res;
			try {
				res = GetResourceImage(context, id);
			} catch {
			}
			if(res == null) return null;
			Images[id] = res;
			return res;
		}
	}
	public class HyperlinkSettings {
		public HyperlinkSettings() {
			Color = Color.Blue;
			Underline = true;
		}
		public Color Color { get; set; }
		public bool Underline { get; set; }
	}
	public class StringParser {
		static ColorConverter converter;
		static ColorConverter Converter {
			get {
				if (converter == null)
					converter = new ColorConverter();
				return converter;
			}
		}
		public static StringCommand ParseCommand(ref int n, string text) {
			StringCommand res = new StringCommand();
			int pos = n + 1;
			if(string.IsNullOrEmpty(text) || (text[n] != '<' && text[n] != '&')) {
				if(IsSeparator(text[n]))
					res.Command = text[n].ToString();
				return res;
			}
			int len = text.Length;
			if (len < 3) return res;
			char chCommandStart = '<';
			char chCommandEnd = '>';
			if(text[n] == '&') {
				chCommandStart = '\0';
				chCommandEnd = ';';
			}
			if(text[pos] == chCommandStart) {
				n++;
				return res;
			}
			if(text[pos] == '/') {
				pos++;
				res.SetEnd();
			}
			else {
				res.SetStart();
			}
			int commandEnd = FindCommandEnd(text, pos, len, chCommandEnd);
			if (commandEnd == -1) return res;
			string cmd = text.Substring(pos, commandEnd - pos - 1);
			if (cmd.Length < 1) return res;
			if(!CheckValidCommand(cmd, out cmd)) return res;
			res.Command = cmd;
			n = commandEnd;
			return res;
		}
		static bool CheckValidCommand(string cmd, out string cmdUpdated) {
			cmdUpdated = cmd.ToLowerInvariant();
			switch(cmdUpdated) {
				case "br":
				case "b":
				case "i":
				case "u":
				case "nbsp":
				case "s": return true;
			}
			if(cmdUpdated.StartsWith("href")) {
				int index = cmd.IndexOf('=');
				if(index != -1)
					cmdUpdated = cmd.Substring(0, index).ToLowerInvariant() + cmd.Substring(index, cmd.Length - index);
				return true;
			}
			if(cmdUpdated.StartsWith("color")) return true;
			if(cmdUpdated.StartsWith("backcolor")) return true;
			if(cmdUpdated.StartsWith("size")) return true;
			if(cmdUpdated.StartsWith("image")) {
				cmdUpdated = "image" + cmd.Substring(5);
				return true;
			}
			return false;
		}
		public static List<StringBlock> Parse(float fontSize, string text, bool allowNewLineSymbols) {
			return Parse(Color.Empty, fontSize, text, allowNewLineSymbols);
		}
		public static List<StringBlock> Parse(Color foreColor, float fontSize, string text, bool allowNewLineSymbols) {
			return Parse(Color.Empty, Color.Empty, fontSize, text, allowNewLineSymbols);
		}
		public static List<StringBlock> Parse(Color foreColor, Color backColor, float fontSize, string text, bool allowNewLineSymbols) {
			return Parse(foreColor, backColor, new HyperlinkSettings() { Color = Color.Blue, Underline = true }, fontSize, text, allowNewLineSymbols);
		}
		public static List<StringBlock> Parse(Color foreColor, Color backColor, HyperlinkSettings hyperlinkSettings, float fontSize , string text, bool allowNewLineSymbols) {
			if(text == null) text = string.Empty;
			List<StringBlock> res = new List<StringBlock>();
			StringBlock block = new StringBlock();
			block.FontSettings.SetColor(foreColor);
			block.FontSettings.SetBackColor(backColor);
			block.FontSettings.SetSize(fontSize);
			StringFontSettings defaultSettings = block.FontSettings.Clone();
			block = ParseCore(text, res, block, defaultSettings, hyperlinkSettings);
			if (!block.IsEmpty) res.Add(block);
			if (allowNewLineSymbols) 
				UpdateLineNumbers(res);
			return res;
		}
		static StringBlock ParseCore(string text, List<StringBlock> res, StringBlock block, StringFontSettings defaultSettings, HyperlinkSettings hyperlinkSettings) {
			int len = text.Length;
			StringBuilder sb = new StringBuilder();
			StringCommand prevCommand = null;
			for(int n = 0; n < len; n++) {
				char ch = text[n];
				if(ch == '\n') {
					if(n > 0 && text[n - 1] == '\r') continue;
					if(n < len - 1 && text[n + 1] == '\r') continue;
					ch = '\r';
				}
				bool isInHref = prevCommand != null && prevCommand.IsStart && IsHrefCommand(prevCommand);
				if((ch == '<' || ch == '&' || (IsSeparator(ch) && !isInHref)) && n < len - 1) {
					StringCommand command = ParseCommand(ref n, text);
					if(command.IsValid) {
						if(command.IsSeparator && sb.Length > 0) {
							sb.Append(ch);
							continue;
						}
						n--; 
						if(command.Command == "br") sb.Append("\xd");
						if(command.Command == "nbsp") {
							sb.Append("\xa0");
							continue;
						}
						if(command.IsSeparator && sb.Length == 0)
							n++;
						StringFontSettings current = block.FontSettings.Clone();
						ApplyCommandSettings(current, defaultSettings, hyperlinkSettings, command);
						if(!MustBreak(command)) {
							if(!command.IsSeparator && block.FontSettings.IsEquals(current)) {
								continue;
							}
							if(sb.Length == 0) {
								if(IsHrefCommand(command) && command.IsStart)
									ApplyCommand(block, command);
								block.FontSettings.Assign(current);
								if(!command.IsSeparator)
									continue;
							}
						}
						block.Text = command.IsSeparator && sb.Length == 0? command.Command: sb.ToString();
						if((block.Type != StringBlockType.Text && block.Type != StringBlockType.Link) || !string.IsNullOrEmpty(block.Text)) res.Add(block); 
						block = new StringBlock();
						block.FontSettings.Assign(current);
						if(ApplyCommand(block, command)) {
							res.Add(block);
							block = new StringBlock();
							block.FontSettings.Assign(current);
						}
						prevCommand = command;
						sb = new StringBuilder();
						continue;
					}
				}
				sb.Append(ch);
			}
			if(sb.Length > 0) block.Text = sb.ToString();
			return block;
		}
		private static bool IsSeparator(char ch) {
			if(ch == '\\' || ch == '/') return false;
			return ch.GetUnicodeCategory() == System.Globalization.UnicodeCategory.OtherPunctuation;
		}
		static bool ApplyCommand(StringBlock block, StringCommand command) {
			if(IsImageCommand(command)) {
				block.Type = StringBlockType.Image;
				block.Text = GetParameterOption(command);
				return true;
			}
			else if(IsHrefCommand(command)) {
				if(command.IsStart) {
					block.Type = StringBlockType.Link;
					block.Link = GetParameterOption(command);
				}
				return false;
			}
			return false;
		}
		static bool MustBreak(StringCommand command) {
			if(command.Command == "br") return true;
			if(!IsImageCommand(command)) return false;
			return true;
		}
		static void ApplyCommandSettings(StringFontSettings current, StringFontSettings defaultSettings, HyperlinkSettings hyperlinkSettings, StringCommand command) {
			if(IsFontStyleCommand(command)) {
				FontStyle options = GetFontStyleCommandOptions(command);
				if(command.IsStart)
					current.SetStyle(current.Style | options);
				else
					current.SetStyle(current.Style & (~options));
				return;
			}
			if(IsHrefCommand(command)) {
				if(command.IsStart) {
					current.SetColor(hyperlinkSettings.Color);
					if(hyperlinkSettings.Underline)	
						current.SetStyle(FontStyle.Underline);
				}
				else {
					current.SetColor(Color.Empty);
					current.SetStyle(current.Style & (~FontStyle.Underline));
				}
			}
			if(IsColorCommand(command)) {
				current.SetColor(command.IsEnd ? Color.Empty : GetFontColorCommandOptions(command, current));
				return;
			}
			if(IsBackColorCommand(command)) {
				current.SetBackColor(command.IsEnd ? Color.Empty : GetFontColorCommandOptions(command, current));
				return;
			}
			if(IsFontSizeCommand(command)) {
				current.SetSize(command.IsEnd ? defaultSettings.Size : GetFontSizeCommandOptions(command, current.Size));
				return;
			}
		}
		static void UpdateLineNumbers(List<StringBlock> stringBlocks) {
			int lineNumber = 0;
			for (int i = 0; i < stringBlocks.Count; i++) {
				stringBlocks[i].LineNumber = lineNumber;
				stringBlocks[i].Text = stringBlocks[i].Text.Replace("\r\n", "\r");
				stringBlocks[i].Text = stringBlocks[i].Text.Replace('\n', '\r');
				string[] strings = stringBlocks[i].Text.Split('\r');
				if (strings.Length > 1) {
					stringBlocks[i].Text = strings[0];
					for (int j = 1; j < strings.Length; j++) {
						lineNumber++;
						StringBlock stringBlock = new StringBlock();
						stringBlock.SetBlock(stringBlocks[i]);
						stringBlock.Text = strings[j];
						stringBlock.LineNumber = lineNumber;
						i++;
						stringBlocks.Insert(i, stringBlock);
					}
				}
			}
			for (int i = 1; i < stringBlocks.Count; i++)
				if (stringBlocks[i].Text == string.Empty && stringBlocks[i].LineNumber == stringBlocks[i - 1].LineNumber)
					stringBlocks.RemoveAt(i--);
			for (int i = 0; i < stringBlocks.Count - 1; i++)
				if (stringBlocks[i].Text == string.Empty && stringBlocks[i].LineNumber == stringBlocks[i + 1].LineNumber)
					stringBlocks.RemoveAt(i--);
			for (int i = 0; i < stringBlocks.Count; i++)
				if (stringBlocks[i].Text == string.Empty)
					stringBlocks[i].Text = "\r";
		}
		static bool IsColorCommand(StringCommand command) {
			return command.Command.StartsWith("color");
		}
		static bool IsHrefCommand(StringCommand command) {
			return command.Command.StartsWith("href");
		}
		static bool IsBackColorCommand(StringCommand command) {
			return command.Command.StartsWith("backcolor");
		}
		static bool IsImageCommand(StringCommand command) {
			return command.Command.StartsWith("image");
		}
		static bool IsFontSizeCommand(StringCommand command) {
			return command.Command.StartsWith("size");
		}
		static bool IsFontStyleCommand(StringCommand command) {
			return "bius".Contains(command.Command);
		}
		static Color GetColorFromString(string str) {
			Color color = Color.Empty;
			try {
				return (Color)Converter.ConvertFromInvariantString(str);
			}
			catch {
			}
			return color;
		}
		static Color GetFontColorCommandOptions(StringCommand command, StringFontSettings current) {
			Color color = current.Color;
			if (command.Command.StartsWith("color=")) {
				Color res = GetColorFromString(command.Command.Substring(6));
				if (res == Color.Empty) return current.Color;
				return res;
			}
			if(command.Command.StartsWith("backcolor=")) {
				Color res = GetColorFromString(command.Command.Substring(10));
				if(res == Color.Empty) return current.BackColor;
				return res;
			}
			return color;
		}
		static string GetParameterOption(StringCommand command) {
			int pos = command.Command.IndexOf('=');
			if(pos > -1) {
				return command.Command.Substring(pos + 1);
			}
			return string.Empty;
		}
		static FontStyle GetFontStyleCommandOptions(StringCommand command) {
			FontStyle res = FontStyle.Regular;
			switch (command.Command) {
				case "b": res |= FontStyle.Bold; break;
				case "i": res |= FontStyle.Italic; break;
				case "u": res |= FontStyle.Underline; break;
				case "s": res |= FontStyle.Strikeout; break;
			}
			return res;
		}
		static float GetFontSizeCommandOptions(StringCommand command, float current) {
			if (command.Command.StartsWith("size=")) {
				float res;
				int sizeStart = 5;
				if (command.Command[5] == '+' || command.Command[5] == '-') sizeStart = 6;
				if (float.TryParse(command.Command.Substring(sizeStart), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out res) && res > 0)
					return sizeStart == 6 ? current + (command.Command[5] == '-' ? -res : res) : res;
			}
			return current;
		}
		static int FindCommandEnd(string text, int pos, int len, char chCommandEnd) {
			int commandEnd = pos + 1;
			bool found = false;
			while (commandEnd < len) {
				if (text[commandEnd++] == chCommandEnd) {
					found = true;
					break;
				}
			}
			if (!found) return -1;
			return commandEnd;
		}
	}
}
namespace DevExpress.Utils.Text.Internal {
	public class StringCommand {
		bool isStart = true;
		string command = string.Empty;
		public bool IsValid { get { return !string.IsNullOrEmpty(command); } }
		public string Command { get { return command; } set { command = value; } }
		public bool IsStart { get { return isStart; } }
		public bool IsEnd { get { return !isStart; } }
		internal void SetStart() { this.isStart = true; }
		internal void SetEnd() { this.isStart = false; }
		public bool IsSeparator { get { return !string.IsNullOrEmpty(Command) && Command[0].GetUnicodeCategory() == System.Globalization.UnicodeCategory.OtherPunctuation; } }
	}
	public class StringFontSettings {
		bool styleSet = false;
		FontStyle style;
		Color color, backColor;
		float size;
		public StringFontSettings() {
			this.style = FontStyle.Regular;
			this.backColor = this.color = Color.Empty;
			this.size = 0;
		}
		public FontStyle Style { get { return style; } }
		public Color Color { get { return color; } }
		public Color BackColor { get { return backColor; } }
		public float Size { get { return size; } }
		public bool IsEquals(StringFontSettings settings) {
			return settings.Style == this.Style && settings.Color == this.Color && settings.Size == this.Size && settings.BackColor == this.BackColor;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsStyleSet { get { return styleSet; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStyle(FontStyle style) { 
			this.style = style;
			this.styleSet = true;
		}
		internal void SetColor(Color color) { this.color = color; }
		internal void SetSize(float size) { this.size = size; }
		internal void SetBackColor(Color backColor) { this.backColor = backColor; }
		internal void Assign(StringFontSettings source) {
			this.styleSet = source.styleSet;
			this.style = source.style;
			this.color = source.color;
			this.size = source.size;
			this.backColor = source.backColor;
		}
		internal StringFontSettings Clone() {
			StringFontSettings res = new StringFontSettings();
			res.Assign(this);
			return res;
		}
	}
	public enum StringBlockType { Text, Image, Object, Link };
	public enum StringBlockAlignment { Top, Center, Bottom };
	public class StringBlock {
		string text, imageName;
		Font font;
		int fontHeight = -1;
		int fontAscentHeight = -1;
		int lineNumber = -1;
		Size? size;
		StringBlockType type;
		StringFontSettings fontSettings;
		StringBlockAlignment? alignment;
		int matchIndex = -1;
		string matchText = null;
		Color matchFore = Color.Empty;
		Color matchBack = Color.Empty;
		public StringBlock() {
			this.imageName = null;
			this.size = null;
			this.alignment = null;
			this.fontSettings = new StringFontSettings();
			this.text = null;
			this.type = StringBlockType.Text;
		}
		public StringBlockAlignment Alignment {
			get {
				if(alignment == null) ParseSettings();
				return alignment.HasValue ? alignment.Value : StringBlockAlignment.Center;
			}
		}
		public string ImageName {
			get {
				if(imageName == null) ParseSettings();
				return imageName == null ? string.Empty : imageName;
			}
		}
		public Size Size {
			get {
				if(size == null) ParseSettings();
				return size.HasValue ? size.Value : Size.Empty;
			}
		}
		public string Link { get; set; }
		public StringBlockType Type { get { return type; } set { type = value; } }
		public Font Font { get { return font; } }
		public bool IsEmpty { get { return string.IsNullOrEmpty(this.text); } }
		public object Data { get; set; }
		public string Text {
			get { return text; }
			set {
				text = value;
			}
		}
		public int LineNumber { get { return lineNumber; } set { lineNumber = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int FontHeight { get { return fontHeight; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int FontAscentHeight { get { return fontAscentHeight; } }
		public StringFontSettings FontSettings { get { return fontSettings; } }
		void ParseSettings() {
			if(Type == StringBlockType.Text || string.IsNullOrEmpty(Text)) return;
			int pos = text.IndexOf(';');
			if(pos < 0) {
				this.imageName = this.Text;
				this.alignment = StringBlockAlignment.Center;
				this.size = Size.Empty;
				return;
			}
			string[] split = text.Split(';');
			this.imageName = split[0];
			for(int n = 1; n < split.Length; n++) {
				string parName, parValue;
				ParseParameter(split[n], out parName, out parValue);
				ParseSettingsParameter(parName, parValue);
			}
		}
		public static void ParseParameter(string parameter, out string name, out string value) {
			name = parameter;
			value = null;
			int eq = parameter.IndexOf('=');
			if(eq < 0) return;
			name = parameter.Substring(0, eq).ToLowerInvariant();
			value = parameter.Substring(eq + 1).ToLowerInvariant();
		}
		void ParseSettingsParameter(string parName, string parValue) {
			switch(parName) {
				case "align":
				case "alignment":
					this.alignment = ParseSettingsAlignment(parValue);
					return;
				case "size" :
					this.size = ParseSettingsSize(parValue);
					return;
			}
		}
		public int MatchIndex { get { return matchIndex; } }
		public string MatchText { get { return matchText; } }
		public Color MatchFore { get { return matchFore; } }
		public Color MatchBack { get { return matchBack; } }
		Size ParseSettingsSize(string parValue) {
			Size res = Size.Empty;
			try {
				int s = parValue.IndexOf(',');
				if(s < 0) return res;
				res.Width = int.Parse(parValue.Substring(0, s));
				res.Height = int.Parse(parValue.Substring(s + 1));
			} catch {
			}
			return res;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetFontInfo(Font font, int fontHeight, int fontAscentHeight) {
			this.font = font;
			this.fontHeight = fontHeight;
			this.fontAscentHeight = fontAscentHeight;
		}
		StringBlockAlignment ParseSettingsAlignment(string parValue) {
			if(parValue == "top") return StringBlockAlignment.Top;
			if(parValue == "bottom") return StringBlockAlignment.Bottom;
			return StringBlockAlignment.Center;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetBlock(StringBlock source) {
			this.size = source.size;
			this.alignment = source.alignment;
			this.font = source.font;
			this.fontHeight = source.fontHeight;
			this.fontAscentHeight = source.fontAscentHeight;
			this.FontSettings.Assign(source.FontSettings);
			this.type = source.Type;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetAscentHeight(int fontAscentHeight) {
			this.fontAscentHeight = fontAscentHeight;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetMatchInfo(string matchText, int matchIndex) {
			this.matchText = matchText;
			this.matchIndex = matchIndex;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetMatchColorInfo(Color matchFore, Color matchBack) {
			this.matchFore = matchFore;
			this.matchBack = matchBack;
		}
	}
}

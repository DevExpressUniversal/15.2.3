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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Office.UI;
using System.Reflection;
#if SL
using System.Windows.Documents;
using DevExpress.Utils.Internal;
#else
#endif
namespace DevExpress.Xpf.Office.UI {
	#region CharacterMapControl
	public partial class CharacterMapControl : UserControl {
		#region SelectionAdorner
		class SelectionAdorner : System.Windows.Documents.Adorner {
			Brush brush;
			TranslateTransform translateTransform;
			public SelectionAdorner(UIElement element, Brush brush)
				: base(element) {
				this.brush = brush;
				this.translateTransform = new TranslateTransform(0, 0);
			}
			public TranslateTransform TranslateTransform { get { return translateTransform; } }
			protected override void OnRender(DrawingContext drawingContext) {
				base.OnRender(drawingContext);
				drawingContext.PushTransform(TranslateTransform);
				Pen pen = new Pen(brush, 1);
				drawingContext.DrawRectangle(null, pen, new Rect(new Point(0, 0), AdornedElement.RenderSize));
			}
		}
		#endregion
		const string AdornerName = "SelectionAdorner";
		#region Fields
		readonly int[] CodePages = { 1250, 1251, 1252, 1253, 1254, 1255, 1256, 1257, 1258, 874, 437, 720, 737, 775, 850, 852, 855, 857, 858, 862, 866, 874 };
		readonly char[] CommonCharacters = { '\u00A3', '\u00A5', '\u00A9', '\u00AE', '\u00B1', '\u2260', '\u2264', '\u2265', '\u00F7', '\u00D7', '\u221E' };
		int charsPerLine = 12;
		int linesPerView = 8;
		Dictionary<char, List<string>> characterNames;
		List<char> chars;
		FrameworkElement selectedElement;
		Brush adornerBrush;
		bool suppressRaiseSearchBoxTextChangedEvent = false;
		#endregion
		public CharacterMapControl() {
			InitializeComponent();
			Loaded += OnLoaded;
			OnCharactersGridSizeChanged();
			InitFontFamilyComboBox();
			InitCharacterSetBox();
			InitFilter();
			InitSearch();
			InitStateButtons();
			InitSpecialList();
			RenewCommonlyUsed();
			RenewCharactersScrollbar();
			RenewCharacters();
			CharactersScrollBar.ValueChanged += CharactersScrollBar_ValueChanged;
		}
		#region Properties
		public int CharsPerLine {
			get { return charsPerLine; }
			set {
				if (value == CharsPerLine)
					return;
				charsPerLine = value;
				OnCharactersGridSizeChanged();
			}
		}
		public int LinesPerView {
			get { return linesPerView; }
			set {
				if (value == LinesPerView)
					return;
				linesPerView = value;
				OnCharactersGridSizeChanged();
			}
		}
		ToggleButton CommonButton { get { return btnCommonChars; } }
		ToggleButton SpecialButton { get { return btnSpecialChars; } }
		public Panel CommonlyUsedPanel { get { return pnlCommonlyUsed; } }
		public ComboBoxEdit FilterBox { get { return cbFilter; } }
		public TextBox SearchBox { get { return tbSearch; } }
		public TextBlock SearchResult { get { return tbSearchResult; } }
		public FontEdit FontNameBox { get { return cbFontFamily; } }
		public ScrollBar CharactersScrollBar { get { return sbCharactersScrollbar; } }
		public virtual string FontName {
			get {
				string name = FontNameBox.EditValue as String;
				if (name != null)
					return name;
				FontFamily family = FontNameBox.EditValue as FontFamily;
				if (family != null)
					return family.Source;
				return String.Empty;
			}
			set { FontNameBox.EditValue = value; }
		}
		public ComboBoxEdit CharacterSetBox { get { return cbCharacterSet; } }
		public virtual UnicodeCategory? FilterCategory {
			get {
				UnicodeCategoryComboBoxItem item = FilterBox.EditValue as UnicodeCategoryComboBoxItem;
				if (item == null)
					return null;
				else
					return item.UnicodeCategory;
			}
		}
		public Dictionary<char, List<string>> CharacterNames {
			get {
				if (characterNames == null) {
					CreateCharacterNames();
				}
				return characterNames;
			}
		}
		public List<char> Chars {
			get {
				if (chars == null) {
					Encoding encoding = GetCurrentEncoding();
					if (encoding == null || encoding == Encoding.Unicode)
						chars = BuildChars(null, 65535).ToList();
					else
						chars = BuildChars(encoding, 255).ToList();
				}
				return chars;
			}
		}
		public char Selection { get; private set; }
		public IServiceProvider ServiceProvider { get; set; }
		FrameworkElement SelectedElement {
			get { return selectedElement; }
			set {
				if (selectedElement == value)
					return;
				FrameworkElement oldElement = SelectedElement;
				selectedElement = value;
				OnSelectedElementChanged(oldElement, value);
			}
		}
		#endregion
		#region Events
		public event EventHandler CharDoubleClick;
		void RaiseCharDoubleClick() {
			if (CharDoubleClick != null)
				CharDoubleClick(this, EventArgs.Empty);
		}
		#endregion
		void OnSelectedElementChanged(FrameworkElement oldElement, FrameworkElement newElement) {
			if (oldElement != null)
				RemoveSelectionAdorner(oldElement);
			TextBlock tb = newElement as TextBlock;
			if (tb == null || tb.Text.Length != 1)
				return;
			Selection = tb.Text[0];
			char ch = tb.Text[0];
			CharacterDescription.Text = "U+" + ((int)ch).ToString("X") + GetOriginName(ch) + "; " + char.GetUnicodeCategory(ch).ToString();
			this.suppressRaiseSearchBoxTextChangedEvent = true;
			SearchBox.Text = ((int)ch).ToString();
			this.suppressRaiseSearchBoxTextChangedEvent = false;
			SetSearchSymbol();
			AddAdorner(newElement);
		}
		#region GetBorderBrush
		Brush GetBorderBrush() {
			Brush result = TryGetBrushByState("FocusedState");
			if (result != null)
				return result;
			result = TryGetBrushByState("HoverState");
			if (result != null)
				return result;
			result = TryGetBrushByState("TextBoxFocusedState");
			if (result != null)
				return result;
			DXBorder dxBorder = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(SearchBox, "Focused") as DXBorder;
			if (dxBorder != null && dxBorder.Background != null)
				return dxBorder.Background;
			Border border = DevExpress.Xpf.Core.Native.LayoutHelper.FindElement(SearchBox, elem => { return elem is Border && elem.Name != "focus"; }) as Border;
			if (border != null && border.Background != null)
				return border.Background;
			return new SolidColorBrush(Colors.Black);
		}
		Brush TryGetBrushByState(string stateName) {
			FrameworkElement element = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(SearchBox, stateName);
			Grid grid = element as Grid;
			if (grid != null)
				element = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByType<DXBorder>(grid);
			DXBorder dxBorder = element as DXBorder;
			return dxBorder != null ? dxBorder.BorderBrush : null;
		}
		#endregion
		void AddAdorner(FrameworkElement element) {
			System.Windows.Documents.AdornerLayer layer = AdornerHelper.FindAdornerLayer(element);
			if (layer != null)
				layer.Add(new SelectionAdorner(element, this.adornerBrush) { Name = AdornerName });
		}
		void RemoveSelectionAdorner(FrameworkElement element) {
			System.Windows.Documents.AdornerLayer layer = AdornerHelper.FindAdornerLayer(element);
			if (layer == null)
				return;
			System.Windows.Documents.Adorner selectionAdorner = GetSelectionAdorner(element);
			if (selectionAdorner != null)
				layer.Remove(selectionAdorner);
		}
		SelectionAdorner GetSelectionAdorner(FrameworkElement element) {
			System.Windows.Documents.AdornerLayer layer = AdornerHelper.FindAdornerLayer(element);
			if (layer == null)
				return null;
			System.Windows.Documents.Adorner[] adorners = layer.GetAdorners(element);
			return adorners != null ? adorners.First<System.Windows.Documents.Adorner>(adorner => { return adorner.Name == AdornerName; }) as SelectionAdorner : null;
		}
		void CreateCharacterNames() {
			characterNames = new Dictionary<char, List<string>>();
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Xpf.Core.Editors.Office.CharacterNames.txt");
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(stream != null && stream.Length > 0);
#endif
			CultureInfo englishCulture = new CultureInfo("en-US");
			TextReader tr = new StreamReader(stream);
			string line;
			while (!String.IsNullOrEmpty(line = tr.ReadLine())) {
				int ch;
				int.TryParse(line.Substring(line.Length - 5), NumberStyles.HexNumber, englishCulture, out ch);
				if (!characterNames.ContainsKey((char)ch)) {
					List<string> list = new List<string>();
					list.Add(ExtractCharacterName(line));
					characterNames.Add((char)ch, list);
				}
				else {
					characterNames[(char)ch].Add(ExtractCharacterName(line));
				}
			}
		}
		internal string ExtractCharacterName(string line) {
			return line.Substring(0, line.Length - 6);
		}
		internal virtual Encoding GetCurrentEncoding() {
			EncodingComboBoxItem item = CharacterSetBox.EditValue as EncodingComboBoxItem;
			if (item != null)
				return item.Encoding;
			else
				return null;
		}
#if SL
		FontDescriptor FontDescriptor { get { return FontManager.GetFontDescriptor(FontName, false, false); } }
		FontFamily GetFontFamily() {
			return FontDescriptor.FontFamily;
		}
		FontSource GetFontSource() {
			return FontDescriptor.FontSource;
		}
		IEnumerable<char> BuildChars(Encoding code, int range) {
			TTFontInfo fontInfo = FontDescriptor.FontInfo;
			List<int> list = new List<int>();
			for (int i = 0; i <= range; i++) {
				char c = code == null ? ((char)i) : code.GetChars(new byte[] { (byte)i })[0];
				int glyphIndex = -1;
				try {
					glyphIndex = fontInfo.GetGlyphIndex(c);
				}
				catch {
				}
				if (glyphIndex > 0 && !list.Contains(glyphIndex) && (FilterCategory == null || char.GetUnicodeCategory(c) == FilterCategory)) {
					list.Add(glyphIndex);
					yield return c;
				}
			}
		}
#else
		FontFamily GetFontFamily() {
			return new FontFamily(FontName);
		}
		IEnumerable<char> BuildChars(Encoding code, int range) {
			List<char> result = new List<char>();
			if (ServiceProvider == null)
				return result;
			IFontCharacterSetService service = ServiceProvider.GetService(typeof(IFontCharacterSetService)) as IFontCharacterSetService;
			if (service == null)
				return result;
			service.BeginProcessing(FontName);
			try {
				for (int i = 0; i < UInt16.MaxValue; i++) {
					char character = (char)i;
					bool isCharCategoryControl = Char.IsControl(character);
					bool isCharCategoryPrivateUse = Char.GetUnicodeCategory(character) == UnicodeCategory.PrivateUse;
					if (!isCharCategoryControl && !isCharCategoryPrivateUse && service.ContainsChar(character) && (FilterCategory == null || char.GetUnicodeCategory(character) == FilterCategory))
						result.Add(character);
				}
			}
			finally {
				service.EndProcessing();
			}
			return result;
		}
#endif
		void OnLoaded(object sender, RoutedEventArgs e) {
			this.adornerBrush = GetBorderBrush();
#if SL
			DXDialog dialog = FloatingContainer.GetDialogOwner(this) as DXDialog;
			if (dialog != null)
				dialog.OkButton.Visibility = System.Windows.Visibility.Collapsed;
#else
			FloatingContainer container = FloatingContainer.GetDialogOwner(this) as FloatingContainer;
			if (container != null) {
				DialogControl dialog = container.Content as DialogControl;
				if (dialog != null)
					dialog.OkButton.Visibility = System.Windows.Visibility.Collapsed;
			}
#endif
		}
		void RenewCharactersScrollbar() {
			CharactersScrollBar.Minimum = 0;
			CharactersScrollBar.SmallChange = 1;
			CharactersScrollBar.LargeChange = LinesPerView;
			CharactersScrollBar.ViewportSize = LinesPerView;
			double scrollmax = FontName != null ? (Chars.Count() / charsPerLine - CharactersScrollBar.ViewportSize + 1) : 0;
			CharactersScrollBar.Maximum = scrollmax > 0 ? scrollmax : 1;
		}
		#region Initialization
		static class Characters {
			public const char Bullet = '•';
			public const char ClosingDoubleQuotationMark = '”';
			public const char ClosingSingleQuotationMark = '’';
			public const char Colon = ':';
			public const char ColumnBreak = '';
			public const char CopyrightSymbol = '©';
			public const char CurrencySign = '¤';
			public const char Dash = '-';
			public const char Dot = '.';
			public const char Ellipsis = '…';
			public const char EmDash = '—';
			public const char EmSpace = ' ';
			public const char EnDash = '–';
			public const char EnSpace = ' ';
			public const char EqualSign = '=';
			public const char FloatingObjectMark = '\b';
			public const char Hyphen = '';
			public const char LeftDoubleQuote = '“';
			public const char LeftSingleQuote = '‘';
			public const char LineBreak = '\v';
			public const char MiddleDot = '·';
			public const char NonBreakingSpace = ' ';
			public const char ObjectMark = '￼';
			public const char OpeningDoubleQuotationMark = '“';
			public const char OpeningSingleQuotationMark = '‘';
			public const char OptionalHyphen = '­';
			public const char PageBreak = '\f';
			public const char ParagraphMark = '\r';
			public const char PilcrowSign = '¶';
			public const char QmSpace = ' ';
			public const char RegisteredTrademarkSymbol = '®';
			public const char RightDoubleQuote = '”';
			public const char RightSingleQuote = '’';
			public const char SectionMark = '';
			public const char SeparatorMark = '|';
			public const char Space = ' ';
			public const char TabMark = '\t';
			public const char TrademarkSymbol = '™';
			public const char Underscore = '_';
		}
		void InitSpecialList() {
			foreach (var prop in typeof(Characters).GetMembers()) {
				if (prop.MemberType == System.Reflection.MemberTypes.Field) {
					System.Reflection.FieldInfo field = typeof(Characters).GetField(prop.Name);
					if (field.IsStatic || field.IsLiteral) {
						char val = (char)field.GetValue(null);
						SpecialList.Items.Add(new TextBlock() {
							Text = val.ToString() + " " + prop.Name,
							Tag = val
						});
					}
				}
			}
			SpecialList.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(SpecialList_MouseLeftButtonDown);
		}
		void InitCharacterSetBox() {
			CharacterSetBox.Items.Add(new EncodingComboBoxItem(Encoding.Unicode));
			foreach (int i in CodePages.ToArray()) {
				Encoding encoding = DXEncoding.GetEncoding(i);
				if (encoding != null)
					CharacterSetBox.Items.Add(new EncodingComboBoxItem(encoding));
			}
			if (CharacterSetBox.Items.Count > 0)
				CharacterSetBox.EditValue = CharacterSetBox.Items[0];
			CharacterSetBox.EditValueChanged += OnCharacterSetBoxEditValueChanged;
		}
		void InitStateButtons() {
			CommonButton.Click += CommonButton_Click;
			SpecialButton.Click += SpecialButton_Click;
		}
		void RenewCommonlyUsed() {
			CommonlyUsedPanel.Children.Clear();
			if (FontName == null)
				return;
			FontFamily fontFamily = GetFontFamily();
#if SL
			FontSource fontSource = GetFontSource();
#endif
			foreach (char c in CommonCharacters.ToArray()) {
				TextBlock tb = CreateTextBlock();
				tb.Text = c.ToString();
				tb.FontWeight = FontWeights.Bold;
				tb.FontSize = 32;
				tb.Width = 33;
				tb.Height = 40;
				tb.Margin = new Thickness(1);
				tb.FontFamily = fontFamily;
#if SL
				tb.FontSource = fontSource;
#endif
				CommonlyUsedPanel.Children.Add(tb);
			}
		}
		void InitSearch() {
			SearchBox.TextChanged += new TextChangedEventHandler(SearchBox_TextChanged);
			SearchResult.Foreground = Brushes.Black;
		}
		void InitFilter() {
			FilterBox.Items.Add(new UnicodeCategoryComboBoxItem(null));
			foreach (var prop in typeof(UnicodeCategory).GetMembers()) {
				if (prop.DeclaringType.IsEnum && prop.Name != "value__")
					FilterBox.Items.Add(new UnicodeCategoryComboBoxItem((UnicodeCategory)Enum.Parse(typeof(UnicodeCategory), prop.Name, true)));
			}
			if (FilterBox.Items.Count > 0)
				FilterBox.EditValue = FilterBox.Items[0];
			FilterBox.EditValueChanged += OnFilterBoxEditValueChanged;
		}
		void InitFontFamilyComboBox() {
			if (FontNameBox.Items.Count > 0)
				FontNameBox.SelectedIndex = 0;
			FontNameBox.EditValueChanged += OnFontNameBoxEditValueChanged;
		}
		#endregion
		#region Handlers
		void SpecialList_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if (DevExpress.Xpf.Office.Internal.MouseHelper.IsDoubleClick(e))
				RaiseCharDoubleClick();
		}
		void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
			if (this.suppressRaiseSearchBoxTextChangedEvent)
				return;
			int charCode = -1;
			if (!Int32.TryParse(SearchBox.Text, out charCode))
				return;
			string ch = ((char)charCode).ToString();
			foreach (TextBlock tb in CommonlyUsedPanel.Children) {
				if (tb.Text == ch) {
					SelectedElement = tb;
					return;
				}
			}
			foreach (TextBlock tb in CharactersGrid.Children) {
				if (tb.Text == ch) {
					SelectedElement = tb;
					return;
				}
			}
			int charIndex = Chars.IndexOf(ch[0]);
			if (charIndex > 0) {
				CharactersScrollBar.Value = charIndex / CharsPerLine;
				SearchBox_TextChanged(sender, e);
			}
			else {
				SelectedElement = null;
				SetSearchSymbol();
			}
		}
		void SetSearchSymbol() {
			if (SearchBox == null)
				return;
			int result = -1;
			if (!int.TryParse(SearchBox.Text, out result))
				result = Int32.MaxValue;
			if (result > 65535)
				result = -1;
			SearchResult.FontFamily = GetFontFamily();
#if SL
			SearchResult.FontSource = GetFontSource();
#endif
			SearchResult.Text = result < 0 ? "Err" : ((char)result).ToString();
		}
		void OnFilterBoxEditValueChanged(object sender, EditValueChangedEventArgs e) {
			UpdateAll();
		}
		void OnCharacterSetBoxEditValueChanged(object sender, EditValueChangedEventArgs e) {
			UpdateAll();
		}
		void OnFontNameBoxEditValueChanged(object sender, EditValueChangedEventArgs e) {
			UpdateAll();
		}
		void tb_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			TextBlock tb = ((TextBlock)sender);
			if (tb.Text.Length != 1)
				return;
			SelectedElement = tb;
		}
		void CharactersScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			RenewCharactersScrollbar();
			RenewCharacters();
		}
		void tb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (e.ClickCount > 1)
				RaiseCharDoubleClick();
		}
		void OnCharactersGridSizeChanged() {
			CharactersGrid.ColumnDefinitions.Clear();
			CharactersGrid.RowDefinitions.Clear();
			CharactersGrid.Children.Clear();
			for (int i = 0; i < CharsPerLine; i++)
				CharactersGrid.ColumnDefinitions.Add(new ColumnDefinition());
			for (int i = 0; i < LinesPerView; i++)
				CharactersGrid.RowDefinitions.Add(new RowDefinition());
			for (int i = 0; i < LinesPerView; i++)
				for (int j = 0; j < CharsPerLine; j++) {
					TextBlock tb = CreateTextBlock();
					tb.SetValue(Grid.RowProperty, i);
					tb.SetValue(Grid.ColumnProperty, j);
					CharactersGrid.Children.Add(tb);
				}
		}
		#endregion
		void UpdateAll() {
			chars = null;
			RenewCharacters();
			RenewCharactersScrollbar();
			RenewCommonlyUsed();
			SetSearchSymbol();
		}
		void RenewCharacters() {
			if (FontName == null)
				return;
			FontFamily fontFamily = GetFontFamily();
#if SL
			FontSource fontSource = GetFontSource();
#endif
			if (SelectedElement != null && !CommonlyUsedPanel.Children.Contains(SelectedElement))
				RemoveSelectionAdorner(SelectedElement);
			int i = (int)CharactersScrollBar.Value * CharsPerLine;
			foreach (TextBlock tb in CharactersGrid.Children) {
				if (i >= Chars.Count) {
					tb.Text = "";
					continue;
				}
				char c = Chars[i];
				tb.FontFamily = fontFamily;
#if SL
				tb.FontSource = fontSource;
#endif
				tb.Text = c.ToString();
				if (c == Selection) {
					if (SelectedElement != tb)
						SelectedElement = tb;
					else
						AddAdorner(tb);
				}
				i++;
			}
		}
		TextBlock CreateTextBlock() {
			TextBlock tb = new TextBlock();
			tb.FontSize = 15;
			tb.TextAlignment = TextAlignment.Center;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			tb.MinHeight = 10;
			tb.MinWidth = 15;
			tb.MouseLeftButtonUp += tb_MouseLeftButtonUp;
			tb.MouseLeftButtonDown += tb_MouseLeftButtonDown;
			return tb;
		}
		internal string GetOriginName(char c) {
			List<string> chName;
			string res = String.Empty;
			CharacterNames.TryGetValue(c, out chName);
			if (chName == null)
				return String.Empty;
			foreach (string name in chName) {
				if (name.ToUpper() == name)
					res = name;
			}
			if (string.IsNullOrEmpty(res))
				res = GetAlternativeName(c);
			if (string.IsNullOrEmpty(res))
				res = GetGroupName(c);
			return String.IsNullOrEmpty(res) ? String.Empty : ";" + res;
		}
		internal string GetAlternativeName(char c) {
			List<string> chName;
			string res = String.Empty;
			CharacterNames.TryGetValue(c, out chName);
			if (chName == null)
				return String.Empty;
			foreach (string name in chName) {
				if (name.ToLower() == name)
					res += name + "; ";
			}
			return res;
		}
		internal string GetGroupName(char c) {
			List<string> chName;
			string res = String.Empty;
			CharacterNames.TryGetValue(c, out chName);
			if (chName == null)
				return String.Empty;
			foreach (string name in chName) {
				if (name.ToLower() != name && name.ToUpper() != name)
					res = name;
			}
			return res;
		}
		#region Tabbed control
		void SpecialButton_Click(object sender, RoutedEventArgs e) {
			SpecialButton.IsChecked = true;
			CommonButton.IsChecked = false;
			SpecialGrid.Visibility = Visibility.Visible;
			CommonGrid.Visibility = Visibility.Collapsed;
		}
		void CommonButton_Click(object sender, RoutedEventArgs e) {
			SpecialButton.IsChecked = false;
			CommonButton.IsChecked = true;
			SpecialGrid.Visibility = Visibility.Collapsed;
			CommonGrid.Visibility = Visibility.Visible;
		}
		#endregion
	}
	#endregion
	#region EncodingComboBoxItem
	public class EncodingComboBoxItem {
		internal EncodingComboBoxItem(Encoding encoding) {
			Encoding = encoding;
		}
		public Encoding Encoding { get; set; }
		public override string ToString() {
			if (Encoding == Encoding.Unicode)
				return "Unicode";
			else
				return Encoding.WebName;
		}
	}
	#endregion
	#region UnicodeCategoryComboBoxItem
	public class UnicodeCategoryComboBoxItem {
		public UnicodeCategory? UnicodeCategory { get; set; }
		internal UnicodeCategoryComboBoxItem(UnicodeCategory? category) {
			UnicodeCategory = category;
		}
		public override string ToString() {
			if (UnicodeCategory == null)
				return "All Symbols";
			else
				return UnicodeCategory.ToString();
		}
	}
	#endregion
}
namespace DevExpress.Xpf.Office.Internal {
	#region MouseHelper
	public static class MouseHelper {
		static int Timeout = 500;
		static bool clicked = false;
		static System.Windows.Point position;
		public static bool IsDoubleClick(MouseButtonEventArgs e) {
			Point location = e.GetPosition(null);
			if (clicked) {
				clicked = false;
				return position.Equals(location);
			}
			clicked = true;
			position = location;
			System.Threading.ParameterizedThreadStart threadStart = new System.Threading.ParameterizedThreadStart(ResetThread);
			System.Threading.Thread thread = new System.Threading.Thread(threadStart);
			thread.Start();
			return false;
		}
		static void ResetThread(object state) {
			System.Threading.Thread.Sleep(Timeout);
			clicked = false;
		}
	}
	#endregion
}

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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.CodeParser;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;
namespace DevExpress.Web.Design {
	public class ClientSideEventsFrame : TwoSidesEditorFrame {
		ITypeDescriptorContext typeDescriptorContext;
		protected ClientSideEventsBase ClientSideEvents { get { return EventsOwner.ClientSideEvents; } }
		protected ITypeDescriptorContext Context { get { return Designer.GetTypeDescriptorContext("ClientSideEvents"); } }
		protected override string FrameName { get { return "ClientSideEventsFrame"; } }
		protected ClientSideEventsOwner EventsOwner { get; set; }
		DevExpress.XtraEditors.ListBoxControl ListBoxEvents { get; set; }
		RichEditControl SelectedEventEditor { get; set; }
		ASPxWebControlDesigner Designer { get { return EventsOwner.Designer; } }
		ITypeDescriptorContext TypeDescriptorContext {
			get {
				if(typeDescriptorContext == null)
					typeDescriptorContext = EventsOwner != null ? EventsOwner.Provider as ITypeDescriptorContext : null;
				return typeDescriptorContext;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitializeEventsOwner();
			Font = GetDialogFont(EventsOwner.Provider);
			PostponedCreateInnerControls();
			FillListBoxEvents();
		}
		void InitializeEventsOwner() {
			EventsOwner = (ClientSideEventsOwner)DesignerItem.Tag;
			EventsOwner.OnSaveLastChanges = () => {
				SelectedEventEditor.LostFocus -= OnEventCodeTextBox_LostFocus;
				EventsOwner.AssignEventCode(SelectedEventEditor.Text);
			};
		}
		void PostponedCreateInnerControls() {
			SuspendLayout();
			CreateListBoxEvents("ListBoxEvents");
			SelectedEventEditor = CreateRichEditor("SelectedEventEditor");
			RightPanel.Controls.Add(SelectedEventEditor);
			ResumeLayout(false);
		}
		void CreateListBoxEvents(string name) {
			ListBoxEvents = new DevExpress.XtraEditors.ListBoxControl() {
				Name = name,
				SortOrder = SortOrder.Ascending,
				Dock = DockStyle.Fill
			};
			LeftPanel.Controls.Add(ListBoxEvents);
			ListBoxEvents.ItemHeight = Font.Height + SystemInformation.Border3DSize.Height * 2;
			ListBoxEvents.Font = Font;
			ListBoxEvents.SelectedIndexChanged += ListBoxEventsSelectedIndexChanged;
			ListBoxEvents.Click += (object sender, EventArgs e) => { ShowEventCode(); };
			ListBoxEvents.DrawItem += ListBoxEvents_DrawItem;
		}
		void ListBoxEvents_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			if(e.Item == null)
				return;
			var eventText = GetEventHandler(e.Item.ToString());
			if(!string.IsNullOrEmpty(eventText))
				e.Appearance.FontStyleDelta = FontStyle.Bold;
		}
		string GetEventHandler(string eventHandlerName) {
			var eventHandlerProperty = TypeDescriptor.GetProperties(ClientSideEvents)[eventHandlerName];
			return eventHandlerProperty.GetValue(ClientSideEvents) as string;
		}
		RichEditControl CreateRichEditor(string name) { 
			var richEditor = new RichEditControl() {
				AcceptsTab = true,
				ActiveViewType = RichEditViewType.Simple
			};
			richEditor.Name = name;
			richEditor.LostFocus += OnEventCodeTextBox_LostFocus;
			richEditor.Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
			richEditor.Options.VerticalRuler.Visibility = RichEditRulerVisibility.Hidden;
			richEditor.Dock = DockStyle.Fill;
			richEditor.AddService(typeof(ISyntaxHighlightService), new SyntaxHighlightService(richEditor));
			return richEditor;
		}
		void FillListBoxEvents() {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ClientSideEvents);
			var selectedProperty = TypeDescriptorContext != null ? TypeDescriptorContext.PropertyDescriptor.DisplayName : string.Empty;
			bool propertyFound = false;
			foreach(PropertyDescriptor property in properties) {
				if(property.PropertyType != typeof(string) || !property.IsBrowsable)
					continue;
				ListBoxEvents.Items.Add(property.Name);
				if(property.Name == selectedProperty) {
					ListBoxEvents.SelectedItem = property.Name;
					propertyFound = true;
				}
			}
			if(!propertyFound)
				ListBoxEvents.SelectedIndex = 0;
		}
		void ListBoxEventsSelectedIndexChanged(object sender, EventArgs e) {
			EventsOwner.SelectedEventName = ListBoxEvents.SelectedItem.ToString();
			ShowEventCode();
		}
		void OnEventCodeTextBox_LostFocus(object sender, EventArgs e) {
			EventsOwner.AssignEventCode(SelectedEventEditor.Text);
		}
		void ShowEventCode() {
			SelectedEventEditor.Text = EventsOwner.GetCurrentClientSideEventCode();
			SelectedEventEditor.Focus();
		}
	}
	public class ClientSideEventsOwner : IOwnerEditingProperty {
		public delegate void SaveLastChanges();
		ASPxWebControlDesigner designer;
		Dictionary<string, string> undoClientSideEvents;
		ClientSideEventsBase clientSideEvents;
		public ClientSideEventsOwner(object component, IServiceProvider provider, ASPxWebControlDesigner designer)
			: this(component, provider) {
			this.designer = designer;
		}
		public ClientSideEventsOwner(object component, IServiceProvider provider) 
		: this(component, provider, ((ASPxWebControl)component).ClientSideEventsInternal){
		}
		public ClientSideEventsOwner(object component, IServiceProvider provider, ClientSideEventsBase clientSideEvents) {
			Component = component;			
			Provider = provider;
			this.clientSideEvents = clientSideEvents;
			SaveUndo();
		}
		public ClientSideEventsBase ClientSideEvents { get { return clientSideEvents;} }
		protected Dictionary<string, string> UndoClientSideEvents {
			get {
				if(undoClientSideEvents == null)
					undoClientSideEvents = new Dictionary<string, string>();
				return undoClientSideEvents;
			}
		}
		public string SelectedEventName { get; set; }
		public ASPxWebControl EventsControl { get { return (ASPxWebControl)Component; } }
		public object Component { get; private set; }
		public IServiceProvider Provider { get; private set; }
		public ASPxWebControlDesigner Designer {
			get {
				if(designer == null) {
					var host = Provider != null ? Provider.GetService(typeof(IDesignerHost)) as IDesignerHost : null;
					designer = host != null ? host.GetDesigner((Component as IComponent)) as ASPxWebControlDesigner : null;
				}
				return designer;
			}
		}
		public SaveLastChanges OnSaveLastChanges { get; set; }
		public object PropertyInstance { get { return ClientSideEvents; } }
		public void SaveUndo() {
			UndoClientSideEvents.Clear();
			var properties = TypeDescriptor.GetProperties(ClientSideEvents);
			foreach(PropertyDescriptor property in properties)
				UndoClientSideEvents[property.Name] = property.GetValue(ClientSideEvents).ToString();
		}
		void IOwnerEditingProperty.SaveChanges() { 
		}
		public void UndoChanges() {
			foreach(var eventName in UndoClientSideEvents.Keys) {
				var eventHandler = TypeDescriptor.GetProperties(ClientSideEvents)[eventName];
				var convert = eventHandler.Converter;
				eventHandler.SetValue(ClientSideEvents, UndoClientSideEvents[eventName]);
			}
		}
		public void BeforeClosed() {
			if(OnSaveLastChanges != null)
				OnSaveLastChanges();
		}
		public bool ItemsChanged {
			get {
				var properties = TypeDescriptor.GetProperties(ClientSideEvents);
				foreach(PropertyDescriptor property in properties) {
					var key = property.Name;
					if(UndoClientSideEvents.ContainsKey(key) && UndoClientSideEvents[key] != property.GetValue(ClientSideEvents).ToString())
						return true;
				}
				return false;
			}
			set { }
		}
		public void AssignEventCode(string eventCode) {
			if(SelectedEventName != null) {
				string newEventCode = eventCode;
				if(newEventCode == GetEventHandlerPattern())
					newEventCode = "";
				if(GetEventHandler(SelectedEventName) != newEventCode) {
					PropertyDescriptor eventHandlerProperty = TypeDescriptor.GetProperties(ClientSideEvents)[SelectedEventName];
					TypeConverter convert = eventHandlerProperty.Converter;
					eventHandlerProperty.SetValue(ClientSideEvents, convert.ConvertFromString(newEventCode));
					ComponentChanged(false);
				}
			}
		}
		public string GetCurrentClientSideEventCode() {
			var eventCode = GetEventHandler(SelectedEventName);
			return !string.IsNullOrEmpty(eventCode) ? eventCode : GetEventHandlerPattern();
		}
		protected virtual string GetEventHandlerPattern() {
			return StringResources.ClientSideEventsForm_EventCodePattern;
		}
		string GetEventHandler(string eventHandlerName) {
			PropertyDescriptor eventHandlerProperty = TypeDescriptor.GetProperties(ClientSideEvents)[eventHandlerName];
			return eventHandlerProperty.GetValue(ClientSideEvents) as string;
		}
		protected virtual void ComponentChanged(bool checkChanged) {
			if(Designer == null)
				return;
			if(!checkChanged || Designer.IsComponentChanged())
				Designer.ComponentChanged();
		}
	}
	public class SyntaxHighlightService : ISyntaxHighlightService {
		RichEditControl editor;
		SyntaxHighlightInfo syntaxHighlightInfo;
		public SyntaxHighlightService(RichEditControl editor) {
			this.editor = editor;
			this.syntaxHighlightInfo = new SyntaxHighlightInfo();
		}
		public RichEditControl Editor {
			get { return editor; }
		}
		public Document Document {
			get { return Editor.Document; }
		}
		DevExpress.CodeParser.TokenCollection Parse(string code) {
			if(string.IsNullOrEmpty(code))
				return null;
			ITokenCategoryHelper tokenizer = TokenCategoryHelperFactory.CreateHelper(ParserLanguageID.JavaScript);
			return (tokenizer != null) ? tokenizer.GetTokens(code) : new DevExpress.CodeParser.TokenCollection();
		}
		void HighlightSyntax(DevExpress.CodeParser.TokenCollection tokens) {
			if(tokens == null || tokens.Count == 0)
				return;
			CharacterProperties cp = Document.BeginUpdateCharacters(0, 1);
			List<SyntaxHighlightToken> syntaxTokens = new List<SyntaxHighlightToken>(tokens.Count);
			foreach(Token token in tokens)
				HighlightCategorizedToken((CategorizedToken)token, syntaxTokens);
			Document.ApplySyntaxHighlight(syntaxTokens);
			Document.EndUpdateCharacters(cp);
		}
		void HighlightCategorizedToken(CategorizedToken token, List<SyntaxHighlightToken> syntaxTokens) {
			SyntaxHighlightProperties highlightProperties = syntaxHighlightInfo.CalculateTokenCategoryHighlight(token.Category);
			Color backColor = highlightProperties.BackColor.HasValue ? highlightProperties.BackColor.Value : Editor.ActiveView.BackColor;
			SyntaxHighlightToken syntaxToken = SetTokenColor(token, highlightProperties, backColor);
			if(syntaxToken != null)
				syntaxTokens.Add(syntaxToken);
		}
		SyntaxHighlightToken SetTokenColor(Token token, SyntaxHighlightProperties foreColor, Color backColor) {
			if(Document.Paragraphs.Count < token.Range.Start.Line)
				return null;
			int paragraphStart = DocumentHelper.GetParagraphStart(Document.Paragraphs[token.Range.Start.Line - 1]);
			int tokenStart = paragraphStart + token.Range.Start.Offset - 1;
			if(token.Range.End.Line != token.Range.Start.Line)
				paragraphStart = DocumentHelper.GetParagraphStart(Document.Paragraphs[token.Range.End.Line - 1]);
			int tokenEnd = paragraphStart + token.Range.End.Offset - 1;
			return new SyntaxHighlightToken(tokenStart, tokenEnd - tokenStart, foreColor);
		}
		void ExecuteHighlighting() {
			DevExpress.CodeParser.TokenCollection tokens = Parse(Editor.Text);
			HighlightSyntax(tokens);
		}
		void ISyntaxHighlightService.ForceExecute() {
			ExecuteHighlighting();
		}
		void ISyntaxHighlightService.Execute() {
			ExecuteHighlighting();
		}
	}
	[CLSCompliant(false)]
	public class SyntaxHighlightInfo {
		readonly Dictionary<TokenCategory, SyntaxHighlightProperties> properties;
		public SyntaxHighlightInfo() {
			this.properties = new Dictionary<TokenCategory, SyntaxHighlightProperties>();
			Reset();
		}
		public void Reset() {
			properties.Clear();
			Add(TokenCategory.Text, DXColor.Black);
			Add(TokenCategory.Keyword, DXColor.Blue);
			Add(TokenCategory.String, DXColor.Brown);
			Add(TokenCategory.Comment, DXColor.Green);
			Add(TokenCategory.Identifier, DXColor.Black);
			Add(TokenCategory.PreprocessorKeyword, DXColor.Blue);
			Add(TokenCategory.Number, DXColor.Red);
			Add(TokenCategory.Operator, DXColor.Black);
			Add(TokenCategory.Unknown, DXColor.Black);
			Add(TokenCategory.XmlComment, DXColor.Gray);
			Add(TokenCategory.CssComment, DXColor.Green);
			Add(TokenCategory.CssKeyword, DXColor.Brown);
			Add(TokenCategory.CssPropertyName, DXColor.Red);
			Add(TokenCategory.CssPropertyValue, DXColor.Blue);
			Add(TokenCategory.CssSelector, DXColor.Maroon);
			Add(TokenCategory.CssStringValue, DXColor.Blue);
			Add(TokenCategory.HtmlAttributeName, DXColor.Red);
			Add(TokenCategory.HtmlAttributeValue, DXColor.Blue);
			Add(TokenCategory.HtmlComment, DXColor.Green);
			Add(TokenCategory.HtmlElementName, DXColor.Brown);
			Add(TokenCategory.HtmlEntity, DXColor.Gray);
			Add(TokenCategory.HtmlOperator, DXColor.Black);
			Add(TokenCategory.HtmlServerSideScript, DXColor.Black, DXColor.Yellow);
			Add(TokenCategory.HtmlString, DXColor.Blue);
			Add(TokenCategory.HtmlTagDelimiter, DXColor.Blue);
		}
		void Add(TokenCategory category, Color foreColor) {
			Add(category, foreColor, null);
		}
		void Add(TokenCategory category, Color foreColor, Color? backColor) {
			SyntaxHighlightProperties item = new SyntaxHighlightProperties();
			item.ForeColor = foreColor;
			item.BackColor = backColor;
			properties.Add(category, item);
		}
		public SyntaxHighlightProperties CalculateTokenCategoryHighlight(TokenCategory category) {
			SyntaxHighlightProperties result = null;
			if(properties.TryGetValue(category, out result))
				return result;
			else
				return properties[TokenCategory.Text];
		}
	}
	public class ClientSideEventsCommonFormDesigner : CommonFormDesigner {
		ClientSideEventsBase clientSideEvents;
		public ClientSideEventsCommonFormDesigner(object component, IServiceProvider provider, ClientSideEventsBase events)
			: base((ASPxWebControl)component, provider) {
			clientSideEvents = events;
		}
		protected override void CreateClientSideEventsItem() {
			var eventsHelper = new ClientSideEventsOwner(Control, Provider, clientSideEvents);
			MainGroup.Add(CreateDesignerItem("ClientSideEvents", ClientSideEventsCaption, typeof(ClientSideEventsFrame), Control, ClientSideEventsItemImageIndex, eventsHelper));
		}
	}
}

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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Linq;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerView_OutlookTemplate : DocumentManagerView_OutlookTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	DocumentManagerViewModelInfo viewModelData = templateInfo.Properties["IViewModelInfo"] as DocumentManagerViewModelInfo;
	string viewName = templateInfo.Properties["ViewName"].ToString();
	string peekCollectionViewName = templateInfo.Properties["RelatedViewName"].ToString();
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	XamlNamespaces xamlNamespaces = templateInfo.Properties["XamlNamespaces"] as XamlNamespaces;
	string viewModelPrefix = templateInfo.Properties["viewModelPrefix"].ToString();
			this.Write("<UserControl x:Class=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\"\r\n");
	PasteXamlNamespaces(xamlNamespaces);
			this.Write("\txmlns:view=\"clr-namespace:");
			this.Write(this.ToStringHelper.ToStringWithCulture(localNamespace));
			this.Write("\"\r\n\tmc:Ignorable=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(xamlNamespaces.GetPrefix(XamlNamespaces.xmlns_blend)));
			this.Write("\"\r\n    d:DesignHeight=\"600\" d:DesignWidth=\"800\" UseLayoutRounding=\"True\" DataCont" +
					"ext=\"{dxmvvm:ViewModelSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("}\">\r\n\t    <dxmvvm:Interaction.Behaviors>\r\n");
if(viewModelData.Tables.Any()) {
			this.Write("\t\t\t<dxmvvm:EventToCommand EventName=\"Initialized\" Command=\"{Binding OnLoadedComma" +
					"nd}\" CommandParameter=\"{Binding DefaultModule}\" />\r\n");
}
			this.Write("\t\t\t<dxmvvm:CurrentWindowService ClosingCommand=\"{Binding OnClosingCommand}\" />\r\n " +
					"           ");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_GenerateAdditionalBehaviors);
			this.Write("\t\t</dxmvvm:Interaction.Behaviors>\r\n    <UserControl.Resources>\r\n        <Resource" +
					"Dictionary>\r\n            <dxmvvm:BooleanNegationConverter x:Key=\"booleanNegation" +
					"Converter\"/>\r\n            <dxmvvm:ObjectToObjectConverter x:Key=\"NavigationPaneI" +
					"sMinimizedConverter\">\r\n                <dxmvvm:MapItem Target=\"True\" Source=\"Min" +
					"imized\"/>\r\n                <dxmvvm:MapItem Target=\"False\" Source=\"Normal\"/>\r\n   " +
					"             <dxmvvm:MapItem Target=\"False\" Source=\"Off\"/>\r\n            </dxmvvm" +
					":ObjectToObjectConverter>\r\n            <dxmvvm:ObjectToObjectConverter x:Key=\"Na" +
					"vigationPaneIsNormalConverter\">\r\n                <dxmvvm:MapItem Target=\"False\" " +
					"Source=\"Minimized\"/>\r\n                <dxmvvm:MapItem Target=\"True\" Source=\"Norm" +
					"al\"/>\r\n                <dxmvvm:MapItem Target=\"False\" Source=\"Off\"/>\r\n          " +
					"  </dxmvvm:ObjectToObjectConverter>\r\n            <dxmvvm:ObjectToObjectConverter" +
					" x:Key=\"NavigationPaneOffConverter\">\r\n                <dxmvvm:MapItem Target=\"Fa" +
					"lse\" Source=\"Minimized\"/>\r\n                <dxmvvm:MapItem Target=\"False\" Source" +
					"=\"Normal\"/>\r\n                <dxmvvm:MapItem Target=\"True\" Source=\"Off\"/>\r\n     " +
					"       </dxmvvm:ObjectToObjectConverter>\r\n\t\t\t<dxmvvm:ObjectToObjectConverter x:K" +
					"ey=\"NavigationPaneIsExpandedConverter\">\r\n                <dxmvvm:MapItem Target=" +
					"\"False\" Source=\"Minimized\"/>\r\n                <dxmvvm:MapItem Target=\"True\" Sour" +
					"ce=\"Normal\"/>\r\n            </dxmvvm:ObjectToObjectConverter>\r\n            <dxmvv" +
					"m:ObjectToObjectConverter x:Key=\"NavigationPaneVisibilityConverter\">\r\n          " +
					"      <dxmvvm:MapItem Target=\"Visible\" Source=\"Minimized\"/>\r\n                <dx" +
					"mvvm:MapItem Target=\"Visible\" Source=\"Normal\"/>\r\n                <dxmvvm:MapItem" +
					" Target=\"Collapsed\" Source=\"Off\"/>\r\n            </dxmvvm:ObjectToObjectConverter" +
					">\r\n        </ResourceDictionary>\r\n    </UserControl.Resources>\r\n    <Grid>      " +
					"  \r\n            <DockPanel>\r\n                <dxr:RibbonControl RibbonStyle=\"Off" +
					"ice2010\" DockPanel.Dock=\"Top\" AllowCustomization=\"False\">\r\n                    ");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_GenerateRibbonPageHeaderItems);
			this.Write(@"                    <dxr:RibbonControl.ToolbarItems>
                        <dxb:BarButtonItem Content=""Save All"" Command=""{Binding SaveAllCommand}"" LargeGlyph=""{dx:DXImageOffice2013 Image=SaveAll_32x32.png}"" Glyph=""{dx:DXImageOffice2013 Image=SaveAll_16x16.png}"" />
                    </dxr:RibbonControl.ToolbarItems>
                    <dxr:RibbonDefaultPageCategory Caption=""defaultCategory"">
                        <dxr:RibbonPage Caption=""VIEW"" MergeOrder=""1000"">
							<dxr:RibbonPageGroup Caption=""Module"">
                                <dxb:BarSubItem Content=""Navigation"" LargeGlyph=""{dx:DXImageOffice2013 Image=NavigationBar_32x32.png}"" Glyph=""{dx:DXImageOffice2013 Image=NavigationBar_16x16.png}"">
");
int i = 0; foreach(DocumentInfo info in viewModelData.Tables) {
			this.Write("\t                                <dxb:BarCheckItem GroupIndex=\"100\" Command=\"{Bin" +
					"ding ShowCommand}\" CommandParameter=\"{Binding Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(i++));
			this.Write("]}\" Content=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.Caption));
			this.Write("\"/>              \r\n");
}
			this.Write("\t\t\t\t\t\t\t\r\n                                </dxb:BarSubItem>\r\n                     " +
					"       </dxr:RibbonPageGroup>\r\n                            <dxr:RibbonPageGroup " +
					"Caption=\"Layout\" MergeOrder=\"1001\">\r\n                                <dxb:BarSub" +
					"Item Content=\"Folder Pane\"  LargeGlyph=\"{dx:DXImage Image=FolderPanel_32x32.png}" +
					"\" Glyph=\"{dx:DXImage Image=FolderPanel_16x16.png}\" >\r\n                          " +
					"          <dxb:BarCheckItem Content=\"Normal\" IsChecked=\"{Binding NavigationPaneV" +
					"isibility, Mode=TwoWay, Converter={StaticResource NavigationPaneIsNormalConverte" +
					"r}}\"/>\r\n                                    <dxb:BarCheckItem Content=\"Minimized" +
					"\" IsChecked=\"{Binding NavigationPaneVisibility, Mode=TwoWay, Converter={StaticRe" +
					"source NavigationPaneIsMinimizedConverter}}\"/>\r\n                                " +
					"    <dxb:BarCheckItem Content=\"Off\" IsChecked=\"{Binding NavigationPaneVisibility" +
					", Mode=TwoWay, Converter={StaticResource NavigationPaneOffConverter}}\"/>\r\n      " +
					"                          </dxb:BarSubItem>\r\n                            </dxr:R" +
					"ibbonPageGroup>\r\n                            <dxr:RibbonPageGroup Caption=\"Appea" +
					"rance\" MergeOrder=\"1002\">\r\n                                <dxr:RibbonGalleryBar" +
					"Item>\r\n                                    <dxmvvm:Interaction.Behaviors>\r\n     " +
					"                                   <dxr:RibbonGalleryItemThemeSelectorBehavior/>" +
					"\r\n                                    </dxmvvm:Interaction.Behaviors>\r\n         " +
					"                       </dxr:RibbonGalleryBarItem>\r\n                            " +
					"</dxr:RibbonPageGroup>\r\n                        </dxr:RibbonPage>\r\n             " +
					"       </dxr:RibbonDefaultPageCategory>\r\n                </dxr:RibbonControl>\r\n " +
					"               <dxr:RibbonStatusBarControl DockPanel.Dock=\"Bottom\">\r\n           " +
					"         <dxr:RibbonStatusBarControl.RightItems>\r\n                        <dxb:B" +
					"arCheckItem Content=\"Normal\" IsChecked=\"{Binding NavigationPaneVisibility, Mode=" +
					"TwoWay, Converter={StaticResource NavigationPaneIsNormalConverter}}\" Glyph=\"{dx:" +
					"DXImage Image=FolderPanel_16x16.png}\" RibbonStyle=\"SmallWithoutText\" />\r\n       " +
					"                 <dxb:BarCheckItem Content=\"Minimized\" IsChecked=\"{Binding Navig" +
					"ationPaneVisibility, Mode=TwoWay, Converter={StaticResource NavigationPaneIsMini" +
					"mizedConverter}}\" Glyph=\"{dx:DXImage Image=Reading_16x16.png}\" RibbonStyle=\"Smal" +
					"lWithoutText\" />\r\n                    </dxr:RibbonStatusBarControl.RightItems>\r\n" +
					"                </dxr:RibbonStatusBarControl>\r\n                <Grid Background=" +
					"\"Transparent\">\r\n\t\t\t\t\t<dxmvvm:Interaction.Behaviors>\r\n\t\t\t\t\t\t<dxmvvm:LayoutSeriali" +
					"zationService Name=\"RootLayoutSerializationService\" />\r\n\t\t\t\t\t\t<dxmvvm:CurrentWin" +
					"dowSerializationBehavior />\r\n\t\t\t\t\t</dxmvvm:Interaction.Behaviors>\r\n             " +
					"       <dxdo:DockLayoutManager MDIMergeStyle=\"WhenChildActivated\">\r\n            " +
					"            <dxdo:LayoutGroup Caption=\"LayoutRoot\" Orientation=\"Vertical\" ItemHe" +
					"ight=\"*\">\r\n\t\t\t\t\t\t\t<dxdo:LayoutGroup>\r\n                                <dxmvvm:In" +
					"teraction.Behaviors>\r\n                                    <dxdo:DockingDocumentU" +
					"IService x:Name=\"WorkspaceDocumentManagerService\">\r\n                            " +
					"            <dxdo:DockingDocumentUIService.LayoutPanelStyle>\r\n                  " +
					"                          <Style TargetType=\"dxdo:LayoutPanel\">\r\n               " +
					"                                 <Setter Property=\"AllowFloat\" Value=\"False\" />\r" +
					"\n                                                <Setter Property=\"AllowMove\" Va" +
					"lue=\"True\" />\r\n                                                <Setter Property=" +
					"\"ShowPinButton\" Value=\"False\" />\r\n                                              " +
					"  <Setter Property=\"ItemWidth\" Value=\"250\" />\r\n                                 " +
					"               <Setter Property=\"Padding\" Value=\"10,0\" />\r\n                     " +
					"                       </Style>\r\n                                        </dxdo:" +
					"DockingDocumentUIService.LayoutPanelStyle>\r\n                                    " +
					"</dxdo:DockingDocumentUIService>\r\n                                </dxmvvm:Inter" +
					"action.Behaviors>\r\n\t                            <dxdo:LayoutPanel Caption=\"Navig" +
					"ation\" ItemWidth=\"0.5*\" MaxWidth=\"200\" AllowClose=\"False\" AllowFloat=\"False\" Sho" +
					"wPinButton=\"False\" ShowBorder=\"False\" ShowCaption=\"False\" Visibility=\"{Binding N" +
					"avigationPaneVisibility, Converter={StaticResource NavigationPaneVisibilityConve" +
					"rter}}\">\r\n\t\t\t\t\t\t            <dxn:NavBarControl ItemsSource=\"{Binding Modules}\" S" +
					"electedGroup=\"{Binding SelectedModule}\" Compact=\"False\" x:Name=\"navBarControl\">\r" +
					"\n                                        <dxn:NavBarControl.Resources>\r\n        " +
					"                                    <dxmvvm:ObjectToObjectConverter x:Key=\"PeekF" +
					"ormTemplateConverter\">\r\n                                                <dxmvvm:" +
					"MapItem Source=\"{x:Null}\" Target=\"{x:Null}\" />\r\n                                " +
					"                <dxmvvm:ObjectToObjectConverter.DefaultTarget>\r\n                " +
					"                                    <DataTemplate>\r\n                            " +
					"                            <dxwui:Flyout ShowIndicator=\"True\" Command=\"{Binding" +
					" DataContext.PinPeekCollectionViewCommand, RelativeSource={RelativeSource FindAn" +
					"cestor, AncestorType=UserControl}}\" CommandParameter=\"{Binding}\">\r\n             " +
					"                                               <Grid Height=\"400\" Width=\"250\" x:" +
					"Name=\"grid\">\r\n                                                                <v" +
					"iew:PeekCollectionView dxmvvm:ViewModelExtensions.ParentViewModel=\"{Binding Data" +
					"Context, ElementName=grid}\" Margin=\"10\" dxmvvm:ViewModelExtensions.DocumentTitle" +
					"=\"{Binding DataContext.ModuleTitle, ElementName=grid}\" DataContext=\"{Binding Pee" +
					"kCollectionViewModel}\" />\r\n                                                     " +
					"       </Grid>\r\n                                                        </dxwui:" +
					"Flyout>\r\n                                                    </DataTemplate>\r\n  " +
					"                                              </dxmvvm:ObjectToObjectConverter.D" +
					"efaultTarget>\r\n                                            </dxmvvm:ObjectToObje" +
					"ctConverter>\r\n                                        </dxn:NavBarControl.Resour" +
					"ces>\r\n\t\t\t                            <dxn:NavBarControl.ItemTemplate>\r\n\t\t\t\t     " +
					"                       <DataTemplate>\r\n\t\t\t\t\t                            <Content" +
					"Control>\r\n\t\t\t\t\t\t                            <dxn:NavBarGroup Header=\"{Binding Mo" +
					"duleTitle}\" DisplaySource=\"Content\" DisplayMode=\"Text\" NavigationPaneVisible=\"Tr" +
					"ue\" NavPaneShowMode=\"All\"\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tPeekFormTemplate=\"{Binding PeekCollect" +
					"ionViewModel, Converter={StaticResource PeekFormTemplateConverter}}\">\r\n\t\t\t\t\t\t\t\t\t" +
					"\t\t\t\t\t<dxn:NavBarGroup.Content>\r\n");
if(!this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_OutlookUI_GenerateCustomNavBarGroupContent)) {
			this.Write("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t<TextBlock Text=\"{Binding ModuleTitle}\" HorizontalAlignment=\"Cente" +
					"r\" VerticalAlignment=\"Center\" />\r\n");
}
			this.Write("\t\t\t\t\t\t\t\t\t\t\t\t\t\t</dxn:NavBarGroup.Content>\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t</dxn:NavBarGroup>\r\n\t\t\t\t\t\t" +
					"\t                    </ContentControl>\r\n\t\t\t\t\t\t\t\t            </DataTemplate>\r\n\t\t\t" +
					"\t\t\t\t\t\t    </dxn:NavBarControl.ItemTemplate>\r\n\t\t\t\t\t\t\t\t\t\t<dxn:NavBarControl.View>\r" +
					"\n\t                                        <dxn:NavigationPaneView  IsExpanded=\"{" +
					"Binding NavigationPaneVisibility, Mode=TwoWay, Converter={StaticResource Navigat" +
					"ionPaneIsExpandedConverter}}\" />\r\n\t\t                                </dxn:NavBar" +
					"Control.View>\r\n\t\t\t                        </dxn:NavBarControl>\r\n\t\t\t\t            " +
					"    </dxdo:LayoutPanel>\r\n                                <dxdo:LayoutPanel Allow" +
					"Close=\"False\" AllowFloat=\"False\" ShowPinButton=\"False\" ShowBorder=\"False\" ShowCa" +
					"ption=\"False\">\r\n                                    <dxwui:NavigationFrame Anima" +
					"tionType=\"SlideHorizontal\" AllowMerging=\"True\">\r\n                               " +
					"         <dxmvvm:Interaction.Behaviors>\r\n                                       " +
					"     <dxwuin:FrameDocumentUIService>\r\n                                          " +
					"      <dxwuin:FrameDocumentUIService.PageAdornerControlStyle>\r\n                 " +
					"                                   <Style TargetType=\"dxwui:PageAdornerControl\">" +
					"\r\n                                                        <Setter Property=\"Temp" +
					"late\">\r\n                                                            <Setter.Valu" +
					"e>\r\n                                                                <ControlTemp" +
					"late TargetType=\"dxwui:PageAdornerControl\">\r\n                                   " +
					"                                 <ContentPresenter />\r\n                         " +
					"                                       </ControlTemplate>\r\n                     " +
					"                                       </Setter.Value>\r\n                        " +
					"                                </Setter>\r\n                                     " +
					"               </Style>\r\n                                                </dxwui" +
					"n:FrameDocumentUIService.PageAdornerControlStyle>\r\n                             " +
					"               </dxwuin:FrameDocumentUIService>\r\n                               " +
					"         </dxmvvm:Interaction.Behaviors>\r\n                                    </" +
					"dxwui:NavigationFrame>\r\n                                </dxdo:LayoutPanel>\r\n\t\t " +
					"                   </dxdo:LayoutGroup>\r\n\t\t\t                <dxdo:LayoutPanel All" +
					"owClose=\"False\" AllowFloat=\"False\" ShowPinButton=\"False\" ShowBorder=\"False\" Show" +
					"Caption=\"False\" ItemHeight=\"Auto\">\r\n\t\t\t\t\t            <dxnav:OfficeNavigationBar " +
					"NavigationClient=\"{Binding ElementName=navBarControl}\" AllowItemDragDrop=\"True\" " +
					"/>\r\n                            </dxdo:LayoutPanel>\r\n\t\t\t\t\t\t</dxdo:LayoutGroup>\r\n" +
					"                    </dxdo:DockLayoutManager>\r\n                </Grid>\r\n        " +
					"    </DockPanel>        \r\n    </Grid>\r\n</UserControl>\r\n");
			return this.GenerationEnvironment.ToString();
		}
void PasteXamlNamespaces(XamlNamespaces xamlNamespaces) {
	string[] toAdd = new string[]{
		XmlNamespaceConstants.RibbonNamespaceDefinition,
		XmlNamespaceConstants.BarsNamespaceDefinition,
		XmlNamespaceConstants.UtilsNamespaceDefinition,
		XmlNamespaceConstants.EditorsNamespaceDefinition,
		XmlNamespaceConstants.GridNamespaceDefinition,
		XmlNamespaceConstants.MvvmNamespaceDefinition,
		XmlNamespaceConstants.DockingNamespaceDefinition,
		XmlNamespaceConstants.NavBarNamespaceDefinition,
		XmlNamespaceConstants.WindowsUINamespaceDefinition,
		XmlNamespaceConstants.WindowsUINavigationNamespaceDefinition,
		XmlNamespaceConstants.NavigationNamespaceDefinition,
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_OutlookTemplateBase
	{
		#region Fields
		private global::System.Text.StringBuilder generationEnvironmentField;
		private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
		private global::System.Collections.Generic.List<int> indentLengthsField;
		private string currentIndentField = "";
		private bool endsWithNewline;
		private global::System.Collections.Generic.IDictionary<string, object> sessionField;
		#endregion
		#region Properties
		protected System.Text.StringBuilder GenerationEnvironment
		{
			get
			{
				if ((this.generationEnvironmentField == null))
				{
					this.generationEnvironmentField = new global::System.Text.StringBuilder();
				}
				return this.generationEnvironmentField;
			}
			set
			{
				this.generationEnvironmentField = value;
			}
		}
		public System.CodeDom.Compiler.CompilerErrorCollection Errors
		{
			get
			{
				if ((this.errorsField == null))
				{
					this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
				}
				return this.errorsField;
			}
		}
		private System.Collections.Generic.List<int> indentLengths
		{
			get
			{
				if ((this.indentLengthsField == null))
				{
					this.indentLengthsField = new global::System.Collections.Generic.List<int>();
				}
				return this.indentLengthsField;
			}
		}
		public string CurrentIndent
		{
			get
			{
				return this.currentIndentField;
			}
		}
		public virtual global::System.Collections.Generic.IDictionary<string, object> Session
		{
			get
			{
				return this.sessionField;
			}
			set
			{
				this.sessionField = value;
			}
		}
		#endregion
		#region Transform-time helpers
		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			if (((this.GenerationEnvironment.Length == 0) 
						|| this.endsWithNewline))
			{
				this.GenerationEnvironment.Append(this.currentIndentField);
				this.endsWithNewline = false;
			}
			if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
			{
				this.endsWithNewline = true;
			}
			if ((this.currentIndentField.Length == 0))
			{
				this.GenerationEnvironment.Append(textToAppend);
				return;
			}
			textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
			if (this.endsWithNewline)
			{
				this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
			}
			else
			{
				this.GenerationEnvironment.Append(textToAppend);
			}
		}
		public void WriteLine(string textToAppend)
		{
			this.Write(textToAppend);
			this.GenerationEnvironment.AppendLine();
			this.endsWithNewline = true;
		}
		public void Write(string format, params object[] args)
		{
			this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void WriteLine(string format, params object[] args)
		{
			this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void Error(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			this.Errors.Add(error);
		}
		public void Warning(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			error.IsWarning = true;
			this.Errors.Add(error);
		}
		public void PushIndent(string indent)
		{
			if ((indent == null))
			{
				throw new global::System.ArgumentNullException("indent");
			}
			this.currentIndentField = (this.currentIndentField + indent);
			this.indentLengths.Add(indent.Length);
		}
		public string PopIndent()
		{
			string returnValue = "";
			if ((this.indentLengths.Count > 0))
			{
				int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
				this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
				if ((indentLength > 0))
				{
					returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
					this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
				}
			}
			return returnValue;
		}
		public void ClearIndent()
		{
			this.indentLengths.Clear();
			this.currentIndentField = "";
		}
		#endregion
		#region ToString Helpers
		public class ToStringInstanceHelper
		{
			private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
			public System.IFormatProvider FormatProvider
			{
				get
				{
					return this.formatProviderField ;
				}
				set
				{
					if ((value != null))
					{
						this.formatProviderField  = value;
					}
				}
			}
			public string ToStringWithCulture(object objectToConvert)
			{
				if ((objectToConvert == null))
				{
					throw new global::System.ArgumentNullException("objectToConvert");
				}
				System.Type t = objectToConvert.GetType();
				System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
							typeof(System.IFormatProvider)});
				if ((method == null))
				{
					return objectToConvert.ToString();
				}
				else
				{
					return ((string)(method.Invoke(objectToConvert, new object[] {
								this.formatProviderField })));
				}
			}
		}
		private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
		public ToStringInstanceHelper ToStringHelper
		{
			get
			{
				return this.toStringHelperField;
			}
		}
		#endregion
	}
	#endregion
}

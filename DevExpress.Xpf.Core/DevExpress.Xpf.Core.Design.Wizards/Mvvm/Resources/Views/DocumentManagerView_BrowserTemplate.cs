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
	public partial class DocumentManagerView_BrowserTemplate : DocumentManagerView_BrowserTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	DocumentManagerViewModelInfo viewModelData = templateInfo.Properties["IViewModelInfo"] as DocumentManagerViewModelInfo;
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	XamlNamespaces xamlNamespaces = templateInfo.Properties["XamlNamespaces"] as XamlNamespaces;
	string viewModelPrefix = templateInfo.Properties["viewModelPrefix"].ToString();
			this.Write("<UserControl x:Class=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\"\r\n");
	PasteXamlNamespaces(xamlNamespaces);
			this.Write("\txmlns:pfdata=\"clr-namespace:System.Windows.Data;assembly=PresentationFramework\"\r" +
					"\n    mc:Ignorable=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(xamlNamespaces.GetPrefix(XamlNamespaces.xmlns_blend)));
			this.Write("\"\r\n    d:DesignHeight=\"600\" d:DesignWidth=\"800\" UseLayoutRounding=\"True\" DataCont" +
					"ext=\"{dxmvvm:ViewModelSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("}\">\r\n\t<dxmvvm:Interaction.Behaviors>\r\n\t\t<dx:DXMessageBoxService />\r\n");
if(viewModelData.Tables.Any()) {
			this.Write("\t\t<dxmvvm:EventToCommand EventName=\"Initialized\" Command=\"{Binding OnLoadedComman" +
					"d}\" />\r\n");
}
			this.Write("\t\t<dxmvvm:TabbedWindowLayoutSerializationService Name=\"RootLayoutSerializationSer" +
					"vice\" />\r\n        <dxmvvm:CurrentWindowSerializationBehavior />\r\n        ");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_GenerateAdditionalBehaviors);
			this.Write("\t</dxmvvm:Interaction.Behaviors>\r\n\t<UserControl.Resources>\r\n        <Style x:Key=" +
					"\"speedDialTileStyle\" TargetType=\"dxlc:Tile\">\r\n            <Setter Property=\"Widt" +
					"h\" Value=\"210\" />\r\n            <Setter Property=\"Height\" Value=\"150\" />\r\n       " +
					"     <Setter Property=\"ContentTemplate\">\r\n                <Setter.Value>\r\n      " +
					"              <DataTemplate>\r\n                        <TextBlock Text=\"{Binding " +
					"ModuleTitle}\" TextWrapping=\"Wrap\" TextAlignment=\"Center\" />\r\n                   " +
					" </DataTemplate>\r\n                </Setter.Value>\r\n            </Setter>\r\n      " +
					"      <Setter Property=\"VerticalContentAlignment\" Value=\"Center\" />\r\n           " +
					" <Setter Property=\"Command\" Value=\"{Binding DataContext.ShowCommand, RelativeSou" +
					"rce={RelativeSource AncestorType=dxlc:TileLayoutControl}}\" />\r\n            <Sett" +
					"er Property=\"CommandParameter\" Value=\"{Binding}\" />\r\n            <Setter Propert" +
					"y=\"FontSize\" Value=\"20\" />\r\n            <Setter Property=\"Effect\">\r\n            " +
					"    <Setter.Value>\r\n                    <DropShadowEffect ShadowDepth=\"0\" Color=" +
					"\"#FF2980D1\" Opacity=\"0\" BlurRadius=\"0\"/>\r\n                </Setter.Value>\r\n     " +
					"       </Setter>\r\n            <Style.Triggers>\r\n                <Trigger Propert" +
					"y=\"IsMouseOver\" Value=\"True\">\r\n                    <Trigger.EnterActions>\r\n     " +
					"                   <BeginStoryboard>\r\n                            <Storyboard>\r\n" +
					"                                <DoubleAnimation Storyboard.TargetProperty=\"Effe" +
					"ct.BlurRadius\" To=\"14\" Duration=\"0:0:0.25\"/>\r\n                                <D" +
					"oubleAnimation Storyboard.TargetProperty=\"Effect.Opacity\" To=\"0.6\" Duration=\"0:0" +
					":0.20\"/>\r\n                            </Storyboard>\r\n                        </B" +
					"eginStoryboard>\r\n                    </Trigger.EnterActions>\r\n                  " +
					"  <Trigger.ExitActions>\r\n                        <BeginStoryboard>\r\n            " +
					"                <Storyboard>\r\n                                <DoubleAnimation S" +
					"toryboard.TargetProperty=\"Effect.BlurRadius\" To=\"0\" Duration=\"0:0:0.14\"/>\r\n     " +
					"                           <DoubleAnimation Storyboard.TargetProperty=\"Effect.Op" +
					"acity\" To=\"0\" Duration=\"0:0:0.14\"/>\r\n                            </Storyboard>\r\n" +
					"                        </BeginStoryboard>\r\n                    </Trigger.ExitAc" +
					"tions>\r\n                </Trigger>\r\n            </Style.Triggers>\r\n        </Sty" +
					"le>\r\n    </UserControl.Resources>\r\n    <Grid>\r\n        <dx:DXTabControl x:Name=\"" +
					"tabControl\" Padding=\"0,-1,0,0\" TabContentCacheMode=\"CacheAllTabs\" dx:DXSerialize" +
					"r.Enabled=\"True\">\r\n            <dx:DXTabControl.Resources>\r\n                <Sty" +
					"le TargetType=\"{x:Type dx:DXTabItem}\">\r\n                    <Setter Property=\"He" +
					"aderTemplate\">\r\n                        <Setter.Value>\r\n                        " +
					"    <DataTemplate>\r\n                                <TextBlock Text=\"{Binding}\" " +
					"TextTrimming=\"CharacterEllipsis\">\r\n                                    <TextBloc" +
					"k.ToolTip>\r\n                                        <ToolTip Content=\"{Binding}\"" +
					"/>\r\n                                    </TextBlock.ToolTip>\r\n                  " +
					"              </TextBlock>\r\n                            </DataTemplate>\r\n       " +
					"                 </Setter.Value>\r\n                    </Setter>\r\n               " +
					" </Style>\r\n            </dx:DXTabControl.Resources>\r\n            <dx:DXTabContro" +
					"l.ContentHeaderTemplate>\r\n                <DataTemplate>\r\n                    <d" +
					"xr:RibbonControl Visibility=\"{Binding RelativeSource={RelativeSource AncestorTyp" +
					"e=dx:DXTabControl}, Path=SelectedContainer.IsNew, Converter={dxmvvm:BooleanToVis" +
					"ibilityConverter Inverse=True}}\" \r\n                                       dx:DXS" +
					"erializer.Enabled=\"False\"\r\n                                       RibbonStyle=\"O" +
					"fficeSlim\" \r\n                                       ShowApplicationButton=\"False" +
					"\" \r\n                                       AllowMinimizeRibbon=\"False\" \r\n       " +
					"                                RibbonHeaderVisibility=\"Collapsed\" \r\n           " +
					"                            ToolbarShowMode=\"Hide\" \r\n                           " +
					"            AllowCustomization=\"False\">\r\n                        <dxr:RibbonDefa" +
					"ultPageCategory Caption=\"defaultCategory\">\r\n                            <dxr:Rib" +
					"bonPage>\r\n                                <dxr:RibbonPageGroup />\r\n             " +
					"               </dxr:RibbonPage>\r\n                        </dxr:RibbonDefaultPag" +
					"eCategory>\r\n                    </dxr:RibbonControl>\r\n                </DataTemp" +
					"late>\r\n            </dx:DXTabControl.ContentHeaderTemplate>\r\n            <dx:DXT" +
					"abControl.ContentFooterTemplate>\r\n                <DataTemplate>\r\n              " +
					"      <dxr:RibbonStatusBarControl Visibility=\"{Binding RelativeSource={RelativeS" +
					"ource AncestorType=dx:DXTabControl}, Path=SelectedContainer.IsNew, Converter={dx" +
					"mvvm:BooleanToVisibilityConverter Inverse=True}}\" />\r\n                </DataTemp" +
					"late>\r\n            </dx:DXTabControl.ContentFooterTemplate>\r\n            <dx:DXT" +
					"abControl.ControlBoxRightTemplate>\r\n                <DataTemplate>\r\n            " +
					"        <dxb:ToolBarControl BackgroundTemplate=\"{x:Null}\" GlyphSize=\"Small\" Show" +
					"DragWidget=\"False\" AllowCollapse=\"False\" AllowCustomizationMenu=\"False\" AllowHid" +
					"e=\"False\" AllowQuickCustomization=\"False\" AllowRename=\"False\">\r\n                " +
					"        <dxb:BarButtonItem Content=\"Save All\" Command=\"{Binding SaveAllCommand}\"" +
					" LargeGlyph=\"{dx:DXImage Image=SaveAll_32x32.png}\" Glyph=\"{dx:DXImage Image=Save" +
					"All_16x16.png}\" />\r\n                        <dxb:BarSubItem Glyph=\"{dx:DXImage I" +
					"mage=Colors_16x16.png}\" LargeGlyph=\"{dx:DXImage Image=Colors_32x32.png}\">\r\n     " +
					"                       <dxmvvm:Interaction.Behaviors>\r\n                         " +
					"       <dxb:BarSubItemThemeSelectorBehavior />\r\n                            </dx" +
					"mvvm:Interaction.Behaviors>\r\n                        </dxb:BarSubItem>\r\n        " +
					"            </dxb:ToolBarControl>\r\n                </DataTemplate>\r\n            " +
					"</dx:DXTabControl.ControlBoxRightTemplate>\r\n            <dxmvvm:Interaction.Beha" +
					"viors>\r\n                <dx:TabbedWindowDocumentUIService WindowClosingCommand=\"" +
					"{Binding OnClosingCommand}\">\r\n                    <dx:TabbedWindowDocumentUIServ" +
					"ice.NewItemTemplate>\r\n                        <DataTemplate>\r\n                  " +
					"          <dx:DXTabItem Header=\"Speed Dial\">\r\n                                <d" +
					"xlc:TileLayoutControl ItemsSource=\"{Binding Modules}\" ItemContainerStyle=\"{Stati" +
					"cResource speedDialTileStyle}\" Orientation=\"Horizontal\" AllowItemMoving=\"False\" " +
					"VerticalAlignment=\"Center\" ItemSpace=\"25\" Margin=\"0,1,0,0\" Padding=\"40\" />\r\n    " +
					"                        </dx:DXTabItem>\r\n                        </DataTemplate>" +
					"\r\n                    </dx:TabbedWindowDocumentUIService.NewItemTemplate>\r\n     " +
					"           </dx:TabbedWindowDocumentUIService>\r\n            </dxmvvm:Interaction" +
					".Behaviors>\r\n            <dx:DXTabControl.View>\r\n                <dx:TabControlS" +
					"tretchView MoveItemsWhenDragDrop=\"False\" RemoveTabItemsOnHiding=\"True\" DragDropM" +
					"ode=\"Full\" HideButtonShowMode=\"InAllTabs\" NewButtonShowMode=\"InTabPanel\" CloseWi" +
					"ndowOnSingleTabItemHiding=\"True\" SingleTabItemHideMode=\"HideAndShowNewItem\" />\r\n" +
					"            </dx:DXTabControl.View>\r\n        </dx:DXTabControl>\r\n    </Grid>\r\n</" +
					"UserControl>\r\n");
			return this.GenerationEnvironment.ToString();
		}
void PasteXamlNamespaces(XamlNamespaces xamlNamespaces) {
	string[] toAdd = new string[]{
		XmlNamespaceConstants.RibbonNamespaceDefinition,
		XmlNamespaceConstants.EditorsNamespaceDefinition,
		XmlNamespaceConstants.LayoutControlNamespaceDefinition,
		XmlNamespaceConstants.BarsNamespaceDefinition,
		XmlNamespaceConstants.UtilsNamespaceDefinition,
		XmlNamespaceConstants.GridNamespaceDefinition,
		XmlNamespaceConstants.MvvmNamespaceDefinition,
		XmlNamespaceConstants.DockingNamespaceDefinition,
		XmlNamespaceConstants.NavBarNamespaceDefinition,
		XmlNamespaceConstants.WindowsUINamespaceDefinition,
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_BrowserTemplateBase
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

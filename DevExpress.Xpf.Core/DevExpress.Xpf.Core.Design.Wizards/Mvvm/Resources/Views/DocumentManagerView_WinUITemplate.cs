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
	public partial class DocumentManagerView_WinUITemplate : DocumentManagerView_WinUITemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	DocumentManagerViewModelInfo viewModelData = templateInfo.Properties["IViewModelInfo"] as DocumentManagerViewModelInfo;
	string[] tileColors = new string[] { "#FF00879C", "#FF404040", "#FFCC6D00", "#FF0073C4", "#FF3E7038" };
	string viewModelNamespace = viewModelData.Namespace;
	if(!viewModelData.IsLocalType && !string.IsNullOrEmpty(viewModelData.AssemblyName))
		viewModelNamespace += ";assembly="+ viewModelData.AssemblyName;
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
			this.Write("\"\r\n\tdx:ScrollBarExtensions.ScrollBarMode=\"TouchOverlap\"\r\n    d:DesignHeight=\"600\"" +
					" d:DesignWidth=\"800\" DataContext=\"{dxmvvm:ViewModelSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("}\">\r\n\t<UserControl.Resources>\r\n\t\t<dxmvvm:ObjectToObjectConverter x:Key=\"TileColor" +
					"Converter\">\r\n");
for(int tileColorIndex = 0; tileColorIndex < tileColors.Length; ++tileColorIndex) {
			this.Write("            <dxmvvm:MapItem Source=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(tileColorIndex));
			this.Write("\" Target=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(tileColors[tileColorIndex]));
			this.Write("\" />\r\n");
}
			this.Write("\t\t</dxmvvm:ObjectToObjectConverter>\r\n\t</UserControl.Resources>\r\n    <dxmvvm:Inter" +
					"action.Behaviors>\r\n");
if(viewModelData.Tables.Any()) {
			this.Write("\t\t<dxmvvm:EventToCommand EventName=\"Initialized\" Command=\"{Binding OnLoadedComman" +
					"d}\" CommandParameter=\"{Binding DefaultModule}\" />\r\n");
}
			this.Write("        <dxmvvm:CurrentWindowService ClosingCommand=\"{Binding OnClosingCommand}\" " +
					"/>\r\n\t\t<dxmvvm:LayoutSerializationService Name=\"RootLayoutSerializationService\" /" +
					">\r\n\t\t<dxmvvm:CurrentWindowSerializationBehavior />\r\n\t\t");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_GenerateAdditionalBehaviors);
			this.Write(@"    </dxmvvm:Interaction.Behaviors>
	<dx:BackgroundPanel>
        <DockPanel>
            <DockPanel.Resources>
                <CollectionViewSource x:Key=""ItemsSource"" Source=""{Binding Modules}"">
                    <CollectionViewSource.GroupDescriptions>
                        <pfdata:PropertyGroupDescription PropertyName=""ModuleGroup"" />
                    </CollectionViewSource.GroupDescriptions>
                </CollectionViewSource>
            </DockPanel.Resources>
            <dxnav:TileBar ItemsSource=""{Binding Source={StaticResource ItemsSource}}"" DockPanel.Dock=""Top"" Padding=""0,0,0,27"" Background=""#FFE8E8E8"" AlternationCount=""");
			this.Write(this.ToStringHelper.ToStringWithCulture(tileColors.Length));
			this.Write(@""" ShowGroupHeaders=""False"" SelectedItem=""{Binding SelectedModule}"">
				<dxnav:TileBar.ItemContainerStyle>
					<Style TargetType=""dxnav:TileBarItem"">
						<Setter Property=""Width"" Value=""166"" />
						<Setter Property=""AllowGlyphTheming"" Value=""True"" />
						<Setter Property=""Content"" Value=""{Binding ModuleTitle}"" />
						<Setter Property=""TileGlyph"" Value=""");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.ExpandMarkupExtension(new DXImageGrayscaleExtension() { Image = (DXImageInfo)new DXImageGrayscaleConverter().ConvertFrom("Cube_16x16.png") })));
			this.Write("\" />\r\n\t\t\t\t\t\t<Setter Property=\"Background\" Value=\"{Binding Path=(ItemsControl.Alte" +
					"rnationIndex), RelativeSource={RelativeSource Self}, Converter={StaticResource T" +
					"ileColorConverter}}\" />\r\n\t\t\t\t\t\t");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_WinUI_GenerateAdditionalTileBarItemSettings);
			this.Write("\t\t\t\t\t</Style>\r\n\t\t\t\t</dxnav:TileBar.ItemContainerStyle>\r\n                <dxnav:Ti" +
					"leBar.GroupStyle>\r\n                    <GroupStyle>\r\n                        <Gr" +
					"oupStyle.HeaderTemplate>\r\n                            <DataTemplate>\r\n          " +
					"                      <TextBlock Text=\"{Binding Name, Converter={dxmvvm:Criteria" +
					"OperatorConverter Expression=Upper(This)}}\" Margin=\"9,28,0,5\" />\r\n              " +
					"              </DataTemplate>\r\n                        </GroupStyle.HeaderTempla" +
					"te>\r\n                        <GroupStyle.Panel>\r\n                            <It" +
					"emsPanelTemplate>\r\n                                <dxnavi:TileBarItemsPanel Ori" +
					"entation=\"Horizontal\" />\r\n                            </ItemsPanelTemplate>\r\n   " +
					"                     </GroupStyle.Panel>\r\n                    </GroupStyle>\r\n   " +
					"             </dxnav:TileBar.GroupStyle>\r\n            </dxnav:TileBar>\r\n\t\t\t<dxwu" +
					"i:NavigationFrame AnimationType=\"SlideHorizontal\">\r\n\t\t\t\t<dxmvvm:Interaction.Beha" +
					"viors>\r\n                    <dxwuin:FrameDocumentUIService>\r\n                   " +
					"     <dxwuin:FrameDocumentUIService.PageAdornerControlStyle>\r\n                  " +
					"          <Style TargetType=\"dxwui:PageAdornerControl\">\r\n                       " +
					"         <Setter Property=\"HeaderTemplate\">\r\n                                   " +
					" <Setter.Value>\r\n                                        <DataTemplate>\r\n       " +
					"                                     <TextBlock Text=\"{Binding}\" FontSize=\"18\" M" +
					"argin=\"5,0,0,0\" />\r\n                                        </DataTemplate>\r\n   " +
					"                                 </Setter.Value>\r\n                              " +
					"  </Setter>\r\n                                <Setter Property=\"Template\">\r\n     " +
					"                               <Setter.Value>\r\n                                 " +
					"       <ControlTemplate TargetType=\"dxwui:PageAdornerControl\">\r\n                " +
					"                            <Border Background=\"{TemplateBinding Background}\" Bo" +
					"rderThickness=\"{TemplateBinding BorderThickness}\" BorderBrush=\"{TemplateBinding " +
					"BorderBrush}\" Margin=\"{TemplateBinding Padding}\">\r\n                             " +
					"                   <Grid>\r\n                                                    <" +
					"Grid.RowDefinitions>\r\n                                                        <R" +
					"owDefinition Height=\"Auto\" />\r\n                                                 " +
					"       <RowDefinition />\r\n                                                    </" +
					"Grid.RowDefinitions>\r\n                                                        <d" +
					"xwuii:NavigationHeaderControl Margin=\"20,10,10,8\" VerticalAlignment=\"Center\" x:N" +
					"ame=\"PART_NavigationHeader\" Content=\"{TemplateBinding Header}\" ContentTemplate=\"" +
					"{TemplateBinding HeaderTemplate}\" BackCommand=\"{TemplateBinding BackCommand}\" Sh" +
					"owBackButton=\"{TemplateBinding ShowBackButton}\" />\r\n                            " +
					"                        <ContentPresenter Grid.Row=\"1\" />\r\n                     " +
					"                           </Grid>\r\n                                            " +
					"</Border>\r\n                                        </ControlTemplate>\r\n         " +
					"                           </Setter.Value>\r\n                                </Se" +
					"tter>\r\n                            </Style>\r\n                        </dxwuin:Fr" +
					"ameDocumentUIService.PageAdornerControlStyle>\r\n                    </dxwuin:Fram" +
					"eDocumentUIService>\r\n\t\t\t\t</dxmvvm:Interaction.Behaviors>\r\n\t\t\t</dxwui:NavigationF" +
					"rame>\r\n        </DockPanel>\r\n\t</dx:BackgroundPanel>\r\n</UserControl>\r\n\r\n");
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
		XmlNamespaceConstants.WindowsUINavigationNamespaceDefinition,
		XmlNamespaceConstants.WindowsUIInternalNamespaceDefinition,
		XmlNamespaceConstants.NavigationNamespaceDefinition,
		XmlNamespaceConstants.NavigationInternalNamespaceDefinition
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_WinUITemplateBase
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

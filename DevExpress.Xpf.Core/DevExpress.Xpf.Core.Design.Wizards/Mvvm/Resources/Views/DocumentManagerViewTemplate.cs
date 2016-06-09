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
	public partial class DocumentManagerViewTemplate : DocumentManagerViewTemplateBase
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
			this.Write("}\">\r\n\t    <dxmvvm:Interaction.Behaviors>\r\n");
if(viewModelData.Tables.Any()) {
			this.Write("\t\t\t<dxmvvm:EventToCommand EventName=\"Initialized\" Command=\"{Binding OnLoadedComma" +
					"nd}\" CommandParameter=\"{Binding DefaultModule}\" />\r\n");
}
			this.Write("\t\t\t<dxmvvm:CurrentWindowService ClosingCommand=\"{Binding OnClosingCommand}\" />\r\n " +
					"           ");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_GenerateAdditionalBehaviors);
			this.Write("\t\t</dxmvvm:Interaction.Behaviors>\r\n    <Grid>\r\n            <DockPanel>\r\n         " +
					"       <dxr:RibbonControl RibbonStyle=\"Office2010\" DockPanel.Dock=\"Top\" AllowCus" +
					"tomization=\"False\">\r\n                    ");
this.ExecuteDocumentManagerViewHook(TemplatesCodeGen.STR_DocumentManagerViewHook_GenerateRibbonPageHeaderItems);
			this.Write("                    <dxr:RibbonControl.ToolbarItems>\r\n                        <dx" +
					"b:BarButtonItem Content=\"Save All\" Command=\"{Binding SaveAllCommand}\" LargeGlyph" +
					"=\"{dx:DXImage Image=SaveAll_32x32.png}\" Glyph=\"{dx:DXImage Image=SaveAll_16x16.p" +
					"ng}\" />\r\n                    </dxr:RibbonControl.ToolbarItems>\r\n                " +
					"    <dxr:RibbonDefaultPageCategory Caption=\"defaultCategory\">\r\n                 " +
					"       <dxr:RibbonPage Caption=\"View\" MergeOrder=\"1000\">\r\n                      " +
					"      <dxr:RibbonPageGroup Caption=\"Appearance\">\r\n                              " +
					"  <dxr:RibbonGalleryBarItem>\r\n                                    <dxmvvm:Intera" +
					"ction.Behaviors>\r\n                                        <dxr:RibbonGalleryItem" +
					"ThemeSelectorBehavior/>\r\n                                    </dxmvvm:Interactio" +
					"n.Behaviors>\r\n                                </dxr:RibbonGalleryBarItem>\r\n     " +
					"                       </dxr:RibbonPageGroup>\r\n                        </dxr:Rib" +
					"bonPage>\r\n                    </dxr:RibbonDefaultPageCategory>\r\n                " +
					"</dxr:RibbonControl>\r\n                <dxr:RibbonStatusBarControl DockPanel.Dock" +
					"=\"Bottom\"/>\r\n                <Grid Background=\"Transparent\">\r\n\t\t\t\t    <dxmvvm:In" +
					"teraction.Behaviors>\r\n\t\t\t\t\t\t<dxmvvm:LayoutSerializationService Name=\"RootLayoutS" +
					"erializationService\" />\r\n\t\t\t\t\t\t<dxmvvm:CurrentWindowSerializationBehavior />\r\n\t\t" +
					"\t\t\t</dxmvvm:Interaction.Behaviors>\r\n                    <dxdo:DockLayoutManager " +
					"MDIMergeStyle=\"WhenChildActivated\">\r\n                        <dxdo:LayoutGroup C" +
					"aption=\"LayoutRoot\">\r\n                            <dxdo:LayoutPanel Caption=\"Nav" +
					"igation\" ItemWidth=\"0.5*\" MaxWidth=\"300\">\r\n\t\t\t\t\t\t\t\t<dxdo:LayoutPanel.Resources>\r" +
					"\n\t\t\t\t\t\t\t\t\t<CollectionViewSource x:Key=\"ItemsSource\" Source=\"{Binding Modules}\">\r" +
					"\n\t\t\t\t\t\t\t\t\t\t<CollectionViewSource.GroupDescriptions>\r\n\t\t\t\t\t\t\t\t\t\t\t<pfdata:Property" +
					"GroupDescription PropertyName=\"ModuleGroup\" />\r\n\t\t\t\t\t\t\t\t\t\t</CollectionViewSource" +
					".GroupDescriptions>\r\n\t\t\t\t\t\t\t\t\t</CollectionViewSource>\r\n\t\t\t\t\t\t\t\t</dxdo:LayoutPane" +
					"l.Resources>\r\n                                <dxn:NavBarControl ItemsSource=\"{B" +
					"inding Groups, Source={StaticResource ItemsSource}}\" SelectedItem=\"{Binding Sele" +
					"ctedModule}\">\r\n\t\t\t\t\t\t\t\t\t<dxn:NavBarControl.Resources>\r\n\t                        " +
					"            <DataTemplate x:Key=\"ItemTemplate\">\r\n\t\t\t\t\t\t\t\t\t\t\t<ContentControl>\r\n\t\t" +
					"\t\t\t\t\t\t\t\t\t\t<dxn:NavBarItem Content=\"{Binding ModuleTitle}\" />\r\n                  " +
					"                          </ContentControl>\r\n                                   " +
					"     </DataTemplate>\r\n\t\t\t\t\t\t\t\t\t</dxn:NavBarControl.Resources>\r\n                 " +
					"                   <dxn:NavBarControl.ItemTemplate>\r\n                           " +
					"             <DataTemplate>\r\n                                            <Conten" +
					"tControl>\r\n                                                <dxn:NavBarGroup Head" +
					"er=\"{Binding}\" ItemTemplate=\"{StaticResource ItemTemplate}\" />\r\n                " +
					"                            </ContentControl>\r\n                                 " +
					"       </DataTemplate>\r\n                                    </dxn:NavBarControl." +
					"ItemTemplate>\r\n                                    <dxn:NavBarControl.View>\r\n   " +
					"                                     <dxn:NavigationPaneView IsExpandButtonVisib" +
					"le=\"False\"/>\r\n                                    </dxn:NavBarControl.View>\r\n   " +
					"                             </dxn:NavBarControl>\r\n                            <" +
					"/dxdo:LayoutPanel>\r\n                            <dxdo:DocumentGroup x:Name=\"docu" +
					"mentGroup\">\r\n\t\t\t\t\t\t\t    <dxmvvm:Interaction.Behaviors>\r\n\t\t\t\t\t\t\t\t\t<dxdo:TabbedDoc" +
					"umentUIService />\r\n\t\t\t\t\t\t\t\t</dxmvvm:Interaction.Behaviors>\r\n\t\t\t\t\t\t\t</dxdo:Docume" +
					"ntGroup>\r\n                        </dxdo:LayoutGroup>\r\n                    </dxd" +
					"o:DockLayoutManager>\r\n                </Grid>\r\n            </DockPanel>\r\n    </G" +
					"rid>\r\n</UserControl>\r\n");
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
		XmlNamespaceConstants.NavBarNamespaceDefinition
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerViewTemplateBase
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

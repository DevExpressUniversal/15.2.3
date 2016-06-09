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
	public partial class PeekCollectionViewTemplate : PeekCollectionViewTemplateBase
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
			this.Write("\tmc:Ignorable=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(xamlNamespaces.GetPrefix(XamlNamespaces.xmlns_blend)));
			this.Write("\"\r\n    d:DesignHeight=\"400\" d:DesignWidth=\"600\">\r\n\t<UserControl.Resources>\r\n     " +
					"   <dxmvvm:DataTemplateByTypeNameSelector x:Key=\"ItemTemplateSelector\">\r\n       " +
					"     <dxmvvm:DataTemplateByTypeNameSelector.Templates>\r\n");
			foreach(DocumentInfo info in viewModelData.Tables.Concat(viewModelData.Views)) {
			string imagePropertyName = DevExpress.Mvvm.UI.Native.ViewGenerator.EditorsSource.GetImagePropertyName(info.EntityInfo.ElementType);
			string displayMemberPropertyName = DevExpress.Mvvm.UI.Native.ViewGenerator.EditorsSource.GetDisplayMemberPropertyName(info.EntityInfo.ElementType.Type);
			this.Write("\t\t\t\t<DataTemplate x:Key=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.EntityInfo.ElementType.Type.Name));
			this.Write(@""">
		            <ContentControl Background=""Transparent"">
		                <dxmvvm:Interaction.Behaviors>
		                    <dxmvvm:EventToCommand Command=""{Binding DataContext.NavigateCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"" CommandParameter=""{Binding}"" EventName=""MouseDoubleClick"" />
				        </dxmvvm:Interaction.Behaviors>
						<Grid>
		                    <Grid.ColumnDefinitions>
				                <ColumnDefinition Width=""Auto"" />
								<ColumnDefinition Width=""12"" />
				                <ColumnDefinition Width=""*"" />
						    </Grid.ColumnDefinitions>
		                    <dxe:ImageEdit IsReadOnly=""True""");
if(imagePropertyName != null) {
			this.Write(" EditValue=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(imagePropertyName));
			this.Write("}\"");
}
			this.Write(" Width=\"36\" Height=\"36\" Stretch=\"UniformToFill\" />\r\n\t\t                    <TextBl" +
					"ock Grid.Column=\"2\" Text=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(displayMemberPropertyName));
			this.Write("}\" FontSize=\"16\" VerticalAlignment=\"Center\" />\r\n\t\t                </Grid>\r\n\t\t    " +
					"        </ContentControl>\r\n\t\t\t\t</DataTemplate>\r\n");
	}
			this.Write(@"            </dxmvvm:DataTemplateByTypeNameSelector.Templates>
        </dxmvvm:DataTemplateByTypeNameSelector>
	</UserControl.Resources>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height=""Auto"" />
            <RowDefinition Height=""Auto"" />
            <RowDefinition Height=""*"" />
        </Grid.RowDefinitions>
        <dxe:SelectorEditColumnProvider x:Name=""columnProvider"" OwnerEdit=""{Binding ElementName=listBoxEdit}"" ItemsSourceType=""{Binding SelectedEntity, Converter={dxmvvm:ReflectionConverter ConvertMethod=GetType}}"" />
        <dxe:SearchControl x:Name=""searchControl"" ColumnProvider=""{Binding ElementName=columnProvider}"" ShowCloseButton=""False"" Margin=""0"" FilterByColumnsMode=""Custom"" ImmediateMRUPopup=""False"">
            <dxmvvm:Interaction.Behaviors>
                <dxmvvm:FocusBehavior />
            </dxmvvm:Interaction.Behaviors>
        </dxe:SearchControl>
        <TextBlock Text=""{Binding Path=(dxmvvm:ViewModelExtensions.DocumentTitle), RelativeSource={RelativeSource Self}}"" Grid.Row=""1"" Margin=""0,5"" FontSize=""16"" />
        <dxe:ListBoxEdit x:Name=""listBoxEdit"" Grid.Row=""2"" ItemsSource=""{Binding Entities}"" SelectedItem=""{Binding SelectedEntity}"" ShowBorder=""False"" Padding=""0"" FilterCriteria=""{Binding FilterCriteria, ElementName=searchControl}"" ItemTemplateSelector=""{StaticResource ItemTemplateSelector}"" />
    </Grid>
</UserControl>
");
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
		XmlNamespaceConstants.MvvmNamespaceDefinition
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class PeekCollectionViewTemplateBase
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

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
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class RepositoryViewTemplate : RepositoryViewTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	CollectionViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as CollectionViewModelData;
	UIType uiType = (UIType)templateInfo.Properties["UIType"];
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;	
	var generatedFieldsBase64 = ((string)templateInfo.Properties["GeneratedFieldsBase64"]).Split('\n').Select(x => x.Trim()).Where(x => x != null).ToList();
	string generatedBandsBase64 = (string)templateInfo.Properties["GeneratedBandsBase64"];
	XamlNamespaces xamlNamespaces = templateInfo.Properties["XamlNamespaces"] as XamlNamespaces;
	string viewModelPrefix = templateInfo.Properties["viewModelPrefix"].ToString();
			this.Write("<UserControl x:Class=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\"\r\n");
	PasteXamlNamespaces(xamlNamespaces);
	this.ExecuteCollectionViewHook(TemplatesCodeGen.STR_CollectionViewHook_GenerateCustomXmlNamespaces);
			this.Write("    mc:Ignorable=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(xamlNamespaces.GetPrefix(XamlNamespaces.xmlns_blend)));
			this.Write("\"\r\n    d:DesignHeight=\"400\" d:DesignWidth=\"600\"\r\n");
if(viewModelData.UseProxyFactory) {
			this.Write("    DataContext=\"{dxmvvm:ViewModelSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("}\"\r\n");
}
			this.Write(">\r\n");
if(!viewModelData.UseProxyFactory) {
			this.Write("    <UserControl.DataContext>\r\n        <");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("/>\r\n    </UserControl.DataContext>\r\n");
}
			this.Write("      \r\n");
if(viewModelData.SupportServices) {
			this.Write(@"    <dxmvvm:Interaction.Behaviors>
        <dx:DXMessageBoxService/>
        <dxmvvm:EventToCommand Event=""Loaded"" Command=""{Binding OnLoadedCommand}"" />
        <dxmvvm:EventToCommand Event=""Unloaded"" Command=""{Binding OnUnloadedCommand}"" />
        <dx:WindowedDocumentUIService");
if(uiType == UIType.Standard || uiType == UIType.Browser) {
			this.Write(" YieldToParent=\"True\"");
}
if(uiType == UIType.OutlookInspired) {
			this.Write(" DocumentShowMode=\"Dialog\" WindowType=\"dxr:DXRibbonWindow\"");
}
			this.Write(" />\r\n    </dxmvvm:Interaction.Behaviors>\r\n");
}
			this.Write("    <Grid>\r\n        <DockPanel>\r\n            <dxr:RibbonControl RibbonStyle=\"Offi" +
					"ce2010\" DockPanel.Dock=\"Top\" AllowCustomization=\"False\" ");
if(uiType == UIType.Browser) {
			this.Write(" MDIMergeStyle=\"Always\"");
}
			this.Write(">\r\n                <dxr:RibbonDefaultPageCategory Caption=\"defaultCategory\">\r\n   " +
					"                 <dxr:RibbonPage Caption=\"");
if(uiType == UIType.OutlookInspired) {
			this.Write("HOME");
} else {
			this.Write("Home");
}
			this.Write("\">\r\n                        <dxr:RibbonPageGroup ");
if(viewModelData.HasEntityPropertyName()){
			this.Write("Caption=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(TemplatesCodeGen.GetCaption(viewModelData.EntityTypeName)));
			this.Write(" Tasks\"");
}
			this.Write(">\r\n");
foreach(CommandInfo command in viewModelData.Commands) {
			this.Write("                            <dxb:BarButtonItem Content=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
			this.Write("\" Command=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
			this.Write("}\" ");
if(command.HasParameter()){
			this.Write("CommandParameter=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.ParameterPropertyName));
			this.Write("}\"");
}
			this.Write(" ");
if(command.HasGlyphs()){
			this.Write("LargeGlyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.LargeGlyph));
			this.Write("\" Glyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
			this.Write("\"");
}
			this.Write("/>\r\n");
}
			this.Write(@"                            <dxb:BarSplitButtonItem Content=""Reports"" ActAsDropDown=""True"" LargeGlyph=""{dx:DXImage Image=Print_32x32.png}"" Glyph=""{dx:DXImage Image=Print_16x16.png}"">
                                <dxmvvm:Interaction.Behaviors>
                                    <dxrudex:ReportManagerBehavior Service=""{Binding ElementName=");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("ReportService}\"/>\r\n                                </dxmvvm:Interaction.Behaviors" +
					">\r\n                            </dxb:BarSplitButtonItem>\r\n                      " +
					"  </dxr:RibbonPageGroup>\r\n");
this.ExecuteCollectionViewHook(TemplatesCodeGen.STR_CollectionViewHook_GenerateAdditionalRibbonPageGroups);
			this.Write(@"                    </dxr:RibbonPage>
                </dxr:RibbonDefaultPageCategory>
            </dxr:RibbonControl>
            <dxr:RibbonStatusBarControl DockPanel.Dock=""Bottom"">
                <dxr:RibbonStatusBarControl.LeftItems>
                    <dxb:BarStaticItem Content=""{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
			this.Write(".Count, Converter={dxmvvm:FormatStringConverter FormatString=\'RECORDS: {0}\'}}\" />" +
					"\r\n                </dxr:RibbonStatusBarControl.LeftItems>\r\n            </dxr:Rib" +
					"bonStatusBarControl>\r\n");
if(!this.ExecuteCollectionViewHook(TemplatesCodeGen.STR_CollectionViewHook_GenerateCustomContent)) {
			this.Write(@"            <Grid>
                <dxmvvm:Interaction.Behaviors>
                    <dxmvvm:LayoutSerializationService/>
                </dxmvvm:Interaction.Behaviors>
                <dxg:GridControl Name=""gridControl""
                                 ItemsSource=""{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
			this.Write("}\"\r\n");
if(uiType == UIType.Standard || uiType == UIType.Browser) {
			this.Write("                                 ShowBorder=\"False\"\r\n");
} else {
			this.Write("                                 Margin=\"1\"\r\n");
}
if(viewModelData.HasEntityPropertyName()){
			this.Write("                                 CurrentItem=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
			this.Write("}\"\r\n");
}
if(viewModelData.UseIsLoadingBinding){
			this.Write("                                 ShowLoadingPanel=\"{Binding IsLoading}\"\r\n");
}
			this.Write("                                 ");
this.ExecuteCollectionViewHook(TemplatesCodeGen.STR_CollectionViewHook_GenerateAdditionalGridProperties);
			this.Write(">\r\n");
if(viewModelData.HasEntityEditProperty()) {
			this.Write(@"                    <dxmvvm:Interaction.Behaviors>
                        <dxmvvm:EventToCommand PassEventArgsToCommand=""True"" Command=""{Binding EditCommand}"" EventName=""MouseDoubleClick"" MarkRoutedEventsAsHandled=""True"">
                            <dxmvvm:EventToCommand.EventArgsConverter>
                                <dx:EventArgsToDataRowConverter/>
                            </dxmvvm:EventToCommand.EventArgsConverter>
                        </dxmvvm:EventToCommand>
                    </dxmvvm:Interaction.Behaviors>
");
}
if(!this.ExecuteCollectionViewHook(TemplatesCodeGen.STR_CollectionViewHook_GenerateCustomGridColumns)) {
if(generatedFieldsBase64.Any()) {
			this.Write("                <dxg:GridControl.Columns>\r\n");
foreach(var line in generatedFieldsBase64) {
	WriteLine("					" + line);
}
			this.Write("                </dxg:GridControl.Columns>\r\n");
}
if(!string.IsNullOrEmpty(generatedBandsBase64)) {
			this.Write("                    <dxg:GridControl.Bands>\r\n                        ");
			this.Write(this.ToStringHelper.ToStringWithCulture(generatedBandsBase64));
			this.Write("\r\n                    </dxg:GridControl.Bands>\r\n");
}}
			this.Write(@"                    <dxg:GridControl.TotalSummary>
                        <dxg:GridSummaryItem SummaryType=""Count"" Alignment=""Right""/>
                    </dxg:GridControl.TotalSummary>
                    <dxg:GridControl.GroupSummary>
                        <dxg:GridSummaryItem SummaryType=""Count""/>
                    </dxg:GridControl.GroupSummary>
                    <dxg:GridControl.View>
                        <dxg:TableView Name=""tableView"" AllowEditing=""False"" ShowFixedTotalSummary=""True"" AllowPerPixelScrolling=""True"" ");
this.ExecuteCollectionViewHook(TemplatesCodeGen.STR_CollectionViewHook_GenerateAdditionalViewProperties);
			this.Write(">\r\n                            <dxmvvm:Interaction.Behaviors>\r\n                  " +
					"              <dxrudex:GridReportManagerService x:Name=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("ReportService\" />\r\n                            </dxmvvm:Interaction.Behaviors>\r\n " +
					"                           <dxg:TableView.RowCellMenuCustomizations>\r\n");
foreach(CommandInfo command in viewModelData.Commands) {
			this.Write("\t\t\t\t\t\t\t\t<dxb:BarButtonItem Content=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
			this.Write("\" Command=\"{Binding View.DataContext.");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
			this.Write("}\"");
if(command.HasParameter()){
			this.Write(" CommandParameter=\"{Binding Row.Row}\"");
}
if(command.HasGlyphs()){
			this.Write(" Glyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.SmallGlyph));
			this.Write("\"");
}
			this.Write("/>\r\n");
}
			this.Write("\t\t\t\t\t\t\t</dxg:TableView.RowCellMenuCustomizations>\r\n                        </dxg:" +
					"TableView>\r\n                    </dxg:GridControl.View>\r\n                </dxg:G" +
					"ridControl>\r\n            </Grid>\r\n");
}
			this.Write("        </DockPanel>\r\n    </Grid>\r\n</UserControl>\r\n    \r\n    \r\n    \r\n");
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
		XmlNamespaceConstants.ReportDesignerExtensionsNamespaceDefinition
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class RepositoryViewTemplateBase
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

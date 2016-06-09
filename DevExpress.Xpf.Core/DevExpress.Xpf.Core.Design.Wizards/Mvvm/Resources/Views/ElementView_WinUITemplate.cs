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
	using System.Text;
	using System.Linq;
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	using DevExpress.Mvvm.UI.Native.ViewGenerator;
	using DevExpress.Mvvm.Native;
	using DevExpress.Xpf.Internal.EntityFrameworkWrappers;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class ElementView_WinUITemplate : ElementView_WinUITemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	EntityViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as EntityViewModelData;
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	var editorInfos = (PropertyEditorGroupInfo)templateInfo.Properties["EditorInfos"];
	var allInfos = editorInfos.Groups.Flatten(g => g.Groups).Concat(new[] { editorInfos }).SelectMany(g => g.Items);
	bool hasHiddenEditors = allInfos.Any(i => i.Property.Attributes.Hidden());
	XamlNamespaces xamlNamespaces = templateInfo.Properties["XamlNamespaces"] as XamlNamespaces;
	string viewModelPrefix = templateInfo.Properties["viewModelPrefix"].ToString();
	var context = (TemplateGenerationContext)templateInfo.Properties["TemplateGenerationContext"];
	string defaultNamespace = (string)templateInfo.Properties["DefaultNamespacePrefix"];
			this.Write("<UserControl x:Class=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\"\r\n\t");
PasteXamlNamespaces(xamlNamespaces);
			this.Write("\txmlns:view=\"clr-namespace:");
			this.Write(this.ToStringHelper.ToStringWithCulture(defaultNamespace));
			this.Write(this.ToStringHelper.ToStringWithCulture(localNamespace));
			this.Write("\"\r\n    mc:Ignorable=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(xamlNamespaces.GetPrefix(XamlNamespaces.xmlns_blend)));
			this.Write("\"\r\n    d:DesignHeight=\"400\" d:DesignWidth=\"600\"\r\n\t");
if(viewModelData.UseProxyFactory) {		
			this.Write(" DataContext=\"{dxmvvm:ViewModelSource ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("}\"\r\n");
}
			this.Write(" \r\n>\r\n");
if(!viewModelData.UseProxyFactory) {
			this.Write("        <UserControl.DataContext>\r\n        <");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelPrefix));
			this.Write(":");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.Name));
			this.Write("/>\r\n    </UserControl.DataContext>\r\n");
}
if(viewModelData.SupportServices) {		
			this.Write("\t<dxmvvm:Interaction.Behaviors>\r\n\t\t<dxwui:WinUIMessageBoxService/>\r\n\t\t<dxmvvm:Eve" +
					"ntToCommand Event=\"Loaded\" Command=\"{Binding OnLoadedCommand}\" />\r\n    </dxmvvm:" +
					"Interaction.Behaviors>\r\n");
}
			this.Write("  \r\n    <DockPanel>\r\n\t\t<dxwui:AppBar DockPanel.Dock=\"Bottom\" HideMode=\"AlwaysVisi" +
					"ble\">\r\n");
foreach(CommandInfo command in viewModelData.NonLayoutCommands) {
	if(command.Caption != "Close") {
			this.Write("\t\t\t<dxwui:AppBarButton Label=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
			this.Write("\" Command=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
			this.Write("}\"");
if(command.HasParameter()){
			this.Write(" CommandParameter=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.ParameterPropertyName));
			this.Write("}\"");
}
if(command.HasGlyphs()){
			this.Write(" Glyph=\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(command.LargeGlyph));
			this.Write("\" IsEllipseEnabled=\"False\" AllowGlyphTheming=\"True\" GlyphStretch=\"None\"");
}
			this.Write(" HorizontalAlignment=\"Center\" />\r\n");
	}
}
			this.Write("        </dxwui:AppBar>\r\n\t\t<dxlc:DataLayoutControl AutoGenerateItems=\"False\" Curr" +
					"entItem=\"{Binding ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
			this.Write("}\" Orientation=\"Vertical\" Padding=\"67,0,43,0\" Margin=\"0,0,0,16\">\r\n\t\t\t<dxmvvm:Inte" +
					"raction.Behaviors>\r\n\t\t\t\t<dxmvvm:EventToCommand Event=\"{x:Static Binding.SourceUp" +
					"datedEvent}\" Command=\"{Binding UpdateCommand}\" />\r\n\t\t\t</dxmvvm:Interaction.Behav" +
					"iors>\r\n");
			var elementView = new ElementViewTemplate();
			elementView.PushIndent("            ");
			elementView.WriteEditors(editorInfos, viewModelData.EntityPropertyName, viewModelData.EntityTypeName, context, true);
			Write(elementView.GeneratedText);
AddLookUpTables(context, viewModelData, templateInfo);
if(hasHiddenEditors) {
			this.Write("            <dxlc:LayoutControl.AvailableItems>\r\n");
				elementView = new ElementViewTemplate();
				elementView.PushIndent("                ");
				elementView.WriteEditors(editorInfos, viewModelData.EntityPropertyName, viewModelData.EntityTypeName, context, false);
				Write(elementView.GeneratedText);
			this.Write("            </dxlc:LayoutControl.AvailableItems>\r\n");
}
			this.Write("\t\t</dxlc:DataLayoutControl>\r\n    </DockPanel>\r\n</UserControl>\r\n");
			return this.GenerationEnvironment.ToString();
		}
void AddLookUpTables(TemplateGenerationContext context, EntityViewModelData viewModelData, T4TemplateInfo templateInfo) {
	var m2ms = new List<Tuple<ManyToManyInfo, NavigationPropertyRuntimeWrapper>>();
	if (context.MetadataWorkspace != null) { 
		var entityTypeWrapper = TemplatesCodeGen.FindEntityType(context.MetadataWorkspace, viewModelData.EntityTypeName);
		foreach(var navigation in entityTypeWrapper.NavigationProperties) {
			var manyToMany = TemplatesCodeGen.FindManyToManyAssociation(context.MetadataWorkspace, entityTypeWrapper.Name, navigation.ToEndMember.GetEntityType().Name);
			if (manyToMany != null) {
				m2ms.Add(Tuple.Create(manyToMany, navigation));
			}
		}
	}
	if (!viewModelData.LookUpTables.Any() && !m2ms.Any())
		return;
this.Write("    <dxlc:LayoutGroup x:Name=\"Tabs\" View=\"Tabs\" MinHeight=\"250\">\r\n");
	foreach(var t in m2ms) {
		var navigation = t.Item2;
		var m2m = t.Item1;
this.Write("\t<dxb:BarManager MDIMergeStyle=\"Never\" dxlc:LayoutControl.TabHeader=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(m2m.OtherEntity.Name));
this.Write("\" dxb:BarNameScope.IsScopeOwner=\"True\" DataContext=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(navigation.Name));
this.Write("DetailEntities}\">\r\n        <dxmvvm:Interaction.Behaviors>\r\n            <dx:Dialog" +
		"Service DialogWindowStartupLocation=\"CenterOwner\">\r\n                <dx:DialogSe" +
		"rvice.DialogStyle>\r\n                    <Style TargetType=\"Window\">\r\n           " +
		"             <Setter Property=\"Width\" Value=\"600\" />\r\n                        <S" +
		"etter Property=\"Height\" Value=\"400\" />\r\n                        <Setter Property" +
		"=\"ResizeMode\" Value=\"NoResize\" />\r\n                        <Setter Property=\"Sho" +
		"wInTaskbar\" Value=\"False\" />\r\n                        <Setter Property=\"WindowSt" +
		"yle\" Value=\"ToolWindow\" />\r\n                    </Style>\r\n                </dx:D" +
		"ialogService.DialogStyle>\r\n                <dx:DialogService.ViewTemplate>\r\n    " +
		"                <DataTemplate>\r\n                        <dxg:GridControl ItemsSo" +
		"urce=\"{Binding AvailableEntities}\" SelectedItems=\"{Binding SelectedEntities}\" Se" +
		"lectionMode=\"Row\" ShowBorder=\"False\" AutoGenerateColumns=\"RemoveOld\" EnableSmart" +
		"ColumnsGeneration=\"True\">\r\n                            <dxg:GridControl.TotalSum" +
		"mary>\r\n                                <dxg:GridSummaryItem SummaryType=\"Count\" " +
		"Alignment=\"Right\" />\r\n                            </dxg:GridControl.TotalSummary" +
		">\r\n                            <dxg:GridControl.View>\r\n                         " +
		"       <dxg:TableView AllowEditing=\"False\" ShowFixedTotalSummary=\"True\" AllowPer" +
		"PixelScrolling=\"True\" ShowGroupPanel=\"False\" AutoWidth=\"True\" />\r\n              " +
		"              </dxg:GridControl.View>\r\n                        </dxg:GridControl" +
		">\r\n                    </DataTemplate>\r\n                </dx:DialogService.ViewT" +
		"emplate>\r\n            </dx:DialogService>\r\n        </dxmvvm:Interaction.Behavior" +
		"s>\r\n        <dxb:BarManager.Bars>\r\n            <dxb:Bar>\r\n\t\t\t\t<dxb:BarButtonItem" +
		" IsVisible=\"{Binding IsCreateDetailButtonVisible}\" ToolTip=\"Create Detail Entity" +
		"\" Glyph=\"{dx:DXImage Image=New_16x16.png}\" Command=\"{Binding CreateDetailEntityC" +
		"ommand}\" />\r\n                <dxb:BarButtonItem ToolTip=\"Edit Detail Entity\" Gly" +
		"ph=\"{dx:DXImage Image=Edit_16x16.png}\" Command=\"{Binding EditDetailEntityCommand" +
		"}\" />\r\n                <dxb:BarButtonItem ToolTip=\"Add Associations\" Glyph=\"{dx:" +
		"DXImage Image=Add_16x16.png}\" Command=\"{Binding AddDetailEntitiesCommand}\" />\r\n " +
		"               <dxb:BarButtonItem ToolTip=\"Remove Associations\" Glyph=\"{dx:DXIma" +
		"ge Image=Remove_16x16.png}\" Command=\"{Binding RemoveDetailEntitiesCommand}\" />\r\n" +
		"            </dxb:Bar>\r\n        </dxb:BarManager.Bars>\r\n        <dxg:GridControl" +
		" ItemsSource=\"{Binding DetailEntities}\" SelectedItems=\"{Binding SelectedEntities" +
		"}\" ShowBorder=\"False\" SelectionMode=\"Row\" AutoGenerateColumns=\"RemoveOld\" Enable" +
		"SmartColumnsGeneration=\"True\">\r\n            <dxg:GridControl.TotalSummary>\r\n    " +
		"            <dxg:GridSummaryItem SummaryType=\"Count\" Alignment=\"Right\" />\r\n     " +
		"       </dxg:GridControl.TotalSummary>\r\n            <dxg:GridControl.View>\r\n    " +
		"            <dxg:TableView AllowEditing=\"False\" ShowFixedTotalSummary=\"True\" All" +
		"owPerPixelScrolling=\"True\" ShowGroupPanel=\"False\" AutoWidth=\"True\">\r\n           " +
		"     </dxg:TableView>\r\n            </dxg:GridControl.View>\r\n        </dxg:GridCo" +
		"ntrol>\r\n    </dxb:BarManager>\r\n");
	}
	foreach(LookUpCollectionViewModelData item in viewModelData.LookUpTables) {
		if (TemplatesCodeGen.IsAlreadyHandled(item, m2ms.Select(t => t.Item2)))
			continue;
		AddLookUpTable(viewModelData, item, templateInfo);
	}
this.Write("\t</dxlc:LayoutGroup>\r\n");
}
void AddLookUpTable(EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData, T4TemplateInfo templateInfo){
	string generatedFieldsBase64 = (string)templateInfo.Properties["GeneratedFieldsBase64."+viewModelData.Name];
	string generatedBandsBase64 = (string)templateInfo.Properties["GeneratedBandsBase64."+viewModelData.Name];
	string dataContext = GetLookUpPropertyName(parentViewModelData, viewModelData);
	string lookUpGridName = GetLookUpGridName(viewModelData);
this.Write("\t\t<dxlc:LayoutGroup DataContext=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(dataContext));
this.Write("}\" dxlc:LayoutControl.TabHeader=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.LookUpCollectionPropertyAssociationName));
this.Write("\">\r\n            <dxg:GridControl Name=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(lookUpGridName));
this.Write("\" ItemsSource=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
this.Write("}\"");
if(viewModelData.HasEntityPropertyName()){
this.Write(" CurrentItem=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
this.Write("}\"");
}
this.Write(" ShowLoadingPanel=\"{Binding IsLoading}\">\r\n\t\t\t");
if(viewModelData.HasEntityEditProperty()) {
this.Write(@"				<dxmvvm:Interaction.Behaviors>
					<dxmvvm:EventToCommand PassEventArgsToCommand=""True"" Command=""{Binding EditCommand}"" EventName=""MouseDoubleClick"">
						<dxmvvm:EventToCommand.EventArgsConverter>
							<dx:EventArgsToDataRowConverter/>
						</dxmvvm:EventToCommand.EventArgsConverter>
					</dxmvvm:EventToCommand>
				</dxmvvm:Interaction.Behaviors>
");
}
if(!string.IsNullOrEmpty(generatedFieldsBase64)) {	
this.Write("\t\t\t\t<dxg:GridControl.Columns>\r\n\t\t\t\t\t");
this.Write(this.ToStringHelper.ToStringWithCulture(generatedFieldsBase64));
this.Write("\r\n\t\t\t\t</dxg:GridControl.Columns>\r\n");
}
if(!string.IsNullOrEmpty(generatedBandsBase64)) {	
this.Write("\t\t\t\t<dxg:GridControl.Bands>\r\n\t\t\t\t\t");
this.Write(this.ToStringHelper.ToStringWithCulture(generatedBandsBase64));
this.Write("\r\n\t\t\t\t</dxg:GridControl.Bands>\r\n");
}
this.Write(@"                <dxg:GridControl.TotalSummary>
                    <dxg:GridSummaryItem SummaryType=""Count"" Alignment=""Right""/>
                </dxg:GridControl.TotalSummary>
                <dxg:GridControl.GroupSummary>
                    <dxg:GridSummaryItem SummaryType=""Count""/>
                </dxg:GridControl.GroupSummary>
                <dxg:GridControl.View>
                    <dxg:TableView AllowEditing=""False"" ShowFixedTotalSummary=""True"" AllowPerPixelScrolling=""True"" ShowGroupPanel=""False"" ShowIndicator=""False"">
					    <dxg:TableView.RowCellMenuCustomizations>
");
foreach(CommandInfo command in viewModelData.NonLayoutCommands) {	
this.Write("\t\t\t\t                  <dxb:BarButtonItem Content=\"");
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
this.Write(" />\r\n");
}
this.Write("\t\t\t\t    </dxg:TableView.RowCellMenuCustomizations>\r\n                    </dxg:Tab" +
		"leView>\r\n                </dxg:GridControl.View>\r\n            </dxg:GridControl>" +
		"\t\t\t\t\r\n\t\t\t");
AddButtonsForLookUpTable(parentViewModelData, viewModelData, templateInfo);
this.Write("\t\t</dxlc:LayoutGroup>\r\n");
}
void AddButtonsForLookUpTable(EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData, T4TemplateInfo templateInfo) {
	if(viewModelData.NonLayoutCommands.Length > 0) {
this.Write("\t\t\t<dxlc:LayoutGroup Orientation=\"Vertical\" HorizontalAlignment=\"Right\" VerticalA" +
		"lignment=\"Top\" MinWidth=\"100\">\r\n");
	}	
	string lookUpTablePropertyName = GetLookUpPropertyName(parentViewModelData, viewModelData);
	foreach(CommandInfo command in viewModelData.NonLayoutCommands) {	
		string commandName = GetCommandName(command, viewModelData.EntityTypeName);
this.Write("\t\t\t\t<Button Content=\"");
this.Write(this.ToStringHelper.ToStringWithCulture(command.Caption));
this.Write("\" Command=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(command.CommandPropertyName));
this.Write("}\"");
if(command.HasParameter()){
this.Write(" CommandParameter=\"{Binding ");
this.Write(this.ToStringHelper.ToStringWithCulture(command.ParameterPropertyName));
this.Write("}\"");
}
this.Write(" />\r\n");
	}
	if(viewModelData.NonLayoutCommands.Length > 0) {
this.Write("\t\t\t</dxlc:LayoutGroup>\r\n");
	}
}
string GetCommandName(CommandInfo info, string entityTypeName){
	string str_command = "Command";
	string infoCommandPropertyName = info.CommandPropertyName;
	string shortName = infoCommandPropertyName.EndsWith(str_command) ? infoCommandPropertyName.Remove(infoCommandPropertyName.Length - str_command.Length) : null;
	if(string.IsNullOrEmpty(shortName))
		return infoCommandPropertyName+entityTypeName;
	return shortName+entityTypeName+str_command;
}
string GetLookUpGridName(LookUpCollectionViewModelData viewModelData){
	return viewModelData.LookUpCollectionPropertyAssociationName+"Grid";
}
string GetLookUpPropertyName(EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData){
	return parentViewModelData.EntityTypeName+viewModelData.LookUpCollectionPropertyAssociationName + "Details";	
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
		XmlNamespaceConstants.WindowsUINamespaceDefinition
	};
	xamlNamespaces.AddDevExpressXamlNamespaces(toAdd);
	this.WriteLine(xamlNamespaces.GetXaml());
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class ElementView_WinUITemplateBase
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

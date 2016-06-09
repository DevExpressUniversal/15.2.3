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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.ViewModel
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentsViewModelTemplate : DocumentsViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Collections.Generic;\r\nusing Syste" +
					"m.Collections.ObjectModel;\r\nusing System.ComponentModel;\r\nusing DevExpress.Mvvm;" +
					"\r\nusing DevExpress.Mvvm.POCO;\r\nusing DevExpress.Mvvm.DataAnnotations;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// The base class for POCO view models that operate t" +
					"he collection of documents.\r\n    /// </summary>\r\n    /// <typeparam name=\"TModul" +
					"e\">A navigation list entry type.</typeparam>\r\n    /// <typeparam name=\"TUnitOfWo" +
					"rk\">A unit of work type.</typeparam>\r\n    public abstract class DocumentsViewMod" +
					"el<TModule, TUnitOfWork> : ISupportLogicalLayout\r\n        where TModule : Module" +
					"Description<TModule>\r\n        where TUnitOfWork : IUnitOfWork {\r\n\t\t\r\n\t\tconst str" +
					"ing ViewLayoutName = \"DocumentViewModel\";\r\n\r\n        protected readonly IUnitOfW" +
					"orkFactory<TUnitOfWork> unitOfWorkFactory;\r\n\r\n        /// <summary>\r\n        ///" +
					" Initializes a new instance of the DocumentsViewModel class.\r\n        /// </summ" +
					"ary>\r\n        /// <param name=\"unitOfWorkFactory\">A factory used to create a uni" +
					"t of work instance.</param>\r\n        protected DocumentsViewModel(IUnitOfWorkFac" +
					"tory<TUnitOfWork> unitOfWorkFactory) {\r\n            this.unitOfWorkFactory = uni" +
					"tOfWorkFactory;\r\n            Modules = CreateModules().ToArray();\r\n            f" +
					"oreach(var module in Modules)\r\n                Messenger.Default.Register<Naviga" +
					"teMessage<TModule>>(this, module, x => Show(x.Token));\r\n\t\t\tMessenger.Default.Reg" +
					"ister<DestroyOrphanedDocumentsMessage>(this, x => DestroyOrphanedDocuments());\r\n" +
					"        }\r\n\r\n\t\tvoid DestroyOrphanedDocuments() {\r\n            var orphans = this" +
					".GetOrphanedDocuments().Except(this.GetImmediateChildren());\r\n            foreac" +
					"h(IDocument orphan in orphans) {\r\n                orphan.DestroyOnClose = true;\r" +
					"\n                orphan.Close();\r\n            }\r\n        }\r\n\r\n        /// <summa" +
					"ry>\r\n        /// Navigation list that represents a collection of module descript" +
					"ions.\r\n        /// </summary>\r\n        public TModule[] Modules { get; private s" +
					"et; }\r\n\r\n        /// <summary>\r\n        /// A currently selected navigation list" +
					" entry. This property is writable. When this property is assigned a new value, i" +
					"t triggers the navigating to the corresponding document.\r\n        /// Since Docu" +
					"mentsViewModel is a POCO view model, this property will raise INotifyPropertyCha" +
					"nged.PropertyEvent when modified so it can be used as a binding source in views." +
					"\r\n        /// </summary>\r\n\t\tpublic virtual TModule SelectedModule { get; set; }\r" +
					"\n\r\n        /// <summary>\r\n        /// A navigation list entry that corresponds t" +
					"o the currently active document. If the active document does not have the corres" +
					"ponding entry in the navigation list, the property value is null. This property " +
					"is read-only.\r\n        /// Since DocumentsViewModel is a POCO view model, this p" +
					"roperty will raise INotifyPropertyChanged.PropertyEvent when modified so it can " +
					"be used as a binding source in views.\r\n        /// </summary>\r\n        public vi" +
					"rtual TModule ActiveModule { get; protected set; }\r\n\r\n        /// <summary>\r\n   " +
					"     /// Saves changes in all opened documents.\r\n        /// Since DocumentsView" +
					"Model is a POCO view model, an instance of this class will also expose the SaveA" +
					"llCommand property that can be used as a binding source in views.\r\n        /// <" +
					"/summary>\r\n        public void SaveAll() {\r\n            Messenger.Default.Send(n" +
					"ew SaveAllMessage());\r\n        }\r\n\r\n        /// <summary>\r\n        /// Used to c" +
					"lose all opened documents and allows you to save unsaved results and to cancel c" +
					"losing.\r\n        /// Since DocumentsViewModel is a POCO view model, an instance " +
					"of this class will also expose the OnClosingCommand property that can be used as" +
					" a binding source in views.\r\n        /// </summary>\r\n        /// <param name=\"ca" +
					"ncelEventArgs\">An argument of the System.ComponentModel.CancelEventArgs type whi" +
					"ch is used to cancel closing if needed.</param>\r\n        public virtual void OnC" +
					"losing(CancelEventArgs cancelEventArgs) {\r\n\t\t    if (GroupedDocumentManagerServi" +
					"ce != null && GroupedDocumentManagerService.Groups.Count() > 1) {\r\n             " +
					"   var activeGroup = GroupedDocumentManagerService.ActiveGroup;\r\n               " +
					" var message = new CloseAllMessage(cancelEventArgs, vm => {\r\n                   " +
					" var activeVMs = activeGroup.Documents.Select(d => d.Content);\r\n                " +
					"    return activeVMs.Contains(vm);\r\n                });\r\n                Messeng" +
					"er.Default.Send(message);\r\n                return;\r\n            }\r\n\t\t\tSaveLogica" +
					"lLayout();\r\n\t\t\tif(LayoutSerializationService != null) {\r\n\t\t\t\tPersistentLayoutHel" +
					"per.PersistentViewsLayout[ViewLayoutName] = LayoutSerializationService.Serialize" +
					"();\r\n\t\t\t}\r\n            Messenger.Default.Send(new CloseAllMessage(cancelEventArg" +
					"s, vm => true));\r\n\t\t\tPersistentLayoutHelper.SaveLayout();\r\n        }\r\n\r\n\t\t/// <s" +
					"ummary>\r\n\t\t/// Contains a current state of the navigation pane.\r\n\t\t/// </summary" +
					">\r\n        /// Since DocumentsViewModel is a POCO view model, this property will" +
					" raise INotifyPropertyChanged.PropertyEvent when modified so it can be used as a" +
					" binding source in views.\r\n\t\tpublic virtual NavigationPaneVisibility NavigationP" +
					"aneVisibility { get; set; }\r\n\r\n\t\t/// <summary>\r\n\t\t/// Navigates to a document.\r\n" +
					"        /// Since DocumentsViewModel is a POCO view model, an instance of this c" +
					"lass will also expose the ShowCommand property that can be used as a binding sou" +
					"rce in views.\r\n\t\t/// </summary>\r\n        /// <param name=\"module\">A navigation l" +
					"ist entry specifying a document what to be opened.</param>\r\n        public void " +
					"Show(TModule module) {\r\n            ShowCore(module);\r\n        }\r\n\r\n\t\tpublic IDo" +
					"cument ShowCore(TModule module) {\r\n            if(module == null || DocumentMana" +
					"gerService == null)\r\n                return null;\r\n            IDocument documen" +
					"t = DocumentManagerService.FindDocumentByIdOrCreate(module.DocumentType, x => Cr" +
					"eateDocument(module));\r\n            document.Show();\r\n\t\t\treturn document;\r\n     " +
					"   }\r\n\r\n\t\t/// <summary>\r\n\t\t/// Creates and shows a document which view is bound " +
					"to PeekCollectionViewModel. The document is created and shown using a document m" +
					"anager service named \"WorkspaceDocumentManagerService\".\r\n        /// Since Docum" +
					"entsViewModel is a POCO view model, an instance of this class will also expose t" +
					"he PinPeekCollectionViewCommand property that can be used as a binding source in" +
					" views.\r\n\t\t/// </summary>\r\n        /// <param name=\"module\">A navigation list en" +
					"try that is used as a PeekCollectionViewModel factory.</param>\r\n        public v" +
					"oid PinPeekCollectionView(TModule module) {\r\n            if(WorkspaceDocumentMan" +
					"agerService == null)\r\n                return;\r\n            IDocument document = " +
					"WorkspaceDocumentManagerService.FindDocumentByIdOrCreate(module.DocumentType, x " +
					"=> CreatePinnedPeekCollectionDocument(module));\r\n            document.Show();\r\n " +
					"       }\r\n\r\n\t\t/// <summary>\r\n\t\t/// Finalizes the DocumentsViewModel initializati" +
					"on and opens the default document.\r\n        /// Since DocumentsViewModel is a PO" +
					"CO view model, an instance of this class will also expose the OnLoadedCommand pr" +
					"operty that can be used as a binding source in views.\r\n\t\t/// </summary>\r\n       " +
					" public virtual void OnLoaded(TModule module) {\r\n\t\t\tIsLoaded = true;\r\n          " +
					"  DocumentManagerService.ActiveDocumentChanged += OnActiveDocumentChanged;\r\n\t\t\ti" +
					"f (!RestoreLogicalLayout()) {\r\n\t\t\t\tShow(module);\r\n\t\t\t}\r\n\t\t\tPersistentLayoutHelpe" +
					"r.TryDeserializeLayout(LayoutSerializationService, ViewLayoutName);\r\n        }\r\n" +
					"\r\n\t\tbool documentChanging = false;\r\n        void OnActiveDocumentChanged(object " +
					"sender, ActiveDocumentChangedEventArgs e) {\r\n            if(e.NewDocument == nul" +
					"l) {\r\n                ActiveModule = null;\r\n            } else {\r\n\t\t\t\tdocumentCh" +
					"anging = true;\r\n                ActiveModule = Modules.FirstOrDefault(m => m.Doc" +
					"umentType == (string)e.NewDocument.Id);\r\n\t\t\t\tdocumentChanging = false;\r\n        " +
					"    }\r\n        }\r\n\r\n\t\tprotected IGroupedDocumentManagerService GroupedDocumentMa" +
					"nagerService { get { return this.GetService<IGroupedDocumentManagerService>(); }" +
					" }\r\n        protected IDocumentManagerService DocumentManagerService { get { ret" +
					"urn this.GetService<IDocumentManagerService>(); } }\r\n\t\tprotected ILayoutSerializ" +
					"ationService LayoutSerializationService { get { return this.GetService<ILayoutSe" +
					"rializationService>(\"RootLayoutSerializationService\"); } }\r\n        protected ID" +
					"ocumentManagerService WorkspaceDocumentManagerService { get { return this.GetSer" +
					"vice<IDocumentManagerService>(\"WorkspaceDocumentManagerService\"); } }\r\n\r\n       " +
					" public virtual TModule DefaultModule { get { return Modules.First(); } }\r\n\r\n\t\tp" +
					"rotected bool IsLoaded { get; private set; }\r\n\r\n        protected virtual void O" +
					"nSelectedModuleChanged(TModule oldModule) {\r\n\t\t\tif(IsLoaded && !documentChanging" +
					")\r\n\t\t\t\tShow(SelectedModule);\r\n\t\t}\r\n\r\n        protected virtual void OnActiveModu" +
					"leChanged(TModule oldModule) {\r\n\t\t\tSelectedModule = ActiveModule;\r\n\t\t}\r\n\r\n      " +
					"  IDocument CreateDocument(TModule module) {\r\n            var document = Documen" +
					"tManagerService.CreateDocument(module.DocumentType, null, this);\r\n            do" +
					"cument.Title = GetModuleTitle(module);\r\n            document.DestroyOnClose = fa" +
					"lse;\r\n            return document;\r\n        }\r\n\r\n        protected virtual strin" +
					"g GetModuleTitle(TModule module) {\r\n            return module.ModuleTitle;\r\n    " +
					"    }\r\n\r\n        IDocument CreatePinnedPeekCollectionDocument(TModule module) {\r" +
					"\n            var document = WorkspaceDocumentManagerService.CreateDocument(\"Peek" +
					"CollectionView\", module.CreatePeekCollectionViewModel());\r\n            document." +
					"Title = module.ModuleTitle;\r\n            return document;\r\n        }\r\n\r\n        " +
					"protected Func<TModule, object> GetPeekCollectionViewModelFactory<TEntity, TPrim" +
					"aryKey>(Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc) " +
					"where TEntity : class {\r\n            return module => PeekCollectionViewModel<TM" +
					"odule, TEntity, TPrimaryKey, TUnitOfWork>.Create(module, unitOfWorkFactory, getR" +
					"epositoryFunc).SetParentViewModel(this);\r\n        }\r\n\r\n        protected abstrac" +
					"t TModule[] CreateModules();\r\n\r\n        protected TUnitOfWork CreateUnitOfWork()" +
					" {\r\n            return unitOfWorkFactory.CreateUnitOfWork();\r\n        }\r\n\r\n\t\tpub" +
					"lic void SaveLogicalLayout() {\r\n            PersistentLayoutHelper.PersistentLog" +
					"icalLayout = this.SerializeDocumentManagerService();\r\n        }\r\n\r\n        publi" +
					"c bool RestoreLogicalLayout() {\r\n            if(string.IsNullOrEmpty(PersistentL" +
					"ayoutHelper.PersistentLogicalLayout))\r\n                return false;\r\n          " +
					"  this.RestoreDocumentManagerService(PersistentLayoutHelper.PersistentLogicalLay" +
					"out);\r\n\t\t\treturn true;\r\n        }\r\n\r\n\t\tbool ISupportLogicalLayout.CanSerialize {" +
					"\r\n            get { return true; }\r\n        }\r\n\r\n        IDocumentManagerService" +
					" ISupportLogicalLayout.DocumentManagerService {\r\n            get { return Docume" +
					"ntManagerService; }\r\n        }\r\n\r\n\t\tIEnumerable<object> ISupportLogicalLayout.Lo" +
					"okupViewModels {\r\n            get { return null; }\r\n        }\r\n    }\r\n\r\n\t/// <su" +
					"mmary>\r\n    /// A base class representing a navigation list entry.\r\n    /// </su" +
					"mmary>\r\n    /// <typeparam name=\"TModule\">A navigation list entry type.</typepar" +
					"am>\r\n    public abstract partial class ModuleDescription<TModule> where TModule " +
					": ModuleDescription<TModule> {\r\n        \r\n\t\treadonly Func<TModule, object> peekC" +
					"ollectionViewModelFactory;\r\n        object peekCollectionViewModel;\r\n\r\n        /" +
					"// <summary>\r\n        /// Initializes a new instance of the ModuleDescription cl" +
					"ass.\r\n        /// </summary>\r\n        /// <param name=\"title\">A navigation list " +
					"entry display text.</param>\r\n        /// <param name=\"documentType\">A string val" +
					"ue that specifies the view type of corresponding document.</param>\r\n        /// " +
					"<param name=\"group\">A navigation list entry group name.</param>\r\n        /// <pa" +
					"ram name=\"peekCollectionViewModelFactory\">An optional parameter that provides a " +
					"function used to create a PeekCollectionViewModel that provides quick navigation" +
					" between collection views.</param>\r\n        public ModuleDescription(string titl" +
					"e, string documentType, string group, Func<TModule, object> peekCollectionViewMo" +
					"delFactory = null) {\r\n            ModuleTitle = title;\r\n            ModuleGroup " +
					"= group;\r\n            DocumentType = documentType;\r\n            this.peekCollect" +
					"ionViewModelFactory = peekCollectionViewModelFactory;\r\n        }\r\n\r\n        /// " +
					"<summary>\r\n        /// The navigation list entry display text.\r\n        /// </su" +
					"mmary>\r\n        public string ModuleTitle { get; private set; }\r\n\r\n        /// <" +
					"summary>\r\n        /// The navigation list entry group name.\r\n        /// </summa" +
					"ry>\r\n        public string ModuleGroup { get; private set; }\r\n\r\n        /// <sum" +
					"mary>\r\n        /// Contains the corresponding document view type.\r\n        /// <" +
					"/summary>\r\n        public string DocumentType { get; private set; }\r\n\r\n        /" +
					"// <summary>\r\n        /// A primary instance of corresponding PeekCollectionView" +
					"Model used to quick navigation between collection views.\r\n        /// </summary>" +
					"\r\n        public object PeekCollectionViewModel {\r\n            get {\r\n          " +
					"      if(peekCollectionViewModelFactory == null)\r\n                    return nul" +
					"l;\r\n                if(peekCollectionViewModel == null)\r\n                    pee" +
					"kCollectionViewModel = CreatePeekCollectionViewModel();\r\n                return " +
					"peekCollectionViewModel;\r\n            }\r\n        }\r\n\r\n        /// <summary>\r\n   " +
					"     /// Creates and returns a new instance of the corresponding PeekCollectionV" +
					"iewModel that provides quick navigation between collection views.\r\n        /// <" +
					"/summary>\r\n        public object CreatePeekCollectionViewModel() {\r\n            " +
					"return peekCollectionViewModelFactory((TModule)this);\r\n        }\r\n    }\r\n\r\n    /" +
					"// <summary>\r\n    /// Represents a navigation pane state.\r\n    /// </summary>\r\n\t" +
					"public enum NavigationPaneVisibility {\r\n\r\n        /// <summary>\r\n        /// Nav" +
					"igation pane is visible and minimized.\r\n        /// </summary>\r\n\t    Minimized,\r" +
					"\n\r\n        /// <summary>\r\n        /// Navigation pane is visible and not minimiz" +
					"ed.\r\n        /// </summary>\r\n        Normal,\r\n\r\n        /// <summary>\r\n        /" +
					"// Navigation pane is invisible.\r\n        /// </summary>\r\n        Off\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentsViewModelTemplateBase
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

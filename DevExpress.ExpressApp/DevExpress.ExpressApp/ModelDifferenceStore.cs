#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Xml;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public abstract class ModelStoreBase {
		public const string ModelDiffDefaultName = "Model.DesignedDiffs";
		public const string AppDiffDefaultName = "Model";
		public const string AppDiffDefaultTabletName = "Model.Tablet";
		public const string AppDiffDefaultMobileName = "Model.Mobile";
		public const string AppDiffDefaultDesktopName = "Model.Desktop";
		public const string UserDiffDefaultName = "Model.User";
		public const string UnusableDiffDefaultName = "UnusableNodes";
		public const string LogonParametesDefaultName = "LogonParameters";
		public const string LocalizationPrefix = ".Localization";
		public const string ModelFileExtension = ".xafml";
		protected string GetAspectFromResourceName(string resourceName, string contextName) {
			if(resourceName.Contains(contextName)) {
				int startPosition = resourceName.IndexOf(contextName) + contextName.Length;
				startPosition = resourceName.Contains(LocalizationPrefix) ?
										resourceName.IndexOf(LocalizationPrefix, startPosition) + LocalizationPrefix.Length :
										resourceName.IndexOf("_", startPosition);
				if(startPosition >= 0) {
					int startExtension = resourceName.IndexOf(((contextName.Contains(UnusableDiffDefaultName)) ? ".xml" : ModelFileExtension), startPosition);
					int count = startExtension - startPosition;
					if(startPosition >= 0 && startPosition <= resourceName.Length && count > 0) {
						string aspect = resourceName.Substring(startPosition, count);
						if(aspect[0] == '.' || aspect[0] == '_') {
							return GetAspect(aspect.Substring(1));
						}
					}
				}
			}
			return string.Empty;
		}
		private string GetAspect(string cultureName) {
			CultureInfo cultureInfo;
			try {
				cultureInfo = CultureInfo.GetCultureInfo(cultureName);
			}
			catch {
				return string.Empty;
			}
			if(cultureInfo.EnglishName == string.Format("Unknown Locale ({0})", cultureName)) {
				return string.Empty;
			}
			return cultureInfo.Name;
		}
		public virtual IEnumerable<string> GetAspects() {
			return new string[0];   
		}
		public abstract void Load(ModelApplicationBase model);
		public abstract string Name { get; }
		public virtual bool ReadOnly { get { return false; } }
		public class EmptyModelStore : ModelStoreBase {
			public override string Name { get { return ""; } }
			public override void Load(ModelApplicationBase model) { }
		}
		public static readonly EmptyModelStore Empty = new EmptyModelStore();
	}
	public abstract class ModelDifferenceStore : ModelStoreBase {
		public abstract void SaveDifference(ModelApplicationBase model);
	}
	public class StringModelStore : ModelDifferenceStore {
		private readonly IDictionary<string, string> data;
		public StringModelStore(string xml) {
			data = new Dictionary<string, string>();
			if(!string.IsNullOrEmpty(xml)) {
				data[string.Empty] = xml;
			}
		}
		public StringModelStore() : this(null) { }
		public override string Name { get { return GetType().Name; } }
		public override bool ReadOnly { get { return true; } }
		public void Add(string aspect, string xml) {
			Guard.ArgumentNotNullOrEmpty(xml, "xml");
			if(aspect == null) return;
			data[aspect] = xml;
		}
		public override void Load(ModelApplicationBase model) {
			if(data.Count == 0 || model == null) return;
			ModelXmlReader reader = new ModelXmlReader();
			foreach(KeyValuePair<string, string> item in data) {
				string aspect = item.Key;
				string xml = item.Value;
				reader.ReadFromString(model, aspect, xml);
			}
		}
		public override void SaveDifference(ModelApplicationBase model) {
			throw new NotSupportedException();
		}
	}
	public interface ISupportModelDifferences {
		string Model { get; set; }
	}
	public class ResourcesModelStore : ModelStoreBase {
		public const string ModelDesignedDefaultDiffsName = ModelDiffDefaultName + ModelFileExtension;
		private Assembly assembly;
		private string modelDiffName;
		private string modelDesignedDiffsName;
		public ResourcesModelStore(Assembly assembly) : this(assembly, null) { }
		public ResourcesModelStore(Assembly assembly, string modelDiffName) {
			Guard.ArgumentNotNull(assembly, "assembly");
			this.assembly = assembly;
			this.modelDiffName = modelDiffName ?? ModelDiffDefaultName;
			modelDesignedDiffsName = this.modelDiffName + ModelFileExtension;
		}
		public override void Load(ModelApplicationBase model) {
			ReadFromResource(model, string.Empty, assembly, modelDesignedDiffsName);
			foreach(string resourceName in assembly.GetManifestResourceNames()) {
				if(resourceName.EndsWith(".bo")) {
					ReadFromResource(model, string.Empty, assembly, resourceName);
				}
				if(resourceName.Contains(modelDiffName) && !resourceName.Contains(modelDesignedDiffsName)) {
					string aspect = GetAspectFromResourceName(resourceName, modelDiffName);
					if(!string.IsNullOrEmpty(aspect)) {
						ReadFromResource(model, aspect, assembly, GetShortResourceName(resourceName));
					}
				}
			}
			foreach(string aspectName in model.GetAspectNames()) {
				CultureInfo cultureInfo = new CultureInfo(aspectName);
				Assembly satelliteAssembly = GetSatelliteAssembly(cultureInfo);
				if(satelliteAssembly != null) {
					foreach(string resourceName in satelliteAssembly.GetManifestResourceNames()) {
						if(resourceName.Contains(modelDiffName) && !resourceName.Contains(modelDesignedDiffsName)) {
							ReadFromResource(model, cultureInfo.Name, satelliteAssembly, GetShortResourceName(resourceName));
						}
					}
				}
			}
		}
		private void ReadFromResource(ModelNode rootNode, string aspect, Assembly assembly, string resourceName) {
			new ModelXmlReader().ReadFromResource(rootNode, aspect, assembly, resourceName);
		}
		private string GetShortResourceName(string resourceName) {
			return resourceName.Substring(resourceName.IndexOf(modelDiffName));
		}
		private Assembly GetSatelliteAssembly(CultureInfo cultureInfo) {
			Assembly satelliteAssembly = null;
			CultureInfo currentCultureInfo = cultureInfo;
			Version satelliteAssemblyVersion = GetSatelliteContractVersion(assembly);
			do {
				satelliteAssembly = GetSatelliteAssembly(assembly, currentCultureInfo, satelliteAssemblyVersion);
				currentCultureInfo = currentCultureInfo.Parent;
			} while((satelliteAssembly == null) && currentCultureInfo != CultureInfo.InvariantCulture);
			return satelliteAssembly;
		}
		private Version GetSatelliteContractVersion(Assembly assembly) {
			Version result;
			SatelliteContractVersionAttribute attribute = AttributeHelper.GetAttributes<SatelliteContractVersionAttribute>(assembly, false).FirstOrDefault();
			if(attribute != null && Version.TryParse(attribute.Version, out result)) {
				return result;
			}
			return null;
		}
		private Assembly GetSatelliteAssembly(Assembly assembly, CultureInfo culture, Version version) {
			try {
#if DEBUG && !MediumTrust
				AssemblyName assemblyName = this.assembly.GetName();
				assemblyName.Name += ".resources";
				assemblyName.CultureInfo = culture;
				assemblyName.Version = version;
				Boolean loadAssembly = true;
#if DebugTest
				loadAssembly = File.Exists(Path.Combine(Environment.CurrentDirectory, culture.Name, assemblyName.Name + ".dll"));
#endif
				if(loadAssembly) {
					return Assembly.Load(assemblyName);
				}
#else 
				return assembly.GetSatelliteAssembly(culture, version);
#endif
			}
			catch { }
			return null;
		}
		public override string Name {
			get { return string.Format("Resource '{0}' of the assembly '{1}'", modelDesignedDiffsName, assembly.FullName); }
		}
		public override bool ReadOnly {
			get { return true; }
		}
		public override string ToString() {
			return base.ToString() + "(" + Name + ")";
		}
	}
	public interface IFileModelStore {
		List<string> GetBoFilesToSave(ModelApplicationBase model);
	}
	public class FileModelStore : ModelDifferenceStore, IFileModelStore {
		private static readonly Encoding defaultEncoding = Encoding.UTF8;
		private static ModelXmlWriter writer = new ModelXmlWriter();
		private static ModelXmlReader reader = new ModelXmlReader();
		private string storePath;
		private string mainFileNameTemplate;
		private IList<string> lastSavedFiles;
		private Dictionary<string, string> businessObjectsDiffsFiles = new Dictionary<string, string>();
		private void ReadFromFile(ModelNode rootNode, string fileName, string aspect) {
			reader.ReadFromFile(rootNode, aspect, fileName);
		}
		private Encoding GetEncodingForFile(string fileName) {
			if(!File.Exists(fileName)) return defaultEncoding;
			using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				return reader.GetStreamEncoding(stream) ?? defaultEncoding;
			}
		}
		public string GetFileNameForAspect(string aspect) {
			return GetFileNameForAspect(aspect, mainFileNameTemplate);
		}
		private string GetAspectFileNameTemplate(string fileNameTemplate) {
			return fileNameTemplate + (fileNameTemplate == ModelDifferenceStore.ModelDiffDefaultName ? ".Localization." : "_");
		}
		private string GetExtensionTemplate(string fileNameTemplate) {
			return ((fileNameTemplate.Contains(ModelDifferenceStore.UnusableDiffDefaultName)) ? ".xml" : ModelFileExtension);
		}
		protected string GetFileNameForAspect(string aspect, string fileNameTemplate) {
			string result = fileNameTemplate;
			if(!string.IsNullOrEmpty(aspect) && (aspect != CaptionHelper.DefaultLanguage)) {
				result = GetAspectFileNameTemplate(fileNameTemplate) + aspect;
			}
			return result + GetExtensionTemplate(fileNameTemplate);
		}
		public virtual bool CheckFileExist(string fileName) { return true; }
		private Dictionary<string, string> GetAspectDiffs(string fileNameTemplate) {
			Dictionary<string, string> result = new Dictionary<string, string>();
			if(Directory.Exists(storePath)) {
				string defaultDiffsFileName = Path.Combine(storePath, GetFileNameForAspect(null, fileNameTemplate));
				string[] diffsFileNames = Directory.GetFiles(storePath, GetAspectFileNameTemplate(fileNameTemplate) + "*" + GetExtensionTemplate(fileNameTemplate));
				foreach(string diffsFileName in diffsFileNames) {
					if(diffsFileName != defaultDiffsFileName && CheckFileExist(diffsFileName)) {
						string aspect = GetAspectFromResourceName(diffsFileName, fileNameTemplate);
						if(aspect != string.Empty) {
							result.Add(diffsFileName, aspect);
						}
					}
				}
			}
			return result;
		}
		public override void Load(ModelApplicationBase model) {
			if(DesignerOnlyCalculator.IsRunFromDesigner) {
				LoadBusinessObjectsModels(model);
			}
			Load(model, mainFileNameTemplate);
		}
		private void LoadBusinessObjectsModels(ModelApplicationBase model) {
			businessObjectsDiffsFiles.Clear();
			foreach(KeyValuePair<string, string> item in TryGetBusinessObjectsFilesAssociations(FindBusinessObjectsDiffFiles(storePath))) {
				ReadFromFile(model, item.Value, "");
				businessObjectsDiffsFiles.Add(item.Key, item.Value);
			}
		}
		private Dictionary<string, string> TryGetBusinessObjectsFilesAssociations(IList<string> boDiffs) {
			Dictionary<string, List<string>> allFiles = new Dictionary<string, List<string>>();
			foreach(string boDiffsFileName in boDiffs) {
				string className = GetBusinessObjectClassName(boDiffsFileName);
				if(!string.IsNullOrEmpty(className)) {
					if(allFiles.ContainsKey(boDiffsFileName)) {
						allFiles[className].Add(boDiffsFileName);
					}
					else {
						if(XafTypesInfo.Instance.FindTypeInfo(className) != null) {
							List<string> files = new List<string>();
							files.Add(boDiffsFileName);
							allFiles[className] = files;
						}
					}
				}
			}
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach(KeyValuePair<string, List<string>> item in allFiles) {
				if(item.Value.Count > 1) {
					string message = string.Format("Several '*.bo' files are available for the '{0}' class:{1}", item.Key, Environment.NewLine);
					message += string.Join(Environment.NewLine, item.Value.ToArray());
					message += string.Format("{0}{0}Please leave a single file and remove unused.", Environment.NewLine);
					throw new InvalidOperationException(message);
				}
				else {
					result[item.Key] = item.Value[0];
				}
			}
			return result;
		}
		internal static string GetBusinessObjectClassName(string boDiffsFileName) {
			XmlDocument document = new XmlDocument();
			document.Load(boDiffsFileName);
			XmlElement classNode = (XmlElement)document.SelectSingleNode("/Application/BOModel/Class[@IsDesigned='True']");
			if(classNode != null) {
				XmlAttribute nameAttribute = classNode.Attributes["Name"];
				if(nameAttribute != null) {
					return nameAttribute.Value;
				}
			}
			return null;
		}
		private IList<string> FindBusinessObjectsDiffFiles(string storePath) {
			List<string> diffFileNames = new List<string>();
			foreach(string fileName in Directory.GetFiles(storePath)) {
				if(fileName.EndsWith(".bo") && CheckFileExist(fileName)) {
					diffFileNames.Add(fileName);
				}
			}
			foreach(string directoryName in Directory.GetDirectories(storePath)) {
				diffFileNames.AddRange(FindBusinessObjectsDiffFiles(directoryName));
			}
			return diffFileNames;
		}
		private void Load(ModelApplicationBase model, string fileNameTemplate) {
			if(Directory.Exists(storePath)) {
				string defaultDiffsFileName = Path.Combine(storePath, GetFileNameForAspect(null, fileNameTemplate));
				if(File.Exists(defaultDiffsFileName)) {
					ReadFromFile(model, defaultDiffsFileName, "");
				}
				foreach(KeyValuePair<string, string> aspectDiff in GetAspectDiffs(fileNameTemplate)) {
					ReadFromFile(model, aspectDiff.Key, aspectDiff.Value);
				}
			}
		}
		public override IEnumerable<string> GetAspects() {
			return GetAspectDiffs(mainFileNameTemplate).Values;
		}
		public override void SaveDifference(ModelApplicationBase model) {
			if(model == null) return;
			lastSavedFiles.Clear();
			SaveDifferenceIfNeed(model, mainFileNameTemplate, businessObjectsDiffsFiles);
			if(model.UnusableModel != null && model.UnusableModel.HasModification) {
				SaveUnusableDifferenceIfNeed(model.UnusableModel, UnusableDiffDefaultName);
			}
		}
		public List<string> GetBoFilesToSave(ModelApplicationBase model) {
			List<string> result = new List<string>();
			if(model != null) {
				UpdateBusinessObjects(model, businessObjectsDiffsFiles);
				string xml = writer.WriteToString(model, 0);
				XmlDocument document = new XmlDocument();
				if(string.IsNullOrEmpty(xml)) {
					document.AppendChild(document.CreateElement("Application"));
				}
				else {
					document.LoadXml(xml);
				}
				foreach(KeyValuePair<string, string> item in businessObjectsDiffsFiles) {
					if(NeedSaveBo(model, document, item.Key, item.Value)) {
						result.Add(item.Value);
					}
				}
			}
			return result;
		}
		private void SaveUnusableDifferenceIfNeed(ModelApplicationBase model, string fileNameTemplate) {
			if(model == null) return;
			for(int aspectIndex = 0; aspectIndex < model.AspectCount; ++aspectIndex) {
				string incrementIndex = GetFileIndexIfNeed(fileNameTemplate, model, aspectIndex);
				if(incrementIndex != null) {
					string xml = writer.WriteToString(model, aspectIndex);
					if(!string.IsNullOrEmpty(xml)) {
						string aspect = model.GetAspect(aspectIndex);
						string fileName = Path.Combine(storePath, GetFileNameForAspect(aspect, fileNameTemplate + incrementIndex));
						Encoding encoding = GetEncodingForFile(fileName);
						if(writer.WriteToFile(model, aspectIndex, fileName, encoding)) {
							lastSavedFiles.Add(fileName);
						}
					}
				}
			}
		}
		private void SaveDifferenceIfNeed(ModelApplicationBase model, string fileNameTemplate, Dictionary<string, string> businessObjects) {
			if(model == null) return;
			UpdateBusinessObjects(model, businessObjects);
			for(int aspectIndex = 0; aspectIndex < model.AspectCount; ++aspectIndex) {
				string aspect = model.GetAspect(aspectIndex);
				string fileName = Path.Combine(storePath, GetFileNameForAspect(aspect, fileNameTemplate));
				Encoding encoding = GetEncodingForFile(fileName);
				string xml = writer.WriteToString(model, aspectIndex);
				XmlDocument document = new XmlDocument();
				if(string.IsNullOrEmpty(xml)) {
					document.AppendChild(document.CreateElement("Application"));
				}
				else {
					document.LoadXml(xml);
				}
				if(aspectIndex == 0) {
					foreach(KeyValuePair<string, string> item in businessObjectsDiffsFiles) {
						SaveBusinessObjectDifferensesIfNeed(model, document, item.Key, item.Value, encoding);
					}
				}
				try {
					WriteXmlDocument(document, fileName, encoding);
					lastSavedFiles.Add(fileName);
				}
				catch(IOException) { }
			}
		}
		private static void UpdateBusinessObjects(ModelApplicationBase model, Dictionary<string, string> businessObjects) {
			IModelBOModel boModel = (IModelBOModel)model.GetNode("BOModel");
			foreach(string className in businessObjects.Keys) {
				IModelClass modelClass = boModel[className];
				if(modelClass == null) {
					throw new InvalidOperationException(string.Format("Can not find the '{0}' class in the model.", className));
				}
				((IModelClassDesignable)modelClass).IsDesigned = true;
			}
		}
		private List<XmlElement> CollectViewsForClass(ModelApplicationBase model, XmlDocument sourceModelDocument, string className) {
			IModelBOModel boModel = (IModelBOModel)model.GetNode("BOModel");
			IModelClass modelClass = boModel[className];
			IModelViews views = (IModelViews)model.GetNode("Views");
			List<XmlElement> viewNodes = new List<XmlElement>();
			foreach(IModelView view in views) {
				IModelObjectView objectView = view as IModelObjectView;
				if(objectView != null && objectView.ModelClass == modelClass) {
					string nodeName = objectView.GetType().Name.Remove(0, 5);
					XmlElement viewNode = (XmlElement)sourceModelDocument.DocumentElement.SelectSingleNode(string.Format("/Application/Views/{0}[@Id='{1}']", nodeName, objectView.Id));
					if(viewNode != null) {
						viewNode.ParentNode.RemoveChild(viewNode);
						viewNodes.Add(viewNode);
					}
				}
			}
			return viewNodes;
		}
		private XmlDocument CreateBoClassXml(List<XmlElement> viewNodes, XmlDocument sourceModelDocument, string className) {
			XmlDocument boDocument = new XmlDocument();
			XmlElement classNode = (XmlElement)sourceModelDocument.DocumentElement.SelectSingleNode(string.Format("/Application/BOModel/Class[@Name='{0}']", className));
			boDocument.AppendChild(boDocument.CreateElement("Application"));
			if(classNode != null) {
				classNode.ParentNode.RemoveChild(classNode);
				XmlElement boModelElement = boDocument.CreateElement("BOModel");
				boDocument.DocumentElement.AppendChild(boModelElement);
				boModelElement.AppendChild(boDocument.ImportNode(classNode, true));
			}
			XmlElement viewsElement = boDocument.CreateElement("Views");
			boDocument.DocumentElement.AppendChild(viewsElement);
			foreach(XmlElement viewNode in viewNodes) {
				viewsElement.AppendChild(boDocument.ImportNode(viewNode, true));
			}
			return boDocument;
		}
		private bool NeedSaveBo(ModelApplicationBase model, XmlDocument sourceModelDocument, string className, string classDiffsFileName) {
			List<XmlElement> viewNodes = CollectViewsForClass(model, sourceModelDocument, className);
			if(viewNodes.Count > 0) {
				XmlDocument boDocument = CreateBoClassXml(viewNodes, sourceModelDocument, className);
				return !CompareBOXml(boDocument, classDiffsFileName);
			}
			return false;
		}
		private void SaveBusinessObjectDifferensesIfNeed(ModelApplicationBase model, XmlDocument sourceModelDocument, string className, string classDiffsFileName, Encoding encoding) {
			List<XmlElement> viewNodes = CollectViewsForClass(model, sourceModelDocument, className);
			if(viewNodes.Count > 0) {
				XmlDocument boDocument = CreateBoClassXml(viewNodes, sourceModelDocument, className);
				if(!CompareBOXml(boDocument, classDiffsFileName)) {
					WriteXmlDocument(boDocument, classDiffsFileName, encoding);
					lastSavedFiles.Add(classDiffsFileName);
#if (DebugTest)
					if(TestBOXmlDocumentChanged != null) {
						FileSystemEventArgs args = new FileSystemEventArgs(WatcherChangeTypes.Changed, classDiffsFileName, classDiffsFileName);
						TestBOXmlDocumentChanged(this, args);
					}
#endif
				}
			}
		}
		private bool CompareBOXml(XmlDocument newBoDocument, string boFileName) {
			XmlDocument oldBoDocument = new XmlDocument();
			oldBoDocument.LoadXml(LoadModelXmlFromFile(boFileName));
			int index = oldBoDocument.InnerXml.IndexOf("<Application>");
			if(index != -1) {
				string oldXml = oldBoDocument.InnerXml.Substring(index);
				return newBoDocument.InnerXml == oldXml;
			}
			return false;
		}
		private static XmlWriterSettings GetXmlWriterSettings(Encoding encoding) {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = encoding;
			settings.OmitXmlDeclaration = false;
			settings.Indent = true;
			settings.NewLineHandling = NewLineHandling.Entitize;
			return settings;
		}
		private static void WriteXmlDocument(XmlDocument document, string fileName, Encoding encoding) {
			using(XmlWriter writer = XmlWriter.Create(fileName, GetXmlWriterSettings(encoding))) {
				document.WriteTo(writer);
			}
		}
		private bool OldFileEqualsNewUnusabelModel(string fileNameTemplate, ModelApplicationBase model, int aspectIndex, int index) {
			int prevIndex = index - 1;
			string fileIndex = prevIndex == 0 ? "" : prevIndex.ToString();
			string aspect = model.GetAspect(aspectIndex);
			string prevFileName = Path.Combine(storePath, GetFileNameForAspect(aspect, fileNameTemplate + fileIndex));
			string fileContent = File.ReadAllText(prevFileName);
			int startHeaderIndex = fileContent.IndexOf(@"<?");
			int endHeaderIndex = fileContent.IndexOf(@"?>");
			if(startHeaderIndex != -1 && endHeaderIndex != -1 && endHeaderIndex > startHeaderIndex) {
				fileContent = fileContent.Substring(endHeaderIndex + 2);
			}
			fileContent = ModelXmlHelper.SimplifyString(fileContent);
			string modelContent = ModelXmlHelper.SimplifyString(writer.WriteToString(model, aspectIndex));
			return fileContent == modelContent || (string.IsNullOrEmpty(modelContent) && fileContent == "<Application/>");
		}
		private string GetFileIndexIfNeed(string fileNameTemplate, ModelApplicationBase model, int aspectIndex) {
			string result = "";
			string aspect = model.GetAspect(aspectIndex);
			string fileName = Path.Combine(storePath, GetFileNameForAspect(aspect, fileNameTemplate));
			if(!File.Exists(fileName)) {
				return result;
			}
			else {
				int index = 0;
				while(File.Exists(fileName)) {
					index++;
					fileName = Path.Combine(storePath, GetFileNameForAspect(aspect, fileNameTemplate + index.ToString()));
				}
				if(index > 0) {
					if(!OldFileEqualsNewUnusabelModel(fileNameTemplate, model, aspectIndex, index)) {
						result = index == 0 ? "" : index.ToString();
					}
					else {
						result = null;
					}
				}
				else {
					result = "";
				}
			}
			return result;
		}
		public FileModelStore(string storePath, string fileNameTemplate) {
			this.storePath = storePath;
			this.mainFileNameTemplate = fileNameTemplate;
			lastSavedFiles = new List<string>();
		}
		public override bool ReadOnly { get { return false; } }
		public override string Name { get { return Path.Combine(storePath, mainFileNameTemplate + ModelFileExtension); } }
		public IList<string> LastSavedFiles { get { return lastSavedFiles; } }
		private string LoadModelXmlFromFile(string fileName) {
			string xml = "";
			using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
				Guard.ArgumentNotNull(stream, "stream");
				if(stream.Length > 0) {
					Encoding encoding = new ModelXmlReader().GetStreamEncoding(stream) ?? Encoding.UTF8;
					using(StreamReader reader = new StreamReader(stream, encoding)) {
						xml = reader.ReadToEnd();
					}
				}
			}
			return xml;
		}
#if (DebugTest)
		public event FileSystemEventHandler TestBOXmlDocumentChanged;
#endif
	}
	#region Obsolete 14.2
	[Obsolete("Use the 'ModelDifferenceDbStore' class instead.", false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class DatabaseEntityModelStore : ModelDifferenceStore {
		private XafApplication application;
		protected virtual ISupportModelDifferences GetDatabaseEntity(IObjectSpace os) {
			return (ISupportModelDifferences)os.GetObject(SecuritySystem.CurrentUser);
		}
		public DatabaseEntityModelStore(XafApplication application) {
			this.application = application;
		}
		public override string Name { get { return GetType().Name; } }
		public override void Load(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			using(IObjectSpace os = application.CreateObjectSpace(SecuritySystem.CurrentUser != null ? SecuritySystem.CurrentUser.GetType() : null)) {
				ISupportModelDifferences supportModelSettings = GetDatabaseEntity(os);
				if(supportModelSettings != null && !string.IsNullOrEmpty(supportModelSettings.Model)) {
					new ModelXmlReader().ReadFromString(model, "", supportModelSettings.Model);
				}
			}
		}
		public override void SaveDifference(ModelApplicationBase model) {
			using(IObjectSpace os = application.CreateObjectSpace(SecuritySystem.CurrentUser != null ? SecuritySystem.CurrentUser.GetType() : null)) {
				ISupportModelDifferences supportModelSettings = GetDatabaseEntity(os);
				if(supportModelSettings != null) {
					supportModelSettings.Model = new ModelXmlWriter().WriteToString(model, 0);
					os.CommitChanges();
				}
			}
		}
	}
	#endregion
}

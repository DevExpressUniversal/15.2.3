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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	[PredefinedFileSystemProvider]
	public class DataSourceFileSystemProvider : FileSystemProviderBase {
		bool dataLoaded = false;
		string lastWriteTimeFieldName;
		string nameFieldName;
		string parentKeyFieldName;
		string keyFieldName;
		string isFolderFieldName;
		string fileBinaryContentFieldName;
		FileManagerBoundEntity rootEntity;
		DataHelperCore dataHelper;
		public DataSourceFileSystemProvider(string rootFolder)
			: base(rootFolder) {
			DataLoaded = false;
		}
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderLastWriteTimeFieldName")]
#endif
		public string LastWriteTimeFieldName { get { return lastWriteTimeFieldName; } set { lastWriteTimeFieldName = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderNameFieldName")]
#endif
		public string NameFieldName { get { return nameFieldName; } set { nameFieldName = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderParentKeyFieldName")]
#endif
		public string ParentKeyFieldName { get { return parentKeyFieldName; } set { parentKeyFieldName = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderKeyFieldName")]
#endif
		public string KeyFieldName { get { return keyFieldName; } set { keyFieldName = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderIsFolderFieldName")]
#endif
		public string IsFolderFieldName { get { return isFolderFieldName; } set { isFolderFieldName = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderFileBinaryContentFieldName")]
#endif
		public string FileBinaryContentFieldName { get { return fileBinaryContentFieldName; } set { fileBinaryContentFieldName = value; } }
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderDataHelper")]
#endif
		public DataHelperCore DataHelper { get { return dataHelper; } set { dataHelper = value; } }
		internal FileManagerBoundEntity RootEntity {
			get {
				if(!DataLoaded)
					LoadData();
				CheckRootEntity();
				return this.rootEntity;
			}
		}
		bool DataLoaded {
			get { return dataLoaded; }
			set { dataLoaded = value; }
		}
		internal static DateTime LastDateTime {
			get {
				DateTime curDate = DateTime.Now; 
				return new DateTime(curDate.Year, curDate.Month, curDate.Day, curDate.Hour, curDate.Minute, curDate.Second);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("DataSourceFileSystemProviderRootFolderDisplayName")]
#endif
		public override string RootFolderDisplayName {
			get {
				string name = RootEntity.Name;
				return string.IsNullOrEmpty(name) ? base.RootFolderDisplayName : name;
			}
		}
		public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder) {
			FileManagerBoundEntity parentEntity = RootEntity.Find(parentFolder.RelativeName, true, false);
			List<FileManagerFolder> folders = parentEntity.Folders.ConvertAll<FileManagerFolder>(ConvertEntity<FileManagerFolder>(parentFolder));
			folders.Sort(CompareByNames);
			return folders;
		}
		public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder) {
			FileManagerBoundEntity parentEntity = RootEntity.Find(folder.RelativeName, true, false);
			List<FileManagerFile> files = parentEntity.Files.ConvertAll<FileManagerFile>(ConvertEntity<FileManagerFile>(folder));
			files.Sort(CompareByNames);
			return files;
		}
		public override bool Exists(FileManagerFile file) {
			return RootEntity.Find(file.RelativeName, false, false) != null;
		}
		public override bool Exists(FileManagerFolder folder) {
			return RootEntity.Find(folder.RelativeName, true, false) != null;
		}
		public override DateTime GetLastWriteTime(FileManagerFile file) {
			if(string.IsNullOrEmpty(LastWriteTimeFieldName))
				return base.GetLastWriteTime(file);
			FileManagerBoundEntity entity = RootEntity.Find(file.RelativeName, false, true);
			return entity.LastWriteTime;
		}
		public override Stream ReadFile(FileManagerFile file) {
			FileManagerBoundEntity entity = RootEntity.Find(file.RelativeName, false, true);
			byte[] bytes = entity.BinaryContent as byte[];
			return bytes != null ? new MemoryStream(bytes) : Stream.Null;
		}
		public override long GetLength(FileManagerFile file) {
			FileManagerBoundEntity entity = RootEntity.Find(file.RelativeName, false, true);
			byte[] bytes = entity.BinaryContent as byte[];
			return bytes != null ? bytes.Length : 0;
		}
		public override void DeleteFile(FileManagerFile file) {
			FileManagerBoundEntity entity = RootEntity.Find(file.RelativeName, false, true);
			DeleteEntity(entity);
		}
		public override void DeleteFolder(FileManagerFolder folder) {
			FileManagerBoundEntity entity = RootEntity.Find(folder.RelativeName, true, true);
			DeleteFolderChilds(entity);
			DeleteEntity(entity);
		}
		public override void RenameFile(FileManagerFile file, string name) {
			FileManagerBoundEntity entity = RootEntity.Find(file.RelativeName, false, true);
			UpdateEntity(entity, delegate(OrderedDictionary newValues) {
				newValues[NameFieldName] = name;
			});
		}
		public override void RenameFolder(FileManagerFolder folder, string name) {
			FileManagerBoundEntity entity = RootEntity.Find(folder.RelativeName, true, true);
			UpdateEntity(entity, delegate(OrderedDictionary newValues) {
				newValues[NameFieldName] = name;
			});
		}
		public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			FileManagerBoundEntity fileEntity = RootEntity.Find(file.RelativeName, false, true);
			UpdateEntity(fileEntity, delegate(OrderedDictionary newValues) {
				newValues[ParentKeyFieldName] = RootEntity.Find(newParentFolder.RelativeName, true, true).ID;
			});
		}
		public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			FileManagerBoundEntity folderEntity = RootEntity.Find(folder.RelativeName, true, true);
			UpdateEntity(folderEntity, delegate(OrderedDictionary newValues) {
				newValues[ParentKeyFieldName] = RootEntity.Find(newParentFolder.RelativeName, true, true).ID;
			});
		}
		public override void CopyFile(FileManagerFile file, FileManagerFolder newParentFolder) {
			FileManagerBoundEntity fileEntity = RootEntity.Find(file.RelativeName, false, true);
			CopyCore(fileEntity, newParentFolder.RelativeName, false);
		}
		public override void CopyFolder(FileManagerFolder folder, FileManagerFolder newParentFolder) {
			List<FileManagerFolder> folders = new List<FileManagerFolder>();
			folders.Add(folder);
			FillSubFoldersList(folder, folders);
			foreach(FileManagerFolder currentFolder in folders) {
				FileManagerBoundEntity folderEntity = RootEntity.Find(currentFolder.RelativeName, true, true);
				var subFolderNameOffset = string.IsNullOrEmpty(folder.Parent.RelativeName) ? folder.Parent.RelativeName.Length : folder.Parent.RelativeName.Length + 1;
				string folderPath = currentFolder == folder ? newParentFolder.RelativeName :
									Path.Combine(newParentFolder.RelativeName, currentFolder.Parent.RelativeName.Substring(subFolderNameOffset));
				CopyCore(folderEntity, folderPath, true);				
				foreach(FileManagerFile currentFile in currentFolder.GetFiles()) {
					FileManagerBoundEntity fileEntity = RootEntity.Find(currentFile.RelativeName, false, true);
					string filePath = Path.Combine(folderPath, currentFolder.Name);
					CopyCore(fileEntity, filePath, false);
				}
			}					 
		}
		void CopyCore(FileManagerBoundEntity entity, string destinationPath, bool isFolder) {
			FileManagerBoundEntity newParentFolderEntity = RootEntity.Find(destinationPath, true, true);			
			if(newParentFolderEntity.Find(entity.Name, true, false) != null)
				throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
			ASPxDataInsertingEventArgs args = new ASPxDataInsertingEventArgs();
			DataSourceView view = DataHelper.GetView();
			InitializeOrderedDictionary(args.NewValues, GetInsertParameters(view));
			FillOrderedDictionary(args.NewValues, null, entity.Name, newParentFolderEntity.ID, isFolder, LastDateTime, entity.BinaryContent);
			view.Insert(args.NewValues, new DataSourceViewOperationCallback(HandleDataSourceViewEditOperationCallback)); 
		}
		void FillSubFoldersList(FileManagerFolder folder, List<FileManagerFolder> list) {
			foreach(FileManagerFolder subFolder in folder.GetFolders()) {
				list.Add(subFolder);
				FillSubFoldersList(subFolder, list);
			}
		}
		public override void CreateFolder(FileManagerFolder parent, string name) {
			FileManagerBoundEntity entity = RootEntity.Find(parent.RelativeName, true, true);
			if(entity.Find(name, true, false) != null)
				throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
			ASPxDataInsertingEventArgs args = new ASPxDataInsertingEventArgs();
			DataSourceView view = DataHelper.GetView();
			InitializeOrderedDictionary(args.NewValues, GetInsertParameters(view));
			FillOrderedDictionary(args.NewValues, null, name, entity.ID, true, LastDateTime, null);
			view.Insert(args.NewValues, new DataSourceViewOperationCallback(HandleDataSourceViewEditOperationCallback));
		}
		public override void UploadFile(FileManagerFolder folder, string fileName, Stream content) {
			FileManagerBoundEntity parent = RootEntity.Find(folder.RelativeName, true, true);
			if(parent.Find(fileName, false, false) != null)
				throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
			ASPxDataInsertingEventArgs args = new ASPxDataInsertingEventArgs();
			DataSourceView view = DataHelper.GetView();
			InitializeOrderedDictionary(args.NewValues, GetInsertParameters(view));
			FillOrderedDictionary(args.NewValues, null, fileName, parent.ID, false, LastDateTime, CommonUtils.GetBytesFromStream(content));
			view.Insert(args.NewValues, new DataSourceViewOperationCallback(HandleDataSourceViewEditOperationCallback));
		}
		void DeleteEntity(FileManagerBoundEntity entity) {
			ASPxDataDeletingEventArgs args = new ASPxDataDeletingEventArgs();
			FillKeysDictionary(args.Keys, entity);
			DataSourceView view = DataHelper.GetView();
			InitializeOrderedDictionary(args.Values, GetUpdateParameters(view));
			FillOrderedDictionary(args.Values, entity);
			view.Delete(args.Keys, args.Values, new DataSourceViewOperationCallback(HandleDataSourceViewEditOperationCallback));
		}
		void UpdateEntity(FileManagerBoundEntity entity, Action<OrderedDictionary> setNewValue) {
			ASPxDataUpdatingEventArgs args = new ASPxDataUpdatingEventArgs();
			FillKeysDictionary(args.Keys, entity);
			DataSourceView view = DataHelper.GetView();
			InitializeOrderedDictionary(args.OldValues, GetUpdateParameters(view));
			InitializeOrderedDictionary(args.NewValues, GetUpdateParameters(view));
			FillOrderedDictionary(args.OldValues, entity);
			FillOrderedDictionary(args.NewValues, entity);
			FillValue(args.NewValues, LastWriteTimeFieldName, LastDateTime);
			setNewValue(args.NewValues);
			view.Update(args.Keys, args.NewValues, args.OldValues, new DataSourceViewOperationCallback(HandleDataSourceViewEditOperationCallback));
		}
		void LoadData() {
			DataLoaded = true;
			IEnumerable result = DataHelper.Select();
			Dictionary<object, FileManagerBoundEntity> entities = new Dictionary<object, FileManagerBoundEntity>();
			if(result != null) {
				foreach(object entityData in result) {
					FileManagerBoundEntity entity = new FileManagerBoundEntity(this, entityData);
					entities.Add(entity.ID, entity);
				}
				CreateDataTree(entities);
			}
			else
				throw new Exception("DataSource is empty");
		}
		void CheckRootEntity() {
			if(this.rootEntity == null)
				throw new Exception(StringResources.FileManager_ErrorRootFolderNotSpecified);
		}
		void CreateDataTree(Dictionary<object, FileManagerBoundEntity> entities) {
			FileManagerBoundEntity root = null;
			foreach(FileManagerBoundEntity entity in entities.Values) {
				object pid = entity.ParentID;
				if(entity.IsFolder && (Object.Equals(entity.ID, pid) || !entities.ContainsKey(pid)))
					root = entity;
				else if(entities.ContainsKey(pid))
					entities[pid].Childs.Add(entity);
			}
			if(root == null)
				throw new Exception(StringResources.FileManager_ErrorRootFolderNotSpecified);
			this.rootEntity = string.IsNullOrEmpty(RootFolder) || string.Equals(RootFolder, root.Name, StringComparison.InvariantCultureIgnoreCase)
				? root
				: root.Find(RootFolder, true, false);
			CheckRootEntity();
		}
		void FillKeysDictionary(OrderedDictionary keys, FileManagerBoundEntity entity) {
			keys.Add(KeyFieldName, entity.ID);
		}
		void InitializeOrderedDictionary(OrderedDictionary values, ParameterCollection parameters) {
			if(parameters != null) {
				foreach(Parameter par in parameters)
					values.Add(par.Name, null);
			}
			else {
				string[] fields = new string[] { KeyFieldName, NameFieldName, ParentKeyFieldName, LastWriteTimeFieldName, IsFolderFieldName, FileBinaryContentFieldName };
				foreach(string field in fields) {
					if(!string.IsNullOrEmpty(field))
						values.Add(field, null);
				}
			}
		}
		void FillOrderedDictionary(OrderedDictionary values, FileManagerBoundEntity entity) {
			FillOrderedDictionary(values, entity.ID, entity.Name, entity.ParentID, entity.IsFolder, entity.LastWriteTime, entity.BinaryContent);
		}
		void FillOrderedDictionary(OrderedDictionary values, object id, string name, object pid, bool isFolder, DateTime lastWriteTime, object content) {
			FillValue(values, KeyFieldName, id);
			FillValue(values, NameFieldName, name);
			FillValue(values, ParentKeyFieldName, pid);
			FillValue(values, IsFolderFieldName, isFolder);
			FillValue(values, LastWriteTimeFieldName, lastWriteTime);
			FillValue(values, FileBinaryContentFieldName, content);
		}
		void FillValue(OrderedDictionary values, string fieldName, object value) {
			if(!string.IsNullOrEmpty(fieldName) && values.Contains(fieldName))
				values[fieldName] = value;
		}
		void DeleteFolderChilds(FileManagerBoundEntity entity) {
			List<FileManagerBoundEntity> entities = GetChildsHierarchically(entity, null);
			foreach(FileManagerBoundEntity ent in entities) {
				try {
					DeleteEntity(ent);
				}
				catch { }
			}
		}
		List<FileManagerBoundEntity> GetChildsHierarchically(FileManagerBoundEntity entity, List<FileManagerBoundEntity> entities) {
			entities = entities == null ? new List<FileManagerBoundEntity>() : entities;
			foreach(FileManagerBoundEntity ent in entity.Childs) {
				if(ent.IsFolder && ent.Childs.Count > 0)
					GetChildsHierarchically(ent, entities);
				entities.Add(ent);
			}
			return entities;
		}
		ParameterCollection GetUpdateParameters(DataSourceView view) {
			SqlDataSourceView sqlView = view as SqlDataSourceView;
			if(sqlView != null) return sqlView.UpdateParameters;
			ObjectDataSourceView objView = view as ObjectDataSourceView;
			if(objView != null) return objView.UpdateParameters;
			return null;
		}
		ParameterCollection GetInsertParameters(DataSourceView view) {
			SqlDataSourceView sqlView = view as SqlDataSourceView;
			if(sqlView != null) return sqlView.InsertParameters;
			ObjectDataSourceView objView = view as ObjectDataSourceView;
			if(objView != null) return objView.InsertParameters;
			return null;
		}
		bool HandleDataSourceViewEditOperationCallback(int affectedRecords, Exception e) {
			DataLoaded = false;
			return e == null;
		}
		static int CompareByNames(FileManagerItem item1, FileManagerItem item2) {
			return string.Compare(item1.Name, item2.Name);
		}
		Converter<FileManagerBoundEntity, T> ConvertEntity<T>(FileManagerFolder parentFolder) where T : FileManagerItem {
			return new Converter<FileManagerBoundEntity, T>(delegate(FileManagerBoundEntity entity) {
				return (T)Activator.CreateInstance(typeof(T), this, parentFolder, entity.Name, entity.ID.ToString());
			});
		}
	}
	internal class FileManagerBoundEntity {
		DataSourceFileSystemProvider provider;
		List<FileManagerBoundEntity> childs;
		object dataSourceObj;
		public FileManagerBoundEntity(DataSourceFileSystemProvider provider, object data) {
			this.dataSourceObj = data;
			this.provider = provider;
			this.childs = new List<FileManagerBoundEntity>();
		}
		public object ID { get { return GetFieldValue(Provider.KeyFieldName); } }
		public object ParentID { get { return GetFieldValue(Provider.ParentKeyFieldName); } }
		public string Name { get { return GetFieldValue(Provider.NameFieldName) as string; } }
		public bool IsFolder { get { return Convert.ToBoolean(GetFieldValue(Provider.IsFolderFieldName)); } }
		public object BinaryContent { get { return GetFieldValue(Provider.FileBinaryContentFieldName); } }
		public DateTime LastWriteTime {
			get {
				object value = GetFieldValue(Provider.LastWriteTimeFieldName);
				return value is DateTime ? (DateTime)value : DataSourceFileSystemProvider.LastDateTime;
			}
		}
		public List<FileManagerBoundEntity> Childs { get { return childs; } }
		public List<FileManagerBoundEntity> Files { get { return Childs.FindAll(delegate(FileManagerBoundEntity ent) { return !ent.IsFolder; }); } }
		public List<FileManagerBoundEntity> Folders { get { return Childs.FindAll(delegate(FileManagerBoundEntity ent) { return ent.IsFolder; }); } }
		protected object DataSourceObj { get { return dataSourceObj; } }
		protected DataSourceFileSystemProvider Provider { get { return provider; } }
		internal FileManagerBoundEntity Find(string path, bool isFolder, bool throwException) {
			FileManagerBoundEntity entity = string.IsNullOrEmpty(path) && IsFolder == isFolder
				? this
				: Find(new ArraySegment<string>(path.Split('\\')), isFolder);
			if(throwException && entity == null)
				throw new FileManagerIOException(isFolder ? FileManagerErrors.FolderNotFound : FileManagerErrors.FileNotFound);
			return entity;
		}
		FileManagerBoundEntity Find(ArraySegment<string> chain, bool isFolder) {
			bool isLastElement = chain.Count == 1;
			FileManagerBoundEntity entity = Childs.Find(delegate(FileManagerBoundEntity ent) {
				return string.Equals(ent.Name, chain.Array[chain.Offset], StringComparison.InvariantCultureIgnoreCase) && ent.IsFolder == (isFolder || !isLastElement);
			});
			return entity == null || isLastElement ? entity : entity.Find(new ArraySegment<string>(chain.Array, chain.Offset + 1, chain.Count - 1), isFolder);
		}
		object GetFieldValue(string fieldName) {
			object value = null;
			if(!string.IsNullOrEmpty(fieldName))
				ReflectionUtils.TryToGetPropertyValue(dataSourceObj, fieldName, out value);
			return value;
		}
		public override string ToString() {
			return Name;
		}
	}
	class FileManagerItemComparer {
		public int Compare(FileManagerItem i1, FileManagerItem i2) {
			return i1.Name.CompareTo(i2.Name);
		}
	}
}

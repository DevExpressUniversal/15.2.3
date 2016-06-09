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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System.Collections.Generic;
namespace DevExpress.XtraEditors.Registrator {
	public enum EditImageIndexes {
		TextEdit = 0, ButtonEdit = 1, SpinEdit = 2, CheckEdit = 3, ComboBoxEdit = 4, ImageComboBoxEdit = 5, DateEdit = 6,
		CalcEdit = 7, ColorEdit = 8, MemoExEdit = 9, ImageEdit = 10, MemoEdit = 11, PictureEdit = 12, ProgressBarControl = 13,
		LookUpEdit = 14, PopupContainerEdit = 15, RadioGroup = 16, TimeEdit = 17, HyperLinkEdit = 18, MRUEdit = 19, ToggleSwitch = 20, SparklineEdit = 21
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class UserRepositoryItem : Attribute {
		string registratorMethodName;
		public UserRepositoryItem(string registratorMethodName) {
			this.registratorMethodName = registratorMethodName;
		}
		public string RegistratorMethodName { get { return registratorMethodName; } }
	}
	public enum ShowInContainerDesigner { Anywhere, OnlyInBars, Never }
	public class EditorClassInfo {
		[ThreadStatic]
		static ImageList editImageList = null;
		ShowInContainerDesigner allowInplaceEditing = ShowInContainerDesigner.Anywhere;
		protected static ImageList EditImageList {
			get {
				if(editImageList == null) {
					editImageList = DevExpress.Utils.ResourceImageHelper.CreateImageListFromResources("DevExpress.XtraEditors.Images.Editors.bmp", typeof(EditorClassInfo).Assembly, new Size(16, 16));
				}
				return editImageList;
			}
		}
		protected static Image GetImage(EditImageIndexes image) {
			int index = (int)image;
			if(index >= 0 && index < EditImageList.Images.Count) {
				return EditImageList.Images[index];
			}
			return null;
		}
		string name;
		bool designTimeVisible;
		Type editorType, repositoryType, viewInfoType, accessibleType;
		BaseEditPainter painter;
		ConstructorInfo viewInfoConstructor, editorConstructor, repositoryConstructor, accessibleConstructor;
		ConstructorInfo ViewInfoConstructor {
			get {
				if(viewInfoConstructor == null)
					viewInfoConstructor = ViewInfoType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { typeof(RepositoryItem) }, null);
				return viewInfoConstructor;
			}
		}
		ConstructorInfo EditorConstructor {
			get {
				if(editorConstructor == null)
					editorConstructor = EditorType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { }, null);
				return editorConstructor;
			}
		}
		ConstructorInfo RepositoryConstructor {
			get {
				if(repositoryConstructor == null)
					repositoryConstructor = RepositoryType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { }, null);
				return repositoryConstructor;
			}
		}
		ConstructorInfo AccessibleConstructor {
			get {
				if(accessibleConstructor == null)
					accessibleConstructor = AccessibleType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { typeof(RepositoryItem) }, null);
				return accessibleConstructor;
			}
		}
		Image image;
		int imageIndex;
		public EditorClassInfo(string name, Type editorType, Type repositoryType, Type viewInfoType, BaseEditPainter painter, bool designTimeVisible, EditImageIndexes image)
			:
			this(name, editorType, repositoryType, viewInfoType, painter, designTimeVisible, image, typeof(DevExpress.Accessibility.BaseEditAccessible)) { }
		public EditorClassInfo(string name, Type editorType, Type repositoryType, Type viewInfoType, BaseEditPainter painter, bool designTimeVisible, EditImageIndexes image, Type accessibleType)
			:
			this(name, editorType, repositoryType, viewInfoType, painter, designTimeVisible, null, accessibleType) {
			this.imageIndex = (int)image;
		}
		public EditorClassInfo(string name, Type editorType, Type repositoryType, Type viewInfoType, BaseEditPainter painter, bool designTimeVisible)
			:
			this(name, editorType, repositoryType, viewInfoType, painter, designTimeVisible, null) { }
		public EditorClassInfo(string name, Type editorType, Type repositoryType, Type viewInfoType, BaseEditPainter painter, bool designTimeVisible, Image image)
			:
			this(name, editorType, repositoryType, viewInfoType, painter, designTimeVisible, image, typeof(DevExpress.Accessibility.BaseEditAccessible)) { }
		public EditorClassInfo(string name, Type editorType, Type repositoryType, Type viewInfoType, BaseEditPainter painter, bool designTimeVisible, Image image, Type accessibleType) {
			this.imageIndex = -1;
			this.image = image;
			this.name = name;
			this.editorType = editorType;
			this.repositoryType = repositoryType;
			this.viewInfoType = viewInfoType;
			this.painter = painter;
			this.designTimeVisible = designTimeVisible;
			this.accessibleType = accessibleType;
			ExtractConstructors();
		}
		public virtual ShowInContainerDesigner AllowInplaceEditing {
			get { return allowInplaceEditing; }
			set { allowInplaceEditing = value; }
		}
		public virtual int ImageIndex { get { return imageIndex; } }
		public virtual Image Image {
			get {
				if(image == null) {
					System.Drawing.Design.ToolboxItem item = new System.Drawing.Design.ToolboxItem(EditorType);
					if(item != null) image = item.Bitmap;
				}
				if(image == null)
					image = GetImage((EditImageIndexes)imageIndex);
				return image;
			}
			set {
				image = value;
			}
		}
		protected virtual void ExtractConstructors() {
		}
		public virtual DevExpress.Accessibility.BaseAccessible CreateAccessible(RepositoryItem item) {
			return AccessibleConstructor.Invoke(new object[] { item }) as DevExpress.Accessibility.BaseAccessible;
		}
		public virtual BaseEditViewInfo CreateViewInfo(RepositoryItem item) {
			return ViewInfoConstructor.Invoke(new object[] { item }) as BaseEditViewInfo;
		}
		public virtual BaseEdit CreateEditor() {
			return EditorConstructor.Invoke(null) as BaseEdit;
		}
		public virtual RepositoryItem CreateRepositoryItem() {
			return RepositoryConstructor.Invoke(null) as RepositoryItem;
		}
		public string Name { get { return name; } }
		public bool DesignTimeVisible { get { return designTimeVisible; } }
		public Type AccessibleType { get { return accessibleType; } }
		public Type EditorType { get { return editorType; } }
		public Type RepositoryType { get { return repositoryType; } }
		public BaseEditPainter Painter { get { return painter; } }
		public Type ViewInfoType { get { return viewInfoType; } }
		public override string ToString() { return Name; }
	}
	public abstract class EditorClassInfoCollection : ICollection {
		readonly List<EditorClassInfo> InnerList = new List<EditorClassInfo>();
		readonly Dictionary<string, int> _NamesToIndexes = new Dictionary<string, int>();
		void ClearNamesIndex() {
			_NamesToIndexes.Clear();
		}
		Dictionary<string, int> NamesToIndexes {
			get {
				if(_NamesToIndexes.Count != InnerList.Count) {
					_NamesToIndexes.Clear();
					for(int i = 0; i < InnerList.Count; ++i) {
						_NamesToIndexes.Add(InnerList[i].Name, i);
					}
				}
				return _NamesToIndexes;
			}
		}
		public virtual void Add(EditorClassInfo itemInfo) {
			lock(SyncRoot) {
				int oldIndex;
				if(NamesToIndexes.TryGetValue(itemInfo.Name, out oldIndex)) {
					InnerList.RemoveAt(oldIndex);
					InnerList.Insert(oldIndex, itemInfo);
				}
				else {
					InnerList.Add(itemInfo);
					ClearNamesIndex();
				}
			}
		}
		public virtual EditorClassInfo this[string name] {
			get {
				lock(SyncRoot) {
					int index;
					if(NamesToIndexes.TryGetValue(name, out index)) {
						return InnerList[index];
					} else {
						AddDefEditor(name);
						if(NamesToIndexes.TryGetValue(name, out index)) {
							return InnerList[index];
						}
					}
					return null;
				}
			}
		}
		public virtual bool Contains(string name) {
			lock(SyncRoot) {
				return this[name] != null;
			}
		}
		public virtual EditorClassInfo this[int index] {
			get {
				lock(this) {
					if(index < 0 || index >= InnerList.Count)
						return null;
					return InnerList[index];
				}
			}
		}
		public virtual int IndexOf(EditorClassInfo itemInfo) {
			lock(SyncRoot) {
				return InnerList.IndexOf(itemInfo);
			}
		}
#if DEBUGTEST
		[Obsolete("Remove(...) not required anymore. It must be removed.")] 
#endif
		public virtual void Remove(EditorClassInfo itemInfo) {
			lock(SyncRoot) {
				ClearNamesIndex();
				InnerList.Remove(itemInfo);
			}
		}
		public virtual void CopyTo(Array array, int index) {
			lock(SyncRoot) {
				for(int i = 0; i < Count; ++i) {
					((IList)array)[i + index] = this[i];
				}
			}
		}
		public virtual int Count {
			get {
				lock(SyncRoot) {
					AddAll();
					return InnerList.Count;
				}
			}
		}
		public bool IsSynchronized {
			get { return true; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public virtual IEnumerator GetEnumerator() {
			lock(SyncRoot) {
				AddAll();
				return InnerList.GetEnumerator();
			}
		}
		protected abstract void AddDefEditor(string name);
		protected abstract void AddAll();
	}
	public class EditorClassInfoDefaultCollection : EditorClassInfoCollection {
		readonly Dictionary<string, string> AskedClasses = new Dictionary<string, string>();
		bool allAdded = false;
		static string[] allClasses = new string[] {
				 "BaseEdit",
				 "TextEdit",
				 "ButtonEdit",
				 "BaseSpinEdit",
				 "SpinEdit",
				 "TimeSpanEdit",
				 "TimeEdit",
				 "PopupBaseEdit",
				 "ComboBoxEdit",
				 "ImageComboBoxEdit",
				 "BreadCrumbEdit",
				 "TokenEdit",
				 "MRUEdit",
				 "CheckEdit",
				 "PictureEdit",
				 "MemoEdit",
				 "RadioGroup",
				 "HyperLinkEdit",
				 "PopupContainerEdit",
				 "BlobBaseEdit",
				 "ImageEdit",
				 "MemoExEdit",
				 "ProgressBarControl",
				 "MarqueeProgressBarControl",
				 "DateEdit",
				 "CalcEdit",
				 "ColorEdit",
				 "LookUpEdit",
				 "TrackBarControl",
				 "RangeTrackBarControl",
				 "FontEdit",
				 "ZoomTrackBarControl",
				 "CheckedComboBoxEdit",
				 "RichTextEdit",
				 "ColorPickEdit",
				 "ToggleSwitch",
				 "SearchControl",
				 "RatingControl",
				 RepositoryItemSparklineEdit.SparklineEditName
		};
		protected override void AddDefEditor(string name) {
			if(AskedClasses.ContainsKey(name))
				return;
			AskedClasses.Add(name, name);
			switch(name) {
				case "BaseEdit":
					Add(CreateClassInfoBaseEdit());
					break;
				case "TextEdit":
					Add(CreateClassInfoTextEdit());
					break;
				case "ButtonEdit":
					Add(CreateClassInfoButtonEdit());
					break;
				case "BaseSpinEdit":
					Add(CreateClassInfoBaseSpinEdit());
					break;
				case "SpinEdit":
					Add(CreateClassInfoSpinEdit());
					break;
				case "TimeEdit":
					Add(CreateClassInfoTimeEdit());
					break;
				case "PopupBaseEdit":
					Add(CreateClassInfoPopupBaseEdit());
					break;
				case "ComboBoxEdit":
					Add(CreateClassInfoComboBoxEdit());
					break;
				case "ImageComboBoxEdit":
					Add(CreateClassInfoImageComboBoxEdit());
					break;
				case "BreadCrumbEdit":
					Add(CreateClassInfoBreadCrumbEdit());
					break;
				case "TokenEdit":
					Add(CreateClassInfoTokenEdit());
					break;
				case "MRUEdit":
					Add(CreateClassInfoMRUEdit());
					break;
				case "CheckEdit":
					Add(CreateClassInfoCheckEdit());
					break;
				case "ToggleSwitch":
					Add(CreateClassInfoToggleSwitch());
					break;
				case "PictureEdit":
					Add(CreateClassInfoPictureEdit());
					break;
				case "MemoEdit":
					Add(CreateClassInfoMemoEdit());
					break;
				case "RadioGroup":
					Add(CreateClassInfoRadioGroup());
					break;
				case "HyperLinkEdit":
					Add(CreateClassInfoHyperLinkEdit());
					break;
				case "PopupContainerEdit":
					Add(CreateClassInfoPopupContainerEdit());
					break;
				case "BlobBaseEdit":
					Add(CreateClassInfoBlobBaseEdit());
					break;
				case "ImageEdit":
					Add(CreateClassInfoImageEdit());
					break;
				case "MemoExEdit":
					Add(CreateClassInfoMemoExEdit());
					break;
				case "ProgressBarControl":
					Add(CreateClassInfoProgressBarControl());
					break;
				case "MarqueeProgressBarControl":
					Add(CreateClassInfoMarqueeProgressBarControl());
					break;
				case "DateEdit":
					Add(CreateClassInfoDateEdit());
					break;
				case "CalcEdit":
					Add(CreateClassInfoCalcEdit());
					break;
				case "ColorEdit":
					Add(CreateClassInfoColorEdit());
					break;
				case "LookUpEdit":
					Add(CreateClassInfoLookUpEdit());
					break;
				case "TrackBarControl":
					Add(CreateClassInfoTrackBarControl());
					break;
				case "RangeTrackBarControl":
					Add(CreateClassInfoRangeTrackBarControl());
					break;
				case "FontEdit":
					Add(CreateClassInfoFontEdit());
					break;
				case "ZoomTrackBarControl":
					Add(CreateClassInfoZoomTrackBarControl());
					break;
				case "CheckedComboBoxEdit":
					Add(CreateClassInfoCheckedComboBoxEdit());
					break;
				case "RichTextEdit":
					RegisterRichTextEdit();
					break;
				case "ColorPickEdit":
					RegisterColorPickEdit();
					break;
				case "SearchControl":
					Add(CreateClassInfoSearchControl());
					break;
				case "RatingControl":
					Add(CreateClassInfoRatingControl());
					break;
				case "TimeSpanEdit":
					Add(CreateClassInfoTimeSpanEdit());
					break;
				case RepositoryItemSparklineEdit.SparklineEditName:
					Add(CreateClassInfoSparklineEdit());
					break;
			}
		}
		protected override void AddAll() {
			if(allAdded)
				return;
			foreach(string nm in allClasses)
				AddDefEditor(nm);
			allAdded = true;
		}
		protected virtual EditorClassInfo CreateClassInfoBaseEdit() {
			return new EditorClassInfo("BaseEdit", typeof(BaseEdit), typeof(RepositoryItem), typeof(BaseEditViewInfo), new BaseEditPainter(), false, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.BaseAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoSearchControl() {
			return new EditorClassInfo("SearchControl", typeof(SearchControl), typeof(RepositoryItemSearchControl), typeof(SearchControlViewInfo), new ButtonEditPainter(), true, EditImageIndexes.MemoEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)) { AllowInplaceEditing = ShowInContainerDesigner.OnlyInBars };
		}
		protected virtual EditorClassInfo CreateClassInfoSparklineEdit() {
			return new EditorClassInfo(RepositoryItemSparklineEdit.SparklineEditName, typeof(SparklineEdit), typeof(RepositoryItemSparklineEdit), typeof(SparklineEditViewInfo), new SparklineEditPainter(), true, EditImageIndexes.SparklineEdit, typeof(DevExpress.Accessibility.BaseEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoTextEdit() {
			return new EditorClassInfo("TextEdit", typeof(TextEdit), typeof(RepositoryItemTextEdit), typeof(TextEditViewInfo), new TextEditPainter(), true, EditImageIndexes.TextEdit, typeof(DevExpress.Accessibility.TextEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoButtonEdit() {
			return new EditorClassInfo("ButtonEdit", typeof(ButtonEdit), typeof(RepositoryItemButtonEdit), typeof(ButtonEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoBaseSpinEdit() {
			return new EditorClassInfo("BaseSpinEdit", typeof(BaseSpinEdit), typeof(RepositoryItemBaseSpinEdit), typeof(BaseSpinEditViewInfo), new ButtonEditPainter(), false, EditImageIndexes.SpinEdit, typeof(DevExpress.Accessibility.BaseAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoSpinEdit() {
			return new EditorClassInfo("SpinEdit", typeof(SpinEdit), typeof(RepositoryItemSpinEdit), typeof(BaseSpinEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.SpinEdit, typeof(DevExpress.Accessibility.BaseSpinEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoTimeSpanEdit() {
			return new EditorClassInfo("TimeSpanEdit", typeof(TimeSpanEdit), typeof(RepositoryItemTimeSpanEdit), typeof(TimeSpanEditViewInfo), new TimeSpanEditPainter(), true, EditImageIndexes.TextEdit, typeof(DevExpress.Accessibility.TimeSpanEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoTimeEdit() {
			return new EditorClassInfo("TimeEdit", typeof(TimeEdit), typeof(RepositoryItemTimeEdit), typeof(TimeEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.TimeEdit, typeof(DevExpress.Accessibility.BaseSpinEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoPopupBaseEdit() {
			return new EditorClassInfo("PopupBaseEdit", typeof(PopupBaseEdit), typeof(RepositoryItemPopupBase), typeof(PopupBaseEditViewInfo), new ButtonEditPainter(), false, EditImageIndexes.TextEdit, typeof(DevExpress.Accessibility.BaseAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoComboBoxEdit() {
			return new EditorClassInfo("ComboBoxEdit", typeof(DevExpress.XtraEditors.ComboBoxEdit), typeof(RepositoryItemComboBox), typeof(ComboBoxViewInfo), new ButtonEditPainter(), true, EditImageIndexes.ComboBoxEdit, typeof(DevExpress.Accessibility.ComboBoxEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoImageComboBoxEdit() {
			return new EditorClassInfo("ImageComboBoxEdit", typeof(DevExpress.XtraEditors.ImageComboBoxEdit), typeof(RepositoryItemImageComboBox), typeof(ImageComboBoxEditViewInfo), new ImageComboBoxEditPainter(), true, EditImageIndexes.ImageComboBoxEdit, typeof(DevExpress.Accessibility.ComboBoxEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoBreadCrumbEdit() {
			return new EditorClassInfo("BreadCrumbEdit", typeof(BreadCrumbEdit), typeof(RepositoryItemBreadCrumbEdit), typeof(BreadCrumbEditViewInfo), new BreadCrumbEditPainter(), true, null, typeof(DevExpress.Accessibility.ComboBoxEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoTokenEdit() {
			return new EditorClassInfo("TokenEdit", typeof(TokenEdit), typeof(RepositoryItemTokenEdit), typeof(TokenEditViewInfo), new TokenEditPainter(), true, null, typeof(DevExpress.Accessibility.TokenEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoMRUEdit() {
			return new EditorClassInfo("MRUEdit", typeof(DevExpress.XtraEditors.MRUEdit), typeof(RepositoryItemMRUEdit), typeof(MRUEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.MRUEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoCheckEdit() {
			return new EditorClassInfo("CheckEdit", typeof(CheckEdit), typeof(RepositoryItemCheckEdit), typeof(CheckEditViewInfo), new CheckEditPainter(), true, EditImageIndexes.CheckEdit, typeof(DevExpress.Accessibility.CheckEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoToggleSwitch() {
			return new EditorClassInfo("ToggleSwitch", typeof(ToggleSwitch), typeof(RepositoryItemToggleSwitch), typeof(ToggleSwitchViewInfo), new ToggleSwitchPainter(), true, EditImageIndexes.ToggleSwitch, typeof(DevExpress.Accessibility.ToggleSwitchAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoPictureEdit() {
			return new EditorClassInfo("PictureEdit", typeof(PictureEdit), typeof(RepositoryItemPictureEdit), typeof(PictureEditViewInfo), new PictureEditPainter(), true, EditImageIndexes.PictureEdit, typeof(DevExpress.Accessibility.BaseEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoMemoEdit() {
			return new EditorClassInfo("MemoEdit", typeof(MemoEdit), typeof(RepositoryItemMemoEdit), typeof(MemoEditViewInfo), new MemoEditPainter(), true, EditImageIndexes.MemoEdit, typeof(DevExpress.Accessibility.MemoEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoRadioGroup() {
			return new EditorClassInfo("RadioGroup", typeof(RadioGroup), typeof(RepositoryItemRadioGroup), typeof(RadioGroupViewInfo), new RadioGroupPainter(), true, EditImageIndexes.RadioGroup, typeof(DevExpress.Accessibility.BaseAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoHyperLinkEdit() {
			return new EditorClassInfo("HyperLinkEdit", typeof(HyperLinkEdit), typeof(RepositoryItemHyperLinkEdit), typeof(HyperLinkEditViewInfo), new HyperLinkEditPainter(), true, EditImageIndexes.HyperLinkEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoPopupContainerEdit() {
			return new EditorClassInfo("PopupContainerEdit", typeof(PopupContainerEdit), typeof(RepositoryItemPopupContainerEdit), typeof(PopupContainerEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.PopupContainerEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoBlobBaseEdit() {
			return new EditorClassInfo("BlobBaseEdit", typeof(BlobBaseEdit), typeof(RepositoryItemBlobBaseEdit), typeof(BlobBaseEditViewInfo), new BlobBaseEditPainter(), false);
		}
		protected virtual EditorClassInfo CreateClassInfoImageEdit() {
			return new EditorClassInfo("ImageEdit", typeof(ImageEdit), typeof(RepositoryItemImageEdit), typeof(ImageEditViewInfo), new BlobBaseEditPainter(), true, EditImageIndexes.ImageEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoMemoExEdit() {
			return new EditorClassInfo("MemoExEdit", typeof(MemoExEdit), typeof(RepositoryItemMemoExEdit), typeof(MemoExEditViewInfo), new BlobBaseEditPainter(), true, EditImageIndexes.MemoExEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoProgressBarControl() {
			return new EditorClassInfo("ProgressBarControl", typeof(ProgressBarControl), typeof(RepositoryItemProgressBar), typeof(ProgressBarViewInfo), new ProgressBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(DevExpress.Accessibility.ProgressBarAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoMarqueeProgressBarControl() {
			return new EditorClassInfo("MarqueeProgressBarControl", typeof(MarqueeProgressBarControl), typeof(RepositoryItemMarqueeProgressBar), typeof(MarqueeProgressBarViewInfo), new ProgressBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(DevExpress.Accessibility.ProgressBarAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoDateEdit() {
			return new EditorClassInfo("DateEdit", typeof(DateEdit), typeof(RepositoryItemDateEdit), typeof(DateEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.DateEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoCalcEdit() {
			return new EditorClassInfo("CalcEdit", typeof(CalcEdit), typeof(RepositoryItemCalcEdit), typeof(CalcEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.CalcEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoColorEdit() {
			return new EditorClassInfo("ColorEdit", typeof(ColorEdit), typeof(RepositoryItemColorEdit), typeof(ColorEditViewInfo), new ColorEditPainter(), true, EditImageIndexes.ColorEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoLookUpEdit() {
			return new EditorClassInfo("LookUpEdit", typeof(LookUpEdit), typeof(RepositoryItemLookUpEdit), typeof(LookUpEditViewInfo), new ButtonEditPainter(), true, EditImageIndexes.LookUpEdit, typeof(DevExpress.Accessibility.PopupEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoTrackBarControl() {
			return new EditorClassInfo("TrackBarControl", typeof(TrackBarControl), typeof(RepositoryItemTrackBar), typeof(TrackBarViewInfo), new TrackBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(DevExpress.Accessibility.TrackBarAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoRangeTrackBarControl() {
			return new EditorClassInfo("RangeTrackBarControl", typeof(RangeTrackBarControl), typeof(RepositoryItemRangeTrackBar), typeof(RangeTrackBarViewInfo), new RangeTrackBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(DevExpress.Accessibility.RangeTrackBarAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoFontEdit() {
			return new EditorClassInfo("FontEdit", typeof(FontEdit), typeof(RepositoryItemFontEdit), typeof(ComboBoxViewInfo), new ButtonEditPainter(), true, 0, typeof(DevExpress.Accessibility.ComboBoxEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoZoomTrackBarControl() {
			return new EditorClassInfo("ZoomTrackBarControl", typeof(ZoomTrackBarControl), typeof(RepositoryItemZoomTrackBar), typeof(ZoomTrackBarViewInfo), new ZoomTrackBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(DevExpress.Accessibility.ZoomTrackBarAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoCheckedComboBoxEdit() {
			return new EditorClassInfo("CheckedComboBoxEdit", typeof(CheckedComboBoxEdit), typeof(RepositoryItemCheckedComboBoxEdit), typeof(DevExpress.XtraEditors.ViewInfo.PopupContainerEditViewInfo), new ButtonEditPainter(), true, null, typeof(DevExpress.Accessibility.ComboBoxEditAccessible));
		}
		protected virtual EditorClassInfo CreateClassInfoRatingControl() { 
			return new EditorClassInfo("RatingControl", typeof(RatingControl), typeof(RepositoryItemRatingControl), typeof(RatingControlViewInfo), new RatingControlPainter(), true, EditImageIndexes.TextEdit, typeof(DevExpress.Accessibility.RatingControlAccessible)); 
		}
		protected virtual void RegisterRichTextEdit() {
			DevExpress.Xpo.Helpers.XPTypeActivator.AuxRegistrationInvoker(
				AssemblyInfo.SRAssemblyRichEdit + ", Version=" + AssemblyInfo.Version,
				"DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit",
				"Register");
		}
		protected virtual void RegisterColorPickEdit() {
			RepositoryItemColorPickEdit.RegisterRepositoryItemColorPickEdit();
		}
	}
	public class EditorRegistrationInfo {
		int  assembliesCountWhenRegisterUserItemsWasFired = -1;
		protected EditorClassInfoCollection fEditors;
		static object defaultLocker = new object();
		static EditorRegistrationInfo fDefault;
		public static EditorRegistrationInfo Default {
			get {
				if(fDefault == null) {
					lock(defaultLocker) {
						if(fDefault == null) {
							fDefault = new EditorRegistrationInfo();
						}
					}
				}
				return fDefault;
			}
		}
		public EditorRegistrationInfo() {
			this.fEditors = new EditorClassInfoDefaultCollection();
		}
		public virtual RepositoryItem CreateRepositoryItem(string editName) {
			EditorClassInfo classInfo = Editors[editName];
			if(classInfo != null) {
				return classInfo.CreateRepositoryItem();
			}
			return null;
		}
		protected virtual void OnRegisterType(Type type) {
			object[] attrs = type.GetCustomAttributes(typeof(UserRepositoryItem), false);
			if(attrs == null || attrs.Length != 1) return;
			UserRepositoryItem attr = attrs[0] as UserRepositoryItem;
			if(attr != null && attr.RegistratorMethodName != "") {
				MethodInfo mi = type.GetMethod(attr.RegistratorMethodName, BindingFlags.Public | BindingFlags.Static);
				if(mi != null) mi.Invoke(null, null);
			}
		}
		public virtual void RegisterUserItems(IDesignerHost designerHost) {
			int newAssembliesCountWhenRegisterUserItemsWasFired = AppDomain.CurrentDomain.GetAssemblies().Length;
			if(this.assembliesCountWhenRegisterUserItemsWasFired > 0 && assembliesCountWhenRegisterUserItemsWasFired == newAssembliesCountWhenRegisterUserItemsWasFired) return;
			this.assembliesCountWhenRegisterUserItemsWasFired = newAssembliesCountWhenRegisterUserItemsWasFired;
			new DevExpress.Utils.Registrator.RegistratorHelper(new DevExpress.Utils.Registrator.CheckTypeHandler(OnRegisterType), typeof(RepositoryItem), designerHost);
		}
		public virtual EditorClassInfoCollection Editors { get { return fEditors; } }
	}
}

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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public interface IEditorsRepository {
		RepositoryItemCollection Items { get; }
	}
	public class RepositoryItemChangedEventArgs : EventArgs {
		RepositoryItem item;
		public RepositoryItemChangedEventArgs(RepositoryItem item) {
			this.item = item;
		}
		public RepositoryItem Item { get { return item; } }
	}
	public delegate void RepositoryItemChangedEventHandler(object sender, RepositoryItemChangedEventArgs e);
	[ListBindable(false)]
	public class RepositoryItemCollection : CollectionBase {
		EditorsRepositoryBase owner;
		public RepositoryItemCollection(EditorsRepositoryBase owner) {
			this.owner = owner;
		}
		protected EditorsRepositoryBase Owner { get { return owner; } }
		internal bool AllowDisposeOnRemove = true;
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RepositoryItemCollectionItem")]
#endif
		public virtual RepositoryItem this[int index] { get { return List[index] as RepositoryItem; } }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RepositoryItemCollectionItem")]
#endif
		public virtual RepositoryItem this[string name] {
			get {
				foreach(RepositoryItem ri in List) {
					if(ri.Name == name) return ri;
				}
				return null;
			}
		}
		public virtual RepositoryItem Add(string editorName) {
			RepositoryItem item = DevExpress.XtraEditors.Registrator.EditorRegistrationInfo.Default.CreateRepositoryItem(editorName);
			if(item == null) throw new WarningException(string.Format("Wrong repositoryItemName {0}", editorName));
			Add(item);
			return item;
		}
		public virtual int Add(RepositoryItem item) {
			if(item == null) return -1;
			if(Contains(item)) return IndexOf(item);
			return List.Add(item);
		}
		public virtual void AddRange(RepositoryItem[] items) {
			foreach(RepositoryItem item in items) { Add(item); }
		}
		public virtual void Remove(RepositoryItem item) {
			if(!Contains(item)) return;
			List.Remove(item);
		}
		public virtual int IndexOf(RepositoryItem item) { return List.IndexOf(item); }
		public virtual bool Contains(RepositoryItem item) { return List.Contains(item); }
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) {
				RemoveAt(n);
			}
		}
		protected override void OnInsertComplete(int index, object val) {
			RepositoryItem item = val as RepositoryItem;
			item.RefreshRequired += new EventHandler(OnItem_RefreshRequired);
			item.PropertiesChanged += new EventHandler(OnItem_PropertiesChanged);
			item.Disposed += new EventHandler(OnItem_Disposed);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object val) {
			RepositoryItem item = val as RepositoryItem;
			item.PropertiesChanged -= new EventHandler(OnItem_PropertiesChanged);
			item.Disposed -= new EventHandler(OnItem_Disposed);
			item.RefreshRequired -= new EventHandler(OnItem_RefreshRequired);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
			if(AllowDisposeOnRemove) item.Dispose();
		}
		protected virtual void OnItem_Disposed(object sender, EventArgs e) {
			Remove(sender as RepositoryItem);
		}
		protected virtual void OnItem_RefreshRequired(object sender, EventArgs e) {
			Owner.OnItems_RefreshRequired(this, new RepositoryItemChangedEventArgs(sender as RepositoryItem));
		}
		protected virtual void OnItem_PropertiesChanged(object sender, EventArgs e) {
			Owner.OnItems_PropertiesChanged(this, new RepositoryItemChangedEventArgs(sender as RepositoryItem));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			Owner.OnItems_CollectionChanged(this, e);
		}
	}
	[ToolboxItem(false)]
	public class EditorsRepositoryBase : Component, IEditorsRepository {
		RepositoryItemCollection items;
		public event RepositoryItemChangedEventHandler PropertiesChanged;
		internal event RepositoryItemChangedEventHandler RefreshRequired;
		public event CollectionChangeEventHandler CollectionChanged;
		public EditorsRepositoryBase() {
			this.items = new RepositoryItemCollection(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Items.Clear();
			}
			base.Dispose(disposing);
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorsRepositoryBaseItems")
#else
	Description("")
#endif
]
		public virtual RepositoryItemCollection Items { get { return items; } }
		protected internal virtual void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
		protected internal virtual void OnItems_RefreshRequired(object sender, RepositoryItemChangedEventArgs e) {
			if(RefreshRequired != null) {
				RefreshRequired(this, e);
			}
		}
		protected internal virtual void OnItems_PropertiesChanged(object sender, RepositoryItemChangedEventArgs e) {
			if(PropertiesChanged != null) {
				PropertiesChanged(this, e);
			}
		}
	}
	[ToolboxItem(false)]
	public partial class DefaultEditorsRepository : EditorsRepositoryBase {
		protected enum DefaultEditor { Text, Date, Check, Picture, Enum }
		Hashtable editorsHash;
		public DefaultEditorsRepository() {
			this.editorsHash = new Hashtable();
		}
		public virtual RepositoryItem GetRepositoryItem(Type type, Data.Utils.AnnotationAttributes annotationAttributes) {
			DefaultAddEnumeratorIntegerValues = IsEnum(type) ? Data.Utils.AnnotationAttributes.CheckAddEnumeratorIntegerValues(annotationAttributes) : null;
			try {
				return (annotationAttributes != null) && annotationAttributes.Any() ?
					GetRepositoryItem(type, annotationAttributes, annotationAttributes.GetActualDataType()) :
					GetRepositoryItem(type);
			}
			finally { DefaultAddEnumeratorIntegerValues = null; }
		}
		public virtual RepositoryItem GetRepositoryItemForEditing(RepositoryItem displayItem, Type type, Data.Utils.AnnotationAttributes annotationAttributes) {
			return (annotationAttributes != null) && annotationAttributes.Any() ?
				GetRepositoryItemForEditing(displayItem, type, annotationAttributes, annotationAttributes.GetActualDataType()) :
				displayItem;
		}
		public virtual RepositoryItem GetRepositoryItemForEditing(RepositoryItem displayItem, Type type, Data.Utils.AnnotationAttributes annotationAttributes, System.ComponentModel.DataAnnotations.DataType? dataType) {
			object annotationKey = annotationAttributes.Key;
			if(object.Equals(displayItem, EditorsHash[annotationKey])) {
				Type valueType = type;
				if(dataType.HasValue) {
					if(dataType.Value == System.ComponentModel.DataAnnotations.DataType.Custom)
						valueType = annotationAttributes.EnumType;
					RepositoryItem editItem = EditorsHash[GetEditKey(annotationKey)] as RepositoryItem;
					if(editItem == null) {
						switch(dataType.Value) {
							case System.ComponentModel.DataAnnotations.DataType.Url:
							case System.ComponentModel.DataAnnotations.DataType.EmailAddress:
								editItem = GetRepositoryItem(DefaultEditor.Text, null, GetEditKey(annotationKey));
								break;
						}
						if(editItem != null)
							editItem.AnnotationAttributes = annotationAttributes;
					}
					return editItem ?? displayItem;
				}
			}
			return displayItem;
		}
		static string GetEditKey(object annotationKey) {
			return "#EditKey#" + annotationKey.ToString();
		}
		public bool SupportEditMaskFormPrimitiveTypes { get; set; }
		public virtual RepositoryItem GetRepositoryItem(Type type, Data.Utils.AnnotationAttributes annotationAttributes, System.ComponentModel.DataAnnotations.DataType? dataType) {
			RepositoryItem item = null;
			object annotationKey = annotationAttributes.Key;
			if(typeof(DevExpress.Utils.Filtering.Internal.IValueViewModel).IsAssignableFrom(type))
				item = GetRepositoryItemForFiltering(type, annotationAttributes, dataType);
			if(dataType.HasValue && item == null) {
				if(dataType.Value != System.ComponentModel.DataAnnotations.DataType.Custom)
					item = GetRepositoryItem(dataType.Value, type, annotationKey);
				else
					item = GetRepositoryItem(type, annotationAttributes.EnumType, annotationKey);
			}
			if(item == null) {
				if(type == typeof(DateTime) || type == typeof(DateTime?))
					item = GetRepositoryItem(DefaultEditor.Date, type, annotationKey);
				if(type == typeof(TimeSpan) || type == typeof(TimeSpan?))
					item = GetRepositoryItem(DefaultEditor.Date, type, annotationKey);
				if(type == typeof(Boolean) || type == typeof(Boolean?))
					item = GetRepositoryItem(DefaultEditor.Check, null, annotationKey);
				if(type == typeof(Byte[]) || type == typeof(System.Data.Linq.Binary) || type == typeof(System.Drawing.Image))
					item = GetRepositoryItem(DefaultEditor.Picture, null, annotationKey);
				if(SupportEditMaskFormPrimitiveTypes && CanUseMaskForType(annotationAttributes)) {
					Type defaultNumericType;
					if(IsNumeric(type, out defaultNumericType)) {
						item = GetRepositoryItem(DefaultEditor.Text, defaultNumericType, annotationKey);
						RepositoryItemTextEdit textEditItem = item as RepositoryItemTextEdit;
						if(textEditItem != null)
							InitializeDefaultEditMaskForNumericType(textEditItem, defaultNumericType);
					}
					if(IsChar(type)) {
						item = GetRepositoryItem(DefaultEditor.Text, typeof(char), annotationKey);
						RepositoryItemTextEdit textEditItem = item as RepositoryItemTextEdit;
						if(textEditItem != null)
							InitializeDefaultEditMaskForCharacterType(textEditItem);
					}
				}
				if(IsEnum(type))
					item = GetRepositoryItem(DefaultEditor.Enum, type, annotationKey);
			}
			if(item == null)
				item = GetRepositoryItem(DefaultEditor.Text, null, annotationKey);
			item.AnnotationAttributes = annotationAttributes;
			InitializeNullValueRelatedProperties(item, annotationAttributes);
			InitializeDisplayFormatFromDataFormatString(item, type, annotationAttributes);
			return item;
		}
		public virtual RepositoryItem GetRepositoryItem(Type type) {
			RepositoryItem item = null;
			if(type == typeof(DateTime) || type == typeof(DateTime?))
				item = GetRepositoryItem(DefaultEditor.Date);
			if(type == typeof(Boolean) || type == typeof(Boolean?))
				item = GetRepositoryItem(DefaultEditor.Check);
			if(type == typeof(Byte[]) || type == typeof(System.Data.Linq.Binary) || type == typeof(System.Drawing.Image))
				item = GetRepositoryItem(DefaultEditor.Picture);
			if(SupportEditMaskFormPrimitiveTypes) {
				Type defaultNumericType;
				if(IsNumeric(type, out defaultNumericType)) {
					item = GetRepositoryItem(DefaultEditor.Text, defaultNumericType);
					RepositoryItemTextEdit textEdit = item as RepositoryItemTextEdit;
					if(textEdit != null)
						InitializeDefaultEditMaskForNumericType(textEdit, defaultNumericType);
				}
				if(IsChar(type)) {
					item = GetRepositoryItem(DefaultEditor.Text, typeof(char));
					RepositoryItemTextEdit textEdit = item as RepositoryItemTextEdit;
					if(textEdit != null)
						InitializeDefaultEditMaskForCharacterType(textEdit);
				}
			}
			if(IsEnum(type))
				item = GetRepositoryItem(DefaultEditor.Enum, type);
			if(item == null)
				item = GetRepositoryItem(DefaultEditor.Text);
			return item;
		}
		internal static bool CanUseMaskForType(Data.Utils.AnnotationAttributes attributes) {
			return !attributes.HasDisplayFormatAttribute || string.IsNullOrEmpty(attributes.DataFormatString);
		}
		static bool IsEnum(Type type) {
			return EnumDisplayTextHelper.IsEnum(type);
		}
		internal static bool IsNumeric(Type type, out Type defaultType) {
			defaultType = null;
			if(IsFloatingPoint(type))
				defaultType = typeof(float);
			if(IsInteger(type))
				defaultType = typeof(int);
			if(IsDecimal(type))
				defaultType = typeof(decimal);
			return defaultType != null;
		}
		static bool IsFloatingPoint(Type type) {
			return
				type == typeof(Single) || type == typeof(Single?) ||
				type == typeof(Double) || type == typeof(Double);
		}
		static bool IsInteger(Type type) {
			return
				type == typeof(Byte) || type == typeof(Byte?) ||
				type == typeof(UInt16) || type == typeof(UInt16?) ||
				type == typeof(UInt32) || type == typeof(UInt32?) ||
				type == typeof(UInt64) || type == typeof(UInt64?) ||
				type == typeof(SByte) || type == typeof(SByte?) ||
				type == typeof(Int16) || type == typeof(Int16?) ||
				type == typeof(Int32) || type == typeof(Int32?) ||
				type == typeof(Int64) || type == typeof(Int64?);
		}
		static bool IsDecimal(Type type) {
			return type == typeof(decimal) || type == typeof(decimal?);
		}
		static bool IsChar(Type type) {
			return type == typeof(char) || type == typeof(char?);
		}
		static bool IsDateTime(Type type) {
			return
				type == typeof(DateTime) || type == typeof(DateTime?) ||
				type == typeof(TimeSpan) || type == typeof(TimeSpan?);
		}
		protected virtual Hashtable EditorsHash {
			get { return editorsHash; }
		}
		protected virtual RepositoryItem GetRepositoryItem(DefaultEditor type) {
			return GetRepositoryItem(type, null, null);
		}
		protected virtual RepositoryItem GetRepositoryItem(DefaultEditor type, Type valueType) {
			return GetRepositoryItem(type, valueType, null);
		}
		protected virtual RepositoryItem GetRepositoryItem(DefaultEditor type, Type valueType, object annotationKey) {
			RepositoryItem item = EditorsHash[GetHashValue(type, valueType, annotationKey)] as RepositoryItem;
			if(item == null) {
				switch(type) {
					case DefaultEditor.Check:
						item = CreateDefaultCheckEdit(); break;
					case DefaultEditor.Date:
						if(valueType != null && (valueType == typeof(TimeSpan) || valueType == typeof(TimeSpan?)))
							item = CreateDefaultTimeSpanEdit();
						else
							item = CreateDefaultDateEdit();
						break;
					case DefaultEditor.Picture:
						item = CreateDefaultPictureEdit(); break;
					case DefaultEditor.Enum:
						item = CreateDefaultEnumEdit(valueType); break;
					default:
						item = CreateDefaultTextEdit(); break;
				}
				Items.Add(item);
				EditorsHash[GetHashValue(type, valueType, annotationKey)] = item;
			}
			return item;
		}
		protected virtual RepositoryItem GetRepositoryItem(Type valueType, Type customType, object annotationKey) {
			RepositoryItem item = EditorsHash[annotationKey] as RepositoryItem;
			if(customType != null && item == null) {
				if(customType.IsEnum) {
					item = CreateDefaultEnumEdit(customType, customType != valueType);
				}
				if(item != null) {
					Items.Add(item);
					EditorsHash[annotationKey] = item;
				}
			}
			return item;
		}
		protected virtual RepositoryItem GetRepositoryItem(System.ComponentModel.DataAnnotations.DataType type, Type valueType, object annotationKey) {
			RepositoryItem item = EditorsHash[annotationKey] as RepositoryItem;
			if(item == null) {
				switch(type) {
					case System.ComponentModel.DataAnnotations.DataType.Date:
						item = CreateDefaultDateEdit();
						break;
					case System.ComponentModel.DataAnnotations.DataType.DateTime:
						item = CreateDefaultDateEdit(); break;
					case System.ComponentModel.DataAnnotations.DataType.Duration:
						item = CreateDefaultTimeSpanEdit();
						break;
					case System.ComponentModel.DataAnnotations.DataType.Time:
						item = CreateDefaultTimeEdit();
						if(DevExpress.XtraEditors.WindowsFormsSettings.TouchUIMode == LookAndFeel.TouchUIMode.True)
							((RepositoryItemTimeEdit)item).TimeEditStyle = TimeEditStyle.TouchUI;
						break;
					case System.ComponentModel.DataAnnotations.DataType.Text:
						item = CreateDefaultTextEdit(); break;
					case System.ComponentModel.DataAnnotations.DataType.MultilineText:
						item = CreateDefaultMultiLineTextEdit(); break;
					case System.ComponentModel.DataAnnotations.DataType.Password:
						item = CreateDefaultTextEdit();
						((RepositoryItemTextEdit)item).UseSystemPasswordChar = true;
						break;
					case System.ComponentModel.DataAnnotations.DataType.Url:
						item = CreateDefaultHyperLinkEdit(); break;
					case System.ComponentModel.DataAnnotations.DataType.PhoneNumber:
						item = CreateDefaultTextEdit();
						InitializeDefaultEditMaskForPhoneNumberType((RepositoryItemTextEdit)item);
						break;
					case System.ComponentModel.DataAnnotations.DataType.EmailAddress:
						item = CreateDefaultHyperLinkEdit();
						break;
					case System.ComponentModel.DataAnnotations.DataType.Currency:
						item = CreateDefaultTextEdit();
						InitializeDefaultEditMaskForCurrencyType((RepositoryItemTextEdit)item);
						break;
					case System.ComponentModel.DataAnnotations.DataType.Html:
						item = CreateDefaultHtmlEdit();
						break;
				}
				Items.Add(item);
				EditorsHash[annotationKey] = item;
			}
			return item;
		}
		void InitializeDefaultEditMaskForPhoneNumberType(RepositoryItemTextEdit textEdit) {
			textEdit.Mask.EditMask = "(999) 000-0000";
			textEdit.Mask.MaskType = Mask.MaskType.Simple;
			textEdit.Mask.UseMaskAsDisplayFormat = true;
		}
		internal static void InitializeDefaultEditMaskForCurrencyType(RepositoryItemTextEdit textEdit) {
			textEdit.Mask.EditMask = "c";
			textEdit.Mask.MaskType = Mask.MaskType.Numeric;
			textEdit.Mask.UseMaskAsDisplayFormat = true;
		}
		internal static void InitializeDefaultEditMaskForNumericType(RepositoryItemTextEdit textEdit, Type type) {
			if(type == typeof(int))
				textEdit.Mask.EditMask = "N0";
			if(type == typeof(float))
				textEdit.Mask.EditMask = "F";
			if(type == typeof(decimal))
				textEdit.Mask.EditMask = "G";
			textEdit.Mask.MaskType = Mask.MaskType.Numeric;
			textEdit.Mask.UseMaskAsDisplayFormat = true;
		}
		void InitializeDefaultEditMaskForCharacterType(RepositoryItemTextEdit textEdit) {
			textEdit.Mask.EditMask = "c";
			textEdit.Mask.MaskType = Mask.MaskType.Simple;
			textEdit.Mask.UseMaskAsDisplayFormat = true;
		}
		internal static void InitializeDisplayFormatFromDataFormatString(RepositoryItem item, Type type, DevExpress.Data.Utils.AnnotationAttributes annotationAttributes) {
			if(annotationAttributes.HasDisplayFormatAttribute) {
				string dataFormatString = annotationAttributes.DataFormatString;
				if(!string.IsNullOrEmpty(dataFormatString)) {
					FormatType formatType;
					if(TestDataFormatType(dataFormatString, type, out formatType)) {
						item.DisplayFormat.FormatType = formatType;
						item.DisplayFormat.FormatString = dataFormatString;
						if(annotationAttributes.ApplyFormatInEditMode) {
							item.EditFormat.FormatType = formatType;
							item.EditFormat.FormatString = dataFormatString;
						}
					}
				}
			}
		}
		internal static void InitializeNullValueRelatedProperties(RepositoryItem item, Data.Utils.AnnotationAttributes annotationAttributes) {
			RepositoryItemTextEdit textEdit = item as RepositoryItemTextEdit;
			if(textEdit != null) {
				if(annotationAttributes.HasDisplayAttribute)
					textEdit.NullValuePrompt = annotationAttributes.Prompt;
				if(annotationAttributes.HasDisplayFormatAttribute)
					textEdit.NullValuePromptShowForEmptyValue = annotationAttributes.ConvertEmptyStringToNull;
				if(annotationAttributes.IsRequired.GetValueOrDefault())
					textEdit.AllowNullInput = DefaultBoolean.False;
			}
			RepositoryItemCheckEdit checkEdit = item as RepositoryItemCheckEdit;
			if(checkEdit != null) {
				if(annotationAttributes.IsRequired.GetValueOrDefault())
					checkEdit.AllowGrayed = false;
			}
		}
		object GetHashValue(DefaultEditor type, Type valueType, object annotationKey) {
			return annotationKey ?? valueType ?? (object)type;
		}
		protected virtual RepositoryItem CreateDefaultTextEdit() {
			return new RepositoryItemTextEdit();
		}
		protected virtual RepositoryItem CreateDefaultMultiLineTextEdit() {
			return new RepositoryItemMemoExEdit();
		}
		protected virtual RepositoryItem CreateDefaultHtmlEdit() {
			return new RepositoryItemTextEdit(); 
		}
		protected virtual RepositoryItem CreateDefaultHyperLinkEdit() {
			return new RepositoryItemHyperLinkEdit();
		}
		protected virtual RepositoryItem CreateDefaultCheckEdit() {
			return new RepositoryItemCheckEdit();
		}
		protected virtual RepositoryItem CreateDefaultDateEdit() {
			return new RepositoryItemDateEdit();
		}
		protected virtual RepositoryItem CreateDefaultTimeEdit() {
			return new RepositoryItemTimeEdit();
		}
		protected virtual RepositoryItem CreateDefaultTimeSpanEdit() {
			return new RepositoryItemTimeSpanEdit();
		}
		bool? DefaultAddEnumeratorIntegerValues;
		protected virtual RepositoryItem CreateDefaultEnumEdit(Type type) {
			return CreateDefaultEnumEdit(type, DefaultAddEnumeratorIntegerValues.GetValueOrDefault());
		}
		protected virtual RepositoryItem CreateDefaultEnumEdit(Type type, bool addEnumeratorIntegerValues) {
			RepositoryItemImageComboBox ret = new RepositoryItemImageComboBox();
			ret.Items.AddEnum(type, addEnumeratorIntegerValues);
			return ret;
		}
		protected virtual RepositoryItem CreateDefaultPictureEdit() {
			RepositoryItemPictureEdit ret = new RepositoryItemPictureEdit();
			ret.SizeMode = PictureSizeMode.Squeeze;
			return ret;
		}
		protected virtual void RemoveHashItem(object val) {
			object key = null;
			foreach(DictionaryEntry entry in EditorsHash) {
				if(entry.Value == val) {
					key = entry.Key;
					break;
				}
			}
			if(key != null) EditorsHash.Remove(key);
		}
		protected internal override void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Remove) {
				if(EditorsHash.ContainsValue(e.Element)) {
					RemoveHashItem(e.Element);
				}
			}
			if(e.Action == CollectionChangeAction.Refresh) EditorsHash.Clear();
			base.OnItems_CollectionChanged(sender, e);
		}
		static bool TestDataFormatType(string dataFormatString, Type columnType, out FormatType formatType) {
			if(columnType == null || columnType == typeof(Byte[]) || columnType == typeof(Image) || columnType == typeof(System.Data.Linq.Binary)) {
				formatType = FormatType.None;
				return false;
			}
			if(columnType == typeof(DateTime) || columnType == typeof(DateTime?))
				return TestDataFormatTypeCore(DateTime.Now, System.Globalization.DateTimeFormatInfo.CurrentInfo, dataFormatString, FormatType.DateTime, out formatType);
			return TestDataFormatTypeCore(1, System.Globalization.NumberFormatInfo.CurrentInfo, dataFormatString, FormatType.Numeric, out formatType);
		}
		static bool TestDataFormatTypeCore(IFormattable testObj, IFormatProvider provider, string formatString, FormatType desired, out FormatType formatType) {
			try { testObj.ToString(formatString, provider); }
			catch {
				formatType = FormatType.None;
				return false;
			}
			formatType = desired;
			return true;
		}
	}
	[DXToolboxItem(DXToolboxItemKind.Regular),
	 Designer("DevExpress.XtraEditors.Design.PersistentRepositoryDesigner, " + AssemblyInfo.SRAssemblyEditorsDesignFull),
	 Description("Stores repository items to be shared between container controls (e.g. GridControl, TreeList, VGridControl, RibbonControl, bars, etc)."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "PersistentRepository")
	]
	public class PersistentRepository : EditorsRepositoryBase {
		Component parentComponent = null;
		public PersistentRepository(Component parentComponent) {
			this.parentComponent = parentComponent;
		}
		public PersistentRepository() {
		}
		public PersistentRepository(IContainer container)
			: this() {
			container.Add(this);
		}
		protected virtual IContainer ComponentContainer {
			get {
				if(parentComponent != null) return parentComponent.Container;
				return Container;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParentComponent(Component parentComponent) {
			this.parentComponent = parentComponent;
		}
		protected internal override void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			RepositoryItem item = e.Element as RepositoryItem;
			if(ComponentContainer != null) {
				switch(e.Action) {
					case CollectionChangeAction.Add:
						if(item != null && item.Site == null) {
							ComponentContainer.Add(item);
							AddReference(item);
						}
						break;
					case CollectionChangeAction.Remove:
						ComponentContainer.Remove(item);
						break;
				}
			}
			base.OnItems_CollectionChanged(sender, e);
		}
		void AddReference(RepositoryItem item) {
			try {
				if(item == null || item.Site == null) return;
				if(item.GetType().Assembly.Equals(typeof(BaseEdit).Assembly)) return;
				ITypeResolutionService service = item.Site.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
				if(service != null) service.ReferenceAssembly(item.GetType().Assembly.GetName());
			}
			catch {
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PersistentRepositoryItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraEditors.Design.RepositoryItemsEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))
#if DXWhidbey
, DevExpress.Utils.Design.InheritableCollection
#endif
]
		public override RepositoryItemCollection Items { get { return base.Items; } }
	}
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class InternalPersistentRepository : DevExpress.XtraEditors.Repository.PersistentRepository {
		public InternalPersistentRepository() {
			Items.AllowDisposeOnRemove = false;
		}
		protected internal override void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
		}
	}
}

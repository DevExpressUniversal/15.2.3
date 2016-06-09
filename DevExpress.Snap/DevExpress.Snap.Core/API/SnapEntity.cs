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

namespace DevExpress.Snap.Core.API {
	using System;
	using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
	public interface SnapEntity : IDisposable {
		void BeginUpdate();
		void EndUpdate();
		bool Active { get; }
		SnapDocument Document { get; }
		SnapSubDocument SubDocument { get; }
		ApiField Field { get; }
	}
	public interface SnapSingleListItemEntity : SnapEntity {
		string DataFieldName { get; set; }			  
		string EmptyFieldDataAlias { get; set; }		
		bool EnableEmptyFieldDataAlias { get; set; }	
	}
}
namespace DevExpress.Snap.API.Native {
	using System;
	using DevExpress.Snap.Core.API;
	using DevExpress.Snap.Core.Native;
	using DevExpress.XtraRichEdit.API.Native.Implementation;
	using DevExpress.XtraRichEdit.Fields;
	using ApiField = DevExpress.XtraRichEdit.API.Native.Field;
	using ModelField = DevExpress.XtraRichEdit.Model.Field;
	using DevExpress.Snap.Core.Fields;
	public abstract class NativeSnapEntityBase : SnapEntity {
		readonly SnapNativeDocument document;
		readonly SnapSubDocument subDocument;
		InstructionController controller;
		ApiField apiField;
		ModelField rawField;
		CalculatedFieldBase parsedField;
		public static readonly string TextBeforeIfFieldNotBlankSwitch = "b";
		public static readonly string TextAfterIfFieldNotBlankSwitch = "f";
		protected NativeSnapEntityBase(SnapNativeDocument document, ApiField field)
			: this(document, document, field) { }
		protected NativeSnapEntityBase(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) {
			this.subDocument = subDocument;
			this.document = document;
			this.apiField = field;
			this.rawField = ((NativeField)field).Field;
			Init();
		}
		protected virtual string LockExceptionText { get { return DevExpress.Snap.Localization.SnapLocalizer.GetString(Localization.SnapStringId.SnapListLockException); } }
		protected virtual void Init() {
			this.parsedField = new SnapFieldCalculatorService().ParseField(((NativeSubDocument)subDocument).PieceTable, RawField);
		}
		public void EnsureUpdateBegan() {
			if(!Active)
				throw new InvalidOperationException(LockExceptionText);
		}
		public InstructionController Controller {
			get {
				if(!Active)
					throw new InvalidOperationException(LockExceptionText);
				return controller;
			}
		}
		protected virtual void Reset() {
			Init();
		}
		protected ModelField RawField { get { return rawField; } }
		protected T GetParsedField<T>() where T : CalculatedFieldBase { return parsedField as T; }
		#region SnapEntity Members
		public virtual void BeginUpdate() {
			if(Active)
				throw new InvalidOperationException(DevExpress.Snap.Localization.SnapLocalizer.GetString(Localization.SnapStringId.SnapListSecondBeginUpdateException));
			this.document.SetActiveEntity(this);
			controller = new InstructionController(((NativeSubDocument)this.subDocument).PieceTable, parsedField, rawField) { SuppressFieldsUpdateAfterUpdateInstruction = true };
		}
		public virtual void EndUpdate() {
			this.controller.Dispose();
			this.controller = null;
			if (object.ReferenceEquals(this.document, this.subDocument))
				this.document.SetActiveEntity(null);
			else
				((SnapNativeSubDocument)this.subDocument).SetActiveEntity(null);
			Reset();
		}
		public bool Active { get { return controller != null; } }
		public SnapNativeDocument Document { get { return document; } }
		SnapDocument SnapEntity.Document { get { return this.Document; } }
		public SnapSubDocument SubDocument { get { return subDocument; } }
		public ApiField Field { get { return apiField; } }
		#endregion
		#region IDisposable Members
		public virtual void Dispose() {
			EndUpdate();
		}
		#endregion
	}
	#region NativeSnapSingleListItemEntity
	public abstract class NativeSnapSingleListItemEntity : NativeSnapEntityBase {
		string dataFieldName;
		string emptyFieldDataAlias;
		bool enableEmptyFieldDataAlias;
		protected NativeSnapSingleListItemEntity(SnapNativeDocument document, ApiField field) : base(document, field) { }
		protected NativeSnapSingleListItemEntity(SnapSubDocument subDocument, SnapNativeDocument document, ApiField field) : base(subDocument, document, field) { }
		protected override void Init() {
			base.Init();
			SNMergeFieldSupportsEmptyFieldDataAlias parsedField = GetParsedField<SNMergeFieldSupportsEmptyFieldDataAlias>();
			this.dataFieldName = parsedField.DataFieldName ?? String.Empty;
			this.emptyFieldDataAlias = parsedField.EmptyFieldDataAlias;
			this.enableEmptyFieldDataAlias = parsedField.EnableEmptyFieldDataAlias;
		}
		public string DataFieldName {
			get {
				return dataFieldName;
			}
			set {
				EnsureUpdateBegan();
				if (String.Equals(dataFieldName, value))
					return;
				Controller.SetArgument(0, value);
				dataFieldName = value;
			}
		}
		public string EmptyFieldDataAlias {
			get {
				return emptyFieldDataAlias;
			}
			set {
				EnsureUpdateBegan();
				if (String.Equals(emptyFieldDataAlias, value))
					return;
				Controller.SetSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EmptyFieldDataAliasSwitch, value);
				emptyFieldDataAlias = value;
			}
		}
		public bool EnableEmptyFieldDataAlias {
			get {
				return enableEmptyFieldDataAlias;
			}
			set {
				EnsureUpdateBegan();
				if (enableEmptyFieldDataAlias == value)
					return;
				if (value)
					Controller.SetSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EnableEmptyFieldDataAliasSwitch);
				else
					Controller.RemoveSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EnableEmptyFieldDataAliasSwitch);
				enableEmptyFieldDataAlias = value;
			}
		}
	}
	#endregion
}

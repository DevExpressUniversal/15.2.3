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
using System.Collections.Concurrent;
using System.Collections.Generic;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Editors.Helpers {
	public enum EditorOptimizationMode {
		Disabled,
		Simple,
		Extended,
	}
	public delegate IBaseEdit CreateEditorMethod();
	public delegate IBaseEdit CreateEditorMethod2(bool isOptimized = false);
	public delegate BaseEditSettings CreateEditorSettingsMethod();
	public class EditorSettingsProvider {
		public static EditorSettingsProvider Default { get; private set; }
		static EditorSettingsProvider CreateDefaultSettingsProvider() {
			return new EditorSettingsProvider();
		}
		static EditorSettingsProvider() {
			Default = CreateDefaultSettingsProvider();
			Default.RegisterEditors();
		}
		ConcurrentDictionary<Type, CreateEditorMethod2> createEditor;
		ConcurrentDictionary<Type, CreateEditorSettingsMethod> createEditorSettings;
		void RegisterEditors() {
			this.createEditor = new ConcurrentDictionary<Type, CreateEditorMethod2>();
			this.createEditorSettings = new ConcurrentDictionary<Type, CreateEditorSettingsMethod>();
			RegisterEditor(typeof(PopupColorEdit), typeof(PopupColorEditSettings), optimized => new PopupColorEdit(), () => new PopupColorEditSettings());
			RegisterEditor(typeof(ColorEdit), typeof(ColorEditSettings), optimized => new ColorEdit(), () => new ColorEditSettings());
			RegisterEditor(typeof(ListBoxEdit), typeof(ListBoxEditSettings), optimized => new ListBoxEdit(), () => new ListBoxEditSettings());
			RegisterEditor(typeof(MemoEdit), typeof(MemoEditSettings), optimized => new MemoEdit(), () => new MemoEditSettings());
			RegisterEditor(typeof(PopupBaseEdit), typeof(PopupBaseEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new PopupBaseEdit(), () => new PopupBaseEditSettings());
			RegisterEditor(typeof(PasswordBoxEdit), typeof(PasswordBoxEditSettings), optimized => new PasswordBoxEdit(), () => new PasswordBoxEditSettings());
			RegisterEditor(typeof(TrackBarEdit), typeof(TrackBarEditSettings), optimized => new TrackBarEdit(), () => new TrackBarEditSettings());
			RegisterEditor(typeof(ImageEdit), typeof(ImageEditSettings), optimized => new ImageEdit(), () => new ImageEditSettings());
			RegisterEditor(typeof(PopupImageEdit), typeof(PopupImageEditSettings), optimized => new PopupImageEdit(), () => new PopupImageEditSettings());
			RegisterEditor(typeof(ProgressBarEdit), typeof(ProgressBarEditSettings), optimized => new ProgressBarEdit(), () => new ProgressBarEditSettings());
			RegisterEditor(typeof(FontEdit), typeof(FontEditSettings), optimized => new FontEdit(), () => new FontEditSettings());
			RegisterEditor(typeof(CheckEdit), typeof(CheckEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new CheckEdit(), () => new CheckEditSettings());
			RegisterEditor(typeof(ButtonEdit), typeof(ButtonEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new ButtonEdit(), () => new ButtonEditSettings());
			RegisterEditor(typeof(PopupCalcEdit), typeof(CalcEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new PopupCalcEdit(), () => new CalcEditSettings());
			RegisterEditor(typeof(TextEdit), typeof(TextEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new TextEdit(), () => new TextEditSettings());
			RegisterEditor(typeof(ComboBoxEdit), typeof(ComboBoxEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new ComboBoxEdit(), () => new ComboBoxEditSettings());
			RegisterEditor(typeof(DateEdit), typeof(DateEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new DateEdit(), () => new DateEditSettings());
			RegisterEditor(typeof(SpinEdit), typeof(SpinEditSettings), optimized => optimized ? new InplaceBaseEdit(true) : (IBaseEdit)new SpinEdit(), () => new SpinEditSettings());
			RegisterEditor(typeof(SparklineEdit), typeof(SparklineEditSettings), optimized => new SparklineEdit(), () => new SparklineEditSettings());
			RegisterEditor(typeof(BarCodeEdit), typeof(BarCodeEditSettings), optimized => new BarCodeEdit(), () => new BarCodeEditSettings());
		}
		public virtual void RegisterUserEditor(Type editor, Type editorSettings, CreateEditorMethod createEditorMethod, CreateEditorSettingsMethod createEditorSettingsMethod) {
			RegisterEditor(editor, editorSettings, optimized => createEditorMethod(), createEditorSettingsMethod);
		}
		public virtual void RegisterUserEditor2(Type editor, Type editorSettings, CreateEditorMethod2 createEditorMethod, CreateEditorSettingsMethod createEditorSettingsMethod) {
			RegisterEditor(editor, editorSettings, createEditorMethod, createEditorSettingsMethod);
		}
		protected void RegisterEditor(Type editor, Type editorSettings, CreateEditorMethod2 createEditorMethod, CreateEditorSettingsMethod createEditorSettingsMethod) {
			this.createEditor[editorSettings] = createEditorMethod;
			this.createEditorSettings[editor] = createEditorSettingsMethod;
		}
		public IBaseEdit CreateEditor(Type editorSettings, EditorOptimizationMode optimizationMode = EditorOptimizationMode.Simple) {
			if (!editorSettings.IsSubclassOf(typeof(BaseEditSettings)))
				throw new ArgumentException("Wrong base class supplied", "editor");
			IBaseEdit editor;
			if (CreateEditorCore(editorSettings, optimizationMode, out editor))
				return editor;
			return null;
		}
		bool CreateEditorCore(Type editorSettings, EditorOptimizationMode optimizationMode, out IBaseEdit editor) {
			editor = null;
			if (optimizationMode == EditorOptimizationMode.Extended) {
				editor = new InplaceBaseEdit();
				return true;
			}
			bool isOptimized = optimizationMode == EditorOptimizationMode.Simple;
			CreateEditorMethod2 createEditorMethod = null;
			if (this.createEditor.TryGetValue(editorSettings, out createEditorMethod)) {
				editor = createEditorMethod(isOptimized);
				return true;
			}
			while (editorSettings != null) {
				editorSettings = editorSettings.BaseType;
				if (editorSettings != null && this.createEditor.TryGetValue(editorSettings, out createEditorMethod)) {
					editor = createEditorMethod(isOptimized);
					return true;
				}
			}
			return false;
		}
		public BaseEditSettings CreateEditorSettings(Type editor) {
			if (!editor.IsSubclassOf(typeof(BaseEdit)))
				throw new ArgumentException("Wrong base class supplied", "editor");
			CreateEditorSettingsMethod createEditorSettingsMethod = null;
			if (createEditorSettings.TryGetValue(editor, out createEditorSettingsMethod))
				return createEditorSettingsMethod();
			while (editor != null) {
				editor = editor.BaseType;
				if (editor != null && createEditorSettings.TryGetValue(editor, out createEditorSettingsMethod))
					return createEditorSettingsMethod();
			}
			return null;
		}
#if DEBUGTEST
		public IEnumerable<CreateEditorMethod2> RegisteredEditors {
			get { return createEditor.Values; }
		}
		public IEnumerable<CreateEditorSettingsMethod> RegisteredEditorSettings {
			get { return createEditorSettings.Values; }
		}
		public SortedDictionary<Type, CreateEditorSettingsMethod> RegisteredEditorSettingsSorted {
			get {
				SortedDictionary<Type, CreateEditorSettingsMethod> dict = new SortedDictionary<Type, CreateEditorSettingsMethod>
					(createEditorSettings, new CompareType());
				return dict;			  
			}
		}
		class CompareType : IComparer<Type> {
			public int Compare(Type x, Type y) {
				return x.Name.CompareTo(y.Name);
			}
		}
		public void UnregisterEditor(Type editor, Type editorSettings) {
			((IDictionary<Type, CreateEditorMethod2>)createEditor).Remove(editorSettings);
			((IDictionary<Type, CreateEditorSettingsMethod>)createEditorSettings).Remove(editor);
		}
#endif
		public bool IsCompatible(IBaseEdit editCore, BaseEditSettings editSettings) {
			var editor = editSettings.CreateEditor(false, EmptyDefaultEditorViewInfo.Instance, EditorOptimizationMode.Simple);
			return editor.GetType() == editCore.GetType();
		}
	}
}

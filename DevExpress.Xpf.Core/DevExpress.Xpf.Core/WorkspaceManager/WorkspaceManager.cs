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
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Serialization;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
#if SL
using System.IO.IsolatedStorage;
#else
using System.Windows.Documents;
using System.Windows.Interop;
#endif
namespace DevExpress.Xpf.Core {
	public interface IWorkspace {
		string Name { get; }
		object SerializationData { get; }
	}
	public interface IWorkspaceManager {
		FrameworkElement TargetControl { get; }
		List<IWorkspace> Workspaces { get; }
		TransitionEffect TransitionEffect { get; set; }
		event EventHandler BeforeApplyWorkspace;
		event EventHandler AfterApplyWorkspace;
		void CaptureWorkspace(string name);
		void RemoveWorkspace(string name);
		void RenameWorkspace(string oldName, string newName);
		void ApplyWorkspace(string name);
		bool LoadWorkspace(string name, object path);
		bool SaveWorkspace(string name, object path);
		bool? CloseStreamOnWorkspaceSaving { get; set; }
		bool? CloseStreamOnWorkspaceLoading { get; set; }
	}
	class Workspace : IWorkspace {
		readonly string name;
		readonly object serializationData;
		public Workspace(string name, object serializationData) {
			this.name = name;
			this.serializationData = serializationData;
		}
		public string Name { get { return name; } }
		public object SerializationData { get { return serializationData; } }
	}
	public class WorkspaceManager : IWorkspaceManager {
		#region Fields
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty IsEnabledProperty;
		[IgnoreDependencyPropertiesConsistencyChecker()]
		public static readonly DependencyProperty WorkspaceManagerProperty;
		readonly FrameworkElement targetControl;
		readonly List<IWorkspace> workspaces = new List<IWorkspace>();
		TransitionEffect transitionEffect = TransitionEffect.None;
		Effect saveEffect;
#if !SL
		Effect saveAdornerEffect;
#endif
		#endregion
		static WorkspaceManager() {
			IsEnabledProperty =
				DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(WorkspaceManager),
					new PropertyMetadata(false, (d, e) => OnIsEnabledPropertyChanged((FrameworkElement)d, (bool)e.NewValue)));
			WorkspaceManagerProperty =
				DependencyProperty.RegisterAttached("WorkspaceManager", typeof(IWorkspaceManager), typeof(WorkspaceManager),
					new PropertyMetadata(null));
		}
		protected WorkspaceManager(FrameworkElement targetControl) {
			this.targetControl = targetControl;
			DXSerializer.SetSerializationID(targetControl, SerializationName);
		}
		#region Properties
#if !SL
	[DevExpressXpfCoreLocalizedDescription("WorkspaceManagerTargetControl")]
#endif
		public FrameworkElement TargetControl { get { return targetControl; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("WorkspaceManagerWorkspaces")]
#endif
		public List<IWorkspace> Workspaces { get { return workspaces; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("WorkspaceManagerTransitionEffect")]
#endif
		public TransitionEffect TransitionEffect { get { return transitionEffect; } set { transitionEffect = value; } }
		protected internal virtual string SerializationName {
			get {
				return TargetControl == null ? string.Empty : TargetControl.GetType().Name;
			}
		}
		#endregion
		#region Events
		public event EventHandler BeforeApplyWorkspace;
		public event EventHandler AfterApplyWorkspace;
		#endregion
		public static bool GetIsEnabled(FrameworkElement targetControl) {
			return (targetControl == null) ? false : (bool)targetControl.GetValue(IsEnabledProperty);
		}
		public static void SetIsEnabled(FrameworkElement targetControl, bool enabled) {
			if(targetControl != null)
				targetControl.SetValue(IsEnabledProperty, enabled);
		}
		public static IWorkspaceManager GetWorkspaceManager(FrameworkElement targetControl) {
			return (targetControl == null) ? null : (IWorkspaceManager)targetControl.GetValue(WorkspaceManagerProperty);
		}
		static void SetWorkspaceManager(FrameworkElement targetControl, IWorkspaceManager workspaceManager) {
			if(targetControl != null)
				targetControl.SetValue(WorkspaceManagerProperty, workspaceManager);
		}
		static void OnIsEnabledPropertyChanged(FrameworkElement targetControl, bool enabled) {
			if(targetControl != null) {
				IWorkspaceManager workspaceManager = enabled ? new WorkspaceManager(targetControl) : null;
				SetWorkspaceManager(targetControl, workspaceManager);
			}
		}
		public void CaptureWorkspace(string name) {
			if(TargetControl == null || string.IsNullOrEmpty(name))
				return;
			using(MemoryStream stream = new MemoryStream()) {
				DXSerializer.Serialize(TargetControl, stream, SerializationName, null);
				AddWorkspaceCore(name, stream.ToArray());
			}
		}
		public void RemoveWorkspace(string name) {
			IWorkspace workspace = GetWorkspace(name);
			if(workspace != null)
				Workspaces.Remove(workspace);
		}
		public void RenameWorkspace(string oldName, string newName) {
			IWorkspace workspace = GetWorkspace(oldName);
			if(workspace == null || string.IsNullOrEmpty(newName) || oldName == newName)
				return;
			int index = GetWorkspaceIndex(oldName);
			Workspaces[index] = new Workspace(newName, workspace.SerializationData);
		}
		public void ApplyWorkspace(string name) {
			Brush screenshot = null;
			if(TransitionEffect != TransitionEffect.None)
				screenshot = GetScreenshot();
			OnBeforeApplyWorkspace();
			OnBeforeTransitionAnimation(screenshot);
			ApplyWorkspaceCore(GetWorkspace(name));
			TransitionAnimation(screenshot);
		}
		public bool? CloseStreamOnWorkspaceSaving { get; set; }
		public bool? CloseStreamOnWorkspaceLoading { get; set; }
		public bool LoadWorkspace(string name, object path) {
			bool keepOpened;
			return LoadWorkspaceCore(name, GetLoadStream(path, out keepOpened), keepOpened);
		}
		public bool SaveWorkspace(string name, object path) {
			bool keepOpened;
			return SaveWorkspaceCore(name, GetSaveStream(path, out keepOpened), keepOpened);
		}
		protected virtual void AddWorkspaceCore(string name, object serializationData) {
			if(string.IsNullOrEmpty(name) || serializationData == null)
				return;
			IWorkspace workspace = new Workspace(name, serializationData);
			int index = GetWorkspaceIndex(name);
			if(index != -1)
				Workspaces[index] = workspace;
			else
				Workspaces.Add(workspace);
		}
		protected virtual void ApplyWorkspaceCore(IWorkspace workspace) {
			if(workspace == null)
				return;
			Stream stream = GetSerializationStream(workspace);
			if(stream == null)
				return;
			using(stream)
				DXSerializer.Deserialize(TargetControl, stream, SerializationName, null);
		}
		protected virtual bool LoadWorkspaceCore(string name, Stream stream) {
			return LoadWorkspaceCore(name, stream, true);
		}
		protected virtual bool LoadWorkspaceCore(string name, Stream stream, bool keepOpened) {
			if(string.IsNullOrEmpty(name) || stream == null)
				return false;
			try {
				byte[] buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				AddWorkspaceCore(name, buffer);
				return true;
			}
			catch { return false; }
			finally {
				if(CloseStreamOnWorkspaceLoading.GetValueOrDefault(!keepOpened))
					stream.Dispose();
			}
		}
		protected virtual bool SaveWorkspaceCore(string name, Stream stream) {
			return SaveWorkspaceCore(name, stream, true);
		}
		protected virtual bool SaveWorkspaceCore(string name, Stream stream, bool keepOpened) {
			IWorkspace workspace = GetWorkspace(name);
			if(!ValidateWorkspace(workspace) || stream == null)
				return false;
			try {
				byte[] buffer = (byte[])workspace.SerializationData;
				stream.Write(buffer, 0, buffer.Length);
				stream.Flush();
				return true;
			}
			catch { return false; }
			finally {
				if(CloseStreamOnWorkspaceSaving.GetValueOrDefault(!keepOpened))
					stream.Dispose();
			}
		}
		protected virtual bool WorkspaceNameEquals(IWorkspace workspace, string name) {
			return workspace.Name == name;
		}
		protected internal virtual bool ValidateWorkspace(IWorkspace workspace) {
			return workspace != null && !string.IsNullOrEmpty(workspace.Name) && (workspace.SerializationData as byte[]) != null;
		}
		protected internal virtual IWorkspace GetWorkspace(int index) {
			return (TargetControl == null || index < 0 || index >= Workspaces.Count) ? null : Workspaces[index];
		}
		protected internal virtual IWorkspace GetWorkspace(string name) {
			return (TargetControl == null || string.IsNullOrEmpty(name)) ? null :
				Workspaces.Find(workspace => WorkspaceNameEquals(workspace, name));
		}
		protected internal virtual int GetWorkspaceIndex(string name) {
			for(int i = 0; i < Workspaces.Count; i++) {
				if(WorkspaceNameEquals(Workspaces[i], name))
					return i;
			}
			return -1;
		}
		protected virtual Stream GetSerializationStream(IWorkspace workspace) {
			return ValidateWorkspace(workspace) ? new MemoryStream((byte[])workspace.SerializationData) : null;
		}
		protected virtual Stream GetFileStream(string filePath, bool save) {
			try {
				FileMode mode = save ? FileMode.Create : FileMode.Open;
#if SL
				IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
				return (file != null) ? new IsolatedStorageFileStream(filePath, mode, file) : null;
#else
				return File.Open(filePath, mode);
#endif
			}
			catch { return null; }
		}
		protected virtual Stream GetStreamCore(object path, bool save, out bool keepOpened) {
			keepOpened = false;
			if(path is string && !string.IsNullOrEmpty((string)path))
				return GetFileStream((string)path, save);
			if(path is Stream) {
				keepOpened = true;
				return (Stream)path;
			}
			return null;
		}
		protected virtual Stream GetLoadStream(object path) {
			bool keepOpened;
			return GetLoadStream(path, out keepOpened);
		}
		protected virtual Stream GetLoadStream(object path, out bool keepOpened) {
			return GetStreamCore(path, false, out keepOpened);
		}
		protected virtual Stream GetSaveStream(object path) {
			bool keepOpened;
			return GetSaveStream(path, out keepOpened);
		}
		protected virtual Stream GetSaveStream(object path, out bool keepOpened) {
			return GetStreamCore(path, true, out keepOpened);
		}
		protected virtual Brush GetScreenshot() {
#if !SL
			if(!BrowserInteropHelper.IsBrowserHosted)
				RemoveTransitionEffectAnimation();
#endif
			return new ImageBrush() {
				ImageSource = RenderFrameworkElement(TargetControl),
				Stretch = Stretch.None,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top
			};
		}
		protected virtual void SaveTargetControlEffect() {
			saveEffect = TargetControl.Effect;
		}
		protected virtual void RestoreTargetControlEffect() {
			TargetControl.Effect = saveEffect;
		}
		protected virtual void RemoveTransitionEffectAnimation() {
#if !SL
			if(TargetControl.Effect is TransitionEffectBase) {
				TargetControl.Effect.BeginAnimation(TransitionEffectBase.ProgressProperty, null, HandoffBehavior.SnapshotAndReplace);
				RestoreTargetControlEffect();
			}
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TargetControl);
			if(adornerLayer != null && adornerLayer.Effect is TransitionEffectBase) {
				adornerLayer.Effect.BeginAnimation(TransitionEffectBase.ProgressProperty, null, HandoffBehavior.SnapshotAndReplace);
				RestoreTargetControlAdornerEffect();
			}
#endif
		}
		protected virtual TransitionEffectBase GetTransitionEffect(TransitionEffect effect) {
			switch(effect) {
				case TransitionEffect.Dissolve:
					return new DissolveTransitionEffect();
				case TransitionEffect.Fade:
					return new FadeTransitionEffect();
				case TransitionEffect.LineReveal:
					return new LineRevealTransitionEffect();
				case TransitionEffect.RadialBlur:
					return new RadialBlurTransitionEffect();
				case TransitionEffect.Ripple:
					return new RippleTransitionEffect();
				case TransitionEffect.Wave:
					return new WaveTransitionEffect();
				case TransitionEffect.None:
				default:
					return null;
			}
		}
		void OnBeforeTransitionAnimation(Brush screenshot) {
#if !SL
			if(BrowserInteropHelper.IsBrowserHosted) {
				return;
			}
			TransitionEffectBase effect = GetTransitionEffect(TransitionEffect);
			TransitionEffectBase adornerEffect = GetAdornerTransitionEffect(TransitionEffect);
			if(effect == null || adornerEffect == null) {
				return;
			}
			effect.OldInput = screenshot;
			SaveTargetControlEffect();
			TargetControl.Effect = effect;
			adornerEffect.OldInput = new ImageBrush();
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TargetControl);
			if(adornerLayer != null) {
				SaveTargetControlAdornerEffect();
				adornerLayer.Effect = adornerEffect;
			}
#endif
		}
		protected virtual void TransitionAnimation(Brush screenshot) {
#if SL
			TransitionEffectBase effect = GetTransitionEffect(TransitionEffect);
			if(effect == null) {
				OnAfterApplyWorkspace();
				return;
			}
			effect.OldInput = screenshot;
			SaveTargetControlEffect();
			TargetControl.Effect = effect;
			DoubleAnimation animation = new DoubleAnimation() {
				From = 0.0,
				To = 1.0,
				Duration = TimeSpan.FromSeconds(1.5)
			};
			animation.Completed += OnTransitionAnimationCompleted;
			Storyboard.SetTarget(animation, effect);
			Storyboard.SetTargetProperty(animation, new PropertyPath("Progress"));
			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(animation);
			storyboard.Begin();
#else
			if(BrowserInteropHelper.IsBrowserHosted) {
				OnAfterApplyWorkspace();
				return;
			}
			TransitionEffectBase effect = TargetControl.Effect as TransitionEffectBase;
			TransitionEffectBase adornerEffect = AdornerLayer.GetAdornerLayer(TargetControl).Return(x => x.Effect, null) as TransitionEffectBase;
			if(effect == null || adornerEffect == null) {
				OnAfterApplyWorkspace();
				return;
			}
			DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(1.0)));
			DoubleAnimation adornerAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(1.0)));
			animation.Completed += OnTransitionAnimationCompleted;
			TargetControl.Effect.BeginAnimation(TransitionEffectBase.ProgressProperty, animation);
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TargetControl);
			if(adornerLayer != null) {
				adornerAnimation.Completed += OnAdornerTransitionAnimationCompleted;
				adornerEffect.BeginAnimation(TransitionEffectBase.ProgressProperty, adornerAnimation);
			}
#endif
		}
		protected virtual ImageSource RenderFrameworkElement(FrameworkElement source) {
#if SL
			return source == null ? null : new WriteableBitmap(source, null);
#else
			if(source == null)
				return null;
			Rect bounds = VisualTreeHelper.GetDescendantBounds(source);
			if(bounds.IsEmpty)
				return null;
			int width = (int)Math.Round(bounds.Width);
			int height = (int)Math.Round(bounds.Height);
			RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, 96.0, 96.0, PixelFormats.Pbgra32);
			VisualBrush brush = new VisualBrush(source);
			DrawingVisual drawingVisual = new DrawingVisual();
			using(DrawingContext drawingContext = drawingVisual.RenderOpen()) {
				drawingContext.DrawRectangle(brush, null, bounds);
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(source);
				if(adornerLayer != null) {
					Adorner[] adorners = adornerLayer.GetAdorners(source);
					if(adorners != null) {
						foreach(Adorner adorner in adorners) {
							bounds = VisualTreeHelper.GetDescendantBounds(adorner);
							if(!bounds.IsEmpty) {
								brush = new VisualBrush(adorner);
								drawingContext.DrawRectangle(brush, null, bounds);
							}
						}
					}
				}
			}
			bitmap.Render(drawingVisual);
			return bitmap;
#endif
		}
		protected virtual void OnTransitionAnimationCompleted(object sender, EventArgs e) {
			RestoreTargetControlEffect();
			OnAfterApplyWorkspace();
		}
		protected virtual void OnBeforeApplyWorkspace() {
			if(BeforeApplyWorkspace != null)
				BeforeApplyWorkspace(this, EventArgs.Empty);
		}
		protected virtual void OnAfterApplyWorkspace() {
			if(AfterApplyWorkspace != null)
				AfterApplyWorkspace(this, EventArgs.Empty);
		}
#if !SL
		protected virtual void SaveTargetControlAdornerEffect() {
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TargetControl);
			if(adornerLayer != null)
				saveAdornerEffect = adornerLayer.Effect;
		}
		protected virtual void RestoreTargetControlAdornerEffect() {
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TargetControl);
			if(adornerLayer != null)
				adornerLayer.Effect = saveAdornerEffect;
		}
		protected virtual TransitionEffectBase GetAdornerTransitionEffect(TransitionEffect effect) {
			switch(effect) {
				case TransitionEffect.Wave:
					return GetTransitionEffect(TransitionEffect.Fade);
				default:
					return GetTransitionEffect(TransitionEffect);
			}
		}
		protected virtual void OnAdornerTransitionAnimationCompleted(object sender, EventArgs e) {
			RestoreTargetControlAdornerEffect();
		}
#endif
	}
}

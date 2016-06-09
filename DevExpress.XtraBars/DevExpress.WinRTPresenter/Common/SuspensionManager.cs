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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
namespace DevExpress.WinRTPresenter.Common {
	internal sealed class SuspensionManager {
		private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();
		private static List<Type> _knownTypes = new List<Type>();
		private const string sessionStateFilename = "_sessionState.xml";
		public static Dictionary<string, object> SessionState {
			get { return _sessionState; }
		}
		public static List<Type> KnownTypes {
			get { return _knownTypes; }
		}
		public static async Task SaveAsync() {
			try {
				foreach(var weakFrameReference in _registeredFrames) {
					Frame frame;
					if(weakFrameReference.TryGetTarget(out frame)) {
						SaveFrameNavigationState(frame);
					}
				}
				MemoryStream sessionData = new MemoryStream();
				DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
				serializer.WriteObject(sessionData, _sessionState);
				StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
				using(Stream fileStream = await file.OpenStreamForWriteAsync()) {
					sessionData.Seek(0, SeekOrigin.Begin);
					await sessionData.CopyToAsync(fileStream);
					await fileStream.FlushAsync();
				}
			}
			catch(Exception e) {
				throw new SuspensionManagerException(e);
			}
		}
		public static async Task RestoreAsync() {
			_sessionState = new Dictionary<String, Object>();
			try {
				StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
				using(IInputStream inStream = await file.OpenSequentialReadAsync()) {
					DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
					_sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
				}
				foreach(var weakFrameReference in _registeredFrames) {
					Frame frame;
					if(weakFrameReference.TryGetTarget(out frame)) {
						frame.ClearValue(FrameSessionStateProperty);
						RestoreFrameNavigationState(frame);
					}
				}
			}
			catch(Exception e) {
				throw new SuspensionManagerException(e);
			}
		}
		private static DependencyProperty FrameSessionStateKeyProperty =
			DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(SuspensionManager), null);
		private static DependencyProperty FrameSessionStateProperty =
			DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(SuspensionManager), null);
		private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();
		public static void RegisterFrame(Frame frame, String sessionStateKey) {
			if(frame.GetValue(FrameSessionStateKeyProperty) != null) {
				throw new InvalidOperationException("Frames can only be registered to one session state key");
			}
			if(frame.GetValue(FrameSessionStateProperty) != null) {
				throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
			}
			frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
			_registeredFrames.Add(new WeakReference<Frame>(frame));
			RestoreFrameNavigationState(frame);
		}
		public static void UnregisterFrame(Frame frame) {
			SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
			_registeredFrames.RemoveAll((weakFrameReference) =>
			{
				Frame testFrame;
				return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
			});
		}
		public static Dictionary<String, Object> SessionStateForFrame(Frame frame) {
			var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);
			if(frameState == null) {
				var frameSessionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
				if(frameSessionKey != null) {
					if(!_sessionState.ContainsKey(frameSessionKey)) {
						_sessionState[frameSessionKey] = new Dictionary<String, Object>();
					}
					frameState = (Dictionary<String, Object>)_sessionState[frameSessionKey];
				}
				else {
					frameState = new Dictionary<String, Object>();
				}
				frame.SetValue(FrameSessionStateProperty, frameState);
			}
			return frameState;
		}
		private static void RestoreFrameNavigationState(Frame frame) {
			var frameState = SessionStateForFrame(frame);
			if(frameState.ContainsKey("Navigation")) {
				frame.SetNavigationState((String)frameState["Navigation"]);
			}
		}
		private static void SaveFrameNavigationState(Frame frame) {
			var frameState = SessionStateForFrame(frame);
			frameState["Navigation"] = frame.GetNavigationState();
		}
	}
	public class SuspensionManagerException : Exception {
		public SuspensionManagerException() {
		}
		public SuspensionManagerException(Exception e)
			: base("SuspensionManager failed", e) {
		}
	}
}

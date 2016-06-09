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
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
namespace DevExpress.Data.Camera.Interfaces {
	public interface ICameraDeviceClient {
		void OnNewFrame();
		void OnDeviceLost(CameraDeviceBase lostDevice);
		void OnResolutionChanged();
		IntPtr Handle { get; }
		CameraDeviceBase Device { get; }
	}
	[ComImport, Guid("56A868A9-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IGraphBuilder {
		[PreserveSig]
		int AddFilter([In] IBaseFilter filter, [In, MarshalAs(UnmanagedType.LPWStr)] string name);
		[PreserveSig]
		int RemoveFilter([In] IBaseFilter filter);
		[PreserveSig]
		int EnumFilters([Out] out IntPtr enumerator);
		[PreserveSig]
		int FindFilterByName([In, MarshalAs(UnmanagedType.LPWStr)] string name, [Out] out IBaseFilter filter);
		[PreserveSig]
		int ConnectDirect([In] IPin pinOut, [In] IPin pinIn, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int Reconnect([In] IPin pin);
		[PreserveSig]
		int Disconnect([In] IPin pin);
		[PreserveSig]
		int SetDefaultSyncSource();
		[PreserveSig]
		int Connect([In] IPin pinOut, [In] IPin pinIn);
		[PreserveSig]
		int Render([In] IPin pinOut);
		[PreserveSig]
		int RenderFile(
			[In, MarshalAs(UnmanagedType.LPWStr)] string file,
			[In, MarshalAs(UnmanagedType.LPWStr)] string playList);
		[PreserveSig]
		int AddSourceFilter(
			[In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
			[In, MarshalAs(UnmanagedType.LPWStr)] string filterName,
			[Out] out IBaseFilter filter);
		[PreserveSig]
		int SetLogFile(IntPtr handlerFile);
		[PreserveSig]
		int Abort();
		[PreserveSig]
		int ShouldOperationContinue();
	}
	[ComImport, Guid("56A86895-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IBaseFilter {
		[PreserveSig]
		int GetClassID([Out] out Guid classID);
		[PreserveSig]
		int Stop();
		[PreserveSig]
		int Pause();
		[PreserveSig]
		int Run(long start);
		[PreserveSig]
		int GetState(int milliSecsTimeout, [Out] out int filterState);
		[PreserveSig]
		int SetSyncSource([In] IntPtr clock);
		[PreserveSig]
		int GetSyncSource([Out] out IntPtr clock);
		[PreserveSig]
		int EnumPins([Out] out IEnumPins enumPins);
		[PreserveSig]
		int FindPin([In, MarshalAs(UnmanagedType.LPWStr)] string id, [Out] out IPin pin);
		[PreserveSig]
		int QueryFilterInfo([Out] CameraDeviceInfo filterInfo);
		[PreserveSig]
		int JoinFilterGraph([In] IFilterGraph graph, [In, MarshalAs(UnmanagedType.LPWStr)] string name);
		[PreserveSig]
		int QueryVendorInfo([Out, MarshalAs(UnmanagedType.LPWStr)] out string vendorInfo);
	}
	[ComImport, Guid("56A86891-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPin {
		[PreserveSig]
		int Connect([In] IPin receivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int ReceiveConnection([In] IPin receivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int Disconnect();
		[PreserveSig]
		int ConnectedTo([Out] out IPin pin);
		[PreserveSig]
		int ConnectionMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int QueryPinInfo([Out, MarshalAs(UnmanagedType.LPStruct)] PinInfo pinInfo);
		[PreserveSig]
		int QueryDirection(out PinDirection pinDirection);
		[PreserveSig]
		int QueryId([Out, MarshalAs(UnmanagedType.LPWStr)] out string id);
		[PreserveSig]
		int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int EnumMediaTypes(IntPtr enumerator);
		[PreserveSig]
		int QueryInternalConnections(IntPtr apPin, [In, Out] ref int nPin);
		[PreserveSig]
		int EndOfStream();
		[PreserveSig]
		int BeginFlush();
		[PreserveSig]
		int EndFlush();
		[PreserveSig]
		int NewSegment(long start, long stop, double rate);
	}
	[ComImport, Guid("56A86892-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumPins {
		[PreserveSig]
		int Next(
		   [In] int cPins,
		   [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IPin[] pins,
		   [Out] out int pinsFetched);
		[PreserveSig]
		int Skip([In] int cPins);
		[PreserveSig]
		int Reset();
		[PreserveSig]
		int Clone([Out] out IEnumPins enumPins);
	}
	[ComImport, Guid("56A8689F-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IFilterGraph {
		[PreserveSig]
		int AddFilter([In] IBaseFilter filter, [In, MarshalAs(UnmanagedType.LPWStr)] string name);
		[PreserveSig]
		int RemoveFilter([In] IBaseFilter filter);
		[PreserveSig]
		int EnumFilters([Out] out IntPtr enumerator);
		[PreserveSig]
		int FindFilterByName([In, MarshalAs(UnmanagedType.LPWStr)] string name, [Out] out IBaseFilter filter);
		[PreserveSig]
		int ConnectDirect([In] IPin pinOut, [In] IPin pinIn, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int Reconnect([In] IPin pin);
		[PreserveSig]
		int Disconnect([In] IPin pin);
		[PreserveSig]
		int SetDefaultSyncSource();
	}
	[ComImport, Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface ICaptureGraphBuilder2 {
		[PreserveSig]
		int SetFiltergraph([In] IGraphBuilder pfg);
		[PreserveSig]
		int GetFiltergraph(out IGraphBuilder ppfg);
		[PreserveSig]
		int SetOutputFileName([MarshalAs(UnmanagedType.LPStruct)] [In] Guid pType, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpstrFile, out IBaseFilter ppbf, out IFileSinkFilter ppSink);
		[PreserveSig]
		int FindInterface([MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid pCategory, [MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid pType, [In] IBaseFilter pbf, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppint);
		[PreserveSig]
		int RenderStream([MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid PinCategory, [MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid MediaType, [MarshalAs(UnmanagedType.IUnknown)] [In] object pSource, [In] IBaseFilter pfCompressor, [In] IBaseFilter pfRenderer);
		[PreserveSig]
		int ControlStream([MarshalAs(UnmanagedType.LPStruct)] [In] Guid PinCategory, [MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid MediaType, [MarshalAs(UnmanagedType.Interface)] [In] IBaseFilter pFilter, [In] DsLong pstart, [In] DsLong pstop, [In] short wStartCookie, [In] short wStopCookie);
		[PreserveSig]
		int AllocCapFile([MarshalAs(UnmanagedType.LPWStr)] [In] string lpstrFile, [In] long dwlSize);
		[PreserveSig]
		int CopyCaptureFile([MarshalAs(UnmanagedType.LPWStr)] [In] string lpwstrOld, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpwstrNew, [MarshalAs(UnmanagedType.Bool)] [In] bool fAllowEscAbort, [In] IAMCopyCaptureFileProgress pFilter);
		[PreserveSig]
		int FindPin([MarshalAs(UnmanagedType.IUnknown)] [In] object pSource, [In] PinDirection pindir, [MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid PinCategory, [MarshalAs(UnmanagedType.LPStruct)] [In] DsGuid MediaType, [MarshalAs(UnmanagedType.Bool)] [In] bool fUnconnected, [In] int ZeroBasedIndex, [MarshalAs(UnmanagedType.Interface)] out IPin ppPin);
	}
	[ComImport, Guid("55272A00-42CB-11CE-8135-00AA004BB851"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPropertyBag {
		[PreserveSig]
		int Read(
			[In, MarshalAs(UnmanagedType.LPWStr)] string propertyName,
			[In, Out, MarshalAs(UnmanagedType.Struct)] ref object pVar,
			[In] IntPtr pErrorLog);
		[PreserveSig]
		int Write(
			[In, MarshalAs(UnmanagedType.LPWStr)] string propertyName,
			[In, MarshalAs(UnmanagedType.Struct)] ref object pVar);
	}
	[ComImport, Guid("6B652FFF-11FE-4FCE-92AD-0266B5D7C78F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ISampleGrabber {
		[PreserveSig]
		int SetOneShot([In, MarshalAs(UnmanagedType.Bool)] bool oneShot);
		[PreserveSig]
		int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int GetConnectedMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType mediaType);
		[PreserveSig]
		int SetBufferSamples([In, MarshalAs(UnmanagedType.Bool)] bool bufferThem);
		[PreserveSig]
		int GetCurrentBuffer(ref int bufferSize, IntPtr buffer);
		[PreserveSig]
		int GetCurrentSample(IntPtr sample);
		[PreserveSig]
		int SetCallback(ISampleGrabberCB callback, int whichMethodToCallback);
	}
	[ComImport, Guid("0579154A-2B53-4994-B0D0-E773148EFF85"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ISampleGrabberCB {
		[PreserveSig]
		int SampleCB(double sampleTime, IntPtr sample);
		[PreserveSig]
		int BufferCB(double sampleTime, IntPtr buffer, int bufferLen);
	}
	[ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ICreateDevEnum {
		[PreserveSig]
		int CreateClassEnumerator([In] ref Guid type, [Out] out IEnumMoniker enumMoniker, [In] int flags);
	}
	[ComImport, Guid("56A868B4-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
	internal interface IVideoWindow {
		[PreserveSig]
		int put_Caption(string caption);
		[PreserveSig]
		int get_Caption([Out] out string caption);
		[PreserveSig]
		int put_WindowStyle(int windowStyle);
		[PreserveSig]
		int get_WindowStyle(out int windowStyle);
		[PreserveSig]
		int put_WindowStyleEx(int windowStyleEx);
		[PreserveSig]
		int get_WindowStyleEx(out int windowStyleEx);
		[PreserveSig]
		int put_AutoShow([In, MarshalAs(UnmanagedType.Bool)] bool autoShow);
		[PreserveSig]
		int get_AutoShow([Out, MarshalAs(UnmanagedType.Bool)] out bool autoShow);
		[PreserveSig]
		int put_WindowState(int windowState);
		[PreserveSig]
		int get_WindowState(out int windowState);
		[PreserveSig]
		int put_BackgroundPalette([In, MarshalAs(UnmanagedType.Bool)] bool backgroundPalette);
		[PreserveSig]
		int get_BackgroundPalette([Out, MarshalAs(UnmanagedType.Bool)] out bool backgroundPalette);
		[PreserveSig]
		int put_Visible([In, MarshalAs(UnmanagedType.Bool)] bool visible);
		[PreserveSig]
		int get_Visible([Out, MarshalAs(UnmanagedType.Bool)] out bool visible);
		[PreserveSig]
		int put_Left(int left);
		[PreserveSig]
		int get_Left(out int left);
		[PreserveSig]
		int put_Width(int width);
		[PreserveSig]
		int get_Width(out int width);
		[PreserveSig]
		int put_Top(int top);
		[PreserveSig]
		int get_Top(out int top);
		[PreserveSig]
		int put_Height(int height);
		[PreserveSig]
		int get_Height(out int height);
		[PreserveSig]
		int put_Owner(IntPtr owner);
		[PreserveSig]
		int get_Owner(out IntPtr owner);
		[PreserveSig]
		int put_MessageDrain(IntPtr drain);
		[PreserveSig]
		int get_MessageDrain(out IntPtr drain);
		[PreserveSig]
		int get_BorderColor(out int color);
		[PreserveSig]
		int put_BorderColor(int color);
		[PreserveSig]
		int get_FullScreenMode(
			[Out, MarshalAs(UnmanagedType.Bool)] out bool fullScreenMode);
		[PreserveSig]
		int put_FullScreenMode([In, MarshalAs(UnmanagedType.Bool)] bool fullScreenMode);
		[PreserveSig]
		int SetWindowForeground(int focus);
		[PreserveSig]
		int NotifyOwnerMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
		[PreserveSig]
		int SetWindowPosition(int left, int top, int width, int height);
		[PreserveSig]
		int GetWindowPosition(out int left, out int top, out int width, out int height);
		[PreserveSig]
		int GetMinIdealImageSize(out int width, out int height);
		[PreserveSig]
		int GetMaxIdealImageSize(out int width, out int height);
		[PreserveSig]
		int GetRestorePosition(out int left, out int top, out int width, out int height);
		[PreserveSig]
		int HideCursor([In, MarshalAs(UnmanagedType.Bool)] bool hideCursor);
		[PreserveSig]
		int IsCursorHidden([Out, MarshalAs(UnmanagedType.Bool)] out bool hideCursor);
	}
	[ComImport, Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface IFileSinkFilter {
		[PreserveSig]
		int SetFileName([MarshalAs(UnmanagedType.LPWStr)] [In] string pszFileName, [MarshalAs(UnmanagedType.LPStruct)] [In] AMMediaType pmt);
		[PreserveSig]
		int GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string pszFileName, [MarshalAs(UnmanagedType.LPStruct)] [Out] AMMediaType pmt);
	}
	[ComImport, Guid("56A868B1-0AD4-11CE-B03A-0020AF0BA770"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
	internal interface IMediaControl {
		[PreserveSig]
		int Run();
		[PreserveSig]
		int Pause();
		[PreserveSig]
		int Stop();
		[PreserveSig]
		int GetState(int timeout, out int filterState);
		[PreserveSig]
		int RenderFile(string fileName);
		[PreserveSig]
		int AddSourceFilter([In] string fileName, [Out, MarshalAs(UnmanagedType.IDispatch)] out object filterInfo);
		[PreserveSig]
		int get_FilterCollection(
			[Out, MarshalAs(UnmanagedType.IDispatch)] out object collection);
		[PreserveSig]
		int get_RegFilterCollection(
			[Out, MarshalAs(UnmanagedType.IDispatch)] out object collection);
		[PreserveSig]
		int StopWhenReady();
	}
	[ComImport, Guid("C6E13360-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface IAMVideoProcAmp {
		[PreserveSig]
		int GetRange([In] VideoProcAmpProperty Property, out int pMin, out int pMax, out int pSteppingDelta, out int pDefault, out VideoProcAmpFlags pCapsFlags);
		[PreserveSig]
		int Set([In] VideoProcAmpProperty Property, [In] int lValue, [In] VideoProcAmpFlags Flags);
		[PreserveSig]
		int Get([In] VideoProcAmpProperty Property, out int lValue, out VideoProcAmpFlags Flags);
	}
	[ComImport, Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface IAMStreamConfig {
		[PreserveSig]
		int SetFormat([MarshalAs(UnmanagedType.LPStruct)] [In] AMMediaType pmt);
		[PreserveSig]
		int GetFormat(out AMMediaType pmt);
		[PreserveSig]
		int GetNumberOfCapabilities(out int piCount, out int piSize);
		[PreserveSig]
		int GetStreamCaps([In] int iIndex, out AMMediaType ppmt, [In] IntPtr pSCC);
	}
	[ComImport, Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface IAMCopyCaptureFileProgress {
		[PreserveSig]
		int Progress(int iProgress);
	}
	[ComImport, Guid("36b73882-c2c8-11cf-8b46-00805f6cef60"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface IFilterGraph2 : IGraphBuilder, IFilterGraph {
		[PreserveSig]
		int EnumFilters(out IEnumFilters ppEnum);
		[PreserveSig]
		int AddSourceFilterForMoniker([In] IMoniker pMoniker, [In] IBindCtx pCtx, [MarshalAs(UnmanagedType.LPWStr)] [In] string lpcwstrFilterName, out IBaseFilter ppFilter);
		[PreserveSig]
		int ReconnectEx([In] IPin ppin, [In] AMMediaType pmt);
		[PreserveSig]
		int RenderEx([In] IPin pPinOut, [In] AMRenderExFlags dwFlags, [In] IntPtr pvContext);
	}
	[ComImport, Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
	internal interface IEnumFilters {
		[PreserveSig]
		int Next([In] int cFilters, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] [Out] IBaseFilter[] ppFilter, [In] IntPtr pcFetched);
		[PreserveSig]
		int Skip([In] int cFilters);
		[PreserveSig]
		int Reset();
		[PreserveSig]
		int Clone(out IEnumFilters ppEnum);
	}
	[Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsDual), SuppressUnmanagedCodeSecurity]
	[ComImport]
	public interface IMediaEventEx  {
		[PreserveSig]
		int GetEventHandle(out IntPtr hEvent);
		[PreserveSig]
		int GetEvent(out EventCode lEventCode, out IntPtr lParam1, out IntPtr lParam2, [In] int msTimeout);
		[PreserveSig]
		int WaitForCompletion([In] int msTimeout, out EventCode pEvCode);
		[PreserveSig]
		int CancelDefaultHandling([In] EventCode lEvCode);
		[PreserveSig]
		int RestoreDefaultHandling([In] EventCode lEvCode);
		[PreserveSig]
		int FreeEventParams([In] EventCode lEvCode, [In] IntPtr lParam1, [In] IntPtr lParam2);
		[PreserveSig]
		int SetNotifyWindow([In] IntPtr hwnd, [In] int lMsg, [In] IntPtr lInstanceData);
		[PreserveSig]
		int SetNotifyFlags([In] NotifyFlags lNoNotifyFlags);
		[PreserveSig]
		int GetNotifyFlags(out NotifyFlags lplNoNotifyFlags);
	}
}

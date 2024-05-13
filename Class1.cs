
using System;
using AutoCAD;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace AcadExample
{
    public static class Marshal2
    {
        internal const String OLEAUT32 = "oleaut32.dll";
        internal const String OLE32 = "ole32.dll";

        [System.Security.SecurityCritical]  // auto-generated_required
        public static Object GetActiveObject(String progID)
        {
            Object obj = null;
            Guid clsid;

            // Call CLSIDFromProgIDEx first then fall back on CLSIDFromProgID if
            // CLSIDFromProgIDEx doesn't exist.
            try
            {
                CLSIDFromProgIDEx(progID, out clsid);
            }
            //            catch
            catch (Exception)
            {
                CLSIDFromProgID(progID, out clsid);
            }

            GetActiveObject(ref clsid, IntPtr.Zero, out obj);
            return obj;
        }

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        //[ResourceExposure(ResourceScope.None)]
        //[SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] String progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        //[ResourceExposure(ResourceScope.None)]
        //[SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] String progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLEAUT32, PreserveSig = false)]
        [DllImport(OLEAUT32, PreserveSig = false)]
        //[ResourceExposure(ResourceScope.None)]
        //[SuppressUnmanagedCodeSecurity]
        [System.Security.SecurityCritical]  // auto-generated
        private static extern void GetActiveObject(ref Guid rclsid, IntPtr reserved, [MarshalAs(UnmanagedType.Interface)] out Object ppunk);

    }
    public class AutoCADConnector : IDisposable
    {
        private AcadApplication _application;
        private bool _initialized;
        private bool _disposed;
        public AutoCADConnector()
        {
            try
            {
                   // Upon creation, attempt to retrieve running instance
                   _application = (AcadApplication)Marshal2.GetActiveObject("AutoCAD.Application");
            }
            catch
            {
                try
                {
                    // Create an instance and set flag to indicate this
                    _application = new AcadApplicationClass();
                    _initialized = true;
                }
                catch
                {
                    throw;
                }
            }
        }
        // If the user doesn‘t call Dispose, the
        // garbage collector will upon destruction
        ~AutoCADConnector()
        {
            Dispose(false);
        }

        public AcadApplication Application
        {
            get
            {
                // Return our internal instance of AutoCAD
                return _application;
            }
        }

        // This is the user-callable version of Dispose.
        // It calls our internal version and removes the
        // object from the garbage collector‘s queue.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // This version of Dispose gets called by our
        // destructor.
        protected virtual void Dispose(bool disposing)
        {
            // If we created our AutoCAD instance, call its
            // Quit method to avoid leaking memory.
            if (!this._disposed && _initialized)
                _application.Quit();

            _disposed = true;
        }
    }
}

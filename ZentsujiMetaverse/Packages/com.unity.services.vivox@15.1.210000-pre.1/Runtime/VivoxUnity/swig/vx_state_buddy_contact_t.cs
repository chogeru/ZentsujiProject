//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class vx_state_buddy_contact_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal vx_state_buddy_contact_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(vx_state_buddy_contact_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~vx_state_buddy_contact_t() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          VivoxCoreInstancePINVOKE.delete_vx_state_buddy_contact_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public vx_buddy_presence_state presence {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_presence_set(swigCPtr, (int)value);
    } 
    get {
      vx_buddy_presence_state ret = (vx_buddy_presence_state)VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_presence_get(swigCPtr);
      return ret;
    } 
  }

  public string display_name {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_display_name_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_display_name_get(swigCPtr);
      return ret;
    } 
  }

  public string application {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_application_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_application_get(swigCPtr);
      return ret;
    } 
  }

  public string custom_message {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_custom_message_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_custom_message_get(swigCPtr);
      return ret;
    } 
  }

  public string contact {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_contact_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_contact_get(swigCPtr);
      return ret;
    } 
  }

  public string priority {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_priority_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_priority_get(swigCPtr);
      return ret;
    } 
  }

  public string id {
    set {
      VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_id_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_state_buddy_contact_t_id_get(swigCPtr);
      return ret;
    } 
  }

  public vx_state_buddy_contact_t() : this(VivoxCoreInstancePINVOKE.new_vx_state_buddy_contact_t(), true) {
  }

}

//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class vx_evt_base_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal vx_evt_base_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(vx_evt_base_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~vx_evt_base_t() {
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
          VivoxCoreInstancePINVOKE.delete_vx_evt_base_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

        public static implicit operator vx_evt_account_login_state_change_t(vx_evt_base_t t) {
            return t.as_vx_evt_account_login_state_change();
        }
        public static implicit operator vx_evt_buddy_presence_t(vx_evt_base_t t) {
            return t.as_vx_evt_buddy_presence();
        }
        public static implicit operator vx_evt_subscription_t(vx_evt_base_t t) {
            return t.as_vx_evt_subscription();
        }
        public static implicit operator vx_evt_session_notification_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_notification();
        }
        public static implicit operator vx_evt_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_message();
        }
        public static implicit operator vx_evt_session_delete_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_delete_message();
        }
        public static implicit operator vx_evt_session_edit_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_edit_message();
        }
        public static implicit operator vx_evt_account_delete_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_account_delete_message();
        }
        public static implicit operator vx_evt_account_edit_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_account_edit_message();
        }
        public static implicit operator vx_evt_session_archive_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_archive_message();
        }
        public static implicit operator vx_evt_transcribed_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_transcribed_message();
        }
        public static implicit operator vx_evt_session_archive_query_end_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_archive_query_end();
        }
        public static implicit operator vx_evt_aux_audio_properties_t(vx_evt_base_t t) {
            return t.as_vx_evt_aux_audio_properties();
        }
        public static implicit operator vx_evt_buddy_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_buddy_changed();
        }
        public static implicit operator vx_evt_buddy_group_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_buddy_group_changed();
        }
        public static implicit operator vx_evt_buddy_and_group_list_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_buddy_and_group_list_changed();
        }
        public static implicit operator vx_evt_keyboard_mouse_t(vx_evt_base_t t) {
            return t.as_vx_evt_keyboard_mouse();
        }
        public static implicit operator vx_evt_idle_state_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_idle_state_changed();
        }
        public static implicit operator vx_evt_media_stream_updated_t(vx_evt_base_t t) {
            return t.as_vx_evt_media_stream_updated();
        }
        public static implicit operator vx_evt_text_stream_updated_t(vx_evt_base_t t) {
            return t.as_vx_evt_text_stream_updated();
        }
        public static implicit operator vx_evt_sessiongroup_added_t(vx_evt_base_t t) {
            return t.as_vx_evt_sessiongroup_added();
        }
        public static implicit operator vx_evt_sessiongroup_removed_t(vx_evt_base_t t) {
            return t.as_vx_evt_sessiongroup_removed();
        }
        public static implicit operator vx_evt_session_added_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_added();
        }
        public static implicit operator vx_evt_session_removed_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_removed();
        }
        public static implicit operator vx_evt_participant_added_t(vx_evt_base_t t) {
            return t.as_vx_evt_participant_added();
        }
        public static implicit operator vx_evt_participant_removed_t(vx_evt_base_t t) {
            return t.as_vx_evt_participant_removed();
        }
        public static implicit operator vx_evt_participant_updated_t(vx_evt_base_t t) {
            return t.as_vx_evt_participant_updated();
        }
        public static implicit operator vx_evt_sessiongroup_playback_frame_played_t(vx_evt_base_t t) {
            return t.as_vx_evt_sessiongroup_playback_frame_played();
        }
        public static implicit operator vx_evt_session_updated_t(vx_evt_base_t t) {
            return t.as_vx_evt_session_updated();
        }
        public static implicit operator vx_evt_sessiongroup_updated_t(vx_evt_base_t t) {
            return t.as_vx_evt_sessiongroup_updated();
        }
        public static implicit operator vx_evt_media_completion_t(vx_evt_base_t t) {
            return t.as_vx_evt_media_completion();
        }
        public static implicit operator vx_evt_server_app_data_t(vx_evt_base_t t) {
            return t.as_vx_evt_server_app_data();
        }
        public static implicit operator vx_evt_user_app_data_t(vx_evt_base_t t) {
            return t.as_vx_evt_user_app_data();
        }
        public static implicit operator vx_evt_network_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_network_message();
        }
        public static implicit operator vx_evt_voice_service_connection_state_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_voice_service_connection_state_changed();
        }
        public static implicit operator vx_evt_publication_state_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_publication_state_changed();
        }
        public static implicit operator vx_evt_audio_device_hot_swap_t(vx_evt_base_t t) {
            return t.as_vx_evt_audio_device_hot_swap();
        }
        public static implicit operator vx_evt_user_to_user_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_user_to_user_message();
        }
        public static implicit operator vx_evt_account_archive_message_t(vx_evt_base_t t) {
            return t.as_vx_evt_account_archive_message();
        }
        public static implicit operator vx_evt_account_archive_query_end_t(vx_evt_base_t t) {
            return t.as_vx_evt_account_archive_query_end();
        }
        public static implicit operator vx_evt_account_send_message_failed_t(vx_evt_base_t t) {
            return t.as_vx_evt_account_send_message_failed();
        }
        public static implicit operator vx_evt_tts_injection_started_t(vx_evt_base_t t) {
            return t.as_vx_evt_tts_injection_started();
        }
        public static implicit operator vx_evt_tts_injection_ended_t(vx_evt_base_t t) {
            return t.as_vx_evt_tts_injection_ended();
        }
        public static implicit operator vx_evt_tts_injection_failed_t(vx_evt_base_t t) {
            return t.as_vx_evt_tts_injection_failed();
        }
        public static implicit operator vx_evt_stt_failed_t(vx_evt_base_t t) {
            return t.as_vx_evt_stt_failed();
        }
        public static implicit operator vx_evt_connection_state_changed_t(vx_evt_base_t t) {
            return t.as_vx_evt_connection_state_changed();
        }
        public static implicit operator vx_evt_presence_updated_t(vx_evt_base_t t) {
            return t.as_vx_evt_presence_updated();
        }
    
  public vx_message_base_t message {
    set {
      VivoxCoreInstancePINVOKE.vx_evt_base_t_message_set(swigCPtr, vx_message_base_t.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_message_get(swigCPtr);
      vx_message_base_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_message_base_t(cPtr, false);
      return ret;
    } 
  }

  public vx_event_type type {
    set {
      VivoxCoreInstancePINVOKE.vx_evt_base_t_type_set(swigCPtr, (int)value);
    } 
    get {
      vx_event_type ret = (vx_event_type)VivoxCoreInstancePINVOKE.vx_evt_base_t_type_get(swigCPtr);
      return ret;
    } 
  }

  public string extended_status_info {
    set {
      VivoxCoreInstancePINVOKE.vx_evt_base_t_extended_status_info_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_evt_base_t_extended_status_info_get(swigCPtr);
      return ret;
    } 
  }

  public vx_evt_account_login_state_change_t as_vx_evt_account_login_state_change() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_account_login_state_change(swigCPtr);
    vx_evt_account_login_state_change_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_account_login_state_change_t(cPtr, false);
    return ret;
  }

  public vx_evt_buddy_presence_t as_vx_evt_buddy_presence() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_buddy_presence(swigCPtr);
    vx_evt_buddy_presence_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_buddy_presence_t(cPtr, false);
    return ret;
  }

  public vx_evt_subscription_t as_vx_evt_subscription() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_subscription(swigCPtr);
    vx_evt_subscription_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_subscription_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_notification_t as_vx_evt_session_notification() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_notification(swigCPtr);
    vx_evt_session_notification_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_notification_t(cPtr, false);
    return ret;
  }

  public vx_evt_message_t as_vx_evt_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_message(swigCPtr);
    vx_evt_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_delete_message_t as_vx_evt_session_delete_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_delete_message(swigCPtr);
    vx_evt_session_delete_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_delete_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_edit_message_t as_vx_evt_session_edit_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_edit_message(swigCPtr);
    vx_evt_session_edit_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_edit_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_account_delete_message_t as_vx_evt_account_delete_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_account_delete_message(swigCPtr);
    vx_evt_account_delete_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_account_delete_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_account_edit_message_t as_vx_evt_account_edit_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_account_edit_message(swigCPtr);
    vx_evt_account_edit_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_account_edit_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_archive_message_t as_vx_evt_session_archive_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_archive_message(swigCPtr);
    vx_evt_session_archive_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_archive_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_transcribed_message_t as_vx_evt_transcribed_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_transcribed_message(swigCPtr);
    vx_evt_transcribed_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_transcribed_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_archive_query_end_t as_vx_evt_session_archive_query_end() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_archive_query_end(swigCPtr);
    vx_evt_session_archive_query_end_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_archive_query_end_t(cPtr, false);
    return ret;
  }

  public vx_evt_aux_audio_properties_t as_vx_evt_aux_audio_properties() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_aux_audio_properties(swigCPtr);
    vx_evt_aux_audio_properties_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_aux_audio_properties_t(cPtr, false);
    return ret;
  }

  public vx_evt_buddy_changed_t as_vx_evt_buddy_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_buddy_changed(swigCPtr);
    vx_evt_buddy_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_buddy_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_buddy_group_changed_t as_vx_evt_buddy_group_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_buddy_group_changed(swigCPtr);
    vx_evt_buddy_group_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_buddy_group_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_buddy_and_group_list_changed_t as_vx_evt_buddy_and_group_list_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_buddy_and_group_list_changed(swigCPtr);
    vx_evt_buddy_and_group_list_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_buddy_and_group_list_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_keyboard_mouse_t as_vx_evt_keyboard_mouse() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_keyboard_mouse(swigCPtr);
    vx_evt_keyboard_mouse_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_keyboard_mouse_t(cPtr, false);
    return ret;
  }

  public vx_evt_idle_state_changed_t as_vx_evt_idle_state_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_idle_state_changed(swigCPtr);
    vx_evt_idle_state_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_idle_state_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_media_stream_updated_t as_vx_evt_media_stream_updated() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_media_stream_updated(swigCPtr);
    vx_evt_media_stream_updated_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_media_stream_updated_t(cPtr, false);
    return ret;
  }

  public vx_evt_text_stream_updated_t as_vx_evt_text_stream_updated() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_text_stream_updated(swigCPtr);
    vx_evt_text_stream_updated_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_text_stream_updated_t(cPtr, false);
    return ret;
  }

  public vx_evt_sessiongroup_added_t as_vx_evt_sessiongroup_added() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_sessiongroup_added(swigCPtr);
    vx_evt_sessiongroup_added_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_sessiongroup_added_t(cPtr, false);
    return ret;
  }

  public vx_evt_sessiongroup_removed_t as_vx_evt_sessiongroup_removed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_sessiongroup_removed(swigCPtr);
    vx_evt_sessiongroup_removed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_sessiongroup_removed_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_added_t as_vx_evt_session_added() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_added(swigCPtr);
    vx_evt_session_added_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_added_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_removed_t as_vx_evt_session_removed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_removed(swigCPtr);
    vx_evt_session_removed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_removed_t(cPtr, false);
    return ret;
  }

  public vx_evt_participant_added_t as_vx_evt_participant_added() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_participant_added(swigCPtr);
    vx_evt_participant_added_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_participant_added_t(cPtr, false);
    return ret;
  }

  public vx_evt_participant_removed_t as_vx_evt_participant_removed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_participant_removed(swigCPtr);
    vx_evt_participant_removed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_participant_removed_t(cPtr, false);
    return ret;
  }

  public vx_evt_participant_updated_t as_vx_evt_participant_updated() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_participant_updated(swigCPtr);
    vx_evt_participant_updated_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_participant_updated_t(cPtr, false);
    return ret;
  }

  public vx_evt_sessiongroup_playback_frame_played_t as_vx_evt_sessiongroup_playback_frame_played() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_sessiongroup_playback_frame_played(swigCPtr);
    vx_evt_sessiongroup_playback_frame_played_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_sessiongroup_playback_frame_played_t(cPtr, false);
    return ret;
  }

  public vx_evt_session_updated_t as_vx_evt_session_updated() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_session_updated(swigCPtr);
    vx_evt_session_updated_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_session_updated_t(cPtr, false);
    return ret;
  }

  public vx_evt_sessiongroup_updated_t as_vx_evt_sessiongroup_updated() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_sessiongroup_updated(swigCPtr);
    vx_evt_sessiongroup_updated_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_sessiongroup_updated_t(cPtr, false);
    return ret;
  }

  public vx_evt_media_completion_t as_vx_evt_media_completion() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_media_completion(swigCPtr);
    vx_evt_media_completion_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_media_completion_t(cPtr, false);
    return ret;
  }

  public vx_evt_server_app_data_t as_vx_evt_server_app_data() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_server_app_data(swigCPtr);
    vx_evt_server_app_data_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_server_app_data_t(cPtr, false);
    return ret;
  }

  public vx_evt_user_app_data_t as_vx_evt_user_app_data() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_user_app_data(swigCPtr);
    vx_evt_user_app_data_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_user_app_data_t(cPtr, false);
    return ret;
  }

  public vx_evt_network_message_t as_vx_evt_network_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_network_message(swigCPtr);
    vx_evt_network_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_network_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_voice_service_connection_state_changed_t as_vx_evt_voice_service_connection_state_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_voice_service_connection_state_changed(swigCPtr);
    vx_evt_voice_service_connection_state_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_voice_service_connection_state_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_publication_state_changed_t as_vx_evt_publication_state_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_publication_state_changed(swigCPtr);
    vx_evt_publication_state_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_publication_state_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_audio_device_hot_swap_t as_vx_evt_audio_device_hot_swap() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_audio_device_hot_swap(swigCPtr);
    vx_evt_audio_device_hot_swap_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_audio_device_hot_swap_t(cPtr, false);
    return ret;
  }

  public vx_evt_user_to_user_message_t as_vx_evt_user_to_user_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_user_to_user_message(swigCPtr);
    vx_evt_user_to_user_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_user_to_user_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_account_archive_message_t as_vx_evt_account_archive_message() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_account_archive_message(swigCPtr);
    vx_evt_account_archive_message_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_account_archive_message_t(cPtr, false);
    return ret;
  }

  public vx_evt_account_archive_query_end_t as_vx_evt_account_archive_query_end() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_account_archive_query_end(swigCPtr);
    vx_evt_account_archive_query_end_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_account_archive_query_end_t(cPtr, false);
    return ret;
  }

  public vx_evt_account_send_message_failed_t as_vx_evt_account_send_message_failed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_account_send_message_failed(swigCPtr);
    vx_evt_account_send_message_failed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_account_send_message_failed_t(cPtr, false);
    return ret;
  }

  public vx_evt_tts_injection_started_t as_vx_evt_tts_injection_started() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_tts_injection_started(swigCPtr);
    vx_evt_tts_injection_started_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_tts_injection_started_t(cPtr, false);
    return ret;
  }

  public vx_evt_tts_injection_ended_t as_vx_evt_tts_injection_ended() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_tts_injection_ended(swigCPtr);
    vx_evt_tts_injection_ended_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_tts_injection_ended_t(cPtr, false);
    return ret;
  }

  public vx_evt_tts_injection_failed_t as_vx_evt_tts_injection_failed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_tts_injection_failed(swigCPtr);
    vx_evt_tts_injection_failed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_tts_injection_failed_t(cPtr, false);
    return ret;
  }

  public vx_evt_stt_failed_t as_vx_evt_stt_failed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_stt_failed(swigCPtr);
    vx_evt_stt_failed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_stt_failed_t(cPtr, false);
    return ret;
  }

  public vx_evt_connection_state_changed_t as_vx_evt_connection_state_changed() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_connection_state_changed(swigCPtr);
    vx_evt_connection_state_changed_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_connection_state_changed_t(cPtr, false);
    return ret;
  }

  public vx_evt_presence_updated_t as_vx_evt_presence_updated() {
    global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_evt_base_t_as_vx_evt_presence_updated(swigCPtr);
    vx_evt_presence_updated_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_evt_presence_updated_t(cPtr, false);
    return ret;
  }

  public vx_evt_base_t() : this(VivoxCoreInstancePINVOKE.new_vx_evt_base_t(), true) {
  }

}

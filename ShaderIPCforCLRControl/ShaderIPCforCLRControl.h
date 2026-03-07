#pragma once

#include "ControlMessageWindow.h"
#include "ManagedObject.h"

using namespace System;

namespace ShaderIPCforCLRControl {

public
ref class ShaderMessagingWindow : ManagedObject<ControlMessageWindow>
	{
public:
    ShaderMessagingWindow();

        delegate void StringReceive(ShaderMessagingWindow ^ win, String ^ data);
        delegate void CodeReceive(ShaderMessagingWindow ^ win, UINT_PTR data);
        delegate void ReceiveParameter(ShaderMessagingWindow ^ win, int range, int relativeValue, String ^ name, String ^ tooltip);

        String ^ GetLastLoadedProfilePath();
        String ^ GetLastLoadedShader();

        /// <summary>
        /// Handles registration, creation and handshake, after which the communication object is live.
        /// You can perform preparatory steps:
        ///     CustomRegisterClass (if there is a specific class name you would use)
        ///     SetTimeout
        /// You should perform preparatory steps:
        ///     specify at least a pid in SetPartnerDetails
        ///     register events you wish to respond to, possibly including AfterHandshake
        /// </summary>
        void StandardInit();
        void StandardInit(HINSTANCE hInst);

        void SetPartnerDetails(int pid, String ^ name, String ^ className);

	void AnticipateHandshake();
    void Create(HINSTANCE hInst, String ^ name);

    HWND         GetHWND();
    static void CustomRegisterClass(HINSTANCE hInst, String ^ name);
    unsigned int ExtendedGetState();

    void SendParameterValue(String ^ name, int relativeValue);

    void ReloadProfile();
    void LoadProfile(String ^ path);
    void LoadShader(String ^ path);
    void ResetParameters();
    void Refresh();
    void Refresh(unsigned int delay_ms);

	void SetTimeout(unsigned int ms);
    unsigned int GetTimeout();

    event EventHandler ^ Created;
    event StringReceive ^ ProfileSaved;
    event CodeReceive ^ SendFailed;
    event EventHandler ^ BeginRedefine;
    event EventHandler ^ EndRedefine;
    event EventHandler ^ FullscreenChanged;
    event EventHandler ^ MinimizedChanged;
    event EventHandler ^ AfterHandshake;
    event EventHandler ^ ProfileIsLoaded;
    event EventHandler ^ ShaderIsLoaded;
	event ReceiveParameter ^ ParameterReceived;
    event StringReceive ^ DiagnosticTextReceived;

	void SetFullscreen(bool fullscreen);
    bool GetFullscreen();
    void SetMinimized(bool minimized);
    bool GetMinimized();

    bool IsCreated();
    bool HasCompletedHandshake();

	protected:
    void        OnDispose(ControlMessageWindow* disposeInstance) override;
	private:
    array<Object ^>^ live_with_this_object;
    void OnCreated(ControlMessageWindow* window);
    void OnHandshakeCompleted(ControlMessageWindow* window);
    void OnProfileSaved(LPCWSTR path, ControlMessageWindow* window);
    void OnShaderBeginRedefine(ControlMessageWindow* window);
    void OnShaderEndRedefine(ControlMessageWindow* window);
    void OnUpdateFullscreen(bool fs, ControlMessageWindow* window);
    void OnUpdateMinimized(bool fs, ControlMessageWindow* window);
    void OnReceiveParameter(int range, int relativeValue, LPCWSTR name, LPCWSTR tooltip, ControlMessageWindow* window);
    void OnSendFailed(UINT_PTR msgType, IPCWindow* ptr);
    void OnReceiveDiagnosticText(LPCWSTR text, ControlMessageWindow* window);
    void OnProfileLoaded(LPCWSTR text, ControlMessageWindow* window);
    void OnShaderLoaded(LPCWSTR text, ControlMessageWindow* window);

    bool fs, min, created = false;
    Object ^lifedlg;
    String ^ profile = String::Empty, ^shader = String::Empty;

	void OnCreated(System::Object ^ sender, System::EventArgs ^ e);
    void OnAfterHandshake(System::Object ^ sender, System::EventArgs ^ e);

    };
    }

#include "ShaderIPCforCLRControl.h"
#include <vcclr.h>

#include <codecvt>

static String ^ AsString(LPCWSTR str) { 
    if(!str)
            return nullptr;
        return gcnew String(str, 0, wcslen(str));
    }

    namespace ShaderIPCforCLRControl
{
    #define makefuncptr(dlg, call) {auto f = gcnew dlg(this, &ShaderMessagingWindow::call);  l->Add(f); i->call = static_cast<decltype(i->call)>((void*)System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate<dlg ^>(f));    }


        ///Only call this/overloads once per instance, even though the underlying IPC window may support reuse of an instance, the wrapper does not
        void ShaderMessagingWindow::StandardInit() {
            HMODULE module = NULL;
            if(!GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, (LPCWSTR) & AsString, &module))
                module = GetModuleHandle(NULL);
            StandardInit((HINSTANCE)module);
        }
        ///Only call this/overloads once per instance, even though the underlying IPC window may support reuse of an instance, the wrapper does not
    void ShaderMessagingWindow::StandardInit(HINSTANCE hInst) { 
        Created += gcnew System::EventHandler(this, &ShaderIPCforCLRControl::ShaderMessagingWindow::OnCreated);
        AfterHandshake += gcnew System::EventHandler(this, &ShaderIPCforCLRControl::ShaderMessagingWindow::OnAfterHandshake);
        GetInstance()->Create(hInst);
    }
    void ShaderMessagingWindow::SetPartnerDetails(int pid, String^ name, String^ classname) {
        if(name == nullptr)
        {
            if(classname == nullptr)
            {
                GetInstance()->SetPartnerDetails(pid);
                return;
            }
            pin_ptr<const wchar_t> c = PtrToStringChars(classname);
            GetInstance()->SetPartnerDetails(pid, nullptr, c);
        }
        else if(classname == nullptr)
        {
            pin_ptr<const wchar_t> n = PtrToStringChars(name);
            GetInstance()->SetPartnerDetails(pid, n);
        }
        else
        {
            pin_ptr<const wchar_t> n = PtrToStringChars(name);
            pin_ptr<const wchar_t> c = PtrToStringChars(classname);
            GetInstance()->SetPartnerDetails(pid, n, c);
        }
    }


        
	void ShaderMessagingWindow::SetTimeout(unsigned int ms)
{
        GetInstance()->IPCTimeoutMS = ms;
    }
unsigned int ShaderMessagingWindow::GetTimeout()
{
        return GetInstance()->IPCTimeoutMS;
    }

void ShaderMessagingWindow::ResetParameters() {
    GetInstance()->ResetParameters();
    }


    void          ShaderMessagingWindow::LoadProfile(String^ path) {
        pin_ptr<const wchar_t> s = PtrToStringChars(path);
        GetInstance()->SendProfilePath(s);
    }
    void          ShaderMessagingWindow::LoadShader(String^ path) {
        pin_ptr<const wchar_t> s = PtrToStringChars(path);
        GetInstance()->SendShaderPath(s);
    }
    void ShaderMessagingWindow::ReloadProfile()
    {
        GetInstance()->ReloadProfile();
    }
    unsigned int ShaderMessagingWindow::ExtendedGetState()
    {
        return GetInstance()->ExtendedGetState();
    }

    void ShaderMessagingWindow::Create(HINSTANCE hInst, String ^ name)
    {
        pin_ptr<const wchar_t> s = PtrToStringChars(name);
        GetInstance()->Create(hInst, s);
    }
    void ShaderMessagingWindow::CustomRegisterClass(HINSTANCE hInst, String^ name) {
        pin_ptr<const wchar_t> s = PtrToStringChars(name);
        ControlMessageWindow::CustomRegisterClass(hInst, s);
    }
    void ShaderMessagingWindow::AnticipateHandshake()
    {
        GetInstance()->AnticipateHandshake();
    }

    delegate void sendvoid(ControlMessageWindow* window);
    delegate void sendstr(LPCWSTR path, ControlMessageWindow* window);
    delegate void         receiveuintptr(UINT_PTR msg, IPCWindow * window);
    delegate void sendbool(bool fs, ControlMessageWindow* window);
    delegate void         sendparam(int range, int relativeValue, LPCWSTR name, LPCWSTR tooltip, ControlMessageWindow* window);

    String ^ ShaderMessagingWindow::GetLastLoadedProfilePath()
    {
        return profile;
    }
    String ^ ShaderMessagingWindow::GetLastLoadedShader()
    {
        return shader;
    }

    void ShaderMessagingWindow::OnProfileLoaded(LPCWSTR text, ControlMessageWindow* window) {
        auto txt = AsString(text)->Replace("\"", String::Empty)->Trim();
        if(txt != profile)
        {
            profile = txt;
            ProfileIsLoaded(this, EventArgs::Empty);
        }
    }
    void ShaderMessagingWindow::OnShaderLoaded(LPCWSTR text, ControlMessageWindow* window) {
        auto txt = AsString(text)->Replace("\"", String::Empty)->Trim();
        if(txt != shader)
        {
            shader = txt;
            ShaderIsLoaded(this, EventArgs::Empty);
        }
    }
    void ShaderMessagingWindow::OnProfileSaved(LPCWSTR path, ControlMessageWindow * window)
    {
        ProfileSaved(this, AsString(path));
    }
    void ShaderMessagingWindow::OnReceiveDiagnosticText(LPCWSTR path, ControlMessageWindow * window)
    {
        DiagnosticTextReceived(this, AsString(path));
    }
    void ShaderMessagingWindow::OnShaderBeginRedefine(ControlMessageWindow* window) {
        BeginRedefine(this, EventArgs::Empty);
    }
    void ShaderMessagingWindow::OnShaderEndRedefine(ControlMessageWindow* window) {
        EndRedefine(this, EventArgs::Empty);
    }
    void ShaderMessagingWindow::OnUpdateFullscreen(bool fs, ControlMessageWindow* window) {
        this->fs = fs;
        FullscreenChanged(this, EventArgs::Empty);
    }
    void ShaderMessagingWindow::OnUpdateMinimized(bool fs, ControlMessageWindow* window) {
        this->min = min;
        MinimizedChanged(this, EventArgs::Empty);
    }
    void ShaderMessagingWindow::OnReceiveParameter(int range, int relativeValue, LPCWSTR name, LPCWSTR tooltip, ControlMessageWindow* window)
    {
        DiagnosticTextReceived(this, String::Format(L"(internal) OnReceiveParameter({0}, {1}, {2}, {3})", range, relativeValue, name ? AsString(name) : String::Empty, tooltip ? AsString(tooltip) : String::Empty));
        ParameterReceived(this, range, relativeValue, AsString(name), AsString(tooltip));
    }
    void ShaderMessagingWindow::OnSendFailed(UINT_PTR msgType, IPCWindow* ptr) {
        SendFailed(this, msgType);
    }

	ShaderMessagingWindow::ShaderMessagingWindow() :
        ManagedObject(new ControlMessageWindow())
	{
        auto i = GetInstance();
        auto l = gcnew System::Collections::Generic::List<Object ^>();

        makefuncptr(sendstr, OnProfileSaved);
        makefuncptr(sendstr, OnReceiveDiagnosticText);
        makefuncptr(sendvoid, OnShaderBeginRedefine);
        makefuncptr(sendvoid, OnShaderEndRedefine);
        makefuncptr(sendbool, OnUpdateFullscreen);
        makefuncptr(sendbool, OnUpdateMinimized);
        makefuncptr(sendparam, OnReceiveParameter);
        makefuncptr(sendstr, OnProfileLoaded);
        makefuncptr(sendstr, OnShaderLoaded);

        live_with_this_object = l->ToArray();

        {
            auto f  = gcnew sendvoid(this, &ShaderMessagingWindow::OnCreated);
            lifedlg = f;
            auto p  = (void (*)(IPCWindow*))(void*)System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate<sendvoid ^>(f);
            i->RunAfterCreated(p);
        }
        /* this looks misplaced
        {
            auto f  = gcnew getid(this, &ShaderMessagingWindow::Identify);
            lifedlg = f;
            auto p  = (void (*)(IPCWindow*))(void*)System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate<getid ^>(f);
            i->RunAfterCreated(p);
        }
        */
	}

    void ShaderMessagingWindow::SetFullscreen(bool fullscreen) {
        fs = fullscreen;
        GetInstance()->SetFullscreen(fullscreen);
    }
    bool ShaderMessagingWindow::GetFullscreen() {
        return fs;
    }
    void ShaderMessagingWindow::SetMinimized(bool minimized) {
        min = minimized;
        GetInstance()->SetMinimized(minimized);
    }
    bool ShaderMessagingWindow::GetMinimized() {
        return min;
    }

    bool ShaderMessagingWindow::IsCreated()
    {
        return created;
    }
    bool ShaderMessagingWindow::HasCompletedHandshake()
    {
        return created && lifedlg == nullptr;
    }
    
    void ShaderMessagingWindow::OnCreated(ControlMessageWindow * window) 
    { 
        created = true;

        {
            auto f           = gcnew sendvoid(this, &ShaderMessagingWindow::OnHandshakeCompleted);
            auto           p = (void (*)(IPCWindow*))(void*)System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate<sendvoid ^>(f);
            GetInstance()->RunAfterHandshake(p);
            lifedlg = f;
        }

        Created(this, EventArgs::Empty);
    }
    void ShaderMessagingWindow::OnHandshakeCompleted(ControlMessageWindow* window) {
        lifedlg = nullptr;
        AfterHandshake(this, EventArgs::Empty);
    }

    void ShaderMessagingWindow::OnDispose(ControlMessageWindow * window)
    {
        DestroyWindow(window->GetHWND());
        ManagedObject::OnDispose(window);
    }

    HWND ShaderMessagingWindow::GetHWND() {
        return GetInstance()->GetHWND();
    }

    void ShaderMessagingWindow::OnAfterHandshake(System::Object ^ sender, System::EventArgs ^ e)
    {
        auto i = GetInstance();
        auto l = gcnew System::Collections::Generic::List<Object ^>((array<Object ^> ^) live_with_this_object);
        makefuncptr(receiveuintptr, OnSendFailed);
        live_with_this_object = l->ToArray();
    }
    static void RefreshByPtr(IPCWindow* ipc) {
        ((ControlMessageWindow*)ipc)->RequestDefinition();
    }
    void ShaderMessagingWindow::Refresh(unsigned int delay_ms) {
        GetInstance()->RunOnce(RefreshByPtr, delay_ms);
    }
    void ShaderMessagingWindow::Refresh()
    {
        GetInstance()->RequestDefinition();
    }
    void ShaderMessagingWindow::OnCreated(System::Object ^ sender, System::EventArgs ^ e)
    {
        GetInstance()->AnticipateHandshake();
    }

    void ShaderMessagingWindow::SendParameterValue(String^ name, int relativeValue) {
        pin_ptr<const wchar_t> n = PtrToStringChars(name);
        GetInstance()->SetParameter(relativeValue, n);
    }

    } // namespace ShaderIPCforCLRControl

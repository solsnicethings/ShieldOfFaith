#include "ControlMessageWindow.h"

static void clear_param_cache(std::vector<std::tuple<int, int, LPWSTR, LPWSTR>>& cache) 
{
    for(auto p : cache)
    {
        auto s = std::get<2>(p);
        if(s) delete[] s;
        s = std::get<3>(p);
        if(s) delete[] s;
    }
    cache.clear();
}

ControlMessageWindow::~ControlMessageWindow() 
{
    clear_param_cache(Parameters);
    IPCWindow::~IPCWindow();
}

unsigned long long ControlMessageWindow::SendProfilePath(LPCWSTR path)
{
    if ( ReverifyPartnerCriteriaInStatedataPtr())
        return SendStringPtr(ShaderControlCode::NewProfilePath, path);
    return 0;
}
unsigned long long ControlMessageWindow::SendShaderPath(LPCWSTR path)
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
    return SendStringPtr(ShaderControlCode::NewShaderPath, path);
    return 0;
}

unsigned long long ControlMessageWindow::SetParameter(int relative_value, LPCWSTR name) {
    if(!ReverifyPartnerCriteriaInStatedataPtr())
        return 0;

    auto parameter_key = RequestRemoteId(name);
    if(!parameter_key) return FALSE;
    int data[2];
    data[0] = parameter_key;
    data[1] = relative_value;
    return SendDataPtr(ShaderControlCode::ParameterData, &data, sizeof(data));
}

unsigned long long ControlMessageWindow::ResetParameters() {
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(ShaderControlCode::ResetShaderParams);
    return 0;
}
unsigned long long ControlMessageWindow::ReloadProfile()
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(ShaderControlCode::ReloadProfile);
    return 0;
}
unsigned long long ControlMessageWindow::RequestDefinition()
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(ShaderControlCode::BeginRedefine);
    return 0;
}

unsigned long long ControlMessageWindow::SetFullscreen(bool fullscreen)
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(fullscreen ? ShaderControlCode::FullscreenOn : ShaderControlCode::FullscreenOff);
    return 0;
}
unsigned long long ControlMessageWindow::SaveProfile(LPCWSTR profile_save_as) {
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendStringPtr(ShaderControlCode::SaveProfile, profile_save_as);
    return 0;
}

unsigned long long ControlMessageWindow::ToggleMenu()
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(ShaderControlCode::ToggleMenu);
    return 0;
}
unsigned long long ControlMessageWindow::SetMinimized(bool minimized)
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(minimized ? ShaderControlCode::Minimize : ShaderControlCode::Restore);
    return 0;
}

LRESULT ControlMessageWindow::HandleReceivedSignal(HWND from, UINT signal)
{
    if(!ReverifyPartnerCriteriaInStatedataPtr())
        return 0;

    if(OnReceiveDiagnosticText)
    {
        auto c_str =  std::wstring(L"(internal) signal received: ").append(std::to_wstring(signal));
        OnReceiveDiagnosticText(c_str.c_str(), this);
    }

    switch(signal)
    {
    case ShaderControlCode::BeginRedefine:
        clear_param_cache(Parameters);
        if(OnShaderBeginRedefine)
            OnShaderBeginRedefine(this);
        break;
    case ShaderControlCode::EndRedefine:
        if(OnShaderEndRedefine)
            OnShaderEndRedefine(this);
        break;
    case ShaderControlCode::FullscreenOff:
        if(OnUpdateFullscreen)
            OnUpdateFullscreen(false, this);
        break;
    case ShaderControlCode::FullscreenOn:
        if(OnUpdateFullscreen)
            OnUpdateFullscreen(true, this);
        break;
    case ShaderControlCode::Minimize:
        if(OnUpdateMinimized)
            OnUpdateMinimized(true, this);
        break;
    case ShaderControlCode::Restore:
        if(OnUpdateMinimized)
            OnUpdateMinimized(false, this);
        break;

    default:
        return 0;
    }

    return signal;
}


void ControlMessageWindow::HandleQueuedData(HWND from, COPYDATASTRUCT* data) { 
    
    switch (data->dwData)
    {
    case ShaderControlCode::ParameterData:
        if(!OnReceiveParameter)
            break;
        {
            int* specs = (int*)data->lpData;
            auto p     = Parameters.at((*specs) - 1);
            switch(data->cbData)
            {
            case sizeof(int[3]):
                std::get<1>(p) = *(++specs);
                std::get<0>(p) = *(++specs);
                break;
            case sizeof(int[2]):
                std::get<1>(p) = *(++specs);
                OnReceiveParameter(0, std::get<1>(p), std::get<2>(p), 0, this);
                return;
            }
            OnReceiveParameter(std::get<0>(p), std::get<1>(p), std::get<2>(p), std::get<3>(p), this);
            return;
        }
        break;
    case ShaderControlCode::SaveProfile:
        if(OnProfileSaved)
        {
            OnProfileSaved((LPWSTR)data->lpData, this);
            return;
        }
        break;
    case ShaderControlCode::NewProfilePath:
        if(OnProfileLoaded)
        {
            OnProfileLoaded((LPWSTR)data->lpData, this);
            return;
        }
        break;
    case ShaderControlCode::NewShaderPath:
        if(OnShaderLoaded)
        {
            OnShaderLoaded((LPWSTR)data->lpData, this);
            return;
        }
        break;

    case IPCWCode::DiagnosticMessage:
        if(OnReceiveDiagnosticText)
            OnReceiveDiagnosticText((LPCWSTR)data->lpData, this);
        return;
    default:
        if(OnReceiveDiagnosticText)
        {
            auto size = data->cbData;
            auto s = std::wstring(L"(internal) queued data type ").append(std::to_wstring(data->dwData)).append(L" of size ").append(std::to_wstring(size));
            auto c    = (char*)data->lpData;
            if(size && c)
            {
                s.append(L"=");
                do
                {
                    auto v = *c++;
                    auto digits = std::wstring(L"0123456789abcdef");
                    s.append(digits.substr(v > 1, 1));
                    s.append(digits.substr(v & 0xf, 1));
                }
                while(--size);
            }
            else
                s.append(L" IS NULL");
            OnReceiveDiagnosticText(s.c_str(), this);
        }
    }
    IPCWindow::HandleQueuedData(from, data);
}
    LRESULT ControlMessageWindow::HandleReceivedData(HWND from, COPYDATASTRUCT * data)
    {
    if(!ReverifyPartnerCriteriaInStatedataPtr())
        return 0;
    switch (data->dwData)
    {
    case IPCWCode::ResolveStringToNonZeroUINT_ID:
    {
        auto name = (WCHAR*)data->lpData;
        for(int i = Parameters.size(); --i > -1;)
            if(wcscmp(std::get<2>(Parameters[i]), name) == 0)
                return i + 1;
        auto p = std::tuple<int, int, LPWSTR, LPWSTR>();
        auto bl         = data->cbData;
        std::get<2>(p) = new WCHAR[bl / sizeof(WCHAR)];
        memcpy_s((void*)std::get<2>(p), bl, name, bl);
        Parameters.push_back(p);
        return Parameters.size();
    }
    case ShaderControlCode::AddTooltipToNewParameter:
    {
        int i = Parameters.size();
        if(!i)
            break;
        auto p = Parameters[i - 1];
        auto bl        = data->cbData;
        std::get<3>(p) = new WCHAR[bl / sizeof(WCHAR)];
        memcpy_s((void*)std::get<3>(p), bl, data->lpData, bl);
        return i;
    }
    }
    return IPCWindow::HandleReceivedData(from, data);
}

/* #ifdef _DEBUG
LRESULT ControlMessageWindow::WindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    if(OnReceiveDiagnosticText && IsConfirmedPartner((HWND)wParam))
    {
        auto c_str = std::wstring(L"(internal) accepting message: ").append(std::to_wstring(msg)).append(L"/").append(std::to_wstring(lParam));
        OnReceiveDiagnosticText(c_str.c_str(), this);
    }
    return IPCWindow::WindowProc(hWnd, msg, wParam, lParam);
}
#endif*/
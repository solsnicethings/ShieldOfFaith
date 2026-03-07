#include "ShaderMessageWindow.h"

unsigned long long ShaderMessageWindow::NotifyBeginRedefine() {
    if(ReverifyPartnerCriteriaInStatedataPtr())
        return SendSignal(ShaderControlCode::BeginRedefine);
    return 0;
}

void ShaderMessageWindow::NotifyEndRedefine() {
    if(ReverifyPartnerCriteriaInStatedataPtr())
        PostSignal(ShaderControlCode::EndRedefine);
}
unsigned long long ShaderMessageWindow::SendParameter(int distinct_values, int relative_value, LPCWSTR name, LPCWSTR tooltip) {
    if(!ReverifyPartnerCriteriaInStatedataPtr())
        return 0;
    int id = RequestRemoteId(name);
    if(!id)
        return 0;
    if(distinct_values)
    {
        int data[3];
        sizeof(data);
        data[0] = id;
        data[1] = relative_value;
        data[2] = distinct_values;
        return (tooltip == nullptr || (*tooltip) == 0 || SendStringPtr(ShaderControlCode::AddTooltipToNewParameter, tooltip)) ?
        SendDataPtr(ShaderControlCode::ParameterData, &data, sizeof(data)) : FALSE;
    }
    else
    {
        int data[2];
        data[0] = id;
        data[1] = relative_value;
        return SendDataPtr(ShaderControlCode::ParameterData, &data, sizeof(data));
    }
}
void ShaderMessageWindow::SendUpdateFullscreen(bool fullscreen) {
    if(ReverifyPartnerCriteriaInStatedataPtr())
        PostSignal(fullscreen ? ShaderControlCode::FullscreenOn : ShaderControlCode::FullscreenOff);
}
void ShaderMessageWindow::SendUpdateMinimized(bool minimized)
{
    if(ReverifyPartnerCriteriaInStatedataPtr())
        PostSignal(minimized ? ShaderControlCode::Minimize : ShaderControlCode::Restore);
}
unsigned long long ShaderMessageWindow::NotifyProfileSaved(LPCWSTR profile_save_as)
{
    if(!ReverifyPartnerCriteriaInStatedataPtr())
        return 0;
    return SendStringPtr(ShaderControlCode::SaveProfile, profile_save_as);
}

unsigned long long ShaderMessageWindow::SendLoadedShaderPath(LPCWSTR shader_path)
{
    return SendStringPtr(ShaderControlCode::NewShaderPath, shader_path);
}
unsigned long long ShaderMessageWindow::SendLoadedProfilePath(LPCWSTR profile_path) {
    return SendStringPtr(ShaderControlCode::NewProfilePath, profile_path);
}

LRESULT ShaderMessageWindow::HandleReceivedSignal(HWND from, UINT signal) {

    if(!ReverifyPartnerCriteriaInStatedataPtr())
        return 0;

    switch (signal)
    {
    case ShaderControlCode::BeginRedefine:
        if(OnDefinitionRequest)
        {
            OnDefinitionRequest(this);
            return 1;
        }
        return -1;
    case ShaderControlCode::ResetShaderParams:
        if(OnResetParametersCommand)
        {
            OnResetParametersCommand(this);
            return 1;
        }
        return -1;
    case ShaderControlCode::ReloadProfile:
        if (OnReloadProfileCommand)
        {
            OnReloadProfileCommand(this);
            return 1;
        }
        return -1;
    case ShaderControlCode::FullscreenOff:
        if(OnSetFullscreenCommand)
        {
            OnSetFullscreenCommand(false, this);
            return 1;
        }
        return -1;
    case ShaderControlCode::FullscreenOn:
        if(OnSetFullscreenCommand)
        {
            OnSetFullscreenCommand(true, this);
            return 1;
        }
        return -1;
    case ShaderControlCode::ToggleMenu:
        if(OnToggleMenuCommand)
        {
            OnToggleMenuCommand(this);
            return 1;
        }
        return -1;
    case ShaderControlCode::Minimize:
        if(OnSetMinimizedCommand)
        {
            OnSetMinimizedCommand(true, this);
            return 1;
        }
        return -1;
    case ShaderControlCode::Restore:
        if(OnSetMinimizedCommand)
        {
            OnSetMinimizedCommand(false, this);
            return 1;
        }
        return -1;
    }

    return 0;
}
void ShaderMessageWindow::HandleQueuedData(HWND from, COPYDATASTRUCT* data)
{
    switch (data->dwData)
    {
    case ShaderControlCode::NewProfilePath:
        if(OnReceiveProfilePath)
            OnReceiveProfilePath((LPCWSTR)data->lpData, this);
        return;
    case ShaderControlCode::NewShaderPath:
        if(OnReceiveShaderPath)
            OnReceiveShaderPath((LPCWSTR)data->lpData, this);
        return;
    case ShaderControlCode::ParameterData:
        if(OnGetParameterRelativeValue)
        {
            auto p = (int*)data->lpData;
            OnGetParameterRelativeValue(p[1], p[0], this);
        }
        return;
    case ShaderControlCode::SaveProfile:
        if(OnSaveProfileCommand)
        {
            OnSaveProfileCommand((LPCWSTR)data->lpData, this);
        }
        return;
    }
}


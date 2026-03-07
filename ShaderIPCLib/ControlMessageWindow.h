#pragma once
#include "IPCWindow.h"
#include "ShaderControlCodes.h"
#include <Vector>

class ControlMessageWindow : public IPCWindow
{
public:
    unsigned long long SendProfilePath(LPCWSTR path);
    unsigned long long SendShaderPath(LPCWSTR path);

    /// <summary>
    /// Called when the shader is loading a profile or a shader
    /// </summary>
    void (*OnShaderBeginRedefine)(ControlMessageWindow* window);
    /// <summary>
    /// Called when the shader is no longer loading a profile or a shader
    /// </summary>
    void (*OnShaderEndRedefine)(ControlMessageWindow* window);

    unsigned long long RequestDefinition();

    /// <summary>
    /// Between BeginRedefine and EndRedefine the shader will send a list of distinct parameters with their current values.
    /// When parameters are changed, either by the control app, or other means, the new value will be reported to the control app.
    /// When the call is a value update rather than an initial listing, distinct_values and tooltip are 0.
    /// </summary>
    void (*OnReceiveParameter)(int distinct_values, int relative_value, LPCWSTR name, LPCWSTR tooltip, ControlMessageWindow* window);

    void (*OnReceiveDiagnosticText)(LPCWSTR text, ControlMessageWindow* window);

    unsigned long long SetParameter(int relative_value, LPCWSTR name);

    unsigned long long ResetParameters();
    unsigned long long ReloadProfile();

    unsigned long long SetFullscreen(bool fullscreen);
    void (*OnUpdateFullscreen)(bool fullscreen, ControlMessageWindow* window);
    unsigned long long ToggleMenu();
    unsigned long long SetMinimized(bool minimized);
    void (*OnUpdateMinimized)(bool minimized, ControlMessageWindow* window);

    unsigned long long SaveProfile(LPCWSTR profile_save_as);
    void (*OnProfileSaved)(LPCWSTR profile_save_as, ControlMessageWindow* window);
    void (*OnProfileLoaded)(LPCWSTR profile_load_from, ControlMessageWindow* window);
    void (*OnShaderLoaded)(LPCWSTR shader_id, ControlMessageWindow* window);

    ~ControlMessageWindow();

    protected:
    LRESULT HandleReceivedSignal(HWND from, UINT signal) override;
    LRESULT HandleReceivedData(HWND from, COPYDATASTRUCT* data) override;
    void HandleQueuedData(HWND from, COPYDATASTRUCT* data) override;
    /*
    #ifdef _DEBUG
    LRESULT WindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) override;
    #endif
    */
    private:
    std::vector<std::tuple<int, int, LPWSTR, LPWSTR>> Parameters = std::vector<std::tuple<int, int, LPWSTR, LPWSTR>>();
};

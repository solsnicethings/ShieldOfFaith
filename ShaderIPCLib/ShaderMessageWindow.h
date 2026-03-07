#pragma once
#include "IPCWindow.h"
#include "ShaderControlCodes.h"

class ShaderMessageWindow : public IPCWindow
{
public:
    void (*OnReceiveProfilePath)(LPCWSTR path, ShaderMessageWindow * window);
    void (*OnReceiveShaderPath)(LPCWSTR path, ShaderMessageWindow * window);

    /// <summary>
    /// Call when loading a profile or a shader
    /// </summary>
    unsigned long long NotifyBeginRedefine();
    /// <summary>
    /// Called when no longer loading a profile or a shader (once for each begin, even if overlapping)
    /// </summary>
    void NotifyEndRedefine();

    /// <summary>
    /// Between BeginRedefine and EndRedefine the shader must send a list of distinct parameters with their current values.
    /// When parameters are changed, either by the control app, or other means, the new value must also be reported to the control app.
    /// When the call is a value update rather than an initial listing, distinct_values should be 0.
    /// </summary>
    unsigned long long SendParameter(int distinct_values, int relative_value, LPCWSTR name, LPCWSTR tooltip);
    void (*OnGetParameterRelativeValue)(int relative_value, unsigned int id, ShaderMessageWindow* window);
    void (*OnResetParametersCommand)(ShaderMessageWindow* window);
    void (*OnReloadProfileCommand)(ShaderMessageWindow* window);
    void (*OnDefinitionRequest)(ShaderMessageWindow* window);

    void (*OnToggleMenuCommand)(ShaderMessageWindow* window);
    void (*OnSetFullscreenCommand)(bool fullscreen, ShaderMessageWindow* window);
    void (*OnSetMinimizedCommand)(bool minimized, ShaderMessageWindow* window);
    void SendUpdateFullscreen(bool fullscreen);
    void SendUpdateMinimized(bool minimized);

    unsigned long long SendLoadedShaderPath(LPCWSTR shader_path);
    unsigned long long SendLoadedProfilePath(LPCWSTR profile_path);

    void (*OnSaveProfileCommand)(LPCWSTR profile_save_as, ShaderMessageWindow* window);
    unsigned long long NotifyProfileSaved(LPCWSTR profile_save_as);

protected:
    LRESULT HandleReceivedSignal(HWND from, UINT signal) override;
    void HandleQueuedData(HWND from, COPYDATASTRUCT* data) override;
};

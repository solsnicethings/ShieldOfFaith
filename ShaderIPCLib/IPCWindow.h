#pragma once
#include "framework.h"

// https://learn.microsoft.com/en-us/windows/win32/winmsg/window-features#message-only-windows

/// <summary>
/// Remains as statedata after handhsake.
/// Contains actual partner PID, PartnerWindowName, PartnerWindowClass
/// </summary>
typedef std::tuple<int, LPCWSTR, LPCWSTR> PartnerCriteria;

/// <summary>
/// These codes serve as message types (WM_USER message or type info for WM_DATACOPY),
/// as Window States, and as timer identifiers.
/// </summary>
enum IPCWCode
{
    None = 0,

    Unspecified = WM_USER,
    NewlyCreated,
    NewData,
    PresentlyUnavailable ,
    PermanentlyUnavailable,
    NoMatch,
    IPCError,
    PartnerShuttingDown,

    Timer_RepeatUntilComplete,
    Timer_RunOnceWhenEverySubmissionIsReady,

    CallShowWindowForMe,

    /// <summary>
    /// it is natural to send and expect strings for this code, but specific implementations could agree on a different practice
    /// many diagnostics could of course instead be implemented using specific codes starting from ExtendedCodeStart and going up
    /// there is no specific code in place to handle the diagnostic message in the base class
    /// </summary>
    DiagnosticMessage,

    ResolveStringToNonZeroUINT_ID,

    AnticipatingHandshake,
    AnticipatingAnotherHandshake,
    InitiatingHandshake,
    PartnerMayHaveFailed,
    CompletingHandshake,
    CompletingHandshakeResponse,
    HandshakeComplete,

    ExtendedCodeStart
};

class IPCWindow
{
public:
    bool IsWindowDestroyed();
    void ForceFailState();
    
    /// Note regarding various lifecycle events internally executed running TriggerLifecycleEvents():
    ///     a single lifecycle trigger executes each pointer only once, even if it is submitted more than once.
    void RunAfterCreated(void (*handler)(IPCWindow* createdWindow), bool runIfAlreadyCreated = true);

    void RequestShowWindowCall(HWND targetInThisProcess, int cmdShow);
    
    /// <summary>
    /// In default implementation this can only be called in context of a completed handshake.
    /// </summary>
    unsigned int (*ResolveNameToNonZeroId)(LPCWSTR name, IPCWindow* window);
    LRESULT (*OnPartnerShuttingDown)(IPCWindow* window);

    void (*OnSendFailed)(UINT_PTR signal_or_data_type, IPCWindow* window);

    /// <summary>
    /// These are called once the handshake succeeds or ultimately fails.
    /// Note regarding various lifecycle events internally executed running TriggerLifecycleEvents():
    ///     a single lifecycle trigger executes each pointer only once, even if it is submitted more than once.
    /// </summary>
    /// <param name="handler">the function to call, check for shakenWindow->ExtendedGetState() == IPCWCode::Error to react to failures.</param>
    /// <param name="runIfAlreadyShaken"></param>
    void RunAfterHandshake(void (*handler)(IPCWindow* shakenWindow), bool runIfAlreadyShaken = true);

    /// <summary>
    /// With a partner that has completed a handshake, RunOnce can be called.
    /// The default implementation uses the lifecycle event collection and the Timer_RunOnceWhenEverySubmissionIsReady ID
    /// Any RunOnce calls pending are run together. If the timer was already pending, it gets reset to the latest submitted time,
    /// be that sooner or later than before. Deriving classes may do this differently.
    /// 
    /// Note regarding various lifecycle events internally executed running TriggerLifecycleEvents():
    ///     a single lifecycle trigger executes each pointer only once, even if it is submitted more than once.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="wait_ms"></param>
    virtual void RunOnce(void (*handler)(IPCWindow* shakenWindow), unsigned int wait_ms = 300);

    /// <summary>
    /// Once you have prepared the window, create it using this function.
    /// On the first invocation, a class is registered to make the window connect with this class and classes that extend this.
    /// </summary>
    /// <param name="hInstance">pass the value from wWinMain / entrypoint</param>
    /// <param name="name">this will not be shown, but can be distinguished for finding the right window - however, in simple scenarios, the class name is sufficiently distinctive.</param>
    /// <returns>HWND if successful</returns>
    HWND Create(HINSTANCE hInstance, LPCWSTR name = nullptr);

    /// <summary>
    /// Specifies which process to communicate with.
    /// If the value is unspecified or 0, parent pid will be used.
    /// If the value is negative, any pid is accepted.
    /// </summary>
    void SetPartnerDetails(int partner_pid = 0, LPCWSTR partner_name = nullptr, LPCWSTR partner_class = nullptr);

    /// <summary>
    /// Send and receive messages with the partner window, having both ends verify that the windows belong to correct processes.
    /// Future communications will be between the two handshake windows only. Use RunAfterHandshake to execute commands when the last message
    /// of the handshake is processed, on the same thread. One party should call AnticipateHandshake(), the other InitiateHandshake().
    /// </summary>
    void AnticipateHandshake();
    /// <summary>
    /// Counterpart to AnticipateHandshake(). Details on handshake added to that function.
    /// 
    /// This function return true if it found a handshake partner that joined the handshake process.
    /// If it returns false, you need to call it again in a little while in case the partner process/window wasn't ready.
    /// The state of the window after this call will be one of
    ///     CompletingHandshake, NoMatch, PartnerMayHaveFailed, PresentlyUnavailable or PermanentlyUnavailable
    ///     CompletingHandshake will be fulfilled by initiative from the partner window.
    ///     NoMatch indicates that the name/class/process constraints found no match; no such window even exists.
    ///     - maybe never opened, maybe crashed, maybe still opening
    ///     PresentlyUnavailable indicates a window that may well be the intended partner is currently too early or late in handshake process.
    ///     If it completes a different handshake, that window will subsequently report PermanetlyUnavailable instead.
    ///     - PermanentlyUnavailable indicates a window already partnered with another window.
    ///     - PartnerMayHaveFailed indicates a window that may be the one intended to partner with this window, but it isn't going to.
    /// 
    /// The most informative state is set, out of all the possibilities encountered:
    ///     1. CompletingHandshake (indicating you're nearly there)
    ///     2. PresentlyUnavailable (indicating the partner window is moments away from readiness)
    ///     3. PartnerMayHaveFailed (indicating you may need to assume the worst)
    ///     4. PermanentlyUnavailable / NoMatch (never true at the same time)
    /// 
    /// These states can help you decide how frequently and often to retry initiation.
    /// </summary>
    bool InitiateHandshake();

    /// <summary>
    /// You can attach any information you need here.
    /// </summary>
    void* CustomRef;

    void BeginShakingHandsUntilSuccessful(UINT interval_ms = 500);

    unsigned int IPCTimeoutMS { 
        #ifdef _DEBUG
        30000
        #else
        900 
        #endif
    };

    ~IPCWindow();

    static ATOM CustomRegisterClass(HINSTANCE hInstance, LPCWSTR class_name);

    unsigned int ExtendedGetState();

    HWND GetHWND();

    template <typename Type>
    unsigned long long SendData(UINT_PTR msgType, Type message);
    unsigned long long SendDataPtr(UINT_PTR msgType, void * data, size_t size_override);
    unsigned long long SendDataPtr(UINT_PTR msgType, void* data, size_t size_override, HWND partner);
    unsigned long long SendSignal(UINT signal);
    void PostSignal(UINT signal);
    template <typename Type>
    unsigned long long SendData(UINT_PTR msgType, Type message, HWND partner);
    unsigned long long SendSignal(UINT signal, HWND partner);

    unsigned long long SendStringPtr(UINT_PTR msgType, LPCWSTR message);
    unsigned long long SendStringPtr(UINT_PTR msgType, LPCWSTR message, HWND partner);

protected:
    virtual LRESULT WindowProc(HWND hwndMain, UINT uMsg, WPARAM wParam, LPARAM lParam);
    void*           GetStatedataPtr();
    template <typename Type>
    bool SetStatedataFromMemBlock(Type* data);

    template <typename Type>
    Type GetStatedata();


    unsigned int RequestRemoteId(LPCWSTR name);

    BOOL QueueReceivedData            (HWND from, COPYDATASTRUCT* data);
    virtual void                  HandleQueuedData(HWND from, COPYDATASTRUCT* data);

    void         ExtendedSetState(unsigned int state);

    virtual LRESULT HandleReceivedData(HWND from, COPYDATASTRUCT* data);
    virtual LRESULT HandleReceivedSignal(HWND from, UINT signal);
    bool         IsConfirmedPartner(HWND wnd);

    /// <summary>
    /// Events will first be executed and cleared on CREATE
    /// Then on completed handshake
    /// After that, more specialised classes can make use of these functions to have a simple event system
    /// </summary>
    /// <param name="eventHandler"></param>
    void RegisterLifecycleEvent(void (*eventHandler)(IPCWindow*));
    void TriggerLifecycleEvents(bool retainEvents = false);
    void ClearLifecycleEvents();

    bool ReverifyPartnerCriteria(PartnerCriteria& criteria);
    bool ReverifyPartnerCriteriaInStatedataPtr();

    /// <summary>
    /// Removes state data / partner information and lifecycle events and sets state to newly created
    /// Does nothing to a window that is None or Newly created
    /// If handshake is completed or completing: requires force, notifies partner of error
    /// If handshake is completed: notifies partner as if window is being destroyed.
    /// Returns true if carried out and the final state is that a window exists.
    /// If the result is false, state could be:
    /// None (0) indicating that the window was closed or never created
    /// Unless force is true, state could also be:
    /// CompletingHandshake/CompletingHandshakeResponse/HandshakeComplete
    /// and extended states.
    /// If it returns true, the prior window/HWND is still in use, and you may need to invoke logic as if
    /// calling RunAfterCreated(). Actually calling RunAfterCreated will work, as default behaviour is to run immediately
    /// if the condition has already occured.
    /// </summary>
    virtual bool ResetToNewlyCreatedState(bool force = false);

private:

    HWND self = 0, partner = 0;
    IPCWCode             state = None;
    char                    statedata_type = '\0';
    static LRESULT CALLBACK IPCWindowProc(HWND hwndMain, UINT uMsg, WPARAM wParam, LPARAM lParam);

    PartnerCriteria determine_partner_criteria();
    void* statedata = nullptr;
    static bool     CheckWindowPid(HWND window, int pid);
    static bool     CheckWindowStrings(HWND window, LPCWSTR className, LPCWSTR windowName);

    std::tuple<HWND, COPYDATASTRUCT*, void**>* InboundMessageQueue = nullptr;
    std::pair<void (*)(IPCWindow*), void*>*    LifecycleEvent       = nullptr;
    void                                       SetHandshakeCompletedState();
};
